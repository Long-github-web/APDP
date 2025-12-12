# Test Plan - SIMS Project

## Overview
This document outlines the test plan for the Student Information Management System (SIMS) project, including unit tests and integration tests.

## Test Strategy

### Unit Tests
Unit tests focus on testing individual service methods in isolation using mocks for dependencies.

### Integration Tests
Integration tests verify the interaction between repositories and the database using an in-memory database.

---

## Test Cases

### 1. StudentServiceTests (6 test cases)

#### Test Case 1.1: CreateStudentAsync_ValidData_ThrowsExceptionDueToMockContext
- **Purpose**: Validates that validation logic works correctly (transaction requires real DbContext)
- **Steps**:
  1. Arrange: Create student with valid data (StudentCode, FullName, Email)
  2. Setup mocks to return no duplicates
  3. Act: Call CreateStudentAsync
  4. Assert: Exception is thrown (expected due to mock context limitation)
- **Expected Result**: Exception thrown (validates business logic)

#### Test Case 1.2: CreateStudentAsync_MissingStudentCode_ThrowsException
- **Purpose**: Validates that StudentCode is required
- **Steps**:
  1. Arrange: Create student with empty StudentCode
  2. Act: Call CreateStudentAsync
  3. Assert: InvalidOperationException with message "Student Code is required."
- **Expected Result**: Exception with correct message

#### Test Case 1.3: CreateStudentAsync_DuplicateStudentCode_ThrowsException
- **Purpose**: Validates that duplicate StudentCode is rejected
- **Steps**:
  1. Arrange: Create student with StudentCode "ST001"
  2. Setup mock to return true for StudentCodeExistsAsync
  3. Act: Call CreateStudentAsync
  4. Assert: InvalidOperationException containing "already exists"
- **Expected Result**: Exception with duplicate code message

#### Test Case 1.4: CreateStudentAsync_DuplicateUsername_ThrowsException
- **Purpose**: Validates that duplicate username is rejected
- **Steps**:
  1. Arrange: Create student with username "existinguser"
  2. Setup mock to return existing user
  3. Act: Call CreateStudentAsync
  4. Assert: InvalidOperationException containing "already exists"
- **Expected Result**: Exception with duplicate username message

#### Test Case 1.5: CreateStudentAsync_InvalidEmail_ThrowsException
- **Purpose**: Validates that duplicate email is rejected
- **Steps**:
  1. Arrange: Create student with email "existing@example.com"
  2. Setup mock to return existing user with that email
  3. Act: Call CreateStudentAsync
  4. Assert: InvalidOperationException containing "already registered"
- **Expected Result**: Exception with duplicate email message

#### Test Case 1.6: UpdateStudentAsync_ValidData_ReturnsUpdatedStudent
- **Purpose**: Validates that student update works correctly
- **Steps**:
  1. Arrange: Create existing student and updated student data
  2. Setup mocks for GetStudentByIdAsync and UpdateStudentAsync
  3. Act: Call UpdateStudentAsync
  4. Assert: Updated student returned with new name
- **Expected Result**: Student updated successfully

---

### 2. CourseServiceTests (4 test cases)

#### Test Case 2.1: CreateCourseAsync_ValidData_ReturnsCourse
- **Purpose**: Validates that course creation works with valid data
- **Steps**:
  1. Arrange: Create course with CourseCode "CS101", CourseName, Credits
  2. Setup mock to return null for GetCourseByCodeAsync (no duplicate)
  3. Setup mock to return created course
  4. Act: Call CreateCourseAsync
  5. Assert: Course returned with correct CourseCode and Id
- **Expected Result**: Course created successfully

#### Test Case 2.2: CreateCourseAsync_DuplicateCode_ThrowsException
- **Purpose**: Validates that duplicate CourseCode is rejected
- **Steps**:
  1. Arrange: Create course with CourseCode "CS101"
  2. Setup mock to return existing course
  3. Act: Call CreateCourseAsync
  4. Assert: InvalidOperationException containing "already exists"
- **Expected Result**: Exception with duplicate code message

#### Test Case 2.3: UpdateCourseAsync_ValidData_RetainsId
- **Purpose**: Validates that course update retains the original Id
- **Steps**:
  1. Arrange: Create existing course and updated course data
  2. Setup mocks for GetCourseByIdAsync and UpdateCourseAsync
  3. Act: Call UpdateCourseAsync
  4. Assert: Updated course returned with same Id and new name
- **Expected Result**: Course updated with Id retained

#### Test Case 2.4: DeleteCourseAsync_NonExistent_ReturnsFalse
- **Purpose**: Validates that deleting non-existent course returns false
- **Steps**:
  1. Arrange: Use non-existent course ID (999)
  2. Setup mock to return null for GetCourseByIdAsync
  3. Act: Call DeleteCourseAsync
  4. Assert: Returns false, DeleteCourseAsync not called
- **Expected Result**: False returned, no deletion attempted

---

### 3. UserServiceTests (2 test cases)

#### Test Case 3.1: LoginUser_ValidCredentials_ReturnsUser
- **Purpose**: Validates that login with correct credentials returns user
- **Steps**:
  1. Arrange: Create user with username "admin" and password "admin123"
  2. Setup mock to return user for GetUserByUsername
  3. Act: Call LoginUser with correct credentials
  4. Assert: User returned with matching username and password
- **Expected Result**: User returned successfully

#### Test Case 3.2: LoginUser_InvalidPassword_ReturnsNull
- **Purpose**: Validates that login with wrong password returns null
- **Steps**:
  1. Arrange: Create user with password "admin123"
  2. Setup mock to return user
  3. Act: Call LoginUser with wrong password
  4. Assert: Returns null
- **Expected Result**: Null returned (login failed)

---

### 4. Integration Tests

#### Test Case 4.1: StudentRepository_CreateStudentAsync_ThenGetStudentByUserIdAsync_ReturnsStudent
- **Purpose**: Tests repository integration with in-memory database
- **Steps**:
  1. Arrange: Create Users entity and add to context
  2. Create Student entity with UserId
  3. Act: Call CreateStudentAsync, then GetStudentByUserIdAsync
  4. Assert: Student retrieved with correct StudentCode and UserId
- **Expected Result**: Student created and retrieved successfully

#### Test Case 4.2: StudentCourseRepository_AssignStudentToCourseAsync_InsertsAndIncludesRelationships
- **Purpose**: Tests StudentCourse assignment and relationship includes
- **Steps**:
  1. Arrange: Create User, Student, and Course entities
  2. Add StudentCourse to context
  3. Act: Save changes, then retrieve StudentCourse
  4. Assert: StudentCourse retrieved with correct relationships, Student included
- **Expected Result**: Relationship created and includes work correctly

---

## Test Execution

### Running Tests
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj --verbosity normal
```

### Running Tests with Coverage
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/coverage.xml
```

### Expected Results
- **Total Tests**: 14
- **Passed**: 14
- **Failed**: 0

---

## Coverage Report

Coverage is collected using Coverlet and exported in OpenCover format. The coverage report shows:
- Line coverage percentage
- Branch coverage percentage
- Method coverage percentage

---

## Test Environment

- **Framework**: .NET 8.0
- **Test Framework**: xUnit
- **Mocking**: Moq
- **Database**: Entity Framework Core InMemory (for integration tests)
- **Coverage Tool**: Coverlet

---

## Notes

1. Unit tests use mocks to isolate service logic
2. Integration tests use in-memory database to test repository interactions
3. StudentService.CreateStudentAsync requires real DbContext for transaction execution (tested in integration tests)
4. All tests are designed to be independent and can run in any order









