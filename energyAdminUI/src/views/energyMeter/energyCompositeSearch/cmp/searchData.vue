<template>
    <div class="search-data">
        <div class="search-data-heard">
            <div class="search-data-table">
                <div class="search-data-table-heard">
                    <div class="heard-item" v-for="(item, index) in tableHeard" :key="index">
                        {{ item.title }}
                        <el-tooltip v-if="item.tip" placement="right-start">
                            <div slot="content" class="unitSelect_content">{{ item.tipText }}</div>
                            <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color:rgba(255, 255, 255, 0.6);font-size:12px;"></i>
                        </el-tooltip>
                    </div>
                </div>
                <div class="search-data-table-content">
                    <div class="content-item" :style="{ width: itemWidth + '%' }" v-for="(item, index) in mergedResult" :key="index">
                        <div>{{ item.FactorName }}</div>
                        <div>{{ unitConversion(item.UseVale) }}<span v-if="item.UseVale!=='-'">{{ item[unit] }}</span></div>
                        <div>{{ unitConversion(item.CostVale) }}<span v-if="item.CostVale!=='-'">{{ unit === 'Unit' ? '元' : '万元' }}</span></div>
                        <div>{{ unitConversion(item.ConvertCoal) }}<span v-if="item.ConvertCoal!=='-'">{{ unit === 'Unit'  ? 'kgce' : 'tce' }}</span></div>
                        <div>{{ unitConversion(item.CarbonEmission) }}<span v-if="item.CarbonEmission!=='-'">{{ unit === 'Unit' ? 'tCO₂e' : 'tCO₂e' }}</span></div>
                    </div>
                </div>
            </div>
            <div class="search-data-pie">
                 <div class="search-data-bar-content">
                    <div class="search-data-bar-heard">
                        <div class="title">
                            能源消耗占比 <span> （{{ getTargetUnit(dataTypePie) }}）</span>
                        </div>
                        <el-select popper-class="search-data-select" v-model="dataTypePie" placeholder="请选择">
                            <el-option v-for="(item, index) in filteredPie" :key="index" :label="item.title" :value="item.prop" />
                        </el-select>
                    </div>
                    <div class="search-data-bar-info">
                        <echartPie3d :pieData="mergedResult.slice(0, -1)" :unit="unit" :dataType="dataTypePie" :colorPool="colorPool"></echartPie3d>
                    </div>
                </div>
            </div>
        </div>
        <div class="search-data-total">
            <div class="search-data-total-item" v-for="(item, index) in energyConsume" :key="index">
                <img :src="require('@/assets/images/energyConsume.png')" alt="">
                <div class="content">
                    <div class="title">{{ item.title }}</div>
                    <div class="value">{{ item.value }} <span v-if="item.value !=='-'">{{ item.unit }}</span> </div>
                </div>
            </div>
        </div>
        <div class="search-data-bar">
            <div class="search-data-bar-content">
                <div class="search-data-bar-heard">
                    <div class="title">
                        能耗趋势图
                        <div class="colorLegend">
                            <div class="colorLegend-info" v-for="(item, index) in mergedResult.slice(0, -1)" :key="index">
                                <div :style="{ background: getBackground(index)}"></div>
                                {{ item.FactorName }} <span> （{{ getTargetUnit(dataType, item) }}）</span>
                            </div>
                        </div>
                    </div>
                    <el-select popper-class="search-data-select" v-model="dataType" placeholder="请选择">
                        <el-option v-for="(item, index) in filteredTableHeard" :key="index" :label="item.title" :value="item.prop" />
                    </el-select>
                </div>
                <div class="search-data-bar-info">
                    <echartsBar :barData="tableData" :unit="unit" :dataType="dataType" :colorPool="colorPool"></echartsBar>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import { mergeData } from './searchFunction.js';
import { selectFactorOrg } from '@/api/energy/gatherManage';
import echartsBar from './echartsBar.vue';
import echartPie3d from '../cmp/echartPie3d.vue';
export default {
    name: 'searchData',
    components: {
        echartsBar,
        echartPie3d
    },
    props: {
        unit: {
            type: String,
            default: 'Unit',
        },
        tableData: {
            type: Array,
            default: () => [],
        },
        dateType: {
            type: String,
            default: '月',
        },
    },
    computed: {
        itemWidth() {
            // 处理 tableData 为空的情况，避免除以 0
            return this.mergedResult.length > 0 ? 100 / this.mergedResult.length  : 100;
        },
        filteredTableHeard() {
            return Array.isArray(this.tableHeard) ? this.tableHeard.slice(1) : [];
        },
        filteredPie() {
            return Array.isArray(this.tableHeard) ? this.tableHeard.slice(2) : [];
        }
    },
    watch: {
        tableData: {
            handler(newVal, oldVal) {
                this.getInitList();
            },
            deep: true
        }
    },
    data() {
        return {
            factorList: [],
            mergedResult: [],
            dataType: 'UseVale',
            dataTypePie: 'CostVale',
            colorPool: ['rgb(42, 211, 154)', 'rgb(53, 200, 255)', 'rgb(255, 193, 7)', 'rgb(255, 87, 34)', 'rgb(156, 39, 176)'],
            tableHeard: [
                {
                    title: '能源种类',
                    tip: false,
                    prop: 'FactorName',
                },
                {
                    title: '能源消耗',
                    tip: false,
                    prop: 'UseVale',
                },
                {
                    title: '能源成本',
                    tip: false,
                    prop: 'CostVale',
                },
                {
                    title: '标准煤',
                    tip: true,
                    tipText: '标准煤量=能源消耗量*该能源低位发热量/折标煤系数',
                    prop: 'ConvertCoal',
                },
                {
                    title: '碳排放',
                    tip: true,
                    tipText: '碳排放=电量消耗量×当地电网因子值',
                    prop: 'CarbonEmission',
                },
            ],
            // 单位配置表：key=数据类型，value=单位映射（Unit/LageUnit）
            unitConfig: {
                UseVale: (item) => this.unit === 'Unit' ? item.Unit : item.LageUnit, // 动态取item的Unit/LageUnit
                CostVale: { Unit: '元', LageUnit: '万元' },
                ConvertCoal: { Unit: 'kgce', LageUnit: 'tce' },
                CarbonEmission: { Unit: 'tCO₂e', LageUnit: 'MtCO₂e' }
            },
            energyConsume: []
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        getInitList() {
             const loading = this.$loading({
                lock: true,
                text: '数据加载中...',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            selectFactorOrg({ OrgId: this.$store.getters.orgId }).then(async res  => {
                this.factorList = res.data;
                this.mergedResult = await mergeData(this.tableData, this.factorList);
                this.getEnergyConsume();
                loading.close();
            }) 
        },

        // 获取能源消耗数据
        getEnergyConsume() {
             // 1. 容错处理：确保 mergedResult 是数组且有数据，避免报错
            const lastItem = Array.isArray(this.mergedResult) && this.mergedResult.length > 0 
                ? this.mergedResult[this.mergedResult.length - 1] 
                : {}; // 空数组时返回空对象，避免后续取值报错

            // 2. 提取碳排数据并处理非数字/'-'情况
            const carbonEmission = lastItem.CarbonEmission;
            // 处理 '-' 或非数字，统一转为 0（便于后续计算，显示时再还原为 '-'）
            const carbonNum = carbonEmission === '-' || isNaN(Number(carbonEmission)) 
                ? 0 
                : Number(carbonEmission);

            // 3. 根据 dateType 计算不同维度的平均值（处理除零和精度问题）
            const calculateAvg = (type) => {
                let divisor = 1; // 除数默认1（日维度无需除法）
                switch (type) {
                    case '日':
                        divisor = 24; // 日 → 平均小时：除以24小时
                        break;
                    case '月':
                        divisor = 30 * 24; // 月 → 平均小时：按30天×24小时计算
                        break;
                    case '年':
                        divisor = 30 * 24 * 12; // 年 → 平均小时：按30天×24小时×12月计算
                        break;
                    default:
                        divisor = 1;
                }
                // 避免除以0，同时保留2位小数（四舍五入，解决浮点数精度问题）
                return divisor > 0 ? Math.round((carbonNum / divisor) * 100) / 100 : 0;
            };

            // 4. 组装最终数据（修复单位错误，处理 '-' 显示）
            this.energyConsume = [
                {
                    title: '综合能源碳排',
                    // 若原始数据是 '-', 显示 '-'；否则显示转换后的值（支持 LageUnit 单位）
                    value: carbonEmission === '-' ? '-' : this.unitConversion(carbonEmission),
                    // 碳排单位：调用通用单位方法，避免硬编码（对应 CarbonEmission 类型）
                    unit: this.getTargetUnit('CarbonEmission')
                },
                {
                    title: '综合能源碳排平均小时累计',
                    // 原始数据是 '-', 显示 '-'；否则显示计算后的平均值
                    value: carbonEmission === '-' ? '-' : calculateAvg(this.dateType),
                    // 碳排单位：非货币，修复原代码的 '元' 错误
                    unit: this.getTargetUnit('CarbonEmission')
                },
                {
                    title: '综合能源碳排日预测累计', // 修正重复标题，改为“日累计”（与计算逻辑匹配）
                    value: carbonEmission === '-' ? '-' : (() => {
                        let avgDay = 0;
                        if (this.dateType === '日') {
                            avgDay = carbonNum; // 日维度：单日值即日均
                        } else if (this.dateType === '月') {
                            avgDay = Math.round((carbonNum / 30) * 100) / 100; // 月 → 日均：除以30天
                        } else if (this.dateType === '年') {
                            avgDay = Math.round((carbonNum / (30 * 12)) * 100) / 100; // 年 → 日均：除以360天
                        }
                        return avgDay;
                    })(),
                    unit: this.getTargetUnit('CarbonEmission')
                }
            ];
        },
        // 单位换算
        unitConversion(value) {
            if (value === '-' || value == null) return '-';
            const numValue = Number(value);
            return isNaN(numValue) ? '-' : (this.unit === 'LageUnit' ? Math.round((numValue / 10000) * 100) / 100 : numValue);
        },

        // 获取单位
        getTargetUnit(targetType, item = {}) {
            const config = this.unitConfig[targetType];
            if (!config) return '';
            return typeof config === 'function' ? config(item) : config[this.unit] || '';
        },

        // 获取背景颜色
        getBackground(index) {
            return this.colorPool[index % this.colorPool.length];
        }

    }
}
</script>
<style lang="less" scoped>
.search-data{
    width: 100%;
    margin-top: 12px;
    .search-data-heard{
        width: 100%;
        display: flex;
        justify-content: space-between;
        border-left: 1px solid rgba(255, 255, 255, 0.1);
        height: 200px;
        .search-data-table{
            width: 53.5%;
            height: 100%;
            display: flex;
            .search-data-table-heard{
                width: 16%;
                height: 40px;
                min-width: 70px;
                .heard-item{
                    width: 100%;
                    height: 100%;
                    line-height: 40px;
                    background: #222E40;
                    text-align: center;
                    font-weight: 400;
                    font-size: 14px;
                    color: #FFFFFF; 
                    border-right: 1px solid rgba(255, 255, 255, 0.1);
                    &:nth-child(2n){
                        background: #19212D;
                    }
                }
            }
            .search-data-table-content{
                width: 84%;
                height: 100%;
                display: flex;
                .content-item{
                    height: 40px;
                    line-height: 40px;
                    text-align: center;
                    font-weight: 400;
                    font-size: 14px;
                    color: #FFFFFF; 
                    >div{
                        background: #222E40;
                        width: 100%;
                        border-right: 1px solid rgba(255, 255, 255, 0.1);
                        >span{
                            margin-left: 4px;
                        }
                        &:nth-child(2n){
                           background: #19212D;
                        }
                    }
                }
            }
        }
        .search-data-pie{
            background: #222E40;
            width: 46%;
            height: 100%;
            border-radius: 4px;
        }
    }
    .search-data-total{
        width: 100%;
        margin-top: 10px;
        height: 80px;
        background-color: #222E40;
        border-radius: 4px;
        display: flex;
        .search-data-total-item{
            padding-left: 24px;
            width: 33.33%;
            height: 100%;
            display: flex;
            align-items: center;
            position: relative;
            &:not(:last-child)::after{
                content: '';
                position: absolute;
                right: 0;
                top: 50%;
                transform: translateY(-50%);
                width: 1px;
                height: 24px;
                background: rgba(255, 255, 255, 0.1);
            }
            >img{
                width: 40px;
                height: 40px;
                margin-right: 20px;
            }
            .content{
                height: 100%;
                display: flex;
                flex-direction: column;
                justify-content: center;
                 .title{
                    font-weight: 400;
                    font-size: 14px;
                    color: #FFFFFF;
                    margin-bottom: 4px;
                }
                .value{
                    font-weight: 600;
                    font-size: 24px;
                    color: #3DB98F;
                    >span{
                        font-weight: 400;
                        font-size: 14px;
                        color: rgba(255, 255, 255, 0.6);
                    }
                }
            } 
        }
    }
    .search-data-bar{
        width: 100%;
        margin-top: 10px;
        height: 340px;
        background-color: #222E40;
        border-radius: 4px;
    }
    .search-data-bar-content{
        padding: 8px 16px 16px;
        height: calc(100% - 16px);
        .search-data-bar-heard{
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: relative;
            z-index: 10;
            .title{
                font-weight: 500;
                font-size: 14px;
                color: #FFFFFF;
                position: relative;
                padding-left: 8px;
                display: flex;
                align-items: center;
                .colorLegend{
                    display: flex;
                    align-items: center;
                    font-weight: 500;
                    font-size: 12px;
                    color: #FFFFFF;
                    margin-left: 40px;
                    .colorLegend-info{
                        display: flex;
                        align-items: center;
                        margin-right: 40px;
                        >div{
                            width: 10px;
                            height: 4px;
                            margin-right: 4px;
                        }
                        >span{
                            color: rgba(255, 255, 255, 0.6);
                            font-weight: 500;
                            font-size: 12px;
                        }
                    }
                    
                }
                
                &::after{
                    content: '';
                    position: absolute;
                    left: 0;
                    top: 50%;
                    transform: translateY(-50%);
                    width: 4px;
                    height: 14px;
                    background: #3DB98F;
                    border-radius: 2px;
                }
            }
            ::v-deep .el-select{
                width: 90px;
                height: 30px;
                .el-input__inner{
                    border: 0;
                    height: 30px;
                    line-height: 30px;
                    font-weight: 400;
                    font-size: 12px;
                }
                .el-input__icon{
                    line-height: 30px !important;
                }
            }
        }
        .search-data-bar-info{
            width: 100%;
            height: calc(100% - 10px);
        }
    }
}
.el-select-dropdown.el-popper .el-select-dropdown__item.selected::after{
    content: '';
}

</style>
<style>
.search-data-select[x-placement^=bottom] {
    margin-top: -3px !important;
}

/* 修复tooltip箭头颜色（与背景匹配） */
.el-tooltip__popper.is-dark .popper__arrow::after {
  border-color: transparent  !important;
}
body .el-tooltip__popper.is-dark[x-placement^="right"] .popper__arrow::after{
     border-right-color: #000000 !important;
}
</style>