using CardiacPatientMonitoring.Models;

namespace CardiacPatientMonitoring.Repositories;

public interface IVitalSignRepository
{
    Task<List<VitalSign>> GetAllAsync();
    Task<VitalSign> GetByIdAsync(int id);
    Task AddAsync(VitalSign vitalSign);
}
