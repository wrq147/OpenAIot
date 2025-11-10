<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="散点" name="3">
          <el-form-item v-if="configData.chartOption.symbolSize!==undefined" label="散点大小">
            <el-slider  v-model="configData.chartOption.symbolSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color!==undefined" label="散点颜色">
            <el-color-picker v-model="configData.chartOption.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="X轴" name="4">
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示X轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴" name="5">
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="分隔线" name="6">
          <el-form-item v-if="configData.chartOption.isSplitLine!==undefined" label="显示分隔线">
            <el-switch v-model="configData.chartOption.isSplitLine" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="6">
          <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
      </el-collapse>
    </el-form>
</template>
<script>
import { animateOptions } from "../../../animate/animate";
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
    costomData: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      activeNames: ["1"],
      animateOptions,
      chartList: this.drawingList,
      detailFamily: ['宋体', '黑体', '微软雅黑', 'Digital', 'Chunkfive', 'unidreamLED', 'Arial', 'Helvetica', 'sans-serif'],
      detailWeight: ['normal', 'bold', 'bolder', 'lighter'],
    }
  },
  methods: {
  }
}
</script>
<style lang="scss" scoped>
::v-deep {
    .el-input--medium .el-input__inner {
      height: 32px;
      width: 100%;
    }
    .inputFontSize{
      width: 100%;
    }
    .el-select{
      height: 32px;
      width: 100%;
    }
    .el-input--suffix, .el-input__inner{
      height: 32px;
    }
    .el-select .el-input__icon {
      line-height: 32px; //el-select 改了多高，这边多高
    }
    .lableText .el-form-item__label{
        float: none;
    }
  }
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>