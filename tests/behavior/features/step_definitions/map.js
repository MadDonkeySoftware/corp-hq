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

Given('there are {int} regions in the database', function (total) {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    return new Promise((resolve, reject) => {
        MongoClient.connect(test.mongoUrl, function(err, db) {
            try{
                if (err) reject(err)
                let dbo = db.db(test.appDb)
                let dbc = dbo.collection('regions')
                let query = { }
                let baseRegionId = 1000000
                let items = []

                // clear the collection
                dbc.deleteMany(query)

                for(let i = 0; i < total; i++){
                    items.push(
                        {
                            "regionId": baseRegionId + i,
                            "name": "Region-" + (baseRegionId + i)
                        }
                    )
                }

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

When('I query the list region endpoint', function () {
    let test = this
    return new Promise((resolve, reject) => {
        let args = {
            method: 'GET',
            uri: this.v1Url('map/regions'),
            headers: {
                'auth-token': test.token
            },
            json: true
        }

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

Then('I receive {int} regions', function (expectedRegionsCount) {
    let test = this
    return new Promise((resolve) => {
        let regions = test.respBody['data']
        assert.equal(expectedRegionsCount, regions.length)
        resolve()
    })
})

Then('the region id and name are present', function () {
    let test = this
    return new Promise((resolve) => {
        let regions = test.respBody['data']
        regions.forEach(function(element) {
            if (!("name" in element)) {
                console.debug(element)
                assert.fail("Name not found in result set.")
            }
            if (!("id" in element)) {
                console.debug(element)
                assert.fail("Id not found in result set.")
            }
        })
        resolve()
    })
})
