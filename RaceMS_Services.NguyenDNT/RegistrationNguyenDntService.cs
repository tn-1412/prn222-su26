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
        public async Task<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt?> GetByIdAsync(int id)
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
        // Cả 3 tham số đều rỗng/null → trả về toàn bộ danh sách
        public async Task<List<RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt>> SearchAsync(int? code, decimal? amount, string? name)
        {
            try
            {
                if (!code.HasValue && !amount.HasValue && string.IsNullOrWhiteSpace(name))
                    return await _repository.GetAllAsync();

                return await _repository.SearchAsync(code, amount, name?.Trim());
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
                await ValidateAsync(registration);
                return await _repository.CreateAsync(registration);
            }
            catch (InvalidOperationException)
            {
                throw;
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
                var existing = (await _repository.GetByIdAsync(registration.RegistrationNguyenDntid)).FirstOrDefault();
                if (existing == null)
                    throw new InvalidOperationException(":Đăng ký không tồn tại.");

                await ValidateAsync(registration);
                return await _repository.UpdateAsync(registration);
            }
            catch (InvalidOperationException)
            {
                throw;
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
                var success = await _repository.DeleteAsync(id);
                if (!success)
                    throw new InvalidOperationException(":Đăng ký không tồn tại hoặc đã bị xóa.");
                return success;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting registration: {ex.Message}", ex);
            }
        }

        // Validation tập trung — dùng chung cho Create và Update.
        // Format exception: "FieldName:Thông báo lỗi" để controller parse ra ModelState đúng field.
        private async Task ValidateAsync(RaceMS.Entities.NguyenDNT.Models.RegistrationNguyenDnt registration)
        {
            if (string.IsNullOrWhiteSpace(registration.RaceName))
                throw new InvalidOperationException("RaceName:Tên giải đua không được để trống.");

            if (string.IsNullOrWhiteSpace(registration.HorseName))
                throw new InvalidOperationException("HorseName:Tên ngựa không được để trống.");

            if (registration.PrizeMoney.HasValue && registration.PrizeMoney < 0)
                throw new InvalidOperationException("PrizeMoney:Tiền thưởng không được âm.");

            if (registration.RegisteredDate.HasValue && registration.ResponseDate.HasValue
                && registration.ResponseDate.Value < registration.RegisteredDate.Value)
                throw new InvalidOperationException("ResponseDate:Ngày phản hồi không được trước ngày đăng ký.");

            if (registration.JockeyNguyenDntid.HasValue
                && !await _repository.JockeyExistsAsync(registration.JockeyNguyenDntid.Value))
                throw new InvalidOperationException("JockeyNguyenDntid:Jockey được chọn không tồn tại.");
        }
    }
}
