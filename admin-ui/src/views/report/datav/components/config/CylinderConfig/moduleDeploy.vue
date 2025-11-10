<template>
    <el-form size="small" label-width="90px">
      <el-collapse v-model="activeNames" accordion>
        <el-collapse-item title="图层" name="1">
          <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
            <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="标题" name="2">
          <el-form-item v-if="configData.chartOption.istitle!==undefined" label="是否显示标题">
            <el-switch v-model="configData.chartOption.istitle" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.text!==undefined" label="标题">
            <el-input v-model="configData.chartOption.title.text" placeholder="请输入标题" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.title.subtext!==undefined" label="副标题">
            <el-input v-model="configData.chartOption.title.subtext" placeholder="请输入副标题" />
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="柱体" name="3">
          <el-form-item v-if="configData.chartOption.width!==undefined" label="柱体宽度">
            <el-slider v-model="configData.chartOption.width" :min="1" :max="1000" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color1!==undefined" label="上表面颜色">
            <el-color-picker v-model="configData.chartOption.color1" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color2!==undefined" label="中间隔层颜色">
            <el-color-picker v-model="configData.chartOption.color2" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color3!==undefined" label="下表面颜色">
            <el-color-picker v-model="configData.chartOption.color3" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color4!==undefined" label="圆柱下半部分颜色">
            <el-color-picker v-model="configData.chartOption.color4" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color6!==undefined" label="圆柱上半部分颜色">
            <el-color-picker v-model="configData.chartOption.color6" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="文字" name="4">
          <el-form-item v-if="configData.chartOption.size!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.size" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.family!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.family" placeholder="请选择">
              <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.weight!==undefined" label="文字粗细">
            <el-select v-model="configData.chartOption.weight" placeholder="请选择">
              <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item" />
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.color5!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.color5" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.suffix!==undefined" label="文字后缀">
            <el-input v-model="configData.chartOption.suffix" placeholder="请输入文字后缀" />
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
      fontFamilys: [],
      activeNames: ["1"],
      animateOptions,
      fontFamilys:[],
      fontWeights:['normal','bold','bolder','lighter'],
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