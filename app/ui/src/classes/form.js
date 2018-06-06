import axios from 'axios'
import Errors from '@/classes/errors.js'

export default class Form {
  constructor (data) {
    this.originalData = data

    for (let field in data) {
      this[field] = data[field]
    }

    this.errors = new Errors()
  }

  data () {
    let data = {}

    for (let property in this.originalData) {
      data[property] = this[property]
    }

    return data
  }

  reset () {
    for (let field in this.originalData) {
      this[field] = this.originalData[field]
    }

    this.errors.clear()
  }

  post (url) {
    return this.submit('post', url)
  }

  put (url) {
    return this.submit('put', url)
  }

  patch (url) {
    return this.submit('patch', url)
  }

  delete (url) {
    return this.submit('delete', url)
  }

  submit (requestType, url) {
    return new Promise((resolve, reject) => {
      axios[requestType](url, this.data(), {requestType: 'application/json'})
        .then(
          response => {
            this.onSuccess(response.data)
            resolve(response.data)
          },
          error => {
            this.onFail(error.response.data)
            reject(error)
          })
        .catch(error => {
          this.onFail(error.response.data)
          reject(error)
        })
    })
  }

  onSuccess (data) {
    this.reset()
  }

  onFail (errors) {
    if (errors !== undefined && errors.messages !== undefined) {
      this.errors.record(errors.messages)
    }
  }

  addError (key, message) {
    this.errors.addMessage(key, message)
  }
}
