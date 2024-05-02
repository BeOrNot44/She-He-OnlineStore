using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace She___He_Store.Models;

public partial class Customer
{
    public decimal Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }
    public string? ImagePath { get; set; } = "~/HomeAssets/img/anonymous.png";
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }

    public DateTime? RegistrationDate { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    [JsonIgnore]
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
