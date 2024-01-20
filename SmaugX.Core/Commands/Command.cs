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
        var commandParts = commandText.Split(' ');

        Name = commandParts[0].ToUpper();
        Parameters = commandParts.Skip(1).ToArray(); 

        Client = client;
    }
}
