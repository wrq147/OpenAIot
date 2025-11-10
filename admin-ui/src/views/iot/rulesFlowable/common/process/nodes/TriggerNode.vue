<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请设置触发器" header-bgc="#47bc82" header-icon="el-icon-set-up" />
</template>

<script>
import Node from './Node'

export default {
  name: "TriggerNode",
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
    content() {
      if (!this.$isNotEmpty(this.config.props.url)) {
        return "请设置Http请求的URL地址";
      }
      return "触发Http请求:" + this.config.props.url;
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      this.showError = false
      if (this.$isNotEmpty(this.config.props.url)) {
        this.showError = false
      } else {
        this.showError = true
        this.errorInfo = '请设置Http请求的URL地址'
      }
      if (this.showError) {
        err.push(`${this.config.name} Http节点未设置完善`)
      }
      return !this.showError
    }
  }
}
</script>

<style scoped></style>
