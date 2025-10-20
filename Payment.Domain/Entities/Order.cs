using System;
using System.Collections.Generic;

namespace Payment.Domain.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationInMonths { get; set; }

    public string? Status { get; set; }

    public int? ConfirmedBy { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    //public virtual User? ConfirmedByNavigation { get; set; }

    //public virtual User User { get; set; } = null!;
}
