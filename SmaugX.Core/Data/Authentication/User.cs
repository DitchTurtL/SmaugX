
using SmaugX.Core.Helpers;

namespace SmaugX.Core.Data.Authentication;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Permissions Permissions { get; set; } = Permissions.Player;

    internal bool HasPermission(Permissions builder)
    {
        return PermissionsHelper.HasPermission(Permissions, builder);
    }
}
