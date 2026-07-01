using Microsoft.EntityFrameworkCore;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT.DBContext;

namespace RaceMS_Repositories.NguyenDNT
{
    public class JockeyNguyenDntRepository : Base.GenericRepository<JockeyNguyenDnt>
    {
        public JockeyNguyenDntRepository() { }
        public JockeyNguyenDntRepository(RaceManagementDBContext context) => _context = context;

        public new async Task<List<JockeyNguyenDnt>> GetAllAsync()
        {
            return await _context.JockeyNguyenDnts
                .OrderByDescending(j => j.JockeyNguyenDntid)
                .ToListAsync();
        }

        public new async Task<JockeyNguyenDnt?> GetByIdAsync(int id)
        {
            return await _context.JockeyNguyenDnts.FindAsync(id);
        }

        // Tìm theo 3 trường độc lập: FullName / Email / LicenseCode (AND giữa các ô có nhập)
        public async Task<List<JockeyNguyenDnt>> SearchAsync(string? fullName, string? email, string? licenseCode)
        {
            var query = _context.JockeyNguyenDnts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(j => j.FullName != null && j.FullName.Contains(fullName));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(j => j.Email != null && j.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(licenseCode))
                query = query.Where(j => j.LicenseCode != null && j.LicenseCode.Contains(licenseCode));

            return await query
                .OrderByDescending(j => j.JockeyNguyenDntid)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var jockey = await _context.JockeyNguyenDnts.FindAsync(id);
            if (jockey == null) return false;

            var registrations = _context.RegistrationNguyenDnts
                .Where(r => r.JockeyNguyenDntid == id);
            _context.RegistrationNguyenDnts.RemoveRange(registrations);

            _context.JockeyNguyenDnts.Remove(jockey);
            return await _context.SaveChangesAsync() > 0;
        }

        // excludeId: bỏ qua chính bản ghi đang update khi kiểm tra trùng
        public async Task<bool> LicenseCodeExistsAsync(string licenseCode, int excludeId = 0)
        {
            return await _context.JockeyNguyenDnts
                .AnyAsync(j => j.LicenseCode == licenseCode && j.JockeyNguyenDntid != excludeId);
        }

        public async Task<bool> EmailExistsAsync(string email, int excludeId = 0)
        {
            return await _context.JockeyNguyenDnts
                .AnyAsync(j => j.Email == email && j.JockeyNguyenDntid != excludeId);
        }

        public async Task<bool> PhoneExistsAsync(string phone, int excludeId = 0)
        {
            return await _context.JockeyNguyenDnts
                .AnyAsync(j => j.PhoneNumber == phone && j.JockeyNguyenDntid != excludeId);
        }
    }
}
