<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
        @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
        placeholder="请设置数据转换" header-bgc="#FF943E" :is-svgicon='true' header-icon="shujuzhuanhuan"/>
</template>

<script>
import Node from './Node'
import DefaultProps from "../../../definition/layout/process/DefaultNodeProps"
export default {
  name: "ConversionNode",
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
      if(this.config.props.func!=DefaultProps.CONVERSION_PROPS.func){
        return "输入的数据已被转换";
      }
      return "";
    }
  },
  methods: {
  }
}
</script>

<style scoped>

</style>
