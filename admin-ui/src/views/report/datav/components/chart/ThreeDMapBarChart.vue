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
import VueEvent from "../../VueEvent";
import dataChart from '../mixins/dataChart.js'
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
     
      let mapName = dataOption.mapName;
      let geoCoordMap = {};
      /*获取地图数据*/
      //myChart.showLoading();
      let mapFeatures = echarts.getMap(mapName).geoJson.features;
      //myChart.hideLoading();
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
              value: geoCoord.concat(data[i].value).concat(data[i].sort)
            });
          } else if (data[i].lng != undefined && data[i].lat != undefined) {
            res.push({
              name: data[i].name,
              value: [data[i].lng,data[i].lat,data[i].value,data[i].sort]
            });
          }
        }
        return res;
      };

      //计算静态数据最大值
      let sortData = [...result]
      sortData.sort((a, b) => {
        return b.value - a.value;
      })

      let max = sortData[0].value

      sortData.forEach((element,index) => {
        element.sort = index + 1
      });
     
      let option = {
        bindingType: "threedMapBar",
          tooltip: {
              show: true,
              formatter:(params)=>{
                let data = params.name + "："+ params.value[2] + dataOption.suffix;
                return data;
              },
              textStyle:{
                fontSize:20
              }
          },
          visualMap: [{
              show:dataOption.visualMap.show,
              type: 'continuous',
              seriesIndex: 0,
              calculable: true,
              max: max,
              left:dataOption.visualMap.left,
              bottom:dataOption.visualMap.bottom,
              itemWidth:dataOption.visualMap.itemWidth,
              itemHeight:dataOption.visualMap.itemHeight,
              inRange: {
                  color: dataOption.visualMap.color
              },
              textStyle:{
                  color:'#fff',
                  fontSize:dataOption.fontSize
              }
          }],
          geo3D: {
              map: mapName,
              itemStyle: dataOption.itemStyle,
              label: dataOption.label,
              emphasis: {
                itemStyle:dataOption.emphasis.itemStyle
              },
              //shading: 'lambert',
              light: { //光照阴影
                  main: {
                      //场景主光源的设置
                      intensity: 1, //主光源的强度
                      shadow: true, //主光源是否投射阴影
                      shadowQuality: 'medium', //阴影的质量
                      alpha: 30, //主光源绕 x 轴偏离的角度
                      beta: -80, //主光源绕 y 轴偏离的角度

                  },
                  ambient: {
                      //全局的环境光设置。
                      intensity: 0.5, //环境光的强度
                      quality: 'high',
                  },
                  ambientCubemap: {
                      //会使用纹理作为环境光的光源，会为物体提供漫反射和高光反射
                      exposure: 1.0,
                      diffuseIntensity: 8, //漫反射的强度。
                      specularIntensity: 4, //高光反射的强度。
                  },
              },
            viewControl: {
                    rotateSensitivity: dataOption.viewControl.isRotate ? 1 : 0, //旋转灵敏度，0不能旋转
                    alpha:  dataOption.viewControl.alpha, //上下旋转角度
                    beta: dataOption.viewControl.beta, //左右旋转角度
                  },
          },
          series: [{
              name: 'bar3D',
              type: "bar3D",
              coordinateSystem: 'geo3D',
              shading: 'lambert', //color,lambert,realistic
              lambertMaterial: {
                  //detailTexture:
                  textureTiling: 1,
                  textureOffset: 0,

              },
              //zlevel: -10,
              animation: true,
              animationDurationUpdate: 500,
              animationEasingUpdate: 'cubicOut',
              opacity: 1,
              barSize: dataOption.barSize, //柱子粗细
              bevelSmoothness: dataOption.bevelSmoothness, //倒角的光滑度
              bevelSize: dataOption.bevelSize,
              label: {
                  show: true,
                  position:'top',
                  // textStyle: dataOption.textStyle,
                  distance:0,
                  align:'center',
                  textAlign:'center',
                  formatter: function(params){
                      let str = ""
                      if(dataOption.sortShow==true){
                        str+= "{a|" + params.data.value[3] +"}" +"   ";
                      }
                      if(dataOption.labelShow==true){
                        str+="{b|"+params.name+"："+params.data.value[2]+ dataOption.suffix+"}";
                      }
                      return str//params.name+"："+params.data.value[2]+ dataOption.suffix +params.data.value[3]
                  },
                  textStyle:{
                    backgroundColor:"rgba(1, 1, 1, 0)",
                    rich: {
                        a: {
                            backgroundColor:"rgba(1, 1, 1, 0)",
                            color: dataOption.textStyle.sortColor,
                            fontSize: dataOption.textStyle.sortFontSize,
                            fontWeight:600,
                            borderColor: dataOption.textStyle.borderColor,
                            borderRadius: 50,
                            borderWidth:4,
                            borderType :'solid',
                            padding: [8,15]
                        },
                        b: {
                          backgroundColor: dataOption.textStyle.backgroundColor,
                          color: dataOption.textStyle.color,
                          fontSize: dataOption.textStyle.fontSize,
                          padding: [8,15],
                          borderRadius: 10,
                        },
                    }
                  }
                
              },
              emphasis: {

                    label: {
                        
                        show: dataOption.label.show,
                        // textStyle: dataOption.emphasis.textStyle
                       textStyle:{
                          backgroundColor:"rgba(1, 1, 1, 0)",
                          rich: {
                            a: {
                                color: dataOption.emphasis.textStyle.sortColor,
                                fontSize: dataOption.emphasis.textStyle.sortFontSize,
                                borderColor: dataOption.emphasis.textStyle.borderColor,
                            },
                            b: {
                              backgroundColor: dataOption.emphasis.textStyle.backgroundColor,
                              color: dataOption.emphasis.textStyle.color,
                              fontSize: dataOption.emphasis.textStyle.fontSize,
                            },
                        }
                       }
                    }
                    
                },
              data: convertData(sortData)
          }],
          suffix:dataOption.suffix,
          textStyle: dataOption.textStyle,
          labelShow:dataOption.labelShow
      }

      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          
          //设置参数
          let arrObject = {
            seriesName: params.name,
            data: params.value[2]
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
            data: params.value[2]
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

<style scoped>
.circle {
  background: #456BD9;
  border: 0.1875em solid #0F1C3F;
  border-radius: 50%;
  box-shadow: 0.375em 0.375em 0 0 rgba(15, 28, 63, 0.125);
  height: 5em;
  width: 5em;
}
</style>