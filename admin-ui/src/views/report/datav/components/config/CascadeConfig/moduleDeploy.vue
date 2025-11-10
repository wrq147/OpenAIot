<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="90px">
        <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
          <el-input v-model="configData.layerName" placeholder="请输入图层名称"/>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.context !== undefined" label="label内容">
          <el-input v-model="configData.chartOption.context" placeholder="请输入label内容"/>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.fontSize !== undefined" label="label字号">
          <el-slider v-model="configData.chartOption.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.fontFamily !== undefined" label="label字体">
          <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
            <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item">
            </el-option>
          </el-select>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.fontColor !== undefined" label="label字体颜色">
          <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.fontWeight !== undefined" label="文字粗细">
          <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
            <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item">
            </el-option>
          </el-select>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.name !== undefined" label="name属性值">
          <el-input v-model="configData.chartOption.name" placeholder="请输入name属性值"/>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.width !== undefined" label="宽度">
          <el-slider v-model="configData.chartOption.width" :min="1" :max="500" :step="1" show-input></el-slider>
        </el-form-item>

        <el-form-item label="绑定组件">
          <el-select v-model="configData.chartOption.bindList" multiple filterable placeholder="请选择" @change="bindCharts">
            <el-option v-for="item in chartList" :key="item.customId" :label="item.layerName" :value="item.customId">
            </el-option>
          </el-select>
        </el-form-item>

        <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
          <el-select v-model="configData.chartOption.animate" placeholder="请选择">
            <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value">
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { getLinkChart } from "../../../util/LinkageChart";
export default {
  props: ["costomData", "drawingList"],
  data() {
    return {
      fontFamilys: [
        "宋体",
        "黑体",
        "微软雅黑",
        "Digital",
        "Chunkfive",
        "unidreamLED",
        "Arial",
        "Helvetica",
        "sans-serif",
      ],
      fontWeights: ["normal", "bold", "bolder", "lighter"],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {},
  methods: {
    bindCharts(val) {
      this.$set(this.configData.chartOption, "selectedCharts", val);
    },
  },
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
