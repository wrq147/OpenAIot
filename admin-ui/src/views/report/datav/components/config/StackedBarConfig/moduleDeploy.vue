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
        <el-collapse-item  title="图例" name="3">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item label="标记宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.itemWidth"/>
          </el-form-item>
          <el-form-item label="标记高度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.itemHeight"/>
          </el-form-item> 
          <el-form-item label="字号">
            <el-slider v-model="configData.chartOption.legendFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="字体颜色">
            <el-color-picker v-model="configData.chartOption.legendFontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="布局朝向">
            <el-select v-model="configData.chartOption.legendOrient" placeholder="请选择">
              <el-option
                v-for="item in legendOrient"
                :key="item.key"
                :label="item.name"
                :value="item.key">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="水平位置">
            <el-input-number class="inputFontSize" :min="0" :max="100" v-model="configData.chartOption.legendX"/>
          </el-form-item>
          <el-form-item label="垂直位置">
            <el-input-number class="inputFontSize" :min="0" :max="100" v-model="configData.chartOption.legendY"/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="4">
          <el-form-item v-if="configData.chartOption.barWidth!==undefined" label="柱体宽度">
            <el-input v-model="configData.chartOption.barWidth" placeholder="请输入柱体宽度" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isVertical!==undefined" label="竖展示">
            <el-switch v-model="configData.chartOption.isVertical" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="坐标轴" name="5">
          <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="X轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="y轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="是否显示X轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="是否显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.show!==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
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
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
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