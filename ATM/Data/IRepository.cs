using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATM.Data
{
    public interface IRepository<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task EditAsync(TEntity entity);
        bool Exists(int id);
    }
}
