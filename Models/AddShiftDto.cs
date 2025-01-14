namespace restAPI.Models;

public class AddShiftDto
{
    public long Employee_id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
}