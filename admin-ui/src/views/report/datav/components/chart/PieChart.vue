<template>
  <div
    :class="animate"
    :style="{ height: height, width: width,...bgImageStyle }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  ></div>
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
      animate: this.className,
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
  computed:{
    bgImageStyle(){
      //背景图片的样式
      let styleobj={}
      if(this.chartOption.bgImage){
        styleobj.backgroundColor='transparent'
        styleobj.backgroundImage = `url(${this.chartOption.bgImage}) `;
        styleobj.backgroundSize = "100% 100%";
        styleobj.backgroundRepeat = "no-repeat";
      }else{
        if(this.chartOption.containerBgColor){
          styleobj.background=this.chartOption.containerBgColor
        }
      }
      if(this.chartOption.containerRadius){
        styleobj.borderRadius=this.chartOption.containerRadius+'px'
      }
      return styleobj
    }
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
      try {
        if (this.chart == null) {
          echarts.registerTheme("customTheme", this.chartOption.theme);
          this.chart = echarts.init(document.getElementById(this.chartOption.bindingDiv),"customTheme",{renderer: 'canvas'});
        }
        let dataOption = JSON.parse(JSON.stringify(this.dataOption))
        if (dataOption.bgColor === undefined) {
          dataOption.bgColor = ['#fc8251', '#5470c6', '#9A60B4', '#ef6567', '#f9c956'];
        }
        var data = [];
        if (dataOption.isRoseType == true) {
          dataOption.series[0].roseType = "area";
        } else {
          dataOption.series[0].roseType = false;
        }
        if (dataOption.isRing == true) {
          dataOption.series[0].radius = [
            dataOption.innerRadius + '%',
            dataOption.outerRadius + '%'
          ];
        } else {
          dataOption.series[0].radius = "55%";
        }
        if (dataOption.isFan == true) {
          dataOption.series[0].clockwise = false;
          if (dataOption.isRoseType == true) {
            for (let i = 0; i < result.length; i++) {
              data.push(result[i]);
            }
            var all =(result.length * 360) / (dataOption.endAngle - dataOption.startAngle);
            var add = all - result.length;
            for (let i = 0; i < add; i++) {
              var thedata = {
                value: 0,
                name: "",
                itemStyle: { color: "rgba(255, 0, 0, 0)" },
                tooltip: { show: false }
              };
              data.push(thedata);
            }
          } else {
            var sum = 0;
            for (let i = 0; i < result.length; i++) {
              data.push(result[i]);
              sum += result[i].value;
            }
            let add =(sum * 360) / (dataOption.endAngle - dataOption.startAngle) - sum;
            var thedata = {
              value: add,
              name: "",
              itemStyle: { color: "rgba(255,0,0,0)" },
              tooltip: { show: false }
            };
            data.push(thedata);
          }
        } else {
          data = result;
        }
        dataOption.series[0].startAngle = dataOption.startAngle;
        dataOption.series[0].data = data;
        if (dataOption.centerX != undefined && dataOption.centerY != undefined) {
          dataOption.series[0].center[0] = dataOption.centerX + "%";
          dataOption.series[0].center[1] = dataOption.centerY + "%";
        }
        
        
        let formatter = []; //标签显示内容
        if (dataOption.labelFormatter == undefined) {
          dataOption.labelFormatter = ["类别名称"];
        }
        for (let item of dataOption.labelFormatter) {
          if (item == "类别名称") {
            formatter[0] = "{b} ";
          }
          if (item == "数值") {
            formatter[1] = "：{c}" + dataOption.formatterSuffix;
          }
          if (item == "百分比") {
            formatter[2] = "({d}%)";
          }
        }
        let formatterStr = "";
        for (let item of formatter) {
          if (item != undefined) {
            formatterStr += item;
          }
        }
        //标签设置
        // dataOption.series[0].label.formatter = formatterStr;
        // if (result.length > 0 && result.length < 3) {
        //   dataOption.series[0].label.position = 'center';
        // }
        dataOption.series[0].label.fontSize = dataOption.labelFontSize;
        dataOption.series[0].itemStyle = {
          color: function (colors) {
            return dataOption.bgColor[colors.dataIndex]
          }
        }
        //引导线设置
        if (dataOption.series[0].label.show == true) {
          //显示引导线时，执行以下方法
          dataOption.series[0].itemStyle.normal={
            color: function (colors) {
              return dataOption.bgColor[colors.dataIndex]
            }
          }
          dataOption.series[0].itemStyle.normal.label={
            show: true,
            position: "outside",
            color: "#ddd",
            fontSize:dataOption.labelFontSize
          }
          if(dataOption.labelFontColor){
            dataOption.series[0].itemStyle.normal.label.color=dataOption.labelFontColor
          }
          // if(dataOption.legend.isShowVal){
            dataOption.series[0].itemStyle.normal.label.formatter=  formatterStr
            // }
          let labelLine = {
            length:dataOption.labelLineLength == undefined ? 15 : dataOption.labelLineLength, //第一段引导线长度
            length2:dataOption.labelLineLength2 == undefined ? 10 : dataOption.labelLineLength2, //第二段引导线长度
            lineStyle: {
              width: dataOption.labelLineWidth //引导线宽度
            },
          };
          dataOption.series[0].itemStyle.normal.labelLine = labelLine;
        }
        //图例设置
        let legends = [];
        let formatterAgrs = {};
        dataOption.legend.height = "80%"; //图例显示区域的高度
        dataOption.legend.itemGap = dataOption.legendItemGapY || 10; //图例垂直方向间隔，默认10
        dataOption.legend.textStyle.fontSize = dataOption.legendFontSize; //图例字号
        dataOption.legend.textStyle.color = dataOption.legendFontColor; //图例字号
        dataOption.legend.icon = dataOption.icon; //图例形状
        dataOption.legend.itemWidth = dataOption.legend.itemWidth; //图例形状
        dataOption.legend.itemHeight = dataOption.legend.itemHeight; //图例形状

        if (dataOption.showPercentage == true) {
          let rich = {
            // 通过富文本rich给每个项设置样式，下面的one、two可以理解为"每一列"的样式
            one: {
              // 设置类别这一列的样式
              width: dataOption.textWidth,
              color: dataOption.legendFontColor,
              fontSize: dataOption.legendFontSize
            },
            two: {
              // 设置百分比这一列的样式
              width: 50,
              color: dataOption.legendFontColor,
              fontSize: dataOption.legendFontSize
            }
          };
          dataOption.legend.textStyle.rich = rich;

          formatterAgrs = {
            formatter: name => {
              // formatter格式化函数动态呈现数据
              var data = dataOption.series[0].data;
              var total = 0; // 用于计算总数
              var target; // 遍历拿到数据
              for (var i = 0; i < data.length; i++) {
                total += data[i].value;
                if (data[i].name == name) {
                  target = data[i].value;
                }
              }
              let v = ((target / total) * 100).toFixed(1);
              return `{one|${name}}  {two|${v}%　}`;
              //     富文本第一列样式应用    富文本第二列样式应用
            }
          };
        }

        let colNumber = dataOption.legendCols || 1; //选择图例分布的列数，默认1
        let legendCols = []; //用于装载每一列图例显示的标签
        let items = dataOption.series[0].data; // 饼图的数据

        for (let i = 0; i < colNumber; i++) {
          let legendData = [];
          let k = 0;
          for (let j = i; j < items.length; j += colNumber) {
            //以列数为步长
            legendData[k] = items[j]["name"];
            
            k++;
          }
          legendCols[i] = legendData;
        }
        for (let i = 0; i < legendCols.length; i++) {
          let legendTemp = JSON.parse(JSON.stringify(dataOption.legend));
          legendTemp.left =(dataOption.legendPositionLeft + (dataOption.legendItemGapX == undefined ? 0 : dataOption.legendItemGapX) * i || 60) + "%"; //图例距离左边框 + 间隔的距离，默认60%
          legendTemp.top = (dataOption.legendPositionTop || 30) + "%"; //图例距离上边框距离，默认30%
          legendTemp.data = legendCols[i];
          if(dataOption.legend.isShowVal){
            legendTemp.formatter=  (name)=>{
              let findIx=items.findIndex(rw=>rw.name==name)
              if(findIx!==undefined){
                return name+'：'+ items[findIx]["value"]; // 返回自定义的文本
              }else{
                return name
              }
              
            }
          }
          legends.push(legendTemp);
        }
        //双环图 对series[0].data的处理，需要写在上面，否则创建newSeriesTemp，复制不到
        if (dataOption.isRing && dataOption.innerShadow) {
          let newSeriesTemp = JSON.parse(JSON.stringify(dataOption.series[0]));
          newSeriesTemp.emphasis = {};
          newSeriesTemp.radius = [
            dataOption.innerRadius * (1 - dataOption.innerRadiusScale),
            dataOption.innerRadius * 1.1
          ];
          let innerItemStyle = {
            opacity: dataOption.innerRadiusOpacity // 内圈 透明度
          };
          (newSeriesTemp.hoverAnimation = false),
            (newSeriesTemp.itemStyle = innerItemStyle);
          newSeriesTemp.label.show = false;
          dataOption.series.push(newSeriesTemp);
        }
        dataOption.title.top=dataOption.title.top+"%"
        // dataOption.title.textAlign="center"
        dataOption.title.left=dataOption.title.left+"%"
        let total=0//总数
        for(let i=0;i<result.length;i++){
          total=total+result[i].value
        }
        if(dataOption.titletextType&&dataOption.titletextType=='1'){//副标题展示总数
          dataOption.title.text=total
        }
        if(dataOption.subtextType&&dataOption.subtextType=='1'){//副标题展示总数
          
          dataOption.title.subtext=total
        }
        dataOption.tooltip.formatter="{b} : {c}"//提示显示去除百分比
        let optionTitle2={}
        if(dataOption.otherTitle){//第二标题设置
          optionTitle2=dataOption.otherTitle
          if(dataOption.otherTitletextType&&dataOption.otherTitletextType=='1'){//副标题展示总数
            optionTitle2.text=total
          }
          if(dataOption.otherTitleSubtextType&&dataOption.otherTitleSubtextType=='1'){//副标题展示总数
            optionTitle2.subtext=total
          }
        }
        let option = {
          title: [dataOption.title,optionTitle2],
          tooltip: dataOption.tooltip,
          // legend: dataOption.legend,
          legend: legends,
          series: dataOption.series,
          formatter: formatterAgrs.formatter
          //, color: color
        };
        this.chart.setOption(option, true);
        // this.$message({
        //   message: "这是饼图的绘制"+this.chart.id,
        //   type: "warning",
        // });
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
      } catch (error) {
        console.log("报错",error);
        // this.$message('饼图报错'+JSON.stringify(error));
      }

    },
  
  }
};
</script>
