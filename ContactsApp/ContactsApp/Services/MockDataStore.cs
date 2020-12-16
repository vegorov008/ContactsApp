using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsApp.Models;

namespace ContactsApp.Services
{
    public class MockDataStore : IDataStore<Contact>
    {
        readonly List<Contact> _items;

        public MockDataStore()
        {
            _items = new List<Contact>()
            {
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 1", LastName="LastName 1", Phone="Phone 1" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 2", LastName="LastName 2", Phone="Phone 2" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 3", LastName="LastName 3", Phone="Phone 3" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 4", LastName="LastName 4", Phone="Phone 4" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 5", LastName="LastName 5", Phone="Phone 5" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 6", LastName="LastName 6", Phone="Phone 6" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 7", LastName="LastName 7", Phone="Phone 7" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 8", LastName="LastName 8", Phone="Phone 8" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 9", LastName="LastName 9", Phone="Phone 9" },
                new Contact { Id = Guid.NewGuid(), FirstName = "FirstName 10", LastName="LastName 10", Phone="Phone 10" }
            };
        }

        public async Task<bool> AddOrUpdateAsync(Contact item)
        {
            _items.RemoveAll(x => x.Equals(item));
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> AddOrUpdateAsync(IEnumerable<Contact> items)
        {
            foreach (var item in items)
            {
                _items.RemoveAll(x => x.Equals(item));
            }
            _items.AddRange(items);

            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            _items.RemoveAll(x => x.Id == id);

            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAllAsync()
        {
            _items.Clear();

            return await Task.FromResult(true);
        }

        public async Task<Contact> GetItemAsync(Guid id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<List<Contact>> GetItemsAsync()
        {
            return await Task.FromResult(_items);
        }
    }
}