Feature: Treasury Direct Securities API
  As a client
  I want to query the Treasury Direct Securities API
  So that I can retrieve accurate information about government securities.

  Scenario: Retrieve securities by a specific CUSIP
    When I make a GET request with JSON format to the securities search endpoint with CUSIP "9128283R9"
    Then the response status code should be 200
    And the response body should be a non-empty JSON array
    And every security in the response array should have a "cusip" of "9128283R9"

  Scenario: Search for all "Note" securities within a specific date range
    When I make a GET request with JSON format to search for "Note" securities between "01/01/2024" and "03/31/2024"
    Then the response status code should be 200
    And the response body should be a non-empty JSON array
    And every security in the response array should have a "securityType" of "Note"
    And every security's "issueDate" should be within the requested date range
    
  Scenario: Request data in XHTML format
    When I make a GET request to search for "Bond" securities with the format set to "xhtml"
    Then the response status code should be 200
    And the response "Content-Type" header should contain "application/xhtml+xml"
    And the response body should be valid XHTML
