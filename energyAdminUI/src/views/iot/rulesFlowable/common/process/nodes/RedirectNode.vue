<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请设置转发节点属性" header-bgc="#C53EF3" :is-svgicon='true' header-icon="a-zhuanfajiedian" />
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
      console.log(this.config,'this.configthis.configthis.configthis.config');
      if (this.config&&this.config.props&&this.config.props.TargetDtuIdsList&&this.config.props.TargetDtuIdsList.length>0) {
        return '转发节点' + this.config.props.TargetDtuIdsList.join();
      } else {
        return ''
      }
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      if (this.config&&this.config.props&&this.config.props.TargetDtuIdsList&&this.config.props.TargetDtuIdsList.length==0 || this.config.props.ProductId == '') {
        this.showError = true;
        this.errorInfo = `请设置转发节点${this.config.name} 的属性`
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
