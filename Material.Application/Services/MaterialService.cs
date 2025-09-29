using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Material.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Material.Application.Services
{
    public class MaterialService
    {
        private readonly CloudinaryService _cloudinary;
        private readonly IMaterialRepository _materialRepo;

        public MaterialService(CloudinaryService cloudinary, IMaterialRepository materialRepo)
        {
            _cloudinary = cloudinary;
            _materialRepo = materialRepo;
        }

        // Upload file lên Cloudinary + lưu metadata vào DB
        public async Task<Material.Domain.Entities.Material> UploadAndSaveAsync(
        IFormFile file, int classId, int uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.", nameof(file));

            // Upload lên Cloudinary
            var url = await _cloudinary.UploadFileAsync(file, "materials");

            // Lấy file extension an toàn (không bị null/uppercase)
            var extension = Path.GetExtension(file.FileName)?.Trim('.').ToLower() ?? "unknown";

            // Tạo entity
            var material = new Material.Domain.Entities.Material
            {
                ClassId = classId,
                UploadedBy = uploadedBy,
                FileName = Path.GetFileName(file.FileName), // tránh path injection
                FileUrl = url,
                FileType = extension,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào DB qua repository
            return await _materialRepo.AddAsync(material);
        }


        public Task<Material.Domain.Entities.Material?> GetByIdAsync(int id)
            => _materialRepo.GetByIdAsync(id);

        public Task<IEnumerable<Material.Domain.Entities.Material>> GetByClassIdAsync(int classId)
            => _materialRepo.GetByClassIdAsync(classId);

        public Task<IEnumerable<Material.Domain.Entities.Material>> GetAllAsync()
            => _materialRepo.GetAllAsync();

        public Task DeleteAsync(int id)
            => _materialRepo.DeleteAsync(id);
    }
}
