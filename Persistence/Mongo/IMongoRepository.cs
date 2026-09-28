using Fundation.Abstractions.Domain;

namespace Fundation.Abstractions.Persistence.Mongo;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IHaveIdentity<TId>
{
}
