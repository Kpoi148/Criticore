using System;
using System.Collections.Generic;

namespace Identity.Domain.Entities;

public partial class Aianalysis
{
    public int AianalysisId { get; set; }

    public int TopicId { get; set; }

    public string? Sentiment { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Topic Topic { get; set; } = null!;
}
