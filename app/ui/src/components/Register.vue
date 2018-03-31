<template>
  <section class="section">
    <div class="columns">
      <div class="column is-one-fifth"></div>
        <div class="column" v-if="errors.length">
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
          <label class="label">{{$t('confirmPassword')}}</label>
          <div class="control has-icons-left">
            <input class="input" type="password" :placeholder="$t('confirmPassword')" v-model="passwordConfirm">
            <span class="icon is-small is-left">
              <i class="fas fa-key"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <label class="label">{{$t('email')}}</label>
          <div class="control has-icons-left">
            <input class="input" type="email" :placeholder="$t('email')" v-model="email">
            <span class="icon is-small is-left">
              <i class="fas fa-envelope"></i>
            </span>
          </div>
        </div>

        <div class="field">
          <div class="control">
            <label class="checkbox">
              <input type="checkbox" v-model="agreeWithTermsAndConditions">
              {{$t('agreeWithTermsAndConditions')}}
              <a href="#">{{$t('reviewTermsAndConditions')}}</a>
            </label>
          </div>
        </div>

        <div class="field">
          <div class="control">
            <button class="button is-link is-pulled-right" @click="submitRegistration" :disabled="submittingRegistration">{{$t('submit')}}</button>
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
  name: 'Register',
  data () {
    return {
      submittingRegistration: false,
      errors: [],
      username: '',
      password: '',
      passwordConfirm: '',
      email: '',
      agreeWithTermsAndConfitions: false
    }
  },
  methods: {
    submitRegistration () {
      this.errors = []
      this.submittingRegistration = true
      if (this.isFormValid()) {
        var data = {
          username: this.username,
          password: this.password,
          passwordConfirm: this.passwordConfirm,
          email: this.email
        }
        axios.post('http://127.0.0.1:5000/api/v1/registration', data)
          .then(response => {
            this.errors.push('Success!')
          })
          .catch(e => {
            e.response.data['messages'].forEach(message => {
              this.errors.push(message)
            })
          })
      }
      this.submittingRegistration = false
    },
    isFormValid () {
      if (this.username.length === 0) {
        this.errors.push('Username is a required field.')
      }
      if (this.password.length === 0) {
        this.errors.push('Password is a required field.')
      }
      if (this.passwordConfirm.length === 0) {
        this.errors.push('Password Confirmation is a required field.')
      }
      if (this.password !== this.passwordConfirm) {
        this.errors.push('Password and Password Confirmation must match.')
      }
      if (this.email.length === 0) {
        this.errors.push('Email is a required field.')
      }
      if (!this.agreeWithTermsAndConfitions.checked) {
        this.errors.push('You must agree with the Terms and Conditions to register with this site.')
      }

      return this.errors.length === 0
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
