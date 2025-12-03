<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="82px" label-position="top" class="custom_form_item">

        <el-collapse v-model="activeNames" accordion>

          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="标签边距" name="2" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.paddingLeftRight !== undefined" label="左右内边距">
              <el-input-number size="mini" controls-position="right"
                v-model="configData.chartOption.paddingLeftRight"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.paddingTopBottom !== undefined" label="上下内边距">
              <el-input-number size="mini" controls-position="right"
                v-model="configData.chartOption.paddingTopBottom"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginLeftRight !== undefined" label="左右外边距">
              <el-input-number size="mini" controls-position="right"
                v-model="configData.chartOption.marginLeftRight"></el-input-number>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginTopBottom !== undefined" label="上下外边距">
              <el-input-number size="mini" controls-position="right"
                v-model="configData.chartOption.marginTopBottom"></el-input-number>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="默认样式" name="3" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.fontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.fontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.letterSpacing !== undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.letterSpacing" :min="0" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.fontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontFamily !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.fontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.fontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.fontWeight" placeholder="请选择">
                <el-option v-for="(item, index) in fontWeight" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>


            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.normalBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.backgroundColor !== undefined && configData.chartOption.normalBGFlag == 'color'"
              label="背景颜色">
              <el-color-picker v-model="configData.chartOption.backgroundColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.normalBGFlag == 'img'" label="背景图片">
              <image-gallary @getImg="getNormalBg"></image-gallary>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="选中样式" name="4" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.selectedFontSize !== undefined" label="字体大小">
              <el-slider v-model="configData.chartOption.selectedFontSize" :min="1" :step="1" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedLetterSpacing !== undefined" label="字体间距">
              <el-slider v-model="configData.chartOption.selectedLetterSpacing" :min="0" :step="1"
                show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontColor !== undefined" label="字体颜色">
              <el-color-picker v-model="configData.chartOption.selectedFontColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontFamily !== undefined" label="字体名称">
              <el-select v-model="configData.chartOption.selectedFontFamily" placeholder="请选择">
                <el-option v-for="(item, index) in fontFamilys" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedFontWeight !== undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.selectedFontWeight" placeholder="请选择">
                <el-option v-for="(item, index) in fontWeight" :key="index" :label="item" :value="item">
                </el-option>
              </el-select>
            </el-form-item>

            <el-form-item label="背景设置">
              <el-radio-group v-model="configData.chartOption.selectedBGFlag">
                <el-radio label="color">背景颜色</el-radio>
                <el-radio label="img">背景图片</el-radio>
              </el-radio-group>
            </el-form-item>

            <el-form-item
              v-if="configData.chartOption.selectedBackgroundColor !== undefined && configData.chartOption.selectedBGFlag == 'color'"
              label="背景颜色">
              <el-color-picker v-model="configData.chartOption.selectedBackgroundColor" show-alpha></el-color-picker>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.selectedBGFlag == 'img'" label="背景图片">
              <!-- <image-gallary @getImg="getSelectedBg"></image-gallary> -->
              <image-upload v-model="configData.chartOption.selectedBGImage" :limit="1"></image-upload>
            </el-form-item>

          </el-collapse-item>

          <el-collapse-item title="选中方式" name="5" class="nopaddingbottom">
            <el-form-item label="选择方式">
              <el-radio-group v-model="configData.chartOption.checkType">
                <el-radio label="single">单选</el-radio>
                <el-radio label="multiple">多选</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-collapse-item>


          <el-collapse-item title="绑定组件" name="6" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.name !== undefined" label="参数名称">
              <el-input v-model="configData.chartOption.name" placeholder="请输入参数名称" />
            </el-form-item>

            <el-form-item label="绑定组件">
              <el-select v-model="configData.chartOption.selectedCharts" multiple placeholder="请选择" filterable
                @change="bindCharts">
                <el-option v-for="item in chartList" :key="item.customId" :label="item.layerName"
                  :value="item.customId">
                </el-option>
              </el-select>
            </el-form-item>

          </el-collapse-item>

          <!-- <el-collapse-item title="自动轮播" name="7">

            <el-form-item label="是否自动轮播">
              <el-switch v-model="configData.chartOption.isRotation" />
            </el-form-item>

            <el-form-item label="间隔时长">
              <el-input-number v-model="configData.chartOption.dur" controls-position="right"  :step="1000"></el-input-number>
            </el-form-item>

          </el-collapse-item> -->
          <el-collapse-item title="动画" name="8" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.animate !== undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value">
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
import { getLinkChart } from "../../../util/LinkageChart";
import { fontFamilys } from "../../../ComponentsConfig";
export default {
  props: ["costomData", "drawingList"],
  data() {
    return {
      fontFamilys: fontFamilys,
      fontWeight: ['normal', 'bold', 'bolder', 'lighter'],
      activeNames: ['1'],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
      isUpdatingFromCostomData: false
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        if (this.isUpdatingFromCostomData) {
          this.isUpdatingFromCostomData = false;
          return;
        }
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
      },
    },
  },
  //页面加载完执行
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {},
  methods: {
    bindCharts(val) {
      this.$set(this.configData.chartOption, 'selectedCharts', val);
    },
    getNormalBg(val) {
      this.$set(this.configData.chartOption, 'normalBGImage', val);
    },
    getSelectedBg(val) {
      this.$set(this.configData.chartOption, 'selectedBGImage', val);
    },
  },
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
