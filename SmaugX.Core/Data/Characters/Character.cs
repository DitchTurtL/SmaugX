using SmaugX.Core.Constants;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;
using SmaugX.Core.Services;
using System.Text;

namespace SmaugX.Core.Data.Characters;

public class Character
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = StringConstants.DEFAULT_CHARACTER_NAME;
    public int Race { get; set; }
    public int Class { get; set; }

    #region Movement
    public int CurrentRoomId { get; set; } = 1;
    public Position Position { get; set; }

    #endregion

    private Room? CurrentRoom { get; set; }

    public Client? Client { get; set; } = null!;


    internal async Task ShowStatus()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are {StringConstants.GetPosition(Position)}");
        sb.AppendLine($" in {CurrentRoom?.Name ?? StringConstants.DEFAULT_CHARACTER_NAME}.");
        await Client!.SendLine(sb.ToString(), Helpers.MessageColor.Status);

    }
}
