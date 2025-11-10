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
          @changeSource="changeSource"
          @changeData="changeData"
          @changeGlobalProcessor="changeGlobalProcessor"
          :baseType="''"
        >
          <template v-slot:staticSlot v-if="configData.chartOption.dataSourceType == 'static'">
            <el-form-item></el-form-item>
          </template>
          <template>
            <div style="margin-bottom: 20px">
              <el-table ref="multipleTable" v-loading="false" border:data="resultTableList"
                style="width: 100%" max-height="500" :fit="true"
                @cell-click="(row, column) => cellhandleClick({ column })"
                @header-click="(column) => cellhandleClick({ column })"
                :cell-style="cellStyle" :header-cell-style="headercellStyle"
              >
                <el-table-column
                  :label="item.name"
                  align="left"
                  :key="item.key"
                  :prop="item.key"
                  :class-name="item.current ? 'cellSelected' : ''"
                  :show-overflow-tooltip="true"
                  v-for="item in tableColum"
                >
                  <template #header="data">
                    <div @click="cellhandleClick(data)">{{ item.key }}</div>
                  </template>
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
      filedSelected2: [],
      finalResult: [],
      filterResultData: [],
    };
  },
  watch: {
    resultTableList: {
      deep: true,
      handler(newVal) {
        this.globalChange();
      },
    }
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
        return valueObj[0].content ? valueObj[0].content : [];
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
      if (
        valueObj.length > 0 &&
        valueObj[0].content &&
        valueObj[0].content[0]
      ) {
        let arr = [];
        for (var keys in valueObj[0].content[0]) {
          let issel = this.filedSelected.includes(keys);
          let obj = { key: keys, name: keys, current: issel };
          arr.push(obj);
        }
        return arr;
      } else {
        return [];
      }
    },
  },
  methods: {
    addItem(){
      this.configData.chartOption.staticDataValue.push("");
    },
    delItem(idx){
      this.configData.chartOption.staticDataValue.splice(idx, 1);
    },
    costomChange(newVal) {
      this.configData = newVal;
    },
    headercellStyle({ row, column, rowIndex, columnIndex }) {
      if (this.filedSelected.includes(column.property)) {
        return "border-top:1px solid #2C89E5;border-left:1px solid #2C89E5;border-right:1px solid #2C89E5;color:#2C89E5;";
      }
    },
    cellStyle({ row, column, rowIndex, columnIndex }) {
      let styleStr = "";
      if (this.filedSelected.includes(column.property)) {
        styleStr +=
          "border-left:1px solid #2C89E5;border-right:1px solid #2C89E5;color:#2C89E5;";

        if (rowIndex == this.resultTableList.length - 1) {
          styleStr += "border-bottom:1px solid #2C89E5;";
        }
      }
      return styleStr;
    },
    cellhandleClick({ column }) {
      this.tableColum.map((item) => {
        if (item.key === column.property) {
          this.filedSelected = [item.key];
        }
      });
      this.$set(
        this.configData.chartOption,
        "tableSelectLine",
        this.filedSelected
      );
      let arr = this.resultTableList.map((row) => row[column.property]);
      this.$set(this.configData.chartOption, "finalResult", arr);
      this.finalResult = arr;
    },
    changeGlobalProcessor(val, isType) {
      //用于切换
      // this.configData = val;
      if (isType) {
        //如果是静态数据与动态数据的切换，需要在切换为动态数据时对表格重新赋值
        if (this.filedSelected && this.filedSelected.length > 0) {
          this.$set(
            this.configData.chartOption,
            "finalResult",
            this.finalResult
          );
          this.$set(
            this.configData.chartOption,
            "tableSelectLine",
            this.filedSelected
          );
        } else {
          //如果是数据源或者过滤器的切换需赋值默认值
          this.$set(this.configData.chartOption, "finalResult", [
            "哈哈哈",
            "跑马灯",
          ]);
          this.$set(this.configData.chartOption, "tableSelectLine", []);
        }
      }
    },
    handleClick(val) {
      // console.log("切换tab", this.configData.chartOption.tableSelectLine);
      if (this.currentTab == "data") {
        this.$nextTick(() => {
          if (
            this.resultTableList &&
            this.resultTableList.length > 0 &&
            this.configData.chartOption &&
            this.configData.chartOption.tableSelectLine &&
            this.$refs.multipleTable
          ) {
            //切换到数据这一栏目需要对表格之前所选值赋值
            this.filedSelected = this.configData.chartOption.tableSelectLine;
            this.finalResult = this.configData.chartOption.finalResult;
          }
        });
      }
    },
    globalChange() {
      this.$set(
        this.configData.chartOption,
        "tableSelectLine",
        this.filedSelected
      );
      let arr = this.resultTableList.map((row) => row[this.filedSelected[0]]);
      this.$set(this.configData.chartOption, "finalResult", arr);
      this.finalResult = arr;
      this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
    },
  },
};
</script>

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