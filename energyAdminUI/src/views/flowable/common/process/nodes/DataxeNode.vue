<template>
  <node :title="config.name" :show-error="showError" :content="content" :error-info="errorInfo"
    @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
    placeholder="请配置数据执行动作" header-bgc="#5913db" header-icon="el-icon-set-up" />
</template>

<script>
import Node from './Node'
import { getDataList } from "@/api/flowable/design";
export default {
  name: "DataxeNode",
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
      formlist:[]
    }
  },
  created(){
    getDataList().then(rsp=>{
      this.formlist=rsp.data;
    })
  },
  computed: {
    content() {
      if (this.config.props.targetform != ''){
        let sels=this.formlist.filter(item=>item.code==this.config.props.targetform);
        if(sels.length>0){
          if(this.config.props.action=="Update"){
            return `修改 '${sels[0].name}'`;
          }
          else if(this.config.props.action=="Delete"){
            return `删除 '${sels[0].name}'`;
          }
        }
      }
      return ''
    }
  },
  methods: {
    //校验数据配置的合法性
    validate(err) {
      this.showError = false

      if (!this.$isNotEmpty(this.config.props.targetform)) {
        this.showError = true
        this.errorInfo = '请选择目标表单'
      }
      if (this.config.props.conditions.length == 0) {
        this.showError = true
        this.errorInfo = '请添加执行过滤条件'
      }
      if (this.config.props.action == 'Update' && this.config.props.fields.length == 0) {
        this.showError = true
        this.errorInfo = '请添加修改字段'
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
