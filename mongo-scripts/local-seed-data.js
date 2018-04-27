var rabbitSettings = {
    hosts: [
        // {address: "rabbit"},
        {address: "localhost"}
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
};

db.settings.replaceOne({ key: "rabbitConnection" }, { key: "rabbitConnection", value: rabbitSettings}, {upsert: true});
db.settings.replaceOne({ key: "eveDataUri" }, { key: "eveDataUri", value: "https://esi.tech.ccp.is/latest"}, {upsert: true});