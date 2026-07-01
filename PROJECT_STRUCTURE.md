# BÁO CÁO CẤU TRÚC & HOẠT ĐỘNG — RaceMS (PRN222 ASM01, NguyenDNT)

> Tài liệu mô tả kiến trúc và cách hoạt động thực tế của dự án tại thời điểm 2026-07-01.
> Xem [TEST_REPORT.md](TEST_REPORT.md) để biết kết quả kiểm thử/bug đã fix — tài liệu này chỉ mô tả **cấu trúc & cách hoạt động**.

---

## 1. Tổng quan

**RaceMS** (Race Management System) là ứng dụng ASP.NET Core MVC (.NET 8) quản lý jockey (tay đua) và đăng ký tham gia giải đua, có real-time sync qua SignalR và chat nội bộ. Ứng dụng dùng kiến trúc 4 tầng cổ điển, không có API riêng (server-render toàn bộ bằng Razor View).

| | |
|---|---|
| Framework | ASP.NET Core MVC 8.0 |
| ORM | Entity Framework Core (Database First — scaffold từ DB có sẵn) |
| DB | SQL Server (`ASUS-1412\SQLEXPRESS`, database `RaceManagementDB`) |
| Real-time | SignalR (WebSocket) |
| Auth | ASP.NET Core Cookie Authentication + Role-based Authorization |
| Frontend | Razor Views (server-render) + jQuery + Bootstrap 5 + CSS tự viết (không SPA/React) |
| URL chạy dev | `https://localhost:7229` |

---

## 2. Kiến trúc 4 tầng

```
RaceMS.WebMVCApp.NguyenDNT   ← Web layer: Controllers, Views, Hubs, Program.cs
        ↓ phụ thuộc
RaceMS_Services.NguyenDNT    ← Business logic + validation
        ↓ phụ thuộc
RaceMS_Repositories.NguyenDNT ← Data access (EF Core), DbContext
        ↓ phụ thuộc
RaceMS.Entities.NguyenDNT    ← POCO models (không phụ thuộc gì cả)
```

Dependency luôn đi 1 chiều từ trên xuống — tầng dưới không biết gì về tầng trên. Đây là kiến trúc Layered Architecture chuẩn cho project academic.

### 2.1. `RaceMS.Entities.NguyenDNT` — Model
File `.cs` auto-generate bởi **EF Core Power Tools** (`#nullable disable`, comment "auto-generated" ở đầu file — nghĩa là DB được thiết kế trước, code sinh ra sau, không phải Code First).

| File | Nội dung |
|---|---|
| `JockeyNguyenDnt.cs` | Entity tay đua |
| `RegistrationNguyenDnt.cs` | Entity đăng ký đua |
| `SystemUserAccount.cs` | Entity tài khoản hệ thống (dùng để login) |
| `UserRole.cs` | Enum tự thêm — map RoleId ↔ tên role (không sinh tự động) |

### 2.2. `RaceMS_Repositories.NguyenDNT` — Data Access
- `Base/GenericRepository<T>` — repository dùng chung: `GetAll`, `GetById`, `Create`, `Update`, `Remove` (cả bản sync và async), thao tác trực tiếp qua `DbContext.Set<T>()`.
- `JockeyNguyenDntRepository`, `RegistrationNguyenDntRepository`, `SystemUserAccountRepository` — kế thừa `GenericRepository<T>`, override/thêm method riêng (search, kiểm tra trùng, xóa cascade...).
- `DBContext/RaceManagementDBContext.cs` — DbContext, cấu hình tất cả bảng, khóa chính/ngoại, độ dài cột, collation `Vietnamese_CI_AS` (case-insensitive, accent-sensitive) trong `OnModelCreating`.

**Điểm đặc biệt:** `GenericRepository` có **2 constructor**:
```csharp
public GenericRepository() { _context ??= new RaceManagementDBContext(); }        // tự tạo DbContext riêng, đọc appsettings.json
public GenericRepository(RaceManagementDBContext context) { _context = context; } // nhận qua DI
```
→ `JockeyNguyenDntRepository` được đăng ký qua DI (constructor có `context`), nhưng `RegistrationNguyenDntService`/`SystemUserAccountService` lại `new Repository()` trực tiếp trong constructor của chính nó (không qua DI) — 2 cách khởi tạo khác nhau tồn tại song song, không nhất quán nhưng cả 2 đều hoạt động đúng (vì cùng đọc chung 1 connection string).

### 2.3. `RaceMS_Services.NguyenDNT` — Business Logic
Mỗi entity có 1 cặp `I...Service` (interface) + `...Service` (implementation). Trách nhiệm:
- Validate dữ liệu (bắt buộc, khoảng giá trị, trùng lặp, khóa ngoại tồn tại) **trước khi** gọi Repository.
- Quy ước lỗi: ném `InvalidOperationException("TenTruong:Thông báo lỗi")` — Controller parse chuỗi này bằng `Split(':', 2)` để gắn lỗi đúng vào field tương ứng trong `ModelState`, hiển thị ngay dưới ô input.

### 2.4. `RaceMS.WebMVCApp.NguyenDNT` — Web Layer
```
Controllers/   AccountController, JockeyNguyenDntsController, RegistrationNguyenDntsController, ChatController, HomeController
Hubs/          MSRaceHub (Jockey + Registration realtime), ChatHub
Models/        LoginRequest, RoleNames (hằng số role dùng cho [Authorize])
Views/         Account/, JockeyNguyenDnts/, RegistrationNguyenDnts/, Chat/, Home/, Shared/(_Layout, _ValidationScriptsPartial)
wwwroot/       lib/ (bootstrap, jquery, signalr client, table-pagination), css/, js/
Program.cs     Đăng ký DI, Auth, Routing, Middleware pipeline
```

---

## 3. Cơ sở dữ liệu

3 bảng chính trong `RaceManagementDB` (SQL Server):

```
JockeyNguyenDNT                    System.UserAccount
├─ JockeyNguyenDNTId (PK)          ├─ UserAccountID (PK)
├─ FullName* (100)                 ├─ UserName* (50)
├─ PhoneNumber (20)                ├─ Password* (100) — LƯU PLAINTEXT
├─ Email (150)                     ├─ FullName* (100)
├─ LicenseCode (50)                ├─ Email* (150)
├─ Weight decimal(5,2)             ├─ Phone* (50)
├─ DateOfBirth                     ├─ EmployeeCode* (50)
└─ IsActive (default true)         ├─ RoleId (int — không có bảng Role riêng!)
                                    ├─ IsActive
        │ 1                        └─ ... (audit fields: CreatedBy/Date, ModifiedBy/Date)
        │
        │ N
RegistrationNguyenDNT
├─ RegistrationNguyenDNTId (PK)
├─ RaceName* (200)
├─ HorseName (150)
├─ RegisteredDate / ResponseDate
├─ PrizeMoney decimal(18,2)
├─ IsConfirmed (bool?, nullable!)
├─ Note (500)
└─ JockeyNguyenDNTId (FK → JockeyNguyenDNT, nullable — 1 jockey có N đăng ký)
```

**Ghi chú quan trọng:**
- `RoleId` chỉ là số nguyên thô, **không có bảng `Role` lookup** trong DB. Việc map RoleId → tên role (`ChiefAccountant`/`Accountant`/`InternalAuditor`) được làm ở tầng code (`UserRole` enum) dựa trên suy luận từ dữ liệu mẫu (username/email), không phải từ thiết kế DB gốc.
- `IsConfirmed` là `bool?` — đây là nguồn gốc 1 bug từng gặp (chi tiết ở TEST_REPORT.md) vì `asp-for` checkbox của ASP.NET Core không hỗ trợ trực tiếp kiểu nullable.
- Xóa 1 Jockey sẽ **cascade xóa cả các Registration liên quan** (xử lý thủ công trong `JockeyNguyenDntRepository.DeleteAsync`, không phải cascade delete ở tầng DB).

---

## 4. Authentication & Authorization

### 4.1. Cơ chế: Cookie Authentication (không phải JWT)
```csharp
// Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(..., options => {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Forbidden";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;   // bắt buộc HTTPS
    });
```
Gói `System.IdentityModel.Tokens.Jwt` có trong `.csproj` nhưng **không được dùng ở đâu cả** — dead dependency, không có JWT thật nào được phát hành trong toàn bộ project (đã grep xác nhận qua git history).

### 4.2. Luồng Login
1. User submit form `/Account/Login` (POST) với `userName` + `password`.
2. `SystemUserAccountService.GetUserAccountAsync` → so khớp trực tiếp trong SQL (`Password == password`, **plaintext, không hash**) + `IsActive=true`.
3. Nếu khớp: map `RoleId` (int) → tên role qua `UserRole` enum, tạo `ClaimsIdentity` gồm `Name`, `GivenName`, `Role` → `HttpContext.SignInAsync(...)` → ASP.NET Core mã hoá (Data Protection) và set cookie `.AspNetCore.Cookies`.
4. Redirect `/JockeyNguyenDnts/Index`.

### 4.3. Role-based Authorization (mới thêm)
```
RoleId=1 → ChiefAccountant  → Full quyền (Create/Edit/Delete)
RoleId=2 → Accountant       → Create/Edit, KHÔNG Delete
RoleId=3 → InternalAuditor  → Chỉ xem (Index/Details), không Create/Edit/Delete
```
Áp dụng ở **3 lớp phòng thủ độc lập** (defense in depth):
1. **Controller**: `[Authorize(Roles = RoleNames.CanWrite)]` / `RoleNames.CanDelete` trên từng action Create/Edit/Delete của `JockeyNguyenDntsController` và `RegistrationNguyenDntsController`.
2. **SignalR Hub**: cùng attribute trên từng method của `MSRaceHub` (`HubCreate_*`, `HubEdit_*`, `HubDelete_*`) — chặn cả đường vòng qua WebSocket, không chỉ HTTP form.
3. **View (UX)**: `User.IsInRole(...)` ẩn hẳn nút Create/Edit/Delete nếu không đủ quyền, kể cả row mới được SignalR đẩy về real-time (dùng biến JS `canWrite`/`canDelete` render từ server).

Nếu bị chặn ở Controller → tự động redirect `/Account/Forbidden` (trang thông báo, không phải lỗi 403 trần trụi).

---

## 5. Luồng request điển hình

**Ví dụ: xem danh sách Jockey có tìm kiếm**

```
Browser GET /JockeyNguyenDnts/Index?fullName=Messi
   ↓
Middleware pipeline (Program.cs, theo đúng thứ tự):
  UseHttpsRedirection → UseStaticFiles → UseRouting → UseAuthentication → UseAuthorization
   ↓
JockeyNguyenDntsController.Index(fullName, email, licenseCode)
   ↓
IJockeyNguyenDntService.SearchAsync(fullName, email, licenseCode)
   ↓ (nếu cả 3 tham số rỗng → gọi GetAllAsync, ngược lại build query động)
JockeyNguyenDntRepository.SearchAsync(...)
   ↓
EF Core dịch sang SQL: WHERE FullName LIKE '%Messi%' [AND Email LIKE ... AND LicenseCode LIKE ...]
   ↓
Trả về List<JockeyNguyenDnt> → View(jockeys)
   ↓
Views/JockeyNguyenDnts/Index.cshtml render HTML (kèm script table-pagination.js chạy phía client)
```

**Ví dụ: xóa qua SignalR (không reload trang)**

```
Browser: click nút xóa → JS gọi connection.invoke("HubDelete_JockeyNguyenDnt", id)
   ↓ (qua kết nối WebSocket đã mở sẵn tới /msRaceHub, xác thực bằng cookie lúc negotiate)
MSRaceHub.HubDelete_JockeyNguyenDnt(id)
   ↓ [Authorize(Roles = RoleNames.CanDelete)] chặn trước nếu không đủ quyền
IJockeyNguyenDntService.DeleteAsync(id) → xóa DB (kèm cascade Registration liên quan)
   ↓
Clients.All.SendAsync("Receiver_DeleteJockeyNguyenDnt", id)
   ↓ (broadcast tới TẤT CẢ client đang mở kết nối, kể cả các tab/trình duyệt khác)
Mọi tab đang mở Index.cshtml: JS lắng nghe event này → document.getElementById(`jockey-row-${id}`).remove()
```

---

## 6. Các module chức năng

| Module | Route | Controller | View | CRUD | Search | SignalR |
|---|---|---|---|---|---|---|
| **Jockey** | `/JockeyNguyenDnts/*` | `JockeyNguyenDntsController` | `Views/JockeyNguyenDnts/*` | ✅ đầy đủ | 3 trường độc lập (FullName/Email/LicenseCode, AND) | ✅ Create/Edit/Delete qua `MSRaceHub` |
| **Registration** | `/RegistrationNguyenDnts/*` | `RegistrationNguyenDntsController` | `Views/RegistrationNguyenDnts/*` | ✅ đầy đủ | 3 trường độc lập (Code/PrizeMoney/HorseName, AND) | ✅ Create/Edit/Delete qua `MSRaceHub` |
| **Chat** | `/Chat/*` | `ChatController` | `Views/Chat/Index.cshtml` | — | — | ✅ gửi/nhận tin nhắn + thông báo connect/disconnect qua `ChatHub` |
| **Account** | `/Account/*` | `AccountController` | `Views/Account/*` | — | — | — |
| **Home** | `/Home/*` | `HomeController` | `Views/Home/*` | — | — | — (route ít dùng, app mặc định vào thẳng `/Account/Login` → `/JockeyNguyenDnts/Index`) |

**Route mặc định:** `{controller=Account}/{action=Login}/{id?}` — vào domain gốc `/` sẽ trúng `AccountController.Login` (Home không phải trang chủ thật sự).

---

## 7. SignalR — 2 Hub

### `MSRaceHub` (`/msRaceHub`)
| Method (client gọi) | Event broadcast | Ai được gọi |
|---|---|---|
| `HubCreate_JockeyNguyenDnt(jockey)` | `Receiver_CreateJockeyNguyenDnt` | CanWrite |
| `HubEdit_JockeyNguyenDnt(jockey)` | `Receiver_EditJockeyNguyenDnt` | CanWrite |
| `HubDelete_JockeyNguyenDnt(id)` | `Receiver_DeleteJockeyNguyenDnt` | CanDelete |
| `HubCreate_RegistrationNguyenDnt(reg)` | `Receiver_CreateRegistrationNguyenDnt` | CanWrite |
| `HubEdit_RegistrationNguyenDnt(reg)` | `Receiver_EditRegistrationNguyenDnt` | CanWrite |
| `HubDelete_RegistrationNguyenDnt(id)` | `Receiver_DeleteRegistrationNguyenDnt` | CanDelete |

Lỗi nghiệp vụ (validate fail, không tồn tại...) không throw ra ngoài mà gửi riêng cho người gọi qua `Clients.Caller.SendAsync("CreateError"/"EditError"/"DeleteError", message)`.

### `ChatHub` (`/chatHub`)
`SendMessage` → broadcast `ReceiveMessage`; `OnConnectedAsync`/`OnDisconnectedAsync` → broadcast `UserConnected`/`UserDisconnected` cho các client còn lại.

Cả 2 Hub đều `[Authorize]` — kết nối `negotiate` bị từ chối (302 redirect / 401) nếu chưa có cookie hợp lệ.

---

## 8. Frontend / UI

- Không dùng SPA framework — mỗi View tự viết `<style>` inline riêng (không tách file `.css` chung), theo phong cách gradient tím-đen (`#1a1a2e → #16213e → #0f3460`, accent `#7c3aed`/`#8b5cf6`), font `Playfair Display` (tiêu đề) + `Inter` (nội dung).
- `_Layout.cshtml` — layout dùng chung cho hầu hết trang (navbar tối, hiện tên user + badge role + nút đăng xuất). Riêng `Login.cshtml` và `Forbidden.cshtml` dùng `Layout = null` (trang độc lập, không qua `_Layout`).
- Phân trang client-side: thư viện tự viết `wwwroot/lib/table-pagination/table-pagination.js` (jQuery plugin `$(table).createTablePagination(options)`), sinh class CSS theo hậu tố `id` của bảng (`table-pagination-*-jockeyTable`, `...-registrationTable`) — mỗi trang phải tự viết CSS override riêng theo đúng hậu tố này.
- SignalR client: `wwwroot/js/signalr/dist/browser/signalr.min.js` (bundle local, không load CDN).

---

## 9. Điểm cần lưu ý khi đọc/sửa code

1. **2 nguồn sự thật cho "search"**: Jockey Index từng có JS lọc trùng lặp với search server-side (đã bỏ, xem TEST_REPORT.md) — giờ search hoàn toàn server-side qua query string.
2. **DbContext dùng `QueryTrackingBehavior.NoTracking`** mặc định (`OnConfiguring`) — nghĩa là entity lấy ra không tự động track thay đổi, các thao tác Update phải dùng `_context.Attach(entity)` + `EntityState.Modified` (đã cài sẵn trong `GenericRepository.UpdateAsync`).
3. **Antiforgery**: chỉ dùng đúng 1 lần cho mỗi `<form>` — tag helper `asp-action` đã tự sinh token, **không được** gọi thêm `@Html.AntiForgeryToken()` thủ công (từng gây bug 400 ở form Create, xem TEST_REPORT.md).
4. **Checkbox cho property `bool?`** không dùng được `asp-for` trực tiếp — phải viết tay `<input type="hidden" value="false">` + `<input type="checkbox" value="true" checked="@(...)">`.
5. **Password chưa hash** — nếu triển khai thật cần đổi sang BCrypt/Argon2 trước khi dùng ngoài phạm vi bài tập.

---

*Tài liệu này mô tả trạng thái code tại thời điểm viết — khi code thay đổi thêm, cập nhật lại các mục liên quan.*
