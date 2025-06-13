using System;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<Guid, AuditLog> _auditLogRepository;

        public AuditLogService(IRepository<Guid, AuditLog> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(string entity, Guid entityId, string action, string changedBy, string changes)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = entity,
                EntityId = entityId,
                Action = action,
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow,
                Changes = changes
            };
            await _auditLogRepository.AddAsync(log);
        }
    }
}