<template>
  <section class="section">
    <p>{{ msg }}</p>
    <div class="columns is-desktop">
      <div class="column">
        <link-group>
          <template slot="header">
            Some Test Header
          </template>

          <router-link :to="{name:'Dashboard'}">Test</router-link>
        </link-group>
      </div>
      <div class="column">
        <link-group>
          <template slot="header">
            Other
          </template>

          <router-link :to="{name:'MiningOperations'}">Mining Operations</router-link>
        </link-group>
      </div>
    </div>
  </section>
</template>

<script>
import linkGroup from '@/components/linkGroup'

export default {
  name: 'Dashboard',
  data () {
    return {
      msg: 'Dashboard Stub'
    }
  },
  components: {
    linkGroup // Shorthand for linkGroup: linkGroup
  },
  methods: {
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
