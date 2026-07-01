using Microsoft.EntityFrameworkCore;
using RaceMS_Repositories.NguyenDNT.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Repositories.NguyenDNT
{
    public class RegistrationNguyenDntRepository : Base.GenericRepository<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>
    {
        public RegistrationNguyenDntRepository() { }
        public RegistrationNguyenDntRepository(RaceManagementDBContext context) => _context = context;

        public async Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> GetAllAsync(int id)
        {
            return await _context.RegistrationNguyenDnts.Where(c => c.JockeyNguyenDntid == id).ToListAsync();
        }
        public new async Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> GetByIdAsync(int id)
        {
            return await _context.RegistrationNguyenDnts.Where(c => c.RegistrationNguyenDntid == id).ToListAsync();
        }
        // Tìm theo 3 trường độc lập: RegistrationNguyenDntid (mã) / PrizeMoney (tiền thưởng) / HorseName (tên ngựa), AND giữa các ô có nhập
        public async Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> SearchAsync(int? code, decimal? amount, string? name)
        {
            var query = _context.RegistrationNguyenDnts.AsQueryable();

            if (code.HasValue)
                query = query.Where(c => c.RegistrationNguyenDntid == code.Value);

            if (amount.HasValue)
                query = query.Where(c => c.PrizeMoney == amount.Value);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.HorseName != null && c.HorseName.Contains(name));

            return await query
                .OrderByDescending(c => c.RegistrationNguyenDntid)
                .ToListAsync();
        }
        //public async Task<int> CreateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration)
        //{
        //    _context.RegistrationNguyenDnts.Add(registration);
        //    return await _context.SaveChangesAsync();
        //}
        //public async Task<int> UpdateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration)
        //{
        //    _context.RegistrationNguyenDnts.Update(registration);
        //    return await _context.SaveChangesAsync();
        //}
        public async Task<bool> DeleteAsync(int id)
        {
            var registration = await _context.RegistrationNguyenDnts.FindAsync(id);
            if (registration == null) return false;
            _context.RegistrationNguyenDnts.Remove(registration);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> JockeyExistsAsync(int jockeyId)
        {
            return await _context.JockeyNguyenDnts.AnyAsync(j => j.JockeyNguyenDntid == jockeyId);
        }
    }
}
