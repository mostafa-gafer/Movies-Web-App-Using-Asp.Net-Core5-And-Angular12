using Microsoft.EntityFrameworkCore;
using Movies.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Movies.CORE.Context;
using Movies.CORE.ViewModels;
using Helper.Helpers;

namespace Movies.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        private readonly IHttpContextAccessor _CA;
        public BaseRepository(ApplicationDbContext context , IHttpContextAccessor CA)
        {
            _context = context;
            _CA = CA;
        }

        public BaseRepository(ApplicationDbContext context)
        {
        }

        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id); //get by id function
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();//get all function
        public IQueryable<T> GetAllAsQueryable()=> _context.Set<T>().AsQueryable();//to get all AsQueryable
        public async Task<T> AddAsync(T entity) {       //get all function
            await _context.Set<T>().AddAsync(entity);
            return entity; 
        }
        public T UpdateAsync(T entity)
        {   
             _context.Set<T>().Update(entity);
            return entity;
        }
        public void Delete(T entity) => _context.Set<T>().Remove(entity);


    }
}
