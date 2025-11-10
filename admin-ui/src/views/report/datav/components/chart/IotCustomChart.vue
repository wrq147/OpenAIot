<template>
  <div :class="[animate,,'configuration'+chartOption.bindingDiv]" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv"></div>
</template>

<script>
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";
import zutaiChart from "../mixins/zutaiChart.js";
import imgUrl1 from '@/views/report/datav/image/zutai/fengji2/1.png'
import { isArray } from 'xe-utils';
export default {
  mixins: [resize, dataChart, zutaiChart],
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
      chart: null,
      value: "",
      animate: this.className,
      chartKey:0
    };
  },
  watch: {
    "chartOption.theme": {
      handler() {},
    },
    className: {
      handler(value) {
        this.animate = value;
      },
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    
  },
  beforeDestroy() {},
  computed: {
    
  },
  methods: {
    setChartVal(result) {
      // console.log("组态数据",result,this.chartOption);
      this.$nextTick(() => {
        let dom = document.getElementById(this.chartOption.bindingDiv);
        if (dom) {
          dom.addEventListener("mouseup", this.domOnMouseUp, true);
        }
        // const imgUrl=require(this.chartOption.svgUrl)
        // const imgUrl = "https://s5.ssl.qhres2.com/static/ec9f373a383d7664.svg";
        let imgUrl=null
        let tableSelectLine=this.chartOption.tableSelectLine
        if(tableSelectLine&&isArray(tableSelectLine)){
          let filedObj=tableSelectLine.find(rw=>rw.key=='filed')
          let valueObj=tableSelectLine.find(rw=>rw.key=='value')
          if(filedObj&&filedObj.filed&&valueObj&&valueObj.filed){
            
            let conditionGroup=this.chartOption.conditionGroup
            for(let i=0;i<conditionGroup.length;i++){
              if(conditionGroup[i].animation!==null&&conditionGroup[i].animation!==''){//判断对应条件组动画不为空
                let rowCondition=conditionGroup[i].condition
                let rowCompare=false//比较结果
                for(let j=0;j<rowCondition.length;j++){//循环条件组条件
                  if(rowCondition[j]!==null&&rowCondition[j]!==''){//确定条件不为空
                    let colCondi=this.chartOption.conditionList[rowCondition[j]]
                    if(colCondi&&colCondi.field){
                      let resFiledRow=null
                      // console.log(result,'resultresult');
                      if(result&&isArray(result)){
                        resFiledRow=result.find(rw=>rw[filedObj.filed]==colCondi.field)
                      }
                      if(resFiledRow){
                        let colRes=this.setNumRes(resFiledRow[valueObj.filed],colCondi.value,colCondi.symbol)
                        if(j==0){
                          rowCompare=colRes
                        }else{
                          if(conditionGroup[i].groups[j-1]){
                            if(conditionGroup[i].groups[j-1]=='and'){//处理多个条件是与时需同时满足
                              rowCompare=rowCompare&&colRes
                            }
                            if(conditionGroup[i].groups[j-1]=='or'){
                              rowCompare=rowCompare||colRes
                            }
                          }
                        }
                      }
                    }
                  }
                }
                // console.log(rowCompare,'rowCompare');
                if(rowCompare){
                  if(this.chartOption.animationList[conditionGroup[i].animation]){
                    imgUrl=this.chartOption.animationList[conditionGroup[i].animation].imglist
                  
                  }
                  
                }
              }
            }
          }
        }
        
        if(imgUrl){
          this.sprite(imgUrl);
        }else{
          imgUrl=imgUrl1
          this.sprite(imgUrl);
        }
        
      });
    },
    setNumRes(a,b,symbol){//根据比较符号比较数据
      // console.log("比较数据",a,b,symbol);
      if(Number(a)!=NaN&&Number(b)!=NaN){
        if(symbol=='>'){
          return a>b
        }
        if(symbol=='=='){
          return a==b
        }
        if(symbol=='<'){
          return a<b
        }
        if(symbol=='>='){
          return a>=b
        }
        if(symbol=='<='){
          return a<=b
        }
        if(symbol=='><'){
          return a!=b
        }
      }else{
        return false
      }
      
    }
  },
};
</script>
