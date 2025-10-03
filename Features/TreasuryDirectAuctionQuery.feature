Feature: Treasury Direct Securities API
  As a client
  I want to query the Treasury Direct Securities API
  So that I can retrieve accurate information about government securities.

  Scenario: Retrieve securities by a specific CUSIP
    When I make a GET request with JSON format to the securities search endpoint with CUSIP "9128283R9"
    Then the response status code should be 200
    And the response body should be a non-empty JSON array
    And every security in the response array should have a "cusip" of "9128283R9"
