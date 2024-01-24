namespace SmaugX.Core.Data.Characters;

internal enum CharacterCreationState
{
    None = 0,
    WaitingForName = 10,
    WaitingForRace = 20,
    WaitingForClass = 30,
    Loading = 95,
    Loaded = 100,
}
