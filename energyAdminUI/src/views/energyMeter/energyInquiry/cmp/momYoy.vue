<template>
  <div>
      <div class="table_pie">
        <div class="table_con">
            <div class="table_ul" v-for="(ite,inx) in tableDataArray" :key="ite.type" :class="{'oddNumber':inx%2==0}">
              <div class="table_li first">
                <span>{{ite.title}}</span>
                <el-tooltip v-if="ite.tip" placement="right" popper-class="unit_tips">
                  <div slot="content" class="unitSelect_content">{{ ite.tipText }}</div>
                  <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color: rgba(255, 255, 255, 0.6); font-size: 12px;margin-left:4px;cursor: pointer;"></i>
                </el-tooltip>
              </div>
              <div class="table_li time_li">
                <span style="margin-right:4px;" v-if="unitSelect=='LageUnit'">{{ite.lastLageValue}}</span>
                <span style="margin-right:4px;" v-else>{{ite.lastValue}}</span>
                <span v-if="unitSelect=='LageUnit'">{{ite.LageUnit}}</span>
                <span v-else>{{ite.Unit}}</span>
              </div>
              <div class="table_li time_li">
                <span style="margin-right:4px;" v-if="unitSelect=='LageUnit'">{{ite.lageValue}}</span>
                <span style="margin-right:4px;" v-else>{{ite.value}}</span>
                <span v-if="unitSelect=='LageUnit'">{{ite.LageUnit}}</span>
                <span v-else>{{ite.Unit}}</span>
              </div>
              <div class="table_li">
                <span style="margin-right:4px;" v-if="ite.diffVal!=undefined">{{ite.diffVal}}</span>
              </div>
              <div class="table_li last">
                <span v-if="ite.diffPercent!=undefined" style="margin-right:4px;" :class="{'red':ite.diffPercent!='-'&&ite.diffPercent>0,'green':ite.diffPercent!='-'&&ite.diffPercent<0}">{{ite.diffPercent=='-'?ite.diffPercent:ite.diffPercent+'%'}}</span>
              </div>
              
            </div>
            <!-- <div class="table_ul" v-for="(ite,inx) in tableDataArray" :key="ite.type" :class="{'oddNumber':inx%2==1}">
              <div class="table_li">{{ite.energyUsage}}<span v-if="activeEngryUnit">({{activeEngryUnit}})</span></div>
              <div class="table_li">{{ite.cost}}<span>({{unitSelect=='LageUnit'?'万元':'元'}})</span></div>
              <div class="table_li">{{ite.standardCoal}}<span>({{unitSelect=='LageUnit'?'tce':'kgce'}})</span></div>
              <div class="table_li">{{ite.carbon}}<span>({{unitSelect=='LageUnit'?'tCO₂e':'kgCO₂e'}})</span></div>
            </div> -->
        </div>
        <div class="pice_con">
            <div class="trend_title">
              <div class="line"></div>
              <div class="text">环比排名</div>
              <!-- <div class="text_unit" v-if="activeEngryUnit">(单位：{{activeEngryUnit}})</div> -->
            </div>
            <echartDoubleXBar style="width:100%;height:100%;" ref="echartXBar" :echartData="echartXBarData" :unitSelect="unitSelect"></echartDoubleXBar>
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
        <echartBar style="min-width:930px" :dateTitle="dateTitle" :formatStr="formatStr" :dataType="echartsFiled" ref="echartBar" :echartsTitle="echartsTitle" :allData="echartBarData" :isShowMax="false"></echartBar>
    </div>
  </div>
</template>

<script>
import echartBar from '@/views/energyMeter/energyInquiry/cmp/echartDoubleBar'
import echartPie from '@/views/energyMeter/energyInquiry/cmp/echartPie'
import echartDoubleXBar from '@/views/energyMeter/energyInquiry/cmp/echartDoubleXBar'
import { selectEnergyHourList } from "@/api/energy/energyMeter";
var dayjs = require("@/utils/day.js");
export default {
  name: 'momYoy',
  props:{
    tableConHeight:{
        type:[Number,String],
        default:0
    },
    allData:{
        type:Object,
        default:()=>{
            return {}
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
    dateVal:{
        type:Object,
        default:()=>{
            return {}
        }
    },
    unitSelect:{
        type:String,
        default:'Unit'
    },
    totalType:{
        type:String,
        default:'mom'
    },
  },
  components:{echartBar,echartPie,echartDoubleXBar},
  data() {
    return {
      filedList:[{filed:'UseVale',label:'能源消耗'},{filed:'CostVale',label:'成本'},{filed:'ConvertCoal',label:'标准煤'},{filed:'CarbonEmission',label:'碳排放'}],
      // echartsTitle:'能源消耗',
      echartsFiled:'UseVale',
      tableDataArray:[{
        title: '能源消耗',
        tip: false,
        prop: 'UseVale',
        value:'-'
      },
      {
        title: '能源成本',
        tip: false,
        prop: 'CostVale',
        value:'-'
      },
      {
        title: '标准煤',
        tip: true,
        tipText: '标准煤量=能源消耗量*该能源低位发热量/折标煤系数',
        prop: 'ConvertCoal',
        value:'-'
      },
      {
        title: '碳排放',
        tip: true,
        tipText: '碳排放=电量消耗量×当地电网因子值',
        prop: 'CarbonEmission',
        value:'-'
      },],//
    //   unitSelect:'Unit',
      echartBarData:[],
      echartXBarData:[],
      pieData:[],
      activeEngryUnit:'',
      dateTitle:'日期',
      formatStr:'MM-DD',
      currentBarData:[],
      currentXBarData:[],
      tableArray:[]
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
        handler(val){
          if(val){
            // console.log(val,'本期');
            
            let newVal=JSON.parse(JSON.stringify(val))
            if(newVal){
              let totalData=JSON.parse(JSON.stringify(newVal))
              this.currentXBarData=this.processXbarData(totalData)
            }

            if(this.unitSelect=='Unit'){
                this.activeEngryUnit=newVal.Unit
            }else if(this.unitSelect=='LageUnit'){
                this.activeEngryUnit=newVal.LageUnit
            }
            if(newVal&&newVal.Details&&newVal.Details.length>0){
              let Details=JSON.parse(JSON.stringify(newVal.Details))
              this.currentBarData=this.processTrendData(Details)
            }else{
              this.currentBarData=[]
            }
            this.loadEquipLineTableData()
            this.tableArray=[
              {title: '能源消耗',Unit:newVal.Unit,LageUnit:newVal.LageUnit,tip: false,prop: 'UseVale',value:newVal.UseVale==null?'-':newVal.UseVale,lageValue:newVal.UseVale==null?'-':Number((Number(newVal.UseVale)/10000).toFixed(6))},
              {title: '能源成本',Unit:'元',LageUnit:'万元',tip: false,prop: 'CostVale',value:newVal.CostVale==null?'-':newVal.CostVale,lageValue:newVal.CostVale==null?'-':Number((Number(newVal.CostVale)/10000).toFixed(6))},
              {title: '标准煤',Unit:'kgce',LageUnit:'tce',tip: true,tipText: '标准煤量=能源消耗量*该能源低位发热量/折标煤系数',prop: 'ConvertCoal',value:newVal.ConvertCoal==null?'-':newVal.ConvertCoal,lageValue:newVal.ConvertCoal==null?'-':Number((Number(newVal.ConvertCoal)/1000).toFixed(6))},
              {title: '碳排放',Unit:'kgCO₂e',LageUnit:'tCO₂e',tip: true,tipText: '碳排放=电量消耗量×当地电网因子值',prop: 'CarbonEmission',value:newVal.CarbonEmission==null?'-':newVal.CarbonEmission,lageValue:newVal.CarbonEmission==null?'-':Number((Number(newVal.CarbonEmission)/1000).toFixed(6))}];
          }
          
        },
        immediate:true,
        deep:true,
    },
    totalType:{
      handler(nval){
        if(this.allData){
          // console.log(nval,'本期');
          
          let newVal=JSON.parse(JSON.stringify(this.allData))
          if(newVal){
            let totalData=JSON.parse(JSON.stringify(newVal))
            this.currentXBarData=this.processXbarData(totalData)
          }

          if(this.unitSelect=='Unit'){
              this.activeEngryUnit=newVal.Unit
          }else if(this.unitSelect=='LageUnit'){
              this.activeEngryUnit=newVal.LageUnit
          }
          if(newVal&&newVal.Details&&newVal.Details.length>0){
            let Details=JSON.parse(JSON.stringify(newVal.Details))
            this.currentBarData=this.processTrendData(Details)
          }else{
            this.currentBarData=[]
          }
          this.loadEquipLineTableData()
          this.tableArray=[
            {title: '能源消耗',Unit:newVal.Unit,LageUnit:newVal.LageUnit,tip: false,prop: 'UseVale',value:newVal.UseVale==null?'-':newVal.UseVale,lageValue:newVal.UseVale==null?'-':Number((Number(newVal.UseVale)/10000).toFixed(6))},
            {title: '能源成本',Unit:'元',LageUnit:'万元',tip: false,prop: 'CostVale',value:newVal.CostVale==null?'-':newVal.CostVale,lageValue:newVal.CostVale==null?'-':Number((Number(newVal.CostVale)/10000).toFixed(6))},
            {title: '标准煤',Unit:'kgce',LageUnit:'tce',tip: true,tipText: '标准煤量=能源消耗量*该能源低位发热量/折标煤系数',prop: 'ConvertCoal',value:newVal.ConvertCoal==null?'-':newVal.ConvertCoal,lageValue:newVal.ConvertCoal==null?'-':Number((Number(newVal.ConvertCoal)/1000).toFixed(6))},
            {title: '碳排放',Unit:'kgCO₂e',LageUnit:'tCO₂e',tip: true,tipText: '碳排放=电量消耗量×当地电网因子值',prop: 'CarbonEmission',value:newVal.CarbonEmission==null?'-':newVal.CarbonEmission,lageValue:newVal.CarbonEmission==null?'-':Number((Number(newVal.CarbonEmission)/1000).toFixed(6))}];
        }
      }
    },
  },
  mounted() {
    
  },

  methods: {
    processTrendData(data){
      let Times=JSON.parse(JSON.stringify(data))
      let arrData=Times.map(row=>{
        if(this.dateVal&&this.dateVal.Type=='月'){
          row.DDate=row.TTime
          this.dateTitle='日期'
        }else if(this.dateVal&&this.dateVal.Type=='年'){
          row.DDate=row.TTime
          this.dateTitle='月份'
        }else if(this.dateVal&&this.dateVal.Type=='日'){
          row.DDate=row.TTime
          this.dateTitle='时间'
          // console.log(row.DDate,'row.DDaterow.DDate');
        }
        // console.log(row,'rowrow');
        if(this.unitSelect=='Unit'){
        }else if(this.unitSelect=='LageUnit'){
          row.UseVale=Number((Number(row.UseVale)/10000).toFixed(6))
          row.CostVale=Number((Number(row.CostVale)/10000).toFixed(6))
          row.ConvertCoal=Number((Number(row.ConvertCoal)/1000).toFixed(6))
          row.CarbonEmission=Number((Number(row.CarbonEmission)/1000).toFixed(6))
        }
        return row
      })
      return arrData
    },
    processXbarData(data){
      let totalData=JSON.parse(JSON.stringify(data))
      let currentArr=[]
      for(let totalKey in totalData){
        if(totalKey=='ConvertCoal'){
          if(this.unitSelect=='LageUnit'){
            totalData[totalKey]=Number((Number(totalData[totalKey])/1000).toFixed(6))
          }
          let obj={
            name:totalKey,
            text:'标准煤',
            value:totalData[totalKey]
          }
          currentArr.push(obj)
        }
        if(totalKey=='CarbonEmission'){
          if(this.unitSelect=='LageUnit'){
            totalData[totalKey]=Number((Number(totalData[totalKey])/1000).toFixed(6))
          }
          let obj={
            name:totalKey,
            text:'碳排放',
            value:totalData[totalKey]
          }
          currentArr.push(obj)
        }
        if(totalKey=='UseVale'){
          if(this.unitSelect=='LageUnit'){
            totalData[totalKey]=Number((Number(totalData[totalKey])/10000).toFixed(6))
          }
          let obj={
            name:totalKey,
            text:'能源消耗',
            value:totalData[totalKey]
          }
          currentArr.push(obj)
        }
        if(totalKey=='CostVale'){
          if(this.unitSelect=='LageUnit'){
            totalData[totalKey]=Number((Number(totalData[totalKey])/10000).toFixed(6))
          }
          let obj={
            name:totalKey,
            text:'能源成本',
            value:totalData[totalKey]
          }
          currentArr.push(obj)
        }
      }
      return currentArr
    },
    loadEquipLineTableData() {//环比数据
      let queryParams=JSON.parse(JSON.stringify(this.queryParams))
      if(this.totalType=='mom'){
        if(this.dateVal.Type=='日'){
          queryParams.beginDate=dayjs(queryParams.beginDate).subtract(1, 'day').format('YYYY-MM-DD HH:mm:ss')
          queryParams.endDate=dayjs(queryParams.endDate).subtract(1, 'day').format('YYYY-MM-DD HH:mm:ss')
        }
        if(this.dateVal.Type=='月'){
          queryParams.beginDate=dayjs(queryParams.beginDate).subtract(1, 'month').format('YYYY-MM-DD HH:mm:ss')
          queryParams.endDate=dayjs(queryParams.endDate).subtract(1, 'month').format('YYYY-MM-DD HH:mm:ss')
        }
        if(this.dateVal.Type=='年'){
          queryParams.beginDate=dayjs(queryParams.beginDate).subtract(1, 'year').format('YYYY-MM-DD HH:mm:ss')
          queryParams.endDate=dayjs(queryParams.endDate).subtract(1, 'year').format('YYYY-MM-DD HH:mm:ss')
        }
      }else if(this.totalType=='yoy'){
        queryParams.beginDate=dayjs(queryParams.beginDate).subtract(1, 'year').format('YYYY-MM-DD HH:mm:ss')
        queryParams.endDate=dayjs(queryParams.endDate).subtract(1, 'year').format('YYYY-MM-DD HH:mm:ss')
      }
      // if(this.totalType=='mom'){
        selectEnergyHourList(queryParams).then((res) => {
          // console.log("上期", res);
          let lastAllData=res.data.find(row=>row.TimePeriod=='合计')
          let lastVal=JSON.parse(JSON.stringify(lastAllData))
          let lastXBarData=[]
          let lastBarData=[]
          if(lastVal){
            let totalData=JSON.parse(JSON.stringify(lastVal))
            lastXBarData=this.processXbarData(totalData)
          }

          if(lastVal&&lastVal.Details&&lastVal.Details.length>0){
            let Details=JSON.parse(JSON.stringify(lastVal.Details))
            lastBarData=this.processTrendData(Details)
          }else{
            lastBarData=[]
          }
          // this.allDataArray = res.data;
          let currentData={
            data:this.currentBarData,
            name:'本期'
          }
          let lastData={
            data:lastBarData,
            name:this.totalType=='yoy'?'去年同期':'上期'
          }
          let currentXData={
            data:this.currentXBarData,
            name:'本期'
          }
          let lastXData={
            data:lastXBarData,
            name:this.totalType=='yoy'?'去年同期':'上期'
          }
          this.echartBarData=[lastData,currentData]
          this.echartXBarData=[lastXData,currentXData]
          this.tableDataArray=this.tableArray.map(ro=>{
            let rowObj=JSON.parse(JSON.stringify(ro))
            if(ro.prop=='ConvertCoal'||ro.prop=='CarbonEmission'){
              rowObj.lastValue=lastVal[ro.prop]==null?'-':lastVal[ro.prop]
              rowObj.lastLageValue=lastVal[ro.prop]==null?'-':Number((Number(lastVal[ro.prop])/1000).toFixed(6))
            }else{
              rowObj.lastValue=lastVal[ro.prop]==null?'-':lastVal[ro.prop]
              rowObj.lastLageValue=lastVal[ro.prop]==null?'-':Number((Number(lastVal[ro.prop])/10000).toFixed(6))
            }
            if(this.unitSelect=='Unit'){
              if(ro.value!='-'&&rowObj.lastValue!='-'){
                rowObj.diffVal=Number(Number(Number(ro.value)-Number(rowObj.lastValue)).toFixed(2));
                rowObj.diffPercent=rowObj.lastValue?Number((Number((Number(ro.value)-Number(rowObj.lastValue))/Number(rowObj.lastValue))*100).toFixed(2)):0;
              }else{
                rowObj.diffVal='-'
                rowObj.diffPercent='-'
              }
              
            }else if(this.unitSelect=='LageUnit'){
              if(ro.value!='-'&&rowObj.lastValue!='-'){
                rowObj.diffVal=Number(Number(Number(ro.lageValue)-Number(rowObj.lastLageValue)).toFixed(2));
                rowObj.diffPercent=rowObj.lastLageValue?Number((Number((Number(ro.lageValue)-Number(rowObj.lastLageValue))/Number(rowObj.lastLageValue))*100).toFixed(2)):0;
              }else{
                rowObj.diffVal='-'
                rowObj.diffPercent='-'
              }
            }
            
            return rowObj
          })
          let formatStr=''
          if(this.dateVal.Type=='日'){
            formatStr='YYYY-MM-DD'
          }
          if(this.dateVal.Type=='月'){
            formatStr='YYYY-MM'
          }
          if(this.dateVal.Type=='年'){
            formatStr='YYYY'
          }
          // console.log(queryParams,this.queryParams,'queryParams,this.queryParams',formatStr);
          let timeObj={
            title:'统计时间',
            lastValue:dayjs(queryParams.beginDate).format(formatStr),
            value:dayjs(this.queryParams.beginDate).format(formatStr),
            lageValue:dayjs(this.queryParams.beginDate).format(formatStr),
            lastLageValue:dayjs(queryParams.beginDate).format(formatStr),
            diffVal:'-',
            diffPercent:'-',
          }
          let titleObj={
            title:'数据指标',
            lastValue:this.totalType=='yoy'?'去年同期':'上期',
            value:'本期',
            lastLageValue:this.totalType=='yoy'?'去年同期':'上期',
            lageValue:'本期',
            diffVal:'差异',
            diffPercent:'比率',
          }
          this.tableDataArray.unshift(timeObj)
          this.tableDataArray.unshift(titleObj)
        });
      // }
      
    },
    echartsFiledChange(){
      if(this.$refs.echartBar){
        this.$refs.echartBar.loadChart(this.echartBarData)
      }
      if(this.$refs.echartXBar){
        this.$refs.echartXBar.loadChart(this.echartXBarData)
      }
    },
    unitChange(val){
        this.$emit('unitChange',val)
    },
    returndayjs(val,formatstr){
      return dayjs(val).format(formatstr)
    },
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
    // min-height: 500px;
    width: 50%;
    border-right: 1px solid rgba(255, 255, 255, 0.1);
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    .table_ul{
      display: flex;
      background: rgba(25, 33, 45, 1);
      width: 100%;
      min-width: 500px;
      &.oddNumber{
        background: rgba(34, 46, 64, 1);
      }
      .table_li{
        width: 17%;
        height: 40px;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        color: rgba(255, 255, 255, 1);
        font-size: 14px;
        border-left: 1px solid rgba(255, 255, 255, 0.1);
        padding-left: 16px;
        box-sizing: border-box;
        &.time_li{
          width: 23.5%;
          height: 40px;
        }
        &.first{
          width: 18%;
        }
        &.last{
          width: 15%;
          .red{
            color: rgba(241, 92, 92, 1);
          }
          .green{
            color: rgba(61, 185, 143, 1);
          }
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
  min-width: 930px;
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