using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto);
        Task<GroupDto> UpdateGroupAsync(Guid groupId, UpdateGroupDto updateGroupDto);
        Task<GroupDto> GetGroupByIdAsync(Guid groupId);
        Task<List<GroupDto>> GetGroupsByMemberIdAsync(Guid memberId);
        Task<bool> DeleteGroupAsync(Guid groupId);
        Task<bool> AddMemberToGroupAsync(Guid groupId, Guid memberId);
        Task<bool> RemoveMemberFromGroupAsync(Guid groupId, Guid memberId);
        Task<bool> IsMemberOfGroupAsync(Guid groupId, Guid memberId);
    }
} 