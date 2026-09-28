using Fundation.Abstractions.Domain;

namespace Fundation.Abstractions.Persistence.EfCore;

public interface IPageRepository<TEntity, TKey>
    where TEntity : IHaveIdentity<TKey>
{
}

public interface IPageRepository<TEntity> : IPageRepository<TEntity, Guid>
    where TEntity : IHaveIdentity<Guid>
{
}
