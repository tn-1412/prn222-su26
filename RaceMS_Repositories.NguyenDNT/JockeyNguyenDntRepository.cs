using Microsoft.EntityFrameworkCore;
using RaceMS_Repositories.NguyenDNT.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Repositories.NguyenDNT
{
    public class JockeyNguyenDntRepository : Base.GenericRepository<RaceMS.Entities.NguyenDNT.Models.JockeyNguyenDnt>
    {
        public JockeyNguyenDntRepository() { }  
        public JockeyNguyenDntRepository(RaceManagementDBContext context) => _context = context;
        public async Task<List<RaceMS.Entities.NguyenDNT.Models.JockeyNguyenDnt>> GetAllAsync(int id)
        {
            return await _context.JockeyNguyenDnts.ToListAsync();
        }
    }
}
