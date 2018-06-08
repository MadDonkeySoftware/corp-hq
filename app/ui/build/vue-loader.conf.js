'use strict'
const utils = require('./utils')
const config = require('../config')
const isProduction = process.env.NODE_ENV === 'production'
const sourceMapEnabled = isProduction
  ? config.build.productionSourceMap
  : config.dev.cssSourceMap

const loaders = Object.assign(
  {},
  {i18n: '@kazupon/vue-i18n-loader'},
  utils.cssLoaders({ sourceMap: sourceMapEnabled, extract: isProduction, })
)

module.exports = {
  loaders: loaders,
  cssSourceMap: sourceMapEnabled,
  cacheBusting: config.dev.cacheBusting,
  transformToRequire: {
    video: ['src', 'poster'],
    source: 'src',
    img: 'src',
    image: 'xlink:href'
  }
}
