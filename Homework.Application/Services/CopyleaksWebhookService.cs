using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ************************************************
// THÊM: Class DTO để khắc phục lỗi serialization 500
// ************************************************
public class WebhookResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

namespace Homework.Application.Services
{
    public class CopyleaksWebhookService
    {
        private readonly ICopyleaksReportRepository _reportRepo;

        public CopyleaksWebhookService(ICopyleaksReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        // ĐÃ SỬA: Thay thế ValueTuple bằng WebhookResponse
        public async Task<WebhookResponse> HandleCompletedWebhookAsync(dynamic data)
        {
            try
            {
                // Lấy scanId từ scannedDocument
                string scanId = data?.scannedDocument?.scanId;
                string developerPayloadString = data.developerPayload;
                if (string.IsNullOrEmpty(scanId))
                    return new WebhookResponse { Success = false, Message = "Missing scanId" };

                int userId = 0;
                // XHUYỂN ĐỔI USER ID
                if (!int.TryParse(developerPayloadString, out userId))
                {
                    // Ghi log lỗi nếu không lấy được UserId, nhưng vẫn tiếp tục để tránh lỗi
                    // Nếu bạn bắt buộc phải có UserId, hãy ném exception để được bắt ở Controller
                    // Nhưng tốt nhất là xử lý ngoại lệ bên trong
                    // Ghi log ở đây và dùng return Ok() ở Controller.
                }
                string fileName = data?.scannedDocument?.metadata?.filename;
                // ✅ Đọc kết quả internet/database nếu có
                // Lưu ý: data?.pdfReport thường nằm ở cấp gốc, không phải trong 'scannedDocument'
                // Tùy thuộc vào cấu hình webhook của bạn, nó có thể không có ở đây
                string reportUrl = data?.pdfReport ?? "";

                // Lấy AggregatedScore trực tiếp từ response (vì nó có sẵn trong JSON bạn cung cấp)
                double plagiarismScore = data?.results?.score?.aggregatedScore ?? 0.0;

                // ************************************************************
                // SỬA ĐỔI ĐỂ LẤY ĐIỂM AI (AI Content Score)
                // ************************************************************
                double aiContentScore = 0;

                // 1. Tìm Alert có mã 'suspected-ai-text'
                // Dùng JsonConvert.SerializeObject(data.notifications.alerts) nếu data là dynamic
                // Hoặc cố gắng ép kiểu nếu data?.notifications?.alerts có thể truy cập được
                dynamic aiAlert = ((IEnumerable<dynamic>)data?.notifications?.alerts ?? Enumerable.Empty<dynamic>())
                                    .FirstOrDefault(a => a.code == "suspected-ai-text");
                if (aiAlert != null)
                {
                    // 2. Trường 'additionalData' là một chuỗi JSON, cần Deserialize lại
                    string additionalDataJson = aiAlert.additionalData;

                    // Sử dụng JObject/dynamic để phân tích chuỗi lồng nhau này
                    dynamic additionalData = JObject.Parse(additionalDataJson);

                    // 3. Lấy điểm AI từ summary.ai (Giá trị này là 1.0 = 100% trong JSON mẫu của bạn)
                    // Hoặc lấy điểm xác suất (probability)
                    double aiSummaryScore = additionalData?.summary?.ai ?? 0.0;

                    // Chúng ta sẽ lấy điểm từ summary.ai (thang điểm từ 0.0 đến 1.0)
                    aiContentScore = aiSummaryScore * 100; // Chuyển sang thang điểm 0-100%
                }

                // Nếu bạn muốn tính tổng số matches (như logic cũ của bạn) thay vì dùng aggregatedScore:
                // int internetMatches = ((IEnumerable<dynamic>)data?.results?.internet ?? new List<dynamic>()).Count();
                // int databaseMatches = ((IEnumerable<dynamic>)data?.results?.database ?? new List<dynamic>()).Count();
                // plagiarismScore = internetMatches + databaseMatches;


                var report = new CopyleaksReport
                {
                    ScanId = scanId,
                    Status = "Completed",
                    SimilarityScore = plagiarismScore,
                    AiContentScore = aiContentScore,
                    ReportUrl = reportUrl,
                    RawResponse = JsonConvert.SerializeObject(data),
                    CreatedAt = DateTime.Now,
                    UserId = userId,
                    FileName = fileName
                };

                await _reportRepo.AddReportAsync(report);
                await _reportRepo.SaveChangesAsync();

                // ĐÃ SỬA: Trả về DTO WebhookResponse
                return new WebhookResponse { Success = true, Message = "Copyleaks report saved successfully" };
            }
            catch (Exception ex)
            {
                // ĐÃ SỬA: Trả về DTO WebhookResponse
                return new WebhookResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ĐÃ SỬA: Thay thế ValueTuple bằng WebhookResponse
        public async Task<WebhookResponse> HandleErrorWebhookAsync(dynamic error)
        {
            try
            {
                string scanId = error.scanId ?? "unknown";
                var report = new CopyleaksReport
                {
                    ScanId = scanId,
                    Status = "Error",
                    ReportUrl = null,
                    RawResponse = JsonConvert.SerializeObject(error),
                    CreatedAt = DateTime.Now
                };

                await _reportRepo.AddReportAsync(report);
                await _reportRepo.SaveChangesAsync();

                // ĐÃ SỬA: Trả về DTO WebhookResponse
                return new WebhookResponse { Success = true, Message = "Error report saved" };
            }
            catch (Exception ex)
            {
                // ĐÃ SỬA: Trả về DTO WebhookResponse
                return new WebhookResponse { Success = false, Message = $"Error saving error report: {ex.Message}" };
            }
        }
    }
}