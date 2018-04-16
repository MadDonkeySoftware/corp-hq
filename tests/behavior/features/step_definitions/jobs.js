const {Given, When, Then} = require('cucumber')
const assert = require('assert')
const request = require('request')
const {MongoClient} = require('mongodb')

let emptyMapDataBodyBuilder = function (body) { }
let importMapDataBodyBuilder = function(body) {
    body['data'] = {
        "regionId": 10000042,
        "marketIds": [34, 35]
    }
}

let jobMap = {
    "apply indexes": "ApplyDbIndexes",
    "import map data": "ImportMapData",
    "import market data": "ImportMarketData",
}
let requestDataMap = {
    "apply indexes": emptyMapDataBodyBuilder,
    "import map data": emptyMapDataBodyBuilder,
    "import market data": importMapDataBodyBuilder,
}

let respCode = null
let respBody = null

When('I schedule the {string} job', function (job, callback) {
    // TODO: Get this working with promises.
    // NOTE: I tried to get this working with promises but I kept getting an error
    // about missing the 'render' on undefined... I used examples from
    // https://www.sitepoint.com/bdd-javascript-cucumber-gherkin/
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
            respCode = response.statusCode;
            respBody = body;
            callback();
        }
    });
});

Then('the response code is {int}', function(code) {
    assert.equal(code, respCode)
})

Then('a job id is returned', function() {
    let jobUuid = respBody['data']['uuid']
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

    let jobUuid = respBody['data']['uuid']
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

    let jobUuid = respBody['data']['uuid']
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

    let jobUuid = respBody['data']['uuid']
    this.waitForJobToComplete(jobUuid, verify, callback)

});