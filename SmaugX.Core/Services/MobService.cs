using SmaugX.Core.Data.NPCs.Mobs;
using SmaugX.Core.Data.World.Rooms;

namespace SmaugX.Core.Services;

public class MobService : IMobService
{
    private readonly IDatabaseService databaseService;
    private readonly IRoomService roomService;

    private static List<IMob> Mobs = new();

    public MobService(IDatabaseService databaseService, IRoomService roomService)
    {
        this.databaseService = databaseService;
        this.roomService = roomService;
    }

    public Task Tick()
    {
        foreach (var m in Mobs)
            m.Tick();

        return Task.CompletedTask;
    }




}
