<template>
    <div class="chartBar">
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
        dataType: {
            type: String,
            default: 'link',
        },
    },
    data() {
        return {
            chart: null,
            totalInfo: {
                xData: [],
                processData: [],
                linkIds: []
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
        },
        dataType: {
            handler(newVal) {
                if (newVal === 'link') {
                    this.initChart();
                }
            },
        },
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
            const { Processes = [] } = newVal;
            // 1. 第一步：按LinkId分组，计算每个LinkId的总碳排放量和对应的LinkName
            const linkCarbonMap = {}; // key: LinkId（字符串转数字）, value: { linkName, totalCarbon }
            Processes.forEach(process => {
                const { LinkId, LinkName, ProcessItems = [] } = process;
                // 关键：将LinkId转为数字（避免字符串排序问题，如"10"排在"2"前面）
                const linkIdNum = Number(LinkId);
                if (isNaN(linkIdNum)) return; // 过滤无效LinkId（非数字）

                // 计算当前工序的碳排放量总和
                const processTotal = ProcessItems.reduce((sum, item) => {
                    return sum + (item.CarbonEmission || 0);
                }, 0);

                // 按LinkId分组累加
                if (linkCarbonMap[linkIdNum]) {
                    // 已有该LinkId：累加碳排放量（LinkName沿用首次出现的）
                    linkCarbonMap[linkIdNum].totalCarbon += processTotal;
                } else {
                    // 首次出现该LinkId：初始化LinkName和总碳排放量
                    linkCarbonMap[linkIdNum] = {
                        linkName: LinkName || `环节${linkIdNum}`, // 空名称时补默认值
                        totalCarbon: processTotal
                    };
                }
            });

            // 2. 第二步：提取所有LinkId并按数值大小排序（核心：实现LinkId顺序）
            // 从map中获取所有LinkId（数字类型），并按升序排序
            const sortedLinkIds = Object.keys(linkCarbonMap)
                .map(key => Number(key)) // 确保是数字类型
                .sort((a, b) => a - b); // 数值升序排序（1→3→4）

            // 3. 第三步：按排序后的LinkId提取xData和processData
            const xData = [];
            const processData = [];
            const linkIds = []; // 存储排序后的LinkId（数字类型，可选）
            sortedLinkIds.forEach(linkId => {
                const { linkName, totalCarbon } = linkCarbonMap[linkId];
                linkIds.push(linkId); // 排序后的LinkId
                xData.push(linkName); // 排序后的环节名称
                processData.push(parseFloat(totalCarbon.toFixed(2))); // 排序后的碳排放量（保留两位小数）
            });
            // 更新totalInfo（顺序已按LinkId数值排序）
            this.totalInfo = { linkIds, xData, processData };
        },

        initChart() {
            const chartDom = document.getElementById('echartsLinkBar');
            if (this.chart) this.chart.dispose(); // 销毁旧图表，避免冲突
            this.chart = echarts.init(chartDom);
            const { xData, processData } = this.totalInfo;
            const { OutPut, Unit } = this.modelData;
            const option = {
                grid: {
                    left: '0%', // 左边距离
                    right: '0%', // 右边距离
                    bottom: '2%', // 底部距离
                    top: '3%', // 顶部距离
                    containLabel: true, // 包含标签
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
                            type: 'solid', // 关键配置：设置为虚线
                            width: 1 // 线宽
                        }
                    },
                    axisLabel: {
                        color: 'rgba(255, 255, 255, 1)',
                        fontSize: 14,
                        align: 'center', // 设置刻度标签右对齐
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
                        name: '碳排放量',
                        type: 'bar',
                        data: processData,
                        itemStyle: {
                            color: '#3DB98F',
                            borderRadius: [4, 4, 0, 0],
                        },
                        barWidth: 40,
                    },
                ],
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    extraCssText: 'text-align: left;',
                    formatter: function (params) {
                        let result = `
                            <div style="display: flex;align-items: center;font-size: 12px;color: #FFFFFF;">
                                <span style="width: 12px;height: 12px;background: #2AD39A;margin-right:8px"></span>
                                环节：${params[0].name}
                            </div>
                            <div style="font-size: 12px;color: #FFFFFF;font-weight: 500;">
                                碳排放量：${params[0].value} <span> kgCO₂e/${OutPut}${Unit}</span>
                            </div>
                        `;
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
}
.chartBar .chart-container{
    width: 100%;
    height: 100%;
}
</style>