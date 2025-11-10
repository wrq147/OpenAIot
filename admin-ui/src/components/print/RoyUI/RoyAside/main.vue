<template>
  <div :style="{ width }" class="roy-aside">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'RoyAside',

  componentName: 'RoyAside',

  props: {
    width: {
      type: String,
      default: '300px'
    }
  }
}
</script>
<style scoped>

</style>