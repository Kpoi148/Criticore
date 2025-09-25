using Class.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainClass = Class.Domain.Entities.Class;

namespace Front_end.Pages.Class
{
    public class ClassDetailModel : PageModel
    {
        public DomainClass CurrentClass { get; set; } = new();
        public List<User> Students { get; set; } = new();
        public List<object> ReportData { get; set; } = new();

        public void OnGet(int id)
        {
            // Mock danh sách học sinh
            Students = new List<User>
            {
                new() { UserId = 1, FullName = "Student1" },
                new() { UserId = 2, FullName = "Student2" },
                new() { UserId = 3, FullName = "Student3" }
            };

            // Mock thông tin lớp
            CurrentClass = new DomainClass
            {
                ClassId = id,
                SubjectCode = "PRN212",
                Semester = "Semester 1 - 2025",
                ClassName = "Advanced Critical Thinking",
                CreatedBy = 1,
                Status = "In Progress",
                CreatedByNavigation = new User { UserId = 4, FullName = "Nguyen Van A" }
            };

            // Mock danh sách thành viên lớp
            CurrentClass.ClassMembers = new List<ClassMember>
            {
                new()
                {
                    ClassMemberId = 1,
                    UserId = 1,
                    User = new User { UserId = 1, FullName = "Student1" }
                },
                new()
                {
                    ClassMemberId = 2,
                    UserId = 2,
                    User = new User { UserId = 2, FullName = "Student2" }
                },
                new()
                {
                    ClassMemberId = 3,
                    UserId = 3,
                    User = new User { UserId = 3, FullName = "Student3" }
                },
                new()
                {
                    ClassMemberId = 4,
                    UserId = 4,
                    User = new User { UserId = 4, FullName = "Nguyen Van A" },
                    RoleInClass = "Teacher"
                }
            };

            // Mock dữ liệu báo cáo
            ReportData = new List<object>
            {
                new { senderName = "Student1", engagement = "Positive" },
                new { senderName = "Student1", engagement = "Neutral" },
                new { senderName = "Student2", engagement = "Negative" },
                new { senderName = "Student3", engagement = "Positive" }
            };
        }
    }
}
