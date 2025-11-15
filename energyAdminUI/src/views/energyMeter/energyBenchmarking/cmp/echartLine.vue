<template>
    <div class="chartLine" :style="{'min-height':(tableConHeight/2-14)+'px'}">
        <!-- <div class="heard-title">
           <div class="title">碳排放趋势分析 <span>(单位：tCO₂e)</span></div>
           <div class="tabLine">
              <div> <div></div> <span>实际值</span></div>
           </div>
        </div> -->
        <div class="integrative-chart" :style="{'min-height':(tableConHeight/2-14)+'px'}">
            <div id="chartInter" class="chart-container" :style="{'min-height':(tableConHeight/2-14)+'px'}"></div>
        </div>
    </div>
</template>

<script>
import * as echarts from 'echarts';
import moment from 'moment';
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
            default:''
        },
        trendType:{
            type:String,
            default:''
        },
        lengright:{
            type:String,
            default:'10%'
        },
        trendTypeList: {
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
            }]
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
                if(this.dataType){
                    this.processTableData(JSON.parse(JSON.stringify(newVal)));
                    this.initChart();
                    window.addEventListener('resize', this.resizeChart);
                }
                
            },
            deep: true,
        },
    },
    beforeDestroy() {
        if (this.chart) {
            this.chart.dispose();
        }
        if(this.dataType){
            window.removeEventListener('resize', this.resizeChart);
        }
    },
    methods: {
        loadChart(){
            this.processTableData(newVal);
            this.initChart();
        },
        // 处理数据：按日期汇总CarbonEmission
         processTableData(data) {
            // 重置数据
            this.xData = [];
            this.yData = [];
            this.tooltipData = [];
            this.averageValue = 0;

            // 过滤无效数据
            const validData = data.map(row=>{
                row.Details=row.Details.filter(item => 
                    item.DDate && item.CarbonEmission !== undefined && item.CarbonEmission !== null
                )
                return row
            });

            if (validData.length === 0) {
                this.isDataZoom = false;
                return;
            }
            validData.map((row,inx)=>{
                this.yData.push([])
                this.tooltipData.push({name:row.FacilityName})
                row.Details.map(rw=>{
                    if(this.xData.indexOf(rw.DDate)>-1){}else{
                        this.xData.push(rw.DDate)
                    }
                    this.yData[inx].push(rw[this.trendType])
                })
            })
            // 当数据点超过7个时显示数据缩放条
            this.isDataZoom = (this.xData.map(row=>row.Details&&row.Details.length>7))&&(this.xData.map(row=>row.Details&&row.Details.length>7)).length>0;
        },
        initChart() {
            const chartDom = document.getElementById('chartInter');
            if (!chartDom) return;
            
            // 销毁旧实例
            if (this.chart) this.chart.dispose();
            this.chart = echarts.init(chartDom);
            let seriesData=[]
            seriesData=this.yData.map((row,ix)=>{
                let seriesObj={
                    name:this.tooltipData[ix].name,
                    data: row,
                    type: 'line',
                    smooth: true,
                    symbol: "none",
                    itemStyle: {
                        color: this.colorList[ix].lineColor,
                        borderColor: '#fff',
                        borderWidth: 2
                    },
                    lineStyle: {
                        color: this.colorList[ix].lineColor,
                        width: 2 // 折线宽度
                    },
                }
                return seriesObj
            })
            let trendTypeText=this.trendTypeList.find(row=>row.filed==this.trendType)

            const option = {
                grid: {
                    left: '1%', // 左边距离
                    right: '4%', // 右边距离
                    bottom: '3%', // 底部距离
                    top: '15%', // 顶部距离
                    containLabel: true, // 包含标签
                },
                legend: {
                    data: this.tooltipData.map(row=>row.name),
                    right: this.lengright,
                    itemWidth: 12,
                    itemHeight:8,
                    align: "left",
                    top: "3%",
                    itemStyle: {
                        // borderRadius: [transformFontSize(2), transformFontSize(2), transformFontSize(2), transformFontSize(2)],
                        // opacity: 0,
                    },
                    textStyle: {
                        color: "rgba(255,255,255,0.6)",
                        fontSize: 12,
                        fontWeight: 400,
                    },
                },
                tooltip: {
                    trigger: 'axis',
                    confine: true,
                    formatter: (params) => {
                        const idx = params[0].dataIndex;
                        let str=''
                        str=`<div style="font-size:14px; line-height:22px; padding:8px 12px;">`
                        this.tooltipData.map((row,ix)=>{
                            if(ix==0){
                                if(trendTypeText&&trendTypeText.label){
                                    str=str+`${trendTypeText.label}<br>`
                                }
                                str=str+`日期: ${this.xData[idx]}<br>
                                    <span>${row.name}: ${params[ix].value}</span><br>`
                            }else{
                                str=str+`<span>${row.name}: ${params[ix].value}</span><br>`
                            }
                            
                        })
                        str=str+'</div>'
                        return str;
                    },
                    backgroundColor: 'rgba(18, 19, 27, 0.95)',
                    borderColor: '#3DB98F',
                    borderWidth: 1
                },
                xAxis: [{
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
                }],
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
                dataZoom: {
                    type: 'slider', // 滑动条模式
                    show: false, // 显示拖拉条
                    // xAxisIndex: 0, // 关联 x 轴
                    color: 'rgba(255, 255, 255, 1)',
                    start: 0, // 初始缩放起始比例（%）
                    end: 100, // 初始缩放结束比例（%）
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
                },
                series: seriesData
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
    padding: 0 16px 0;
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
