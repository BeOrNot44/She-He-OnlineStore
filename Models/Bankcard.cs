using System;
using System.Collections.Generic;

namespace She___He_Store.Models;

public partial class Bankcard
{
    public decimal Id { get; set; }

    public string? CardNumber { get; set; }

    public string? Cvv { get; set; }

    public string? ExpirationDate { get; set; }

    public decimal? Balance { get; set; }
}
