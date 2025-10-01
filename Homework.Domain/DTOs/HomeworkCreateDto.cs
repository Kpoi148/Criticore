using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Domain.DTOs
{
    public class HomeworkCreateDto
    {
        [Required(ErrorMessage = "TopicID là bắt buộc.")]
        public int TopicID { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự.")]
        public string? Status { get; set; } // Ví dụ: 'Draft', 'Active'

        public DateTime? DueDate { get; set; }

        // CreatedBy sẽ được lấy từ context người dùng đang đăng nhập, không cần truyền từ client
    }
}

