# meter-reading-uploads

For an example of using the endpoint please see the ~/Postman directory.

## Background

A simple POST endpoint to accept a CSV file, validate the input againt a series of business rules and persist the data if those rules are met.

For an example of using the endpoint please see the "Postman" directory.

## Design

It was assumed that, in the "real world", this would be hosted in the Cloud (e.g. AWS) and would use the appropriate NoSQL database (e.g. DynamoDB) for data storage. As these are JSON blobs data storage is through JSON files.

Unit test coverage isn't 100%, but I have tried to demonstrate a few different technologies. Testing uses MSTest; I am more familiar with XTest, but faniced trying something new. NSubstitute has been used for mocking dependencies; Moq was considered, but I have a preference for the former.
