using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT;

namespace RaceMS_Services.NguyenDNT
{
    public class JockeyNguyenDntService : IJockeyNguyenDntService
    {
        private readonly JockeyNguyenDntRepository _repository;

        public JockeyNguyenDntService(JockeyNguyenDntRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<JockeyNguyenDnt>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<JockeyNguyenDnt> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(JockeyNguyenDnt jockey)
        {
            await ValidateAsync(jockey, excludeId: 0);
            return await _repository.CreateAsync(jockey);
        }

        public async Task<int> UpdateAsync(JockeyNguyenDnt jockey)
        {
            var existing = await _repository.GetByIdAsync(jockey.JockeyNguyenDntid);
            if (existing == null)
                throw new InvalidOperationException(":Jockey không tồn tại.");

            await ValidateAsync(jockey, excludeId: jockey.JockeyNguyenDntid);
            return await _repository.UpdateAsync(jockey);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(":Jockey không tồn tại hoặc đã bị xóa.");

            return await _repository.DeleteAsync(id);
        }

        // Validation tập trung — dùng chung cho Create và Update.
        // Format exception: "FieldName:Thông báo lỗi" để controller parse ra ModelState đúng field.
        private async Task ValidateAsync(JockeyNguyenDnt jockey, int excludeId)
        {
            if (string.IsNullOrWhiteSpace(jockey.FullName))
                throw new InvalidOperationException("FullName:Họ và tên không được để trống.");

            if (jockey.DateOfBirth.HasValue && jockey.DateOfBirth.Value > DateTime.Today)
                throw new InvalidOperationException("DateOfBirth:Ngày sinh không được ở tương lai.");

            if (jockey.Weight.HasValue && (jockey.Weight < 40 || jockey.Weight > 80))
                throw new InvalidOperationException("Weight:Cân nặng phải trong khoảng 40 – 80 kg.");

            if (!string.IsNullOrWhiteSpace(jockey.LicenseCode)
                && await _repository.LicenseCodeExistsAsync(jockey.LicenseCode.Trim(), excludeId))
                throw new InvalidOperationException("LicenseCode:Mã giấy phép này đã được đăng ký.");

            if (!string.IsNullOrWhiteSpace(jockey.Email)
                && await _repository.EmailExistsAsync(jockey.Email.Trim(), excludeId))
                throw new InvalidOperationException("Email:Email này đã được sử dụng bởi một jockey khác.");

            if (!string.IsNullOrWhiteSpace(jockey.PhoneNumber)
                && await _repository.PhoneExistsAsync(jockey.PhoneNumber.Trim(), excludeId))
                throw new InvalidOperationException("PhoneNumber:Số điện thoại này đã được đăng ký.");
        }
    }
}
