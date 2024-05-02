using System;
using System.Collections.Generic;

namespace She___He_Store.Models;

public partial class Review
{
    public decimal Id { get; set; }

    public decimal? ProductId { get; set; }

    public decimal? CustomerId { get; set; }

    public string? ReviewText { get; set; }

    public decimal? Rating { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
