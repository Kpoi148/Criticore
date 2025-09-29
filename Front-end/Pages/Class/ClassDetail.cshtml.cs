using Class.Domain.DTOs;
using Class.Domain.Entities;
using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassDto = Front_end.Models.ClassDto;
using TopicDto = Front_end.Models.TopicDto;

namespace Front_end.Pages.Class
{
    public class ClassDetailModel : PageModel
    {
        public List<TopicDto> Topics { get; set; } = new();
        public ClassDto CurrentClass { get; set; } = new();
        public List<User> Students { get; set; } = new();
        public List<object> ReportData { get; set; } = new();
        public string TeacherName { get; set; } = "Unknown"; // Teacher động
        public int MembersCount { get; set; } = 0; // Số members động

        private readonly ITopicService _topicService;
        private readonly IClassesService _classesService;

        public ClassDetailModel(ITopicService topicService, IClassesService classesService)
        {
            _topicService = topicService;
            _classesService = classesService;
        }

        public async Task OnGetAsync(int id)
        {
            CurrentClass = await _classesService.GetByIdAsync(id) ?? new ClassDto();

            Students = CurrentClass.Members
                .Where(m => m.RoleInClass != "Teacher")
                .Select(m => new User { UserId = m.UserId, FullName = m.FullName })
                .ToList();

            // Lấy teacher động
            var teacher = CurrentClass.Members.FirstOrDefault(m => m.RoleInClass == "Teacher");
            TeacherName = teacher?.FullName ?? "Unknown";

            // Tính số members
            MembersCount = CurrentClass.Members.Count;

            Topics = await _topicService.GetAllByClassAsync(id);

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