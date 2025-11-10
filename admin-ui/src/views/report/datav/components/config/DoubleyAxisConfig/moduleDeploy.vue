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
          <el-form-item label="副标题类型">
            <el-radio-group v-model="configData.chartOption.subtextType">
              <el-radio label="0">自定义</el-radio>
              <el-radio label="1">总数</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item label="标题字体大小">
            <el-slider v-model="configData.chartOption.title.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="标题和副标题距离">
            <el-slider v-model="configData.chartOption.title.itemGap" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.color!==undefined" label="标题字体颜色">
            <el-color-picker v-model="configData.chartOption.title.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtextStyle.fontSize!==undefined" label="副标题字体大小">
            <el-slider v-model="configData.chartOption.title.subtextStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtextStyle.color!==undefined" label="副标题字体颜色">
            <el-color-picker v-model="configData.chartOption.title.subtextStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="标题上边距">
            <el-slider v-model="configData.chartOption.title.top" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题左边距">
            <el-slider v-model="configData.chartOption.title.left" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题对齐方式">
            <el-select v-model="configData.chartOption.title.textAlign" placeholder="请选择">
              <el-option  label="左对齐" value="left"></el-option>
              <el-option  label="中间对齐" value="center"></el-option>
              <el-option  label="右对齐" value="right"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图表位置" name="3">
          <el-form-item v-if="configData.chartOption.top!==undefined " label="与上侧的距离(%)">
            <el-slider v-model="configData.chartOption.top" :min="-100" :max="100" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.bottom!==undefined " label="与下侧的距离(%)">
            <el-slider v-model="configData.chartOption.bottom" :min="-100" :max="100" :step="1" show-input></el-slider>
          </el-form-item>

            <el-form-item v-if="configData.chartOption.left!==undefined " label="与左侧的距离(%)">
            <el-slider v-model="configData.chartOption.left" :min="-100" :max="100" :step="1" show-input></el-slider>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.right!==undefined " label="与右侧的距离(%)">
            <el-slider v-model="configData.chartOption.right" :min="-100" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="4">
          <el-form-item v-if="configData.chartOption.backgroundData!==undefined " label="柱体总量">
            <el-input-number v-model="configData.chartOption.backgroundData" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.barWidth!==undefined " label="柱体宽度">
            <el-slider v-model="configData.chartOption.barWidth" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.barBorderRadius!==undefined " label="柱体边框半径">
            <el-slider v-model="configData.chartOption.barBorderRadius" :min="1" :max="30" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor===false" label="柱体渐变开始颜色">
            <el-color-picker v-model="configData.chartOption.gradientsColorStart" show-alpha></el-color-picker>
          </el-form-item>  
          <el-form-item v-if="configData.chartOption.isThemeColor===false" label="柱体渐变结束颜色">
            <el-color-picker v-model="configData.chartOption.gradientsColorEnd" show-alpha></el-color-picker>
          </el-form-item>   
          <el-form-item v-if="configData.chartOption.backgroundColor!==undefined" label="柱体背景颜色">
            <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
          </el-form-item> 
        </el-collapse-item>
        <el-collapse-item title="y轴" name="5" v-if="configData.chartOption.yAxis!==undefined">
          <el-form-item v-if="configData.chartOption.yAxisShow!==undefined" label="是否显示纵坐标">
            <el-switch v-model="configData.chartOption.yAxisShow" />
          </el-form-item>
          <div v-if="configData.chartOption.yAxisShow&&configData.chartOption.yAxis!==undefined">
            <el-form-item v-if="configData.chartOption.yAxis.position!==undefined" label="坐标轴位置">
              <el-radio-group v-model="configData.chartOption.yAxis.position">
                <el-radio label="left">左</el-radio>
                <el-radio label="right">右</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisLabel!==undefined" label="显示坐标轴标签">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisLabel" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisLabel">
              <el-form-item v-if="configData.chartOption.yAxis.isSetLabelPosition!==undefined" label="是否设置坐标轴标签位置">
                <el-switch v-model="configData.chartOption.yAxis.isSetLabelPosition" />
              </el-form-item>
              <div v-if="configData.chartOption.yAxis.isSetLabelPosition">
                <el-form-item label="标签内边距上">
                  <el-slider v-model="configData.chartOption.yAxis.axisLabelPaddingTop" :min="-100" :max="100" :step="1" show-input></el-slider>
                </el-form-item>
                <el-form-item label="标签内边距右">
                  <el-slider v-model="configData.chartOption.yAxis.axisLabelPaddingRight" :min="-100" :max="100" :step="1" show-input></el-slider>
                </el-form-item>
                <el-form-item label="标签内边距下">
                  <el-slider v-model="configData.chartOption.yAxis.axisLabelPaddingBottom" :min="-100" :max="100" :step="1" show-input></el-slider>
                </el-form-item>
                <el-form-item label="标签内边距左">
                  <el-slider v-model="configData.chartOption.yAxis.axisLabelPaddingLeft" :min="-100" :max="100" :step="1" show-input></el-slider>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.yAxis.axisLabelAlign!==undefined" label="标签水平对齐">
                  <el-radio-group v-model="configData.chartOption.yAxis.axisLabelAlign">
                    <el-radio label="left">左</el-radio>
                    <el-radio label="right">右</el-radio>
                  </el-radio-group>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.yAxis.axisLabelVerticalAlign!==undefined" label="标签垂直对齐">
                  <el-radio-group v-model="configData.chartOption.yAxis.axisLabelVerticalAlign">
                    <el-radio label="bottom">下</el-radio>
                    <el-radio label="top">上</el-radio>
                  </el-radio-group>
                </el-form-item>
              </div>
              <el-form-item v-if="configData.chartOption.yAxis.margin!==undefined" label="坐标轴线距离">
                <el-slider v-model="configData.chartOption.yAxis.margin" :min="-100" :max="100" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelInside!==undefined" label="坐标轴标签是否显示内侧">
                <el-switch v-model="configData.chartOption.yAxis.axisLabelInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelColor!==undefined" label="坐标轴标签颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisLabelColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontSize!==undefined" label="坐标轴标签字体大小">
                <el-slider v-model="configData.chartOption.yAxis.axisLabelFontSize" :min="0" :max="100" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontWeight !== undefined" label="文字粗细">
                <el-select v-model="configData.chartOption.yAxis.axisLabelFontWeight" placeholder="请选择">
                  <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelFontStyle !== undefined" label="文字样式">
                <el-select v-model="configData.chartOption.yAxis.axisLabelFontStyle" placeholder="请选择">
                  <el-option v-for="(item, index) in fontStyles" :key="index" :label="item.label" :value="item.value">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLabelRotate!==undefined" label="标签旋转角度">
                <el-slider v-model="configData.chartOption.yAxis.axisLabelRotate" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisLine!==undefined" label="显示坐标轴轴线">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisLine" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisLine">
              <el-form-item v-if="configData.chartOption.yAxis.axisLineColor!==undefined" label="坐标轴轴线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLineType!==undefined" label="坐标轴样式">
                <el-select v-model="configData.chartOption.yAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisLineWidth!==undefined" label="坐标轴轴线宽度">
                <el-slider v-model="configData.chartOption.yAxis.axisLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowAxisTick!==undefined" label="显示刻度线">
              <el-switch v-model="configData.chartOption.yAxis.isShowAxisTick" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowAxisTick">
              <el-form-item v-if="configData.chartOption.yAxis.axisLineType!==undefined" label="刻度线样式">
                <el-select v-model="configData.chartOption.yAxis.axisLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickInside!==undefined" label="刻度线是否显示内侧">
                <el-switch v-model="configData.chartOption.yAxis.axisTickInside" />
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.axisTickColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.yAxis.axisTickWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.axisTickLength!==undefined" label="刻度线长度">
                <el-slider v-model="configData.chartOption.yAxis.axisTickLength" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.yAxis.isShowSplitLine!==undefined" label="显示分割线">
              <el-switch v-model="configData.chartOption.yAxis.isShowSplitLine" />
            </el-form-item>
            <div v-if="configData.chartOption.yAxis.isShowSplitLine">
              <el-form-item v-if="configData.chartOption.yAxis.splitLineType!==undefined" label="分割线样式">
                <el-select v-model="configData.chartOption.yAxis.splitLineType" placeholder="请选择">
                  <el-option  label="实线" value="solid"></el-option>
                  <el-option  label="虚线" value="dashed"></el-option>
                  <el-option  label="点线" value="dotted"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.splitLineColor!==undefined" label="刻度线颜色">
                <el-color-picker v-model="configData.chartOption.yAxis.splitLineColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.yAxis.splitLineWidth!==undefined" label="刻度线宽度">
                <el-slider v-model="configData.chartOption.yAxis.splitLineWidth" :min="0" :max="360" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
          </div>
        </el-collapse-item>
        <!-- <el-collapse-item title="标签" name="5">
          
          <el-form-item v-if="configData.chartOption.name!==undefined" label="提示名称">
            <el-input v-model="configData.chartOption.name" placeholder="请输入提示名称" />
          </el-form-item>
        </el-collapse-item> -->
        <el-collapse-item title="图例设置" name="6" v-if="configData.chartOption.legend!==undefined">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item label="隐藏图例文本" v-if="configData.chartOption.isHideLegendText!==undefined">
            <el-switch v-model="configData.chartOption.isHideLegendText" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legendText!==undefined" label="图例文本">
            <el-input v-model="configData.chartOption.legendText" placeholder="请输入图例文本" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例宽度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemWidth"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例高度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemHeight"/>
          </el-form-item>
          <!-- <el-form-item v-if="configData.chartOption.legend.show==true" label="图例列数">
            <el-input-number :min="1" v-model="configData.chartOption.legendCols"/>
          </el-form-item> -->
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平位置">
            <el-input-number  :min="-100" :max="100" v-model="configData.chartOption.legendPositionLeft"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直位置">
            <el-input-number  :min="-100" :max="100" v-model="configData.chartOption.legendPositionTop"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平间隔">
            <el-input-number  :min="-100" :max="100" v-model="configData.chartOption.legendItemGapX"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直间隔">
            <el-input-number  :min="-100" :max="100" v-model="configData.chartOption.legendItemGapY"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例形状">
            <el-select v-model="configData.chartOption.icon" placeholder="请选择">
              <el-option v-for="item in legendShape" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例字号">
            <el-slider v-model="configData.chartOption.legendFontSize"  :step="1" show-input/>
          </el-form-item>
          <el-form-item label="字体颜色" v-if="configData.chartOption.textStyle!==undefined">
            <el-color-picker v-model="configData.chartOption.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="图型颜色" v-if="configData.chartOption.legendItemColor!==undefined">
            <el-color-picker v-model="configData.chartOption.legendItemColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签" name="7" v-if="configData.chartOption.label!==undefined">
          <el-form-item v-if="configData.chartOption.label.isShow!==undefined" label="显示标签">
            <el-switch v-model="configData.chartOption.label.isShow" />
          </el-form-item>
          <div v-if="configData.chartOption.label.isShow">
            <el-form-item v-if="configData.chartOption.label.position!==undefined" label="位置">
              <el-radio-group v-model="configData.chartOption.label.position">
                <el-radio label="">默认</el-radio>
                <el-radio label="left">左</el-radio>
                <el-radio label="right">右</el-radio>
                <el-radio label="top">上</el-radio>
                <el-radio label="bottom">下</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="标签内边距上">
              <el-slider v-model="configData.chartOption.label.labelPaddingTop" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item label="标签内边距右">
              <el-slider v-model="configData.chartOption.label.labelPaddingRight" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item label="标签内边距下">
              <el-slider v-model="configData.chartOption.label.labelPaddingBottom" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item label="标签内边距左">
              <el-slider v-model="configData.chartOption.label.labelPaddingLeft" :min="-100" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.distance!==undefined" label="距离">
              <el-slider v-model="configData.chartOption.label.distance" :min="-200" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.fontSize!==undefined " label="字体大小">
              <el-slider v-model="configData.chartOption.fontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
            </el-form-item>
          </div>
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
      legendShape:[
        {label:'圆形',value:'circle'}
      ,{label:'矩形',value:'rect'}
      ,{label:'圆形矩形',value:'roundRect'}
      ,{label:'三角形',value:'triangle'}
      ,{label:'菱形',value:'diamond'}
      ,{label:'别针',value:'pin'}
      ,{label:'箭头',value:'arrow'}],
      activeNames: ["1"],
      animateOptions,
      chartList: this.drawingList,
      fontWeights:['normal', 'bold', 'bolder', 'lighter'],
      fontStyles:[{value:'normal',label:'默认'},{value:'italic',label:'斜体'},{value:'oblique',label:'倾斜字体'},],
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