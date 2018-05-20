const {Given, When, Then} = require('cucumber')
const assert = require('assert')

Given('wait for {int} seconds', function (delay) {
    this.sleep(parseFloat(delay * 1000))
})

Then('the response code is {int}', function(code) {
    assert.equal(code, this.respCode)
})
