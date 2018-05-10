Feature: Jobs

  Verify that jobs schedule and process completely

  Scenario: Schedule apply indexes
    When I schedule the "apply indexes" job
    And wait for 1 seconds
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate indexes applied

  Scenario: Import Map Data
    Given there is no map data in the database
    When I schedule the "import map data" job
    And wait for 1 seconds
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate map data

  Scenario: Re-Import Map Data
    Given there is map data in the database
    When I schedule the "import map data" job
    And wait for 1 seconds
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate map data

  Scenario: Import Market Data
    Given there is no market data in the system
    When I schedule the "import market data" job
    And wait for 1 seconds
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate market data

  Scenario: Re-Import Market Data
    Given there is market data in the system for item 34
    And there is market data in the system for item 35
    When I schedule the "import market data" job
    And wait for 1 seconds
    Then the response code is 201
    And a job id is returned
    And the database has the appropriate market data