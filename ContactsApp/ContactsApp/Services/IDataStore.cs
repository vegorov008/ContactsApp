using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddOrUpdateItemAsync(T item);
        Task<bool> RemoveItemAsync(Guid id);
        Task<bool> RemoveItemsAsync();
        Task<T> GetItemAsync(Guid id);
        Task<List<T>> GetItemsAsync();
    }
}
