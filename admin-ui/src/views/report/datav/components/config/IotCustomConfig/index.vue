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
      :drawingList="drawingList"
      @costom-change="costomChange"
      :filedSelected="filedSelected"
      :resultTableList="resultTableList"
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
          :isConfiguration="true"
          @changeSource="changeSource"
          @changeData="changeData"
          @changeGlobalProcessor="changeGlobalProcessor"
          :baseType="''"
        >
          <template v-slot:staticSlot v-if="configData.chartOption.dataSourceType == 'static'">
            <el-form-item>
              <el-input type="textarea" :rows="5" :value="refData" />
            </el-form-item>
          </template>
          <template>
            <div class="dataProduct">1、数据生成</div>
            <div style="margin-bottom: 20px">
              <el-table ref="multipleTable" v-loading="false" border :data="resultTableList" style="width: 100%" :fit="true" max-height="500">
                <el-table-column :label="item.name" align="left" :key="item.key" :prop="item.key" :show-overflow-tooltip="true" v-for="item in tableColum"></el-table-column>
              </el-table>
            </div>
            <div class="dataProduct" v-if="tableColum && tableColum.length > 0">2、数据映射</div>
            <div style="margin-bottom: 20px" v-if="tableColum && tableColum.length > 0">
              <el-form-item>
                <div v-for="(tmpitem, index) in filedSelected" :key="'a' + index" style="margin-bottom: 10px">
                  <el-select style="width: calc(50% - 14px)" @change="valHasChange" v-model="filedSelected[index].key" placeholder="请选择" :disabled="true">
                    <el-option v-for="item in defaultKeyList" :key="item.key" :label="item.name" :value="item.key"></el-option>
                  </el-select>
                  <span>-</span>
                  <el-select style="width: calc(50% - 14px)" @change="valHasChange" v-model="filedSelected[index].filed" placeholder="请选择" :clearable="true">
                    <el-option v-for="item in tableColum" :key="item.key" :label="item.key" :value="item.key"></el-option>
                  </el-select>
                </div>
              </el-form-item>
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
      // filedSelected: [], //被选中的字段
      filedSelected2: [],
      finalResult: [],
      filterResultData: [],
      defaultKeyList: [
        {
          key: "value",
          name: "值",
        },
        {
          key: "filed",
          name: "字段",
        },{
          key: "filedName",
          name: "字段名称",
        },
      ],
    };
  },
  watch: {
    resultTableList: {
      deep: true,
      handler(newVal) {
        this.valHasChange();
      },
    }
  },
  //页面加载完执行
  mounted() {},
  computed: {
    refData() {
      let chaArr = [];
      this.configData.chartOption.staticDataValue.map((row) => {
        let obj = {
          值: row.value,
          字段: row.filed,
          字段名称: row.filedName,
        };
        chaArr.push(obj);
      });
      return JSON.stringify(chaArr);
    },
    filedSelected() {
      if (this.configData.chartOption.tableSelectLine) {
        return this.configData.chartOption.tableSelectLine;
      } else {
        return [];
      }
    },
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
      if (valueObj.length > 0 && valueObj[0].content) {
        let arr = [];
        for (var keys in valueObj[0].content[0]) {
          let obj = { key: keys, name: keys };
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
    valHasChange(val) {
      //值发生了变化
      // console.log("下拉列表选择的值变量", val,this.filedSelected);
      this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
      this.$set(
        this.configData.chartOption,
        "tableSelectLine",
        this.filedSelected2
      );
    },
    changeGlobalProcessor(val, isType) {
      //用于切换
      // this.configData = val;
      if (isType) {
        if (this.filedSelected && this.filedSelected.length > 0) {
        } else {
          let defarr = [
            {
              key: "filed",
              filed: "",
            },
            {
              key: "filedName",
              filed: "",
            },
            {
              key: "value",
              filed: "",
            },
          ];
          this.$set(this.configData.chartOption, "tableSelectLine", defarr);
        }
      }
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