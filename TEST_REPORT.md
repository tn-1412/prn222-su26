# BÁO CÁO KIỂM THỬ — RaceMS Web MVC App (NguyenDNT)

> **Ngày kiểm thử:** 2026-07-01
> **Tester:** Claude Sonnet 5 — kiểm thử **trực tiếp qua HTTP** (curl + cookie jar có xác thực, đối chiếu dữ liệu trong SQL Server), không phải chỉ đọc code tĩnh
> **Môi trường:** .NET 8.0 · ASP.NET Core MVC · SQL Server (`ASUS-1412\SQLEXPRESS` · DB: `RaceManagementDB`) · App chạy tại `https://localhost:7229`

---

## I. CẤU TRÚC PROJECT

Kiến trúc 4 tầng (layered), tách theo project riêng trong solution:

| Project | Vai trò |
|---|---|
| `RaceMS.Entities.NguyenDNT` | POCO entity models (EF Core, auto-generated từ DB): `JockeyNguyenDnt`, `RegistrationNguyenDnt`, `SystemUserAccount` |
| `RaceMS_Repositories.NguyenDNT` | Data access — `GenericRepository<T>` dùng chung + repository riêng cho từng entity, `RaceManagementDBContext` |
| `RaceMS_Services.NguyenDNT` | Business logic + validation, interface `I...Service` cho từng entity |
| `RaceMS.WebMVCApp.NguyenDNT` | MVC Controllers, Razor Views, SignalR Hubs, Program.cs (DI, auth, routing) |

**Entity/module có UI hoàn chỉnh:**

| Module | Route | CRUD | Search (3 trường) | SignalR |
|---|---|---|---|---|
| Jockey | `/JockeyNguyenDnts/*` | ✅ đầy đủ | ✅ FullName/Email/LicenseCode | ✅ Create/Edit/Delete |
| Registration | `/RegistrationNguyenDnts/*` | ✅ đầy đủ (mới thêm) | ✅ Code/PrizeMoney/HorseName | ✅ Create/Edit/Delete |
| Chat | `/Chat/*` | — | — | ✅ ChatHub |
| SystemUserAccount | chỉ dùng để login, **không có UI quản trị** | — | — | — |

---

## II. KẾT QUẢ TEST THEO TỪNG YÊU CẦU

### 1. Authentication / Authorization / "JWT" Cookie

| # | Test | Kết quả |
|---|------|---------|
| Chưa login truy cập `/JockeyNguyenDnts/Index` | Redirect `/Account/Login?ReturnUrl=...` | ✅ PASS |
| Login sai mật khẩu (`acc` / sai pass) | Ở lại Login, hiện "Tên đăng nhập hoặc mật khẩu không đúng." | ✅ PASS |
| Login đúng (`acc` / `@a`) | Redirect `/JockeyNguyenDnts/Index`, `Set-Cookie: .AspNetCore.Cookies=...` (`Secure`, `SameSite=None`, `HttpOnly`) | ✅ PASS |
| Đã login, vào lại `/Account/Login` | Tự redirect sang Index | ✅ PASS |
| SignalR hub negotiate không có cookie | Redirect (302) sang Login — bị chặn | ✅ PASS |
| SignalR hub negotiate có cookie | 200, trả `connectionId` hợp lệ | ✅ PASS |
| `[Authorize]` trên Jockey/Registration/Chat controllers + 2 Hub | Có đầy đủ | ✅ PASS |

**⚠️ Về tên gọi "JWT cookie":** Đã kiểm tra thực tế — cookie đăng nhập **KHÔNG phải JWT**. Giá trị cookie có dạng `CfDJ8...` (định dạng Data Protection chuẩn của ASP.NET Core), không phải 3 đoạn base64url phân cách bằng dấu chấm như JWT thật. Đây là cơ chế **Cookie Authentication cổ điển** (`AddAuthentication().AddCookie(...)` trong `Program.cs`). Gói `System.IdentityModel.Tokens.Jwt` có trong `.csproj` nhưng **không được dùng ở bất kỳ đâu trong code** — là dependency thừa, không có JWT nào thực sự được phát hành. Nếu đề bài yêu cầu bắt buộc phải là JWT, đây là điểm cần bổ sung thực sự (đổi sang JWT Bearer hoặc issue JWT rồi lưu vào cookie thủ công).

**🔴 Bảo mật — Password lưu plaintext (xác nhận, không phải nghi ngờ):** `SystemUserAccountRepository.GetUserAccountAsync` so sánh trực tiếp `x.Password == password` bằng SQL, không hash. Query DB xác nhận cột `Password` chứa giá trị thô (`@a`, `Accountant@`...). Đây là lỗ hổng bảo mật nghiêm trọng nếu dữ liệu thật.

### 2. CRUD + Search 3 trường (2 entity)

| Entity | Create | Edit | Delete | Details | Search (3 trường, AND) |
|---|---|---|---|---|---|
| **Jockey** | ✅ PASS (full flow: tạo → tìm thấy → sửa tên → xác nhận → xóa → 404) | ✅ PASS | ✅ PASS | ✅ PASS (404 khi id không tồn tại) | ✅ PASS — test riêng từng trường (FullName/Email/LicenseCode) + kết hợp AND (không khớp → rỗng, đúng thông báo) + chuỗi trắng → trả toàn bộ + validation trùng LicenseCode hoạt động đúng |
| **Registration** | ⚠️ **Bug tìm thấy & đã fix trong source, cần bạn Restart app để load lại** | ⚠️ tương tự | ✅ PASS (không phụ thuộc view bị lỗi) | ✅ PASS | ✅ PASS — test Code/PrizeMoney/HorseName riêng lẻ, đều khớp đúng |

**Validation đã test và đúng:**
- Jockey: bỏ trống FullName, Weight ngoài khoảng 40–80, trùng LicenseCode → đúng message field-level.
- Registration: FK `JockeyNguyenDntid` không tồn tại → validation error đúng field (sau khi fix bug #2 dưới đây, cần restart để xác nhận lại HTTP-level).

### 3. SignalR

| Test | Kết quả |
|---|---|
| `/msRaceHub/negotiate` yêu cầu auth | ✅ PASS |
| `/chatHub/negotiate` yêu cầu auth | ✅ PASS |
| Hub methods `HubCreate/Edit/Delete_JockeyNguyenDnt` tồn tại, gọi qua HTTP form flow thành công | ✅ PASS (gián tiếp, qua flow CRUD) |
| Hub methods `HubCreate/Edit/Delete_RegistrationNguyenDnt` (mới thêm) | Đã implement, chưa verify runtime do view Create/Edit Registration bị lỗi 500 (xem Bug #2) |
| Real-time 2-tab sync (xóa/sửa/tạo hiển thị tức thời ở tab khác) | **Chưa test được bằng trình duyệt thật** — không có công cụ browser khả dụng trong phiên này (Chrome extension mất kết nối, computer-use bị từ chối quyền, preview sandbox không tương thích HTTPS-only app). Cần bạn tự test bằng 2 tab trình duyệt. |

### 4. Pagination

| Test | Kết quả |
|---|---|
| `table-pagination.js` được include ở cả 2 trang Index | ✅ PASS |
| Jockey: 11 dòng > `rowPerPage:5` → đủ điều kiện phân trang nhiều trang | ✅ PASS |
| Registration: 12 dòng > `rowPerPage:5` → tương tự | ✅ PASS |
| CSS nút phân trang Registration đồng bộ style với Jockey | ✅ Đã sửa (thêm block CSS `-registrationTable` giống `-jockeyTable`) |

### 5. Login / Logout

| Test | Kết quả |
|---|---|
| Login thành công → cookie được set | ✅ PASS |
| Logout → `Set-Cookie` với ngày hết hạn 1970 (xóa cookie `.AspNetCore.Cookies`, `UserName`, `Role`) | ✅ PASS |
| Sau Logout, truy cập trang bảo vệ (Jockey/Registration) → redirect về Login | ✅ PASS |
| Giao diện Login đã làm mới, bỏ "PRN222", bỏ 3 mục feature, bỏ copyright | ✅ Hoàn thành theo yêu cầu trước |

---

## III. BUG PHÁT HIỆN TRONG QUÁ TRÌNH TEST (không phải lỗi giả do công cụ)

### 🔴 Bug #1 — Antiforgery token bị nhân đôi → Create bị chặn hoàn toàn (ĐÃ FIX)
**File:** `Views/JockeyNguyenDnts/Create.cshtml` (lỗi có sẵn từ trước, không phải do tôi gây ra) + `Views/RegistrationNguyenDnts/Create.cshtml`, `Edit.cshtml`, `Delete.cshtml` (tôi vô tình lặp lại pattern này khi viết mới).

**Nguyên nhân:** Form vừa dùng tag helper `asp-action="Create"` (tự động chèn 1 hidden `__RequestVerificationToken`), vừa gọi thêm `@Html.AntiForgeryToken()` thủ công → HTML render ra **2 input cùng tên** `__RequestVerificationToken`. Khi trình duyệt submit, cả 2 giá trị (giống hệt nhau) đều được gửi lên (`name=X&name=X`). ASP.NET Core antiforgery middleware không xử lý được multi-value cho field này → validation luôn fail → **HTTP 400 rỗng, nút "Create"/"Save" không bao giờ hoạt động được**, kể cả với dữ liệu hợp lệ 100%.

**Cách xác nhận:** Tôi tái tạo chính xác request mà trình duyệt thật sẽ gửi (POST với field `__RequestVerificationToken` xuất hiện 2 lần) → **400**. Cùng request nhưng chỉ 1 field → **302 (thành công)**. Đây là bug thật, ảnh hưởng luồng "Create qua form truyền thống" của Jockey từ trước khi tôi bắt đầu làm việc trên project này.

**Đã fix:** Xóa dòng `@Html.AntiForgeryToken()` dư thừa ở cả 4 file, chỉ giữ lại token do tag helper tự sinh.
**Đã verify lại bằng HTTP:** Jockey Create/Edit/Delete full flow chạy đúng 100% sau khi fix (test end-to-end: tạo → tìm → sửa → xác nhận → xóa → 404).

### 🔴 Bug #2 — `asp-for` checkbox crash với kiểu `bool?` → HTTP 500 (ĐÃ FIX trong source, CẦN RESTART APP)
**File:** `Views/RegistrationNguyenDnts/Create.cshtml`, `Edit.cshtml` (lỗi do tôi viết khi thêm CRUD cho Registration).

**Nguyên nhân:** `RegistrationNguyenDnt.IsConfirmed` có kiểu `bool?` (nullable), nhưng tôi viết `<input asp-for="IsConfirmed" type="checkbox" />`. ASP.NET Core's `InputTagHelper` **không hỗ trợ `bool?` cho checkbox** — chỉ nhận `bool` hoặc `string`. Kết quả: **mở trang Create/Edit Registration bị crash HTTP 500** (`System.InvalidOperationException`), toàn bộ chức năng Create/Edit của Registration bị hỏng hoàn toàn.

**Đã fix:** Thay bằng checkbox thủ công (không dùng `asp-for`) + hidden input `value="false"` mô phỏng đúng pattern mà tag helper tự sinh cho `bool` thường:
```html
<input type="hidden" name="IsConfirmed" value="false" />
<input type="checkbox" name="IsConfirmed" id="IsConfirmed" value="true"
       checked="@(Model?.IsConfirmed == true ? "checked" : null)" />
```

**⚠️ CẦN BẠN LÀM:** Fix đã nằm đúng trong file trên đĩa, nhưng **process app đang chạy (bạn start qua Visual Studio) vẫn phục vụ bản Razor view đã biên dịch cũ** (project không bật Razor Runtime Compilation, hoặc Hot Reload không áp dụng được cho thay đổi cấu trúc tag helper này). Bạn cần **Stop debug rồi F5 lại (hoặc Rebuild)** để nạp lại 2 view này, sau đó test lại `/RegistrationNguyenDnts/Create` và `/Edit/{id}` — tôi không thể tự restart vì bạn đang debug qua VS.

### 🟡 Ghi chú — không phải bug
- Trường hợp search `horseName` chứa dấu tiếng Việt trả về rỗng lúc đầu: đã điều tra kỹ, nguyên nhân là **công cụ test của tôi** (Git Bash → curl.exe trên Windows làm hỏng encoding UTF-8 khi truyền tham số có dấu qua argv). Sau khi encode đúng, search hoạt động chính xác — **không phải bug ứng dụng**.

---

## IV. CÁC ĐIỂM CẦN LƯU Ý KHÁC (kế thừa từ review code, chưa phải bug chức năng)

| # | Vấn đề | Mức độ |
|---|--------|--------|
| B-01 | Password lưu plaintext trong DB, so sánh trực tiếp, không hash (BCrypt/Argon2...) | 🔴 HIGH |
| B-02 | Cookie đăng nhập không phải JWT dù project được gọi là có "JWT cookie" — chỉ là Cookie Authentication chuẩn | 🟡 MEDIUM (tùy yêu cầu đề bài) |
| B-03 | `catch (Exception) { }` nuốt lỗi trong `AccountController.Login` — lỗi kết nối DB sẽ hiện nhầm thành "sai mật khẩu", khó debug | 🟢 LOW |
| B-04 | Session hết hạn sau 30 phút không gia hạn (`IsPersistent=false`), không cảnh báo trước | 🟢 LOW |
| B-05 | `SystemUserAccount` không có UI quản trị (chỉ dùng để login) | INFO |
| B-06 | Registration Create/Edit qua SignalR redirect về Index thay vì cập nhật row tức thời như Jockey (đơn giản hóa có chủ đích, đã báo trước) | INFO |

---

## V. TỔNG KẾT

| Nhóm chức năng | Trạng thái |
|---|---|
| Authentication / Authorization | ✅ Hoạt động đúng |
| Cookie (không phải JWT thật) | ⚠️ Cần làm rõ với yêu cầu đề bài |
| CRUD + Search 3 trường — Jockey | ✅ Test đầy đủ, PASS 100% |
| CRUD + Search 3 trường — Registration | ✅ Search/Delete/Details PASS · ⚠️ Create/Edit đã fix, **chờ bạn restart app để xác nhận lại** |
| SignalR (auth, hub methods) | ✅ PASS ở tầng có thể test qua HTTP · ⚠️ Real-time 2-tab cần test thủ công bằng trình duyệt |
| Pagination | ✅ PASS, đồng bộ style 2 trang |
| Login / Logout | ✅ PASS hoàn toàn |

**Việc cần làm tiếp theo (theo thứ tự ưu tiên):**
1. **Restart app trong Visual Studio** để nạp lại fix Bug #2, sau đó thử tạo/sửa 1 Registration để xác nhận.
2. Tự test SignalR 2-tab bằng trình duyệt thật (tạo/sửa/xóa Jockey và Registration ở tab 1, xem tab 2 có cập nhật tức thời không).
3. Cân nhắc hash password nếu đây không chỉ là bài tập demo.
4. Quyết định có cần chuyển sang JWT thật hay giữ Cookie Auth (tùy rubric của đề bài).
