# Hướng dẫn chạy Unit Tests

## Trong Visual Studio

### Cách 1: Test Explorer (Khuyến nghị)
1. Mở **Test Explorer**: `Test` → `Test Explorer` (hoặc `Ctrl+E, T`)
2. Build solution: `Build` → `Build Solution` (`Ctrl+Shift+B`)
3. Chạy tests:
   - **Run All**: Click nút "Run All" hoặc `Ctrl+R, A`
   - **Run Selected**: Chọn test(s) và click "Run Selected Tests"
   - **Debug Test**: Click phải → "Debug Test"

### Cách 2: CodeLens
- Mở file test (ví dụ: `UserServiceTests.cs`)
- Trên dòng `[Fact]`, click vào link "Run Test" hoặc "Debug Test"

### Cách 3: Menu Test
- `Test` → `Run All Tests` (`Ctrl+R, A`)
- `Test` → `Debug All Tests` (`Ctrl+R, Ctrl+A`)

## Trong Terminal/Command Line

### Sử dụng dotnet CLI
```bash
# Chạy tất cả tests
dotnet test

# Chạy với output chi tiết
dotnet test --verbosity normal

# Chạy một test class cụ thể
dotnet test --filter "FullyQualifiedName~UserServiceTests"

# Chạy một test method cụ thể
dotnet test --filter "FullyQualifiedName~LoginUser_ValidCredentials_ReturnsUser"

# Chạy với code coverage
dotnet test /p:CollectCoverage=true
```

### Sử dụng Visual Studio Developer Command Prompt
```bash
# Navigate to solution folder
cd "C:\Users\Admin\Downloads\se07201_aspnetcoremvc-main"

# Run tests
dotnet test SIMS.Tests\SIMS.Tests.csproj
```

## Danh sách 5 Unit Tests hiện có:

1. **UserServiceTests.cs**
   - `LoginUser_ValidCredentials_ReturnsUser` - Test login page

2. **StudentServiceTests.cs**
   - `GetAllStudentsAsync_ReturnsAllStudents` - Test list students
   - `CreateStudentAsync_ValidData_ReturnsStudent` - Test add student page

3. **CourseServiceTests.cs**
   - `CreateCourseAsync_ValidData_ReturnsCourse` - Test add course page

4. **ActivityLogServiceTests.cs**
   - `LogActivityAsync_ValidData_CreatesActivityLog` - Test activity log

## Kết quả mong đợi

Khi chạy thành công, bạn sẽ thấy:
- ✅ 5 tests passed
- Test Explorer hiển thị tất cả tests với dấu tick xanh
- Output window hiển thị: "Test Run Successful. 5 tests passed"





