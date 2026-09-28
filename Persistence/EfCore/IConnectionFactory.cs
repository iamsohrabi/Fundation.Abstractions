using System.Data;

namespace Fundation.Abstractions.Persistence.EfCore;

public interface IConnectionFactory : IDisposable
{
    IDbConnection GetOrCreateConnection();
}
