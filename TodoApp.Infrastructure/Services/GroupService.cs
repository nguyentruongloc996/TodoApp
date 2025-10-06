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
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(createGroupDto.CreatedBy); 
            // Ensure the user exists
            if (user == null)
                throw new ArgumentException("User not found");

            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = createGroupDto.Name,
                Members = new List<User> { user },
                Tasks = new List<Domain.Entities.Task>()
            };

            var createdGroup = await _unitOfWork.Groups.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();

            return new GroupDto
            {
                Id = createdGroup.Id,
                Name = createdGroup.Name,
                MemberIds = createdGroup.Members.Select(member => member.Id).ToList(),
                TaskIds = createdGroup.Members.Select(member => member.Id).ToList()
            };
        }

        public async System.Threading.Tasks.Task<GroupDto> UpdateGroupAsync(Guid groupId, UpdateGroupDto updateGroupDto)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found");

            ICollection<User> members = new List<User>();
            foreach (var memberId in updateGroupDto.MemberIds ?? new List<Guid>())
            {
                var user = await _unitOfWork.DomainUsers.GetByIdAsync(memberId);
                if (user == null)
                    throw new ArgumentException($"User with ID {memberId} not found");

                members.Add(user);
            }

            ICollection<Domain.Entities.Task> tasks = new List<Domain.Entities.Task>();
            foreach (var taskId in updateGroupDto.TaskIds ?? new List<Guid>())
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                    throw new ArgumentException($"Task with ID {taskId} not found");
                tasks.Add(task);
            }

            group.Name = updateGroupDto.Name;
            group.Members = members;
            group.Tasks = tasks;

            var updatedGroup = await _unitOfWork.Groups.UpdateAsync(group);
            await _unitOfWork.SaveChangesAsync();

            return new GroupDto
            {
                Id = updatedGroup.Id,
                Name = updatedGroup.Name,
                MemberIds = updatedGroup.Members.Select(member => member.Id).ToList(),
                TaskIds = updatedGroup.Tasks.Select(task => task.Id).ToList()
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
                MemberIds = group.Members.Select(member => member.Id).ToList(),
                TaskIds = group.Members.Select(member => member.Id).ToList()    
            };
        }

        public async System.Threading.Tasks.Task<List<GroupDto>> GetGroupsByMemberIdAsync(Guid memberId)
        {
            var groups = await _unitOfWork.Groups.GetByMemberIdAsync(memberId);
            
            return groups.Select(group => new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                MemberIds = group.Members.Select(member => member.Id).ToList(),
                TaskIds = group.Members.Select(member => member.Id).ToList()
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

            if (!group.Members.Any(m => m.Id == memberId))
            {
                var user = await _unitOfWork.DomainUsers.GetByIdAsync(memberId);
                if (user == null)
                    throw new ArgumentException("User not found");

                group.Members.Add(user);
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

            var memberToRemove = group.Members.FirstOrDefault(m => m.Id == memberId);
            if (memberToRemove != null)
            {
                group.Members.Remove(memberToRemove);
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