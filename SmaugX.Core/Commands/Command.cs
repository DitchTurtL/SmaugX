using SmaugX.Core.Hosting;

namespace SmaugX.Core.Commands;

internal class Command : ICommand
{
    public string Name { get; set; }
    public string[] Parameters { get; set; }
    public bool Handled { get; set; }
    public Client Client { get; set; }

    public Command(Client client, string commandText)
    {
        // Remove line endings and new lines
        commandText = commandText.ReplaceLineEndings().Replace(Environment.NewLine, string.Empty);

        var commandParts = commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Name = commandParts[0];
        Parameters = commandParts.Skip(1).ToArray(); 

        Client = client;
    }
}
