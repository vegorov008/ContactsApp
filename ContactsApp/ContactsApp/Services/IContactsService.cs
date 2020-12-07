using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContactsApp.Models;

namespace ContactsApp.Services
{
    public interface IContactsService
    {
        Task<List<Contact>> GetContactsAsync();
    }
}
