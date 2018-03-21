var rabbitSettings = {
    hosts: [
        {address: "localhost"},
        {address: "rabbit"}
    ],
    username: "rabbitmq",
    password: "rabbitmq"
};

db.settings.replaceOne({ key: "rabbitConnection" }, { key: "rabbitConnection", value: rabbitSettings});