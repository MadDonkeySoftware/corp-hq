const {Given, When, Then} = require('cucumber')
const assert = require('assert')
const request = require('request')
const {MongoClient} = require('mongodb')

const defaultDelta = 30 * 60 * 1000  // 30 minutes
let getExpiryTime = function (futureDelta = defaultDelta) {
    return new Date(new Date().getTime() + futureDelta)
}

let generateToken = function (length = 40) {
    var text = ''
    var possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'

    for (var i = 0; i < length; i++) {
      text += possible.charAt(Math.floor(Math.random() * possible.length))
    }

    return text
}

Given('I already have an auth token expiring in {int} seconds', function (expireSeconds) {
    this.token = generateToken()
    this.tokenExpiry = getExpiryTime(expireSeconds * 1000)
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('sessions')
                let item = {
                    'expireAt': test.tokenExpiry,
                    'key': test.token,
                    'username': 'testUser',
                    'endpoint': '127.0.0.1'
                }

                // clear the collection
                dbc.insertOne(item)

                resolve()
            }
            catch(err){
                reject(err)
            }
            finally{
                db.close()
            }
        })
    })
})

When('I log out of the system', function () {
    let test = this
    return new Promise((resolve, reject) => {
        let args = {
            method: 'DELETE',
            uri: this.v1Url('token/' + test.token),
            json: true
        }

        request(args, (err, response, body) => {
            if (err) {
                reject(err)
            } else {
                test.respCode = response.statusCode
                resolve()
            }
        })
    })
})

When('I refresh the token with the system', function () {
    let test = this
    return new Promise((resolve, reject) => {
        let args = {
            method: 'PUT',
            uri: this.v1Url('token/' + test.token),
            json: true
        }

        request(args, (err, response, body) => {
            if (err) {
                reject(err)
            } else {
                test.respCode = response.statusCode
                resolve()
            }
        })
    })
})

Then('a session no longer exists in the system for my token', function () {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('sessions')
                let query = { 'key': test.token }

                dbc.count(query).then(function (count){
                    assert.equal(count, 0)
                    resolve()
                }).catch(function (reason) {
                    reject('Failed: ' + reason)
                })

            }
            catch(err){
                reject(err)
            }
            finally{
                db.close()
            }
        })
    })
})

Then('the session for my token has an updated expiration timestamp', function () {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('sessions')
                let query = { 'key': test.token }

                dbc.findOne(query).then(function (sessionDocument) {
                    if (sessionDocument['expireAt'] <= test.tokenExpiry) {
                        assert.fail('Token expireAt not updated')
                    }
                    resolve()
                }).catch(function (reason) {
                    reject('Failed: ' + reason)
                })
            }
            catch(err){
                reject(err)
            }
            finally{
                db.close()
            }
        })
    })
})