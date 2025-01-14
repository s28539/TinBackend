namespace restAPI.Models;

public class ChangeRaportDto
{
    public long EmployeeIDPoster { get; set; }
    public int RaportID { get; set; }
    public string NewStatus { get; set; }
}