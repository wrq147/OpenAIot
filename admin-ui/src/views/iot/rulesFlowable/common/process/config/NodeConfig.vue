<template>
  <div>
    <component :is="comName" :config="selectNode.props" />
  </div>
</template>

<script>
import NodeConfigExport from './NodeConfigExport'
export default {
  name: "NodeConfig",
  components: NodeConfigExport,
  data() {
    return {
      active: 'properties',
    }
  },
  computed: {
    selectNode() {
      return this.$store.state.rulesFlowable.rulesSelectedNode
    },
    comName() {
      return ((this.selectNode.type && this.selectNode.type != 'ROOT') ? this.selectNode.type : '' || '').toLowerCase();
    }
  },
  methods: {

  }
}
</script>

<style lang="less" scoped></style>
