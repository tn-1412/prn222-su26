using RaceMS.Entities.NguyenDNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public interface ISystemUserAccountService
    {
        Task<SystemUserAccount> GetUserAccountAsync(string username, string password);
    }
}
