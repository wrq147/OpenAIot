<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请设置标签节点" header-bgc="#2d7577" :is-svgicon='true' header-icon="shujuhuancun" />
</template>

<script>
import Node from './Node'

export default {
  name: "TagNode",
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
      if (this.config.props.TagId != '') {
        return '标签 ' + this.config.props.TagId + ' 已赋值';
      } else {
        return ''
      }
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      if (this.config.props.TagId == ''||this.config.props.TargetType == null) {
        this.showError = true;
        this.errorInfo = `请设置标签节点${this.config.name} 的属性`
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
