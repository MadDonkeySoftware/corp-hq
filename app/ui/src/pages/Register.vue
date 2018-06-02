<template>
  <section class="section">
    <div class="columns" style="margin-top: 8px"></div>

    <div class="columns" v-if="form.errors.has('General')">
      <div class="column is-one-fifth"></div>
      <div class="column" style="margin-top: 18px">
        <errorBlock :errors="form.errors.get('General')"></errorBlock>
      </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column" @keydown="form.errors.clear($event.target.name) & form.errors.clear('General')">

        <formInput :label="$t('username')" :placeholder="$t('username')" name="Username" v-model="form.username"
                   :errors="form.errors.get('Username')" >
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-user"></i></span>
          </template>
        </formInput>

        <formInput :label="$t('password')" :placeholder="$t('password')" name="Password" v-model="form.password"
                   :errors="form.errors.get('Password')" type="password">
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-key"></i></span>
          </template>
        </formInput>

        <formInput :label="$t('confirmPassword')" :placeholder="$t('confirmPassword')" name="PasswordConfirm" v-model="form.passwordConfirm"
                   :errors="form.errors.get('PasswordConfirm')" type="password">
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-key"></i></span>
          </template>
        </formInput>

        <formInput :label="$t('email')" :placeholder="$t('email')" name="Email" v-model="form.email"
                   :errors="form.errors.get('Email')">
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-envelope"></i></span>
          </template>
        </formInput>

        <div class="field">
          <div class="control">
            <label class="checkbox">
              <input type="checkbox" name="TermsAccepted" v-model="form.terms" @click="form.errors.clear($event.target.name)">
              {{$t('agreeWithTermsAndConditions')}}
              <a href="#">{{$t('reviewTermsAndConditions')}}</a>
            </label>
          </div>
          <span class="help is-danger" v-if="form.errors.has('TermsAccepted')">
            <span v-for="message in form.errors.get('TermsAccepted')" v-bind:key="message"><span class="errorMessage">{{ message }}</span></span>
          </span>
        </div>

        <div class="field">
          <div class="control">
            <button class="button is-link is-pulled-right" @click="submitRegistration" :disabled="submittingRegistration || form.errors.any()">{{$t('submit')}}</button>
          </div>
        </div>
      </div>
      <div class="column is-one-fifth"></div>
    </div>
  </section>
</template>

<script>
import utils from '@/utils'
import errorBlock from '@/components/errorBlock'
import formInput from '@/components/formInput'
import Form from '@/classes/form.js'

export default {
  name: 'Register',
  components: {
    errorBlock, formInput
  },
  data () {
    return {
      form: new Form({
        username: '',
        password: '',
        passwordConfirm: '',
        email: '',
        terms: false
      }),
      submittingRegistration: false
    }
  },
  methods: {
    submitRegistration () {
      let parent = this
      this.form.errors.clear()
      if (this.isFormValid()) {
        this.submittingRegistration = true
        this.form.post(utils.buildApiUrl('/api/v1/registration'))
          .then(r => {
            parent.$store.commit('addMessage', {message: this.$t('registrationSuccessful')})
            parent.$router.push({name: 'LogIn'})
          }, f => {
          })
          .catch(e => { console.log(e) })
      }
      this.submittingRegistration = false
    },
    isFormValid () {
      /* NOTE: The keys here need to match what is being returned from the API. */
      if (this.form.username.length === 0) {
        this.form.addError('Username', 'Username is a required field.')
      }
      if (this.form.password.length === 0) {
        this.form.addError('Password', 'Password is a required field.')
      }
      if (this.form.passwordConfirm.length === 0) {
        this.form.addError('PasswordConfirm', 'Password confirmation is a required field.')
      }
      if (this.form.password !== this.form.passwordConfirm) {
        this.form.addError('Password', 'Password and password confirmation must match.')
        this.form.addError('PasswordConfirm', 'Password and password confirmation must match.')
      }
      if (this.form.email.length === 0) {
        this.form.addError('Email', 'Email is a required field.')
      }
      if (!this.form.terms) {
        this.form.addError('TermsAccepted', 'You must agree with the terms and conditions to register with this site.')
      }

      return !this.form.errors.any()
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.errorMessage {
  margin-right: 4px;
}
</style>
