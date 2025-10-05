using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.Class
{
    public class HomeworkListModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;
        private readonly ITopicService _topicService;
        private readonly IClassesService _classesService;

        public HomeworkListModel(
            IHomeworkService homeworkService,
            ITopicService topicService,
            IClassesService classesService)
        {
            _homeworkService = homeworkService;
            _topicService = topicService;
            _classesService = classesService;
        }

        public ClassDto CurrentClass { get; set; } = new();
        public TopicDto CurrentTopic { get; set; } = new();
        public List<HomeworkDto> Homeworks { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string class_id, string topic_id)
        {
            if (string.IsNullOrEmpty(class_id) || string.IsNullOrEmpty(topic_id))
                return BadRequest("Thiếu class_id hoặc topic_id");

            int classId = int.Parse(class_id);
            int topicId = int.Parse(topic_id);

            CurrentClass = await _classesService.GetByIdAsync(classId);
            if (CurrentClass == null)
                return NotFound("Không tìm thấy lớp học");

            CurrentTopic = await _topicService.GetByIdAsync(topicId);
            if (CurrentTopic == null)
                return NotFound("Không tìm thấy chủ đề");

            Homeworks = await _homeworkService.GetByTopicAsync(topicId);

            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync(HomeworkCreateDto dto)
        {
            // Lấy ClassID thủ công vì không có trong DTO
            var classId = Request.Form["ClassID"];
            var topicId = dto.TopicID;
            if (string.IsNullOrEmpty(classId))
            {
                TempData["Error"] = "Thiếu thông tin lớp học.";
                return RedirectToPage();
            }

            // Kiểm tra dữ liệu hợp lệ
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Title))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin bài tập.";
                return RedirectToPage(new
                {
                    class_id = Request.Query["class_id"],
                    topic_id = Request.Query["topic_id"]
                });
            }

            // Gọi service tạo bài tập
            var created = await _homeworkService.CreateAsync(dto);
            if (created == null)
            {
                TempData["Error"] = "Tạo bài tập thất bại.";
                return RedirectToPage(new { class_id = classId, topic_id = topicId });
            }

            // Thành công
            TempData["Success"] = "Tạo bài tập thành công!";
            return RedirectToPage(new { class_id = classId, topic_id = topicId });
        }


    }
}
