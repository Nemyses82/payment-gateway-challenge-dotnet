# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - Host with API Controller as an entry point for handling Payment
    PaymentGateway.Processor - Collection of services which isolate the logic for external concerns (e.g. Repository, Payment service provider)
test/
    PaymentGateway.Api.Tests - Set of Unit/Integration tests for API Host project
    PaymentGateway.Processor.Tests - Set of Unit/Integration tests for processing services
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

## Note of improvements
* Code coverage does not reach the 100% on purpose as I thought to leave some space of improvements for a second interview phase: for example discussing with interviewers till at which point is preferable cover the implementation with further tests
* InMemoryCaching integration for repeated API calls for retrieving Payment details
* Integration of Acceptance tests for verifying that End-User requirements are met
* ...