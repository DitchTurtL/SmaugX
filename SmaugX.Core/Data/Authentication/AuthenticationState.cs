namespace SmaugX.Core.Data.Authentication;

internal enum AuthenticationState
{
    NotAuthenticated = 0,
    WaitingForEmail = 10,
    WaitingForPassword = 20,
    Authenticated = 30,
}
