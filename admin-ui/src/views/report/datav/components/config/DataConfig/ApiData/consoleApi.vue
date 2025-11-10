<template>
  <div>
    <el-form size="small" label-width="90px">
      <el-form-item label="接口地址">
        <el-input placeholder="请输入接口地址" v-model="apiInterfaceUrl">
          <el-select v-model="apiRequestMethod" style="width: 85px" slot="prepend" placeholder="URL" @change="changeMethod()">
            <el-option label="GET" value="GET"></el-option>
            <el-option label="POST" value="POST"></el-option>
            <el-option label="PUT" value="PUT"></el-option>
            <el-option label="DELETE" value="DELETE"></el-option>
          </el-select>
        </el-input>
      </el-form-item>

      <el-form-item label="请求头">
        <div>
          <el-button type="text" @click="addHeaderItem()">+ 添加</el-button>
        </div>
        <div v-for="(header, index) in apiRequestHeader" :key="'a' + index">
          <el-input placeholder="参数名" size="small" v-model="header.name" style="width: 100px"/>
          <span style="padding: 0 10px">-</span>
          <el-input
            placeholder="请设置字段值"
            size="small"
            v-model="header.value"
            style="width: 300px"
          />
          <el-button style="margin-left: 10px" size="mini" @click="delHeaderItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
        </div>
      </el-form-item>
      <el-form-item label="请求参数">
        <div style="margin-bottom: 15px">
          <el-button style="margin-right: 20px" type="text" @click="addDataItem()">+ 添加</el-button
          >
          <span>参数类型 - </span>
          <el-select v-model="apiRequestParamType" @change="chgParamType" style="width: 85px">
            <el-option label="param" value="PARAM"></el-option>
            <el-option label="json" value="JSON"></el-option>
            <el-option label="form" value="FORM"></el-option>
          </el-select>
        </div>
        <div v-if="apiRequestParamType == 'PARAM'">
          <div v-for="(param, index) in apiRequestParameters" :key="'b' + index" style="margin-bottom: 10px">
            <el-input placeholder="参数名" size="small" style="width: 100px" v-model="param.name"/>
            <span style="padding: 0 10px">-</span>
            <el-autocomplete
              size="small"
              v-model="param.value"
              :fetch-suggestions="querySearch"
              placeholder="请设置字段值"
              @select="handleSelect($event, param.name)"
              style="width: 180px"
            ></el-autocomplete>
            <el-button style="margin-left: 10px" size="mini" @click="delDataItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
          </div>
        </div>
        <div v-else-if="apiRequestParamType == 'JSON'">
          <div style="margin-bottom: 10px">
            普通格式
            <el-switch v-model="apiJsonEdit"> </el-switch>
            代码格式
          </div>
          <template v-if="apiJsonEdit">
            <vue-json-editor v-model="apiRequestJson" :showBtns="false" :mode="'code'" lang="zh"/>
          </template>
          <div style="border-radius: 5px;background-color: #f5f7fa;border: solid 1px #dadada;padding: 20px 10px 10px 10px;" v-else>
            <div v-for="(val, key) in apiRequestJson" :key="key" style="margin-bottom: 10px">
              <span style="width: 100px; display: inline-block; text-align: center">{{ key }}</span>
              <span style="padding: 0 10px">-</span>
              <el-autocomplete
                size="small"
                v-model="apiRequestJson[key]"
                :fetch-suggestions="querySearch"
                placeholder="请设置字段值"
                @select="handleSelect($event, key)"
                style="width: 180px"
              ></el-autocomplete>
              <el-button style="margin-left: 10px" size="mini" @click="delDataItem(key)" type="danger" icon="el-icon-delete" circle></el-button>
            </div>
          </div>
        </div>
        <div v-else-if="apiRequestParamType == 'FORM'">
          <div v-for="(param, index) in apiRequestForm" :key="'c' + index" style="margin-bottom: 10px">
            <el-input placeholder="参数名" size="small" style="width: 100px" v-model="param.name"/>
            <span style="padding: 0 10px">-</span>
            <el-autocomplete size="small" v-model="param.value" :fetch-suggestions="querySearch" placeholder="请设置字段值" @select="handleSelect($event, param.name)" style="width: 180px"
            ></el-autocomplete>
            <el-button style="margin-left: 10px" size="mini" @click="delDataItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
          </div>
        </div>
      </el-form-item>

      <el-form-item label="刷新时间">
        <el-input-number @change="confirmValue" v-model="apiTimeout" :min="0" controls-position="right" :step="1000"></el-input-number>
      </el-form-item>

      <el-form-item label="接口返回">
        <div>
          <div style="margin-bottom: 15px">
            <el-button @click="onRefresh" type="primary" plain>刷新结果</el-button>
          </div>
        </div>
      </el-form-item>
      <tabsData :ref="'tabsData' + curIdx" @baseDataTotal="baseDataTotal" :isShowrequest="true" :globalData="configData.globalData"/>
    </el-form>
  </div>
</template>

<script>
import vueJsonEditor from "vue-json-editor";
import { getLinkChart, replaceLinkParam } from "../../../../util/LinkageChart";
import { chartApi } from "@/api/report/chartApi";
import DataEditor from "../../runcode/DataEditor";
import tabsData from "../DataBaseData/tabsData";
export default {
  components: {
    vueJsonEditor,
    DataEditor,
    tabsData,
  },
  props: ["drawingList", "dialogVisible", "curIdx","tableType","costomData"],
  data() {
    return {
      apiJsonEdit: false,
      apiInterfaceUrl: "", //接口地址
      apiRequestMethod: "GET", //请求方式
      apiRequestHeader: [],
      apiRequestParameters: [], //请求参数
      apiRequestJson: {}, //请求Json
      apiRequestForm: [], //请求Form
      apiRequestParamType: "PARAM",
      apiTimeout: 30, //刷新时间
      apiSourceId: "",
      rawData: "",
      preprocess: {},
      newPreCode: "",
      newPreName: "",
      newRadio: 0,
      numberTypeArr: [], //数字类型组合
      booleanTypeArr: [], //布尔类型组合
      resultDlgOpen: false, //结果处理脚本弹窗
      resultPreCode: "", //结果处理脚本
      resultPreName: "", //结果处理器名称
      resultProcessor: {},
      resultTableList: [], //转化结果的表格数据
      resultDataTable: {}, //多个结果处理器转化结果
      tableLoading: false,
      tableColum: [], //表格的字段和名称对应
      tabActiveName: "", //tab活动页签
      tabParamsList: [], //tab参数列表
      sourceOpenDis: false,
      configData:this.costomData
    };
  },
  computed: {
  },
  mounted() {},
  watch:{
    costomData:{
      handler(newval){
        this.configData=newval
      },
      immediate:true,
      deep:true
    },
  },
  methods: {
    baseDataTotal(data) {
      // console.log("编辑器结果", data);
      this.rawData = JSON.stringify(data);
      this.$emit("changeconfirmValue", true);
    },
    closesource() {
      //关闭接口源
      this.$emit("closesource");
    },
    handleClick() {},
    async getVal() {
      return await this.confirmValue(false);
    },
    initCom(option) {
      this.apiInterfaceUrl = option.interfaceURL;
      this.rawData = option.rawData;
      let rawData = option.rawData || option.rawData !== undefined ? JSON.parse(option.rawData) : [];
      // console.log(rawData,'rawDatarawDatarawData');
      this.$refs["tabsData" + this.curIdx].data = rawData != "" || (rawData && rawData.length > 0) ? [...rawData[0].content] : [];
      this.$refs["tabsData" + this.curIdx].editableTabs = rawData != "" || (rawData && rawData.length > 0) ? [...rawData] : [{ title: "默认数据", content: [], name: "0",cascaderValue:[],resultPrePath:'', resultPreCode: "",selectKeyFiled:'' }];
      this.$refs["tabsData" + this.curIdx].editableTabsValue = rawData != "" || (rawData && rawData.length > 0) ? rawData[0].name : "0";

      if (option.rawData) {
        const maxValue = Math.max(...rawData.map((item) => Number(item.name)));
        
        this.$refs["tabsData" + this.curIdx].tabIndex = maxValue;
      } else {
        this.$refs["tabsData" + this.curIdx].tabIndex = 0;
      }
      this.apiRequestMethod = option.requestMethod;
      this.apiRequestHeader = option.requestHeader;
      let paramsDataObj = JSON.parse(JSON.stringify(option.requestParameters));
      for (let key in paramsDataObj) {
        if (typeof paramsDataObj[key] === "number") {
          this.numberTypeArr.push(key);
          paramsDataObj[key] = String(paramsDataObj[key]);
        }
        if (typeof paramsDataObj[key] === "boolean") {
          this.booleanTypeArr.push(key);
          paramsDataObj[key] = String(paramsDataObj[key]);
        }
      }
      this.apiRequestParameters = paramsDataObj;
      this.apiRequestJson = paramsDataObj;
      this.apiRequestForm = paramsDataObj;
      this.apiRequestParamType = option.requestParamType || "PARAM";
      this.apiTimeout = option.timeout;
      this.preprocess = option.preprocess; //预处理
      this.confirmValue(true);
    },
    onRefresh() {
      this.confirmValue(true);
      this.$message.success("刷新成功");
    },
    chgParamType() {
      this.apiRequestParameters = [];
      this.apiRequestJson = {};
      this.apiRequestForm = [];
    },
    querySearch(queryString, cb) {
      let newArr = []
      if (this.tableType === 'spreadSheet') {
        let searchTableData = JSON.parse(localStorage.getItem("searchTableData"))
        if (searchTableData === null) return
        newArr = searchTableData.map((x) => {
          return { value: x.fieldName, id: x.formatDefault };
        });
      } else {
        let drawArr = getLinkChart(this.drawingList);
        // console.log(drawArr,'drawArr');
        newArr = drawArr.map((x) => {
          // console.log("单个",x);
          if(x.chartType=='timeFrame'){
            return [{ value: x.layerName+'#0', id: x.customId },{ value: x.layerName+'#1', id: x.customId }];
          }else{
            return [{ value: x.layerName, id: x.customId }];
          }
          
        }).flat();
        this.drawingList.map(item => {
          if (item.chartOption.pageSize !==undefined) {
            newArr.push({ value: item.chartOption.pageSize, id: item.customId })
          }
        })
      }
      cb(newArr);
    },
    handleSelect(item, name) {
      if (this.apiRequestParamType == "PARAM") {
          this.apiRequestParameters.forEach((x) => {
            if (x.name == name) {
              // x.value = "@" + item.value;
              if (isNaN(Number(item.value))) {
                x.value = "@" + item.value;
              } else {
                x.value = item.value.toString();
              }
            }
          });
        } else if (this.apiRequestParamType == "JSON") {
          // this.apiRequestJson[name] = "@" + item.value;
          if (isNaN(Number(item.value))) {
            this.apiRequestJson[name] = "@" + item.value;
          } else {
            this.apiRequestJson[name] = item.value.toString();
          }
        } else {
          this.apiRequestForm.forEach((x) => {
            if (x.name == name) {
              if (isNaN(Number(item.value))) {
                x.value = "@" + item.value;
              } else {
                x.value = item.value.toString();
              }
            }
          });
        }
    },
    addHeaderItem() {
      if (this.apiRequestHeader == null) {
        this.apiRequestHeader = [];
      }

      if (
        this.apiRequestHeader.length > 0 &&
        (this.apiRequestHeader[this.apiRequestHeader.length - 1].name.trim() ===
          "" ||
          this.apiRequestHeader[
            this.apiRequestHeader.length - 1
          ].value.trim() === "")
      ) {
        this.$message.warning("请完善之前项后在添加");
        return;
      }
      this.apiRequestHeader.push({ name: "", value: "" });
    },
    delHeaderItem(index) {
      this.apiRequestHeader.splice(index, 1);
    },
    addResultPreItem() {
      if (this.apiInterfaceUrl == "") {
        this.$message.error("请先填写接口地址");
        return null;
      }
      this.resultPreName = "";
      this.resultPreCode = `(input)=>{
            return input;
          }`;
      this.resultDlgOpen = true;
      this.$nextTick(() => {
        this.$refs.resultrefEditor.setVal(this.resultPreCode);
        this.$refs.resultrefEditor.format();
      });
    },
    editResultPreCode(key) {
      this.resultPreName = key;
      this.resultPreCode = this.resultProcessor[key];
      this.resultDlgOpen = true;
      this.$nextTick(() => {
        this.$refs.resultrefEditor.setVal(this.resultPreCode);
        this.$refs.resultrefEditor.format();
      });
    },
    addResultPreOk() {
      if (this.resultPreName == "") {
        this.$message.error("名称不能为空");
        return;
      }
      this.resultPreCode = this.$refs.resultrefEditor.getVal();
      this.$set(this.resultProcessor, this.resultPreName, this.resultPreCode);
      if (!this.tabParamsList.includes(this.resultPreName)) {
        this.tabParamsList.push(this.resultPreName);
      }
      this.confirmValue(true, this.resultPreName);
      this.resultDlgOpen = false;
    },
    delPreItem(key) {
      this.$delete(this.preprocess, key);
    },
    addDataItem() {
      let reqdata;
      if (this.apiRequestParamType == "PARAM") {
        if (this.apiRequestParameters == null) {
          this.apiRequestParameters = [];
        }
        reqdata = this.apiRequestParameters;
      } else if (this.apiRequestParamType == "JSON") {
        this.$prompt("请输入新的参数名", "添加新参数", {
          confirmButtonText: "提交",
          cancelButtonText: "取消",
          inputPattern: /^[A-Za-z0-9\\-\\_]{1,30}$/,
          inputErrorMessage: "参数名不能为空且长度小于30",
          inputPlaceholder: "请输入参数名",
          closeOnClickModal: false,
        }).then(({ value }) => {
          if (this.apiRequestJson == null) {
            this.apiRequestJson = {};
          }

          this.$set(this.apiRequestJson, value, "");
        });
        return;
      } else {
        if (this.apiRequestForm == null) {
          this.apiRequestForm = [];
        }
        reqdata = this.apiRequestForm;
      }

      if (
        reqdata.length > 0 &&
        (reqdata[reqdata.length - 1].name.trim() === "" ||
          reqdata[reqdata.length - 1].value.trim() === "")
      ) {
        this.$message.warning("请完善之前项后在添加");
        return;
      }
      reqdata.push({ name: "", value: "" });
    },
    delDataItem(index) {
      if (this.apiRequestParamType == "PARAM") {
        this.apiRequestParameters.splice(index, 1);
      } else if (this.apiRequestParamType == "JSON") {
        if (this.apiRequestJson.hasOwnProperty(index)) {
          this.$delete(this.apiRequestJson, index);
        } else {
          this.$message.warning("JSON的一级对象不包含" + index);
        }
      } else {
        this.apiRequestForm.splice(index, 1);
      }
    },
    openImport() {
      this.$refs.iptDlg.openImport();
    },
    importApi(data) {
      this.apiInterfaceUrl = data.Url;
      this.apiRequestMethod = data.Method;
      this.apiRequestHeader = data.Header;
      this.apiRequestParamType = data.ParamType;
      this.numberTypeArr = [];
      this.booleanTypeArr = [];
      let paramsDataObj = JSON.parse(JSON.stringify(data.ParamData));
      for (let key in paramsDataObj) {
        if (typeof paramsDataObj[key] === "number") {
          this.numberTypeArr.push(key);
          paramsDataObj[key] = String(paramsDataObj[key]);
        }
        if (typeof paramsDataObj[key] === "boolean") {
          this.booleanTypeArr.push(key);
          paramsDataObj[key] = String(paramsDataObj[key]);
        }
      }
      data.ParamData = JSON.parse(JSON.stringify(paramsDataObj));
      switch (this.apiRequestParamType) {
        case "PARAM":
          this.apiRequestParameters = data.ParamData;
          break;
        case "JSON":
          this.apiRequestJson = data.ParamData;
          break;
        case "FORM":
          this.apiRequestForm = data.ParamData;
          break;
      }
      this.confirmValue(true);
    },
    async confirmValue(enableRes) {
      if (this.apiInterfaceUrl == "") {
        this.rawData = "";
        return null;
      }
      let changeOption = {
        interfaceURL: this.apiInterfaceUrl,
        requestMethod: this.apiRequestMethod,
        requestHeader: this.apiRequestHeader,
        requestParamType: this.apiRequestParamType,
        timeout: this.apiTimeout,
        preprocess: this.preprocess,
      };
      switch (this.apiRequestParamType) {
        case "PARAM":
          let paramsObj1 = JSON.parse(JSON.stringify(this.apiRequestParameters));
          for (let key in paramsObj1) {
            if (this.numberTypeArr.includes(key)) {
              paramsObj1[key] = Number(paramsObj1[key]);
            }
            if (this.booleanTypeArr.includes(key)) {
              if (paramsObj1[key] && paramsObj1[key] == "true") {
                paramsObj1[key] = true;
              } else {
                paramsObj1[key] = false;
              }
            }
          }
          changeOption["requestParameters"] = paramsObj1;
          break;
        case "JSON":
          let paramsObj2 = JSON.parse(JSON.stringify(this.apiRequestJson));
          for (let key in paramsObj2) {
            if (this.numberTypeArr.includes(key)) {
              paramsObj2[key] = Number(paramsObj2[key]);
            }
            if (this.booleanTypeArr.includes(key)) {
              if (paramsObj2[key] && paramsObj2[key] == "true") {
                paramsObj2[key] = true;
              } else {
                paramsObj2[key] = false;
              }
            }
          }
          changeOption["requestParameters"] = paramsObj2;
          break;
        case "FORM":
          let paramsObj3 = JSON.parse(JSON.stringify(this.apiRequestForm));
          for (let key in paramsObj3) {
            if (this.numberTypeArr.includes(key)) {
              paramsObj3[key] = Number(paramsObj3[key]);
            }
            if (this.booleanTypeArr.includes(key)) {
              if (paramsObj3[key] && paramsObj3[key] == "true") {
                paramsObj3[key] = true;
              } else {
                paramsObj3[key] = false;
              }
            }
          }
          changeOption["requestParameters"] = paramsObj3;
          break;
      }
      if (enableRes == true) {
        let newOption = replaceLinkParam(this.drawingList, changeOption,this.tableType);
        // console.log(newOption,'newOptionnewOption111111');
        let resInfo = {};
        try {
          let res = await chartApi(newOption);
        
          if (typeof res !== "string") {
            resInfo = res;
          } else {
            resInfo = JSON.parse(res);
          }
        } catch (error) {
          resInfo={
            data:[]
          }
        }
        // console.log("执行结果",resInfo);
        let apiTableRes=[]
        if (resInfo.data && resInfo.data.List) {
          apiTableRes=JSON.parse(JSON.stringify(resInfo.data.List))
        } else if (resInfo.data) {
          if(Array.isArray(resInfo.data)){
            apiTableRes=JSON.parse(JSON.stringify(resInfo.data))
          }else{
            apiTableRes.push(resInfo.data)
          }
        } else {
          if(resInfo.message){
            this.$message.error(resInfo.message);
            return
          }
          apiTableRes=JSON.parse(JSON.stringify(resInfo))
        }
        if(apiTableRes&&apiTableRes.length>0){
          for(let i=0;i<apiTableRes.length;i++){
            let apiRowTableRes2={}
            for(let key in apiTableRes[i]){
              if(apiTableRes[i][key]&&typeof(apiTableRes[i][key])=='object'&&apiTableRes[i][key]&&apiTableRes[i][key].length==undefined){
                for(let keyN in apiTableRes[i][key]){
                  apiRowTableRes2[key+'_'+keyN]=apiTableRes[i][key][keyN]
                }
              }else{
                apiRowTableRes2[key]=apiTableRes[i][key]
              }
            }
            apiTableRes[i]=JSON.parse(JSON.stringify(apiRowTableRes2))
          }
          this.$refs["tabsData" + this.curIdx].data = [...apiTableRes];
          this.$refs["tabsData" + this.curIdx].editableTabs[0].content = [
            ...apiTableRes,
          ];
        }else{
          this.$refs["tabsData" + this.curIdx].data = [];
          this.$refs["tabsData" + this.curIdx].editableTabs[0].content = [];
        }
        changeOption.rawData = this.rawData;
        if (this.rawData && this.rawData.length > 0) {
          let rawArr = JSON.parse(this.rawData);
          // console.log("一行1111", this.rawData);
          let activName = this.$refs["tabsData" + this.curIdx].editableTabsValue;
          rawArr.map((rw) => {
            if(this.$refs["tabsData" + this.curIdx].cascaderOptions.length==0){
              this.$refs["tabsData" + this.curIdx].getTreeParams(apiTableRes,'')
              this.$refs["tabsData" + this.curIdx].setKeysTree()
            }
            if (rw.resultPreCode) {
              this.$refs["tabsData" + this.curIdx].editableTabsValue = rw.name;
              if (rw.requestType == "0") {
                this.$refs["tabsData" + this.curIdx].submitData(rw.resultPreCode);
              } else {
                this.$refs["tabsData" + this.curIdx].submitData();
              }
            }else{
              this.$nextTick(()=>{
                this.$refs["tabsData" + this.curIdx].editableTabsValue = rw.name;
                this.$refs["tabsData" + this.curIdx].submitData();
              })
            }
            // if(rw.name=='0'&&rw.selectKeyFiled){
            //   this.$refs["tabsData" + this.curIdx].changeFilterFiled(rw.selectKeyFiled)
            // }
            this.$refs["tabsData" + this.curIdx].editableTabsValue = activName;
            // console.log("一行", rw);
          });
        }

        this.$emit("changeconfirmValue", true);
      }
      // console.log(this.rawData,'执行时结果保存');
      changeOption.rawData = this.rawData;
      // console.log(changeOption, "=============changeOption");

      return changeOption;
    },
  },
};
</script>

<style lang="scss" scoped>
/* jsoneditor右上角默认有一个链接,加css去掉了 */
div.jsoneditor-menu a.jsoneditor-poweredBy {
  display: none;
}
::v-deep {
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
