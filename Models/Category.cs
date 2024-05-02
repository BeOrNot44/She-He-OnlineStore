using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace She___He_Store.Models;

public partial class Category
{
    public decimal Id { get; set; }

    public string? CategoryName { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
