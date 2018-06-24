// Environment Consts
const localDocker = "localDocker"
const local = "local"

// Helpers
var updateSetting = function (key, environment, value) {
    try {
        db.settings.replaceOne({ key: key, environment: environment }, { key: key, environment: environment, value: value }, { upsert: true })
    } catch (error) {
        db.settings.remove({ key: key, environment: environment })
        db.settings.insert({ key: key, environment: environment, value: value })
    }
}

// RabbitMQ
var rabbitSettings = {
    hosts: [
        {address: ""},
    ],
    username: "rabbitmq",
    password: "rabbitmq",
    recordTtl: 5,
    recordHeartbeat: 1
}
rabbitSettings.hosts[0].address = "test-rabbitmq"
updateSetting("rabbitConnection", localDocker, rabbitSettings)

// Eve Api
var eveDataUri = "http://test-eve-api:3000";
updateSetting("eveDataUri", localDocker, eveDataUri)

// Redis
var redisSettings = {
    hosts: [
        {address: "", port: 6379}
    ]
}
redisSettings.hosts[0].address = "test-redis"
updateSetting("redisConnection", localDocker, redisSettings)