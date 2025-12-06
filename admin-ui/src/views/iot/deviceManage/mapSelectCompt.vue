<template>
  <div>
    <el-dialog top="2vh" title="选择地址" :visible.sync="openSelectMap" width="96%" @close="closeDislog" class="select_map"
      :destroy-on-close="true" center :show-close="false">
      <div style="position: relative; height: 80vh;overflow-y: hidden;">
        <div style="line-height: 60px; margin-bottom:18px">
          <el-cascader v-model="alChooseArea" clearable placeholder="请选择省市区"
            :props="{ value: 'Id', label: 'Name', children: 'children', checkStrictly: true,emitPath:true }"
            :options="areaLis" @change="choiceArea" style="width:30%;" popper-class="address_popper"></el-cascader>
        </div>
        <div id="map_container" style="height: calc(100% - 78px)"></div>
        <div id="panel">
          <p>输入关键字，将展示相关地点提示，点击提示可定位到该处。</p>
          <div class="search_con">
            <input id="keyword" type="text" value />
            <input id="search" type="button" class="btn" value="搜索" @click="searchByKeyword()"/>
          </div>
          <ul id="suggestionList">
          </ul>
        </div>
      </div>
      <div slot="title">
        <div class="select_title">
          <span>选择地址</span>
          <div>
            <el-button @click="closeDislog">取消选择</el-button>
            <el-button type="primary" @click="getAddressInfo">确定选择</el-button>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { initMap } from "@/utils/amap";
export default {
  name: "AdminUiMapSelectCompt",

  data() {
    return {
      openSelectMap: false,
      isFirstDraw: true,
      choiceAddress: {},
      suggestionList: [],
      placeSearch:null,//搜索
      Geocoder:null,///
      alChooseArea:[],
      areaLis: [], //地址区域树状列表
      areaAllLis: [], //地址区域非树状列表
      chooseAreaStr:''
    };
  },
  beforeCreate() {
    // console.log('iiiiiiii');
    initMap().catch((ex) => {
      // this.$message.error(ex);
    });
  },
  created(){
    this.getDatas(); //获取地址列表
  },
  mounted() {
  },

  methods: {
    choiceArea(value) {
      //选择地址后，//选择区域后获取区域代码
      // console.log(this.alChooseArea,'选择的省市区',value);
      // console.log("选择的区域字符串", this.chooseAreaStr);
      if(value&&value.length>0){
        let str=''
        str=this.returnAdressRes(this.areaLis,value,'')
        // console.log("获取的省市区结果",str);
        this.choiceAddress.AddressName=str
        this.choiceAddress.AddressCode=value[value.length-1]
      }else{
        this.choiceAddress.AddressName=''
        this.choiceAddress.AddressCode=''
      }
    },
    returnAdressRes(areaLis,value,previous){
      let str=''
      for(let i=0;i<value.length;i++){
        let rw=areaLis.find(ro=>ro.Id==value[i])
        if(rw){
          // console.log(rw,'rw',rw.Name);
          if(rw.Name!=previous){
            str=str+rw.Name
          }
          
          if(rw.children&&rw.children.length>0){
            str=str+this.returnAdressRes(rw.children,value,rw.Name)
          }
        }
        
      }
      return str
    },
    codeGetAllPath(code,arr){
      let row=this.areaAllLis.find(row=>row.Id==code)
      if(row&&row.ParentId!='100000'){
        arr.unshift(row.ParentId)
        let rowRes=this.codeGetAllPath(row.ParentId,arr)
        arr=JSON.parse(JSON.stringify(rowRes))
      }
      return arr
    },
    getDatas() {
      //获取地址列表
      this.$store.dispatch("datas/areaTree").then(area => {
        // console.log("地址", area);
        this.areaLis = area;
        this.areaAllLis=this.$store.state.datas.areaAllData;
      });
    },
    getAddressInfo() {
      //   if (this.choiceAddress) {
      //     this.orgForm.addressName = this.choiceAddress.AddressName;
      //     this.orgForm.addressDetail = this.choiceAddress.AddressDetail;
      //     this.orgForm.addressCode = this.choiceAddress.AddressCode;
      //     this.orgForm.lat = this.choiceAddress.Lat;
      //     this.orgForm.lng = this.choiceAddress.Lng;
      //   }
      this.$emit("returnMapInfo", this.choiceAddress);
      this.openSelectMap = false;
      // console.log("选择地址后的信息", this.choiceAddress);
    },
    closeDislog() {
      this.openSelectMap = false;
      this.isFirstDraw = true;
    },
    choiceMap() {
      //打开地址选择的弹窗
      this.openSelectMap = true;
      let that = this;
      this.$nextTick(() => {
        if (this.isFirstDraw) {
          that.loadMap();
          this.isFirstDraw = false;
        }
      });
    },
    loadMap() {
      this.map = null;
      this.center = [118.681941, 24.880935];
      this.map = new AMap.Map("map_container", {
        center: this.center,
        zoomTo: 4,
      });
      this.suggestionList = [];
      // this.search = new AMap.service.PlaceSearch({ pageSize: 1 }); // 新建一个地点搜索类，搜索类设置为一条数据
      // this.suggest = new AMap.service.Suggestion({
      //   // 新建一个关键字输入提示类
      //   pageSize: 15, // 返回结果每页条目数
      //   region: "", // 限制城市范围
      //   regionFix: false, // 搜索无结果时是否固定在当前城市
      // });
      let _this = this;
      this.markers = new AMap.Marker({
        map: this.map,
        geometries: [],
      });
      this.infoWindowList = Array(1);

      if (this.choiceAddress.addressCode && this.choiceAddress.lat && this.choiceAddress.lng) {
        //初始化已经选择的地址
        this.alChooseArea=this.codeGetAllPath(this.choiceAddress.AddressCode,[this.choiceAddress.AddressCode])
        this.map.panTo([this.choiceAddress.lng, this.choiceAddress.lat]);
      }
      //监听点击事件添加marker
      this.map.on("click", (evt) => {
        // console.log("地图的点击事件",evt);
        this.map.panTo([evt.lnglat.lng, evt.lnglat.lat]);
        // this.map.zoomTo((this.map.getZoom() + 1) % 17);
        this.infoWindowList.forEach((infoWindow) => {
          infoWindow.close();
        });
        this.infoWindowList = [];
        const lat = Number(evt.lnglat.lat.toFixed(6));
        const lng = Number(evt.lnglat.lng.toFixed(6));

        this.getLocatInfo(lat, lng);
      });
    },
    getLocatInfo(lat, lng) {
      this.reloadMarker([lng, lat])
      AMap.plugin('AMap.Geocoder', ()=>{
        let geocoder = new AMap.Geocoder({
          // city 指定进行编码查询的城市，支持传入城市名、adcode 和 citycode
          city: '全国'
        })
        // console.log([lng, lat]);
        geocoder.getAddress([lng, lat], (status, result)=>{
          // console.log(status, result,'地址解析结果');
          if (status === 'complete' && result.info === 'OK') {
             
            let addressInfoObj = JSON.parse(JSON.stringify(result.regeocode.addressComponent));
            addressInfoObj.province = addressInfoObj.province ? addressInfoObj.province : "";
            addressInfoObj.city = addressInfoObj.city ? addressInfoObj.city == addressInfoObj.province ? "" : addressInfoObj.city : "";
            addressInfoObj.district = addressInfoObj.district ? addressInfoObj.district == addressInfoObj.city ? "" : addressInfoObj.district : "";
            this.choiceAddress={}
            this.choiceAddress.AddressName = addressInfoObj.province + addressInfoObj.city + addressInfoObj.district;
            this.choiceAddress.AddressDetail = result.regeocode.formattedAddress.substring(this.choiceAddress.AddressName.length);
            // this.choiceAddress.AddressDetail = result.regeocode.formattedAddress;
            this.choiceAddress.AddressCode = addressInfoObj.adcode;
            this.choiceAddress.Lat = lat;
            this.choiceAddress.Lng = lng;
            this.alChooseArea=this.codeGetAllPath(this.choiceAddress.AddressCode,[this.choiceAddress.AddressCode])
            // console.log("this.choiceAddress地址",this.choiceAddress);
            var infoWindow = new AMap.InfoWindow({
              map: this.map,
              isCustom: true,
              draggable: false, //是否可拖动
              position: [lng, lat],
              content: `<div class="suggest_infowindow"><h3>${result.regeocode.formattedAddress}</p></div>`,
              offset: new AMap.Pixel(-5, -50),
            });
            this.infoWindowList[0] = infoWindow;
            infoWindow.open(this.map, this.markers.getPosition());
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
    },
    reloadMarker(entr_location){
      this.markers.setMap(null); //每次点击地图时将地图的标记清空
      this.markers = null;
      this.markers= new AMap.Marker({
        position: entr_location,
        image: require("@/assets/images/address_map.png"),
        map: this.map,
      });
      const icon = new AMap.Icon({
        size: new AMap.Size(36, 36),
        imageOffset: new AMap.Pixel(0, 0),
        imageSize: new AMap.Size(36, 36),
      });
      this.markers.setIcon(icon);
    },
    setSuggestion(index) {
      // 点击输入提示后，于地图中用点标记绘制该地点，并显示信息窗体，包含其名称、地址等信息
      this.infoWindowList.forEach((infoWindow) => {
        infoWindow.close();
      });
      this.infoWindowList.length = 0;
      document.getElementById("keyword").value =this.suggestionList[index].name;
      document.getElementById("suggestionList").innerHTML = "";
      let addressInfoObj = JSON.parse(JSON.stringify(this.suggestionList[index]));
      if (addressInfoObj.entr_location) {
        this.reloadMarker(addressInfoObj.entr_location)//重新加载marker的图标
        if (addressInfoObj) {
          // console.log("选择地图",addressInfoObj);
          addressInfoObj.province = addressInfoObj.pname ? addressInfoObj.pname : "";
          addressInfoObj.city = addressInfoObj.cityname ? addressInfoObj.cityname == addressInfoObj.province ? "" : addressInfoObj.cityname : "";
          addressInfoObj.district = addressInfoObj.adname ? addressInfoObj.adname == addressInfoObj.city ? "" : addressInfoObj.adname : "";
          this.choiceAddress = {};
          this.choiceAddress.AddressName = addressInfoObj.province + addressInfoObj.city + addressInfoObj.district;
          // this.choiceAddress.AddressDetail = addressInfoObj.address.substring(this.choiceAddress.AddressName.length) + addressInfoObj.name;
          this.choiceAddress.AddressDetail = addressInfoObj.address + addressInfoObj.name;
          this.choiceAddress.AddressCode = addressInfoObj.adcode;
          this.alChooseArea=this.codeGetAllPath(this.choiceAddress.AddressCode,[this.choiceAddress.AddressCode])
          this.choiceAddress.Lat = addressInfoObj.entr_location[1];
          this.choiceAddress.Lng = addressInfoObj.entr_location[0];
        }

        let infoWindow = new AMap.InfoWindow({
          map: this.map,
          isCustom: true,
          draggable: false, //是否可拖动
          center: addressInfoObj.entr_location,
          content: `<div class="suggest_infowindow"><h3>${this.suggestionList[index].name}</h3><p>地址：${this.suggestionList[index].address}</p></div>`,
          offset: new AMap.Pixel(-5, -50),
        });
        infoWindow.open(this.map, this.markers.getPosition());
        this.infoWindowList.push(infoWindow);
        this.map.setCenter(addressInfoObj.entr_location);
      }else{
        this.placeSearch.search(document.getElementById("keyword").value, (status, result) => {
          if (status === "complete" && result.info === "OK") {
            this.suggestionList = result.poiList.pois;
          } else {
            console.error("搜索失败:", result.info);
            new Vue().$message({
              message: '未开启高级会员,地图调用已达上限',
              type: 'error',
              duration: 3000
            })
          }
        });
      }
    },
    searchByKeyword() {
      let keyword = document.getElementById("keyword").value;
      if (!keyword) return;
      // 创建地点搜索插件实例
      AMap.plugin(["AMap.PlaceSearch"], () => {
        this.placeSearch = new AMap.PlaceSearch({
          pageSize: 15, // 每页显示结果数量
          pageIndex: 1, // 当前页码
          city: "全国", // 搜索城市范围
          panel: "suggestionList",
          autoFitView: true,
        });
        // 给搜索结果面板绑定点击事件
        const panel = document.getElementById("suggestionList");
        panel.addEventListener("click", (event) => {
          const target = event.target;
          if (target.classList.contains("poi-title") ||target.classList.contains("poi-info") ||target.classList.contains("amap_lib_placeSearch_pic")) {
            let ix = target.parentNode.getAttribute("data-idx");
            this.setSuggestion(ix);
          }
          if (target.classList.contains("poi-name") ||target.classList.contains("amap-ellipsis")) {
            let ix = target.parentNode.parentNode.getAttribute("data-idx");
            this.setSuggestion(ix);
          }
        });
        // 执行搜索
        this.placeSearch.search(keyword, (status, result) => {
          if (status === "complete" && result.info === "OK") {
            this.suggestionList = result.poiList.pois;
          } else {
            console.error("搜索失败:", result.info);
            new Vue().$message({
              message: '未开启高级会员,地图调用已达上限',
              type: 'error',
              duration: 3000
            })
          }
        });
      });
    },
  },
};
</script>
<style lang="scss">
.address_popper.el-cascader__dropdown .el-cascader-panel{
  height: 204px !important;
}
</style>
<style lang="scss" scoped>

::v-deep .suggest_infowindow {
  background: #ffffff;
}
::v-deep .el-cascader {
  .el-input__inner{
    height: 48px;
    line-height: 48px;
  }
}
::v-deep .el-dialog__wrapper.select_map{
  overflow-y: hidden;
}
#map_container ::v-deep .suggest_infowindow {
  border: 0;
  padding: 10px 20px;
  box-shadow: 0px 4px 16px 0px rgba(0, 0, 0, 0.06);
  border-radius: 8px;
  font-size: 14px;
  color: #666666;
  line-height: 18px;
  h3 {
    padding: 0;
    margin: 0;
    margin-bottom: 10px;
  }
}
#map_container ::v-deep .suggest_infowindow::after {
  content: "";
  width: 0px;
  height: 0px;
  border: 16px solid #ffffff;
  border-right-color: transparent;
  border-bottom-color: transparent;
  border-left-color: transparent;
  position: absolute;
  bottom: -30px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}
.select_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
#container {
  overflow: hidden;
  width: 100%;
  height: 100%;
  margin: 0;
  font-family: "微软雅黑";
}

.anchorBL {
  display: none;
}

#panel {
  position: absolute;
  background: #fff;
  width: 350px;
  padding: 20px;
  z-index: 9999;
  top: 110px;
  left: 30px;
  .search_con {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}

#suggestionList {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

#suggestionList li .a {
  margin-top: -1px;
  background-color: #f6f6f6;
  text-decoration: none;
  font-size: 18px;
  color: black;
  display: block;
}

#suggestionList li .item_info {
  font-size: 12px;
  color: grey;
}

#suggestionList li .a:hover:not(.header) {
  background-color: #eee;
}
</style>