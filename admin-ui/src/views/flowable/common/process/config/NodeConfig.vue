<template>
  <div>
    <el-tabs v-model="active" v-if="name && formConfig.length > 0">
      <el-tab-pane :label="name" name="properties">
        <component :is="(selectNode.type||'').toLowerCase()" :config="selectNode.props"/>
      </el-tab-pane>
      <el-tab-pane label="表单权限设置" name="permissions">
        <form-authority-config/>
      </el-tab-pane>
    </el-tabs>
    <component :is="(selectNode.type||'').toLowerCase()" v-else :config="selectNode.props"/>
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
      return this.$store.state.flowable.selectedNode
    },
    formConfig() {
      return this.$store.state.flowable.design.formItems
    },
    name() {
      switch (this.selectNode.type) {
        case 'ROOT':
          return '设置发起人';
        case 'APPROVAL':
          return '设置审批人';
        case 'CC':
          return '设置抄送人';
        case 'USER':
          return '设置办理人';
        default:
          return null;
      }
    }
  },
  methods: {}
}
</script>

<style lang="less" scoped>

</style>
