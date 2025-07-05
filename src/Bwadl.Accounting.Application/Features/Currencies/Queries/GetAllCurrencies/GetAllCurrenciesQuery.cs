using Bwadl.Accounting.Application.Common.DTOs;
using MediatR;

namespace Bwadl.Accounting.Application.Features.Currencies.Queries.GetAllCurrencies;

public record GetAllCurrenciesQuery() : IRequest<List<CurrencyDto>>;
