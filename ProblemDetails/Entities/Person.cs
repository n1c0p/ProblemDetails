using System;
using System.ComponentModel.DataAnnotations;

namespace ProblemDetails.Entities
{
    public class Person
    {
        [Required]
        public string Name { get; set; }

        public string LastName { get; set; }
    }
}

