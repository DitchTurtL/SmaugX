using SmaugX.Core.Services;
using System.Text;

namespace SmaugX.Core.Commands.Commands;

internal class Look : AbstractBaseCommand
{
    private readonly IRoomService roomService;

    public override string Name => nameof(Look);

    public Look(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public override Task Run()
    {
        var room = Client.Character!.CurrentRoom;
        if (room == null)
        {
            Client.SendSystemMessage("You are in the void.");
            return Task.CompletedTask;
        }

        // Get list of players in this room
        var standingHere = new StringBuilder();
        foreach (var c in room.Characters)
        {
            if (c == Client.Character)
                continue;

            standingHere.Append($"{c.Name} is {c.Position} here." + Environment.NewLine);
        }

        // Get list of exits
        var exits = roomService.GetExitsByRoomId(room.Id);
        string exitsString = string.Empty;
        if (!exits.Any())
            exitsString = "#YELLOW(*None*)";
        else
            exitsString = $"#GREEN(*{string.Join(", ", exits.Select(e => e.Direction.ToString()))}*)";
        
        // Build room description
        var roomDesc = $"""
            {new string('-', room.Name.Length + 1)}
            {room.Name}
            {new string('-', room.Name.Length + 1)}
            {room.ShortDescription}

            {standingHere}
            Exits: {exitsString}

            """.Replace("\\n", Environment.NewLine);

        Client.SendLines(roomDesc.Split(Environment.NewLine));

        return Task.CompletedTask;
    }


    public override object Clone()
    {
        return new Look(roomService);
    }
}
