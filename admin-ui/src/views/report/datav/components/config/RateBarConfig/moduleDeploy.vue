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
        <el-collapse-item title="x轴" name="3">  
          <el-form-item  label="x轴线">
            <el-switch v-model="configData.chartOption.xAxis.axisLine.show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item  label="x轴颜色">
            <el-color-picker v-model="configData.chartOption.xAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="x轴粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.xAxis.axisLine.lineStyle.width" controls-position="right" />
          </el-form-item>
          <el-form-item label="标签字号">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.xAxis.axisLabel.fontSize" controls-position="right" />
          </el-form-item>
          <el-form-item  label="标签颜色">
            <el-color-picker v-model="configData.chartOption.xAxis.axisLabel.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.fontFamily!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.xAxis.axisLabel.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.fontWeight!==undefined" label="文字粗细">
            <el-select v-model="configData.chartOption.xAxis.axisLabel.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.margin!==undefined" label="标签间距">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.xAxis.axisLabel.margin" controls-position="right" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴" name="4">
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.show!==undefined" label="y轴线">
            <el-switch v-model="configData.chartOption.yAxis.axisLine.show" active-text="显示" inactive-text="不显示"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.lineStyle.color!==undefined" label="y轴颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.lineStyle.width!==undefined" label="y轴粗细">
            <el-slider v-model="configData.chartOption.yAxis.axisLine.lineStyle.width"  :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLabel.fontSize!==undefined"  label="标签字号">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.yAxis.axisLabel.fontSize" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLabel.textStyle.color!==undefined"  label="标签颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.axisLabel.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.margin!==undefined" label="标签间距">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.yAxis.axisLabel.margin" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.show!==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.yAxis.splitLine.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.lineStyle.type!==undefined" label="样式">
            <el-select v-model="configData.chartOption.yAxis.splitLine.lineStyle.type" placeholder="请选择">
              <el-option label="实线" value="solid"></el-option>
              <el-option label="虚线" value="dashed"></el-option>
              <el-option label="点线" value="dotted"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.lineStyle.width!==undefined" label="粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.yAxis.splitLine.lineStyle.width" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.splitLine.lineStyle.color!==undefined" label="颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.splitLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>

        <el-collapse-item title="柱体" name="5">
          <el-form-item v-if="configData.chartOption.max!==undefined" label="最大值">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.max"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.totalBar.barWidth!==undefined" label="柱体宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.totalBar.barWidth" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.totalBar.backgroundColor!==undefined" label="背景颜色">
            <el-color-picker v-model="configData.chartOption.totalBar.backgroundColor" show-alpha />
          </el-form-item>            
          <el-form-item v-if="configData.chartOption.splitBar.width!==undefined" label="间隔宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.splitBar.width" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.splitBar.height!==undefined" label="间隔高度">
           <el-input-number class="inputFontSize" v-model="configData.chartOption.splitBar.height" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.splitBar.color!==undefined" label="间隔颜色">
            <el-color-picker v-model="configData.chartOption.splitBar.color" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.splitBar.symbolMargin!==undefined" label="间隔距离">
           <el-input-number class="inputFontSize" v-model="configData.chartOption.splitBar.symbolMargin" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.totalBar.color!==undefined"  class="barColor" label="总量颜色">
            <div style="margin-bottom:20px">
              <div style="display:flex">
                <el-form-item label="颜色(始)"></el-form-item>
                <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
              </div>
              <div class="select-item">
                <el-color-picker v-model="configData.chartOption.totalBar.color[0]" show-alpha style="margin-left:30px"></el-color-picker>
                <div :style="gradientColor(configData.chartOption.totalBar.color)"></div>
                <el-color-picker v-model="configData.chartOption.totalBar.color[1]" show-alpha style="margin-left:20px"></el-color-picker>
              </div>
            </div>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.totalBar.completedColor!==undefined" class="barColor" label="完成颜色">
            <div style="margin-bottom:20px">
              <div style="display:flex">
                <el-form-item label="颜色(始)"></el-form-item>
                <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
              </div>
              <div class="select-item">
                <el-color-picker v-model="configData.chartOption.totalBar.completedColor[0]" show-alpha style="margin-left:30px"></el-color-picker>
                <div :style="gradientColor(configData.chartOption.totalBar.completedColor)"></div>
                <el-color-picker v-model="configData.chartOption.totalBar.completedColor[1]" show-alpha style="margin-left:20px"></el-color-picker>
              </div>
            </div>
          </el-form-item>
        </el-collapse-item>

        <el-collapse-item title="标签" name="6">
          <el-form-item v-if="configData.chartOption.label.show!==undefined" label="显示">
            <el-switch v-model="configData.chartOption.label.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.distance!==undefined"  label="标签距离">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.distance" controls-position="right" />
          </el-form-item>            
          <el-form-item v-if="configData.chartOption.label.fontSize!==undefined"  label="标签字号">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.fontSize" controls-position="right" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.color!==undefined"  label="标签颜色">
            <el-color-picker v-model="configData.chartOption.label.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.fontFamily!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.label.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>

          <el-form-item v-if="configData.chartOption.label.fontWeight!==undefined" label="文字粗细">
            <el-select v-model="configData.chartOption.label.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="标签内间距">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.padding[0]"/>
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.padding[1]"/>
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.padding[2]"/>
            <el-input-number class="inputFontSize" v-model="configData.chartOption.label.padding[3]"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.backgroundShow!==undefined" label="背景图片">
            <el-switch v-model="configData.chartOption.label.backgroundShow" @change="backgroundChange"/>
          </el-form-item>           
          <el-form-item v-if="configData.chartOption.label.backgroundShow" label="背景图">
            <image-gallary @getImg="getImg"></image-gallary>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.backgroundShow"  label="图片宽度">
            <el-slider v-model="configData.chartOption.label.imageWidth" :min="1" :step="1" :max="500" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.backgroundShow"  label="图片高度">
            <el-slider v-model="configData.chartOption.label.imageHeight" :min="1" :step="1" :max="500" show-input></el-slider>
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
      fontWeights:['normal','bold','bolder','lighter'],
      chartList: this.drawingList,
      fontFamilys: [],
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
    }
  },
  methods: {
    gradientColor(item){
      let style = {width:'100px',height:'30px',marginLeft:'20px',background:`-webkit-linear-gradient(left, ${item[0]},${item[1]})`}
      return style
    },
    getImg(val){
      this.$set(this.configData.chartOption.label, 'backgroundImg', val);
    },
    backgroundChange(status){
      if(!status){
        this.$set(this.configData.chartOption.label, 'backgroundImg', "");
      }
    }
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