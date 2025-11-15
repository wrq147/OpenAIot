<template>
    <div class="energyStatis" style="padding:10px" id="big_con">
        <div class="energyStatis-left">
            <factorType @getList="getList" />
        </div>
        <div class="energyStatis-right">
            <div class="energyStatis-table">
                <div class="from_con" id="from_con">
                    <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                        <div class="biaodan_input_con">
                            <el-form-item label="日期类型" prop="dateType">
                                <el-select class="set_radius" style="width: 90px;" v-model="dateType" placeholder="请选择日期类型" @change="changeDateType">
                                    <el-option label="日" value="日" />
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
                </div>
                <div class="elbiaoge_elform">
                    <el-row :gutter="10" class="mb8 button_row">
                        <div style="display: flex;align-items: center;">
                            <el-col :span="1.5">
                                <div class="description">
                                    统计数据
                                    <span>({{ queryParams.beginDate }} ~ {{ queryParams.endDate }})</span>
                                </div>
                            </el-col>
                        </div>
                        <div class="unitSelect">
                            <el-tooltip placement="left-start">
                                <div slot="content" class="unitSelect_content">
                                    <div class="tab">
                                        <div>数据名称</div>
                                        <div>一级单位</div>
                                        <div>二级单位</div>
                                    </div>
                                    <div class="tab_content" v-for="item in unitList" :key="item.name">
                                        <div>{{item.name}}</div>
                                        <div>{{item.unit}}</div>
                                        <div>{{item.lastUnit}}</div>
                                    </div>
                                </div>
                                <i class="zhongtaiiconfont zhongtai-icon-bangzhu" style="color:rgba(255, 255, 255, 0.6);font-size:12px;"></i>
                            </el-tooltip>
                            <span>单位换算</span>
                            <el-select style="width: 110px;" v-model="queryParams.unitSelect">
                                <el-option label="一级单位" value="Unit" />
                                <el-option label="二级单位" value="LageUnit" />
                            </el-select>
                        </div>
                    </el-row>
                    <div class="energyCompositeSearch-info">
                        <searchData :unit="queryParams.unitSelect" :tableData="tableData" :dateType="dateType" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { facilityEnergyPageDateList } from '@/api/energy/energyMeter';
import factorType from '../energyCellStatis/cmp/factorType.vue';
import searchData from './cmp/searchData.vue';
export default {
    name: 'energyCompositeSearch',
    components: {
        factorType,
        searchData
    },
    data() {
        return {
            unitList: [
                { name: '电', unit: 'KW(千瓦)', lastUnit: 'MW(万瓦)' },
                { name: '天然气', unit: 'Nm³(标准m³)', lastUnit: '万Nm³' },
                { name: '金额', unit: '元', lastUnit: '万元' },
                { name: '标准煤', unit: 'kgce', lastUnit: 'tce' },
                { name: '碳排放', unit: 'kgCO₂e', lastUnit: 'tCO₂e' },
            ],
            queryParams: {
                OrgId: this.$store.getters.orgId,
                unitSelect: 'Unit',
                dateType: '日',
                endDate: moment().endOf('month').format('YYYY-MM-DD'),
                beginDate: moment().startOf('month').format('YYYY-MM-DD'),
                facilityId: '',
            },
            dateType: '月',
            timeType: {
                date: moment().format('YYYY-MM'),
                type: 'month',
                format: 'yyyy-MM',
            },
            tableData: [],
            factorList: [],
        };
    },
    methods: {
        getList(currentKey) {
            this.queryParams.facilityId = currentKey ? currentKey : this.queryParams.facilityId;
            facilityEnergyPageDateList(this.queryParams).then(res => {
                this.tableData = res.data.List;
            })
        },

        changeDateType(e) {
            if (e === '日') {
                this.timeType.type = 'date';
                this.timeType.format = 'yyyy-MM-dd';
                this.timeType.date = moment().format('YYYY-MM-DD'); 
            } else if (e === '月') {
                this.timeType.type = 'month';
                this.timeType.format = 'yyyy-MM';
                this.timeType.date = moment().format('YYYY-MM'); 
            } else if (e === '年') {
                this.timeType.type = 'year';
                this.timeType.format = 'yyyy';
                this.timeType.date = moment().format('YYYY'); 
            }
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.timeType = {
                date: moment().format('YYYY-MM'),
                type: 'month',
                format: 'yyyy-MM',
            };
            this.resetForm("queryForm");
            this.handleQuery();
        },

        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            if (this.dateType === '日') {
                this.queryParams.dateType = '日';
                this.queryParams.beginDate = moment(this.timeType.date).format('YYYY-MM-DD 00:00:00');
                this.queryParams.endDate = moment(this.timeType.date).format('YYYY-MM-DD 23:59:59');
            } else if (this.dateType === '月') {
                this.queryParams.dateType = '日';
                this.queryParams.beginDate = moment(this.timeType.date).startOf('month').format('YYYY-MM-DD');
                this.queryParams.endDate = moment(this.timeType.date).endOf('month').format('YYYY-MM-DD');
            } else if (this.dateType === '年') {
                this.queryParams.dateType = '月';
                this.queryParams.beginDate = moment(this.timeType.date).startOf('year').format('YYYY-MM-DD');
                this.queryParams.endDate = moment(this.timeType.date).endOf('year').format('YYYY-MM-DD');
            }
            this.getList();
        },

    }
}
</script>
<style lang="less" scoped>
.energyStatis {
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: space-between;
    .energyStatis-left {
        width: 240px;
        background: #19212D;
        border-radius: 4px;
    }
    .energyStatis-right {
        width: calc(100% - 250px);
        margin-left: 10px;
        background: #19212D;
        border-radius: 4px;
        .energyStatis-table{
            padding: 0 4px;
            .description{
                font-weight: 500;
                font-size: 16px;
                color: #FFFFFF;
                position: relative;
                padding-left: 24px;
                ::before{
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
                >span{
                    font-weight: 400;
                    font-size: 14px;
                    color: rgba(255, 255, 255, 0.6);
                    margin-left: 8px;
                }
            }
            .unitSelect{
                display: flex;
                align-items: center;
                font-weight: 400;
                font-size: 12px;
                color: #FFFFFF;
                i{
                    font-size: 12px;
                    color: rgba(255, 255, 255, 0.6);
                    margin-right: 4px;
                    cursor: pointer;
                }
                >span{
                    margin-right: 10px;
                }
            }
            .energyCompositeSearch-info{
                width: 100%;
            }
        }
    }
}
.unitSelect_content{
    width: 300px;
    height: 168px;
    .tab{
        display: flex;
        height: 28px;
        border-bottom: 1px solid rgba(255, 255, 255, 0.2);
        >div{
            flex: 1;
            text-align: center;
            height: 100%;
            line-height: 28px;
            font-weight: 500;
            font-size: 12px;
            color: rgba(255, 255, 255, 0.6);
        }
    }
    .tab_content{
        height: 28px;
        display: flex;
        >div{
            flex: 1;
            text-align: center;
            height: 100%;
            line-height: 28px;
            font-weight: 500;
            font-size: 12px;
            color: #FFFFFF;
        }

    }

}
</style>
<style lang="less">
body .el-tooltip__popper.is-dark {
    background: #000000;
    box-shadow: 0px 10px 20px 0px rgba(0,0,0,0.4);
    border-radius: 4px;
}
/* 修复tooltip箭头颜色（与背景匹配） */
.el-tooltip__popper.is-dark .popper__arrow::after {
  border-color: transparent  !important;
}
body .el-tooltip__popper.is-dark[x-placement^="left"] .popper__arrow::after {
  border-left-color: #000000 !important;
}
</style>