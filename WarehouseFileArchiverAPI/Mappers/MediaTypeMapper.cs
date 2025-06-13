
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class MediaTypeMapper 
    {
        public MediaType MapMediaTypeAddDtoToMediaType(MediaTypeAddRequestDto media)
        {
            MediaType newType = new()
            {
                TypeName = media.TypeName,
                Extension = media.Extension
            };

            return newType;
        }
    }
}