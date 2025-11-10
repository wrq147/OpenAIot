<template>
  <node title="规则源" :currentNode='config' :is-root="true" :content="content" @insertNode="type => $emit('insertNode', type)"
        :placeholder="content" header-bgc="#576a95" header-icon="el-icon-coin"/>
</template>

<script>
import Node from './Node'

export default {
  name: "RootNode",
  components: {Node},
  props:{
    config:{
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  computed:{
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
    content(){
      return this.setup.TriggerWay==0?'设备触发':(this.setup.TriggerWay==1?'HTTP触发':'定时触发');
    }
  },
  data() {
    return {
    }
  },
  methods: {
  }
}
</script>

<style scoped>

</style>
