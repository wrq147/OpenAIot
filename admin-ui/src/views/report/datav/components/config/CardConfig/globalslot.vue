<template>
  <div>
    <div
      class="dataOrigin"
      v-if="configData.chartOption.globalData && !activeTab.globalData"
    >
      请先添加数据源
    </div>
    <div
      class="dataOrigin"
      v-if="
        configData.chartOption.globalProcessor &&
        activeTab.globalData &&
        !activeTab.globalProcessor
      "
    >
      请选择处理器
    </div>
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
    <div class="dataProduct" v-if="mainTableColum && mainTableColum.length > 0">
      2、数据映射
    </div>
    <div
      style="margin-bottom: 20px"
      v-if="mainTableColum && mainTableColum.length > 0"
    >
      <el-form-item>
        <div
          v-for="(tmpitem, index) in filedSelected.main"
          :key="'a' + index"
          style="margin-bottom: 10px"
        >
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChange"
            v-model="filedSelected.main[index].key"
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
            @change="valHasChange"
            v-model="filedSelected.main[index].filed"
            placeholder="请选择"
            v-if="
              filedSelected.main[index].key != 'infoItem' &&
              filedSelected.main[index].key != 'detailItem'
            "
          >
            <el-option
              v-for="item in mainTableColum"
              :key="item.key"
              :label="item.key"
              :value="item.key"
            >
            </el-option>
          </el-select>
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChangeTable(filedSelected.main[index].key)"
            v-model="filedSelected.main[index].filed"
            placeholder="请选择"
            v-else
          >
            <el-option
              v-for="item in slectTableList"
              :key="item.name"
              :label="item.title"
              :value="item.name"
            >
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
    <div
      class="dataProduct"
      v-if="infoItemTableColum && infoItemTableColum.length > 0"
    >
      3、infoItem数据映射
    </div>
    <div
      style="margin-bottom: 20px"
      v-if="infoItemTableColum && infoItemTableColum.length > 0"
    >
      <el-form-item>
        <div
          v-for="(tmpitem, index) in filedSelected.infoItem"
          :key="'a' + index"
          style="margin-bottom: 10px"
        >
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChange"
            v-model="filedSelected.infoItem[index].key"
            placeholder="请选择"
            :disabled="true"
          >
            <el-option
              v-for="item in defaultInfo"
              :key="item.key"
              :label="item.name"
              :value="item.key"
            >
            </el-option>
          </el-select>
          <span>-</span>
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChange"
            v-model="filedSelected.infoItem[index].filed"
            placeholder="请选择"
          >
            <el-option
              v-for="item in infoItemTableColum"
              :key="item.key"
              :label="item.key"
              :value="item.key"
            >
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
    <div
      class="dataProduct"
      v-if="detailItemTableColum && detailItemTableColum.length > 0"
    >
      4、detailItem 数据映射
    </div>
    <div
      style="margin-bottom: 20px"
      v-if="detailItemTableColum && detailItemTableColum.length > 0"
    >
      <el-form-item>
        <div
          v-for="(tmpitem, index) in filedSelected.detailItem"
          :key="'a' + index"
          style="margin-bottom: 10px"
        >
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChange"
            v-model="filedSelected.detailItem[index].key"
            placeholder="请选择"
            :disabled="true"
          >
            <el-option
              v-for="item in defaultDetail"
              :key="item.key"
              :label="item.name"
              :value="item.key"
            >
            </el-option>
          </el-select>
          <span>-</span>
          <el-select
            style="width: calc(50% - 10px)"
            @change="valHasChange"
            v-model="filedSelected.detailItem[index].filed"
            placeholder="请选择"
          >
            <el-option
              v-for="item in detailItemTableColum"
              :key="item.key"
              :label="item.key"
              :value="item.key"
            >
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
  </div>
</template>

<script>
export default {
  props: [
    "costomData",
    "processorTabs",
    "activeTab",
    "drawingList",
    "themeForm",
    "tableListMap",
  ],
  data() {
    return {
      configData: this.costomData,
      filedSelected2: [],
      filterResultData1: [],
      filterResultData: [],
      filterResultData2: [],
      filterResultData3: [],
      defaultKeyList: [
        {
          key: "id",
          name: "编码",
        },
        {
          key: "headImg",
          name: "头像图片",
        },
        {
          key: "tipText",
          name: "提示文本",
        },
        {
          key: "name",
          name: "名称",
        },
        {
          key: "infoItem",
          name: "信息对应数据",
        },
        {
          key: "detailItem",
          name: "详情对应数据",
        },
      ],
      defaultDetail: [
        {
          key: "label",
          name: "详情标签",
        },
        {
          key: "address",
          name: "详情地址",
        },
      ],
      defaultInfo: [
        {
          key: "label",
          name: "信息标签",
        },
        {
          key: "value",
          name: "信息值",
        },
      ],
      slotactiveTab: {},
      infoItemTableColumVal: [],
      detailItemTableColumVal: [],
      mainTableColumVal: [],
      filedSelectedVal: [],
    };
  },
  //页面加载完执行
  mounted() {},
  watch: {
    resultTableList: {
      deep: true,
      handler(newVal) {
        this.$emit("setTabsData", newVal);
        this.$set(this.detailItemTableColum, this.detailTableReturn());
        this.$set(this.infoItemTableColum, this.infoItemReturn());
      },
    },
    filterResultData1: {
      deep: true,
      handler(newVal) {
        this.valHasChange();
      },
    },
    filterResultData2: {
      deep: true,
      handler(newVal) {
        this.valHasChange();
      },
    },
    filterResultData3: {
      deep: true,
      handler(newVal) {
        this.valHasChange();
      },
    },
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.configData = newVal;
      },
    },
  },
  computed: {
    filedSelected() {
      if (this.configData.chartOption.tableSelectLine) {
        if (this.processorTabs && this.processorTabs[0]) {
          this.$set(this.configData.chartOption,"globalData",this.processorTabs[0].globalData);
          this.$set(this.configData.chartOption,"globalProcessor",this.processorTabs[0].globalProcessor);
        }
        return this.configData.chartOption.tableSelectLine;
      } else {
        return {
          main: [],
          infoItem: [],
          detailItem: [],
        };
      }
    },
    resultTableList() {
      if (!this.activeTab || this.activeTab.globalData == "") {
        return [];
      }
      if (!this.activeTab || this.activeTab.globalProcessor == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter(
        (x) => x.name == this.activeTab.globalData
      );
      if (tdlist.length > 0 && this.activeTab.globalProcessor) {
        let dblist = JSON.parse(JSON.stringify(tdlist));
        let tableArr = [];
        if (dblist[0] && dblist[0].rawData) {
        } else {
          return [];
        }
        let dataArr = dblist[0].rawData ? JSON.parse(dblist[0].rawData) : [];
        let valueObj = dataArr.filter((item) => item.title === this.activeTab.globalProcessor);
        this.filterResultData = JSON.parse(JSON.stringify(valueObj[0].content));
        if (!valueObj[0] || !valueObj[0].content[0]) {
          return tableArr;
        }
        this.$emit("setTabsData", valueObj[0].content);
        return valueObj[0].content;
      } else {
        return [];
      }
    },
    infoItemTableColum:{
      get() {
        return this.infoItemReturn();
      },
      set(val) {
        //设置了set方法，可直接修改计算属性
        //在这里修改依赖数据
        console.log(val);
      },
    },
    detailItemTableColum: {
      get() {
        return this.detailTableReturn();
      },
      set(val) {
        //设置了set方法，可直接修改计算属性
        //在这里修改依赖数据
        console.log(val);
      },
    },
    mainTableColum() {
      if(this.processorTabs[0] == undefined ||this.processorTabs[0].globalData == "") {
        return [];
      }
      if(this.processorTabs[0] == undefined || this.processorTabs[0].globalProcessor == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter((x) => x.name == this.processorTabs[0].globalData);
      if (!tdlist[0]) {
        return [];
      }
      let dataArr = tdlist[0].rawData ? JSON.parse(tdlist[0].rawData) : [];
      let valueObj = dataArr.filter((item) => item.title === this.processorTabs[0].globalProcessor);
      if (valueObj.length > 0 && valueObj[0].content) {
        this.filterResultData1 = valueObj[0].content;
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
    tableColum() {
      if (!this.activeTab || this.activeTab.globalData == "") {
        return [];
      }
      if (!this.activeTab || this.activeTab.globalProcessor == "") {
        return [];
      }
      let tdlist = this.themeForm.globalData.filter((x) => x.name == this.activeTab.globalData);
      if (!tdlist[0]) {
        return [];
      }
      let dataArr = tdlist[0].rawData ? JSON.parse(tdlist[0].rawData) : [];
      let valueObj = dataArr.filter((item) => item.title === this.activeTab.globalProcessor);
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
    slectTableList() {
      if (this.processorTabs && this.processorTabs.length > 0) {
        return this.processorTabs;
      } else {
        return [];
      }
    },
  },
  methods: {
    infoItemReturn() {
      if (this.filedSelected &&this.filedSelected.main &&this.filedSelected.main.length > 0) {
        let infoItemobj = this.filedSelected.main.find((row) => row.key == "infoItem");
        if (infoItemobj && infoItemobj.filed) {
          let tabobkTable = this.tableListMap.get(infoItemobj.filed);
          if (tabobkTable && tabobkTable.length > 0) {
            this.filterResultData2 = tabobkTable;
            let arr = [];
            for (var keys in tabobkTable[0]) {
              let obj = { key: keys, name: keys };
              arr.push(obj);
            }
            return arr;
          } else {
            return [];
          }
        } else {
          return [];
        }
      } else {
        return [];
      }
    },
    detailTableReturn() {
      if (this.filedSelected &&this.filedSelected.main &&this.filedSelected.main.length > 0) {
        let detailItemobj = this.filedSelected.main.find((row) => row.key == "detailItem");
        if (detailItemobj && detailItemobj.filed) {
          let tabobkTable = this.tableListMap.get(detailItemobj.filed);
          if (tabobkTable && tabobkTable.length > 0) {
            let arr = [];
            this.filterResultData3 = tabobkTable;
            for (var keys in tabobkTable[0]) {
              let obj = { key: keys, name: keys };
              arr.push(obj);
            }
            return arr;
          } else {
            return [];
          }
        }else{
          return [];
        }
      } else {
        return [];
      }
    },
    valHasChange(val) {
      //值发生了变化
      let arr = [];
      // console.log(this.filterResultData1,'valHasChange');
      this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
      this.$set(
        this.configData.chartOption,
        "tableSelectLine",
        this.filedSelected2
      );
    },
    valHasChangeTable(filed) {
      if (filed) {
        let defobj = {
          main: this.filedSelected.main,
          infoItem: this.filedSelected.infoItem,
          detailItem: this.filedSelected.detailItem,
        };
        if (filed == "infoItem") {
          let defarr = [
            {
              key: "label",
              filed: "",
            },
            {
              key: "value",
              filed: "",
            },
          ];

          defobj.infoItem = defarr;
        } else if (filed == "detailItem") {
          let defarr = [
            {
              key: "label",
              filed: "",
            },
            {
              key: "address",
              filed: "",
            },
          ];
          defobj.detailItem = defarr;
        }
        this.$set(this.configData.chartOption, "tableSelectLine", defobj);
      }
    },
  },
};
</script>

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
