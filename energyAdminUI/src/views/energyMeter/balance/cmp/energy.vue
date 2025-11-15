<template>
  <div :style="{'min-height':tableConHeight+'px'}">
    <div class="from_con" id="from_con">
        <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
            <div class="biaodan_input_con">
                <!-- <el-form-item label="类型" prop="factorId">
                    <el-select class="set_radius" style="width:140px" v-model="benchmarkingType" placeholder="请选择" @change="loadLineTableData">
                        <el-option :label="it.label" :value="it.value" v-for="(it,ix) in benchmarkingTypeList" :key="'Type'+ix"/>
                    </el-select>
                </el-form-item> -->
                <el-form-item label="能源类型" prop="factorId">
                    <el-select class="set_radius" style="width:140px" v-model="queryParams.factorId" placeholder="请选择">
                        <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
                    </el-select>
                </el-form-item>
                <el-form-item label="时间选择" prop="equipmentName">
                    <div class="form_content_con">
                    <el-select class="set_radius" style="width:52px" v-model="dateVal.Type" placeholder="选" @change="dateTypeChange">
                        <!-- <el-option label="日" value="日"/> -->
                        <el-option label="月" value="月"/>
                        <el-option label="年" value="年"/>
                    </el-select>
                    <el-date-picker :clearable="false" @change="dateValChange" style="margin-left:4px;width:120px" v-model="dateVal.val" :type="dateVal.Type=='年'?'year':dateVal.Type=='月'?'month':'date'" :placeholder="dateVal.Type==3?'选择年':dateVal.Type==2?'选择月':'选择日期'"></el-date-picker>
                    </div>
                </el-form-item>
            </div>
            <el-form-item class="button_con">
                <!-- <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button> -->
                <el-button type="primary" icon="el-icon-search" @click="loadEnergyFlow">搜索</el-button>
            </el-form-item>
        </el-form>
    </div>
    <echartTree ref="echartTree" :allData="echartTreeData"></echartTree>
    <strategy ref="strategy" :strategyList="strategyList" :factorName="factorName"></strategy>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import echartTree from './echartTree.vue'
import strategy from './strategy.vue'
import {EnergyTree} from '@/api/energy/energyMeter';
import {selectFactorTypeOrg} from "@/api/energy/factorLibrary";
var dayjs = require("@/utils/day.js");
export default {
  name: 'EnergyAdminUIEnergy',
  components:{echartTree,strategy},
  mixins: [resizeTableCon],
  data() {
    return {
      dateVal:{
        Type:'月',
        val:''
      },
      queryParams:{},
      OrgEngryList:[],
      echartTreeData:{},
      strategyList:[]
    };
  },

  mounted() {
    this.dateVal.val=dayjs().format('YYYY-MM')
    this.dateValChange()
    this.orgId = this.$store.state.user.orgId;
    this.loadSelectFactorOrg()
    
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
  methods: {
    loadEnergyFlow(){
      //能流数据
      this.queryParams.orgId=this.orgId
      EnergyTree(this.queryParams).then(res=>{
        // console.log(res,'能流树');
        this.orgEnergyList=JSON.parse(JSON.stringify(res.data))
        this.echartTreeData=[]
        this.strategyList=[]
        let treedata=this.loadTreeData(this.orgEnergyList)
        this.echartTreeData=treedata&&treedata[0]?JSON.parse(JSON.stringify(treedata[0])):{}
      })
    },
    loadTreeData(list,parentTotal){
      //处理树形数据
      let dataArr=JSON.parse(JSON.stringify(list))
      try {
        let resList=dataArr.map(row=>{
            let rowObj={}
            rowObj.name=row.FacilityName;
            rowObj.value=row.UseVale;
            rowObj.unit=row.Unit;
            if(parentTotal==undefined){
              parentTotal=row.UseVale;
            }else{
              rowObj.parentTotal=parentTotal
            }
          if(row.Children&&row.Children.length>0){
            rowObj.children=this.loadTreeData(row.Children,parentTotal)
          }
          rowObj.pence=0
          if(rowObj.value>0){
              rowObj.pence=(Number((Number(rowObj.value)/Number(rowObj.parentTotal)).toFixed(4))*100).toFixed(2)
          }
          if(!row.EquipmentType&&row.FacilityType){
            this.strategyList.push(rowObj)
          }
          return rowObj
        })
        return resList
      } catch (error) {
        // 捕获到错误后的处理逻辑
        console.log(error,'errorerror');
      }
    },
    dateTypeChange(){
      this.dateVal.val=''
    },
    async dateValChange(){
      
      if(this.dateVal.val){
        if(this.dateVal.Type=='日'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('date').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('date').format('YYYY-MM-DD HH:mm:ss')
        }else if(this.dateVal.Type=='月'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('month').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('month').format('YYYY-MM-DD HH:mm:ss')
        }else if(this.dateVal.Type=='年'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('year').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('year').format('YYYY-MM-DD HH:mm:ss')
        }
        this.queryParams.dateType=this.dateVal.Type
      }else{
        delete this.queryParams.beginDate
        delete this.queryParams.endDate
        delete this.queryParams.dateType
      }
    },
    async loadSelectFactorOrg(){
      let res=await selectFactorTypeOrg({OrgId:this.orgId})
      // console.log(res,'res');
      this.OrgEngryList=res.data
      if(this.OrgEngryList[0]){
        this.queryParams.factorId=this.OrgEngryList[0].Id
      }
      this.loadEnergyFlow()
    },
  },
};
</script>

<style lang="less" scoped>

</style>