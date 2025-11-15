<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item label="日期类型" prop="dateType">
                        <el-select class="set_radius" style="width: 90px;" v-model="dateType" placeholder="请选择日期类型" @change="changeDateType">
                            <el-option label="月" value="月" />
                            <el-option label="年" value="年" />
                        </el-select>
                    </el-form-item>
                    <el-form-item style="margin-left: 4px;">
                        <el-date-picker class="form_input_style" v-model="timeType.date" style="width: 140px;" 
                        :value-format="timeType.format" :type="timeType.type" placeholder="选择日期" :clearable='false' />
                    </el-form-item>
                </div>
                <el-form-item class="button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">查询</el-button>
                </el-form-item>
            </el-form>
            <div class="checkAnalyse">
                <div class="checkAnalyse-overview">
                    <div class="heard-title">碳排概览</div>
                    <div class="overview-list">
                        <div class="overview-item" v-for="item in checkAnalyseOverview" :key="item.title">
                            <img src="@/assets/images/overview.png" alt="">
                            <div class="item-title">{{item.title}}</div>
                            <div class="item-content">{{item.value}}</div>
                            <div class="item-basic">
                                <div class="lastYear">上年：<span>{{item.lastYear}}</span></div>
                                <div class="yearOnYear">同比：
                                    <span :class="{
                                        'red': item.yearOnYear.startsWith('+') && item.yearOnYear !== '0.00%',
                                        'green': item.yearOnYear.startsWith('-')
                                        }"
                                    >
                                        {{item.yearOnYear}}
                                        <i v-if="item.yearOnYear.startsWith('+') && item.yearOnYear !== '0.00%'" class="zhongtaiiconfont zhongtai-icon-shangsheng" style="color: #F15C5C"></i>
                                        <i v-if="item.yearOnYear.startsWith('-')" class="zhongtaiiconfont zhongtai-icon-xiajiang" style="color: #3DB98F"></i>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="checkAnalyse-detail">
                    <div class="heard-title">
                        碳排详情
                        <div class="tab-list">
                            <div :class="['tab-item', {'active': tabActive === 1}]" @click="handleTabClick(1)">按排放类别分析</div>
                            <div :class="['tab-item', {'active': tabActive === 2}]" @click="handleTabClick(2)">按排放范围分析</div>
                        </div>
                    </div>
                    <div class="detail-list">
                        <div class="left">
                            <echart-pie :typeColor="typeColor" :currentSum="currentSum" />
                        </div>
                        <div calss="right">
                            <echart-line :tableData="tableData" />
                        </div>
                    </div>
                </div>
                <div class="checkAnalyse-constitute">
                    <constituteInfo :currentPeriodData="currentPeriodData" :typeColor="typeColor" :currentSum="currentSum" />
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { carbonReportByType, carbonReportByRange } from '@/api/energy/emissionCategory';
import { facilityEnergyPageDateList } from '@/api/energy/energyMeter';
import constituteInfo from './cmp/constituteInfo.vue'
import EchartPie from './cmp/echartPie.vue';
import EchartLine from './cmp/echartLine.vue';

export default {
    name: 'checkAnalyse',
    components: {
        constituteInfo,
        EchartPie,
        EchartLine
    },
    data() {
        return {
            tabActive: 1, // 1：按排放类别分析 2：按排放范围分析
            queryParams: {
                OrgId: this.$store.getters.orgId,
                dateType: '日',
                endDate: moment().endOf('month').format('YYYY-MM-DD'),
                beginDate: moment().startOf('month').format('YYYY-MM-DD'),
            },
            tableData: [], // 折线图数据
            dateType: '月',
            currentPeriodData: [],  // 当前选择期数据（第一次请求）
            lastYearPeriodData: {}, // 去年同期数据（第二次请求）
            currentSum: {}, // 当前期数据累加结果
            timeType: {
                date: moment().format('YYYY-MM'),
                type: 'month',
                format: 'yyyy-MM',
            },
            // 概览数据模板（初始空值，请求后填充）
            checkAnalyseOverview: [
                { title: '总碳排', value: 0, lastYear: 0, yearOnYear: '0.00%' },
                { title: '碳排放强度 (tCO₂e/￥10000)', value: 0, lastYear: 0, yearOnYear: '0.00%' },
                { title: '生产产值 (万元)', value: 0, lastYear: 0, yearOnYear: '0.00%' },
                { title: '能源总成本 (万元)', value: 0, lastYear: 0, yearOnYear: '0.00%' }
            ],
            // 临时存储两次请求的原始累加结果（便于后续映射）
            currentTotal: { carbon: 0, cost: 0, output: 0 },  // 当前期
            lastYearTotal: { carbon: 0, cost: 0, output: 0 },  // 去年同期
            typeColor: ['#2AD39A', '#35C8FF', '#4E5CFE', '#AE59FF', '#FF57BA', '#FFC425']
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        async getInitList() {
             // 初始化时按默认日期类型（月）计算时间
            this.calcPeriodTime('月', moment().format('YYYY-MM'));
            this.getLineList();
            await this.getList();
        },

        // 切换tab
        async handleTabClick(tab) {
            this.tabActive = tab;
            await this.getList();
        },
         // 核心：根据“日期类型”和“选择的日期”计算时间范围（当前期 + 去年同期）
        calcPeriodTime(dateType, selectedDate) {
            let currentBegin, currentEnd, lastYearBegin, lastYearEnd;

            switch (dateType) {
                // 2. 日期类型：月
                case '月':
                    // 当前选择月：月初至月末
                    currentBegin = moment(selectedDate).startOf('month').format('YYYY-MM-DD 00:00:00');
                    currentEnd = moment(selectedDate).endOf('month').format('YYYY-MM-DD 23:59:59');
                    // 去年同期月：减1年，同月的月初至月末
                    lastYearBegin = moment(selectedDate).subtract(1, 'year').startOf('month').format('YYYY-MM-DD 00:00:00');
                    lastYearEnd = moment(selectedDate).subtract(1, 'year').endOf('month').format('YYYY-MM-DD 23:59:59');
                    break;

                // 3. 日期类型：年
                case '年':
                    // 当前选择年：年初至年末
                    currentBegin = moment(selectedDate).startOf('year').format('YYYY-MM-DD 00:00:00');
                    currentEnd = moment(selectedDate).endOf('year').format('YYYY-MM-DD 23:59:59');
                    // 去年同期年：减1年，同年的年初至年末
                    lastYearBegin = moment(selectedDate).subtract(1, 'year').startOf('year').format('YYYY-MM-DD 00:00:00');
                    lastYearEnd = moment(selectedDate).subtract(1, 'year').endOf('year').format('YYYY-MM-DD 23:59:59');
                    break;

                default:
                    // 默认按“月”处理
                    currentBegin = moment().startOf('month').format('YYYY-MM-DD 00:00:00');
                    currentEnd = moment().endOf('month').format('YYYY-MM-DD 23:59:59');
                    lastYearBegin = moment().subtract(1, 'year').startOf('month').format('YYYY-MM-DD 00:00:00');
                    lastYearEnd = moment().subtract(1, 'year').endOf('month').format('YYYY-MM-DD 23:59:59');
            }

            // 返回计算后的时间范围（当前期 + 去年同期）
            return {
                current: { begin: currentBegin, end: currentEnd },
                lastYear: { begin: lastYearBegin, end: lastYearEnd }
            };
        },

        // 两次请求：先当前期，再去年同期
        async getList() {
            try {
                // 1. 获取计算后的时间范围（当前期 + 去年同期）
                const { current, lastYear } = this.calcPeriodTime(this.dateType, this.timeType.date);

                // 根据 tabActive 确定请求的接口
                let apiPromise;
                if (this.tabActive === 1) {
                    apiPromise = carbonReportByType; // 按排放类别分析
                } else if (this.tabActive === 2) {
                    apiPromise = carbonReportByRange; // 按排放范围分析
                } else {
                    // 默认按排放类别分析
                    apiPromise = carbonReportByType;
                }

                // 2. 第一次请求：当前选择期数据
                const currentParams = {
                    ...this.queryParams,
                    beginDate: current.begin,
                    endDate: current.end
                };
                const currentRes = await apiPromise(currentParams);
                this.currentPeriodData = currentRes.data || {}; // 存储当前期数据

                const currentSum = this.calcEmissionAndCost(currentRes.data || []);
                this.currentSum = currentSum;
              
                this.currentTotal = {
                    carbon: currentSum.totalCarbon, // 总碳排（carbonEmission 累加）
                    cost: currentSum.totalCost / 10000, // 能源总成本（costVale 累加，转为万元）
                    output: currentSum.totalOutput / 10000, // 生产产值（outputValue 累加，转为万元）
                };


                // console.log('当前选择期碳排数据:', this.currentPeriodData);

                // 3. 第二次请求：去年同期数据
                const lastYearParams = {
                    ...this.queryParams,
                    beginDate: lastYear.begin,
                    endDate: lastYear.end
                };
                const lastYearRes = await apiPromise(lastYearParams);
                this.lastYearPeriodData = lastYearRes.data || {}; // 存储去年同期数据

                const lastYearSum = this.calcEmissionAndCost(lastYearRes.data || []);
                this.lastYearTotal = {
                    carbon: lastYearSum.totalCarbon, // 去年同期总碳排
                    cost: lastYearSum.totalCost / 10000, // 去年同期能源总成本（转为万元）
                    output: lastYearSum.totalOutput / 10000, // 去年同期生产产值（转为万元）
                };

                // console.log('去年同期碳排数据:', this.lastYearPeriodData);

                this.updateOverviewData();

            } catch (error) {
                console.error('获取碳排数据失败:', error);
                this.$message.error('数据加载失败，请重试'); // 错误提示
            }
        },
        
        // 碳排放趋势分析数据
        getLineList() {
             facilityEnergyPageDateList(this.queryParams).then(res => {
                this.tableData = res.data.List;
            })
        },

        //  核心：累加数据
        calcEmissionAndCost(dataList) {
            let totalCarbon = 0; // 总碳排（所有类别累加）
            let totalCost = 0;   // 总成本（所有类别累加）
            let totalOutput = 0; // 生产产值（所有类别累加）
            
            // 按子类别分组累加碳排放量
            const carbonBySubClass = {};
            // 新增：按顶层类别分组累加碳排放量（键：顶层类别名称，值：该类别总碳排）
            const carbonByTopClass = {};

            // 遍历顶层类别（如“类别一：直接排放”“类别二：间接排放”）
            dataList.forEach(topClass => {
                // 初始化顶层类别的碳排累加器（使用顶层类别名称或ID作为键）
                const topClassName = topClass.ClassName; // 如"类别一：直接温室气体排放和移除"
                if (!carbonByTopClass[topClassName]) {
                    carbonByTopClass[topClassName] = 0;
                }

                // 遍历 OrgClasses 下的子类别（如“固定燃烧源”等）
                const subClassEntries = Object.entries(topClass.OrgClasses || {});
                
                subClassEntries.forEach(([subClassName, subClassItems]) => {
                    // 初始化子类别碳排累加器
                    if (!carbonBySubClass[subClassName]) {
                        carbonBySubClass[subClassName] = 0;
                    }

                    // 遍历设备项累加各字段
                    subClassItems.forEach(item => {
                        const carbon = Number(item.CarbonEmission) || 0;
                        const cost = Number(item.CostVale) || 0;
                        const output = Number(item.OutValue) || 0;

                        // 累加至总结果
                        totalCarbon += carbon;
                        totalCost += cost;
                        totalOutput += output;

                        // 累加至对应子类别
                        carbonBySubClass[subClassName] += carbon;
                        // 累加至当前顶层类别
                        carbonByTopClass[topClassName] += carbon;
                    });
                });
            });

            // 处理精度：保留2位小数
            Object.keys(carbonBySubClass).forEach(subClassName => {
                carbonBySubClass[subClassName] = Number(carbonBySubClass[subClassName].toFixed(2));
            });
            // 新增：处理顶层类别碳排精度
            Object.keys(carbonByTopClass).forEach(topClassName => {
                carbonByTopClass[topClassName] = Number(carbonByTopClass[topClassName].toFixed(2));
            });

            // 返回结果：总累加值 + 顶层类别碳排 + 子类别碳排
            return {
                totalCarbon: Number(totalCarbon.toFixed(2)),
                totalCost: Number(totalCost.toFixed(2)),
                totalOutput: Number(totalOutput.toFixed(2)),
                carbonBySubClass: carbonBySubClass,   // 子类别碳排分组
                carbonByTopClass: carbonByTopClass    // 新增：顶层类别碳排分组
            };
        },

        // 4. 更新概览数组（将累加结果和同比映射到 checkAnalyseOverview）
        updateOverviewData() {
            const current = this.currentTotal;
            const lastYear = this.lastYearTotal;

            // 1. 先创建更新后的完整数组（推荐：替换整个数组，响应式最可靠）
            const updatedOverview = [
                // -------------------------- ① 总碳排 --------------------------
                {
                    title: '总碳排',
                    value: (current.carbon).toFixed(2),
                    lastYear: (lastYear.carbon).toFixed(2),
                    yearOnYear: this.calcRate(current.carbon, lastYear.carbon)
                },
                // -------------------------- ② 碳排放强度 --------------------------
                {
                    title: '碳排放强度 (tCO₂e/￥10000)',
                    value: current.output > 0 ? (current.carbon / current.output).toFixed(2) : '0.00',
                    lastYear: lastYear.output > 0 ? (lastYear.carbon / lastYear.output).toFixed(2) : '0.00',
                    yearOnYear: this.calcRate(
                        Number(current.output > 0 ? (current.carbon / current.output).toFixed(2) : '0.00'),
                        Number(lastYear.output > 0 ? (lastYear.carbon / lastYear.output).toFixed(2) : '0.00')
                    )
                },
                // -------------------------- ③ 生产产值 --------------------------
                {
                    title: '生产产值 (万元)',
                    value: (current.output).toFixed(2),
                    lastYear: (lastYear.output).toFixed(2),
                    yearOnYear: this.calcRate(Number(current.output), Number(lastYear.output))
                },
                // -------------------------- ④ 能源总成本 --------------------------
                {
                    title: '能源总成本 (万元)',
                    value: (current.cost).toFixed(2),
                    lastYear: (lastYear.cost).toFixed(2),
                    yearOnYear: this.calcRate(Number(current.cost), Number(lastYear.cost))
                }
            ];

            // 2. 直接替换整个 checkAnalyseOverview 数组（Vue 能检测到数组替换，触发视图更新）
            this.checkAnalyseOverview = updatedOverview;
        },

        // 5. 同比计算工具（通用）
        calcRate(current, lastYear) {
            if (lastYear === 0 || isNaN(lastYear)) {
                return current > 0 ? '100.00%' : '0.00%';
            }
            const rate = ((current - lastYear) / lastYear * 100).toFixed(2);
            // 正数加“+”，负数直接显示（如+2.50%、-1.20%）
            return `${rate > 0 ? '+' : ''}${rate}%`;
        },

        // 切换日期类型：更新日期选择器配置
        changeDateType(e) {
            this.dateType = e; // 同步下拉框值
            switch (e) {
                case '月':
                    this.queryParams.dateType = '日';
                    this.timeType = {
                        date: moment().format('YYYY-MM'),
                        type: 'month',
                        format: 'yyyy-MM'
                    };
                    break;
                case '年':
                    this.queryParams.dateType = '月';
                    this.timeType = {
                        date: moment().format('YYYY'),
                        type: 'year',
                        format: 'yyyy'
                    };
                    break;
            }
        },
         /** 重置按钮操作 */
        resetQuery() {
            this.dateType = '月';
            this.queryParams.dateType = '日';
            this.timeType = {
                date: moment().format('YYYY-MM'),
                type: 'month',
                format: 'yyyy-MM',
            };
            this.resetForm("queryForm");
            this.handleQuery();
        },

        /** 搜索按钮操作 */
        async handleQuery() {
            this.queryParams.pageNum = 1;
            this.getLineList();
            await this.getList();
        },
    }
}
</script>
<style lang="less" scoped>
.checkAnalyse{
    width: 100%;
}
.checkAnalyse-overview{
    width: 100%;
}
.heard-title{
    width: 100%;
    margin-top: 30px;
    font-weight: 500;
    font-size: 16px;
    color: #FFFFFF;
    position: relative;
    padding-left: 24px;
    display: flex;
    align-items: center;
    &::before{
        content: '';
        position: absolute;
        left: 0;
        top: 50%;
        transform: translateY(-50%);
        background: url('~@/assets/images/zs.png') no-repeat;
        background-size: 100% 100%;
        width: 16px;
        height: 16px;
    }
    .tab-list{
        margin-left: 18px;
        width: 214px;
        height: 32px;
        background: #131922;
        border-radius: 2px;
        display: flex;
        align-items: center;
        line-height: 32px;
        text-align: center;
        .tab-item{
            width: calc(50% - 6px);
            height: 88%;
            text-align: center;
            font-weight: 400;
            font-size: 12px;
            color: rgba(255, 255, 255, 0.6);
            display: flex;
            align-items: center;
            justify-content: center;
            margin-left: 3px;
            margin-right: 3px;
            cursor: pointer;
            &.active{
                background: #222E40;
                color: #fff;
            }
        }
    }
}
.overview-list{
    margin-top: 16px;
    width: 100%;
    display: flex;
    justify-content: space-between;
    .overview-item{
        width: calc(25% - 6px);
        height: 132px;
        background: #222E40;
        border-radius: 4px;
        padding: 16px 20px;
        position: relative;
        z-index: 1;
        > img {
            width: 45%;
            height: 100%;
            position: absolute;
            top: 0;
            right: 0;
            z-index: -1;
        }
        .item-title{
            font-weight: 400;
            font-size: 14px;
            color: rgba(255, 255, 255, 0.6);
        }
        .item-content{
            font-weight: 600;
            font-size: 32px;
            color: #FFFFFF;
            margin-top: 16px;
        }
        .item-basic{
            font-weight: 400;
            font-size: 14px;
            color: rgba(255, 255, 255, 0.6);
            margin-top: 10px;
            display: flex;
            align-items: center;
            // width: 80%;
            > .lastYear{
                margin-right: 24px;
                >span{
                    font-weight: 500;
                    font-size: 14px;
                    color: #FFFFFF;
                }
            }
            > .yearOnYear{
                >.red{
                   color: #F15C5C; 
                }
                >.green{
                    color: #3DB98F;
                }
            }
        }
    }
}
.checkAnalyse-detail{
    margin-top: 19px;
    .detail-list{
        margin-top: 16px;
        width: 100%;
        height: 300px;
        display: flex;
        justify-content: space-between;
        >div{
            width: calc(50% - 6px);
            height: 100%;
            background: #222E40;
            border-radius: 4px;
        }
    }
}
.checkAnalyse-constitute{
    width: 100%;
    margin-top: 12px;
    border-radius: 4px;
    border: 1px solid rgba(255, 255, 255, 0.2);
}
</style>