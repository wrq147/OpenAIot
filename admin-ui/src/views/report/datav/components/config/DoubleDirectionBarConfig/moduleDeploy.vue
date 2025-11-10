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
          <el-form-item v-if="configData.chartOption.top!==undefined " label="与上侧的距离(%)">
            <el-slider v-model="configData.chartOption.top" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.right!==undefined " label="与右侧的距离(%)">
            <el-slider v-model="configData.chartOption.right" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.legendFontSize!==undefined " label="字体大小">
            <el-slider v-model="configData.chartOption.legendFontSize" :min="1" :step="1" show-input></el-slider>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="4">
          <el-form-item v-if="configData.chartOption.barWidth!==undefined " label="柱体宽度">
            <el-slider v-model="configData.chartOption.barWidth" :min="1" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.leftColorStart!==undefined" label="左侧柱体颜色(始)">
            <el-color-picker v-model="configData.chartOption.leftColorStart" show-alpha></el-color-picker>
          </el-form-item>  
          <el-form-item v-if="configData.chartOption.leftColorStart!==undefined" label="左侧柱体颜色(终)">
            <el-color-picker v-model="configData.chartOption.leftColorEnd" show-alpha></el-color-picker>
          </el-form-item>  
          <el-form-item v-if="configData.chartOption.rightColorStart!==undefined" label="右侧柱体颜色(始)">
            <el-color-picker v-model="configData.chartOption.rightColorStart" show-alpha></el-color-picker>
          </el-form-item>  
          <el-form-item v-if="configData.chartOption.rightColorStart!==undefined" label="右侧柱体颜色(终)">
            <el-color-picker v-model="configData.chartOption.rightColorEnd" show-alpha></el-color-picker>
          </el-form-item>  
          <el-form-item v-if="configData.chartOption.position!==undefined" label="横坐标位置">
            <el-radio-group v-model="configData.chartOption.position">
              <el-radio label="top">上</el-radio>
              <el-radio label="bottom">下</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.showLabel!==undefined" label="是否显示分隔线">
            <el-switch v-model="configData.chartOption.showLabel" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标签" name="5">
          <el-form-item v-if="configData.chartOption.labelTop!==undefined " label="与上侧的距离(%)">
            <el-slider v-model="configData.chartOption.labelTop" :min="0" :max="100" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontSize!==undefined " label="字体大小">
            <el-slider v-model="configData.chartOption.fontSize" :min="1" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.letterSpacing!==undefined " label="字体间距">
            <el-slider v-model="configData.chartOption.letterSpacing" :min="0" :step="1" show-input></el-slider>
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
      chartList: this.drawingList
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