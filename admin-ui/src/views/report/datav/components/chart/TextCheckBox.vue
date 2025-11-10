<template>
  <div :class="animate" :style="{ height: height, width: width,overflowY:'hidden' }" :id="chartOption.bindingDiv" ref="text" @mouseenter="visible" @mouseleave="invisible">
      <!-- <div :style="normalItem"> -->
        <span v-for="(item, index) in boxes" 
            class="chooseCard" 
            :name="[isClicked(index) ? 'onSelected' : '']"
            :class="[isClicked(index) ? 'onSelected' : '']"
            :style="[isClicked(index) ? selectedStyle: normalStyle]"
            :id="item.key" 
            @click="chooseBox(index)"
            @mouseenter="pause" @mouseleave="start"
            :key="index"
        >
        {{ item.value }} 
        </span>
      <!-- </div> -->
  </div>
</template>

<script>
import "../../animate/animate.css";
import { parseQueryString, fIsUrL } from "../../util/urlUtil";
import dataChart from '../mixins/dataChart.js'
import {objectArrayDataHandle} from '../../util/commonChartChange'
import { t } from '@wangeditor/editor';
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
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      boxes: this.chartOption.staticDataValue,
      clicked_list: [],
      max_clicked: this.chartOption.staticDataValue.length,
      paramStr: '',
      animate: this.className,
      carouselTimer:"",//轮播定时器
      rotationIndex:0,//选中序号
    };
  },
  watch: {
    width() {},
    height() {},
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
            this.paramStr=pars[this.chartOption.params]
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
        this.paramStr=pars[this.chartOption.params]
      }
      
    }
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.paramStr;
      };
    }
  },
  beforeDestroy() {
    if(this.carouselTimer!=null){
      clearTimeout(this.carouselTimer);
    }
  },
  computed: {
    selectId() {
      return "boxId" + this.customId;
    },
    normalItem() {
        this.width = '100%',
        this.height = '100%'
    },
    normalStyle() {
      const style = {
        // 字体
        color: this.chartOption.fontColor,
        fontSize: this.chartOption.fontSize + "px",
        fontWeight: this.chartOption.fontWeight,
        fontFamily: this.chartOption.fontFamily,
        letterSpacing: this.chartOption.letterSpacing + "px",

        // 内边距
        paddingLeft: this.chartOption.paddingLeftRight + "px",
        paddingRight: this.chartOption.paddingLeftRight + "px",
        paddingTop: this.chartOption.paddingTopBottom + "px",
        paddingBottom: this.chartOption.paddingTopBottom + "px",
        // 外边距
        marginLeft: this.chartOption.marginLeftRight + "px",
        marginRight: this.chartOption.marginLeftRight + "px",
        marginTop: this.chartOption.marginTopBottom + "px",
        marginBottom: this.chartOption.marginTopBottom + "px", 
        
      };
      //设置背景为图片
      if(this.chartOption.normalBGFlag == 'img'){
        // 边框
        style.borderWidth = "0px";
        style.borderColor = undefined;
        style.backgroundColor =  undefined;
        style.backgroundImage = `url(${this.chartOption.normalBGImage}) `;
        style.backgroundRepeat =  'no-repeat';
        style.backgroundSize = '100% 100%';

      }
      //设置背景为颜色
      else{
        // 边框
        style.borderWidth = this.chartOption.borderWidth + "px";
        style.borderColor = this.chartOption.borderColor;
        style.backgroundColor = this.chartOption.backgroundColor;
      }

      return style;
    },
    selectedStyle() {
      const style = {
        // 字体
        color: this.chartOption.selectedFontColor,
        fontSize: this.chartOption.selectedFontSize + "px",
        fontWeight: this.chartOption.selectedFontWeight,
        fontFamily: this.chartOption.selectedFontFamily,
        letterSpacing: this.chartOption.selectedLetterSpacing + "px",

        // 内边距
        paddingLeft: this.chartOption.paddingLeftRight + "px",
        paddingRight: this.chartOption.paddingLeftRight + "px",
        paddingTop: this.chartOption.paddingTopBottom + "px",
        paddingBottom: this.chartOption.paddingTopBottom + "px",
        // 外边距
        marginLeft: this.chartOption.marginLeftRight + "px",
        marginRight: this.chartOption.marginLeftRight + "px",
        marginTop: this.chartOption.marginTopBottom + "px",
        marginBottom: this.chartOption.marginTopBottom + "px",
      };

      //设置背景为图片
      if(this.chartOption.selectedBGFlag == 'img'){
        // 边框
        style.borderWidth = "0px";
        style.borderColor = undefined;
        style.backgroundColor =  undefined;
        style.backgroundImage = `url(${this.chartOption.selectedBGImage}) `;
        style.backgroundRepeat =  'no-repeat';
        style.backgroundSize = '100% 100%';

      }
      //设置背景为颜色
      else{
        // 边框
        style.borderWidth = this.chartOption.selectedBorderWidth + "px";
        style.borderColor = this.chartOption.selectedBorderColor;
        style.backgroundColor = this.chartOption.selectedBackgroundColor;
      }

      return style;
    },
    
  },
  methods: {
    visible:function(){
      let style = document.getElementById(this.chartOption.bindingDiv).style;
      style.overflowY = 'auto'
    },
    invisible:function(){
      let style = document.getElementById(this.chartOption.bindingDiv).style;
      style.overflowY = 'hidden'
    },
    chooseBox(index) {
        this.$emit("onChange", this.boxes[index]);
        //如果开启了远程控制
        if(this.chartOption.isSameRemote === true && this.chartOption.sameRemoteKey !== undefined && this.chartOption.sameRemoteKey !== ''){
          
          let remoteData = {};
          
          remoteData.result = this.clicked_list;
          remoteData.key = this.chartOption.sameRemoteKey;

          remoteTabApi(remoteData);

        }
        //alert("OvO");
        let c_clicked_list = this.clicked_list;
        if(this.chartOption.checkType == 'single'){
            //单选
            this.clicked_list=[];
            this.clicked_list.push(index);
            
        }else{
            //如果点击块数大于或等于最大点击块数 && 点击的色块未变色: 直接返回
            if (c_clicked_list.length >= this.max_clicked && (c_clicked_list.indexOf(index) == -1)){
                return
            }
            var listIdx = c_clicked_list.indexOf(index);
            if (listIdx > -1) {
                this.clicked_list.splice(listIdx, 1);
            } else {
                this.clicked_list.push(index);
            }
           
        }
            
        // 0. 获取当前绑定组件id
        // console.log(">3<", this.clicked_list);
        
        this.$nextTick(() => {
            // 1. 创建请求参数的字符串
            let arr = [];
            let slectedList = document.getElementsByName("onSelected")
            //console.log('slectedList', slectedList);
            if(slectedList.length == 0 ) { 
              
              // alert("无选中的标签，请选择标签。"); 
              return ;
            } else { 
                
                for (let index of c_clicked_list) {
                   
                    arr.push(this.boxes[index].key);
                    //.log("arr", arr);
                }
                    this.paramStr = arr.join(',');
            }
        
            // 2. 把参数赋给绑定组件的请求参数
            let bindList = this.chartOption.selectedCharts;
            let name = this.chartOption.name;
            if(bindList.length > 0){
                let exclusion = ["input","timeframe", "select", "cascade","tab","textCheckBox","timeline","group"];
                //console.log("O-O", this.drawingList);
                let renderCharts = this.drawingList.filter(item => { 
                  // console.log(item.customId,'uuuuuuuuuuuuuuuuuuuuu',item.chartType,item);
                    return bindList.indexOf(item.customId) > -1 && exclusion.indexOf(item.chartType) == -1
                })
                let that = this;
                
                //遍历绑定组件
                renderCharts.forEach(item => {

                //获取组件参数
                let requestParameters = item.chartOption.requestParameters;

                //如果已经包含该名称的参数则替换
                if(requestParameters != "" && requestParameters.indexOf(name) != -1){
                    //拆分成数组
                    let paramArr = requestParameters.split('&');
                    if(paramArr.length>1){
                      paramArr.pop();
                    }
                    //获取到包含该名称的数组项
                    for(let i in paramArr){
                    if(paramArr[i].indexOf(name) != -1){
                        //替换位=为当前内容
                        //console.log('你猜我有没有值', that.paramStr)
                        paramArr.splice(i, 1, name + "=" + that.paramStr)
                    
                    }
                    }
                    //将数组重新按照&符号拼接为字符串
                    requestParameters = '&' + paramArr.join("&")
                    
                } else {
                    if(requestParameters != ""){
                      let paramArr = requestParameters.split('&');
                      if(paramArr.length>1){
                        paramArr.pop();
                      }
                      requestParameters = '&' + paramArr.join("&")
                    }
                    //如果为新名称参数直接拼在结尾
                    requestParameters += "&" + this.chartOption.name + "=" + that.paramStr;
                }
                //判断参数是否已&符号开始，是则删除该符号
                if(requestParameters.indexOf('&') == 0){
                    item.chartOption.requestParameters = requestParameters.substring(1, requestParameters.length) + "&timestamp="+new Date().getTime();
                }else{
                    // 3. 给绑定组件重新赋值参数渲染组件
                    item.chartOption.requestParameters = requestParameters + "&timestamp="+new Date().getTime();
                }
                
                });

            }
  
		    } )	

    },

    isClicked(index) {
        return this.clicked_list.indexOf(index)>-1;
    },
    setChartVal(result,rowGlobal) {
      if(this.chartOption.params){
        let pars = this.$route.query;
        if(pars[this.chartOption.params]){
          this.paramStr=pars[this.chartOption.params]
        }
        
      }
      this.rotationIndex = 0;
      // if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      // };
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.boxes =objectArrayDataHandle(rowGlobal,this.chartOption)?objectArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        this.boxes = this.chartOption.staticDataValue;
      }
      // this.boxes = result;
      if(this.chartOption.checkType == "single"){
        this.rotationChart();
      }
    },
    rotationChart(){
      //判断是否开启定时器，选择开启轮播并且dur不为0时候开启定时器
      if(this.carouselTimer != null) {
        clearTimeout(this.carouselTimer);
      }
      if(this.chartOption.isRotation == true && this.chartOption.dur > 0 && this.boxes.length > 0) {
       
        let timerTask = () => {
          this.chooseBox(this.rotationIndex);
          this.rotationIndex ++ ;
          if(this.rotationIndex == this.boxes.length){
            this.rotationIndex = 0;
          }
          this.carouselTimer = setTimeout(() => {
            timerTask();
          }, this.chartOption.dur);
        }

        timerTask();
      }
    },
    start(){
      if(this.chartOption.isRotation == true && this.chartOption.dur > 0 && this.boxes.length > 0) {
       
        let timerTask = () => {
          this.chooseBox(this.rotationIndex);
          this.rotationIndex ++ ;
          if(this.rotationIndex == this.boxes.length){
            this.rotationIndex = 0;
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
 
  },
};
</script>
<style ang="scss" scoped>
.chooseCard {
    float: left;
    cursor: pointer;
}

</style>
