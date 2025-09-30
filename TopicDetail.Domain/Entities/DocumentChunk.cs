using System;
using System.Collections.Generic;

namespace TopicDetail.Domain.Models;

public partial class DocumentChunk
{
    public int ChunkId { get; set; }

    public int AidocumentId { get; set; }

    public string Content { get; set; } = null!;

    public int? PageNumber { get; set; }

    public byte[]? Embedding { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Aidocument Aidocument { get; set; } = null!;
}
