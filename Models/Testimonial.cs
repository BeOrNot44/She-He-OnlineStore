using System;
using System.Collections.Generic;

namespace She___He_Store.Models;

public partial class Testimonial
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public string? TestimonialText { get; set; }

    public DateTime? TestimonialDate { get; set; }

    public decimal? Rating { get; set; }

    public decimal? Isapproved { get; set; }

    public virtual Customer? Customer { get; set; }
}
