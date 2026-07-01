using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Repositories.NguyenDNT;
using System.Text.RegularExpressions;

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

        // Cả 3 ô đều rỗng → trả về toàn bộ danh sách
        public async Task<List<JockeyNguyenDnt>> SearchAsync(string? fullName, string? email, string? licenseCode)
        {
            if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(licenseCode))
                return await _repository.GetAllAsync();

            return await _repository.SearchAsync(fullName?.Trim(), email?.Trim(), licenseCode?.Trim());
        }

        public async Task<JockeyNguyenDnt?> GetByIdAsync(int id)
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

            // Giới hạn độ dài phải khớp đúng cột DB (xem RaceManagementDBContext.OnModelCreating) —
            // nếu không chặn ở đây, EF Core sẽ ném DbUpdateException/SqlException lúc SaveChanges (HTTP 500, lộ stack trace).
            if (jockey.FullName.Trim().Length > 100)
                throw new InvalidOperationException("FullName:Họ và tên không được vượt quá 100 ký tự.");

            if (jockey.DateOfBirth.HasValue && jockey.DateOfBirth.Value > DateTime.Today)
                throw new InvalidOperationException("DateOfBirth:Ngày sinh không được ở tương lai.");

            if (jockey.Weight.HasValue && (jockey.Weight < 40 || jockey.Weight > 80))
                throw new InvalidOperationException("Weight:Cân nặng phải trong khoảng 40 – 80 kg.");

            // Bắt buộc + đúng định dạng JCK-YYYY-NNNNN (VD: JCK-2024-00123).
            // Áp dụng cho mọi Create/Edit từ giờ — không hồi tố dữ liệu cũ có sẵn trong DB (LIC-...).
            if (string.IsNullOrWhiteSpace(jockey.LicenseCode))
                throw new InvalidOperationException("LicenseCode:Mã giấy phép không được để trống.");

            if (jockey.LicenseCode.Trim().Length > 50)
                throw new InvalidOperationException("LicenseCode:Mã giấy phép không được vượt quá 50 ký tự.");

            if (!Regex.IsMatch(jockey.LicenseCode.Trim(), @"^JCK-\d{4}-\d{5}$"))
                throw new InvalidOperationException("LicenseCode:Mã giấy phép phải đúng định dạng JCK-YYYY-NNNNN (VD: JCK-2024-00123).");

            if (await _repository.LicenseCodeExistsAsync(jockey.LicenseCode.Trim(), excludeId))
                throw new InvalidOperationException("LicenseCode:Mã giấy phép này đã được đăng ký.");

            if (!string.IsNullOrWhiteSpace(jockey.Email))
            {
                if (jockey.Email.Trim().Length > 150)
                    throw new InvalidOperationException("Email:Email không được vượt quá 150 ký tự.");

                if (!Regex.IsMatch(jockey.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new InvalidOperationException("Email:Email không đúng định dạng (VD: ten@domain.com).");

                if (await _repository.EmailExistsAsync(jockey.Email.Trim(), excludeId))
                    throw new InvalidOperationException("Email:Email này đã được sử dụng bởi một jockey khác.");
            }

            if (!string.IsNullOrWhiteSpace(jockey.PhoneNumber))
            {
                if (jockey.PhoneNumber.Trim().Length > 20)
                    throw new InvalidOperationException("PhoneNumber:Số điện thoại không được vượt quá 20 ký tự.");

                if (await _repository.PhoneExistsAsync(jockey.PhoneNumber.Trim(), excludeId))
                    throw new InvalidOperationException("PhoneNumber:Số điện thoại này đã được đăng ký.");
            }
        }
    }
}
