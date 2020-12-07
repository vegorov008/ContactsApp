using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts;
using ContactsApp.Models;
using ContactsApp.Services;
using Foundation;

namespace ContactsApp.iOS.Services
{
    public class ContactsService : IContactsService
    {
        static readonly NSString[] _keys = new NSString[] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers };

        public async Task<List<Contact>> GetContactsAsync()
        {
            var result = new List<Contact>();

            var iosContacts = GetIosContacts();
            if (iosContacts != null)
            {
                foreach (var item in iosContacts)
                {
                    try
                    {
                        result.Add(new Contact
                        {
                            FirstName = item.GivenName,
                            LastName = item.FamilyName,
                            Phone = item.PhoneNumbers?.FirstOrDefault(x => x?.Value?.StringValue.IsNullOrEmpty() == false).Value.StringValue
                        });
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }
                }
            }

            return result;
        }

        List<CNContact> GetIosContacts()
        {
            var result = new List<CNContact>();

            using (var store = new CNContactStore())
            {
                NSError error;
                var allContainers = store.GetContainers(null, out error);
                foreach (var container in allContainers)
                {
                    try
                    {
                        using (var predicate = CNContact.GetPredicateForContactsInContainer(container.Identifier))
                        {
                            var containerResults = store.GetUnifiedContacts(predicate, _keys, out error);
                            result.AddRange(containerResults);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }
                }
            }

            return result;
        }
    }
}