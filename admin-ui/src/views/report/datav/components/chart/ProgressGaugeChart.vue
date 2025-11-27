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
import dataChart from '../mixins/dataChart.js'
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

      let spareData = (100 - parseInt(result)) / 100;
      if(parseInt(result) == 100){
        spareData = 0.00001;
      }

      let option = {
        //backgroundColor: '#02203d',
        title: {
          // text: '{a|' + value + '}{c|%}',
          text: dataOption.title.text,
          x: dataOption.title.left + "%",
          y: dataOption.title.top + "%",
          textStyle: {
            fontSize: dataOption.title.fontSize,
            color: dataOption.title.color,
            fontFamily: dataOption.title.fontFamily,
            fontWeight: dataOption.title.fontWeight
          }
        },
        series: [
          {
            name: "圆盘",
            type: "gauge",
            radius: "70%",
            clockwise: false,
            startAngle: "90",
            endAngle: "-269.9999",
            splitNumber: dataOption.splitNumber,
            detail: {
              show: dataOption.isValue,
              offsetCenter: [
                dataOption.offsetCenter[0] + "%",
                dataOption.offsetCenter[1] + "%"
              ],
              // formatter: `{fline|${result}%}`,
              formatter: function (value) {
                return '{value|' + value.toFixed(0) + '}{unit|%}';
              },
              color: "#7BCEF6",
              rich: {
                value: {
                  fontSize: dataOption.label.fontSize,
                  color: dataOption.label.color,
                  fontFamily: dataOption.label.fontFamily,
                  fontWeight: dataOption.label.fontWeight
                }
              }
            },
            pointer: {
              show: false
            },
            axisLine: {
              show: true,
              lineStyle: {
                color: [
                  [
                    spareData,
                    new echarts.graphic.LinearGradient(0, 0, 1, 0, [
                      {
                        offset: 0,
                        color: dataOption.processColor[0]
                      },
                      {
                        offset: 1,
                        color: dataOption.processColor[1]
                      }
                    ])
                  ],
                  [1, dataOption.surplusColor]
                ],
                width: dataOption.progressWidth,
                shadowColor: "rgba(33, 174, 234, 1)",
                shadowBlur: 0
              }
            },
            axisTick: {
              show: false
            },
            splitLine: {
              show: true,
              length: dataOption.progressWidth,
              lineStyle: {
                color: dataOption.splitLineColor,
                width: dataOption.splitLineWidth
              }
            },
            axisLabel: {
              show: false
            },
            data: [
              {
                value: result[0].value
              }
            ]
          },

          {
            name: "外环",
            type: "pie",
            radius: [
              dataOption.outRadius[0] + "%",
              dataOption.outRadius[1] + "%"
            ],
            hoverAnimation: false,
            clockWise: false,
            itemStyle: {
              normal: {
                shadowBlur: 0,
                shadowColor: "rgba(0, 118, 239,0.5)",
                color: dataOption.outColor
              }
            },
            label: {
              show: false
            },
            data: [100]
          },
          {
            name: "内环",
            type: "pie",
            radius: [
              dataOption.innerRadius[0] + "%",
              dataOption.innerRadius[1] + "%"
            ],
            hoverAnimation: false,
            clockWise: false,
            itemStyle: {
              normal: {
                shadowBlur: 0,
                shadowColor: "rgba(0, 118, 239,0.5)",
                color: dataOption.innerColor
              }
            },
            label: {
              show: false
            },
            data: [100]
          }
        ]
      };

   
      this.chart.setOption(option, true);

    },
    
  }
};
</script>
