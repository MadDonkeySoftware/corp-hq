var rabbitSettings = {
    hosts: [
        {address: "rabbit"}
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
};
var eveDataUri = "http://test-eve-api:3000";

var updateSetting = function (key, value) {
    try {
        db.settings.replaceOne({ key: key }, { key: key, value: value }, { upsert: true })
    } catch (error) {
        db.settings.remove({ key: key })
        db.settings.insert({ key: key , value: value })
    }
}

updateSetting("rabbitConnection", rabbitSettings)
updateSetting("eveDataUri", eveDataUri)