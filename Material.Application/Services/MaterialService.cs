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
        public async Task UploadAndSaveAsync(IFormFile file, int classId, int uploadedBy,
            int? homeworkId = null)
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
                CreatedAt = DateTime.UtcNow,
            };
            // Nếu có homeworkId hợp lệ thì thêm vào
            if (homeworkId.HasValue && homeworkId.Value != 0)
            {
                material.HomeworkId = homeworkId.Value;
            }
            // Lưu vào DB qua repository
            await _materialRepo.AddAsync(material);
        }

        public async Task<Material.Domain.DTOs.MaterialDto?> GetByIdAsync(int id)
        {
            var material = await _materialRepo.GetByIdAsync(id);

            if (material == null) return null;

            return new Material.Domain.DTOs.MaterialDto
            {
                MaterialId = material.MaterialId,
                ClassId = material.ClassId,
                UploadedBy = material.UploadedBy,
                FileName = material.FileName,
                FileUrl = material.FileUrl,
                FileType = material.FileType,
                FileSize = material.FileSize,
                CreatedAt = material.CreatedAt,
                HomeworkId = material.HomeworkId
            };
        }

        public async Task<IEnumerable<Material.Domain.DTOs.MaterialDto>> GetByClassIdAsync(int classId)
        {
            var materials = await _materialRepo.GetByClassIdAsync(classId);

            return materials.Select(m => new Material.Domain.DTOs.MaterialDto
            {
                MaterialId = m.MaterialId,
                ClassId = m.ClassId,
                UploadedBy = m.UploadedBy,
                FileName = m.FileName,
                FileUrl = m.FileUrl,
                FileType = m.FileType,
                FileSize = m.FileSize,
                CreatedAt = m.CreatedAt,
                HomeworkId = m.HomeworkId
            });
        }
        public async Task<IEnumerable<Material.Domain.DTOs.MaterialDto>> GetAllAsync()
        {
            var materials = await _materialRepo.GetAllAsync();

            return materials.Select(m => new Material.Domain.DTOs.MaterialDto
            {
                MaterialId = m.MaterialId,
                ClassId = m.ClassId,
                UploadedBy = m.UploadedBy,
                FileName = m.FileName,
                FileUrl = m.FileUrl,
                FileType = m.FileType,
                FileSize = m.FileSize,
                CreatedAt = m.CreatedAt,
                HomeworkId = m.HomeworkId
            });
        }
        public async Task<IEnumerable<Material.Domain.DTOs.MaterialDto>> GetByHomeworkIdAsync(int homeworkId)
        {
            var materials = await _materialRepo.GetByHomeworkIdAsync(homeworkId);

            return materials.Select(m => new Material.Domain.DTOs.MaterialDto
            {
                MaterialId = m.MaterialId,
                ClassId = m.ClassId,
                UploadedBy = m.UploadedBy,
                FileName = m.FileName,
                FileUrl = m.FileUrl,
                FileType = m.FileType,
                FileSize = m.FileSize,
                CreatedAt = m.CreatedAt,
                HomeworkId = m.HomeworkId
            });
        }

        public Task DeleteAsync(int id)
            => _materialRepo.DeleteAsync(id);
    }
}
