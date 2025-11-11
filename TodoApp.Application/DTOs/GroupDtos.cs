using System;
using System.Collections.Generic;

namespace TodoApp.Application.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> MemberIds { get; set; } = new();
        public List<Guid> TaskIds { get; set; } = new();
    }

    public class CreateGroupDto
    {
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class UpdateGroupDto
    {
        public string Name { get; set; }
        public List<Guid>? MemberIds { get; set; }
        public List<Guid>? TaskIds { get; set; }
    }
} 