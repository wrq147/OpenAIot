<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="交互" name="interaction" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <!-- 组件属性 -->
    <moduleDeploy
      v-if="currentTab === 'field'"
      :costomData="configData"
      :drawingList="drawingList"
      @costom-change="costomChange"
    ></moduleDeploy>
    <div class="field-box" v-if="currentTab === 'data'">
      <el-scrollbar class="right-scrollbar">
        <!-- 表单属性 -->
        <data-source-config
          :drawingList="drawingList"
          :themeForm="themeForm"
          :dataSourceType="configData.chartOption.dataSourceType"
          :customData="configData"
          :customId="configData.customId"
          :baseType="''"
          :isOnlyStatic="true"
        >
        </data-source-config>
      </el-scrollbar>
    </div>
    <!-- 组件交互 -->
    <interaction
      @costom-change="costomChange"
      :costomData="configData"
      :themeForm="themeForm"
      :drawingList="drawingList"
      @changeData="changeInteractData"
      v-if="currentTab === 'interaction'"
    ></interaction>
    <!-- 组件位置 -->
    <modulePosition
      v-if="currentTab === 'location'"
      :costomData="configData"
    ></modulePosition>
  </div>
</template>

<script>
import DataSourceConfig from "../DataConfig/DataSourceConfig";
import sourceConfig from "../../mixins/sourceConfig.js";
import modulePosition from "./modulePosition";
import moduleDeploy from "./moduleDeploy";
import interaction from "./interaction";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    modulePosition,
    moduleDeploy,
    interaction,
  },
  data() {
    return {
      currentTab: "field",
      filedSelected: [], //被选中的字段
      filedSelected2: [],
      finalResult: [],
      filterResultData: [],
    };
  },
  watch: {},
  //页面加载完执行
  mounted() {},
  computed: {
  },
  methods: {
    costomChange(newVal) {
      this.configData = newVal;
      console.log(this.configData, 'this.configData')
    },
  },
};
</script>
<style lang="scss">
.field-box {
  .right-scrollbar {
    .el-collapse-item.nopaddingbottom {
      .el-collapse-item__content {
        padding-bottom: 0;
      }
    }
    .el-collapse-item .el-collapse-item__header {
      font-size: 16px;
      font-weight: bold;
    }
    .el-collapse-item__content {
      .el-form-item .el-form-item__label {
        font-size: 14px;
        color: #666;
      }
    }
  }
}
</style>
<style lang="scss" scoped>
.cellSelected {
  border-color: #409eff !important;
  border-left: 1px solid;
  border-right: 1px solid;
  background: #ecf5ff !important;
  color: #409eff !important;
  font-weight: 600;
}
th.cellSelected {
  border-top: 1px solid;
}
::v-deep .center-tabs .el-tabs__item {
  width: 25%;
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
