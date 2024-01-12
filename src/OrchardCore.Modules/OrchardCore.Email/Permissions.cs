using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Infrastructure.Security;
using OrchardCore.Security.Permissions;

namespace OrchardCore.Email
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageEmailSettings = new("ManageEmailSettings", "Manage Email Settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[]
            {
                ManageEmailSettings,
            }
            .AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = RoleNames.Administrator,
                    Permissions = new[] { ManageEmailSettings },
                },
            };
        }
    }
}
