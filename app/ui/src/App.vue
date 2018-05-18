<template>
  <div>
    <nav-bar></nav-bar>
    <div id="app">
      <router-view/>
    </div>
  </div>
</template>

<script>
import navBar from '@/components/navBar'

export default {
  name: 'App',
  data: function () {
    return {
    }
  },
  components: {
    navBar
  },
  methods: {
  }
}
</script>

<style>
</style>
