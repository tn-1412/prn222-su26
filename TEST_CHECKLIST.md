# CHECKLIST KIỂM THỬ — RaceMS (Cơ bản)

> Start app → mở https://localhost:{port} → test theo thứ tự dưới đây  
> Tick ✅ khi pass, ❌ khi fail (ghi note lỗi bên cạnh)

---

## 1. AUTHENTICATION / AUTHORIZATION

| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| A1 | Gõ thẳng URL `/JockeyNguyenDnts/Index` khi **chưa login** | Redirect về `/Account/Login` | |
| A2 | Login với **username/password đúng** | Redirect vào `/JockeyNguyenDnts/Index` | |
| A3 | Login với **password sai** | Ở lại Login, hiện "Tên đăng nhập hoặc mật khẩu không đúng." | |
| A4 | Login với **tài khoản không tồn tại** | Ở lại Login, hiện thông báo lỗi | |
| A5 | **Submit form rỗng** (không nhập gì) | Không crash, hiện thông báo lỗi | |
| A6 | Đang login, gõ lại URL `/Account/Login` | Tự redirect sang Index (không cho vào lại form login) | |
| A7 | Click **Logout** | Redirect về Login, truy cập lại protected URL bị chặn | |

---

## 2. JOCKEY — CRUD + SEARCH

### 2a. Search
| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| S1 | Để trống ô search → Submit | Hiển thị toàn bộ danh sách | |
| S2 | Nhập **một phần tên** jockey → Submit | Lọc đúng jockey có tên chứa từ khóa | |
| S3 | Nhập **email** → Submit | Lọc đúng jockey có email khớp | |
| S4 | Nhập **LicenseCode** → Submit | Lọc đúng jockey có mã giấy phép khớp | |
| S5 | Nhập **chuỗi không tồn tại** → Submit | Bảng rỗng, không lỗi | |
| S6 | Nhập **chỉ khoảng trắng** → Submit | Trả về toàn bộ danh sách | |

### 2b. Create
| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| C1 | Điền đầy đủ hợp lệ → Save | Redirect Index, jockey mới xuất hiện đầu danh sách | |
| C2 | Bỏ trống **FullName** → Save | Lỗi tại field FullName: "Họ và tên không được để trống." | |
| C3 | **DateOfBirth = ngày mai** → Save | Lỗi: "Ngày sinh không được ở tương lai." | |
| C4 | **Weight = 35** (dưới 40) → Save | Lỗi: "Cân nặng phải trong khoảng 40 – 80 kg." | |
| C5 | **Weight = 85** (trên 80) → Save | Lỗi: "Cân nặng phải trong khoảng 40 – 80 kg." | |
| C6 | **LicenseCode trùng** với jockey đã có → Save | Lỗi: "Mã giấy phép này đã được đăng ký." | |
| C7 | **Email trùng** với jockey đã có → Save | Lỗi: "Email này đã được sử dụng bởi một jockey khác." | |
| C8 | **PhoneNumber trùng** với jockey đã có → Save | Lỗi: "Số điện thoại này đã được đăng ký." | |

### 2c. Edit
| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| E1 | Sửa FullName hợp lệ → Save | Redirect Index, tên mới hiển thị | |
| E2 | Xóa FullName → Save | Lỗi: "Họ và tên không được để trống." | |
| E3 | Giữ nguyên LicenseCode/Email/Phone của chính jockey đó → Save | Thành công (không báo trùng với chính nó) | |
| E4 | Truy cập `/JockeyNguyenDnts/Edit/99999` | HTTP 404 | |

### 2d. Delete
| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| D1 | Xác nhận xóa jockey hợp lệ | Redirect Index, jockey biến mất khỏi danh sách | |
| D2 | Truy cập `/JockeyNguyenDnts/Delete/99999` | HTTP 404 | |

### 2e. Details
| # | Hành động | Kết quả mong đợi | ✅/❌ |
|---|-----------|-----------------|------|
| V1 | Click xem chi tiết jockey | Hiển thị đầy đủ thông tin | |
| V2 | Truy cập `/JockeyNguyenDnts/Details/99999` | HTTP 404 | |

---

## 3. SIGNALR — REAL-TIME

> **Chuẩn bị:** Mở 2 tab trình duyệt, cùng đăng nhập, cùng ở trang Index

| # | Hành động (Tab 1) | Kết quả mong đợi | ✅/❌ |
|---|------------------|-----------------|------|
| R1 | Xóa jockey qua modal confirm | Jockey biến mất ở **cả 2 tab ngay lập tức** (không reload) | |
| R2 | Tạo jockey mới qua modal | Jockey mới xuất hiện ở **cả 2 tab ngay lập tức** | |
| R3 | Sửa jockey qua modal | Dữ liệu jockey cập nhật ở **cả 2 tab ngay lập tức** | |
| R4 | Vào `/Chat/Index`, gõ tin nhắn → Send | Tab 2 nhận tin nhắn tức thì | |
| R5 | Tab 2 đóng tab Chat | Tab 1 thấy thông báo "user đã rời" | |

---

## LƯU Ý TRƯỚC KHI TEST

- Chạy app bằng **HTTPS** (F5 trong Visual Studio, chọn profile `https`) — cookie `Secure=Always` không hoạt động trên HTTP
- Đảm bảo SQL Server `ASUS-1412\SQLEXPRESS` đang chạy và DB `RaceManagementDB` tồn tại
- Cần ít nhất **1 tài khoản** trong bảng `System.UserAccount` có `IsActive = true`

---

*Tổng: **35 test cases** | Thời gian ước tính: ~20 phút*
