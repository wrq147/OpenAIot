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
        <el-collapse-item title="提示框" name="3">
          <el-form-item label="提示单位">
            <el-input v-model="configData.chartOption.unit"  placeholder="请输入提示单位" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="饼图" name="4">
          <el-form-item  v-if="configData.chartOption.isRoseType !== undefined"  label="南丁格尔玫瑰">
            <el-switch v-model="configData.chartOption.isRoseType" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.radiusI !== undefined" label="内半径(%)">
            <el-slider v-model="configData.chartOption.radiusI" :min="1" :max="100" :step="1" show-input />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.radiusO !== undefined" label="外半径(%)">
            <el-slider v-model="configData.chartOption.radiusO" :min="1" :max="100" :step="1" show-input />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.centerX !== undefined" label="与左侧的距离(%)">
            <el-slider v-model="configData.chartOption.centerX" :min="1" :max="100" :step="1" show-input />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.centerY !== undefined" label="与上侧的距离(%)">
            <el-slider v-model="configData.chartOption.centerY" :min="1" :max="100" :step="1" show-input />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱状图" name="5">
          <el-form-item v-if="configData.chartOption.isVertical !== undefined" label="竖展示">
            <el-switch v-model="configData.chartOption.isVertical" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.xAxis.show !== undefined" label="x轴是否显示">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.yAxis.show !== undefined" label="y轴是否显示">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.xAxis.name !== undefined" label="x轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入x轴名称" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.yAxis.name !== undefined" label="y轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.series[0].barWidth !== undefined" label="柱体宽度">
            <el-slider v-model="configData.chartOption.series[0].barWidth" :min="1" :max="100" :step="1"  show-input />
          </el-form-item>

          <el-form-item label="显示平均线">
            <el-switch v-model="configData.chartOption.isMarkLine" />
          </el-form-item>

          <el-form-item label="显示最大最小值">
            <el-switch v-model="configData.chartOption.isMarkPoint" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.yAxis.splitLine.show !== undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例" name="6">
          <el-form-item v-if="configData.chartOption.legend !== undefined" label="是否显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.legendcenterX !== undefined" label="与左侧的距离(%)">
            <el-slider v-model="configData.chartOption.legendcenterX" :min="1" :max="100" :step="1" show-input />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.legendcenterY !== undefined" label="与上侧的距离(%)">
            <el-slider v-model="configData.chartOption.legendcenterY" :min="1" :max="100" :step="1" show-input  />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.legend.orient !== undefined" label="图例方向">
            <el-radio-group v-model="configData.chartOption.legend.orient">
              <el-radio label="horizontal">横向</el-radio>
              <el-radio label="vertical">纵向</el-radio>
            </el-radio-group>
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
  }
  .delete-icon {
    line-height: 32px;
    font-size: 22px;
    padding: 0 4px;
    cursor: pointer;
    color: #f56c6c;
  }
</style>