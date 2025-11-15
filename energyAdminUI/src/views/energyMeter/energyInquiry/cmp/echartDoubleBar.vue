<template>
    <div class="chartBar" :style="{'min-height':(tableConHeight/2-14)+'px'}">
        <div class="integrative-chart" :style="{'min-height':(tableConHeight/2-14)+'px'}">
            <div id="chartBar" class="chart-container" :style="{'min-height':(tableConHeight/2-14)+'px'}"></div>
        </div>
    </div>
</template>

<script>
import * as echarts from 'echarts';
var dayjs = require("@/utils/day.js");
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
    mixins: [resizeTableCon],
    props: {
        allData: {
            type: Array,
            default: () => {
                return [];
            }
        },
        dataType:{
            type:String,
            default:'UseVale'
        },
        lengright:{
            type:String,
            default:'10%'
        },
        formatStr:{
            type:String,
            default:'MM-DD'
        },
        echartsTitle:{
            type:String,
            default:'能源消耗'
        },
        dateTitle:{
            type:String,
            default:'日期'
        },
        isShowMax:{
            type:Boolean,
            default:true
        }
    },
    data() {
        return {
            unit:'unit',
            chart: null,
            xData: [],
            yData: [],
            tooltipData: [],    // 提示框显示的日期
            isDataZoom: true,  // 是否显示数据缩放条
            colorList: [{
                lineColor: '#28B589',
                areaColorTop: 'rgba(53, 200, 255, 1)',
                areaColorBom: 'rgba(53, 200, 255, 0.50)'
            },{
                lineColor: '#32AEDF',
                areaColorTop: 'rgba(42, 211, 154, 0.80)',
                areaColorBom: 'rgba(42, 211, 154, 0.40)'
            },{
                lineColor: '#7A227A',
                areaColorTop: 'rgba(106,9,106, 0.3)',
                areaColorBom: 'rgba(106,9,106, 0.05)'
            }],
            chartData:[],
            colorPool: ['rgba(53, 200, 255, 1)', 'rgba(42, 211, 154, 1)', 'rgb(255, 193, 7)', 'rgb(255, 87, 34)', 'rgb(156, 39, 176)'],
        };
    },
    watch: {
        // tableData: {
        //     handler(newVal) {
        //         this.processTableData(newVal);
        //         this.initChart();
        //         window.addEventListener('resize', this.resizeChart);
        //     },
        //     deep: true,
        // },
        allData: {
            handler(newVal) {
                if(newVal){
                    this.$nextTick(()=>{
                        this.loadChart(newVal)
                        window.addEventListener('resize', this.resizeChart);
                    })
                }
            },
            deep: true,
            immediate:true
        },
        dataType:{
            handler(newval){
                this.loadChart(this.allData)
            }
        }
    },
    beforeDestroy() {
        if (this.chart) {
            this.chart.dispose();
        }
        window.removeEventListener('resize', this.resizeChart);
    },
    methods: {
        hexToRgb(hex) {
            if (hex.startsWith('rgb')) {
                return hex.match(/\d+/g).join(',');
            }
            const r = parseInt(hex.slice(1, 3), 16);
            const g = parseInt(hex.slice(3, 5), 16);
            const b = parseInt(hex.slice(5, 7), 16);
            return `${r},${g},${b}`;
        },
        loadChart(newVal){
            this.processTableData(newVal);
            this.initChart();
        },
        // 处理数据：按日期汇总CarbonEmission
        processTableData(barData) {
            console.log(barData,'barData');
            if (!barData.length) {
                this.chartData = { dates: [], series: [] };
                return;
            }
           // 1. 获取所有唯一日期并排序
           let dates=[]
           let names=[]
           let series=[]
           const barWidth = this.transformFontSize(12);
           let seriesS=barData.map((row,inx)=>{
            let rowSeries={
                name: row.name,
                color:this.colorPool[inx],
                mainColor: this.colorList[inx].areaColorTop,
                lightColor: this.colorList[inx].areaColorBom,
            }
            let Details=JSON.parse(JSON.stringify(row.data))
            let sideData=Details.map(ro=>[ro.DDate,ro[this.dataType]])
            let rowleft={
                z: 1,
                type: 'bar',
                name: row.name,
                itemStyle: {
                    borderRadius: [0, 0, 0, 0],
                    color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                },
                itemStyle: {
                    borderWidth: 1,
                    borderColor: this.colorList[inx].lineColor,
                    color: this.colorPool[inx], // 控制顶部方形的颜色
                },
                data: sideData,
                barWidth: 7,
                barGap: 0,
            }
            let rowRight={
                type: 'bar',
                // stack: 'right', // 设置堆叠的组名
                itemStyle: {
                    borderWidth: 1,
                    borderColor: this.colorList[inx].lineColor,
                    color: this.colorPool[inx], // 控制顶部方形的颜色
                },
                z: 2,
                name: row.name,
                // type: "pictorialBar",
                data: sideData,
                symbol: "diamond",
                symbolOffset: ["50%", "50%"],
                symbolSize: [7, 7], //=========================
                itemStyle: {
                    normal: {
                        color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                    },
                },
                tooltip: {
                    show: false,
                },
                barWidth:8,
                barGap: 0,
            }
            // console.log(topDataArr,'topDataArr');
            let xOffset=0
            if(inx==0){
                xOffset=-60
            }else{
                xOffset=58*inx
            }
            let rowTop={
                z: 5,
                name: row.name,
                type: "pictorialBar",
                symbolPosition: "end",
                data: sideData,
                symbol: "diamond",
                // itemStyle: {
                //     borderWidth: 1,
                //     borderColor: this.colorList[inx].lineColor,
                //     color: this.colorPool[inx], // 控制顶部方形的颜色
                // },
                symbolOffset: [xOffset+"%", "-50%"],
                symbolSize: [barWidth+3, (7 * (barWidth+3)) / barWidth],
                itemStyle: {
                    normal: {
                        color: this.colorPool[inx]
                    },
                },
                tooltip: {
                    show: false,
                },
            }
            series.push(rowleft)
            series.push(rowRight)
            series.push(rowTop)
            if(inx<barData.length-1){
                let rowleft1={
                    z: 1,
                    type: 'bar',
                    name: row.name,
                    itemStyle: {
                        borderRadius: [0, 0, 0, 0],
                        color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                    },
                    data: [],
                    barWidth: 1.5,
                    barGap: 0,
                }
                let rowRight1={
                    type: 'bar',
                    // stack: 'right', // 设置堆叠的组名
                    
                    z: 2,
                    name: row.name,
                    // type: "pictorialBar",
                    data: [],
                    symbol: "diamond",
                    symbolOffset: ["50%", "50%"],
                    symbolSize: [1.5, 1.5], //=========================
                    itemStyle: {
                        normal: {
                            color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                        },
                    },
                    tooltip: {
                        show: false,
                    },
                    barWidth:1.5,
                    barGap: 0,
                }
                // console.log(topDataArr,'topDataArr');
                let xOffset1=0
                if(inx==0){
                    xOffset1=-50
                }else{
                    xOffset1=50*inx
                }
                let rowTop1={
                    z: 3,
                    name: row.name,
                    type: "pictorialBar",
                    symbolPosition: "end",
                    data: [],
                    symbol: "diamond",
                    symbolOffset: [xOffset1+"%", "-50%"],
                    symbolSize: [4.5, 4.5],
                    itemStyle: {
                        normal: {
                            color: this.colorPool[inx]
                        },
                    },
                    tooltip: {
                        show: false,
                    },
                }
                series.push(rowleft1)
                series.push(rowRight1)
                series.push(rowTop1)
            }
            let dataArr=row.data.map(rw=>{
                let arr=[rw.DDate,rw[this.dataType]]
                if(dates.includes(rw.DDate)){}else{
                    dates.push(rw.DDate)
                }
                return arr
            })
            names.push(row.name)
            rowSeries.data=dataArr
            return rowSeries
           })
           dates.sort((a,b)=>Number(a)-Number(b))
           this.chartData = { dates,series,names };
        },
        formatValueByUnit(rawValue) {
            // 先将 rawValue 转为安全数字（排除 '-'、NaN 等）
            const safeValue = this.safeParseNumber(rawValue);
            if (this.unit === 'LageUnit') {
                // LageUnit：除以10000 + 保留两位小数（避免后续累加精度问题）
                return Math.round((safeValue / 10000) * 100) / 100;
            }
            // 其他单位：直接返回安全数字
            return safeValue;
        },
        safeParseNumber(value) {
            // 处理 '-' 无数据标识
            if (value === '-' || value === null || value === undefined) {
                return 0;
            }
            // 尝试转为数字，失败则返回 0
            const num = Number(value);
            return isNaN(num) ? 0 : num;
        },
        initChart() {
            if (this.chart) {
                this.chart.dispose();
            }
            const chartDom = document.getElementById('chartBar');
            this.chart = echarts.init(chartDom);
            const { dates, series,names } = this.chartData;
            const that = this;

            
            const option = {
                grid: {
                    left: '0%', // 左边距离
                    right: '0%', // 右边距离
                    bottom: '0%', // 底部距离
                    top: '13%', // 顶部距离
                    containLabel: true, // 包含标签
                },
                legend: {
                    show: true, // 确保图例显示
                    data: names, // 数据系列名称
                    itemWidth:10,
                    itemHeight:4,
                    textStyle: {
                        color: '#fff', // 图例文字颜色
                    },
                    orient: 'horizontal', // 图例的排列方式：'horizontal' 或 'vertical'
                    top: '0', // 图例位置，可选值 'top', 'bottom', 'left', 'right'
                    left:"197px",
                    formatter: function (name) {
                        // if (name === '尖') {
                        // return '全部'; // 只替换 '尖' 为 '全部'
                        // }
                        return name; // 其他保持不变
                    },
                },
                xAxis: {
                    type: 'category',
                    data: dates,
                    axisLine: {
                        lineStyle: {
                            color: 'rgba(255,255,255,0.2)',
                            type: 'solid',
                        },
                    },
                    splitLine: {
                        lineStyle: {
                            color: 'rgba(255,255,255,0.2)',
                            type: 'solid', // 关键配置：设置为虚线
                            width: 1 // 线宽
                        }
                    },
                    axisLabel: {
                        color: 'rgba(255, 255, 255, 0.6)',
                        fontSize: 14,
                        align: 'center', // 设置刻度标签右对齐
                    },
                    axisTick: {
                        show:false,
                        alignWithLabel: true, // 设置刻度线居中
                    },
                },
                yAxis: {
                    type: 'value',
                    axisLine: {
                        lineStyle: {
                            color: 'rgba(255, 255, 255, 0.2)',
                            type: 'solid',
                        },
                    },
                    splitLine: {
                        lineStyle: {
                            color: 'rgba(255, 255, 255, 0.2)',
                            type: 'solid',
                        },
                    },
                    axisLabel: {
                        color: 'rgba(255, 255, 255, 0.6)',
                        fontSize: 14,
                        align: 'right', // 设置刻度标签右对齐
                    },
                },
                series: series,
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    extraCssText: 'text-align: left;',
                    formatter: function (params) {
                        const date = params[0].name;
                        let content = `<div style="line-height: 1.6;">${that.dateTitle}: ${date}</div>`;
                        params.forEach(item => {
                            content += `<div>${item.seriesName}: ${item.value[1]}</div>`;
                        });
                        return content;
                    },
                    backgroundColor: 'rgba(18, 19, 27, 0.9)',
                    textStyle: {
                        color: '#ffffff',
                        rich: {
                            title: {
                                color: 'rgba(255,255,255,0.6)',
                                textAlign: 'left'
                            },
                            content: {
                                color: '#ffffff',
                                textAlign: 'left'
                            },
                        },
                    },
                },
            };
            console.log(JSON.parse(JSON.stringify(option)),'optionoption');
            option && this.chart.setOption(option);
        },

        transformFontSize(fontsize) {
            // 获取屏幕宽度
            const width = window.screen.width;
            const ratio = width / 1920;
            // 取下整
            return parseInt(fontsize * ratio);
        },
        // 调整图表大小
        resizeChart() {
            if (this.chart) {
                this.chart.resize();
            }
        }
    },
};
</script>

<style lang="less" scoped>
.chartBar {
    padding: 0;
    height: 100%;
    width: 100%;
    .heard-title {
        font-weight: 500;
        font-size: 14px;
        color: #FFFFFF;
        position: relative;
        padding-left: 12px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        &::before{
            position: absolute;
            content: '';
            top: 50%;
            left: 0;
            width: 4px;
            height: 14px;
            transform: translateY(-50%);
            background: #3DB98F;
            border-radius: 2px;
        }
        > .title span{
            font-weight: 500;
            font-size: 12px;
            color: rgba(255, 255, 255, 0.6);
        }
        .tabLine{
            display: flex;
            align-items: center;
            font-weight: 500;
            font-size: 12px;
            color: #FFFFFF;
            >div{
                margin-left: 24px;
                display: flex;
                align-items: center;
                >div{
                    width: 20px;
                    height: 1px;
                    border: 1px solid #2AD39A;
                    margin-right: 4px;
                }
            }
        }
    }
}
.integrative-chart {
    width: 100%;
    height: calc(100% - 30px);
}
.integrative-chart .chart-container{
    width: 100%;
    min-width: 930px;
    height: 100%;
}
</style>
