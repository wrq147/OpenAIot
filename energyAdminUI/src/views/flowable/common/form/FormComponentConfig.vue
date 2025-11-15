<template>
  <div>
    <el-form label-width="90px" v-if="form.name !== 'SpanLayout'">
      <el-form-item label="字段名称">
        <el-input size="small" clearable v-model="form.title"/>
      </el-form-item>
      <component :is="form.name" v-model="form.props"/>
      <el-form-item label="必填项">
        <el-switch v-model="form.props.required"></el-switch>
      </el-form-item>
      <el-form-item label="可打印">
        <el-switch v-model="form.props.enablePrint"></el-switch>
      </el-form-item>
    </el-form>
    <el-empty v-else description="当前组件不支持配置"></el-empty>
  </div>

</template>

<script>
import TextInput from './config/TextInputConfig.vue'
import NumberInput from './config/NumberInputConfig.vue'
import AmountInput from './config/AmountInputConfig.vue'
import TextareaInput from './config/TextareaInputConfig.vue'
import MultipleSelect from './config/SelectInputConfig.vue'
import SelectInput from './config/SelectInputConfig.vue'
import DateTime from './config/DateTimeConfig.vue'
import DateTimeRange from './config/DateTimeRangeConfig.vue'
import ImageUpload from './config/ImageUploadConfig.vue'
import FileUpload from './config/FileUploadConfig.vue'
import Description from './config/DescriptionConfig.vue'
import DeptPicker from './config/OrgPickerConfig.vue'
import UserPicker from './config/OrgPickerConfig.vue'
import DevicPicker from './config/DevicPickerConfig.vue'
import TableList from './config/TableListConfig.vue'
import ParamInput from './config/ParamInputConfig.vue'

export default {
  name: "FormComponentConfig",
  components: {
    TextInput,
    NumberInput,
    AmountInput,
    TextareaInput,
    SelectInput,
    MultipleSelect,
    DateTime,
    DateTimeRange,
    ImageUpload,
    FileUpload,
    Description,
    DeptPicker,
    UserPicker,
    DevicPicker,
    TableList,
    ParamInput
  },
  props:{},
  computed:{
    form(){
      return this.$store.state.flowable.selectFormItem
    }
  },
  data() {
    return {}
  },
  methods: {}
}
</script>

<style scoped>

</style>
