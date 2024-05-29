using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OrchardCore.Security.Permissions;
using OrchardCore.Security;
using OrchardCore.Testing.Fakes;
using System.Threading.Tasks;

namespace OrchardCore.Testing.Security;

public static class PermissionHandlerHelper
{
    public static AuthorizationHandlerContext CreateTestAuthorizationHandlerContext(Permission required, string[] allowed = null,
        bool authenticated = false, object resource = null)
    {
        var identity = authenticated
            ? new ClaimsIdentity("Testing")
            : new ClaimsIdentity();

        if (allowed != null)
        {
            foreach (var permissionName in allowed)
            {
                var permission = new Permission(permissionName);
                identity.AddClaim(permission);
            }
        }

        var principal = new ClaimsPrincipal(identity);

        return new AuthorizationHandlerContext(new[] { new PermissionRequirement(required) }, principal, resource);
    }

}
