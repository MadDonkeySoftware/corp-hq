const {Given, When, Then} = require('cucumber');
const assert = require('assert');
const request = require('request');

var respCode = null

When('I check the health endpoint', function () {
    return new Promise((resolve) => {
        request(this.rootUrl + '/health', (err, response, body) => {
            if (err) {
                callback(err)
            } else {
                respCode = response.statusCode
            }
            resolve()
        })
    })
})

Then('I recieve a healthy response', function () {
    return new Promise((resolve) => {
        assert.equal(respCode, 200)
        resolve()
    })
});