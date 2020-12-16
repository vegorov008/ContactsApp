using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContactsApp.Models;

namespace ContactsApp.Services
{
    public class SqLiteDataStore<TModel> : IDataStore<TModel> where TModel : class, IHasIdentity, new()
    {
        readonly SqLiteDataProvider _dataProvider;

        public SqLiteDataStore(SqLiteDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> AddOrUpdateAsync(TModel item)
        {
            await _dataProvider.AddOrUpdate(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> AddOrUpdateAsync(IEnumerable<TModel> items)
        {
            await _dataProvider.AddOrUpdate(items);
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            await _dataProvider.Remove<TModel>(id);
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAllAsync()
        {
            await _dataProvider.RemoveAll<TModel>();
            return await Task.FromResult(true);
        }

        public async Task<TModel> GetItemAsync(Guid id)
        {
            return await _dataProvider.Select<TModel>(id);
        }

        public async Task<List<TModel>> GetItemsAsync()
        {
            return await _dataProvider.SelectAll<TModel>();
        }
    }
}
