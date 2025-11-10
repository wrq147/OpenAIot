<template>
  <el-form size="small" label-width="60px">
    <el-form-item label="X位置">
      <el-input-number v-model="configData.x" controls-position="right" :step="1" />
    </el-form-item>
    <el-form-item label="y位置">
      <el-input-number  v-model="configData.y" controls-position="right" :step="1" />
    </el-form-item>
    <el-form-item label="宽度">
      <el-input-number  v-model="configData.width" controls-position="right" :step="1" />
    </el-form-item>
    <el-form-item label="高度">
      <el-input-number  v-model="configData.height" controls-position="right" :step="1" />
    </el-form-item>
    <el-form-item label="zIndex">
      <el-input-number  v-model="configData.zindex" controls-position="right" :step="1" />
    </el-form-item>
  </el-form>
</template>
<script>
export default {
  props: {
    configData: {
      type: Object,
      required: true
    }
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-input-number{
    width: 100%;
  }
}
</style>