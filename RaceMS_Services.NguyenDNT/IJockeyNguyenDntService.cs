using RaceMS.Entities.NguyenDNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public interface IJockeyNguyenDntService
    {
        Task<List<JockeyNguyenDnt>> GetAllAsync();
        Task<List<JockeyNguyenDnt>> SearchAsync(string? fullName, string? email, string? licenseCode);
        Task<JockeyNguyenDnt?> GetByIdAsync(int id);
        Task<int> CreateAsync(JockeyNguyenDnt jockey);
        Task<int> UpdateAsync(JockeyNguyenDnt jockey);
        Task<bool> DeleteAsync(int id);

    }
}
