using Serilog;
using SmaugX.Core.Constants;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;
using System.Text;

namespace SmaugX.Core.Services;

public class RoomService : IRoomService
{
    private readonly IDatabaseService databaseService;

    private static List<Room> Rooms = new();

    public RoomService(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public void CharacterJoined(Character character)
    {


    }

    public void MoveCharacter(Character character, Direction direction)
    {
        // Get the current room.
        // If the current room is null, get the room from the database and populate cache.
        var currentRoom = character.CurrentRoom ??= GetRoomById(character.CurrentRoomId);

        // Get the exits for the current room.
        // If the exits are null, get the exits from the database and populate cache.
        var exits = currentRoom.Exits ??= GetExitsByRoomId(character.CurrentRoomId)!;
        
        // Get the exit for the direction the character is trying to move.
        var exit = exits.FirstOrDefault(e => e?.Direction == direction);
        
        // If the exit doesn't exist, let the character know.
        if (exit == null)
        {
            character.Client!.SendSystemMessage(StringConstants.BAD_MOVE_DIRECTION);
            return;
        }

        // Get the room the character is trying to move to.
        var room = GetRoomById(exit.RoomId);
        if (room == null)
        {
            Log.Warning("Room with id {id} not found in database.", exit.RoomId);
            character.Client!.SendSystemMessage(StringConstants.BAD_MOVE_DIRECTION);
            return;
        }

        // Update the character's current room.
        character.CurrentRoom = room;
        character.CurrentRoomId = room.Id;

        SendCharacterStatus(character);
    }

    public Room GetRoomById(int id)
    {
        // get room from cache
        var room = Rooms.FirstOrDefault(r => r.Id == id);

        // if not in cache, get from database
        if (room == null)
            room = databaseService.GetRoomById(id);

        // If not in database, log error and return void room
        if (room == null)
        {
            Log.Warning("Room with id {id} not found in database.", id);
            // This will always return a room,
            // The Void should always be loaded into this cache.
            return Rooms[0];
        }

        Rooms.Add(room);

        return room;
    }

    public void SendCharacterStatus(Character character)
    {
        var currentRoom = character.CurrentRoom ??= GetRoomById(character.CurrentRoomId);

        var sb = new StringBuilder();
        sb.AppendLine($"You are {StringConstants.GetPosition(character.Position)}");
        sb.AppendLine($" in {currentRoom.Name}.");
        character.Client!.SendLine(sb.ToString(), Helpers.MessageColor.Status);

    }

    public List<Exit> GetExitsByRoomId(int id)
    {
        var room = GetRoomById(id);
        return (room.Exits ??= databaseService.GetExitsByRoomId(id)!)!;
    }
}
