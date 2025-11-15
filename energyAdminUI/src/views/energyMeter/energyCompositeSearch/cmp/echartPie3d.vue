<template>
    <div class="pie3d-chart-container">
        <div id="pie3dChart" class="chart-container"></div>
        <div class="summation"> 合计：<span>{{ calculateTotal() }}</span></div>
    </div>
</template>
<script>
import * as echarts from 'echarts';
import 'echarts-gl'; // 引入 ECharts GL 扩展，使能 3D 功能
export default {
    name: 'EchartPie3d',
    props: {
        pieData: { type: Array, default: () => [] },
        unit: { type: String, default: 'Unit' },
        dataType: { type: String, default: 'dataType' },
        colorPool: { type: Array, default: () => [] },
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
        // 获取数据
        initChart() {
            // 1. 销毁旧图表（避免重复渲染导致内存泄漏）
            if (this.myChart) this.myChart.dispose();

            // 2. 处理能源数据（过滤无效数据 + 适配 3D 格式）
            const processedData = this.processPieData();
            if (!processedData.length) {
                console.warn('无有效能源数据（数值需>0），无法渲染图表');
                return;
            }
            this.myChart = echarts.init(document.getElementById('pie3dChart'));
            this.option = this.getPie3D(processedData, 0.8);
			this.myChart.setOption(this.option);
            this.option.series.push({
                name: 'pie2d',
                type: 'pie',
                labelLine:{
                    length: 50,
                    length2: 30
                },
                startAngle: -10 , //起始角度，支持范围[0, 360]。
                clockwise: false,//饼图的扇区是否是顺时针排布。上述这两项配置主要是为了对齐3d的样式
                radius: ['20%', '50%'],
                center: ['50%', '50%'],
                data: processedData,
                itemStyle:{
                    opacity:0,
                }
            });
            this.myChart.setOption(this.option);
            this.bindListen(this.myChart);
        },
        // 根据factorId动态分配颜色（同一id始终对应同一颜色）
        getColorByFactorId(factorId) {
            // 如果已存在映射，直接返回（保证同一factorId颜色不变）
            if (this.factorColorMap[factorId]) {
                return this.factorColorMap[factorId];
            }
            // 计算当前应分配的颜色索引：根据已有映射的数量，从0开始依次递增，超出颜色池长度则循环
            const currentIndex = Object.keys(this.factorColorMap).length % this.colorPool.length;
            
            // 获取对应的颜色（从颜色池第一个开始按顺序使用）
            const color = this.colorPool[currentIndex];
            
            // 记录映射关系
            this.factorColorMap[factorId] = color;
            
            return color;
        },
        // 处理能源数据：将 pieData 转换为 3D 饼图所需格式
        processPieData() {
            // 过滤逻辑：只保留 dataType 对应值不是 "-"、空字符串且为数字的数据
            const validData = this.pieData.filter(item => {
                const targetValue = item[this.dataType];
                // 排除 "-"、空字符串、null/undefined，且确保是数字
                return targetValue !== "-" && targetValue !== "" && targetValue != null && !isNaN(Number(targetValue));
            });

            // 转换为图表所需格式（仅处理有效数据）
            return validData.map(item => ({
                name: item.FactorName || `未知能源(${item.FactorId})`,
                value: this.formatValueByUnit(Number(item[this.dataType])), // 强制转为数字
                itemStyle: {
                    color: this.getColorByFactorId(item.FactorId)
                },
                originalData: item
            }));
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

        getPie3D(pieData, internalDiameterRatio) {
            //internalDiameterRatio:透明的空心占比
            let that = this;
            let series = [];
            let sumValue = 0;
            let startValue = 0;
            let endValue = 0;
            let legendData = [];
            let legendBfb = [];
            let k = 1 - internalDiameterRatio;
            pieData.sort((a, b) => {
                return (b.value - a.value);
            });
            // 为每一个饼图数据，生成一个 series-surface 配置
            for (let i = 0; i < pieData.length; i++) {
                sumValue += pieData[i].value;
                let seriesItem = {
                    name: typeof pieData[i].name === 'undefined' ? `series${i}` : pieData[i].name,
                    type: 'surface',
                    parametric: true,
                    wireframe: {
                        show: false
                    },
                    pieData: pieData[i],
                    pieStatus: {
                        selected: false,
                        hovered: false,
                        k: k
                    },
                    center: ['10%', '50%']
                };

                if (typeof pieData[i].itemStyle != 'undefined') {
                    let itemStyle = {};
                    typeof pieData[i].itemStyle.color != 'undefined' ? itemStyle.color = pieData[i].itemStyle.color : null;
                    typeof pieData[i].itemStyle.opacity != 'undefined' ? itemStyle.opacity = pieData[i].itemStyle.opacity : null;
                    seriesItem.itemStyle = itemStyle;
                }
                series.push(seriesItem);
            }

            // 使用上一次遍历时，计算出的数据和 sumValue，调用 getParametricEquation 函数，
            // 向每个 series-surface 传入不同的参数方程 series-surface.parametricEquation，也就是实现每一个扇形。
            legendData = [];
            legendBfb = [];
            for (let i = 0; i < series.length; i++) {
                endValue = startValue + series[i].pieData.value;
                series[i].pieData.startRatio = startValue / sumValue;
                series[i].pieData.endRatio = endValue / sumValue;
                series[i].parametricEquation = this.getParametricEquation(series[i].pieData.startRatio, series[i].pieData.endRatio,
                    false, false, k, series[i].pieData.value);
                startValue = endValue;
                let bfb = that.fomatFloat(series[i].pieData.value / sumValue, 4);
                legendData.push({
                    name: series[i].name,
                    value: bfb
                });
                legendBfb.push({
                    name: series[i].name,
                    value: bfb
                });
            }
            let boxHeight = this.getHeight3D(series, 16); //通过传参设定3d饼/环的高度，10代表10px
            // 准备待返回的配置项，把准备好的 legendData、series 传入。
            let option = {
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
                        // 1. 获取转换后的当前值（避免直接用 params.value，防止未转换）
                        const formattedValue = this.formatValueByUnit(params.value);
                        // 2. 获取所有转换后的 data（用于计算占比）
                        const allData = this.option.series.find(s => s.name === 'pie2d')?.data || [];
                        const formattedAllData = allData.map(item => this.formatValueByUnit(item.value));
                        // 3. 计算转换后的总和（避免占比计算错误）
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
                tooltip: {
                    formatter: params => {
                       if (params.seriesName !== 'mouseoutSeries' && params.seriesName !== 'pie2d') {
                            // 1. 获取转换后的当前值
                            const formattedValue = this.formatValueByUnit(option.series[params.seriesIndex].pieData.value);
                            // 2. 计算占比（基于转换后的总和）
                            const total = this.pieData.reduce((sum, item) => {
                                return sum + this.formatValueByUnit(item[this.dataType]);
                            }, 0);
                            const bfb = total > 0 ? ((formattedValue / total) * 100).toFixed(2) : '0.00';
                            
                            // 3. 显示转换后的值 + 占比
                            return `<span style="display:inline-block;margin-right:5px;border-radius:10px;width:10px;height:10px;background-color:${params.color};"></span>` +
                                `${params.seriesName}<br/>` +
                                `数值：${formattedValue}<br/>` + 
                                `占比：${bfb}%`;
                        }
                    }
                },
                xAxis3D: {
                    min: -1,
                    max: 1
                },
                yAxis3D: {
                    min: -1,
                    max: 1
                },
                zAxis3D: {
                    min: -1,
                    max: 1
                },
                grid3D: {
                    show: false,
                    boxHeight: boxHeight, //圆环的高度
                    boxWidth: 180,
                    boxDepth: 180,
                    disableDepthTest: true,
                    viewControl: { //3d效果可以放大、旋转等，请自己去查看官方配置
                        alpha: 30, //角度
                        distance: 260,//调整视角到主体的距离，类似调整zoom
                        rotateSensitivity: 0, //设置为0无法旋转
                        zoomSensitivity: 0, //设置为0无法缩放
                        panSensitivity: 0, //设置为0无法平移
                        autoRotate: false //自动旋转
                    }
                },
                series: series
            };
            return option;
        },

        //获取3d丙图的最高扇区的高度
        getHeight3D(series, height) {
            series.sort((a, b) => {
                return (b.pieData.value - a.pieData.value);
            })
            return height * 25 / series[0].pieData.value;
        },

        // 生成扇形的曲面参数方程，用于 series-surface.parametricEquation
        getParametricEquation(startRatio, endRatio, isSelected, isHovered, k, h) {
            // 计算
            let midRatio = (startRatio + endRatio) / 2;
            let startRadian = startRatio * Math.PI * 2;
            let endRadian = endRatio * Math.PI * 2;
            let midRadian = midRatio * Math.PI * 2;
            // 如果只有一个扇形，则不实现选中效果。
            if (startRatio === 0 && endRatio === 1) {
                isSelected = false;
            }
            // 通过扇形内径/外径的值，换算出辅助参数 k（默认值 1/3）
            k = typeof k !== 'undefined' ? k : 1 / 3;
            // 计算选中效果分别在 x 轴、y 轴方向上的位移（未选中，则位移均为 0）
            let offsetX = isSelected ? Math.cos(midRadian) * 0.1 : 0;
            let offsetY = isSelected ? Math.sin(midRadian) * 0.1 : 0;
            // 计算高亮效果的放大比例（未高亮，则比例为 1）
            let hoverRate = isHovered ? 1.05 : 1;
            // 返回曲面参数方程
            return {
                u: {
                    min: -Math.PI,
                    max: Math.PI * 3,
                    step: Math.PI / 32
                },
                v: {
                    min: 0,
                    max: Math.PI * 2,
                    step: Math.PI / 20
                },
                x: function(u, v) {
                    if (u < startRadian) {
                        return offsetX + Math.cos(startRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    if (u > endRadian) {
                        return offsetX + Math.cos(endRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    return offsetX + Math.cos(u) * (1 + Math.cos(v) * k) * hoverRate;
                },
                y: function(u, v) {
                    if (u < startRadian) {
                        return offsetY + Math.sin(startRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    if (u > endRadian) {
                        return offsetY + Math.sin(endRadian) * (1 + Math.cos(v) * k) * hoverRate;
                    }
                    return offsetY + Math.sin(u) * (1 + Math.cos(v) * k) * hoverRate;
                },
                z: function(u, v) {
                    if (u < -Math.PI * 0.5) {
                        return Math.sin(u);
                    }
                    if (u > Math.PI * 2.5) {
                        return Math.sin(u) * h * .1;
                    }
                    return Math.sin(v) > 0 ? 1 * h * .1 : -1;
                }
            };
        },

        fomatFloat(num, n) {
            var f = parseFloat(num);
            if (isNaN(f)) {
                return false;
            }
            f = Math.round(num * Math.pow(10, n)) / Math.pow(10, n); // n 幂   
            var s = f.toString();
            var rs = s.indexOf('.');
            //判定如果是整数，增加小数点再补0
            if (rs < 0) {
                rs = s.length;
                s += '.';
            }
            while (s.length <= rs + n) {
                s += '0';
            }
            return s;
        },
        
        bindListen(myChart) {
            // 监听鼠标事件，实现饼图选中效果（单选），近似实现高亮（放大）效果。
            let that = this;
            let selectedIndex = '';
            let hoveredIndex = '';
            // 监听点击事件，实现选中效果（单选）
            myChart.on('click', function(params) {
                // 从 option.series 中读取重新渲染扇形所需的参数，将是否选中取反。
                let isSelected = !that.option.series[params.seriesIndex].pieStatus.selected;
                let isHovered = that.option.series[params.seriesIndex].pieStatus.hovered;
                let k = that.option.series[params.seriesIndex].pieStatus.k;
                let startRatio = that.option.series[params.seriesIndex].pieData.startRatio;
                let endRatio = that.option.series[params.seriesIndex].pieData.endRatio;
                // 如果之前选中过其他扇形，将其取消选中（对 option 更新）
                if (selectedIndex !== '' && selectedIndex !== params.seriesIndex) {
                    that.option.series[selectedIndex].parametricEquation = that.getParametricEquation(that.option.series[
                            selectedIndex].pieData
                        .startRatio, that.option.series[selectedIndex].pieData.endRatio, false, false, k, that.option.series[
                            selectedIndex].pieData
                        .value);
                    that.option.series[selectedIndex].pieStatus.selected = false;
                }
                // 对当前点击的扇形，执行选中/取消选中操作（对 option 更新）
                that.option.series[params.seriesIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                    isSelected,
                    isHovered, k, that.option.series[params.seriesIndex].pieData.value);
                that.option.series[params.seriesIndex].pieStatus.selected = isSelected;
                // 如果本次是选中操作，记录上次选中的扇形对应的系列号 seriesIndex
                isSelected ? selectedIndex = params.seriesIndex : null;
                // 使用更新后的 option，渲染图表
                myChart.setOption(that.option);
            });

            // 监听 mouseover，近似实现高亮（放大）效果
            myChart.on('mouseover', function(params) {
                // 准备重新渲染扇形所需的参数
                let isSelected;
                let isHovered;
                let startRatio;
                let endRatio;
                let k;
                // 如果触发 mouseover 的扇形当前已高亮，则不做操作
                if (hoveredIndex === params.seriesIndex) {
                    return;
                    // 否则进行高亮及必要的取消高亮操作
                } else {
                    // 如果当前有高亮的扇形，取消其高亮状态（对 option 更新）
                    if (hoveredIndex !== '') {
                        // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 false。
                        isSelected = that.option.series[hoveredIndex].pieStatus.selected;
                        isHovered = false;
                        startRatio = that.option.series[hoveredIndex].pieData.startRatio;
                        endRatio = that.option.series[hoveredIndex].pieData.endRatio;
                        k = that.option.series[hoveredIndex].pieStatus.k;
                        // 对当前点击的扇形，执行取消高亮操作（对 option 更新）
                        that.option.series[hoveredIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                            isSelected,
                            isHovered, k, that.option.series[hoveredIndex].pieData.value);
                        that.option.series[hoveredIndex].pieStatus.hovered = isHovered;
                        // 将此前记录的上次选中的扇形对应的系列号 seriesIndex 清空
                        hoveredIndex = '';
                    }
                    // 如果触发 mouseover 的扇形不是透明圆环，将其高亮（对 option 更新）
                    if (params.seriesName !== 'mouseoutSeries' && params.seriesName !== 'pie2d') {
                        // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 true。
                        isSelected = that.option.series[params.seriesIndex].pieStatus.selected;
                        isHovered = true;
                        startRatio = that.option.series[params.seriesIndex].pieData.startRatio;
                        endRatio = that.option.series[params.seriesIndex].pieData.endRatio;
                        k = that.option.series[params.seriesIndex].pieStatus.k;
                        // 对当前点击的扇形，执行高亮操作（对 option 更新）
                        that.option.series[params.seriesIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                            isSelected, isHovered, k, that.option.series[params.seriesIndex].pieData.value + 5);
                        that.option.series[params.seriesIndex].pieStatus.hovered = isHovered;
                        // 记录上次高亮的扇形对应的系列号 seriesIndex
                        hoveredIndex = params.seriesIndex;
                    }
                    // 使用更新后的 option，渲染图表
                    myChart.setOption(that.option);
                }
            });
            // 修正取消高亮失败的 bug
            myChart.on('globalout', function() {
                // 准备重新渲染扇形所需的参数
                let isSelected;
                let isHovered;
                let startRatio;
                let endRatio;
                let k;
                if (hoveredIndex !== '') {
                    // 从 option.series 中读取重新渲染扇形所需的参数，将是否高亮设置为 true。
                    isSelected = that.option.series[hoveredIndex].pieStatus.selected;
                    isHovered = false;
                    k = that.option.series[hoveredIndex].pieStatus.k;
                    startRatio = that.option.series[hoveredIndex].pieData.startRatio;
                    endRatio = that.option.series[hoveredIndex].pieData.endRatio;
                    // 对当前点击的扇形，执行取消高亮操作（对 option 更新）
                    that.option.series[hoveredIndex].parametricEquation = that.getParametricEquation(startRatio, endRatio,
                        isSelected,
                        isHovered, k, that.option.series[hoveredIndex].pieData.value);
                    that.option.series[hoveredIndex].pieStatus.hovered = isHovered;
                    // 将此前记录的上次选中的扇形对应的系列号 seriesIndex 清空
                    hoveredIndex = '';
                }
                // 使用更新后的 option，渲染图表
                myChart.setOption(that.option);
            });
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
    height: calc(100% - 20px);
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