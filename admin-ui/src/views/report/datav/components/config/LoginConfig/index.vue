<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <!-- 组件属性 -->
    <moduleDeploy
      v-if="currentTab === 'field'"
      :costomData="configData"
    ></moduleDeploy>
    <div class="field-box" v-if="currentTab !== 'field'">
      <el-scrollbar class="right-scrollbar">
        <!-- 表单属性 -->
        <el-form v-if="currentTab === 'data'" size="small" label-width="76px" label-position="top" class="custom_form_item">
          <data-source-config
            :drawingList="drawingList"
            :themeForm="themeForm"
            :dataSourceType="configData.chartOption.dataSourceType"
            :customData="configData"
            :customId="configData.customId"
            @changeSource="changeSource"
            @changeData="changeData"
            :baseType="'three'"
          ></data-source-config>
        </el-form>
        <!-- 组件位置 -->
        <el-form
          v-if="currentTab === 'location'"
          size="small"
          label-width="54px"
          class="custom_form_item"
        >
          <el-form-item label="X位置">
            <el-input-number
              v-model="configData.x"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="y位置">
            <el-input-number
              v-model="configData.y"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="宽度">
            <el-input-number
              v-model="configData.width"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number
              v-model="configData.height"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
          <el-form-item label="zIndex">
            <el-input-number
              v-model="configData.zindex"
              controls-position="right"
              :step="1"
            ></el-input-number>
          </el-form-item>
        </el-form>
      </el-scrollbar>
    </div>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import DataSourceConfig from "../DataConfig/DataSourceConfig";
import sourceConfig from "../../mixins/sourceConfig.js";
import moduleDeploy from "./moduleDeploy";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    moduleDeploy
  },
  data() {
    return {
      fontFamilys: this.fontFamilys,
      types: [
        "text",
        "number",
        "button",
        "textarea",
        "date",
        "datetime-local",
        "time",
        "month",
        "week",
        "password",
      ],
      positionList:[{value:'top',label:'上'},{value:'left',label:'左'},],
      dataList: [],
      currentTab: "field",
      animateOptions,
    };
  },
  //页面加载完执行
  mounted() {
    //获取可联动数据源列表
    this.dataList = this.themeForm.globalData;
  },
  computed: {},
  methods: {
    bindDataList(val) {
      this.$set(this.configData.chartOption, "dataList", val);
    },
  },
};
</script>

<style scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}
</style>
