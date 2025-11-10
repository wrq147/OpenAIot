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
            v-slot:referFormat
            v-if="configData.chartOption.dataSourceType != 'static'"
          >
            <el-form-item label="参考格式">
              <el-input type="textarea" :rows="5" :value="refData" />
            </el-form-item>
          </template>
          <template
            v-slot:staticSlot
            v-if="configData.chartOption.dataSourceType == 'static'"
          >
            <!-- 静态样式 -->
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
                max-height="500"
              >
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
            <div class="dataProduct" v-if="tableColum && tableColum.length > 0">
              2、数据映射
            </div>
            <div
              style="margin-bottom: 20px"
              v-if="tableColum && tableColum.length > 0"
            >
              <el-form-item>
                <div
                  v-for="(tmpitem, index) in filedSelected"
                  :key="'a' + index"
                  style="margin-bottom: 10px"
                >
                  <el-select
                    style="width: calc(50% - 10px)"
                    @change="valHasChange"
                    v-model="filedSelected[index].key"
                    placeholder="请选择"
                    :disabled="true"
                  >
                    <el-option
                      v-for="item in defaultKeyList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.key"
                    >
                    </el-option>
                  </el-select>
                  <span>-</span>
                  <el-select
                    style="width: calc(50% - 10px)"
                    @change="valHasChange(index, 'filed')"
                    v-model="filedSelected[index].filed"
                    placeholder="请选择"
                  >
                    <template v-for="item in tableColum">
                      <el-option
                        :key="item.key"
                        :label="item.key"
                        :value="item.key"
                        v-if="
                          (filedSelected[index].key &&
                            filedSelected[index].key != 'children') ||
                          (filedSelected[index].key &&
                            filedSelected[index].key == 'children' &&
                            resultTableList[0] &&
                            resultTableList[0][item.key] &&
                            typeof resultTableList[0][item.key] === 'array') ||
                          (filedSelected[index].key &&
                            filedSelected[index].key == 'children' &&
                            resultTableList[0] &&
                            resultTableList[0][item.key] &&
                            typeof resultTableList[0][item.key] === 'object' &&
                            resultTableList[0][item.key].length != undefined &&
                            resultTableList[0][item.key].length != null)
                        "
                      >
                      </el-option>
                    </template>
                  </el-select>
                </div>
              </el-form-item>
            </div>
          </template>
        </data-source-config>
      </el-scrollbar>
    </div>
    <!-- 组件交互 -->
    <interaction
      @costom-change="costomChange"
      :costomData="configData"
      :themeForm="themeForm"
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
import interaction from "./interaction";
import moduleDeploy from "./moduleDeploy";
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
      // filedSelected: [], //被选中的字段
      filedSelected2: [],
      filterResultData: [],
      defauleValue: [
        { label: "北京", value: 273 },
        { label: "上海", value: 265 },
        { label: "深圳", value: 200 },
        { label: "重庆", value: 150 },
        { label: "天津", value: 300 },
        { label: "哈尔滨", value: 205 },
      ],
      defaultKeyList: [
        {
          key: "value",
          name: "值",
        },
        {
          key: "label",
          name: "标签",
        },
        {
          key: "children",
          name: "子集",
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
    },
  },
  //页面加载完执行
  mounted() {},
  computed: {
    refData(){
      let arr=this.setrefData(this.configData.chartOption.staticDataValue)
      return JSON.stringify(arr)
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
        return valueObj[0].content;
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
    setrefData(arr){
      let chaArr=[]
      for(let i=0;i<arr.length;i++){
        let row=arr[i]
        let obj={
          "值":row.value,
          "标签":row.label
        }
        if(row.children){
          obj["子集"]=this.setrefData(row.children)
        }
        chaArr.push(obj)
      }
      return chaArr
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
              key: "label",
              filed: "",
            },
            {
              key: "value",
              filed: "",
            },
            {
              key: "children",
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
</style>
