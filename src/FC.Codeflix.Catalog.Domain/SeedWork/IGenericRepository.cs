namespace FC.Codeflix.Catalog.Domain.SeedWork;
public interface IGenericRepository<TAggregate> : IRepository
{
    Task Insert(TAggregate aggregate, CancellationToken cancellationToken);
}
