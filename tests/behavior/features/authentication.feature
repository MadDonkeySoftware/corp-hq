@ignore
Feature: Authentication

  Verify that authentication in the system works appropriately

  Scenario: API authenticate provides new token
    Given I do not have an auth token
    When I authenticate to the system
    Then I have an auth token
    And a session exists in the system for my token