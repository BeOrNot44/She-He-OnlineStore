using System;
using System.Collections.Generic;

namespace She___He_Store.Models;

public partial class ContactU
{
    public decimal Id { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? VegetarianFriendly { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? DateAdded { get; set; }

    public string? DescriptionText { get; set; }
}
