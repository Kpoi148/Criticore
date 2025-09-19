using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Class.Domain.Repositories
{
    public interface IGroupRepository
    {
        Task AddMemberToGroupAsync(int groupId, int classMemberId);
        Task RemoveMemberFromGroupAsync(int groupId, int classMemberId);
    }
}