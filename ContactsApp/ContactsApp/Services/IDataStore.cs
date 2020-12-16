using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddOrUpdateAsync(T item);
        Task<bool> AddOrUpdateAsync(IEnumerable<T> items);

        Task<bool> RemoveAsync(Guid id);
        Task<bool> RemoveAllAsync();
        Task<T> GetItemAsync(Guid id);
        Task<List<T>> GetItemsAsync();
    }
}
