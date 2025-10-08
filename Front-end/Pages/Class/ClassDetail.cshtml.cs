using System.Text.Json;
using Class.Domain.DTOs;
using Class.Domain.Entities;
using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassDto = Front_end.Models.ClassDto;
using TopicDto = Front_end.Models.TopicDto;
namespace Front_end.Pages.Class
{
    public class ClassDetailModel : PageModel
    {
        public string? CurrentUserId { get; set; }
        public List<TopicDto> Topics { get; set; } = new();
        public ClassDto CurrentClass { get; set; } = new();
        public List<User> Students { get; set; } = new();
        public List<object> ReportData { get; set; } = new();
        public string TeacherName { get; set; } = "Unknown"; // Teacher động
        public int MembersCount { get; set; } = 0; // Số members động
        public bool IsTeacher { get; set; } = false;
        // Quản lí tài liệu
        public List<MaterialDto> Materials { get; set; } = new(); // Tài liệu của lớp
        [BindProperty] public IFormFile? UploadFile { get; set; }
        [BindProperty] public int ClassId { get; set; }
        private readonly ITopicService _topicService;
        private readonly IClassesService _classesService;
        private readonly IMaterialService _materialService;
        public ClassDetailModel(ITopicService topicService, IClassesService classesService
            , IMaterialService materialService)
        {
            _topicService = topicService;
            _classesService = classesService;
            _materialService = materialService;
        }
        public async Task OnGetAsync(int id)
        {
            // Debug: Bắt đầu OnGetAsync
            Console.WriteLine($"=== OnGetAsync started for class id: {id} ===");

            CurrentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine($"CurrentUserId from claims: {CurrentUserId ?? "null"}");

            // Lấy thông tin lớp học (di chuyển lên đầu để sử dụng Members sau)
            CurrentClass = await _classesService.GetByIdAsync(id) ?? new ClassDto();
            ClassId = CurrentClass.ClassId;

            // Debug dữ liệu CurrentClass
            var json = JsonSerializer.Serialize(CurrentClass, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("=== CurrentClass loaded ===");
            Console.WriteLine(json);

            // Set IsTeacher SAU khi load CurrentClass
            if (!string.IsNullOrEmpty(CurrentUserId))
            {
                var member = CurrentClass.Members.FirstOrDefault(m => m.UserId == int.Parse(CurrentUserId));
                IsTeacher = member?.RoleInClass == "Teacher";
                Console.WriteLine($"User {CurrentUserId} is teacher: {IsTeacher} (Role: {member?.RoleInClass ?? "not found"})");
            }
            else
            {
                Console.WriteLine("CurrentUserId is null or empty, skipping IsTeacher check.");
            }

            // Lấy danh sách students động
            Students = CurrentClass.Members
                .Where(m => m.RoleInClass != "Teacher")
                .Select(m => new User { UserId = m.UserId, FullName = m.FullName })
                .ToList();
            Console.WriteLine($"Students count: {Students.Count}");

            // Lấy teacher động
            var teacher = CurrentClass.Members.FirstOrDefault(m => m.RoleInClass == "Teacher");
            TeacherName = teacher?.FullName ?? "Unknown";
            Console.WriteLine($"TeacherName: {TeacherName}");

            // Tính số members
            MembersCount = CurrentClass.Members.Count;
            Console.WriteLine($"Members count: {MembersCount}");
            foreach (var m in CurrentClass.Members)
            {
                Console.WriteLine($"{m.FullName} - {m.RoleInClass} (UserId: {m.UserId})");
            }

            Topics = await _topicService.GetAllByClassAsync(id);
            Console.WriteLine($"Topics count: {Topics.Count}");

            Materials = await _materialService.GetByClassIdAsync(id);
            Console.WriteLine($"Materials count: {Materials.Count}");

            ReportData = new List<object>
            {
                new { senderName = "Student1", engagement = "Positive" },
                new { senderName = "Student1", engagement = "Neutral" },
                new { senderName = "Student2", engagement = "Negative" },
                new { senderName = "Student3", engagement = "Positive" }
            };
            Console.WriteLine($"ReportData count: {ReportData.Count}");

            // Debug: Kết thúc OnGetAsync
            Console.WriteLine("=== OnGetAsync completed ===");
        }
        public async Task<IActionResult> OnPostUploadAsync()
        {
            // Debug: Bắt đầu OnPostUploadAsync
            Console.WriteLine($"=== OnPostUploadAsync started for ClassId: {ClassId} ===");

            CurrentUserId = User.FindFirst("UserId")?.Value;
            Console.WriteLine($"CurrentUserId: {CurrentUserId ?? "null"}");

            if (UploadFile == null)
            {
                Console.WriteLine("UploadFile is null.");
            }
            else
            {
                Console.WriteLine($"UploadFile: {UploadFile.FileName}, Size: {UploadFile.Length}");
            }

            if (UploadFile == null || string.IsNullOrEmpty(CurrentUserId))
            {
                Console.WriteLine("Upload skipped due to null file or userId.");
                return Page();
            }

            var success = await _materialService.UploadAsync(
                 UploadFile,
                 ClassId, // lấy trực tiếp từ CurrentClass
                 int.Parse(CurrentUserId)
             );
            Console.WriteLine($"Upload success: {success}");

            if (!success) ModelState.AddModelError("", "Upload thất bại!");
            return RedirectToPage(new { id = ClassId });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Debug: Bắt đầu OnPostDeleteAsync
            Console.WriteLine($"=== OnPostDeleteAsync started for MaterialId: {id}, ClassId: {ClassId} ===");

            var success = await _materialService.DeleteAsync(id);
            Console.WriteLine($"Delete success: {success}");

            if (!success) ModelState.AddModelError("", "Xóa thất bại!");
            // Dùng CurrentClass.ClassId để redirect
            return RedirectToPage(new { id = ClassId });
        }
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.#} {sizes[order]}";
        }
    }
}