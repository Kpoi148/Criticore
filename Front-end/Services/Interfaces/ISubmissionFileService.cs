namespace Front_end.Services.Interfaces
{
    public interface ISubmissionFileService
    {
        Task<string?> UploadAsync(IFormFile file);
    }
}
