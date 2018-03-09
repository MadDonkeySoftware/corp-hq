<template>
  <section class="section">
    <div class="hello">
      <h1>{{ msg }}</h1>
      <h2>Essential Links</h2>
      <div>
        <button @click="query" :disabled="quering">Query</button>
      </div>
      <ul>
        <li>
          <a href="https://vuejs.org" target="_blank"> Core Docs </a>
        </li>
        <li>
          <a href="https://forum.vuejs.org" target="_blank"> Forum </a>
        </li>
        <li>
          <a href="https://chat.vuejs.org" target="_blank"> Community Chat </a>
        </li>
        <li>
          <a href="https://twitter.com/vuejs" target="_blank"> Twitter </a>
        </li>
        <br>
        <li>
          <a href="http://vuejs-templates.github.io/webpack/" target="_blank"> Docs for This Template </a>
        </li>
      </ul>
      <h2>Ecosystem</h2>
      <ul>
        <li>
          <a href="http://router.vuejs.org/" target="_blank"> vue-router </a>
        </li>
        <li>
          <a href="http://vuex.vuejs.org/" target="_blank"> vuex </a>
        </li>
        <li>
          <a href="http://vue-loader.vuejs.org/" target="_blank"> vue-loader </a>
        </li>
        <li>
          <a href="https://github.com/vuejs/awesome-vue" target="_blank"> awesome-vue </a>
        </li>
      </ul>
    </div>
  </section>
</template>

<script>
import axios from 'axios'

export default {
  name: 'HelloWorld',
  data () {
    return {
      msg: 'Welcome to Your Vue.js App',
      quering: false
    }
  },
  methods: {
    query () {
      this.quering = true
      axios.get('http://127.0.0.1:5000/api/v1/values')
        .then(response => {
          // Json response
          var items = []
          response.data.forEach(element => {
            items.push(element['value'])
          })
          this.msg = 'Welcome to Your Vue.js App. ' + items.join(', ')
        })
        .catch(e => {
          this.errors.push(e)
        })
      this.quering = false
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h1, h2 {
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
