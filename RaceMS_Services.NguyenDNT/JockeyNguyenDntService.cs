using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public class JockeyNguyenDntService : IJockeyNguyenDntService
    {
        private readonly JockeyNguyenDntRepository _repository;
        public JockeyNguyenDntService() => _repository = new JockeyNguyenDntRepository();

        public async Task<List<JockeyNguyenDnt>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching jockeys: {ex.Message}", ex);
            }
        }
    }
}
