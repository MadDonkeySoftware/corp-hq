const {Given, When, Then} = require('cucumber')
const assert = require('assert')

Given('wait for {int} seconds', function (delay) {
    return new Promise((resolve) => {
        this.sleep(parseFloat(delay * 1000))
        resolve()
    })
})

Then('the response code is {int}', function(code) {
    return new Promise((resolve) => {
        assert.equal(code, this.respCode)
        resolve()
    })
})
