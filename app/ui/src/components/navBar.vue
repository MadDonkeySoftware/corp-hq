<template>
  <nav class="navbar is-dark is-fixed-top" role="navigation" aria-label="main navigation">
    <div class="navbar-brand">
      <router-link class="navbar-item" :to="{name:'Main'}">
        <label>Corp HQ</label>
      </router-link>
      <div v-bind:class="navBurgerCss" v-on:click="navBurgerClick">
        <span></span>
        <span></span>
        <span></span>
      </div>
    </div>
    <div v-bind:class="menuBurgerCss">
      <div class="navbar-start">
        <!-- navbar items -->
      </div>
      <div class="navbar-end">
        <!-- navbar items -->
        <!-- This is where the log in/out & register buttons will go -->
        <div class="navbar-item" v-if="!isAuthenticated">
          <div class="field is-grouped">
            <p class="control">
              <button class="button" v-on:click="navToLogIn">{{$t('logIn')}}</button>
            </p>
            <p class="control">
              <button class="button" v-on:click="navToRegistration">{{$t('register')}}</button>
            </p>
          </div>
        </div>
        <div class="navbar-item" v-if="isAuthenticated">
          <div class="field is-grouped">
            <p class="control">
              <button class="button" v-on:click="performLogOut">{{$t('logOut')}}</button>
            </p>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
import axios from 'axios'
import constants from '@/constants'

export default {
  name: 'nav-bar',
  data: function () {
    return {
      navBurgerCss: {
        'navbar-burger': true,
        'burger': true,
        'is-active': false
      },
      menuBurgerCss: {
        'navbar-menu': true,
        'is-active': false
      },
      authToken: null
    }
  },
  methods: {
    navBurgerClick: function (event) {
      var newVal = !this.navBurgerCss['is-active']
      this._setNavState(newVal)
    },
    navToRegistration: function (event) {
      this._navHelper({name: 'Register'})
    },
    navToLogIn: function (event) {
      this._navHelper({name: 'LogIn'})
    },
    performLogOut: function () {
      let parent = this

      // TODO: Let the API know we've logged out.
      axios.delete('http://127.0.0.1:5000/api/v1/token/' + this.authToken)
        .then(response => {
          Event.fire(constants.authTokenUpdated, null)
          parent._navHelper({name: 'Main'})
        })
        .catch(e => {
          this.errors.push('Invalid login information.')
          this.password = ''
        })
    },
    _setNavState: function (state) {
      this.navBurgerCss['is-active'] = state
      this.menuBurgerCss['is-active'] = state
    },
    _navHelper: function (target) {
      this._setNavState(false)
      this.$router.push(target)
    }
  },
  computed: {
    isAuthenticated: function () {
      return this.authToken != null
    }
  },
  mounted () {
    let parent = this
    Event.listen(constants.authTokenUpdated, function (token) {
      parent.authToken = token
    })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
