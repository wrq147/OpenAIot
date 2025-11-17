/**
 * 组件配置，表单配置项
 */

export function getNewDefaultForm() {
    return {
        'pc': {
            isSelfAdaption: true,
            adaptionType: "0", //全自适应
            panelWidth: 1920,
            panelHeight: 1080,
            bgColor: "rgb(112, 110, 231)",
            bgImage: "",
            chartType: "themeForm",
            customId: "0",
            globalData: [],
            chartOption: {
                database: {
                    type: 'MySQL',
                    ipAdress: '127.0.0.1',
                    cacheTime: 0,
                    port: '3306',
                    username: 'root',
                    password: '123',
                    baseName: 'test',
                    tableName: 'test1',
                    dataSourceId: '',
                    sqlType: 'MySQL',
                    executeSql: ''
                },
                timeout: 200
            },
            themeColor: {
                color: [
                    "#2ec7c9",
                    "#b6a2de",
                    "#5ab1ef",
                    "#ffb980",
                    "#d87a80",
                    "#8d98b3",
                    "#e5cf0d",
                    "#97b552",
                    "#95706d",
                    "#dc69aa",
                    "#07a2a4",
                    "#9a7fd1",
                    "#588dd5",
                    "#f5994e",
                    "#c05050",
                    "#59678c",
                    "#c9ab00",
                    "#7eb00a",
                    "#6f5553",
                    "#c14089",
                ],
                backgroundColor: "rgba(0,0,0,0)",
                textStyle: {},
                title: {
                    textStyle: {
                        color: "#008acd",
                    },
                    subtextStyle: {
                        color: "#aaaaaa",
                    },
                },
                line: {
                    itemStyle: {
                        borderWidth: 1,
                    },
                    lineStyle: {
                        width: 2,
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                },
                radar: {
                    itemStyle: {
                        borderWidth: 1,
                    },
                    lineStyle: {
                        width: 2,
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                },
                bar: {
                    itemStyle: {
                        barBorderWidth: 0,
                        barBorderColor: "#ccc",
                    },
                },
                pie: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                scatter: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                boxplot: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                parallel: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                sankey: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                funnel: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                gauge: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                candlestick: {
                    itemStyle: {
                        color: "#d87a80",
                        color0: "#2ec7c9",
                        borderColor: "#d87a80",
                        borderColor0: "#2ec7c9",
                        borderWidth: 1,
                    },
                },
                graph: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                    lineStyle: {
                        width: 1,
                        color: "#aaa",
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                    color: [
                        "#2ec7c9",
                        "#b6a2de",
                        "#5ab1ef",
                        "#ffb980",
                        "#d87a80",
                        "#8d98b3",
                        "#e5cf0d",
                        "#97b552",
                        "#95706d",
                        "#dc69aa",
                        "#07a2a4",
                        "#9a7fd1",
                        "#588dd5",
                        "#f5994e",
                        "#c05050",
                        "#59678c",
                        "#c9ab00",
                        "#7eb00a",
                        "#6f5553",
                        "#c14089",
                    ],
                    label: {
                        color: "#eee",
                    },
                },
                map: {
                    itemStyle: {
                        normal: {
                            areaColor: "#dddddd",
                            borderColor: "#eeeeee",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            areaColor: "rgba(254,153,78,1)",
                            borderColor: "#444",
                            borderWidth: 1,
                        },
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#d87a80",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "rgb(100,0,0)",
                            },
                        },
                    },
                },
                geo: {
                    itemStyle: {
                        normal: {
                            areaColor: "#dddddd",
                            borderColor: "#eeeeee",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            areaColor: "rgba(254,153,78,1)",
                            borderColor: "#444",
                            borderWidth: 1,
                        },
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#d87a80",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "rgb(100,0,0)",
                            },
                        },
                    },
                },
                categoryAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: false,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: false,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                valueAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                logAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                timeAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: false,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                toolbox: {
                    iconStyle: {
                        normal: {
                            borderColor: "#2ec7c9",
                        },
                        emphasis: {
                            borderColor: "#18a4a6",
                        },
                    },
                },
                legend: {
                    textStyle: {
                        color: "#ffffff",
                    },
                },
                tooltip: {
                    axisPointer: {
                        lineStyle: {
                            color: "#008acd",
                            width: "1",
                        },
                        crossStyle: {
                            color: "#008acd",
                            width: "1",
                        },
                    },
                },
                timeline: {
                    lineStyle: {
                        color: "#008acd",
                        width: 1,
                    },
                    itemStyle: {
                        normal: {
                            color: "#008acd",
                            borderWidth: 1,
                        },
                        emphasis: {
                            color: "#a9334c",
                        },
                    },
                    controlStyle: {
                        normal: {
                            color: "#008acd",
                            borderColor: "#008acd",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            color: "#008acd",
                            borderColor: "#008acd",
                            borderWidth: 0.5,
                        },
                    },
                    checkpointStyle: {
                        color: "#2ec7c9",
                        borderColor: "#2ec7c9",
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#008acd",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "#008acd",
                            },
                        },
                    },
                },
                visualMap: {
                    color: ["#5ab1ef", "#e0ffff"],
                },
                dataZoom: {
                    backgroundColor: "rgba(47,69,84,0)",
                    dataBackgroundColor: "#efefff",
                    fillerColor: "rgba(182,162,222,0.2)",
                    handleColor: "#008acd",
                    handleSize: "100%",
                    textStyle: {
                        color: "#333333",
                    },
                },
                markPoint: {
                    label: {
                        color: "#eee",
                    },
                    emphasis: {
                        label: {
                            color: "#eee",
                        },
                    },
                },
            },
        },
        'phone': {
            customId: "0",
            isSelfAdaption: true,
            adaptionType: "0", //全自适应
            panelWidth: 375,
            panelHeight: 667,
            bgColor: "rgb(112, 110, 231)",
            bgImage: "",
            chartType: "themeForm",
            customId: "0",
            globalData: [],
            chartOption: {
                database: {
                    cacheTime: 0,
                    type: 'MySQL',
                    ipAdress: '127.0.0.1',
                    port: '3306',
                    username: 'root',
                    password: '123',
                    baseName: 'test',
                    tableName: 'test1',
                    dataSourceId: '',
                    sqlType: 'MySQL',
                    executeSql: ''
                },
                timeout: 200
            },
            themeColor: {
                color: [
                    "#2ec7c9",
                    "#b6a2de",
                    "#5ab1ef",
                    "#ffb980",
                    "#d87a80",
                    "#8d98b3",
                    "#e5cf0d",
                    "#97b552",
                    "#95706d",
                    "#dc69aa",
                    "#07a2a4",
                    "#9a7fd1",
                    "#588dd5",
                    "#f5994e",
                    "#c05050",
                    "#59678c",
                    "#c9ab00",
                    "#7eb00a",
                    "#6f5553",
                    "#c14089",
                ],
                backgroundColor: "rgba(0,0,0,0)",
                textStyle: {},
                title: {
                    textStyle: {
                        color: "#008acd",
                    },
                    subtextStyle: {
                        color: "#aaaaaa",
                    },
                },
                line: {
                    itemStyle: {
                        borderWidth: 1,
                    },
                    lineStyle: {
                        width: 2,
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                },
                radar: {
                    itemStyle: {
                        borderWidth: 1,
                    },
                    lineStyle: {
                        width: 2,
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                },
                bar: {
                    itemStyle: {
                        barBorderWidth: 0,
                        barBorderColor: "#ccc",
                    },
                },
                pie: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                scatter: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                boxplot: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                parallel: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                sankey: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                funnel: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                gauge: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                },
                candlestick: {
                    itemStyle: {
                        color: "#d87a80",
                        color0: "#2ec7c9",
                        borderColor: "#d87a80",
                        borderColor0: "#2ec7c9",
                        borderWidth: 1,
                    },
                },
                graph: {
                    itemStyle: {
                        borderWidth: 0,
                        borderColor: "#ccc",
                    },
                    lineStyle: {
                        width: 1,
                        color: "#aaa",
                    },
                    symbolSize: 3,
                    symbol: "emptyCircle",
                    smooth: true,
                    color: [
                        "#2ec7c9",
                        "#b6a2de",
                        "#5ab1ef",
                        "#ffb980",
                        "#d87a80",
                        "#8d98b3",
                        "#e5cf0d",
                        "#97b552",
                        "#95706d",
                        "#dc69aa",
                        "#07a2a4",
                        "#9a7fd1",
                        "#588dd5",
                        "#f5994e",
                        "#c05050",
                        "#59678c",
                        "#c9ab00",
                        "#7eb00a",
                        "#6f5553",
                        "#c14089",
                    ],
                    label: {
                        color: "#eee",
                    },
                },
                map: {
                    itemStyle: {
                        normal: {
                            areaColor: "#dddddd",
                            borderColor: "#eeeeee",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            areaColor: "rgba(254,153,78,1)",
                            borderColor: "#444",
                            borderWidth: 1,
                        },
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#d87a80",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "rgb(100,0,0)",
                            },
                        },
                    },
                },
                geo: {
                    itemStyle: {
                        normal: {
                            areaColor: "#dddddd",
                            borderColor: "#eeeeee",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            areaColor: "rgba(254,153,78,1)",
                            borderColor: "#444",
                            borderWidth: 1,
                        },
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#d87a80",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "rgb(100,0,0)",
                            },
                        },
                    },
                },
                categoryAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: false,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: false,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                valueAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                logAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                timeAxis: {
                    axisLine: {
                        show: true,
                        lineStyle: {
                            color: "#008acd",
                        },
                    },
                    axisTick: {
                        show: true,
                        lineStyle: {
                            color: "#333",
                        },
                    },
                    axisLabel: {
                        show: true,
                        textStyle: {
                            color: "#ffffff",
                        },
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: ["#eee"],
                        },
                    },
                    splitArea: {
                        show: false,
                        areaStyle: {
                            color: ["rgba(250,250,250,0.3)", "rgba(200,200,200,0.3)"],
                        },
                    },
                },
                toolbox: {
                    iconStyle: {
                        normal: {
                            borderColor: "#2ec7c9",
                        },
                        emphasis: {
                            borderColor: "#18a4a6",
                        },
                    },
                },
                legend: {
                    textStyle: {
                        color: "#ffffff",
                    },
                },
                tooltip: {
                    axisPointer: {
                        lineStyle: {
                            color: "#008acd",
                            width: "1",
                        },
                        crossStyle: {
                            color: "#008acd",
                            width: "1",
                        },
                    },
                },
                timeline: {
                    lineStyle: {
                        color: "#008acd",
                        width: 1,
                    },
                    itemStyle: {
                        normal: {
                            color: "#008acd",
                            borderWidth: 1,
                        },
                        emphasis: {
                            color: "#a9334c",
                        },
                    },
                    controlStyle: {
                        normal: {
                            color: "#008acd",
                            borderColor: "#008acd",
                            borderWidth: 0.5,
                        },
                        emphasis: {
                            color: "#008acd",
                            borderColor: "#008acd",
                            borderWidth: 0.5,
                        },
                    },
                    checkpointStyle: {
                        color: "#2ec7c9",
                        borderColor: "#2ec7c9",
                    },
                    label: {
                        normal: {
                            textStyle: {
                                color: "#008acd",
                            },
                        },
                        emphasis: {
                            textStyle: {
                                color: "#008acd",
                            },
                        },
                    },
                },
                visualMap: {
                    color: ["#5ab1ef", "#e0ffff"],
                },
                dataZoom: {
                    backgroundColor: "rgba(47,69,84,0)",
                    dataBackgroundColor: "#efefff",
                    fillerColor: "rgba(182,162,222,0.2)",
                    handleColor: "#008acd",
                    handleSize: "100%",
                    textStyle: {
                        color: "#333333",
                    },
                },
                markPoint: {
                    label: {
                        color: "#eee",
                    },
                    emphasis: {
                        label: {
                            color: "#eee",
                        },
                    },
                },
            },
        },
    };
}

// 图表配置
export const chartsComponents = [
    // 柱状图插件
    {
        chartType: "bar",
        layerName: "柱状图",
        icon: "el-icon-datav-bar",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "bar",
                layerName: "基本柱图",
                icon: "el-icon-datav-bar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            fontStyle: "normal", // 风格
                            fontWeight: "normal", // 粗细
                            //fontFamily: 'Microsoft yahei',   // 字体
                            //fontSize: 20,     // 大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    legend: {
                        show: true, // 是否显示图例
                        type: "scroll",
                        x: "center",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    stack: false, // 是否堆叠
                    itemStyle: "false", // 是否圆角
                    barWidth: 10, // 柱体宽度
                    tooltip: {
                        trigger: "axis",
                        axisPointer: {
                            type: "shadow",
                        },
                    },
                    xAxis: {
                        name: "x轴",
                        type: "category",
                        data: null,
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        show: true,
                    },
                    yAxis: {
                        name: "y轴",
                        type: "value",
                        boundaryGap: [0, 0.01],
                        /*splitLine: {
                                    lineStyle: {
                                        // 使用深浅的间隔色
                                        color: ['#fff']
                                    }
                                },
                                nameTextStyle: {
                                    color: ['#fff']
                                },*/
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                                width: 1, //这里是为了突出显示加上的
                            },
                        },
                        splitLine: {
                            show: true,
                        },
                        show: true,
                    },
                    //竖显示
                    isVertical: false,
                    animate: "",
                    //静态数据值
                    staticDataValue: [{
                            name: "巴西",
                            value: 18203,
                        },
                        {
                            name: "印尼",
                            value: 23489,
                        },
                        {
                            name: "美国",
                            value: 29034,
                        },
                        {
                            name: "印度",
                            value: 104970,
                        },
                        {
                            name: "中国",
                            value: 131744,
                        },
                        {
                            name: "世界人口(万)",
                            value: 630230,
                        }
                    ],
                    itemWidth: 15, //图例宽度
                    itemHeight: 10, //图例高度
                    legendFontSize: 12, //图例字体大小
                    legendFontColor: "#fff", //图例字体颜色
                    legendOrient: "horizontal", //图例布局
                    legendX: 25, //图例水平位置
                    legendY: 1, //图例垂直位置
                    gap: false, //是否重叠
                    radiusType: "half", //半圆角显示
                    radius: 12, //圆角半径
                    xLineShow: true, //显示x轴线
                    xLineWidth: 2, //x轴粗细
                    xLineColor: "#fff", //x轴线颜色
                    xFontSize: 12, //x轴标签字体大小
                    xFontColor: "#fff", //x轴标签字体颜色
                    yLineShow: true, //显示y轴线
                    yLineWidth: 2, //y轴粗细
                    yLineColor: "#fff", //y轴线颜色
                    yFontSize: 12, //y轴标签字体大小
                    yFontColor: "#fff", //y轴标签字体颜色
                    lineWidth: 2, //折线粗细
                    lineStyle: "solid", //分隔线实线
                    splitLineWidth: 1, // 分隔线宽度
                    splitLineColor: "#fff", //分隔线颜色
                    customData: "",
                    interactData: [],
                    //是否查看详情1/0
                    isView: "0",
                    //查看详情接口地址
                    viewURL: "",
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                },
            },
            {
                chartType: "stackedBar",
                layerName: "堆叠图",
                icon: "el-icon-datav-stackedBar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    legend: {
                        show: true, // 是否显示图例
                        type: "scroll",
                        x: "center",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    tooltip: {
                        trigger: "axis",
                        axisPointer: {
                            type: "shadow", //点击某一个柱时，该柱有阴影覆盖
                        },
                    },
                    barWidth: 20, // 柱体宽度
                    legend: {
                        show: true, // 是否显示图例
                        type: "scroll", //当图例数量较多时，可滚动翻页
                        x: "center",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED", //图例中文字颜色
                        },
                        height: 150, //图例高度
                    },
                    color: ["#003366", "#006699", "#4cabce", "#e5323e"],
                    xAxis: {
                        name: "x轴",
                        type: "category", //类目轴，存放离散的数据
                        data: null,
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        show: true,
                    },
                    yAxis: {
                        name: "y轴",
                        type: "value",
                        boundaryGap: [0, 0.01],
                        /*splitLine: {
                                    lineStyle: {
                                        // 使用深浅的间隔色
                                        color: ['#fff']
                                    }
                                },
                                nameTextStyle: {
                                    color: ['#fff']
                                },*/
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                                width: 1, //这里是为了突出显示加上的
                            },
                        },
                        splitLine: {
                            show: true,
                        },
                        show: true,
                    },
                    //显示图例
                    isShowLegend: true,
                    itemWidth: 15, //图例宽度
                    itemHeight: 10, //图例高度
                    legendFontSize: 12, //图例字体大小
                    legendFontColor: "#fff", //图例字体颜色
                    legendOrient: "horizontal", //图例布局
                    legendX: 25, //图例水平位置
                    legendY: 1, //图例垂直位置
                    //竖显示
                    isVertical: false,
                    customData: "",
                    interactData: [],
                    //载入动画
                    animate: "",
                    //记录前一次切换标识
                    //preChangeFlag: "false",
                    //静态数据值
                    staticDataValue: [{
                            series: '2011年',
                            name: "巴西",
                            stack: "Ad",
                            data: 320,
                        },
                        {
                            series: '2012年',
                            name: "巴西",
                            stack: "Ad",
                            data: 332,
                        },
                        {
                            series: '2011年',
                            name: "印尼",
                            stack: "Ad",
                            data: 120,
                        },
                        {
                            series: '2012年',
                            name: "印尼",
                            stack: "Ad",
                            data: 132,
                        },
                        {
                            series: '2011年',
                            name: "中国",
                            data: 220,
                        },
                        {
                            series: '2012年',
                            name: "中国",
                            data: 182,
                        },
                        {
                            series: '2011年',
                            name: "美国",
                            data: 150,
                        },
                        {
                            series: '2012年',
                            name: "美国",
                            data: 232,
                        }
                    ],
                    //color:['#37a2da', '#fb7293', '#8378ea', '#e7bcf3', '#ffdb5c', '#32c5e9', '#9fe6b8', '#ff9f7f'],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "staggeredLabel",
                layerName: "交错正负轴标签",
                icon: "el-icon-datav-staggeredLabel",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            fontStyle: "normal", // 风格
                            fontWeight: "normal", // 粗细
                            //fontFamily: 'Microsoft yahei',   // 字体
                            //fontSize: 20,     // 大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    tooltip: {
                        trigger: "axis",
                        axisPointer: {
                            // 坐标轴指示器，坐标轴触发有效
                            type: "shadow", // 默认为直线，可选为：'line' | 'shadow'
                        },
                    },
                    top: 30,
                    bottom: 30,
                    direction: "level",
                    position: "top",
                    lineStyle: "dashed",
                    axisLine: false,
                    axisLabel: false,
                    axisTick: false,
                    splitLine: false,
                    xAxisShow: true,
                    showLabel: true,
                    name: "生活费",
                    labelPositive: "",
                    labelNegative: "",
                    itemPositive: "#c23531",
                    itemNegative: "#c23531",
                    isThemeColor: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{
                            name: "苹果",
                            value: 100,
                        },
                        {
                            name: "三星",
                            value: -349,
                        },
                        {
                            name: "小米",
                            value: 230,
                        },
                        {
                            name: "华为",
                            value: 540,
                        },
                        {
                            name: "海尔",
                            value: -140,
                        },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "doubleBar",
                layerName: "双柱状图",
                icon: "el-icon-datav-doublebar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    istitle: false,
                    leftColor: "#01C5B2",
                    rightColor: "#FB6F6C",
                    textColor: "#fff",
                    xAxisShow: true,
                    position: "top",
                    lineStyle: "dashed",
                    showLegend: true,
                    barWidth: 15,
                    isThemeColor: false,
                    //静态数据值
                    staticDataValue: [
                        { name: "男性数量", label: "10~20岁", value: 349 },
                        { name: "男性数量", label: "20~30岁", value: 230 },
                        { name: "男性数量", label: "30~40岁", value: 400 },
                        { name: "男性数量", label: "40~50岁", value: 840 },
                        { name: "女性数量", label: "10~20岁", value: 549 },
                        { name: "女性数量", label: "20~30岁", value: 930 },
                        { name: "女性数量", label: "30~40岁", value: 840 },
                        { name: "女性数量", label: "40~50岁", value: 640 },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "doubleBarX",
                layerName: "共x轴双柱状图",
                icon: "el-icon-datav-doublebar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "rgba(51, 51, 51, 1)", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 20, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "rgba(51, 51, 51, 1)",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                        top: 1,
                        textAlign: 'left',
                        left: 3,
                    },
                    legend: {
                        show: true,
                        type: "scroll",
                        orient: "vertical",
                        right: "10%",
                        align: "left",
                        top: "middle",
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                        itemWidth: 20,
                        itemHeight: 20,
                        icon: 'circle', //图例形状
                    },
                    formatterSuffix: "", //数值标签的后缀
                    legendCols: 2,
                    legendPositionLeft: 73,
                    legendPositionTop: 2,
                    legendItemGapX: 13,
                    legendItemGapY: 8,
                    legendFontSize: 14,
                    bgSetting: {
                        backgroundType: 'color',
                        backgroundColor: '#ffffff',
                        backgroundImg: '',
                        top: 40,
                        bottom: 20,
                        left: 20,
                        right: 0,
                        radius: 5,
                    },
                    xAxis: { //x轴样式
                        margin: 20,
                        isShowAxisLabel: true, //坐标轴标签
                        axisLabelColor: 'rgba(153, 153, 153, 1)',
                        axisLabelFontSize: 16,
                        axisLabelFontStyle: 'normal',
                        axisLabelFontWeight: 'normal',
                        axisLabelInside: false,
                        axisLabelRotate: 0,
                        axisLabelAlign: 'center', //对齐
                        isShowAxisLine: false, //坐标轴线
                        axisLineColor: 'rgba(244, 244, 244, 1)',
                        axisLineWidth: 2,
                        axisLineType: 'solid',
                        isShowAxisTick: false, //刻度线
                        axisTickType: 'solid', //坐标刻度
                        axisTickColor: 'rgba(244, 244, 244, 1)',
                        axisTickWidth: 2, //坐标刻度
                        axisTickInside: false, //坐标刻度
                        axisTickLength: 6, //坐标刻度
                        isShowSplitLine: false, //分割线
                        splitLineType: 'solid', //
                        splitLineWidth: 2, //
                        splitLineColor: 'rgba(244, 244, 244, 1)',
                    },
                    yAxis: { //y轴样式
                        margin: 20,
                        position: 'left',
                        isShowAxisLabel: true, //坐标轴标签
                        axisLabelColor: 'rgba(153, 153, 153, 1)',
                        axisLabelFontSize: 16,
                        axisLabelFontStyle: 'normal',
                        axisLabelFontWeight: 'normal',
                        axisLabelInside: false,
                        isShowAxisLine: false, //坐标轴线
                        axisLineColor: 'rgba(244, 244, 244, 1)',
                        axisLineWidth: 2,
                        axisLineType: 'solid',
                        isShowAxisTick: false, //刻度线
                        axisTickType: 'solid', //坐标刻度
                        axisTickColor: 'rgba(244, 244, 244, 1)',
                        axisTickWidth: 2, //坐标刻度
                        axisTickInside: false, //坐标刻度
                        axisTickLength: 6, //坐标刻度
                        isShowSplitLine: true, //分割线
                        splitLineType: 'solid', //
                        splitLineWidth: 2, //分割线
                        splitLineColor: 'rgba(244, 244, 244, 1)',
                    },
                    label: { //标签
                        isShow: false, //
                        fontSize: 16,
                        distance: 10, //距离
                        position: '',
                        color: 'rgba(244, 244, 244, 1)',
                    },
                    istitle: false,
                    leftColor: "#01C5B2",
                    rightColor: "#FB6F6C",
                    barColorlist: [],
                    xAxisShow: true, //是否显示横坐标
                    yAxisShow: true, //是否显示纵坐标
                    position: "bottom",
                    lineStyle: "dashed",
                    showLegend: true,
                    barWidth: 15,
                    barRadius: 5,
                    width: 95,
                    isThemeColor: false,
                    //静态数据值
                    staticDataValue: [
                        { name: "男性数量", label: "10~20岁", value: 349 },
                        { name: "男性数量", label: "20~30岁", value: 230 },
                        { name: "男性数量", label: "30~40岁", value: 400 },
                        { name: "男性数量", label: "40~50岁", value: 840 },
                        { name: "女性数量", label: "10~20岁", value: 549 },
                        { name: "女性数量", label: "20~30岁", value: 930 },
                        { name: "女性数量", label: "30~40岁", value: 840 },
                        { name: "女性数量", label: "40~50岁", value: 640 },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            }, {
                chartType: "doubleParalleBar",
                layerName: "水平柱状图",
                icon: "el-icon-datav-doublebar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "rgba(51, 51, 51, 1)", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 20, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "rgba(51, 51, 51, 1)",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                        top: 1,
                        textAlign: 'left',
                        left: 3,
                    },
                    legend: {
                        show: true,
                        type: "scroll",
                        orient: "vertical",
                        right: "10%",
                        align: "left",
                        top: "middle",
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                        itemWidth: 20,
                        itemHeight: 20,
                        icon: 'circle', //图例形状
                    },
                    formatterSuffix: "", //数值标签的后缀
                    legendCols: 2,
                    legendPositionLeft: 73,
                    legendPositionTop: 2,
                    legendItemGapX: 13,
                    legendItemGapY: 8,
                    legendFontSize: 14,
                    bgSetting: {
                        backgroundType: 'color',
                        backgroundColor: '#ffffff',
                        backgroundImg: '',
                        top: 40,
                        bottom: 20,
                        left: 20,
                        right: 0,
                        radius: 5,
                    },
                    xAxis: { //x轴样式
                        margin: 20,
                        isShowAxisLabel: true, //坐标轴标签
                        axisLabelColor: 'rgba(153, 153, 153, 1)',
                        axisLabelFontSize: 16,
                        axisLabelFontStyle: 'normal',
                        axisLabelFontWeight: 'normal',
                        axisLabelInside: false,
                        axisLabelRotate: 0,
                        axisLabelAlign: 'center', //对齐
                        isShowAxisLine: false, //坐标轴线
                        axisLineColor: 'rgba(244, 244, 244, 1)',
                        axisLineWidth: 2,
                        axisLineType: 'solid',
                        isShowAxisTick: false, //刻度线
                        axisTickType: 'solid', //坐标刻度
                        axisTickColor: 'rgba(244, 244, 244, 1)',
                        axisTickWidth: 2, //坐标刻度
                        axisTickInside: false, //坐标刻度
                        axisTickLength: 6, //坐标刻度
                        isShowSplitLine: false, //分割线
                        splitLineType: 'solid', //
                        splitLineWidth: 2, //
                        splitLineColor: 'rgba(244, 244, 244, 1)',
                    },
                    yAxis: { //y轴样式
                        margin: 20,
                        position: 'left',
                        isShowAxisLabel: true, //坐标轴标签
                        axisLabelColor: 'rgba(153, 153, 153, 1)',
                        axisLabelFontSize: 16,
                        axisLabelFontStyle: 'normal',
                        axisLabelFontWeight: 'normal',
                        axisLabelInside: false,
                        isSetLabelPosition: false, //是否设置标签位置
                        axisLabelVerticalAlign: 'bottom',
                        axisLabelAlign: 'left',
                        axisLabelPaddingBottom: 10,
                        axisLabelPaddingLeft: 10,
                        axisLabelPaddingTop: 10,
                        axisLabelPaddingRight: 10,
                        isShowAxisLine: false, //坐标轴线
                        axisLineColor: 'rgba(244, 244, 244, 1)',
                        axisLineWidth: 2,
                        axisLineType: 'solid',
                        isShowAxisTick: false, //刻度线
                        axisTickType: 'solid', //坐标刻度
                        axisTickColor: 'rgba(244, 244, 244, 1)',
                        axisTickWidth: 2, //坐标刻度
                        axisTickInside: false, //坐标刻度
                        axisTickLength: 6, //坐标刻度
                        isShowSplitLine: true, //分割线
                        splitLineType: 'solid', //
                        splitLineWidth: 2, //分割线
                        splitLineColor: 'rgba(244, 244, 244, 1)',
                    },
                    label: { //标签
                        isShow: false, //
                        fontSize: 16,
                        distance: 10, //距离
                        position: '',
                        color: 'rgba(244, 244, 244, 1)',
                    },
                    istitle: false,
                    barColorlist: [], //柱状图颜色
                    xAxisShow: true, //是否显示横坐标
                    yAxisShow: true, //是否显示纵坐标
                    position: "bottom",
                    lineStyle: "dashed",
                    showLegend: true,
                    barWidth: 15,
                    barRadius: 5,
                    width: 95,
                    isThemeColor: false,
                    //静态数据值
                    staticDataValue: [
                        { name: "男性数量", label: "10~20岁", value: 349 },
                        { name: "男性数量", label: "20~30岁", value: 230 },
                        { name: "男性数量", label: "30~40岁", value: 400 },
                        { name: "男性数量", label: "40~50岁", value: 840 },
                        { name: "女性数量", label: "10~20岁", value: 549 },
                        { name: "女性数量", label: "20~30岁", value: 930 },
                        { name: "女性数量", label: "30~40岁", value: 840 },
                        { name: "女性数量", label: "40~50岁", value: 640 },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "topBar",
                layerName: "排行榜",
                icon: "el-icon-datav-topbar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        subtext: "", //副标题
                    },
                    istitle: false,
                    order: "descend",
                    color: [
                        { color0: "#37a2da", color1: "#fff" },
                        { color0: "#fb7293", color1: "#fff" },
                        { color0: "#8378ea", color1: "#fff" },
                        { color0: "#e7bcf3", color1: "#fff" },
                        { color0: "#33CC33", color1: "#fff" },
                    ],
                    label: {
                        show: true,
                        position: "left",
                        textStyle: {
                            color: "#b3ccf8",
                            fontSize: 16,
                            fontFamily: "黑体",
                        },
                    },
                    valueLabel: {
                        show: true,
                        inside: false,
                        textStyle: {
                            color: "#b3ccf8",
                            fontSize: 16,
                            fontFamily: "黑体",
                        },
                        formatter: "{value}k",
                    },
                    width: 10,
                    suf: "k",
                    isThemeColor: false, //应用主题颜色
                    //静态数据值
                    staticDataValue: [
                        { value: 335, name: "直接访问" },
                        { value: 310, name: "邮件营销" },
                        { value: 234, name: "联盟广告" },
                        { value: 135, name: "视频广告" },
                        { value: 548, name: "搜索引擎" },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "paralleBar",
                customId: "",
                layerName: "水平柱图",
                icon: "el-icon-datav-parallebar",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            //fontStyle: 'normal', //风格
                            fontWeight: "normal", //粗细
                            fontFamily: "微软雅黑", //字体
                            fontSize: 18, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            //fontSize: 14,
                            align: "center",
                        },
                        x: "left",
                        itemGap: 7,
                    },
                    xAxis: {
                        max: 10000,
                        splitLine: {
                            show: false,
                        },
                        axisLine: {
                            show: false,
                        },
                        axisLabel: {
                            show: false,
                        },
                        axisTick: {
                            show: false,
                        },
                        splitArea: { show: false }, //去除网格区域
                    },
                    grid: {
                        left: 80,
                        top: 20, // 设置条形图的边距
                        right: 80,
                        bottom: 20,
                    },
                    yAxis: [{
                        type: "category",
                        data: null,
                        inverse: false,
                        axisLine: {
                            show: false,
                        },
                        axisTick: {
                            show: false,
                        },
                        axisLabel: {
                            show: false,
                        },
                    }, ],
                    total: 100,
                    borderColor: "#1C4B8E",
                    linearColor0: "#1588D1",
                    linearColor1: "#1C4B8E",
                    symbolColor: "#061348",
                    fontSize: 14,
                    fontFamily: "宋体",
                    fontColor: "#fff",
                    isThemeColor: false,
                    staticDataValue: [{
                            name: "管控",
                            value: 25,
                        },
                        {
                            name: "集中式",
                            value: 80,
                        },
                        {
                            name: "暗管",
                            value: 67,
                        },
                        {
                            name: "水渠",
                            value: 38,
                        },
                        {
                            name: "纳管",
                            value: 49,
                        },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "doubleyAxis",
                customId: "",
                layerName: "双y轴柱图",
                icon: "el-icon-datav-doubleyAxis",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    subtextType: '0', //是否是动态的副标题
                    title: {
                        text: "",
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 20, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                        top: 1,
                        textAlign: 'left',
                        left: 3,
                    },
                    isHideLegendText: true, //标题前是否显示图例
                    name: '', //图形名称
                    legendText: '总计量', //图例文本
                    legendCols: 1,
                    legendPositionLeft: 73,
                    legendPositionTop: 2,
                    legendItemGapX: 13,
                    legendItemGapY: 8,
                    legendFontSize: 14,
                    legendItemColor: 'rgba(244, 244, 244, 0)',
                    legend: {
                        show: true,
                        type: "scroll",
                        orient: "vertical",
                        right: "10%",
                        align: "left",
                        top: "middle",
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                        itemWidth: 20,
                        itemHeight: 20,
                        icon: 'circle', //图例形状
                    },
                    label: { //标签
                        isShow: false, //
                        fontSize: 16,
                        distance: 10, //距离
                        position: '',
                        color: 'rgba(244, 244, 244, 1)',
                        labelPaddingBottom: 10,
                        labelPaddingLeft: 10,
                        labelPaddingTop: 10,
                        labelPaddingRight: 10,
                    },
                    yAxisShow: true,
                    yAxis: { //y轴样式
                        margin: 20,
                        position: 'right',
                        isShowAxisLabel: true, //坐标轴标签
                        axisLabelColor: 'rgba(153, 153, 153, 1)',
                        axisLabelFontSize: 16,
                        axisLabelFontStyle: 'normal',
                        axisLabelFontWeight: 'normal',
                        axisLabelInside: false,
                        isSetLabelPosition: false, //是否设置标签位置
                        axisLabelVerticalAlign: 'bottom',
                        axisLabelAlign: 'left',
                        axisLabelPaddingBottom: 10,
                        axisLabelPaddingLeft: 10,
                        axisLabelPaddingTop: 10,
                        axisLabelPaddingRight: 10,
                        isShowAxisLine: false, //坐标轴线
                        axisLineColor: 'rgba(244, 244, 244, 1)',
                        axisLineWidth: 2,
                        axisLineType: 'solid',
                        isShowAxisTick: false, //刻度线
                        axisTickType: 'solid', //坐标刻度
                        axisTickColor: 'rgba(244, 244, 244, 1)',
                        axisTickWidth: 2, //坐标刻度
                        axisTickInside: false, //坐标刻度
                        axisTickLength: 6, //坐标刻度
                        isShowSplitLine: false, //分割线
                        splitLineType: 'dashed', //
                        splitLineWidth: 2, //分割线
                        splitLineColor: 'rgba(244, 244, 244, 1)',
                    },
                    top: 10,
                    bottom: 5,
                    left: 5,
                    right: 5,
                    fontColor: "#fff",
                    fontSize: 14,
                    barWidth: 20,
                    barBorderRadius: 20,
                    gradientsColorStart: "#3959FF",
                    gradientsColorEnd: "#2EC8CF",
                    backgroundColor: "#181F44",
                    backgroundData: 5000,
                    isThemeColor: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{
                            name: "大米",
                            value: 5000,
                        },
                        {
                            name: "玉米",
                            value: 2200,
                        },
                        {
                            name: "蔬菜",
                            value: 1000,
                        },
                        {
                            name: "鸡蛋",
                            value: 3000,
                        },
                        {
                            name: "坚果",
                            value: 1200,
                        },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "doubleDirectionBar",
                customId: "",
                layerName: "双向柱形图",
                icon: "el-icon-datav-doubleDirectionBar",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "",
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 15, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    labelTop: 15,
                    top: 2,
                    right: 2,
                    legendFontSize: 14,
                    showLegend: true,
                    position: "top",
                    fontColor: "#fff",
                    fontSize: 14,
                    letterSpacing: 10,
                    leftColorStart: "rgba(227,161,96,0.8)",
                    leftColorEnd: "rgba(227,161,96,0.3)",
                    rightColorStart: "rgba(0,222,255,0.8)",
                    rightColorEnd: "rgba(0,222,255,0.3)",
                    barWidth: 15,
                    showLabel: true,
                    isThemeColor: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [
                        { name: "2017", label: "A", value: 3 },
                        { name: "2017", label: "B", value: 12 },
                        { name: "2017", label: "C", value: 32 },
                        { name: "2017", label: "D", value: 23 },
                        { name: "2018", label: "A", value: 5 },
                        { name: "2018", label: "B", value: 14 },
                        { name: "2018", label: "C", value: 36 },
                        { name: "2018", label: "D", value: 30 },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "doubleBarTime",
                layerName: "动态双向柱状图",
                icon: "el-icon-datav-doublebartime",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    istitle: true,
                    title: "?月 标题",
                    subtitle: "",

                    //时间轴
                    isautoplay: true,
                    playinterval: 2000,
                    suffix: "月",
                    timetooltip: "?月份数据统计",
                    //柱状图
                    datasuffix: "人",
                    barwidth: 15,
                    isThemeColor: false,
                    leftColor: "#659F83",
                    leftfocusColor: "#08C7AE",

                    rightColor: "#F68989",
                    rightfocusColor: "#F94646",
                    //静态数据值
                    staticDataValue: [
                        { name: "男性", label: "10~20岁", value1: 30, value2: 50 },
                        { name: "男性", label: "20~30岁", value1: 12, value2: 12 },
                        { name: "男性", label: "30~40岁", value1: 32, value2: 32 },
                        { name: "女性", label: "10~20岁", value1: 69, value2: 30 },
                        { name: "女性", label: "20~30岁", value1: 12, value2: 12 },
                        { name: "女性", label: "30~40岁", value1: 32, value2: 32 }
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },

            {
                chartType: "cylinder",
                layerName: "立体柱",
                icon: "el-icon-datav-cylinder",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    width: 300,
                    istitle: false,
                    color1: "#363F5E", //上表面
                    color2: "#2B5B4D", //隔层
                    color3: "#01CC04", //下表面
                    color4: "#01CC04", //下柱

                    color5: "#FFFE00", //文字
                    suffix: "%",
                    size: 50,
                    family: "黑体",
                    weight: "normal",

                    color6: "#36405E", //上柱
                    isThemeColor: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ name: "数据1", value: 25 }],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                //比例柱图
                chartType: "rateBar",
                layerName: "比例柱状图",
                icon: "el-icon-datav-rateBar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    xAxis: {
                        axisLine: {
                            show: true, //隐藏X轴轴线
                            lineStyle: {
                                color: "#001C50",
                                width: 1,
                            },
                        },
                        axisTick: {
                            show: false, //隐藏X轴刻度
                        },
                        axisLabel: {
                            show: true,
                            margin: 10,
                            fontSize: 15,
                            textStyle: {
                                color: "#ffffff", //X轴文字颜色
                            },
                            fontWeight: 600,
                            fontFamily: "微软雅黑",
                        },
                    },
                    yAxis: {
                        type: "value",
                        gridIndex: 0,

                        splitLine: {
                            show: true,
                            lineStyle: {
                                color: "rgba(46,92,145,1)",
                                width: 1,
                                type: "dashed",
                            },
                        },
                        axisTick: {
                            show: false,
                        },
                        axisLine: {
                            show: false,
                            lineStyle: {
                                color: "#11417a",
                                width: 1,
                            },
                        },
                        axisLabel: {
                            show: true,
                            margin: 14,
                            fontSize: 10,
                            textStyle: {
                                color: "#ffffff", //X轴文字颜色
                            },
                        },
                    },
                    totalBar: {
                        barWidth: 20,
                        backgroundColor: "#001C50",
                        color: ["#49FFAA", "#02E8FC"],
                        completedColor: ["#2456C4", "#4C79D2"],
                    },
                    label: {
                        show: true,
                        distance: 15,
                        color: "#02E6FC",
                        fontSize: 16,
                        fontFamily: "Digital",
                        fontWeight: "bold",
                        padding: [0, 30, 20, 30],
                        backgroundShow: false,
                        backgroundImg: "",
                        imageHeight: 50,
                        imageWidth: 120,
                    },
                    splitBar: {
                        color: "#0F375F",
                        width: 20,
                        height: 3,
                        symbolMargin: 3,
                    },
                    max: 1000,
                    staticDataValue: [{
                            name: "营商",
                            value: 420,
                            total: 700,
                        },
                        {
                            name: "信用",
                            value: 630,
                            total: 900,
                        },
                        {
                            name: "环保",
                            value: 630,
                            total: 900,
                        },
                        {
                            name: "税务",
                            value: 440,
                            total: 800,
                        },
                    ],

                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //绑定的div
                    bindingDiv: "",
                    theme: "", //主题
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            //象形柱图
            {
                chartType: "pictorialBar",
                layerName: "象形柱图",
                icon: "el-icon-datav-pictorialBar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            fontStyle: "normal", // 风格
                            fontWeight: "normal", // 粗细
                            //fontFamily: 'Microsoft yahei',  // 字体
                            //fontSize: 20, // 大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    tooltip: {
                        //提示
                        trigger: "axis", //坐标轴触发tooltip
                        axisPointer: {
                            type: "none",
                        },
                        formatter: "{b} : {c}",
                    },
                    legend: {
                        show: true,
                        type: "scroll",
                        orient: "horizontal",
                        left: "center",
                        align: "left",
                        top: "top",
                        itemGap: 10, //图例间隔
                        itemWidth: 16,
                        itemHeight: 9,
                        textStyle: {
                            color: "#E7EAED",
                            fontSize: 14,
                        },
                        height: "50%",
                    },
                    xAxis: {
                        show: true,
                        name: "x轴",
                        nameLocation: "end",
                        nameTextStyle: {
                            fontSize: 16,
                            color: ["#fff"],
                        },
                        type: "category", //x值为category，y值为value时，图形纵向
                        boundaryGap: true, // 从0开始
                        //data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                        splitLine: {
                            show: false,
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        axisLabel: {
                            show: true,
                            fontSize: 14,
                            color: "#fff",
                            fontFamily: "微软雅黑",
                        },
                        axisTick: { show: false },
                        axisLine: {
                            show: true,
                            lineStyle: {
                                color: "#fff",
                                width: 1,
                            },
                        },
                    },
                    yAxis: {
                        show: true,
                        name: "y轴",
                        type: "value", //y值为category，x值为value时，图形横向
                        nameLocation: "end",
                        nameTextStyle: {
                            fontSize: 16,
                            color: ["#fff"],
                        },
                        splitLine: {
                            show: false,
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        axisLabel: {
                            show: true,
                            align: "right",
                            fontSize: 14,
                            color: "#fff",
                            fontFamily: "微软雅黑",
                        },
                        axisTick: { show: false },
                        axisLine: {
                            show: true,
                            lineStyle: {
                                color: "#fff",
                                width: 1,
                            },
                        },
                    },
                    series: [{
                        name: "系列名称",
                        type: "pictorialBar",
                        barGap: "20%",
                        label: {
                            show: true,
                            position: "top",
                            fontSize: 16,
                            fontFamily: "微软雅黑",
                            color: "#fff",
                        },
                        symbol: "image://data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAMgAAADICAYAAACtWK6eAAAAAXNSR0IArs4c6QAAFXpJREFUeF7tnXuQHNV1xr/Ts5LQTo8QSNMjYSBQgKXtEQ+HZwFhWQUQxraEnSLIZZuY2PEDHFd42C4KxxjHcbkwBKowkNjOAxsMTjkRQ9kmGoWHeQYIYEDTI4EcFAskTc8KwU6PFml3+qR6JRwQkra7px+3d85W6S/uOec7v3M/Znpm+jZB/oSAENgrARI2QkAI7J2AGER2hxDYBwExiGwPISAGkT0gBMIRkFeQcNwkqkcIiEF6ZNDSZjgCYpBw3CSqRwiIQXpk0NJmOAJikHDcJKpHCIhBemTQ0mY4AmKQcNwkqkcIiEF6ZNDSZjgCYpBw3CSqRwiIQXpk0NJmOAJikHDcIo8aaS5YkOPcaR3AJU17sjB3dT3yIpIwMAExSGBk0Qa0bPMCIlwLxsBumdeDeYVeql8ebUXJFoSAGCQIrYjXTpgD+Nd9pSXgibxhnRpxaUnnk4AYxCeoqJc5m8uXQeO/85tXNyyZlV9YEa4T6BHC9JvqzU3mSbkcnvS73lvHwGMFwzo9SIys7Z6AGKR7hoEzOLbJgYO8AKLr9GLta6FiJSgUATFIKGzhg9q2uY6BI8JmcF1aNmte7d6w8RIXjIAYJBivrlY7dvl2gC/qKon3dsvtKxXmvWB3m0fiJycgBpmcUSQr2k3zc8z4hyiSEdDOG5YeRS7JsW8CYpAEdkhrk1mmHFZHW4qrulFfEm1OybY7ATFIAnvCsc0xAH1RlyKia/LF2reiziv5/p+AGCTm3eDY5nMAjoutjIaz9LnW/bHl7/HEYpAYN0C7Wb6Vmb8YY4m3U8/XDWtzAnV6roQYJKaRO03zE2DcEVP6d6Vl5mahVDeSqNVrNcQgMUx8dOvCwzpj2isxpN5rSgKtyBu1jyVZsxdqiUFimLJjmw0Ayf8fnXClXrRuiKGlnk0pBol49E7TfACMoYjT+k6XI1o8s1h70HeALNwnATFIhBvEaZjXg3BFhClDpdK08YP75770WqhgCXoXATFIRBuiPTxwPru0IqJ03aVhrNdL1uHdJZFoj4AYJIJ9wK0ji+3R6Wr9Nopxp16yPhlBez2dQgwSwfgd21wDYEEEqSJNwaAvFYzaLZEm7bFkYpAuB96yB/6NQOp+vMp8tl6q/2eXbfZsuBiki9G3Gua1RPhGFykSCdVy2qH9c1ZvSKTYFCsiBgk5UKex8ByQtjJkeLJhhJpetBYlW3RqVBODhJjj5s3H5HVtfBOAQojwVEKY8YNCyfp8KsUzXFQMEmJ4rYb5BBFOCRGaaggDXy4Y1s2pishYcTFIwIE5dvlHAH8mYJg6y3PaEn3O6qo6gtRWIgYJMJ9tzYG/cpluDBCi5NKxjnv4AfPXrFdSnGKixCA+BzLSKJ+mET/qc7nSy4jwVL5onay0SEXEiUF8DsJpmP8LwqE+lyu/jIhuyxdrlygvNGWBYhAfA2g3zV8y4zwfS7O1xOXL9Hn1m7IlOlm1YpBJeDtN8zowvpLsWJKrRsB5ecO6L7mK2aokBtnHvBy7/GcA/0u2RhpY7Vgf5xbuV3rxfwJH9kCAGGQvQ3Zs0zuJ5Ok4jutRbl8RPaQXa6nd5KUcj3cIEoPs3SDxHtej2K6Qb9r3PBAxyB64OE3zDjA+odgejl8O4Qq9aPl+Zkn8gtKvIAbZbQaObV4F4DvpjyYdBUT84Xyx/st0qqtXVQzyjpmMbC4v1TSuqDemRBW90QGftL9RfznRqooWE4PsGswbjUVH9JH7eCrH9ai3OR7RDesM9WQlr0gMsot52sf1JD/6fVck0I/yRu0vVNOVtB4xCIC2bd7MwJeShq98Peav6qX695TXGaPAnjdIq2F+gQi3xcg406nJpWX5Hn7kW08bZNQ2T+8Aj2R6B8cvfljr4EP9862n4i+lXoWeNMj21wcW7RjHMgJ9SsXjetTbJhMnqD2oMVdoGldmHtA795L0jEHazYH53hNiibAUwAeV3IRZEMXYDkIFRJV8J1eheS+0syA7rMYpbRBmc7rTxE5T8IQxZoUFJXHvJUCEDWBUmPlevVRfNRUZTUmDtDaXF5OGpQQsZbCcUZvMzn2WiCrsoqKXas8nUzL+KlPGIE6jfKxnCoa7FEwnxI9OKuyVAGMlgyua5lbyxbUbs0wq0wbZ8uoxB0+fNraUiLy3T/JIZPV24oh3raK5qMw0ihWih8bVkzjZF6YZU7xhw8EzZ88oTJiC2HvFgJ6xFnpSLgO/Je96hbhSMOoPZwVCZl5BnMbAWROvFISlzPiDrAAWnXu4uAeeYFBlmqZVZsx90TsZX9k/pQ3Sts0P7DLEMgAfUJaiCAtNgBn3aoRKZ4dbmXXwmi2hE8UUqJxBRjeah3Iflrrwriv47Jj6lrTqEWh6n4IBVMkXV/9CFXlKGGTiMOhcx7vQXgrXXQaimaoAEh0pEGBY3peR3sV9/zzryRQU/L5kqgZxGovOIeosZZD3FurgNEFIbUUJED2kwa3scMfvnV16OfGTVxI3SLu58HhA8y60PVMcq+hYRJZqBAg7dn4KhkqeUSHDcpKQmIhBRjctPIxz2lLXMwVhcRKNSY2pTIBfY88k4IpeWhPrSfWxGYSbCwpt5CZeKbyffACYMZVHJr2lRuA5Iu/3YKjohvWbqFVEbhDHLp/LzG//avagqAVLPiGwdwJc9YySy3Uq/XNfei0KUpEYpD1cPtF1vVcK9l4pjo5CmOQQAl0QaE18CsZUmVncr0L0zFjYXKENsnXrcbOnbR+7GJq7FKAzwwqQOCEQJwEGvE++bgr76LlQBhlpDJyvAdeC6Jg4m5PcQiAyAoSfjqLzhWJxbStIzsAGcZrlH4PZu1VV/oRA1gg8u+Z3M0854QT/b7kCGWSbbf6pC/wsa1RErxD4PQHGFXrJ//nDgQzSss1HCThNcAuBDBNYD9ZO0UurG3568G0Q72chIHeln6SyRgioTIBI+4jfH0T6NkirObCcmO5SuXHRJgT8EHCBi2cZlq8nh/k2SLsxcAkT3eJHgKwRAkoTCHAd4t8gTfMaZnxT6cZFnBDwQYCZLymU6r6Om/VtkO1byuZYh2s+6ssSIaAygfHtuR1z5sxZN+JHpG+DeMna9sBjDDrVT2JZIwSUJMC4Qy9Zvr/HC2YQeZul5MxFlH8CHeIP7x/gEXOBDOLJcBrm/XJPh/+ByEqFCDDfqJfqlwdRFNgg214vn+aO84MApgUpJGuFQMoEnt2e2zHk99rjba2BDeIFjtjmVVoPPwk25UFL+RAEgnw5+M70oQwy8VbLNn8ljxEIMSkJSZ4A43q9ZH0lTOHQBnlzU/nEvhw/IEd/hsEuMUkRIMJT/XNnDhE9sy1MzdAG2XXBfjkIN4QpLDFCIAkCBJyXN6z7wtbqyiBe0XbTXMGM88MKkDghEBcBDfhuv2Fd1U3+rg3iNBYdA3IfADCnGyESKwSiJMDAE/qbO4boqHXbu8nbtUG84i27fCmBv9+NEIkVApESYHdJFGdmRWKQnZ9qle8G+MJIm5RkQiAEAQb9bcGofT1E6HtCIjPISHPBghxyDzBDzsKKYjKSIyyBR56uGYuHhqJ5mlVkBtl5wT7wWWb6YdjOJE4IdEuANfrjwtyad00cyV+kBpkwiV2+ncEXRaJOkgiBAASY8a1CybomQMikSyM3yOjWhYd1dmgPgnDYpNVlgRCIjAA/pBv1ocjS7UoUuUEmLtiHBz4Fl34ctVjJJwT2QoCZ3KFCcc2voyYUi0E8kS174IcE+mzUgiWfENidABF9I1+s/U0cZGIzSLs5MJ+ZvJ/FL4hDuOQUAhMEGPfrJeusuGjEZpBdryIXEujuuMRL3h4nQBjTXBrqL9Uei4tErAbxRLftge8z6NK4GpC8vUuAgKvzhvWdOAnEbpCRVxfO0abTA4CcBB/nIHsw90rdsM6Nu+/YDeI1MPG4BKIVcTcj+XuEAOOtDvPQ/vPq/xV3x4kYxGvCaZg3gBDohvm4m5f8GSWg8df0ufXrklCfmEFs29TzBO+3Wicm0ZjUmLIEfqUb1oeS6i4xg+y8YDc/yIB3L7v8CYHABAhwQNpQvrj6vwMHhwxI1CATb7Vs0/vUoau7vEL2KmFZJ0C4Ui9aid7inbhBmI+ftq056h32cHrW5yX6kyNAhHvzRWtZchV3VkrcIF7R1nB5Mbl8f9LNSr2sEuA3CTSUN6znku4gFYNMXI/IOb9Jzzq79Vy+TJ9XvymNBlIzyMT1iJzzm8bMM1WTQCvyRu1jaYlO1SByzm9aY89GXWa8TugM6aW1L6SlOFWDeE3LOb9pjV79ugz8ZcGwUj0tJ3WD7ProV875VX+/JqqQgZ8XDOuCRIvuoZgSBpFzftPeBsrVtzlHQ4U5NSttZUoYZNcFu5zzm/ZuUKR+kIdsxi1ZGYPs+uhXzvmNe+LK56e7daP2cVVkKmUQOedXlW2Rmo5NLnWGZhXXrk1NwW6FlTKIp03O+VVlaySvg4g+ly/WlDp4UDmD7PxUS875TX57plyRcadesj6Zsor3lFfSIHLOr2rbJF49RNiQm6YN7Td79W/jrRQ8u5IG2XnBLuf8Bh9nNiMI+EzesP5JRfXKGmTCJHLOr4p7JmJNfLtu1D8dcdLI0iltkIlzfsc072VXi6xjSaQQAerkOp0jZ85fs14hUe+SorRBdr6KmA4DeVUBiq6uCIzohrV/VxliDlbeIK2GOUwkzz+MeR+klX6TblhKP3BJeYM4trkBwMFpTVDqxkeAgHV5wzoqvgrdZ86CQV4CoDTE7sfQsxme1w3rOJW7V98gDfMFEI5WGaJoC0eAwI/njfpp4aKTiVLfIE3zSTBOSgaHVEmWAK3Sjdo5ydYMVk15g7Rs89cEnBGsLVmdBQJEuCdftD6qslblDeLY5n8AWKIyRNEWkgDhTr2o3u+v3tmN8gZpN025RyTk/lM9jAg/yBetz6usU3mDOPbAXQAtVxmiaAtJgPlGvVRX+sT/DBjE9H7EdnHIEUiYwgQI9O28UftrhSWmc/RoECCtRvlWIv5ikBhZmxkCV+mG9V2V1ar/CiIP3lF5/3SljYEvFwzr5q6SxBysvEHadvnbDL46Zg6SPgUCKt8H8jYO5Q3Ssge+TqBYHhKfwp6Qku8gwODlBaP+M5WhKG8Qp2leAcb1KkMUbeEIEGkfyRdX/yJcdDJRyhtETjlJZiOkUYWJFheKtQfTqO23pvIGadvmnzPwj34bknXZIaBpOLl/rvWUyoqVN4jTLH8czD9VGaJoC0eAc1hUmGPVwkUnE6W8QdrDA+ezSyuSwSFVkiSQ67iHq3w/usdCeYM4dvlcgO9LcnBSKxkC3NlhFOavayZTLVwV5Q3Sai4cJNYeCteeRKlMID8+M08HPbNNZY3KG2TbJvMkN4cnVYYo2kIRGNcNa1qoyASDlDeIs3nh0dC01J5Rl+AseqoUg98sGPXZqjetvEHesstHjoNfVh2k6AtMYKNuWO8LHJVwgPIG2Tb8/ve5bt+rCXORcjETYODlgmG9P+YyXadX3iAjry6co03XhrvuVBKoRuA53bD+UDVRu+tR3iAbNx7fP6tvtK06SNEXjAABj+YN64+CRSW/WnmDMIPaTbOThe9skh9fpiuu1A3rXNU7UN4gHsC2bbYZ6FcdpujzT4DB/14w6n/iPyKdlZkwiNMwt4BwYDqIpGosBJh+opdqF8WSO8Kk2TCIbXqfYin/kWCEc5nyqZjx94WSpfxZA5kwSMs21xFwxJTfNb3UIOMGvWRdqXrLmTCIY5svAlikOkzR558AEa7NF61v+o9IZ2UmDNJumk8x48R0EEnVWAgQf1Uv1r8XS+4Ik2bCIC3bfJgA5T8zj3AuUz4VMV+aL9VvVb3RTBjEscsrAVb6mHzVB62ePvq0btRuV0/XuxVlwiDtpnkPM5apDlP0+SfAoAsKRu3n/iPSWZkJg7Ts8t0EvjAdRFI1DgIEnJc3LOXvFM2EQRzb/GcAyj5sPo4NNNVz8jgPFg6qP6x6n5kwSKtZvpVYDrBWfTMF0UfEx+eL9WeDxKSxNhMGceQA6zT2Rqw1+6izcL/i2rWxFokgeSYMIgdYRzBpxVJoub5D+ue8oPyNcBkxyMDVDPq2YjOOTg5hKzNWMbiaA+AynQvCEgIK0RVRK1N+eu5Amv3iVrVUvVdNJgziNMwrQFPuAOuHCVyFplXzc2tP72mjjDQWnZqDuwQaljDjZNU3UxB9+SJmEFk7gsSksTYTBmk1Bi4holvSABRVTe8ebM0zBGnVfqddpcPXvxUkt3frMfXR2aSR98Rf79/8IPGKrR3TDWu6Ypr2KCcTBnFs03tGofeswiz9tQiouoxVHe5UZ89b+0qU4p1G+Vho7jns0rlEWBxl7rhzMfBGwbAOiLtOFPkzYZBWc2A5Md0VRcNx5iDw4yCqkuZW++eseSLOWu/M3WwuKPRj2iDYXcI7X12OSqp2mDpEeDVftA4JE5t0TCYM0h42l7GLe5KGM2k95lcYVAXc6ihpVcOwnEljEljQ2lQ2oblnArSEaMIwMxIoG6TEWt2wFgYJSGttJgzibFl0DjruyrQgvV2XQKMg7zqCq+OjWLX/IXXlD7RjPnJG295vEOgsBk1cvxyXNkcAz+qGdbwCOiaVkA2DvL7oGIy7z0/aTRwLGE8xUbUPXJ1pWI/EUSLJnG/a5SP74A56T3diF971Sxr3+j+iG9YZSfYdtlYmDOI159jmZgClsI36jSPC75hp4juJ8WnTqwcc8Js3/MZmbZ13pNJbw4sGXbiDYJzFwOmJ9EB0k16sXZZIrS6LZMgg5bsAXt5lv+8NJ4yBqep9BJvTctUZc19cE3mNjCQcfd08tDPGgyBtEMxng3BoHNJd5o/OKtXVu6bcQ7OZMUirWV5OzFF9kuX9SK7qvXVS/SGScWxQvzlHGuXTQDyogQcBiuqGtZfHpk0/KSuvzJkxiDfUsE+8JWDjhCFcWuW6Y9VZB70kZ/36dcmudY5tzvOMwqQNsmcYhhkwxc7l7C7RS2uqoWJTCMqUQTw+bdvX77LYMwQIVc5xtXBgfXUKbKd0yfZw+UTueG/HeJBAZzKgT9YwaXxJfm79tsnWqfTfM2cQD972LWVzx7h7KRFdAmCYwVs00DAzngC4qpfqq1SCPNW1TPwMZhomrl0IGARw7Lt6JvrJuNu5Y3aGXjne1p9Jg0z1DZf1/hzbPA4un0m53Dqg80y+WN+U1Z7EIFmdnOhOhIAYJBHMUiSrBMQgWZ2c6E6EgBgkEcxSJKsExCBZnZzoToSAGCQRzFIkqwTEIFmdnOhOhIAYJBHMUiSrBMQgWZ2c6E6EgBgkEcxSJKsExCBZnZzoToSAGCQRzFIkqwTEIFmdnOhOhMD/AZ0v3RTOUf0QAAAAAElFTkSuQmCC",
                        symbolRepeat: "fixed",
                        symbolClip: true,
                        symbolSize: [30, 30],
                        color: "#fff",
                        barCategoryGap: "40%",
                        z: 2,
                    }, ],
                    max: 100,
                    //自定义图案
                    imageList: [],
                    //显示占比
                    showAll: false,
                    //占比阴影偏移
                    symbolOffset: [0, 0],
                    //图形朝向
                    barOrient: "纵向",
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    // staticDataValue: [
                    //   {
                    //     series: "2011年",
                    //     data: [
                    //       {
                    //         name: "reindeer",
                    //         value: 57
                    //       },
                    //       {
                    //         name: "ship",
                    //         value: 21
                    //       },
                    //       {
                    //         name: "plane",
                    //         value: 66
                    //       },
                    //       {
                    //         name: "train",
                    //         value: 78
                    //       },
                    //       {
                    //         name: "car",
                    //         value: 83
                    //       }
                    //     ]
                    //   },
                    //   {
                    //     series: "2012年",
                    //     data: [
                    //       {
                    //         name: "reindeer",
                    //         value: 66
                    //       },
                    //       {
                    //         name: "ship",
                    //         value: 41
                    //       },
                    //       {
                    //         name: "plane",
                    //         value: 55
                    //       },
                    //       {
                    //         name: "train",
                    //         value: 23
                    //       },
                    //       {
                    //         name: "car",
                    //         value: 33
                    //       }
                    //     ]
                    //   }
                    // ],
                    staticDataValue: [{
                            series: "2011年",
                            name: "reindeer",
                            value: 57
                        },
                        {
                            series: "2011年",
                            name: "ship",
                            value: 21
                        },
                        {
                            series: "2011年",
                            name: "plane",
                            value: 66
                        },
                        {
                            series: "2011年",
                            name: "train",
                            value: 78
                        },
                        {
                            series: "2011年",
                            name: "car",
                            value: 83
                        },
                        {
                            series: "2012年",
                            name: "reindeer",
                            value: 66
                        },
                        {
                            series: "2012年",
                            name: "ship",
                            value: 41
                        },
                        {
                            series: "2012年",
                            name: "plane",
                            value: 55
                        },
                        {
                            series: "2012年",
                            name: "train",
                            value: 23
                        },
                        {
                            series: "2012年",
                            name: "car",
                            value: 33
                        }
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
        ],
    },

    // 折线图插件
    {
        chartType: "line",
        layerName: "折线图",
        icon: "el-icon-datav-line",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "line",
                layerName: "基本折线图",
                icon: "el-icon-datav-line",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            fontStyle: "normal", // 风格
                            fontWeight: "normal", // 粗细
                            //fontFamily: 'Microsoft yahei',  // 字体
                            //fontSize: 20, // 大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    legend: {
                        show: true, // 是否显示图例
                        type: "scroll",
                        x: "center",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    tooltip: {
                        trigger: "axis",
                    },
                    xAxis: {
                        name: "x轴",
                        type: "category",
                        boundaryGap: false, // 从0开始
                        //data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        show: true,
                    },
                    yAxis: {
                        name: "y轴",
                        type: "value",
                        /*splitLine: {
                                    lineStyle: {
                                        // 使用深浅的间隔色
                                        color: ['#fff']
                                    }
                                },
                                nameTextStyle: {
                                    color: ['#fff']
                                },*/
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                                width: 1, //这里是为了突出显示加上的
                            },
                        },
                        splitLine: {
                            show: true,
                        },
                        show: true,
                    },

                    //折线的新增属性
                    lineSymbolSize: 8,
                    lineWidth: 3,

                    // y坐标轴新增属性
                    yAxisLineShow: true,
                    yAxisLineWidth: 1,
                    yAxisLineColor: "#fff",
                    yAxisFontSize: 14,
                    // yAxisFontColor:'#fff',
                    // yAxisFontFamily:'微软雅黑',
                    yAxisnameLocation: "end",
                    yLabelShow: true,
                    yLabelFontSize: 14,
                    yLabelFontColor: "#fff",
                    yLabelFontFamily: "微软雅黑",

                    // x坐标轴新增属性
                    xAxisLineShow: true,
                    xAxisLineWidth: 1,
                    xAxisLineColor: "#fff",
                    xAxisFontSize: 14,
                    // xAxisFontColor:'#fff',
                    // xAxisFontFamily:'微软雅黑',
                    xAxisnameLocation: "end",
                    xLabelShow: true,
                    xLabelFontSize: 14,
                    xLabelFontColor: "#fff",
                    xLabelFontFamily: "微软雅黑",

                    //图例属性
                    legendOrient: "horizontal",
                    legendX: 30,
                    legendY: 0,
                    legendFontSize: 14,
                    itemWidth: 14,
                    itemHeight: 8,

                    //分割线属性
                    splitLineType: "solid",
                    splitLineWidth: 2,
                    splitLineColor: "#ccc",

                    //是否显示平均线
                    isMarkLine: false,
                    //是否显示最大值和最小值
                    isMarkPoint: false,
                    //折线是否平滑
                    isSmooth: true,
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{
                            name: "苹果",
                            value: 879,
                            type: "手机品牌",
                        },
                        {
                            name: "三星",
                            value: 340,
                            type: "手机品牌",
                        },
                        {
                            name: "小米",
                            value: 230,
                            type: "手机品牌",
                        },
                        {
                            name: "oppo",
                            value: 540,
                            type: "手机品牌",
                        },
                        {
                            name: "vivo",
                            value: 340,
                            type: "手机品牌",
                        },
                    ],

                    //color: ['#00CFFF', '#006CED', '#FFE000', '#FFA800', '#FF5B00', '#FF3000'],
                    //绑定的div
                    bindingDiv: "",
                    theme: "", //主题
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "shadowLine",
                layerName: "阴影折线图",
                icon: "el-icon-datav-shadowline",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    lineStyle: "dashed",
                    legend: {
                        show: true,
                        type: "scroll",
                        x: "center",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    tooltip: {
                        trigger: "axis",
                    },
                    xAxis: {
                        name: "x轴",
                        //type: "category",
                        boundaryGap: false, // 从0开始
                        //data: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'],
                        show: true,
                        axisLabel: {
                            // show: true,
                            textStyle: {
                                color: "#fff",
                                fontSize: 10,
                            },
                        },
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        axisTick: {
                            show: true,
                            alignWithLabel: true,
                        },
                    },
                    yAxis: {
                        name: "y轴",
                        type: "value",
                        splitNumber: 5,
                        axisLabel: {
                            textStyle: {
                                color: "#fff",
                                fontSize: 10,
                            },
                        },
                        axisLine: {
                            lineStyle: {
                                color: "transparent",
                            },
                        },

                        splitLine: {
                            show: true,
                            lineStyle: {
                                color: "#B4B3C0",
                                type: "dashed",
                            },
                        },
                        show: true,
                    },
                    //折线是否平滑
                    isSmooth: true,
                    //是否显示平均线
                    isMarkLine: false,
                    //是否显示最大值和最小值
                    isMarkPoint: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{
                            name: "苹果",
                            value: 879,
                            type: "手机品牌",
                        },
                        {
                            name: "三星",
                            value: 340,
                            type: "手机品牌",
                        },
                        {
                            name: "小米",
                            value: 230,
                            type: "手机品牌",
                        },
                        {
                            name: "oppo",
                            value: 540,
                            type: "手机品牌",
                        },
                        {
                            name: "vivo",
                            value: 340,
                            type: "手机品牌",
                        },
                    ],
                    // color: ['#00CFFF', '#006CED', '#FFE000', '#FFA800', '#FF5B00', '#FF3000'],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "rain",
                layerName: "雨量流量关系图",
                icon: "el-icon-datav-rain",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "",
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "",
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        //itemGap:7,
                        left: "center",
                        align: "right",
                    },
                    backgroundColor: "#101129",
                    //color: ['#37a2da', '#8378ea', '#fb7293', '#e7bcf3', '#ffdb5c', '#32c5e9', '#9fe6b8', '#ff9f7f'],
                    grid: {
                        bottom: 80,
                        left: 50,
                        right: 45,
                    },
                    toolbox: {
                        //top: '80%',
                        x: "80%",
                        feature: {
                            dataZoom: {
                                yAxisIndex: "none",
                            },
                            restore: {},
                            saveAsImage: {},
                        },
                        show: true,
                    },
                    tooltip: {
                        trigger: "axis",
                        axisPointer: {
                            type: "cross",
                            animation: false,
                            label: {
                                backgroundColor: "#505765",
                            },
                        },
                    },
                    legend: {
                        show: true,
                        type: "scroll",
                        x: "5%",
                        //orient: 'vertical',
                        //left:'0%',
                        align: "left",
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    dataZoom: [{
                            show: true,
                            realtime: true,
                            start: 65,
                            end: 85,
                        },
                        {
                            type: "inside",
                            realtime: true,
                            start: 65,
                            end: 85,
                        },
                    ],
                    xAxis: [{
                        type: "category",
                        boundaryGap: false,
                        axisLine: { onZero: false },
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        data: null,
                        show: true,
                    }, ],
                    yAxis: [{
                            name: "流量(m^3/s)",
                            type: "value",
                            max: 2,
                            alignTicks: true,
                            axisLine: {
                                lineStyle: {
                                    color: "#fff",
                                },
                            },
                            splitLine: {
                                show: true,
                            },
                            show: true,
                        },
                        {
                            name: "降雨量(mm)",
                            nameLocation: "start",
                            max: 2,
                            type: "value",
                            inverse: true,
                            alignTicks: true,
                            axisLine: {
                                lineStyle: {
                                    color: "#fff",
                                },
                            },
                            splitLine: {
                                show: true,
                            },
                            show: true,
                        },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    // staticDataValue: [
                    //   {
                    //     name: '流量',
                    //     data: [
                    //       {
                    //         name: '2009/6/12 1:00',
                    //         value: '0.97'
                    //       },
                    //       {
                    //         name: '2009/6/12 2:00',
                    //         value: '0.56'
                    //       },
                    //       {
                    //         name: '2009/6/12 3:00',
                    //         value: '0.67'
                    //       },
                    //       {
                    //         name: '2009/6/12 4:00',
                    //         value: '0.76'
                    //       },
                    //       {
                    //         name: '2009/6/12 5:00',
                    //         value: '0.87'
                    //       },
                    //       {
                    //         name: '2009/6/12 6:00',
                    //         value: '0.36'
                    //       },
                    //       {
                    //         name: '2009/6/12 7:00',
                    //         value: '0.47'
                    //       },
                    //       {
                    //         name: '2009/6/12 8:00',
                    //         value: '0.66'
                    //       },
                    //       {
                    //         name: '2009/6/12 9:00',
                    //         value: '0.76'
                    //       },
                    //       {
                    //         name: '2009/6/12 10:00',
                    //         value: '0.37'
                    //       },
                    //     ]
                    //   },
                    //   {
                    //       name: '雨量',
                    //       data: [
                    //         {
                    //           name: '2009/6/12 1:00',
                    //           value: '0.97'
                    //         },
                    //         {
                    //           name: '2009/6/12 2:00',
                    //           value: '0.56'
                    //         },
                    //         {
                    //           name: '2009/6/12 3:00',
                    //           value: '0.67'
                    //         },
                    //         {
                    //           name: '2009/6/12 4:00',
                    //           value: '0.92'
                    //         },
                    //         {
                    //           name: '2009/6/12 5:00',
                    //           value: '0.8'
                    //         },
                    //         {
                    //           name: '2009/6/12 6:00',
                    //           value: '1.045'
                    //         },
                    //         {
                    //           name: '2009/6/12 7:00',
                    //           value: '0.65'
                    //         },
                    //         {
                    //           name: '2009/6/12 8:00',
                    //           value: '0.68'
                    //         },
                    //         {
                    //           name: '2009/6/12 9:00',
                    //           value: '0.9'
                    //         },
                    //         {
                    //           name: '2009/6/12 10:00',
                    //           value: '0.34'
                    //         },
                    //       ]
                    //   }
                    // ],
                    staticDataValue: [{
                            series: '流量',
                            name: '2009/6/12 1:00',
                            value: '0.97'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 2:00',
                            value: '0.56'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 3:00',
                            value: '0.67'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 4:00',
                            value: '0.76'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 5:00',
                            value: '0.87'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 6:00',
                            value: '0.36'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 7:00',
                            value: '0.47'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 8:00',
                            value: '0.66'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 9:00',
                            value: '0.76'
                        },
                        {
                            series: '流量',
                            name: '2009/6/12 10:00',
                            value: '0.37'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 1:00',
                            value: '0.97'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 2:00',
                            value: '0.56'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 3:00',
                            value: '0.67'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 4:00',
                            value: '0.92'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 5:00',
                            value: '0.8'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 6:00',
                            value: '1.045'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 7:00',
                            value: '0.65'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 8:00',
                            value: '0.68'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 9:00',
                            value: '0.9'
                        },
                        {
                            series: '雨量',
                            name: '2009/6/12 10:00',
                            value: '0.34'
                        },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
        ],
    },

    // 饼图插件
    {
        chartType: "pie",
        layerName: "饼图",
        icon: "el-icon-datav-pie",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "pie",
                layerName: "基本饼图",
                icon: "el-icon-datav-pie",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    bgImage: '', //整体组件背景
                    containerBgColor: '', //整体组件背景
                    containerRadius: 0, //容器的弧度
                    titletextType: '0', //标题显示类型
                    subtextType: '0', //副标题显示类型
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            // fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 20, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                        top: 44,
                        textAlign: 'center',
                        left: 49,
                    },
                    otherTitletextType: '0', //第二标题显示类型
                    otherTitleSubtextType: '0', //第二标题显示类型
                    otherTitle: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            // fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            fontSize: 20, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                        top: 44,
                        textAlign: 'center',
                        left: 49,
                    },
                    tooltip: {
                        //提示
                        trigger: "item", //饼图
                        formatter: "{b} : {c} ({d}%)",
                    },
                    //color: ['#37a2da','#32c5e9','#9fe6b8','#ffdb5c','#ff9f7f','#fb7293','#e7bcf3','#8378ea'],
                    legend: {
                        show: true,
                        type: "scroll",
                        orient: "vertical",
                        right: "10%",
                        align: "left",
                        top: "middle",
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                        itemWidth: 20,
                        itemHeight: 20,
                        icon: 'circle', //图例形状
                        isShowVal: false,
                    },
                    series: [{
                        name: "系列名称",
                        type: "pie",
                        radius: "55%",
                        center: ["35%", "50%"], //饼图的位置
                        roseType: false, //南丁格尔玫瑰
                        stillShowZeroSum: true, //不显示零
                        emphasis: {
                            itemStyle: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: "rgba(0, 0, 0, 0.5)",
                            },
                        },
                        label: {
                            show: false,
                        },
                    }, ],
                    //新增属性
                    bgColor: ['#fc8251', '#5470c6', '#9A60B4', '#ef6567', '#f9c956'],
                    labelFontSize: 14,
                    labelFontColor: '#ddd',
                    labelLineWidth: 2, //新增引导线宽度
                    labelLineLength: 15,
                    labelLineLength2: 10,
                    labelFormatter: ["类别名称"],
                    formatterSuffix: "", //数值标签的后缀
                    legendCols: 1,
                    legendPositionLeft: 60,
                    legendPositionTop: 30,
                    legendItemGapX: 10,
                    legendItemGapY: 8,
                    legendFontSize: 14,
                    showPercentage: false,
                    textWidth: 100,
                    //内环阴影
                    innerShadow: false,
                    //内环半径比例
                    innerRadiusScale: 0.3,
                    //内环透明度
                    innerRadiusOpacity: 0.75,
                    //内半径
                    innerRadius: 50,
                    //外半径
                    outerRadius: 70,
                    //isLabel :"false",
                    //南丁格尔玫瑰
                    isRoseType: false,
                    //是否环饼
                    isRing: false,
                    isFan: false,
                    startAngle: "0",
                    endAngle: "180",

                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [
                        { value: 335, name: "直接访问" },
                        { value: 310, name: "邮件营销" },
                        { value: 234, name: "联盟广告" },
                        { value: 135, name: "视频广告" },
                        { value: 1548, name: "搜索引擎" },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "ringPie",
                layerName: "环饼图",
                icon: "el-icon-datav-ringPie",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "交通方式", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "数据来自网络", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    legend: {
                        show: true, // 是否显示图例
                        icon: "circle",
                        orient: "horizontal", //垂直horizontal  水平level
                        x: "right",
                        y: "top",
                        textStyle: {
                            color: "#fff",
                        },
                        itemGap: 20,
                    },
                    fontColor: "#00ffff",
                    fontSize: 22,
                    length: 20,
                    length2: 30,
                    //内半径
                    innerRadius: 80,
                    //外半径
                    outerRadius: 90,
                    //内环线宽度
                    innerPieWidth: 130,
                    //静态数据值
                    staticDataValue: [
                        { name: "火车", value: 20 },
                        { name: "飞机", value: 10 },
                        { name: "客车", value: 30 },
                        { name: "轮渡", value: 40 },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],

                    //是否查看详情1/0
                    isView: "0",
                    //查看详情接口地址
                    viewURL: "",
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
        ],
    },

    //漏斗图插件
    {
        chartType: "funnel",
        layerName: "漏斗图",
        icon: "el-icon-datav-funnel",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "", //主标题
                textStyle: {
                    color: "#fff", //颜色
                    fontStyle: "normal", //风格
                    fontWeight: "normal", //粗细
                    //fontFamily: 'Microsoft yahei',   //字体
                    //fontSize: 20,     //大小
                    align: "center", //水平对齐
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontSize: 14,
                    align: "center",
                },
                itemGap: 7,
            },
            tooltip: {
                //提示
                trigger: "item", //饼图
                formatter: "{b} : {c} ({d}%)",
            },
            legend: {
                show: true,
                type: "scroll",
                orient: "vertical",
                right: "10%",
                align: "left",
                top: "middle",
                textStyle: {
                    color: "#E7EAED",
                },
                height: 150,
            },
            series: [{
                name: "系列名称",
                type: "funnel",
                maxSize: "120%",
                //top: '20%',
                right: "50%",
                // sort: 'descending',
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: "rgba(0, 0, 0, 0.5)",
                    },
                },
            }, ],

            sort: "descending",
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [
                { value: 335, name: "直接访问" },
                { value: 310, name: "邮件营销" },
                { value: 234, name: "联盟广告" },
                { value: 135, name: "视频广告" },
                { value: 154, name: "搜索引擎" },
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },

    //仪表盘插件
    {
        chartType: "gauge",
        layerName: "仪表盘",
        icon: "el-icon-datav-gauge",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "gauge",
                layerName: "基本仪表盘",
                icon: "el-icon-datav-gauge",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    tooltip: {
                        //提示
                        trigger: "item", //饼图
                        formatter: "{b} : {c}%",
                    },
                    series: [{
                        name: "系列名称",
                        type: "gauge",
                        radius: "70",
                        min: "0",
                        max: "100",
                        startAngle: "225",
                        endAngle: "-45",
                        clockwise: true,
                        splitNumber: 10,
                        axisLine: {
                            //表盘
                            show: true,
                            lineStyle: {
                                width: 15,
                                color: [
                                    [1, '#E6EBF8']
                                ],
                            },
                        },
                        splitLine: {
                            //分割线
                            show: true,
                            length: "10",
                            lineStyle: {
                                color: "#63677A",
                                width: "1.5",
                                type: "solid",
                            },
                        },
                        axisTick: {
                            //刻度
                            show: true,
                            splitNumber: "5",
                            length: "5",
                            lineStyle: {
                                color: "",
                                width: "1",
                                type: "solid",
                            },
                        },
                        detail: {
                            formatter: '{value}'
                        },
                        axisLabel: {
                            //刻度值
                            show: true,
                            distance: "12",
                            color: "#fff",
                            fontSize: "17",
                        },
                    }, ],
                    isGradients: false,
                    isClockwise: true, //是否逆时针表盘
                    isAxisLineShow: true, //是否显示表盘轴线
                    isSplitLineShow: true, //是否显示分割线
                    isAxisTickShow: true, //是否显示刻度线
                    isAxisLabelShow: true, //是否显示刻度值
                    radius: "100", //百分比半径
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ name: "数据1", value: 10 }],
                    thiscolor: [
                        [0.2, "#893448"],
                        [0.8, "#d95850"],
                        [1, "#eb8146"]
                    ],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "keygauge",
                layerName: "键盘仪表盘",
                icon: "el-icon-datav-keygauge",
                customId: "",
                width: 350,
                height: 250,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    color: [
                        [0.2, "#893448"],
                        [0.8, "#d95850"],
                        [1, "#eb8146"],
                    ],
                    startAngle: "180", //起始角度
                    endAngle: "0", //终止角度
                    min: "0", //最小值
                    max: "100", //最大值
                    radius: "150", //半径(绝对值)
                    width: "7", //刻度值宽度
                    length: "17", //刻度值长度
                    center: ["50%", "70%"], //仪表盘位置
                    splitNumber: "40", //被分割线分出的段数，用于确定刻度值的个数

                    isdetail: true,
                    detailOffset: [0, -40],
                    detailColor: "#fff",
                    detailFamily: "黑体",
                    detailSize: "45",
                    detailWeight: "normal",

                    noColor: "#fff",
                    isinline: false,
                    inlineColor: "#8D8D8D",
                    inlineWidth: "3",
                    inlineRadius: "130", //仪表盘内侧弧线
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ name: "数据1", value: 80 }],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            //进度仪表盘插件
            {
                chartType: "progressGauge",
                layerName: "进度仪表盘",
                icon: "el-icon-datav-progauge",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    //标签
                    label: {
                        fontSize: 22,
                        color: "rgba(0,254,240,1)",
                        fontFamily: "Digital",
                        fontWeight: "normal",
                    },
                    title: {
                        text: "统计",
                        fontSize: 20,
                        color: "#fff",
                        fontFamily: "宋体",
                        fontWeight: "normal",
                        top: 50, //标题上边距
                        left: 42, //标题左边距
                    },
                    isValue: true, //显示数值
                    offsetCenter: [0, -18], //数值位置
                    splitNumber: 50, //分隔线数量
                    surplusColor: "#0a1f4d", //剩余部分颜色
                    processColor: ["#256BC5", "#52EFCA"], //进度条颜色
                    progressWidth: 10, //进度条宽度
                    splitLineColor: "#020f18", //分隔线颜色
                    splitLineWidth: 2, //分隔线宽度
                    outRadius: [76, 78], //外环半径
                    outColor: "#7ED0F6", //外环颜色
                    innerRadius: [51, 53], //外环半径
                    innerColor: "#7ED0F6", //外环颜色

                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ name: "数据1", value: 80 }],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
        ],
    },

    //散点图插件
    {
        chartType: "scatter",
        layerName: "散点图",
        icon: "el-icon-datav-scatter",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "", // 主标题
                textStyle: {
                    color: "#fff", // 颜色
                    fontStyle: "normal", // 风格
                    fontWeight: "normal", // 粗细
                    //fontFamily: 'Microsoft yahei',   // 字体
                    //fontSize: 20,     // 大小
                    align: "center", // 水平对齐
                },
                subtext: "", // 副标题
                subtextStyle: {
                    // 对应样式
                    color: "#fff",
                    fontSize: 14,
                    align: "center",
                },
                itemGap: 7,
            },
            color: "rgb(204, 46, 72)",
            symbolSize: 20,
            xAxis: {
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    },
                },
                splitLine: {
                    show: true,
                },
                show: true,
            },
            yAxis: {
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    },
                },
                splitLine: {
                    show: true,
                },
                show: true,
            },
            isSplitLine: true,
            //静态数据值
            staticDataValue: [{
                    x: 1,
                    y: 8.04,
                    value: 20,
                    type: "Bor",
                    series: "2020",
                },
                {
                    x: 2,
                    y: 6.95,
                    value: 15,
                    type: "Lon",
                    series: "2020",
                },
                {
                    x: 10,
                    y: 8.04,
                    value: 50,
                    type: "Bor",
                    series: "2021",
                },
                {
                    x: 2,
                    y: 26.95,
                    value: 75,
                    type: "Lon",
                    series: "2021",
                },
            ],

            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    //热力图插件
    {
        chartType: "heatMap",
        layerName: "热力图",
        icon: "el-icon-datav-heatMap",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                show: true,
                text: "",
                textStyle: {
                    color: "#fff",
                    fontSize: 24,
                    fontFamily: "微软雅黑",
                    align: "center",
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontSize: 20,
                    fontFamily: "微软雅黑",
                    align: "center",
                },
            },
            tooltip: {
                position: "top",
            },
            xAxis: {
                type: "category",
                data: [],
                axisLine: {
                    show: false,
                },
                axisLabel: {
                    textStyle: {
                        fontSize: 20,
                        color: "#fff",
                    },
                },
                splitArea: {
                    show: true,
                },
            },
            yAxis: {
                type: "category",
                data: [],
                axisLabel: {
                    textStyle: {
                        fontSize: 20,
                        color: "#fff",
                    },
                },
                axisLine: {
                    show: false,
                },
                splitArea: {
                    show: true,
                },
            },
            visualMap: {
                min: 0,
                max: 10,
                show: true,
                calculable: true,
                orient: "vertical",
                left: "right",
                bottom: "3%",
                inRange: {
                    color: ["#f6efa6", "#bf444c"],
                },
                textStyle: {
                    color: "#FFF",
                    fontSize: 18,
                },
            },
            series: [{
                name: "",
                type: "heatmap",
                data: [],
                label: {
                    show: true,
                    color: "#000",
                    fontSize: 18,
                },
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowColor: "rgba(0, 0, 0, 0.5)",
                    },
                },
            }, ],
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                xAxisData: [
                    '12a', '1a', '2a', '3a', '4a', '5a', '6a',
                    '7a', '8a', '9a', '10a', '11a',
                    '12p', '1p', '2p', '3p', '4p', '5p',
                    '6p', '7p', '8p', '9p', '10p', '11p'
                ],
                yAxisData: [
                    'Saturday', 'Friday', 'Thursday',
                    'Wednesday', 'Tuesday', 'Monday', 'Sunday'
                ],
                data: [
                    [0, 0, 5],
                    [0, 1, 1],
                    [0, 2, 0],
                    [0, 3, 0],
                    [0, 4, 0],
                    [0, 5, 0],
                    [0, 6, 0],
                    [0, 7, 0],
                    [0, 8, 0],
                    [0, 9, 0],
                    [0, 10, 0],
                    [0, 11, 2],
                    [0, 12, 4],
                    [0, 13, 1],
                    [0, 14, 1],
                    [0, 15, 3],
                    [0, 16, 4],
                    [0, 17, 6],
                    [0, 18, 4],
                    [0, 19, 4],
                    [0, 20, 3],
                    [0, 21, 3],
                    [0, 22, 2],
                    [0, 23, 5],
                    [1, 0, 7],
                    [1, 1, 0],
                    [1, 2, 0],
                    [1, 3, 0],
                    [1, 4, 0],
                    [1, 5, 0],
                    [1, 6, 0],
                    [1, 7, 0],
                    [1, 8, 0],
                    [1, 9, 0],
                    [1, 10, 5],
                    [1, 11, 2],
                    [1, 12, 2],
                    [1, 13, 6],
                    [1, 14, 9],
                    [1, 15, 11],
                    [1, 16, 6],
                    [1, 17, 7],
                    [1, 18, 8],
                    [1, 19, 12],
                    [1, 20, 5],
                    [1, 21, 5],
                    [1, 22, 7],
                    [1, 23, 2],
                    [2, 0, 1],
                    [2, 1, 1],
                    [2, 2, 0],
                    [2, 3, 0],
                    [2, 4, 0],
                    [2, 5, 0],
                    [2, 6, 0],
                    [2, 7, 0],
                    [2, 8, 0],
                    [2, 9, 0],
                    [2, 10, 3],
                    [2, 11, 2],
                    [2, 12, 1],
                    [2, 13, 9],
                    [2, 14, 8],
                    [2, 15, 10],
                    [2, 16, 6],
                    [2, 17, 5],
                    [2, 18, 5],
                    [2, 19, 5],
                    [2, 20, 7],
                    [2, 21, 4],
                    [2, 22, 2],
                    [2, 23, 4],
                    [3, 0, 7],
                    [3, 1, 3],
                    [3, 2, 0],
                    [3, 3, 0],
                    [3, 4, 0],
                    [3, 5, 0],
                    [3, 6, 0],
                    [3, 7, 0],
                    [3, 8, 1],
                    [3, 9, 0],
                    [3, 10, 5],
                    [3, 11, 4],
                    [3, 12, 7],
                    [3, 13, 14],
                    [3, 14, 13],
                    [3, 15, 12],
                    [3, 16, 9],
                    [3, 17, 5],
                    [3, 18, 5],
                    [3, 19, 10],
                    [3, 20, 6],
                    [3, 21, 4],
                    [3, 22, 4],
                    [3, 23, 1],
                    [4, 0, 1],
                    [4, 1, 3],
                    [4, 2, 0],
                    [4, 3, 0],
                    [4, 4, 0],
                    [4, 5, 1],
                    [4, 6, 0],
                    [4, 7, 0],
                    [4, 8, 0],
                    [4, 9, 2],
                    [4, 10, 4],
                    [4, 11, 4],
                    [4, 12, 2],
                    [4, 13, 4],
                    [4, 14, 4],
                    [4, 15, 14],
                    [4, 16, 12],
                    [4, 17, 1],
                    [4, 18, 8],
                    [4, 19, 5],
                    [4, 20, 3],
                    [4, 21, 7],
                    [4, 22, 3],
                    [4, 23, 0],
                    [5, 0, 2],
                    [5, 1, 1],
                    [5, 2, 0],
                    [5, 3, 3],
                    [5, 4, 0],
                    [5, 5, 0],
                    [5, 6, 0],
                    [5, 7, 0],
                    [5, 8, 2],
                    [5, 9, 0],
                    [5, 10, 4],
                    [5, 11, 1],
                    [5, 12, 5],
                    [5, 13, 10],
                    [5, 14, 5],
                    [5, 15, 7],
                    [5, 16, 11],
                    [5, 17, 6],
                    [5, 18, 0],
                    [5, 19, 5],
                    [5, 20, 3],
                    [5, 21, 4],
                    [5, 22, 2],
                    [5, 23, 0],
                    [6, 0, 1],
                    [6, 1, 0],
                    [6, 2, 0],
                    [6, 3, 0],
                    [6, 4, 0],
                    [6, 5, 0],
                    [6, 6, 0],
                    [6, 7, 0],
                    [6, 8, 0],
                    [6, 9, 0],
                    [6, 10, 1],
                    [6, 11, 0],
                    [6, 12, 2],
                    [6, 13, 1],
                    [6, 14, 3],
                    [6, 15, 4],
                    [6, 16, 0],
                    [6, 17, 0],
                    [6, 18, 0],
                    [6, 19, 0],
                    [6, 20, 1],
                    [6, 21, 2],
                    [6, 22, 2],
                    [6, 23, 6]
                ]
            }, ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    //雷达图插件
    {
        chartType: "radar",
        layerName: "雷达图",
        icon: "el-icon-datav-radar",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "",
                textStyle: {
                    color: "#fff", //颜色
                    fontStyle: "normal", //风格
                    fontWeight: "normal", //粗细
                    //fontFamily: 'Microsoft yahei',   //字体
                    //fontSize: 20,     //大小
                    align: "center", //水平对齐
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontSize: 14,
                    align: "center",
                },
                itemGap: 7,
            },
            tooltip: {},
            legend: {
                show: true, // 是否显示图例
                type: "scroll",
                x: "right",
                //orient: 'vertical',
                //left:'10%',
                //align: 'left',
                //top:'middle',
                textStyle: {
                    color: "#E7EAED",
                },
                height: 150,
            },
            showLegend: true,
            fontColor: "#fff",
            backgroundColor: "#999",
            shape: "",
            //是否显示阴影效果
            isArea: false,
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    series: "Allocated Budget",
                    name: "Sales",
                    value: 57
                },
                {
                    series: "Allocated Budget",
                    name: "Administration",
                    value: 21
                },
                {
                    series: "Allocated Budget",
                    name: "Customer Support",
                    value: 66
                },
                {
                    series: "Allocated Budget",
                    name: "Development",
                    value: 78
                },
                {
                    series: "Allocated Budget",
                    name: "Marketing",
                    value: 83
                },
                {
                    series: "Actual Spending",
                    name: "reindeer",
                    value: 66
                },
                {
                    series: "Actual Spending",
                    name: "ship",
                    value: 41
                },
                {
                    series: "Actual Spending",
                    name: "plane",
                    value: 55
                },
                {
                    series: "Actual Spending",
                    name: "train",
                    value: 23
                },
                {
                    series: "Actual Spending",
                    name: "car",
                    value: 33
                }
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },

    //水球图插件
    {
        chartType: "waterball",
        layerName: "水球图",
        icon: "el-icon-datav-waterballline",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "waterBall",
                layerName: "水球图",
                icon: "el-icon-datav-waterballline",
                customId: "",
                width: 500,
                height: 500,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            //fontStyle: 'normal', // 风格
                            fontWeight: "normal", // 粗细
                            fontFamily: "微软雅黑", // 字体
                            fontSize: 17, // 字号大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        x: "center",
                        itemGap: 7,
                    },
                    series: [{
                            type: "liquidFill",
                            radius: 235,
                            center: ["50%", "50%"],
                            data: [0.7, 0.7, 0.7], // data个数代表波浪数
                            backgroundStyle: {
                                borderWidth: 1,
                                color: "rgb(255, 0, 255, 0.1)",
                            },
                            label: {
                                normal: {
                                    formatter: (0.7 * 100).toFixed(2) + "%",
                                    textStyle: {
                                        fontSize: 50,
                                        fontFamily: "微软雅黑",
                                    },
                                },
                            },
                            outline: {
                                show: false,
                            },
                        },
                        {
                            type: "pie",
                            center: ["50%", "50%"],
                            radius: ["50%", "53%"],
                            hoverAnimation: false,
                            data: [{
                                    name: "",
                                    value: 500,
                                    labelLine: {
                                        show: false,
                                    },
                                    itemStyle: {
                                        color: "#5886f0",
                                    },
                                    emphasis: {
                                        labelLine: {
                                            show: false,
                                        },
                                        itemStyle: {
                                            color: "#5886f0",
                                        },
                                    },
                                },
                                {
                                    //画中间的图标
                                    name: "",
                                    value: 4,
                                    labelLine: {
                                        show: false,
                                    },
                                    itemStyle: {
                                        color: "#ffffff",
                                        normal: {
                                            color: "#5886f0",
                                            borderColor: "#5886f0",
                                            borderWidth: 20,
                                            // borderRadius: '100%'
                                        },
                                    },
                                    label: {
                                        borderRadius: "100%",
                                    },
                                    emphasis: {
                                        labelLine: {
                                            show: false,
                                        },
                                        itemStyle: {
                                            color: "#5886f0",
                                        },
                                    },
                                },
                                {
                                    // 解决圆点过大后续部分显示明显的问题
                                    name: "",
                                    value: 8,
                                    labelLine: {
                                        show: false,
                                    },
                                    itemStyle: {
                                        color: "#5886f0",
                                    },
                                    emphasis: {
                                        labelLine: {
                                            show: false,
                                        },
                                        itemStyle: {
                                            color: "#5886f0",
                                        },
                                    },
                                },
                                {
                                    //画剩余的刻度圆环
                                    name: "",
                                    value: 88,
                                    itemStyle: {
                                        color: "#101129",
                                    },
                                    label: {
                                        show: false,
                                    },
                                    labelLine: {
                                        show: false,
                                    },
                                    emphasis: {
                                        labelLine: {
                                            show: false,
                                        },
                                        itemStyle: {
                                            color: "rgba(255, 255, 255, 0)",
                                        },
                                    },
                                },
                            ],
                        },
                    ],
                    //静态数据值
                    staticDataValue: [{ value: 0.7 }],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "waterBallLine",
                layerName: "水球图加基线",
                icon: "el-icon-datav-waterballline",
                customId: "",
                width: 350,
                height: 300,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            //fontStyle: 'normal', // 风格
                            fontWeight: "normal", // 粗细
                            fontFamily: "微软雅黑", // 字体
                            fontSize: 17, // 字号大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        x: "center",
                        itemGap: 7,
                    },
                    // x轴
                    xAxis: {
                        show: false, // 不显示
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                    },
                    // y轴
                    yAxis: {
                        show: false, // 不显示
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                    },
                    series: [{
                            type: "liquidFill",
                            radius: "85%", // 半径大小
                            center: ["50%", "50%"], // 布局位置
                            data: [0.68, 0.36], // 水球波纹值

                            outline: {
                                // 轮廓设置
                                show: true,
                                borderDistance: 2, // 轮廓间距
                                itemStyle: {
                                    borderColor: "#294D99", // 轮廓颜色
                                    borderWidth: 4, // 轮廓大小
                                    shadowBlur: "none", // 轮廓阴影
                                },
                            },
                        },
                        {
                            type: "line", // 折线图
                            markLine: {
                                silent: false, // true:不触发鼠标事件, false:触发鼠标事件
                                symbol: "", // 标线两端样式
                                lineStyle: {
                                    // 标线样式
                                    color: "#f00",
                                    type: "solid",
                                },
                                data: [{
                                    // 标线数据
                                    //yAxis: data[1], // y轴
                                }, ],
                            },
                        },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ value: 0.7, line: 0.5 }],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "customWaterBall",
                layerName: "自定义水球图",
                icon: "el-icon-datav-waterballline",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#000",
                            fontSize: 34,
                            fontFamily: "微软雅黑",
                            align: "center",
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#000",
                            fontSize: 14,
                            align: "center",
                        },
                        x: "center",
                        itemGap: 7,
                    },
                    series: [{
                        type: "liquidFill",
                        data: [],
                        radius: "100%",
                        //外边线
                        outline: {
                            show: true,
                            borderDistance: 5,
                            itemStyle: {
                                borderColor: "#4053C6", //外边框的颜色
                                color: "rgba(0, 0, 0, 0)", //边框和核心内容之间的背景颜色
                                borderWidth: 5, //外边框的宽度
                                shadowBlur: 20, //外边阴影的模糊距离
                                shadowColor: "#4053C6",
                            },
                        },
                        // color: {
                        //     type: 'linear',
                        //     x: 0,
                        //     y: 0,
                        //     x2: 0,
                        //     y2: 1,
                        //     colorStops: [{
                        //         offset: 1,
                        //         color: 'rgba(2, 159, 200, 1)'
                        //     }, {
                        //         offset: 0.5,
                        //         color: 'rgba(0, 186, 211, .5)'
                        //     }, {
                        //         offset: 0,
                        //         color: 'rgba(0, 230, 220, 1)'
                        //     }],
                        //     globalCoord: false
                        // },
                        backgroundStyle: {
                            borderColor: "#156ACF", //水球边框颜色
                            borderWidth: 1,
                            color: "#E3F7FF",
                            shadowColor: "rgba(0, 0, 0, 0.7)", //边框阴影颜色
                            shadowBlur: 20, //边框阴影范围
                        },

                        waveAnimation: true, //水波是否滚动
                        waveLength: 200, //水波数量
                        amplitude: "10%", //水波的起伏程度
                        direction: "right", //水波的滚动方向
                        //水球图形状
                        isShape: true,
                        shape: "path://M367.855,428.202c-3.674-1.385-7.452-1.966-11.146-1.794c0.659-2.922,0.844-5.85,0.58-8.719 c-0.937-10.407-7.663-19.864-18.063-23.834c-10.697-4.043-22.298-1.168-29.902,6.403c3.015,0.026,6.074,0.594,9.035,1.728 c13.626,5.151,20.465,20.379,15.32,34.004c-1.905,5.02-5.177,9.115-9.22,12.05c-6.951,4.992-16.19,6.536-24.777,3.271 c-13.625-5.137-20.471-20.371-15.32-34.004c0.673-1.768,1.523-3.423,2.526-4.992h-0.014c0,0,0,0,0,0.014 c4.386-6.853,8.145-14.279,11.146-22.187c23.294-61.505-7.689-130.278-69.215-153.579c-61.532-23.293-130.279,7.69-153.579,69.202 c-6.371,16.785-8.679,34.097-7.426,50.901c0.026,0.554,0.079,1.121,0.132,1.688c4.973,57.107,41.767,109.148,98.945,130.793 c58.162,22.008,121.303,6.529,162.839-34.465c7.103-6.893,17.826-9.444,27.679-5.719c11.858,4.491,18.565,16.6,16.719,28.643 c4.438-3.126,8.033-7.564,10.117-13.045C389.751,449.992,382.411,433.709,367.855,428.202z",
                        label: {
                            position: ["50%", "50%"],
                            formatter: function() {
                                return "ECharts\nLiquid Fill"; //水球图中间文字
                            },
                            align: "center", //文本的位置
                            baseline: "middle", //文本垂直方向上对齐方向
                            fontSize: 27, //字体大小
                            fontFamily: "微软雅黑", //字体样式
                            fontWeight: "bolder", // 粗细
                            color: "#156ACF", //字体颜色
                            insideColor: "#E3F7FF", //在水里文字的样式
                        },
                        //鼠标悬浮时波浪的样式
                        emphasis: {
                            itemStyle: {
                                opacity: 60, //鼠标悬浮时波浪的透明度
                            },
                        },
                    }, ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{ value: 0.7 }],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
        ],
    },

    //环形图插件
    // {
    //   chartType: "circle",
    //   layerName: "环形图",
    //   icon: "el-icon-datav-circle",
    //   customId: "",
    //   width: 350,
    //   height: 200,
    //   x: 50,
    //   y: 100,
    //   zindex: 0,
    //   child: [
    //     {
    //       chartType: "circle",
    //       layerName: "圆点环形图",
    //       icon: "el-icon-datav-circle",
    //       customId: "",
    //       width: 350,
    //       height: 200,
    //       x: 50,
    //       y: 100,
    //       zindex: 0,
    //       isShow: true,
    //       chartOption: {
    //         title: {
    //           text: "", // 主标题
    //           textStyle: {
    //             color: "#fff", // 颜色
    //             fontWeight: "normal", // 粗细
    //             fontFamily: "微软雅黑", // 字体
    //             fontSize: 17, // 字号大小
    //             align: "center", // 水平对齐
    //           },
    //           subtext: "", // 副标题
    //           subtextStyle: {
    //             // 对应样式
    //             color: "#fff",
    //             fontSize: 14,
    //             align: "center",
    //           },
    //           x: "left",
    //           itemGap: 7,
    //         },
    //         //color: color,
    //         color: ["#EEF2A1", "#EFCF54", "#91BE98", "#CAE5C8", "#F5C9A1"],
    //         splitNumber: "10", //刻度数量
    //         /*legend: {
    //                   type:"scroll",
    //                   orient: 'vertical',
    //                   right:'10%',
    //                   align:'left',
    //                   top:'middle',
    //                   textStyle: {
    //                       color:'#E7EAED'
    //                   },
    //                   x: 'left',
    //                   height:150
    //               },*/
    //         legend: {
    //           show: true, // 是否显示图例
    //           type: "plain",
    //           orient: "horizontal",
    //           right: "10%",
    //           align: "left",
    //           top: "middle",
    //           textStyle: {
    //             color: "#E7EAED",
    //           },
    //           x: "left",
    //           //height: 150
    //         },
    //         toolbox: {
    //           show: false,
    //         },
    //         pieRadiusMin: 30, //内半径
    //         pieRadiusMax: 70, //外半径
    //         pieRadiusDiff: 2, //宽度
    //         maxValue: 800,
    //         borderWidth: 5,
    //         //载入动画
    //         animate: "",
    //         //通用处理函数
    //         customData: "",
    //         interactData: [],
    //         //静态数据值
    //         staticDataValue: [
    //           { value: 14, name: "暗渠" },
    //           { value: 32, name: "暗管" },
    //           { value: 288, name: "管控" },
    //           { value: 463, name: "水渠" },
    //           { value: 763, name: "集中式" },
    //         ],

    //         //绑定的div
    //         bindingDiv: "",
    //         theme: "",
    //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
    //         dataSourceType: "static",
    //         bi: null,
    //         timeout: 0,
    //       },
    //     },
    //     {
    //       chartType: "pieMore",
    //       layerName: "多环形图",
    //       icon: "el-icon-datav-piemore",
    //       customId: "",
    //       width: 350,
    //       height: 200,
    //       x: 50,
    //       y: 100,
    //       zindex: 0,
    //       isShow: true,
    //       chartOption: {
    //         title: {
    //           text: "", //主标题
    //           textStyle: {
    //             color: "#fff", //颜色
    //             fontStyle: "normal", //风格
    //             fontWeight: "normal", //粗细
    //             fontFamily: "微软雅黑", //字体
    //             fontSize: 17, //大小
    //             align: "center", //水平对齐
    //           },
    //           subtext: "", //副标题
    //           subtextStyle: {
    //             //对应样式
    //             color: "#fff",
    //             fontSize: 14,
    //             align: "center",
    //           },
    //           x: "center",
    //           itemGap: 7,
    //         },
    //         legend: {
    //           show: true, // 是否显示图例
    //           x: "center",
    //           y: "middle",
    //           textStyle: {
    //             color: "#E7EAED",
    //           },
    //           data: ["剧情", "奇幻", "爱情"],
    //         },
    //         //内半径
    //         innerRadius: 35,
    //         //外半径
    //         outerRadius: 45,
    //         col: 5,
    //         centerX: 20,
    //         centerY: 30,
    //         //是否显示工具箱
    //         isToolBox: false,
    //         //载入动画
    //         animate: "",
    //         //通用处理函数
    //         customData: "",
    //         interactData: [],
    //         //静态数据值
    //         staticDataValue: [
    //           {
    //             name: "剧情",
    //             value: 13,
    //           },
    //           {
    //             name: "奇幻",
    //             value: 11,
    //           },
    //           {
    //             name: "爱情",
    //             value: 24,
    //           },
    //           {
    //             name: "惊悚",
    //             value: 55,
    //           },
    //           {
    //             name: "冒险",
    //             value: 23,
    //           },
    //         ],

    //         //绑定的div
    //         bindingDiv: "",
    //         theme: "",
    //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
    //         dataSourceType: "static",
    //         bi: null,
    //         timeout: 0,
    //       },
    //     },
    //     {
    //       chartType: "progress",
    //       layerName: "进度图表",
    //       icon: "el-icon-datav-progressChart",
    //       customId: "",
    //       width: 350,
    //       height: 200,
    //       x: 50,
    //       y: 100,
    //       zindex: 0,
    //       isShow: true,
    //       chartOption: {
    //         title: {
    //           text: "", // 主标题
    //           textStyle: {
    //             color: "#fff", // 颜色
    //             fontWeight: "normal", // 粗细
    //             fontFamily: "微软雅黑", // 字体
    //             fontSize: 17, // 字号大小
    //             align: "center", // 水平对齐
    //           },
    //           subtext: "", // 副标题
    //           subtextStyle: {
    //             // 对应样式
    //             color: "#fff",
    //             fontSize: 14,
    //             align: "center",
    //           },
    //           x: "left",
    //           itemGap: 7,
    //         },
    //         grid: {
    //           top: "16%",
    //           left: "50%",
    //           bottom: "55%",
    //           containLabel: false,
    //         },
    //         yAxis: {
    //           type: "category",
    //           inverse: true,
    //           z: 3,
    //           axisLine: {
    //             show: false,
    //           },
    //           axisTick: {
    //             show: false,
    //           },
    //           axisLabel: {
    //             interval: 0,
    //             inside: false,
    //             textStyle: {
    //               color: "RGB(255, 255, 255)",
    //             },
    //             show: true,
    //           },
    //           splitLine: {
    //             show: false,
    //           },
    //           data: null,
    //         },
    //         xAxis: [
    //           {
    //             show: false,
    //           },
    //         ],
    //         tooltip: {
    //           show: true,
    //           trigger: "item",
    //           formatter: "{b} : {c} ({d}%)",
    //         },
    //         legend: {
    //           show: true, // 是否显示图例
    //           type: "plain",
    //           orient: "horizontal",
    //           right: "10%",
    //           align: "left",
    //           top: "top",
    //           textStyle: {
    //             color: "#E7EAED",
    //           },
    //           x: "center",
    //           //height: 150
    //         },
    //         toolbox: {
    //           show: false,
    //         },
    //         //100%代表的数值
    //         maxValue: 1000,
    //         //中心点坐标
    //         center: ["50%", "55%"],
    //         //大环半径
    //         longRingRadius: 85,
    //         //大环宽度
    //         longRingWidth: 3,
    //         //大环颜色
    //         longRingColor: "#1f59a7",
    //         //小环半径
    //         shortRingRadius: 80,
    //         //小环宽度
    //         shortRingWidth: 2,
    //         //小环半径差
    //         shortRingRadiusDiff: 5,
    //         //小环颜色
    //         shortRingColor: "#1f59a7",
    //         //pie半径范围
    //         pieRadiusMin: 30,
    //         pieRadiusMax: 70,
    //         //背景线的半径差
    //         backgroundLineRadiusDiff: 1,
    //         //背景线的颜色
    //         backgroundLineColor: "rgba(12, 64, 128)",
    //         //pie的半径差
    //         pieRadiusDiff: 2,
    //         //改变大小时刷新的时间间隔(ms)
    //         refreshInterval: 1000,
    //         //是否显示图例
    //         isShowLegend: true,
    //         //是否显示label
    //         isShowLabel: true,
    //         //label大小
    //         labelSizeRatio: 10,
    //         //label颜色
    //         labelColor: "#fff",
    //         //是否显示number
    //         isShowNumber: true,
    //         //短刻度内半径
    //         shortRingRadiusMin: 10,
    //         //number大小
    //         numberSizeRatio: 10,
    //         //number颜色
    //         numberColor: "#fff",
    //         // color: ['#003366', '#006699', '#4cabce', '#e5323e'],
    //         color: [
    //           "#37a2da",
    //           "#fb7293",
    //           "#8378ea",
    //           "#e7bcf3",
    //           "#ffdb5c",
    //           "#32c5e9",
    //           "#9fe6b8",
    //           "#ff9f7f",
    //         ],
    //         //载入动画
    //         animate: "",
    //         //通用处理函数
    //         customData: "",
    //         interactData: [],
    //         //静态数据值
    //         staticDataValue: [
    //           { name: "本科", value: 754 },
    //           { name: "硕士", value: 611 },
    //           { name: "大专", value: 400 },
    //           { name: "博士", value: 200 },
    //         ],

    //         //绑定的div
    //         bindingDiv: "",
    //         theme: "",
    //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
    //         dataSourceType: "static",
    //         bi: null,
    //         timeout: 0,
    //       },
    //     },
    //   ],
    // },

    //数据集插件
    {
        chartType: "datamap",
        layerName: "数据集",
        icon: "el-icon-datav-datamap",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "pieLine",
                layerName: "饼图-折线图",
                icon: "el-icon-datav-pieline",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    legend: {
                        show: true, // 是否显示图例
                        type: "scroll",
                        x: "center",
                        y: "8%",
                        //orient: 'vertical',
                        //left:'10%',
                        //align: 'left',
                        //top:'middle',
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                    },
                    tooltip: {
                        trigger: "axis",
                        showContent: true,
                    },
                    dataset: {
                        //静态数据值
                        source: [
                            ["product", "2012", "2013", "2014", "2015", "2016", "2017"],
                            ["Matcha Latte", 41.1, 30.4, 65.1, 53.3, 83.8, 98.7],
                            ["Milk Tea", 86.5, 92.1, 85.7, 83.1, 73.4, 55.1],
                            ["Cheese Cocoa", 24.1, 67.2, 79.5, 86.4, 65.2, 82.5],
                            ["Walnut Brownie", 55.2, 67.1, 69.2, 72.4, 53.9, 39.1],
                        ],
                    },
                    backgroundColor: "#101129",
                    //color: ['#37a2da','#32c5e9','#9fe6b8','#ffdb5c','#ff9f7f','#fb7293','#e7bcf3','#8378ea'],
                    //color: ['#00CFFF', '#006CED', '#FFE000', '#FFA800', '#FF5B00', '#FF3000'],
                    xAxis: {
                        name: "x轴",
                        type: "category",
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        show: true,
                    },
                    yAxis: {
                        gridIndex: 0,
                        name: "y轴",
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        splitLine: {
                            show: true,
                        },
                        show: true,
                    },
                    grid: {
                        top: "55%",
                        bottom: 30,
                    },
                    series: [{
                            type: "line",
                            smooth: true,
                            seriesLayoutBy: "row",
                            //是否显示平均线
                            markLine: {
                                data: [{ type: "average", name: "平均值" }],
                            },
                            //是否显示最大值和最小值
                            markPoint: {
                                data: [
                                    { type: "max", name: "最大值" },
                                    { type: "min", name: "最小值" },
                                ],
                            },
                            lineStyle: {
                                width: 3,
                                shadowColor: "rgba(231,234,237,0.4)",
                                shadowBlur: 10,
                                shadowOffsetY: 10,
                            },
                        },
                        {
                            type: "line",
                            smooth: true,
                            seriesLayoutBy: "row",
                            //是否显示平均线
                            markLine: {
                                data: [{ type: "average", name: "平均值" }],
                            },
                            //是否显示最大值和最小值
                            markPoint: {
                                data: [
                                    { type: "max", name: "最大值" },
                                    { type: "min", name: "最小值" },
                                ],
                            },
                            lineStyle: {
                                width: 3,
                                shadowColor: "rgba(231,234,237,0.4)",
                                shadowBlur: 10,
                                shadowOffsetY: 10,
                            },
                        },
                        {
                            type: "line",
                            smooth: true,
                            seriesLayoutBy: "row",
                            //是否显示平均线
                            markLine: {
                                data: [{ type: "average", name: "平均值" }],
                            },
                            //是否显示最大值和最小值
                            markPoint: {
                                data: [
                                    { type: "max", name: "最大值" },
                                    { type: "min", name: "最小值" },
                                ],
                            },
                            lineStyle: {
                                width: 3,
                                shadowColor: "rgba(231,234,237,0.4)",
                                shadowBlur: 10,
                                shadowOffsetY: 10,
                            },
                        },
                        {
                            type: "line",
                            smooth: true,
                            seriesLayoutBy: "row",
                            //是否显示平均线
                            markLine: {
                                data: [{ type: "average", name: "平均值" }],
                            },
                            //是否显示最大值和最小值
                            markPoint: {
                                data: [
                                    { type: "max", name: "最大值" },
                                    { type: "min", name: "最小值" },
                                ],
                            },
                            lineStyle: {
                                width: 3,
                                shadowColor: "rgba(231,234,237,0.4)",
                                shadowBlur: 10,
                                shadowOffsetY: 10,
                            },
                        },
                        {
                            type: "pie",
                            id: "pie",
                            radius: "30%", //饼图半径
                            center: ["50%", "30%"], //饼图位置
                            roseType: false, //南丁格尔玫瑰
                            label: {
                                formatter: "{b}: {@2012} ({d}%)",
                            },
                            encode: {
                                itemName: "product",
                                value: "2012",
                                tooltip: "2012",
                            },
                        },
                    ],
                    //内半径
                    innerRadius: 30,
                    //外半径
                    outerRadius: 55,
                    //是否显示平均线
                    isMarkLine: false,
                    //是否显示最大值和最小值
                    isMarkPoint: false,
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [{
                            name: "2012",
                            type: "Matcha Latte",
                            value: 41.1,
                        },
                        {
                            name: "2012",
                            type: "Milk Tea",
                            value: 86.5,
                        },
                        {
                            name: "2012",
                            type: "Cheese Cocoa",
                            value: 24.1,
                        },
                        {
                            name: "2012",
                            type: "Walnut Brownie",
                            value: 55.2,
                        },

                        {
                            name: "2013",
                            type: "Matcha Latte",
                            value: 30.1,
                        },
                        {
                            name: "2013",
                            type: "Milk Tea",
                            value: 45.5,
                        },
                        {
                            name: "2013",
                            type: "Cheese Cocoa",
                            value: 64.1,
                        },
                        {
                            name: "2013",
                            type: "Walnut Brownie",
                            value: 44.2,
                        },
                        // ['product', '2012', '2013', '2014', '2015', '2016', '2017'],
                        // ['Matcha Latte', '41.1', '30.4', '65.1', '53.3', '83.8', '98.7'],
                        // ['Milk Tea', '86.5', '92.1', '85.7', '83.1', '73.4', '55.1'],
                        // ['Cheese Cocoa', '24.1', '67.2', '79.5', '86.4', '65.2', '82.5'],
                        // ['Walnut Brownie', '55.2', '67.1', '69.2', '72.4', '53.9', '39.1']
                    ],
                    //南丁格尔玫瑰
                    isRoseType: false,
                    //是否环饼
                    isRing: false,
                    //折线是否平滑
                    isSmooth: true,

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "barPie",
                layerName: "柱状图-饼图",
                icon: "el-icon-datav-barPie",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontStyle: "normal", //风格
                            fontWeight: "normal", //粗细
                            //fontFamily: 'Microsoft yahei',   //字体
                            //fontSize: 20,     //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    tooltip: {
                        //提示
                        trigger: "item", //饼图
                        formatter: "{b} : {c}",
                    },
                    //color: ['#37a2da','#32c5e9','#9fe6b8','#ffdb5c','#ff9f7f','#fb7293','#e7bcf3','#8378ea'],
                    legend: {
                        type: "scroll",
                        orient: "vertical",
                        left: "10",
                        align: "left",
                        top: "50",
                        textStyle: {
                            color: "#E7EAED",
                        },
                        height: 150,
                        show: true,
                    },
                    xAxis: {
                        name: "x轴",
                        type: "category",
                        axisLabel: {
                            color: "#fff",
                        },
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        data: [],
                        show: true,
                    },
                    yAxis: {
                        type: "value",
                        name: "加载时间/秒",
                        nameTextStyle: {
                            color: "#fff",
                        },
                        axisLabel: {
                            color: "#fff",
                        },
                        axisLine: {
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        splitLine: {
                            show: true,
                        },
                        show: true,
                    },
                    series: [{
                            type: "bar",
                            barWidth: 30,
                            //是否显示平均线
                            markLine: {
                                data: [{ type: "average", name: "平均值" }],
                            },
                            //是否显示最大值和最小值
                            markPoint: {
                                data: [
                                    { type: "max", name: "最大值" },
                                    { type: "min", name: "最小值" },
                                ],
                            },
                            lineStyle: {
                                width: 3,
                                shadowColor: "rgba(231,234,237,0.4)",
                                shadowBlur: 10,
                                shadowOffsetY: 10,
                            },
                            data: [],
                        },
                        {
                            name: "网络流量监控",
                            type: "pie",
                            radius: ["10", "40"],
                            center: ["75", "25"],
                            roseType: "radius",
                            label: {
                                fontSize: 10,
                                formatter: "{d}%",
                            },
                            labelLine: {
                                length: 10,
                                length2: 10,
                            },
                        },
                    ],
                    unit: "次",
                    //是否显示平均线
                    isMarkLine: false,
                    //是否显示最大值和最小值
                    isMarkPoint: false,
                    //动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //柱状图竖显示
                    isVertical: false,
                    //记录前一次切换标识
                    //preChangeFlag: "false",
                    //南丁格尔玫瑰
                    isRoseType: false,
                    //静态数据值
                    staticDataValue: [
                        { value: 335, name: "直接访问" },
                        { value: 310, name: "邮件营销" },
                        { value: 234, name: "联盟广告" },
                        { value: 135, name: "视频广告" },
                        { value: 154, name: "搜索引擎" },
                    ],
                    radiusI: 10,
                    radiusO: 40,
                    centerX: 75,
                    centerY: 25,
                    legendcenterX: 0,
                    legendcenterY: 50,

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    bi: null,
                    timeout: 0,
                },
            },
            {
                chartType: "lineBar",
                layerName: "折线图-柱状图",
                icon: "el-icon-datav-linebar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", // 主标题
                        textStyle: {
                            color: "#fff", // 颜色
                            fontStyle: "normal", // 风格
                            fontWeight: "normal", // 粗细
                            //fontFamily: 'Microsoft yahei',   // 字体
                            //fontSize: 20,     // 大小
                            align: "center", // 水平对齐
                        },
                        subtext: "", // 副标题
                        subtextStyle: {
                            // 对应样式
                            color: "#fff",
                            fontSize: 14,
                            align: "center",
                        },
                        itemGap: 7,
                    },
                    tooltip: {
                        trigger: "axis",
                        axisPointer: {
                            type: "cross",
                            crossStyle: {
                                color: "#999",
                            },
                        },
                    },
                    toolbox: {
                        feature: {
                            dataView: { show: true, readOnly: false },
                            magicType: { show: true, type: ["line", "bar"] },
                            restore: { show: true },
                        },
                        show: false,
                    },
                    //legend: ['蒸发量', '降水量', '平均温度'],
                    showLegend: true,
                    barWidth: 10, // 柱体宽度
                    isSmooth: false, //曲线是否平湖
                    xAxis: [{
                        // axisLabel: {
                        //     color: '#fff'
                        // },
                        axisLine: {
                            show: true,
                            lineStyle: {
                                color: "#fff",
                            },
                        },
                        type: "category",
                        data: [
                            "1月",
                            "2月",
                            "3月",
                            "4月",
                            "5月",
                            "6月",
                            "7月",
                            "8月",
                            "9月",
                            "10月",
                            "11月",
                            "12月",
                        ],
                        axisPointer: {
                            type: "shadow",
                        },
                        show: true,
                    }, ],
                    yAxis: [{
                            type: "value",
                            name: "水量",
                            //            min: 0,
                            //            max: 250,
                            //            interval: 50,
                            axisLabel: {
                                formatter: "ml",
                                //color: '#fff'
                            },
                            // nameTextStyle: {
                            //     color: '#ccc'
                            // },
                            axisLine: {
                                show: true,
                                lineStyle: {
                                    color: "#fff",
                                },
                            },
                            splitLine: {
                                show: false,
                            },
                            show: true,
                        },
                        {
                            type: "value",
                            name: "温度",
                            //            min: 0,
                            //            max: 25,
                            //            interval: 5,
                            axisLabel: {
                                formatter: "°C",
                                //color: '#fff'
                            },
                            // nameTextStyle: {
                            //     color: '#ccc'
                            // },
                            axisLine: {
                                show: true,
                                lineStyle: {
                                    color: "#fff",
                                },
                            },
                            splitLine: {
                                show: false,
                            },
                            show: true,
                        },
                    ],
                    staticDataValue: [{
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "1月",
                            value: 12.0,
                        },
                        {
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "2月",
                            value: 22.0,
                        },
                        {
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "3月",
                            value: 32.0,
                        },
                        {
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "4月",
                            value: 42.0,
                        },
                        {
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "5月",
                            value: 52.0,
                        },
                        {
                            series: "蒸发量",
                            yAxisIndex: 0,
                            type: "bar",
                            name: "6月",
                            value: 62.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            type: "line",
                            name: "1月",
                            value: 21.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            type: "line",
                            name: "2月",
                            value: 22.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            type: "line",
                            name: "3月",
                            value: 23.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            type: "line",
                            name: "4月",
                            value: 24.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            yAxisIndex: 1,
                            type: "line",
                            name: "5月",
                            value: 25.0,
                        },
                        {
                            series: "平均温度",
                            yAxisIndex: 1,
                            type: "line",
                            name: "6月",
                            value: 26.0,
                        }
                    ],
                    //是否显示平均线
                    isMarkLine: false,
                    //是否显示最大最小值
                    isMarkPoint: false,
                    itemWidth: 15, //图例宽度
                    itemHeight: 10, //图例高度
                    legendFontSize: 12, //图例字体大小
                    legendFontColor: "#fff", //图例字体颜色
                    legendOrient: "horizontal", //图例布局
                    legendX: 25, //图例水平位置
                    legendY: 1, //图例垂直位置
                    xLineWidth: 2, //x轴粗细
                    xFontSize: 12, //x轴标签字体大小
                    xFontColor: "#fff", //x轴标签字体颜色
                    yLineWidth: 2, //y轴粗细
                    yFontSize: 12, //y轴标签字体大小
                    yFontColor: "#fff", //y轴标签字体颜色
                    lineWidth: 2, //折线粗细
                    lineSymbolSize: 5, //折线标记大小
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "mapBar",
                layerName: "地图-柱状图",
                icon: "el-icon-datav-mapbar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        //标题
                        text: "",
                        subtext: "",
                        left: "center",
                        textStyle: {
                            color: "#fff",
                        },
                    },
                    isScatter: true,
                    mapTooltip: "mapTooltip",
                    topNum: 5,
                    barNum: 5,
                    color: "#ddb926",
                    isToolBox: true,
                    staticDataValue: [
                        { name: "北京市", value: 69 },
                        { name: "天津市", value: 93 },
                        { name: "河北省", value: 113 },
                        { name: "山西省", value: 31 },
                        { name: "内蒙古自治区", value: 31 },
                        { name: "辽宁省", value: 55 },
                        { name: "吉林省", value: 29 },
                        { name: "黑龙江省", value: 71 },
                        { name: "上海市", value: 50 },
                        { name: "江苏省", value: 436 },
                        { name: "浙江省", value: 132 },
                        { name: "安徽省", value: 160 },
                        { name: "福建省", value: 68 },
                        { name: "江西省", value: 49 },
                        { name: "重庆市", value: 143 },
                        { name: "河南省", value: 99 },
                        { name: "湖北省", value: 44 },
                        { name: "湖南省", value: 47 },
                        { name: "四川省", value: 83 },
                        { name: "贵州省", value: 60 },
                        { name: "云南省", value: 55 },
                    ],
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
        ],
    },

    //地图插件
    {
        chartType: "map",
        layerName: "地图",
        icon: "el-icon-datav-map",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "map",
                layerName: "地图",
                icon: "el-icon-datav-map",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontFamily: "微软雅黑", //字体
                            fontWeight: "normal", //粗细
                            fontSize: 17, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontFamily: "微软雅黑", //字体
                            fontWeight: "normal", //粗细
                            fontSize: 14,
                            align: "center",
                        },
                        x: "center",
                        itemGap: 7,
                    },
                    roam: true, //控制缩放、平移
                    isScatter: true, //是否是散点图
                    scatterData: {
                        name: "散点",
                        type: "scatter",
                        coordinateSystem: "geo",
                        data: null,
                        symbolSize: function(val) {
                            return val[2] / 10;
                        },
                        label: {
                            normal: {
                                formatter: "{b}",
                                position: "right",
                                show: true,
                            },
                            emphasis: {
                                show: true,
                            },
                        },
                        itemStyle: {
                            normal: {
                                color: "#05C3F9",
                            },
                        },
                        tooltip: {
                            trigger: "item",
                            //formatter: "{a} <br/>{b} : {c} ({d}%)"
                            formatter: function(val) {
                                return val.name + ":" + val.value[2] + "(个)";
                            },
                        },
                    },
                    isGeo: true, //是否是气泡图
                    popORimg: "pop", //选择气泡图案还是图片
                    geoData: {
                        name: "点",
                        type: "scatter",
                        coordinateSystem: "geo",
                        symbol: "pin", //气泡

                        symbolSize: function(val) {
                            //console.log(val[2]);
                            var a = (maxSize4Pin - minSize4Pin) / (max - min);
                            var b = minSize4Pin - a * min;
                            b = maxSize4Pin - a * max;
                            return a * val[2] + b;
                        },
                        label: {
                            normal: {
                                formatter: "{@[2]}",
                                show: true,
                                textStyle: {
                                    color: "#fff",
                                    fontSize: 9,
                                },
                            },
                        },
                        itemStyle: {
                            normal: {
                                color: "#F62157", //标志颜色
                            },
                        },
                        tooltip: {
                            trigger: "item",
                            //formatter: "{a} <br/>{b} : {c} ({d}%)"
                            formatter: function(val) {
                                return val.name + ":" + val.value[2] + "(个)";
                            },
                        },
                        zlevel: 6,
                        data: null,
                    },
                    isTop: true, //是否是top5
                    top5Data: {
                        name: "Top 5",
                        type: "effectScatter",
                        coordinateSystem: "geo",
                        /*data: convertData(data.sort(function(a, b) {
                                    return b.value - a.value;
                                }).slice(0, 5)),*/
                        data: null,
                        symbolSize: function(val) {
                            return val[2] / 25;
                        },
                        showEffectOn: "render",
                        rippleEffect: {
                            brushType: "stroke",
                        },
                        hoverAnimation: true,
                        label: {
                            normal: {
                                formatter: "{b}",
                                position: "right",
                                show: true,
                            },
                        },
                        itemStyle: {
                            normal: {
                                color: "yellow",
                                shadowBlur: 10,
                                shadowColor: "yellow",
                            },
                        },
                        tooltip: {
                            trigger: "item",
                            //formatter: "{a} <br/>{b} : {c} ({d}%)"
                            formatter: function(val) {
                                return val.name + ":" + val.value[2] + "人";
                            },
                        },
                        zlevel: 1,
                    },
                    scatterSize: 5,
                    popSize: 20,
                    popColor: "rgba(216, 55, 55, 1)",
                    emphasisColor: "#389BB7",
                    suffix: "人",
                    topNum: 5,
                    // 区域颜色
                    areaColor: "#031525",
                    // 描边颜色
                    borderColor: "#3B5077",
                    visualMap: {
                        show: true,
                        min: 0,
                        max: 200,
                        left: "10%",
                        //left: 'left',
                        top: "20%",
                        //top: 'bottom',
                        text: ["高", "低"], // 文本，默认为数值文本
                        textStyle: {
                            color: "#fff",
                        },
                        calculable: true,
                        seriesIndex: [1],
                        inRange: {
                            color: ["#26587D", "#3BB3C4"],
                        },
                    },
                    // 地图名称
                    mapName: "china",
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    staticDataValue: [
                        { name: "北京", value: 69 },
                        { name: "天津", value: 93 },
                        { name: "河北", value: 113 },
                        { name: "山西", value: 31 },
                        { name: "内蒙古", value: 31 },
                        { name: "辽宁", value: 55 },
                        { name: "吉林", value: 29 },
                        { name: "黑龙江", value: 71 },
                        { name: "上海", value: 50 },
                        { name: "江苏", value: 436 },
                        { name: "浙江", value: 132 },
                        { name: "安徽", value: 160 },
                        { name: "福建", value: 68 },
                        { name: "江西", value: 49 },
                        { name: "山东", value: 143 },
                        { name: "河南", value: 99 },
                        { name: "湖北", value: 44 },
                        { name: "湖南", value: 47 },
                        { name: "重庆", value: 53 },
                        { name: "四川", value: 83 },
                        { name: "贵州", value: 60 },
                        { name: "云南", value: 55 },
                        { name: "西藏", value: 7 },
                        { name: "陕西", value: 232 },
                        { name: "甘肃", value: 44 },
                        { name: "青海", value: 15 },
                        { name: "宁夏", value: 61 },
                        { name: "新疆", value: 76 },
                        { name: "广东", value: 41 },
                        { name: "广西", value: 44 },
                        { name: "海南", value: 44 },
                    ],

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "mapMore",
                layerName: "地图下钻",
                icon: "el-icon-datav-mapMore",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    text: "中国地图（省市级地图数据下钻）",
                    subtext: "温馨提示：单机主地图省份下钻到各个地市，进入地市地图后双击返回省份主地图。",
                    fontSize: 17, //大小
                    fontFamily: "微软雅黑", //字体
                    fontWeight: "normal", //粗细
                    x: "center", //标题位置
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    roam: true, //控制缩放、平移
                    //静态数据值
                    staticDataValue: [
                        { name: "北京", value: 5 },
                        { name: "天津", value: 33 },
                        { name: "上海", value: 27 },
                        { name: "重庆", value: 52 },
                        { name: "河北", value: 38 },
                        { name: "河南", value: 68 },
                        { name: "云南", value: 50 },
                        { name: "辽宁", value: 51 },
                        { name: "黑龙江", value: 93 },
                        { name: "湖南", value: 8 },
                        { name: "安徽", value: 8 },
                        { name: "山东", value: 39 },
                        { name: "新疆", value: 5 },
                        { name: "江苏", value: 90 },
                        { name: "浙江", value: 86 },
                        { name: "江西", value: 11 },
                        { name: "湖北", value: 25 },
                        { name: "广西", value: 89 },
                        { name: "甘肃", value: 35 },
                        { name: "山西", value: 37 },
                        { name: "内蒙古", value: 97 },
                        { name: "陕西", value: 31 },
                        { name: "吉林", value: 88 },
                        { name: "福建", value: 22 },
                        { name: "贵州", value: 76 },
                        { name: "广东", value: 25 },
                        { name: "青海", value: 90 },
                        { name: "西藏", value: 18 },
                        { name: "四川", value: 79 },
                        { name: "宁夏", value: 44 },
                        { name: "海南", value: 22 },
                        { name: "台湾", value: 52 },
                        { name: "香港", value: 2 },
                        { name: "澳门", value: 65 },
                    ],
                    //是否显示工具条
                    isVmap: true,

                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "threeDMap",
                layerName: "3D地图",
                icon: "el-icon-datav-threedMap",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    textStyle: {
                        // 标签的字体样式
                        color: "#ffffff", // 地图初始化区域字体颜色
                        fontSize: 14, // 字体大小
                        opacity: 1, // 字体透明度
                        backgroundColor: "rgba(0,23,11,0.5)", // 字体背景色
                    },
                    itemStyle: {
                        // 三维地理坐标系组件 中三维图形的视觉属性，包括颜色，透明度，描边等。
                        // areaColor: 'rgba(95,158,160,0.5)', // 地图板块的颜色
                        color: "#10786c", // 地图板块的颜色
                        opacity: 0.3, // 图形的不透明度 [ default: 1 ]
                        borderWidth: 2, // (地图板块间的分隔线)图形描边的宽度。加上描边后可以更清晰的区分每个区域 [ default: 0 ]
                        borderColor: "#5CFFE0", // 图形描边的颜色。[ default: #333 ]
                    },
                    tooltip: {
                        backgroundColor: "rgba(30, 54, 124,0.3)",
                        borderWidth: 1,
                        borderRadius: 4,
                        borderColor: "#00B2AC",
                        textStyle: {
                            color: "rgba(255,255,255,1)",
                        },
                    },
                    //上下旋转角度
                    alpha: 60,
                    //左右旋转角度
                    beta: 0,
                    // 地图名称
                    //是否显示工具条
                    isVmap: true,
                    mapName: "china",
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [
                        { name: "北京", value: 5 },
                        { name: "天津", value: 33 },
                        { name: "上海", value: 27 },
                        { name: "重庆", value: 52 },
                        { name: "河北", value: 38 },
                        { name: "河南", value: 68 },
                        { name: "云南", value: 50 },
                        { name: "辽宁", value: 51 },
                        { name: "黑龙江", value: 93 },
                        { name: "湖南", value: 8 },
                        { name: "安徽", value: 8 },
                        { name: "山东", value: 39 },
                        { name: "新疆", value: 5 },
                        { name: "江苏", value: 90 },
                        { name: "浙江", value: 86 },
                        { name: "江西", value: 11 },
                        { name: "湖北", value: 25 },
                        { name: "广西", value: 89 },
                        { name: "甘肃", value: 35 },
                        { name: "山西", value: 37 },
                        { name: "内蒙古", value: 97 },
                        { name: "陕西", value: 31 },
                        { name: "吉林", value: 88 },
                        { name: "福建", value: 22 },
                        { name: "贵州", value: 76 },
                        { name: "广东", value: 25 },
                        { name: "青海", value: 90 },
                        { name: "西藏", value: 18 },
                        { name: "四川", value: 79 },
                        { name: "宁夏", value: 44 },
                        { name: "海南", value: 22 },
                        { name: "台湾", value: 52 },
                        { name: "香港", value: 2 },
                        { name: "澳门", value: 65 },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "threeDMapBar",
                layerName: "3D地图柱状图",
                icon: "el-icon-datav-threedMapBar",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    itemStyle: {
                        // 三维地理坐标系组件 中三维图形的视觉属性，包括颜色，透明度，描边等。
                        // areaColor: 'rgba(95,158,160,0.5)', // 地图板块的颜色
                        color: "rgba(95,158,160,0.25)", // 地图板块的颜色 ]
                        borderWidth: 2, // (地图板块间的分隔线)图形描边的宽度。加上描边后可以更清晰的区分每个区域 [ default: 0 ]
                        borderColor: "#5CFFE0", // 图形描边的颜色。[ default: #333 ]
                    },
                    label: {
                        show: true,
                        textStyle: {
                            color: "#ffffff", // 地图初始化区域字体颜色
                            fontSize: 18, // 字体大小
                            backgroundColor: "rgb(1,1,1,0)", // 字体背景色
                        },
                    },
                    visualMap: {
                        show: true,
                        left: 10,
                        bottom: 10,
                        itemWidth: 20,
                        itemHeight: 150,
                        color: ["#87aa66", "#eba438", "#d94d4c"],
                        fontSize: 18,
                    },
                    textStyle: {
                        backgroundColor: "rgba(173, 244, 159, 1)",
                        color: "rgba(26, 15, 98, 1)",
                        fontSize: 18,
                        borderColor: "rgba(247, 178, 4, 1)",
                        sortColor: "rgba(255, 217, 2, 1)",
                        sortFontSize: 20,
                    },
                    emphasis: {
                        textStyle: {
                            backgroundColor: "rgba(226, 234, 133, 1)",
                            color: "#333",
                            fontSize: 24,
                            borderColor: "rgba(247, 178, 4, 1)",
                            sortColor: "rgba(255, 217, 2, 1)",
                            sortFontSize: 30,
                        },
                        itemStyle: {
                            color: "#1d5e98",
                        },
                    },
                    labelShow: true,
                    sortShow: true,
                    suffix: "人",
                    barSize: 1.5, //柱状图大小
                    bevelSize: 0.3, //柱子的倒角尺寸
                    bevelSmoothness: 2, //柱子的倒角光滑度
                    viewControl: {
                        isRotate: true, //缩放平移
                        //上下旋转角度
                        alpha: 40,
                        //左右旋转角度
                        beta: 0,
                    },

                    // 地图名称
                    mapName: "china",
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    //静态数据值
                    staticDataValue: [
                        { name: "呼伦贝尔", value: 88 },
                        { name: "北京", value: 50 },
                        { name: "天津", value: 45 },
                        { name: "上海", value: 22 },
                        { name: "重庆", value: 56 },
                        { name: "河北", value: 70 },
                    ],
                    //是否显示工具条
                    isVmap: true,
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            {
                chartType: "mapLine",
                layerName: "飞线地图",
                icon: "el-icon-datav-mapline",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    title: {
                        text: "", //主标题
                        textStyle: {
                            color: "#fff", //颜色
                            fontFamily: "微软雅黑", //字体
                            fontWeight: "normal", //粗细
                            fontSize: 17, //大小
                            align: "center", //水平对齐
                        },
                        subtext: "", //副标题
                        subtextStyle: {
                            //对应样式
                            color: "#fff",
                            fontFamily: "微软雅黑", //字体
                            fontWeight: "normal", //粗细
                            fontSize: 14,
                            align: "center",
                        },
                        x: "center",
                        itemGap: 7,
                    },
                    lines: {
                        effect: {
                            show: true,
                            period: 3, //箭头指向速度，值越小速度越快
                            trailLength: 0.02, //特效尾迹长度[0,1]值越大，尾迹越长重
                            symbol: "arrow", //箭头图标
                            symbolSize: 5, //图标大小
                        },
                        lineStyle: {
                            normal: {
                                width: 2, //尾迹线条宽度
                                opacity: 1, //尾迹线条透明度
                                curveness: 0.1, //尾迹线条曲直度
                            },
                        },
                    },
                    rippleEffect: {
                        //涟漪特效
                        period: 2, //动画时间，值越小速度越快
                        brushType: "stroke", //波纹绘制方式 stroke, fill
                        scale: 2, //波纹圆环最大限制，值越大波纹越大
                    },
                    label: {
                        position: "right",
                        fontSize: 20,
                        color: "#fff",
                    },
                    itemStyle: {
                        normal: {
                            color: "rgba(51, 69, 89, .5)", //地图背景色
                            borderColor: "#516a89", //省市边界线00fcff 516a89
                            borderWidth: 2,
                        },
                        emphasis: {
                            color: "rgba(37, 43, 61, .5)", //悬浮背景
                        },
                    },
                    symbol: {
                        auto: false,
                        size: 10,
                        coefficient: 0.1,
                    },
                    roam: true, //缩放
                    suffix: "人",
                    visualMap: {
                        show: true, //显示图例
                        left: 1,
                        bottom: 1,
                    },
                    tooltip: {
                        fontSize: 20,
                    },
                    // 地图名称
                    mapName: "china",
                    //载入动画
                    animate: "",
                    //通用处理函数
                    customData: "",
                    interactData: [],
                    staticDataValue: [{
                            from: "新乡",
                            to: "新乡",
                            value: 200,
                            lng1: 116.402217,
                            lat1: 35.311657,
                            lng2: 116.402217,
                            lat2: 35.311657,
                        },
                        {
                            from: "新乡",
                            to: "呼和浩特",
                            value: 90,
                            lng1: 116.402217,
                            lat1: 35.311657,
                            lng2: 111.4124,
                            lat2: 40.4901,
                        },
                        {
                            from: "新乡",
                            to: "哈尔滨",
                            value: 60,
                            lng1: 116.402217,
                            lat1: 35.311657,
                            lng2: 127.9688,
                            lat2: 45.368,
                        },
                        {
                            from: "新乡",
                            to: "石家庄",
                            value: 100,
                            lng1: 116.402217,
                            lat1: 35.311657,
                            lng2: 114.4995,
                            lat2: 38.1006,
                        },
                        {
                            from: "新乡",
                            to: "昆明",
                            value: 30,
                            lng1: 116.402217,
                            lat1: 35.311657,
                            lng2: 102.9199,
                            lat2: 25.4663,
                        },
                        {
                            from: "昆明",
                            to: "昆明",
                            type: "node",
                            value: 70,
                            lng1: 102.9199,
                            lat1: 25.4663,
                            lng2: 102.9199,
                            lat2: 25.4663,
                        },
                    ],
                    //绑定的div
                    bindingDiv: "",
                    theme: "",
                    // 数据源类型：静态数据：static；接口数据：url;数据源：database
                    dataSourceType: "static",
                    timeout: 0,
                },
            },
            //  {
            //     chartType: 'tiandiMap',
            //     layerName: '天地图',
            //     icon: "el-icon-datav-tiandiMap",
            //     customId: '',
            //     width: 350,
            //     height: 200,
            //     x: 50,
            //     y: 100,
            //     zindex: 0,
            //     isShow: true,
            //     chartOption: {

            //         //是否显示标注
            //         isBounds: true,
            //         //是否显示鹰眼
            //         isHawkEye: true,
            //         // 是否显示地图类型控件
            //         isMapType: true,

            //         //载入动画
            //         animate: '',
            //         //静态数据值
            //         staticDataValue: [

            //         ],

            //         //绑定的div
            //         bindingDiv: '',
            //         theme: '',
            //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
            //         dataSourceType: 'static',
            //         timeout: 0
            //     }

            // }
        ],
    },

    //知识图谱插件
    {
        chartType: "knowledge",
        layerName: "关系图",
        icon: "el-icon-datav-knowledge",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "", //主标题
                textStyle: {
                    color: "#fff", //颜色
                    fontFamily: "微软雅黑", //字体
                    fontWeight: "normal", //粗细
                    fontSize: 17, //大小
                    align: "center", //水平对齐
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontFamily: "微软雅黑", //字体
                    fontWeight: "normal", //粗细
                    fontSize: 14,
                    align: "center",
                },
                x: "center",
                itemGap: 7,
            },
            legend: {
                show: true, // 是否显示图例
                type: "plain",
                orient: "horizontal",
                right: "10%",
                align: "left",
                top: "top",
                textStyle: {
                    color: "#E7EAED",
                },
                x: "center",
                //height: 150
            },
            pointStyles: {}, //不同类别的节点样式
            lineStyles: {}, //不同链路的样式
            fontColor: "#fff", //颜色
            fontFamily: "微软雅黑", //字体
            fontWeight: "normal", //粗细
            fontSize: 17, //大小
            symbolSize: 50,
            repulsion: 500, //斥力系数
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: {
                nodes: [{
                        name: 'Node 1',
                        x: 300,
                        y: 300
                    },
                    {
                        name: 'Node 2',
                        x: 800,
                        y: 300
                    },
                    {
                        name: 'Node 3',
                        x: 550,
                        y: 100
                    },
                    {
                        name: 'Node 4',
                        x: 550,
                        y: 500
                    }
                ],
                links: [{
                        source: 'Node 2',
                        target: 'Node 1',
                    },
                    {
                        source: 'Node 1',
                        target: 'Node 3'
                    },
                    {
                        source: 'Node 2',
                        target: 'Node 3'
                    },
                    {
                        source: 'Node 2',
                        target: 'Node 4'
                    },
                    {
                        source: 'Node 1',
                        target: 'Node 4'
                    }
                ]
            },
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },

    //气泡图插件
    {
        chartType: "pops",
        layerName: "气泡图",
        icon: "el-icon-datav-pops",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "", //主标题
                textStyle: {
                    color: "#fff", // 颜色
                    fontStyle: "normal", // 风格
                    fontWeight: "normal", // 粗细
                    //fontFamily: 'Microsoft yahei', // 字体
                    fontSize: 20, // 大小
                    align: "center", // 水平对齐
                },
                subtext: "", //副标题
                subtextStyle: {
                    // 对应样式
                    color: "#fff",
                    fontSize: 14,
                    align: "center",
                },
                x: "left",
                itemGap: 7,
            },
            fontSize: 15,
            fontColor: "#fff",
            repulsion: 100, // 泡泡的间距
            shadowBlur: 100, // 阴影范围
            tooltip: {},
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    name: "螺蛳粉",
                    data: 1024,
                },
                {
                    name: "花甲粉",
                    data: 996,
                },
                {
                    name: "酸辣粉",
                    data: 404,
                },
                {
                    name: "鱼粉",
                    data: 403,
                },
                {
                    name: "麻辣烫",
                    data: 2048,
                },
                {
                    name: "米线",
                    data: 980,
                },
                {
                    name: "石锅拌饭",
                    data: 1080,
                },
                {
                    name: "椒麻拌面",
                    data: 1920,
                },
                {
                    name: "炸鸡",
                    data: 3306,
                },
                {
                    name: "汉堡",
                    data: 500,
                },
                {
                    name: "水饺",
                    data: 501,
                },
            ],
            //color: ['#00CFFF', '#006CED', '#FFE000', '#FFA800', '#FF5B00', '#FF3000'],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            bi: null,
            timeout: 0,
        },
    },
    //旭日图插件
    {
        chartType: "sunburst",
        layerName: "旭日图",
        icon: "el-icon-datav-sunburst",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                text: "", //主标题
                textStyle: {
                    color: "#fff", //颜色
                    fontFamily: "微软雅黑", //字体
                    fontWeight: "normal", //粗细
                    fontSize: 17, //大小
                    align: "center", //水平对齐
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontFamily: "微软雅黑", //字体
                    fontWeight: "normal", //粗细
                    fontSize: 14,
                    align: "center",
                },
                x: "center",
            },
            radius: [20, "90%"],
            itemStyle: {
                borderRadius: 17,
                borderWidth: 2,
            },
            label: {
                rotate: "radial",
                fontSize: 12,
                color: "#fff",
                show: true,
            },
            sort: "null",
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    name: "Grandpa",
                    parent: "",
                },
                {
                    name: "Uncle Leo",
                    value: 15,
                    parent: "Grandpa",
                },
                {
                    name: "Cousin Jack",
                    value: 2,
                    parent: "Uncle Leo",
                },
                {
                    name: "Cousin Mary",
                    value: 5,
                    parent: "Uncle Leo",
                },
                {
                    name: "Jackson",
                    value: 2,
                    parent: "Cousin Mary",
                },
                {
                    name: "Cousin Ben",
                    value: 4,
                    parent: "Uncle Leo",
                },
                {
                    name: "Father",
                    value: 10,
                    parent: "Grandpa",
                },
                {
                    name: "Me",
                    value: 5,
                    parent: "Father",
                },
                {
                    name: "Brother Peter",
                    value: 1,
                    parent: "Father",
                },
                {
                    name: "Nancy",
                    parent: "",
                },
                {
                    name: "Uncle Nike",
                    value: 4,
                    parent: "Nancy",
                },
                {
                    name: "Cousin Betty",
                    value: 1,
                    parent: "Uncle Nike",
                },
                {
                    name: "Cousin Jenny",
                    value: 3,
                    parent: "Uncle Nike",
                },
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    //桑基图插件
    {
        chartType: "sankey",
        layerName: "桑基图",
        icon: "el-icon-datav-sankey",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            title: {
                show: true,
                text: "",
                textStyle: {
                    color: "#fff",
                    fontSize: 24,
                    fontFamily: "微软雅黑",
                    align: "center",
                },
                subtext: "", //副标题
                subtextStyle: {
                    //对应样式
                    color: "#fff",
                    fontSize: 20,
                    fontFamily: "微软雅黑",
                    align: "center",
                },
            },
            tooltip: {
                show: true,
                trigger: "item",
                triggerOn: "mousemove",
                textStyle: {
                    fontSize: 20,
                },
            },
            series: [{
                name: "桑基图",
                type: "sankey",
                left: 5,
                top: 5,
                right: 5,
                bottom: 5,
                nodeWidth: 20, //节点宽度
                nodeGap: 8, //节点间距
                // nodeAlign:"justify",//节点对齐方式
                orient: "horizontal", //排布方向
                draggable: false, //是否可拖拽
                label: {
                    show: true,
                    position: "right", //标签位置
                    distance: 5, //标签距离节点距离
                    fontSize: 20,
                    color: "#fff",
                    fontFamily: "微软雅黑",
                },
                lineStyle: {
                    colorSelect: "source",
                    color: "",
                    opacity: 0.3,
                    curveness: 0.5, //阴影曲度
                },
                // emphasis: {
                //     focus: 'series' ,
                // },
            }, ],
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    name: 'a',
                    source: "a",
                    target: "b",
                    value: 3,
                },
                {
                    name: 'b',
                    source: "b",
                    target: "c",
                    value: 2,
                },
                {
                    name: 'c',
                    source: "e",
                    target: "d",
                    value: 5,
                },
                {
                    name: 'd',
                    source: "c",
                    target: "d",
                    value: 1,
                },
                {
                    name: 'e',
                    source: "d",
                    target: "",
                    value: 1,
                },
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },

    //箱线图插件
    {
        chartType: "boxline",
        layerName: "盒须图",
        icon: "el-icon-datav-boxline",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            // title: {
            text: "", // 主标题
            subtext: "", // 副标题
            fontWeight: "normal", // 粗细
            fontFamily: "微软雅黑", // 字体
            fontSize: 20, // 大小
            fontColor: "#3259B8", // 颜色
            x: "center",
            tooltip: {
                trigger: "item",
                axisPointer: {
                    type: "shadow",
                },
            },
            // grid: {
            //     left: '15%',
            //     right: '10%',
            // },
            xAxis: {
                show: true,
                name: "",
                type: "category",
                boundaryGap: true,
                nameGap: 30,
                nameTextStyle: {
                    color: "#3259B8",
                    fontSize: 14,
                },
                axisTick: {
                    show: false,
                },
                splitArea: {
                    show: false
                },
                axisLine: {
                    lineStyle: {
                        color: "#3259B8", // x轴颜色
                    },
                },
                splitLine: {
                    show: false, // x轴分割线
                    lineStyle: {
                        color: "#A7BAFA", // 分割线颜色
                    },
                },
            },
            yAxis: {
                show: true,
                name: "",
                type: "value",
                nameTextStyle: {
                    color: "#3259B8",
                    fontSize: 14,
                },
                axisLabel: {
                    formatter: "{value} CNY/㎡",
                },
                splitArea: {
                    show: true
                },
                axisTick: {
                    show: false,
                },
                axisLine: {
                    lineStyle: {
                        color: "#3259B8", // y轴颜色
                    },
                },
                splitLine: {
                    show: true, // 是否显示分割线
                    lineStyle: {
                        color: "#A7BAFA", // 分割线颜色
                    },
                },
            },
            series: [{
                type: "boxplot",
                data: [
                    [19747, 50024, 64617, 86808, 159949],
                    [23635, 46878, 58095, 84877, 158312],
                    [21543, 47623, 61315.5, 85947, 159978],
                    [18679, 46646, 59842, 84798.25, 159980],
                    [15202, 43073.25, 55168.5, 80136.25, 159825],
                ],
                itemStyle: {
                    normal: {
                        borderColor: {
                            type: "linear",
                            x: 0,
                            y: 0,
                            x2: 0,
                            y2: 1,
                            colorStops: [{
                                    offset: 0,
                                    color: "#F02FC2", // 0% 处的颜色
                                },
                                {
                                    offset: 1,
                                    color: "#3EACE5", // 100% 处的颜色
                                },
                            ],
                            globalCoord: true, // 缺省为 false
                        },
                        borderWidth: 2,
                        color: {
                            type: "linear",
                            x: 0,
                            y: 0,
                            x2: 0,
                            y2: 1,
                            colorStops: [{
                                    offset: 0,
                                    color: "rgba(240,47,194,0.7)", // 0% 处的颜色
                                },
                                {
                                    offset: 1,
                                    color: "rgba(62,172,299,0.7)", // 100% 处的颜色
                                },
                            ],
                            globalCoord: false, // 缺省为 false
                        },
                    },
                },
                tooltip: {
                    trigger: "item",
                    axisPointer: {
                        type: "shadow",
                    },
                },
            }, ],
            //y轴单位
            unit: "CNY/㎡",
            //是否竖显式
            isVertical: false,
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    name: "Bottom",
                    min: 19747,
                    q1: 50024,
                    medium: 64617,
                    q3: 86808,
                    max: 159949,
                    type: "boxplot",
                },
                {
                    name: "Low",
                    min: 23635,
                    q1: 46878,
                    medium: 58095,
                    q3: 84877,
                    max: 158312,
                    type: "boxplot",
                },
                {
                    name: "Middle",
                    min: 21543,
                    q1: 47623,
                    medium: 61315,
                    q3: 85947,
                    max: 159978,
                    type: "boxplot",
                },
                {
                    name: "High",
                    min: 18679,
                    q1: 46646,
                    medium: 59842,
                    q3: 84798,
                    max: 159980,
                    type: "boxplot",
                },
                {
                    name: "Top",
                    min: 15202,
                    q1: 43073,
                    medium: 55168,
                    q3: 80136,
                    max: 159825,
                    type: "boxplot",
                },
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },

    //树状结构图
    // {
    //     chartType:'relationGraph',
    //     layerName:"树状结构图",
    //     icon: "el-icon-datav-tree",
    //     customId: '',
    //     width: 350,
    //     height: 200,
    //     x: 50,
    //     y: 100,
    //     zindex: 0,
    //     isShow: true,
    //     chartOption:{
    //         defaultGraphOptions:{
    //             backgrounImage:"",//图谱水印url
    //             backgrounImageNoRepeat:true, //只在右下角显示水印，不重复显示水印
    //             allowShowMiniToolBar:false, //是否显示工具栏
    //             allowShowMiniView:false, //是否显示缩略图
    //             allowShowMiniNameFilter:false, //是否显示搜索框
    //             allowSwitchLineShape:true, //是否在工具栏中显示切换线条形状的按钮
    //             allowSwitchJunctionPoint:false, //是否在工具栏中显示切换连接点位置的按钮
    //             disableZoom:false, //是否禁用鼠标滚轮缩放功能
    //             disableDragNode:false, //是否禁用图谱中节点的拖动
    //             moveToCenterWhenResize:true, //当图谱的大小发生变化时，是否重新让图谱的内容看起来居中
    //             defaultFocusRootNode:true, //默认为根节点添加一个被选中的样式
    //             allowShowZoomMenu:true, //是否在右侧菜单栏显示放大缩小的按钮，此设置和disableZoom不冲突
    //             isMoveByParentNode:false, //是否在拖动节点后让子节点跟随
    //             hideNodeContentByZoom:false, //是否根据缩放比例隐藏节点内容
    //             layouts:[{
    //                 label: '树状布局',
    //                 layoutName: 'tree',
    //                 layoutClassName: 'seeks-layout-center',
    //                 useLayoutStyleOptions: true,
    //                 from: 'top',
    //                 defaultJunctionPoint: 'tb',
    //                 defaultNodeWidth: 60,
    //                 defaultNodeHeight: 60,
    //                 defaultNodeShape: 0,//默认的节点形状，0:圆形；1:矩形
    //                 defaultNodeColor: 'rgba(0, 206, 209, 1)',
    //                 defaultNodeBorderWidth: 0,
    //                 defaultNodeBorderColor:"#fc9803",
    //                 defaultNodeFontColor:"#000",
    //                 defaultLineWidth: 2,
    //                 defaultLineShape: 4,
    //                 defaultLineColor: 'rgba(0, 186, 189, 1)',
    //                 min_per_width: 50,
    //                 max_per_width: 150,
    //                 min_per_height: 200
    //             }]
    //         },
    //         animate: '',
    //         //静态数据值
    //         staticDataValue:
    //             {
    //                 rootId: 'a',
    //                 nodes: [
    //                     { id: 'a', text: 'A', },
    //                     { id: 'b', text: 'B', },
    //                     { id: 'c', text: 'C', },
    //                     { id: 'e', text: 'E', },
    //                 ],
    //                 links: [
    //                     { from: 'a', to: 'b', text: '关系1' },
    //                     { from: 'a', to: 'c', text: '关系2' },
    //                     { from: 'a', to: 'e', text: '关系3' },
    //                 ]
    //             },
    //         //绑定的div
    //         bindingDiv: '',
    //         theme: '',
    //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
    //         dataSourceType: 'static',
    //         bi: null,
    //         timeout: 0
    //     }
    // },
];

// 文本组件
export const textComponents = [{
        chartType: "text",
        layerName: "文本框",
        icon: "el-icon-datav-text",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            //静态数据值
            staticDataValue: "文本框",
            fontSize: 25,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            letterSpacing: 0,
            lineHeight: 40,
            border: 0, //0无边框//1有边框
            borderColor: '',
            borderWidth: 1,
            backgroundType: "color",
            backgroundColor: "",
            backgroundImage: '', //背景图片
            textAlign: "center",
            alignItems: 'flex-start',
            display: 'block',
            conRadius: 0,
            topPadding: 0,
            leftPadding: 0,
            customData: "",
            interactData: [],
            border: "",
            animate: "",
            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "lamp",
        layerName: "跑马灯",
        icon: "el-icon-datav-lamp",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            fontSize: 25,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            letterSpacing: 0,
            lineHeight: 40,
            backgroundColor: "",
            textAlign: "center",
            loopDelay: 30,
            border: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: ["跑马灯", "哈哈哈"],

            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "date",
        layerName: "实时时间",
        icon: "el-icon-datav-date",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            fontSize: 25,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            letterSpacing: 0,
            lineHeight: 40,
            backgroundColor: "",
            textAlign: "center",
            dateFormat: "yyyy-MM-dd HH:mm:ss",
            customFormat: "yyyy-MM-dd HH:mm:ss",
            isCustom: false,
            border: "",
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: [{ text: "" }],

            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "colorBlock",
        layerName: "颜色块",
        icon: "el-icon-datav-colorBlock",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            //块属性：颜色，后缀
            colorBlock: [],
            text: {
                left: "flex-start",
                size: 18,
                color: "#fff",
                weight: "normal",
                family: "黑体",
                width: 60,
            },
            value: {
                left: "flex-end",
                size: 36,
                color: "#fff",
                family: "Digital",
                weight: "normal",
            },
            suffix: {
                size: 16,
                color: "#fff",
                family: "黑体",
                weight: "normal",
                width: 30,
            },
            textLocate: "left",

            numOfCol: 2, //列数
            //colorheight:'30',//颜色块的高
            marginLeft: 0,
            marginTop: 0, //间隔
            bradius: 5, //边框圆角

            isThousand: ",",
            //静态数据值
            staticDataValue: [
                { value: 335, name: "数据1", suffix: "单位1" },
                { value: 310, name: "数据2", suffix: "单位2" },
                { value: 234, name: "数据3", suffix: "单位3" },
                { value: 135, name: "数据4", suffix: "单位4" },
                { value: 135, name: "数据5", suffix: "单位5" },
                { value: 1548, name: "数据6", suffix: "单位6" },
            ],
            tableSelectLine: [],
            finalResult: [],
            //载入动画
            animate: "",
            customData: "",
            interactData: [],

            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "wordCloud",
        layerName: "字符云",
        icon: "el-icon-datav-wordcloud",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            tooltip: {},
            gridSize: 3, // 单词之间的间隔大小
            sizeMin: 15,
            sizeMax: 60, // 最小字体和最大字体
            rotationMin: -45,
            rotationMax: 90, // 字体旋转角度的范围
            shape: "pentagon", // 词云的形状
            textStyle: {
                normal: {
                    color: function() {
                        return (
                            "rgb(" + [
                                Math.round(Math.random() * 255),
                                Math.round(Math.random() * 255),
                                Math.round(Math.random() * 255),
                            ].join(",") +
                            ")"
                        );
                    },
                },
                emphasis: {
                    shadowBlur: 10,
                    shadowColor: "#333",
                },
            },
            //静态数据值
            staticDataValue: [{
                    name: "北京",
                    value: 273,
                },
                {
                    name: "上海",
                    value: 265,
                },
                {
                    name: "深圳",
                    value: 200,
                },
                {
                    name: "重庆",
                    value: 150,
                },
                {
                    name: "天津",
                    value: 300,
                },
                {
                    name: "哈尔滨",
                    value: 205,
                },
            ],
            //载入动画
            animate: "",
            //通用处理函数
            customData: "",
            interactData: [],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "flop",
        layerName: "翻牌器",
        icon: "el-icon-datav-flop",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            fontSize: 60,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "#CDFBFF", //字体颜色
            letterSpacing: 0, //字体间距
            backgroundColor: "rgba(0, 0, 0, 0)", //背景颜色1
            selectedBackgroundColor: "#0070C6", //背景颜色2
            borderColor: "rgba(105, 123, 148, 0.5)",
            borderWidth: "0",
            cardWidth: "38",
            startVal: 0, //开始值
            duration: 3000, //持续时间，以毫秒为单位
            decimals: 0, //要显示的小数位数
            separator: ",", //分隔符
            prefix: "¥ ", //前缀
            suffix: " rmb", //后缀
            //静态数据值
            staticDataValue: 2021,
            //载入动画
            animate: "",
            customData: "",
            interactData: [],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "card",
        layerName: "卡片",
        icon: "el-icon-datav-card",
        customId: "",
        width: 450,
        height: 520,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            head: {
                height: 30, //头像整体div高度占比
                marginTop: 0, //头像整体div上边距
                headImg: {
                    height: 90, //头像高度占比
                    width: 29, //头像宽度占比
                    marginLeft: 38, //头像左边距
                },
                tipImg: {
                    src: "", //提示框图片地址
                    height: 20, //提示框图片高度占比
                    width: 35, //提示框图片宽度占比
                    marginLeft: 60, //提示框图片左边距
                    marginTop: 0, //提示框图片上边距
                    show: "block",
                    sync: true, //与提示框文字同步
                },
                tipText: {
                    fontSize: 16, //提示框文字大小
                    fontColor: "#fff", //提示框文字颜色
                    fontFamily: "宋体", //字体名称
                    fontWeight: "normal", //字体粗细
                    letterSpacing: 2, //字体间距
                    marginTop: 2, //提示框文字上边距
                    marginLeft: 75, //提示框文字左边距
                },
            },
            name: {
                fontSize: 30, //名称文字大小
                fontColor: "#fff", //名称文字颜色
                fontFamily: "宋体", //字体名称
                fontWeight: "normal", //字体粗细
                letterSpacing: 2, //字体间距
                marginTop: 0, //名称文字上边距
                marginLeft: 38, //名称文字左边距
                marginBottom: 2, //名称文字左边距
            },
            infoItem: {
                height: 30, //整体高度占比
                fontSize: 30, //信息行文字大小
                fontColor: "#fff", //信息行文字颜色
                fontFamily: "宋体", //字体名称
                fontWeight: "normal", //字体粗细
                letterSpacing: 2, //字体间距
                marginTop: 5, //信息行上边距
                textMarginLeft: 3, //文字距背景图片左边距
                textMarginTop: 5, //文字距背景图片上边距
                background: "img", //信息行文字背景类型
                backgroundColor: "#001C50", //信息行文字背景色
                backgroundImg: "", //信息行文字背景图片
            },
            detailItem: {
                height: 30, //整体高度占比
                fontSize: 30, //详细信息行文字大小
                fontColor: "#fff", //详细信息行文字颜色
                fontFamily: "宋体", //字体名称
                fontWeight: "normal", //字体粗细
                letterSpacing: 2, //字体间距
                marginTop: 5, //详细信息行上边距
                textMarginTop: 5, //文字距背景图片上边距
                background: "img", //详细信息行文字背景类别
                backgroundColor: "#001C50", //详细信息行文字背景颜色
                backgroundImg: "", //详细信息行文字背景图片
            },
            numOfCol: 1, //列数
            marginLeft: 0, //横向间距
            marginTop: 0, //纵向间距
            borderRadius: 0, //圆角
            bgType: "color", //卡片背景类型
            bgTypeColor: "", //卡片背景色
            bgTypeImg: "", //卡片背景图片
            //静态数据值
            staticDataValue: [{
                id: "1",
                head: {
                    headImg: "http://221.212.111.73:2080/prod-api/profile/backgroundBox/2022/04/14/d60a075bdd447611d2ac2f48806226cc.png",
                    tipText: "迟到1次",
                },
                name: "测试人员",
                infoItem: [
                    { label: "联系电话", value: "13312345678" },
                    { label: "工作年限", value: "12年" },
                ],
                detailItem: [{
                        label: "工作日报查看",
                        address: "https://echarts.apache.org/zh/option.html#series-effectScatter.label.show",
                    },
                    { label: "工作周报查看", address: "" },
                ],
            }, ],
            //载入动画
            animate: "",
            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "详情项事件", func: "onItemClick", code: "" },
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    }, {
        chartType: "paigangBlock",
        layerName: "排缸文本块",
        icon: "el-icon-datav-colorBlock",
        customId: "",
        width: 180,
        height: 168,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            containerBgImage: '', //容器背景图片
            containerBgColor: '', //容器背景颜色
            containerRadius: 0, //容器弧度
            loopDirection: '1', //循环方向，1为横向，2为纵向
            isRoll: true,
            rollSpeed: 0.5,
            rollDirection: 1,
            blockalignContent: 'stretch',
            //块属性：颜色，后缀
            blockwidth: 25,
            blockheight: 16.66,
            colorBlock: [],
            blockBorderColorType: '1', //边框1单个2根据字段控制
            blockBgColorType: '1', //1单个颜色2根据字段控制
            blockBgType: '0', //undefind和0颜色，1图片
            blockBgColor: 'rgba(7, 24, 105, 1)', //颜色块背景设设置
            liTextStyleList: [],
            allStyleList: [], //所有的样式
            ishideIcon: false, //是否隐藏图标
            liText: {
                lefttext: "left",
                righttext: "right",
                size: 14,
                colorType: '1', //1单个颜色2根据字段控制
                color: '#fff',
                weight: "normal",
                family: "黑体",
                width: 60,
                marginLeft: 12,
                marginTop: 7, //间隔
                lineHeight: 28,
                minwidth: 50,
                isOverHide: false, //是否溢出隐藏
                isShowEllipsis: false, //是否显示省略号
            },
            progressbar: {
                conBgJianbianList: [
                    [{
                        color: '#1E35BA',
                        percentage: 100
                    }]
                ],
                imageUrl: '/assets/images/paigangicon.png',
                iconUrl: '',
                baralign: "flex-start",
                alignItems: "flex-start",
                size: 14,
                conBgJianbian: [], //进度条背景颜色
                bgJianbianlist: [
                    [{
                        color: '#3CC7FF',
                        percentage: 100
                    }]
                ],
                bgJianbian: [], //进度条颜色
                colorType: '1', //1单个颜色2根据字段控制
                color: '#fff', //进度条字体颜色
                weight: "normal",
                family: "黑体",
                width: 60,
                height: 34,
                conRadius: 6,
                radius: 6,
                again: 'left',
                paddingTop: 0,
                paddingRight: 0,
                paddingBottom: 0,
                paddingLeft: 0,
                isshowstatus: false,
                statusFontSize: 16,
                statuscolorType: '1',
                statusBorderShowType: '1',
                statusFontColor: '#ffffff',
                isshowstatusBorder: true,
                statusBorderColor: '#E74033',
                statusBorderWidth: 1,
                statusBorderType: 'solid',
                statuspaddingTop: 10,
                statuspaddingRight: 10,
                statuspaddingBottom: 10,
                statuspaddingLeft: 10,
                statusmarginTop: 0,
                statusmarginRight: 0,
                statusmarginBottom: 0,
                statusmarginLeft: 0,
                statusHasBorder: false,
                statusflexDirection: 'row',
                barStatusalign: 'flex-start',
                statusAlignItems: 'center',
                statusweight: "normal",
                statusfamily: "黑体",
                statusconRadius: 6,
                statusbgJianbian: [], //进度条背景颜色
                statusbgJianbianlist: [
                    [{
                        color: '#3CC7FF',
                        percentage: 100
                    }]
                ],
            },
            textLocate: "left",

            numOfCol: 2, //列数
            //colorheight:'30',//颜色块的高
            marginLeft: 12,
            marginTop: 7, //间隔
            bradius: 5, //边框圆角
            isetBorder: false,
            borderInfolist: [{
                width: 1,
                type: 'solid',
                color: '#fff'
            }],
            borderInfoVallist: [{
                value: '',
                sort: 0,
            }],
            align: 'flex-start',
            isThousand: ",", //千分位
            //静态数据值
            staticDataValue: [
                { name: 'HG001[001]', list: ['6370', '2活性黑', '942米', '200KG', '预 13:06', '预 02:06'] },
                { name: 'HG001[001]', list: ['6370', '2活性黑', '942米', '200KG', '预 13:06', '预 02:06'] },
                { name: 'HG001[001]', list: ['6370', '2活性黑', '942米', '200KG', '预 13:06', '预 02:06'] },
            ],
            tableSelectLine: [],
            finalResult: [],
            //载入动画
            animate: "",
            customData: "",
            interactData: [],

            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
];

// 媒体组件
export const mediaComponents = [{
        chartType: "image",
        layerName: "图片",
        icon: "el-icon-datav-img",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            fit: "fill",
            bindingDiv: "",
            imgURL: "", //图片地址
            opacity: 1, //透明度
            rotation: false,
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: "https://cn.vuejs.org/images/logo.png",

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "qRcode",
        layerName: "二维码",
        icon: "el-icon-datav-qRcode",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            size: 200,
            imageUrl: "",
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: "https://www.baidu.com",

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "video",
        layerName: "视频",
        icon: "el-icon-datav-video",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            replay: "loop",
            auto: false,
            controls: true,
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: "http://x5.demo.openwbs.com/ow-content/uploads/videos/demovideo.mp4",
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "realtime",
        layerName: "实时视频",
        icon: "el-icon-datav-realtime",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            controls: true,
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: {
                type: "rtmp/mp4", // 类型
                src: "rtmp://58.200.131.2:1935/livetv/hunantv", // url地址
            },
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            timeout: 0,
        },
    },
    {
        chartType: "hyperLink",
        layerName: "超链接",
        icon: "el-icon-datav-hyperlink",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            target: "_blank",
            label: {
                context: "超链接",
                fontSize: 20,
                fontFamily: "宋体",
                linkColor: "rgb(81,0,254)",
                visitedColor: "rgb(128,0,128)",
                hoverColor: "rgb(255,0,0)",
                activeColor: "rgb(255,0,0)",
                decoration: "underline", //overline有问题 blink 只能在firefox中使用 只有 none underline line-through能用
            },
            fontWeight: 'normal',
            img: {
                src: "https://cn.vuejs.org/images/logo.png",
                width: "50",
                height: "50",
            },
            //显示字体还是图片
            type: "label",
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: "https://www.baidu.com",

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "iframe",
        layerName: "iframe",
        icon: "el-icon-datav-iframe",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            backgroundColor: "",
            customData: "",
            interactData: [],
            //载入动画
            animate: "",
            staticDataValue: "https://www.baidu.com",
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            timeout: 0,
        },
    },
    {
        chartType: "rotation",
        layerName: "轮播图",
        icon: "el-icon-datav-rotation",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            //是否3D效果
            isEffect: "slide", // 3d效果
            //轮播速度
            playSpeed: 5000,
            //滚动方向
            direction: "horizontal", //vertical 纵向
            isPagination: false, //是否显示分页器
            paginationType: "bullets",
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                    value: "https://cn.vuejs.org/images/logo.png",
                },
                {
                    value: "https://cn.vuejs.org/images/logo.png",
                },
            ],
            //载入动画
            animate: "",

            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "effects",
        layerName: "特效",
        icon: "el-icon-datav-effects",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        child: [{
                chartType: "scanning",
                layerName: "扫描",
                icon: "el-icon-datav-scanning",
                customId: "",
                width: 600,
                height: 600,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    bindingDiv: "",
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    border: "",
                    opacity: 1,
                    //载入动画
                    animate: "",
                    //扫描动画时长(秒)
                    scanDur: 3,
                    //光晕动画时长(秒)
                    haloDur: 2,
                    color1: "#37a2da",
                    color2: "#007cbd",
                },
            },
            {
                chartType: "turntable",
                layerName: "转盘",
                icon: "el-icon-datav-turntable",
                customId: "",
                width: 300,
                height: 300,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    bindingDiv: "",
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    border: "",
                    opacity: 1,
                    //载入动画
                    animate: "",
                    //单次动画时长(秒)
                    dur: 3,
                    color1: "#37a2da",
                    color2: "#007cbd",
                },
            },
            {
                chartType: "galaxy",
                layerName: "银河系",
                icon: "el-icon-datav-galaxy",
                customId: "",
                width: 300,
                height: 300,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    opts: {
                        lineCount: 100,
                        starCount: 30,

                        radVel: 0.01,
                        lineBaseVel: 0.1,
                        lineAddedVel: 0.1,
                        lineBaseLife: 0.4,
                        lineAddedLife: 0.01,

                        starBaseLife: 10,
                        starAddedLife: 10,

                        ellipseTilt: -0.3,
                        ellipseBaseRadius: 0.15,
                        ellipseAddedRadius: 0.02,
                        ellipseAxisMultiplierX: 2,
                        ellipseAxisMultiplierY: 1,
                        repaintAlpha: 0.015,
                    },
                    bindingDiv: "",
                    dataSourceType: "static",
                    //静态数据值
                    staticDataValue: "",
                    border: "",
                    opacity: 1,
                    //载入动画
                    animate: "",
                },
            },
            {
                chartType: "twinkleBorder",
                layerName: "边框",
                icon: "el-icon-datav-twinkleborder",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    borderType: "dv-border-box-1",
                    color1: "#37a2da",
                    color2: "#007cbd",
                    backgroundColor: "",
                    dur: 6,
                    reverse: false,
                    title: "",
                    titleWidth: 250,
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    animate: "",
                    //绑定的div
                    bindingDiv: "",
                },
            },
            {
                chartType: "ray",
                layerName: "射线",
                icon: "el-icon-datav-ray",
                customId: "",
                width: 350,
                height: 200,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    color1: "#37a2da",
                    reverse: false,
                    dur: 6,
                    rotate: 0,
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    animate: "",
                    //绑定的div
                    bindingDiv: "",
                },
            },
            {
                chartType: "midiKeyboard",
                layerName: "MIDI键盘",
                icon: "el-icon-datav-midikeyboard",
                customId: "",
                width: 350,
                height: 50,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    color1: "#37a2da",
                    color2: "#031a31",
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    animate: "",
                    //绑定的div
                    bindingDiv: "",
                },
            },
            {
                chartType: "voiceprint",
                layerName: "声纹",
                icon: "el-icon-datav-voiceprint",
                customId: "",
                width: 350,
                height: 50,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    color1: "#37a2da",
                    color2: "#37a2da",
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    animate: "",
                    //绑定的div
                    bindingDiv: "",
                },
            },
            {
                chartType: "diffuseray",
                layerName: "发散射线",
                icon: "el-icon-datav-diffuseray",
                customId: "",
                width: 350,
                height: 50,
                x: 50,
                y: 100,
                zindex: 0,
                isShow: true,
                chartOption: {
                    color1: "#37a2da",
                    color2: "#37a2da",
                    dur: 2,
                    //静态数据值
                    staticDataValue: "",
                    dataSourceType: "static",
                    animate: "",
                    //绑定的div
                    bindingDiv: "",
                },
            },
        ],
    },
];

//表格组件
export const tableComponents = [{
        chartType: "table",
        layerName: "分页表格",
        icon: "el-icon-datav-taBle",
        customId: "",
        width: 600,
        height: 350,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            pageSize: 5,
            cols: [
                { field: "id", title: "列名id", width: '', textColor: "#ffffff" },
                { field: "name", title: "列名name", width: '', textColor: "#ffffff" },
                { field: "age", title: "列名age", width: '', textColor: "#ffffff" },
                { field: "score", title: "列名score", width: '', textColor: "#ffffff" },
            ], //表头
            isLeft: "center", //对齐
            theadbackgroundcolor: "rgba(10, 30, 94, 0.83)",
            theadheight: 35, //表头行高
            theadfontcolor: "rgba(255,255,255,.6)",
            theadfontsize: 18,
            theadfontfamily: "微软雅黑",
            tbodyheight: 40, //表格行高
            tbodybackgroundcolor: "",
            tbodyfontcolor: "rgba(255,255,255,.6)",
            tbodyfontsize: 18,
            tbodyfontfamily: "微软雅黑",
            isInitialized: false,
            pagination: true,
            //静态数据值
            staticDataValue: [
                { id: "20078d08", name: "学生0", age: 77, score: 99 },
                { id: "87450164", name: "学生1", age: 19, score: 18 },
                { id: "1baa7837", name: "学生2", age: 24, score: 51 },
                { id: "0cbbc35d", name: "学生3", age: 14, score: 72 },
                { id: "6e1fe529", name: "学生4", age: 10, score: 88 },
                { id: "c6f63bd4", name: "学生5", age: 64, score: 74 },
                { id: "558410ad", name: "学生6", age: 68, score: 92 },
                { id: "5473509b", name: "学生7", age: 18, score: 7 },
            ],
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            bi: null,
            timeout: 0,
            linkageCols: [{
                field: "",
                bindingChart: [],
                paramName: "",
            }, ],
        },
    },
    {
        chartType: "tableRotation",
        layerName: "滚动表格",
        icon: "el-icon-datav-tablerotation",
        customId: "",
        width: 600,
        height: 350,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            cols: [
                { field: "id", title: "列名id", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "name", title: "列名name", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "age", title: "列名age", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "score", title: "列名score", width: 25, isLeft: '', textColor: "#ffffff" },
            ], //表头
            theadStyle: [], //表头样式:：列名，绑定id，列宽
            isLeft: "center", //对齐
            tableColumsStyle: [], //表格行根据是字段判断样式
            isthead: true,
            theadbackgroundcolor: "rgba(10, 30, 94, 0.83)",
            theadheight: 35, //表头行高
            theadfontcolor: "rgba(255,255,255,.6)",
            theadfontsize: 18,
            theadfontfamily: "微软雅黑",

            tbodyheight: 40, //表格行高
            tbodyfontcolor: "rgba(255,255,255,.6)",
            tbodyfontsize: 18,
            tbodyfontfamily: "微软雅黑",

            evencolor: "",
            oddcolor: "",

            progress: {
                color: "#10FEF2",
                textcolor: "#10FEF2",
                textsize: 16,
                width: 13,
            },

            isoverflowtip: true, //提示框
            Ocolor: "#000000", //字色
            Ofontsize: 18, //字号
            Obgcolor: "#fff", //背景色
            direction: "0", //向下
            velocity: 0.6, //滚动速度
            isrunshort: false, //不足是否滚动
            isThousand: ",",
            singleHeight: 0, //单行停顿
            waitTime: 0, //停顿时间
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //isfirst:"true",//是否第一次加载，true：是，false：否
            //静态数据值
            staticDataValue: [
                { id: "20078d08", name: "学生0", age: 77, score: 99 },
                { id: "87450164", name: "学生1", age: 19, score: 98 },
                { id: "1baa7837", name: "学生2", age: 24, score: 91 },
                { id: "0cbbc35d", name: "学生3", age: 14, score: 92 },
                { id: "6e1fe529", name: "学生4", age: 10, score: 98 },
                { id: "c6f63bd4", name: "学生5", age: 64, score: 74 },
                { id: "558410ad", name: "学生6", age: 68, score: 92 },
                { id: "5473509b", name: "学生7", age: 18, score: 97 },
            ],
            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            bi: null,
            timeout: 0,
            linkageCols: [{
                field: "",
                bindingChart: [],
                paramName: "",
            }, ],
        },
    }, {
        chartType: "tablePaging",
        layerName: "自定义分页表格",
        icon: "el-icon-datav-taBle",
        customId: "",
        width: 600,
        height: 350,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            cols: [
                { field: "id", title: "列名id", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "name", title: "列名name", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "age", title: "列名age", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "score", title: "列名score", width: 25, isLeft: '', textColor: "#ffffff" },
            ], //表头
            theadStyle: [], //表头样式:：列名，绑定id，列宽
            isLeft: "center", //对齐
            tableColumsStyle: [], //表格行根据是字段判断样式
            isthead: true,
            theadbackgroundcolor: "rgba(10, 30, 94, 0.83)",
            theadheight: 35, //表头行高
            theadfontcolor: "rgba(255,255,255,.6)",
            theadfontsize: 18,
            theadfontfamily: "微软雅黑",

            tbodyheight: 40, //表格行高
            tbodyfontcolor: "rgba(255,255,255,.6)",
            tbodyfontsize: 18,
            tbodyfontfamily: "微软雅黑",

            evencolor: "",
            oddcolor: "",

            progress: {
                color: "#10FEF2",
                textcolor: "#10FEF2",
                textsize: 16,
                width: 13,
            },

            isoverflowtip: true, //提示框
            Ocolor: "#000000", //字色
            Ofontsize: 18, //字号
            Obgcolor: "#fff", //背景色
            isThousand: ",",
            pagination: true,
            pageSize: 5,
            pageColor: '#ffffff',
            showRowBorder: true,
            showColumnBorder: false,
            borderWidth: 1,
            borderColor: '#ffffff',
            hideOne: true, //只有第一页是否隐藏
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //isfirst:"true",//是否第一次加载，true：是，false：否
            //静态数据值
            staticDataValue: [
                { id: "20078d08", name: "学生0", age: 77, score: 99 },
                { id: "87450164", name: "学生1", age: 19, score: 98 },
                { id: "1baa7837", name: "学生2", age: 24, score: 91 },
                { id: "0cbbc35d", name: "学生3", age: 14, score: 92 },
                { id: "6e1fe529", name: "学生4", age: 10, score: 98 },
                { id: "c6f63bd4", name: "学生5", age: 64, score: 74 },
                { id: "558410ad", name: "学生6", age: 68, score: 92 },
                { id: "5473509b", name: "学生7", age: 18, score: 97 },
            ],
            //绑定的div
            bindingDiv: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            bi: null,
            timeout: 0,
            linkageCols: [{
                field: "",
                bindingChart: [],
                paramName: "",
            }, ],
        },
    },
    {
        chartType: "tableTop",
        layerName: "排行表格",
        icon: "el-icon-datav-tabletop",
        customId: "",
        width: 600,
        height: 350,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            cols: [
                { field: "id", title: "列名id", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "name", title: "列名name", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "age", title: "列名age", width: 25, isLeft: '', textColor: "#ffffff" },
                { field: "score", title: "列名score", width: 25, isLeft: '', textColor: "#ffffff" },
            ],
            theadStyle: [], //表头样式:：列名，绑定id，列宽
            isLeft: "center", //对齐
            tableColumsStyle: [], //表格行根据是字段判断样式
            isthead: true,
            theadbackgroundcolor: "rgba(10, 30, 94, 0.83)",
            theadheight: 32, //表头行高
            theadfontcolor: "rgba(255,255,255,.6)",
            theadfontsize: 16,
            theadfontfamily: "微软雅黑",

            tbodyheight: 40, //表格行高
            tbodyfontcolor: "rgba(255,255,255,.6)",
            tbodyfontsize: 18,
            tbodyfontfamily: "微软雅黑",

            isThousand: ",",
            evencolor: "",
            oddcolor: "",

            progress: {
                color: "#10FEF2",
                textcolor: "#10FEF2",
                textsize: 16,
                width: 13,
            },

            showRank: true, //显示排行
            isoverflowtip: true, //提示框
            Ocolor: "#000000", //字色
            Ofontsize: 18, //字号
            Obgcolor: "#fff", //背景色
            //isfirst:"true",//是否第一次加载，true：是，false：否
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [
                { id: "20078d08", name: "学生0", age: 77, score: 99 },
                { id: "87450164", name: "学生1", age: 19, score: 98 },
                { id: "1baa7837", name: "学生2", age: 24, score: 91 },
                { id: "0cbbc35d", name: "学生3", age: 14, score: 92 },
                { id: "6e1fe529", name: "学生4", age: 10, score: 98 },
                { id: "c6f63bd4", name: "学生5", age: 64, score: 74 },
                { id: "558410ad", name: "学生6", age: 68, score: 92 },
                { id: "5473509b", name: "学生7", age: 18, score: 97 },
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            bi: null,
            timeout: 0,
            linkageCols: [{
                field: "",
                bindingChart: [],
                paramName: "",
            }, ],
        },
    },
    // {
    //     chartType: 'handsontable',
    //     layerName: '报表',
    //     icon: "el-icon-datav-handsonTable",
    //     customId: '',
    //     width: 600,
    //     height: 350,
    //     x: 50,
    //     y: 100,
    //     zindex: 0,
    //     isShow: true,
    //     chartOption: {
    //         //表头
    //         cols: [
    //             {  title: "选择" },
    //             {  title: "题目" },
    //             {  title: "A选项" },
    //             {  title: "B选项" },
    //             {  title: "C选项" },
    //             {  title: "D选项" },
    //             {  title: "答案" }
    //         ],
    //         autoWrapRow: true,//自动换行
    //         fixedColumnsLeft: 0,//固定左边列数
    //         fixedRowsTop: 0,//固定上边列数
    //         rowHeaders: true,//显示行号
    //         colHeaders: true,//显示表头
    //         readonly: true,//表格是否只读，有true和false两种选择
    //         comments: true, //添加注释 ，可以显示注释信息
    //         //载入动画
    //         animate: '',
    //         //静态数据值
    //         staticDataValue: [
    //             [false,'20080101', 10, 11, 12, 13,true],

    //             [false,'20090101', 20, 11, 14, 13,true],

    //             [false,'20010101', 30, 15, 12, 13,true],

    //             [false,'20010101', 32, 213, 21, 312,true],

    //             [false,'20010201', 32, 213, 21, 312,true]
    //         ],
    //         //绑定的div
    //         bindingDiv: '',
    //         theme: '',
    //         // 数据源类型：静态数据：static；接口数据：url;数据源：database
    //         dataSourceType: 'static',
    //         bi: null,
    //         timeout: 0
    //     }
    // }
];

//条件组件
export const factorComponents = [{
        chartType: "input",
        layerName: "输入框",
        icon: "el-icon-datav-input",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            label: {
                context: "标签",
                fontSize: 20,
                fontFamily: "宋体",
                fontColor: "#fff",
                position: 'left',
                icon: ''
            },
            input: {
                name: "input_name",
                maxlength: 20,
                type: "text",
            },
            params: '',
            isHideInput: false,
            isDisableInput: false,
            width: 200,
            height: 40,
            //载入动画
            bindList: [],
            animate: "",
            customData: "",
            //请求参数 例如：?a=1&b=2
            requestParameters: [],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "输入事件", func: "onInput", code: "" },
                { name: "变更事件", func: "onChange", code: "" },
            ],
            timeout: 0,
        },
    },
    {
        chartType: "timeFrame",
        layerName: "时间范围",
        icon: "el-icon-datav-time",
        customId: "",
        width: 500,
        height: 100,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            context: "时间范围",
            fontSize: 18,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            type: "daterange",
            name: "time_name",
            bindList: [],
            width: 200,
            //载入动画
            animate: "",
            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "变更事件", func: "onChange", code: "" },
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "select",
        layerName: "下拉菜单",
        icon: "el-icon-datav-select",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            context: "搜索框",
            fontSize: 18,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            name: "select_name",
            width: 200,
            //绑定组件id
            selectedCharts: "",
            //静态数据值
            staticDataValue: [{
                    value: "1",
                    tab: "test1",
                },
                {
                    value: "2",
                    tab: "test2",
                },
                {
                    value: "3",
                    tab: "test3",
                },
                {
                    value: "4",
                    tab: "test4",
                },
            ],
            //载入动画
            animate: "",
            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "变更事件", func: "onChange", code: "" }
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "cascade",
        layerName: "级联选择",
        icon: "el-icon-datav-cascade",
        customId: "",
        width: 550,
        height: 100,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            context: "省市区选择",
            fontSize: 18,
            fontFamily: "宋体",
            fontColor: "#fff",
            fontWeight: "normal",
            name: "cascade_name",
            width: 300,
            bindList: [],
            //静态数据值
            staticDataValue: [{
                value: 'zhinan',
                label: '指南',
                children: [{
                    value: 'shejiyuanze',
                    label: '设计原则',
                    children: [{
                        value: 'yizhi',
                        label: '一致'
                    }, {
                        value: 'fankui',
                        label: '反馈'
                    }, {
                        value: 'xiaolv',
                        label: '效率'
                    }, {
                        value: 'kekong',
                        label: '可控'
                    }]
                }, {
                    value: 'daohang',
                    label: '导航',
                    children: [{
                        value: 'cexiangdaohang',
                        label: '侧向导航'
                    }, {
                        value: 'dingbudaohang',
                        label: '顶部导航'
                    }]
                }]
            }, {
                value: 'zujian',
                label: '组件',
                children: [{
                    value: 'basic',
                    label: 'Basic',
                    children: [{
                        value: 'layout',
                        label: 'Layout 布局'
                    }, {
                        value: 'color',
                        label: 'Color 色彩'
                    }, {
                        value: 'typography',
                        label: 'Typography 字体'
                    }, {
                        value: 'icon',
                        label: 'Icon 图标'
                    }, {
                        value: 'button',
                        label: 'Button 按钮'
                    }]
                }, {
                    value: 'form',
                    label: 'Form',
                    children: [{
                        value: 'radio',
                        label: 'Radio 单选框'
                    }, {
                        value: 'checkbox',
                        label: 'Checkbox 多选框'
                    }, {
                        value: 'input',
                        label: 'Input 输入框'
                    }, {
                        value: 'input-number',
                        label: 'InputNumber 计数器'
                    }, {
                        value: 'select',
                        label: 'Select 选择器'
                    }, {
                        value: 'cascader',
                        label: 'Cascader 级联选择器'
                    }, {
                        value: 'switch',
                        label: 'Switch 开关'
                    }, {
                        value: 'slider',
                        label: 'Slider 滑块'
                    }, {
                        value: 'time-picker',
                        label: 'TimePicker 时间选择器'
                    }, {
                        value: 'date-picker',
                        label: 'DatePicker 日期选择器'
                    }, {
                        value: 'datetime-picker',
                        label: 'DateTimePicker 日期时间选择器'
                    }, {
                        value: 'upload',
                        label: 'Upload 上传'
                    }, {
                        value: 'rate',
                        label: 'Rate 评分'
                    }, {
                        value: 'form',
                        label: 'Form 表单'
                    }]
                }, {
                    value: 'data',
                    label: 'Data',
                    children: [{
                        value: 'table',
                        label: 'Table 表格'
                    }, {
                        value: 'tag',
                        label: 'Tag 标签'
                    }, {
                        value: 'progress',
                        label: 'Progress 进度条'
                    }, {
                        value: 'tree',
                        label: 'Tree 树形控件'
                    }, {
                        value: 'pagination',
                        label: 'Pagination 分页'
                    }, {
                        value: 'badge',
                        label: 'Badge 标记'
                    }]
                }, {
                    value: 'notice',
                    label: 'Notice',
                    children: [{
                        value: 'alert',
                        label: 'Alert 警告'
                    }, {
                        value: 'loading',
                        label: 'Loading 加载'
                    }, {
                        value: 'message',
                        label: 'Message 消息提示'
                    }, {
                        value: 'message-box',
                        label: 'MessageBox 弹框'
                    }, {
                        value: 'notification',
                        label: 'Notification 通知'
                    }]
                }, {
                    value: 'navigation',
                    label: 'Navigation',
                    children: [{
                        value: 'menu',
                        label: 'NavMenu 导航菜单'
                    }, {
                        value: 'tabs',
                        label: 'Tabs 标签页'
                    }, {
                        value: 'breadcrumb',
                        label: 'Breadcrumb 面包屑'
                    }, {
                        value: 'dropdown',
                        label: 'Dropdown 下拉菜单'
                    }, {
                        value: 'steps',
                        label: 'Steps 步骤条'
                    }]
                }, {
                    value: 'others',
                    label: 'Others',
                    children: [{
                        value: 'dialog',
                        label: 'Dialog 对话框'
                    }, {
                        value: 'tooltip',
                        label: 'Tooltip 文字提示'
                    }, {
                        value: 'popover',
                        label: 'Popover 弹出框'
                    }, {
                        value: 'card',
                        label: 'Card 卡片'
                    }, {
                        value: 'carousel',
                        label: 'Carousel 走马灯'
                    }, {
                        value: 'collapse',
                        label: 'Collapse 折叠面板'
                    }]
                }]
            }, {
                value: 'ziyuan',
                label: '资源',
                children: [{
                    value: 'axure',
                    label: 'Axure Components'
                }, {
                    value: 'sketch',
                    label: 'Sketch Templates'
                }, {
                    value: 'jiaohu',
                    label: '组件交互文档'
                }]
            }],
            //载入动画
            animate: "",
            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "变更事件", func: "onChange", code: "" }
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "tab",
        layerName: "选项卡",
        icon: "el-icon-datav-interactivetab",
        customId: "",
        width: 350,
        height: 200,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            fontColor: "#fff",
            fontSize: 16,
            fontFamily: "宋体",
            fontWeight: "normal",
            letterSpacing: 0,
            backgroundColor: "#0F1622",
            borderWidth: 2,
            borderColor: "#A0A9B8",
            selectedFontColor: "#fff",
            selectedFontSize: 16,
            selectedFontFamily: "宋体",
            selectedFontWeight: "normal",
            selectedLetterSpacing: 0,
            selectedBackgroundColor: "#163467",
            selectedBorderWidth: 2,
            selectedBorderColor: "#1890FF",
            direction: "horizontal",
            textAlign: "center",
            isRotation: false,
            dur: 3000,
            //载入动画
            animate: "",
            bindingObjs: [],
            //静态数据值
            staticDataValue: [{
                    bindid: "1",
                    content: "Tab A",
                },
                {
                    bindid: "2",
                    content: "Tab B",
                },
                {
                    bindid: "3",
                    content: "Tab C",
                },
                {
                    bindid: "4",
                    content: "Tab D",
                },
            ],
            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "变更事件", func: "onChange", code: "" }
            ],

            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            timeout: 0,
        },
    },
    {
        chartType: "textCheckBox",
        layerName: "文本复选框",
        icon: "el-icon-datav-textCheckBox",
        customId: "",
        width: 350,
        height: 210,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            //所绑定组件id
            selectedCharts: "",
            name: "textcheck_name",
            /* 内边距 */
            paddingLeftRight: 15, //左右内边距
            paddingTopBottom: 5, //上下内边距
            /* 外边距 */
            marginLeftRight: 10, //左右外边距
            marginTopBottom: 7, //上下外边距
            /* 默认样式 */
            fontSize: 16,
            fontWeight: "normal",
            fontFamily: "微软雅黑",
            letterSpacing: 0,
            fontColor: "#779FCD",
            //borderWidth: 1,
            //borderColor: "#0F355B",
            backgroundColor: "#0F355B",
            /* 选中样式 */
            selectedFontSize: 16,
            selectedFontFamily: "微软雅黑",
            selectedFontWeight: "normal",
            selectedLetterSpacing: 0,
            selectedFontColor: "#fff",
            //selectedBorderWidth: 1,
            //selectedBorderColor: "#0F355B",
            selectedBackgroundColor: "#3973C7",
            checkType: "single",
            dataSourceType: "static",
            //静态数据值
            staticDataValue: [
                { key: "all", value: "全国" },
                { key: "impArea", value: "重点区域" },
                { key: "river", value: "长江经济带" },
                { key: "unimpArea", value: "非重点区域" },
                { key: "ringArea", value: "环渤海城市" },
                { key: "goodDays", value: "优良天数" },
                { key: "PM2.5", value: "PM2.5" },
                { key: "l5", value: "劣V类" },
                { key: "COD", value: "COD" },
                { key: "NH", value: "NH" },
                { key: "NHx", value: "NHx" },
                { key: "goodWater", value: "水质优良" },
                { key: "SO2", value: "SO2" },
            ],

            customData: "",
            interactData: [
                { name: "组件初始化", func: "init", code: "" },
                { name: "变更事件", func: "onChange", code: "" }
            ],
            theme: "",

            timeout: 0,
            //绑定动画
            animate: "",
            bindingDiv: "",
            bindList: [],
        },
    },
    {
        chartType: "timeline",
        layerName: "时间轴",
        icon: "el-icon-datav-timeline",
        customId: "",
        width: 500,
        height: 100,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            symbolSize: 10,
            fontSize: 14,
            fontColor: "#fff",
            lineColor: "#fff",
            buttonColor: "#fff",
            checkFontColor: "#fff",
            borderColor: "#555",
            borderWidth: 2,
            checkButtonColor: "#fff",
            pointColor: "#c7254e",
            checkPointColor: "#c7254e",
            autoPlay: true,
            playInterval: 1500,
            showPlayBtn: true,
            showNextBtn: true,
            showPrevBtn: true,
            aggrName: "timeline_name",
            suffix: "月",
            bindList: [],
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
            ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            bi: null,
            timeout: 0,
        },
    },
    {
        chartType: "interact",
        layerName: "交互组件",
        icon: "el-icon-datav-interact",
        customId: "",
        width: 400,
        height: 150,
        x: 50,
        y: 100,
        zindex: 0,
        isShow: true,
        chartOption: {
            textStyle: {
                fontSize: 50,
                fontFamily: "宋体",
                fontColor: "#fff",
                fontWeight: "normal",
                letterSpacing: 25,
                lineHeight: 135,
                textAlign: "center",
                content: "点击区域",
            },
            background: {
                type: "color",
                backgroundColor: "",
                backgroundImg: "",
            },
            eventKey: 1,
            events: [{
                name: "事件1",
                key: 1,
                type: "", //点击或者数据变化
                chartType: "", //弹窗或者组件
                chart: null, //绑定组件id
                interactChartType: "", //绑定组件类型
                act: "", //显隐或者配置变化
                condition: "(data) => { \n" + " return true \n" + "}", //事件发生条件
                customConfig: "", //自定义组件配置
                chartHide: false, //默认隐藏
            }, ],
            //载入动画
            animate: "",
            customData: "",
            interactData: [],
            //静态数据值
            staticDataValue: [{
                text: "点 击 区 域",
            }, ],
            //绑定的div
            bindingDiv: "",
            theme: "",
            // 数据源类型：静态数据：static；接口数据：url;数据源：database
            dataSourceType: "static",

            bi: null,
            timeout: 0,
        },
    }
];
export const configurationComponents = [{
    chartType: "iotCustom",
    chartTypeGroup: 'configuration',
    layerName: "自定义组态",
    icon: "el-icon-datav-custom-iot",
    customId: "",
    width: 120,
    height: 120,
    x: 50,
    y: 100,
    zindex: 0,
    isShow: true,
    chartOption: {
        isAnimation: true,
        aggrName: "iot_custom_name",
        bindList: [],
        //载入动画
        animate: "",
        customData: "",
        interactData: [],
        //静态数据值
        staticDataValue: [{
                filedName: "排气压力",
                filed: "presoure",
                value: 0,
            },
            {
                filedName: "排气温度",
                filed: "outTemp",
                value: 25,
            },
            {
                filedName: "主机电压",
                filed: "mainVolte",
                value: 0,
            },
        ],
        //绑定的div
        bindingDiv: "",
        theme: "",
        // 数据源类型：静态数据：static；接口数据：url;数据源：database
        dataSourceType: "static",
        animationList: [{
            name: '', //动画名称
            imglist: [''], //动画序列图
        }], //动画列表
        conditionList: [{
            field: '', //字段
            symbol: '', //符号
            valtype: '',
            value: '', //数值
            // activeAnimate: '', //选择的动画
        }], //条件列表
        conditionGroup: [{
            condition: [],
            groups: [],
            animation: null
        }], //条件列表
        bi: null,
        // timeout: 0,
    },
}, {
    chartType: "polyline",
    chartTypeGroup: 'configuration',
    layerName: "连接线",
    icon: "el-icon-datav-line-iot",
    customId: "",
    width: 400,
    height: 100,
    x: 50,
    y: 100,
    zindex: 0,
    isShow: true,
    chartOption: {
        start: {
            cx: 50,
            cy: 50,
        },
        mid1: {
            cx: 150,
            cy: 50,
        },
        mid2: {
            cx: 250,
            cy: 50,
        },
        end: {
            cx: 350,
            cy: 50,
        },
        isAnimation: true, //是否是动画
        animateType: 'droplet', //动画类型，线有三种动画：电流，水珠，轨迹
        delayTime: 10, //延迟时间s
        lineColor: '#999',
        lineWidth: 20, //线的宽度
        flowColor: '#B37066',
        flowWidth: 10, //动画的宽度
        radius: 4,
        radiusFillColor: 'rgb(130, 179, 102)',
        isReverseAnimation: false, //是否反转动画
        dasharray: 39, //线缝隙间隔
        aggrName: "fengji_dong_name",
        bindList: [],
        //载入动画
        animate: "",
        customData: "",
        interactData: [],
        //静态数据值
        staticDataValue: ["1"],
        //绑定的div
        bindingDiv: "",
        theme: "",
        // 数据源类型：静态数据：static；接口数据：url;数据源：database
        dataSourceType: "static",
        svgUrl: '@/views/report/datav/image/zutai/fengji1.svg',
        bi: null,
        // timeout: 0,
    },
}, ]
export const otherComponents = [{
    chartType: "login",
    layerName: "登录",
    icon: "el-icon-datav-input",
    customId: "",
    width: 540,
    height: 686,
    x: 50,
    y: 100,
    zindex: 0,
    isShow: true,
    chartOption: {
        levelsetting: {
            bgColor: '#ffffff',
            bgRadius: 20
        },
        navsetting: {
            fontSize: 16,
            fontWeight: "normal",
            fontColor: '#999999',
            activeColor: '#333333', //活动的字体颜色
            borderColor: '#EAEAEA', //底边框颜色
            activeBorderColor: '#3572FF', //活动的底边框颜色
            lineHeight: 16, //行高
            fontPadding: 25, //字体上下内边距
        }, //头部设置
        label: {
            fontSize: 18,
            fontFamily: "宋体",
            fontColor: "#999999",
            fontWeight: "normal",
        },
        input: {
            fontSize: 16,
            fontFamily: "宋体",
            fontColor: "rgba(120, 130, 157, 1)",
            inputBorderColor: 'rgba(248, 248, 248, 1)',
            inputBgColor: 'rgba(248, 248, 248, 1)',
            focusBorderColor: 'rgba(53, 114, 255, 1)',
            inputRadius: 4,
            inputHeight: 48
        },
        buttonSetting: {
            btnBgStart: '#4c79ff',
            btnBgEnd: '#6da8ff',
            fontColor: '#ffffff',
            fontSize: 18,
            fontWeight: "normal",
            btnHeight: 52,
            btnRadius: 10,
            isDefalutUrl: true, //是否使用默认地址
            loginJumpUrl: '' //跳转的其他地址
        },
        width: 200,
        height: 40,
        //载入动画
        bindList: [],
        animate: "",
        customData: "",
        //请求参数 例如：?a=1&b=2
        requestParameters: [],
        //绑定的div
        bindingDiv: "",
        theme: "",
        // 数据源类型：静态数据：static；接口数据：url;数据源：database
        dataSourceType: "static",
        // interactData: [
        //   { name: "组件初始化", func: "init", code: "" },
        //   { name: "输入事件", func: "onInput", code: "" },
        //   { name: "变更事件", func: "onChange", code: "" },
        // ],
        timeout: 0,
    },
}, {
    chartType: "arrangeHorizontal",
    layerName: "横向排列框",
    icon: "el-icon-datav-flop",
    customId: "",
    width: 408,
    height: 220,
    x: 50,
    y: 100,
    zindex: 0,
    isShow: true,
    chartOption: {
        containerTitle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            paddingLeft: 14,
            paddingRight: 0,
            paddingTop: 14,
            paddingBottom: 8,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 1, //下边框
            borderColor: 'rgba(255,255,255, 0.2)',
            letterSpacing: 0, //字体间距
            cotBgColor: 'rgba(31, 87, 245, 0.2)',
            textAlign: 'left',
        },
        cotUlStyle: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 0.2)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlLiStyle: {
            width: 25, //单位百分比
            paddingLeft: 10,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 10,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 10,
            marginBottom: 6,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlLiLabelStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
        },
        cotUlLiValueStyle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "#CDFBFF", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 10,
            textAlign: 'center',
        },
        cotUlLiUnitStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
        },
        cotUlHasbgStyle: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            marginLeft: 12,
            marginRight: 12,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 0.2)',
            cotBgColor: 'rgba(31, 87, 245, 0.2)',
        },
        cotUlHasbgLiStyle: {
            width: 25,
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 12,
            paddingBottom: 12,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlHasbgLiLabelStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
        },
        cotUlHasbgLiValueStyle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(60, 199, 255, 1)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 10,
            textAlign: 'center',
        },
        cotUlHasbgLiUnitStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
        },
        cotUlTitleStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 1)", //字体颜色
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 14,
            paddingBottom: 14,
            marginLeft: 12,
            marginRight: 12,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 1, //下边框
            borderColor: 'rgba(255,255,255, 0.2)',
            letterSpacing: 0, //字体间距
            textAlign: 'left',
        },
        cotUlTitleNumStyle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(60, 199, 255, 1)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
        },
        cotStyle: {
            bgType: 'img',
            containerBgImage: '', //背景图片
            containerColor: 'rgba(27,43,138, 1)',
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)'
        },
        //静态数据值
        staticDataValue: [{
            title: "1号电表(后处理)",
            currentList: [{
                    Name: "功率",
                    Code: "sumkwh",
                    Value: 0,
                    Unit: "kw",
                    OptionType: "float",
                    UpdatedOn: "2025-10-28 16:06:57",
                    Description: "",
                },
                {
                    Name: "A相电流",
                    Code: "Fa",
                    Value: 0,
                    Unit: "A",
                    OptionType: "float",
                    UpdatedOn: "2025-10-28 16:06:57",
                    Description: "",
                },
                {
                    Name: "B相电流",
                    Code: "Fb",
                    Value: 0,
                    Unit: "A",
                    OptionType: "float",
                    UpdatedOn: "2025-10-28 16:06:57",
                    Description: "",
                },
                {
                    Name: "C相电流",
                    Code: "Fc",
                    Value: 0,
                    Unit: "A",
                    OptionType: "float",
                    UpdatedOn: "2025-10-28 16:06:57",
                    Description: "",
                },
            ],
            rowListObj: {
                title: [{
                    Name: "当前总电能",
                    Code: "Totalkwh",
                    Value: 259.2,
                    Unit: "kwh",
                    OptionType: "float",
                    UpdatedOn: "2025-10-28 16:00:58",
                    Description: "",
                }],
                list: [{
                        Value: 0,
                        Unit: "Nm³",
                        Name: "今日用电量",
                        Code: "todayuseenerge",
                    },
                    {
                        Value: 0,
                        Unit: "Nm³",
                        Name: "昨日用电量",
                        Code: "yesterdayepi",
                    },
                    {
                        Value: 209.6,
                        Unit: "Nm³",
                        Name: "本月用电量",
                        Code: "currentenerge",
                    },
                    {
                        Value: 20.8,
                        Unit: "kwh",
                        Name: "上月用电量",
                        Code: "preenerge",
                    },
                ],
            },
        }],
        tableSelectLine: [],
        finalResult: [],
        //载入动画
        animate: "",
        customData: "",
        interactData: [],

        //绑定的div
        bindingDiv: "",
        theme: "",
        // 数据源类型：静态数据：static；接口数据：url;数据源：database
        dataSourceType: "static",

        timeout: 0,
    },
}, {
    chartType: "arrangeVertical",
    layerName: "纵向排列框",
    icon: "el-icon-datav-flop",
    customId: "",
    width: 246,
    height: 280,
    x: 50,
    y: 100,
    zindex: 0,
    isShow: true,
    chartOption: {
        containerTitle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            paddingLeft: 14,
            paddingRight: 0,
            paddingTop: 14,
            paddingBottom: 8,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 1, //下边框
            borderColor: 'rgba(255,255,255, 0.2)',
            letterSpacing: 0, //字体间距
            cotBgColor: 'rgba(31, 87, 245, 0.2)',
            textAlign: 'left',
        },
        titleBunberStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
        },
        cotUlStyle: {
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 30,
            marginBottom: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 0.2)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlLiStyle: {
            width: 100, //单位百分比
            paddingLeft: 10,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 10,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlLiLabelStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
            width: 30,
        },
        cotUlLiValueStyle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(60, 199, 255, 1)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 10,
            textAlign: 'center',
            width: 30,
        },
        cotUlLiUnitStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            textAlign: 'center',
            width: 30,
        },
        cotStyle: {
            bgType: 'img',
            containerBgImage: '', //背景图片
            containerColor: 'rgba(27,43,138, 1)',
            paddingLeft: 0,
            paddingRight: 8,
            paddingTop: 0,
            paddingBottom: 0,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)'
        },
        cotUlRowsStyle: {
            width: 100,
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            marginLeft: 12,
            marginRight: 12,
            marginTop: 0,
            marginBottom: 0,
            borderRadius: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 0)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlRowsLiStyle: {
            width: 25,
            paddingLeft: 0,
            paddingRight: 0,
            paddingTop: 0,
            paddingBottom: 0,
            borderRadius: 0,
            marginLeft: 0,
            marginRight: 0,
            marginTop: 0,
            marginBottom: 0,
            borderWidth: 0,
            borderColor: 'rgba(255,255,255, 1)',
            cotBgColor: 'rgba(31, 87, 245, 0)',
        },
        cotUlRowsLiLineStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
            isShow: true,
        },
        cotUlRowsLiValueStyle: {
            fontSize: 14,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(60, 199, 255, 1)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
        },
        cotUlRowsLiUnitStyle: {
            fontSize: 12,
            fontFamily: "微软雅黑",
            fontWeight: "normal",
            fontColor: "rgba(255, 255, 255, 0.8)", //字体颜色
            letterSpacing: 0, //字体间距
            marginLeft: 0,
            marginRight: 0,
        },
        //静态数据值
        staticDataValue: [{
            title: '智慧电表',
            xuhao: '(2#气站，2号表)',
            list: [
                { cnName: '当前总电能', value: 2013130, unit: 'kwh' },
                { cnName: '功率', value: 163.20, unit: 'kw' },
                { cnName: '上月累计电能', value: 109436, unit: 'kwh' },
                { cnName: '本月累计电能', value: 79666, unit: 'kwh' },
                { cnName: '昨日累计电能', value: 4410, unit: 'kwh' },
            ],
            currentList: [{ cnName: 'A相电流', value: 272.40, unit: 'A' }, { cnName: 'B相电流', value: 270, unit: 'A' }, { cnName: 'C相电流', value: 246, unit: 'A' }]
        }],
        //载入动画
        animate: "",
        customData: "",
        interactData: [],

        //绑定的div
        bindingDiv: "",
        theme: "",
        // 数据源类型：静态数据：static；接口数据：url;数据源：database
        dataSourceType: "static",

        timeout: 0,
    },
}, ]
export const statisticsTypeOptions = [{
        value: "count",
        label: "计数",
    },
    {
        value: "sum",
        label: "求和",
    },
];

export const filterTypeOptions = [{
        value: "standard",
        label: "标准",
    },
    {
        value: "range",
        label: "范围",
    },
];

export const conditionOptions = [{
        value: "=",
        label: "等于",
    },
    {
        value: "!=",
        label: "不等于",
    },
    {
        value: ">",
        label: "大于",
    },
    {
        value: ">=",
        label: "大于等于",
    },
    {
        value: "<",
        label: "小于",
    },
    {
        value: "<=",
        label: "小于等于",
    },
    {
        value: "= ''",
        label: "是空的",
    },
    {
        value: "!= ''",
        label: "不是空的",
    },
    {
        value: "in",
        label: "包含",
    },
    {
        value: "not in",
        label: "不包含",
    },
    {
        value: "is null",
        label: "是null",
    },
    {
        value: "is not null",
        label: "不是null",
    },
];

export const echartsCharts = [
    "bar",
    "stackedBar",
    "staggeredLabel",
    "doublebar",
    "topbar",
    "parallebar",
    "doubleyAxis",
    "doubleDirectionBar",
    "doublebartime",
    "singleColumn",
    "threeDBar",
    "cylinder",
    "CustomThreeDBar",
    "rateBar",
    "pictorialBar",
    "line",
    "shadowLine",
    "rain",
    "pie",
    "ringPie",
    "funnel",
    "gauge",
    "keygauge",
    "progauge",
    "scatter",
    "heatMap",
    "radar",
    "waterball",
    "waterballline",
    "customWaterBall",
    "circle",
    "piemore",
    "progress",
    "datamap",
    "pieline",
    "barPie",
    "linebar",
    "mapbar",
    "map",
    "mapMore",
    "threedMap",
    "threedMapBar",
    "mapline",
    "knowledge",
    "pops",
    "sunburst",
    "sankey",
    "boxline",
    "customChart",
    "wordcloud",
    "timeline",
];

export const fontFamilys = [
    "宋体",
    "黑体",
    "微软雅黑",
    "Digital",
    "auto",
    "Chunkfive",
    "unidreamLED",
    "Arial",
    "Helvetica",
    "sans-serif",
];

export const fontWeights = ["normal", "bold", "bolder", "lighter"];

export const fontWeightsNum = [100, 200, 300, 400, 500, 600, 700, 800, 900]