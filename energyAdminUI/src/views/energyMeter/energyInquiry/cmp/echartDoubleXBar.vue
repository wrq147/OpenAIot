<template>
    <div class="chartBar" :style="{'min-height':'240px'}">
        <div class="integrative-chart" :style="{'min-height':'240px'}">
            <div id="chartxBar" class="chart-container" :style="{'min-height':240+'px'}"></div>
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
        echartData: {
            type: Array,
            default: () => {
                return [];
            }
        },
        dataType:{
            type:String,
            default:'value'
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
            default:'数据指标'
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
                lineColor: 'rgba(53, 200, 255, 1)',
                areaColorTop: 'rgba(53, 200, 255, 1)',
                areaColorBom: 'rgba(53, 200, 255, 0.50)'
            },{
                lineColor: 'rgba(42, 211, 154, 1)',
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
        echartData: {
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
        // dataType:{
        //     handler(newval){
        //         this.loadChart(this.echartData)
        //     }
        // }
    },
    beforeDestroy() {
        if (this.chart) {
            this.chart.dispose();
        }
        window.removeEventListener('resize', this.resizeChart);
    },
    methods: {
        loadBarData(){
            let that=this
            let barData={
                "title": {
                    "text": "",
                    "textStyle": {
                        "color": "rgba(51, 51, 51, 1)",
                        "fontStyle": "normal",
                        "fontWeight": "normal",
                        "fontSize": 20,
                        "align": "center"
                    },
                    "subtext": "",
                    "subtextStyle": {
                        "color": "rgba(51, 51, 51, 1)",
                        "fontSize": 14,
                        "align": "center"
                    },
                    "itemGap": 7,
                    "top": 1,
                    "textAlign": "left",
                    "left": 3,
                    "show": false
                },
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    extraCssText: 'text-align: left;',
                    formatter: function (params) {
                        const date = params[0].name;
                        let content = `<div style="line-height: 1.6;">${that.dateTitle}: ${date}</div>`;
                        params.forEach(item => {
                            content += `<div>${item.seriesName}: ${item.value[0]}</div>`;
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
                "legend": [],
                "grid": {
                    "left": this.transformFontSize(14),
                    "right": this.transformFontSize(35),
                    // "width": "95%",
                    "containLabel": true,
                    "top": this.transformFontSize(50),
                    "bottom": 0
                },
                "xAxis": {
                    "type": "value",
                    "axisLabel": {
                        "show": true,
                        "color": "rgba(153, 153, 153, 1)",
                        "fontStyle": "normal",
                        "fontWeight": "normal",
                        "fontSize": this.transformFontSize(12),
                        "align": "center",
                        "verticalAlign": "center",
                        "lineHeight": this.transformFontSize(14),
                        "interval": "0",
                        "inside": false,
                        "rotate": 0,
                        "margin": this.transformFontSize(20)
                    },
                    "axisLine": {
                        "show": true,
                        "lineStyle": {
                            "color": "rgba(255, 255, 255, 0.2)",
                            "width": this.transformFontSize(1),
                            "type": "solid"
                        }
                    },
                    "axisTick": {
                        "show": false,
                        "length": false,
                        "inside": this.transformFontSize(6),
                        "lineStyle": {
                            "color": "rgba(244, 244, 244, 1)",
                            "width": this.transformFontSize(2),
                            "type": "solid"
                        }
                    },
                    "splitLine": {
                        "show": true,
                        "lineStyle": {
                            "color": "rgba(255, 255, 255, 0.2)",
                            "width": this.transformFontSize(1),
                            "type": "solid"
                        }
                    },
                    "position": "bottom",
                    "show": true
                },
                "yAxis": {
                    "position": "left",
                    "type": "category",
                    "axisLabel": {
                        "show": true,
                        "color": "rgba(153, 153, 153, 1)",
                        "fontStyle": "normal",
                        "fontWeight": "normal",
                        "fontSize": this.transformFontSize(12),
                        "interval": "0",
                        "inside": false,
                        "margin": this.transformFontSize(10),
                        "textStyle": {
                            "verticalAlign": null,
                            "align": null,
                            "padding": null
                        }
                    },
                    "axisLine": {
                        "show": false,
                        "lineStyle": {
                            "color": "rgba(244, 244, 244, 1)",
                            "width": this.transformFontSize(2),
                            "type": "solid"
                        }
                    },
                    "axisTick": {
                        "show": false,
                        "length": false,
                        "inside": this.transformFontSize(6),
                        "lineStyle": {
                            "color": "rgba(244, 244, 244, 1)",
                            "width": this.transformFontSize(2),
                            "type": "solid"
                        }
                    },
                    "splitLine": {
                        "show": false,
                        "lineStyle": {
                            "color": "rgba(244, 244, 244, 1)",
                            "width": this.transformFontSize(2),
                            "type": "solid"
                        }
                    },
                    "show": true,
                    "data": []
                },
                "series": []
            }
            return barData
        },
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
            setTimeout(()=>{
                if(newVal&&newVal.length>0){
                    this.initChart();
                }
            },50)
        },
        // 处理数据：按日期汇总CarbonEmission
        processTableData(barData) {
            if (!barData.length) {
                this.chartData = { dates: [], series: [] };
                return;
            }
           // 1. 获取所有唯一日期并排序
           let dates=[]
           let series=barData.map((row,ix)=>{
            let rowSeries={
                name: row.name,
                color:this.colorPool[ix],
            }
            let dataArr=row.data.map(rw=>{
                let arr=[rw[this.dataType],rw.text]
                if(dates.includes(rw.text)){}else{
                    dates.push(rw.text)
                }
                return arr
            })
            rowSeries.data=dataArr
            return rowSeries
           })
           
            this.chartData = { dates,series };
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
            const chartDom = document.getElementById('chartxBar');
            this.chart = echarts.init(chartDom);
            const { dates,series } = this.chartData;
            const that = this;
            // 计算柱体间距（根据系列数量动态调整）
            const barWidth = this.transformFontSize(10);
            const barGap = this.transformFontSize(5);
            let barData=this.loadBarData()
            let lengArr=[]
            let seriesArr=[]
            if(series&&series.length>0){
                let att=series.map((row,ix)=>{
                    let lengRow={
                        "show": true,
                        "type": "scroll",
                        "orient": "vertical",
                        "right": "10%",
                        "align": "left",
                        "top": "4%",
                        "textStyle": {
                            "fontSize": this.transformFontSize(14),
                            color:'rgba(255, 255, 255, 1)'
                        },
                        "height": "80%",
                        "itemWidth": this.transformFontSize(10),
                        "itemHeight": this.transformFontSize(4),
                        "icon": "rect",
                        "itemGap": this.transformFontSize(8),
                        "left": Number(63+ix*13)+"%",
                        "data": [row.name]
                    }
                    lengArr.unshift(lengRow)
                    let seriesRow={
                        "type": "bar",
                        "barWidth": barWidth,
                        "label": {
                            "show": false,
                        },
                        "name": row.name,
                        "itemStyle": {
                            "color": this.colorPool[ix],
                            "barBorderRadius": [0,0,0,0]
                        },
                        "data": row.data
                    }
                    seriesArr.unshift(seriesRow)
                    return {
                        lengRow:lengRow,
                        seriesRow:seriesRow
                    }
                })
                barData.legend=JSON.parse(JSON.stringify(lengArr))
                barData.series=JSON.parse(JSON.stringify(seriesArr))
                barData.yAxis.data=JSON.parse(JSON.stringify(dates))
                this.chart.setOption(barData);
            }
            
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
    background: rgba(34, 46, 64, 1);
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
    height: 100%;
}
</style>
