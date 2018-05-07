const {Given, When, Then} = require('cucumber');
const assert = require('assert');
const request = require('request');

var respCode = null;
var respBody = null;

When('I check the health endpoint', function (callback) {
    // NOTE: I tried to get this working with promises but I kept getting an error
    // about missing the 'render' on undefined... I used examples from
    // https://www.sitepoint.com/bdd-javascript-cucumber-gherkin/
    request(this.rootUrl + '/health', (err, response, body) => {
        if (err) {
            callback(err);
        } else {
            respCode = response.statusCode;
            respBody = body;
            callback();
        }
    });
});

Then('I recieve a healthy response', function () {
    // Write code here that turns the phrase above into concrete actions
    assert.equal(respCode, 200);
});