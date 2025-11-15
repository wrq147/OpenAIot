<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="from_con" id="from_con">
      <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
          <div class="biaodan_input_con">
              <!-- <el-form-item label="类型" prop="factorId">
                  <el-select class="set_radius" style="width:140px" v-model="benchmarkingType" placeholder="请选择" @change="loadLineTableData">
                      <el-option :label="it.label" :value="it.value" v-for="(it,ix) in benchmarkingTypeList" :key="'Type'+ix"/>
                  </el-select>
              </el-form-item> -->
              <el-form-item label="单元范围" prop="equipmentCode">
                <treeselect @input="handleInput" :flat="true" :default-expand-level="1" :limit="6" style="min-width:140px;max-width:360px" :multiple="true" v-model="scopeValue" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择分组" />
              </el-form-item>
              <el-form-item label="能源类型" prop="factorId">
                <el-select class="set_radius" style="width:140px" v-model="queryParams.factorId" placeholder="请选择">
                    <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
                </el-select>
              </el-form-item>
              <!-- <el-form-item label="产品名称" prop="productId">
                <el-select class="set_radius" v-model="queryParams.productId" filterable placeholder="请选择产品名称">
                    <el-option v-for="item in productList" :key="item.Id" :label="item.ProductName" :value="item.Id" />
                </el-select>
              </el-form-item> -->
              <el-form-item label="时间选择" prop="equipmentName">
                <div class="form_content_con">
                  <el-select class="set_radius" style="width:52px" v-model="dateVal.Type" placeholder="选" @change="dateTypeChange">
                      <!-- <el-option label="日" value="日"/> -->
                      <el-option label="月" value="月"/>
                      <el-option label="年" value="年"/>
                  </el-select>
                  <el-date-picker @change="dateValChange" style="margin-left:4px;width:120px" v-model="dateVal.val" :type="dateVal.Type=='年'?'year':dateVal.Type=='月'?'month':'date'" :placeholder="dateVal.Type==3?'选择年':dateVal.Type==2?'选择月':'选择日期'"></el-date-picker>
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
      <div class="trend_chart_title">
        <div class="title_text"><img src="@/assets/images/zs.png" alt=""><span>对标趋势图分析</span></div>
        <el-select class="set_radius" style="width: 110px;" v-model="trendType" @change="loadLineTableData">
          <el-option label="能源消耗" value="UseVale" />
          <el-option label="成本" value="CostVale" />
          <el-option label="标准煤" value="ConvertCoal" />
          <el-option label="碳排放" value="CarbonEmission" />
        </el-select>
      </div>
      <echartLine :trendTypeList="trendTypeList" :style="{'min-height':(tableConHeight/2-14)+'px'}" ref="echartLine" :allData="echartData" :dataType="dateVal.Type" :trendType="trendType"></echartLine>
      <el-table class="data_table" :data="tableData" style="width:100%;margin-top:20px">
        <el-table-column label="对标设施" align="center" key="FacilityName" prop="FacilityName" :show-overflow-tooltip="true"/>
        <el-table-column label="能源消耗" align="center" key="UseVale" prop="UseVale" :show-overflow-tooltip="true">
          <template slot-scope="scope">
            <div>{{scope.row.UseVale}}<span v-if="scope.row.Unit">{{scope.row.Unit}}</span></div>
          </template>
        </el-table-column>
        <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" :show-overflow-tooltip="true" v-if="queryParams.productId"/>
        <el-table-column label="能源成本" align="center" key="CostVale" prop="CostVale" :show-overflow-tooltip="true"/>
        <el-table-column label="标准煤" align="center" key="ConvertCoal" prop="ConvertCoal" :show-overflow-tooltip="true"/>
        <el-table-column label="碳排放" align="center" key="CarbonEmission" prop="CarbonEmission" :show-overflow-tooltip="true"/>
      </el-table>
    </div>

  </div>
</template>

<script>
var dayjs = require("@/utils/day.js");
import {selectFactorTypeOrg} from "@/api/energy/factorLibrary";
import {selectFacilityEquipmentTree,benchEnergyDate} from '@/api/energy/energyMeter'
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import { facilityProductList } from '@/api/energy/product';
import echartLine from './cmp/echartLine.vue'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: 'EnergyAdminUIIndex',
  components: { Treeselect,echartLine },
  mixins: [resizeTableCon],
  data() {
    return {
      trendTypeList:[{
        filed:'UseVale',
        label:'能源消耗'
      },{
        filed:'CostVale',
        label:'成本'
      },{
        filed:'ConvertCoal',
        label:'标准煤'
      },{
        filed:'CarbonEmission',
        label:'碳排放'
      }],
      trendType:'UseVale',
      scopeValue:null,
      queryParams:{
        benchType:'单元对标'
      },
      dateVal:{
        Type:'月',
        val:''
      },
      benchmarkingTypeList:[
        {label:'设备对标',value:'1'},
        {label:'单元对标',value:'2'},
      ],
      benchmarkingType:'',
      OrgEngryList:[],
      groupTreeList:[],
      deviceList:[],//设备列表
      facilityList:[],//设施列表
      productList: [],
      echartData:[],
      tableData:[],
    };
  },

  async mounted() {
    this.dateVal.val=dayjs().format('YYYY-MM')
    await this.dateValChange()
    this.orgId = this.$store.state.user.orgId;
    this.queryParams.OrgId=this.orgId
    await this.loadSelectFactorOrg()
    await this.loadTreeList()
    this.loadLineTableData()
  },

  methods: {
    loadLineTableData(){
      let queryParams=JSON.parse(JSON.stringify(this.queryParams))
      queryParams.ids= this.scopeValue&&this.scopeValue.length>0?this.scopeValue.join(','):''
      this.tableData=[]
      this.trendTypeList=[{
        filed:'UseVale',
        label:'能源消耗'
      },{
        filed:'CostVale',
        label:'成本(元)'
      },{
        filed:'ConvertCoal',
        label:'标准煤(tCO₂e)'
      },{
        filed:'CarbonEmission',
        label:'碳排放(kgce)'
      }]
      benchEnergyDate(queryParams).then(res=>{
        let energyUnit=''
        // console.log(res,'resres111111111111');
        let echartData=res.data.map(row=>{
          if(this.queryParams.factorId&&row.Unit){
            energyUnit=row.Unit
          }
          let rowObj={
            FacilityName: row.FacilityName,
            LageUnit: row.LageUnit,
            Unit: row.Unit,
            Details: row.Details
          }
          let rowObj2={
            FacilityName: row.FacilityName,
            LageUnit: row.LageUnit,
            Unit: row.Unit,
            UseVale:row.UseVale,
            CostVale:row.CostVale+'元',
            ProductName:row.ProductName,
            CarbonEmission:row.CarbonEmission+'tCO₂e',
            ConvertCoal:row.ConvertCoal+'kgce'
          }
          this.tableData.push(rowObj2)
          return rowObj
        })
        if(energyUnit){
          this.trendTypeList.map(row=>{
            if(row.filed=='UseVale'){
              row.label=row.label+'('+energyUnit+')'
            }
          })
        }
        this.echartData=JSON.parse(JSON.stringify(echartData))
      })
    },
    async getProductList(currentKey) {
      let queryParams=JSON.parse(JSON.stringify(this.queryParams))
      queryParams.facilityId = this.scopeValue&&this.scopeValue.length>0?this.scopeValue.join(','):''
      let res=await facilityProductList(queryParams)
      this.productList = res.data;
      this.queryParams.productId = this.productList.length > 0 ? this.productList[0].Id : '';
    },
    handleInput(value) {
        // 如果选择数量未超过最大限制，则更新selectedItems
        if(value){
            if (value.length <= 3) {
                this.scopeValue = value;
            } else {
                // 如果超过最大限制，可以选择最后一个选项，并弹出提示或警告
                this.scopeValue = this.scopeValue.slice(0, 3); // 保持之前的选项不变，只更新最后一个选项
                this.$message.warning("你不能选择超过3个选项");// 弹出警告信息
            }
        }
        
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
    async loadTreeList(){
      //
      let res=await selectFacilityEquipmentTree({OrgId:this.orgId})
      console.log(res,'facilityTree');
      // this.treeList = res.data; //选择分类时分类树
      this.groupTreeList= JSON.parse(JSON.stringify(res.data));
      if(this.groupTreeList[0]){
        this.scopeValue=[this.groupTreeList[0].Id]
      }
      this.$forceUpdate()
    },
    async loadSelectFactorOrg(){
      let res=await selectFactorTypeOrg({OrgId:this.orgId})
      // console.log(res,'res');
      this.OrgEngryList=res.data
      if(this.OrgEngryList[0]){
        this.queryParams.factorId=this.OrgEngryList[0].Id
      }
    },
    resetQuery(){

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
      // await this.getProductList()
    },
  },
};
</script>

<style lang="less" scoped>
.trend_chart{
  margin-top: 10px;
  background-color: #19212d;
  padding: 20px;
  width: 100%;
  box-sizing: border-box;
  position: relative;
  .trend_chart_title{
    width: 100%;
    display: flex;
    justify-content: space-between;
    position: absolute;
    left: 0;
    top: 20px;
    padding: 0 20px;
    box-sizing: border-box;
    .title_text{
      display: flex;
      align-items: center;
      line-height: 16px;
      color: #ffffff;
      font-size: 16px;
      img{
        margin-right: 10px;
      }
    }
    .set_radius{
      height: 28px;
      line-height: 28px;
    }
    .el-select{
      ::v-deep .el-input{
        font-size: 12px;
        .el-input__inner{
          height: 28px;
          line-height: 28px;
        }
        .el-select__caret.el-input__icon{
          height: 26px;
          line-height: 26px;
          font-size: 10px;
        }
      }
    }
  }
  
}
::v-deep .vue-treeselect__menu{
  overflow: auto;
  width: 160px;
}
::v-deep .vue-treeselect__label{
  overflow: unset;
  text-overflow: unset;
}
::v-deep .vue-treeselect div, .vue-treeselect span{
  box-sizing:content-box;
}
::v-deep .vue-treeselect {
  &.vue-treeselect--has-value{
    .vue-treeselect__multi-value {
      margin-bottom: 1px;
    }
  }
    .vue-treeselect__control{
        // height: 20px;
        // line-height: 20px;
    }
    .vue-treeselect__menu-container{
        .vue-treeselect__menu{
            &::-webkit-scrollbar {
                width: 6px;
                height: 6px;
            }
            
            /* 修改滚动条轨道 */
            &::-webkit-scrollbar-track {
                background: rgba(25, 33, 45, 1);
            }
            
            /* 修改滚动条滑块 */
            &::-webkit-scrollbar-thumb {
                background: rgba(255, 255, 255, 0.2);
                border-radius: 5px;
            }
            
            /* 修改滑块在鼠标悬浮时的样式 */
            &::-webkit-scrollbar-thumb:hover {
                background: rgba(255, 255, 255, 0.2);
            }
        }
    }
}

::v-deep .vue-treeselect__value-container {
    display: flex;
    align-items: center;
    overflow-x: auto;
    line-height: 24px;
    .vue-treeselect__multi-value{
        display: flex;
        line-height: 24px;
        .vue-treeselect__multi-value-item-container{
            .vue-treeselect__multi-value-item{
                line-height: 20px;
            }
            .vue-treeselect__multi-value-label{
                word-break: keep-all;
                white-space:nowrap;
            }
        }
        .vue-treeselect__limit-tip{
            .vue-treeselect__limit-tip-text{
                word-break: keep-all;
                white-space:nowrap;
            }
        }
        
    }
    &::-webkit-scrollbar {
        width: 6px;
        height: 6px;
    }
    
    /* 修改滚动条轨道 */
    &::-webkit-scrollbar-track {
        background: rgba(25, 33, 45, 1);
    }
    
    /* 修改滚动条滑块 */
    &::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.2);
        border-radius: 5px;
    }
    
    /* 修改滑块在鼠标悬浮时的样式 */
    &::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.2);
    }
}
</style>