Feature: Health

  Verify the integrity of the health endpoint

  Scenario: System is Active
    When I check the health endpoint
    Then I recieve a healthy response