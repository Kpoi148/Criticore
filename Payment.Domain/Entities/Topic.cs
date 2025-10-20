using System;
using System.Collections.Generic;

namespace Payment.Domain.Entities;

public partial class Topic
{
    public int TopicId { get; set; }

    public int ClassId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public DateTime? EndTime { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Aianalysis> Aianalyses { get; set; } = new List<Aianalysis>();

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Class Class { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<HomeWork> HomeWorks { get; set; } = new List<HomeWork>();
}
