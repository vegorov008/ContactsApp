using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Database;
using Android.Provider;
using ContactsApp.Models;
using ContactsApp.Services;

namespace ContactsApp.Droid.Services
{
    public class ContactsService : IContactsService
    {
        static readonly string[] _projection =
        {
            ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId,
            ContactsContract.CommonDataKinds.StructuredName.GivenName,
            ContactsContract.CommonDataKinds.StructuredName.FamilyName,
            ContactsContract.CommonDataKinds.Phone.NormalizedNumber
        };

        readonly BasePermissionsActivity _activity;

        public ContactsService(BasePermissionsActivity context)
        {
            _activity = context;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            List<Contact> result = null;

            bool granted = await _activity.TryGrantPermissions(Android.Manifest.Permission.ReadContacts);
            if (granted)
            {
                var androidContacts = await _activity.RunOnUiTrhreadAsync<List<AndroidContact>>(GetAndroidContacts);
                if (androidContacts != null)
                {
                    result = new List<Contact>();
                    for (int i = 0; i < androidContacts.Count; i++)
                    {
                        try
                        {
                            var contact = androidContacts[i];
                            result.Add(new Contact()
                            {
                                Id = Guid.NewGuid(),
                                FirstName = contact.FirstName,
                                LastName = contact.LastName,
                                Phone = contact.NormalizedNumber
                            });
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.HandleException(ex);
                        }
                    }
                }
            }

            return result;
        }

        List<AndroidContact> GetAndroidContacts()
        {
            var result = new HashSet<AndroidContact>();

            using (var loader = new CursorLoader(_activity, ContactsContract.Data.ContentUri, _projection, null, null, null))
            {
                using (var cursor = (ICursor)loader.LoadInBackground())
                {
                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            try
                            {
                                AndroidContact contact = new AndroidContact
                                {
                                    ContactId = cursor.GetLong(cursor.GetColumnIndex(_projection[0])),
                                    FirstName = cursor.GetString(cursor.GetColumnIndex(_projection[1])),
                                    LastName = cursor.GetString(cursor.GetColumnIndex(_projection[2])),
                                    NormalizedNumber = cursor.GetString(cursor.GetColumnIndex(_projection[3]))
                                };

                                var existed = result.FirstOrDefault(x => x.Equals(contact));
                                if (existed != null)
                                {
                                    existed.Merge(contact);
                                }
                                else
                                {
                                    result.Add(contact);
                                }
                            }
                            catch (Exception ex)
                            {
                                ExceptionHandler.HandleException(ex);
                            }
                        } while (cursor.MoveToNext());
                    }
                }
            }

            return result.ToList();
        }

        class AndroidContact : IEquatable<AndroidContact>
        {
            public long ContactId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NormalizedNumber { get; set; }

            public override int GetHashCode()
            {
                return ContactId.GetHashCode();
            }

            public bool Equals(AndroidContact other)
            {
                return ReferenceEquals(this, other) ||
                    (other != null &&
                    ContactId == other.ContactId);
            }

            public void Merge(AndroidContact other)
            {
                if (this.Equals(other))
                {
                    if ((FirstName.IsNullOrEmpty() || FirstName.IsNumber()) && !other.FirstName.IsNullOrEmpty())
                    {
                        FirstName = other.FirstName;
                    }

                    if ((LastName.IsNullOrEmpty() || LastName.IsNumber()) && !other.LastName.IsNullOrEmpty())
                    {
                        LastName = other.LastName;
                    }

                    if (NormalizedNumber.IsNullOrEmpty() && !other.NormalizedNumber.IsNullOrEmpty())
                    {
                        NormalizedNumber = other.NormalizedNumber;
                    }
                }
            }
        }
    }
}