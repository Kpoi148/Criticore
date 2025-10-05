using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Material.Domain.DTOs
{
    public class MaterialUploadDto
    {
        public int ClassId { get; set; }          // Lớp thuộc về
        public int UploadedBy { get; set; }       // Người upload (UserId)

        [FromForm]
        public IFormFile File { get; set; } = default!;
        public int? HomeworkId { get; set; }   // nếu là file của homework
    }
}
