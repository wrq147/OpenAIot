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
import VueEvent from "../../VueEvent";
import dataChart from '../mixins/dataChart.js'
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

      let charts = Array.from(result); //es6 数组拷贝

      if (dataOption.isThemeColor == true) {
        //获取主题颜色
        let customcolor = this.chartOption.theme.color;
        for (var i = 0; i < dataOption.color.length; i++) {
          dataOption.color[i].color0 = customcolor[i % customcolor.length];
        }
      }

      let length = dataOption.color.length;
      if (length < dataOption.staticDataValue.length) {
        for (let i = length; i < dataOption.staticDataValue.length; i++) {
          let cblock = dataOption.color[i % length];
          dataOption.color.push(cblock);
        }
      } else if (length > dataOption.staticDataValue.length) {
        dataOption.color = dataOption.color.slice(
          0,
          dataOption.staticDataValue.length
        );
      }
      //排序  start-----------------------------------------------------------
      if (dataOption.order == "ascend") {
        for (let i = 0; i < charts.length - 1; i++) {
          for (let j = i + 1; j < charts.length; j++) {
            if (charts[i].value < charts[j].value) {
              let data = charts[i];
              charts[i] = charts[j];
              charts[j] = data;
            }
          }
        }
      } else if (dataOption.order == "descend") {
        //降序
        for (let i = 0; i < charts.length - 1; i++) {
          for (let j = i + 1; j < charts.length; j++) {
            if (charts[i].value > charts[j].value) {
              let data = charts[i];
              charts[i] = charts[j];
              charts[j] = data;
            }
          }
        }
      }
      // 排序 end-------------------------------------------------------------

      //圆角显示
      let raduis = dataOption.radius == undefined ? 12 : dataOption.radius;
      if (dataOption.radiusShow == undefined) {
        raduis = 12;
      } else {
        if (!dataOption.radiusShow) {
          raduis = 0;
        }
      }

      let top10CityList = [];
      let top10CityData = [];
      let top10CityDataY = [];
      let color = [];
      let lineY = [];
      let llength = charts.length;
      for (let i = 0; i < llength; i++) {
        let x = i;
        x %= dataOption.color.length;

        top10CityDataY.push(charts[llength - i - 1].value);
        top10CityList.push(charts[i].name);
        top10CityData.push(charts[i].value);
        color.push(dataOption.color[x].color0);
        let data = {
          name: charts[i].name,
          color: dataOption.color[x].color0,
          value: top10CityData[i],
          itemStyle: {
            normal: {
              show: true,
              color: new echarts.graphic.LinearGradient(
                0,
                0,
                1,
                0,
                [
                  {
                    offset: 0,
                    color: dataOption.color[x].color0
                  },
                  {
                    offset: 1,
                    color: dataOption.color[x].color1
                  }
                ],
                false
              ),
              barBorderRadius: raduis
            },
            emphasis: {
              shadowBlur: 15,
              shadowColor: "rgba(0, 0, 0, 0.1)"
            }
          }
        };
        lineY.push(data);
      }

      //是否显示标题 start-----------------------------------------
      if (dataOption.istitle == false) {
        dataOption.title.show = false;
      } else {
        dataOption.title.show = true;
      }

      dataOption.valueLabel.formatter = "{value}" + dataOption.suf;

      if (dataOption.customPosition) {
        dataOption.label.position = [
          dataOption.leftPositon + "%",
          dataOption.topPositon + "%"
        ];
      }

      let option = {
        backgroundColor: "rgba(0,0,0,0)",
        title: {
          show: dataOption.istitle == false ? false : true,
          text: dataOption.title.text,
          textStyle: {
            color: "#fff", //颜色
            fontStyle: "normal", //风格
            fontWeight: "normal", //粗细
            //fontFamily: 'Microsoft yahei',   //字体
            //fontSize: 20,     //大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontSize: 14,
            align: "center"
          },
          itemGap: 7
        },
        tooltip: {
          trigger: "item"
        },
        grid: {
          left: dataOption.label.position == "left" ? "20%" : "3%"
        },
        color: color,
        yAxis: [
          {
            type: "category",
            name: dataOption.yAxisName,
            nameTextStyle: {
              color: dataOption.label.textStyle.color,
              fontSize: dataOption.label.textStyle.fontSize,
              fontFamily: dataOption.label.textStyle.fontFamily
            },
            inverse: false,
            axisTick: {
              show: false
            },
            axisLine: {
              show:
                dataOption.yLineShow == undefined
                  ? false
                  : dataOption.yLineShow,
              lineStyle: {
                color:
                  typeof dataOption.yLineColor == "undefined"
                    ? "#fff"
                    : dataOption.yLineColor == null
                    ? "transparent"
                    : dataOption.yLineColor,
                width:
                  dataOption.yLineWidth == undefined ? 1 : dataOption.yLineWidth
              }
            },
            axisLabel: {
              show: false,
              inside: false
            },
            data: top10CityList
          },
          {
            type: "category",
            axisLine: {
              show: false
            },
            axisTick: {
              show: false
            },
            axisLabel: dataOption.valueLabel,
            splitArea: {
              show: false
            },
            splitLine: {
              show: false
            },
            data: top10CityData, //右侧数据
            show:
              dataOption.valueLabelShow == undefined
                ? true
                : dataOption.valueLabelShow
          }
        ],
        xAxis: {
          type: "value",
          name: dataOption.xAxisName,
          nameTextStyle: {
            color:
              typeof dataOption.xFontColor == "undefined"
                ? "#fff"
                : dataOption.xFontColor == null
                ? "transparent"
                : dataOption.xFontColor,
            fontSize:
              typeof dataOption.xFontSize == "undefined"
                ? 0
                : dataOption.xFontSize
          },
          axisTick: {
            show: false
          },
          axisLine: {
            show:
              dataOption.xLineShow == undefined ? false : dataOption.xLineShow,
            lineStyle: {
              color:
                typeof dataOption.xLineColor == "undefined"
                  ? "#fff"
                  : dataOption.xLineColor == null
                  ? "transparent"
                  : dataOption.xLineColor,
              width:
                dataOption.xLineWidth == undefined ? 1 : dataOption.xLineWidth
            }
          },
          splitLine: {
            show:
              dataOption.splitLineShow == undefined
                ? false
                : dataOption.splitLineShow,
            lineStyle: {
              color:
                dataOption.splitLineColor == undefined
                  ? "#ccc"
                  : dataOption.splitLineColor,
              width:
                dataOption.splitLineWidth == undefined
                  ? 1
                  : dataOption.splitLineWidth,
              type:
                dataOption.splitLineStyle == undefined
                  ? "solid"
                  : dataOption.splitLineStyle
            }
          },
          axisLabel: {
            textStyle: {
              show: true,
              color:
                typeof dataOption.xFontColor == "undefined"
                  ? "transparent"
                  : dataOption.xFontColor == null
                  ? "transparent"
                  : dataOption.xFontColor,
              fontSize:
                typeof dataOption.xFontSize == "undefined"
                  ? 0
                  : dataOption.xFontSize
            }
          }
        },
        series: [
          {
            name: "",
            type: "bar",
            zlevel: 2,
            barWidth: dataOption.width,
            data: lineY,
            animationDuration: 1500,
            label: dataOption.label
          }
        ],
        animationEasing: "cubicOut"
      };
      dataOption.label.formatter = function(a, b) {
        return a.name;
      };

      this.chart.setOption(option, true);

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
