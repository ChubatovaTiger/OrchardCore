using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Placements
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManagePlacements = new("ManagePlacements", "Manage placements");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[] { ManagePlacements }.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = RoleNames.Administrator,
                    Permissions = new[] { ManagePlacements },
                },
                new PermissionStereotype
                {
                    Name = RoleNames.Editor,
                    Permissions = new[] { ManagePlacements },
                },
            };
        }
    }
}
