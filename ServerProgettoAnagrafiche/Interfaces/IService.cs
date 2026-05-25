using ProgettoAnagrafiche.Models;
using BlazorServerProgettoAnagrafiche.Services.ResponseWrapper;

namespace BlazorServerProgettoAnagrafiche.Interfaces
    {
    public interface IService<T> where T : BaseEntity
        {
        // success auto-inferred by return whether there's any error
        // check for errors, validate, return list of strings (empty or not)
        public Task<List<string>> CheckUpdateAsync(T entity, int? excludeId = null);
        // get by id and return data or error list
        public Task<Return<T>> GetByIdAsync(int pkValue);  
        // get all and return data list or error lsit
        public Task<Return<List<T>>> GetAllAsync();

        // create, update -> return entity
        public Task<Return<T>> CreateAsync(T entity);
        public Task<Return<T>> UpdateAsync(T entity);

        // don't return anything, just wrapper
        public Task<Result> DeleteAsync(int pkValue);        
        }
    }




    