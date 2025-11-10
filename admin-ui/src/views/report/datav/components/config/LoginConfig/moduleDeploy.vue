<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form
        size="small"
        label-width="90px"
        label-position="top"
        class="custom_form_item"
      >
        <el-collapse v-model="activeNames" accordion>
          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称"/>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.levelsetting.bgColor !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.levelsetting.bgColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.levelsetting.bgRadius !== undefined" label="背景圆角大小">
              <el-slider v-model="configData.chartOption.levelsetting.bgRadius"
                :min="0" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="导航设置" name="2" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.navsetting.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.navsetting.fontSize"
                :min="10" :max="200" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.fontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.navsetting.fontWeight" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.lineHeight !== undefined" label="字体行高">
              <el-slider v-model="configData.chartOption.navsetting.lineHeight"
                :min="0" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.fontPadding !== undefined" label="字体上下内边距">
              <el-slider v-model="configData.chartOption.navsetting.fontPadding"
                :min="0" :max="200" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.navsetting.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.activeColor !== undefined" label="活动字体颜色">
              <el-color-picker v-model="configData.chartOption.navsetting.activeColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.borderColor !== undefined" label="底边颜色">
              <el-color-picker v-model="configData.chartOption.navsetting.borderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.navsetting.activeBorderColor !== undefined" label="底边活动颜色">
              <el-color-picker v-model="configData.chartOption.navsetting.activeBorderColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="表单标签设置" name="3" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.label.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.label.fontSize"
                :min="10" :max="50" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.fontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.label.fontWeight" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.label.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.label.fontColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="表单输入框设置" name="4" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.input.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.input.fontSize"
                :min="10" :max="50" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.inputRadius !== undefined" label="输入框圆角">
              <el-slider v-model="configData.chartOption.input.inputRadius"
                :min="0" :max="20" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.inputHeight !== undefined" label="输入框高度">
              <el-slider v-model="configData.chartOption.input.inputHeight"
                :min="38" :max="200" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.input.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.inputBorderColor !== undefined" label="输入框边框颜色">
              <el-color-picker v-model="configData.chartOption.input.inputBorderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.focusBorderColor !== undefined" label="输入框聚焦时边框颜色">
              <el-color-picker v-model="configData.chartOption.input.focusBorderColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.input.inputBgColor !== undefined" label="输入框背景颜色">
              <el-color-picker v-model="configData.chartOption.input.inputBgColor" show-alpha></el-color-picker>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="按钮样式" name="5" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.buttonSetting.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.buttonSetting.fontSize"
                :min="10" :max="50" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.buttonSetting.btnRadius !== undefined" label="圆角">
              <el-slider v-model="configData.chartOption.buttonSetting.btnRadius"
                :min="0" :max="20" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.buttonSetting.btnHeight !== undefined" label="高度">
              <el-slider v-model="configData.chartOption.buttonSetting.btnHeight"
                :min="38" :max="200" :step="1" show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.buttonSetting.fontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.buttonSetting.fontWeight" placeholder="请选择">
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.buttonSetting.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.buttonSetting.fontColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.buttonSetting.btnBgStart !== undefined&&configData.chartOption.buttonSetting.btnBgEnd !== undefined" label="背景颜色">
              <el-color-picker v-model="configData.chartOption.buttonSetting.btnBgStart" show-alpha></el-color-picker>
              <el-color-picker v-model="configData.chartOption.buttonSetting.btnBgEnd" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item label="登录后跳转地址">
              <el-radio-group v-model="configData.chartOption.buttonSetting.isDefalutUrl">
                <el-radio :label="true">默认</el-radio>
                <el-radio :label="false">其他地址</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="!configData.chartOption.buttonSetting.isDefalutUrl" label="跳转地址">
              <el-input type="textarea" v-model="configData.chartOption.buttonSetting.loginJumpUrl" placeholder="请输入跳转地址"/>
            </el-form-item>
          </el-collapse-item>
          <el-collapse-item title="动画" name="9" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.animate !== undefined"
              label="载入动画"
            >
              <el-select
                v-model="configData.chartOption.animate"
                placeholder="请选择"
              >
                <el-option
                  v-for="item in animateOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                >
                </el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>
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
      activeNames: ["1"],
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
