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
        <el-collapse-item title="图例" name="3">
          <el-form-item v-if="configData.chartOption.showLegend!==undefined" label="是否显示图例">
            <el-switch v-model="configData.chartOption.showLegend" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.toolbox.show!==undefined" label="是否显示工具箱">
            <el-switch v-model="configData.chartOption.toolbox.show" />
          </el-form-item>
          <el-form-item label="标记宽度">
            <el-input-number v-model="configData.chartOption.itemWidth"/>
          </el-form-item>
          <el-form-item label="标记高度">
            <el-input-number v-model="configData.chartOption.itemHeight"/>
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
            <el-input-number :min="0" :max="100" v-model="configData.chartOption.legendX"/>
          </el-form-item>
          <el-form-item label="垂直位置">
            <el-input-number :min="0" :max="100" v-model="configData.chartOption.legendY"/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="x轴" name="4">
          <el-form-item v-if="configData.chartOption.xAxis[0].show!==undefined" label="x轴">
            <el-switch v-model="configData.chartOption.xAxis[0].show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.xAxis[0].show!==undefined" label="x轴线">
            <el-switch v-model="configData.chartOption.xAxis[0].axisLine.show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>

          <el-form-item  label="x轴颜色">
            <el-color-picker v-model="configData.chartOption.xAxis[0].axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>

          <el-form-item label="x轴粗细">
            <el-slider v-model="configData.chartOption.xLineWidth"  :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item label="x轴字号">
            <el-slider v-model="configData.chartOption.xFontSize" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item  label="x轴字体颜色">
            <el-color-picker v-model="configData.chartOption.xFontColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴" name="5"> 
          <el-form-item v-if="configData.chartOption.yAxis[0].show!==undefined" label="左侧y轴" >
            <el-switch v-model="configData.chartOption.yAxis[0].show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].show!==undefined" label="右侧y轴">
            <el-switch v-model="configData.chartOption.yAxis[1].show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[0].show!==undefined" label="左侧轴线" >
            <el-switch v-model="configData.chartOption.yAxis[0].axisLine.show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].show!==undefined" label="右侧轴线">
            <el-switch v-model="configData.chartOption.yAxis[1].axisLine.show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item label="y轴颜色">
            <el-color-picker v-model="configData.chartOption.yAxis[0].axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="y轴粗细">
            <el-slider v-model="configData.chartOption.yLineWidth"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="y轴字号">
            <el-slider v-model="configData.chartOption.yFontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item  label="y轴字体颜色">
            <el-color-picker v-model="configData.chartOption.yFontColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="折线图" name="6"> 
          <el-form-item v-if="configData.chartOption.yAxis[1].name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.yAxis[1].name" placeholder="请输入坐标轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].axisLabel.formatter!==undefined" label="刻度单位">
            <el-input v-model="configData.chartOption.yAxis[1].axisLabel.formatter" placeholder="请输入刻度单位" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isSmooth!==undefined" label="曲线是否平滑">
            <el-switch v-model="configData.chartOption.isSmooth" />
          </el-form-item>
          <el-form-item label="折线粗细">
            <el-input-number :step="1" v-model="configData.chartOption.lineWidth"/>
          </el-form-item>
          <el-form-item label="标记大小">
            <el-input-number :step="1" v-model="configData.chartOption.lineSymbolSize"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[1].splitLine.show !==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis[1].splitLine.show" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱状图" name="7">
          <el-form-item v-if="configData.chartOption.yAxis[0].name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.yAxis[0].name" placeholder="请输入坐标轴名称" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[0].axisLabel.formatter!==undefined" label="刻度单位">
            <el-input v-model="configData.chartOption.yAxis[0].axisLabel.formatter" placeholder="请输入刻度单位" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.data!==undefined" label="分类数据" >
            <el-input type="textarea" :rows="3" v-model="configData.chartOption.xAxis.data" ></el-input>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.barWidth!==undefined" label="柱体宽度">
            <el-slider v-model="configData.chartOption.barWidth" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="是否堆叠">
            <el-switch v-model="configData.chartOption.isStack" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[0].splitLine.show !==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis[0].splitLine.show" />
          </el-form-item>
          <el-form-item  v-if="configData.chartOption.yAxis[0].splitLine.show" label="样式">
            <el-select v-model="configData.chartOption.lineStyle" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[0].splitLine.show" label="粗细">
            <el-input-number :min="1" :max="10" v-model="configData.chartOption.splitLineWidth"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis[0].splitLine.show" label="颜色">
            <el-color-picker v-model="configData.chartOption.splitLineColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="辅助功能" name="8">
          <el-form-item label="显示平均线">
            <el-switch v-model="configData.chartOption.isMarkLine" />
          </el-form-item>
          <el-form-item label="显示最大最小值">
            <el-switch v-model="configData.chartOption.isMarkPoint" />
          </el-form-item>
          <el-form-item label="是否竖显示">
            <el-switch v-model="configData.chartOption.isVertical" />
          </el-form-item>
          <el-form-item label="左边距">
            <el-input-number v-model="configData.chartOption.gridLeft" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="右边距">
            <el-input-number v-model="configData.chartOption.gridRight" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="上边距">
            <el-input-number v-model="configData.chartOption.gridTop" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>
          <el-form-item label="下边距">
            <el-input-number v-model="configData.chartOption.gridBottom" controls-position="right"  :step="1"></el-input-number>
          </el-form-item>                                          
        </el-collapse-item>
        <el-collapse-item title="动画" name="9">
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