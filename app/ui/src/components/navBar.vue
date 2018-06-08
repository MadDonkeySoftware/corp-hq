<i18n>
{
  "en": {
    "dashboard": "Dashboard",
    "logIn": "Log In",
    "logOut": "Log Out",
    "register": "Register"
  }
}
</i18n>

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
        <a class="navbar-item" v-if="isAuthenticated" v-on:click="_navHelper({name: 'Dashboard'})">{{$t('dashboard')}}</a>
      </div>
      <div class="navbar-end">
        <!-- navbar items -->
        <!-- This is where the log in/out & register buttons will go -->
        <div class="navbar-item" v-if="!isAuthenticated">
          <div class="field is-grouped">
            <p class="control">
              <button class="button" v-on:click="_navHelper({name: 'LogIn'})">{{$t('logIn')}}</button>
            </p>
            <p class="control">
              <button class="button" v-on:click="_navHelper({name: 'Register'})">{{$t('register')}}</button>
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
import utils from '@/utils'
import axios from 'axios'

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
    performLogOut: function () {
      let parent = this

      // TODO: Let the API know we've logged out.
      axios.delete(utils.buildApiUrl('/api/v1/token/' + this.authToken))
        .then(response => {
          this.$store.commit('updateKey', {apiKey: null})
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
      return this.$store.state.apiKey != null
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
