Feature: Jobs

  Verify that jobs schedule and process completely

  Scenario: Schedule apply indexes
    When I schedule the "apply indexes" job
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate indexes applied

  Scenario: Import Map Data
    When I schedule the "import map data" job
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate map data

  Scenario: Import Market Data
    When I schedule the "import market data" job
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate market data