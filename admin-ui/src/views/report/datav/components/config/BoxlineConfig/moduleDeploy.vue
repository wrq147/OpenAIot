<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.x!==undefined" label="标题位置">
            <el-select v-model="configData.chartOption.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="标题字号">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.fontSize" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
              <el-option label="normal" value="normal"></el-option>
              <el-option label="bold" value="bold"></el-option>
              <el-option label="bolder" value="bolder"></el-option>
              <el-option label="lighter" value="lighter"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontFamily!==undefined" label="字体样式">
            <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="箱线样式" name="3">
          <el-form-item v-if="configData.chartOption.series[0].itemStyle.normal.borderWidth!==undefined" label="箱框粗细">
            <el-input-number class="inputFontSize" size="mini" controls-position="right" v-model="configData.chartOption.series[0].itemStyle.normal.borderWidth" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic!==undefined" label="使用主题">
            <el-switch v-model="configData.chartOption.isTopic" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic === false" label="箱框0%颜色">
            <el-color-picker v-model="configData.chartOption.series[0].itemStyle.normal.borderColor.colorStops[0].color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic === false" label="箱框100%处颜色">
            <el-color-picker v-model="configData.chartOption.series[0].itemStyle.normal.borderColor.colorStops[1].color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic === false" label="箱体0%颜色">
            <el-color-picker v-model="configData.chartOption.series[0].itemStyle.normal.color.colorStops[0].color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isTopic === false" label="箱体100%处颜色">
            <el-color-picker v-model="configData.chartOption.series[0].itemStyle.normal.color.colorStops[1].color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isVertical!==undefined" label="是否竖显示">
            <el-switch v-model="configData.chartOption.isVertical" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="坐标轴样式" name="4">
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示x轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show===true" label="x轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入x轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show===true" label="x轴分割线">
            <el-switch v-model="configData.chartOption.xAxis.splitLine.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show===true" label="x轴颜色">
            <el-color-picker v-model="configData.chartOption.xAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show===true" label="y轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show===true" label="y轴单位">
            <el-input v-model="configData.chartOption.unit" placeholder="请输入坐标轴单位" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show===true" label="y轴分割线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show===true" label="y轴颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="5">
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
      fontFamilys: []
    }
  },
  methods: {}
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
    .el-slider{
      width: 95%;
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