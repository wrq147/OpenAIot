<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs" @tab-click="handleClick">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="交互" name="interaction" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <!-- 组件属性 -->
    <moduleDeploy
      v-if="currentTab === 'field'"
      :costomData="configData"
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
          @changeSource="changeSource"
          @changeData="changeData"
          :baseType="'three'"
          @changeGlobalProcessor="changeGlobalProcessor"
        >
          <template
            v-slot:referFormat
            v-if="configData.chartOption.dataSourceType != 'static'"
          >
            <el-form-item label="参考格式">
              <el-input type="textarea" :rows="5" :value="refData" />
            </el-form-item>
          </template>
          <template v-slot:processorSlot>
            <el-tabs
              v-model="activeProcessor"
              class="processor-tabs"
              editable
              @edit="handleTabsEdit"
              v-if="configData.chartOption.dataSourceType === 'gobal'"
            >
              <el-tab-pane
                v-for="item in processorTabs"
                :key="'tab' + item.name"
                :label="item.title"
                :name="item.name"
              >
                <el-form-item label="绑定数据">
                  <el-select
                    v-model="item.globalData"
                    placeholder="请选择"
                    @change="changeActiveGlobal"
                  >
                    <el-option
                      v-for="(item, idx) in globalArr"
                      :key="idx"
                      :label="item.name"
                      :value="item.name"
                    />
                  </el-select>
                </el-form-item>
                <el-form-item v-if="item && item.globalData" label="过滤器">
                  <el-select
                    v-model="item.globalProcessor"
                    placeholder="请选择"
                  >
                    <el-option
                      v-for="(item, itemKey) in globalProcessor"
                      :key="itemKey"
                      :label="item"
                      :value="item"
                    />
                  </el-select>
                </el-form-item>
              </el-tab-pane>
            </el-tabs>
          </template>
          <template>
            <globalslot
              :costomData="configData"
              ref="globalslot"
              :processorTabs="processorTabs"
              :activeTab="activeTab"
              :themeForm="themeForm"
              :drawingList="drawingList"
              :tableListMap="tableListMap"
              @setTabsData="setTabsData"
              @costom-change="costomChange"
            ></globalslot>
          </template>
        </data-source-config>
      </el-scrollbar>
    </div>
    <!-- 组件位置 -->
    <modulePosition
      v-if="currentTab === 'location'"
      :costomData="configData"
    ></modulePosition>
    <!-- 组件交互 -->
    <el-form
      v-if="currentTab === 'interaction'"
      size="small"
      label-width="110px"
      style="padding: 20px 10px"
    >
      <div
        style="
          background-color: #f5f5f5;
          padding: 10px 15px;
          border: solid 1px #dadada;
          font-size: 14px;
          margin-bottom: 15px;
          color: #999;
        "
      >
        事件预处理
      </div>
      <chart-interact
        :chartOption="configData.chartOption"
        @changeData="changeInteractData"
      ></chart-interact>
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
      defauleValue: [
        {
          id: "1",
          head: {
            headImg:
              "http://221.212.111.73:2080/prod-api/profile/backgroundBox/2022/04/14/d60a075bdd447611d2ac2f48806226cc.png",
            tipText: "迟到1次",
          },
          name: "测试1人员",
          infoItem: [
            { label: "联系电话", value: "13312345678" },
            { label: "工作年限", value: "12年" },
          ],
          detailItem: [
            {
              label: "工作111日报查看",
              address:
                "https://echarts.apache.org/zh/option.html#series-effectScatter.label.show",
            },
            { label: "工作周报查看", address: "" },
          ],
        },
      ],
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
          编码: row.id,
          头像: {
            头像图片: row.head.headImg,
            提示文本: row.head.tipText,
          },
          名称: row.name,
          信息: [],
          详情: [],
        };
        row.infoItem.map((rw) => {
          let obj2 = {
            信息标签: rw.label,
            信息值: rw.value,
          };
          obj["详情"].push(obj2);
        });
        row.detailItem.map((rw) => {
          let obj2 = {
            详情标签: rw.label,
            详情地址: rw.address,
          };
          obj["信息"].push(obj2);
        });
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
              key: "id",
              filed: "",
            },
            {
              key: "headImg",
              filed: "",
              parentIdKey: "head",
            },
            {
              key: "tipText",
              filed: "",
              parentIdKey: "head",
            },
            {
              key: "name",
              filed: "",
            },
            {
              key: "infoItem",
              filed: "",
            },
            {
              key: "detailItem",
              filed: "",
            },
          ];
          let defobj = {
            main: defarr,
            infoItem: [],
            detailItem: [],
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
      this.$set(
        this.configData.chartOption,
        "configProcessorTabs",
        this.processorTabs
      );
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
