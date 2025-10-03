using AutoMapper;
using Class.Domain.DTOs;
using Class.Domain.Entities;
using Class.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Application.Services
{
    public class ClassService
    {
        private readonly IClassRepository _classRepo;
        private readonly IClassMemberRepository _memberRepo;
        private readonly IMapper _mapper;

        public ClassService(IClassRepository classRepo, IClassMemberRepository memberRepo, IMapper mapper)
        {
            _classRepo = classRepo;
            _memberRepo = memberRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            var entities = await _classRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassDto>>(entities);
        }

        public async Task<ClassDto?> GetByIdAsync(int id)
        {
            var entity = await _classRepo.GetByIdAsync(id);
            return _mapper.Map<ClassDto?>(entity);
        }

        public async Task<ClassDto> AddAsync(ClassCreateDto dto)
        {
            var entity = _mapper.Map<Class.Domain.Entities.Class>(dto);
            // Lưu class trước để có ClassId
            await _classRepo.AddAsync(entity);
            // thêm giáo viên vào classmember
            if (dto.TeacherId > 0)
            {
                var teacherMember = new ClassMember
                {
                    ClassId = entity.ClassId,
                    UserId = dto.TeacherId,
                    RoleInClass = "Teacher",
                    JoinedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                await _memberRepo.AddAsync(teacherMember);
            }
            return _mapper.Map<ClassDto>(entity); // entity lúc này có ClassId
        }

        // File: Class.Application\Services\ClassService.cs

        public async Task UpdateAsync(ClassDto dto)
        {
            // 1. TÌM KIẾM LỚP HỌC HIỆN TẠI (Đảm bảo lớp tồn tại và được theo dõi bởi DbContext)
            var existingEntity = await _classRepo.GetByIdAsync(dto.ClassId);

            if (existingEntity == null)
            {
                throw new Exception("Class not found");
            }

            // 2. ÁNH XẠ THỦ CÔNG (Manual Mapping)
            // Gán các giá trị từ DTO vào Entity đã tồn tại, BỎ QUA các collection/navigation properties.
            existingEntity.ClassName = dto.ClassName;
            existingEntity.SubjectCode = dto.SubjectCode;
            existingEntity.Semester = dto.Semester;
            existingEntity.Description = dto.Description;
            existingEntity.Status = dto.Status;

            // BỎ QUA: ClassId (không thay đổi), ClassMembers (để tránh lỗi), CreatedBy, JoinCode (thường không cập nhật qua form này)

            // 3. GỌI REPOSITORY ĐỂ LƯU THAY ĐỔI
            // Vì existingEntity đã được track, EF Core chỉ cập nhật các trường đã thay đổi.
            await _classRepo.UpdateAsync(existingEntity);
        }

        public async Task DeleteAsync(int classId)
        {
            // hiện tại chưa xóa group khi nào làm group xong thì cần chỉnh lại
            // Xóa hết member trước
            await _memberRepo.RemoveAllMembersFromClassAsync(classId);

            // Xóa lớp
            await _classRepo.DeleteAsync(classId);
        }


        public async Task<ClassDto?> JoinByCodeAsync(string joinCode, int userId)
        {
            var cls = await _classRepo.GetByJoinCodeAsync(joinCode);
            if (cls == null) return null;

            await _memberRepo.AddStudentToClassAsync(cls.ClassId, userId);
            return _mapper.Map<ClassDto>(cls);
        }

        public async Task<List<ClassMemberDto>> GetMembersByClassAsync(int classId)
        {
            var members = await _memberRepo.GetMembersByClassAsync(classId);
            return _mapper.Map<List<ClassMemberDto>>(members);
        }
        public async Task<List<ClassMemberDto>> GetTeachersByClassAsync(int classId)
        {
            var teachers = await _memberRepo.GetTeachersByClassAsync(classId);
            return _mapper.Map<List<ClassMemberDto>>(teachers);
        }

        public Task RemoveMemberFromClassAsync(int classMemberId)
            => _memberRepo.RemoveMemberFromClassAsync(classMemberId);

        // Lấy danh sách lớp theo userId
        public async Task<List<ClassSummaryDto>> GetClassesByUserAsync(int userId)
        {
            var classes = await _memberRepo.GetClassesByUserAsync(userId);

            var result = classes.Select(c => new ClassSummaryDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                SubjectCode = c.SubjectCode,
                Semester = c.Semester ?? "",
                Description = c.Description ?? "",
                Status = c.Status ?? "",
                CreatedBy = c.CreatedBy,
                JoinCode = c.JoinCode ?? "",
                MemberCount = c.ClassMembers.Count(m => m.RoleInClass == "Student"),
                Teacher = c.ClassMembers.FirstOrDefault(m => m.RoleInClass == "Teacher")?.User.FullName ?? "Unknown"
            }).ToList();

            return result;
        }
        public async Task<bool> AssignTeacherAsync(int classId, int teacherId)
        {
            // Lấy tất cả teacher hiện tại trong lớp
            var currentTeachers = await _memberRepo.GetTeachersByClassAsync(classId);

            // Xóa tất cả teacher cũ
            foreach (var t in currentTeachers)
            {
                await _memberRepo.RemoveMemberFromClassAsync(t.ClassMemberId);
            }

            // Thêm giáo viên mới
            var newTeacher = new ClassMember
            {
                ClassId = classId,
                UserId = teacherId,
                RoleInClass = "Teacher",
                JoinedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            await _memberRepo.AddAsync(newTeacher);
            return true;
        }
    }
}


