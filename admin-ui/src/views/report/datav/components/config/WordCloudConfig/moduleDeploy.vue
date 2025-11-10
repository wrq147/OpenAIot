<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="82px" label-position="top" class="custom_form_item">

          <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.gridSize!==undefined" label="字符间隔">
            <el-input-number v-model="configData.chartOption.gridSize" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.sizeMin!==undefined" label="最小字体">
            <el-input-number v-model="configData.chartOption.sizeMin" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.sizeMax!==undefined" label="最大字体">
            <el-input-number v-model="configData.chartOption.sizeMax" controls-position="right" :min="1" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.rotationMin!==undefined" label="最小旋转角">
            <el-input-number v-model="configData.chartOption.rotationMin" controls-position="right" :min="-180" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.rotationMax!==undefined" label="最大旋转角">
            <el-input-number v-model="configData.chartOption.rotationMax" controls-position="right" :min="-180" :step="1"></el-input-number>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.shape!==undefined" label="形状">
            <el-select v-model="configData.chartOption.shape" placeholder="请选择">
              <el-option label="心形" value="cardioid"></el-option>
              <el-option label="菱形" value="diamond"></el-option>
              <el-option label="正方形" value="square"></el-option>
              <el-option label="五角形" value="pentagon"></el-option>
              <el-option label="五角星形" value="star"></el-option>
              <el-option label="三角形" value="triangle"></el-option>
            </el-select>
          </el-form-item>


          <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option
                v-for="item in animateOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>


      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";

export default {
  props: ["costomData"],
  data() {
    return {
      animateOptions,
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