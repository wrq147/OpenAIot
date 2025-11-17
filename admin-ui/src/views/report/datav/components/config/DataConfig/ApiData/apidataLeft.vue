<template>
  <div style="padding: 10px 10px 0 10px; height: 100%">
    <div style="font-size: 16px;line-height: 45px;background-color: #f5f5f5;color: #333;text-align:center;font-weight:bold;">
        接口模板
    </div>
    <el-table ref="apiTable" :data="apiList" tooltip-effect="dark" style="width: 100%" highlight-current-row @current-change="onItemChange">
      <el-table-column align="center" prop="label" label="接口名称">
      </el-table-column>
      <el-table-column align="center" prop="typename" label="接口类型">
      </el-table-column>
    </el-table>
    <pagination :pageSizes="pageSizes" v-show="apiTotal > 0" :total="apiTotal" :page.sync="apiQuery.pageNum" :limit.sync="apiQuery.pageSize" @pagination="initApiList"/>

    <div style="margin-top: 20px;display: flex;flex-direction: row;justify-content: right;" v-if="currentRow&&currentRow.typename=='数据源接口'">
      <el-button plain type="primary" @click="apiImport">导 入</el-button>
    </div>
    <el-form ref="form" label-width="80px" v-if="currentRow&&currentRow.typename=='开发者接口'&&DeveloperInfo != null">
      <div style="font-size: 14px;line-height: 45px;background-color: #f5f5f5;padding: 0 15px;margin-bottom: 20px;color: #999;">
        设置接口参数
      </div>
      <div>
        <template
          v-if="
            devValue == 'Api01' ||
            devValue == 'Api04' ||
            devValue == 'Api05' ||
            devValue == 'Api06' ||
            devValue == 'Api07' ||
            devValue == 'Api08' ||
            devValue == 'Api09' ||
            devValue == 'Api10' ||
            devValue == 'Api11' ||
            devValue == 'Api12' ||
            devValue == 'Api13' ||
            devValue == 'Api15'
          "
        >
          <all-params :devValue="devValue" @cancel="open = false" @ok="developerImport"></all-params>
        </template>
        <template v-else>
          <div v-if="devValue == ''" style="text-align: center; color: #999; margin-bottom: 30px">
            请先选择接口类型
          </div>
          <div v-else style="text-align: center; color: #999; margin-bottom: 30px">
            当前接口不需要传参
          </div>
          <div style="margin-top: 20px;display: flex;flex-direction: row;justify-content: right;">
            <el-button plain type="primary" @click="developerImport({})">导 入</el-button>
          </div>
        </template>
      </div>
    </el-form>
  </div>
</template>

<script>
import { listApiSource } from "@/api/report/apisource";
import { devProfile } from "@/api/dev";
import AllParams from "../apiparams/AllParams.vue";
export default {
  components: {AllParams},
  name: "ApiImport",
  data() {
    return {
      open: false,
      allloading: false,
      activeName: "first",
      apiQuery: {
        pageNum: 1,
        pageSize: 5,
        Name: "",
      },
      apiTotal: 0,
      apiList: [],
      pageSizes:[2,5,10, 20, 30, 50],
      currentRow: null,
      devValue: "",
      paramArray: [
        {
          label: "获取物联实时数据",
          value: "Api01",
          url: "/IoTRulesService/HttpRule/Live",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "查询所有设备分组",
          value: "Api02",
          url: "/IoTRulesService/HttpRule/SelectGroups",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "获取产品名称列表",
          value: "Api04",
          url: "/IoTService/HttpSync/ProductNames",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "查询设备列表",
          value: "Api05",
          url: "/IoTService/HttpSync/ListPage",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "获取指定设备的实时属性数据",
          value: "Api06",
          url: "/IoTRulesService/HttpRule/Live",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "查询设备历史数据",
          value: "Api07",
          url: "/IoTRulesService/HttpRule/SelectHistory",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "查询规则列表",
          value: "Api08",
          url: "/IoTRulesService/HttpRule/RuleListPage",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "查询规则模板详情",
          value: "Api09",
          url: "/IoTRulesService/HttpRule/RuleInfo",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "按分组查询实时数据",
          value: "Api10",
          url: "/IoTRulesService/HttpRule/GroupLive",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "获取设备的标签列表",
          value: "Api11",
          url: "/IoTService/HttpSync/TagList",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },
        {
          label: "通过批次编号查询设备",
          value: "Api12",
          url: "/IoTService/HttpSync/DeviceByNumber",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },{
          label: "计划任务汇总",
          value: "Api13",
          url: "/AfterService/HttpSync/PlaneTaskStatis",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },{
          label: "计划类型列表",
          value: "Api14",
          url: "/AfterService/HttpSync/PlaneTaskTypeList",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },{
          label: "计划任务列表",
          value: "Api15",
          url: "/AfterService/HttpSync/PlaneTaskStatisList",
          method: "GET",
          needEnterprise: true,
          typename:'开发者接口'
        },

      ],
      DeveloperInfo: null,
    };
  },
  computed: {},
  async created() {
    let rsp = await devProfile();
    this.DeveloperInfo = rsp.data;
    if(this.DeveloperInfo){}else{
      this.paramArray=[]
    }
  },
  methods: {
    async openImport() {
      this.activeName = "first";
      this.apiQuery.pageNum = 1;
      this.currentRow = null;
      this.devValue = "";
      this.open = true;
      await this.initApiList();
    },
    async initApiList() {
      this.allloading = true;
      this.apiQuery.ApiType = "0";
      let rsp = await listApiSource(this.apiQuery);
      this.apiList = rsp.data.List.map(row=>{
        row.label=row.InterfaceName
        row.typename='数据源接口'
        return row
      });
      if(this.apiList&&this.apiList.length==this.apiQuery.pageSize){
        
      }else if(!this.apiList||this.apiList.length<this.apiQuery.pageSize){
        // if(this.apiQuery.pageNum==1){
        //   let len=this.apiList.length
        //   let arr=this.paramArray.filter((row,inx)=>inx<this.apiQuery.pageSize-len)
        //   this.apiList=[...this.apiList,...arr]
        //   console.log(this.apiList,'this.apiList');
        // }else{
          let tolpagenum=Math.ceil(rsp.data.Total/this.apiQuery.pageSize)
          let yushu=rsp.data.Total%this.apiQuery.pageSize
          if(this.apiQuery.pageNum==tolpagenum){
            let arr=this.paramArray.filter((row,inx)=>inx<this.apiQuery.pageSize-this.apiList.length)
            this.apiList=[...this.apiList,...arr]
          }else{
            let otherTotal=tolpagenum*this.apiQuery.pageSize-yushu
            if(yushu==0){
              otherTotal=0
            }
            let chanum=this.apiQuery.pageNum-tolpagenum
            let arr=this.paramArray.filter((row,inx)=>inx>=otherTotal+this.apiQuery.pageSize*(chanum-1)&&inx<otherTotal+this.apiQuery.pageSize*chanum)
            this.apiList=[...this.apiList,...arr]
            
          }
        // }
        
      }
      
      this.apiTotal = rsp.data.Total+this.paramArray.length;
      this.allloading = false;
    },
    onItemChange(val) {
      this.currentRow = val;
      // console.log(val,'val');
      if(val&&val.typename=="开发者接口"){
        this.devValue=val.value
      }
      
    },
    apiImport() {
      if (this.currentRow == null) {
        this.$message.error("请选择要导入的数据源接口");
        return;
      }
      let importOjb = {
        Url: this.currentRow.Url,
        Method: this.currentRow.Method,
        Header: JSON.parse(this.currentRow.HeaderJson),
        ParamType: this.currentRow.ParamType,
        ParamData: JSON.parse(this.currentRow.ParamJson),
      };
      this.$emit("import", importOjb);
      this.open = false;
    },
    developerImport(apiParams) {
      if (this.devValue == "") {
        this.$message.error("请选择要导入的开发者接口");
        return;
      }
      let curdata = this.paramArray.filter((x) => x.value == this.devValue)[0];
      let importOjb = {
        Url: curdata.url,
        Method: curdata.method,
        Header: [{ name: "token", value: this.DeveloperInfo.SecKey }],
        ParamType: "JSON",
        ParamData: apiParams,
      };
      this.$emit("import", importOjb);
      this.open = false;
    },
  },
};
</script>
<style lang="scss" scoped>
::v-deep .center-tabs .el-tabs__item {
  width: 50%;
  text-align: center;
}
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }
  .el-input.inputText {
    width: 100%;
  }
}

</style>
