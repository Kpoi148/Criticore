using Newtonsoft.Json;
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
        public string? FileName { get; set; }
        public string? RawResponse { get; set; } // dữ liệu JSON gốc từ Copyleaks

        // List hiển thị nguồn internet (tự parse từ RawResponse)
        public List<CopyleaksInternetMatch> InternetSources
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(RawResponse))
                        return new List<CopyleaksInternetMatch>();

                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(RawResponse)!;
                    var list = new List<CopyleaksInternetMatch>();

                    foreach (var src in json.results.internet)
                    {
                        list.Add(new CopyleaksInternetMatch
                        {
                            Url = (string?)src.url ?? "",
                            Title = (string?)src.title ?? "",
                            MatchedWords = (int?)src.matchedWords ?? 0,
                            IdenticalWords = (int?)src.identicalWords ?? 0
                        });
                    }
                    return list;
                }
                catch
                {
                    return new List<CopyleaksInternetMatch>();
                }
            }
        }
    }
}
