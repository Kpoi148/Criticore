using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainClass = Class.Domain.Entities.Class;
using Class.Domain.Entities;
namespace Front_end.Pages.Class
{
    public class ClassListModel : PageModel
    {
        public List<DomainClass> Classes { get; set; } = new List<DomainClass>();
        public List<User> Students { get; set; } = new List<User>();

        public void OnGet()
        {
            // Dữ liệu hardcode cho demo giao diện
            Students = new List<User>
        {
            new() { UserId = 1, FullName = "Student1" },
            new() { UserId = 2, FullName = "Student2" },
            new() { UserId = 3, FullName = "Student3" }
        };

            Classes = new List<DomainClass>
        {
            new() { ClassId = 1, SubjectCode = "PRN212", Semester = "Semester 1 - 2025", ClassName = "PRN232", CreatedBy = 1, MembersCount = 50, Status = "Not Started" },
            new() { ClassId = 2, SubjectCode = "PRN213", Semester = "Semester 2 - 2025", ClassName = "EXE201", CreatedBy = 1, MembersCount = 60, Status = "In Progress" }
        };
        }
    }
}
