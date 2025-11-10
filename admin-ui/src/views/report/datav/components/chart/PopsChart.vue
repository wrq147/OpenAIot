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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));

      let colorList = [
        "#ff7f50",
        "#87cefa",
        "#da70d6",
        "#32cd32",
        "#6495ed",
        "#ff69b4",
        "#ba55d3",
        "#cd5c5c",
        "#ffa500",
        "#40e0d0",
        "#1e90ff",
        "#ff6347",
        "#7b68ee",
        "#00fa9a",
        "#ffd700",
        "#6b8e23",
        "#ff00ff",
        "#3cb371",
        "#b8860b",
        "#30e0e0"
      ];

      let sizeList = [
        48,
        73,
        67,
        50,
        88,
        55,
        70,
        67,
        47,
        82,
        59,
        90,
        134,
        75,
        68,
        62,
        114,
        130,
        123,
        141,
        48,
        73,
        67,
        50,
        88,
        55,
        70,
        67,
        47,
        82,
        59,
        90,
        134,
        75,
        68,
        62,
        114,
        130,
        123,
        141
      ];

      if (dataOption.isTopic == true) {
        if (this.chartOption.theme != null) {
          colorList = this.chartOption.theme.color;
        } else {
          colorList = colorList;
        }
      } else if (dataOption.isTopic == false) {
        colorList = colorList;
      }

      let seriesData = [];
      //$.each(dataOption.staticDataValue, function (index, value) {
        result.forEach((value, index) => {
        //console.log("value", value);
        var data = {};
        data.name = value.name;
        data.value = value.data;
        //data.symbolSize = Math.ceil(Math.random()*100);
        data.symbolSize = sizeList[index];
        data.draggable = true;
        data.itemStyle = {
          normal: {
            shadowBlur: dataOption.shadowBlur,
            shadowColor: colorList[index % colorList.length],
            color: colorList[index % colorList.length]
          }
        };

        seriesData.push(data);
      });

      let option = {
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center"
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        tooltip: dataOption.tooltip,
        //legend: dataOption.legend,
        animationDurationUpdate: function(idx) {
          // 越往后的数据延迟越大
          return idx * 100;
        },
        animationEasingUpdate: "bounceIn",
        series: [
          {
            type: "graph",
            layout: "force",
            force: {
              repulsion: dataOption.repulsion, //泡泡的间距
              edgeLength: 10
            },
            roam: true,
            label: {
              normal: {
                show: true,
                textStyle: {
                  color: dataOption.fontColor ? dataOption.fontColor : "#fff",
                  fontSize: dataOption.fontSize ? dataOption.fontSize : 15
                }
              }
            },
            data: seriesData
          }
        ],
        bindingType: "pops"
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
    
  }
};
</script>
