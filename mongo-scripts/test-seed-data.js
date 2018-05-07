var rabbitSettings = {
    hosts: [
        {address: "rabbit"}
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
};

db.settings.replaceOne({ key: "rabbitConnection" }, { key: "rabbitConnection", value: rabbitSettings}, {upsert: true});
db.settings.replaceOne({ key: "eveDataUri" }, { key: "eveDataUri", value: "http://test-eve-api:3000"}, {upsert: true});