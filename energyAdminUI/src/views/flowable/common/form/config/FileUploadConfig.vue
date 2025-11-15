<template>
  <div>
    <el-form-item label="提示文字">
      <el-input size="small" v-model="value.placeholder" placeholder="请设置提示语"/>
    </el-form-item>
    <el-form-item label="数量限制">
      <tip slot="label" content="限制最大上传图片数量（为0则不限制）">数量限制</tip>
      <el-input-number class="max-fill" controls-position="right" :precision="0" size="small" v-model="value.maxNumber" placeholder="最多上传几张图片"/>
    </el-form-item>
    <el-form-item label="大小限制">
      <tip slot="label" content="限制单个文件最大大小-MB（为0则不限制）">大小限制</tip>
      <el-input-number class="max-fill" controls-position="right" :precision="1" size="small" v-model="value.maxSize" placeholder="单个文件最大大小"/>
    </el-form-item>
    <el-form-item label="类型限制">
      <tip slot="label" content="限制上传文件的后缀类型">类型限制</tip>
      <el-select size="small" style="width: 100%;" v-model="value.fileTypes" multiple
                 filterable allow-create default-first-option clearable placeholder="允许上传文件的后缀格式，可设置多种">
          <el-option
          v-for="item in options"
          :key="item.value"
          :label="item.label"
          :value="item.value">
        </el-option>
      </el-select>
    </el-form-item>
    <el-form-item label="不可下载">
      <el-switch v-model="value.onlyRead"></el-switch>
    </el-form-item>
  </div>
</template>

<script>
import Tip from '../../Tip.vue'
export default {
  name: "FileUploadConfig",
  components: {Tip},
  props: {
    value: {
      type: Object,
      default: () => {
        return {}
      }
    }
  },
  data() {
    return {
      options: [{
          value: 'doc',
          label: '.doc'
        }, {
          value: 'xls',
          label: '.xls'
        }, {
          value: 'ppt',
          label: '.ppt'
        }, {
          value: 'txt',
          label: '.txt'
        }, {
          value: 'pdf',
          label: '.pdf'
        }]
    }
  },
  methods: {}
}
</script>

<style lang="less" scoped>
/deep/ .el-form-item__label{
  padding: 0 12px 0 0;
}
</style>
