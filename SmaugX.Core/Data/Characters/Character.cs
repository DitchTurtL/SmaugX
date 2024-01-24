﻿using SmaugX.Core.Constants;
using SmaugX.Core.Data.Hosting;
using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmaugX.Core.Data.Characters;

public class Character
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = StringConstants.DEFAULT_CHARACTER_NAME;
    public int Race { get; set; }
    public int Class { get; set; }

    #region Movement
    public int CurrentRoomId { get; set; } = 1;
    public Position Position { get; set; }

    #endregion

    [NotMapped]
    public Room? CurrentRoom { get; set; } = null!;

    [NotMapped]
    public Client? Client { get; set; } = null!;

}
