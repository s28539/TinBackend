using Microsoft.AspNetCore.Mvc;
using restAPI.Models;

namespace restAPI.Repositories;

public interface IReportsRepository
{
    Task<double> GetWorkedHours(long employeeid, DateTime startTime, DateTime stopTime);
}