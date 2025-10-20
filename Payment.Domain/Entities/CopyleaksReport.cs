using System;
using System.Collections.Generic;

namespace Payment.Domain.Entities;

public partial class CopyleaksReport
{
    public int ReportId { get; set; }

    public int? UserId { get; set; }

    public string ScanId { get; set; } = null!;

    public string? Status { get; set; }

    public double? SimilarityScore { get; set; }

    public double? AiContentScore { get; set; }

    public string? ReportUrl { get; set; }

    public string? RawResponse { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
