using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.Commands;

internal class Passwd : AbstractBaseCommand
{
    private readonly IDatabaseService databaseService;

    public override string Name => nameof(Passwd);

    public Passwd(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public override Task Run()
    {
        var parm = Parameters.FirstOrDefault();
        if (parm == null)
        {
            Client.SendSystemMessage("Change your password to what?");
            return Task.CompletedTask;
        }

        var oldPassword = Parameters.FirstOrDefault();
        if (oldPassword == null)
        {
            Client.SendSystemMessage("You must enter your old password.");
            Handled = true;
            return Task.CompletedTask;
        }

        var newPassword = Parameters.ElementAtOrDefault(1);
        var newPasswordConfirm = Parameters.ElementAtOrDefault(2);
        if (newPassword == null || newPasswordConfirm == null)
        {
            Client.SendSystemMessage("You must enter your new password twice.");
            Handled = true;
            return Task.CompletedTask;
        }

        if (newPassword != newPasswordConfirm)
        {
            Client.SendSystemMessage("Your new password and confirmation do not match.");
            Handled = true;
            return Task.CompletedTask;
        }

        if (newPassword.Length < 6)
        {
            Client.SendSystemMessage("Your new password must be at least 6 characters.");
            Handled = true;
            return Task.CompletedTask;
        }

        var tmpUser = databaseService.GetUserForAuth(Client.AuthenticatedUser!.Name, oldPassword);
        if (tmpUser == null)
        {
            Client.SendSystemMessage("Password change failed.");
            Handled = true;
            return Task.CompletedTask;
        }

        var success = databaseService.UpdateUserPassword(tmpUser.Id, newPassword);
        if (success)
            Client.SendSystemMessage("Password changed.");
        else
            Client.SendSystemMessage("Password change failed.");

        Handled = true;
        return Task.CompletedTask;
    }

    public override string[] GetHelp(params string[] parameters)
    {
        return ["Password Help: passwd <old password> <new password> <new password>"];
    }

    public override object Clone()
    {
        return new Passwd(databaseService);
    }

}
