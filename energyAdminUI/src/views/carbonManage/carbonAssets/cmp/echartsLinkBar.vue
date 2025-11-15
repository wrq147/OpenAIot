<template>
    <div class="chartBar">
        <div class="tendency-heard">
            <div class="title">碳排放量趋势<span>(单位：kgCO₂e/台)</span></div>
        </div>
        <div id="echartsLinkBar" class="chart-container"></div>
    </div>
</template>
<script>
import * as echarts from 'echarts';

export default {
    props: {
        modelData: {
            type: Object,
            default: () => {},
        },
    },
    data() {
        return {
            chart: null,
            totalInfo: {
                xData: [],
                barData: [],
                lineData: [],
            },
        };
    },
    watch: {
        modelData: {
            handler(newVal) {
                // 先处理数据（提取xData、processData、tooltipData）
                this.processChartData(newVal);
                this.initChart();
                window.addEventListener('resize', this.resizeChart);
            },
            deep: true,
        }
    },
    beforeDestroy() {
        if (this.chart) {
            this.chart.dispose();
        }
        window.removeEventListener('resize', this.resizeChart);
    },
    methods: {
        /**
         * 处理图表数据：提取xData、计算processData、补充tooltipData
         * @param {Object} newVal - 最新的modelData
         */
        processChartData(newVal) {
            // 提取“碳排量”的月度数据（用于折线图）
            const carbonEmissionItem = newVal.Details.find(
                (item) => item.CarbonType === "碳排量"
            );
            const quotaTotalItem = newVal.Details.find(
                (item) => item.CarbonType === "配额总量"
            );
            const lineData = [];
            const lineDataTwo = [];
            // 生成 x 轴（月份）和折线图数据
            const xData = [];
            for (let i = 1; i <= 12; i++) {
                xData.push(`${i}月`);
                lineData.push(carbonEmissionItem[`Month${i}`] || 0);
                lineDataTwo.push(quotaTotalItem[`Month${i}`] || 0);
            }

            // 提取 CarbonEmission 系列的月度数据（用于柱状图）
            const barData = [];
            for (let i = 1; i <= 12; i++) {
                barData.push(newVal[`CarbonEmission${i}`] || 0);
            }

            // 存储处理后的数据
            this.totalInfo = {
                xData,
                barData,
                lineData,
                lineDataTwo,
            };
        },

        initChart() {
            const chartDom = document.getElementById('echartsLinkBar');
            if (this.chart) this.chart.dispose(); // 销毁旧图表，避免冲突
            this.chart = echarts.init(chartDom);
            const { xData, barData, lineData, lineDataTwo } = this.totalInfo;
            const option = {
                grid: {
                    left: '0%', // 左边距离
                    right: '0%', // 右边距离
                    bottom: '2%', // 底部距离
                    top: '10%', // 顶部距离
                    containLabel: true, // 包含标签
                },
                legend: {
                    data: ['实际碳排放量', '计划碳排放量', '配额量'],
                    textStyle: {
                        color: 'rgba(255, 255, 255, 1)',
                        fontSize: 12,
                    },
                    itemWidth: 30,
                    itemHeight: 4,
                    right: 0,
                    top: 0,
                },
                xAxis: {
                    type: 'category',
                    data: xData,
                    axisLine: {
                        lineStyle: {
                            color: 'rgba(255,255,255,0.2)',
                            type: 'solid',
                        },
                    },
                    splitLine: {
                        lineStyle: {
                            color: 'rgba(255,255,255,0.2)',
                            type: 'solid',
                            width: 1
                        }
                    },
                    axisLabel: {
                        color: 'rgba(255, 255, 255, 1)',
                        fontSize: 14,
                        align: 'center',
                    },
                    axisTick: {
                        alignWithLabel: true, // 设置刻度线居中
                    },
                },
                yAxis: [
                    {
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
                    }
                ],
                series: [
                    {
                        name: '实际碳排放量',
                        type: 'bar',
                        data: barData,
                        itemStyle: {
                            color: '#3DB98F',
                            borderRadius: [4, 4, 0, 0],
                        },
                        barWidth: 40,
                    },
                    {
                        name: "计划碳排放量",
                        type: "line",
                        data: lineData,
                        itemStyle: {
                            color: "rgb(53, 200, 255)", // 折线颜色，可自定义
                        },
                        lineStyle: {
                            color: "rgb(53, 200, 255)",
                        },
                        symbol: "circle", // 折线点样式
                        symbolSize: 6, // 点大小
                    },
                    {
                        name: "配额量",
                        type: "line",
                        data: lineDataTwo,
                        itemStyle: {
                            color: "#FFC425", // 折线颜色，可自定义
                        },
                        lineStyle: {
                            color: "#FFC425",
                        },
                        symbol: "circle", // 折线点样式
                        symbolSize: 6, // 点大小
                    },
                ],
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    extraCssText: 'text-align: left;',
                    formatter: function (params) {
                        let result = `<div style="display: flex;align-items: center;font-size: 12px;color: #FFFFFF;">
                                            月份：${params[0].name}
                                     </div>`;
                        // 遍历所有系列，展示对应值
                        params.forEach((item) => {
                            result += `<div style="font-size: 12px;color: #FFFFFF;font-weight: 500;">
                                       ${item.marker} ${item.seriesName}：${item.value}
                                    </div>`;
                        });
                        return result;
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
.chartBar{
   width: 100%;
   height: 100%;
   position: relative;
}
.chartBar .chart-container{
    width: 100%;
    height: 100%;
}
.tendency-heard{
    position: absolute;
    top: 0;
    left: 0;
    display: flex;
    align-items: center;
    justify-content: space-between;
    .title{
        font-size: 14px;
        font-weight: 500;
        color: rgba(255, 255, 255, 1);
        position: relative;
        padding-left: 12px;
        >span{
            font-size: 12px;
            font-weight: 500;
            color: rgba(255, 255, 255, 0.6);
            margin-left: 8px;
        }
        &::before{
            content: '';
            position: absolute;
            left: 0;
            top: 50%;
            transform: translateY(-50%);
            width: 4px;
            height: 14px;
            background: #3DB98F;
            border-radius: 2px;
        }
    }
}
</style>