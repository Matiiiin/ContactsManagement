﻿using System.ComponentModel.DataAnnotations;

namespace ContactsManagement.Core.Domain.Entities
{
    public class Country
    {
        [Key]
        public Guid CountryID { get; set; }
        public string CountryName { get; set; }
        // public ICollection<Person>? Persons { get; set; }
    }
}
