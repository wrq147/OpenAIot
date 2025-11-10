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
          <el-form-item v-if="configData.chartOption.total!==undefined" label="柱体总量">
            <el-input-number v-model="configData.chartOption.total" controls-position="right" :step="1"></el-input-number>
          </el-form-item>
        </el-collapse-item>
        <el-collapse-item title="颜色" name="4">
          <el-form-item v-if="configData.chartOption.isThemeColor!==undefined" label="是否应用主题颜色">
            <el-switch v-model="configData.chartOption.isThemeColor" />
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor===false" label="0%处颜色">
            <el-color-picker v-model="configData.chartOption.linearColor0" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.isThemeColor===false" label="100%处颜色">
            <el-color-picker v-model="configData.chartOption.linearColor1" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.borderColor!==undefined" label="边框颜色">
            <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.symbolColor!==undefined" label="分隔线颜色">
            <el-color-picker v-model="configData.chartOption.symbolColor" show-alpha></el-color-picker>
          </el-form-item>
        </el-collapse-item>

        <el-collapse-item title="文字" name="5">
          <el-form-item v-if="configData.chartOption.fontSize!==undefined" label="字体大小">
            <el-slider v-model="configData.chartOption.fontSize" :min="1" :max="200" :step="1" show-input></el-slider>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontFamily!==undefined" label="字体名称">
            <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
              <el-option
                v-for="(item,index) in fontFamilys"
                :key="index"
                :label="item"
                :value="item">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="configData.chartOption.fontColor!==undefined" label="字体颜色">
            <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
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
      chartList: this.drawingList,
      fontWeights:['normal','bold','bolder','lighter'],
      fontFamilys: []
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