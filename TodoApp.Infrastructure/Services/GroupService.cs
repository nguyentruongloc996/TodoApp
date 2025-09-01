using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = createGroupDto.Name,
                MemberIds = new List<Guid> { createGroupDto.CreatedBy },
                TaskIds = new List<Guid>()
            };

            var createdGroup = await _unitOfWork.Groups.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();

            return new GroupDto
            {
                Id = createdGroup.Id,
                Name = createdGroup.Name,
                MemberIds = createdGroup.MemberIds,
                TaskIds = createdGroup.TaskIds
            };
        }

        public async System.Threading.Tasks.Task<GroupDto> UpdateGroupAsync(Guid groupId, UpdateGroupDto updateGroupDto)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found");

            group.Name = updateGroupDto.Name;
            group.MemberIds = updateGroupDto.MemberIds ?? group.MemberIds;
            group.TaskIds = updateGroupDto.TaskIds ?? group.TaskIds;

            var updatedGroup = await _unitOfWork.Groups.UpdateAsync(group);
            await _unitOfWork.SaveChangesAsync();

            return new GroupDto
            {
                Id = updatedGroup.Id,
                Name = updatedGroup.Name,
                MemberIds = updatedGroup.MemberIds,
                TaskIds = updatedGroup.TaskIds
            };
        }

        public async System.Threading.Tasks.Task<GroupDto> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found");

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                MemberIds = group.MemberIds,
                TaskIds = group.TaskIds
            };
        }

        public async System.Threading.Tasks.Task<List<GroupDto>> GetGroupsByMemberIdAsync(Guid memberId)
        {
            var groups = await _unitOfWork.Groups.GetByMemberIdAsync(memberId);
            
            return groups.Select(group => new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                MemberIds = group.MemberIds,
                TaskIds = group.TaskIds
            }).ToList();
        }

        public async System.Threading.Tasks.Task<bool> DeleteGroupAsync(Guid groupId)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                return false;

            await _unitOfWork.Groups.DeleteAsync(groupId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task<bool> AddMemberToGroupAsync(Guid groupId, Guid memberId)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                return false;

            if (!group.MemberIds.Contains(memberId))
            {
                group.MemberIds.Add(memberId);
                await _unitOfWork.Groups.UpdateAsync(group);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async System.Threading.Tasks.Task<bool> RemoveMemberFromGroupAsync(Guid groupId, Guid memberId)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                return false;

            if (group.MemberIds.Contains(memberId))
            {
                group.MemberIds.Remove(memberId);
                await _unitOfWork.Groups.UpdateAsync(group);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async System.Threading.Tasks.Task<bool> IsMemberOfGroupAsync(Guid groupId, Guid memberId)
        {
            return await _unitOfWork.Groups.IsMemberAsync(groupId, memberId);
        }
    }
} 