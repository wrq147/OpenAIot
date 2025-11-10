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
          <el-form-item v-if="configData.chartOption.xAxis.name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.xAxis.name" placeholder="请输入X轴名称" />
          </el-form-item>
          <el-form-item label="坐标轴名称位置">
            <el-select v-model="configData.chartOption.xAxis.nameLocation" placeholder="请选择">
              <el-option v-for="item in nameLocation" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴名称字号">
            <el-slider v-model="configData.chartOption.xAxis.nameTextStyle.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show!==undefined" label="显示X轴">
            <el-switch v-model="configData.chartOption.xAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show==true" label="显示轴线">
            <el-switch v-model="configData.chartOption.xAxis.axisLine.show"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLine.show==true && configData.chartOption.xAxis.show==true" label="轴线粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.xAxis.axisLine.lineStyle.width"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLine.show==true && configData.chartOption.xAxis.show==true" label="轴线颜色">
            <el-color-picker v-model="configData.chartOption.xAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.show==true" label="显示标签">
            <el-switch v-model="configData.chartOption.xAxis.axisLabel.show"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show" label="标签字体">
            <el-select v-model="configData.chartOption.xAxis.axisLabel.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show && configData.chartOption.xAxis.show==true" label="标签字号">
            <el-slider v-model="configData.chartOption.xAxis.axisLabel.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show && configData.chartOption.xAxis.show==true" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.xAxis.axisLabel.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="y轴设置" name="4">
          <el-form-item v-if="configData.chartOption.yAxis.name!==undefined" label="坐标轴名称">
            <el-input v-model="configData.chartOption.yAxis.name" placeholder="请输入y轴名称" />
          </el-form-item>
          <el-form-item label="坐标轴名称位置">
            <el-select v-model="configData.chartOption.yAxis.nameLocation" placeholder="请选择">
              <el-option v-for="item in nameLocation" :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
          <el-form-item label="坐标轴字号">
            <el-slider v-model="configData.chartOption.yAxis.nameTextStyle.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show!==undefined" label="显示y轴">
            <el-switch v-model="configData.chartOption.yAxis.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show==true" label="显示轴线">
            <el-switch v-model="configData.chartOption.yAxis.axisLine.show"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.show==true && configData.chartOption.yAxis.show==true" label="轴线粗细">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.yAxis.axisLine.lineStyle.width"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLine.show==true && configData.chartOption.yAxis.show==true" label="轴线颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.axisLine.lineStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.show==true" label="显示标签">
            <el-switch v-model="configData.chartOption.yAxis.axisLabel.show"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.xAxis.axisLabel.show" label="标签字体">
            <el-select v-model="configData.chartOption.xAxis.axisLabel.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLabel.show && configData.chartOption.yAxis.show==true" label="标签字号">
            <el-slider v-model="configData.chartOption.yAxis.axisLabel.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.yAxis.axisLabel.show && configData.chartOption.yAxis.show==true" label="标签颜色">
            <el-color-picker v-model="configData.chartOption.yAxis.axisLabel.color" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例设置" name="5">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show == true" label="标记宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.legend.itemWidth"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show == true" label="标记高度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.legend.itemHeight"/>
          </el-form-item> 
          <el-form-item v-if="configData.chartOption.legend.show == true" label="字号">
            <el-slider v-model="configData.chartOption.legend.textStyle.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show == true" label="文字颜色">
            <el-color-picker v-model="configData.chartOption.legend.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show == true" label="布局朝向">
            <el-select v-model="configData.chartOption.legend.orient" placeholder="请选择">
              <el-option v-for="item in legendOrient"  :key="item.key" :label="item.name" :value="item.key" />
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图形设置" name="6">
          <el-form-item label="最大值">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.max"/>
          </el-form-item>
          <el-form-item label="图形朝向">
            <el-radio-group v-model="configData.chartOption.barOrient" @change="handleBarOrient">
              <el-radio-button label="纵向"></el-radio-button>
              <el-radio-button label="横向"></el-radio-button>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.showAll && configData.chartOption.barOrient=='纵向'" label="阴影水平位置">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.symbolOffset[0]" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.showAll && configData.chartOption.barOrient=='横向'" label="阴影垂直位置">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.symbolOffset[1]" />
          </el-form-item>
          <el-form-item label="自定义图案">
            <image-upload :value="configData.chartOption.imageList" :limit="10"></image-upload>
          </el-form-item>
          <el-form-item label="图形宽度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.series[0].symbolSize[0]" />
          </el-form-item>
          <el-form-item label="图形高度">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.series[0].symbolSize[1]" />
          </el-form-item>
          <el-form-item label="图标间距">
            <el-slider v-model="configData.chartOption.symbolMargin" :step="1" show-input></el-slider>
          </el-form-item>              
        </el-collapse-item>

        <el-collapse-item title="占比设置" name="7">
          <el-form-item  label="显示占比">
            <el-switch v-model="configData.chartOption.showAll"/>
          </el-form-item>              
          <el-form-item label="显示标签">
            <el-switch v-model="configData.chartOption.series[0].label.show"/>
          </el-form-item>
          <el-form-item label="字体">
            <el-select v-model="configData.chartOption.series[0].label.fontFamily" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="字号">
            <el-slider v-model="configData.chartOption.series[0].label.fontSize" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="文字颜色">
            <el-color-picker v-model="configData.chartOption.series[0].label.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="位置">
            <el-select v-model="configData.chartOption.series[0].label.position" placeholder="请选择">
              <el-option v-for="(item,index) in labelPosition" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item label="标签水平位置">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.labelOffset0" />
          </el-form-item>
          <el-form-item label="标签垂直位置">
            <el-input-number class="inputFontSize" v-model="configData.chartOption.labelOffset1" />
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
      chartList: this.drawingList,
      fontFamilys: [],
      labelPosition:['top','left','right','bottom','inside','insideLeft','insideRight','insideTop','insideBottom','insideTopLeft','insideBottomLeft','insideTopRight','insideBottomRight'],
      nameLocation:[{key:'start',name:"起点"},{key:'center',name:"中间"},{key:'end',name:"末尾"}],
      legendOrient:[{key:"horizontal",name:"水平"},{key:"vertical",name:"垂直"}],
      splitLineType:[{key:'solid', name:"实线"},{key:'dashed',name:"短横虚线"},{key:'dotted',name:'点状虚线'}],
    }
  },
  methods: {
    handleBarOrient(val){
      if(val === "纵向"){
        this.$set(this.configData.chartOption.xAxis,'type',"category")
        this.$set(this.configData.chartOption.yAxis,'type',"value")
        this.$set(this.configData.chartOption.yAxis,'inverse',false)
        this.$set(this.configData.chartOption.series[0].label,'position','top')
      }
      if(val === "横向"){
        this.$set(this.configData.chartOption.xAxis,'type',"value")
        this.$set(this.configData.chartOption.yAxis,'type',"category")
        this.$set(this.configData.chartOption.yAxis,'inverse',true)
        this.$set(this.configData.chartOption.series[0].label,'position','right')
      }
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