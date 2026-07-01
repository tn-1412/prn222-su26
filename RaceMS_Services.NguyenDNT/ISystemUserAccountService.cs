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
        Task<SystemUserAccount?> GetUserAccountAsync(string username, string password);

        // Đăng ký tài khoản mới — luôn gán role thấp nhất (InternalAuditor), không cho tự chọn role lúc đăng ký.
        Task<int> RegisterAsync(SystemUserAccount account);
    }
}
