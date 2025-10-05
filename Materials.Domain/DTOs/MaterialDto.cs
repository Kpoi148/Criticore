using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Material.Domain.DTOs
{
    public class MaterialDto
    {
        public int MaterialId { get; set; }
        public int ClassId { get; set; }
        public int UploadedBy { get; set; }
        public string FileName { get; set; } = "";
        public string FileUrl { get; set; } = "";
        public string FileType { get; set; } = "";
        public long FileSize { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? HomeworkId { get; set; }
    }
}
