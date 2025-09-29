using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front_end.Pages.Class
{
    public class TopicDetailModel : PageModel
    {
        private readonly IClassesService _classesService;
        private readonly ITopicService _topicService;

        public TopicDetailModel(IClassesService classesService, ITopicService topicService)
        {
            _classesService = classesService;
            _topicService = topicService;
        }

        public ClassDto CurrentClass { get; set; }
        public TopicDto CurrentTopic { get; set; }

        public async Task<IActionResult> OnGetAsync(string class_id, string topic_id)
        {
            if (string.IsNullOrEmpty(class_id) || string.IsNullOrEmpty(topic_id))
            {
                return BadRequest("Thi?u class_id ho?c topic_id");
            }

            // Load class t? backend
            CurrentClass = await _classesService.GetByIdAsync(int.Parse(class_id));
            if (CurrentClass == null)
            {
                return NotFound("Kh�ng t?m th?y l?p h?c");
            }

            // Load topic t? backend (gi? s? topic_id l� int)
            CurrentTopic = await _topicService.GetByIdAsync(int.Parse(topic_id));
            if (CurrentTopic == null)
            {
                return NotFound("Kh�ng t?m th?y ch? �?");
            }

            // Ki?m tra topic thu?c class (n?u c?n)
            if (CurrentTopic.ClassId != CurrentClass.ClassId)
            {
                return BadRequest("Ch? �? kh�ng thu?c l?p n�y");
            }

            return Page();
        }
    }
}
