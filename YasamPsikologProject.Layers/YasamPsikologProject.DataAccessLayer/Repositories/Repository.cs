using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            // Soft delete kontrolü
            if (entity != null && entity.IsDeleted)
                return null;
            return entity;
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // Soft delete filtresi - silinmiş kayıtları getirme
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public T Update(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            // Soft delete
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            // Soft delete
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            return filter == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(filter);
        }
    }
}
