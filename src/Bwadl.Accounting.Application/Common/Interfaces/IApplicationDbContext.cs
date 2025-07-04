using Microsoft.EntityFrameworkCore;

namespace Bwadl.Accounting.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
