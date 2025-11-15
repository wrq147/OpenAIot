<template>
    <div class="chartBar"  :style="{'height':tableConHeight-352+'px'}">
        <div class="integrative-chart"  :style="{'height':tableConHeight-352+'px'}">
            <div id="chartBar" class="chart-container"  :style="{'height':tableConHeight-352+'px'}"></div>
        </div>
    </div>
</template>

<script>
import * as echarts from 'echarts';
var dayjs = require("@/utils/day.js");
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
    name:'echartsBar3d',
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
        dataTitle:{
            type:String,
            default:'能源消耗'
        },
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
                lineColor: '#D97965',
                areaColorTop: 'rgba(255, 137, 109, 0.80)',
                areaColorBom: 'rgba(255, 137, 109, 0.40)'
            },{
                lineColor: '#D2A855',
                areaColorTop: 'rgba(255, 199, 90, 1)',
                areaColorBom: 'rgba(255, 199, 90, 0.50)'
            },{
                lineColor: '#31A9D8',
                areaColorTop: 'rgba(53, 200, 255, 1)',
                areaColorBom: 'rgba(53, 200, 255, 0.50)'
            },{
                lineColor: '#28B288',
                areaColorTop: 'rgba(42, 211, 154, 0.80)',
                areaColorBom: 'rgba(42, 211, 154, 0.40)'
            }],
            chartData:[],
            colorPool: ['rgba(255, 137, 109, 1)', 'rgba(255, 199, 90, 1)', 'rgba(54, 183, 231, 1)', 'rgba(42, 211, 154, 1)'],
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
                this.loadChart(newVal)
                window.addEventListener('resize', this.resizeChart);
                
            },
            deep: true,
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
            try {
                this.processTableData(newVal);
                this.initChart();
            } catch (error) {
               console.log(error,'error'); 
            }
        },
        // 处理数据：按日期汇总CarbonEmission
        processTableData(barData) {
            if (!barData.length) {
                this.chartData = { dates: [], series: [] };
                return;
            }
            const barWidth = this.transformFontSize(10);
            let series=[]
            let beforeBottomData=[]
            let seriesarr=barData.map((row,inx)=>{
                let Details=JSON.parse(JSON.stringify(row.Details))
                // Details=Details.filter(rs=>Number(rs[this.dataType])>0)
                Details.sort((a,b)=>Number(a.TTime)-Number(b.TTime))
                // console.log(Details,'DetailsDetails');
                let minVal=Details.map(r=>Number(r.TTime))
                let minData=minVal&&minVal.length>0?Math.min(...minVal):0
                // console.log(minVal,'minValminVal',minData);
                let sideData=Details.map(ro=>[ro.TTime,ro[this.dataType]])
                let rowleft={
                    type: 'bar',
                    stack: 'left', // 设置堆叠的组名
                    name: row.TimePeriod,
                    barWidth: barWidth,
                    itemStyle: {
                        borderWidth: 1,
                        borderColor: this.colorList[inx].lineColor,
                        normal: {
                            color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                        },
                    },
                    barGap: 0,  // 柱条间距分为两种，一种是不同系列在同一类目下的距离 barGap，另一种是类目与类目的距离 barCategoryGap。
                    data: sideData,
                    minValdata:minData
                }
                let rowRight={
                    type: 'bar',
                    stack: 'right', // 设置堆叠的组名
                    name: row.TimePeriod,
                    barWidth: barWidth,
                    tooltip: {
                        show: false,
                    },
                    itemStyle: {
                        // borderWidth: 1,
                        // borderColor: this.colorList[inx].lineColor,
                        normal: {
                            color: new echarts.graphic.LinearGradient(0,1,0,0,[{offset: 0,color: this.colorList[inx].areaColorBom, },{offset: 1,color: this.colorList[inx].areaColorTop, },],false),
                        },
                    },
                    data: sideData,
                    label: {
                    show: false,
                    position: 'top',
                    textStyle: {
                        color: 'white',
                        fontSize: 10,
                    },
                    },
                    minValdata:minData
                }
                let topDataArr=JSON.parse(JSON.stringify(sideData))
                topDataArr=topDataArr.filter(rs=>Number(rs[1])>0)//顶部只过滤数据大于0的
                for(let i=0;i<inx;i++){
                    topDataArr=topDataArr.map(rw=>{
                        let findRowDetails=JSON.parse(JSON.stringify(barData[i].Details))
                        // findRowDetails.sort((a,b)=>Number(a.TTime)-Number(b.TTime))
                        if(findRowDetails&&findRowDetails.length>0){
                            let findRow= findRowDetails.find(rs=>rs.TTime==rw[0])
                            if(findRow){
                                rw[1]=Number(rw[1])+Number(findRow[this.dataType])
                            }
                        }
                        
                        return rw
                    })
                }
                // console.log(topDataArr,'topDataArr');
                let rowTop={
                    tooltip: {
                        show: false,
                    },
                    name: row.TimePeriod,
                    stack: 'top', // 设置堆叠的组名
                    type: 'pictorialBar',
                    itemStyle: {
                        borderWidth: 1,
                        borderColor: this.colorList[inx].lineColor,
                        color: this.colorPool[inx], // 控制顶部方形的颜色
                    },
                    symbol: 'diamond', // 有三类图形可选，一种是 ECharts 内置形状，第二种是图片，第三种是 SVG 的路径
                    symbolSize: [barWidth * 2-1, '10'], // 第一个值控制顶部方形大小
                    symbolOffset: ['0', '-5'], // 控制顶部放行 左右和上下
                    symbolRotate: 0,
                    symbolPosition: 'end',
                    data: JSON.parse(JSON.stringify(topDataArr)),
                    z: 3,
                    minValdata:100
                }
                beforeBottomData=JSON.parse(JSON.stringify(sideData))
                series.push(rowleft)
                series.push(rowRight)
                series.push(rowTop)
            })
            // series.sort((a,b)=>a.minValdata-b.minValdata)
            console.log(JSON.parse(JSON.stringify(series)),'seriesseriesseries');
            this.chartData = { series };
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
            let {series}=this.chartData 
            // 计算柱体间距（根据系列数量动态调整）
            const barWidth = this.transformFontSize(8);
            
            // const x = ['宿舍环境', '支援服务', '荣誉表彰', '社会实践', '实践技能'];
            // // const barWidth = 12;
            // const sideData = [['宿舍环境',220], ['支援服务',182], ['荣誉表彰',191]];
            // const sideData1 = [['荣誉表彰',120], ['社会实践',134]];
            // const sideData2 = [['荣誉表彰',191],['社会实践',90], ['实践技能',60]];
            // const summedData1 = sideData1.map((value, index) => {
            //     // 确保sideData1对应索引有值，避免undefined相加
            //     let sideDatafind=sideData.find(rw=>rw[0]==value[0])
            //     if(sideDatafind){
            //         return [value[0],value[1] + (sideDatafind[1] || 0)];
            //     }else{
            //         return value
            //     }
                
            // });
            // const summedData2 = sideData2.map((value, index) => {
            //     // 确保sideData1对应索引有值，避免undefined相加
            //     let valueData=value[1]
            //     let sideDatafind=sideData.find(rw=>rw[0]==value[0])
            //     let sideData1find=sideData1.find(rw=>rw[0]==value[0])
            //     if(sideDatafind){
            //         valueData=valueData+sideDatafind[1]
            //     }
            //     if(sideData1find){
            //         valueData=valueData+sideData1find[1]
            //     }
            //     return [value[0],valueData]
            // });
            let that=this
            const option  = {
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow',
                    },
                    formatter: function (params) {
                        let tooltipText = `<strong>${that.dataTitle}=>${params[0].name}</strong><br/>`;

                        // 获取每个数据项

                        const sideDataValue = params.find((item) => item.seriesName === '尖')?params.find((item) => item.seriesName === '尖').value[1]:0;
                        const sideData1Value = params.find((item) => item.seriesName === '峰')?params.find((item) => item.seriesName === '峰').value[1]:0;
                        const sideData2Value = params.find((item) => item.seriesName === '平')?params.find((item) => item.seriesName === '平').value[1]:0;
                        const sideData3Value = params.find((item) => item.seriesName === '谷')?params.find((item) => item.seriesName === '谷').value[1]:0;
                        // 计算总和
                        const totalValue = Number(Number(sideDataValue + sideData1Value+sideData2Value+sideData3Value).toFixed(2));

                        // 显示每个系列的数值
                        params.forEach((item) => {tooltipText += `${item.seriesName} : ${item.value[1]}<br/>`;});

                        // 显示总和
                        tooltipText += `总和 : ${totalValue}`;

                        return tooltipText;
                    },
                },
                legend: {
                    show: true, // 确保图例显示
                    data: ['尖', '峰','平','谷'], // 数据系列名称
                    itemWidth:10,
                    itemHeight:4,
                    textStyle: {
                        color: '#fff', // 图例文字颜色
                    },
                    orient: 'horizontal', // 图例的排列方式：'horizontal' 或 'vertical'
                    top: '0', // 图例位置，可选值 'top', 'bottom', 'left', 'right'
                    left:"197px",
                    formatter: function (name) {
                        // if (name === '尖') {
                        // return '全部'; // 只替换 '尖' 为 '全部'
                        // }
                        return name; // 其他保持不变
                    },
                },
                grid: {
                    top: '16%',
                    left: '5px',
                    right: '0px',
                    bottom: '0px',
                    containLabel: true,
                },
                toolbox: {
                    show: true,
                },
                calculable: true,
                xAxis: {
                    type: 'category',
                    data:['00','01','02','03','04','05','06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23'],
                    axisLine: {
                        show:false,
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
                        show:false,
                        alignWithLabel: true, // 设置刻度线居中
                    },
                },
                yAxis: {
                    type: 'value',
                    axisLine: {
                        show:false,
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
                    axisTick: {
                        show: false, // 设置刻度线居中
                    },
                },
                series: series,
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
    width: 100%;
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
