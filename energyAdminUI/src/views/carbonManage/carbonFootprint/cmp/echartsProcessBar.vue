<template>
    <div class="chartBar">
        <div id="echartsProcessBar" class="chart-container"></div>
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
            default: 'process'
        },
    },
    data() {
        return {
            chartProcess: null,
            totalInfo: {
                xData: [],
                processData: []
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
                if (newVal === 'process') {
                   this.initChart();
                }
            }
        },
    },
    beforeDestroy() {
        if (this.chartProcess) {
            this.chartProcess.dispose();
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
            const xData = [];
            const processData = [];

            // 1. 核心步骤：将Processes数组按LinkId数值升序排序
            // - 先过滤无效数据（无LinkId的工序）
            // - 再按LinkId转为数字后比较排序
            const sortedProcesses = Processes
                .filter(process => process.LinkId !== undefined && process.LinkId !== null) // 过滤无LinkId的工序
                .sort((a, b) => {
                    // 将LinkId转为数字（避免字符串排序偏差，如"10"排在"2"前面）
                    const linkIdA = Number(a.LinkId);
                    const linkIdB = Number(b.LinkId);
                    // 升序排序（小的在前，大的在后）
                    return linkIdA - linkIdB;
                });

            // 2. 遍历排序后的Processes，提取xData和processData
            sortedProcesses.forEach(process => {
                const { ProcessName, ProcessItems = [] } = process;
                // 2.1 提取ProcessName作为x轴标签（空值时补默认名称）
                xData.push(ProcessName || `工序（LinkId:${process.LinkId}）`);
                
                // 2.2 计算当前工序的CarbonEmission总和（兼容null/undefined）
                const totalCarbon = ProcessItems.reduce((sum, item) => {
                    return sum + (item.CarbonEmission || 0);
                }, 0);
                
                // 2.3 保留两位小数（四舍五入，转为数字类型）
                processData.push(parseFloat(totalCarbon.toFixed(2)));
            });

            // 更新totalInfo，触发图表刷新
            this.totalInfo = { xData, processData };
        },

        initChart() {
            const chartDom = document.getElementById('echartsProcessBar');
            if (this.chartProcess) this.chartProcess.dispose(); // 销毁旧图表，避免冲突
            this.chartProcess = echarts.init(chartDom);
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
            option && this.chartProcess.setOption(option);
        },
        // 调整图表大小
        resizeChart() {
            if (this.chartProcess) {
                this.chartProcess.resize();
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