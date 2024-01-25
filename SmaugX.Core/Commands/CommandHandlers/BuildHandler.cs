using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.CommandHandlers;

internal class BuildHandler : ICommandHandler
{
    private readonly IRoomService roomService;

    public BuildHandler(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public void HandleCommand(ICommand command)
    {
        // Don't process this if it's not a build command.
        if (command.Name.ToUpper() != "BUILD")
            return;






    }
}
