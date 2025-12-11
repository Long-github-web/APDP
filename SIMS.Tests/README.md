# SIMS Tests

This directory contains unit tests and integration tests for the Student Information Management System (SIMS) project.

## Test Structure

### Unit Tests
- **StudentServiceTests.cs**: 6 test cases for student service validation and business logic
- **CourseServiceTests.cs**: 4 test cases for course service operations
- **UserServiceTests.cs**: 2 test cases for user authentication

### Integration Tests
- **RepositoryIntegrationTests.cs**: 2 test cases for repository database interactions

## Running Tests

### Basic Test Execution
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj
```

### Verbose Output
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj --verbosity normal
```

### List All Tests
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj --list-tests
```

## Code Coverage

### Using Coverlet (Recommended)
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/coverage.xml
```

### Using XPlat Code Coverage
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
```

### View Coverage Summary
After running with coverage, the summary will be displayed in the console:
```
+---------+--------+--------+--------+
|         | Line   | Branch | Method |
+---------+--------+--------+--------+
| Total   | 10.67% | 3.34%  | 19.39% |
+---------+--------+--------+--------+
```

## Test Reports

### Generate TRX Report
```bash
dotnet test SIMS.Tests/SIMS.Tests.csproj --logger "trx;LogFileName=test-results.trx" --results-directory:"./TestResults"
```

### Coverage Report Location
- **Coverlet**: `TestResults/coverage.xml` (OpenCover format)
- **XPlat**: `TestResults/{guid}/coverage.cobertura.xml` (Cobertura format)

## Test Cases Summary

### StudentServiceTests (6 tests)
1. ✅ CreateStudentAsync_ValidData_ReturnsStudent
2. ✅ CreateStudentAsync_MissingStudentCode_ThrowsException
3. ✅ CreateStudentAsync_DuplicateStudentCode_ThrowsException
4. ✅ CreateStudentAsync_DuplicateUsername_ThrowsException
5. ✅ CreateStudentAsync_InvalidEmail_ThrowsException
6. ✅ UpdateStudentAsync_ValidData_ReturnsUpdatedStudent

### CourseServiceTests (4 tests)
1. ✅ CreateCourseAsync_ValidData_ReturnsCourse
2. ✅ CreateCourseAsync_DuplicateCode_ThrowsException
3. ✅ UpdateCourseAsync_ValidData_RetainsId
4. ✅ DeleteCourseAsync_NonExistent_ReturnsFalse

### UserServiceTests (2 tests)
1. ✅ LoginUser_ValidCredentials_ReturnsUser
2. ✅ LoginUser_InvalidPassword_ReturnsNull

### RepositoryIntegrationTests (2 tests)
1. ✅ StudentRepository_CreateStudentAsync_ThenGetStudentByUserIdAsync_ReturnsStudent
2. ✅ StudentCourseRepository_AssignStudentToCourseAsync_InsertsAndIncludesRelationships

## Dependencies

- **xUnit**: Test framework
- **Moq**: Mocking framework for unit tests
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for integration tests
- **coverlet.collector**: Code coverage collection
- **coverlet.msbuild**: Code coverage build integration

## Documentation

- **TEST_PLAN.md**: Detailed test plan with step-by-step test scenarios
- **TEST_EXECUTION_SUMMARY.md**: Test execution results and coverage summary

## Expected Results

- **Total Tests**: 14
- **Passed**: 14
- **Failed**: 0
- **Success Rate**: 100%

All tests should pass successfully when executed.





