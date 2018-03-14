<template>
  <section class="section">
    <div class="title">
      <h1>{{msg}}</h1>
    </div>
  </section>
</template>

<script>
export default {
  name: 'HelloWorld',
  data () {
    return {
      msg: 'Log in page'
    }
  },
  methods: {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
