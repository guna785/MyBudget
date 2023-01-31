using LazyCache;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Application.Interfaces.Repositories;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Domain.Contract;
using System.Collections;

namespace MyBudget.Infrastructure.Repositories
{
    public class UnitOfWork<TId> : IUnitOfWork<TId>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _dbContext;
        private bool disposed;
        private Hashtable _repositories;
        private readonly IAppCache _cache;

        public UnitOfWork(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IAppCache cache)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _currentUserService = currentUserService;
            _cache = cache;
        }

        public IRepositoryAsync<TEntity, TId> Repository<TEntity>() where TEntity : AuditableEntity<TId>
        {
            _repositories ??= new Hashtable();
            string type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                Type repositoryType = typeof(RepositoryAsync<,>);

                object? repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryAsync<TEntity, TId>)_repositories[type];
        }

        public async Task<int> Commit(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            int result = await _dbContext.SaveChangesAsync(cancellationToken);
            foreach (string cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
            return result;
        }

        public Task Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}
