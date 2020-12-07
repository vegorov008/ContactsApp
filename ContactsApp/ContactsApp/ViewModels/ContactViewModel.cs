using System;
using System.Collections.Generic;
using System.Text;
using ContactsApp.Models;
using ContactsApp.ViewModels.Abstractions;

namespace ContactsApp.ViewModels
{
    public class ContactViewModel : BaseViewModel<Contact>
    {
        public string DisplayName => $"{Model?.FirstName ?? string.Empty} {Model?.LastName ?? string.Empty}";
        public string Phone => Model?.Phone ?? string.Empty;

        public ContactViewModel()
        {

        }

        public ContactViewModel(Contact model)
        {
            Model = model;
        }
    }
}
