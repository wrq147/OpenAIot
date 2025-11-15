<template>
  <div style="height: 100%;width: 100%;">
    <div class="pie_echart" style="height: 100%;width: 100%;"></div>
  </div>
</template>

<script>
import * as echarts from 'echarts';
export default {
  name: 'EnergyAdminUIEchartPie',
  props:{
    pieData:{
      type:Array,
      default:()=>{
        return []
      }
    },
    unitSelect:{
      type:String,
      default:'Unit'
    },
    nameFiled:{
      type:String,
      default:'FacilityName'
    },
    valueFiled:{
      type:String,
      default:'UseVale'
    },
  },
  data() {
    return {
      EquipemntList:[],
      colorList:['rgba(255, 199, 90, 1)','rgba(53, 200, 255, 1)','rgba(42, 211, 154, 1)','rgba(255, 137, 109, 1)'],
      options:{
        title: [{
          text: "",
          subtext:'',
          left: "25%",
          top: "32%",
          textAlign: "center",
          subtextStyle: {
            fill: "rgba(153, 153, 153, 1)",
            fontSize: 12,
            fontWeight: 200,
            color: 'rgba(153, 153, 153, 1)'
          },
          textStyle:{
            align: "center",
            fill: "rgba(51, 51, 51, 1)",
            fontSize: 24,
            fontWeight: 500,
            color: 'rgba(51, 51, 51, 1)'
          }
        }],
        series: [{
          name: '设备状态',
          type: 'pie',
          center: ['50%', '50%'] ,
          radius: ['40%', '58%'] ,
          // roseType: 'area',
          // roseType: 'radius',
          minAngle: 5, // 设置最小角度为5度
          itemStyle: {
            borderRadius: 0
          },
          label: {
            show:true,
            position: "outside",
            formatter: (params) => {
              //只有“直接访问”使用大标签，其他都使用小标签
              return params.data.name
            },
            rich: {
              // colorBlock1: {
              //     backgroundColor:'rgba(53, 200, 255, 1)',
              //     width: 10, // 块的大小
              //     height: 10, // 块的大小
              //     align: 'center',
              //     borderRadius: 2 // 可选，圆角半径
              // },
              // colorBlock2: {
              //     backgroundColor:'rgba(42, 211, 154, 1)',
              //     width: 10, // 块的大小
              //     height: 10, // 块的大小
              //     align: 'center',
              //     borderRadius: 2 // 可选，圆角半径
              // },
              namef: {
                color: 'rgba(255, 255, 255, 0.6)',
                height: 12,
                fontSize:12,
                align: 'left',
              },
              valuef: {
                color: 'rgba(255, 255, 255, 1)',
                height: 28,
                fontWeight: "bold"
              },
              percentf: {
                color: 'rgba(255, 255, 255, 1)',
                height: 28,
                fontWeight: "bold"
              },
            }
            
          },
          //饼块起始角度
          startAngle: 390,
          avoidLabelOverlap: false,
          //设置数据标签引导线
          itemStyle:{
            normal:{
              labelLine: {
                show: true,
                length: 22,
                length2: 12,
                minTurnAngle:30,
                lineStyle: {
                  width: 1 //引导线宽度
                },
              },
            }
          },
          data: [{
                "name": "设备类型一",
                "value": 15,
                "itemStyle": {
                    "color": 'rgba(54, 183, 231, 1)',
                }
            }, {
                "name": "设备类型二",
                "value": 22,
                "itemStyle": {
                    "color": 'rgba(42, 211, 154, 1)',
                }
            }]
        }]
      },
    };
  },
  watch:{
    pieData:{
      handler(newval){
        this.getDevTotal()
      },
      deep:true,
    }
  },
  mounted() {
    
  },

  methods: {
    hexToRgb(hex) {
      if (hex&&hex.startsWith('rgb')) {
          return hex.match(/\d+/g).join(',');
      }
      const r = parseInt(hex.slice(1, 3), 16);
      const g = parseInt(hex.slice(3, 5), 16);
      const b = parseInt(hex.slice(5, 7), 16);
      return `${r},${g},${b}`;
    },
    getRandomColor() {
      var r = Math.floor(Math.random() * 256);
      var g = Math.floor(Math.random() * 256);
      var b = Math.floor(Math.random() * 256);
      if(this.colorList.includes(`rgba(${r},${g},${b},1)`)){
        return this.getRandomColor()
      }
      return `${r},${g},${b}`;
    },
    getDevTotal(){
      //获取设备统计信息
      let colorIndex = 0;
      let typeData=this.pieData.map((row,inx)=>{
        let obj={}
        let index=inx+1
        let customColor=this.getRandomColor()
        // if(this.unitSelect=='LageUnit'){
        //   row[this.valueFiled]=row[this.valueFiled]/10000
        // }
        obj={
          "name": row[this.nameFiled],
          "value": row[this.valueFiled],
          "itemStyle": {
              "color": this.colorList[inx]?this.colorList[inx]:`rgba(${customColor},1)`,
          }
        }
        this.options.series[0].label.rich['colorBlock'+index]={
            backgroundColor:this.colorList[inx]?this.colorList[inx]:`rgba(${customColor},1)`,
            width: 10, // 块的大小
            height: 10, // 块的大小
            align: 'center',
            borderRadius: 2 // 可选，圆角半径
        }
        colorIndex++
        return obj
      })
      this.options.series[0].data=typeData
      this.options.series[0].label.formatter=(params) => {
        //只有“直接访问”使用大标签，其他都使用小标签
        let index=params.dataIndex+1
        return `{colorBlock${index}|} {namef|${params.data.name}}\n{valuef|${params.data.value} }{percentf|${params.percent}%}`;
      }
      setTimeout(() => {
        this.setDraw()
        
      }, 1000);
    },
    setDraw(){
      let statusChart1 = echarts.init(
        document.querySelector(".pie_echart")
      );
      // console.log(this.options,'this.options');
      statusChart1.setOption(this.options);
    },
  },
};
</script>

<style lang="less" scoped>
.pie_echart{
  width: 420px;
  height: 240px;
  background: rgba(34, 46, 64, 1);
  border-radius: 4px;
}
</style>