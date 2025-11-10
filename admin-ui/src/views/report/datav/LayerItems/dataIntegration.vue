<template>
  <div>
    <el-form size="small" label-width="90px">
      <div class="data-title">
        <el-dropdown trigger="click" @command="onNewData" style="width: 100%">
          <div
            style="
              color: #3572ff;
              cursor: pointer;
              text-align: center;
              width: 100%;
            "
          >
            <i class="el-icon-plus"></i>
            <span style="margin-left: 6px; margin-right: 6px">新增</span>
            <!-- <i class="el-icon-arrow-down"></i> -->
          </div>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item command="url">接口源</el-dropdown-item>
            <el-dropdown-item command="database">数据库</el-dropdown-item>
            <!-- v-if="tableType!=='spreadSheet'" -->
            <el-dropdown-item command="combination"
              >组合数据集</el-dropdown-item
            >
          </el-dropdown-menu>
        </el-dropdown>
      </div>
      <div class="data-bd">
        <el-row class="rw">
          <el-col :span="8" class="t"> 名称 </el-col>
          <el-col :span="8" class="t"> 类型 </el-col>
          <el-col :span="8" class="t"> 操作 </el-col>
        </el-row>
        <el-row
          style="margin-bottom: 15px"
          v-for="(dataoption, idx) in themeForm.globalData"
          :key="idx"
        >
          <el-col :span="8">
            <el-input style="width: 120%;" v-model="dataoption.name" placeholder="请输入名称" />
          </el-col>
          <el-col :span="8" class="tp">
            {{ soureType2Name(dataoption.dataSourceType) }}
          </el-col>
          <el-col :span="8" class="op">
            <el-button
              size="mini"
              @click="editDataItem(idx)"
              icon="el-icon-edit"
              circle
            />
            <el-button
              style="margin-left: 10px"
              size="mini"
              @click="delDataItem(idx)"
              type="danger"
              icon="el-icon-delete"
              circle
            />
          </el-col>
        </el-row>
      </div>
    </el-form>
    <api-data
      ref="apiRef"
      title="编辑数据集"
      :dialog-visible="sourceOpen"
      :drawingList="drawingList"
      :curIdx="curIdx"
      :tableType="tableType"
      :costomData="themeForm"
      @changeconfirmValue="confirmValue"
      @closesource="cancelForm"
    />
    <data-base-data
      ref="apiBaseData"
      :title="'编辑数据库'"
      :dialog-visible="databaseFlag"
      :drawingList="drawingList"
      :tableType="tableType"
      :costomData="themeForm"
      @changeDataBase="changeDataBase"
      @cancelForm="cancelForm"
    />
    <combinationData
      ref="combinationRef"
      title="编辑组合数据集"
      :dialog-visible="combinationOpen"
      :drawingList="drawingList"
      :costomData="themeForm"
      :curIdx="curIdx"
      @changeconfirmValue="changeCombination"
      @closesource="cancelForm"
    />
  </div>
</template>
<script>
import combinationData from "../components/config/DataConfig/combinationData/index.vue";
import ApiData from "../components/config/DataConfig/ApiData/index.vue";
import DataBaseData from "../components/config/DataConfig/DataBaseData/index";
import VueEvent from "../VueEvent";
export default {
  props: {
    themeForm: {
      type: Object,
    },
    drawingList: {
      type: Array,
    },
    tableType: {
      type: String,
      default: "default",
    }
  },
  components: {
    combinationData,
    ApiData,
    DataBaseData,
  },
  computed: {
    curOption() {
      // console.log(this.curIdx,'this.curIdxthis.curIdx');
      if (this.curIdx == null) {
        return null;
      }
      return this.themeForm.globalData[this.curIdx];
    },
  },
  data() {
    return {
      combinationOpen: false,
      sourceOpen: false,
      databaseFlag: false,
      curIdx: null,
    };
  },
  watch: {
    "themeForm.globalData": {
      handler(val) {
        // console.log("总体数据发生变化",val);
        VueEvent.$emit("refreshGlobal",this.curOption ? this.curOption.name : "",true);
      },
    },
  },
  methods: {
    retunGlobalDataName(command,maxNum){
      let name=this.soureType2Name(command) + (maxNum + 1);
      if(this.themeForm.globalData&&this.themeForm.globalData.length>0){
        let globalData=JSON.parse(JSON.stringify(this.themeForm.globalData))
        let obj=globalData.find(row=>row.name==name)
        if(obj){
          return this.retunGlobalDataName(command,maxNum+1)
        }else{
          return name
        }
      }else{
        return name
      }
    },
    onNewData(command) {
      let maxNum=this.themeForm?(this.themeForm.globalData?this.themeForm.globalData.length:0):0
      let tmpname=this.retunGlobalDataName(command,maxNum);
      this.themeForm.globalData.push({
        name: tmpname,
        preprocess: {},
        dataSourceType: command,
        interfaceURL: "",
        requestMethod: "GET",
        requestParameters: [],
        combinationTable: [],
        database: {
          type: "mysql",
          ipAdress: "",
          port: "",
          baseName: "",
          username: "",
          password: "",
        },
        timeout: 30,
        combType: "0",
      });
      // this.curIdx = this.themeForm.globalData.length -1
      // switch (command) {
      //   case "url":
      //     return this.sourceOpen = true;
      //   case "database":
      //     return this.databaseFlag = true;
      //   case "combination":
      //     return this.combinationOpen = true;
      // }
      this.editDataItem(this.themeForm.globalData.length -1)
    },
    soureType2Name(t) {
      switch (t) {
        case "url":
          return "接口源";
        case "database":
          return "数据库";
        case "combination":
          return "组合数据集";
      }
    },
    editDataItem(idx) {
      this.curIdx = idx;
      let tmpoption = this.themeForm.globalData[this.curIdx];
      let ddtype = tmpoption.dataSourceType;
      // console.log(ddtype, "ddtypeddtype",idx);
      if (ddtype === "url") {
        this.sourceOpen = true;
        this.$nextTick(() => {
          this.$refs.apiRef.initCom(tmpoption);
        });
      } else if (ddtype === "database") {
        this.databaseFlag = true;
        this.$nextTick(() => {
          this.$refs.apiBaseData.initCom(tmpoption);
        });
      } else if (ddtype === "combination") {
        this.combinationOpen = true;
        this.$nextTick(() => {
          this.$refs.combinationRef.initCom(tmpoption);
        });
      }
    },
    changeDataBase(val) {
      let tmpdata = this.themeForm.globalData[this.curIdx];
      tmpdata.database = val.database;
      tmpdata.rawData = JSON.stringify(val.data);
      tmpdata.variableList = JSON.stringify(val.variableList);
      tmpdata.timeout=val.timeout
      this.databaseFlag = false;
    },
    async changeCombination(noclose) {
      let curData = null;
      if (this.themeForm.globalData[this.curIdx].dataSourceType == "combination") {
        curData = await this.$refs.combinationRef.getVal();
      }
      if (curData == null) {
        return;
      }
      let tmpdata = this.themeForm.globalData[this.curIdx];
      tmpdata.combinationTable = curData.tableColum;
      tmpdata.rawData = curData.rawData;
      tmpdata.timeout = curData.timeout;
      tmpdata.combType = curData.combType;
      this.$set(this.themeForm.globalData, this.curIdx, tmpdata);
      // console.log(this.themeForm.globalData[this.curIdx], "最后保存的数据");
      if (noclose == true) {
      } else {
        this.combinationOpen = false;
      }
    },
    async confirmValue(noclose) {
      let curData = null;
      if (this.themeForm.globalData[this.curIdx].dataSourceType == "url") {
        curData = await this.$refs.apiRef.getVal();
      }
      if (curData == null) {
        return;
      }
      let tmpdata = this.themeForm.globalData[this.curIdx];
      tmpdata.interfaceURL = curData.interfaceURL;
      tmpdata.requestMethod = curData.requestMethod;
      tmpdata.requestHeader = curData.requestHeader;
      tmpdata.requestParamType = curData.requestParamType;
      tmpdata.requestParameters = curData.requestParameters;
      tmpdata.rawData = curData.rawData;
      tmpdata.timeout = curData.timeout;
      tmpdata.preprocess = curData.preprocess; //预处理
      // tmpdata.resultDataTable = curData.resultDataTable;
      // tmpdata.resultProcessor = curData.resultProcessor;

      this.$set(this.themeForm.globalData, this.curIdx, tmpdata);
      if (noclose == true) {
      } else {
        this.sourceOpen = false;
      }
    },
    delDataItem(idx) {
      this.$confirm('此操作将永久删除该数据源, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.$delete(this.themeForm.globalData, idx);
        // console.log("删除成功了",this.themeForm.globalData);
      }).catch(() => {})
    },
    cancelForm() {
      this.sourceOpen = false;
      this.databaseFlag = false;
      this.combinationOpen = false;
    },
  },
};
</script>
<style lang="scss" scoped>
.data-title {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
  display: flex;
  justify-content: space-between;
  height: 45px;
  align-items: center;

  .fz {
    font-size: 14px;
  }
}
.data-bd {
  .rw {
    margin-bottom: 15px;
  }

  .t {
    font-size: 12px;
    color: #999;
    text-align: center;
  }

  .tp {
    display: flex;
    align-items: center;
    font-size: 12px;
    height: 40px;
    justify-content: center;
  }

  .op {
    display: flex;
    align-items: center;
    height: 40px;
    justify-content: center;
  }
}
</style>
