<template>
  <div>
    <el-form-item label="提示文字">
      <el-input size="small" v-model="value.placeholder" placeholder="提醒添加记录的提示"/>
    </el-form-item>
    <el-form-item label="最大行数">
      <tip slot="label" content="允许添加多少条记录（为0则不限制）">最大行数</tip>
      <el-input-number controls-position="right" :precision="0" :max="100" :min="0" size="small" v-model="value.maxSize"  placeholder="限制条数"/>
    </el-form-item>
    <el-form-item label="布局方式">
      <el-radio name="layout" :label="true" v-model="value.rowLayout">按表格</el-radio>
      <el-radio name="layout" :label="false" v-model="value.rowLayout">按表单</el-radio>
    </el-form-item>
    <el-form-item label="索引字段">
      <el-select style="width: 100%;" size="small" v-model="value.IdxColName" multiple clearable placeholder="请选择要索引的字段">
        <el-option :label="column.title" :value="column.id" v-for="column in IdxColumns" :key="column.id"/>
      </el-select>
    </el-form-item>
    <el-form-item label="展示合计">
      <el-switch v-model="value.showSummary"></el-switch>
      <el-select v-if="value.showSummary" style="width: 100%;" size="small" v-model="value.summaryColumns" multiple clearable placeholder="请选择合计项">
        <el-option :label="column.title" :value="column.id" v-for="column in columns" :key="column.id"/>
      </el-select>
      <el-input v-if="value.showSummary" style="width: 100%;" v-model="value.summaryUnit" placeholder="合计单位"></el-input>
      <el-select v-if="value.showSummary" style="width: 100%;" size="small" v-model="value.deductid" clearable placeholder="请选择关联字段">
        <el-option
        clearable
            v-for="item in formData"
            :key="item.id"
            :label="item.title"
            :value="item.id"
          ></el-option>
      </el-select>
    </el-form-item>
    <el-form-item label="展示边框">
      <el-switch v-model="value.showBorder"></el-switch>
    </el-form-item>
  </div>
</template>

<script>
import Tip from '../../Tip.vue'
import {getItems} from '../../utlity.js'
export default {
  name: "TableListConfig",
  components: {Tip},
  props:{
    value:{
      type: Object,
      default: ()=>{
        return {}
      }
    }
  },
  computed:{
    columns(){
      return this.value.columns.filter(c => c.valueType === 'Number')
    },
    IdxColumns() {
      return this.value.columns.filter(c => c.valueType === 'Number'||c.valueType==='String')
    },
    formData() {
      return getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='AmountInput'||x.name=="NumberInput");
    }
  },
  data() {
    return {}
  },
  methods: {

  }
}
</script>

<style scoped>

</style>
