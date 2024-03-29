﻿using SmaugX.Core.Data.Characters;
using SmaugX.Core.Data.Items;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;

namespace SmaugX.Core.Data.NPCs;

public abstract class AbstractBaseNpc : ICharacter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public abstract string Name { get; set; }
    public virtual int Race { get; set; }
    public virtual int Class { get; set; }
    public virtual int Level { get; set; } = 1;
    public virtual int CurrentRoomId { get; set; }
    public virtual Position Position { get; set; }
    public Room? CurrentRoom { get; set; }
    public virtual List<IItem> Inventory { get; set; } = new List<IItem>();

    public virtual Task Tick()
    {

        return Task.CompletedTask;
    }
}
