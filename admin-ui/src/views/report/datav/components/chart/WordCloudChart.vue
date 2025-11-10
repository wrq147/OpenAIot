<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv" />
</template>

<script>
import echarts from "echarts";
import "echarts-wordcloud/dist/echarts-wordcloud.min";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import VueEvent from "../../VueEvent";
import dataChart from '../mixins/dataChart.js'
import {objectArrayDataHandle} from '../../util/commonChartChange'
export default {
  mixins: [resize, dataChart],
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
  methods: {
    setChartVal(resdata,rowGlobal) {
      let result=[]
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        result =objectArrayDataHandle(rowGlobal,this.chartOption)?objectArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        result = this.chartOption.staticDataValue;
      }
      if (this.chart == null) {
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      let color = this.theme.color;

      const textStyle = {
        normal: {
          color: function () {
            const i = Math.round(Math.random() * (color.length + 1));
            return color[i];
          }
        },
        emphasis: {
          shadowBlur: 10,
          shadowColor: "#333"
        }
      };
      //封装数据
      let seriesData = [];
      let wordData = {};

      wordData.gridSize = parseInt(dataOption.gridSize);
      wordData.type = "wordCloud";
      //文字大小
      let sizeRange = [];
      sizeRange.push(parseInt(dataOption.sizeMin));
      sizeRange.push(parseInt(dataOption.sizeMax));
      //旋转角度
      let rotationRange = [];
      rotationRange.push(parseInt(dataOption.rotationMin));
      rotationRange.push(parseInt(dataOption.rotationMax));

      wordData.sizeRange = sizeRange;
      wordData.rotationRange = rotationRange;
      //形状
      wordData.shape = dataOption.shape;
      //文字样式
      wordData.textStyle = textStyle;

      wordData.data = result;
      seriesData.push(wordData);

      let option = {
        tooltip: dataOption.tooltip,
        series: seriesData,
        bindingType: "wordcloud"
      };


      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            seriesName: params.name,
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
      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            seriesName: params.name,
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
              [{ "name": "drillParam", "value": arrs }]
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

  }
};
</script>
