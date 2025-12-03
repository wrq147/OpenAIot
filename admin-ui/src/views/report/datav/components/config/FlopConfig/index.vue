<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs" @tab-click="handleClick">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
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
          @changeGlobalProcessor="changeGlobalProcessor"
          :baseType="''"
        >
          <template
            v-slot:staticslot
            v-if="configData.chartOption.dataSourceType == 'static'"
          >
            <el-form-item label="结束值">
              <el-input-number
                controls-position="right"
                v-model="configData.chartOption.staticDataValue"
              ></el-input-number>
            </el-form-item>
          </template>
          <template>
            <div class="dataProduct">1、数据生成</div>
            <div style="margin-bottom: 20px">
              <el-table
                ref="multipleTable"
                v-loading="false"
                border
                :data="resultTableList"
                style="width: 100%"
                :fit="true"
                @selection-change="handleSelectionChange"
                @select="handleSelect"
                @row-click="clickRow"
                row-key="id"
              >
                <el-table-column
                  type="selection"
                  width="50"
                  key="selection"
                  align="center"
                  v-if="tableColum && tableColum.length > 0"
                />
                <el-table-column
                  :label="item.name"
                  align="left"
                  :key="item.key"
                  :prop="item.key"
                  :show-overflow-tooltip="true"
                  v-for="item in tableColum"
                >
                </el-table-column>
              </el-table>
            </div>
          </template>
        </data-source-config>
      </el-scrollbar>
    </div>
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
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    modulePosition,
    moduleDeploy,
  },
  data() {
    return {
      currentTab: "field",
      filedSelected: [], //被选中的字段
      filedSelected2: [], //被选中的字段
      filterResultData: [],
    };
  },
  //页面加载完执行
  mounted() {},
  computed: {
    resultTableList() {
      if (this.configData.chartOption.globalData == "") {
        return [];
      }
      if (this.configData.chartOption.globalProcessor == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.configData.chartOption.globalData
      );
      if (tdlist.length > 0 && this.configData.chartOption.globalProcessor) {
        let dblist = JSON.parse(JSON.stringify(tdlist));
        let tableArr = [];
        if (dblist[0] && dblist[0].rawData) {
        } else {
          return [];
        }
        let dataArr = dblist[0].rawData ? JSON.parse(dblist[0].rawData) : [];
        let valueObj = dataArr.filter(
          (item) => item.title === this.configData.chartOption.globalProcessor
        );
        this.filterResultData = JSON.parse(JSON.stringify(valueObj[0].content));
        if (!valueObj[0] || !valueObj[0].content[0]) {
          return tableArr;
        }
        let ids = 0;
        for (let keys in valueObj[0].content[0]) {
          let obj = {
            id: ids,
            filed: keys,
            value: valueObj[0].content[0][keys],
          };
          tableArr.push(obj);
          ids++;
        }
        return tableArr;
      } else {
        return [];
      }
    },
    tableColum() {
      if (this.configData.chartOption.globalData == "") {
        return [];
      }
      if (this.configData.chartOption.globalProcessor == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.configData.chartOption.globalData
      );
      if (!tdlist[0]) {
        return [];
      }
      let dataArr = tdlist[0].rawData ? JSON.parse(tdlist[0].rawData) : [];
      let valueObj = dataArr.filter(
        (item) => item.title === this.configData.chartOption.globalProcessor
      );
      if (valueObj.length > 0 && valueObj[0].content) {
        let arr = [
          { key: "filed", name: "字段" },
          { key: "value", name: "值" },
        ];
        return arr;
      } else {
        return [];
      }
    },
  },
  methods: {
    changeGlobalProcessor(val, isType) {
      //用于切换
      // this.configData = val;
      if (isType) {
        if (this.filedSelected && this.filedSelected.length > 0) {
          this.$set(
            this.configData.chartOption,
            "tableSelectLine",
            this.filedSelected
          );
          this.$nextTick(() => {
            if (
              this.configData.chartOption &&
              this.configData.chartOption.tableSelectLine &&
              this.$refs.multipleTable
            ) {
              this.resultTableList.map((row) => {
                if (
                  this.configData.chartOption.tableSelectLine.some(
                    (user) => user.id === row.id
                  )
                ) {
                  // 存在添加
                  this.$refs.multipleTable.toggleRowSelection(row, true);
                }
              });
            }
          });
        } else {
          this.$set(this.configData.chartOption, "tableSelectLine", []);
        }
      }
    },
    handleClick(val) {
      if (this.currentTab == "data") {
        let arr = JSON.parse(JSON.stringify(this.configData));
        this.$nextTick(() => {
          if (
            this.configData.chartOption &&
            this.configData.chartOption.tableSelectLine &&
            this.$refs.multipleTable
          ) {
            this.resultTableList.map((row) => {
              if (
                this.configData.chartOption.tableSelectLine.some(
                  (user) => user.id === row.id
                )
              ) {
                // 存在添加
                this.$refs.multipleTable.toggleRowSelection(row, true);
              }
            });
          }
        });
      }
    },
    handleSelect(arr, row) {
      this.ishandleSelect = true;
    },
    handleSelectionChange(val) {
      // 单选
      this.filedSelected = val;
      if (val.length > 1) {
        this.$refs.multipleTable.clearSelection();
        this.$refs.multipleTable.toggleRowSelection(val.pop());
      }
      if (this.filedSelected.length > 0) {
        // if (this.ishandleSelect) {
        let str = 0;
        if (Number(this.filterResultData[0][this.filedSelected[0].filed])) {
          str = Number(this.filterResultData[0][this.filedSelected[0].filed]);
        }

        this.$set(
          this.configData.chartOption,
          "tableSelectLine",
          this.filedSelected
        );
        this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
        this.ishandleSelect = false;
        // }
      } else {
        if (this.ishandleSelect) {
          //判断是否是手动清空选项
          this.$set(this.configData.chartOption, "tableSelectLine", []);
          // this.ishandleSelect = false;
        } else {
          if (this.filedSelected2 && this.filedSelected2.length > 0) {
            this.resultTableList.map((row) => {
              if (this.filedSelected2.some((user) => user.id === row.id)) {
                // 存在添加
                this.$refs.multipleTable.toggleRowSelection(row, true);
              }
            });
          }
        }
      }
      this.ishandleSelect = false;
    },
    // 点击一行时选中
    clickRow(row) {
      this.ishandleSelect = true;
      if (this.filedSelected && this.filedSelected.length > 0) {
        if (this.filedSelected[0] == row) {
          // 取消
          this.$refs.multipleTable.clearSelection();
        } else {
          // 选择
          this.$nextTick(() => {
            this.$refs.multipleTable.clearSelection();
            this.$refs.multipleTable.toggleRowSelection(row, true);
          });
        }
      } else {
        this.$refs.multipleTable.toggleRowSelection(row, true);
      }
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
