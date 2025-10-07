using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Material.Domain.DTOs
{
    public class SubmissionFileUploadDto
    {
        [FromForm]
        public IFormFile File { get; set; } = default!;
    }
}
