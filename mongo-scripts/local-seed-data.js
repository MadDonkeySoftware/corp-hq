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
rabbitSettings.hosts[0].address = "rabbit"
updateSetting("rabbitConnection", localDocker, rabbitSettings)
rabbitSettings.hosts[0].address = "localhost"
updateSetting("rabbitConnection", local, rabbitSettings)

// Eve Api
var eveDataUri = "https://esi.tech.ccp.is/latest"
updateSetting("eveDataUri", localDocker, eveDataUri)
updateSetting("eveDataUri", local, eveDataUri)

// Redis
var redisSettings = {
    hosts: [
        {address: "", port: 6379}
    ]
}
redisSettings.hosts[0].address = "redis"
updateSetting("redisConnection", localDocker, redisSettings)
redisSettings.hosts[0].address = "localhost"
updateSetting("redisConnection", local, redisSettings)