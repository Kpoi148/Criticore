namespace Front_end.Models
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? PackageName { get; set; }
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ConfirmedBy { get; set; }
    }
}
