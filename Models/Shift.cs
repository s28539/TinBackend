using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;

namespace restAPI.Models;

public class Shift
{
    public long EmployeeID { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
    public long ShiftID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
}