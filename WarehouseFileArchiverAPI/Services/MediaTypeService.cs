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
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IRepository<Guid, MediaType> _mediaTypeRepository;
        private readonly MediaTypeMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public MediaTypeService(
            IRepository<Guid, MediaType> mediaTypeRepository,
            IAuditLogService auditLogService)
        {
            _mediaTypeRepository = mediaTypeRepository;
            _mapper = new();
            _auditLogService = auditLogService;
        }

        public async Task<PaginationDto<MediaType>> SearchMediaTypes(SearchQueryDto searchDto)
        {
            var mediaTypes = await _mediaTypeRepository.GetAllAsync() ?? throw new CollectionEmptyException("No media types in the Database");

           
            if (!searchDto.IncludeInactive)
                mediaTypes = mediaTypes.Where(m => !m.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
                mediaTypes = mediaTypes.Where(m => m.TypeName.Contains(searchDto.Search) || m.Extension.Contains(searchDto.Search));

            mediaTypes = searchDto.SortBy?.ToLower() switch
            {
                "extension" => searchDto.Desc ? mediaTypes.OrderByDescending(m => m.Extension) : mediaTypes.OrderBy(m => m.Extension),
                _ => searchDto.Desc ? mediaTypes.OrderByDescending(m => m.TypeName) : mediaTypes.OrderBy(m => m.TypeName)
            };

            var totalRecords = mediaTypes.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No Media Typed Matched");
            var items = mediaTypes
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            return new PaginationDto<MediaType>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        public async Task<MediaType> GetById(Guid id)
        {
            var mediaType = await _mediaTypeRepository.GetByIdAsync(id);
            if (mediaType == null || mediaType.IsDeleted)
                throw new CollectionEmptyException($"MediaType with id: {id} was not found");
            return mediaType;
        }

        public async Task<MediaType> AddMediaType(MediaTypeAddRequestDto mediaType, string currUser)
        {
            try
            {
                var types = await _mediaTypeRepository.GetAllAsync();
                var exist = types.FirstOrDefault(media =>  media.TypeName == mediaType.TypeName && media.Extension == mediaType.Extension);

                if (exist != null)
                {
                    if (!exist.IsDeleted)
                        throw new InvalidMediaException($"The mediatype with name: {mediaType.TypeName} and extension: {mediaType.Extension} already exists");

                    exist.IsDeleted = false;
                    await _mediaTypeRepository.UpdateAsync(exist.Id, exist);

                    await _auditLogService.LogAsync(
                        "MediaType",
                        exist.Id,
                        "Re-Activate",
                        currUser,
                        "Changed status from deleted to not deleted"
                    );
                    return exist;
                }
                    

                var newType = _mapper.MapMediaTypeAddDtoToMediaType(mediaType);
                newType.IsDeleted = false;

                var added = await _mediaTypeRepository.AddAsync(newType);

                await _auditLogService.LogAsync(
                    "MediaType",
                    added.Id,
                    "Add",
                    currUser,
                    JsonSerializer.Serialize(mediaType)
                );

                return added;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MediaType> UpdateMediaType(Guid id, MediaTypeAddRequestDto mediaTypeDto, string currUser)
        {
            var mediaType = await _mediaTypeRepository.GetByIdAsync(id);
            if (mediaType == null || mediaType.IsDeleted)
                throw new CollectionEmptyException($"MediaType with id: {id} was not found");

            var types = await _mediaTypeRepository.GetAllAsync();
            var exist = types.Any(mt =>
                mt.Id != id &&
                !mt.IsDeleted &&
                mt.TypeName == mediaTypeDto.TypeName &&
                mt.Extension == mediaTypeDto.Extension);

            if (exist)
                throw new InvalidMediaException($"The mediatype with name: {mediaTypeDto.TypeName} and extension: {mediaTypeDto.Extension} already exists");

            var oldMediaType = JsonSerializer.Serialize(mediaType);

            mediaType.TypeName = mediaTypeDto.TypeName;
            mediaType.Extension = mediaTypeDto.Extension;

            var updated = await _mediaTypeRepository.UpdateAsync(id, mediaType);

            await _auditLogService.LogAsync(
                "MediaType",
                updated.Id,
                "Update",
                currUser,
                $"Old: {oldMediaType}, New: {JsonSerializer.Serialize(mediaTypeDto)}"
            );

            return updated;
        }

        public async Task<MediaType> GetMediaTypeByContentType(string contentType)
        {
            var mediaTypes = await _mediaTypeRepository.GetAllAsync();
            if (mediaTypes.ToList().Count == 0)
                throw new CollectionEmptyException("There are no Media Types in Database");

            var mediaType = mediaTypes.FirstOrDefault(mt => !mt.IsDeleted && mt.TypeName == contentType)
                ?? throw new InvalidMediaException($"The Type : {contentType} is not supported");

            return mediaType;
        }

        public async Task<bool> DeleteMediaType(Guid id, string currUser)
        {
            var mediaType = await _mediaTypeRepository.GetByIdAsync(id);
            if (mediaType == null || mediaType.IsDeleted)
                throw new CollectionEmptyException($"MediaType with id: {id} was not found");

            mediaType.IsDeleted = true;
            await _mediaTypeRepository.UpdateAsync(id, mediaType);

            await _auditLogService.LogAsync(
                "MediaType",
                id,
                "Delete",
                currUser,
                $"MediaType with id {id} marked as deleted."
            );

            return true;
        }
    }
}