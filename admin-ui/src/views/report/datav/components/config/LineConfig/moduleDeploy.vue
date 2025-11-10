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
        <el-collapse-item title="x轴设置" name="3">
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示X轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
          </el-form-item>
          <el-form-item label="坐标轴名称位置">
            <el-select v-model="configData.chartOption.xAxisnameLocation" placeholder="请选择">
              <el-option v-for="item in nameLocation" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴名称字体">
            <el-select v-model="configData.chartOption.xAxisFontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴名称字号">
            <el-slider v-model="configData.chartOption.xAxisFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="坐标轴名称颜色">
            <el-color-picker v-model="configData.chartOption.xAxisFontColor" show-alpha />
          </el-form-item>
          <el-form-item label="显示轴线">
            <el-switch v-model="configData.chartOption.xAxisLineShow"/>
          </el-form-item>
          <el-form-item label="轴线粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.xAxisLineWidth"/>
          </el-form-item>
          <el-form-item label="轴线颜色">
            <el-color-picker v-model="configData.chartOption.xAxisLineColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="显示标签">
            <el-switch v-model="configData.chartOption.xLabelShow" />
          </el-form-item>
          <el-form-item label="标签字体">
            <el-select v-model="configData.chartOption.xLabelFontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="标签字号">
            <el-slider v-model="configData.chartOption.xLabelFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标签颜色">
            <el-color-picker v-model="configData.chartOption.xLabelFontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="坐标轴留白">
            <el-switch v-model="configData.chartOption.xAxis.boundaryGap" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴设置" name="4">
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>
          <el-form-item label="坐标轴名称位置">
            <el-select v-model="configData.chartOption.yAxisnameLocation" placeholder="请选择">
              <el-option v-for="item in nameLocation" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴名称字体">
            <el-select v-model="configData.chartOption.yAxisFontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴名称字号">
            <el-slider v-model="configData.chartOption.yAxisFontSize" :step="1" show-input />
          </el-form-item>
          <el-form-item label="坐标轴名称颜色">
            <el-color-picker v-model="configData.chartOption.yAxisFontColor" show-alpha />
          </el-form-item>
          <el-form-item label="显示轴线">
            <el-switch v-model="configData.chartOption.yAxisLineShow"/>
          </el-form-item>
          <el-form-item label="轴线粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.yAxisLineWidth"/>
          </el-form-item>
          <el-form-item label="轴线颜色">
            <el-color-picker v-model="configData.chartOption.yAxisLineColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="显示标签">
            <el-switch v-model="configData.chartOption.yLabelShow"/>
          </el-form-item>
          <el-form-item label="标签字体">
            <el-select v-model="configData.chartOption.yLabelFontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="标签字号">
            <el-slider v-model="configData.chartOption.yLabelFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标签颜色">
            <el-color-picker v-model="configData.chartOption.yLabelFontColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例设置" name="5">
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
          <el-form-item label="文字颜色">
            <el-color-picker v-model="configData.chartOption.legend.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="布局朝向">
            <el-select v-model="configData.chartOption.legendOrient" placeholder="请选择">
              <el-option v-for="item in legendOrient" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="水平位置">
            <el-input-number class="inputFontSize" :min="0" :max="100" v-model="configData.chartOption.legendX"/>
          </el-form-item>
          <el-form-item label="垂直位置">
            <el-input-number class="inputFontSize" :min="0" :max="100" v-model="configData.chartOption.legendY"/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="分隔线设置" name="6">
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.show!==undefined" label="分隔线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
          </el-form-item>
          <el-form-item label="虚线">
            <el-select v-model="configData.chartOption.splitLineType" placeholder="请选择">
              <el-option v-for="item in splitLineType" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="粗细">
            <el-input-number class="inputFontSize" :min="1" :max="10" v-model="configData.chartOption.splitLineWidth"/>
          </el-form-item>
          <el-form-item label="颜色">
            <el-color-picker v-model="configData.chartOption.splitLineColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="折线设置" name="7">
          <el-form-item v-if="configData.chartOption.isSmooth!==undefined" label="折线是否平滑">
            <el-switch v-model="configData.chartOption.isSmooth" />
          </el-form-item>
          <el-form-item label="标记大小">
            <el-input-number class="inputFontSize" :min="0" v-model="configData.chartOption.lineSymbolSize"/>
          </el-form-item>
          <el-form-item label="折线粗细">
            <el-input-number class="inputFontSize" :min="0" v-model="configData.chartOption.lineWidth"/>
          </el-form-item>
          <el-form-item label="显示平均线">
            <el-switch v-model="configData.chartOption.isMarkLine" />
          </el-form-item>
          <el-form-item label="显示最值">
            <el-switch v-model="configData.chartOption.isMarkPoint" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="动画" name="8">
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
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      fontFamilys:[],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
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