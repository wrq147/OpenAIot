<template>
    <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
      @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
      placeholder="请设置属性节点" header-bgc="#3d5520" :is-svgicon='true' header-icon="shujuhuancun" />
  </template>
  
  <script>
  import Node from './Node'
  
  export default {
    name: "SetPropNode",
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
        if (this.config.props.PropCode != '') {
          return '属性 ' + this.config.props.PropCode + ' 已赋值';
        } else {
          return ''
        }
      }
    },
    methods: {
      //校验数据配置的合法性
      validate(err) {
        if (this.config.props.PropCode == '') {
          this.showError = true;
          this.errorInfo = `请设置属性节点${this.config.name} 的属性`
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
  