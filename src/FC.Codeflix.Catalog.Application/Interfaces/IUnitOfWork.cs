namespace FC.Codeflix.Catalog.Application.Interfaces;
public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
