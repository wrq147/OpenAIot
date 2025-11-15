<template>
  <div>
    <div class="table_pie">
      <div class="table_con">
          <div class="table_ul oddNumber">
            <div class="table_li first">统计</div>
            <div class="table_li">能源消耗<span>({{activeEngryUnit}})</span></div>
            <div class="table_li">成本({{unitSelect=='LageUnit'?'万元':'元'}})</div>
            <div class="table_li">标准煤({{unitSelect=='LageUnit'?'tce':'kgce'}})</div>
            <div class="table_li">碳排放({{unitSelect=='LageUnit'?'tCO₂e':'kgCO₂e'}})</div>
          </div>
          <div class="table_ul" v-for="(ite,inx) in tableDataArray" :key="ite.type" :class="{'oddNumber':inx%2==1}">
          <div class="table_li first">{{ite.TimePeriod}}</div>
          <div class="table_li">{{ite.UseVale}}</div>
          <div class="table_li">{{ite.CostVale}}</div>
          <div class="table_li">{{ite.CarbonEmission}}</div>
          <div class="table_li">{{ite.ConvertCoal}}</div>
          </div>
      </div>
      <div class="pice_con">
          <div class="trend_title">
          <div class="line"></div>
          <div class="text">能源消耗占比</div>
          <div class="text_unit" v-if="activeEngryUnit">(单位：{{activeEngryUnit}})</div>
          </div>
          <echartPie style="width:100%;height:100%;" ref="echartPie" nameFiled="TimePeriod" valueFiled="UseVale" :pieData="pieData" :unitSelect="unitSelect"></echartPie>
      </div>
    </div>
    <div class="trend_chart" :style="{'min-height':tableConHeight-316+'px'}">
      <div class="trend_title_con">
        <div class="trend_title">
          <div class="line"></div>
          <div class="text">能耗趋势图</div>
          <div class="text_unit">
            <span v-if="echartsFiled=='UseVale'&&activeEngryUnit">(单位：{{activeEngryUnit}})</span>
            <span v-if="echartsFiled=='CostVale'">(单位：{{unitSelect=='LageUnit'?'万元':'元'}})</span>
            <span v-if="echartsFiled=='ConvertCoal'">(单位：{{unitSelect=='LageUnit'?'tce':'kgce'}})</span>
            <span v-if="echartsFiled=='CarbonEmission'">(单位：{{unitSelect=='LageUnit'?'tCO₂e':'kgCO₂e'}})</span>
          </div>
        </div>
        <el-select class="set_radius noborder" style="width: 96px" v-model="echartsFiled" :border="false">
          <el-option :label="ite.label" :value="ite.filed" v-for="ite in filedList" :key="ite.filed"/>
        </el-select>
      </div>
      <echartBar ref="echartBar" :allData="echartBarData" :dataType="echartsFiled" :dataTitle="echartsTitle"></echartBar>
    </div>
    <strategy :timeType="timeType" :startTime="queryParams.beginDate" :endTime="queryParams.endDate" ref="strategy" :strategyList="strategyList" :factorName="factorName" :equipName="facilityName"></strategy>
  </div>
</template>

<script>
import echartBar from '@/views/energyMeter/energyInquiry/cmp/echartsBar3d'
import echartPie from '@/views/energyMeter/energyInquiry/cmp/echartPie'
import strategy from '@/views/energyMeter/energyInquiry/cmp/strategy.vue'
var dayjs = require("@/utils/day.js");
export default {
  name: 'EnergyAdminUIUnitPeakVallek',
  props:{
    tableConHeight:{
        type:[Number,String],
        default:0
    },
    allData:{
        type:Array,
        default:()=>{
            return []
        }
    },
    unitList:{
        type:Array,
        default:()=>{
            return []
        }
    },
    queryParams:{
        type:Object,
        default:()=>{
            return {}
        }
    },
    unitSelect:{
        type:String,
        default:'Unit'
    },
    facilityName:{//单元名称
        type:String,
        default:''
    },
    factorName:{//能源类型
        type:String,
        default:''
    },
    timeType:{//同时时间类型
      type:String,
      default:'月'
    },
  },
  components:{echartBar,echartPie,strategy},
  data() {
    return {
      filedList:[{filed:'UseVale',label:'能源消耗'},{filed:'CostVale',label:'成本'},{filed:'ConvertCoal',label:'标准煤'},{filed:'CarbonEmission',label:'碳排放'}],
      // echartsTitle:'能源消耗',
      echartsFiled:'UseVale',
      tableDataArray:[{
        type:'尖',
        energyUsage:'-',
        cost:'-',
        standardCoal:'-',
        carbon:'-',
      },{
        type:'峰',
        energyUsage:'-',
        cost:'-',
        standardCoal:'-',
        carbon:'-',
      },{
        type:'平',
        energyUsage:'-',
        cost:'-',
        standardCoal:'-',
        carbon:'-',
      },{
        type:'谷',
        energyUsage:'-',
        cost:'-',
        standardCoal:'-',
        carbon:'-',
      },{
        type:'合计',
        energyUsage:'-',
        cost:'-',
        standardCoal:'-',
        carbon:'-',
      }],//
    //   unitSelect:'Unit',
      echartBarData:[],
      pieData:[],
      activeEngryUnit:'',
      strategyList:[],
    };
  },
  computed:{
    echartsTitle(){
      if(this.echartsFiled){
        let findObj=this.filedList.find(row=>row.filed==this.echartsFiled)
        if(findObj){
          return findObj.label
        }
      }
      return ''
    }
  },
  watch:{
    allData:{
        handler(newVal){
            let newValArr=JSON.parse(JSON.stringify(newVal))
            let totalUseData=0
            this.tableDataArray=newValArr.map(row=>{
              let rowObj=JSON.parse(JSON.stringify(row))
              if(this.unitSelect=='Unit'){
                if(row.Unit){
                  this.activeEngryUnit=row.Unit
                }
              }else if(this.unitSelect=='LageUnit'){
                if(row.LageUnit){
                  this.activeEngryUnit=row.LageUnit
                }
                rowObj.UseVale=Number((Number(rowObj.UseVale)/10000).toFixed(6))
                rowObj.CostVale=Number((Number(rowObj.CostVale)/10000).toFixed(6))
                rowObj.CarbonEmission=Number((Number(rowObj.CarbonEmission)/1000).toFixed(6))
                rowObj.ConvertCoal=Number((Number(rowObj.ConvertCoal)/1000).toFixed(6))
              }
              if(row.TimePeriod=='合计'){
                totalUseData=rowObj.UseVale
              }
              delete rowObj.Details
              return rowObj
            })
            this.pieData=this.tableDataArray.filter(row=>row.TimePeriod!='合计')
            let barData=newValArr.map(row=>{
              if(row.Details){
                row.Details=row.Details.map(roobj=>{
                  if(this.unitSelect=='Unit'){}else if(this.unitSelect=='LageUnit'){
                    roobj.UseVale=Number((Number(roobj.UseVale)/10000).toFixed(6))
                    roobj.CostVale=Number((Number(roobj.CostVale)/10000).toFixed(6))
                    roobj.CarbonEmission=Number((Number(roobj.CarbonEmission)/1000).toFixed(6))
                    roobj.ConvertCoal=Number((Number(roobj.ConvertCoal)/1000).toFixed(6))
                  }
                  return roobj
                })
              }
              return row
            })
            this.echartBarData=barData.filter(row=>row.TimePeriod!='合计')
            let strategyList=JSON.parse(JSON.stringify(this.pieData))
            this.strategyList=strategyList.map(rw=>{
              rw.pence=totalUseData?(Number((Number(rw.UseVale)/Number(totalUseData)).toFixed(4))*100).toFixed(2):0
              return rw
            })
            // console.log(this.unitSelect, this.activeEngryUnit,'this.activeEngryUnit');
            // this.tableDataArray=JSON.parse(JSON.stringify(newVal))
        },
        immediate:true,
        deep:true,
    }
  },
  mounted() {
    
  },

  methods: {
  },
};
</script>

<style lang="less" scoped>
.noborder.set_radius{
  height: 20px;
  line-height: 20px;
  
  ::v-deep .el-input.el-input--suffix .el-input__inner{
    border: none;
    height: 20px;
    line-height: 20px;
    font-size: 12px;
    text-align: right;
    padding-right: 26px;
  }
  ::v-deep .el-input__suffix{
    right: 0;
  }
  ::v-deep .el-input__suffix .el-input__suffix-inner .el-input__icon{
    line-height: 20px;
    
  }
}
.table_pie{
  display: flex;
  width: 100%;
  .table_con{
    min-height: 240px;
    width: 50%;
    border-right: 1px solid rgba(255, 255, 255, 0.1);
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    .table_ul{
      display: flex;
      background: rgba(25, 33, 45, 1);
      width: 100%;
      &.oddNumber{
        background: rgba(34, 46, 64, 1);
      }
      .table_li{
        width: 21%;
        height: 40px;
        display: flex;
        justify-content: center;
        align-items: center;
        color: rgba(255, 255, 255, 1);
        font-size: 14px;
        border-left: 1px solid rgba(255, 255, 255, 0.1);
        &.first{
          width: 16%;
          height: 40px;
        }
      }
    }
  }
  .pice_con{
    width: calc(50% - 10px);
    margin-left: 10px;
    position: relative;
    
  }
}

.trend_title{
  color: #ffffff;
  display: flex;
  align-items: flex-end;
  position: absolute;
  left: 16px;
  top: 16px;
  font-size: 14px;
  
  .line{
    width: 5px;
    height: 16px;
    border-radius: 2px;
    background: rgb(42, 211, 154);
    margin-right: 10px;
  }
  .text_unit{
    color: rgba(255, 255, 255, 0.6);
    margin-left: 8px;
  }
}
.trend_title_con{
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: absolute;
  padding: 13px 16px 10px;
  box-sizing: border-box;
  left: 0;
  top: 0;
  width: 100%;
  z-index: 9;
  .trend_title{
    position: relative;
    left: 0;
    top: 0;
  }
}
.trend_chart{
  width: 100%;
  padding-bottom: 20px;
  background: rgba(34, 46, 64, 1);
  padding-top: 16px;
  padding-left: 16px;
  padding-right: 16px;
  position: relative;
  margin-top: 10px;
  box-sizing: border-box;
  
  .table_ul{
    width: 100%;
    display: flex;
    flex-wrap: wrap;
    .table_li{
      width: 25%;
      color: #ffffff;
      box-sizing: border-box;
      &.hasbottom{
        border-bottom: 2px solid rgba(255, 255, 255, 0.2);
      }
      &:nth-child(4n),&:last-child{
        border-right: 2px solid rgba(255, 255, 255, 0.2);
      }
      &.noborder{
        border-right:none;
        &.firstno{
          border-left: 2px solid rgba(255, 255, 255, 0.2);
        }
      }
      .table_title_con{
        display: flex;
        width: 100%;
        .table_title_li{
          width: 33.3333333333%;
          text-align: center;
          background: rgba(34, 46, 64, 1);
          height: 40px;
          line-height: 40px;
          border-left: 2px solid rgba(255, 255, 255, 0.2);
          border-top: 2px solid rgba(255, 255, 255, 0.2);
          box-sizing: border-box;
        }
      }
      .table_data_con{
        display: flex;
        width: 100%;
        .table_data_li{
          width: 33.333333333333%;
          text-align: center;
          height: 40px;
          line-height: 40px;
          border-left: 2px solid rgba(255, 255, 255, 0.2);
          border-top: 2px solid rgba(255, 255, 255, 0.2);
          box-sizing: border-box;
        }
      }
    }
  }
}
</style>