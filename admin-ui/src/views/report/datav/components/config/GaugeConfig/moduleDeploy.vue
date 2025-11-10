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
        <el-collapse-item title="仪表盘轴线" name="3">
          <el-form-item v-if="configData.chartOption.isGradients!==undefined" label="是否渐变">
            <el-switch v-model="configData.chartOption.isGradients" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.radius!==undefined" label="仪表盘半径">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.radius"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisLine.lineStyle.width!==undefined" label="表盘轴线宽度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisLine.lineStyle.width"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].startAngle!==undefined" label="起始角度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].startAngle"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].endAngle!==undefined" label="终止角度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].endAngle"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="分割线" name="4">
          <el-form-item v-if="configData.chartOption.isSplitLineShow!==undefined" label="显示分割线">
            <el-switch v-model="configData.chartOption.isSplitLineShow" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].splitLine.length!==undefined" label="分割线长度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].splitLine.length"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].splitLine.lineStyle.width!==undefined" label="分割线宽度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].splitLine.lineStyle.width"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].splitLine.lineStyle.color!==undefined" label="分割线颜色">
            <el-color-picker v-model="configData.chartOption.series[0].splitLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].splitLine.lineStyle.type!==undefined" label="分隔线样式">
            <el-select v-model="configData.chartOption.series[0].splitLine.lineStyle.type" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].splitNumber!==undefined" label="分割段数">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].splitNumber"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="刻度线" name="5">
          <el-form-item v-if="configData.chartOption.isAxisTickShow!==undefined" label="显示刻度线">
            <el-switch v-model="configData.chartOption.isAxisTickShow" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisTick.splitNumber!==undefined" label="两条分割线间的刻度数">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisTick.splitNumber"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisTick.length!==undefined" label="刻度线长度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisTick.length"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisTick.width!==undefined" label="刻度线宽度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisTick.width"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisTick.lineStyle.color!==undefined" label="刻度线颜色">
            <el-color-picker v-model="configData.chartOption.series[0].axisTick.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisTick.lineStyle.type!==undefined" label="刻度线样式">
            <el-select v-model="configData.chartOption.series[0].axisTick.lineStyle.type" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="刻度值" name="6">
          <el-form-item v-if="configData.chartOption.series[0].axisLabel.distance!==undefined" label="刻度值和刻度线的距离">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisLabel.distance"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].axisLabel.fontSize!==undefined" label="刻度值字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].axisLabel.fontSize"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="7">
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