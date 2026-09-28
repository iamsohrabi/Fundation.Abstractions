using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Fundation.Abstractions.Persistence;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
