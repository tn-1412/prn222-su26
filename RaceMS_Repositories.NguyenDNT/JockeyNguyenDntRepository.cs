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
            return await _context.JockeyNguyenDnts.ToListAsync();
        }

        public new async Task<JockeyNguyenDnt> GetByIdAsync(int id)
        {
            return await _context.JockeyNguyenDnts.FindAsync(id);
        }

        public new async Task<bool> DeleteAsync(int id)
        {
            var jockey = await _context.JockeyNguyenDnts.FindAsync(id);
            if (jockey == null) return false;

            var hasRegistrations = await _context.RegistrationNguyenDnts
                .AnyAsync(r => r.JockeyNguyenDntid == id);
            if (hasRegistrations)
                throw new InvalidOperationException(":Không thể xóa jockey này vì đang có đăng ký liên kết.");

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
