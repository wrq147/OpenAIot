<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请设置功能节点属性" header-bgc="#182445" :is-svgicon='true' header-icon="shixucunchu" />
</template>

<script>
import Node from './Node'

export default {
  name: "FuncNode",
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
      if (this.config.props.FunctionId != '') {
        return '执行功能' + this.config.props.FunctionId;
      } else {
        return ''
      }
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      if (this.config.props.FunctionId == '' || this.config.props.TargetType == null) {
        this.showError = true;
        this.errorInfo = `请设置功能节点${this.config.name} 的属性`
        if (this.showError) {
          err.push(this.errorInfo);
        }
        return false;
      }
      else {
        this.showError = false;
        this.errorInfo = '';
        return true;
      }
    }
  }
}
</script>

<style scoped></style>
