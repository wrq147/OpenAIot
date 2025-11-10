<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="text"
  >
    <template v-for="(item, rowindex) in rows">
      <div :style="rowStyle" :key="rowindex">
        <div :style="colorTop"></div>
        <template v-for="(col, colindex) in item">
          <div
            :style="colorRow"
            :key="colindex"
            @click="fieldClick(col)"
            class="color"
          >
            <div :style="getColorBlock(rowindex, colindex)">
              <div :style="colorText">{{ col.name }}</div>
              <div :style="colorValue">
                <span :style="colorSpan">{{ col.value }}</span>
                <span :style="colorSuffix">{{ col.suffix }}</span>
              </div>
            </div>
          </div>
        </template>
      </div>
    </template>
  </div>
</template>

<script>

import VueEvent from "../../VueEvent";
import "../../animate/animate.css";


import { getLinkChart } from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'
import {objectArrayDataHandle} from '../../util/commonChartChange'
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
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      rows: [],
      colorheight: "",
      colorwidth: "",
      animate: this.className
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
    
  },
  computed: {
    rowStyle() {
      let style = {
        width: "100%",
        display: "flex",
        flexDirection: "row",
        height: this.colorheight + "%"
      };
      return style;
    },
    colorTop() {
      const style = { height: this.chartOption.marginTop + "%" };
      return style;
    },
    colorRow() {
      const style = { width: this.colorwidth + "%", height: "100%" };
      return style;
    },
    colorText() {
      let style = {
        justifyContent: this.chartOption.text.left,
        fontSize: this.chartOption.text.size + "px",
        fontFamily: this.chartOption.text.family,
        fontWeight: this.chartOption.text.weight,
        color: this.chartOption.text.color,
        display: "flex",
        alignItems: "center"
      };
      if (
        this.chartOption.textLocate == "left" ||
        this.chartOption.textLocate == "right"
      ) {
        style.width = this.chartOption.text.width + "%";
        style.height = "100%";
        style.lineHeight = "100%";
      } else {
        style.width = "100%";
        style.height = "50%";
        style.lineHeight = "100%";
      }
      return style;
    },
    colorValue() {
      let style = {
        display: "flex",
        alignItems: "center",
        justifyContent: this.chartOption.value.left
      };
      if (
        this.chartOption.textLocate == "left" ||
        this.chartOption.textLocate == "right"
      ) {
        style.width = 100 - this.chartOption.text.width + "%";
        style.height = "100%";
        style.lineHeight = "100%";
      } else {
        style.width = "100%";
        style.height = "50%";
        style.lineHeight = "100%";
      }
      return style;
    },
    colorSpan() {
      let style = {
        fontSize: this.chartOption.value.size + "px",
        fontFamily: this.chartOption.value.family,
        fontWeight: this.chartOption.value.weight,
        color: this.chartOption.value.color,
        letterSpacing: this.chartOption.value.letterSpacing + "px"
      };
      return style;
    },
    colorSuffix() {
      let style = {
        display: "inline-block",
        marginLeft: "5px",
        textAlign: "left",
        fontSize: this.chartOption.suffix.size + "px",
        fontFamily: this.chartOption.suffix.family,
        fontWeight: this.chartOption.suffix.weight,
        color: this.chartOption.suffix.color,
        width: this.chartOption.suffix.width + "%"
      };
      return style;
    }
  },
  methods: {
    setChartVal(resData,rowGlobal) {
      let result=[]
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        result =objectArrayDataHandle(rowGlobal,this.chartOption)?objectArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        result = this.chartOption.staticDataValue;
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      //根据千位分隔符格式化数字
      if (dataOption.isThousand != "false") {
        result.forEach(element => {
          element.value = this.thousandDevide(
            element.value,
            dataOption.isThousand
          );
        });
      }
      this.rows = this.getRows(result);
    },
    getRows(rss){
      //根据列数计算数据的行数，并按顺序将数据构造成按指定行、列分隔的数组
      let results = [];
      //列数，判断列数不能大于数据数组的总长度
      let colsNum = rss.length;
      if(this.chartOption.numOfCol < rss.length){
        colsNum = this.chartOption.numOfCol;
      }
      let col = 0;//颜色块行数
      for(let i = 0;i < rss.length;){
        let rows = [];
        for(let j = 0; j < colsNum && i < rss.length; j++){
          //每行数据
          rows.push(rss[i])
          i++
        }
        results.push(rows)
        col ++ ;
      }
      //每个颜色块宽度占比
      this.colorwidth = (100-this.chartOption.marginLeft*colsNum)/colsNum;

      //每个颜色块高度占比
      this.colorheight = (100-(col)*this.chartOption.marginTop)/(col);
      return results
    },
    thousandDevide(data, devide) {
      //格式化数字
      if (typeof data != "number") {
        return data;
      } else {
        if (!devide) {
          devide = ",";
        }
        var res = data.toString().replace(/\d+/, function(n) {
          // 先提取整数部分
          return n.replace(/(\d)(?=(\d{3})+$)/g, function($1) {
            return $1 + devide;
          });
        });
        return res;
      }
    },
    getColorBlock(rowindex, colindex) {
      //console.log('num=====',this.chartOption.colorBlock);
      let style = {
        width: "100%",
        height: "100%",
        marginLeft: this.chartOption.marginLeft + "%",
        borderRadius: this.chartOption.bradius + "px",
        display: "flex"
      };
      if (this.chartOption.textLocate == "left") {
        style.flexDirection = "row";
      } else if (this.chartOption.textLocate == "right") {
        style.flexDirection = "row-reverse";
      } else if (this.chartOption.textLocate == "top") {
        style.flexDirection = "column";
      } else {
        style.flexDirection = "column-reverse";
      }
      //根据二维数组坐标获取颜色块背景
      //  if(this.chartOption.colorBlock.length > 0){

      //    let num = (colindex) + (this.chartOption.numOfCol* (rowindex+1)) -this.chartOption.numOfCol
      //    style.backgroundColor = this.chartOption.colorBlock[num].color
      //  }

      return style;
    },
    fieldClick(col) {
      //开启图表联动
      if (this.chartOption.isLink == true) {
        //设置参数
        let arrs = JSON.stringify(col);

        //获取绑定的图表
        let bindList = this.chartOption.bindList;

        if (bindList.length > 0) {
          getLinkChart(this.chartOption.arrName, arrs, bindList, this.drawingList);
        }
      }
      //开启图表下钻
      else if (this.chartOption.isDrillDown == true) {
        //设置参数
        let arrs = JSON.stringify(col);

        //获取绑定的图表
        let drillDownChartOption = this.chartOption.drillDownChartOption;

        if (drillDownChartOption != undefined && drillDownChartOption != null) {
          this.$set(
            this.chartOption.drillDownChartOption.chartOption,
            "requestParameters",
            [{"name":"drillParam","value":arrs}]
          );
          //发送下钻消息
          VueEvent.$emit(
            "drill_down_msg",
            this.chartOption.drillDownChartOption
          );
        }
      }

    },
   
  }
};
</script>
<style ang="scss" scoped>
.color:hover {
  cursor: default;
}
</style>
