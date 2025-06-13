using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string entity, Guid entityId, string action, string changedBy, string changes);
    }
}