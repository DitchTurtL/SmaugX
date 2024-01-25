namespace SmaugX.Core.Data.Authentication;

[Flags]
public enum Permissions
{
    None = 0,
    Player = 1,
    Builder = 64,
    Oper = 256,
    Admin = 2048,
    All = Player | Builder | Oper | Admin
}
