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
                'endpiont': '127.0.0.1'
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

/*
Given('there is no market data in the system', function (callback) {
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('marketOrders')
            let query = { }

            // clear the collection
            dbc.deleteMany(query)

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

Given('there is map data in the database', function (callback) {
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('regions')
            let regionIds = [ 10000001, 10000002, 10000003, 10000004, 10000005, 10000006, 10000007, 10000008, 10000009, 10000010 ]
            let items = []

            for (let i = 0; i < regionIds.length; i++) {
                let id = regionIds[i]
                items.push({ 
                    "constellationIds": [],
                    "name": "Region" + id,
                    "regionId": parseInt(id)
                })
            }

            // clear the collection
            dbc.deleteMany({})
            dbc.insertMany(items)

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

Given('there is market data in the system for item {int}', function (type_id, callback) {
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try {
            if (err) callback(err)
            let dbo = db.db(test.appDb)
            let dbc = dbo.collection('marketOrders')
            let maxItems = 1000;
            let issued = new Date().toISOString().split(".")[0] + "Z";
            let items = []

            for(let i = 0; i < maxItems; i++) {
                items.push({
                    "duration": 30,
                    "is_buy_order": false,
                    "issued": issued,
                    "location_id": 60005785,
                    "min_volume": 1,
                    "order_id": i + 1000,
                    "price": 5.7,
                    "range": "region",
                    "system_id": 30002048,
                    "type_id": parseInt(type_id),
                    "volume_remain": 10250,
                    "volume_total": 10250
                })
            }

            // clear the collection
            dbc.deleteMany({})
            dbc.insertMany(items)

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

When('I schedule the {string} job', function (job, callback) {
    // TODO: Get this working with promises.
    // NOTE: I tried to get this working with promises but I kept getting an error
    // about missing the 'render' on undefined... I used examples from
    // https://www.sitepoint.com/bdd-javascript-cucumber-gherkin/
    let test = this
    let args = {
        method: 'POST',
        uri: this.v1Url('job'),
        json: true,
        body: {
            "jobType": jobMap[job]
        }
    }

    requestDataMap[job](args['body'])

    request(args, (err, response, body) => {
        if (err) {
            callback(err);
        } else {
            test.respCode = response.statusCode;
            test.respBody = body;
            callback();
        }
    });
});

Then('a job id is returned', function() {
    let jobUuid = this.respBody['data']['uuid']
    jobId = jobUuid
    if (!jobUuid){
        return 'job uuid not found!'
    }
})

Then('the database has the appropriate indexes applied', {timeout: 60 * 1000}, function (callback) {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let verify = function (job, callback) {
        assert.equal(job['status'], 'Successful')
        // TODO: Actually verify the job data
        callback()
    }

    let jobUuid = test.respBody['data']['uuid']
    this.waitForJobToComplete(jobUuid, verify, callback)

});


Then('the database has the appropriate map data', {timeout: 60 * 1000}, function (callback) {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let verify = function (job, callback) {
        assert.equal(job['status'], 'Successful')

        MongoClient.connect(test.mongoUrl, function(err, db) {
            try{
                if (err) callback(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('regions')
                let query = { }

                dbc.find(query).toArray(function(err, result){
                    if (err) throw err;
                    assert.equal(result.length, 10)

                    let item = result[0];
                    assert.equal(item.constellationIds.length, 5)
                })
                callback()
            }
            catch(err){
                callback(err);
            }
            finally{
                db.close()
            }
        });

        callback()
    }

    let jobUuid = test.respBody['data']['uuid']
    this.waitForJobToComplete(jobUuid, verify, callback)

});

Then('the database has the appropriate market data', {timeout: 60 * 1000}, function (callback) {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let verify = function (job, callback) {
        assert.equal(job['status'], 'Successful')

        MongoClient.connect(test.mongoUrl, function(err, db) {
            try{
                if (err) callback(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('marketOrders')
                let query = { }

                let count = dbc.count(query)
                assert.equal(count, 2200)
                callback()
            }
            catch(err){
                callback(err);
            }
            finally{
                db.close()
            }
        });

        callback()
    }

    let jobUuid = test.respBody['data']['uuid']
    this.waitForJobToComplete(jobUuid, verify, callback)

});
*/