<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeName" accordion>
          <el-collapse-item title="图层" name="0">
              <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
                  <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
              </el-form-item>   
          </el-collapse-item>
          <el-collapse-item title="标题" name="1">

              <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
                <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
              </el-form-item>
              <el-form-item label="标题字号">
                <el-input-number controls-position="right" :min="0" v-model="configData.chartOption.title.textStyle.fontSize"></el-input-number>
              </el-form-item>
              <el-form-item label="标题颜色">
                <el-color-picker v-model="configData.chartOption.title.textStyle.color" show-alpha></el-color-picker>
              </el-form-item>

              <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
                <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
              </el-form-item>
              <el-form-item label="副标题字号">
                <el-input-number controls-position="right" :min="0" v-model="configData.chartOption.title.subtextStyle.fontSize"></el-input-number>
              </el-form-item>
              <el-form-item label="副标题颜色">
                <el-color-picker v-model="configData.chartOption.title.subtextStyle.color" show-alpha></el-color-picker>
              </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="x轴设置" name="2">
              <el-form-item label="标签字号">
                  <el-slider v-model="configData.chartOption.xAxis.axisLabel.textStyle.fontSize" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item label="标签颜色">
                  <el-color-picker v-model="configData.chartOption.xAxis.axisLabel.textStyle.color" show-alpha></el-color-picker>
              </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="y轴设置" name="3">
              <el-form-item label="标签字号">
                  <el-slider v-model="configData.chartOption.yAxis.axisLabel.textStyle.fontSize" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item label="标签颜色">
                  <el-color-picker v-model="configData.chartOption.yAxis.axisLabel.textStyle.color" show-alpha></el-color-picker>
              </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="图例设置" name="4">
              <el-form-item label="最小值">
                  <el-input-number v-model="configData.chartOption.visualMap.min" :max="configData.chartOption.visualMap.max"></el-input-number>
              </el-form-item>
              <el-form-item label="最大值">
                  <el-input-number v-model="configData.chartOption.visualMap.max" :min="configData.chartOption.visualMap.min"></el-input-number>
              </el-form-item>

              <el-form-item label="标签字号">
                  <el-slider v-model="configData.chartOption.visualMap.textStyle.fontSize" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item label="标签颜色">
                  <el-color-picker v-model="configData.chartOption.visualMap.textStyle.color" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item label="柱体颜色">
                  <div style="margin-bottom:20px">
                      <div style="display:flex">
                          <el-form-item label="颜色(始)"></el-form-item>
                          <el-form-item label="颜色(终)" style="margin-left: 60px;"></el-form-item>
                      </div>
                      <div class="select-item">
                          <el-color-picker v-model="configData.chartOption.visualMap.inRange.color[0]" show-alpha style="margin-left:30px"></el-color-picker>
                          <div :style="gradientColor(configData.chartOption.visualMap.inRange.color)"></div>
                          <el-color-picker v-model="configData.chartOption.visualMap.inRange.color[1]" show-alpha style="margin-left:20px"></el-color-picker>
                      </div>
                  </div>
              </el-form-item>
                  
          </el-collapse-item>

          <el-collapse-item title="标签设置" name="5">
              <el-form-item label="显示标签">
                  <el-switch v-model="configData.chartOption.series[0].label.show" />
              </el-form-item>
              <el-form-item label="标签字号">
                  <el-slider v-model="configData.chartOption.series[0].label.fontSize" :step="1" show-input></el-slider>
              </el-form-item>

              <el-form-item label="标签颜色">
                  <el-color-picker v-model="configData.chartOption.series[0].label.color" show-alpha></el-color-picker>
              </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="动画设置" name="6">
          <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option
                  v-for="item in animateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value">
              </el-option>
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
      activeName:['0'],
      animateOptions,
    }
  },
  methods: {
    gradientColor(item){
        let style = {width:'100px',height:'30px',marginLeft:'20px',background:`-webkit-linear-gradient(left, ${item[0]},${item[1]})`}
        return style
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