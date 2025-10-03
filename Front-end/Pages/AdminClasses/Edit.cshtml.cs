using Front_end.Models;
using Front_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front_end.Pages.AdminClasses
{
    public class EditModel : PageModel
    {
        private readonly IClassesService _classService;
        private readonly IUsersService _userService;
        public EditModel(IClassesService service, IUsersService userService)
        {
            _classService = service;
            _userService = userService;
        }

        [BindProperty]
        public ClassDto ClassInput { get; set; } = new();
        public List<SelectListItem> TeacherOptions { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cls = await _classService.GetByIdAsync(id);
            if (cls == null) return NotFound();
            ClassInput = cls;

            // Lấy giáo viên hiện tại của lớp (qua API /classes/{id}/teachers)
            var currentTeachers = await _classService.GetTeachersByClassAsync(id);
            var currentTeacherId = currentTeachers.FirstOrDefault()?.UserId;

            // Lấy tất cả giáo viên khả dụng
            var teachers = await _userService.GetTeachersAsync();

            TeacherOptions = teachers.Select(t => new SelectListItem
            {
                Value = t.UserId.ToString(),
                Text = t.FullName,
                Selected = (currentTeacherId == t.UserId) // đánh dấu giáo viên hiện tại
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int teacherId)
        {
            if (!ModelState.IsValid) return Page();

            var success = await _classService.UpdateAsync(ClassInput.ClassId, ClassInput);
            if (!success)
            {
                ModelState.AddModelError("", "Cập nhật lớp thất bại.");
                return Page();
            }
            if (teacherId > 0)
            {
                await _classService.AssignTeacherAsync(ClassInput.ClassId, teacherId);
            }
            return RedirectToPage("Details", new { id = ClassInput.ClassId });
        }
    }
}
