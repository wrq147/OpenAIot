<template>
    <div class="pie3d-chart-container">
        <div id="pie3dChart" class="chart-container"></div>
        <div class="summation"> 合计：<span>{{ calculateTotal() }}</span></div>
    </div>
</template>
<script>
import * as echarts from 'echarts';
export default {
    name: 'EchartPie',
    props: {
        pieData: {
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
    watch: {
        pieData: {
            handler() {
                this.initChart();
                // 移除旧的 resize 监听，避免重复绑定
                window.removeEventListener('resize', this.resizeChart);
                window.addEventListener('resize', this.resizeChart);
            },
            deep: true,
            immediate: false,
        },
        dataType: {
            handler() {
                this.initChart();
            },
            deep: true,
        },
        unit: {
            handler() {
                this.initChart();
            },
            deep: true,
        },
    },
    data() {
        return {
           myChart: null,
           option: null, // 存储图表配置（用于交互更新）
           factorColorMap: {}
        };
    },
     beforeDestroy() {
        if (this.myChart) {
            this.myChart.dispose();
        }
        window.removeEventListener('resize', this.resizeChart);
    },
    methods: {
        initChart() {
            if (!this.pieData.length) return;
            this.myChart = echarts.init(document.getElementById('pie3dChart'));
            // 处理数据 + 生成配置项
            const pieSeriesData = this.processPieData(); 
              this.option = {
                tooltip: {
                    trigger: 'item',
                    // 自定义 tooltip 内容
                    formatter: (params) => {
                        const { name, value, percent } = params;
                        return `
                        <div style="color:#fff;">
                            <div>${name}</div>
                            <div>数值：${value.toFixed(2)}</div>
                            <div>占比：${percent.toFixed(2)}%</div>
                        </div>
                        `;
                    },
                },
                series: [
                    {
                        type: 'pie', 
                        radius: ['40%', '70%'], // 环形：内半径 40%，外半径 70%
                        // center: ['50%', '50%'], // 饼图位置（左侧留出空间给右侧标注）
                        startAngle: -10 ,
                        data: pieSeriesData,
                        avoidLabelOverlap: false,
                        itemStyle: {
                            borderRadius: 10,
                            borderColor: 'rgba(255,255,255,0.1)',
                            borderWidth: 2
                        },
                        labelLine: {
                            show: true,
                            lineStyle: {
                                color: '#7BC0CB'
                            }
                        },
                        label: {
                            show: true,
                            position: 'outside',
                            rich: {
                                square: {
                                    fontSize: 14, // 控制方块大小
                                    marginTop: 2, // 方块与文字间距
                                    verticalAlign: 'top'
                                },
                                b: {
                                    color: 'rgba(255,255,255,0.6)',
                                    fontSize: 12,
                                    lineHeight: 20
                                },
                                c: {
                                    color: '#FFFFFF',
                                    fontSize: 12,
                                    marginRight: 10
                                },
                            },
                            formatter: function(params) {
                                console.log(params)
                                // 1. 获取转换后的当前值（避免直接用 params.value，防止未转换）
                                const formattedValue = this.formatValueByUnit(params.value);
                                // 2. 获取所有转换后的 data（用于计算占比）
                                const allRawData = this.option.series[0].data; // 因为只有一个 pie series
                                const formattedAllData = allRawData.map(item => this.formatValueByUnit(item.value));
  
                                // 3. 计算总和
                                const total = formattedAllData.reduce((sum, val) => sum + (val || 0), 0);
                                // 4. 计算占比（基于转换后的数据）
                                const ratio = total > 0 ? ((formattedValue / total) * 100).toFixed(2) : '0.00';
                                const bfs = `${ratio}%`;

                                // 颜色方块逻辑（保持不变）
                                const color = params.color || '#cccccc';
                                const squareChar = '■';

                                // 最终标签内容：显示转换后的值
                                return `{square|${squareChar}} {b|${params.name}\n}{c|${formattedValue}  ${bfs}}`;
                            }.bind(this) // 绑定组件this上下文
                        },
                    },
                ],
                // 右侧富文本标注（天然气、电的名称 + 数值 + 占比）
                // graphic: this.buildRightLabels(pieSeriesData), 
            };
            this.myChart.setOption(this.option);
        },
        // 处理能源数据：将 pieData 转换为饼图所需格式
        processPieData() {
            // 过滤无效数据（值为 "-"、空、非数字）
            const validData = this.pieData.filter((item) => {
                const targetValue = item[this.dataType];
                return (
                    targetValue !== '-' &&
                    targetValue !== '' &&
                    targetValue != null &&
                    !isNaN(Number(targetValue))
                );
            });

            // 转换为 ECharts 所需格式
            return validData.map((item) => ({
                name: item.FactorName || `未知能源(${item.FactorId})`,
                value: this.formatValueByUnit(Number(item[this.dataType])), 
                itemStyle: {
                    color: this.getColorByFactorId(item.FactorId), 
                },
                originalData: item, 
            }));
        },

        // 生成右侧富文本标注（天然气、电的名称 + 数值 + 占比）
        buildRightLabels(pieData) {
            if (!pieData.length) return [];
            // 计算总和（用于算占比）
            const total = pieData.reduce((sum, item) => sum + item.value, 0); 
            return pieData.map((item, index) => {
                const percent = total > 0 ? ((item.value / total) * 100).toFixed(2) : '0.00';
                // 每个指标的位置（垂直排列）
                const y = 30 + index * 50; 
                return {
                    type: 'text',
                    left: '60%', // 固定在右侧
                    top: y + '%',
                    style: {
                        fontSize: 14,
                        fill: '#fff',
                        rich: {
                        // 富文本样式：标题、数值、占比区分
                        title: { color: '#fff', fontWeight: 'bold' },
                        value: { color: '#00bfff' },
                        percent: { color: '#ffd700' },
                        },
                    },
                    // 拼接富文本内容
                    content: [
                        `{title|${item.name}}`,
                        `数值：{value|${item.value.toFixed(2)}}`,
                        `占比：{percent|${percent}%}`,
                    ].join('\n'),
                };
            });
        },

        /**
         * 根据单位转换数据值
         * @param {Number} rawValue - 原始数据值
         * @returns {Number} 转换后的数据值（保留两位小数）
         */
        formatValueByUnit(rawValue) {
            if (this.unit === 'LageUnit') {
                // LageUnit：除以10000 + 保留两位小数（四舍五入）
                return (rawValue / 10000) * 100 / 100;
            }
            // 其他单位：直接返回原始值（如需保留小数可统一处理，如 toFixed(2)）
            return rawValue;
        },
        // 根据 FactorId 映射颜色（可从 colorPool 取色）
        getColorByFactorId(factorId) {
            // 如果未缓存，从 colorPool 取色并缓存
            if (!this.factorColorMap[factorId]) {
                // 用已缓存的 factor 数量来循环取色
                const colorIndex = Object.keys(this.factorColorMap).length % this.colorPool.length; 
                this.factorColorMap[factorId] = this.colorPool[colorIndex] || '#ccc';
            }
            return this.factorColorMap[factorId];
        },

        // 计算合计值（根据 dataType 对应的值求和）
        calculateTotal() {
            // 1. 先判断 pieData 是否为有效数组
            if (!Array.isArray(this.pieData) || this.pieData.length === 0) {
                return 0; // 空数组时返回 "-"（也可根据需求改为 0）
            }

            // 2. 累加前处理每个值：将 "-"、空字符串等转为 0，确保累加有效
            const total = this.pieData.reduce((sum, item) => {
                // 获取当前数据项的目标值（如 UseVale/CostVale 等）
                const rawValue = item[this.dataType];
                
                // 核心：处理非数字值（"-"、空字符串、undefined 等）→ 转为 0
                if (rawValue === "-" || rawValue === "" || rawValue == null) {
                    return sum + 0;
                }

                // 转换为数字并按单位处理（若 formatValueByUnit 未处理非数字，这里补充容错）
                const formattedValue = this.formatValueByUnit(Number(rawValue) || 0);
                
                // 确保累加的是有效数字（避免极端情况如 NaN）
                return sum + (isNaN(formattedValue) ? 0 : formattedValue);
            }, 0);

            // 3. 处理合计结果：若总和为 0（可能是所有值都为 "-" 或 0），返回 "-"；否则返回保留 2 位小数的数字
            return total === 0 ? total : Math.round(total * 100) / 100;
        },

        // 调整图表大小
        resizeChart() {
            if (this.myChart) {
                this.myChart.resize();
            }
        },

    }
}
</script>
<style lang="less" scoped>
.pie3d-chart-container{
    margin-top: -20px;
    height: calc(100% - 10px);
    position: relative;
}
.pie3d-chart-container .chart-container{
    width: 100%;
    height: 100%;
}
.pie3d-chart-container .summation{
    position: absolute;
    bottom: -30px;
    right: 50%;
    transform: translateX(50%);
    font-weight: 500;
    font-size: 14px;
    color: rgba(255,255,255,0.6);
    >span{
        color: #fff;
        margin-left: 6px;
    }
}
</style>