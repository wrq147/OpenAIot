<template>
    <node :title="config.name" :show-error="showError" :content="content" :error-info="errorInfo"
      @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
      placeholder="请配置执行的设备项目" header-bgc="#00CED1" header-icon="el-icon-set-up" />
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
        errorInfo: ''
      }
    },
    created(){
    },
    computed: {
      content() {
        if (this.config.props.FieldId != ''){
          return `执行 '${this.config.props.FieldName}' 的项目`;
        }
        return ''
      }
    },
    methods: {
      //校验数据配置的合法性
      validate(err) {
        this.showError = false
  
        if (this.config.props.FieldId=='') {
          this.showError = true
          this.errorInfo = '请选择设备表单'
        }
        else if (this.config.props.Items.length == 0) {
          this.showError = true
          this.errorInfo = '请添加执行的项目'
        }

  
        if (this.showError) {
          err.push(`${this.config.name} 未设置完善`)
        }
  
        return !this.showError
      }
    }
  }
  </script>
  
  <style scoped></style>
  