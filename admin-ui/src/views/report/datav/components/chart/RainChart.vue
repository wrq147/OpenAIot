<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  />
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import dataChart from '../mixins/dataChart.js'
import VueEvent from "../../VueEvent";

export default {
  mixins: [resize,dataChart],
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
      chart: null,
      animate: this.className
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    height() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    "chartOption.theme": {
      handler() {
        if (this.chart != null) {
          this.chart.dispose();
          this.chart = null;
        }
      }
    },
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
    if (!this.chart) {
      return;
    }
    this.chart.dispose();
    this.chart = null;
  },
  methods: {
    setChartVal(result) {
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))

      let seriesData = [];
      let legendData = new Set();
      let timeData = new Set();
      for (let d in result) {
        legendData.add(result[d].series);
        timeData.add(result[d].name);
      }
      legendData = [...legendData];
      timeData = [...timeData]
      for (let index = 0; index < legendData.length; index++) {
        let data = {
          data: []
        };
        data.type = "line";
        data.name = result[index];

        result.forEach(v => {
          if (v.series === legendData[index]) {
            data.data.push(v.value)
          }
        })
        if (index == 1) {
          data.yAxisIndex = 1;
        }
        data.animation = false;
        data.areaStyle = {};
        data.lineStyle = {
          width: 1
        };
        //data.color = color[index];
        data.markArea = {
          silent: true
        };
        seriesData.push(data);
      }
      let xAxisTemp = {};
      let yAxisTemp = {};
      xAxisTemp = { ...dataOption.xAxis };
      yAxisTemp = [ ...dataOption.yAxis ];
      let option = {
        title: dataOption.title,
        grid: dataOption.grid,
        toolbox: dataOption.toolbox, //工具箱
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        dataZoom: dataOption.dataZoom, //时间轴
        xAxis: [
          {
            type: 'category',
            show: xAxisTemp[0].show,
            boundaryGap: false,
            axisLine: { onZero: false },
            data: timeData
          }
        ],
        yAxis: yAxisTemp,
        series: seriesData
      };
      this.chart.setOption(option, true);
      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = this.chartOption.bindList;

          if (bindList.length > 0) {
            getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
          }
        });
      }
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let drillDownChartOption = this.chartOption.drillDownChartOption;

          if (
            drillDownChartOption != undefined &&
            drillDownChartOption != null
          ) {
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
        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.off("click");
      }


    },

    /**************************************比较json-start***************************************/
    isObj(object) {
      return (
        object &&
        typeof object == "object" &&
        Object.prototype.toString.call(object).toLowerCase() ==
          "[object object]"
      );
    },
    isArray(object) {
      return object && typeof object == "object" && object.constructor == Array;
    },
    getLength(object) {
      var count = 0;
      for (var i in object) count++;
      return count;
    },
    CompareObj(objA, objB, flag) {
      for (var key in objA) {
        if (!flag)
          //跳出整个循环
          break;
        if (!objB.hasOwnProperty(key)) {
          flag = false;
          break;
        }
        if (!this.isArray(objA[key])) {
          //子级不是数组时,比较属性值
          if (objB[key] != objA[key]) {
            flag = false;
            break;
          }
        } else {
          if (!this.isArray(objB[key])) {
            flag = false;
            break;
          }
          var oA = objA[key],
            oB = objB[key];
          if (oA.length != oB.length) {
            flag = false;
            break;
          }
          for (var k in oA) {
            if (!flag)
              //这里跳出循环是为了不让递归继续
              break;
            flag = this.CompareObj(oA[k], oB[k], flag);
          }
        }
      }
      return flag;
    },
    Compare(objA, objB) {
      if (!this.isObj(objA) || !this.isObj(objB)) return false; //判断类型是否正确
      if (this.getLength(objA) != this.getLength(objB)) return false; //判断长度是否一致
      return this.CompareObj(objA, objB, true); //默认为true
    },
   
  }
};
</script>
