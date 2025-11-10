<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form
        size="small"
        label-width="76px"
        label-position="top"
        class="custom_form_item"
      >
        <el-form-item
          v-if="configData.layerName !== undefined"
          label="图层名称"
        >
          <el-input
            v-model="configData.layerName"
            placeholder="请输入图层名称"
          />
        </el-form-item>

        <el-form-item
          v-if="configData.chartOption.fontSize !== undefined"
          label="字体大小"
        >
          <el-slider
            v-model="configData.chartOption.fontSize"
            :min="1"
            :max="200"
            :step="1"
            show-input
          ></el-slider>
        </el-form-item>

        <el-form-item
          v-if="configData.chartOption.fontFamily !== undefined"
          label="字体名称"
        >
          <el-select
            v-model="configData.chartOption.fontFamily"
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
          v-if="configData.chartOption.fontColor !== undefined"
          label="字体颜色"
        >
          <el-color-picker
            v-model="configData.chartOption.fontColor"
            show-alpha
          ></el-color-picker>
        </el-form-item>

        <el-form-item
          v-if="configData.chartOption.letterSpacing !== undefined"
          label="字体间距"
        >
          <el-slider
            v-model="configData.chartOption.letterSpacing"
            :min="0"
            :max="200"
            :step="1"
            show-input
          ></el-slider>
        </el-form-item>

        <el-form-item
          v-if="configData.chartOption.backgroundColor !== undefined"
          label="字体背景"
        >
          <el-color-picker
            v-model="configData.chartOption.backgroundColor"
            show-alpha
          ></el-color-picker>
        </el-form-item>

        <el-form-item
          v-if="configData.chartOption.fontWeight !== undefined"
          label="文字粗细"
        >
          <el-select
            v-model="configData.chartOption.fontWeight"
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
          v-if="configData.chartOption.textAlign !== undefined"
          label="对齐方式"
        >
          <el-select
            v-model="configData.chartOption.textAlign"
            placeholder="请选择"
          >
            <el-option label="居左" value="left"></el-option>
            <el-option label="居中" value="center"></el-option>
            <el-option label="居右" value="right"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item label="是否自定义">
          <el-switch v-model="configData.chartOption.isCustom" />
        </el-form-item>

        <el-form-item
          v-show="configData.chartOption.isCustom === false"
          label="时间格式"
        >
          <el-select
            v-model="configData.chartOption.dateFormat"
            placeholder="请选择"
          >
            <el-option
              v-for="(item, index) in dateFormats"
              :key="index"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </el-form-item>

        <el-form-item
          v-show="configData.chartOption.isCustom === true"
          label="自定义格式"
        >
          <el-input
            type="textarea"
            :rows="5"
            v-model="configData.chartOption.customFormat"
          ></el-input>
        </el-form-item>

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
      dateFormats: [
        { value: "yyyy-MM-dd", label: "日期" },
        { value: "yyyy-MM-dd HH:mm", label: "日期+时分" },
        { value: "yyyy-MM-dd HH:mm:ss", label: "日期+时分秒" },
        { value: "MM-dd", label: "日期(无年)" },
        { value: "HH:mm", label: "时分" },
        { value: "HH:mm:ss", label: "时分秒" },
        { value: "day", label: "星期" },
      ],
      animateOptions,
      configData: this.costomData,
    };
  },
  //页面加载完执行
  mounted() {
  },
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
