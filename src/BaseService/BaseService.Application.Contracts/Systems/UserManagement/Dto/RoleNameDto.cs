using System;

namespace BaseService.Systems.UserManagement.Dto
{
    public class RoleNameDto
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        protected RoleNameDto() { }

        public RoleNameDto(Guid roleid, string name)
        {
            RoleId = roleid;
            Name = name;
        }
    }
}
