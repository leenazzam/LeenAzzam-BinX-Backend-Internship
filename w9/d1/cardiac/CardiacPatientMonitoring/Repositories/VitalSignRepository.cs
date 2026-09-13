using Microsoft.EntityFrameworkCore;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;

namespace CardiacPatientMonitoring.Repositories;

public class VitalSignRepository : IVitalSignRepository
{
    private readonly AppDbContext _context;

    public VitalSignRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VitalSign>> GetAllAsync()
    {
        return await _context.VitalSigns.ToListAsync();
    }

    public async Task<VitalSign> GetByIdAsync(int id)
    {
        return await _context.VitalSigns.FindAsync(id);
    }

    public async Task AddAsync(VitalSign vitalSign)
    {
        _context.VitalSigns.Add(vitalSign);
        await _context.SaveChangesAsync();
    }
}
