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

// 绘制四边形ABCD
const quadrilateral = echarts.graphic.extendShape({
  shape: {},
  buildPath: (ctx, shape) => {
    ctx
      .moveTo(shape.A[0], shape.A[1])
      .lineTo(shape.B[0], shape.B[1])
      .lineTo(shape.C[0], shape.C[1])
      .lineTo(shape.D[0], shape.D[1])
      .closePath();
  }
});

// 注册
echarts.graphic.registerShape("quadrilateral", quadrilateral);

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
      this.createGraphBySelected(this.chart, dataOption, false, result);

      //图例点击事件
      this.chart.on("legendselectchanged", obj => {
        let dataOption = this.chartOption;
        this.createGraphBySelected(this.chart, dataOption, obj.selected,result);
      });
    },
    //使用series模板,info需要有name,data,color,这三项和tooltip,legend联动
    //info需要有bar的高度，与投影有关的向量OB,OC,三个面的颜色以及A的坐标
    //由于使用echarts的x轴，因此需要调用api.coord来获得x轴离散数据所生成的label的坐标
    //那么A的坐标使用(xOffset,yOffset)，表示A的坐标相对于label的值
    //A的实际坐标为label坐标(api.value(0),0)+A的相对坐标(xOffset,yOffset)
    addThreeDBarSeries(info, barIndex, series) {
      let data = {
        type: "custom",
        data: info[barIndex].data,
        name: info[barIndex].name,
        color: info[barIndex].color,
        renderItem: function(params, api) {
          const location = api.coord([api.value(0), 0]);
          return {
            type: "group",
            children: [
              {
                //left
                type: "quadrilateral",
                shape: {
                  api,
                  A: [
                    location[0] + info[barIndex].xOffset,
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex]
                  ],
                  B: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OC[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex] +
                      info[barIndex].OC[1]
                  ],
                  C: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OC[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] +
                      info[barIndex].OC[1]
                  ],
                  D: [
                    location[0] + info[barIndex].xOffset,
                    location[1] + info[barIndex].yOffset[params.dataIndex]
                  ]
                },
                style: {
                  fill: info[barIndex].faceColors[0]
                }
              },
              {
                //right
                type: "quadrilateral",
                shape: {
                  api,
                  A: [
                    location[0] + info[barIndex].xOffset,
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex]
                  ],
                  B: [
                    location[0] + info[barIndex].xOffset,
                    location[1] + info[barIndex].yOffset[params.dataIndex]
                  ],
                  C: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OB[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] +
                      info[barIndex].OB[1]
                  ],
                  D: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OB[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex] +
                      info[barIndex].OB[1]
                  ]
                },
                style: {
                  fill: info[barIndex].faceColors[1]
                }
              },
              {
                //top
                type: "quadrilateral",
                shape: {
                  api,
                  A: [
                    location[0] + info[barIndex].xOffset,
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex]
                  ],
                  B: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OB[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex] +
                      info[barIndex].OB[1]
                  ],
                  C: [
                    location[0] +
                      info[barIndex].xOffset +
                      info[barIndex].OB[0] +
                      info[barIndex].OC[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex] +
                      info[barIndex].OB[1] +
                      info[barIndex].OC[1]
                  ],
                  D: [
                    location[0] + info[barIndex].xOffset + info[barIndex].OC[0],
                    location[1] +
                      info[barIndex].yOffset[params.dataIndex] -
                      info[barIndex].height[params.dataIndex] +
                      info[barIndex].OC[1]
                  ]
                },
                style: {
                  fill: info[barIndex].faceColors[2]
                }
              }
            ]
          };
        }
      };
      series.push(data);
      console.log(data,'==============data')
    },
    //投影效果计算
    changeProject(angle, size) {
      let result = [];
      let alpha = (angle[0] * Math.PI) / 180;
      let beta = (angle[1] * Math.PI) / 180;
      let OB = [
        size[0] * Math.sin(alpha),
        -size[0] * Math.cos(alpha) * Math.sin(beta)
      ];
      let OC = [
        -size[1] * Math.cos(alpha),
        -size[1] * Math.sin(alpha) * Math.sin(beta)
      ];
      let heightRatio = Math.cos(beta);
      result.push(OB);
      result.push(OC);
      result.push(heightRatio);
      return result;
    },
    //rgba(a,b,c,d) -> a,b,c,d  #123456 -> a,b,c,d,返回的值为int类型
    getRGBA(str) {
      let datas = [];
      if (str[0] == "r") {
        if (str[3] == "a") {
          let strs = str.split(",");
          datas[0] = parseInt(strs[0].substring(5, strs[0].length));
          datas[1] = parseInt(strs[1]);
          datas[2] = parseInt(strs[2]);
          datas[3] = parseInt(strs[3].substring(0, strs[3].length - 1));
          return datas;
        } else {
          let strs = str.split(",");
          datas[0] = parseInt(strs[0].substring(4, strs[0].length));
          datas[1] = parseInt(strs[1]);
          datas[2] = parseInt(strs[2].substring(0, strs[2].length - 1));
          datas[3] = 1;
          return datas;
        }
      } else if (str[0] == "#") {
        datas[0] = parseInt(str.substring(1, 3), 16);
        datas[1] = parseInt(str.substring(3, 5), 16);
        datas[2] = parseInt(str.substring(5, 7), 16);
        datas[3] = 1;
        return datas;
      } else {
        //errorFlag
        return "error";
      }
    },
    // a,b,c,d -> rgba(a,b,c,d),返回的值为str类型
    setRGBA(datas) {
      return (
        "rgba(" +
        datas[0] +
        "," +
        datas[1] +
        "," +
        datas[2] +
        "," +
        datas[3] +
        ")"
      );
    },
    //自动生成颜色
    createBarColor(datas, angle) {
      let colors = [];
      let alpha = (angle[0] * Math.PI) / 180;
      let beta = (angle[1] * Math.PI) / 180;

      let leftdatas = [];
      let leftParam = Math.cos(alpha) * Math.cos(beta) * 0.8 + 0.2;
      leftdatas[0] = Math.round(datas[0] * leftParam);
      leftdatas[1] = Math.round(datas[1] * leftParam);
      leftdatas[2] = Math.round(datas[2] * leftParam);
      leftdatas[3] = datas[3];
      colors.push(this.setRGBA(leftdatas));

      let rightdatas = [];
      let rightParam = Math.sin(alpha) * Math.cos(beta) * 0.8 + 0.2;
      rightdatas[0] = Math.round(datas[0] * rightParam);
      rightdatas[1] = Math.round(datas[1] * rightParam);
      rightdatas[2] = Math.round(datas[2] * rightParam);
      rightdatas[3] = datas[3];
      colors.push(this.setRGBA(rightdatas));

      let topdatas = [];
      let topParam = Math.sin(beta) * 0.8 + 0.2;
      topdatas[0] = Math.round(datas[0] * topParam);
      topdatas[1] = Math.round(datas[1] * topParam);
      topdatas[2] = Math.round(datas[2] * topParam);
      topdatas[3] = datas[3];
      colors.push(this.setRGBA(topdatas));

      return colors;
    },
    createGraphBySelected(myChart, dataOption, selected, result) {
      //获取颜色
      let color = this.chartOption.theme.color;
      //获取数据
      
      let value = {...result};
      
      dataOption.xAxis.data = value.axisData;

      //根据stack处理数据
      //stack = { stack1: [{ name: bar1, data: [1, 2] },{ name: bar2, data: [3, 4] }] }
      let stack = {};
      //stackTemp临时存储当前bar对应的stack
      let stackTemp = "";
      for (let index in value.series) {
        //如果bar没有stack，那么自动将name作为bar的stack
        if (value.series[index].stack === "" || value.series[index].stack === undefined) {
          stackTemp = value.series[index].name;
        } else {
          stackTemp = value.series[index].stack;
        }
        // stackTemp未创建  初始化
        if (stack[stackTemp] == null) {
          stack[stackTemp] = [];
        }
        
        stack[stackTemp].push({
          name: value.series[index].name,
          data: value.series[index].data
        });
      }
     
      //填充颜色
      //stack = { stack1: [{ name: bar1, data: [1, 2], color: #fff },{ name: bar2, data: [3, 4], color: #fff }] }
      console.log(stack, '=======stack')
      let colorIndex = 0;
      for (let i in stack) {
        for (let j in stack[i]) {
          if (colorIndex == color.length) {
            colorIndex -= color.length;
          }
          stack[i][j].color = color[colorIndex];
          colorIndex++;
        }
      }
      console.log(value, '=======value')
      //获取堆叠图中不同柱的数据总和的最大值，以此确定所有柱的高度
      let maxValue = 0;
      for (let i in value.axisData) {
        for (let j in stack) {
          let temp = 0;
          for (let k in stack[j]) {
            if (!selected || selected[stack[j][k].name]) {
              temp += stack[j][k].data[i];
            }
          }
          if (maxValue < temp) {
            maxValue = temp;
          }
        }
      }

      //获取stackLength
      let stackLength = 0;
      for (let i in stack) {
        for (let j in stack[i]) {
          if (!selected || selected[stack[i][j].name]) {
            stackLength++;
            break;
          }
        }
      }

      let projectResult = this.changeProject(
        dataOption.projectAngle,
        dataOption.bottomSize
      );

      //创建info
      let info = [];
      //生成series
      let series = [];
      //stack的编号
      let stackIndex = 0;
      //bar的编号
      let barIndex = 0;
      for (let i in stack) {
        //初始化
        let heightTemp = [];
        for (let k in value.axisData) {
          heightTemp.push(0);
        }

        let flag = false; //判断这个stack是否有selected bar, true -> has
        for (let j in stack[i]) {
          if (!selected || selected[stack[i][j].name]) {
            flag = true;
            info.push({
              yOffset: [],
              height: []
            });
            info[barIndex].OB = projectResult[0];
            info[barIndex].OC = projectResult[1];
            info[barIndex].name = stack[i][j].name;
            info[barIndex].data = stack[i][j].data;
            info[barIndex].color = stack[i][j].color;
            info[barIndex].faceColors = this.createBarColor(
              this.getRGBA(info[barIndex].color),
              dataOption.lightAngle
            );
            info[barIndex].xOffset =
              dataOption.barSpacing * (stackIndex - (stackLength - 1) / 2);
            for (let k in value.axisData) {
              let height =
                ((dataOption.maxHeight * stack[i][j].data[k]) / maxValue) *
                projectResult[2];
              info[barIndex].height.push(height);
              info[barIndex].yOffset.push(-heightTemp[k]);
              heightTemp[k] += height;
            }
            this.addThreeDBarSeries(info, barIndex, series);
          } else {
            //生成未被选中的图
            info.push({});
            series.push({
              type: "custom",
              name: stack[i][j].name,
              color: stack[i][j].color
            });
          }
          barIndex++;
        }
        if (flag) {
          stackIndex++;
        }
      }
      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        grid: dataOption.grid,
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,
        series: series
      };
      console.log(option, '========option')
      if (!dataOption.isShowLegend) {
        option.legend = { show: false };
      } else if (selected) {
        option.legend.selected = selected;
      }

      this.chart.setOption(option, true);

      //将setting存入map集合
      var setting = {
        bindingType: "threeDBar",
        option: option,
        stack: stack,
        info: info,
        param1: dataOption.barSpacing,
        param2: dataOption.maxHeight * projectResult[2]
      };

      addOption(dataOption.bindingDiv, setting);

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
