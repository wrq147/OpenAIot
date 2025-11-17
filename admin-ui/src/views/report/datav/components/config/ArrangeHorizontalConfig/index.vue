<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs" @tab-click="handleClick">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="交互" name="interaction" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <!-- 组件属性 -->
    <moduleDeploy v-if="currentTab === 'field'" :costomData="configData"></moduleDeploy>
    <div class="field-box" v-if="currentTab === 'data'">
      <el-scrollbar class="right-scrollbar">
        <!-- 表单属性 -->
        <data-source-config :drawingList="drawingList" :themeForm="themeForm" :dataSourceType="configData.chartOption.dataSourceType"
          :customData="configData" :customId="configData.customId" @changeSource="changeSource" @changeData="changeData"
          :baseType="'three'" @changeGlobalProcessor="changeGlobalProcessor">
          <template v-slot:referFormat v-if="configData.chartOption.dataSourceType != 'static'">
            <el-form-item label="参考格式">
              <el-input type="textarea" :rows="5" :value="refData" />
            </el-form-item>
          </template>
          <template v-slot:processorSlot>
            <el-tabs v-model="activeProcessor" class="processor-tabs" editable @edit="handleTabsEdit" v-if="configData.chartOption.dataSourceType === 'gobal'">
              <el-tab-pane v-for="item in processorTabs" :key="'tab' + item.name" :label="item.title" :name="item.name">
                <el-form-item label="绑定数据">
                  <el-select v-model="item.globalData" placeholder="请选择" @change="changeActiveGlobal">
                    <el-option v-for="(item, idx) in globalArr" :key="idx" :label="item.name" :value="item.name"/>
                  </el-select>
                </el-form-item>
                <el-form-item v-if="item && item.globalData" label="过滤器">
                  <el-select v-model="item.globalProcessor" placeholder="请选择">
                    <el-option v-for="(item, itemKey) in globalProcessor" :key="itemKey" :label="item" :value="item"/>
                  </el-select>
                </el-form-item>
              </el-tab-pane>
            </el-tabs>
          </template>
          <template>
            <globalslot :costomData="configData" ref="globalslot" :processorTabs="processorTabs" :activeTab="activeTab" :themeForm="themeForm" :drawingList="drawingList"
              :tableListMap="tableListMap" @setTabsData="setTabsData" @costom-change="costomChange"></globalslot>
          </template>
        </data-source-config>
      </el-scrollbar>
    </div>
    <!-- 组件位置 -->
    <modulePosition v-if="currentTab === 'location'" :costomData="configData"></modulePosition>
    <!-- 组件交互 -->
    <el-form v-if="currentTab === 'interaction'" size="small" label-width="110px" style="padding: 20px 10px">
      <div style="background-color: #f5f5f5;padding: 10px 15px;border: solid 1px #dadada;font-size: 14px;margin-bottom: 15px;color: #999;">
        事件预处理
      </div>
      <chart-interact :chartOption="configData.chartOption" @changeData="changeInteractData"></chart-interact>
    </el-form>
  </div>
</template>

<script>
import DataSourceConfig from "../DataConfig/DataSourceConfig";
import sourceConfig from "../../mixins/sourceConfig.js";
import modulePosition from "./modulePosition";
import moduleDeploy from "./moduleDeploy";
import ChartInteract from "../interact/ChartInteract";
import globalslot from "./globalslot";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    modulePosition,
    moduleDeploy,
    ChartInteract,
    globalslot,
  },
  data() {
    return {
      currentTab: "field",
      // filedSelected: [], //被选中的字段
      processorTabs: [],
      defauleValue: [{
        title: "1号电表(后处理)",
        currentlist: [
          {Name: "功率",Code: "sumkwh",Value: 0,Unit: "kw",OptionType: "float",UpdatedOn: "2025-10-28 16:06:57",Description: "",},
          {Name: "A相电流",Code: "Fa",Value: 0,Unit: "A",OptionType: "float",UpdatedOn: "2025-10-28 16:06:57",Description: "",},
          {Name: "B相电流",Code: "Fb",Value: 0,Unit: "A",OptionType: "float",UpdatedOn: "2025-10-28 16:06:57",Description: "",},
          {Name: "C相电流",Code: "Fc",Value: 0,Unit: "A",OptionType: "float",UpdatedOn: "2025-10-28 16:06:57",Description: "",},
        ],
        rowListObj: {
          title: [{Name: "当前总电能",Code: "Totalkwh",Value: 259.2,Unit: "kwh",OptionType: "float",UpdatedOn: "2025-10-28 16:00:58",Description: "",}],
          list: [
            {Value: 0,Unit: "Nm³",Name: "今日用电量",Code: "todayuseenerge",},
            {Value: 0,Unit: "Nm³",Name: "昨日用电量",Code: "yesterdayepi",},
            {Value: 209.6,Unit: "Nm³",Name: "本月用电量",Code: "currentenerge",},
            {Value: 20.8,Unit: "kwh",Name: "上月用电量",Code: "preenerge",},
          ],
        },
      },],
      activeProcessor: "", //活动的过滤器
      tabIndex: 0,
      tableListMap: new Map(),
    };
  },

  //页面加载完执行
  mounted() {},
  computed: {
    refData() {
      let chaArr = [];
      this.configData.chartOption.staticDataValue.map((row) => {
        let obj = {
          标题: row.title,
          数据列表: [],
          统计数据: {
            统计标题:[],
            统计列表:[],
          },
        };
        row.currentList.map((rw) => {
          let obj2 = {
            标签: rw.Name,
            标识符: rw.Code,
            单位: rw.Unit,
            数值: rw.Value,
          };
          obj["数据列表"].push(obj2);
        });
        row.rowListObj.list.map((rw) => {
          let obj2 = {
            标签: rw.Name,
            标识符: rw.Code,
            单位: rw.Unit,
            数值: rw.Value,
          };
          obj['统计数据']["统计列表"].push(obj2);
        });
        obj['统计数据']['统计标题']=[{
          标签: row.rowListObj.title[0].Name,
          标识符: row.rowListObj.title[0].Code,
          单位: row.rowListObj.title[0].Unit,
          数值: row.rowListObj.title[0].Value,
        }]
        chaArr.push(obj);
      });
      return JSON.stringify(chaArr);
    },
    globalArr() {
      if (this.themeForm == null) {
        return [];
      }
      return this.themeForm.globalData;
    },
    globalProcessor() {
      if (!this.activeTab || this.activeTab.globalData == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.activeTab.globalData
      );
      if (tdlist.length > 0 && tdlist[0].rawData !== undefined) {
        let optionData = [];
        JSON.parse(tdlist[0].rawData).forEach((item, index) => {
          optionData.push(item.title);
        });
        return optionData;
      } else {
        return [];
      }
    },
    activeTab() {
      if (this.processorTabs && this.processorTabs.length > 0) {
        let activeTab = this.processorTabs.find(
          (row) => row.name == this.activeProcessor
        );
        return activeTab;
      } else {
        return {};
      }
    },
  },
  methods: {
    changeActiveGlobal() {
      if (this.activeProcessor) {
        this.processorTabs.map((row) => {
          if (row.name == this.activeProcessor) {
            row.globalProcessor = "";
          }
        });
      }
    },
    costomChange(val) {
      this.configData = val;
    },
    setTabsData(data) {
      if (this.activeProcessor) {
        this.processorTabs.map((row) => {
          if (row.name == this.activeProcessor) {
            // row.tableList = data;
            this.tableListMap.set(this.activeProcessor, data);
          }
        });
      }
    },
    loadProcessData() {
      if (this.configData.chartOption.configProcessorTabs) {
        this.processorTabs = this.configData.chartOption.configProcessorTabs;
        this.loadData()
        this.activeProcessor = this.processorTabs[0].name;
        const maxValue = Math.max(
          ...this.processorTabs.map((item) => Number(item.name))
        );
        this.tabIndex = maxValue;
      }
    },
    loadData() {
      for (let i = 0; i < this.processorTabs.length; i++) {
        let rowTabs = this.processorTabs[i];
        this.activeProcessor=rowTabs.name
        if (!rowTabs || rowTabs.globalData == "") {
          return [];
        }
        if (!rowTabs || rowTabs.globalProcessor == "") {
          return [];
        }
        let tdlist = this.themeForm.globalData.filter(
          (x) => x.name == rowTabs.globalData
        );
        if (tdlist.length > 0 && rowTabs.globalProcessor) {
          let dblist = JSON.parse(JSON.stringify(tdlist));
          if (dblist[0] && dblist[0].rawData) {
          } else {
            return [];
          }
          let dataArr = dblist[0].rawData ? JSON.parse(dblist[0].rawData) : [];
          let valueObj = dataArr.filter(
            (item) => item.title === rowTabs.globalProcessor
          );
          this.setTabsData(valueObj[0].content);
        }
      }
    },
    handleClick(val) {
      // console.log("切换tab", this.configData.chartOption.tableSelectLine);
      if (this.currentTab == "data") {
        // this.$nextTick(() => {});
        this.loadProcessData();
      }
    },
    changeGlobalProcessor(val, isType) {
      //用于切换
      // this.configData = val;
      if (isType) {
        if (this.configData.chartOption.dataSourceType == "global") {
          this.loadProcessData();
        }
        let aq = [];
        if (this.configData.chartOption.tableSelectLine) {
          aq = JSON.parse(
            JSON.stringify(this.configData.chartOption.tableSelectLine)
          );
        }
        if (aq && aq.main && aq.main.length > 0) {
        } else {
          if (this.processorTabs.length == 0) {
            this.handleTabsEdit("", "add");
          }
          let defarr = [
            {
              key: "title",
              filed: "",
            },
            {
              key: "rowtitle",
              // parentIdKey: "rowListObj",
              filed: "",
            },
            {
              key: "currentList",
              filed: "",
            },
            {
              key: "list",
              // parentIdKey: "rowListObj",
              filed: "",
            },
          ];
          let defobj = {
            main: defarr,
            rowtitle:[],
            currentList: [],
            list: [],
          };
          this.$set(this.configData.chartOption, "tableSelectLine", defobj);
        }
      }
    },
    // 添加删除 tabs标签
    handleTabsEdit(targetName, action) {
      if (action === "add") {
        let newTabName = ++this.tabIndex + "";
        this.processorTabs.push({
          title: "数据" + newTabName,
          name: newTabName,
          globalData: "",
          globalProcessor: "",
          // tableList: [],
        });
        this.tableListMap.set(newTabName, []);
        this.resultPreName = "";
        this.activeProcessor = newTabName;
      }
      if (action === "remove") {
        if (this.processorTabs && this.processorTabs.length <= 1) {
          return;
        }
        let tabs = this.processorTabs;
        let activeName = this.activeProcessor;
        if (activeName === targetName) {
          tabs.forEach((tab, index) => {
            if (tab.name === targetName) {
              let nextTab = tabs[index + 1] || tabs[index - 1];
              if (nextTab) {
                activeName = nextTab.name;
              }
            }
          });
        }
        this.activeProcessor = activeName;
        this.processorTabs = tabs.filter((tab) => tab.name !== targetName);
      }
      this.$set(this.configData.chartOption,"configProcessorTabs",this.processorTabs);
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
.dataOrigin {
  font-size: 14px;
  color: red;
  margin-bottom: 10px;
}

.processor-tabs::v-deep {
  .el-input__icon {
    line-height: 28px;
  }
  .el-tabs__new-tab {
    background-color: #1682e6;
    margin-right: 18px;
    line-height: 16px;
  }
}
</style>
