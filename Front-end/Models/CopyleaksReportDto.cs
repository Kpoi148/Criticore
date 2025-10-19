namespace Front_end.Models
{
    public class CopyleaksReportDto
    {
        public int ReportId { get; set; }
        public int? UserId { get; set; }
        public string ScanId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double SimilarityScore { get; set; }
        public double AiContentScore { get; set; }
        public string ReportUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
