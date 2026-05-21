using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public class SystemUserAccountService : ISystemUserAccountService
    {
        private readonly SystemUserAccountRepository _repository;
        public SystemUserAccountService(SystemUserAccountRepository repository) => _repository = repository;

        public async Task<SystemUserAccount> GetUserAccountAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Username and password must not be null or empty.");
                }
                return await _repository.GetUserAccountAsync(username, password);

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while validating input: {ex.Message}", ex);
            }
        }

    }
}
