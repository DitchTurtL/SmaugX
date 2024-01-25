using Serilog;
using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Commands;

internal abstract class AbstractBaseCommand : ICommand
{
    public virtual string Name { get; set; } = "Unknown";
    public virtual string[] Parameters { get; set; } = Array.Empty<string>();
    public virtual bool Handled { get; set; } = false;
    public virtual Client Client { get; set; }

    public AbstractBaseCommand(Client? client = null, string? commandText = null)
    {
        if (client == null || string.IsNullOrEmpty(commandText))
            return;

        // Remove line endings and new lines
        commandText = commandText.ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);

        var commandParts = commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Name = commandParts[0];
        Parameters = commandParts.Skip(1).ToArray();

        Client = client;
    }

    public virtual void Run()
    {
        if (Handled) return;
        Log.Warning($"Command {Name} was not handled.");
        Handled = true;
    }
}
