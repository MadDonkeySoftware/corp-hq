<template>
  <div class="field">
    <label class="label">{{ label }}</label>
    <div class="control has-icons-left">
      <input :class="{input: true, 'is-danger': errors && errors.length > 0}"
            :type="type" :name="name" :placeholder="placeholder" :value="value" v-on:input="$emit('input', $event.target.value)">
            <!--
              v-bind:value="value"
              v-on:input="$emit('input', $event.target.value)"
            -->
        <slot name="decoration"></slot>
    </div>
    <span class="help is-danger" v-if="errors && errors.length > 0">
      <span class="errorMessage" v-for="error in errors" v-bind:key="error">{{ error }}</span>
    </span>
  </div>
</template>

<script>
export default {
  name: 'form-input',
  props: [
    'label',
    'placeholder',
    'name',
    'type',
    'value',
    'errors'
  ]
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
