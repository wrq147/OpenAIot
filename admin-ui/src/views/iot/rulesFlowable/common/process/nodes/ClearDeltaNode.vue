<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请指定要清除的数据" header-bgc="#940404af" :is-svgicon='false' header-icon="el-icon-delete" />
</template>

<script>
import Node from './Node'

export default {
  name: "ClearDeltaNode",
  props: {
    config: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  components: { Node },
  data() {
    return {
      showError: false,
      errorInfo: '',
    }
  },
  computed: {
    setup() {
      return this.$store.state.rulesFlowable.rulesDesign;
    },
    content() {
      if (this.config.props.ClearType == null) {
        return ""
      }
      else {
        switch (this.config.props.ClearType) {
          case 0:
            {
              let parrs = this.setup.HttpParams.filter(x => this.config.props.Codes.indexOf(x.code) >= 0);
              return "清除参数:" + parrs.map(x => x.name).join();
            }
          case 1:
            return "清除设备的缓存";
        }
        return "";
      }
    }
  },
  methods: {
    validate(err) {
      this.showError = false
      this.errorInfo = "";
      if (this.config.props.ClearType === null) {
        this.showError = true
        this.errorInfo = "请指定要清除的数据"
      }
      if (this.config.props.ClearType == 0 && (this.config.props.Codes == null || this.config.props.Codes.length == 0)) {
        this.showError = true
        this.errorInfo = "请指定要清除的参数"
      }
      if (this.config.props.ClearType == 1 && (this.config.props.Codes == null || this.config.props.Codes.length == 0)) {
        this.showError = true
        this.errorInfo = "请指定要清除的设备"
      }
      if (this.showError) {
        err.push(this.errorInfo);
      }
      return !this.showError
    },
  }
}
</script>

<style scoped></style>
