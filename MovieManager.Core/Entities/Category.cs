using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManager.Core.Entities
{
    public class Category : EntityObject
    {
        [Required]
        public String CategoryName { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}
