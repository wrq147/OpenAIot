<template>
    <div class="chartLine">
        <div class="heard-title">
           <div class="title">碳排放趋势分析 <span>(单位：tCO₂e)</span></div>
           <div class="tabLine">
              <!-- <div>--- <span>计划值</span></div> -->
              <div> <div></div> <span>实际值</span></div>
           </div>
        </div>
        <div class="integrative-chart">
            <div id="chartInter" class="chart-container"></div>
        </div>
    </div>
</template>

<script>
import * as echarts from 'echarts';
import moment from 'moment';
export default {
    props: {
        tableData: {
            type: Array,
            default: () => {
                return [];
            }
        },
    },
    data() {
        return {
            chart: null,
            xData: [],
            yData: [],
            tooltipData: [],    // 提示框显示的日期
            isDataZoom: true,  // 是否显示数据缩放条
            colorList: {
                lineColor: '#3DB98F',
                areaColorTop: 'rgba(61, 185, 143, 0.3)',
                areaColorBom: 'rgba(61, 185, 143, 0.05)'
            }
        };
    },
    watch: {
        tableData: {
            handler(newVal) {
                this.processTableData(newVal);
                this.initChart();
                window.addEventListener('resize', this.resizeChart);
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
        // 处理数据：按日期汇总CarbonEmission
         processTableData(data) {
            // 重置数据
            this.xData = [];
            this.yData = [];
            this.tooltipData = [];
            this.averageValue = 0;

            // 过滤无效数据
            const validData = data.filter(item => 
                item.DDate && item.CarbonEmission !== undefined && item.CarbonEmission !== null
            );

            if (validData.length === 0) {
                this.isDataZoom = false;
                return;
            }

            // 按日期汇总
            const dateCarbonMap = {};
            validData.forEach(item => {
                // 统一日期格式处理
                const dateObj = new Date(item.DDate);
                if (!isNaN(dateObj.getTime())) { // 验证日期有效性
                    const originalDate = moment(dateObj).format('YYYY-MM-DD'); // 完整日期用于去重
                    const carbon = Number(item.CarbonEmission) || 0;
                    
                    // 累加同一天的数据
                    if (dateCarbonMap[originalDate]) {
                        dateCarbonMap[originalDate] += carbon;
                    } else {
                        dateCarbonMap[originalDate] = carbon;
                    }
                }
            });

            // 排序并提取数据
            const sortedDateArr = Object.entries(dateCarbonMap)
                .sort((a, b) => new Date(a[0]) - new Date(b[0]));
            
            // 处理格式化后的数据
            sortedDateArr.forEach(([originalDate, value]) => {
                this.xData.push(moment(originalDate).format('MM/DD')); // 格式化为MM/DD
                this.yData.push(value.toFixed(2));
                this.tooltipData.push(originalDate); // 保留完整日期用于提示框
            });

            // 计算统计值
            if (this.yData.length > 0) {
                const numData = this.yData.map(Number);
                this.averageValue = (numData.reduce((a, b) => a + b, 0) / numData.length).toFixed(2);
            }

            // 当数据点超过7个时显示数据缩放条
            this.isDataZoom = this.xData.length > 7;
        },
        initChart() {
            const chartDom = document.getElementById('chartInter');
            if (!chartDom) return;

            // 销毁旧实例
            if (this.chart) this.chart.dispose();
            this.chart = echarts.init(chartDom);
            const option = {
                grid: {
                    left: '1%', // 左边距离
                    right: '4%', // 右边距离
                    bottom: '18%', // 底部距离
                    top: '10%', // 顶部距离
                    containLabel: true, // 包含标签
                },
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    formatter: (params) => {
                        const idx = params[0].dataIndex;
                        return `
                            <div style="font-size:14px; line-height:22px; padding:8px 12px;">
                                日期: ${this.tooltipData[idx]}<br>
                                '碳排放量': ${params[0].value}
                            </div>
                        `;
                    },
                    backgroundColor: 'rgba(18, 19, 27, 0.95)',
                    borderColor: '#3DB98F',
                    borderWidth: 1
                },
                xAxis: {
                    type: 'category',
                    data: this.xData,
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
                        fontSize: 12,
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
                        fontSize: 12,
                        align: 'right', // 设置刻度标签右对齐
                    },
                },
                dataZoom: [
                    {
                        type: 'slider', // 滑动条模式
                        show: true, // 显示拖拉条
                        xAxisIndex: 0, // 关联 x 轴
                        color: 'rgba(255, 255, 255, 1)',
                        start: 10, // 初始缩放起始比例（%）
                        end: 90, // 初始缩放结束比例（%）
                        handleStyle: {
                            color: 'rgba(255, 255, 255, 1)', // 手柄颜色
                            borderColor: 'rgba(255, 255, 255, 1)', // 手柄边框颜色
                            borderWidth: 1,
                            shadowBlur: 2,
                            shadowColor: 'rgba(0, 0, 0, 0.3)',
                            shadowOffsetX: 1,
                            shadowOffsetY: 1
                        },
                        backgroundStyle: {
                            color: 'rgba(200, 200, 200, 0.2)', // 背景颜色
                            borderColor: 'rgba(200, 200, 200, 0.5)', // 背景边框颜色
                            borderWidth: 1,
                            shadowBlur: 2,
                            shadowColor: 'rgba(0, 0, 0, 0.3)',
                            shadowOffsetX: 1,
                            shadowOffsetY: 1
                        },
                        fillerStyle: {
                            color: 'rgba(255, 255, 255, 0.3)', // 填充颜色
                            borderColor: 'rgba(255, 255, 255, 0.5)', // 填充边框颜色
                            borderWidth: 1
                        },
                        dataBackground: {
                            lineStyle: {
                                color: 'rgba(255, 255, 255, 0.8)', // 线条颜色
                                width: 1
                            },
                            areaStyle: {
                                color: 'rgba(255, 255, 255, 0.2)' // 区域颜色
                            }
                        }
                    }
                ],
                series: [
                    {
                        data: this.yData,
                        type: 'line',
                        smooth: true,
                        symbol: 'circle',
                        itemStyle: {
                            color: this.colorList.lineColor,
                            borderColor: '#fff',
                            borderWidth: 2
                        },
                        lineStyle: {
                            color: this.colorList.lineColor,
                            width: 2 // 折线宽度
                        },
                        // markLine: {
                        //     data: [{ type: 'average', name: '平均值', yData: this.averageValue }],
                        // },
                    },
                ]
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
.chartLine {
    padding: 16px 16px 0;
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
