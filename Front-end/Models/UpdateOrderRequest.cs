namespace Front_end.Models
{
    public class UpdateOrderRequest
    {
        public string? PackageName { get; set; }
        public decimal? Price { get; set; }
        public int? DurationInMonths { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ConfirmedBy { get; set; }
    }
}
