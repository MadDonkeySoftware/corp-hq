// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import store from './store'
import App from './App'
import router from './router'
import VueI18n from 'vue-i18n'

// Set up a global mechanism to pass events
/*
window.Event = new class {
  constructor () {
    this.vue = new Vue()
  }

  fire (event, data = null) {
    this.vue.$emit(event, data)
  }

  listen (event, callback) {
    this.vue.$on(event, callback)
  }
}()
*/

Vue.config.productionTip = false

Vue.use(VueI18n)
const i18n = new VueI18n({
  locale: 'en',
  messages: {
    en: {
    }
  }
})

/* eslint-disable no-new */
new Vue({
  i18n,
  store,
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
