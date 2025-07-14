# MeterReadings

## Set up instructions

Run the `MeterReadingsApi` project - this will launch the Swagger UI.

The API uses an In Memory database that is seeded on launch with account data. 

### Nice to Haves With More Time

- A front end client to consume the API
- More & better logging throughout (eventually integration with Application Insights / CloudWatch)
- Use Scalar over Swagger
- Implement Clean Architecture design pattern (especially if the scope was bigger; to benefit from CQRS, Mediatr, FluentValidation)
- Unit tests
- Better exception handling (improves security, do not show stack traces!)
- Append "Utc" to DateTime references (variables, database fields, etc.) to ensure clarity on time zones
- Github actions (CI/CD / Build&Release pipelines) to automate the deployment process and run tests
- Relevant integration tests

### Assumptions Made

- The validation rule regarding meter readings being in the format "NNNNN":
  - I have interpreted this a numeric range from 0 to 99999, as opposed to a string format of "00453".
- Due to the above constraint, `int` is fine for the meter reading value as it does not risk integer overflow.

### Decisions Made

- .NET 8 used as it is latest LTS, created a .NET Core Web API
  - Decided to not use minimal APIs as not widely adopted yet, although could have been good for the smaller size of the project
- Used Swagger for ease & speed, although Scalar is looking better
- N-tier architecture is used for speed, but with flavours of Clean Architecture (specifically separation of concerns)
- Entity Framework Core is used with proper configurations and repositories
  - InMemory database is used for ease of set up with a code first solution (benefits from being easier to track with the DB schema being in source control)
- Ensure to use async/await to allow for non-block procedures
- Business logic & validation
  - All validation rules from the spec are implemented, and there is validation and multiple levels (controller, service, database) 
