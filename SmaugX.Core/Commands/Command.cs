using SmaugX.Core.Data.Hosting;

namespace SmaugX.Core.Commands;

internal class Command : AbstractBaseCommand
{
    public Command(Client client, string commandText) : base(client, commandText)
    {
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }
}
