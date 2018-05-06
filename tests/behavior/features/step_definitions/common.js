const {Given, When, Then} = require('cucumber')
const assert = require('assert')

let sharedDelay = function(delay) {
    this.sleep(parseFloat(delay * 1000))
}

Given('wait for {int} seconds', sharedDelay)

Then('the response code is {int}', function(code) {
    assert.equal(code, this.respCode)
})
