<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item  title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.x!==undefined" label="标题位置">
            <el-select v-model="configData.chartOption.title.x" placeholder="请选择">
              <el-option label="居左" value="left"></el-option>
              <el-option label="居中" value="center"></el-option>
              <el-option label="居右" value="right"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontSize!==undefined" label="标题字号">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.title.textStyle.fontSize"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
              <el-option label="normal" value="normal"></el-option>
              <el-option label="bold" value="bold"></el-option>
              <el-option label="bolder" value="bolder"></el-option>
              <el-option label="lighter" value="lighter"></el-option>
            </el-select> 
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontFamily!==undefined" label="字体样式">
            <el-select v-model="configData.chartOption.title.textStyle.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="水球" name="3">
          <el-form-item label="水球轮廓">
            <el-switch v-model="configData.chartOption.series[0].outline.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.borderDistance!==undefined" label="轮廓间距">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].outline.borderDistance"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[0].outline.itemStyle.borderWidth!==undefined" label="轮廓粗细">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.series[0].outline.itemStyle.borderWidth"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="基准线" name="4">
          <el-form-item label="关闭基准线事件">
            <el-switch v-model="configData.chartOption.series[1].markLine.silent" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[1].markLine.lineStyle.type!==undefined" label="基准线样式">
            <el-select v-model="configData.chartOption.series[1].markLine.lineStyle.type" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.series[1].markLine.lineStyle.color!==undefined" label="基准线颜色">
            <el-color-picker v-model="configData.chartOption.series[1].markLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="轴线" name="5">
          <el-form-item label="显示x轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
          <el-form-item label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
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
import draggable from "vuedraggable";
export default {
  props: {
    configData: {
      type: Object,
      required: true
    },
  },
  components: {
    draggable
  },
  data() {
    return {
      activeNames: ["1"],
      fontFamilys: [],
      animateOptions,
    }
  },
  methods: {}
}
</script>
<style lang="scss" scoped>
::v-deep {
  .el-input-numbe, .el-input-number--small{
    width: 100% !important;
  }
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
</style>