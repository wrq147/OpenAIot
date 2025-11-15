<template>
    <div>
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <!-- <el-form-item label="类型" prop="factorId">
                        <el-select class="set_radius" style="width:140px" v-model="benchmarkingType" placeholder="请选择" @change="loadLineTableData">
                            <el-option :label="it.label" :value="it.value" v-for="(it,ix) in benchmarkingTypeList" :key="'Type'+ix"/>
                        </el-select>
                    </el-form-item> -->
                    <el-form-item label="设施" prop="equipmentCode">
                        <treeselect :flat="true" style="width:140px;" :multiple="false" v-model="queryParams.facilityId" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择分组" />
                    </el-form-item>
                    <el-form-item label="时间选择" prop="equipmentName">
                        <div class="form_content_con">
                        <el-date-picker :clearable="false" @change="dateValChange" style="margin-left:4px;width:120px" v-model="dateVal.val" type="month" placeholder="选择月"></el-date-picker>
                        </div>
                    </el-form-item>
                </div>
                <el-form-item class="button_con">
                    <!-- <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button> -->
                    <el-button type="primary" icon="el-icon-search" @click="loadLineTableData">搜索</el-button>
                </el-form-item>
            </el-form>
        </div>
        <div class="trend_chart" :style="{'min-height':(tableConHeight-10)+'px'}">
          <div class="trend_title" style="margin-top: 10px;margin-bottom: 10px;">
            <div class="line"></div>
            <div class="text">每日最大需量分析</div>
          </div>
          <echartBar ref="echartBar" :allData="echartData"></echartBar>
          <div class="trend_title" style="margin-top: 20px;margin-bottom: 20px;">
            <div class="line"></div>
            <div class="text">每日需量明细</div>
          </div>
          <div class="table_ul">
            <div class="table_li" v-for="(ite,inx) in tableData" :key="'xuliang'+inx" :class="{'hasbottom':inx>=tableData.length-4}">
              <div class="table_title_con" v-if="inx<4">
                <div class="table_title_li">日期</div>
                <div class="table_title_li">最大需量</div>
                <div class="table_title_li">时间段</div>
              </div>
              <div class="table_data_con">
                <div class="table_data_li">{{ite.date}}</div>
                <div class="table_data_li">{{ite.UseVale}}</div>
                <div class="table_data_li">{{ite.TTime}}</div>
              </div>
            </div>
            <div class="table_li noborder" v-for="(it,inx) in nullArr" :key="'kong'+inx" :class="{'firstno':inx==0}"></div>
          </div>
        </div>
        <demandStrategy :queryParams="queryParams" v-if="queryParams.OrgId&&queryParams.facilityId"></demandStrategy>
    </div>
</template>

<script>
var dayjs = require("@/utils/day.js");
import echartBar from './echartBar.vue'
import demandStrategy from './demandStrategy.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import {facilityTree} from '@/api/energy/facility'
import {selectEnergyHour} from '@/api/energy/energyMeter'
export default {
  name: 'EnergyAdminUIDemand',
  components:{echartBar,Treeselect,demandStrategy},
  mixins: [resizeTableCon],
  data() {
    return {
      tableData:[],
      echartData:[],
      queryParams:{},
      dateVal:{
        Type:'月',
        val:''
      },
      groupTreeList:[],
      nullArr:[]
    };
  },

  async mounted() {
    this.dateVal.val=dayjs().format('YYYY-MM')
    await this.dateValChange()
    this.orgId = this.$store.state.user.orgId;
    this.queryParams.OrgId=this.orgId
    await this.loadTreeList()
    this.loadLineTableData()
  },
  computed:{
  },
  methods: {
    loadLineTableData(){
      if(!this.dateVal.val){
        this.$modal.msgWarning("请选择分析的时间段");
        return
      }
      let query=JSON.parse(JSON.stringify(this.queryParams))
      this.queryParams=JSON.parse(JSON.stringify(query))
      selectEnergyHour(this.queryParams).then(res=>{
        // console.log('数据结果',res);
        let tableData=JSON.parse(JSON.stringify(res.data))
        this.tableData=tableData.map(row=>{
          row.date=dayjs(row.DDate).format('MM-DD')
          return row
        })
        let result=tableData.length%4
        this.nullArr=new Array(result).fill('');
        this.echartData=JSON.parse(JSON.stringify(res.data))
      })
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.FacilityName,
        // isDisabled: node.isDisabled,
        children: node.Children
      };
    },
    async dateValChange(){
      if(this.dateVal.val){
        this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('month').format('YYYY-MM-DD HH:mm:ss')
        this.queryParams.endDate=dayjs(this.dateVal.val).endOf('month').format('YYYY-MM-DD HH:mm:ss')
        // this.queryParams.dateType=this.dateVal.Type
      }else{
        delete this.queryParams.beginDate
        delete this.queryParams.endDate
        // delete this.queryParams.dateType
      }
    },
    async loadTreeList(){
      //
      let res=await facilityTree({OrgId:this.orgId})
      // console.log(res,'facilityTree');
      // this.treeList = res.data; //选择分类时分类树
      this.groupTreeList= JSON.parse(JSON.stringify(res.data));
      if(this.groupTreeList[0]){
        this.queryParams.facilityId=this.groupTreeList[0].Id
      }
    },
  },
};
</script>

<style lang="less" scoped>
.trend_chart{
  padding-bottom: 20px;
  .trend_title{
    color: #ffffff;
    display: flex;
    align-items: flex-end;
    
    .line{
      width: 5px;
      height: 20px;
      border-radius: 2px;
      background: rgb(42, 211, 154);
      margin-right: 10px;
    }
  }
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