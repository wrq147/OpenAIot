<template>
    <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field">
        <el-scrollbar class="right-scrollbar">
          <module-deploy ref="moduleDeploy" :configData="configData" :costomData="costomData" />
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
            :baseType="'table'"
          >
            <template #referFormat v-if="configData.chartOption.dataSourceType != 'static'">
              <el-form-item label="参考格式">
                <el-input type="textarea" :rows="5" :value="refData" />
              </el-form-item>
            </template>
            <template #staticSlot v-if="configData.chartOption.dataSourceType === 'static'">
              <staticSolt :costomData="configData" @costom-change="costomChange"/>
            </template>
            <template>
              <gobalSolt :costomData="configData" :themeForm="themeForm" @costom-change="costomChange"/>
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

import DataSourceConfig from "../DataConfig/DataSourceConfig";
import sourceConfig from "../../mixins/sourceConfig.js";
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
    };
  },
  computed:{
    refData() {
      let chaArr = [];
      this.configData.chartOption.staticDataValue.map((row) => {
        let obj={
          'x轴': row.xAxisData,
          'y轴': row.yAxisData,
          '值': row.data,
        };
        chaArr.push(obj)
      });
      return JSON.stringify(chaArr);
    },
  },
  methods: {
    costomChange(val){
      this.configData=val
    },
    changeGlobalProcessor() {
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
/**  滚动条凹槽的颜色，还可以设置边框属性  **/
::-webkit-scrollbar-track-piece {
  background-color: #f8f8f8;
  border-radius: 10px;
}
/** 滚动条的宽度  **/
::-webkit-scrollbar {
  width: 9px;

  height: 9px;
}
/** 滚动条的设置  **/
::-webkit-scrollbar-thumb {
  background-color: #dddddd;

  background-clip: padding-box;

  min-height: 28px;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #bbb;
}
</style>
