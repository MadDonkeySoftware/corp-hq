import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

const state = {
  apiKey: null,
  messages: []
}

const mutations = {
  updateKey (state, payload) {
    state.apiKey = payload.apiKey
  },
  clearMessages (state) {
    state.messages = []
  },
  addMessage (state, payload) {
    state.messages.push(payload.message)
  }
}

export default new Vuex.Store({
  state,
  mutations
})
