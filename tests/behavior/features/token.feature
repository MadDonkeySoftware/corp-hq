@ignore
Feature: Tokens

  Verify that tokens operate as expected in the system

  Scenario: API authenticate provides new token
    Given I do not have an auth token
    When I authenticate to the system 
    Then I have an auth token
    And a session exists in the system for my token

  Scenario: API delete token removes token from system

  Scenario: API Token refresh updates the system 

  Scenario: Tokens expire from the system appropriately
