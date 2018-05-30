export default class Errors {
  constructor () {
    this.errors = {}
  }

  has (field) {
    return this.errors.hasOwnProperty(field)
  }

  any () {
    return Object.keys(this.errors).length > 0
  }

  get (field) {
    if (this.errors[field]) {
      return this.errors[field]
    }
  }

  record (errors) {
    this.errors = errors
  }

  clear (field) {
    if (field) delete this.errors[field]
    else this.errors = {}
  }

  addMessage (field, message) {
    let messages = this.errors[field] ? this.errors[field] : []
    messages.push(message)
    this.errors[field] = messages
  }
}
