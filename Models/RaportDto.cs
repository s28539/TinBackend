namespace restAPI.Models;

public class RaportDto
{
    public string Action { get; set; }
    public long Employee_id { get; set; }
    public string Employee_name { get; set; }
    public string Employee_surname { get; set; }
    public long Employee_id_poster { get; set; }
    public int? Shift_id { get; set; }
    public string? StartDate { get; set; }
    public string? StopDate { get; set; }
}