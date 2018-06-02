<template>
  <section class="section">
    <div class="columns" style="margin-top: 8px"></div>

    <div class="columns" v-if="messages.length">
      <div class="column is-one-fifth"></div>
        <div class="column">
          <ul>
            <li v-for="message in messages" v-bind:key="message">{{ message }}</li>
          </ul>
        </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns" v-if="form.errors.has('General')">
      <div class="column is-one-fifth"></div>
      <div class="column">
        <errorBlock :errors="form.errors.get('General')"></errorBlock>
      </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column" @keydown="form.errors.clear($event.target.name) & form.errors.clear('General')">

        <formInput :label="$t('username')" :placeholder="$t('username')" name="Username" v-model="form.username" :errors="form.errors.get('Username')" >
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-user"></i></span>
          </template>
        </formInput>

        <formInput :label="$t('password')" :placeholder="$t('password')" name="Password" v-model="form.password" :errors="form.errors.get('Password')" type="password">
          <template slot="decoration">
            <span class="icon is-small is-left"><i class="fas fa-key"></i></span>
          </template>
        </formInput>

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
import errorBlock from '@/components/errorBlock'
import formInput from '@/components/formInput'
import Form from '@/classes/form'
import utils from '@/utils'

export default {
  name: 'log-in',
  components: {
    errorBlock, formInput
  },
  data () {
    return {
      form: new Form({
        username: '',
        password: ''
      }),
      submittingLogin: false,
      messages: []
    }
  },
  methods: {
    submitLogin () {
      let parent = this
      this.form.errors.clear()
      if (this.isFormValid()) {
        this.submittingLogin = true
        this.form.post(utils.buildApiUrl('/api/v1/authentication'))
          .then(
            data => {
              parent.$store.commit('updateKey', { apiKey: data['token'] })
              parent.$router.push({ name: 'Dashboard', params: { userId: 123 } })
            },
            f => { })
          .catch(e => { console.log(e) })
      }
      this.submittingLogin = false
    },
    isFormValid () {
      if (this.form.username.length === 0) {
        this.form.addError('Username', 'Username is a required field.')
      }
      if (this.form.password.length === 0) {
        this.form.addError('Password', 'Password is a required field.')
      }

      return !this.form.errors.any()
    }
  },
  mounted () {
    var parent = this
    this.$store.state.messages.forEach(function (item, index) {
      parent.messages.push(item)
    })
    this.$store.commit('clearMessages')
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
