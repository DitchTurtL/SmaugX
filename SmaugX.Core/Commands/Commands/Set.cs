using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.Commands;

internal class Set : AbstractBaseCommand
{
    private readonly IRoomService roomService;

    public override string Name => nameof(Set);

    public Set(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public override Task Run()
    {
        var parm = Parameters.FirstOrDefault();
        if (parm == null)
        {
            Client.SendSystemMessage("Set what?");
            return Task.CompletedTask;
        }

        switch (parm.ToUpper())
        {
            case "ROOM-NAME":
                var roomName = string.Join(' ', Parameters[1..]);
                if (string.IsNullOrEmpty(roomName))
                {
                    Client.SendSystemMessage("Set the room name to what?");
                    return Task.CompletedTask;
                }

                var nameSet = roomService.SetRoomName(Client, roomName);
                if (nameSet)
                    Client.SendSystemMessage($"Room name set to: {roomName}");
                else
                    Client.SendSystemMessage($"Room name not set.");
                
                break;

            case "ROOM-DESCRIPTION":
                var roomDescription = string.Join(' ', Parameters[1..]);
                if (string.IsNullOrEmpty(roomDescription))
                {
                    Client.SendSystemMessage("Set the room description to what?");
                    return Task.CompletedTask;
                }

                var descSet = roomService.SetRoomDescription(Client, roomDescription);
                if (descSet)
                    Client.SendSystemMessage($"Room description set to: {roomDescription}");
                else
                    Client.SendSystemMessage($"Room description not set.");

                break;

            default:
                Client.SendSystemMessage($"Set what?");
                break;

        }

        return Task.CompletedTask;
    }

    public override string[] GetHelp(params string[] parameters)
    {
        return """

            #BLUE(*Set Help:*)
            #CYAN(*-------------*)
            Set the Name of the current room:
            set room-name <room name>
            
            Set the Description of the current room:
            set room-description <room description>



            """.Split(Environment.NewLine);
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }
}
