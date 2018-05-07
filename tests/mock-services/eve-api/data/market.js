let maxItemsMap = {
    1: 1000,
    2: 100
}

let mod = {
    setup: function(server){
        server.get('/markets/:region/orders', (req, res, next) => {
            // /markets/:region/orders?type_id=:type&page=:page

            // Strip the milliseconds off the timestamp since that's how it comes back from the eve api.
            let issued = new Date().toISOString().split(".")[0] + "Z";
            let data = new Array();
            let page = 0;
            if (req.query.page !== undefined){
                page = parseInt(req.query.page)
            }

            if (page == 1 || page == 2) {
                let maxItems = maxItemsMap[page];

                for(let i = 0; i < maxItems; i++){
                    data.push({
                        "duration": 30,
                        "is_buy_order": false,
                        "issued": issued,
                        "location_id": 60005785,
                        "min_volume": 1,
                        "order_id": i + 1000 * page,
                        "price": 5.7,
                        "range": "region",
                        "system_id": 30002048,
                        "type_id": parseInt(req.query.type_id),
                        "volume_remain": 10250,
                        "volume_total": 10250
                    })
                }
            }
            res.jsonp(data)
            next()
        })
    }
}

// NOTE: Can't use ES6 since it blows up for some reason.
module.exports = mod