using System;
using System.Collections.Generic;

namespace Payment.Domain.Entities;

public partial class Material
{
    public int MaterialId { get; set; }

    public int ClassId { get; set; }

    public int UploadedBy { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? HomeworkId { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual HomeWork? Homework { get; set; }

    public virtual User UploadedByNavigation { get; set; } = null!;
}
