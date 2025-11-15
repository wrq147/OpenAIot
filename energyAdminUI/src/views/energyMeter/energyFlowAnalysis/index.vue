<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
      <div class="from_con" id="from_con">
        <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
          <div class="biaodan_input_con">
            <el-form-item label="范围选择" prop="equipmentCode">
              <treeselect @input="loadTreeData(orgEnergyList)" style="width:140px" :multiple="false" v-model="scopeValue" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择分组" />
            </el-form-item>
            <el-form-item label="时间选择" prop="equipmentName">
              <div class="form_content_con">
                <el-select class="set_radius" style="width:52px" v-model="dateVal.Type" placeholder="选" @change="dateTypeChange">
                  <el-option label="日" value="1"/>
                  <el-option label="月" value="2"/>
                  <el-option label="年" value="3"/>
                </el-select>
                <el-date-picker @change="dateValChange" style="margin-left:4px;width:120px" v-model="dateVal.val" :type="dateVal.Type==3?'year':dateVal.Type==2?'month':'date'" :placeholder="dateVal.Type==3?'选择年':dateVal.Type==2?'选择月':'选择日期'"></el-date-picker>
              </div>
            </el-form-item>
            <el-form-item label="能源类型" prop="factorId">
              <el-select class="set_radius" style="width:140px" v-model="queryParams.factorId" placeholder="请选择" @change="handleQuery">
                <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
              </el-select>
            </el-form-item>
          </div>
          <el-form-item class="button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <div class="date_setting_con">
          <div class="date_time_con">
            <div class="date_title">
              <img src="@/assets/images/zs.png" alt="">
              <div>统计时间</div>
            </div>
            <div class="time_cot">
              ({{returndayjs(queryParams.beginDate,'YYYY/MM/DD HH:mm:ss')+'~'+returndayjs(queryParams.endDate,'YYYY/MM/DD HH:mm:ss')}})
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
                      <div>{{item.name}}</div>
                      <div>{{item.unit}}</div>
                      <div>{{item.lastUnit}}</div>
                  </div>
              </div>
              <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color:rgba(255, 255, 255, 0.6);font-size:12px;"></i>
            </el-tooltip>
            <span class="span">单位换算</span>
            <el-select class="set_radius" style="width: 110px;" v-model="unitSelect" @change="loadTreeData(orgEnergyList)">
              <el-option label="一级单位" value="Unit" />
              <el-option label="二级单位" value="LageUnit" />
            </el-select>
          </div>
        </div>
        <div class="sankey_con" :style="{'min-height':(tableConHeight-64)+'px'}">
          <!-- <div class="relationship_con">
            <i class="zhongtaiiconfont zhongtai-icon-nengxiaopinghengyuyouhua" style="color:rgba(255, 255, 255, 0.6);font-size:12px;"></i>
            <span>能效平衡与优化</span>
          </div> -->
          <div class="sankey_echarts" :style="{'height':(tableConHeight-128)+'px'}"></div>
        </div>
        
      </div>
    </div>
</template>

<script>
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {facilityTree} from '@/api/energy/facility'
import {selectFactorTypeOrg} from "@/api/energy/factorLibrary";
var dayjs = require("@/utils/day.js");
import {EnergyTree} from '@/api/energy/energyMeter';
import * as echarts from "echarts5";
export default {
  name: 'EnergyAdminUIEnergyFlowAnalysis',
  components: { Treeselect },
  mixins: [resizeTableCon],
  data() {
    return {
      echartsOptions:{
        tooltip: {
          trigger: 'item',
          triggerOn: 'mousemove',
          backgroundColor:'rgba(0, 0, 0, 1)',
          padding:0,
        },
        series: [
          {
            type: 'sankey',
            data: [],
            links: [],
            emphasis: {
              focus: 'adjacency'
            },
            nodeAlign: "right",
            levels: [
              {
                depth: 0,
                itemStyle: {
                  color: 'rgba(61, 185, 143, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              },
              {
                depth: 1,
                itemStyle: {
                  color: 'rgba(54, 183, 231, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              },
              {
                depth: 2,
                itemStyle: {
                  color: 'rgba(149, 197, 187, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              },
              {
                depth: 3,
                itemStyle: {
                  color: 'rgba(190, 197, 149, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              },
              {
                depth: 4,
                itemStyle: {
                  color: 'rgba(197, 159, 149, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              },
              {
                depth: 5,
                itemStyle: {
                  color: 'rgba(197, 149, 185, 1)',
                  borderWidth:0
                },
                lineStyle: {
                  color: 'gradient',
                  opacity: 0.8
                }
              }
            ],
            // lineStyle: {
            //   color: 'source',
            //   curveness: 0.5
            // }
          }
        ]
      },
      unitSelect:'Unit',
      scopeValue:null,//范围值
      tipsValue:true,
      queryParams:{
        factorId:'',
      },
      OrgEngryList:[],
      groupTreeList:[],
      orgId:'',
      dateVal:{
        Type:'2',
        val:''
      },
      unitList: [
        { name: '电', unit: 'KW(千瓦)', lastUnit: 'MW(万瓦)' },
        { name: '天然气', unit: 'Nm³(标准m³)', lastUnit: '万Nm³' },
        { name: '金额', unit: '元', lastUnit: '万元' },
        { name: '标准煤', unit: 'kgce', lastUnit: 'tce' },
        { name: '碳排放', unit: 'kgCO₂e', lastUnit: 'tCO₂e' },
      ],
      groupmap:new Map(),
      orgEnergyList:[],
      echartsData:[],
      echartslevels:[],
      echartslinks:[],
    };
  },
  mounted() {
    this.dateVal.val=dayjs().format('YYYY-MM')
    this.dateValChange()
    this.orgId = this.$store.state.user.orgId;
    this.loadSelectFactorOrg()
    this.loadTreeList()
    
  },

  methods: {
    loadEnergyFlow(){
      //能流数据
      this.queryParams.orgId=this.orgId
      EnergyTree(this.queryParams).then(res=>{
        // console.log(res,'能流树');
        this.orgEnergyList=JSON.parse(JSON.stringify(res.data))
        this.loadTreeData(this.orgEnergyList)
        
      })
    },
    drawSankey(){
      this.echartsOptions.tooltip.formatter=(params)=>{
        // console.log(params,'params');
        if(params){
          if(this.echartsData&&this.echartsData[params.dataIndex]){
            if(this.unitSelect=='Unit'){
              return `<div style="padding:16px;border-radius:4px;background:rgba(0, 0, 0, 1);box-shadow: 0px 10px 20px 0px rgba(0,0,0,0.4);color:rgba(255, 255, 255, 1);font-size:14px;line-height:14px;">
                <div style="display:flex;align-items:center;">
                  <div style="background:rgba(61, 185, 143, 1);border-radius:2px;width:4px;height:14px;"></div>
                  <span style="margin-left:8px">${params.name}</span>
                </div>
                <div style="display:flex;align-items:center;margin-top:10px">
                  <span style="color:rgba(255, 255, 255, 0.6)">能源消耗：</span>
                  <span>${params.value}</span>
                  <span style="margin-left:2px">${params.data.Unit}</span>
                </div>
              </div>`
            }else if(this.unitSelect=='LageUnit'){
              params.value=Number((Number(params.value)/10000).toFixed(6))
              return `<div style="padding:16px;border-radius:4px;background:rgba(0, 0, 0, 1);box-shadow: 0px 10px 20px 0px rgba(0,0,0,0.4);color:rgba(255, 255, 255, 1);font-size:14px;line-height:14px;">
                <div style="display:flex;align-items:center;">
                  <div style="background:rgba(61, 185, 143, 1);border-radius:2px;width:4px;height:14px;"></div>
                  <span style="margin-left:8px">${params.name}</span>
                </div>
                <div style="display:flex;align-items:center;margin-top:10px">
                  <span style="color:rgba(255, 255, 255, 0.6)">能源消耗：</span>
                  <span>${params.value}</span>
                  <span style="margin-left:2px">${params.data.LageUnit}</span>
                </div>
              </div>`
            }
          }
        }
        
      }
      this.echartsOptions.series[0].data=this.echartsData
      this.echartsOptions.series[0].links=this.echartslinks
      let Chart1 = echarts.init(
        document.querySelector(".sankey_con .sankey_echarts")
      );
      Chart1.setOption(this.echartsOptions);
      let that=this
      window.addEventListener('resize', function() {
        Chart1.setOption(that.echartsOptions);
        Chart1.resize();
      });
    },
    loadTreeData(list){
      //处理树形数据
      let dataArr=JSON.parse(JSON.stringify(list))
      if(this.scopeValue){}else{
        if(this.groupTreeList[0]){
          this.scopeValue=this.groupTreeList[0].Id
        }else{
          return []
        }
      }
      let realTree=[]
      
      try {
        let resList=dataArr.map(row=>{
          if(row.Id==this.scopeValue){
            realTree=[row]
            throw new Error(); // 抛出错误终止循环
          }else{
            if(row.Children&&row.Children.length>0){
              realTree=this.loadTreeData(row.Children)
              if(realTree&&realTree.length>0){
                throw new Error(); // 抛出错误终止循环
              }
            }
          }
        })
      } catch (error) {
        // 捕获到错误后的处理逻辑
      }
      if(realTree&&realTree.length>0){
        this.echartsData=[]
        this.echartslinks=[]
        this.processResTree(realTree)
        this.$nextTick(()=>{
          this.drawSankey()
        })
        // console.log(this.echartsData,'this.echartsData',this.echartslinks);
      }
      return realTree
    },
    processResTree(tree,ParentName,isequip){
      if(tree&&tree.length>0){
        tree.map(row=>{
          if(isequip){
            this.echartsData.push({name:row.EquipmentName,LageUnit:row.LageUnit,Unit:row.Unit})
          }else{
            this.echartsData.push({name:row.FacilityName,LageUnit:row.LageUnit,Unit:row.Unit})
          }
          
          if(isequip){
            this.echartslinks.push({
              source: ParentName, target: row.EquipmentName, value: row.UseVale,LageUnit:row.LageUnit,Unit:row.Unit
            })
          }else if(row.ParentId&&row.ParentId!='-'){
            // if(row.LageUnit){
            //   this.LageUnit=row.LageUnit
            // }
            // if(row.Unit){
            //   this.Unit=row.Unit
            // }
            this.echartslinks.push({
              source: ParentName, target: row.FacilityName, value: row.UseVale,LageUnit:row.LageUnit,Unit:row.Unit
            })
          }
          if(row.Children&&row.Children.length>0){
            this.processResTree(row.Children,row.FacilityName)
          }else if(row.Equipments&&row.Equipments.length>0){
            this.processResTree(row.Equipments,row.FacilityName,true)
          }
        })

      }
    },
    returndayjs(val,formatstr){
      return dayjs(val).format(formatstr)
    },
    dateValChange(){
      if(this.dateVal.val){
        if(this.dateVal.Type=='1'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('date').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('date').format('YYYY-MM-DD HH:mm:ss')
        }else if(this.dateVal.Type=='2'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('month').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('month').format('YYYY-MM-DD HH:mm:ss')
        }else if(this.dateVal.Type=='3'){
          this.queryParams.beginDate=dayjs(this.dateVal.val).startOf('year').format('YYYY-MM-DD HH:mm:ss')
          this.queryParams.endDate=dayjs(this.dateVal.val).endOf('year').format('YYYY-MM-DD HH:mm:ss')
        }
      }else{
        delete this.queryParams.beginDate
        delete this.queryParams.endDate
      }
    },
    dateTypeChange(){
      this.dateVal.val=''
    },
    loadSelectFactorOrg(){
      selectFactorTypeOrg({OrgId:this.orgId}).then(res=>{
        // console.log(res,'res');
        this.OrgEngryList=res.data
        if(this.OrgEngryList[0]){
          this.queryParams.factorId=this.OrgEngryList[0].Id
        }
        
      })
    },
    loadTreeList(){
      //
      facilityTree({OrgId:this.orgId}).then(res=>{
        // console.log(res,'facilityTree');
        // this.treeList = res.data; //选择分类时分类树
        this.groupTreeList= this.setSelDis(res.data);
        if(this.groupTreeList[0]){
          this.scopeValue=this.groupTreeList[0].Id
        }
        this.loadEnergyFlow()
        this.$forceUpdate()
        this.initClassMap(this.groupTreeList);
      })
    },
    setSelDis(list){
      list=list.filter(row=>!row.Manager||row.Manager&&row.Manager=='-')
      for(let i=0;i<list.length;i++){
        if(list[i].Children&&list[i].Children.length>0){
          list[i].Children=this.setSelDis(list[i].Children)
        }
      }
      return list
    },
    initClassMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        // console.log("组合时用户列表curnode",curnode);
        this.groupmap.set(node[idx].Id, curnode);
        
        if (curnode.hasOwnProperty("Children") && curnode.Children && curnode.Children.length > 0) {
          this.initClassMap(curnode.Children);
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
        isDisabled: node.isDisabled,
        children: node.Children
      };
    },
    resetQuery(){
      this.dateVal={
        Type:'2',
        val:''
      }
      this.dateVal.val=dayjs().format('YYYY-MM')
      this.loadEnergyFlow()
    },
    handleQuery(){
      this.loadEnergyFlow()
    }
  },
};
</script>

<style lang="less" scoped>
.groupSet {
  ::v-deep .vue-treeselect__input-container {
    display: flex;
    align-items: center;
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
.form_content_con{
  display: flex;
  align-items: center;
}
.elbiaoge_elform{
  padding-left: 20px;
  padding-right: 20px;
  padding-bottom: 20px;
}
.sankey_con{
  width: 100%;
  padding: 16px;
  box-sizing: border-box;
  height: 100%;
  background: rgba(19, 25, 34, 1);
  margin-top: 16px;
  border-radius: 4px;
  .relationship_con{
    width: 136px;
    box-sizing: border-box;
    padding: 9px 10px;
    font-size: 14px;
    background: rgba(34, 46, 64, 1);
    display: flex;
    align-items: center;
    border-radius: 4px;
    color: rgba(255, 255, 255, 1);
    i{
      margin-right: 6px;
    }
  }
  .sankey_echarts{
    height:calc(100% - 64px);
    width: 100%;
  }
}
.date_setting_con{
  display: flex;
  justify-content: space-between;
  align-items: center;
  .date_time_con{
    display: flex;
    align-items: center;
    
    .date_title{
      display: flex;
      align-items: center;
      line-height: 16px;
      color: rgba(255, 255, 255, 1);
      font-size: 16px;
      img{
        width: 16px;
        height: 16px;
      }
      div{
        margin-left: 8px;
        margin-right: 8px;
      }
    }
    .time_cot{
      font-size: 14px;
      color: rgba(255, 255, 255, 0.6);
    }
  }
  .setting_con{
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
    .span{
      font-size: 12px;
      margin-left: 4px;
      margin-right: 8px;
      color: rgba(255, 255, 255, 1);
    }
  }
}

.unitSelect_content{
  width: 300px;
  height: 168px;
  background: #000000;
  .tab{
      display: flex;
      height: 28px;
      border-bottom: 1px solid rgba(255, 255, 255, 0.2);
      >div{
          flex: 1;
          text-align: center;
          height: 100%;
          line-height: 28px;
          font-weight: 500;
          font-size: 12px;
          color: rgba(255, 255, 255, 0.6);
      }
  }
  .tab_content{
      height: 28px;
      display: flex;
      >div{
          flex: 1;
          text-align: center;
          height: 100%;
          line-height: 28px;
          font-weight: 500;
          font-size: 12px;
          color: #FFFFFF;
      }

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