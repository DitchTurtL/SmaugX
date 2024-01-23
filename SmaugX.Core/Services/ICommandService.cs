using SmaugX.Core.Commands;

namespace SmaugX.Core.Services;

public interface ICommandService
{
    void HandleCommand(ICommand command);



}
