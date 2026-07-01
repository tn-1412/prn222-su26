using RaceMS.Entities.NguyenDNT.Models;

namespace RaceMS.WebMVCApp.NguyenDNT.Models
{
    // Chuỗi tên role dùng cho [Authorize(Roles = "...")] và User.IsInRole(...) trong View.
    // Khớp với enum UserRole — ClaimTypes.Role được set bằng UserRole.ToString() khi login.
    public static class RoleNames
    {
        public const string ChiefAccountant = nameof(UserRole.ChiefAccountant);
        public const string Accountant = nameof(UserRole.Accountant);
        public const string InternalAuditor = nameof(UserRole.InternalAuditor);

        // Được tạo/sửa Jockey & Registration
        public const string CanWrite = ChiefAccountant + "," + Accountant;

        // Chỉ ChiefAccountant được xóa
        public const string CanDelete = ChiefAccountant;
    }
}
