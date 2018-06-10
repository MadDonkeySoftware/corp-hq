<template>
  <section class="section">
    <div class="columns" style="margin-top: 8px"></div>

    <div class="columns" v-if="messages.length">
      <div class="column is-one-fifth"></div>
      <div class="column">
        <ul>
          <li v-for="message in messages" v-bind:key="message">{{ message }}</li>
        </ul>
      </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column">
        <div class="field">
          <label class="label">Regions</label>
          <div class="select">
            <select v-model="selectedRegion">
              <option disabled value="">Select region</option>
              <option v-for="region in regions" v-bind:key="region.id" :value="region.id">{{ region.name }}</option>
            </select>
          </div>
        </div>
      </div>
      <div class="column is-one-fifth"></div>
    </div>

    <div class="columns">
      <div class="column is-one-fifth"></div>
      <div class="column">
        <table class="table">
          <thead>
            <tr>
              <td><label class="label">test 1</label></td>
              <td colspan="2" style="text-align: center;">test 2</td>
              <td>test 3</td>
            </tr>
            <tr>
              <td><label class="label">test 1</label></td>
              <td>test 2</td>
              <td>test 3</td>
              <td>test 4</td>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>data 1</td>
              <td>data 2</td>
            </tr>
            <tr>
              <td>data 3</td>
              <td>data 4</td>
            </tr>
          </tbody>
          <tfoot>
            <tr>
              <td>foot 1</td>
              <td>foot 2</td>
            </tr>
          </tfoot>
        </table>
      </div>
      <div class="column is-one-fifth"></div>
    </div>
  </section>
</template>

<script>
import axios from 'axios'
import utils from '@/utils'

export default {
  name: 'MiningOperations',
  data () {
    return {
      msg: 'Mining Operations Stub',
      messages: [],
      regions: [],
      selectedRegion: ''
    }
  },
  methods: {
  },
  mounted: function () {
    let parent = this
    axios.get(utils.buildApiUrl('/api/v1/map/regions'), { 'headers': { 'auth-token': parent.$store.state.apiKey } })
      .then(
        resp => {
          let data = resp.data.data
          data.sort(function (a, b) {
            if (a.name < b.name) { return -1 }
            if (a.name > b.name) { return 1 }
            return 0
          })
          parent.regions = data
        },
        fail => { console.log(fail) })
      .catch(e => { console.log(e) })
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
