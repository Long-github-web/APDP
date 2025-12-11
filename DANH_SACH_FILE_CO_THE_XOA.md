# Danh sách các file có thể xóa mà không ảnh hưởng đến sản phẩm

## 📋 Phân loại file

### ✅ 1. SQL SCRIPTS TẠM THỜI (Đã chạy xong, có thể xóa)

Các file SQL này đã được sử dụng để fix database và có thể xóa an toàn:

- `ADD_STUDENTID_COLUMN.sql`
- `FIX_DATABASE.sql`
- `FIX_STUDENTS_TABLE.sql`
- `FIX_STUDENTS_ALL_COLUMNS.sql`
- `FIX_ALL_TABLES.sql`
- `CHECK_STUDENTS_COLUMNS.sql`
- `CHECK_AND_FIX_TABLES.sql`
- `EXPORT_STUDENTS_AND_STUDENTCOURSES.sql`
- `EXPORT_ALL_TABLES_TO_CSV.sql`
- `EXPORT_TO_FILE.sql`
- `EXPORT_DATABASE_INFO.sql`
- `CHECK_ALL_TABLES_STRUCTURE.sql`

**Lưu ý:** Các file này đã được tích hợp vào `FIX_ALL_ISSUES_COMPLETE.sql`, nên có thể xóa an toàn.

---

### ❌ 2. SQL SCRIPTS KHÔNG LIÊN QUAN (Có thể xóa)

Các file SQL này không thuộc về project SIMS:

- `BlogSystemSchema.sql` - Schema cho hệ thống Blog (không liên quan)
- `Hostel_sql.sql` - Schema cho hệ thống quản lý nhà trọ (không liên quan)

---

### ⚠️ 3. SQL SCRIPTS CẦN GIỮ LẠI

**KHÔNG XÓA** các file sau (cần thiết cho project):

- `CREATE_ACTIVITYLOGS_TABLE.sql` - Cần để tạo bảng ActivityLogs
- `FIX_ALL_ISSUES_COMPLETE.sql` - Script tổng hợp để fix database (có thể cần dùng lại)

---

### 📄 4. DOCUMENTATION FILES (Tùy chọn - có thể xóa)

Các file hướng dẫn, có thể xóa nếu không cần:

- `HUONG_DAN_EXPORT_CSV.md` - Hướng dẫn export CSV
- `HUONG_DAN_TAO_BANG_ACTIVITYLOGS.md` - Hướng dẫn tạo bảng ActivityLogs
- `TESTING_GUIDE.md` - Hướng dẫn testing

**Lưu ý:** Nên giữ lại nếu cần tham khảo sau này.

---

### 🗑️ 5. BUILD ARTIFACTS (Có thể xóa, sẽ tự tạo lại khi build)

Các thư mục này chứa file build, có thể xóa an toàn:

- `SIMS/bin/` - Thư mục chứa file build (DLL, EXE, etc.)
- `SIMS/obj/` - Thư mục chứa file build tạm thời

**Lưu ý:** 
- Các file này sẽ tự động được tạo lại khi chạy `dotnet build` hoặc `dotnet run`
- Có thể xóa để giảm dung lượng project
- Thường được ignore trong Git (.gitignore)

---

### 🧪 6. FILE TEST KHÔNG DÙNG (Có thể xóa)

File test không được sử dụng trong project:

- `SIMS/Views/Test/Login.cshtml` - File test login (không được reference bởi controller nào)

**Lưu ý:** File này chỉ là file test, không ảnh hưởng đến chức năng chính.

---

### ⚙️ 7. FILE CẤU HÌNH USER (Có thể xóa)

File cấu hình cá nhân của Visual Studio:

- `SIMS/SIMS.csproj.user` - File cấu hình user của Visual Studio

**Lưu ý:** 
- File này chứa cấu hình cá nhân (debug profile, scaffolder settings)
- Sẽ tự động tạo lại khi mở project trong Visual Studio
- Không ảnh hưởng đến build hoặc runtime

---

## 📊 Tổng kết

### File có thể xóa ngay (an toàn 100%):
- ✅ 12 SQL scripts tạm thời
- ✅ 2 SQL scripts không liên quan
- ✅ 1 file test không dùng (`SIMS/Views/Test/Login.cshtml`)
- ✅ 1 file cấu hình user (`SIMS/SIMS.csproj.user`)
- ✅ 2 thư mục build artifacts (`SIMS/bin/`, `SIMS/obj/`)

### File tùy chọn (có thể xóa nếu không cần):
- 📄 3 file documentation (.md)

### File KHÔNG được xóa:
- ❌ `CREATE_ACTIVITYLOGS_TABLE.sql`
- ❌ `FIX_ALL_ISSUES_COMPLETE.sql`
- ❌ Tất cả file code (.cs, .cshtml, .json, .css, .js)
- ❌ File cấu hình project (.csproj, appsettings.json, launchSettings.json)

---

## 🚀 Hướng dẫn xóa nhanh

### Xóa SQL scripts tạm thời:
```powershell
Remove-Item ADD_STUDENTID_COLUMN.sql
Remove-Item FIX_DATABASE.sql
Remove-Item FIX_STUDENTS_TABLE.sql
Remove-Item FIX_STUDENTS_ALL_COLUMNS.sql
Remove-Item FIX_ALL_TABLES.sql
Remove-Item CHECK_STUDENTS_COLUMNS.sql
Remove-Item CHECK_AND_FIX_TABLES.sql
Remove-Item EXPORT_STUDENTS_AND_STUDENTCOURSES.sql
Remove-Item EXPORT_ALL_TABLES_TO_CSV.sql
Remove-Item EXPORT_TO_FILE.sql
Remove-Item EXPORT_DATABASE_INFO.sql
Remove-Item CHECK_ALL_TABLES_STRUCTURE.sql
Remove-Item BlogSystemSchema.sql
Remove-Item Hostel_sql.sql
```

### Xóa file test và cấu hình:
```powershell
Remove-Item SIMS\Views\Test\Login.cshtml
Remove-Item SIMS\SIMS.csproj.user
```

### Xóa build artifacts:
```powershell
Remove-Item -Recurse -Force SIMS\bin
Remove-Item -Recurse -Force SIMS\obj
```

---

## ⚠️ Lưu ý quan trọng

1. **Backup trước khi xóa:** Nên backup project trước khi xóa file
2. **Build artifacts:** Có thể xóa an toàn, sẽ tự tạo lại khi build
3. **SQL scripts:** Chỉ xóa sau khi đã chạy và xác nhận database đã được fix đúng
4. **Documentation:** Nên giữ lại nếu cần tham khảo sau này









