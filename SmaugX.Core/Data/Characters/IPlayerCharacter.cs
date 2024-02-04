using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Data.Characters;

public interface IPlayerCharacter : ICharacter
{
    Client? Client { get; set; }
    Permissions Permissions { get; set; }
    public bool HasPermission(Permissions builder);
    public void SendStatus();
}
