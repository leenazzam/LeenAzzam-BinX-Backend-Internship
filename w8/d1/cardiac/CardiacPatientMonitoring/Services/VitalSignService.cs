using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.Repositories;

namespace CardiacPatientMonitoring.Services;

public class VitalSignService
{
    private readonly IVitalSignRepository _repository;

    public VitalSignService(IVitalSignRepository repository)
    {
        _repository = repository;
    }

    public bool IsCritical(VitalSign vitalSign)
    {
        if (vitalSign.HeartRate > 150 || vitalSign.HeartRate < 40)
        {
            return true;
        }

        if (vitalSign.OxygenLevel < 90)
        {
            return true;
        }

        return false;
    }

    public async Task<List<VitalSign>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task AddAsync(VitalSign vitalSign)
    {
        await _repository.AddAsync(vitalSign);
    }
}
