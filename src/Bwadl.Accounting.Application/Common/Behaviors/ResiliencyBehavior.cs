using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Bwadl.Accounting.Shared.Configuration;

namespace Bwadl.Accounting.Application.Common.Behaviors;

public class ResiliencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ResiliencyBehavior<TRequest, TResponse>> _logger;
    private readonly ResiliencyOptions _settings;

    public ResiliencyBehavior(
        ILogger<ResiliencyBehavior<TRequest, TResponse>> logger,
        IOptions<ResiliencyOptions> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        // Skip retry for non-retriable operations
        if (_settings.NonRetriableOperations.Contains(requestName))
        {
            _logger.LogDebug("Skipping retry for non-retriable operation: {RequestName}", requestName);
            return await next();
        }

        // Skip if retry is disabled
        if (!_settings.EnableRetry)
        {
            _logger.LogDebug("Retry is disabled globally");
            return await next();
        }

        var retryPolicy = Policy
            .Handle<Exception>(ex => IsRetriableException(ex))
            .WaitAndRetryAsync(
                retryCount: _settings.RetryCount,
                sleepDurationProvider: retryAttempt => 
                {
                    var delay = TimeSpan.FromMilliseconds(
                        Math.Min(_settings.BaseDelayMs * Math.Pow(2, retryAttempt - 1), _settings.MaxDelayMs));
                    return delay;
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount}/{MaxRetries} for {RequestName} after {Delay}ms due to: {Exception}",
                        retryCount, _settings.RetryCount, requestName, timespan.TotalMilliseconds, outcome.Message);
                });

        return await retryPolicy.ExecuteAsync(async () =>
        {
            return await next();
        });
    }

    private bool IsRetriableException(Exception exception)
    {
        var exceptionType = exception.GetType().FullName;
        return _settings.RetriableExceptions.Any(retriable => 
            exceptionType?.Contains(retriable) == true || 
            exception.GetType().IsAssignableFrom(Type.GetType(retriable) ?? typeof(object)));
    }
}
