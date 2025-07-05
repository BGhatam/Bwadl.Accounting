# Resiliency Configuration Guide

## Overview

The ResiliencyBehavior provides automatic retry functionality for MediatR commands and queries. It's now fully configurable and selective about which operations to retry.

## How It Works

### When ResiliencyBehavior Gets Invoked ✅

The ResiliencyBehavior runs for **ALL** MediatR requests by default, but applies retry logic selectively based on:

1. **Operation Type**: Only retriable operations get retried
2. **Exception Type**: Only specific exceptions trigger retries
3. **Global Setting**: Can be disabled completely

### Commands That Get Retried:
- `GetAllUsersQuery` ✅
- `GetUserQuery` ✅
- `GetAllCurrenciesQuery` ✅
- `GetCurrencyQuery` ✅
- `GetCurrencyVersionsQuery` ✅
- `CreateUserCommand` ✅
- `UpdateUserCommand` ✅
- `CreateCurrencyCommand` ✅
- `UpdateCurrencyCommand` ✅

### Commands That DON'T Get Retried:
- `DeleteUserCommand` ❌ (Configured as non-retriable)
- `DeleteCurrencyCommand` ❌ (Configured as non-retriable)
- `ChangePasswordCommand` ❌ (Configured as non-retriable)
- `LoginCommand` ❌ (Configured as non-retriable)
- `RegisterCommand` ❌ (Configured as non-retriable)

## Configuration

### appsettings.json Configuration

```json
{
  "Resiliency": {
    "RetryCount": 3,
    "BaseDelayMs": 1000,
    "MaxDelayMs": 30000,
    "EnableRetry": true,
    "RetriableExceptions": [
      "System.Data.SqlClient.SqlException",
      "System.TimeoutException",
      "System.Net.Http.HttpRequestException",
      "Microsoft.EntityFrameworkCore.DbUpdateException",
      "Npgsql.NpgsqlException",
      "System.InvalidOperationException"
    ],
    "NonRetriableOperations": [
      "DeleteUserCommand",
      "DeleteCurrencyCommand",
      "ChangePasswordCommand",
      "LoginCommand",
      "RegisterCommand"
    ]
  }
}
```

### Configuration Properties

| Property | Default | Description |
|----------|---------|-------------|
| `RetryCount` | 3 | Maximum number of retry attempts |
| `BaseDelayMs` | 1000 | Base delay in milliseconds for first retry |
| `MaxDelayMs` | 30000 | Maximum delay between retries |
| `EnableRetry` | true | Global toggle to enable/disable retry |
| `RetriableExceptions` | Array | Exception types that trigger retries |
| `NonRetriableOperations` | Array | Command/Query types that should not be retried |

### Exponential Backoff

The retry behavior uses exponential backoff with the formula:
```
Delay = min(BaseDelayMs * 2^(attempt-1), MaxDelayMs)
```

**Example delays with default settings:**
- Attempt 1: 1000ms (1 second)
- Attempt 2: 2000ms (2 seconds)  
- Attempt 3: 4000ms (4 seconds)

## Exception Handling

### Retriable Exceptions ✅
These exceptions will trigger retries:
- **Database Errors**: `Npgsql.NpgsqlException`, `DbUpdateException`
- **Network Errors**: `HttpRequestException`, `TimeoutException`
- **SQL Errors**: `SqlException`
- **Transient Errors**: `InvalidOperationException`

### Non-Retriable Exceptions ❌
These exceptions will NOT trigger retries:
- **Validation Errors**: `ValidationException`
- **Business Logic Errors**: `DomainException`
- **Authentication Errors**: `UnauthorizedException`
- **Not Found Errors**: `NotFoundException`

## Environment-Specific Configuration

### Development
```json
{
  "Resiliency": {
    "RetryCount": 2,
    "BaseDelayMs": 500,
    "EnableRetry": true
  }
}
```

### Production
```json
{
  "Resiliency": {
    "RetryCount": 5,
    "BaseDelayMs": 2000,
    "MaxDelayMs": 60000,
    "EnableRetry": true
  }
}
```

### Testing (Disable Retries)
```json
{
  "Resiliency": {
    "EnableRetry": false
  }
}
```

## Logging

The ResiliencyBehavior provides detailed logging:

### Retry Attempts
```
WARN: Retry 2/3 for GetUserQuery after 2000ms due to: Connection timeout
```

### Skipped Operations
```
DEBUG: Skipping retry for non-retriable operation: DeleteUserCommand
```

### Disabled Retry
```
DEBUG: Retry is disabled globally
```

## Best Practices

### ✅ DO
- Configure different retry counts for different environments
- Monitor retry logs to identify systemic issues
- Use selective retry for non-idempotent operations
- Set reasonable max delays to avoid blocking

### ❌ DON'T
- Retry authentication or authorization failures
- Retry delete operations (they should be idempotent but cautious)
- Use excessive retry counts (affects user experience)
- Retry validation errors (they won't succeed on retry)

## Performance Impact

### Resource Usage
- **Memory**: Minimal (configuration cached)
- **CPU**: Low (simple exception checking)
- **Latency**: Adds delay only on failures

### Monitoring
Monitor these metrics:
- Retry success rate
- Average retry delays
- Most retried operations
- Exception patterns

## Customization

### Adding New Non-Retriable Operations
```json
{
  "Resiliency": {
    "NonRetriableOperations": [
      "DeleteUserCommand",
      "DeleteCurrencyCommand",
      "SendEmailCommand",
      "ProcessPaymentCommand"
    ]
  }
}
```

### Adding New Retriable Exceptions
```json
{
  "Resiliency": {
    "RetriableExceptions": [
      "System.Net.Sockets.SocketException",
      "System.Threading.Tasks.TaskCanceledException",
      "YourCustom.TransientException"
    ]
  }
}
```

## Troubleshooting

### Issue: Retries Not Working
- Check `EnableRetry` is true
- Verify exception type is in `RetriableExceptions`
- Ensure operation is not in `NonRetriableOperations`

### Issue: Too Many Retries
- Reduce `RetryCount`
- Add problematic operations to `NonRetriableOperations`
- Check for systemic issues causing failures

### Issue: Retries Too Slow
- Reduce `BaseDelayMs`
- Lower `MaxDelayMs`
- Consider reducing `RetryCount`

The ResiliencyBehavior now provides intelligent, configurable retry functionality that improves application reliability while maintaining performance! 🔄
