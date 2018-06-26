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


Given('there is no map data in the database', function () {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('regions')
                let query = { }

                // clear the collection
                dbc.deleteMany(query)

                resolve()
            }
            catch(err){
                reject(err);
            }
            finally{
                db.close()
            }
        })
    })
})

Given('there is no market data in the system', function () {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('marketOrders')
                let query = { }

                // clear the collection
                dbc.deleteMany(query)

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

Given('there is map data in the database', function () {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
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

Given('there is market data in the system for item {int}', function (type_id) {
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try {
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('marketOrders')
                let maxItems = 1000
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

When('I schedule the {string} job', function (job) {
    let test = this
    return new Promise((resolve, reject) => {
        let args = {
            method: 'POST',
            uri: this.v1Url('job'),
            json: true,
            headers: {
                'auth-token': test.token
            },
            body: {
                'jobType': jobMap[job]
            }
        }

        requestDataMap[job](args['body'])

        request(args, (err, response, body) => {
            if (err) {
                reject(err)
            } else {
                test.respCode = response.statusCode
                test.respBody = body
                resolve()
            }
        })
    })
})

Then('a job id is returned', function() {
    let test = this
    return new Promise((resolve, reject) => {
        let jobUuid = test.respBody['data']['uuid']
        jobId = jobUuid
        if (!jobUuid){
            reject('job uuid not found!')
        } else {
            resolve();
        }
    })
})

Then('the database has the appropriate indexes applied', function () {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let jobUuid = test.respBody['data']['uuid']
    return this.waitForJobToComplete(jobUuid).then((job) => {
        return new Promise((resolve) => {
            assert.equal(job['status'], 'Successful')
            resolve()
        })
    })
});

Then('the database has the appropriate map data', function () {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let jobUuid = test.respBody['data']['uuid']
    return test.waitForJobToComplete(jobUuid).then((job) => {
        return new Promise((resolve, reject) => {
            assert.equal(job['status'], 'Successful')

            MongoClient.connect(test.mongoUrl, function(err, db) {
                try{
                    if (err) reject(err)
                    let dbo = db.db(test.appDb)
                    let dbc = dbo.collection('regions')
                    let query = { }

                    dbc.find(query).toArray(function(err, result){
                        if (err) throw err;
                        assert.equal(result.length, 10)

                        let item = result[0];
                        assert.equal(item.constellationIds.length, 5)
                    })
                    resolve()
                }
                catch(err){
                    reject(err);
                }
                finally{
                    db.close()
                }
            })

            resolve()
        })
    })
})

Then('the database has the appropriate market data', function () {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    let jobUuid = test.respBody['data']['uuid']
    return test.waitForJobToComplete(jobUuid).then((job) => {
        return new Promise((resolve, reject) => {
            assert.equal(job['status'], 'Successful')

            MongoClient.connect(test.mongoUrl, function(err, db) {
                try{
                    if (err) reject(err)
                    let dbo = db.db(test.appDb)
                    let dbc = dbo.collection('regions')
                    let query = { }

                    let count = dbc.count(query)
                    assert.equal(count, 2200)
                    resolve()
                }
                catch(err){
                    reject(err);
                }
                finally{
                    db.close()
                }
            })

            resolve()
        })
    })
});
