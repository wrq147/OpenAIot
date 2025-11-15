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
                lineColor: '#3DB98F',
                areaColorTop: 'rgba(61, 185, 143, 0.3)',
                areaColorBom: 'rgba(61, 185, 143, 0.05)'
            },{
                lineColor: '#FEA725',
                areaColorTop: 'rgba(254,167,36, 0.3)',
                areaColorBom: 'rgba(254,167,36, 0.05)'
            },{
                lineColor: '#7A227A',
                areaColorTop: 'rgba(106,9,106, 0.3)',
                areaColorBom: 'rgba(106,9,106, 0.05)'
            }],
            chartData:[],
            colorPool: ['rgb(42, 211, 154)', 'rgb(53, 200, 255)', 'rgb(255, 193, 7)', 'rgb(255, 87, 34)', 'rgb(156, 39, 176)'],
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
            if (!barData.length) {
                this.chartData = { dates: [], series: [] };
                return;
            }
           // 1. 获取所有唯一日期并排序
            const dates = Array.from(new Set(barData.map(item => {
                let timer=dayjs(item.DDate).format(this.formatStr)
                if(this.dateTitle=='时间'){
                    return Number(timer)+'h'
                }
                return timer
            }))).sort((a, b) => new Date(a) - new Date(b));
            // 新增：按日期汇总数据，将相同日期的数据相加
            const dateDataMap = {};
            barData.forEach(item => {
                let date = dayjs(item.DDate).format(this.formatStr);
                if(this.dateTitle=='时间'){
                    date= Number(date)+'h'
                }
                const rawValue = item[this.dataType];
                if (!dateDataMap[date]) {
                    dateDataMap[date] = 0;
                }
                dateDataMap[date] += this.formatValueByUnit(rawValue);
            });
            const data = dates.map(date => {
                return dateDataMap[date] ? (dateDataMap[date]).toFixed(2) : 0;
            });
            // 4. 创建系列数据
            const series = [{
                    name: this.echartsTitle,
                    data,
                    mainColor: 'rgba(42, 211, 154, 0.80)',
                    lightColor: 'rgba(42, 211, 154, 0.40)',
                    
                }];


            this.chartData = { dates, series };
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
            const { dates, series } = this.chartData;
            const that = this;

            // 计算柱体间距（根据系列数量动态调整）
            const barWidth = this.transformFontSize(18);
            const barGap = this.transformFontSize(5);

            const seriesConfig = series.map((item, index) => {
                return {
                    type: 'custom',
                    name: item.name,
                    data: item.data,
                    dimensions: ['name', 'value'],
                    markPoint: {
                        show:false,
                        data: this.isShowMax?[{type: 'max', name: '最大值'}]:[], // 标记最大值点
                        itemStyle:{
                            show:false,
                            borderColor:this.colorPool[0],
                            borderWidth:2,
                            color:this.colorPool[0]
                        }
                    },
                    barWidth,
                    // 计算每个系列的偏移量，实现分组效果
                    xAxisIndex: 0,
                    renderItem: (params, api) => {
                        // 计算当前系列在分组中的位置
                        const groupOffset = (index - (series.length - 1) / 2) * (barWidth + barGap);
                        const xIndex = params.dataIndex;
                        const value = api.value(1);
                        // 计算坐标（加入分组偏移）
                        const [baseX, topY] = api.coord([xIndex, value]);
                        const x = baseX + groupOffset;
                        const bottomY = api.coord([xIndex, 0])[1];

                        return {
                        type: 'group',
                        children: [
                            // 左侧面
                            {
                            type: 'polygon',
                            shape: {
                                points: [
                                [x - this.transformFontSize(10), topY - this.transformFontSize(4)],
                                [x - this.transformFontSize(10), bottomY],
                                [x, bottomY],
                                [x, topY]
                                ]
                            },
                            style: {
                                fill: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                                { offset: 0, color: item.mainColor  },
                                { offset: 1, color: item.lightColor  }
                                ])
                            }
                            },
                            // 右侧面
                            {
                            type: 'polygon',
                            shape: {
                                points: [
                                [x, topY],
                                [x, bottomY],
                                [x + this.transformFontSize(10), bottomY],
                                [x + this.transformFontSize(10), topY - this.transformFontSize(4)]
                                ]
                            },
                            style: {
                                fill: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                                { offset: 0, color: item.mainColor  },
                                { offset: 1, color: item.lightColor  }
                                ])
                            }
                            },
                            // 顶部面
                            {
                                type: 'polygon',
                                shape: {
                                    points: [
                                    [x, topY],
                                    [x - this.transformFontSize(10), topY - this.transformFontSize(4)],
                                    [x, topY - this.transformFontSize(8)],
                                    [x + this.transformFontSize(10), topY - this.transformFontSize(4)]
                                    ]
                                },
                                style: {
                                    fill: 'rgba(42, 211, 154, 1)'
                                }
                            },
                        ]
                        };
                    }
                };
            });
            
            const option = {
                grid: {
                    left: '0%', // 左边距离
                    right: '0%', // 右边距离
                    bottom: '0%', // 底部距离
                    top: '13%', // 顶部距离
                    containLabel: true, // 包含标签
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
                series: seriesConfig,
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    extraCssText: 'text-align: left;',
                    formatter: function (params) {
                        const date = params[0].name;
                        let content = `<div style="line-height: 1.6;">${that.dateTitle}: ${date}</div>`;
                        params.forEach(item => {
                            content += `<div>${item.seriesName}: ${item.value}</div>`;
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
