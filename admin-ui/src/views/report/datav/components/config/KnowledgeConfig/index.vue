<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field">
        <el-scrollbar class="right-scrollbar">
          <module-deploy ref="moduleDeploy" :configData="configData" />
        </el-scrollbar>
      </el-tab-pane>
      <el-tab-pane label="数据" name="data">
        <el-scrollbar class="right-scrollbar">
          <data-source-config
            :drawingList="drawingList"
            :themeForm="themeForm"
            :dataSourceType="configData.chartOption.dataSourceType"
            :customData="configData"
            :customId="configData.customId"
            @changeSource="changeSource"
            @changeData="changeData"
            @changeGlobalProcessor="changeGlobalProcessor"
          >
            <template #staticSlot v-if="configData.chartOption.dataSourceType === 'static'">
              <staticSolt :configData="configData" />
            </template>
            <template>
              <gobalSolt :configData="configData" :themeForm="themeForm" />
            </template>
          </data-source-config>
        </el-scrollbar>
      </el-tab-pane>
      <el-tab-pane label="定位" name="location">
        <el-scrollbar class="right-scrollbar">
          <module-position :configData="configData" />
        </el-scrollbar>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>

import DataSourceConfig from '../DataConfig/DataSourceConfig'
import sourceConfig from '../../mixins/sourceConfig.js'
import modulePosition from "./modulePosition";
import moduleDeploy from "./moduleDeploy";
import staticSolt from "./staticSolt";
import gobalSolt from "./gobalSolt";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    modulePosition,
    moduleDeploy,
    staticSolt,
    gobalSolt
  },
  data() {
    return {
      currentTab: "field",
    }
  },
  //页面加载完执行
  mounted() {
    
  },
  computed: {
    
  },
  methods: {
    changeGlobalProcessor() {
      let modelValue = this.configData.chartOption.modelValue!==undefined ? this.configData.chartOption.modelValue :  {}
      this.$set(this.configData.chartOption, "modelValue", modelValue);
    }
  }
}
</script>

<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
    width: 33%;
    text-align: center;
}
::v-deep .el-scrollbar__wrap{
  height: calc(100vh - 40px);
}
</style>