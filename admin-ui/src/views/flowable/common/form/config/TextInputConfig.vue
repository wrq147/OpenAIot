<template>
  <div>
    <el-form-item label="提示文字">
      <el-input size="small" v-model="value.placeholder" placeholder="请设置提示语"/>
    </el-form-item>
    <el-form-item label="默认值">
      <el-autocomplete
          v-model="value.defaultValue"
          :fetch-suggestions="querySearch"
          placeholder="请输入内容"
          @select="handleSelect" :clearable="true"
        >
        <template slot-scope="{ item }">
          <div class="atname">{{ item.name }}</div>
        </template>
      </el-autocomplete>
    </el-form-item>
  </div>

</template>

<script>
export default {
  name: "TextInput",
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
  methods: {
    querySearch(queryString, cb) {
        // 调用 callback 返回建议列表的数据
        cb([{"name":"空字符串","value": "" },{"name":"流程单号","value": "@FlowNumber" }]);
    },
    handleSelect(item) {
      this.value.defaultValue=item.value;
    }
  }
}
</script>

<style scoped>
    .atname {
      text-overflow: ellipsis;
      overflow: hidden;
    }
</style>
