import VueEvent from '../../VueEvent'
import { initResultStart } from "./dataTreating";
export default {
    props: {
        isDraw: {
            type: Boolean,
            default: false
        },
        chartOption: {
            type: Object
        },
        theme: {
            type: Object
        },
        globalData: {
            type: Array
        },
        alLoadData: {
            type: Array,
            default: () => {
                return []
            }
        }
    },
    data() {
        return {
            chart: null,
            dataOption: this.chartOption,
            lastData: "",
            valUpdate: function(result) {},
        }
    },
    watch: {
        chartOption: {
            deep: true,
            immediate: true,
            handler(newVal) {
                if (this.isDraw) {
                    this.dataOption = newVal;
                    this.$nextTick(() => {
                        if (newVal.dataSourceType == "gobal") {
                            if (newVal.configProcessorTabs && newVal.configProcessorTabs.length > 0) {
                                let arr = newVal.configProcessorTabs.map(row => row.globalData)
                                this.refreshData(arr);
                            } else {
                                this.refreshData(newVal.globalData);
                            }

                        } else {
                            this.initSourceCode(newVal);
                        }
                    })

                }
            }
        },
        "chartOption.interactData": {
            handler() {
                if (this.isDraw) {
                    this.$nextTick(() => {
                        this.initInteract();
                    })
                }
            }
        },
    },
    mounted() {
        if (!this.isDraw) {
            this.$nextTick(() => { //组件数据初始化
                if (this.chartOption.dataSourceType == "gobal") {
                    if (this.chartOption.configProcessorTabs && this.chartOption.configProcessorTabs.length > 0) {
                        let arr = this.chartOption.configProcessorTabs.map(row => row.globalData)

                        arr.map(row => {
                            if (this.alLoadData.includes(row)) {
                                this.refreshData(row, true);
                            } else {
                                this.refreshData(row);
                                this.alLoadData.push(row)
                                this.$emit('setAlLoadData', this.alLoadData)
                            }

                        })

                    } else {
                        if (this.alLoadData.includes(this.chartOption.globalData)) {
                            this.refreshData(this.chartOption.globalData, true);
                        } else {
                            this.refreshData(this.chartOption.globalData);
                            this.alLoadData.push(this.chartOption.globalData);
                            this.$emit('setAlLoadData', this.alLoadData);
                        }

                    }
                    this.$emit('startload')
                } else {
                    this.initSourceCode(this.chartOption);
                }

            })
        }
    },
    beforeCreate() {
        //监听全局数据变化
        VueEvent.$on("GlobalData", (val, option, isNotArrload) => {
                this.$nextTick(() => {
                    if (this.chartOption.dataSourceType === "gobal") {
                        if (option && Array.isArray(option)) { //这里用于处理选择了多个数据源的组件
                            if (this.chartOption.configProcessorTabs && this.chartOption.configProcessorTabs.length > 0) {
                                let tabsArr = this.chartOption.configProcessorTabs.map(row => { if (row.globalData) { return row.globalData } }).filter(row => row)
                                let optionsNameArr = option.map(row => row.name)
                                const result = optionsNameArr.every(name => tabsArr.some(globalData => globalData === name));
                                if (result) {
                                    this.lastData = option;
                                    this.initSourceCode(option);
                                }


                            } else {
                                option.map(rw => {
                                    if (rw.name == this.chartOption.globalData) {
                                        this.lastData = rw;
                                        this.initSourceCode(rw);
                                    }
                                })

                            }
                        } else {
                            if (this.chartOption.configProcessorTabs && this.chartOption.configProcessorTabs.length > 0) {
                                let arr = this.chartOption.configProcessorTabs.map(row => row.globalData)
                                if (isNotArrload) {
                                    this.lastData = option;
                                    this.initSourceCode(option);
                                } else {
                                    if (arr.includes(option.name)) {
                                        this.refreshData(arr, false);
                                    }
                                }

                            } else {
                                if (option.name == this.chartOption.globalData) {
                                    this.lastData = option;
                                    this.initSourceCode(option);
                                }
                            }

                        }

                    }

                })
            })
            //监听全局主题变化
        VueEvent.$on("themeChange", (val) => {
            this.$nextTick(() => {
                if (this.chart != null) {
                    this.chart.dispose();
                    this.chart = null;
                }
                this.initSourceCode(this.lastData);
            })
        })

        this.$nextTick(async() => {
            if (this.chartOption.dataSourceType == "static") {
                await this.initSourceCode("");
            }
            this.initInteract();
            if (this.dataOption.interactData == null) {
                return;
            }
            this.dataOption.interactData.forEach(element => {
                if (element.func == "init") {
                    // 处理事件逻辑
                    if (element.code != null && element.code != "") {
                        try {
                            let callFunction = eval("()=>{return " + element.code + ";}");
                            ((callFunction)()).call(this);
                        } catch (err) {
                            // this.$message('minins的beforeCreate初始化'+err);
                        }
                    }

                }
            });

        })

    },
    beforeDestroy() {
        if (!this.chart) {
            return;
        }
        this.chart.dispose();
        this.chart = null;
    },
    methods: {
        refreshData(name, isNotLoad) {
            VueEvent.$emit("refreshGlobal", name, isNotLoad);
        },
        initInteract() {
            let dataOption = this.dataOption;
            //交互组件配置
            if (dataOption.interactData != undefined && dataOption.interactData != "") {
                dataOption.interactData.forEach(element => {
                    this.$off(element.func);
                    this.$on(element.func, (eventData) => {
                        // 处理事件逻辑
                        if (element.code != null && element.code != "") {
                            try {
                                let callFunction = eval("()=>{return " + element.code + ";}");
                                ((callFunction)()).call(this, eventData);
                            } catch (err) {
                                // this.$message('minins的initInteract初始化'+err);
                            }
                        }

                    });
                });
            }
        },
        async initSourceCode(res) {
            let dataOption = this.dataOption;
            let initResult = []
                //数据处理
                // this.$message({
                //     message: "这是数据处理方法",
                //     type: "warning",
                //   });
            if (dataOption.dataSourceType == "static") {
                initResult = dataOption.staticDataValue;
            } else {
                if (dataOption.modelValue !== undefined && dataOption.modelValue !== null) {
                    // this.$message({
                    //     message: "这是开始进入图表处理",
                    //     type: "warning",
                    //   });
                    initResult = await initResultStart(dataOption, res)
                        // console.log(initResult, '==========')
                } else {
                    let disposeData = res.rawData !== undefined && res.rawData ? JSON.parse(res.rawData) : [];
                    disposeData.forEach((item, index) => {
                        if (item.title === dataOption.globalProcessor) {
                            initResult = item.content
                            return
                        }
                    })
                }
            }
            try {
                this.valUpdate(initResult, res);
            } catch (err) {
                console.log('err报错', err);
                // this.$message('minins初始化'+err);
            }
        },
    }
}