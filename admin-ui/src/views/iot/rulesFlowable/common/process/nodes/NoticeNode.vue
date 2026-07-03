<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
        @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
        placeholder="请设置通知触发方式" header-bgc="#DC362E" :is-svgicon='true' header-icon="chufatongzhi" />
</template>

<script>
import Node from './Node'
export default {
  name: "NoticeNode",
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
  mounted() {
  },
  computed:{
    content(){
      if (this.config.props.NoticeWay === 'APP'){
        return `APP站内通知 ${this.config.props.userName||""}`
      }else if(this.config.props.NoticeWay === 'EMAIL'){
        
        return `通知固定邮箱 ${this.config.props.TargetValue||""}`
      }else if(this.config.props.NoticeWay === 'SMS'){
        return `发送短信给手机号 ${this.config.props.TargetValue||""}`
      }else{
        return ''
      }
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err){
      this.showError = false
      try {
        if (this.config.props.NoticeWay === "APP") {
          if ((this.config.props.TargetValue || "") === ""){
            this.showError = true
            this.errorInfo = "请选择触发通知的用户"
          }
        }else if(this.config.props.NoticeWay === 'EMAIL'){
          if ((this.config.props.TargetValue || "") === ""){
            this.showError = true
            this.errorInfo = "请设置通知邮箱"
          }
        }else if(this.config.props.NoticeWay === 'SMS'){
          if ((this.config.props.TargetValue || "") === ""){
            this.showError = true
            this.errorInfo = "请设置通知手机号"
          }
        } else {
          if (this.config.props.NoticeWay==="") {
            this.showError = true
            this.errorInfo = "请设置触发通知方式"
          }
        }
      } catch (e) {
        this.showError = true
        this.errorInfo = "触发通知配置出现问题"
      }
      if ((this.config.props.Title || "") === ""){
        this.showError = true
        this.errorInfo = "请设置通知内容"
      }
      if (this.showError){
        err.push(`${this.config.name} 设置触发通知规则有误`)
      }
      return !this.showError
    },
  }
}
</script>

<style scoped>

</style>
