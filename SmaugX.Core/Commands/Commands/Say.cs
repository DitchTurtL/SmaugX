using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.Commands;

internal class Say : AbstractBaseCommand
{
    private readonly IRoomService roomService;
    public override string Name => nameof(Say);

    public Say(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public override Task Run()
    {
        var parm = Parameters.FirstOrDefault();
        if (parm == null)
        {
            Client.SendSystemMessage("Say what?");
            return Task.CompletedTask;
        }

        roomService.Say(Client, string.Join(' ', Parameters));

        return Task.CompletedTask;
    }

    public override string[] GetHelp(string[] parameters)
    {
        return new[]
        {
            "SAY",
            "SAY <message>",
            "SAY <message> - Sends a message to everyone in the room."
        };
    }

    public override object Clone()
    {
        return new Say(roomService);
    }
}
