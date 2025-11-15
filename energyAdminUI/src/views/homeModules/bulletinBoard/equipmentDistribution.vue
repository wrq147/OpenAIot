<template>
  <div class="data_container">
    <div id="map_container" :style="{'height':'calc(100% + '+heiCount+'px)'}"></div>
    <div class="typedata_ul">
      <div class="typedata_li" v-for="it in EquipemntTypes" :key="'type'+it.TypeName">
        <div class="image_con">
          <img v-if="it.TypeName&&it.TypeName.indexOf('电力')>-1" class="img" src="~@/assets/images/dianbiao.png" alt="">
          <img v-else-if="it.TypeName&&it.TypeName.indexOf('天然气')>-1" class="img" src="~@/assets/images/dianbiao.png" alt="">
          <div class="img" v-else></div>
        </div>
        <div class="label_num">
          <div class="typedata_li_label">{{it.TypeName}}</div>
          <div class="typedata_li_num">{{it.TypeCount}}</div>
        </div>
      </div>
    </div>
    <div class="echart_info_con">
      <div class="count_con">
        <div class="info_title">
          <img class="img" src="~@/assets/images/zs.png" alt="">
          <span>设备统计</span>
        </div>
        <div class="count_ul">
          <div class="count_li" v-for="ite in countDataList" :key="ite.label">
            <div class="count_num">{{ite.num}}</div>
            <div class="count_label">{{ite.label}}</div>
          </div>
        </div>
      </div>
      <div class="pie_con">
        <div class="info_title">
          <img class="img" src="~@/assets/images/zs.png" alt="">
          <span>设备类型分布</span>
        </div>
        <div class="pie_echart"></div>
      </div>
      <div class="collect_cin_con">
        <div class="collect_cin">
          <div class="info_title date_title">
            <div class="title">
              <img class="img" src="~@/assets/images/zs.png" alt="">
              <span>采集数据</span>
            </div>
            <div class="date_con">
              <el-date-picker @change="isReadLoadcollectionDate=true,getcollectTableData()" class="date_picker_select" v-model="collectionDate" value-format="yyyy-MM-dd" type="date" placeholder="采集日期" prefix-icon="el-icon-arrow-down"></el-date-picker>
            </div>
          </div>
          
          <el-table v-loading="loading" :data="[]" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_head" :header-cell-style="cellSty" style="width:100%;margin-top:10px;" row-key="EquipmentName">
            <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="能源消耗量" align="center" key="UseVale" prop="UseVale" width="80" :show-overflow-tooltip="true"/>
              <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="60" :show-overflow-tooltip="true"/>
              <el-table-column label="成本(万元)" align="center" key="CostVale" prop="CostVale" width="78" :show-overflow-tooltip="true"/>
          </el-table>
          <SeamlessScroll v-if="collectTableData&&collectTableData.length>5" @ScrollEnd="getcollectTableData" v-loading="loading" class="order_warp" :data="collectTableData" :loop="true" :class-option="{direction: 1, step: 1.1,waitTime: 1000,openWatch: true}">
            <el-table :show-header="false" v-loading="loading" :data="collectTableData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0px;" row-key="EquipmentName">
              <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="能源消耗量" align="center" key="UseVale" prop="UseVale" width="80" :show-overflow-tooltip="true"/>
              <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="60" :show-overflow-tooltip="true"/>
              <el-table-column label="成本(万元)" align="center" key="CostVale" prop="CostVale" width="78" :show-overflow-tooltip="true"/>
            </el-table>
          </SeamlessScroll>
          <el-table v-else :show-header="false" v-loading="loading" :data="collectTableData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0px;" row-key="EquipmentName">
            <el-table-column label="设备名称" align="center" key="EquipmentName" prop="EquipmentName" width="120" :show-overflow-tooltip="true"/>
            <el-table-column label="能源消耗量" align="center" key="UseVale" prop="UseVale" width="80" :show-overflow-tooltip="true"/>
            <el-table-column label="单位" align="center" key="Unit" prop="Unit" width="60" :show-overflow-tooltip="true"/>
            <el-table-column label="成本(万元)" align="center" key="CostVale" prop="CostVale" width="78" :show-overflow-tooltip="true"/>
          </el-table>
        </div>
      </div>
      <div class="collect_cin_con">
        <div class="collect_cin">
          <div class="info_title">
            <img class="img" src="~@/assets/images/zs.png" alt="">
              <span>故障设备</span>
          </div>
          <el-table v-loading="loading" :data="[]" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_head" :header-cell-style="cellSty" style="width:100%;margin-top:10px;" row-key="Id">
            <el-table-column label="设备名称" align="center" key="DeviceName" prop="DeviceName" width="120" :show-overflow-tooltip="true"/>
            <el-table-column label="报警类型" align="center" key="Name" prop="Name" width="80" :show-overflow-tooltip="true"/>
            <el-table-column label="报警时间" align="center" key="CreateOn" prop="CreateOn" :show-overflow-tooltip="true">
              <template slot-scope="scope">
                <span>{{ parseTime(scope.row.CreateOn) }}</span>
              </template>
            </el-table-column>
          </el-table>
          <SeamlessScroll v-if="warnData&&warnData.length>5" @ScrollEnd="getwarnData" v-loading="loading" class="order_warp" :data="warnData" :loop="true" :class-option="{direction: 1, step: 1.1,waitTime: 1000,openWatch: true}">
            <el-table :show-header="false" v-loading="loading" :data="warnData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="Id">
              <el-table-column label="设备名称" align="center" key="DeviceName" prop="DeviceName" width="120" :show-overflow-tooltip="true"/>
              <el-table-column label="报警类型" align="center" key="Name" prop="Name" width="80" :show-overflow-tooltip="true"/>
              <el-table-column label="报警时间" align="center" key="CreateOn" prop="CreateOn" :show-overflow-tooltip="true">
              <template slot-scope="scope">
                <span>{{ parseTime(scope.row.CreateOn) }}</span>
              </template>
            </el-table-column>
            </el-table>
          </SeamlessScroll>
          <el-table v-else :show-header="false" v-loading="loading" :data="warnData" :row-style="isRed" :cell-style="isRed" class="data_table home_table table_body" :header-cell-style="cellSty" style="width:100%;margin-top:0;" row-key="Id">
            <el-table-column label="设备名称" align="center" key="DeviceName" prop="DeviceName" width="120" :show-overflow-tooltip="true"/>
            <el-table-column label="报警类型" align="center" key="Name" prop="Name" width="80" :show-overflow-tooltip="true"/>
            <el-table-column label="报警时间" align="center" key="CreateOn" prop="CreateOn" :show-overflow-tooltip="true">
              <template slot-scope="scope">
                <span>{{ parseTime(scope.row.CreateOn) }}</span>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { initMap } from "@/utils/amap";
import * as echarts from "echarts5";
import { devMapRangeList, devRangeAreaList } from "@/api/after/dev.js";
import SeamlessScroll from 'vue-seamless-scroll'
import { energyPageDateList } from '@/api/energy/energyMeter';
import dayjs from 'dayjs';
import {homeDevStatisticsInfo,homeWarningListPage} from '@/api/home'
export default {
  name: 'EnergyAdminUIEquipmentDistribution',
  components:{SeamlessScroll},
  data() {
    return {
      loading:false,
      warnData:[],
      collectionDate:'',
      collectTableData:[],
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
          // labelLine: {
          //   show: true
          // },
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
          
          data: []
        }]
      },
      activeCode: 100000,
      map: null,
      center: null,
      zoom: 5,
      allDevAdrees: [],
      infoWindow:null,
      isOpenWindow:false,
      lastZoom:5,
      DevListloading:false,
      orgId:'',
      countDataList:[{
        label:'设备数',
        num:0
      },{
        label:'计量设备数',
        num:0
      },{
        label:'联网设备数',
        num:0
      },{
        label:'在线数',
        num:0
      },{
        label:'离线数',
        num:0
      },{
        label:'故障数',
        num:0
      },],
      EquipemntTypes:[],
      timer:null,
      isReadLoadcollectionDate:true,//是否要重新加载采集数据
      isReadLoadWarnDate:true,//是否要重新加载故障
    };
  },
  computed:{
    heiCount(){
      let len=this.warnData.length
      let len2=this.collectTableData.length
      if(len+len2>8){
        // console.log(len+len2-8,'len+len2-8');
        if(len+len2-8>=2){
          return 2*32
        }else{
          return (len+len2-8)*32
        }
      }else{
        return 0
      }
    }
  },
  beforeCreate() {
    // console.log('iiiiiiii');
    initMap().catch((ex) => {
      // this.$message.error(ex);
    });
  },
  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.collectionDate=dayjs().subtract(1,'day').format('YYYY-MM-DD')
    this.getcollectTableData()
    this.getwarnData()
    this.getDevTotal()
    this.timer=setInterval(()=>{
      this.getDevTotal()
      this.isReadLoadcollectionDate=true
      this.isReadLoadWarnDate=true
    },10000)
    setTimeout(() => {
      this.loadMap();
    }, 2000);
  },
  beforeDestroy() {
    if (Window.map) {
      Window.map.destroy();
      Window.map = null;
    }
    if(this.timer){
      clearInterval(this.timer)
    }
  },
  methods: {
    getDevTotal(){
      //获取设备统计信息
      homeDevStatisticsInfo({orgId:this.orgId}).then(res=>{
        // console.log(res,'res设备统计');
        this.countDataList=[{
          label:'设备数',
          num:res.data.EquipemntCount
        },{
          label:'计量设备数',
          num:res.data.DataCount
        },{
          label:'联网设备数',
          num:res.data.TotalCount
        },{
          label:'在线数',
          num:res.data.OnlineCount
        },{
          label:'离线数',
          num:res.data.OfflineCount
        },{
          label:'故障数',
          num:res.data.EventCount
        },]
        let typeData=res.data.EquipemntTypes.map(row=>{
          let obj={}
          if(row.TypeName&&row.TypeName.indexOf('电力')>-1){
            obj={
                "name": row.TypeName,
                "value": row.TypeCount,
                "itemStyle": {
                    "color": 'rgba(54, 183, 231, 1)',
                }
            }
          }else if(row.TypeName&&row.TypeName.indexOf('天然气')>-1){
            obj={
                "name": row.TypeName,
                "value": row.TypeCount,
                "itemStyle": {
                    "color": 'rgba(42, 211, 154, 1)',
                }
            }
          }else{
            obj={
                "name": row.TypeName,
                "value": row.TypeCount,
            }
          }
          return obj
        })
        this.options.series[0].data=typeData
        this.options.series[0].label.formatter=(params) => {
          //只有“直接访问”使用大标签，其他都使用小标签
          if(params.data.name.indexOf('电力')>-1) {
            return '{colorBlock|} {namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+'台 }{percentf|'+params.percent+'%}';
          } else if(params.data.name.indexOf('天然气')>-1){
            return '{colorBlock2|} {namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+'台 }{percentf|'+params.percent+'%}';
          }else{
            return '{namef|'+params.data.name+'}'+ '\n' +'{valuef|'+params.data.value+'台 }{percentf|'+params.percent+'%}';
          }
        }
        setTimeout(() => {
          this.setDraw()
          
        }, 1000);
        this.EquipemntTypes=res.data.EquipemntTypes
      })
    },
    getwarnData(){
      if(this.isReadLoadWarnDate){
        homeWarningListPage({Status:0,pageNum: 1,pageSize: 100000,}).then(res=>{
          this.warnData=res.data.List
        })
      }
    },
    getcollectTableData(){
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
      if(this.isReadLoadcollectionDate){
        let queryParams={
          pageNum: 1,
          pageSize: 10000,
          dateType: '日',
          orgId:this.orgId
        }
        if(this.collectionDate){
          queryParams.beginDate = dayjs(this.collectionDate).format('YYYY-MM-DD');
          queryParams.endDate = dayjs(this.collectionDate).format('YYYY-MM-DD');
        }
        
        energyPageDateList(queryParams).then(res => {
            // console.log('res',res);
            this.collectTableData=res.data.List.map(row=>{

              return row
            })
            this.isReadLoadcollectionDate=false
            // this.loading = false;
        })
      }
      
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
    loadMap() {
      // Window.map = null;
      if(Window.map){
        Window.map.destroy();
      }
      this.center = [118.913302, 24.894089];
      Window.map = new AMap.Map("map_container", {
        center: this.center,
        zoom: 11.17, // 初始化地图级别
        expandZoomRange: true,
        zooms: [6, 20],
        resizeEnable: true, //是否监控地图容器尺寸变化
        mapStyle: "amap://styles/darkblue",
        viewMode: '2D',
      });
      //2、创建省市简易行政区图层
      var distProvince = new AMap.DistrictLayer.Province({
        zIndex: 10, //设置图层层级
        zooms: [2, 20], //设置图层显示范围
        adcode: "130000", //设置行政区 adcode
        depth: 2, //设置数据显示层级，0：显示国家面，1：显示省级，当国家为中国时设置depth为2的可以显示市一级
      });

      // 3、设置行政区图层样式
      distProvince.setStyles({
        "stroke-width": 2, //描边线宽
        fill: function (data) {
          //设置区域填充颜色，可根据回调信息返回区域信息设置不同填充色
          //回调返回区域信息数据，字段包括 SOC(国家代码)、NAME_ENG(英文名称)、NAME_CHN(中文名称)等
          //国家代码名称说明参考 https://a.amap.com/jsapi_demos/static/demo-center/js/soc-list.json
          return "#000001";
        },
      });

      //4、将简易行政区图层添加到地图
      Window.map.add(distProvince);
      //异步加载控件
      let that = this;
      // AMap.plugin("AMap.ToolBar,AMap.Scale,AMap.MapType", function () {
      //   var toolbar = new AMap.ToolBar(); //比例尺
      //   Window.map.addControl(toolbar); //添加控件
      //   var scale = new AMap.Scale(); //缩放工具条实例化
      //   Window.map.addControl(scale); //添加控件
      //   var mapType = new AMap.MapType(); //图层切换
      //   Window.map.addControl(mapType); //添加控件
      // });
      // 监听拖拽结束事件
      Window.map.on('dragend', () => {
        this.isOpenWindow=false
        let center = Window.map.getCenter(); // 获取当前地图中心点坐标
        this.center=[center.lng,center.lat]
        this.loadDeviceData();
        // console.log('当前地图中心点坐标:', this.center);
      });
      // 监听地图中心点变化事件
      Window.map.on("zoomchange", (event) => {
        var zoom = Window.map.getZoom();
        // console.log("缩放", zoom,Math.abs(this.lastZoom-this.zoom));
        this.zoom = zoom;
        if(Math.abs(this.lastZoom-this.zoom)>=0.6||this.zoom==7.5){
          this.loadDeviceData();
          this.lastZoom=zoom
        }
        
        //   // }
        // });
        // 业务逻辑...
      });
      Window.map.on("click", (evt) => {
        // console.log("地图的点击事件",evt);
        
      });
      this.loadDeviceData();
      // console.log("地图信息", Window.map);
    },
    loadDeviceData() {
      //加载设备数据
      let groupBy='Province'/// 组合方式：Province、City、District
      let defZoom='4'
      this.zoom = Window.map.getZoom();
      if(this.zoom<=5.8){
        groupBy='Province'
        defZoom=1
      }else if(this.zoom<=8.5){
        groupBy='City'
        defZoom=2
      }else if(this.zoom>8.5){
        groupBy='District'
        defZoom=3
      }
      // if(this.zoom<=7.5){
      //   let mapObj={
      //     GroupBy:groupBy,
      //     Lng:this.center[0],
      //     Lat:this.center[1],
      //     level:defZoom
      //   }
      //   this.loadDevMapList(mapObj,defZoom);
      // }else{
        let mapObj1={
          Lng:this.center[0],
          Lat:this.center[1],
          level:defZoom
        }
        
        this.loadDevMapRangeList(mapObj1,true);
      // }
      
      // this.loadDevMapRangeList(this.mapObj)
      
    },
    loadDevMapRangeList(mapobj,clear) {
      //获取设备统计信息

      devMapRangeList(mapobj).then((res) => {
        // console.log("根据经纬度获取设备信息",res,clear);
        if(clear){
          Window.map.clearMap();
        }
        if (res.data) {
          
          let afterList=[]
          let arr=res.data.map((row) => {
            let findrow=afterList.find(rw=>rw.Lng==row.Lng&&rw.Lat==row.Lat)
            if(findrow){
              row.SameNum=1
            }else{
              let filArr=res.data.filter(rw=>rw.Lng==row.Lng&&rw.Lat==row.Lat)
              row.SameNum=filArr.length
            }
            afterList.push(row)
            return row
          });
          afterList.map(row=>{
            if (row.Online == 0) {
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/ligh_icon.png"),'',row);
            } else if (row.Online == 1) {
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/ligh_icon.png"),'',row);
            } else if (row.Online == 2) {
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/ligh_icon.png"),'',row);
            }
          })
          
        }
      });
    },
    loadDevMapList(mapObj,defZoom) {
      devRangeAreaList(mapObj).then((res) => {
        // console.log("设备统计", res);
        let arr = res.data;
        this.allDevAdrees = arr;
        Window.map.clearMap();
        arr.map((row) => {
          this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/ligh_icon.png"),row.Count);
        });
      });
    },
    dataSetShow(){
      Window.map.clearMap();
      let arr=[{Lng:118.884007,Lat:24.924811},{Lng:118.754009,Lat:24.903777},{Lng:118.796731,Lat:24.893812},{Lng:118.673446,Lat:24.860591},{Lng:118.730206,Lat:24.753668}]
      arr.map((row) => {
        this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/ligh_icon.png"));
      });
    },
    setMarkerIcon(LngLat, iconStr, label,info) {
      // 将 Icon 实例添加到 marker 上:
      // console.log(LngLat,'LngLat');
      const icon = new AMap.Icon({
        size: new AMap.Size(30, 30), //图标尺寸
        image: iconStr, //Icon 的图像
        imageOffset: new AMap.Pixel(0, 0), //图像相对展示区域的偏移量，适于雪碧图等
        imageSize: new AMap.Size(30, 30), //根据所设置的大小拉伸或压缩图片\
      });
      const marker = new AMap.Marker({
        resizeEnable: true,
        position: LngLat, //点标记的位置
        offset: new AMap.Pixel(0, 0), //偏移量
        icon: icon, //添加 Icon 实例
        title: "",
        zooms: [0, 20],
        zIndex: 1,
        // 该层内标注是否避让
        collision: true,
        // 设置 allowCollision：true，可以让标注避让用户的标注
        allowCollision: true,
      });
      Window.map.add(marker);
      // marker.setMap(Window.map);
    },
  },
};
</script>
<style lang="less" scoped>
.data_container{
  // position: relative;
  // max-height: 832px;
  #map_container{
    // position: relative;
    width: 100%;
    height: 100%;
    // min-height:calc(100vh - 60px);
    ::v-deep .amap-layers .amap-layer{
      height: calc(100% + 50px) !important;
    }
  }
  .echart_info_con{
    position: absolute;
    top: 50px;
    right: 10px;
    z-index: 999;
    max-height: 832px;
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
        .date_con{
          padding-top: 4px;
          .date_picker_select{
            width: 120px;
            ::v-deep .el-input__inner{
              height: 28px;
              line-height: 28px;
              text-align: right;
              border:none;
              font-size: 12px;
              padding: 0;
              padding-right: 18px;
            }
            ::v-deep .el-input__prefix{
              right: 10px;
              left: initial;
              width: 8px;
            }
            ::v-deep .el-input__prefix .el-input__icon{
              line-height: 28px;
              
            }
          }
        }
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
    .count_con{
      width: 360px;
      height: 176px;
      background: url('~@/assets/images/data_container2.png') no-repeat;
      padding: 0 10px 10px 10px;
      box-sizing: border-box;
      .count_ul{
        padding-top: 2px;
        display: flex;
        flex-wrap: wrap;
        margin-right: -8px;
        .count_li{
          width: 108px;
          height: 60px;
          background: rgba(34, 46, 64, 1);
          border-radius: 4px;
          margin-top: 8px;
          display: flex;
          justify-content: center;
          align-items: center;
          flex-direction: column;
          margin-right: 8px;
          .count_num{
            color: rgba(255, 255, 255, 1);
            font-size: 16px;
          }
          .count_label{
            font-size: 12px;
            margin-top: 8px;
            color: rgba(255, 255, 255, 0.6);
          }
        }
      }
    }
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
  .typedata_ul{
    display: flex;
    position: absolute;
    left: 2px;
    top:48px;
    z-index: 999;

    .typedata_li{
      width: 180px;
      height: 72px;
      margin-left: 8px;
      color: rgba(255, 255, 255, 1);
      background: url('~@/assets/images/data_container.png') no-repeat;
      display: flex;
      padding: 12px 0 0 20px;
      .image_con{
        width: 50px;
        height: 50px;
        margin-right: 20px;
        .img{
          width: 50px;
          height: 50px;
        }
      }
      .typedata_li_label{
        font-size: 12px;
      }
      .typedata_li_num{
        font-size: 24px;
        margin-top: 8px;
      }
    }
  }
}

</style>