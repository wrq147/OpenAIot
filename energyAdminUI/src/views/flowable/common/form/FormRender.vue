<template>
  <!--渲染表单-->
  <el-form ref="form" class="process-form" label-position="top" :rules="rules" :model="_value">
    <el-form-item v-if="item.name !== 'SpanLayout' && item.name !== 'Description'" :prop="item.id" :label="item.title" v-for="(item, index) in forms" :key="item.name + index">
      <form-design-render :ref="`sub-item_${item.id}`" v-model="_value[item.id]" :valueModel="value" mode="PC" :config="item" :disabled="item.props.disabled||disabled"/>
    </el-form-item>
    <form-design-render ref="span-layout" v-else v-model="_value" mode="PC" :valueModel="value" :config="item" :disabled="item.props.disabled||disabled"/>
  </el-form>
</template>

<script>
import FormDesignRender from '../../definition/layout/form/FormDesignRender'

export default {
  name: "FormRender",
  components: {FormDesignRender},
  props:{
    forms: {
      type: Array,
      default: () => {
        return []
      }
    },
    optionInit: {
      type: Array,
      default: () => {
        return []
      }
    },
    value: {
      type: Object,
      default: () => {
        return {}
      }
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      rules: {},
      isFirst:true,
      optionInitIdList:[]
    }
  },
  watch:{
    forms:{
      handler(to){
        if(to&&to.length>0&&this.isFirst){
          this.loadFormConfig(to)
          this.isFirst=false
        }
      },
      immediate:true,
      deep:true
    }
  },
  mounted() {
    
  },
  computed: {
    _value:{
      get(){
        return this.value
      },
      set(val){
        this.$emit('input', val)
      }
    }
  },
  methods: {
    validate(call,ac) {
      let success = true
      this.$refs.form.validate(valid => {
        success = valid
        if(valid){
          //校验成功再校验内部
          for (let i = 0; i < this.forms.length; i++) {
            if (this.forms[i].name === 'TableList'){
              let formRef = this.$refs[`sub-item_${this.forms[i].id}`]
              if (formRef && Array.isArray(formRef) && formRef.length > 0){
                formRef[0].validate(subValid => {
                  success = subValid
                })
                if (!success){
                  break
                }
              }
            }else{
              if(this.optionInitIdList.includes(this.forms[i].id)){//操作填写初始化提示
                if(this.forms[i].props.required&&this._value[this.forms[i].id]==''||this.forms[i].props.required&&this._value[this.forms[i].id]==null){
                  let acname=this.optionInit.find(rws=>rws.fieldid==this.forms[i].id)
                  if(acname&&acname.optionName !==ac){
                    // this.$refs.promptMsg.open('请输入'+this.forms[i].title, 3000)
                    this.$modal.msgError('请输入'+this.forms[i].title);
                    success=false
                    break
                  }
                  
                }
              }
            }
          }
        }
        call(success)
      });
    },
    loadFormConfig(forms){
      if(this.optionInit){
        this.optionInitIdList=this.optionInit.map(row=>row.fieldid)
      }
      for(let i=0;i<forms.length;i++){
        let item=forms[i]
        if (item.name === 'SpanLayout'){
          this.loadFormConfig(item.props.items)
        }else {
          this.$set(this._value, item.id, this.value[item.id])
          if(!item.props.disabled&&item.props.required){
            if(this.optionInitIdList&&this.optionInitIdList.includes(item.id)){}else{
              this.$set(this.rules, item.id, [{
                type: item.valueType === 'Array' ? 'array':undefined,
                required: true,
                message: `请填写${item.title}`, trigger: 'blur'
              }])
            }
            
          }
        }
      }
    }
  }
}
</script>

<style lang="less" scoped>
.process-form {
  /deep/ .el-form-item__label {
    padding: 0 0;
  }
}
</style>
