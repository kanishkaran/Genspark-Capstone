using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly IAuditLogService _auditLogService;

        public RoleService(
            IRepository<Guid, AccessLevel> accessLevelRepository,
            IRepository<Guid, Role> roleRepository,
            IAuditLogService auditLogService)
        {
            _accessLevelRepository = accessLevelRepository;
            _roleRepository = roleRepository;
            _auditLogService = auditLogService;
        }

        public async Task<PaginationDto<Role>> SearchRoles(SearchQueryDto searchDto)
        {
            var roles = await _roleRepository.GetAllAsync() ?? throw new Exception("No roles in the Database");

            
            if (!searchDto.IncludeInactive)
                roles = roles.Where(r => r.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                roles = roles.Where(r => r.RoleName.ToLower().Contains(search));
            }


            roles = searchDto.SortBy?.ToLower() switch
            {
                "rolename" => searchDto.Desc ? roles.OrderByDescending(r => r.RoleName) : roles.OrderBy(r => r.RoleName),
                _ => searchDto.Desc ? roles.OrderByDescending(r => r.Id) : roles.OrderBy(r => r.Id)
            };

            var totalRecords = roles.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No Roles Matched");

            var items = roles
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            return new PaginationDto<Role>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        public async Task<Role> GetById(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null || role.IsDeleted)
                throw new Exception($"Role with id: {id} was not found or deleted");

            return role;
        }

        public async Task<Role> AddRole(RoleAddRequestDto roleDto, string changedBy = "system")
        {
            if (roleDto.AccessLevel.Equals("admin", StringComparison.CurrentCultureIgnoreCase) &&
                !roleDto.Role.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidAccessLevelException($"Cannot Give Admin Access to {roleDto.Role}");

            var allAccesses = await _accessLevelRepository.GetAllAsync();
            var access = allAccesses.FirstOrDefault(ac => ac.Access.ToLower() == roleDto.AccessLevel.ToLower())
                ?? throw new InvalidAccessLevelException($"The access Level {roleDto.AccessLevel} is Invalid");

            var allRoles = await _roleRepository.GetAllAsync();
            var currRole = allRoles.FirstOrDefault(r => r.RoleName.ToLower() == roleDto.Role.ToLower() && !r.IsDeleted);
            if (currRole != null)
            {
                if (!currRole.IsDeleted)
                    throw new Exception($"The Role : {roleDto.Role} already Exists");

                currRole.IsDeleted = false;
                await _roleRepository.UpdateAsync(currRole.Id, currRole);

                await _auditLogService.LogAsync(
                    "Role",
                    currRole.Id,
                    "Re-Activate",
                    changedBy,
                    "Changed status to not deletd"
                );

                return currRole;
            }
                

            currRole = new Role
            {
                RoleName = roleDto.Role,
                AccessLevelId = access.Id,
                IsDeleted = false
            };

            var added = await _roleRepository.AddAsync(currRole);

            await _auditLogService.LogAsync(
                "Role",
                added.Id,
                "Add",
                changedBy,
                JsonSerializer.Serialize(roleDto)
            );

            return added;
        }

        public async Task<Role> UpdateRole(Guid id, RoleAddRequestDto roleDto, string changedBy = "system")
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null || role.IsDeleted)
                throw new Exception($"Role with id: {id} was not found or deleted");

            var allAccesses = await _accessLevelRepository.GetAllAsync();
            var access = allAccesses.FirstOrDefault(ac => ac.Access.ToLower() == roleDto.AccessLevel.ToLower())
                ?? throw new InvalidAccessLevelException($"The access Level {roleDto.AccessLevel} is Invalid");

            var oldRole = new { role.RoleName, role.AccessLevelId };

            role.RoleName = roleDto.Role;
            role.AccessLevelId = access.Id;

            var updated = await _roleRepository.UpdateAsync(id, role);

            await _auditLogService.LogAsync(
                "Role",
                updated.Id,
                "Update",
                changedBy,
                $"Old: {JsonSerializer.Serialize(oldRole)}, New: {JsonSerializer.Serialize(roleDto)}"
            );

            return updated;
        }

        public async Task<bool> DeleteRole(Guid id, string changedBy = "system")
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null || role.IsDeleted)
                throw new Exception($"Role with id: {id} was not found or already deleted");

            role.IsDeleted = true;
            await _roleRepository.UpdateAsync(id, role);

            await _auditLogService.LogAsync(
                "Role",
                role.Id,
                "Delete",
                changedBy,
                $"Role {role.RoleName} marked as deleted."
            );

            return true;
        }
    }
}