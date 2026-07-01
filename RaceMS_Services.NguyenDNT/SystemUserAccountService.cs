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

        public async Task<SystemUserAccount?> GetUserAccountAsync(string username, string password)
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

        public async Task<int> RegisterAsync(SystemUserAccount account)
        {
            await ValidateRegisterAsync(account);

            account.RoleId = (int)UserRole.InternalAuditor; // tự đăng ký → luôn ở role thấp nhất, không cho tự chọn
            account.IsActive = true;
            account.CreatedDate = DateTime.Now;
            account.CreatedBy = "Self-Registration";

            return await _repository.CreateAsync(account);
        }

        // Format exception: "FieldName:Thông báo lỗi" để controller parse ra ModelState đúng field.
        private async Task ValidateRegisterAsync(SystemUserAccount account)
        {
            if (string.IsNullOrWhiteSpace(account.UserName))
                throw new InvalidOperationException("UserName:Tên đăng nhập không được để trống.");
            if (account.UserName.Trim().Length > 50)
                throw new InvalidOperationException("UserName:Tên đăng nhập không được vượt quá 50 ký tự.");
            if (await _repository.UserNameExistsAsync(account.UserName.Trim()))
                throw new InvalidOperationException("UserName:Tên đăng nhập này đã được sử dụng.");

            if (string.IsNullOrWhiteSpace(account.Password) || account.Password.Length < 6)
                throw new InvalidOperationException("Password:Mật khẩu phải có ít nhất 6 ký tự.");
            if (account.Password.Length > 100)
                throw new InvalidOperationException("Password:Mật khẩu không được vượt quá 100 ký tự.");

            if (string.IsNullOrWhiteSpace(account.FullName))
                throw new InvalidOperationException("FullName:Họ và tên không được để trống.");
            if (account.FullName.Trim().Length > 100)
                throw new InvalidOperationException("FullName:Họ và tên không được vượt quá 100 ký tự.");

            if (string.IsNullOrWhiteSpace(account.Email) || !System.Text.RegularExpressions.Regex.IsMatch(account.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new InvalidOperationException("Email:Email không hợp lệ.");
            if (account.Email.Trim().Length > 150)
                throw new InvalidOperationException("Email:Email không được vượt quá 150 ký tự.");
            if (await _repository.EmailExistsAsync(account.Email.Trim()))
                throw new InvalidOperationException("Email:Email này đã được đăng ký.");

            if (string.IsNullOrWhiteSpace(account.Phone))
                throw new InvalidOperationException("Phone:Số điện thoại không được để trống.");
            if (account.Phone.Trim().Length > 50)
                throw new InvalidOperationException("Phone:Số điện thoại không được vượt quá 50 ký tự.");

            if (string.IsNullOrWhiteSpace(account.EmployeeCode))
                throw new InvalidOperationException("EmployeeCode:Mã nhân viên không được để trống.");
            if (account.EmployeeCode.Trim().Length > 50)
                throw new InvalidOperationException("EmployeeCode:Mã nhân viên không được vượt quá 50 ký tự.");
        }
    }
}
