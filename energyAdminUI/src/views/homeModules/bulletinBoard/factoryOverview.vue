<template>
  <div class="engry_con">
    <!-- <div class="engry_bg_img">
      <img class="img" src="~@/assets/images/quack_bg.png" alt="">
    </div> -->
    <div class="engry_building_con">
      <img class="img" src="~@/assets/images/building_bg.png" alt="">
    </div>
    <div class="engry_data_con" :style="{'height':'calc(100vh - 60px + '+heiCount+'px)'}">
      <div class="engry_data_type">
        <div class="engry_data_type_li" v-for="ite in countDataList" :key="ite.label">
          <div class="label">{{ite.label}}</div>
          <div class="num">{{ite.num}}</div>
          <div class="unit">{{ite.unit}}</div>
        </div>
      </div>
      <div class="echart_info_con">
        <div class="pie_con">
          <div class="info_title date_title">
            <div class="title">
              <img class="img" src="~@/assets/images/zs.png" alt="">
              <span>能源碳排占比</span>
            </div>
            <div class="right_text">单位：kgCO₂e</div>
          </div>
          <div class="pie_echart"></div>
        </div>
        <div class="collect_cin_con">
          <div class="collect_cin">
            <div class="info_title date_title">
              <div class="title">
                <img class="img" src="~@/assets/images/zs.png" alt="">
                <span>能源排名</span>
              </div>
              <div class="tab_con">
                <div class="tab_li" :class="{'active':engryDataType==1}" @click.stop="engryDataType=1,isReadLoadEngryDate=true,getEngryData()">按设备</div>
                <div class="tab_line"> ｜ </div>
                <div class="tab_li" :class="{'active':engryDataType==2}" @click.stop="engryDataType=2,isReadLoadEngryDate=true,getEngryData()">按车间</div>
              </div>
            </div>
            <el-table v-loading="loading" :data="[]" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_head" :header-cell-style="cellSty" style="width:100%;margin-top:10px;" row-key="keys">
              <el-table-column label="排名" align="center" key="NumInx" prop="NumInx" width="60" :show-overflow-tooltip="true"/>
              <el-table-column v-if="engryDataType==1" label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column v-if="engryDataType==2" label="车间名称" align="center" key="FacilityName" prop="FacilityName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="数据" align="center" key="UseVale" prop="UseVale" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="58" :show-overflow-tooltip="true"/>
            </el-table>
            <SeamlessScroll v-if="engryData&&engryData.length>5" @ScrollEnd="getEngryData" v-loading="loading" class="order_warp" :data="engryData" :loop="true" :class-option="{direction: 1, step: 1.1,waitTime: 1000,openWatch: true}">
              <el-table :show-header="false" v-loading="loading" :data="engryData" :row-style="isRed" :cell-style="isRed" class="data_table home_table" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="keys">
                <el-table-column label="排名" align="center" key="NumInx" prop="NumInx" width="60" :show-overflow-tooltip="true"/>
                <el-table-column v-if="engryDataType==1" label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
                <el-table-column v-if="engryDataType==2" label="车间名称" align="center" key="FacilityName" prop="FacilityName" width="120" :show-overflow-tooltip="true"/>
                <el-table-column label="数据" align="center" key="UseVale" prop="UseVale" width="100" :show-overflow-tooltip="true"/>
                <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="58" :show-overflow-tooltip="true"/>
              </el-table>
            </SeamlessScroll>
            <el-table v-else-if="engryData" :show-header="false" v-loading="loading" :data="engryData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="keys">
              <el-table-column label="排名" align="center" key="NumInx" prop="NumInx" width="60" :show-overflow-tooltip="true"/>
              <el-table-column v-if="engryDataType==1" label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column v-if="engryDataType==2" label="车间名称" align="center" key="FacilityName" prop="FacilityName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="数据" align="center" key="UseVale" prop="UseVale" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="58" :show-overflow-tooltip="true"/>
            </el-table>
          </div>
        </div>
        <div class="collect_cin_con">
          <div class="collect_cin">
            <div class="info_title">
              <img class="img" src="~@/assets/images/zs.png" alt="">
                <span>能效指标</span>
            </div>
            <el-table v-loading="loading" :data="[]" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_head" :header-cell-style="cellSty" style="width:100%;margin-top:10px;" row-key="keys">
              <el-table-column label="序号" align="center" type="index" width="40"/>
              <!-- <el-table-column label="序号" align="center" key="num" prop="num" width="40" :show-overflow-tooltip="true"/> -->
              <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="80" :show-overflow-tooltip="true"/>
              <el-table-column label="产量" align="center" key="OutPut" prop="OutPut" width="80" :show-overflow-tooltip="true"/>
              <el-table-column label="单位产品能耗能效" align="center" key="unitEnergy" prop="unitEnergy" width="138" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ Number((scope.row.UseVale/scope.row.OutPut).toFixed(2)) }}</span>
                </template>
              </el-table-column>
            </el-table>
            <SeamlessScroll v-if="efficiencyData&&efficiencyData.length>5" @ScrollEnd="getIndicatorData" v-loading="loading" class="order_warp" :data="efficiencyData" :loop="true" :class-option="{direction: 1, step: 1.1,waitTime: 1000,openWatch: true}">
              <el-table :show-header="false" v-loading="loading" :data="efficiencyData" :row-style="isRed" :cell-style="isRed" class="data_table home_table" :header-cell-style="cellSty" style="width:100%;margin-top:0;height:auto !important;" row-key="keys">
                <el-table-column label="序号" align="center" type="index" width="40"/>
                <!-- <el-table-column label="序号" align="center" key="num" prop="num" width="40" :show-overflow-tooltip="true"/> -->
                <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="80" :show-overflow-tooltip="true"/>
                <el-table-column label="产量" align="center" key="OutPut" prop="OutPut" width="80" :show-overflow-tooltip="true"/>
                <el-table-column label="单位产品能耗能效" align="center" key="unitEnergy" prop="unitEnergy" width="138" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span>{{ Number((scope.row.UseVale/scope.row.OutPut).toFixed(2)) }}</span>
                  </template>
                </el-table-column>
              </el-table>
            </SeamlessScroll>
            <el-table v-else :show-header="false" v-loading="loading" :data="efficiencyData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="keys">
                <el-table-column label="序号" align="center" type="index" width="40"/>
                <!-- <el-table-column label="序号" align="center" key="num" prop="num" width="40" :show-overflow-tooltip="true"/> -->
                <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="80" :show-overflow-tooltip="true"/>
                <el-table-column label="产量" align="center" key="OutPut" prop="OutPut" width="80" :show-overflow-tooltip="true"/>
                <el-table-column label="单位产品能耗能效" align="center" key="unitEnergy" prop="unitEnergy" width="138" :show-overflow-tooltip="true">
                  <template slot-scope="scope">
                    <span>{{ Number((scope.row.UseVale/scope.row.OutPut).toFixed(2)) }}</span>
                  </template>
                </el-table-column>
              </el-table>
          </div>
        </div>
        <div class="collect_cin_con">
          <div class="collect_cin">
            <div class="info_title">
              <img class="img" src="~@/assets/images/zs.png" alt="">
                <span>产品碳足迹</span>
            </div>
            <el-table v-loading="loading" :data="[]" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_head" :header-cell-style="cellSty" style="width:100%;margin-top:10px;" row-key="keys">
              <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="生命周期边界" align="center" key="BorderTitle" prop="BorderTitle" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="产品碳足迹" align="center" key="BorderName" prop="BorderName" width="138" :show-overflow-tooltip="true"/>
            </el-table>
            <SeamlessScroll v-if="lifeCycle&&lifeCycle.length>0" @ScrollEnd="getCarbonFootprint" v-loading="loading" class="order_warp" :data="lifeCycle" :loop="true" :class-option="{direction: 1, step: 1.1,waitTime: 1000,openWatch: true}">
              <el-table :show-header="false" v-loading="loading" :data="lifeCycle" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="keys">
                <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="100" :show-overflow-tooltip="true"/>
                <el-table-column label="生命周期边界" align="center" key="BorderTitle" prop="BorderTitle" width="100" :show-overflow-tooltip="true"/>
                <el-table-column label="产品碳足迹" align="center" key="BorderName" prop="BorderName" width="138" :show-overflow-tooltip="true"/>
              </el-table>
            </SeamlessScroll>
            <el-table v-else :show-header="false" v-loading="loading" :data="lifeCycle" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="keys">
              <el-table-column label="产品名称" align="center" key="ProductName" prop="ProductName" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="生命周期边界" align="center" key="BorderTitle" prop="BorderTitle" width="100" :show-overflow-tooltip="true"/>
              <el-table-column label="产品碳足迹" align="center" key="BorderName" prop="BorderName" width="138" :show-overflow-tooltip="true"/>
            </el-table>
          </div>
        </div>
      </div>
    </div>
    <div class="abs_con">
      <div class="abs_li first">
        <div class="li_ele">生产车间一</div>
        <div class="li_line"></div>
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
      </div>
      <div class="abs_li right second">
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
        <div class="li_line"></div>
        <div class="li_ele">生产车间二</div>
      </div>
      <div class="abs_li third">
        <div class="li_ele">生产车间三</div>
        <div class="li_line"></div>
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
      </div>
      <div class="abs_li right fouth">
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
        <div class="li_line"></div>
        <div class="li_ele">生产车间四</div>
      </div>
      <div class="abs_li fifth">
        <div class="li_ele">生产车间五</div>
        <div class="li_line"></div>
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
      </div>
      <div class="abs_li right sixed">
        <div class="li_icon">
          <img class="img" src="~@/assets/images/ligh_icon.png" alt="">
        </div>
        <div class="li_line"></div>
        <div class="li_ele">生产车间六</div>
        <div class="abs_info_con">
          <div class="abs_info">
            <div class="info_title abs_info_title">
              <img class="img" src="~@/assets/images/zs.png" alt="">
              <span>生产车间六</span>
            </div>
            <div class="abs_info_li">负责人：孟浩然</div>
            <div class="abs_info_li">联系电话：15980777521</div>
            <div class="close">
              <i class="el-icon-close"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import * as echarts from "echarts5";
import SeamlessScroll from 'vue-seamless-scroll'
import { energyPageDateList,facilityEnergyPageDateList } from '@/api/energy/energyMeter';
import dayjs from 'dayjs';
import {selectProductEnergyPage,homeDevStatisticsInfo} from '@/api/home'
import { modelProductModelPage } from '@/api/energy/cyclicalModel';
export default {
  name: 'EnergyAdminUIFactoryOverview',
  components:{SeamlessScroll},
  props:{
    countDate:{
      type:[String,Date],
      default:''
    },
    countType:{
      type:[String,Number],
      default:''
    }
  },
  data() {
    return {
      loading:false,
      options:{
        title: [{
          text: "",
          subtext:'',
          left: "25%",
          top: "32%",
          textAlign: "center",
          subtextStyle: {
            fill: "rgba(153, 153, 153, 1)",
            fontSize: 12,
            fontWeight: 200,
            color: 'rgba(153, 153, 153, 1)'
          },
          textStyle:{
            align: "center",
            fill: "rgba(51, 51, 51, 1)",
            fontSize: 24,
            fontWeight: 500,
            color: 'rgba(51, 51, 51, 1)'
          }
        }],
        series: [{
          name: '设备状态',
          type: 'pie',
          center: ['50%', '50%'] ,
          radius: ['50%', '78%'] ,
          // roseType: 'area',
          roseType: 'radius',
          minAngle: 5, // 设置最小角度为5度
          itemStyle: {
            borderRadius: 0
          },
          label: {
            show:true,
            position: "outside",
            formatter: (params) => {
              //只有“直接访问”使用大标签，其他都使用小标签
              return params.data.name
            },
            rich: {
              colorBlock: {
                  backgroundColor:'rgba(53, 200, 255, 1)',
                  width: 10, // 块的大小
                  height: 10, // 块的大小
                  align: 'center',
                  borderRadius: 2 // 可选，圆角半径
              },
              colorBlock2: {
                  backgroundColor:'rgba(42, 211, 154, 1)',
                  width: 10, // 块的大小
                  height: 10, // 块的大小
                  align: 'center',
                  borderRadius: 2 // 可选，圆角半径
              },
              namef: {
                color: 'rgba(255, 255, 255, 0.6)',
                height: 12,
                fontSize:12
              },
              valuef: {
                color: 'rgba(255, 255, 255, 1)',
                height: 28,
                fontWeight: "bold"
              },
              percentf: {
                color: 'rgba(255, 255, 255, 1)',
                height: 28,
                fontWeight: "bold"
              },
            }
            
          },
          //饼块起始角度
          startAngle: 150,
          avoidLabelOverlap: false,
          //设置数据标签引导线
          itemStyle:{
            normal:{
              labelLine: {
                show: true,
                length: 6,
                length2: 18,
                minTurnAngle:90,
                lineStyle: {
                  width: 1 //引导线宽度
                },
              },
            }
          },
          data: [{
                "name": "设备类型一",
                "value": 15,
                "itemStyle": {
                    "color": 'rgba(54, 183, 231, 1)',
                }
            }, {
                "name": "设备类型二",
                "value": 22,
                "itemStyle": {
                    "color": 'rgba(42, 211, 154, 1)',
                }
            }]
        }]
      },
      efficiencyData:[],//能效指标
      engryData:[],
      lifeCycle:[],
      isReadLoadEngryDate:true,//能耗
      isReadLoadEfficiencyDate:true,//能效指标
      isReadLoadModelProductData:true,//碳足迹
      engryDataType:1,
      orgId:'',
      countDataList:[]
    };
  },
  watch:{
    countDate:{
      handler(val){
        if(val&&this.countType){
          this.isReadLoadEngryDate=true
          this.isReadLoadEfficiencyDate=true
          this.isReadLoadModelProductData=true
          this.getEngryData()
          this.getIndicatorData()
          this.loadModelProductModelPage()
        }
      },
      immediate:true
    },
    countType:{
      handler(val){
        if(val&&this.countDate){
          this.isReadLoadEngryDate=true
          this.isReadLoadEfficiencyDate=true
          this.isReadLoadModelProductData=true
          this.getEngryData()
          this.getIndicatorData()
          this.loadModelProductModelPage()
        }
      },
      immediate:true
    },
  },
  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.getDevTotal()
    
  },
  computed:{
    isCollapse() {
      return !this.sidebar.opened;
    },
    heiCount(){
      let len=this.engryData.length?this.engryData.length:1
      let len2=this.efficiencyData.length?this.efficiencyData.length:1
      let len3=this.lifeCycle.length?this.lifeCycle.length:1
      if(len+len2+len3>8){
        // console.log(len+len2-8,'len+len2-8');
        if(len+len2+len3-8>=7){
          return 7*32
        }else{
          return (len+len2-8)*32
        }
      }else{
        return 0
      }
    }
  },
  methods: {
    loadModelProductModelPage(){//获取碳足迹
      if(this.isReadLoadModelProductData){
        let queryParams={
          pageNum: 1,
          pageSize: 10000,
          orgId:this.orgId,
        }
        if(this.countDate){
          if(this.countType=='月'){
            queryParams.beginDate = dayjs(this.countDate).startOf('month').format('YYYY-MM-DD HH:mm:ss');
            queryParams.endDate = dayjs(this.countDate).endOf('month').format('YYYY-MM-DD HH:mm:ss');
          }else if(this.countType=='年'){
            queryParams.beginDate = dayjs(this.countDate).startOf('year').format('YYYY-MM-DD HH:mm:ss');
            queryParams.endDate = dayjs(this.countDate).endOf('year').format('YYYY-MM-DD HH:mm:ss');
          }
        }
        modelProductModelPage(queryParams).then(res=>{
          // console.log(res,'res碳足迹');
          this.lifeCycle=res.data.List
          this.isReadLoadModelProductData=false
        })
      }
      
    },
    getDevTotal(){
      //获取设备统计信息
      homeDevStatisticsInfo({orgId:this.orgId}).then(res=>{
        // console.log(res,'res设备统计');
        this.countDataList=[{
          label:'碳排放',
          num:res.data.CarbonEmission,
          unit:'tCO₂e'
        },{
          label:'产值',
          num:res.data.OutValue,
          unit:'万元'
        },{
          label:'碳排放强度',
          num:res.data.OutValue&&res.data.CarbonEmission?Number((res.data.CarbonEmission/res.data.OutValue).toFixed(2)):0,
          unit:'tCO₂e/￥10000'
        }]
        let typeData=res.data.EquipemntTypes.map(row=>{
          let obj={}
          if(row.TypeName&&row.TypeName.indexOf('电力')>-1){
            obj={
                "name": row.TypeName,
                "value": row.CarbonEmission,
                "itemStyle": {
                    "color": 'rgba(54, 183, 231, 1)',
                }
            }
          }else if(row.TypeName&&row.TypeName.indexOf('天然气')>-1){
            obj={
                "name": row.TypeName,
                "value": row.CarbonEmission,
                "itemStyle": {
                    "color": 'rgba(42, 211, 154, 1)',
                }
            }
          }else{
            obj={
                "name": row.TypeName,
                "value": row.CarbonEmission,
            }
          }
          this.countDataList.unshift({
            label:row.TypeName,
            num:row.UseVale,
            unit:row.Unit
          })
          return obj
        })
        this.options.series[0].data=typeData
        this.options.series[0].label.formatter=(params) => {
          //只有“直接访问”使用大标签，其他都使用小标签
          if(params.data.name.indexOf('电力')>-1) {
            return '{colorBlock|} {namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+' }{percentf|'+params.percent+'%}';
          } else if(params.data.name.indexOf('天然气')>-1){
            return '{colorBlock2|} {namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+' }{percentf|'+params.percent+'%}';
          }else{
            return '{namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+' }{percentf|'+params.percent+'%}';
          }
        }
        setTimeout(() => {
          this.setDraw()
          
        }, 1000);
        this.EquipemntTypes=res.data.EquipemntTypes
      })
    },
    getEngryData(){//能源排序
      // this.loading = true;
      // this.currentKey = currentKey;
      // this.isEquipment = isEquipment;
      // if (isEquipment) {
      //     this.queryParams.equipmentId = currentKey;
      //     this.queryParams.facilityId = '';
      // } else {
      //     this.queryParams.facilityId = currentKey;
      //     this.queryParams.equipmentId = '';
      // }
      if(this.isReadLoadEngryDate){
        if(this.engryDataType==1){
          let queryParams={
            pageNum: 1,
            pageSize: 10000,
            dateType: this.countType,
            orgId:this.orgId,
            orderByColumn:'UseVale',
            isAsc:'desc'
          }
          // console.log(this.countDate,'时间范围');
          if(this.countDate){
            if(this.countType=='月'){
              queryParams.beginDate = dayjs(this.countDate).startOf('month').format('YYYY-MM-DD');
              queryParams.endDate = dayjs(this.countDate).endOf('month').format('YYYY-MM-DD');
            }else if(this.countType=='年'){
              queryParams.beginDate = dayjs(this.countDate).startOf('year').format('YYYY-MM-DD');
              queryParams.endDate = dayjs(this.countDate).endOf('year').format('YYYY-MM-DD');
            }
            
          }
          // console.log(queryParams,'queryParams处理后');
          energyPageDateList(queryParams).then(res => {
            // console.log('res设备能耗排行',res);
            this.engryData=res.data.List.map((row,inx)=>{
              row.NumInx='NO.'+(inx+1)
              return row
            })
            // console.log(this.engryData,'this.engryData设备结果数据');
            this.isReadLoadEngryDate=false
            // this.loading = false;
          })
        }else{
          let queryParams={
            pageNum: 1,
            pageSize: 10000,
            dateType: this.countType,
            orgId:this.orgId,
            orderByColumn:'UseVale',
            isAsc:'desc'
          }
          if(this.countDate){
            if(this.countType=='月'){
              queryParams.beginDate = dayjs(this.countDate).startOf('month').format('YYYY-MM-DD');
              queryParams.endDate = dayjs(this.countDate).endOf('month').format('YYYY-MM-DD');
            }else if(this.countType=='年'){
              queryParams.beginDate = dayjs(this.countDate).startOf('year').format('YYYY-MM-DD');
              queryParams.endDate = dayjs(this.countDate).endOf('year').format('YYYY-MM-DD');
            }
            
          }
          facilityEnergyPageDateList(queryParams).then(res => {
            // console.log('res',res);
            this.engryData=res.data.List.map((row,inx)=>{
              row.NumInx='NO.'+(inx+1)
              return row
            })
            this.isReadLoadEngryDate=false
            // this.loading = false;
          }).c
        }
        
        
      }
      
    },
    getIndicatorData(){//能效指标
      if(this.isReadLoadEfficiencyDate){
        let queryParams={
          orgId:this.orgId,
          pageNum:1,
          pageSize:10000,
        }
        if(this.countDate){
          if(this.countType=='月'){
            queryParams.beginDate = dayjs(this.countDate).startOf('month').format('YYYY-MM-DD');
            queryParams.endDate = dayjs(this.countDate).endOf('month').format('YYYY-MM-DD');
          }else if(this.countType=='年'){
            queryParams.beginDate = dayjs(this.countDate).startOf('year').format('YYYY-MM-DD');
            queryParams.endDate = dayjs(this.countDate).endOf('year').format('YYYY-MM-DD');
          }
        }
        // console.log(queryParams,'queryParams');
        selectProductEnergyPage(queryParams).then(res=>{
          // console.log('能效指标',res);
          this.efficiencyData=res.data.List
          this.isReadLoadEfficiencyDate=false
        })
      }
    },
    getCarbonFootprint(){//碳足迹

    },
    cellSty({ row, rowIndex }) {
      // console.log(row, rowIndex);
      if (rowIndex == 0) {
          let obj = {
              'color': 'rgba(255, 255, 255, 0.6)',
              'background': 'rgba(34, 46, 64, 1) !important'
          }
          return obj;
      }
    },
    isRed({ row,rowIndex }) {
      // console.log("选中的",rowIndex,row);
      if(rowIndex%2){
        return {
          backgroundColor: "rgba(34, 46, 64, 1)"
        };
      }else{
        return {
          backgroundColor: "rgba(255, 255, 255, 0)"
        };
      }
    },
    setDraw(){
      let statusChart1 = echarts.init(
        document.querySelector(".pie_echart")
      );
      // console.log(this.options,'this.options');
      statusChart1.setOption(this.options);
      let that=this
      window.addEventListener('resize', function() {
        statusChart1.setOption(that.options);
        statusChart1.resize();
      });
    },
  },
};
</script>

<style lang="less" scoped>
.engry_con{
  // display: flex;
  // justify-content: space-between;
  width: 100%;
  height: 100%;
  background: url('~@/assets/images/data_bg.png') no-repeat;
  background-size: cover;
  // background-position: 0 0;
  padding-top: 40px;
  padding-left: 2px;
  padding-right: 10px;
  box-sizing: border-box;
  position: relative;
  min-height: 760px;
  // max-height: 896px;
  .engry_bg_img{
    position: absolute;
    z-index: 2;
    // width: 1719.68px;
    // height: 1020px;
    width: 608px;
    height: 420px;
    
    // background: url('~@/assets/images/data_bg.png') no-repeat;
    // background-size: cover;
    top: 242px;
    left: 80px;
    .img{
      width: 608px;
      height: 420px;
    }
  }
  .engry_building_con{
    position: absolute;
    z-index: 1;
    width: 1240px;
    // min-width:1240px;
    // height: 100%;
    height: 760px;
    // top: -40px;
    top:0px;
    left: 0;
    .img{
      width: 1240px;
      height: 760px;
    }
  }
  .engry_data_con{
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: space-between;
    position: relative;
    z-index: 9;
    max-height: 906px
  }
  .abs_con{
    position: absolute;
    width: 641px;
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;
    left: 92px;
    top: 236px;
    z-index: 5;
    // transform: translate(-100px, -370px); /* 微调居中 */
    transform: skew(-25deg,0deg);
    // background-color: skyblue;
    .abs_li{
      display: flex;
      align-items: center;
      justify-content: flex-start;
      width: 50%;
      box-sizing: border-box;
      transform: skew(25deg,0deg);
      position: relative;
      .abs_info_con{
        position: absolute;
        display: none;
        top: 134px;
        right:-80px;
        width: 200px;
        height: 88px;
        border-radius: 4px;
        padding: 12px 0 12px 12px;
        box-sizing: border-box;
        background: rgba(25, 33, 45, 1);
        border: 1px solid rgba(43, 58, 81, 1);
        .abs_info{
          position: relative;
          .close{
            position: absolute;
            font-size: 8px;
            right: 8px;
            top: 0;
            color: rgba(255, 255, 255, 0.60);
          }
          .abs_info_li{
            color: rgba(255, 255, 255, 1);
            font-size: 12px;
            line-height: 12px;
            margin-top: 10px;
          }
        }
      }
      &.right{
        .li_icon{
          margin-right: -6px;
        }
        .li_line{
          margin-right: -6px;
        }
      }
      &.first{
        // padding-left: 158px;
      }
      &.second{
        
        padding-top: 12px;
        justify-content: flex-end;
      }
      &.third{
        // padding-left: 94px;
        padding-top: 60px;
      }
      &.fouth{
        padding-top:64px;
        // padding-right: 46px;
        justify-content: flex-end;
      }
      &.fifth{
        padding-top: 84px;
      }
      &.sixed{
       padding-top: 86px;
      //  padding-right: 101px;
       justify-content: flex-end;
      }
      .li_ele{
        width: 120px;
        height: 28px;
        background: url('~@/assets/images/choice.png') no-repeat;
        background-size: cover;
        display: flex;
        justify-content: center;
        align-items: center;
        color: rgba(255, 255, 255, 1);
        font-size: 14px;
      }
      .li_icon{
        width: 20px;
          height: 20px;
          margin-left: -6px;
        img{
          width: 20px;
          height: 20px;
        }
      }
      .li_line{
        width: 80px;
        height: 1px;
        background: rgba(255, 255, 255, 1);
        margin-left: -6px;
      }
    }
  }
  .engry_data_type{
    display: flex;
    height: 88px;
    margin-top: 10px;
    .engry_data_type_li{
      display: flex;
      flex-direction: column;
      justify-content: center;
      margin-left: 8px;
      padding-left: 12px;
      box-sizing: border-box;
      width: 160px;
      height: 88px;
      color: rgba(255, 255, 255, 1);
      background: url('~@/assets/images/data_container4.png') no-repeat;
      .label{
        font-size: 12px;
        line-height: 12px;
      }
      .num{
        font-size: 24px;
        line-height: 24px;
        margin-top: 8px;
      }
      .unit{
        font-size: 12px;
        line-height: 12px;
        margin-top: 8px;
      }
    }
  }
}
.info_title{
  display: flex;
  align-items: center;
  height: 28px;
  width: 100%;
  padding-left: 10px;
  box-sizing: border-box;
  color: rgba(255, 255, 255, 1);
  font-size: 14px;
  line-height: 14px;
  background: url('~@/assets/images/data_title.png') no-repeat;
  
  
  &.date_title{
    display: flex;
    justify-content: space-between;
    .right_text{
      color: rgba(255, 255, 255, 0.6);
      font-size: 12px;
      margin-right: 10px;
    }
    .tab_con{
      display: flex;
      justify-content: flex-end;
      align-items: center;
      font-size: 12px;
      line-height: 12px;
      color: rgba(255, 255, 255, 0.60);
      .tab_li{
        cursor: pointer;
        &.active{
          color: rgba(61, 185, 143, 1);
        }
      }
    }
  }
  &.abs_info_title{
    background: transparent;
    height: 14px;
    font-size: 14px;
    line-height: 14px;
    padding-bottom: 4px;
    padding-left: 0;
  }
  .title{
    display: flex;
    align-items: center;
  }
  .img{
    width: 14px;
    height: 14px;
    margin-right: 8px;
  }
}
.echart_info_con{
    z-index: 999;
    // margin-top: 10px;
    
    .pie_con{
      margin-top: 8px;
      width: 360px;
      height: 176px;
      background: url('~@/assets/images/data_container2.png') no-repeat;
      .pie_echart{
        width: 100%;
        height: 150px;
      }
    }
    .collect_cin_con{
      margin-top: 8px;
      width: 360px;
      max-height: 240px;
      overflow: hidden;
      padding: 1px;
      box-sizing: border-box;
      background-color: rgba(43, 58, 81, 1); /* 边框颜色 */
      /* 外层同样应用裁剪路径，确保边框也有斜切效果 */
      clip-path: polygon(
        0 0,                  /* 左上角 */
        calc(100% - 10px) 0,  /* 右上角斜切起点 */
        100% 10px,            /* 右上角斜切终点 */
        100% calc(100% - 10px), /* 右下角斜切起点 */
        calc(100% - 10px) 100%, /* 右下角斜切终点 */
        10px 100%,            /* 左下角斜切起点 */
        0 calc(100% - 10px)   /* 左下角斜切终点 */
      );
      .collect_cin{
        width: 100%;
        height: 100%;
        position: relative;
        background: rgba(25, 33, 45, 1);
        // border: 1px solid rgba(43, 58, 81, 1);
        // background: url('~@/assets/images/data_container2.png') no-repeat;
        clip-path: polygon(
          0 0,                  /* 左上角 */
          calc(100% - 10px) 0,  /* 右上角斜切起点 */
          100% 10px,            /* 右上角斜切终点 */
          100% calc(100% - 10px), /* 右下角斜切起点 */
          calc(100% - 10px) 100%, /* 右下角斜切终点 */
          10px 100%,            /* 左下角斜切起点 */
          0 calc(100% - 10px)   /* 左下角斜切终点 */
        );
        padding: 0 10px 10px 10px;
        box-sizing: border-box;
        overflow: hidden;
        .order_warp{
          overflow-y: hidden;
          .el-loading-mask{
            background: rgba(25, 33, 45, 1);
          }
        }
        .info_title{
          margin-left: -10px;
        }
        .data_table.home_table{
          width: 100%;
          // height: 128px;
          overflow-x: hidden;
          ::v-deep .el-table__empty-block{
            min-height: 32px;
            .el-table__empty-text{
              line-height: 32px;
            }
          }
          &.table_head{
            height: 32px;
            ::v-deep .el-table__body-wrapper{
              display: none;
            }
          }
          &.table_body{
            max-height: 160px;
          }
        }
        .data_table.home_table.el-table{
          ::v-deep .el-table__header thead th.el-table__cell{
            padding: 10px 0;
            .cell{
              line-height: 12px;
              font-size: 12px;
              border: none;
            }
          }
          ::v-deep th.el-table__cell,::v-deep th.el-table__cell.is-leaf,::v-deep td.el-table__cell{
            padding: 10px 0;
            .cell{
              line-height: 12px;
              font-size: 12px;
            }
          }
          ::v-deep .el-table__header-wrapper th,::v-deep .el-table__fixed-header-wrapper th{
            height: 32px;
            font-size: 12px;
          }
          ::v-deep tr.el-table__row{
            border:none;
            border-bottom: none !important;
          }
        }
      }
    }
    

  }
</style>