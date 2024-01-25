using SmaugX.Core.Data.Authentication;

namespace SmaugX.Core.Helpers;

internal class PermissionsHelper
{
    public static bool HasPermission(Permissions permissions, Permissions permission)
    {
        return (permissions & permission) == permission;
    }
}
