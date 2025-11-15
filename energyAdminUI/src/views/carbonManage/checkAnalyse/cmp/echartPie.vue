<template>
    <div class="echart-pie">
        <div class="heard-title">碳排放类别占比</div>
        <div class="echart-pie-content">
            <div class="echart-pie-content-left">
                <div id="pieChart" style="width: 100%; height: 100%;"></div>
            </div>
            <div class="echart-pie-content-right">
               <div v-for="(carbonValue, className, index) in currentSum.carbonByTopClass" :key="className">
                  <div class="chunk" :style="{ backgroundColor: typeColor[index % typeColor.length] }"></div>
                  <div class="chunk-text">{{ className }}</div>
               </div>
            </div>
        </div>
    </div>
</template>
  
<script>
import * as echarts from 'echarts';
  export default {
    props: {
        typeColor: {
            type: Array,
            default: () => {
                return []
            }
        },
        currentSum: {
            type: Object,
            default: () => {
                return {};
            }
        }
    },
    data() {
        return {
            chart: null,
            timelineData: []
        }
    },
    watch: {
        currentSum:{
            handler(newVal , oldVal){
                (async () => {
                    // 处理新数据为饼图所需格式
                    const pieData = this.processTopClassData(newVal);
                    // 重新渲染图表
                    this.initChart(pieData);
                })();
            },
            deep: true,  //深度监听
        },
    },
    mounted() {
        window.addEventListener('resize', this.resizeChart);
    },
    beforeDestroy() {
        if (this.chart) {
            this.chart.dispose();
        }
        window.removeEventListener('resize', this.resizeChart);
    },
    methods: {
        /**
         * 处理顶层类别数据，转换为echarts所需格式
         * @param {Object} currentSum - 碳排汇总数据
         * @returns {Array} 饼图数据数组，格式: [{name: '类别名', value: 碳排量, color: '颜色'}, ...]
         */
        processTopClassData(currentSum) {
            // 从currentSum中解构出顶层类别数据（默认空对象）
            const { carbonByTopClass = {} } = currentSum;
            
            // 将对象转换为数组格式：
            // 1. Object.entries() 将对象转为 [key, value] 数组
            // 2. filter() 过滤掉碳排量为0的类别（可选，避免饼图显示无意义的0值）
            // 3. map() 转换为echarts所需格式，并分配颜色
            return Object.entries(carbonByTopClass)
                // .filter(([_, value]) => {
                //     // 过滤碳排量>0的类别（如果需要显示0值，可删除此句）
                //     return value > 0;
                // })
                .map(([name, value], index) => {
                    // 为每个类别分配颜色：循环使用typeColor数组（避免颜色数量不足）
                    const color = this.typeColor[index % this.typeColor.length];
                    return { name, value, itemStyle: { color } };
                });
        },

        /**
         * 初始化或更新饼图
         * @param {Array} pieData - 处理后的饼图数据
         */
        initChart(pieData) {
            // 获取饼图DOM容器
            const chartDom = document.getElementById('pieChart');
            if (!chartDom) return; // 容器不存在则退出
            
            // 如果已有图表实例，先销毁（避免重复创建导致性能问题）
            if (this.chart) {
                this.chart.dispose();
            }
            
            // 创建新的echarts实例
            this.chart = echarts.init(chartDom);
            const option = {
                tooltip: {
                    trigger: 'item',
                    // 自定义提示框内容（HTML格式）
                    formatter: (params) => {
                        // params包含当前项的所有数据（name, value, percent等）
                        const { name, value, percent, color } = params;
                        return `
                            <div style="display: flex; align-items: center; margin: 3px 0;">
                                <!-- 小色块（与饼图颜色一致） -->
                                <div style="width: 10px; height: 10px; margin-right: 6px; background: ${color};"></div>
                                <span>${name}</span>
                            </div>
                            <div style="margin: 3px 0;">碳排放量：${value.toFixed(2)} tCO₂</div>
                            <div>占比：${percent.toFixed(2)}%</div>
                        `;
                    },
                    // 提示框样式
                    backgroundColor: 'rgba(17, 25, 40, 0.9)', // 深色背景
                    borderColor: 'rgba(255, 255, 255, 0.1)',
                    borderWidth: 1,
                    textStyle: { color: '#FFFFFF' } // 文字白色
                },
                series: [
                    {
                        name: '碳排放占比',
                        type: 'pie',
                        radius: ['70%', '50%'], // 环形饼图（内半径30%，外半径60%）
                        center: ['50%', '50%'], // 图表在容器中的位置（居中）
                        data: pieData,
                        labelLine: {
                            show: false, // 显示标签线
                            length: 40, // 第一段线长度
                            length2: 8, // 第二段线长度
                            lineStyle: {
                                color: 'rgba(255, 255, 255, 0.6)' // 线颜色（浅白色）
                            }
                        },
                        label: {
                            show: true, // 显示标签
                            position: 'outside', // 标签位置：扇区外侧
                            color: '#FFFFFF', // 标签文字颜色（白色）
                            offset: [10, 0],
                            fontSize: 12,
                            lineHeight: 18, // 行高（用于换行）
                            // 标签内容格式：类别名 + 碳排量 + 占比
                            formatter: (params) => {
                                // 关键：切割“类别X：”前缀，取冒号后面的核心名称
                                const pureName = params.name.split('：')[0] || params.name
                                return `${pureName}：${params.percent.toFixed(2)}%`;
                            }
                        },
                        emphasis: {
                            itemStyle: {
                                shadowBlur: 10, // 阴影模糊度
                                shadowOffsetX: 0, // 阴影X偏移
                                shadowColor: 'rgba(0, 0, 0, 0.5)' // 阴影颜色
                            }
                        }
                    }
                ],
                backgroundColor: 'transparent'
            };
            option && this.chart.setOption(option);
        },
        // 调整图表大小
        resizeChart() {
            if (this.chart) {
                this.chart.resize();
            }
        }
    }
  };
</script>
  
<style lang="less" scoped>
.echart-pie{
    padding: 16px 16px 0;
    height: 100%;
    .heard-title {
        font-weight: 500;
        font-size: 14px;
        color: #FFFFFF;
        position: relative;
        padding-left: 12px;
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
    }
    .echart-pie-content{
        width: 100%;
        display: flex;
        justify-content: space-between;
        height: calc(100% - 20px);
        .echart-pie-content-left{
            width: 60%;
            height: 100%;
        }
        .echart-pie-content-right{
            width: 35%;
            height: 100%;
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            justify-content: center;
            >div{
                width: 100%;
                display: flex;
                align-items: flex-start;
                font-weight: 500;
                font-size: 12px;
                color: rgba(255, 255, 255, 0.6);
                margin-bottom: 20px;
                line-height: 20px;
                >.chunk{
                    width: 12px;
                    height: 12px;
                    margin-right: 8px;
                    margin-top: 3px;
                }
                >.chunk-text{
                    width: calc(100% - 20px);
                }
            }
        }
    }
}
</style>