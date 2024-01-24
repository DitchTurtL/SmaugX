using SmaugX.Core.Data.World.Rooms;
using SmaugX.Core.Services;

namespace SmaugX.Core.Commands.CommandHandlers;

internal class MovementHandler : ICommandHandler
{
    private readonly IRoomService roomService;

    internal MovementHandler(IRoomService roomService)
    {
        this.roomService = roomService;
    }

    public void HandleCommand(ICommand command)
    {
        var direction = GetDirection(command.Name);
        
        // This wasn't a direction command.
        if (direction == Direction.None)
            return;

        HandleMovement(command, direction);
    }

    private void HandleMovement(ICommand command, Direction direction)
    {
        command.Handled = true;
        roomService.MoveCharacter(command.Client.Character, direction);
    }

    /// <summary>
    /// Returns a Direction enum based on the direction string.
    /// </summary>
    private Direction GetDirection(string direction)
    {
        return direction.ToUpper() switch
        {
            "N" or "NORTH" => Direction.North,
            "S" or "SOUTH" => Direction.South,
            "E" or "EAST" => Direction.East,
            "W" or "WEST" => Direction.West,
            "U" or "UP" => Direction.Up,
            "D" or "DOWN" => Direction.Down,
            "NE" or "NORTHEAST" => Direction.NorthEast,
            "NW" or "NORTHWEST" => Direction.NorthWest,
            "SE" or "SOUTHEAST" => Direction.SouthEast,
            "SW" or "SOUTHWEST" => Direction.SouthWest,
            _ => Direction.None,
        };
    }
}
