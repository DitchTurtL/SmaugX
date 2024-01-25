namespace SmaugX.Core.Commands.Commands;

internal class Build : AbstractBaseCommand
{
    public override string Name => nameof(Build);

    public override Task Run()
    {
        var parm = Parameters.FirstOrDefault();
        if (parm == null)
        {
            Client.SendSystemMessage("Build what?");
            return Task.CompletedTask;
        }

        // Stuff....

        return Task.CompletedTask;
    }

    public override string[] GetHelp(params string[] parameters)
    {
        return """

            #BLUE(*Build Help:*)
            #CYAN(*-------------*)
            Build a new room:
            build room <name> (returns Room ID)

            Build an exit to another room:
            build exit <direction> <room_id>

            Build a one-way exit:
            build exit <direction> <room_id> one-way


            """.Split(Environment.NewLine);
    }

    /// <summary>
    /// Creates a clone of the command object.
    /// This is used to create a new instance of the command for each client that runs it.
    /// Parameters are copied in the CommandService.
    /// Dependencies should be passed to the new command instance here.
    /// </summary>
    public override object Clone()
    {
        return new Build();
    }

}
