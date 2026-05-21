using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public interface IRegistrationNguyenDntService
    {
            Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> GetAllAsync();
            Task<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt> GetByIdAsync(int id);
            Task <RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt> SearchAsync(int code, decimal amount, string name);
        // Mutation Methods
        Task<int> CreateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration);
        Task<int> UpdateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration);

        Task<bool> DeleteAsync(int id);
    }
}
