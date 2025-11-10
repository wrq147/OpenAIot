<template>
  <div class="right-board">
    <el-tabs v-model="currentTab" class="center-tabs">
      <el-tab-pane label="配置" name="field" />
      <el-tab-pane label="数据" name="data" />
      <el-tab-pane label="定位" name="location" />
    </el-tabs>
    <!-- 组件属性 -->
    <moduleDeploy v-if="currentTab === 'field'" :costomData="configData" :tableColum="tableColum" :filedListArr="filedListArr"></moduleDeploy>
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
          <template v-slot:referFormat v-if="configData.chartOption.dataSourceType != 'static'">
            <el-form-item label="参考格式">
              <el-input type="textarea" :rows="5" :value="refData" />
            </el-form-item>
          </template>
          <template>
            <div class="dataProduct">1、数据生成</div>
            <div style="margin-bottom: 20px">
              <!-- <el-table ref="multipleTable" v-loading="false" border :data="resultTableList" style="width: 100%" :fit="true" max-height="500">
                <el-table-column :label="item.name" align="left" :key="item.key" :prop="item.key" :show-overflow-tooltip="true" v-for="item in tableColum"></el-table-column>
              </el-table> -->
              <tablepage :data="resultTableList" :tableColum="tableColum"></tablepage>
            </div>
            <div class="dataProduct" v-if="tableColum && tableColum.length > 0">2、数据映射</div>
            <div style="margin-bottom: 20px" v-if="tableColum && tableColum.length > 0">
              <el-form-item>
                <div v-for="(tmpitem, index) in filedSelected" :key="'a' + index" style="margin-bottom: 10px">
                  <div v-if="tmpitem.type!=='array'&&tmpitem.key!=='jinduType'" v-show="tmpitem.isshow||tmpitem.isshow==undefined">
                    <el-select style="width: calc(50% - 14px)" filterable @change="valHasChange()" v-model="filedSelected[index].key" placeholder="请选择" :disabled="true">
                      <el-option v-for="item in defaultKeyList" :key="item.key" :label="item.name" :value="item.key"></el-option>
                    </el-select>
                    <span>-</span>
                    <el-select filterable style="width: calc(50% - 14px)" @change="valHasChange()" v-model="filedSelected[index].filed" placeholder="请选择" :clearable="true">
                      <el-option v-for="item in tableColum" :key="item.key" :label="item.key" :value="item.key"></el-option>
                    </el-select>
                  </div>
                  <div v-else-if="tmpitem.type!=='array'&&tmpitem.key==='jinduType'" v-show="tmpitem.isshow">
                    <el-select style="width: calc(50% - 14px)" filterable @change="valHasChange()" v-model="filedSelected[index].key" placeholder="请选择" :disabled="true">
                      <el-option v-for="item in defaultKeyList" :key="item.key" :label="item.name" :value="item.key"></el-option>
                    </el-select>
                    <span>-</span>
                    <el-select filterable style="width: calc(50% - 14px)" @change="valHasChange('jindutype',$event)" v-model="filedSelected[index].filed" placeholder="请选择" :clearable="true">
                      <el-option label="两个值计算" value="2"></el-option>
                      <el-option label="三个值计算" value="3"></el-option>
                    </el-select>
                  </div>
                  <div v-else v-show="tmpitem.isshow||tmpitem.isshow==undefined">
                    <div>
                      <span style="margin-right: 10px">文本内容对应字段</span>
                    </div>
                    <div style="margin-top: -10px">
                      <el-button type="text" @click="addItem(index)">+ 添加</el-button>
                    </div>
                    <div v-for="(tmpitem, inx) in filedSelected[index].filed" :key="index+'a' + inx" style="margin-bottom: 10px;display:flex;align-items:center;">
                      <el-select filterable style="width: calc(100% - 54px)" @change="valHasChange()" v-model="filedSelected[index].filed[inx]" placeholder="请选择" :clearable="true">
                        <el-option v-for="item in tableColum" :key="item.key" :label="item.key" :value="item.key"></el-option>
                      </el-select>
                      <el-button style="margin-left: 10px" size="mini" @click="delItem(index,inx)" type="danger" icon="el-icon-delete" circle></el-button>
                    </div>
                  </div>
                </div>
              </el-form-item>
            </div>
          </template>
        </data-source-config>
      </el-scrollbar>
    </div>
    <!-- 组件位置 -->
    <modulePosition v-if="currentTab === 'location'" :costomData="configData"></modulePosition>
  </div>
</template>

<script>
import DataSourceConfig from "../DataConfig/DataSourceConfig";
import sourceConfig from "../../mixins/sourceConfig.js";
import modulePosition from "./modulePosition";
import moduleDeploy from "./moduleDeploy";
import tablepage from "./tablepage";
export default {
  mixins: [sourceConfig],
  components: {
    DataSourceConfig,
    modulePosition,
    moduleDeploy,
    tablepage
  },
  data() {
    return {
      currentTab: "field",
      // filedSelected: [], //被选中的字段
      filedSelected2: [],
      filterResultData: [],
      defauleValue: [
        { value: "335", name: "数据1", suffix: "单位1" },
        { value: "310", name: "数据2", suffix: "单位2" },
        { value: "234", name: "数据3", suffix: "单位3" },
        { value: "135", name: "数据4", suffix: "单位4" },
        { value: "135", name: "数据5", suffix: "单位5" },
        { value: "1,548", name: "数据6", suffix: "单位6" },
      ],
      defaultKeyList: [
        {
          key: "jinduType",
          name: "进度计算方式",
        },
        {
          key: "bgstatus",
          name: "内容背景设置字段",
        },
        {
          key: "borderstatus",
          name: "背景边框设置字段",
        },
        {
          key: "name",
          name: "名称",
        },
        {
          key: "status",
          name: "状态",
        },
        {
          key: "statusBg",
          name: "状态背景",
        },
        {
          key: "statusColor",
          name: "状态字体颜色",
        },
        {
          key: "statusBorder",
          name: "状态边框",
        },
        {
          key: "jindubgstatus",
          name: "进度背景颜色设置",
        },
        {
          key: "jindubarstatus",
          name: "进度条颜色设置",
        },
        {
          key: "jinduTextstatus",
          name: "进度条文字颜色设置",
        },
        {
          key: "listTextstatus",
          name: "内容颜色设置",
        },
        {
          key: "barStart",
          name: "进度条开始",
        },
        {
          key: "barEnd",
          name: "进度条结束",
        },
        {
          key: "barTotalValue",
          name: "进度条总值",
        },
        {
          key: "barNow",
          name: "进度条当前值",
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
    filedListArr(){
      //array类型的数据单独设置样式
      let selArr=this.configData.chartOption.tableSelectLine.find(row=>row.key=='list')
      if(selArr){
        return selArr.filed
      }else{
        return []
      }
      
    },
    refData() {
      let chaArr = [];
      this.configData.chartOption.staticDataValue.map((row) => {
        let obj = {
          名称: row.name,
          列表: row.list,
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
    addItem(index) {
      this.filedSelected[index].filed.push('')
    },
    delItem(index,idx) {
      this.filedSelected[index].filed.splice(idx, 1);
    },
    valHasChange(type,val) {
      //值发生了变化
      // console.log("下拉列表选择的值变量", val,this.filedSelected);
      if(type&&type=='jindutype'){
        let obj=this.configData.chartOption.tableSelectLine.find(row=>row.key=='jinduType')
        if(obj&&obj.filed=='2'){
          this.configData.chartOption.tableSelectLine=this.configData.chartOption.tableSelectLine.map(row=>{
            if(row.key=='barStart'||row.key=='barEnd'){
              row.isshow=false
            }else {
              row.isshow=true
            }
            return row
          })
        }else if(obj&&obj.filed=='3'){
          this.configData.chartOption.tableSelectLine=this.configData.chartOption.tableSelectLine.map(row=>{
            if(row.key=='barTotalValue'){
              row.isshow=false
            }else {
              row.isshow=true
            }
            return row
          })
        }
      }
      this.filedSelected2 = JSON.parse(JSON.stringify(this.filedSelected));
      this.$set(this.configData.chartOption,"tableSelectLine",this.filedSelected2);
    },
    changeGlobalProcessor(val, isType) {
      //用于切换
      // this.configData = val;
      if (isType) {
        if (this.filedSelected && this.filedSelected.length > 0) {
        } else {
          let defarr = [
            {key: "jinduType",filed: "2",type:'string',isshow:true},
            {key: "name",filed: "",type:'string',isshow:true},
            {key: "status",filed: "",type:'string',isshow:true},
            {key: "statusBg",filed: "",type:'string',isshow:true},
            {key: "statusColor",filed: "",type:'string',isshow:true},
            {key: "statusBorder",filed: "",type:'string',isshow:true},
            {key: "bgstatus",filed: "",type:'string',isshow:true},
            {key: "borderstatus",filed: "",type:'string',isshow:true},
            {key: "jindubgstatus",filed: "",type:'string',isshow:true},
            {key: "jindubarstatus",filed: "",type:'string',isshow:true},
            {key: "jinduTextstatus",filed: "",type:'string',isshow:true},
            {key: "barStart",filed: "",type:'string',isshow:false},
            {key: "barEnd",filed: "",type:'string',isshow:false},
            {key: "barTotalValue",filed: "",type:'string',isshow:true},
            {key: "barNow",filed: "",type:'string',isshow:true},
            {key: "listTextstatus",filed: "",type:'string',isshow:true},
            {key: "list",filed: [],type:'array',isshow:true},
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
