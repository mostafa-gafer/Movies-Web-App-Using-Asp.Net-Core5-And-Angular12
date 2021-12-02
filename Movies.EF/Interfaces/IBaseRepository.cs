using Movies.CORE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllAsQueryable();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        void Delete(T entity);
        T UpdateAsync(T entity);
    }
}
