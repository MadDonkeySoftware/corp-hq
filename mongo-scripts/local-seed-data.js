var rabbitSettings = {
    hosts: [
        {address: "localhost"},
        {address: "rabbit"}
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
};

db.settings.replaceOne({ key: "rabbitConnection" }, { key: "rabbitConnection", value: rabbitSettings}, {upsert: true});