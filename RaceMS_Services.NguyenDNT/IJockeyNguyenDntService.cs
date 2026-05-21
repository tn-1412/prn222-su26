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
    }
}
