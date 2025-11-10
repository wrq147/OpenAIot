<template>
  <node :show="false" :currentNode='config' @insertNode="type => $emit('insertNode', type)"/>
</template>

<script>
import Node from './Node'

export default {
  name: "EmptyNode",
  props:{
    config:{
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  components: {Node},
  data() {
    return {}
  },
  methods: {}
}
</script>

<style scoped>

</style>
