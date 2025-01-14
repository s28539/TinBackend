using System.Runtime.InteropServices.JavaScript;

namespace restAPI.Models;

public class ReportRequest
{
    public string EmployeeID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}