<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
        @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
        placeholder="请设置聚合数据的属性" header-bgc="#FBC31F" :is-svgicon='true' header-icon="jiepaiqi"/>
</template>

<script>
import Node from './Node'

export default {
  name: "MetronomeNode",//聚合数据节点
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
    return {
      showError: false,
      errorInfo: '',
    }
  },
  computed:{
    content(){
      if (this.config.props.FieldName === '') {
        return '请选择聚合字段';
      }
      return "计数方式聚合数据";
    }
  },
  methods: {
  }
}
</script>

<style scoped>

</style>
