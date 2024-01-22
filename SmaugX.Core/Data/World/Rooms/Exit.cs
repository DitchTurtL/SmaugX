namespace SmaugX.Core.Data.World.Rooms;

internal class Exit
{
    public int Id { get; set; }
    public Direction Direction { get; set; }
    public int RoomId { get; set; }
    public int DestinationRoomId { get; set; }
    #region Reference Ids
    public string ReferenceId { get; set; } = string.Empty;
    public string RoomReferenceId { get; set; } = string.Empty;
    public string DestinationRoomReferenceId { get; set; } = string.Empty;
    #endregion
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;

}
