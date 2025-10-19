using System.Data;
using Front_end.Models;
using Front_end.Services;
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
        private readonly IMaterialService _materialService;
        private readonly ISubmissionService _submissionService;
        private readonly IUsersService _usersService;

        public HomeworkListModel(
          IHomeworkService homeworkService,
          ITopicService topicService,
          IClassesService classesService,
          IMaterialService materialService,
          ISubmissionService submissionService,
          IUsersService usersService
        )
        {
            _homeworkService = homeworkService;
            _topicService = topicService;
            _classesService = classesService;
            _materialService = materialService;
            _submissionService = submissionService;
            _usersService = usersService;
        }

        public ClassDto CurrentClass { get; set; } = new();
        public TopicDto CurrentTopic { get; set; } = new();
        public List<HomeworkDto> Homeworks { get; set; } = new();
        public string? CurrentUserId { get; set; }
        public string? CurrentUserRole { get; set; }

        // dictionary tạm chứa materials theo homeworkId
        public Dictionary<int, List<MaterialDto>> HomeworkFiles { get; set; } = new();
        public Dictionary<int, List<SubmissionReadDto>> HomeworkSubmissions { get; set; } = new();
        public Dictionary<int, string> SubmissionUserNames { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string class_id, string topic_id)
        {
            if (string.IsNullOrEmpty(class_id) || string.IsNullOrEmpty(topic_id))
                return BadRequest("Missing class_id or topic_id");

            int classId = int.Parse(class_id);
            int topicId = int.Parse(topic_id);
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            CurrentUserRole = role;

            CurrentClass = await _classesService.GetByIdAsync(classId);
            if (CurrentClass == null)
                return NotFound("Class not found");

            CurrentTopic = await _topicService.GetByIdAsync(topicId);
            if (CurrentTopic == null)
                return NotFound("Topic not found");

            Homeworks = await _homeworkService.GetByTopicAsync(topicId);
            // tải materials và submission cho từng homework
            foreach (var hw in Homeworks)
            {
                try
                {
                    // Lấy materials
                    var materials = await _materialService.GetByHomeworkAsync(hw.HomeworkID);
                    HomeworkFiles[hw.HomeworkID] = materials ?? new List<MaterialDto>();

                    // Lấy submissions
                    var submissions = await _submissionService.GetByHomeworkAsync(hw.HomeworkID);
                    HomeworkSubmissions[hw.HomeworkID] = submissions ?? new List<SubmissionReadDto>();
                    Console.WriteLine($"🧩 DEBUG: Homework {hw.HomeworkID} => Submissions count = {submissions?.Count ?? 0}");
                    // Lấy tên người dùng cho từng submission
                    if (submissions != null)
                    {
                        foreach (var s in submissions)
                        {
                            if (!SubmissionUserNames.ContainsKey(s.SubmissionId))
                            {
                                var user = await _usersService.GetByIdAsync(s.UserId);
                                SubmissionUserNames[s.SubmissionId] = user?.FullName ?? $"User {s.UserId}";
                            }

                            Console.WriteLine($"🧩 SubmissionId: {s.SubmissionId}, UserName: {SubmissionUserNames[s.SubmissionId]}, FileUrl: {s.AttachmentUrl}");
                        }
                    }
                }
                catch
                {
                    HomeworkFiles[hw.HomeworkID] = new List<MaterialDto>();
                    HomeworkSubmissions[hw.HomeworkID] = new List<SubmissionReadDto>();
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync(HomeworkCreateDto dto)
        {
            // Lấy userId từ claims
            CurrentUserId = User.FindFirst("UserId")?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            //Console.WriteLine($"👤 UserId từ claims: {CurrentUserId}");
            //Console.WriteLine($"🎓 Role từ claims: {role}");
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return RedirectToPage("/Signin");
            }
            // Lấy ClassID thủ công vì không có trong DTO
            var classId = Request.Form["ClassID"];
            var topicId = dto.TopicID;

            // Hàm helper để redirect kèm lỗi
            IActionResult RedirectWithError(string message)
            {
                TempData["Error"] = message;
                return RedirectToPage(new { class_id = classId, topic_id = topicId });
            }

            // Kiểm tra dữ liệu
            if (string.IsNullOrEmpty(classId))
                return RedirectWithError("Missing class information.");

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Title))
                return RedirectWithError("Please fill in all required homework details.");

            // Chỉ cho phép Teacher
            if (!string.Equals(role, "Teacher", StringComparison.OrdinalIgnoreCase))
                return RedirectWithError("You don't have permission to create homework. Only teachers can do this.");

            // Tạo bài tập
            var created = await _homeworkService.CreateAsync(dto);
            if (created == null)
                return RedirectWithError("Failed to create homework.");

            // Upload file nếu có
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                var uploadedBy = int.Parse(CurrentUserId!);
                var materialService = HttpContext.RequestServices.GetRequiredService<IMaterialService>();

                await materialService.UploadAsync(file, int.Parse(classId), uploadedBy, created.HomeworkID);
            }
            // Thành công
            TempData["Success"] = "Homework created successfully!";
            return RedirectToPage(new { class_id = classId, topic_id = topicId });
        }


        //public async Task<IActionResult> OnPostSubmitAsync(int homeworkId)
        //{
        //    CurrentUserId = User.FindFirst("UserId")?.Value;
        //    if (string.IsNullOrEmpty(CurrentUserId))
        //        return RedirectToPage("/Signin");

        //    var classId = Request.Form["ClassID"];
        //    var topicId = Request.Query["topic_id"];
        //    if (string.IsNullOrEmpty(classId))
        //    {
        //        TempData["Error"] = "Missing class information.";
        //        return RedirectToPage();
        //    }

        //    try
        //    {
        //        var content = Request.Form["Content"];
        //        var file = Request.Form.Files.FirstOrDefault();

        //        string? fileUrl = null;
        //        if (file != null && file.Length > 0)
        //        {
        //            var submissionFileService = HttpContext.RequestServices.GetRequiredService<ISubmissionFileService>();
        //            fileUrl = await submissionFileService.UploadAsync(file);
        //            // Gửi file lên Copyleaks để scan
        //            try
        //            {
        //                var scanId = await _copyleaksService.SubmitFileForScanAsync(file);
        //                Console.WriteLine($"File đã được gửi scan Copyleaks. ScanId: {scanId}");
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Lỗi khi gửi scan Copyleaks: {ex.Message}");
        //                // Nếu muốn, có thể thêm TempData["Error"] hoặc log
        //            }
        //        }

        //        var userId = int.Parse(CurrentUserId!);

        //        // Kiểm tra xem học sinh đã nộp bài chưa
        //        var existingSubmission = await _submissionService.GetByHomeworkAndUserAsync(homeworkId, userId);

        //        if (existingSubmission != null)
        //        {
        //            // Cập nhật submission cũ
        //            var updateDto = new SubmissionUpdateDto
        //            {
        //                Content = content,
        //                AttachmentUrl = fileUrl
        //            };

        //            var updated = await _submissionService.UpdateAsync(updateDto, existingSubmission.SubmissionId);
        //            if (updated == null)
        //            {
        //                TempData["Error"] = "Failed to update submission.";
        //                return RedirectToPage(new { class_id = classId, topic_id = topicId });
        //            }

        //            TempData["Success"] = "Submission updated successfully!";
        //        }
        //        else
        //        {
        //            // Tạo mới submission
        //            var dto = new SubmissionCreateDto
        //            {
        //                HomeworkId = homeworkId,
        //                UserId = userId,
        //                GroupId = null,
        //                Content = content,
        //                AttachmentUrl = fileUrl
        //            };

        //            var created = await _submissionService.CreateAsync(dto);
        //            if (created == null)
        //            {
        //                TempData["Error"] = "Failed to submit assignment.";
        //                return RedirectToPage(new { class_id = classId, topic_id = topicId });
        //            }

        //            TempData["Success"] = "Assignment submitted successfully!";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi khi nộp bài: {ex.Message}");
        //        TempData["Error"] = "Failed to submit assignment.";
        //    }

        //    return RedirectToPage(new { class_id = classId, topic_id = topicId });
        //}
        public async Task<IActionResult> OnPostSubmitAsync(int homeworkId)
        {
            CurrentUserId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToPage("/Signin");

            var classId = Request.Form["ClassID"];
            var topicId = Request.Query["topic_id"];
            if (string.IsNullOrEmpty(classId))
            {
                TempData["Error"] = "Missing class information.";
                return RedirectToPage();
            }

            try
            {
                var content = Request.Form["Content"];
                var file = Request.Form.Files.FirstOrDefault();

                string? fileUrl = null;
                if (file != null && file.Length > 0)
                {
                    var submissionFileService = HttpContext.RequestServices.GetRequiredService<ISubmissionFileService>();
                    fileUrl = await submissionFileService.UploadAsync(file);
                }

                var userId = int.Parse(CurrentUserId!);
                var existingSubmission = await _submissionService.GetByHomeworkAndUserAsync(homeworkId, userId);

                if (existingSubmission != null)
                {
                    var updateDto = new SubmissionUpdateDto
                    {
                        Content = content,
                        AttachmentUrl = fileUrl
                    };

                    var updated = await _submissionService.UpdateAsync(updateDto, existingSubmission.SubmissionId);
                    if (updated == null)
                    {
                        TempData["Error"] = "Failed to update submission.";
                        return RedirectToPage(new { class_id = classId, topic_id = topicId });
                    }

                    TempData["Success"] = "Submission updated successfully!";
                }
                else
                {
                    var dto = new SubmissionCreateDto
                    {
                        HomeworkId = homeworkId,
                        UserId = userId,
                        GroupId = null,
                        Content = content,
                        AttachmentUrl = fileUrl
                    };

                    var created = await _submissionService.CreateAsync(dto);
                    if (created == null)
                    {
                        TempData["Error"] = "Failed to submit assignment.";
                        return RedirectToPage(new { class_id = classId, topic_id = topicId });
                    }

                    TempData["Success"] = "Assignment submitted successfully!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi nộp bài: {ex.Message}");
                TempData["Error"] = "Failed to submit assignment.";
            }

            return RedirectToPage(new { class_id = classId, topic_id = topicId });
        }



    }
}
