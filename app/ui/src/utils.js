export default {
  getEnvironmentVar: function (varname, defaultvalue) {
    var result = process.env[varname]
    if (result !== undefined) {
      // console.debug('ENV VAR used. ' + varname + ' ' + result)
      return result
    } else {
      // console.debug('DEFAULT used. ' + varname + ' ' + defaultvalue)
      return defaultvalue
    }
  },
  buildApiUrl: function (part) {
    let url = ''
    if (!part.startsWith('/')) {
      url += '/'
    }
    return url + part
  }
}
