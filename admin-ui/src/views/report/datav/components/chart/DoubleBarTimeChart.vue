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
import VueEvent from "../../VueEvent";
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
      let data = result;
      let myData = new Set();
      let data1 = {};
      let data2 = {};
      let timeLineData = new Set();
      for (let i = 0; i < data.length; i++) {
        myData.add(data[i].label)
        timeLineData.add(data[i].name);
      }
      myData = [...myData]
      timeLineData = [...timeLineData]
      for (let d in timeLineData) {
        data1[timeLineData[d]] = []
        data2[timeLineData[d]] = []
        for (let v in data) {
          console.log(data[v])
          if (data[v].name === timeLineData[d]) {
            console.log(data[v].value1)
            data1[timeLineData[d]].push(data[v].value1);
              data2[timeLineData[d]].push(data[v].value2);
          }
        }
      }
      //应用主题
      if (dataOption.isThemeColor == true) {
        var color = this.chartOption.theme.color;

        dataOption.leftColor = color[0 % color.length];
        dataOption.rightColor = color[2 % color.length];
        dataOption.leftfocusColor = color[1 % color.length];
        dataOption.rightfocusColor = color[3 % color.length];
      }

      var option = {
        baseOption: {
          timeline: {
            //时间轴
            show: true,
            axisType: "category",
            tooltip: {
              show: true,
              formatter: function(params) {
                return dataOption.timetooltip.replace("?", params.name); //时间轴提示文字
              }
            },
            autoPlay: dataOption.isautoplay == false ? false : true, //自动播放
            currentIndex: 0, //默认数据
            playInterval: dataOption.playinterval, //播放速度
            label: {
              normal: {
                show: true,
                interval: "auto", //下标的显示间隔
                formatter: "{value}" + dataOption.suffix //下标值
              }
            },
            data: []
          },
          title: {
            show: dataOption.istitle == false ? false : true,
            //   text:'大北京景点帅哥美女统计',
            textStyle: {
              color: "#fff",
              fontSize: 16
            },
            subtext: dataOption.subtitle
          },
          legend: {
            //图例
            data: timeLineData,
            top: 4,
            right: "20%",
            textStyle: {
              color: "#fff"
            }
          },
          tooltip: {
            //提示
            show: true,
            trigger: "axis",
            formatter: "{b}<br/>{a}: {c}" + dataOption.datasuffix,
            axisPointer: {
              type: "shadow"
            }
          },

          grid: [
            {
              //左柱
              show: false,
              left: "4%",
              top: 60,
              bottom: 60,
              containLabel: true,
              width: "37%"
            },
            {
              //中间坐标
              show: false,
              left: "50.5%",
              top: 80,
              bottom: 60,
              width: "0%"
            },
            {
              //右柱
              show: false,
              right: "4%",
              top: 60,
              bottom: 60,
              containLabel: true,
              width: "37%"
            }
          ],

          xAxis: [
            {
              type: "value",
              inverse: true,
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              position: "top",
              axisLabel: {
                show: true,
                textStyle: {
                  color: "#fff",
                  fontSize: 12
                }
              },
              splitLine: {
                show: true,
                lineStyle: {
                  color: "#1F2022",
                  width: 1,
                  type: "solid"
                }
              }
            },
            {
              gridIndex: 1,
              show: false
            },
            {
              gridIndex: 2,
              type: "value",
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              position: "top",
              axisLabel: {
                show: true,
                textStyle: {
                  color: "#fff",
                  fontSize: 12
                }
              },
              splitLine: {
                show: true,
                lineStyle: {
                  color: "#1F2022",
                  width: 1,
                  type: "solid"
                }
              }
            }
          ],
          yAxis: [
            {
              type: "category",
              inverse: true,
              position: "right",
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              axisLabel: {
                show: false,
                margin: 8,
                textStyle: {
                  color: "#fff",
                  fontSize: 12
                }
              },
              data: myData
            },
            {
              gridIndex: 1,
              type: "category",
              inverse: true,
              position: "left",
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              axisLabel: {
                show: true,
                textStyle: {
                  color: "#fff",
                  fontSize: 12
                }
              },
              data: myData.map(function(value) {
                return {
                  value: value,
                  textStyle: {
                    align: "center"
                  }
                };
              })
            },
            {
              gridIndex: 2,
              type: "category",
              inverse: true,
              position: "left",
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              axisLabel: {
                show: false,
                textStyle: {
                  color: "#fff",
                  fontSize: 12
                }
              },
              data: myData
            }
          ],
          series: []
        },

        options: []
      };
      for (var i = 0; i < timeLineData.length; i++) {
        option.baseOption.timeline.data.push(timeLineData[i]);
        option.options.push({
          title: {
            text: dataOption.title.replace("?", timeLineData[i])
          },
          series: [
            {
              name: timeLineData[i],
              type: "bar",
              barWidth: dataOption.barwidth,
              label: {
                normal: {
                  show: false
                },
                emphasis: {
                  show: true,
                  position: "left",
                  offset: [0, 0],
                  textStyle: {
                    color: "#fff",
                    fontSize: 14
                  }
                }
              },
              itemStyle: {
                normal: {
                  color: dataOption.leftColor
                },
                emphasis: {
                  color: dataOption.leftfocusColor
                }
              },
              data: data1[timeLineData[i]]
            },
            {
              name: timeLineData[i],
              type: "bar",
              barGap: 20,
              barWidth: dataOption.barwidth,
              xAxisIndex: 2,
              yAxisIndex: 2,
              label: {
                normal: {
                  show: false
                },
                emphasis: {
                  show: true,
                  position: "right",
                  offset: [0, 0],
                  textStyle: {
                    color: "#fff",
                    fontSize: 14
                  }
                }
              },
              itemStyle: {
                normal: {
                  color: dataOption.rightColor
                },
                emphasis: {
                  color: dataOption.rightfocusColor
                }
              },
              data: data2[timeLineData[i]]
            }
          ]
        });
      }
    console.log(option, '========option')
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.seriesName,
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
            legendName: params.seriesName,
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
