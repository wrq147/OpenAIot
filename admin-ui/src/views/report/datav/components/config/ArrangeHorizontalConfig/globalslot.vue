<template>
  <div>
    <div class="dataOrigin" v-if="configData.chartOption.globalData && !activeTab.globalData">
      请先添加数据源
    </div>
    <div class="dataOrigin"
      v-if="configData.chartOption.globalProcessor && activeTab.globalData && !activeTab.globalProcessor">
      请选择处理器
    </div>
    <div class="dataProduct">1、数据生成</div>
    <div style="margin-bottom: 20px">
      <el-table ref="multipleTable" v-loading="false" border :data="resultTableList" style="width: 100%" :fit="true"
        max-height="500">
        <el-table-column :label="item.name" align="left" :key="item.key" :prop="item.key" :show-overflow-tooltip="true"
          v-for="item in tableColum">
        </el-table-column>
      </el-table>
    </div>
    <div class="dataProduct" v-if="mainTableColum && mainTableColum.length > 0">
      2、数据映射
    </div>
    <div style="margin-bottom: 20px" v-if="mainTableColum && mainTableColum.length > 0">
      <el-form-item>
        <div v-for="(tmpitem, index) in filedSelected.main" :key="'a' + index" style="margin-bottom: 10px">
          <el-select style="width: calc(50% - 10px)" @change="valHasChange" v-model="filedSelected.main[index].key"
            placeholder="请选择" :disabled="true">
            <el-option v-for="item in defaultKeyList" :key="item.key" :label="item.name" :value="item.key">
            </el-option>
          </el-select>
          <span>-</span>
          <el-select :clearable="true" style="width: calc(50% - 10px)" @change="valHasChange"
            v-model="filedSelected.main[index].filed" placeholder="请选择"
            v-if="filedSelected.main[index].key != 'currentList' && filedSelected.main[index].key != 'list' && filedSelected.main[index].key != 'rowtitle'">
            <el-option v-for="item in mainTableColum" :key="item.key" :label="item.key" :value="item.key">
            </el-option>
          </el-select>
          <el-select :clearable="true" style="width: calc(50% - 10px)"
            @change="valHasChangeTable(filedSelected.main[index].key)" v-model="filedSelected.main[index].filed"
            placeholder="请选择" v-else>
            <el-option v-for="item in slectTableList" :key="item.name" :label="item.title" :value="item.name">
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
    <div class="dataProduct" v-if="rowtitleTableColum && rowtitleTableColum.length > 0">
      3、rowtitle数据映射
    </div>
    <div style="margin-bottom: 20px" v-if="rowtitleTableColum && rowtitleTableColum.length > 0">
      <el-form-item>
        <div v-for="(tmpitem, index) in filedSelected.rowtitle" :key="'a' + index" style="margin-bottom: 10px">
          <el-select style="width: calc(50% - 10px)" @change="valHasChange" v-model="filedSelected.rowtitle[index].key"
            placeholder="请选择" :disabled="true">
            <el-option v-for="item in rowtitleDetail" :key="item.key" :label="item.name" :value="item.key">
            </el-option>
          </el-select>
          <span>-</span>
          <el-select style="width: calc(50% - 10px)" @change="valHasChange"
            v-model="filedSelected.rowtitle[index].filed" placeholder="请选择">
            <el-option v-for="item in rowtitleTableColum" :key="item.key" :label="item.key" :value="item.key">
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
    <div class="dataProduct" v-if="infoItemTableColum && infoItemTableColum.length > 0">
      4、currentList数据映射
    </div>
    <div style="margin-bottom: 20px" v-if="infoItemTableColum && infoItemTableColum.length > 0">
      <el-form-item>
        <div v-for="(tmpitem, index) in filedSelected.currentList" :key="'a' + index" style="margin-bottom: 10px">
          <el-select style="width: calc(50% - 10px)" @change="valHasChange"
            v-model="filedSelected.currentList[index].key" placeholder="请选择" :disabled="true">
            <el-option v-for="item in currentList" :key="item.key" :label="item.name" :value="item.key">
            </el-option>
          </el-select>
          <span>-</span>
          <el-select style="width: calc(50% - 10px)" @change="valHasChange"
            v-model="filedSelected.currentList[index].filed" placeholder="请选择">
            <el-option v-for="item in infoItemTableColum" :key="item.key" :label="item.key" :value="item.key">
            </el-option>
          </el-select>
        </div>
      </el-form-item>
    </div>
    <div class="dataProduct" v-if="detailItemTableColum && detailItemTableColum.length > 0">
      5、totalList 数据映射
    </div>
    <div style="margin-bottom: 20px" v-if="detailItemTableColum && detailItemTableColum.length > 0">
      <el-form-item>
        <div v-for="(tmpitem, index) in filedSelected.list" :key="'a' + index" style="margin-bottom: 10px">
          <el-select style="width: calc(50% - 10px)" @change="valHasChange" v-model="filedSelected.list[index].key"
            placeholder="请选择" :disabled="true">
            <el-option v-for="item in totalListInfo" :key="item.key" :label="item.name" :value="item.key"></el-option>
          </el-select>
          <span>-</span>
          <el-select style="width: calc(50% - 10px)" @change="valHasChange" v-model="filedSelected.list[index].filed"
            placeholder="请选择">
            <el-option v-for="item in detailItemTableColum" :key="item.key" :label="item.key" :value="item.key">
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
      filterResultData4: [],
      defaultKeyList: [
        {
          key: "title",
          name: "标题",
        },
        {
          key: "bgstatus",
          name: "背景设置字段",
        },
        {
          key: "rowtitle",
          name: "统计标题",
        },
        {
          key: "currentList",
          name: "数据列表",
        },
        {
          key: "list",
          name: "统计列表",
        },
      ],
      rowtitleDetail: [
        {
          key: "Name",
          name: "标签",
        },
        {
          key: "Code",
          name: "标识符",
        },
        {
          key: "Unit",
          name: "单位",
        },
        {
          key: "Value",
          name: "数值",
        },
      ],
      currentList: [
        {
          key: "Name",
          name: "标签",
        },
        {
          key: "Code",
          name: "标识符",
        },
        {
          key: "Unit",
          name: "单位",
        },
        {
          key: "Value",
          name: "数值",
        },
      ],
      totalListInfo: [
        {
          key: "Name",
          name: "标签",
        },
        {
          key: "Code",
          name: "标识符",
        },
        {
          key: "Unit",
          name: "单位",
        },
        {
          key: "Value",
          name: "数值",
        },
      ],
      slotactiveTab: {},
      infoItemTableColumVal: [],
      detailItemTableColumVal: [],
      mainTableColumVal: [],
      filedSelectedVal: [],
      isUpdatingFromCostomData: false
    };
  },
  //页面加载完执行
  mounted() { },
  watch: {
    resultTableList: {
      deep: true,
      handler(newVal) {
        this.$emit("setTabsData", newVal);
        this.$set(this.detailItemTableColum, this.detailTableReturn());
        this.$set(this.infoItemTableColum, this.infoItemReturn());
        this.$set(this.rowtitleTableColum, this.rowtitleReturn());
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
    filterResultData4: {
      deep: true,
      handler(newVal) {
        this.valHasChange();
      },
    },
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        if (this.isUpdatingFromCostomData) {
          this.isUpdatingFromCostomData = false;
          return;
        }
        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
      },
    },
  },
  computed: {
    filedSelected() {
      if (this.configData.chartOption.tableSelectLine) {
        if (this.processorTabs && this.processorTabs[0]) {
          this.$set(this.configData.chartOption, "globalData", this.processorTabs[0].globalData);
          this.$set(this.configData.chartOption, "globalProcessor", this.processorTabs[0].globalProcessor);
        }
        return this.configData.chartOption.tableSelectLine;
      } else {
        return {
          main: [],
          currentList: [],
          rowtitle: [],
          totalListInfo: [],
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
    rowtitleTableColum: {
      get() {
        return this.rowtitleReturn();
      },
      set(val) {
        //设置了set方法，可直接修改计算属性
        //在这里修改依赖数据
        console.log(val);
      },
    },
    infoItemTableColum: {
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
      if (this.processorTabs[0] == undefined || this.processorTabs[0].globalData == "") {
        return [];
      }
      if (this.processorTabs[0] == undefined || this.processorTabs[0].globalProcessor == "") {
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
    rowtitleReturn() {
      if (this.filedSelected && this.filedSelected.main && this.filedSelected.main.length > 0) {
        let infoItemobj = this.filedSelected.main.find((row) => row.key == "rowtitle");
        if (infoItemobj && infoItemobj.filed) {
          let tabobkTable = this.tableListMap.get(infoItemobj.filed);
          if (tabobkTable && tabobkTable.length > 0) {
            this.filterResultData4 = tabobkTable;
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
    infoItemReturn() {
      if (this.filedSelected && this.filedSelected.main && this.filedSelected.main.length > 0) {
        let infoItemobj = this.filedSelected.main.find((row) => row.key == "currentList");
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
      if (this.filedSelected && this.filedSelected.main && this.filedSelected.main.length > 0) {
        let detailItemobj = this.filedSelected.main.find((row) => row.key == "list");
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
        } else {
          return [];
        }
      } else {
        return [];
      }
    },
    valHasChange(val) {
      //值发生了变化
      let arr = [];
      this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
      this.$set(this.configData.chartOption, "tableSelectLine", this.filedSelected2);
    },
    valHasChangeTable(filed) {
      if (filed) {
        let defobj = {
          main: this.filedSelected.main,
          rowtitle: this.filedSelected.rowtitle,
          currentList: this.filedSelected.currentList,
          list: this.filedSelected.list,
        };
        if (filed == "currentList") {
          let defarr = [
            {
              key: "Name",
              filed: "",
            },
            {
              key: "Code",
              filed: "",
            },
            {
              key: "Unit",
              filed: "",
            },
            {
              key: "Value",
              filed: "",
            },
          ];

          defobj.currentList = defarr;
        } else if (filed == "list") {
          let defarr = [
            {
              key: "Name",
              filed: "",
            },
            {
              key: "Code",
              filed: "",
            },
            {
              key: "Unit",
              filed: "",
            },
            {
              key: "Value",
              filed: "",
            },
          ];
          defobj.list = defarr;
        } else if (filed == "rowtitle") {
          let defarr = [
            {
              key: "Name",
              filed: "",
            },
            {
              key: "Code",
              filed: "",
            },
            {
              key: "Unit",
              filed: "",
            },
            {
              key: "Value",
              filed: "",
            },
          ];
          defobj.rowtitle = defarr;
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
