using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class AccessLevelService : IAccessLevelService
    {
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;
        private readonly AccessLevelMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public AccessLevelService(
            IRepository<Guid, AccessLevel> accessLevelRepository,
            IAuditLogService auditLogService)
        {
            _accessLevelRepository = accessLevelRepository;
            _mapper = new();
            _auditLogService = auditLogService;
        }

        public async Task<PaginationDto<AccessLevel>> SearchAccessLevel(SearchQueryDto searchDto)
        {
            var accessLevels = await _accessLevelRepository.GetAllAsync() ?? throw new CollectionEmptyException("No access levels in the Database");


            if (!searchDto.IncludeInactive)
                accessLevels = accessLevels.Where(a => a.IsActive);


            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                accessLevels = accessLevels.Where(a => a.Access.ToLower().Contains(search));
            }


            accessLevels = searchDto.SortBy?.ToLower() switch
            {
                "access" => searchDto.Desc ? accessLevels.OrderByDescending(a => a.Access) : accessLevels.OrderBy(a => a.Access),
                _ => searchDto.Desc ? accessLevels.OrderByDescending(a => a.Id) : accessLevels.OrderBy(a => a.Id)
            };

            var totalRecords = accessLevels.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No Access Matched");

            var items = accessLevels
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            return new PaginationDto<AccessLevel>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        public async Task<AccessLevel> GetAccessLevelById(Guid id)
        {
            var accessLevel = await _accessLevelRepository.GetByIdAsync(id);
            if (accessLevel == null || !accessLevel.IsActive)
                throw new InvalidAccessLevelException($"AccessLevel with id: {id} was not found or deleted");
            return accessLevel;
        }

        public async Task<AccessLevel> AddAccessLevel(AccessLevelAddRequestDto access, string currUser)
        {
            try
            {
                var allAccesses = await _accessLevelRepository.GetAllAsync();
                var currAccess = allAccesses.FirstOrDefault(ac => ac.Access.Equals(access.Access, StringComparison.CurrentCultureIgnoreCase));
                if (currAccess != null)
                {
                    if (currAccess.IsActive)
                        throw new InvalidAccessLevelException($"Access {access.Access} is already present");

                    currAccess.IsActive = true;
                    await _auditLogService.LogAsync(
                        "AccessLevle",
                        currAccess.Id,
                        "Re-Activate",
                        currUser ?? "system",
                        "Access Level already present so re-activated it"
                    );
                    return await _accessLevelRepository.UpdateAsync(currAccess.Id, currAccess);
                }


                currAccess = _mapper.MapAccessLevelAddDtoToAccessLevel(access);
                currAccess.IsActive = true;

                var added = await _accessLevelRepository.AddAsync(currAccess);

                await _auditLogService.LogAsync(
                    "AccessLevel",
                    added.Id,
                    "Add",
                    currUser ?? "system",
                    JsonSerializer.Serialize(access)
                );

                return added;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteAccessLevel(Guid id, string currUser)
        {
            var accessLevel = await _accessLevelRepository.GetByIdAsync(id);
            if (accessLevel == null || !accessLevel.IsActive)
                throw new CollectionEmptyException($"AccessLevel with id: {id} was not found or deleted");

            accessLevel.IsActive = false;
            await _accessLevelRepository.UpdateAsync(accessLevel.Id, accessLevel);

            await _auditLogService.LogAsync(
                "AccessLevel",
                accessLevel.Id,
                "Delete",
                currUser ?? "system",
                $"AccessLevel with id {id} marked as deleted."
            );

            return $"AccessLevel with id {id} deleted.";
        }
    }
}