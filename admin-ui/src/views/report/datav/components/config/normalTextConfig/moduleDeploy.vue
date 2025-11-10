<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="76px" label-position="top" class="custom_form_item">
        <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
          <el-input v-model="configData.layerName" placeholder="请输入图层名称"/>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.fontSize !== undefined" label="字体大小">
          <el-slider v-model="configData.chartOption.fontSize" :min="10" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.fontFamily !== undefined" label="字体名称">
          <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
            <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.fontColor !== undefined" label="字体颜色">
          <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.letterSpacing !== undefined" label="字体间距">
          <el-slider v-model="configData.chartOption.letterSpacing" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.topPadding !== undefined" label="上下内边距">
          <el-slider v-model="configData.chartOption.topPadding" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.leftPadding !== undefined" label="左右内边距">
          <el-slider v-model="configData.chartOption.leftPadding" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.lineHeight !== undefined" label="字体行高">
          <el-slider v-model="configData.chartOption.lineHeight" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.conRadius !== undefined" label="文本弧度">
          <el-slider v-model="configData.chartOption.conRadius" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-show="configData.chartOption.border !== undefined" label="边框" label-width="100px">
          <el-radio-group v-model="configData.chartOption.border">
            <el-radio :label="0">无</el-radio>
            <el-radio :label="1">有</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.border==1&&configData.chartOption.borderColor !== undefined" label="边框颜色">
          <el-color-picker v-model="configData.chartOption.borderColor" show-alpha></el-color-picker>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.borderWidth !== undefined" label="边框大小">
          <el-slider v-model="configData.chartOption.borderWidth" :min="0" :max="200" :step="1" show-input></el-slider>
        </el-form-item>
        <el-form-item v-show="configData.chartOption.backgroundType !== undefined" label="字体背景" label-width="100px">
          <el-radio-group v-model="configData.chartOption.backgroundType">
            <el-radio label="color">背景色</el-radio>
            <el-radio label="img">背景图</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.backgroundType!='img'&&configData.chartOption.backgroundColor !== undefined" label="背景颜色">
          <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
        </el-form-item>
        <el-form-item label="背景图" label-width="100px" v-if="configData.chartOption.backgroundType=='img'&&configData.chartOption.backgroundImage !== undefined">
          <image-upload v-model="configData.chartOption.backgroundImage" :limit="1"></image-upload>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.fontWeight !== undefined" label="文字粗细">
          <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
            <el-option v-for="(item, index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.textAlign !== undefined" label="对齐方式">
          <el-select v-model="configData.chartOption.textAlign" placeholder="请选择">
            <el-option label="居左" value="left"></el-option>
            <el-option label="居中" value="center"></el-option>
            <el-option label="居右" value="right"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.alignItems !== undefined" label="上下对齐方式">
          <el-select v-model="configData.chartOption.alignItems" placeholder="请选择">
            <el-option label="居上" value="flex-start"></el-option>
            <el-option label="居中" value="center"></el-option>
            <el-option label="居下" value="flex-end"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.display !== undefined" label="布局">
          <el-select v-model="configData.chartOption.display" placeholder="请选择">
            <el-option label="块级元素" value="block"></el-option>
            <el-option label="行内元素" value="inline"></el-option>
            <el-option label="行内块元素" value="inline-block"></el-option>
            <el-option label="弹性布局" value="flex"></el-option>
            <el-option label="行内弹性布局" value="inline-flex"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
          <el-select v-model="configData.chartOption.animate" placeholder="请选择">
            <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { fontFamilys } from "../../../ComponentsConfig";
export default {
  props: ["costomData"],
  data() {
    return {
      fontFamilys: fontFamilys,
      fontWeights: ["normal", "bold", "bolder", "lighter"],
      animateOptions,
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {},
  computed: {},
  methods: {},
};
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>