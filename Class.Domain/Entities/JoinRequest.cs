using System;
using System.Collections.Generic;

namespace Class.Domain.Entities;

public partial class JoinRequest
{
    public int JoinRequestId { get; set; }

    public int ClassId { get; set; }

    public int UserId { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewedBy { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User? ReviewedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
