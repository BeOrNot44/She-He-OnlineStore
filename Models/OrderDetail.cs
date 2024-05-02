using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace She___He_Store.Models;

public partial class OrderDetail
{
    public decimal Id { get; set; }

    public decimal? CustomerId { get; set; }

    public decimal? ProductId { get; set; }

    public decimal? Quantity { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; } = "OnCart";

    public DateTime? DeliveryDate { get; set; }
    [JsonIgnore]
    public virtual Customer? Customer { get; set; }

    [JsonIgnore]
    public virtual Product? Product { get; set; }

}
