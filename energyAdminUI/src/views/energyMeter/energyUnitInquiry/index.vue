<template>
  <div style="padding: 10px 10px 10px 10px" id="big_con" class="energy_inquiry_con">
    <div class="energy_inquiry_left">
      <factorType @getList="getList" />
    </div>
    <div class="energy_inquiry_right">
      <div class="from_con" id="from_con">
        <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
          <div class="biaodan_input_con">
            <el-form-item label="类型" prop="factorId">
              <el-select class="set_radius" style="width: 140px" v-model="dataTypeVal" placeholder="请选择" @change="loadLineTableData">
                <el-option :label="it.label" :value="it.value" v-for="(it, ix) in dataTypeList" :key="'Type' + ix"/>
              </el-select>
            </el-form-item>
            <el-form-item label="能源类型" prop="factorId">
              <el-select @change="factorChange" class="set_radius" style="width: 140px" v-model="queryParams.factorId" placeholder="请选择">
                <el-option :label="it.TypeName" :value="it.Id" v-for="(it, ix) in OrgEngryList" :key="'Type' + ix"/>
              </el-select>
            </el-form-item>
            <el-form-item label="时间选择" prop="equipmentName">
              <div class="form_content_con">
                <el-select class="set_radius" style="width: 52px" v-model="dateVal.Type" placeholder="选" @change="dateTypeChange">
                  <el-option label="日" value="日" />
                  <el-option label="月" value="月" />
                  <el-option label="年" value="年" />
                </el-select>
                <el-date-picker @change="dateValChange" style="margin-left: 4px; width: 120px" v-model="dateVal.val" :type="dateVal.Type == '年' ? 'year' : dateVal.Type == '月' ? 'month' : 'date'"
                  :placeholder=" dateVal.Type == '年' ? '选择年' : dateVal.Type == '月' ? '选择月' : '选择日期'"></el-date-picker>
              </div>
            </el-form-item>
          </div>
          <el-form-item class="button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadLineTableData">搜索</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div class="date_setting_con">
        <div v-if="dataTypeVal==1" class="tabs_ul">
          <div class="tabs_li" :class="{'active':activeTabs=='statistics'}" @click="setActiveTabs('statistics')">
            <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='statistics'">
            <span>统计</span>
          </div>
          <div class="tabs_li" :class="{'active':activeTabs=='MoM'}" @click="setActiveTabs('MoM')">
            <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='MoM'">
            <span>环比</span>
          </div>
          <div class="tabs_li" :class="{'active':activeTabs=='YoY'}" @click="setActiveTabs('YoY')">
            <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='YoY'">
            <span>同比</span>
          </div>
        </div>
        <div class="date_time_con" v-if="dataTypeVal==2">
          <div class="date_title">
            <img src="@/assets/images/zs.png" alt="" />
            <div>本日消耗量</div>
          </div>
          <div class="time_cot">
            ({{returndayjs(queryParams.beginDate, "YYYY/MM/DD HH:mm:ss") +"~" +returndayjs(queryParams.endDate, "YYYY/MM/DD HH:mm:ss")}})
          </div>
        </div>
        <div class="setting_con">
          <el-tooltip placement="left-start" popper-class="unit_tips">
            <div slot="content" class="unitSelect_content">
              <div class="tab">
                <div>数据名称</div>
                <div>一级单位</div>
                <div>二级单位</div>
              </div>
              <div class="tab_content" v-for="item in unitList" :key="item.name">
                <div>{{ item.name }}</div>
                <div>{{ item.unit }}</div>
                <div>{{ item.lastUnit }}</div>
              </div>
            </div>
            <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color: rgba(255, 255, 255, 0.6); font-size: 12px"></i>
          </el-tooltip>
          <span class="span">单位换算</span>
          <el-select class="set_radius" style="width: 110px" v-model="unitSelect" @change="unitChange">
            <el-option label="一级单位" value="Unit" />
            <el-option label="二级单位" value="LageUnit" />
          </el-select>
        </div>
      </div>
      <peak-vallek :timeType="dateVal.Type" :facilityName="facilityName" :factorName="factorName" :unitSelect="unitSelect" v-if="dataTypeVal==2" ref="peakVallek" :queryParams="queryParams" :allData="allDataArray" :tableConHeight="tableConHeight"></peak-vallek>
      <unit-use :unitSelect="unitSelect" v-if="dataTypeVal==1&&activeTabs=='statistics'" :dateVal="dateVal" ref="peakVallek" :queryParams="queryParams" :allData="allDataInfo" :tableConHeight="tableConHeight"></unit-use>
      <mom-yoy :totalType="activeTabs=='YoY'?'yoy':'mom'" :dateVal="dateVal" v-if="dataTypeVal==1&&activeTabs=='MoM'||dataTypeVal==1&&activeTabs=='YoY'" ref="momYoy" :queryParams="queryParams" :allData="allDataInfo" :unitSelect="unitSelect" :tableConHeight="tableConHeight"></mom-yoy>
    </div>
  </div>
</template>

<script>
var dayjs = require("@/utils/day.js");
import peakVallek from "./cmp/peakVallek.vue";
import unitUse from "./cmp/unitUse.vue";
import momYoy from "./cmp/momYoy.vue";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import { facilityTree } from "@/api/energy/facility";
import { selectFacilityEnergyHourList } from "@/api/energy/energyMeter";
import factorType from "@/views/energyMeter/energyCellStatis/cmp/factorType.vue";
import { selectFactorTypeOrg } from "@/api/energy/factorLibrary";
export default {
  name: "energyUnitInquiryAdminUIIndex",
  components: { Treeselect, factorType, peakVallek,unitUse,momYoy },
  mixins: [resizeTableCon],
  data() {
    return {
      OrgEngryList: [],
      allDataArray: [],
      allDataInfo:{},
      echartData: [],
      queryParams: {},
      dateVal: {
        Type: "月",
        val: "",
      },
      groupTreeList: [],
      nullArr: [],
      dataTypeVal: "1",
      dataTypeList: [
        {
          label: "单元消耗",
          value: "1",
        },
        {
          label: "峰谷消耗",
          value: "2",
        },
      ],
      unitSelect: "Unit",
      unitList: [
        { name: "电", unit: "KW(千瓦)", lastUnit: "MW(万瓦)" },
        { name: "天然气", unit: "Nm³(标准m³)", lastUnit: "万Nm³" },
        { name: "金额", unit: "元", lastUnit: "万元" },
        { name: "标准煤", unit: "kgce", lastUnit: "tce" },
        { name: "碳排放", unit: "kgCO₂e", lastUnit: "tCO₂e" },
      ],
      facilityName:'',
      activeTabs:'statistics',
    };
  },
  computed:{
    factorName(){
      if(this.queryParams.factorId){
        let findRow=this.OrgEngryList.find(row=>row.Id==this.queryParams.factorId)
        if(findRow){
          return findRow.TypeName
        }else{
          return ''
        }
      }
    }
  },
  async mounted() {
    // this.dateVal.val = dayjs().format("YYYY-MM");

    // this.orgId = this.$store.state.user.orgId;
    // this.queryParams.OrgId = this.orgId;
    // await this.loadSelectFactorOrg();
    // await this.dateValChange();
    // this.loadLineTableData()
  },

  methods: {
    factorChange(){
      let queryForm=JSON.parse(JSON.stringify(this.queryParams))
      this.queryParams=JSON.parse(JSON.stringify(queryForm))
    },
    setActiveTabs(val){
      this.activeTabs=val
    },
    unitChange(val) {
      this.unitSelect = val;
      this.loadLineTableData();
    },
    loadLineTableData() {
      selectFacilityEnergyHourList(this.queryParams).then((res) => {
        // console.log("单元峰谷数据", res);
        
        this.allDataInfo=JSON.parse(JSON.stringify(res.data))
        let totalObj={
          "TimePeriod": "合计",
          "Unit": this.allDataInfo.Unit,
          "LageUnit": this.allDataInfo.LageUnit,
          "UseVale": this.allDataInfo.UseVale,
          "CostVale": this.allDataInfo.CostVale,
          "CarbonEmission": this.allDataInfo.CarbonEmission,
          "ConvertCoal": this.allDataInfo.ConvertCoal,
          "Details": []
        }
        if(res.data&&res.data.TimePeriods&&res.data.TimePeriods.length>0){
          this.allDataArray=JSON.parse(JSON.stringify(res.data.TimePeriods))
        }
        this.allDataArray.push(totalObj)
      });
    },
    async loadSelectFactorOrg() {
      let res = await selectFactorTypeOrg({ OrgId: this.orgId });
      // console.log(res,'res');
      this.OrgEngryList = res.data;
      if (this.OrgEngryList[0]) {
        this.queryParams.factorId = this.OrgEngryList[0].Id;
      }
      // this.loadEnergyFlow()
    },
    returndayjs(val, formatstr) {
      return dayjs(val).format(formatstr);
    },
    resetQuery() {},
    dateTypeChange() {
      this.dateVal.val = "";
    },
    async getList(facilityId,facilityName) {
      if(facilityName){
        this.facilityName=facilityName
      }else{
        this.facilityName=''
      }
      this.queryParams.facilityId = facilityId
      this.dateVal.val = dayjs().format("YYYY-MM");
      this.orgId = this.$store.state.user.orgId;
      this.queryParams.OrgId = this.orgId;
      await this.loadSelectFactorOrg();
      await this.dateValChange();
      this.$nextTick(()=>{
        this.setTableCon()
      })
      this.loadLineTableData()
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.FacilityName,
        // isDisabled: node.isDisabled,
        children: node.Children,
      };
    },
    async dateValChange() {
      if (this.dateVal.val) {
        if (this.dateVal.Type == "日") {
          this.queryParams.beginDate = dayjs(this.dateVal.val).startOf("date").format("YYYY-MM-DD HH:mm:ss");
          this.queryParams.endDate = dayjs(this.dateVal.val).endOf("date").format("YYYY-MM-DD HH:mm:ss");
        } else if (this.dateVal.Type == "月") {
          this.queryParams.beginDate = dayjs(this.dateVal.val).startOf("month").format("YYYY-MM-DD HH:mm:ss");
          this.queryParams.endDate = dayjs(this.dateVal.val).endOf("month").format("YYYY-MM-DD HH:mm:ss");
        } else if (this.dateVal.Type == "年") {
          this.queryParams.beginDate = dayjs(this.dateVal.val).startOf("year").format("YYYY-MM-DD HH:mm:ss");
          this.queryParams.endDate = dayjs(this.dateVal.val).endOf("year").format("YYYY-MM-DD HH:mm:ss");
        }
        this.queryParams.timeType = this.dateVal.Type;
      } else {
        delete this.queryParams.beginDate;
        delete this.queryParams.endDate;
        delete this.queryParams.timeType;
      }
      // await this.getProductList()
    },
  },
};
</script>


<style lang="less" scoped>
.unitSelect_content {
  width: 300px;
  height: 168px;
  background: #000000;
  .tab {
    display: flex;
    height: 28px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    > div {
      flex: 1;
      text-align: center;
      height: 100%;
      line-height: 28px;
      font-weight: 500;
      font-size: 12px;
      color: rgba(255, 255, 255, 0.6);
    }
  }
  .tab_content {
    height: 28px;
    display: flex;
    > div {
      flex: 1;
      text-align: center;
      height: 100%;
      line-height: 28px;
      font-weight: 500;
      font-size: 12px;
      color: #ffffff;
    }
  }
}
.date_setting_con {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  margin-top: 8px;
  .tabs_ul{
    width: 200px;
    display: flex;
    align-items: center;
    // border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    height: 28px;
    
    .tabs_li{
      line-height: 16px;
      display: flex;
      align-items: center;
      margin-right: 24px;
      cursor: pointer;
      img{
        width: 16px;
        height: 16px;
        margin-right: 8px;
      }
      span{
        font-size: 16px;
        color: rgba(255, 255, 255, 0.6);
      }
      &.active{
        span{
          color: rgba(255, 255, 255, 1);
        }
      }
    }
  }
  .date_time_con {
    display: flex;
    align-items: center;

    .date_title {
      display: flex;
      align-items: center;
      line-height: 16px;
      color: rgba(255, 255, 255, 1);
      font-size: 16px;
      img {
        width: 16px;
        height: 16px;
      }
      div {
        margin-left: 8px;
        margin-right: 8px;
      }
    }
    .time_cot {
      font-size: 14px;
      color: rgba(255, 255, 255, 0.6);
    }
  }
  .setting_con {
    .set_radius {
      height: 28px;
      line-height: 28px;
    }
    .el-select {
      ::v-deep .el-input {
        font-size: 12px;
        .el-input__inner {
          height: 28px;
          line-height: 28px;
        }
        .el-select__caret.el-input__icon {
          height: 26px;
          line-height: 26px;
          font-size: 10px;
        }
      }
    }
    .span {
      font-size: 12px;
      margin-left: 4px;
      margin-right: 8px;
      color: rgba(255, 255, 255, 1);
    }
  }
}
.energy_inquiry_con {
  display: flex;
  justify-content: space-between;
  width: 100%;
  .energy_inquiry_left {
    width: 240px;
    background: rgba(25, 33, 45, 1);
    border-radius: 4px;
  }
  .energy_inquiry_right {
    width: calc(100% - 250px);
    background: rgba(25, 33, 45, 1);
    border-radius: 4px;
    padding-left: 20px;
    padding-right: 20px;
    box-sizing: border-box;
  }
}
</style>
<style lang="less">
.unit_tips.el-tooltip__popper.is-dark{
  background: rgba(0, 0, 0, 1);
  .popper__arrow{
    display: none;
  }
}
</style>