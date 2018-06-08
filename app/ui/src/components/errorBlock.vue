<i18n>
{
  "en": {
    "pleaseCorrectErrors": "Please correct the following error(s):"
  }
}
</i18n>
<template>
  <article class="message is-danger">
    <div class="message-header">
      <p>{{$t('pleaseCorrectErrors')}}</p>
    </div>
    <div class="message-body">
      <ul>
        <li v-for="error in errors" v-bind:key="error">{{ error }}</li>
      </ul>
    </div>
  </article>
</template>

<script>
export default {
  name: 'error-block',
  props: ['errors']
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
