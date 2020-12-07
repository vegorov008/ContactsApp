using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    public class Contact : IEquatable<Contact>, IHasIdentity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public bool Equals(Contact other)
        {
            return ReferenceEquals(this, other) ||
                (other != null &&
                FirstName == other.FirstName &&
                LastName == other.LastName &&
                Phone == other.Phone);
        }
    }
}
