<template>
  <div style="width:100%;height:100vh;" :style="appContainerStyle">
    <div style="-webkit-overflow-scrolling: touch;" :key="loadKey" :style="constainstyle">
      <div id="canvas" v-if="showPanal" :style="canvasStyle">
        <template v-for="(item, index) in viewData">
          <div :key="index" :style="chartStyle(item)" v-show="item.isShow == true" >
            <component :is="chartName(item)" :theme="viewTheme.themeColor" :isDraw="false" :width="item.width + 'px'"
              :height="item.height + 'px'" :chartOption="item.chartOption" :className="item.chartOption.animate"
              :drawingList="viewData" :customId="item.customId" v-if="item.isShow == true" :alLoadData="alLoadData"
              @setAlLoadData="setAlLoadData" :globalData="viewTheme.globalData" @startload="startload"></component>
          </div>
        </template>
      </div>
      <el-dialog v-if="codeFlag" :visible.sync="codeFlag" width="600px" append-to-body :close-on-click-modal="false">
        <div style="display:flex">
          <label style="width:80px;line-height:40px;color:#fff;font-size:16px">查看密码</label>
          <el-input v-model="code" class="effectSpan" @keydown.enter.native="checkCode"></el-input>
        </div>

        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="checkCode">确认</el-button>
        </div>
      </el-dialog>

      <div id="appLoading">
      </div>
    </div>
  </div>
</template>

<script>

import './animate/animate.css'
import VueEvent from './VueEvent'

import AllComponents from './ComponentsExport'
// import { getFormsource } from "@/api/report/formsource";
// import { chartBIanalysis } from "@/api/report/sourse";
// import { chartApi } from "@/api/report/chartApi";
import { rptInfo, shareInfo, checkShare,reportUpdateTime } from "@/api/report/report";
import { getToken, setShareToken, removeShareToken } from '@/utils/auth'
import { replaceLinkParam } from "./util/LinkageChart"
import Vue from 'vue'
import dataV from '@jiaminghi/data-view'
import { GetDataSourceByIds } from "@/api/report/sourse";
Vue.use(dataV);
const WIDTH = 500 // refer to Bootstrap's responsive design
import {
  confirmValue,
  getbaseData,
  combinationTableColum,
  combinationConfirmValue,
  everyOngetData
} from "@/views/report/datav/LayerItems/commonRuning";
import * as spritejs from "spritejs";
export default {
  components: AllComponents,
  data() {
    return {
      viewTheme: null,
      viewData: [],
      showPanal: false,
      //屏幕宽高
      screenWidth: document.body.offsetWidth,
      screenHeight: document.body.offsetHeight,
      basePath: process.env.VUE_APP_BASE_API,
      drillDownDialogFlag: false,
      drillDownDialogData: null,
      code: undefined,
      codeFlag: false,
      tokenStr: "",
      extractionNumber: "",
      isMobileFlag: false,
      screenId: null,
      globalTimers: [],
      tokenId:'',
      loadKey:11,
      doubleType: 'pc',
      constainstyle:'',
      alLoadData:[],//是否是初始化已经加载的数据集
      sourseList: [], // 数据库列表
      hasloadData:[],
      loadDataRow:[],
      loadComptNum:0,
      shouldLoadData:[],
      container: null,
      scene: null,
      layer: null,
      reportUpTimer:null,//定时检验是否更新
      firstLoadTime:null,
      finishFirstLoad:false,//是否是第一次加载
    }
  },
  // updated(){
  //   //清除loading
  //   this.$nextTick(() => {
  //     document.getElementById('appLoading').style.display = 'none';
  //   })
  // },
  async created() {
    let tokenId = this.$route.query.tokenId;
    if(this.$route.query.tokenid){
      tokenId =this.$route.query.tokenid
    }
    if (tokenId != null) {
      this.tokenId=tokenId
      removeShareToken();
      //参数有tokenid 必须输入验证码
      checkShare(tokenId).then(response => {

        this.screenId = response.data.Share.ReportId;
        this.tokenStr = response.data.Share.TokenStr;
        //放入token
        if (!getToken()) {
          setShareToken(this.tokenStr);
        }
        this.initDatavRelease(response.data.Report);

      }).catch(err=>{
        if(err.code==1){
          this.codeFlag = true;
        }
      });
    } else {
      this.screenId = this.$route.query.screenId;
      if (getToken() && this.screenId != null) {
        this.initDatavRelease();
      }
      else {
        
      }
    }

    VueEvent.$on('tabchange', function (chartList) {
      if (chartList != null) {
        //根据tab标签绑定组件设置组件的显示情况
        for (const obj of this.viewData) {
          const chart = chartList.filter(function (item) {
            return item.customId == obj.customId;
          })[0]

          if (chart != null) {
            obj.isShow = chart.isShow;
            //obj.chartOption.animate = chart.animate;
          }
        }
      }
    }.bind(this))

    //交互组件更新组件配置
    VueEvent.$on('interactChange', function (chartId, act) {
      let index = this.viewData.findIndex(item => item.customId == chartId);
      if (index > -1) {
        if (this.viewData[index].chartOption.interactData != undefined) {
          let chartOption = JSON.parse(JSON.stringify(this.viewData[index].chartOption));
          chartOption.interactData = act;
        }
        this.$set(this.viewData[index].chartOption, 'interactData', act);
      }
    }.bind(this))


    //交互组件控制组件显隐
    VueEvent.$on('interactShow', function (chartId, act) {
      let chart = this.viewData.find(item => item.customId == chartId);
      if (chart != null) {
        if (act == 'show') {
          document.getElementById(chartId).style.visibility = 'visible'
        } else {
          document.getElementById(chartId).style.visibility = 'hidden'
        }
      }
    }.bind(this))

    //控制指定全局数据源刷新
    VueEvent.$on('refreshGlobal', function (name,isNotLoad) {
      // console.log(name, 'namename', this.shouldLoadData);
      if(Array.isArray(name)){
        let optionArr=[]
        let optionAloadArr=[]
        for (let i = 0; i < this.viewTheme.globalData.length; i++) {
          let tmpOption = JSON.parse(JSON.stringify(this.viewTheme.globalData[i]));
          // console.log(name,'namenamename',tmpOption.name);
          let findShould=this.shouldLoadData.find(rwo=>rwo.name==tmpOption.name)
          if (name.includes(tmpOption.name)&&!findShould) {
            this.shouldLoadData.push(tmpOption)
            optionArr.push(tmpOption)
            
          }else if (name.includes(tmpOption.name)){
            optionAloadArr.push(tmpOption)
          }
        }
        if(optionArr&&optionArr.length>0){
          this.initDataName(optionArr,isNotLoad);
        }else if(optionAloadArr&&optionAloadArr.length>0){
          this.initDataName(optionAloadArr,true);
        }
        
      }else{
        for (let i = 0; i < this.viewTheme.globalData.length; i++) {
          let tmpOption = JSON.parse(JSON.stringify(this.viewTheme.globalData[i]));
          let findShould=this.shouldLoadData.find(rwo=>rwo.name==tmpOption.name)
          if (name&&tmpOption.name == name&&!findShould) {
            this.shouldLoadData.push(tmpOption)
            // if(tmpOption&&tmpOption.dataSourceType!="combination"){
              if(this.finishFirstLoad){
                this.initDataName(tmpOption,isNotLoad);
              }
            // }
            
          }else if (name&&tmpOption.name == name){
            if(this.finishFirstLoad){
              this.initDataName(tmpOption,true);
            }
          }
        }
        
      }
    }.bind(this))

  },
  computed: {
    canvasStyle() {
      let background = this.viewTheme.bgImage != '' ? `url(${this.viewTheme.bgImage}) no-repeat` : this.viewTheme.bgColor
      // let background = this.viewTheme.bgImage != '' ? `url(${this.basePath + this.viewTheme.bgImage}) no-repeat` : this.viewTheme.bgColor

      if (this.isMobileFlag&&this.doubleType != 'phone') {

        return {
          background: background,
          display: 'flex',
          flexDirection: 'column',
        }
      } else {
        // let widthRatio=this.screenWidth/this.viewTheme.panelWidth
        // let heightRatio=this.screenHeight/this.viewTheme.panelHeight
        // console.log(widthRatio,heightRatio,'heightRatio');
        return {
          // width: this.viewTheme.isSelfAdaption ? this.screenWidth + "px" : this.viewTheme.panelWidth + "px",
          width: this.viewTheme.panelWidth + "px",
          // height: this.viewTheme.isSelfAdaption ? this.screenHeight + "px" : this.viewTheme.panelHeight + "px",
          height: this.viewTheme.panelHeight + "px",
          background: background,
          position: 'relative',
          overflow: 'hidden',
          
        };
      }


    },

    appContainerStyle(){
      if(this.viewTheme){
        let background = this.viewTheme.bgImage != '' ? `url(${this.viewTheme.bgImage}) no-repeat` : this.viewTheme.bgColor
        return {background: background}
      }else{
        return{}
      }
      
    },
  },
  mounted: function () {
    //监听下钻事件
    VueEvent.$on("drill_down_msg", data => {
      this.drillDownDialogData = data;
      this.drillDownDialogFlag = true;
    })
    const isMobile = this.$_isMobile();
    if (isMobile) {
      this.isMobileFlag = true;
    }
    

  },
  beforeDestroy() {
    for (let i = this.globalTimers.length; i >= 0; i--) {
      clearInterval(this.globalTimers[i]);
    }
  },
  methods: {
    loadReportUpdateTime(){
      reportUpdateTime({id:this.screenId}).then(res=>{//大屏刷新
        // console.log("更新时间",res);
        if(this.firstLoadTime&&this.firstLoadTime!=res.data){
          for (let i = this.globalTimers.length; i >= 0; i--) {
            if(this.globalTimers[i]){
              clearInterval(this.globalTimers[i]);
            }
          }
          
          window.location.reload(true)
          if(this.reportUpTimer){
            clearInterval(this.reportUpTimer)
          }
        }
        this.firstLoadTime=res.data
      })
    },
    async startload(){
      //初始化数据
      this.finishFirstLoad=false
      this.loadComptNum++
      let dataa=this.viewData.filter(row=>row.chartOption.dataSourceType == "gobal")
      if(this.loadComptNum==dataa.length){
        try {
          await this.dataSorting()
        } catch (error) {
          this.$message('加载'+error);
        }
      }
    },
    // 获取数据库列表
    async getRptList(items) {
      try {
        const ids = items.filter(item => item.dataSourceType === 'database').map(item => item.database.sourseItem);
        const idsData = [...new Set(ids)];
        const response = await GetDataSourceByIds(idsData);
        this.sourseList = response.data;
      } catch (error) {
        this.sourseList = [];
      }
    },
    setAlLoadData(val){
      this.alLoadData=val
    },
    async initDataSource() {
      await this.getRptList(this.viewTheme.globalData);
      this.viewTheme.globalData = await this.initBasicInfo(this.viewTheme.globalData);
      for (let i = 0; i < this.viewTheme.globalData.length; i++) {
        let tmpOption = JSON.parse(JSON.stringify(this.viewTheme.globalData[i]));
        if (tmpOption.timeout > 0) {
          let timerTask = () => {
            
            this.globalTimers.push(setInterval(() => {

              this.initDataName(tmpOption,false,true);
            },Number(tmpOption.timeout)*1000));
          };
          timerTask();
        }
        // else {
        //   this.initDataName(tmpOption);
        // }
      }
    },
    // 初始化数据库基本信息
    async initBasicInfo(globalData) {
      for (const item of this.sourseList) {
        for (const v of globalData) {
          if (v.dataSourceType === 'database') {
            if (item.Id === v.database.sourseItem) {
              v.database.username = item.UserName;
              v.database.type = item.DatabaseType;
              v.database.ipAdress = item.IpAddress;
              v.database.baseName = item.DatabaseName;
              v.database.password = item.Password;
              v.database.port = item.Port;
            }
          }
        }
      }
      return globalData
    },
    async dataSorting(){//数据排序，将数据集的子数据排前面
      let notcombination=[]
      let combinationChild=[]
      let combination=[]
      // console.log(this.shouldLoadData.length,'this.shouldLoadData');
      for(let i=0;i<this.shouldLoadData.length;i++){
        let row=this.shouldLoadData[i]
        if(row&&row.dataSourceType=="combination"){
          combination.push(row)
          let arr=this.checkGlobalData2(row.combinationTable,[])
          // console.log(arr,'arrarrarr');
          let notcombinationArr=arr.filter(r=>r.dataSourceType!="combination")
          let combinationChildArr=arr.filter(r=>r.dataSourceType=="combination")
          notcombination=[...notcombinationArr,...notcombination]
          combinationChild=[...combinationChildArr,...combinationChild]
        }else{
          notcombination.push(row)
        }
      }
      let resArr=notcombination.concat(combinationChild, combination)
      // console.log(resArr,'resArrresArr');
      const uniqueArray = resArr.reduce((acc, current) => {
          const duplicate = acc.find(item => item.name === current.name);
          if (!duplicate) {
              acc.push(current);
          }
          return acc;
      }, []);
      for(let j=0;j<uniqueArray.length;j++){
        await this.initDataName(uniqueArray[j]);
      }
      this.finishFirstLoad=true
    },
    checkGlobalData2(dataarr,joinArr){
      for(let i=0;i<dataarr.length;i++){
        let rw=dataarr[i]
        let findrow=this.viewTheme.globalData.find(ro=>ro.name==rw.globalData)
        if(findrow){
          if(findrow.dataSourceType=="combination"){
            joinArr.unshift(findrow)
            let arr=this.checkGlobalData2(findrow.combinationTable,[])
            joinArr=[...arr,...joinArr]
          }else{
            joinArr.unshift(findrow)
          }
        }
        // console.log("循环内部",joinArr);
      }
      return joinArr
    },
    async initDataName(iptOption,isNotLoad,issetInterval,isNotArrload) {
      if(isNotLoad){
        VueEvent.$emit("GlobalData", '', iptOption,isNotArrload);
      }else{
        try {
          let initResult = "";
          let newiptOption=await everyOngetData(iptOption,this.viewTheme.globalData,this.viewData)
          let curitem=null
          this.viewTheme.globalData.find((x, inx) => {
            if (x.name == newiptOption.name) {
              curitem = inx
            }
          });
          if(curitem!=null){
            this.$set(this.viewTheme.globalData, curitem, newiptOption);
          }
          VueEvent.$emit("GlobalData", initResult, newiptOption,isNotArrload);
          // if(issetInterval&&!Array.isArray(iptOption)&&iptOption.dataSourceType!="combination"){
          if(issetInterval&&!Array.isArray(iptOption)){
            let filtarr=this.viewTheme.loadingGlobalDataObj[iptOption.name]
            if(filtarr){
              for(let i=0;i<filtarr.length;i++){
                let index=this.viewTheme.globalData.findIndex(rw=>rw.name==filtarr[i].name)
                if(index!=null&&index!=undefined){
                  let newiptOption2=await everyOngetData(this.viewTheme.globalData[index],this.viewTheme.globalData,this.viewData)
                  this.$set(this.viewTheme.globalData, index, newiptOption2);
                  VueEvent.$emit("GlobalData", '', newiptOption2,isNotArrload);
                }
                
              }
            }
            
          }
        } catch (error) {
          console.log("报错",error);
          this.$message(error);
        }
      }
      
    },
    chartName(item) {
      if (item.chartType == "text") {
        return "NormalText";
      }
      else if (item.chartType == "lamp") {
        return "LampText";
      }
      else if (item.chartType == "date") {
        return "DateText";
      }
      else if (item.chartType == "textCheckBox") {
        return "TextCheckBox";
      }
      else {
        return item.chartType + "Chart";
      }
    },
    $_isMobile() {
      const rect = document.body.getBoundingClientRect()
      const mobile = this.isMobileDevice();
      if (mobile) {
        this.doubleType = 'phone';
      } else {
        this.doubleType = 'pc';
      }
      return mobile  //rect.width - 1 < WIDTH
    },
    isMobileDevice() {//判断当前设备是否为移动端
      const ua = navigator.userAgent.toLowerCase();
      const t1 = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(
        ua
      );
      // const t1=window.matchMedia("(max-width: 767px)").matches
      // const t2 = ua.match("iphone") && navigator.maxTouchPoints > 1;
      return t1;
    },
    chartStyle(val) {

      if (this.isMobileFlag&&this.doubleType != 'phone') {
        return {
          //transform: translate(val.x + "px", val.y + "px"),
          marginLeft: val.x + "px",
          marginTop: 7 + "px",
          marginBottom: 7 + "px",
          width: val.width + "px",
          height: val.height + "px",
          // position: 'absolute',
          'z-index': val.zindex
        }
      } else {

        return {
          //transform: translate(val.x + "px", val.y + "px"),
          left: val.x + "px",
          top: val.y + "px",
          width: val.width + "px",
          height: val.height + "px",
          position: 'absolute',
          'z-index': val.zindex
        }
      }
    },
    async initDatavRelease(rreport) {
      //console.log("渲染组件")
      //this.loading = false
      let sId = this.screenId;

      if (sId != '' && sId != undefined) {
        if (rreport == null) {
          let response = await rptInfo(sId);
          rreport = response.data;
        }

        if (rreport.DrawOption != undefined) {
            let drawingList = [];
            if (rreport.DeviceType === 'double') {
              drawingList = JSON.parse(rreport.DrawOption)[this.doubleType]
              this.viewTheme =  JSON.parse(rreport.ThemeOption)[this.doubleType]
              this.viewTheme.globalData =  JSON.parse(rreport.ThemeOption).globalData
            } else {
              drawingList = JSON.parse(rreport.DrawOption)
              this.viewTheme =  JSON.parse(rreport.ThemeOption)
            }
            this.$nextTick(()=>{
              this.viewTheme.loadingGlobalData=this.viewTheme.globalData.filter(rws=>rws.dataSourceType!="combination")
              let combinationGlobalData=this.viewTheme.globalData.filter(rws=>rws.dataSourceType=="combination")
              let loadingGlobalDataObj={}
              for(let i=0;i<this.viewTheme.loadingGlobalData.length;i++){
                loadingGlobalDataObj[this.viewTheme.loadingGlobalData[i].name]=[]
                for(let j=0;j<combinationGlobalData.length;j++){
                  let findSame=combinationGlobalData[j].combinationTable.find(row=>{
                    if(row.globalData==this.viewTheme.loadingGlobalData[i].name){
                      return true
                    }else{
                      return this.checkGlobalData(row.globalData,this.viewTheme.loadingGlobalData[i].name)
                    }
                  })
                  if(findSame){
                    loadingGlobalDataObj[this.viewTheme.loadingGlobalData[i].name].push(combinationGlobalData[j])
                  }
                }
              }
              this.viewTheme.loadingGlobalDataObj=loadingGlobalDataObj
            })
            this.showPanal = true;
            this.$nextTick(()=>{
              this.reportUpTimer=setInterval(()=>{
                this.loadReportUpdateTime()//
              },5000)
            })
            //是否自适应
            let isSelfAdaption = this.viewTheme.isSelfAdaption;
            //自适应类型
            let adaptionType = this.viewTheme.adaptionType;
            //获取编辑页宽高
            let rectWidth = this.viewTheme.panelWidth;
            let rectHeight = this.viewTheme.panelHeight;

            //是手机端页面
            // console.log(this.isMobileFlag,this.doubleType != 'phone');
            if (this.isMobileFlag&&this.doubleType != 'phone') {
              //设置宽是屏幕宽度的90%
              //高度按原图比例
              drawingList.forEach(item => {
                let originalWidth = item.width;
                item.width = this.screenWidth * 0.95;

                if (originalWidth < this.screenWidth) {

                  item.height = (item.height / originalWidth) * this.screenWidth * 0.95;
                }
                item.x = this.screenWidth * 0.025;
                // item.y = (item.y/rectHeight) * this.screenHeight;
              });

            } else {

              //是自适应
              if (isSelfAdaption) {
                //全自适应
                if (typeof adaptionType == 'undefined' || adaptionType == '0') {
                  //遍历组件重新计算自适应宽高
                  // drawingList.forEach(item => {
                  //   item.width = (item.width / rectWidth) * this.screenWidth;
                  //   item.height = (item.height / rectHeight) * this.screenHeight;
                  //   item.x = (item.x / rectWidth) * this.screenWidth;
                  //   item.y = (item.y / rectHeight) * this.screenHeight;
                  // });
                }
                //宽度自适应
                else if (adaptionType == '1') {
                  //遍历组件重新计算自适应宽高
                  // drawingList.forEach(item => {
                  //   item.width = (item.width / rectWidth) * this.screenWidth;
                  //   item.x = (item.x / rectWidth) * this.screenWidth;
                  // });
                }
                //高度自适应
                else if (adaptionType == '2') {
                  //遍历组件重新计算自适应宽高
                  // drawingList.forEach(item => {
                  //   item.height = (item.height / rectHeight) * this.screenHeight;
                  //   item.y = (item.y / rectHeight) * this.screenHeight;
                  // });
                }

              }
            }
            

            this.viewData = drawingList;
            this.$forceUpdate()
            this.loadKey++
            //初始化数据源
            // this.$nextTick(() => {
              this.initDataSource();
            // });
            if(this.viewTheme.isSelfAdaption){
              let widthRatio=this.screenWidth/this.viewTheme.panelWidth
              let heightRatio=this.screenHeight/this.viewTheme.panelHeight
              if (this.isMobileFlag&&this.doubleType != 'phone'){}else{
                //是自适应
              if (isSelfAdaption) {
                //全自适应
                  if (typeof adaptionType == 'undefined' || adaptionType == '0') {
                    //遍历组件重新计算自适应宽高
                    this.constainstyle='transform:scale('+widthRatio+','+heightRatio+');transform-origin:left top'
                  }
                  //宽度自适应
                  else if (adaptionType == '1') {
                    //遍历组件重新计算自适应宽高
                    this.constainstyle='transform:scale('+widthRatio+');transform-origin:left top'
                  }
                  //高度自适应
                  else if (adaptionType == '2') {
                    //遍历组件重新计算自适应宽高
                    this.constainstyle='transform:scale('+heightRatio+');transform-origin:left top'
                  }
                }
              }
              
              
            }
          } else {
            this.$message("数据初始化失败!");
          }
      } else {
        console.log("sId===" + sId)
        this.$message("请刷新页面");
      }
    },
    checkGlobalData(name,globalData){
      let findrow=this.viewTheme.globalData.find(row=>row.name==name)
      // console.log("查找到的一行",findrow,findrow.name,globalData);
      if(findrow){
        if(findrow.name&&findrow.dataSourceType=="combination"){
          let findSame=findrow.combinationTable.find(row=>{
            // console.log(row.globalData,globalData);
            if(row.globalData==globalData){
              return true
            }else{
              return this.checkGlobalData(row.globalData,globalData)
            }
          })
          if(findSame){
            return true
          }else{
            return false
          }
        }else{
          return  findrow.name==globalData
        }
      }else{
        return false
      }
    },
    checkCode() {
      if(this.tokenId&&this.code){
        removeShareToken();
        //参数有tokenid 必须输入验证码
        checkShare(this.tokenId,this.code).then(async response => {
          this.codeFlag = false;
          this.screenId = response.data.Share.ReportId;
          this.tokenStr = response.data.Share.TokenStr;
          //放入token
          if (!getToken()) {
            setShareToken(this.tokenStr);
          }
          await this.initDatavRelease(response.data.Report);

        }).catch(err=>{
          console.log(err,'errerr');
          if(err.code==1){
            this.codeFlag = true;
          }
        });
      }else{
        if(!this.code){
          this.msgError("请输入查看密码")
        }
      }
      // if (this.extractionNumber == this.code) {
      //   this.codeFlag = false;
      //   this.initDatavRelease();
      // } else {
      //   // this.msgError("您输入的提取码错误！")
      // }

    },
  },
};
</script>
<style scoped>
#appLoading {
  background: url("./image/loading3.gif") no-repeat;
  background-size: 100% 100%;
  height: 100%;
  width: 100%;
  display: none;
  z-index: 9999999;
  position: fixed;
}

.el-dialog__wrapper {
  z-index: 9999999 !important;
}

::v-deep .el-dialog {
  background: #172b47;
}

::v-deep .el-input--medium .el-input__inner {
  height: 36px;
  line-height: 36px;
  color: #FFFFFF;
  background-color: #ffffff00;
}

::v-deep .el-form-item--medium .el-form-item__label {
  line-height: 36px;
  color: #FFFFFF;
}
</style>
