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
        <el-collapse-item title="柱体" name="3">
          
          <el-form-item v-if="configData.chartOption.barWidth!==undefined" label="柱体宽度">
            <!--<el-input v-model="configData.chartOption.barWidth" placeholder="请输入柱体宽度" />-->
            <el-input-number class="inputFontSize" controls-position="right" v-model="configData.chartOption.barWidth" />
          </el-form-item>

           <el-form-item v-if="configData.chartOption.stack!==undefined" label="是否堆叠">
            <el-switch v-model="configData.chartOption.stack" />
          </el-form-item>

          <el-form-item  label="是否重叠">
            <el-switch v-model="configData.chartOption.gap" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.itemStyle!==undefined" label="圆角显示">
            <el-switch v-model="configData.chartOption.itemStyle" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.itemStyle == true" label="圆角类型">
            <el-radio-group v-model="configData.chartOption.radiusType">
              <el-radio label="half">半圆角</el-radio>
              <el-radio label="full">全圆角</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.itemStyle == true" label="圆角半径">
              <el-slider v-model="configData.chartOption.radius" :step="1" show-input></el-slider>
          </el-form-item>            

          <el-form-item label="显示平均线">
            <el-switch v-model="configData.chartOption.isMarkLine" />
          </el-form-item>

          <el-form-item label="显示最大最小值">
            <el-switch v-model="configData.chartOption.isMarkPoint" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.isVertical!==undefined" label="是否竖显示">
            <el-switch v-model="configData.chartOption.isVertical" />
          </el-form-item>
          
          <el-form-item label="显示标签">
            <el-switch v-model="configData.chartOption.isLabel" />
          </el-form-item>
              
          <el-form-item label="标签位置">
            <el-select v-model="configData.chartOption.labelPosition" placeholder="请选择">
              <el-option
                  v-for="(item,index) in labelPosition"
                  :key="index"
                  :label="item.label"
                  :value="item.key">
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="距离">
            <el-input-number class="inputFontSize" :min="1"  v-model="configData.chartOption.labelDistance"/>
          </el-form-item>

          <el-form-item label="标签字号">
            <el-slider v-model="configData.chartOption.labelFontSize" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item  label="标签颜色">
            <el-color-picker v-model="configData.chartOption.labelFontColor" show-alpha></el-color-picker>
          </el-form-item>

        </el-collapse-item>
        <el-collapse-item title="x轴" name="4">
          
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示X轴">
            <el-switch v-model="configData.chartOption.xAxis.show" active-text="是" inactive-text="否" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="X轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
          </el-form-item>


          <el-form-item  label="x轴线">
            <el-switch v-model="configData.chartOption.xLineShow" active-text="显示" inactive-text="不显示"/>
          </el-form-item>

          <el-form-item  label="x轴颜色">
            <el-color-picker v-model="configData.chartOption.xLineColor" show-alpha></el-color-picker>
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
        
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" active-text="是" inactive-text="否" />
          </el-form-item>

          <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="y轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>

          <el-form-item  label="y轴线">
            <el-switch v-model="configData.chartOption.yLineShow" active-text="显示" inactive-text="不显示"/>
          </el-form-item>

          <el-form-item  label="y轴颜色">
            <el-color-picker v-model="configData.chartOption.yLineColor" show-alpha></el-color-picker>
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

        <el-collapse-item title="分隔线" name="6">

          <el-form-item v-if="configData.chartOption.yAxis.splitLine.show!==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
          </el-form-item>
          <el-form-item label="样式">
            <el-select v-model="configData.chartOption.lineStyle" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="粗细">
            <el-input-number class="inputFontSize" :min="1" :max="10" v-model="configData.chartOption.splitLineWidth"/>
          </el-form-item>
          <el-form-item label="颜色">
            <el-color-picker v-model="configData.chartOption.splitLineColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        
        <el-collapse-item title="区域缩放" name="7">

          <el-form-item  label="是否开启">
            <el-switch v-model="configData.chartOption.dataZoomShow"/>
          </el-form-item>

          <el-form-item  label="类型">
            <el-radio-group v-model="configData.chartOption.dataZoomType">
              <el-radio label="inside">内置</el-radio>
              <el-radio label="slider">滑动条</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item  label="控制项">
            <el-checkbox-group v-model="axis" @change="handleCheckedChange">
              <el-checkbox  label="x轴" ></el-checkbox>
              <el-checkbox  label="y轴" ></el-checkbox>
            </el-checkbox-group>
          </el-form-item>

          <el-form-item label="起始位置">
            <el-slider v-model="configData.chartOption.dataZoomStart"  :step="1" :min="0" show-input></el-slider>
          </el-form-item>
          <el-form-item label="终点位置">
            <el-slider v-model="configData.chartOption.dataZoomEnd"  :step="1" :min="0" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.dataZoomType=='slider'" label="手柄尺寸">
            <el-slider v-model="configData.chartOption.handleSize"  :step="1" :min="0" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.dataZoomType=='slider'" label="手柄文字大小">
            <el-input-number class="inputFontSize" :min="1" v-model="configData.chartOption.dataZoomFontSize"/>
          </el-form-item>
            
          <el-form-item v-if="configData.chartOption.dataZoomType=='slider'" label="手柄文字颜色">
            <el-color-picker v-model="configData.chartOption.dataZoomFontColor" show-alpha></el-color-picker>
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
      activeNames: ["1"],
      animateOptions,
      labelPosition:[
        {key:"top",label:"上"},{key:"left",label:"左"},{key:"right",label:"右"},{key:"bottom",label:"下"},{key:"inside",label:"内部中间"},
        {key:"insideLeft",label:"中间左部"},{key:"insideRight",label:"中间右部"},{key:"insideTop",label:"中间上部"},{key:"insideBottom",label:"中间下部"},
        {key:"insideTopLeft",label:"内部左上"},{key:"insideBottomLeft",label:"内部左下"},{key:"insideTopRight",label:"内部右上"},{key:"insideBottomRight",label:"内部右下"}],
      chartList: this.drawingList,
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      axis: this.costomData.chartOption.axis != undefined ? this.costomData.chartOption.axis: []
    }
  },
  methods: {
    handleCheckedChange(val){
      this.$set(this.configData.chartOption, 'axis', val);
    },
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