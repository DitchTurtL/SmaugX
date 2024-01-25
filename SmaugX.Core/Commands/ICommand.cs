using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Commands;

public interface ICommand : ICloneable
{
    string Name { get; set; }
    string[] Parameters { get; set; }
    bool Handled { get; set; }
    Client Client { get; set; }
    Task Run();
    string[] GetHelp(params string[] parameters);
}
