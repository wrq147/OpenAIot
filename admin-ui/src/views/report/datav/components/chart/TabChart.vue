<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv">
    <ul :style="{ height: height, width: width}" class="tabul"  ref="text">
      <li class="tabli" :style="liWidth" v-for="(item,index) in tabValue" @click="changeLi(index, tabValue)" :key="index"  @mouseenter="pause" @mouseleave="start">
        <div class="tabBorder" :style="index == selectedIndex?(chartOption.selectedBGFlag == 'img'? selectedImgStyle:selectedBorderStyle):(chartOption.normalBGFlag == 'img'?normalImgStyle:normalBorderStyle)">
          <div class="tabText" :style="index == selectedIndex?selectedTextStyle:normalTextStyle">
            {{item.content}}
          </div>
        </div>
      </li>
    </ul>
  </div>
  
</template>

<script>

import VueEvent from '../../VueEvent'
import '../../animate/animate.css'
import dataChart from '../mixins/dataChart.js'
import {objectArrayDataHandle} from '../../util/commonChartChange'
export default {
  mixins: [dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
    drawingList:{
      type: Array
    }
  },
  data() {
    return {
      selectedIndex:0,
      tabValue:[],
      carouselTimer: '',
      animate: this.className
    };
  },
  watch: {
    width() {
    },
    height() {
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    },
    $route: {
      handler: function (route) {
        // console.log(route,);
        if(route.query){
          let pars = route.query
          if(this.chartOption.params&&pars[this.chartOption.params]){
            this.tabValue=pars[this.chartOption.params]
          }
        }
       
      },
      immediate: true,
    },
  },
  mounted() {
    if(this.chartOption.params){
      let pars = this.$route.query;
      if(pars[this.chartOption.params]){
        this.tabValue=pars[this.chartOption.params]
      }
      
    }
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.tabValue;
      };
    }
  },
  beforeDestroy() {
    if(this.carouselTimer!=null){
      clearTimeout(this.carouselTimer);
    }
  },
  computed:{
    normalBorderStyle(){
      const style = {
        borderWidth:this.chartOption.borderWidth + "px",
        borderColor:this.chartOption.borderColor,
        backgroundColor: this.chartOption.backgroundColor, 
      };
      return style
    },
    selectedBorderStyle(){
      const style = {
        borderWidth:this.chartOption.selectedBorderWidth + "px",
        borderColor:this.chartOption.selectedBorderColor,
        backgroundColor: this.chartOption.selectedBackgroundColor, 
      };
      return style
    },
    normalTextStyle(){
      const style = {color:this.chartOption.fontColor,fontSize:this.chartOption.fontSize + "px",fontFamily:this.chartOption.fontFamily,
        fontWeight:this.chartOption.fontWeight,letterSpacing:this.chartOption.letterSpacing + "px",textAlign:this.chartOption.textAlign}
      return style
    },
    selectedTextStyle(){
      const style = {color:this.chartOption.selectedFontColor,fontSize:this.chartOption.selectedFontSize + "px",fontFamily:this.chartOption.selectedFontFamily,
        fontWeight:this.chartOption.selectedFontWeight,letterSpacing:this.chartOption.selectedLetterSpacing + "px",textAlign:this.chartOption.textAlign}
      return style
    },
    normalImgStyle(){
      const style = {
        borderWidth:'0px',
        borderColor:'',
        backgroundColor: '', 
        backgroundImage: `url(${this.chartOption.normalTabBG}) `,
        backgroundRepeat : 'no-repeat',
        backgroundSize : '100% 100%',
        //animation: 'fade 2s ease-in' 
      };
      return style
    },
    selectedImgStyle(){
      const style = {
        borderWidth:'0px',
        borderColor:'',
        backgroundColor: '',
        backgroundImage:`url(${this.chartOption.selectedTabBG}) `,
        backgroundRepeat : 'no-repeat',
        backgroundSize : '100% 100%',
        //animation: 'fade 2s ease-in'
      };
      return style
    },
    liWidth(){

      const num = this.tabValue.length;//分割成几个tab
      let style = {};

      if(this.chartOption.direction == 'vertical'){
          style = {width: 100 + '%',height:100/num + "%"};
      }else{
          style = {height: 100 + '%',width:100/num + "%"};
      }     
      //console.log("width:" + parseInt(100/num) + "%");
      return style
    }
  },
  methods: {
    setChartVal(result,rowGlobal) {
      if(this.chartOption.params){
        let pars = this.$route.query;
        if(pars[this.chartOption.params]){
          this.tabValue=pars[this.chartOption.params]
        }
        
      }
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.tabValue =objectArrayDataHandle(rowGlobal,this.chartOption)?objectArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        this.tabValue = this.chartOption.staticDataValue;
      }
      // this.tabValue = result;
      this.changeLi(0,result);
      this.rotationChart(result);
    },
    changeLi(index, result){
      this.$emit("onChange", result[index]);
      this.selectedIndex = index;

      //如果开启了远程控制
      if(this.chartOption.isRemote === true && this.chartOption.remoteKey !== undefined && this.chartOption.remoteKey !== ''){
        
        let remoteData = {};
        
        let selectObj = result[index];
        remoteData.result = selectObj;
        remoteData.key = this.chartOption.remoteKey;

        remoteTabApi(remoteData);

      }


      //先将所有组件设置为显示状态
      // this.drawingList.forEach(element => {
      //       element.isShow = true;                   
      // });
      
      const chartList = this.drawingList;
      if(chartList != null){
        const tabList = [];
        //获取当前选中绑定tab id
        const bindingObj = result[index];
        
      
        if(this.chartOption.bindingObjs.length > 0){
          //将所有绑定组件设置隐藏
          for(const obj of this.chartOption.bindingObjs){
            //判断如果新版选项卡则循环获取绑定的多个组件
            if(typeof obj.chartids != "undefined" && obj.chartids != '' ){
              
              obj.chartids.forEach(element => {
                const chart =  chartList.filter(function (item) {
                  return item.customId == element && obj.bindid != null;
                })[0]
                if(chart != null){
                    tabList.push({customId:chart.customId,isShow:false})
                }        
              });

            }
            //旧版选项卡按照原来方式获取绑定的单个组件
            else{
               
                const chart =  chartList.filter(function (item) {
                  return item.customId == obj.chartid && obj.bindid != null;
                })[0]
                if(chart != null){
                    tabList.push({customId:chart.customId,isShow:false})
                }        
             
            }
             
          } 
          
          // 根据绑定tab id 获取 bindingObjs对应对象
          const bindObj = this.chartOption.bindingObjs.filter(function(item){
            return item.bindid == bindingObj.bindid;
          })[0]
            
          if(bindObj != null){
            //在tabList中将选中组件设置为显示
            //判断如果是新版选项卡，则循环获取绑定的组件
            if(typeof bindObj.chartids != "undefined" && bindObj.chartids != ''){
              
              bindObj.chartids.forEach(element => {
                const selectedChart = tabList.filter(function (item) {         
                  return item.customId ==element;
                })[0] 
                if(selectedChart != null){
                  selectedChart.isShow = true;                             
                }  
              });
              
            }
            //旧版选项卡按照原来方式获取绑定组件
            else{
              const selectedChart = tabList.filter(function (item) {         
                return item.customId == bindObj.chartid;
              })[0]   
              if(selectedChart != null){
                selectedChart.isShow = true;                            
              }  
            }
                       
          }

          if(tabList != null){
            //向父组件发送事件，同步修改drawinglist            
            VueEvent.$emit('tabchange',tabList)  
          }
                  
        }
      }
      
    },
    
    rotationChart(result){
      //判断是否开启定时器，选择开启轮播并且dur不为0时候开启定时器
      if(this.carouselTimer != '') {
        clearTimeout(this.carouselTimer);
      }
      if(this.chartOption.isRotation == true && this.chartOption.dur > 0) {
       
        let rotationIndex = 0;
        let timerTask = () => {
          this.changeLi(rotationIndex);
          rotationIndex ++ ;
          if(rotationIndex == result.length){
            rotationIndex = 0;
          }
          this.carouselTimer = setTimeout(() => {
            timerTask();
          }, this.chartOption.dur);
        }

        timerTask();
      }
    },
    start(){
      if(this.chartOption.isRotation == true && this.chartOption.dur > 0) {
       
        let rotationIndex = this.selectedIndex;
        let timerTask = () => {
          this.changeLi(rotationIndex);
          rotationIndex ++ ;
          if(rotationIndex == this.tabValue.length){
            rotationIndex = 0;
          }
          this.carouselTimer = setTimeout(() => {
            timerTask();
          }, this.chartOption.dur);
        }

        timerTask();
      }
    },
    pause(){
      if(this.chartOption.isRotation == true && this.chartOption.dur > 0) {
        clearTimeout(this.carouselTimer);
      }
    },
  
  }
};
</script>
<style ang="scss" scoped>
.tabBorder{
	border-style: solid;
	height: 100%;
	position: relative;
}
.tabText{
	position: absolute;
  top : 38%;
  width: 100%;
}
.tabli{
  list-style: none; 
  padding-left: 1px;
  float: left;
  cursor: default;
}
.tabul{
  position:relative;
  list-style: none;
  margin:0; 
  padding:0; 
}
.fade-enter-active, .fade-leave-active {
    transition: opacity 2s
}
.fade-enter, .fade-leave-to /* .fade-leave-active, 2.1.8 版本以下 */ {
    opacity: 0
}
</style>
