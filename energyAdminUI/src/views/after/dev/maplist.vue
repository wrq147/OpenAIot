<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 136px)', position: 'relative' }">
      <div class="totalinfo_ul" v-if="totalInfo">
        <div class="totalinfo_li">
          <i class="zhongtaiiconfont zhongtai-icon-fenbuzongshu" style="margin-right: 7px;" :style="{'color':'rgba(59, 111, 246, 1)'}"></i>
          <span>总数：{{totalInfo.total}}</span>
        </div>
        <div class="totalinfo_li">
          <i class="zhongtaiiconfont zhongtai-icon-yunhang" style="margin-right: 7px;" :style="{'color':'rgba(83, 214, 112, 1)'}"></i>
          <span>运行：{{totalInfo.online}}</span>
        </div>
        <div class="totalinfo_li">
          <i class="zhongtaiiconfont zhongtai-icon-lixian1" style="margin-right: 7px;" :style="{'color':'rgba(198, 198, 198, 1)'}"></i>
          <span>离线：{{totalInfo.offline}}</span>
        </div>
        <div class="totalinfo_li">
          <i class="zhongtaiiconfont zhongtai-icon-weizhi" style="margin-right: 7px;" :style="{'color':'#95A4CA'}"></i>
          <span>未知：{{totalInfo.unknown}}</span>
        </div>
      </div>
      <div class="map_title">
        <div class="title">
          <i class="zhongtaiiconfont zhongtai-icon-ditufenbu" style="margin-right: 14px;" :style="{'color':'#78829d'}"></i>
          <span>地图分布</span>
        </div>
        <div class="sel_con">
          <el-select @focus="DevListSearch" @change="selectDevAfter" clearable style="width: 100%" v-model="DevListId" filterable remote reserve-keyword
            placeholder="请输入设备编码名称" :remote-method="DevListRemoteMethod" :loading="DevListloading">
            <el-option v-for="item in DevListoptions" :key="item.Id" :label="item.DeviceId" :value="item.Id">{{item.DeviceId}}</el-option>
          </el-select>
        </div>
      </div>
      <div id="map_container" style="height: 690px"></div>
    </div>
  </div>
</template>

<script>
import {
  DeviceList
} from "@/api/rules/device";
import { initMap } from "@/utils/amap";
import { devMapRangeList, devRangeAreaList, devMapInfo } from "@/api/after/dev.js";
export default {
  name: "AdminUiMaplist",

  data() {
    return {
      map: null,
      center: null,
      zoom: 5,
      mapObj: null,
      activeCode: 100000,
      allDevAdrees: [],
      infoWindow:null,
      isOpenWindow:false,
      lastZoom:5,
      DevListloading:false,
      DevListoptions:[],
      DevListQueryParams:{
        pageNum: 1,
        pageSize: 20,
        Key: "",
      },
      DevListId:'',
      totalInfo:null
    };
  },
  beforeCreate() {
    // console.log('iiiiiiii');
    initMap().catch((ex) => {
      // this.$message.error(ex);
    });
  },
  mounted() {
    setTimeout(() => {
      this.loadMap();
    }, 1000);
  },
beforeDestroy() {
    if (Window.map) {
      Window.map.destroy();
      Window.map = null;
    }
  },
  methods: {
    selectDevAfter(val){
      //选择设备后
      console.log("val",val);
      if(val){
        let findObj=this.DevListoptions.find(row=>row.Id==this.DevListId)
        if(findObj&&findObj.Lng&&findObj.Lat){
          // console.log("选择的设备",findObj);
          Window.map.setZoom(12)
          this.center=[findObj.Lng,findObj.Lat]
          Window.map.setCenter(this.center);
          if (findObj.Online == 0) {
            this.setMarkerIcon([findObj.Lng, findObj.Lat],require("@/assets/images/map_offline.png"),'',findObj,true);
          } else if (findObj.Online == 1) {
            this.setMarkerIcon([findObj.Lng, findObj.Lat],require("@/assets/images/map_online.png"),'',findObj,true);
          } else if (findObj.Online == 2) {
            this.setMarkerIcon([findObj.Lng, findObj.Lat],require("@/assets/images/map_unknown.png"),'',findObj,true);
          }
        }else{
          this.$modal.msgError("该设备未设置位置");
        }
      }
    },
    DevListSearch(){
      if(this.DevListId&&this.DevListId.indexOf(',')>-1){
        let keyVal=this.DevListId.split(',')
        this.DevListRemoteMethod(keyVal[1])
      }else{
        this.DevListRemoteMethod('')
      }
    },
    async DevListRemoteMethod(query){//设备编码动态加载
      
      if (query !== ""&&query !== null&&query) {
        // console.log(query,'query物联产品');
        this.DevListloading = true;
        setTimeout(async () => {
          this.DevListQueryParams.Key=query
          await this.getDevListList()
          this.DevListloading = false;
          this.DevListoptions = this.DevListoptionsList.filter((item) => {
            return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
        }, 200);
      } else {
        delete this.DevListQueryParams.Key
        await this.getDevListList()
        this.DevListoptions = JSON.parse(JSON.stringify(this.DevListoptionsList));
      }
    },
    async getDevListList(){//获取设备编码列表接口
     let res=await DeviceList(this.DevListQueryParams)
    //  console.log("查询到设备编码",res);
     if(!this.DevListQueryParams.Key){
        this.DevListoptions=res.data.List
      }
      this.DevListoptionsList = res.data.List;
     
    },
    loadMap() {
      // Window.map = null;
      if(Window.map){
        Window.map.destroy();
      }
      this.center = [113.699336, 33.786861];
      Window.map = new AMap.Map("map_container", {
        center: this.center,
        zoom: 5, // 初始化地图级别
        expandZoomRange: true,
        zooms: [3, 20],
      });
      //   var scale = new AMap.Scale({
      //       visible: false,
      //     }),
      //     toolBar = new AMap.ToolBar({
      //       visible: false,
      //       position: {
      //         top: "110px",
      //         right: "40px",
      //       },
      //     });
      //   Window.map.addControl(scale);
      //   Window.map.addControl(toolBar);
      //异步加载控件
      let that = this;
      AMap.plugin("AMap.ToolBar,AMap.Scale,AMap.MapType", function () {
        var toolbar = new AMap.ToolBar(); //比例尺
        Window.map.addControl(toolbar); //添加控件
        var scale = new AMap.Scale(); //缩放工具条实例化
        Window.map.addControl(scale); //添加控件
        var mapType = new AMap.MapType(); //图层切换
        Window.map.addControl(mapType); //添加控件
      });
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
      

      this.loadDeviceData();
      this.loadDevMapInfo(100000);
      // console.log("地图信息", Window.map);
    },
    loadDeviceData() {
      //加载设备数据
      let groupBy='Province'/// 组合方式：Province、City、District
      let defZoom='4'
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
      if(this.zoom<=7.5){
        let mapObj={
          GroupBy:groupBy,
          Lng:this.center[0],
          Lat:this.center[1],
          level:defZoom
        }
        this.loadDevMapList(mapObj,defZoom);
      }else{
        let mapObj1={
          Lng:this.center[0],
          Lat:this.center[1],
          level:defZoom
        }
        
        this.loadDevMapRangeList(mapObj1,true);
      }
      
      // this.loadDevMapRangeList(this.mapObj)
      
    },
    loadDevMapList(mapObj,defZoom) {
      devRangeAreaList(mapObj).then((res) => {
        // console.log("设备统计", res);
        let arr = res.data;
        this.allDevAdrees = arr;
        Window.map.clearMap();
        arr.map((row) => {
          this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/marker_cion.png"),row.Count);
        });
      });
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
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/map_offline.png"),'',row);
            } else if (row.Online == 1) {
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/map_online.png"),'',row);
            } else if (row.Online == 2) {
              this.setMarkerIcon([row.Lng, row.Lat],require("@/assets/images/map_unknown.png"),'',row);
            }
          })
          
        }
      });
    },
    loadDevMapInfo(code) {
      devMapInfo({ code }).then((res) => {
        // console.log("设备离在线统计信息", res);
        if(res.data){
          let total=Number(res.data.offCount)+Number(res.data.onCount)+Number(res.data.unCount)
          this.totalInfo={
            total:total,
            online:res.data.onCount,
            offline:res.data.offCount,
            unknown:res.data.unCount,
          }
        }
        
      });
    },
    setMarkerIcon(LngLat, iconStr, label,info) {
      // 将 Icon 实例添加到 marker 上:
      // console.log(LngLat,'LngLat');
      if (label) {
        //创建 AMap.Icon 实例：
        const icon = new AMap.Icon({
          size: new AMap.Size(16, 20), //图标尺寸
          image: iconStr, //Icon 的图像
          imageOffset: new AMap.Pixel(0, 0), //图像相对展示区域的偏移量，适于雪碧图等
          imageSize: new AMap.Size(16, 20), //根据所设置的大小拉伸或压缩图片\
        });
        let Pixel=new AMap.Pixel(-15, -20)
        if(label>=10000){
          Pixel=new AMap.Pixel(-35, -20)
        }else if(label>=1000){
          Pixel=new AMap.Pixel(-30, -20)
        }else if(label>=100){
          Pixel=new AMap.Pixel(-25, -20)
        }else if(label>=10){
          Pixel=new AMap.Pixel(-20, -20)
        }
        const marker = new AMap.Marker({
          resizeEnable: true,
          position: LngLat, //点标记的位置
          offset: new AMap.Pixel(-8, -10), //偏移量
          icon: icon, //添加 Icon 实例
          title: '',
          zooms: [0, 20], //点标记显示的层级范围，超过范围不显示
          label: {
            // 添加标签
            content:'<span style="color:rgba(59, 111, 246, 1);font-size:14px;">' +label +"</span>", // 标签内容
            offset: Pixel, // 标签偏移量，确保标签在标记上方显示
          },
          // 该层内标注是否避让
            collision: true,
            // 设置 allowCollision：true，可以让标注避让用户的标注
            allowCollision: true,
          zIndex: 2,
        });
        marker.on("click", (e) => {
          // console.log("你点击了Marker11", label, e, LngLat);
          if(this.zoom<20){
            Window.map.setZoom(this.zoom+3)
          }
          this.center=LngLat
          Window.map.setCenter(this.center);
          this.loadDeviceData()
        });
        Window.map.add(marker);
      } else {
        //创建 AMap.Icon 实例：
        const icon = new AMap.Icon({
          size: new AMap.Size(36, 36), //图标尺寸
          image: iconStr, //Icon 的图像
          imageOffset: new AMap.Pixel(0, 0), //图像相对展示区域的偏移量，适于雪碧图等
          imageSize: new AMap.Size(36, 36), //根据所设置的大小拉伸或压缩图片\
        });
        let marker = null
        if(info.SameNum&&info.SameNum>1){
          let Pixel=new AMap.Pixel(-25, -25)
          if(info.SameNum>=10000){
            Pixel=new AMap.Pixel(-35, -25)
          }else if(info.SameNum>=1000){
            Pixel=new AMap.Pixel(-37, -25)
          }else if(info.SameNum>=100){
            Pixel=new AMap.Pixel(-33, -25)
          }else if(info.SameNum>=10){
            Pixel=new AMap.Pixel(-29, -25)
          }
          marker = new AMap.Marker({
            resizeEnable: true,
            position: LngLat, //点标记的位置
            offset: new AMap.Pixel(-18, -18), //偏移量
            icon: icon, //添加 Icon 实例
            title: "",
            zooms: [0, 20],
            zIndex: 3,
            // 该层内标注是否避让
            collision: true,
            // 设置 allowCollision：true，可以让标注避让用户的标注
            allowCollision: true,
            label: {
              // 添加标签
              content:'<span style="color:rgba(59, 111, 246, 1);font-size:14px;">' +info.SameNum +"</span>", // 标签内容
              offset: Pixel, // 标签偏移量，确保标签在标记上方显示
            },
          });
        }else{
          marker = new AMap.Marker({
            resizeEnable: true,
            position: LngLat, //点标记的位置
            offset: new AMap.Pixel(-18, -18), //偏移量
            icon: icon, //添加 Icon 实例
            title: "",
            zooms: [0, 20],
            zIndex: 1,
            // 该层内标注是否避让
            collision: true,
            // 设置 allowCollision：true，可以让标注避让用户的标注
            allowCollision: true,
          });
        }
        marker.on("click", (e) => {
          // console.log("你点击了Marker22", e, LngLat);
          // console.log(marker.getPosition(),'marker.getPosition()');
          this.setWindowOpen(LngLat,marker,info)
          
        });
        if(this.DevListId==info.Id){
          this.setWindowOpen(LngLat,marker,info)
          Window.map.setCenter(LngLat);
        }
        Window.map.add(marker);
      }

      // marker.setMap(Window.map);
    },
    setWindowOpen(LngLat,marker,info){
      AMap.plugin('AMap.Geocoder', ()=>{
        let geocoder = new AMap.Geocoder({
          // city 指定进行编码查询的城市，支持传入城市名、adcode 和 citycode
          city: '全国'
        })
        // console.log([lng, lat]);
        geocoder.getAddress(LngLat, (status, result)=>{
          // console.log(status, result,'地址解析结果');
          if (status === 'complete' && result.info === 'OK') {
            let infoWindow = new AMap.InfoWindow({
              isCustom: true,
              draggable: false, //是否可拖动
              offset: new AMap.Pixel(22, -23),
              center: LngLat,
              content:`<div class="window_con">
              <div class="title">设备信息</div><i id="map_close_img" class="el-icon-close"></i>
              <div class="info_ul">
              <div class="info_li li_left">设备名称</div><div class="info_li li_right blue">${info.Name}</div>
              <div class="info_li li_left">用户名称</div><div class="info_li li_right">${info.OwnerOrgName}</div>
              <div class="info_li li_left">通讯编码</div><div class="info_li li_right blue">${info.DeviceId}</div>
              <div class="info_li li_left">第三方编码</div><div class="info_li li_right blue">${info.DeviceNumber}</div>
              <div class="info_li li_left two">设备地址</div><div class="info_li li_right two"><div class="text">${result.regeocode.formattedAddress}</div></div>
              <div class="info_li li_left">现场单位名称</div><div class="info_li li_right"></div>
              </div>
              </div>`,
            });
            // console.log(marker,'markermarker');
            infoWindow.open(Window.map,marker.getPosition());
            this.isOpenWindow=true
            let _this=this
            this.$nextTick(()=>{
              document.getElementById("map_close_img").addEventListener("click", function () {
                //点击id为div_link时调用的处理函数
                infoWindow.close();
              });
            })
            
              // result为对应的地理位置详细信息
          }else{
            this.$modal.msgError("未开启高级会员,地图调用已达上限");
            // new Vue().$message({
            //   message: '未开启高级会员,地图调用已达上限',
            //   type: 'error',
            //   duration: 3000
            // })
          }
        })
      })
    }
  },
};
</script>

<style lang="less" scoped>
.totalinfo_ul{
  width: 420px;
  height: 46px;
  position: absolute;
  left: calc(50% - 210px);
  top: 88px;
  z-index: 9;
  display: flex;
  justify-content: space-between;
  align-items: center;
  .totalinfo_li{
    width: 25%;
    display: flex;
    height: 100%;
    justify-content: center;
    align-items: center;
    color: rgba(102, 102, 102, 1);
    font-size: 14px;
    background: #ffffff;
  }
}
.map_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
  .sel_con{
    width: 200px;
  }
}
::v-deep .amap-marker-label {
  border: 0;
  background-color: transparent;
}
::v-deep .amap-info-contentContainer{
  .window_con{
    width: 340px;
    height: 272px;
    padding: 16px;
    box-sizing: border-box;
    background: rgba(255, 255, 255, 1);
    box-shadow: 0px 4px 16px 0px rgba(0,0,0,0.1);
    border-radius: 4px;
    opacity: 0.95;
    position: relative;
    .title{
      font-size: 14px;
      color: rgba(51, 51, 51, 1);
      font-weight: bold;
      margin-bottom: 16px;
    }
    i{
      position: absolute;
      top: 10px;
      right: 10px;
    }
    .info_ul{
      display: flex;
      flex-wrap: wrap;
      width: 100%;
      border-left: 1px solid rgba(234, 234, 234, 1);
      border-top: 1px solid rgba(234, 234, 234, 1);
      .info_li{
        font-size: 12px;
        color: rgba(51, 51, 51, 1);
        padding: 0 10px;
        box-sizing: border-box;
        height: 32px;
        line-height: 32px;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
        border-right: 1px solid rgba(234, 234, 234, 1);
        border-bottom: 1px solid rgba(234, 234, 234, 1);
        &.li_left{
          width: 109px;
          &.two{
            display: flex;
            align-items: center;
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            padding-top: 0;
            padding-bottom: 0;
          }
        }
        &.li_right{
          width: 196px;
          
        }
        &.blue{
          color: rgba(18, 120, 246, 1);
        }
        &.two{
          line-height: 18px;
          height: 50px;
          display: -webkit-box;
          overflow: hidden;
          white-space: normal;
          text-overflow: ellipsis;
          word-wrap: break-word;
          -webkit-line-clamp: 2;
          -webkit-box-orient: vertical;
          padding-top: 8px;
          padding-bottom: 8px;
          .text{
            line-height: 18px;
            height: 36px;
            display: -webkit-box;
            overflow: hidden;
            white-space: normal;
            text-overflow: ellipsis;
            word-wrap: break-word;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
          }
        }
      }
    }
  }
}

</style>