//using System;
//using System.Collections.Generic;

//namespace Material.Domain.Entities;

//public partial class Aidocument
//{
//    public int AidocumentId { get; set; }

//    public int ClassId { get; set; }

//    public string FileName { get; set; } = null!;

//    public string FileUrl { get; set; } = null!;

//    public int UploadedBy { get; set; }

//    public DateTime? CreatedAt { get; set; }

//    public virtual Class Class { get; set; } = null!;

//    public virtual ICollection<DocumentChunk> DocumentChunks { get; set; } = new List<DocumentChunk>();

//    public virtual User UploadedByNavigation { get; set; } = null!;
//}
