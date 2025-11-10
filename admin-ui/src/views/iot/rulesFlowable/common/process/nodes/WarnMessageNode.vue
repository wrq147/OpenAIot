<template>
  <node :title="config.name" :currentNode='config' :show-error="showError" :content="content" :error-info="errorInfo"
        @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
        :placeholder="config.props.EventId?'事件标识:'+EventName:'请设置事件节点属性'" header-bgc="#E11013" :is-svgicon='true' header-icon="xiaoxijiedian"/>
</template>

<script>
import Node from './Node'

export default {
  name: "WarnMessageNode",
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
    EventName(){
      for(let i=0;i<this.eventItems.length;i++){
        if(this.config.props.EventId==this.eventItems[i].code){
         return this.eventItems[i].name
        }
      }
     
    },
    eventItems() {
      return this.$store.state.rulesFlowable.rulesProductEvent;
    },
    content() {
      if (this.config.props.EventId != '') {
        return '执行事件' + this.config.props.EventId;
      } else {
        return ''
      }
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err){
      if (this.config.props.EventId == '' || this.config.props.TargetType == null) {
        this.showError = true;
        this.errorInfo = `请设置告警节点${this.config.name} 的属性`
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

<style scoped>

</style>
