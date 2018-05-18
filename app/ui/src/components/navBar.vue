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
        <div class="navbar-item">
          <div class="field is-grouped">
            <p class="control">
              <button class="button" v-on:click="navToLogIn">Log In</button>
            </p>
            <p class="control">
              <button class="button" v-on:click="navToRegistration">Registration</button>
            </p>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
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
      }
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
    _setNavState: function (state) {
      this.navBurgerCss['is-active'] = state
      this.menuBurgerCss['is-active'] = state
    },
    _navHelper: function (target) {
      this._setNavState(false)
      this.$router.push(target)
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
