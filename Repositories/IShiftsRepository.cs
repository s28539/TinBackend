using restAPI.Models;

namespace restAPI.Repositories;

public interface IShiftsRepository
{
    Task<List<Shift>> GetEmployeeShifts(long id, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<Shift>> GetAllShifts(long? employee_id);
    Task<List<Shift>> GetTodaysShifts(long? employee_id);
    Task<bool> AddShift(AddShiftDto addShiftDto);
    Task<bool> ChangeShift(ChangeShiftDto changeShiftDto);
    Task<bool> DeleteShift(long id);
}
