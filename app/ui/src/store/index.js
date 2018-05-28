import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

const state = {
  apiKey: null
}

const mutations = {
  updateKey (state, payload) {
    state.apiKey = payload.apiKey
  }
}

export default new Vuex.Store({
  state,
  mutations
})
