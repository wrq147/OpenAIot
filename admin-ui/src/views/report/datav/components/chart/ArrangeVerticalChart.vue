<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <div class="content_cot" :style="cotStyle">
      <div class="title" :style="titleStyle">
        {{ dataObj.title }}<span class="number" :style="titleBunberStyle">{{ dataObj.xuhao }}</span>
      </div>
      <div class="cot_ul" :style="cotUlStyle">
        <div class="cot_li" v-for="it in dataObj.list" :key="it.cnName" :style="cotLiStyle">
          <div class="label" :style="cotLiLabelStyle">{{ it.cnName }}</div>
          <div class="num" :style="cotLiNumStyle">{{ formatNumber(it.value, 2, true) }}</div>
          <div class="unit" v-if="it.unit" :style="cotLiUnitStyle">{{ it.unit }}</div>
        </div>
        <div class="cot_li" v-if="dataObj.currentlist && dataObj.currentlist.length > 0" :style="cotUlRowsStyle">
          <div class="num2 hasright" v-for="(it, ix) in dataObj.currentlist" :key="'current' + ix" :style="cotUlRowsLiStyle">
            <span :style="cotUlRowsLiValueStyle">{{ formatNumber(it.value, 2, true) }}</span>
            <span style="color: #ffffff" :style="cotUlRowsLiUnitStyle">{{ it.unit }}</span>
            <span v-if="ix < dataObj.currentlist.length - 1" :style="cotUlRowsLiLineStyle">/</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
import {cardDataHandle} from '../../util/commonChartChange'
export default {
  mixins: [dataChart],
  props: {
    className: {
      type: String,
      default: "chart"
    },
    width: {
      type: String,
      default: "100%"
    },
    height: {
      type: String,
      default: "100%"
    },
  },
  data() {
    return {
      textArr: [],
      lampMsg: "",
      loopTimer:null,
      sObj: null,
      animate: this.className,
      dataArr:[],
      dataObj:{
        title:'智慧电表',
        xuhao:'(2#气站，2号表)',
        list:[
          {cnName:'当前总电能',value:2013130,unit:'kwh'},
          {cnName:'功率',value:163.20,unit:'kw'},
          {cnName:'上月累计电能',value:109436,unit:'kwh'},
          {cnName:'本月累计电能',value:79666,unit:'kwh'},
          {cnName:'昨日累计电能',value:4410,unit:'kwh'},
        ],
        currentlist:[{cnName:'A相电流',value:272.40,unit:'A'},{cnName:'B相电流',value:270,unit:'A'},{cnName:'C相电流',value:246,unit:'A'}]
      }
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    if(this.loopTimer!=null){
      clearTimeout(this.loopTimer);
    }
  },
  computed: {
    cotStyle(){
      let objSty={
        'padding-left': this.chartOption.cotStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotStyle.paddingBottom+'px',
        'border-radius': this.chartOption.cotStyle.borderRadius+'px',
        'margin-left': this.chartOption.cotStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotStyle.marginRight+'px',
        'margin-top': this.chartOption.cotStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotStyle.marginBottom+'px',
        'border-width': this.chartOption.cotStyle.borderWidth+'px',
        'border-color': this.chartOption.cotStyle.borderColor+'px'
      }
      if(this.chartOption.cotStyle.bgType=='img'){
        objSty={...objSty,
        backgroundColor: 'transparent',
        backgroundImage :  `url(${this.chartOption.cotStyle.containerBgImage}) `,
        backgroundSize :  "cover",
        backgroundRepeat :  "no-repeat",}
      }else{
        objSty.backgroundColor=this.chartOption.cotStyle.containerColor
      }
      return objSty
    },
    titleStyle(){
      //标题样式
      let objStyl={
        'text-align': this.chartOption.containerTitle.textAlign,
        'font-size': this.chartOption.containerTitle.fontSize+'px',
        'line-height': this.chartOption.containerTitle.fontSize+'px',
        'font-family': this.chartOption.containerTitle.fontFamily,
        'font-weight': this.chartOption.containerTitle.fontWeight,
        color: this.chartOption.containerTitle.fontColor, //字体颜色
        'padding-left': this.chartOption.containerTitle.paddingLeft+'px',
        'padding-right': this.chartOption.containerTitle.paddingRight+'px',
        'padding-top': this.chartOption.containerTitle.paddingTop+'px',
        'padding-bottom': this.chartOption.containerTitle.paddingBottom+'px',
        'border-radius': this.chartOption.containerTitle.borderRadius,
        'margin-left': this.chartOption.containerTitle.marginLeft+'px',
        'margin-right': this.chartOption.containerTitle.marginRight+'px',
        'margin-top': this.chartOption.containerTitle.marginTop+'px',
        'margin-bottom': this.chartOption.containerTitle.marginBottom+'px',
        borderWidth: this.chartOption.containerTitle.borderWidth+'px', //下边框
        borderColor: this.chartOption.containerTitle.borderColor,
        letterSpacing: this.chartOption.containerTitle.letterSpacing+'px', //字体间距
        'box-sizing': 'border-box',
      }
      return objStyl
    },
    titleBunberStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.titleBunberStyle.fontSize+'px',
        'line-height': this.chartOption.titleBunberStyle.fontSize+'px',
        fontFamily: this.chartOption.titleBunberStyle.fontFamily,
        fontWeight: this.chartOption.titleBunberStyle.fontWeight,
        color: this.chartOption.titleBunberStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.titleBunberStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.titleBunberStyle.marginLeft+'px',
        'margin-right': this.chartOption.titleBunberStyle.marginRight+'px',
        'margin-top': this.chartOption.titleBunberStyle.marginTop+'px',
        'margin-bottom': this.chartOption.titleBunberStyle.marginBottom+'px',
      }
      return objStyl
    },
    cotUlStyle(){
      //标题样式
      let objStyl={
        'padding-left': this.chartOption.cotUlStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlStyle.paddingBottom+'px',
        'margin-left': this.chartOption.cotUlStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlStyle.borderWidth+'px',
        borderColor: this.chartOption.cotUlStyle.borderColor,
        backgroundColor: this.chartOption.cotUlStyle.cotBgColor,
      }
      return objStyl
    },
    cotLiStyle(){
      //标题样式
      let objStyl={
        width: this.chartOption.cotUlLiStyle.width+'%',
        'padding-left': this.chartOption.cotUlLiStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlLiStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlLiStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlLiStyle.paddingBottom+'px',
        'border-radius': this.chartOption.cotUlLiStyle.borderRadius+'px',
        'margin-left': this.chartOption.cotUlLiStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlLiStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlLiStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlLiStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlLiStyle.borderWidth+'px',
        borderColor: this.chartOption.cotUlLiStyle.borderColor,
        backgroundColor: this.chartOption.cotUlLiStyle.cotBgColor,
      }
      return objStyl
    },
    cotLiLabelStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlLiLabelStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlLiLabelStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlLiLabelStyle.fontFamily,
        fontWeight: this.chartOption.cotUlLiLabelStyle.fontWeight,
        color: this.chartOption.cotUlLiLabelStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlLiLabelStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlLiLabelStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlLiLabelStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlLiLabelStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlLiLabelStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlLiLabelStyle.textAlign,
      }
      return objStyl
    },
    cotLiNumStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlLiValueStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlLiValueStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlLiValueStyle.fontFamily,
        fontWeight: this.chartOption.cotUlLiValueStyle.fontWeight,
        color: this.chartOption.cotUlLiValueStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlLiValueStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlLiValueStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlLiValueStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlLiValueStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlLiValueStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlLiValueStyle.textAlign+'px',
      }
      return objStyl
    },
    cotLiUnitStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlLiUnitStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlLiUnitStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlLiUnitStyle.fontFamily,
        fontWeight: this.chartOption.cotUlLiUnitStyle.fontWeight,
        color: this.chartOption.cotUlLiUnitStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlLiUnitStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlLiUnitStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlLiUnitStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlLiUnitStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlLiUnitStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlLiUnitStyle.textAlign+'px',
      }
      return objStyl
    },
    cotUlRowsStyle(){
      let objStyl={
        width: 'calc('+this.chartOption.cotUlRowsStyle.width+'% - '+this.chartOption.cotUlRowsStyle.marginLeft+'px'+' - '+this.chartOption.cotUlRowsStyle.marginRight+'px)',
        'padding-left': this.chartOption.cotUlRowsStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlRowsStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlRowsStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlRowsStyle.paddingBottom+'px',
        'margin-left': this.chartOption.cotUlRowsStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlRowsStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlRowsStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlRowsStyle.marginBottom+'px',
        'margin-bottom': this.chartOption.cotUlRowsStyle.marginBottom+'px',
        'border-radius': this.chartOption.cotUlRowsStyle.borderRadius+'px',
        borderColor: this.chartOption.cotUlRowsStyle.borderColor,
        backgroundColor: this.chartOption.cotUlRowsStyle.cotBgColor,
      }
      return objStyl
    },
    cotUlRowsLiStyle(){
      let objStyl={
        width: 'calc('+this.chartOption.cotUlRowsLiStyle.width+'% - '+this.chartOption.cotUlRowsLiStyle.marginLeft+'px'+' - '+this.chartOption.cotUlRowsLiStyle.marginRight+'px)',
        'padding-left': this.chartOption.cotUlRowsLiStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlRowsLiStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlRowsLiStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlRowsLiStyle.paddingBottom+'px',
        'border-radius': this.chartOption.cotUlRowsLiStyle.borderRadius+'px',
        'margin-left': this.chartOption.cotUlRowsLiStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlRowsLiStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlRowsLiStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlRowsLiStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlRowsLiStyle.borderWidth+'px',
        borderColor: this.chartOption.cotUlRowsLiStyle.borderColor,
        backgroundColor: this.chartOption.cotUlRowsLiStyle.cotBgColor,
      }
      return objStyl
    },
    cotUlRowsLiValueStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlRowsLiValueStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlRowsLiValueStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlRowsLiValueStyle.fontFamily,
        fontWeight: this.chartOption.cotUlRowsLiValueStyle.fontWeight,
        color: this.chartOption.cotUlRowsLiValueStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlRowsLiValueStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlRowsLiValueStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlRowsLiValueStyle.marginRight+'px',
      }
      return objStyl
    },
    cotUlRowsLiUnitStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlLiUnitStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlLiUnitStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlLiUnitStyle.fontFamily,
        fontWeight: this.chartOption.cotUlLiUnitStyle.fontWeight,
        color: this.chartOption.cotUlLiUnitStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlLiUnitStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlLiUnitStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlLiUnitStyle.marginRight+'px',
      }
      return objStyl
    },
    cotUlRowsLiLineStyle(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlRowsLiLineStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlRowsLiLineStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlRowsLiLineStyle.fontFamily,
        fontWeight: this.chartOption.cotUlRowsLiLineStyle.fontWeight,
        color: this.chartOption.cotUlRowsLiLineStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlRowsLiLineStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlRowsLiLineStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlRowsLiLineStyle.marginRight+'px',
      }
      return objStyl
    },
  },
  methods: {
    formatNumber(num, cent, isThousand) {//
      if(num==null||num==undefined) return ''
      num = num.toString().replace(/\$|\,/g, "");
      // 检查传入数值为数值类型
      if (isNaN(num)) num = "0";
      // 获取符号(正/负数)
      let sign = num == (num = Math.abs(num));
      num = Math.floor(num * Math.pow(10, cent) + 0.50000000001); // 把指定的小数位先转换成整数.多余的小数位四舍五入
      let cents = num % Math.pow(10, cent); // 求出小数位数值
      num = Math.floor(num / Math.pow(10, cent)).toString(); // 求出整数位数值
      cents = cents.toString();

      // 把小数位转换成字符串,以便求小数位长度
      // 补足小数位到指定的位数
      while (cents.length < cent) cents = "0" + cents;
      if (isThousand) {
        // 对整数部分进行千分位格式化.
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
          num =
            num.substring(0, num.length - (4 * i + 3)) +
            "," +
            num.substring(num.length - (4 * i + 3));
      }
      if (cent > 0 && cents > 0) return (sign ? "" : "-") + num + "." + cents;
      else return (sign ? "" : "-") + num;
    },
    textWidth() {
      let lampWidth =
        this.lampMsg.length * parseInt(this.chartOption.fontSize) + 5;

      const style = { position: "absolute", width: lampWidth + "px" };
      return style;
    },
    setChartVal(result,rowGlobal) {
      // this.dataArr = result;
      try {
        let result=[]
        if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
          result =cardDataHandle(rowGlobal,this.chartOption)?cardDataHandle(rowGlobal,this.chartOption):[]
        }else{
          result = this.chartOption.staticDataValue;
        }
        // console.log(result,'resultresultresult');
        if(result&&result[0]){
          let resultRow=JSON.parse(JSON.stringify(result[0]))
          this.dataObj={
            title:resultRow.title,
            xuhao:resultRow.xuhao,
            list:resultRow.list,
            currentlist:resultRow.currentList,
          }
        }
        
      } catch (error) {
        console.log("真的报错了",error);
      }
    },
    
  
  }
};
</script>
<style ang="scss" scoped>
.content_cot{
  width: 100%;
  height: 100%;
}
.content_cot .title {
    width: 100%;
    height: 34px;
    line-height: 34px;
    text-align: left;
    font-size: 16px;
    padding-left: 14px;
    box-sizing: border-box;
}

.content_cot .title .number {
    font-size: 14px;
    color: rgba(255, 255, 255, 0.5);
    margin-left: 3px;
}

.content_cot .cot_ul {
    margin-top: 20px;
    width: 100%;
}

.content_cot .cot_ul .cot_li {
    display: flex;
    justify-content: space-between;
    width: 100%;
    padding-right: 14px;
    box-sizing: border-box;
    font-size: 14px;
    line-height: 34px;
    /* margin-top: 0.2rem; */
    padding-left: 14px;
}

.content_cot .cot_ul .cot_li>div {
    white-space: nowrap;
    display: flex;
    justify-content: flex-start;
}

.content_cot .cot_ul .cot_li .num {
    color: #3cc7ff;
    display: flex;
    justify-content: space-between;
}

.content_cot .cot_ul .cot_li .num .hasright {
    margin-right: -5px;
    width: calc(33.3% - 5px);
}

.content_cot .cot_ul .cot_li .num span {
    margin-right: 5px;
}

.content_cot .cot_ul .cot_li .num2 {
    color: #3cc7ff;
    display: flex;
    justify-content: space-between;
}

.content_cot .cot_ul .cot_li .num2.hasright {
    margin-right: -5px;
    width: calc(33.3% - 5px);
    box-sizing: border-box;
}

.content_cot .cot_ul .cot_li .num2 span {
    margin-right: 5px;
}
</style>