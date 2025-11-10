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
            <el-form-item
              v-if="configData.layerName !== undefined"
              label="图层名称"
            >
              <el-input
                v-model="configData.layerName"
                placeholder="请输入图层名称"
              />
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="头像设置" name="2" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.head.height !== undefined"
              label="头部高度占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.height"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.marginTop !== undefined"
              label="头部上边距占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.marginTop"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.headImg.height !== undefined"
              label="头像高度占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.headImg.height"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.headImg.width !== undefined"
              label="头像宽度占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.headImg.width"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.head.headImg.marginLeft !== undefined
              "
              label="头像左边距"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.headImg.marginLeft"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item label="头像图片" label-width="110px">
              <image-upload
                v-model="configData.chartOption.head.headImg.src"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="信息行设置" name="6" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.infoItem.height !== undefined"
              label="整体高度占比"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.height"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.marginTop !== undefined"
              label="行上边距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.marginTop"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.textMarginTop !== undefined"
              label="文字上边距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.textMarginTop"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.infoItem.textMarginLeft !== undefined
              "
              label="文字左边距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.textMarginLeft"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.fontFamily !== undefined"
              label="字体名称"
              label-width="100px"
            >
              <el-select
                v-model="configData.chartOption.infoItem.fontFamily"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.fontSize !== undefined"
              label="字体大小"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.fontSize"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.letterSpacing !== undefined"
              label="字体间距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.infoItem.letterSpacing"
                :min="0"
                :max="200"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.fontWeight !== undefined"
              label="文字粗细"
              label-width="100px"
            >
              <el-select
                v-model="configData.chartOption.infoItem.fontWeight"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.infoItem.fontColor !== undefined"
              label="字体颜色"
              label-width="100px"
            >
              <el-color-picker
                v-model="configData.chartOption.infoItem.fontColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item
              v-show="configData.chartOption.infoItem.background !== undefined"
              label="字体背景"
              label-width="100px"
            >
              <el-radio-group
                v-model="configData.chartOption.infoItem.background"
              >
                <el-radio label="color">背景色</el-radio>
                <el-radio label="img">背景图</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.infoItem.backgroundColor !== undefined
              "
              label="背景颜色"
              label-width="100px"
            >
              <el-color-picker
                v-model="configData.chartOption.infoItem.backgroundColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item label="背景图" label-width="100px">
              <image-upload
                v-model="configData.chartOption.infoItem.backgroundImg"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="详细行设置" name="7" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.detailItem.height !== undefined"
              label="整体高度占比"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.detailItem.height"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.detailItem.marginTop !== undefined"
              label="行上边距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.detailItem.marginTop"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.detailItem.textMarginTop !== undefined
              "
              label="文字上边距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.detailItem.textMarginTop"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.detailItem.fontFamily !== undefined"
              label="字体名称"
              label-width="100px"
            >
              <el-select
                v-model="configData.chartOption.detailItem.fontFamily"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.detailItem.fontSize !== undefined"
              label="字体大小"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.detailItem.fontSize"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.detailItem.letterSpacing !== undefined
              "
              label="字体间距"
              label-width="100px"
            >
              <el-slider
                v-model="configData.chartOption.detailItem.letterSpacing"
                :min="0"
                :max="200"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.detailItem.fontWeight !== undefined"
              label="文字粗细"
              label-width="100px"
            >
              <el-select
                v-model="configData.chartOption.detailItem.fontWeight"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.detailItem.fontColor !== undefined"
              label="字体颜色"
              label-width="100px"
            >
              <el-color-picker
                v-model="configData.chartOption.detailItem.fontColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item
              v-show="
                configData.chartOption.detailItem.background !== undefined
              "
              label="字体背景"
              label-width="100px"
            >
              <el-radio-group
                v-model="configData.chartOption.detailItem.background"
              >
                <el-radio label="color">背景色</el-radio>
                <el-radio label="img">背景图</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.detailItem.backgroundColor !== undefined
              "
              label="背景颜色"
              label-width="100px"
            >
              <el-color-picker
                v-model="configData.chartOption.detailItem.backgroundColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item label="背景图" label-width="100px">
              <image-upload
                v-model="configData.chartOption.detailItem.backgroundImg"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="提示框设置" name="3" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.head.tipImg.sync !== undefined"
              label="是否与提示框文字同步"
              label-width="200px"
            >
              <el-switch
                v-model="configData.chartOption.head.tipImg.sync"
              ></el-switch>
            </el-form-item>

            <el-form-item
              v-show="configData.chartOption.head.tipImg.show !== undefined"
              label="是否显示"
              label-width="110px"
            >
              <el-radio-group v-model="configData.chartOption.head.tipImg.show">
                <el-radio label="block">显示</el-radio>
                <el-radio label="none">隐藏</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipImg.height !== undefined"
              label="提示框高度占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.tipImg.height"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipImg.width !== undefined"
              label="提示框宽度占比"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.tipImg.width"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipImg.marginLeft !== undefined"
              label="提示框左边距"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.tipImg.marginLeft"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipImg.marginTop !== undefined"
              label="提示框上边距"
              label-width="110px"
            >
              <el-slider
                v-model="configData.chartOption.head.tipImg.marginTop"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item label="提示框图片" label-width="110px">
              <image-upload
                v-model="configData.chartOption.head.tipImg.src"
                :limit="1"
              ></image-upload>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="提示框文字" name="4" class="nopaddingbottom">
            <el-form-item
              v-if="
                configData.chartOption.head.tipText.fontFamily !== undefined
              "
              label="字体名称"
            >
              <el-select
                v-model="configData.chartOption.head.tipText.fontFamily"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipText.fontSize !== undefined"
              label="字体大小"
            >
              <el-slider
                v-model="configData.chartOption.head.tipText.fontSize"
                :min="1"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipText.fontColor !== undefined"
              label="字体颜色"
            >
              <el-color-picker
                v-model="configData.chartOption.head.tipText.fontColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.head.tipText.letterSpacing !== undefined
              "
              label="字体间距"
            >
              <el-slider
                v-model="configData.chartOption.head.tipText.letterSpacing"
                :min="0"
                :max="200"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.head.tipText.fontWeight !== undefined
              "
              label="文字粗细"
            >
              <el-select
                v-model="configData.chartOption.head.tipText.fontWeight"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="
                configData.chartOption.head.tipText.marginLeft !== undefined
              "
              label="文字左边距"
            >
              <el-slider
                v-model="configData.chartOption.head.tipText.marginLeft"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.head.tipText.marginTop !== undefined"
              label="文字上边距"
            >
              <el-slider
                v-model="configData.chartOption.head.tipText.marginTop"
                :min="-50"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="名称设置" name="5" class="nopaddingbottom">
            <el-form-item
              v-if="configData.chartOption.name.fontFamily !== undefined"
              label="字体名称"
            >
              <el-select
                v-model="configData.chartOption.name.fontFamily"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontFamilys"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.fontSize !== undefined"
              label="字体大小"
            >
              <el-slider
                v-model="configData.chartOption.name.fontSize"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.fontColor !== undefined"
              label="字体颜色"
            >
              <el-color-picker
                v-model="configData.chartOption.name.fontColor"
                show-alpha
              ></el-color-picker>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.letterSpacing !== undefined"
              label="字体间距"
            >
              <el-slider
                v-model="configData.chartOption.name.letterSpacing"
                :min="0"
                :max="200"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.fontWeight !== undefined"
              label="文字粗细"
            >
              <el-select
                v-model="configData.chartOption.name.fontWeight"
                placeholder="请选择"
              >
                <el-option
                  v-for="(item, index) in fontWeights"
                  :key="index"
                  :label="item"
                  :value="item"
                >
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.marginLeft !== undefined"
              label="名称左边距"
            >
              <el-slider
                v-model="configData.chartOption.name.marginLeft"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.marginTop !== undefined"
              label="名称上边距"
            >
              <el-slider
                v-model="configData.chartOption.name.marginTop"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.name.marginBottom !== undefined"
              label="名称下边距"
            >
              <el-slider
                v-model="configData.chartOption.name.marginBottom"
                :min="-100"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item
            title="卡片属性设置"
            name="8"
            class="nopaddingbottom"
          >
            <el-form-item v-if="configData.chartOption.numOfCol !== undefined"  label="卡片列数">
              <el-slider
                v-model="configData.chartOption.numOfCol"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.borderRadius !== undefined" label="卡片圆角">
              <el-slider
                v-model="configData.chartOption.borderRadius"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.marginLeft !== undefined" label="横向间距(%)">
              <el-slider
                v-model="configData.chartOption.marginLeft"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.marginTop !== undefined"
              label="纵向间距(%)"
            >
              <el-slider
                v-model="configData.chartOption.marginTop"
                :min="0"
                :step="1"
                show-input
              ></el-slider>
            </el-form-item>
            <el-form-item v-show="configData.chartOption.bgType !== undefined" label="字体背景" label-width="100px">
              <el-radio-group v-model="configData.chartOption.bgType">
                <el-radio label="color">背景色</el-radio>
                <el-radio label="img">背景图</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.bgTypeColor !== undefined" label="背景颜色" label-width="100px">
              <el-color-picker v-model="configData.chartOption.bgTypeColor" show-alpha />
            </el-form-item>

            <el-form-item label="背景图" label-width="100px">
              <image-upload v-model="configData.chartOption.bgTypeImg" :limit="1" />
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
      fontFamilys:fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      activeNames: ['1'],
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