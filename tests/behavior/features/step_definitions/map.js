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

Given('there are {int} regions in the database', function (total, callback) {
    // Write code here that turns the phrase above into concrete actions
    let test = this
    MongoClient.connect(test.mongoUrl, function(err, db) {
        try{
            if (err) callback(err)
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


When('I query the list region endpoint', function (callback) {
    let test = this
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
            callback(err);
        } else {
            test.respCode = response.statusCode;
            test.respBody = body;
            callback();
        }
    });
});

Then('I recieve {int} regions', function (expectedRegionsCount) {
    // Write code here that turns the phrase above into concrete actions
    let regions = this.respBody['data']
    assert.equal(expectedRegionsCount, regions.length)
});

Then('the region id and name are present', function () {
    // Write code here that turns the phrase above into concrete actions
    let regions = this.respBody['data']
    regions.forEach(function(element) {
        if (!("name" in element)) {
            console.debug(element)
            assert.fail("Name not found in result set.")
        }
        if (!("id" in element)) {
            console.debug(element)
            assert.fail("Id not found in result set.")
        }
    });
});
