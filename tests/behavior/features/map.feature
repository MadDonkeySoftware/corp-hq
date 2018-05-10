Feature: Map data

  Verify that map data is provided accurately.

  Scenario: Map regions are listed appropriately
    Given there are 10 regions in the database
    When I query the list region endpoint
    Then the response code is 200
    And I recieve 10 regions
    And the region id and name are present