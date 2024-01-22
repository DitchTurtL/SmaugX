using Serilog;
using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

internal static class RoomService
{
    private static List<Room> RoomCache = new();

    internal static async Task CharacterJoined(Character character)
    {


    }

    internal static Room GetRoomById(int id)
    {
        // get room from cache
        var room = RoomCache.FirstOrDefault(r => r.Id == id);

        // if not in cache, get from database
        if (room == null)
            room = DatabaseService.GetRoomById(id);

        // If not in database, log error and return void room
        if (room == null)
        {
            Log.Warning("Room with id {id} not found in database.", id);
            // This will always return a room,
            // The Void should always be loaded into this cache.
            return RoomCache[0];
        }
            
        RoomCache.Add(room);

        return room;
    }

    internal static Room? GetRoomForCache(int currentRoomId)
    {
        throw new NotImplementedException();
    }
}
