namespace SmaugX.Core.Commands;

public interface ICommandHandler
{
    void HandleCommand(ICommand command);
}
