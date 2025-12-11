-- Script TỔNG HỢP để sửa TẤT CẢ các vấn đề trong database SIMS
-- Chạy script này một lần để sửa tất cả

USE SIMS;
GO

PRINT '========================================';
PRINT 'BẮT ĐẦU SỬA TẤT CẢ CÁC VẤN ĐỀ';
PRINT '========================================';
PRINT '';

-- ============================================
-- SỬA BẢNG COURSES
-- ============================================
PRINT '=== SỬA BẢNG COURSES ===';
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Courses')
BEGIN
    -- Department
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'Department')
    BEGIN
        ALTER TABLE Courses ADD Department NVARCHAR(100) NULL;
        PRINT '✓ Đã thêm cột Department';
    END
    
    -- CourseCode
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'CourseCode')
    BEGIN
        ALTER TABLE Courses ADD CourseCode NVARCHAR(50) NOT NULL DEFAULT '';
        PRINT '✓ Đã thêm cột CourseCode';
    END
    
    -- CourseName
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'CourseName')
    BEGIN
        ALTER TABLE Courses ADD CourseName NVARCHAR(200) NOT NULL DEFAULT '';
        PRINT '✓ Đã thêm cột CourseName';
    END
    
    -- Description
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'Description')
    BEGIN
        ALTER TABLE Courses ADD Description NVARCHAR(MAX) NULL;
        PRINT '✓ Đã thêm cột Description';
    END
    
    -- Credits (đảm bảo là nullable)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'Credits')
    BEGIN
        ALTER TABLE Courses ADD Credits INT NULL;
        PRINT '✓ Đã thêm cột Credits';
    END
    
    -- Status
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'Status')
    BEGIN
        ALTER TABLE Courses ADD Status NVARCHAR(50) NOT NULL DEFAULT 'Active';
        PRINT '✓ Đã thêm cột Status';
    END
    
    -- CreatedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE Courses ADD CreatedAt DATETIME NULL;
        PRINT '✓ Đã thêm cột CreatedAt';
    END
    
    -- UpdatedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Courses') AND name = 'UpdatedAt')
    BEGIN
        ALTER TABLE Courses ADD UpdatedAt DATETIME NULL;
        PRINT '✓ Đã thêm cột UpdatedAt';
    END
    
    PRINT '✓ Hoàn tất sửa bảng Courses';
END
GO

PRINT '';

-- ============================================
-- SỬA BẢNG STUDENTS
-- ============================================
PRINT '=== SỬA BẢNG STUDENTS ===';
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    -- StudentId (QUAN TRỌNG)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'StudentId')
    BEGIN
        ALTER TABLE Students ADD StudentId NVARCHAR(20) NULL;
        PRINT '✓ Đã thêm cột StudentId';
    END
    
    -- FullName (QUAN TRỌNG)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'FullName')
    BEGIN
        ALTER TABLE Students ADD FullName NVARCHAR(200) NULL;
        PRINT '✓ Đã thêm cột FullName';
    END
    
    -- DateOfBirth
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'DateOfBirth')
    BEGIN
        ALTER TABLE Students ADD DateOfBirth DATETIME NULL;
        PRINT '✓ Đã thêm cột DateOfBirth';
    END
    
    -- Gender
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Gender')
    BEGIN
        ALTER TABLE Students ADD Gender NVARCHAR(10) NULL;
        PRINT '✓ Đã thêm cột Gender';
    END
    
    -- Address
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Address')
    BEGIN
        ALTER TABLE Students ADD Address NVARCHAR(500) NULL;
        PRINT '✓ Đã thêm cột Address';
    END
    
    -- Phone
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Phone')
    BEGIN
        ALTER TABLE Students ADD Phone NVARCHAR(15) NULL;
        PRINT '✓ Đã thêm cột Phone';
    END
    
    -- Email
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Email')
    BEGIN
        ALTER TABLE Students ADD Email NVARCHAR(200) NULL;
        PRINT '✓ Đã thêm cột Email';
    END
    
    -- AcademicProgram
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'AcademicProgram')
    BEGIN
        ALTER TABLE Students ADD AcademicProgram NVARCHAR(200) NULL;
        PRINT '✓ Đã thêm cột AcademicProgram';
    END
    
    -- Year
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Year')
    BEGIN
        ALTER TABLE Students ADD Year INT NULL;
        PRINT '✓ Đã thêm cột Year';
    END
    
    -- GPA
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'GPA')
    BEGIN
        ALTER TABLE Students ADD GPA DECIMAL(18,2) NULL;
        PRINT '✓ Đã thêm cột GPA';
    END
    
    -- UserId
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'UserId')
    BEGIN
        ALTER TABLE Students ADD UserId INT NULL;
        PRINT '✓ Đã thêm cột UserId';
    END
    
    -- EnrollmentDate
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'EnrollmentDate')
    BEGIN
        ALTER TABLE Students ADD EnrollmentDate DATETIME NULL;
        PRINT '✓ Đã thêm cột EnrollmentDate';
    END
    
    -- Status
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'Status')
    BEGIN
        ALTER TABLE Students ADD Status NVARCHAR(50) NULL DEFAULT 'Active';
        PRINT '✓ Đã thêm cột Status';
    END
    
    -- CreatedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE Students ADD CreatedAt DATETIME NULL;
        PRINT '✓ Đã thêm cột CreatedAt';
    END
    
    -- UpdatedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Students') AND name = 'UpdatedAt')
    BEGIN
        ALTER TABLE Students ADD UpdatedAt DATETIME NULL;
        PRINT '✓ Đã thêm cột UpdatedAt';
    END
    
    PRINT '✓ Hoàn tất sửa bảng Students';
END
GO

PRINT '';

-- ============================================
-- SỬA BẢNG STUDENTCOURSES
-- ============================================
PRINT '=== SỬA BẢNG STUDENTCOURSES ===';
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StudentCourses')
BEGIN
    -- StudentId
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentCourses') AND name = 'StudentId')
    BEGIN
        ALTER TABLE StudentCourses ADD StudentId INT NULL;
        PRINT '✓ Đã thêm cột StudentId';
    END
    
    -- CourseId
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentCourses') AND name = 'CourseId')
    BEGIN
        ALTER TABLE StudentCourses ADD CourseId INT NULL;
        PRINT '✓ Đã thêm cột CourseId';
    END
    
    -- EnrollmentDate
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentCourses') AND name = 'EnrollmentDate')
    BEGIN
        ALTER TABLE StudentCourses ADD EnrollmentDate DATETIME NULL;
        PRINT '✓ Đã thêm cột EnrollmentDate';
    END
    
    -- Grade
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentCourses') AND name = 'Grade')
    BEGIN
        ALTER TABLE StudentCourses ADD Grade NVARCHAR(50) NULL;
        PRINT '✓ Đã thêm cột Grade';
    END
    
    -- Status
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentCourses') AND name = 'Status')
    BEGIN
        ALTER TABLE StudentCourses ADD Status NVARCHAR(50) NULL DEFAULT 'Enrolled';
        PRINT '✓ Đã thêm cột Status';
    END
    
    PRINT '✓ Hoàn tất sửa bảng StudentCourses';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT SỬA TẤT CẢ CÁC VẤN ĐỀ';
PRINT '========================================';
PRINT '';
PRINT 'Bây giờ bạn có thể:';
PRINT '1. Rebuild ứng dụng';
PRINT '2. Thử lại các chức năng';
GO










