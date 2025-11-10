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
import "echarts/map/js/china";
import "../../animate/animate.css";
import cityMap from "../../json/chinacity.json";

import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import VueEvent from "../../VueEvent";
import dataChart from '../mixins/dataChart.js'
let convertedData;
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
  computed: {},
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
    setChartVal(result)  {
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));
      let data = [...result];
      convertedData = [
        this.convertDataMapBar(data),
        this.convertDataMapBar(
          data.sort((a, b) => {
              return b.value - a.value;
            }).slice(0, dataOption.topNum)
        )
      ];

      let seriesData = [];
      if (dataOption.isScatter == true) {
        let scatter = {
          name: dataOption.mapTooltip,
          type: "scatter",
          coordinateSystem: "geo",
          data: convertedData[0],
          symbolSize: function(val) {
            return Math.max(val[2] / 50, 8);
          },
          label: {
            normal: {
              formatter: "{b}",
              position: "right",
              show: false
            },
            emphasis: {
              show: true
            }
          },
          itemStyle: {
            normal: {
              color: dataOption.color
            }
          },
          tooltip: {
            formatter: (params, ticket, callback) => {
              let showHtm =
                params.name +
                params.seriesName +
                ":" +
                params.value[2] +
                "<br />";
              return showHtm;
            }
          }
        };
        if (dataOption.topNum <= 0) {
          scatter.label.normal.show = true;
        }
        seriesData.push(scatter);
      }
      if (dataOption.topNum > 0) {
        let effectScatter = {
          name: "Top " + dataOption.topNum,
          type: "effectScatter",
          coordinateSystem: "geo",
          data: convertedData[1],
          symbolSize: function(val) {
            return Math.max(val[2] / 50, 8);
          },
          showEffectOn: "emphasis",
          rippleEffect: {
            brushType: "stroke"
          },
          hoverAnimation: true,
          label: {
            normal: {
              formatter: "{b}",
              position: "right",
              show: true
            }
          },
          itemStyle: {
            normal: {
              color: dataOption.color,
              shadowBlur: 10,
              shadowColor: "#333"
            }
          },
          tooltip: {
            formatter: (params, ticket, callback) => {
              let showHtm =
                params.seriesName +
                "<br />" +
                params.name +
                dataOption.mapTooltip +
                ":" +
                params.value[2];
              return showHtm;
            }
          },
          zlevel: 1
        };
        seriesData.push(effectScatter);
      }
      let bar = {
        id: "bar",
        zlevel: 2,
        type: "bar",
        symbol: "none",
        itemStyle: {
          normal: {
            color: dataOption.color
          }
        },
        data: []
      };
      seriesData.push(bar);

      let option = {
        //backgroundColor: '#101129',
        animation: true,
        animationDuration: 1000,
        animationEasing: "cubicInOut",
        animationDurationUpdate: 1000,
        animationEasingUpdate: "cubicInOut",
        title: [
          dataOption.title,
          {
            id: "statistic",
            right: 120,
            top: 40,
            width: 100,
            textStyle: {
              color: "#fff",
              fontSize: 16
            }
          }
        ],
        toolbox: {
          //工具栏
          iconStyle: {
            normal: {
              borderColor: "#fff"
            },
            emphasis: {
              borderColor: "#b1e4ff"
            }
          },
          show: dataOption.isToolBox
        },
        brush: {
          //画刷
          outOfBrush: {
            color: "#abc"
          },
          brushStyle: {
            borderWidth: 2,
            color: "rgba(0,0,0,0.2)",
            borderColor: "rgba(0,0,0,0.5)"
          },
          seriesIndex: [0, 1],
          throttleType: "debounce",
          throttleDelay: 300,
          geoIndex: 0
        },

        geo: {
          //地理坐标系组件。
          map: "china",
          top: "5%",
          left: "10",
          right: "10%",
          center: [120.384366, 30.439702],
          zoom: 0.7,
          label: {
            emphasis: {
              show: false
            }
          },
          roam: true,
          itemStyle: {
            normal: {
              areaColor: "#323c48",
              borderColor: "#111"
            },
            emphasis: {
              areaColor: "#2a333d"
            }
          }
        },
        tooltip: {
          //提示
          trigger: "item"
          //formatter: '{b0}: {c0}<br />{b1}: {c1}'
        },
        grid: {
          //直角坐标系内绘图网格，单个 grid 内最多可以放置上下两个 X 轴，左右两个 Y 轴。
          right: 0,
          top: 100,
          bottom: 40,
          width: "30%"
        },
        xAxis: {
          type: "value",
          scale: true,
          position: "top",
          boundaryGap: false,
          splitLine: {
            show: false
          },
          axisLine: {
            show: false
          },
          axisTick: {
            show: false
          },
          axisLabel: {
            margin: 2,
            textStyle: {
              color: "#aaa"
            }
          }
        },
        yAxis: {
          type: "category",
          name: "TOP " + dataOption.barNum,
          nameGap: 16,
          axisLine: {
            show: false,
            lineStyle: {
              color: "#ddd"
            }
          },
          axisTick: {
            show: false,
            lineStyle: {
              color: "#ddd"
            }
          },
          axisLabel: {
            interval: 0,
            textStyle: {
              color: "#ddd"
            }
          },
          data: []
        },

        series: seriesData,
        bindingType: "mapbar"
      };
     
      this.chart.setOption(option, true);
      this.chart.on("brushselected", this.renderBrushed);

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


      setTimeout(() => {
        this.chart.dispatchAction({
          type: "brush",
          areas: [
            {
              geoIndex: 0,
              brushType: "polygon",
              coordRange: [
                [119.72, 34.85],
                [119.68, 34.85],
                [119.5, 34.84],
                [119.19, 34.77],
                [118.76, 34.63],
                [118.6, 34.6],
                [118.46, 34.6],
                [118.33, 34.57],
                [118.05, 34.56],
                [117.6, 34.56],
                [117.41, 34.56],
                [117.25, 34.56],
                [117.11, 34.56],
                [117.02, 34.56],
                [117, 34.56],
                [116.94, 34.56],
                [116.94, 34.55],
                [116.9, 34.5],
                [116.88, 34.44],
                [116.88, 34.37],
                [116.88, 34.33],
                [116.88, 34.24],
                [116.92, 34.15],
                [116.98, 34.09],
                [117.05, 34.06],
                [117.19, 33.96],
                [117.29, 33.9],
                [117.43, 33.8],
                [117.49, 33.75],
                [117.54, 33.68],
                [117.6, 33.65],
                [117.62, 33.61],
                [117.64, 33.59],
                [117.68, 33.58],
                [117.7, 33.52],
                [117.74, 33.5],
                [117.74, 33.46],
                [117.8, 33.44],
                [117.82, 33.41],
                [117.86, 33.37],
                [117.9, 33.3],
                [117.9, 33.28],
                [117.9, 33.27],
                [118.09, 32.97],
                [118.21, 32.7],
                [118.29, 32.56],
                [118.31, 32.5],
                [118.35, 32.46],
                [118.35, 32.42],
                [118.35, 32.36],
                [118.35, 32.34],
                [118.37, 32.24],
                [118.37, 32.14],
                [118.37, 32.09],
                [118.44, 32.05],
                [118.46, 32.01],
                [118.54, 31.98],
                [118.6, 31.93],
                [118.68, 31.86],
                [118.72, 31.8],
                [118.74, 31.78],
                [118.76, 31.74],
                [118.78, 31.7],
                [118.82, 31.64],
                [118.82, 31.62],
                [118.86, 31.58],
                [118.86, 31.55],
                [118.88, 31.54],
                [118.88, 31.52],
                [118.9, 31.51],
                [118.91, 31.48],
                [118.93, 31.43],
                [118.95, 31.4],
                [118.97, 31.39],
                [118.97, 31.37],
                [118.97, 31.34],
                [118.97, 31.27],
                [118.97, 31.21],
                [118.97, 31.17],
                [118.97, 31.12],
                [118.97, 31.02],
                [118.97, 30.93],
                [118.97, 30.87],
                [118.97, 30.85],
                [118.95, 30.8],
                [118.95, 30.77],
                [118.95, 30.76],
                [118.93, 30.7],
                [118.91, 30.63],
                [118.91, 30.61],
                [118.91, 30.6],
                [118.9, 30.6],
                [118.88, 30.54],
                [118.88, 30.51],
                [118.86, 30.51],
                [118.86, 30.46],
                [118.72, 30.18],
                [118.68, 30.1],
                [118.66, 30.07],
                [118.62, 29.91],
                [118.56, 29.73],
                [118.52, 29.63],
                [118.48, 29.51],
                [118.44, 29.42],
                [118.44, 29.32],
                [118.43, 29.19],
                [118.43, 29.14],
                [118.43, 29.08],
                [118.44, 29.05],
                [118.46, 29.05],
                [118.6, 28.95],
                [118.64, 28.94],
                [119.07, 28.51],
                [119.25, 28.41],
                [119.36, 28.28],
                [119.46, 28.19],
                [119.54, 28.13],
                [119.66, 28.03],
                [119.78, 28],
                [119.87, 27.94],
                [120.03, 27.86],
                [120.17, 27.79],
                [120.23, 27.76],
                [120.3, 27.72],
                [120.42, 27.66],
                [120.52, 27.64],
                [120.58, 27.63],
                [120.64, 27.63],
                [120.77, 27.63],
                [120.89, 27.61],
                [120.97, 27.6],
                [121.07, 27.59],
                [121.15, 27.59],
                [121.28, 27.59],
                [121.38, 27.61],
                [121.56, 27.73],
                [121.73, 27.89],
                [122.03, 28.2],
                [122.3, 28.5],
                [122.46, 28.72],
                [122.5, 28.77],
                [122.54, 28.82],
                [122.56, 28.82],
                [122.58, 28.85],
                [122.6, 28.86],
                [122.61, 28.91],
                [122.71, 29.02],
                [122.73, 29.08],
                [122.93, 29.44],
                [122.99, 29.54],
                [123.03, 29.66],
                [123.05, 29.73],
                [123.16, 29.92],
                [123.24, 30.02],
                [123.28, 30.13],
                [123.32, 30.29],
                [123.36, 30.36],
                [123.36, 30.55],
                [123.36, 30.74],
                [123.36, 31.05],
                [123.36, 31.14],
                [123.36, 31.26],
                [123.38, 31.42],
                [123.46, 31.74],
                [123.48, 31.83],
                [123.48, 31.95],
                [123.46, 32.09],
                [123.34, 32.25],
                [123.22, 32.39],
                [123.12, 32.46],
                [123.07, 32.48],
                [123.05, 32.49],
                [122.97, 32.53],
                [122.91, 32.59],
                [122.83, 32.81],
                [122.77, 32.87],
                [122.71, 32.9],
                [122.56, 32.97],
                [122.38, 33.05],
                [122.3, 33.12],
                [122.26, 33.15],
                [122.22, 33.21],
                [122.22, 33.3],
                [122.22, 33.39],
                [122.18, 33.44],
                [122.07, 33.56],
                [121.99, 33.69],
                [121.89, 33.78],
                [121.69, 34.02],
                [121.66, 34.05],
                [121.64, 34.08]
              ]
            }
          ]
        });
      }, 0);
    },
    convertDataMapBar(data) {
      let res = [];
      for (let i = 0; i < data.length; i++) {
        let geoCoordMap = cityMap;
        let geoCoord = geoCoordMap[data[i].name];
        if (geoCoord) {
          res.push({
            name: data[i].name,
            value: geoCoord.concat(data[i].value)
          });
        }
      }
      return res;
    },
    renderBrushed(params) {
      let mainSeries = params.batch[0].selected[0];
      let selectedItems = [];
      let categoryData = [];
      let barData = [];
      let maxBar = this.chartOption.barNum;
      let sum = 0;
      let count = 0;

      for (let i = 0; i < mainSeries.dataIndex.length; i++) {
        let rawIndex = mainSeries.dataIndex[i];
        let dataItem = convertedData[0][rawIndex];
        let pmValue = dataItem.value[2];

        sum += pmValue;
        count++;

        selectedItems.push(dataItem);
      }

      selectedItems.sort((a, b) => {
        return b.value[2] - a.value[2];
      });

      for (let i = 0; i < Math.min(selectedItems.length, maxBar); i++) {
        categoryData.push(selectedItems[i].name);
        barData.push(selectedItems[i].value[2]);
      }
      this.chart.setOption({
        yAxis: {
          data: categoryData,
          inverse: true
        },
        xAxis: {
          axisLabel: {
            show: !!count
          }
        },
        title: {
          id: "statistic",
          text: count ? "平均: " + (sum / count).toFixed(0) : ""
        },
        series: {
          id: "bar",
          data: barData
        }
      });
    },
   
  }
};
</script>
