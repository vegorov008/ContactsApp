using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    public interface IHasIdentity
    {
        Guid Id { get; set; }
    }
}
