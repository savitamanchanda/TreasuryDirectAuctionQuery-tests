# TreasuryDirect Auction Query - API Automation Tests

---

## **TABLE OF CONTENT**

* [Project Overview](#project-overview)

* [Feature Tested](#features-tested)

* [Prerequisites](#prerequisites)

* [Installation Instructions](#installation-instructions)

---

## Project Overview

This project is an automated BDD test suite designed to validate the **Treasury Direct Securities API** available at  
[`https://treasurydirect.gov/TA_WS/securities/search`](https://treasurydirect.gov/TA_WS/securities/search).

It uses **C#**, **Reqnroll**, and **NUnit** for test execution and assertions.  
Each scenario is defined in Gherkin syntax and mapped to C# step definitions under the `Steps` folder.

## Features Tested

**Treasury Direct Securities API**

### Scenarios

- **Retrieve securities by a specific CUSIP**: Verifies correct filtering and valid JSON response.
  
- **Search for “Note” securities in a date range**: Validates issueDate and type for all records.
  
- **Request data in XHTML format**: Verifies Content-Type header and valid HTML response body.
  
- **Search for a non-existing CUSIP**: Ensures response is an empty JSON array [].
  
- **Search with invalid date format**: Ensures proper 400 Bad Request error message is returned.
  
- **Search for securities by reopening flag**: Confirms filtering by “reopening” field.
  
- **Pagination Validation**: Ensures results differ between page 1 and page 2.

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)

## Installation instructions

* **Clone this repository**
   ```bash
   git clone https://github.com/savitamanchanda/TreasuryDirectAuctionQuery-tests.git
   cd TreasuryDirectAuctionQuery-tests

* **Restore project dependencies**
   dotnet restore

* **(Optional) Verify the project builds successfully**
   dotnet build

* **Running the Tests**
   dotnet test



