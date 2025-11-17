<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="54px" class="custom_form_item">
        <el-form-item label="X位置">
          <el-input-number v-model="configData.x" controls-position="right" :step="1"/>
        </el-form-item>
        <el-form-item label="y位置">
          <el-input-number v-model="configData.y" controls-position="right" :step="1"/>
        </el-form-item>
        <el-form-item label="宽度">
          <el-input-number v-model="configData.width" controls-position="right" :step="1"/>
        </el-form-item>
        <el-form-item label="高度">
          <el-input-number v-model="configData.height" controls-position="right" :step="1"/>
        </el-form-item>
        <el-form-item label="zIndex">
          <el-input-number v-model="configData.zindex" controls-position="right" :step="1"/>
        </el-form-item>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
export default {
  props: ["costomData"],
  data() {
    return {
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {},
  computed: {},
  methods: {},
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>