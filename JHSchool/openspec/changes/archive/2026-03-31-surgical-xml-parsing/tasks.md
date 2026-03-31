## 1. Refactor StudentRecord XML Parsing

- [x] 1.1 Analyze all `helper.GetText` calls in `StudentRecord.cs` and map them to `XmlNode` access
- [x] 1.2 Remove `DSXmlHelper` instantiation from `StudentRecord` constructor
- [x] 1.3 Implement direct `GetAttribute` and child node iteration for property mapping
- [x] 1.4 Implement any missing helper methods for robust node extraction (e.g., getting text safely from a potentially null node)

## 2. Refactor Related Record Types (Optional/Cleanup)

- [x] 2.1 Check `ClassRecord.cs`, `CourseRecord.cs`, and `TeacherRecord.cs` for similar patterns
- [x] 2.2 Apply surgical parsing to other high-volume record types if they use `DSXmlHelper` for bulk creation

## 3. Verification and Benchmarking

- [x] 3.1 Verify that all student properties are correctly populated from XML (no regression)
- [x] 3.2 Benchmark the `GetAllStudents()` call with a large dataset (e.g., 3,000+ records) and compare CPU/Time with the previous implementation
- [x] 3.3 Verify memory usage during the peak of the data sync process
