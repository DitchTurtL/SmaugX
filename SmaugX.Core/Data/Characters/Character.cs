using SmaugX.Core.Constants;
using SmaugX.Core.Data.Authentication;
using SmaugX.Core.Helpers;
using SmaugX.Core.Services;
using System.Text;

namespace SmaugX.Core.Data.Characters;

public class Character : AbstractBasePlayerCharacter
{
    public IRoomService roomService;

    public override string Name { get; set; } = StringConstants.DEFAULT_CHARACTER_NAME;

    public override Task Tick()
    {

        return base.Tick();
    }

    public override bool HasPermission(Permissions builder)
    {
        return PermissionsHelper.HasPermission(Permissions, builder);
    }

    /// <summary>
    /// Sends the character's status to the client.
    /// "You are standing in the middle of a forest."
    /// "You are floating in The Void."
    /// </summary>
    public override void SendStatus()
    {
        var currentRoom = CurrentRoom ??= roomService.GetRoomById(CurrentRoomId);

        var sb = new StringBuilder();
        sb.AppendLine($"You are {StringConstants.GetPosition(Position)}");
        sb.AppendLine($" in {currentRoom.Name}.");
        Client!.SendLine(sb.ToString(), Helpers.MessageColor.Status);

    }
}
