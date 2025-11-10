<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  ></div>
</template>

<script>
import echarts from "echarts";
import "echarts-gl";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";
import dataChart from '../mixins/dataChart.js'
import VueEvent from "../../VueEvent";
//加载json文件
import "echarts/map/js/world";
import "echarts/map/js/china";
import "echarts/map/js/china-contour";
import "echarts/map/js/province/anhui";
import "echarts/map/js/province/aomen";
import "echarts/map/js/province/hebei";
import "echarts/map/js/province/heilongjiang";
import "echarts/map/js/province/henan";
import "echarts/map/js/province/hubei";
import "echarts/map/js/province/hunan";
import "echarts/map/js/province/jiangsu";
import "echarts/map/js/province/jiangxi";
import "echarts/map/js/province/jilin";
import "echarts/map/js/province/liaoning";
import "echarts/map/js/province/neimenggu";
import "echarts/map/js/province/beijing";
import "echarts/map/js/province/ningxia";
import "echarts/map/js/province/qinghai";
import "echarts/map/js/province/shandong";
import "echarts/map/js/province/shanghai";
import "echarts/map/js/province/shanxi";
import "echarts/map/js/province/shanxi1";
import "echarts/map/js/province/sichuan";
import "echarts/map/js/province/taiwan";
import "echarts/map/js/province/tianjin";
import "echarts/map/js/province/xianggang";
import "echarts/map/js/province/chongqing";
import "echarts/map/js/province/xinjiang";
import "echarts/map/js/province/xizang";
import "echarts/map/js/province/yunnan";
import "echarts/map/js/province/zhejiang";
import "echarts/map/js/province/fujian";
import "echarts/map/js/province/gansu";
import "echarts/map/js/province/guangdong";
import "echarts/map/js/province/guangxi";
import "echarts/map/js/province/guizhou";
import "echarts/map/js/province/hainan";

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
    area() {
      // 先销毁再重新创建
      this.chart.dispose();
      this.initSourceCode(this.dataOption);
    },
  
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  //created 在模板渲染成html前调用，即通常初始化某些属性值，然后再渲染成视图
  created() {
    //循环注册地图
    for (const index in this.areas) {
      echarts.registerMap(index, this.areas[index]);
    }
  },
  //mounted 在模板渲染成html后调用，通常是初始化页面完成后，再对html的dom节点进行一些需要的操作。
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    //实例销毁之前调用
    if (!this.chart) {
      return;
    }
    //先清空，再重新初始化
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

      // this.area = dataOption.mapName;
      // let geoJson = this.areas[dataOption.mapName];

      // const data = geoJson.features.map((item, index) => {
      //   const geoAreaName = item.properties.name; // geo文件中的地理名称
      //   return {
      //     name: geoAreaName,

      //     coord: item.properties.centroid,
      //     selected: true
      //     // x: 150,
      //     // y: 150
      //   };
      // });

      let mapName = dataOption.mapName;
      let geoCoordMap = {};
      let mapFeatures = echarts.getMap(mapName).geoJson.features;
      mapFeatures.forEach(function(v) {
        // 地区名称
        let name = v.properties.name;
        // 地区经纬度
        geoCoordMap[name] = v.properties.cp;
      });
     
      let convertData = function(data) {
        let res = [];
        for (let i = 0; i < data.length; i++) {
          let geoCoord = geoCoordMap[data[i].name];
          if (geoCoord) {
            res.push({
              name: data[i].name,
              value: typeof data[i].value == 'number'?data[i].value :0
            });
          }
        }
        //console.log(res)
        return res;
      };
      
      let suffix = dataOption.suffix == undefined?"":dataOption.suffix;

      //计算静态数据最大值
      let max = 100;
      let sortData = [...result]
      if(typeof sortData[0].value === Number ){
        sortData.sort((a, b) => {
          return b.value - a.value;
        })
        max = sortData[0].value
      }

      // 注册地图名字(tongren)和数据(geoJson)
      var option = {
        bindingType: "threedMap",
        // title: { // 标题
        //   top: '2%',
        //   text: this.area,
        //   subtext: '',
        //   x: 'center',
        //   textStyle: {
        //     color: '#ccc'
        //   }
        // },
        selectedMode: "multiple", // 选中效果固话
        tooltip: {
          // 提示框
          show: true,
          trigger: "item",
          formatter: function(params) {
            return params.name;
          }
        },
        series: [
          {
            name: "map3D",
            type: "map3D", // map3D / map
            map: mapName,
            label: {
              // 标签的相关设置
              show: true, // (地图上的城市名称)是否显示标签 [ default: false ]
              // distance: 5, // 标签距离图形的距离，在三维的散点图中这个距离是屏幕空间的像素值，其它图中这个距离是相对的三维距离
              //formatter:, // 标签内容格式器
              textStyle: dataOption.textStyle,
              // { // 标签的字体样式
              //   color: '#ffffff', // 地图初始化区域字体颜色
              //   fontSize: 14, // 字体大小
              //   opacity: 1, // 字体透明度
              //   backgroundColor: 'rgba(0,23,11,0.5)' // 字体背景色
              // },
              // normal:{
              //   show:true,
              //   formatter:function(params){ //标签内容
              //     // console.log(params)
              //     return  params.name;
              //   },
              //   // lineHeight: 20,
              //   backgroundColor:'rgba(255,255,255,.9)',
              //   borderColor:'#80cffd',
              //   borderWidth:'1',
              //   padding:[5,15,4],
              //   color:'#000000',
              //   fontSize: 12,
              //   fontWeight:'normal',
              // },
              emphasis: {
                show: true,
              }
            },
            tooltip: {
              //提示框组件。
              alwaysShowContent: true,
              hoverAnimation: true,
              trigger: "item", //触发类型 散点图
              enterable: true, //鼠标是否可进入提示框
              transitionDuration: 1, //提示框移动动画过渡时间
              triggerOn: "click",
              //extraCssText: 'white-space: normal; word-break: break-all;',
              formatter: function(params) {
                // console.log(params.name, 'params.name')
                const tooltip = result.filter(function(
                  item
                ) {
                  return item.name == params.name;
                });
                if (tooltip[0]) {
                  var str = "";
                  if(typeof tooltip[0].value == 'number'){
                    str = `
                    <div class="map-tooltip">
                      <div class="city-name">${tooltip[0].name}：`+tooltip[0].value+suffix+`</div>`;
                  }else{
                    str = `
                    <div class="map-tooltip">
                      <div class="city-name">${tooltip[0].name}</div>`;
                    tooltip[0].value.forEach(element => {
                      str += `<div class="city-info">${element}</div>`;
                    });
                    str = str + `</div>`;
                  }

                  return str;
                }
              },
              backgroundColor: dataOption.tooltip.backgroundColor,
              borderWidth: dataOption.tooltip.borderWidth,
              borderRadius: dataOption.tooltip.borderRadius,
              borderColor: dataOption.tooltip.borderColor,
              textStyle: {
                color: dataOption.tooltip.textStyle.color
              },
              padding: [5, 10]
            },
            itemStyle: dataOption.itemStyle,
            // { // 三维地理坐标系组件 中三维图形的视觉属性，包括颜色，透明度，描边等。
            //   // areaColor: 'rgba(95,158,160,0.5)', // 地图板块的颜色
            //   areaColor: '#10786c', // 地图板块的颜色
            //   opacity: 0.3, // 图形的不透明度 [ default: 1 ]
            //   borderWidth: 2, // (地图板块间的分隔线)图形描边的宽度。加上描边后可以更清晰的区分每个区域 [ default: 0 ]
            //   borderColor: '#5CFFE0' // 图形描边的颜色。[ default: #333 ]
            // },
            viewControl: {
              rotateSensitivity: dataOption.isRotate ? 1 : 0, //旋转灵敏度，0不能旋转
              autoRotate: false,
              animation: false,
              alpha: dataOption.alpha == undefined ? 60 : dataOption.alpha, //上下旋转角度
              beta: dataOption.beta == undefined ? 0 : dataOption.beta, //左右旋转角度
              animationDurationUpdate: 10
            },
            data: convertData(result),
            // 3d地图添加 markPoint 位置不对
            /*markPoint:{
            symbolSize: 45,
            symbol: 'path://m 0,0 h 48 v 20 h -30 l -6,10 l -6,-10 h -6 z',
            itemStyle: {
              normal: {
                borderColor: '#33CBFF',
                color:'#33CBFF',
                borderWidth: 1,            // 标注边线线宽，单位px，默认为1
                label: {
                  show: true
                }
              }
            },
            data: data
          }*/
          }
        ],
        // staticDataValue: dataOption.staticDataValue
      };


      if(dataOption.visualShow == true){
        option.visualMap = {
            show: true,
            left:dataOption.visualX,
            bottom:dataOption.visualY,
            itemWidth:dataOption.itemWidth,
            itemHeight:dataOption.itemHeight,
            min: 0,
            max: max,
            calculable: true,
            realtime: false,
            inRange: {
                color: dataOption.colors != undefined ?dataOption.colors :['#2884db', '#244779']
            },
            textStyle:{
                color:'#fff',
                fontSize:dataOption.visualFontSize
            }
        }
      }

      this.chart.setOption(option, true);
      addOption(dataOption.bindingDiv, option);
      //单击切换到省级地图，当mapCode有值,说明可以切换到下级地图
      // let timeFn = null;
      // this.chart.on("click", (params)=> {
      //   clearTimeout(timeFn);
      //   timeFn = setTimeout(() => {
      //     let mapCode = this.areas[params.name]
      //      this.area = params.name; //地区name
      //      option.series[0].map = params.name;
      //      option.series[0].data = this.getGeoJson(params.name);
      //this.chart.setOption(option, true);

      //   },250);

      // });
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
    getGeoJson(mapName) {
      let geoJson = this.areas[mapName];

      const data = geoJson.features.map((item, index) => {
        const geoAreaName = item.properties.name; // geo文件中的地理名称

        return {
          name: geoAreaName,

          coord: item.properties.centroid,
          selected: true
          // x: 150,
          // y: 150
        };
      });

      return data;
    },
  
  }
};
</script>
