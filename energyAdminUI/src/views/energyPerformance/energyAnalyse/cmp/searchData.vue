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
                        <div v-if="item.FactorName === '合计'">{{ unitConversion(item.OutPut) }}<span v-if="item.OutPut!=='-'">{{ item.ProductUnit }}</span></div>
                        <div v-else>-</div>
                        <div>{{ unitConversion(item.UnitProductEfficien) }}<span v-if="item.UnitProductEfficien!=='-'">{{ `${item[unit]}/${item.ProductUnit}` }}</span></div>
                        <div>{{ unitConversion(item.UnitProductCostEfficien) }}<span v-if="item.UnitProductCostEfficien!=='-'">{{ unit === 'Unit' ? `元/${item.ProductUnit}` : `万元/${item.ProductUnit}` }}</span></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="search-data-bar">
            <div class="search-data-bar-content">
                <div class="search-data-bar-heard">
                    <div class="title">
                        能耗趋势图
                        <div class="colorLegend">
                            <div class="colorLegend-info" v-for="(item, index) in filterMergedResult" :key="index">
                                <div :style="{ background: getBackground(index)}"></div>
                                {{ item.FactorName }} <span> （{{ getTargetUnit(dataType, item) }}）</span>
                            </div>
                        </div>
                    </div>
                    <div class="search-data-bar-select">
                       <el-select popper-class="search-data-select" v-model="dataType" style="width: 140px;" placeholder="请选择">
                            <el-option v-for="(item, index) in filteredTableHeard" :key="index" :label="item.title" :value="item.prop" />
                        </el-select>
                        <el-select popper-class="search-data-select" style="width: 76px;" v-model="factorId" placeholder="请选择" @change="getTableBarData">
                            <el-option label="综合" value="" />
                            <el-option v-for="(item, index) in mergedResult.slice(0, -1)" :key="index" :label="item.FactorName" :value="item.FactorId" />
                        </el-select>
                    </div>
                </div>
                <div class="search-data-bar-info">
                    <echartsBar :barData="tableBarData" :unit="unit" :dataType="dataType" :colorPool="colorPool"></echartsBar>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import { mergeData } from '@/views/energyMeter/energyCompositeSearch/cmp/searchFunction';
import echartsBar from '@/views/energyMeter/energyCompositeSearch/cmp/echartsBar';
import { selectFactorOrg } from '@/api/energy/gatherManage';
export default {
    name: 'searchData',
     components: {
        echartsBar
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
            return Array.isArray(this.tableHeard) ? this.tableHeard.slice(-2) : [];
        },
        filterMergedResult() {
            if (this.factorId === '') {
                return this.mergedResult.slice(0, -1);
            } else {
                return this.mergedResult.filter(item => item.FactorId === this.factorId);
            }
           
        },
    },
    watch: {
        tableData: {
            handler: function(newVal, oldVal) {
                this.getInitList();
            },
            deep: true
        }
    },
    data() {
        return {
            factorList: [],
            factorId: '',
            mergedResult: [],
            tableBarData: [],
            dataType: 'UnitProductEfficien',
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
                {
                    title: '产量',
                    tip: false,
                    prop: 'OutPut',
                },
                {
                    title: '单位产品能耗能效',
                    tip: true,
                    tipText: '单位产品能效=综合能源消费量/产品产量',
                    prop: 'UnitProductEfficien',
                },
                {
                    title: '单位产品成本能效',
                    tip: true,
                    tipText: '单位产品成本能效=能源成本/产品产量',
                    prop: 'UnitProductCostEfficien',
                },
            ],
            // 单位配置表：key=数据类型，value=单位映射（Unit/LageUnit）
            unitConfig: {
                UseVale: (item) => this.unit === 'Unit' ? item.Unit : item.LageUnit, // 动态取item的Unit/LageUnit
                CostVale: { Unit: '元', LageUnit: '万元' },
                ConvertCoal: { Unit: 'kgce', LageUnit: 'tce' },
                CarbonEmission: { Unit: 'tCO₂e', LageUnit: 'MtCO₂e' },
                UnitProductEfficien: (item) => this.unit === 'Unit' ? `${item.Unit}/${item.ProductUnit}` : `${item.LageUnit}/${item.ProductUnit}`,
                UnitProductCostEfficien: (item) => this.unit === 'Unit' ? `元/${item.ProductUnit}` : `万元/${item.ProductUnit}`,
            },
            energyConsume: [],
            loading: null,
        };
    },
    mounted() {
        this.loading = this.$loading({
            lock: true,
            text: '数据加载中...',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
        });
    },
    methods: {
        // 获取数据
        async getInitList() {
            try {
                // 先获取 factorList（await 等待接口返回，替代 .then 回调）
                const res = await selectFactorOrg({ OrgId: this.$store.getters.orgId });
                this.factorList = res.data || []; // 兜底空数组，避免后续报错

                // 校验 tableData 是否有效（核心：避免传入空数据导致 mergeData 无操作）
                if (!Array.isArray(this.tableData) || this.tableData.length === 0) {
                    this.mergedResult = [];
                    loading.close();
                    return;
                }

                // 等待 mergeData 执行完成，再赋值（关键：await 生效）
                this.mergedResult = await mergeData(this.tableData, this.factorList);

                // 合并完成后，同步更新柱状图数据（可选，根据业务需求）
                await this.getTableBarData();

            } catch (err) {
                // 捕获所有异步错误（接口失败、mergeData 失败），避免流程中断
                this.mergedResult = []; // 错误时兜底空数组
            } finally {
                // 无论成功/失败，都关闭加载（避免加载态卡死）
                this.loading.close();
            }
        },
        
        // 获取柱状图用的数据
        async getTableBarData() {
            // 先检查tableData是否有效
            const data = JSON.parse(JSON.stringify(this.tableData));
            if (!Array.isArray(data)) {
                this.tableBarData = [];
                return;
            }
            // 过滤逻辑：如果factorId不为空则过滤，否则返回所有数据
            this.tableBarData = this.factorId 
                ? data.filter(item => item.FactorId === this.factorId)
                : data;
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
        height: 321px;
        border-left: 1px solid rgba(255, 255, 255, 0.1);
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        .search-data-table{
            width: 100%;
            height: 100%;
            display: flex;

            .search-data-table-heard{
                width: 13%;
                height: 40px;
                min-width: 142px;
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
                width: 87%;
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
    }
    .search-data-bar{
        width: 100%;
        margin-top: 10px;
        height: 310px;
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
                    bottom: 0;
                    width: 4px;
                    height: 100%;
                    background: #3DB98F;
                    border-radius: 2px;
                }
            }
            .search-data-bar-select{
                display: flex;
                align-items: center;
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