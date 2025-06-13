using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WarehouseFileArchiverAPI.Policies
{
    public class MinimumAccessLevel : IAuthorizationRequirement
    {
        public MinimumAccessLevel(string accessLevel)
        {
            MinAccessLevel = accessLevel;
        }

        public string MinAccessLevel { get; set; } = string.Empty;
        
    }
}