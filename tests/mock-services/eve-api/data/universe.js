let mod = {
    setup: function(server){
        server.get('/universe/regions', (req, res, next) => {
            res.jsonp(mod.getRegions())
            next()
        })
        server.get('/universe/regions/:id', (req, res, next) => {
            res.jsonp(mod.getRegionDetails(req.params.id))
            next()
        })
    },
    getRegions: function () {
        return [ 10000001, 10000002, 10000003, 10000004, 10000005, 10000006, 10000007, 10000008, 10000009, 10000010 ]
    },
    getRegionDetails: function(id) {
        return { 
            "constellations": mod.generateIdArray(parseInt(id)),
            "description": "Region " + id +" description.",
            "name": "Region" + id,
            "region_id": parseInt(id)
        }
    },
    generateIdArray: function(start, length = 5, step = 1){
        let data = new Array(length)
        for (var i=0; i<length; i++) {
            data[i] = start + i
        }
        return data
    }
}

// NOTE: Can't use ES6 since it blows up for some reason.
module.exports = mod