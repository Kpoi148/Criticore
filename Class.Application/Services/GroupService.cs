using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Class.Application.Services
{
    public class GroupService
    {
        private readonly Class.Domain.Repositories.IGroupRepository _groupRepository;

        public GroupService(Class.Domain.Repositories.IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task AddMemberToGroupAsync(int groupId, int classMemberId)
        {
            await _groupRepository.AddMemberToGroupAsync(groupId, classMemberId);
        }

        public async Task RemoveMemberFromGroupAsync(int groupId, int classMemberId)
        {
            await _groupRepository.RemoveMemberFromGroupAsync(groupId, classMemberId);
        }
    }
}