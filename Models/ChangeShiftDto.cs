namespace restAPI.Models;

public class ChangeShiftDto
{
    public int ShiftID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime StopTime { get; set; }
}