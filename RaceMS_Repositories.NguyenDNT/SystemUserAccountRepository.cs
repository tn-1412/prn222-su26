using Microsoft.EntityFrameworkCore;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Repositories.NguyenDNT
{
    public class SystemUserAccountRepository : Base.GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository() { }
        public SystemUserAccountRepository(RaceManagementDBContext context) => _context = context;
        public async Task<SystemUserAccount?> GetUserAccountAsync(string userName, string password)
        {
            return await _context.SystemUserAccounts.FirstOrDefaultAsync(
                x => (x.UserName == userName || x.Email == userName) && x.Password == password && x.IsActive);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await _context.SystemUserAccounts.AnyAsync(x => x.UserName == userName);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.SystemUserAccounts.AnyAsync(x => x.Email == email);
        }
    }
}
