using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.Commands;

internal class Build : AbstractBaseCommand
{
    private readonly IRoomService roomService;

    public override string Name => nameof(Build);

    public Build(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public override Task Run()
    {
        var parm = Parameters.FirstOrDefault();
        if (parm == null)
        {
            Client.SendSystemMessage("Build what?");
            return Task.CompletedTask;
        }

        int roomId = 0;
        switch (parm.ToUpper())
        {
            case "ROOM":
                var roomName = string.Join(' ', Parameters[1..]);
                if (string.IsNullOrEmpty(roomName))
                {
                    Client.SendSystemMessage("Build a room with what name?");
                    return Task.CompletedTask;
                }

                roomId = roomService.CreateRoom(Client, roomName);
                Client.SendSystemMessage($"Created room: {roomName} with Id ({roomId})");
                break;

            case "TELEPORT":
                Int32.TryParse(Parameters.ElementAtOrDefault(1), out roomId);
                if (roomId == 0)
                {
                    Client.SendSystemMessage("Teleport to what room?");
                    return Task.CompletedTask;
                }

                roomService.Teleport(Client, roomId);

                break;
            case "EXIT":

                Int32.TryParse(Parameters.ElementAtOrDefault(1), out roomId);
                if (roomId == 0)
                {
                    Client.SendSystemMessage("Build an exit to what room?");
                    return Task.CompletedTask;
                }

                var direction = Parameters.ElementAtOrDefault(2);
                if (direction == null)
                {
                    Client.SendSystemMessage("Build an exit in what direction?");
                    return Task.CompletedTask;
                }

                var oneWay = Parameters.ElementAtOrDefault(3) == "one-way";

                var exitCreated = roomService.CreateExit(Client, direction, roomId, oneWay);
                if (exitCreated)
                    Client.SendSystemMessage($"Created exit: {direction} to {roomId} {(oneWay ? "(one-way)" : "")}");
                else
                    Client.SendSystemMessage($"Failed to create exit: {direction} to {roomId} {(oneWay ? "(one-way)" : "")}");
                
                break;

            default:
                Client.SendSystemMessage("Build what?");
                break;
        }

        return Task.CompletedTask;
    }

    public override string[] GetHelp(params string[] parameters)
    {
        return """

            #BLUE(*Build Help:*)
            #CYAN(*-------------*)
            Build a new room:
            build room <name> (returns Room ID)

            Teleport to a room:
            build teleport <room_id>

            Build an exit to another room:
            build exit <room_id> <direction>

            Set the Name or Description of a room:
            See 'Set' command

            """.Split(Environment.NewLine);
    }

    /// <summary>
    /// Creates a clone of the command object.
    /// This is used to create a new instance of the command for each client that runs it.
    /// Parameters are copied in the CommandService.
    /// Dependencies should be passed to the new command instance here.
    /// </summary>
    public override object Clone()
    {
        return new Build(roomService);
    }

}
