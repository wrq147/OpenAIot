<template>
  <div>
    <el-form-item label="工单类型">
        <el-select v-model="value.formType" placeholder="请选择">
            <el-option label="发起的工单" value=""></el-option>
            <el-option label="入库单" value="入库单"></el-option>
            <el-option label="出库单" value="出库单"></el-option>
            <el-option label="出库申请单" value="出库申请单"></el-option>
            <el-option label="告警工单" value="告警工单"></el-option>
        </el-select>
    </el-form-item>
    <el-form-item label="工单单号=" v-if="value.formType!=''">
      <el-select v-model="value.formId" placeholder="请选择值">
        <el-option
            v-for="item in formData"
            :key="item.id"
            :label="item.title"
            :value="item.id"
          ></el-option>
      </el-select>
    </el-form-item>
  </div>
  </template>
  
  <script>
  import {getItems} from '../../utlity.js'
  export default {
    name: "ParamInput",
    components: {},
    props:{
      value:{
        type: Object,
        default: ()=>{
          return {}
        }
      }
    },
    data() {
      return {
  
      }
    },
    computed:{
      formData() {
        return getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='TextInput');
      }
    },
    methods: {

    }
  }
  </script>
  
  <style scoped>
  </style>
  