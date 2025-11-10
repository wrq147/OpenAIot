<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv">
    
    <div :style="textStyle" @click="handleClick">
        {{chartOption.textStyle.content}}
    </div>
   <div v-if="flag" :id="chartOption.bindingDiv+'-wrapper'" style="z-index:99999;position:fixed;top:0;right:0;bottom:0;left:0;overflow:auto;margin:0;background:#ffffff42">
      <div :id="chartOption.bindingDiv+'-dialog'" style="z-index:99999;position: relative;margin: 0 auto 50px;background-color: #fff;margin-top: 15vh;width: 30%;height:400px">

        <div :id="chartOption.bindingDiv+'-close'" style="z-index:99999;float:right;margin-right:20px;margin-top:10px;cursor:default" @click="handleClose">关闭 <i class="el-icon-close" style="font-size:16px"></i></div>

      </div>
    </div>
  </div>
</template>

<script>

import '../../animate/animate.css'
import VueEvent from '../../VueEvent'
import dataChart from '../mixins/dataChart.js'

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
    pageState:{
      type: String,
      default: "edit",
    },
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      animate: this.className,
      flag: false,
      wapperId: this.chartOption.bindingDiv+"-wapper",
      closeId:this.chartOption.bindingDiv+"-close",
      staticValue:""
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
    "chartOption.isControl": {
      handler(value) {
        //切换数据源重新连接
        if (!value) {
          //每次切换数据源恢复原来动画
          this.animate = this.className;
        }
      }
    },
    "chartOption.controlKey": {
      handler(newValue, oldValue) {
        this.animate = this.className;
      }
    },
    flag: {
      handler(value) {
        if (value) {
           this.$nextTick(() => {
            this.initDataSource();
          });
        }
      }
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    this.$nextTick(() => {
        //设置弹窗父组件置顶
       if(this.pageState != 'edit'){
        document.getElementById(this.chartOption.bindingDiv).parentNode.style.zIndex = "9999";
        this.chartOption.events.forEach(element => {
          if(element.chartHide){
             document.getElementById(element.chart).style.visibility = 'hidden'
          }
        });

      }

    });
  },
  
  beforeDestroy() {

  },
  computed:{
    textStyle(){
      let style = {
        width:"100%",
        height:"100%",
        fontSize: this.chartOption.textStyle.fontSize + "px",
        fontFamily: this.chartOption.textStyle.fontFamily,
        fontWeight: this.chartOption.textStyle.fontWeight,
        color: this.chartOption.textStyle.fontColor,
        letterSpacing:this.chartOption.textStyle.letterSpacing + "px",
        lineHeight:this.chartOption.textStyle.lineHeight + "px",
        textAlign: this.chartOption.textStyle.textAlign,
        cursor:"default"
      };

      if(this.chartOption.background.type == 'color'){
        style.backgroundColor = this.chartOption.background.backgroundColor;
      }else{
        style.backgroundImage = `url(${this.chartOption.background.backgroundImg}) `;
        style.backgroundRepeat = 'no-repeat';
        style.backgroundSize = '100% 100%';
      }

        

      return style;
    }
  },
  methods: {
    setChartVal(result) {
      this.staticValue = result;
      dataOption.events.forEach(element => {
        if(element.type == 'change'){
          this.runCode(element)
        }
      });


    },
    runCode(element){
      if(this.pageState != 'edit'){

        if(element.condition != undefined && element.condition != ""){
              //执行条件代码
              let state = (
              
                //里面为要处理的代码块
                eval(element.condition) 
              
              )(
                this.staticValue,

              );

              //如果条件为真
              if(typeof state == "boolean" && state == true){
                if(element.chartType == 'popup'){
                  //当前选中弹窗置顶，其余弹窗恢复原有zindex
                  this.drawingList.forEach(element => {
                    if(element.chartType == 'interact'){
                      document.getElementById(element.chartOption.bindingDiv).parentNode.style.zIndex = "9999";
                    }
                  });

                    document.getElementById(this.chartOption.bindingDiv).parentNode.style.zIndex = "99999";

                  this.flag = true;
                  

                }else{
                  //如果绑定组件不为空并且动作不为空
                  if(element.chart != null && element.act != ''){

                    //组件数据变化
                    if(element.act == 'change'){

                      VueEvent.$emit('interactChange',element.chart,element.customConfig) 

                    }
                    //组件显隐
                    else{
                      VueEvent.$emit('interactShow',element.chart,element.act)
                    }
                    
                  }
                
                }

              }
                
        }
        
        
      }
    },
    handleClick(){
      if(this.pageState != 'edit'){

        this.chartOption.events.forEach(element => {
          if(element.type == 'click'){
            if(element.chartType == 'popup'){
              
              //当前选中弹窗置顶，其余弹窗恢复原有zindex
              
              this.drawingList.forEach(element => {
                if(element.chartType == 'interact'){
                  document.getElementById(element.chartOption.bindingDiv).parentNode.style.zIndex = "9999";
                }
                 
              });

               document.getElementById(this.chartOption.bindingDiv).parentNode.style.zIndex = "99999";

              this.flag = true;
              //如果开启了远程控制
              if(this.chartOption.isRemote === true && this.chartOption.remoteKey !== undefined && this.chartOption.remoteKey !== ''){
                
                let remoteData = {};
                
                remoteData.result = true;
                remoteData.key = this.chartOption.remoteKey;

                remoteTabApi(remoteData);

              }

            }else{
              //如果绑定组件不为空并且动作不为空
              if(element.chart != null && element.act != ''){

                let remoteData = {};

                //组件数据变化
                if(element.act == 'change'){
                  remoteData.result = {chart:element.chart,act:element.customConfig};
                 

                  VueEvent.$emit('interactChange',element.chart,element.customConfig) 

                }
                //组件显隐
                else{
                  remoteData.result = {chart:element.chart,act:element.act};
                  VueEvent.$emit('interactShow',element.chart,element.act)
                }

                
              }
            }
          }
          
        });
      }
    },
    handleClose(){
      this.flag = false;

    },
   
   
  },
};
</script>

<style lang="scss" scoped>

</style>
