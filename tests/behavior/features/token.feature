Feature: Tokens

  Verify that tokens operate as expected in the system

  Scenario: API delete token removes token from system
    Given I already have an auth token expiring in 10 seconds
    When I log out of the system
    Then the response code is 204
    And a session no longer exists in the system for my token

  Scenario: API Token refresh updates the system 
    Given I already have an auth token expiring in 10 seconds
    When I refresh the token with the system
    Then the response code is 204
    And the session for my token has an updated expiration timestamp

  # I'm not sure we can test this since the mongo cleanup runs similarly to dotnet GC.
  @ignore
  Scenario: Tokens expire from the system appropriately
    Given I already have an auth token expiring in -60 seconds
    And wait for 1 seconds
    Then a session no longer exists in the system for my token
