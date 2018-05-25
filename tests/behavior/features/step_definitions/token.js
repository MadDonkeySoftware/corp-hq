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

Given('I already have an auth token expiring in {int} seconds', function (expireSeconds, callback) {
    this.token = generateToken()
    this.tokenExpirary = getExpiryTime(expireSeconds * 1000)
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('sessions')
            let item = {
                'expireAt': test.tokenExpirary,
                'key': test.token,
                'username': 'testUser',
                'endpoint': '127.0.0.1'
            }

            // clear the collection
            dbc.insertOne(item)

            callback()
        }
        catch(err){
            callback(err);
        }
        finally{
            db.close()
        }
    });
});

When('I log out of the system', function (callback) {
    let test = this
    let args = {
        method: 'DELETE',
        uri: this.v1Url('token/' + test.token),
        json: true
    }

    request(args, (err, response, body) => {
        if (err) {
            callback(err);
        } else {
            test.respCode = response.statusCode;
            callback();
        }
    });
});

When('I refresh the token with the system', function (callback) {
    let test = this
    let args = {
        method: 'PUT',
        uri: this.v1Url('token/' + test.token),
        json: true
    }

    request(args, (err, response, body) => {
        if (err) {
            callback(err);
        } else {
            test.respCode = response.statusCode;
            callback();
        }
    });
});

Then('a session no longer exists in the system for my token', function (callback) {
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('sessions')
            let query = { 'key': test.token }

            dbc.count(query).then(function (count){
                assert.equal(count, 0)
                callback()
            }).catch(function (reason) {
                callback('Failed: ' + reason)
            })

        }
        catch(err){
            callback(err);
        }
        finally{
            db.close()
        }
    });
})

Then('the session for my token has an updated exipration timestamp', function (callback) {
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('sessions')
            let query = { 'key': test.token }

            dbc.findOne(query).then(function (sessionDocument) {
                if (sessionDocument['expireAt'] <= test.tokenExpirary) {
                    assert.fail('Token expireAt not updated')
                }
                callback()
            }).catch(function (reason) {
                callback('Failed: ' + reason)
            })
        }
        catch(err){
            callback(err);
        }
        finally{
            db.close()
        }
    });
})