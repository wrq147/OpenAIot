<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
          <el-form-item label="背景图">
            <image-upload v-model="configData.chartOption.bgImage" :limit="1"></image-upload>
          </el-form-item>
          <el-form-item label="背景颜色">
            <el-color-picker v-model="configData.chartOption.containerBgColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item  label="弧度">
            <el-slider v-model="configData.chartOption.containerRadius"  :step="1" show-input/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item label="标题类型">
            <el-radio-group v-model="configData.chartOption.titletextType">
              <el-radio label="0">自定义</el-radio>
              <el-radio label="1">总数</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item label="副标题类型">
            <el-radio-group v-model="configData.chartOption.subtextType">
              <el-radio label="0">自定义</el-radio>
              <el-radio label="1">总数</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="标题字体大小">
            <el-slider v-model="configData.chartOption.title.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.title.textStyle.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
            </el-select>
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
          <el-form-item label="标题和副标题距离">
            <el-slider v-model="configData.chartOption.title.itemGap" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题上边距%">
            <el-slider v-model="configData.chartOption.title.top" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题左边距%">
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
        <el-collapse-item title="第二个标题设置" name="22" v-if="configData.chartOption.otherTitle!==undefined">
          <el-form-item v-if="configData.chartOption.otherTitle.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.otherTitle.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item label="标题类型">
            <el-radio-group v-model="configData.chartOption.otherTitletextType">
              <el-radio label="0">自定义</el-radio>
              <el-radio label="1">总数</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.otherTitle.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.otherTitle.subtext" placeholder="请输入副标题" />
          </el-form-item>
          <el-form-item label="副标题类型">
            <el-radio-group v-model="configData.chartOption.otherTitleSubtextType">
              <el-radio label="0">自定义</el-radio>
              <el-radio label="1">总数</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="标题字体大小">
            <el-slider v-model="configData.chartOption.otherTitle.textStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.otherTitle.textStyle.fontWeight!==undefined" label="标题粗细">
            <el-select v-model="configData.chartOption.otherTitle.textStyle.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.otherTitle.textStyle.color!==undefined" label="标题字体颜色">
            <el-color-picker v-model="configData.chartOption.otherTitle.textStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.otherTitle.subtextStyle.fontSize!==undefined" label="副标题字体大小">
            <el-slider v-model="configData.chartOption.otherTitle.subtextStyle.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.otherTitle.subtextStyle.color!==undefined" label="副标题字体颜色">
            <el-color-picker v-model="configData.chartOption.otherTitle.subtextStyle.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="标题和副标题距离">
            <el-slider v-model="configData.chartOption.otherTitle.itemGap" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题上边距%">
            <el-slider v-model="configData.chartOption.otherTitle.top" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题左边距%">
            <el-slider v-model="configData.chartOption.otherTitle.left" :min="-100" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item label="标题对齐方式">
            <el-select v-model="configData.chartOption.otherTitle.textAlign" placeholder="请选择">
              <el-option  label="左对齐" value="left"></el-option>
              <el-option  label="中间对齐" value="center"></el-option>
              <el-option  label="右对齐" value="right"></el-option>
            </el-select>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图例设置" name="3">
          <el-form-item v-if="configData.chartOption.legend.show!==undefined" label="显示图例">
            <el-switch v-model="configData.chartOption.legend.show" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例宽度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemWidth"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例高度">
            <el-input-number :min="0" v-model="configData.chartOption.legend.itemHeight"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例列数">
            <el-input-number :min="1" v-model="configData.chartOption.legendCols"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平位置">
            <el-input-number :min="0" v-model="configData.chartOption.legendPositionLeft"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直位置">
            <el-input-number :min="0" v-model="configData.chartOption.legendPositionTop"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例水平间隔">
            <el-input-number :min="0" v-model="configData.chartOption.legendItemGapX"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例垂直间隔">
            <el-input-number :min="0" v-model="configData.chartOption.legendItemGapY"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例形状">
            <el-select v-model="configData.chartOption.icon" placeholder="请选择">
              <el-option v-for="item in legendShape" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true" label="图例字号">
            <el-slider v-model="configData.chartOption.legendFontSize"  :step="1" show-input/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legend.show==true">
            <el-checkbox v-model="configData.chartOption.legend.isShowVal">图例是否显示值</el-checkbox>
          </el-form-item>
          
          <el-form-item label="字体颜色">
            <el-color-picker v-model="configData.chartOption.legendFontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item label="图型颜色" v-if="configData.chartOption.bgColor!==undefined">
            <div v-for="(item, index) in originData" :key="index" class="select-item">
               <div style="display: flex;align-items: center;">
                  <span style="margin-right: 10px;">{{ item.name }}</span>
                  <el-color-picker v-model="configData.chartOption.bgColor[index]" show-alpha></el-color-picker>
               </div>
            </div>
          </el-form-item>
          <el-form-item label="显示百分比">
            <el-switch v-model="configData.chartOption.showPercentage"/>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.showPercentage==true" label="文字间隔">
            <el-slider v-model="configData.chartOption.textWidth"  :step="1" :max="500" show-input/>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="图形设置" name="4">
          <el-form-item  label="圆心横坐标">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.centerX"></el-input-number>
          </el-form-item>
          <el-form-item  label="圆心纵坐标">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.centerY"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isRoseType!==undefined" label="南丁格尔玫瑰">
            <el-switch v-model="configData.chartOption.isRoseType" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isFan!==undefined" label="扇形">
            <el-switch v-model="configData.chartOption.isFan" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isFan==true" label="起始角度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.startAngle"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isFan==true" label="终止角度">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.endAngle"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isRing!==undefined" label="环饼">
            <el-switch v-model="configData.chartOption.isRing" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isRing==true" label="内半径">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.innerRadius"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isRing==true" label="外半径">
            <el-input-number size="mini" controls-position="right" v-model="configData.chartOption.outerRadius"></el-input-number>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isRing==true" label="内圈阴影">
            <el-switch v-model="configData.chartOption.innerShadow" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerShadow==true" label="阴影宽度">
            <el-input-number :min="0" :max="1" :step="0.01" :precision="2" v-model="configData.chartOption.innerRadiusScale" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerShadow==true" label="阴影透明度">
            <el-input-number :min="0" :max="1" :step="0.01" :precision="2" v-model="configData.chartOption.innerRadiusOpacity" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="引导线设置" name="5">
          <el-form-item v-if="configData.chartOption.series[0].label.show!==undefined" label="引导线">
            <el-switch v-model="configData.chartOption.series[0].label.show" />
          </el-form-item>
          <el-form-item  label="引导线宽度">
            <el-input-number :min="0" v-model="configData.chartOption.labelLineWidth"/>
          </el-form-item>
          <el-form-item  label="第一段引导线长度">
            <el-input-number :min="0" v-model="configData.chartOption.labelLineLength"/>
          </el-form-item>
          <el-form-item  label="第二段引导线长度">
            <el-input-number :min="0" v-model="configData.chartOption.labelLineLength2"/>
          </el-form-item>
          <el-form-item  label="标签选项">
            <el-checkbox-group v-model="activeCheckBox" @change="handleCheckedDatesChange">
              <el-checkbox v-for="item in checkBoxList" :label="item.key" :key="item.key">{{item.label}}</el-checkbox>
            </el-checkbox-group>
          </el-form-item>
          <el-form-item v-if="activeCheckBox.includes('数值')" label="数值后缀">
            <el-input v-model="configData.chartOption.formatterSuffix" placeholder="请输入后缀" />
          </el-form-item>
          <el-form-item  label="标签字号">
            <el-slider v-model="configData.chartOption.labelFontSize"  :step="1" show-input/>
          </el-form-item>
          <el-form-item label="标签颜色">
            <el-color-picker v-model="configData.chartOption.labelFontColor" show-alpha></el-color-picker>
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
    },
    themeForm: {
      type: Object
    },
  },
  watch: {
    costomData: {
      immediate: true,
      deep: true,
      handler() {
        this.activeCheckBox=JSON.parse(JSON.stringify(this.configData.chartOption.labelFormatter))
        this.initResult()
      },
    },
  },
  data() {
    return {
      fontWeights:['normal','bold','bolder','lighter'],
      legendShape:[
        {label:'圆形',value:'circle'}
      ,{label:'矩形',value:'rect'}
      ,{label:'圆形矩形',value:'roundRect'}
      ,{label:'三角形',value:'triangle'}
      ,{label:'菱形',value:'diamond'}
      ,{label:'别针',value:'pin'}
      ,{label:'箭头',value:'arrow'}],
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      checkBoxList:[{label:'类别名称',key:"类别名称"},{label:'数值',key:'数值'},{label:'百分比',key:'百分比'}],
      activeCheckBox:['类别名称'],
      chartList: this.drawingList,
      originData: []
    }
  },
  methods: {
    // 表格列值配置
    initResult() {
      let data = this.themeForm.globalData.filter(x => x.name == this.configData.chartOption.globalData);
      if( this.configData.chartOption.dataSourceType !== 'static' ) {
        if (data.length > 0 && data[0].rawData !== undefined) {
          let resultData = JSON.parse(data[0].rawData);
          resultData.forEach((item, index) => {
            if (item.title === this.configData.chartOption.globalProcessor) {
              this.originData = item.content
              return
            }
          })
        }
      } else {
        this.originData = this.configData.chartOption.staticDataValue
      }
    },
    handleCheckedDatesChange(val){
      this.$set(this.configData.chartOption, 'labelFormatter', val);
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