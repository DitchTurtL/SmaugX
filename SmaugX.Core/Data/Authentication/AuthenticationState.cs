namespace SmaugX.Core.Data.Authentication;

internal enum AuthenticationState
{
    NotAuthenticated = 0,
    CreatingNewUser = 5,
    CreatingNewPassword = 6,
    CreatingNewPasswordConfirmation = 7,
    WaitingForEmail = 10,
    WaitingForPassword = 20,
    Authenticated = 30,
}
