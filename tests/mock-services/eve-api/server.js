const jsonServer = require('json-server')
const server = jsonServer.create()
const middlewares = jsonServer.defaults()

const universe = require('./data/universe.js')
const market = require('./data/market.js')

server.use(middlewares)

// Add custom routes
universe.setup(server)
market.setup(server)

server.listen(3000, () => {
  console.log('JSON Server is running')
})