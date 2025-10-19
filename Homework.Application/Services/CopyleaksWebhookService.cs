using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Domain.Entities;
using Homework.Domain.Repositories;
using Newtonsoft.Json;

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
                // ✅ Lấy scanId từ scannedDocument
                string scanId = data?.scannedDocument?.scanId;
                if (string.IsNullOrEmpty(scanId))
                    return new WebhookResponse { Success = false, Message = "Missing scanId" };

                // ✅ Đọc kết quả internet/database nếu có
                // Lưu ý: data?.pdfReport thường nằm ở cấp gốc, không phải trong 'scannedDocument'
                // Tùy thuộc vào cấu hình webhook của bạn, nó có thể không có ở đây
                string reportUrl = data?.pdfReport ?? "";

                // Lấy AggregatedScore trực tiếp từ response (vì nó có sẵn trong JSON bạn cung cấp)
                double plagiarismScore = data?.results?.score?.aggregatedScore ?? 0.0;

                // Mặc định AI content score là 0 nếu không có (vì bạn đã tắt AI Detection)
                double aiContentScore = 0;

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
                    CreatedAt = DateTime.Now
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