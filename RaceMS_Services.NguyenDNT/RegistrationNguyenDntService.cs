using Microsoft.Identity.Client;
using RaceMS_Repositories.NguyenDNT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceMS_Services.NguyenDNT
{
    public class RegistrationNguyenDntService : IRegistrationNguyenDntService
    {
        private readonly RegistrationNguyenDntRepository _repository;
        public RegistrationNguyenDntService() => _repository = new RegistrationNguyenDntRepository();

        public async Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching registrations: {ex.Message}", ex);
            }
        }
        public async Task<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt> GetByIdAsync(int id)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching registration by ID: {ex.Message}", ex);
            }
        }
        public async Task<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt> SearchAsync(int code, decimal amount, string name)
        {
            try
            {
                var result = await _repository.SearchAsync(code, amount, name);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching registrations: {ex.Message}", ex);
            }

        }
        public async Task<int> CreateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration)
        {
            try
            {
                return await _repository.CreateAsync(registration);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error creating registration: {ex.Message}", ex);
            }
        }
        public async Task<int> UpdateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration)
        {
            try
            {
                return await _repository.UpdateAsync(registration);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating registration: {ex.Message}", ex);
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                //var item = await _repository.GetByIdAsync(id);
                //if(item == null)
                //    throw new KeyNotFoundException("Registration not found for deletion.");
                //return await _repository.RemoveAsync(item);
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting registration: {ex.Message}", ex);
            }
        }
    }
}
