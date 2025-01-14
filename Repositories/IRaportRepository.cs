using restAPI.Models;

namespace restAPI.Repositories;

public interface IRaportRepository
{
     Task<List<Raport>> GetNotCompletedRaports();
     Task<List<Raport>> GetCompletedRaports();
     Task<int> GetMaxIDRaport();
     Task<bool> AddRaport(RaportDto raportDto);
     Task<bool> DeleteRaport(int id);
     Task<bool> ChangeRaport(ChangeRaportDto changeRaportDto);
     Task<List<DateTime>> GetHistoryOfRaport(int raport_id);
}