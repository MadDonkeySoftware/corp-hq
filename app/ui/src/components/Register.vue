<template>
  <section class="section">
    <div v-if="errors.length">
      <b>Please correct the following error(s):</b>
      <ul>
        <li v-for="error in errors" v-bind:key="error">{{ error }}</li>
      </ul>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column">

        <div class="field">
          <label class="label">Username</label>
          <div class="control has-icons-left">
            <input class="input" type="text" placeholder="Username" v-model="username">
            <span class="icon is-small is-left">
              <i class="fas fa-user"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <label class="label">Password</label>
          <div class="control has-icons-left">
            <input class="input" type="password" placeholder="Password" v-model="password">
            <span class="icon is-small is-left">
              <i class="fas fa-key"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <label class="label">Confirm Password</label>
          <div class="control has-icons-left">
            <input class="input" type="password" placeholder="Confirm Password" v-model="passwordConfirm">
            <span class="icon is-small is-left">
              <i class="fas fa-key"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <label class="label">Email</label>
          <div class="control has-icons-left">
            <input class="input" type="email" placeholder="Email" v-model="email">
            <span class="icon is-small is-left">
              <i class="fas fa-envelope"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <div class="control">
            <label class="checkbox">
              <input type="checkbox">
              I agree to the <a href="#">terms and conditions</a>
            </label>
          </div>
        </div>

        <div class="field">
          <div class="control">
            <button class="button is-link is-pulled-right" @click="submitRegistration" :disabled="submittingRegistration">Submit</button>
          </div>
        </div>
      </div>
      <div class="column is-one-fifth"></div>
    </div>
  </section>
</template>

<script>
import axios from 'axios'

export default {
  name: 'HelloWorld',
  data () {
    return {
      msg: 'Welcome to Your Vue.js App',
      submittingRegistration: false,
      errors: [],
      username: '',
      password: '',
      passwordConfirm: '',
      email: ''
    }
  },
  methods: {
    submitRegistration () {
      this.errors = []
      this.submittingRegistration = true
      var data = {
        username: this.username,
        password: this.password,
        passwordConfirm: this.passwordConfirm,
        email: this.email
      }
      axios.post('http://127.0.0.1:5000/api/v1/registration', data)
        .then(response => {
          // Json response
          // var items = []
          // response.data.forEach(element => {
          //   items.push(element['value'])
          // })
          // this.msg = 'Welcome to your Vue.js App. ' + items.join(', ')
          this.errors.push('Success!')
        })
        .catch(e => {
          this.errors.push(e.response.data['message'])
        })
      this.submittingRegistration = false
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
