<template>
  <section class="section">
    <div class="columns">
      <div class="column is-one-fifth"></div>
        <div class="column is-danger" v-if="errors.length">
          <b>{{$t('pleaseCorrectErrors')}}</b>
          <ul>
            <li v-for="error in errors" v-bind:key="error">{{ error }}</li>
          </ul>
        </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column">

        <div class="field">
          <label class="label">{{$t('username')}}</label>
          <div class="control has-icons-left">
            <input class="input" type="text" :placeholder="$t('username')" v-model="username">
            <span class="icon is-small is-left">
              <i class="fas fa-user"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <label class="label">{{$t('password')}}</label>
          <div class="control has-icons-left">
            <input class="input" type="password" :placeholder="$t('password')" v-model="password">
            <span class="icon is-small is-left">
              <i class="fas fa-key"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <div class="control">
            <button class="button is-link is-pulled-right" @click="submitLogin" :disabled="submittingLogin">{{$t('submit')}}</button>
          </div>
        </div>
      </div>
      <div class="column is-one-fifth"></div>
    </div>
  </section>
</template>

<script>
import axios from 'axios'
import constants from '@/constants'
import utils from '@/utils'

export default {
  name: 'Register',
  data () {
    return {
      submittingLogin: false,
      errors: [],
      username: '',
      password: ''
    }
  },
  methods: {
    submitLogin () {
      this.errors = []
      this.submittingLogin = true
      if (this.isFormValid()) {
        var data = {
          username: this.username,
          password: this.password
        }
        axios.post(utils.buildApiUrl('/api/v1/authentication'), data)
          .then(response => {
            Event.fire(constants.authTokenUpdated, response.data['token'])
            this.$router.push({name: 'Dashboard', params: { userId: 123 }})
          })
          .catch(e => {
            this.errors.push('Invalid login information.')
            this.password = ''
          })
      }
      this.submittingLogin = false
    },
    isFormValid () {
      if (this.username.length === 0) {
        this.errors.push('Username is a required field.')
      }
      if (this.password.length === 0) {
        this.errors.push('Password is a required field.')
      }

      return this.errors.length === 0
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
