var rabbitSettings = {
    hosts: [
        {address: "rabbit"},
        // {address: "localhost"}
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
};
var eveDataUri = "https://esi.tech.ccp.is/latest";

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
