<template>
    <div class="chartLine-pd">
        <div class='integrative-chart'>
            <div id="chartBar" class="chart-container"></div>
        </div>
    </div>
</template>

<script>
import * as echarts from "echarts";

export default {
    props: {
        barData: {
            type: Array,
            default: () => [],
        },
        unit: {
            type: String,
            default: 'Unit',
        },
        dataType: {
            type: String,
            default: 'dataType',
        },
        colorPool: {
            type: Array,
            default: () => [],
        },
    },
    data() {
        return {
            chart: null,
            // 图表核心数据
            chartData: {
                dates: [], // 所有日期（X轴）
                factors: [], // 所有能源类型（如电力、天然气）
                series: [] // 按能源类型分组的系列数据
            },
            // 动态构建的能源类型映射，包含颜色、名称、单位等
            factorMap: {},
        };
    },
    watch: {
        barData: {
            handler(newVal) {
                this.processDataByDate();
                this.initChart();
                // 移除旧的 resize 监听，避免重复绑定
                window.removeEventListener('resize', this.resizeChart);
                window.addEventListener('resize', this.resizeChart);
            },
            deep: true,
        },
        dataType: {
            handler() {
                this.processDataByDate();
                this.initChart();
            },
            deep: true,
        },
        unit: {
            handler() {
                this.processDataByDate();
                this.initChart();
            },
            deep: true,
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
         * 核心：按 FactorId 分组数据
         * @param {Array} rawData - 原始能源数据
         */
        processDataByDate(rawData) {
            if (!this.barData.length) {
                this.chartData = { dates: [], factors: [], series: [] };
                return;
            }
           // 1. 获取所有唯一日期并排序
            const dates = Array.from(new Set(this.barData.map(item => item.DDate)))
                .sort((a, b) => new Date(a) - new Date(b));
            
            // 2. 动态构建factorMap，提取每个FactorId对应的名称、单位等，并分配颜色
            const factorMap = {};
            let colorIndex = 0;
            this.barData.forEach(item => {
                const factorId = String(item.FactorId);
                if (!factorMap[factorId]) {
                    factorMap[factorId] = {
                        name: item.FactorName || `未知能源${factorId}`,
                        main: this.colorPool[colorIndex % this.colorPool.length],
                        light: `rgba(${this.hexToRgb(this.colorPool[colorIndex % this.colorPool.length])}, 0.2)`
                    };
                    colorIndex++;
                }
            });
            this.factorMap = factorMap;

            // 3. 获取所有唯一能源类型（FactorId）
            const factors = Object.keys(factorMap);
            
            // 新增：按日期汇总数据，将相同日期的数据相加
            const dateDataMap = {};
            this.barData.forEach(item => {
                const date = item.DDate;
                const factorId = String(item.FactorId);
                const rawValue = item[this.dataType];
                if (!dateDataMap[date]) {
                    dateDataMap[date] = {};
                }
                if (!dateDataMap[date][factorId]) {
                    dateDataMap[date][factorId] = 0;
                }
                dateDataMap[date][factorId] += this.formatValueByUnit(rawValue);
            });

            // 4. 按能源类型创建系列数据
            const series = factors.map(factorId => {
                const factorInfo = factorMap[factorId];
                const data = dates.map(date => {
                    return dateDataMap[date] && dateDataMap[date][factorId] ? (dateDataMap[date][factorId]).toFixed(2) : 0;
                });

                return {
                    factorId,
                    name: factorInfo.name,
                    data,
                    mainColor: factorInfo.main,
                    lightColor: factorInfo.light
                };
            });


            this.chartData = { dates, factors, series };
        },

         // 新增：安全解析数值，非数字/'-' 转为 0
        safeParseNumber(value) {
            // 处理 '-' 无数据标识
            if (value === '-' || value === null || value === undefined) {
                return 0;
            }
            // 尝试转为数字，失败则返回 0
            const num = Number(value);
            return isNaN(num) ? 0 : num;
        },

         /**
         * 根据单位转换数据值
         * @param {Number} rawValue - 原始数据值
         * @returns {Number} 转换后的数据值（保留两位小数）
         */
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


        /**
         * 将十六进制颜色转换为rgb格式，用于构建light颜色
         * @param {string} hex 十六进制颜色，如#ff0000或rgb(255,0,0)
         * @returns {string} rgb颜色值，如255,0,0
         */
        hexToRgb(hex) {
            if (hex.startsWith('rgb')) {
                return hex.match(/\d+/g).join(',');
            }
            const r = parseInt(hex.slice(1, 3), 16);
            const g = parseInt(hex.slice(3, 5), 16);
            const b = parseInt(hex.slice(5, 7), 16);
            return `${r},${g},${b}`;
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
                    unit: item.unit,
                    dimensions: ['name', 'value'],
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
                                    fill: item.mainColor
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
                    top: '8%', // 顶部距离
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
                        let content = `<div style="line-height: 1.6;">日期: ${date}</div>`;
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
.chartLine-pd {
    padding-top: 10px;
    height: calc(100% - 10px);
}
.integrative-chart{
    width: 100%;
    height: 100%;
}
.integrative-chart .chart-container{
    width: 100%;
    height: 100%;
}
</style>
