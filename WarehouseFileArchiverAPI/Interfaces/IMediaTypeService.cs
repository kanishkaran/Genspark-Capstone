using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IMediaTypeService
    {
        Task<MediaType> AddMediaType(MediaTypeAddRequestDto mediaType, string currUser);

        Task<MediaType> GetMediaTypeByContentType(string contentType);

        Task<PaginationDto<MediaType>> SearchMediaTypes(SearchQueryDto searchDto);
        Task<MediaType> GetById(Guid id);
        Task<MediaType> UpdateMediaType(Guid id, MediaTypeAddRequestDto mediaTypeDto,string currUser);
        Task<bool> DeleteMediaType(Guid id, string currUser);
    }
}