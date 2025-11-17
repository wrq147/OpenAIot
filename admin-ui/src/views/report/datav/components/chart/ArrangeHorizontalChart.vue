<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <div class="content_cot" :style="cotStyle">
      <div class="title" :style="titleStyle"><span>{{ dataObj.title }}</span></div>
      <div class="cot_ul" :style="cotUlStyle">
        <template v-for="(it, inx) in dataObj.currentList">
          <div class="cot_li" :key="it.Name" v-if="it && inx < 4" :style="cotLiStyle">
            <template v-if="it">
              <div class="num" :style="cotLiNumStyle">{{ formatNumber(it.Value, 2, true) }}</div>
              <div class="label" :style="cotLiLabelStyle">{{ it.Name }}<span v-if="it.Unit" :style="cotLiNumStyle">({{ it.Unit }})</span></div>
            </template>
          </div>
        </template>
      </div>
      <div class="cot_ul hasbg" v-if="dataObj.rowListObj" :style="hasbgStyle">
        <div class="ul_title" v-if="dataObj.rowListObj.title&&dataObj.rowListObj.title[0]" :style="cotUlTitleStyle">
          <div class="title_text">{{ dataObj.rowListObj.title[0].Name}}<span v-if="dataObj.rowListObj.title[0].Unit" :style="cotLiUnit2Style">({{ dataObj.rowListObj.title[0].Unit }})</span>
          </div>
          <div class="title_num" :style="cotUlTitleNumStyle">{{ formatNumber(dataObj.rowListObj.title[0].Value, 2, true) }}</div>
        </div>
        <div class="ul_info" v-if="dataObj.rowListObj &&dataObj.rowListObj.list &&dataObj.rowListObj.list.length > 0">
          <template v-for="it in dataObj.rowListObj.list">
            <div class="cot_li" :key="it.Name" v-if="it" :style="cotUlHasbgLiStyle">
              <template v-if="it">
                <div class="num" :style="cotLiNum2Style">{{ formatNumber(it.Value, 2, true) }}</div>
                <div class="label" :style="cotLiLabel2Style">{{ it.Name }}</div>
              </template>
            </div>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import "../../animate/animate.css";
import dataChart from "../mixins/dataChart.js";
import {cardDataHandle} from '../../util/commonChartChange'
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
  },
  data() {
    return {
      textArr: [],
      lampMsg: "",
      loopTimer: null,
      sObj: null,
      animate: this.className,
      dataArr: [],
      dataObj: {
        title: "1号电表(后处理)",
        currentList: [
          {
            Name: "功率",
            Code: "sumkwh",
            Value: 0,
            Unit: "kw",
            OptionType: "float",
            UpdatedOn: "2025-10-28 16:06:57",
            Description: "",
          },
          {
            Name: "A相电流",
            Code: "Fa",
            Value: 0,
            Unit: "A",
            OptionType: "float",
            UpdatedOn: "2025-10-28 16:06:57",
            Description: "",
          },
          {
            Name: "B相电流",
            Code: "Fb",
            Value: 0,
            Unit: "A",
            OptionType: "float",
            UpdatedOn: "2025-10-28 16:06:57",
            Description: "",
          },
          {
            Name: "C相电流",
            Code: "Fc",
            Value: 0,
            Unit: "A",
            OptionType: "float",
            UpdatedOn: "2025-10-28 16:06:57",
            Description: "",
          },
        ],
        rowListObj: {
          title: {
            Name: "当前总电能",
            Code: "Totalkwh",
            Value: 259.2,
            Unit: "kwh",
            OptionType: "float",
            UpdatedOn: "2025-10-28 16:00:58",
            Description: "",
          },
          list: [
            {
              Value: 0,
              Unit: "Nm³",
              Name: "今日用电量",
              Code: "todayuseenerge",
            },
            {
              Value: 0,
              Unit: "Nm³",
              Name: "昨日用电量",
              Code: "yesterdayepi",
            },
            {
              Value: 209.6,
              Unit: "Nm³",
              Name: "本月用电量",
              Code: "currentenerge",
            },
            {
              Value: 20.8,
              Unit: "kwh",
              Name: "上月用电量",
              Code: "preenerge",
            },
          ],
        },
      },
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      },
    },
  },
  created(){
    this.valUpdate = this.setChartVal;
  },
  // mounted() {
  //   this.valUpdate = this.setChartVal;
  // },
  beforeDestroy() {
    if (this.loopTimer != null) {
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
    hasbgStyle(){
      //标题样式
      let widdiff=Number(this.chartOption.cotUlHasbgStyle.marginLeft)+Number(this.chartOption.cotUlHasbgStyle.marginRight)
      let objStyl={
        'padding-left': this.chartOption.cotUlHasbgStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlHasbgStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlHasbgStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlHasbgStyle.paddingBottom+'px',
        'margin-left': this.chartOption.cotUlHasbgStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlHasbgStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlHasbgStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlHasbgStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlHasbgStyle.borderWidth+'px',
        borderColor: this.chartOption.cotUlHasbgStyle.borderColor,
        backgroundColor: this.chartOption.cotUlHasbgStyle.cotBgColor,
        width: 'calc(100% - '+widdiff+'px)',
        'box-sizing': 'border-box',
      }
      return objStyl
    },
    cotUlTitleStyle(){
      //标题样式
      let titleLineHeight=this.chartOption.cotUlTitleStyle.fontSize
      if(this.chartOption.cotUlTitleNumStyle.fontSize>this.chartOption.cotUlTitleStyle.fontSize){
        titleLineHeight=this.chartOption.cotUlTitleNumStyle.fontSize
      }
      let widdiff=Number(this.chartOption.cotUlTitleStyle.marginLeft)+Number(this.chartOption.cotUlTitleStyle.marginRight)
      let objStyl={
        fontSize: this.chartOption.cotUlTitleStyle.fontSize+'px',
        'line-height':titleLineHeight+'px',
        fontFamily: this.chartOption.cotUlTitleStyle.fontFamily,
        fontWeight: this.chartOption.cotUlTitleStyle.fontWeight,
        color: this.chartOption.cotUlTitleStyle.fontColor, //字体颜色
        'padding-left': this.chartOption.cotUlTitleStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlTitleStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlTitleStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlTitleStyle.paddingBottom+'px',
        'border-radius': this.chartOption.cotUlTitleStyle.borderRadius+'px',
        'margin-left': this.chartOption.cotUlTitleStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlTitleStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlTitleStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlTitleStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlTitleStyle.borderWidth+'px', //下边框
        borderColor: this.chartOption.cotUlTitleStyle.borderColor,
        letterSpacing: this.chartOption.cotUlTitleStyle.letterSpacing+'px', //字体间距
        textAlign: this.chartOption.cotUlTitleStyle.textAlign,
        width: 'calc(100% - '+widdiff+'px)',
      }
      return objStyl
    },
    cotUlTitleNumStyle(){//标题数字样式
      let objStyl={
        fontSize: this.chartOption.cotUlTitleNumStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlTitleNumStyle.fontFamily,
        fontWeight: this.chartOption.cotUlTitleNumStyle.fontWeight,
        color: this.chartOption.cotUlTitleNumStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlTitleNumStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlTitleNumStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlTitleNumStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlTitleNumStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlTitleNumStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlTitleNumStyle.textAlign+'px',
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
    cotUlHasbgLiStyle(){
      //标题样式
      let objStyl={
        width: this.chartOption.cotUlHasbgLiStyle.width+'%',
        'padding-left': this.chartOption.cotUlHasbgLiStyle.paddingLeft+'px',
        'padding-right': this.chartOption.cotUlHasbgLiStyle.paddingRight+'px',
        'padding-top': this.chartOption.cotUlHasbgLiStyle.paddingTop+'px',
        'padding-bottom': this.chartOption.cotUlHasbgLiStyle.paddingBottom+'px',
        'border-radius': this.chartOption.cotUlHasbgLiStyle.borderRadius+'px',
        'margin-left': this.chartOption.cotUlHasbgLiStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlHasbgLiStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlHasbgLiStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlHasbgLiStyle.marginBottom+'px',
        borderWidth: this.chartOption.cotUlHasbgLiStyle.borderWidth+'px',
        borderColor: this.chartOption.cotUlHasbgLiStyle.borderColor,
        backgroundColor: this.chartOption.cotUlHasbgLiStyle.cotBgColor,
      }
      return objStyl
    },
    cotLiLabelStyle(){
      //标签
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
      //数字
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
      //单位
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
    
    cotLiLabel2Style(){
      let objStyl={
        fontSize: this.chartOption.cotUlHasbgLiLabelStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlHasbgLiLabelStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlHasbgLiLabelStyle.fontFamily,
        fontWeight: this.chartOption.cotUlHasbgLiLabelStyle.fontWeight,
        color: this.chartOption.cotUlHasbgLiLabelStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlHasbgLiLabelStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlHasbgLiLabelStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlHasbgLiLabelStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlHasbgLiLabelStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlHasbgLiLabelStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlHasbgLiLabelStyle.textAlign,
      }
      return objStyl
    },
    cotLiNum2Style(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlHasbgLiValueStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlHasbgLiValueStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlHasbgLiValueStyle.fontFamily,
        fontWeight: this.chartOption.cotUlHasbgLiValueStyle.fontWeight,
        color: this.chartOption.cotUlHasbgLiValueStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlHasbgLiValueStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlHasbgLiValueStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlHasbgLiValueStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlHasbgLiValueStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlHasbgLiValueStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlHasbgLiValueStyle.textAlign+'px',
      }
      return objStyl
    },
    cotLiUnit2Style(){
      //标题样式
      let objStyl={
        fontSize: this.chartOption.cotUlHasbgLiUnitStyle.fontSize+'px',
        'line-height': this.chartOption.cotUlHasbgLiUnitStyle.fontSize+'px',
        fontFamily: this.chartOption.cotUlHasbgLiUnitStyle.fontFamily,
        fontWeight: this.chartOption.cotUlHasbgLiUnitStyle.fontWeight,
        color: this.chartOption.cotUlHasbgLiUnitStyle.fontColor, //字体颜色
        letterSpacing: this.chartOption.cotUlHasbgLiUnitStyle.letterSpacing+'px', //字体间距
        'margin-left': this.chartOption.cotUlHasbgLiUnitStyle.marginLeft+'px',
        'margin-right': this.chartOption.cotUlHasbgLiUnitStyle.marginRight+'px',
        'margin-top': this.chartOption.cotUlHasbgLiUnitStyle.marginTop+'px',
        'margin-bottom': this.chartOption.cotUlHasbgLiUnitStyle.marginBottom+'px',
        textAlign: this.chartOption.cotUlHasbgLiUnitStyle.textAlign+'px',
      }
      return objStyl
    },
  },
  methods: {
    formatNumber(num, cent, isThousand) {
      if(num==null||num==undefined) return ''
      num = num.toString().replace(/\$|\,/g, "");
      if (num == "-") {
        return num;
      }
      // 检查传入数值为数值类型
      if (isNaN(num)) num = "0";
      // 获取符号(正/负数)
      let sign = num == (num = Math.abs(num));
      // num = Math.floor(num * Math.pow(10, cent) + 0.50000000001); // 把指定的小数位先转换成整数.多余的小数位四舍五入
      num = Math.floor(num * Math.pow(10, cent));
      let cents = num % Math.pow(10, cent); // 求出小数位数值
      num = Math.floor(num / Math.pow(10, cent)).toString(); // 求出整数位数值
      cents = cents.toString();

      // 把小数位转换成字符串,以便求小数位长度
      // 补足小数位到指定的位数
      while (cents.length < cent) cents = "0" + cents;
      if (isThousand) {
        // 对整数部分进行千分位格式化.
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
          num =num.substring(0, num.length - (4 * i + 3)) +"," +num.substring(num.length - (4 * i + 3));
      }
      if (cent > 0 && cents > 0) return (sign ? "" : "-") + num + "." + cents;
      else return (sign ? "" : "-") + num;
    },
    textWidth() {
      let lampWidth =this.lampMsg.length * parseInt(this.chartOption.fontSize) + 5;

      const style = { position: "absolute", width: lampWidth + "px" };
      return style;
    },
    setChartVal(resData, rowGlobal) {
      // this.dataArr = result;
      // console.log(rowGlobal,'rowGlobalrowGlobal');
      try {
        let result=[]
        if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
          result =cardDataHandle(rowGlobal,this.chartOption)?cardDataHandle(rowGlobal,this.chartOption):[]
        }else{
          result = this.chartOption.staticDataValue;
        }
        // console.log(result,'result');
        if(result&&result[0]){
          let resultRow=JSON.parse(JSON.stringify(result[0]))
          this.dataObj={
            title:resultRow.title,
            currentList:resultRow.currentList?resultRow.currentList:resultRow.currentlist,
            rowListObj:{
              title:resultRow.rowtitle?resultRow.rowtitle:resultRow.rowListObj.title,
              list:resultRow.list?resultRow.list:resultRow.rowListObj.list,
            },
          }
        }
        
      } catch (error) {
        console.log("真的报错了",error);
      }
    },
  },
};
</script>
<style lane="less" scoped>
.content_cot {
  width: 100%;
  height: 100%;
  border: 1px solid #ffffff;
}
.content_cot .title {
    /* height: 46px;
    padding: 14px; */
    font-size: 14px;
    color: rgba(255, 255, 255, 1);
    line-height: 14px;
    box-sizing: border-box;
    width: 100%;
    text-align: left;
}

.content_cot .cot_ul {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 0;
}

.content_cot .cot_ul .cot_li {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: calc(100% / 3);
    padding: 0;
    border: 1px solid #ffffff;
    /* height: 56px; */
}

.content_cot .cot_ul.hasbg {
    flex-direction: column;
    background: rgba(31, 87, 245, 0.2);
    /* width: calc(100% - 24px); */
    margin: 0 12px;
    border-radius: 4px;
    margin-top: 6px;
    border: 1px solid #ffffff;
}

.content_cot .cot_ul.hasbg .ul_title {
    /* width: calc(100% - 24px); */
    margin: 0 12px;
    /* height: 40px; */
    padding: 12px 0 14px 0;
    box-sizing: border-box;
    font-size: 12px;
    line-height: 12px;
    color: #ffffff;
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.content_cot .cot_ul.hasbg .ul_title .title_num {
    color: rgba(60, 199, 255, 1);
    font-size: 14px;
}

.content_cot .cot_ul.hasbg .ul_info {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
}

.content_cot .cot_ul.hasbg .ul_info .cot_li {
    width: 25%;
    /* height: 60px; */
}

.content_cot .cot_li .label {
    min-width: none;
    color: rgba(255, 255, 255, 0.8);
    font-size: 12px;
    line-height: 12px;
}

.content_cot .cot_li .num {
    min-width: none;
    font-size: 14px;
    color: rgba(60, 199, 255, 1);
    line-height: 14px;
    margin-bottom: 10px;
}

.content_cot .cot_li .unit {
    min-width: none;
}

.content_cot .cot_ul.pie_ul {
    display: flex;
    justify-content: space-between;
    width: calc(100% - 24px);
    margin: 6px 12px 0;
}

.content_cot .cot_ul.pie_ul .cot_li {
    width: calc(50% - 3px);
    /* height: 60px; */
    background: rgba(31, 87, 245, 0.4);
    border-radius: 4px;
}


</style>
