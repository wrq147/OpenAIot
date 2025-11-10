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
          <el-form-item v-if="configData.chartOption.title.fontFamily!==undefined" label="字体">
            <el-select v-model="configData.chartOption.title.fontFamily" placeholder="请选择">
              <el-option v-for="(item, index) in detailFamily" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.fontSize!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.title.fontSize" :min="0" :step="1" :max="200" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.fontWeight!==undefined" label="字体粗细">
            <el-select v-model="configData.chartOption.title.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in detailWeight" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.color!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.title.color" show-alpha />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.top!==undefined" label="标题上边距">
            <el-slider v-model="configData.chartOption.title.top" :min="-200" :max="200" :step="1" show-input />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.left!==undefined" label="标题左边距">
            <el-slider v-model="configData.chartOption.title.left" :min="-200" :max="200" :step="1" show-input />
          </el-form-item>  
        </el-collapse-item>
        <el-collapse-item title="表盘" name="3">
          <el-form-item v-if="configData.chartOption.progressWidth!==undefined" label="表盘宽度">
            <el-slider v-model="configData.chartOption.progressWidth" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.processColor!==undefined" label="表盘颜色">
            <div style="margin-bottom:20px">
              <div style="display:flex">
                <el-form-item label="颜色(始)"></el-form-item>
                <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
              </div>
              <div class="select-item">
                <el-color-picker v-model="configData.chartOption.processColor[0]" show-alpha style="margin-left:30px"></el-color-picker>
                <div :style="gradientColor(configData.chartOption.processColor)"></div>
                <el-color-picker v-model="configData.chartOption.processColor[1]" show-alpha style="margin-left:20px"></el-color-picker>
              </div>
            </div> 
          </el-form-item>
          <el-form-item v-if="configData.chartOption.surplusColor!==undefined" label="剩余颜色">
            <el-color-picker v-model="configData.chartOption.surplusColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="刻度" name="4">
          <el-form-item v-if="configData.chartOption.splitNumber!==undefined" label="刻度数量">
            <el-slider v-model="configData.chartOption.splitNumber" :min="0" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.splitLineWidth!==undefined" label="刻度宽度">
            <el-slider v-model="configData.chartOption.splitLineWidth" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item> 
          <el-form-item v-if="configData.chartOption.splitLineColor!==undefined" label="刻度颜色">
            <el-color-picker v-model="configData.chartOption.splitLineColor" show-alpha></el-color-picker>
          </el-form-item>         
        </el-collapse-item>
        <el-collapse-item title="数值" name="5">
          <el-form-item v-if="configData.chartOption.isValue!==undefined" label="显示数值">
            <el-switch v-model="configData.chartOption.isValue" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.fontFamily!==undefined" label="字体">
            <el-select v-model="configData.chartOption.label.fontFamily" placeholder="请选择">
              <el-option v-for="(item, index) in detailFamily" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.fontSize!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.label.fontSize" :min="0" :step="1" :max="200" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.fontWeight!==undefined" label="字体粗细">
            <el-select v-model="configData.chartOption.label.fontWeight" placeholder="请选择">
              <el-option v-for="(item,index) in detailWeight" :key="index" :label="item" :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.label.color!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.label.color" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.offsetCenter!==undefined" label="数值左边距">
            <el-slider v-model="configData.chartOption.offsetCenter[0]" :min="-200" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.offsetCenter!==undefined" label="数值上边距">
            <el-slider v-model="configData.chartOption.offsetCenter[1]" :min="-200" :max="200" :step="1" show-input></el-slider>
          </el-form-item>    
        </el-collapse-item>
        <el-collapse-item title="圆环" name="6">
          <el-form-item v-if="configData.chartOption.outRadius!==undefined" label="外环内半径">
            <el-slider v-model="configData.chartOption.outRadius[0]" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.outRadius!==undefined" label="外环外半径">
            <el-slider v-model="configData.chartOption.outRadius[1]" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.outColor!==undefined" label="外环颜色">
            <el-color-picker v-model="configData.chartOption.outColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerRadius!==undefined" label="内环内半径">
            <el-slider v-model="configData.chartOption.innerRadius[0]" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerRadius!==undefined" label="内环外半径">
            <el-slider v-model="configData.chartOption.innerRadius[1]" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.innerColor!==undefined" label="内环颜色">
            <el-color-picker v-model="configData.chartOption.innerColor" show-alpha></el-color-picker>
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
      detailFamily: ['宋体', '黑体', '微软雅黑', 'Digital', 'Chunkfive', 'unidreamLED', 'Arial', 'Helvetica', 'sans-serif'],
      detailWeight: ['normal', 'bold', 'bolder', 'lighter'],
    }
  },
  methods: {
    gradientColor(item){
      let style = {width:'100px',height:'30px',marginLeft:'20px',background:`-webkit-linear-gradient(left, ${item[0]},${item[1]})`}
      return style
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