namespace Front_end.Models
{
    //lớp nhỏ để deserialize phần internet trong rawResponse:
    public class CopyleaksInternetMatch
    {
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int MatchedWords { get; set; }
        public int IdenticalWords { get; set; }
    }
}
