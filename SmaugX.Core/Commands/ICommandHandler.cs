namespace SmaugX.Core.Commands;

interface ICommandHandler
{
    Task HandleCommand(ICommand command);



}
