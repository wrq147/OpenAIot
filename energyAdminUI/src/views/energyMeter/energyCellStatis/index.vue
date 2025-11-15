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
                            <el-form-item label="能源类型" prop="factorId">
                                <el-select class="set_radius" v-model="queryParams.factorId" placeholder="请选择能源类型" clearable>
                                    <el-option v-for="item in factorList" :key="item.Id" :label="item.TypeName" :value="item.Id" />
                                </el-select>
                            </el-form-item>
                            <el-form-item label="日期类型" prop="dateType">
                                <el-select class="set_radius" v-model="queryParams.dateType" placeholder="请选择日期类型" clearable>
                                    <el-option label="日" value="日" />
                                    <el-option label="月" value="月" />
                                    <el-option label="年" value="年" />
                                </el-select>
                            </el-form-item>
                            <el-form-item label="选择时间">
                                <el-date-picker class="form_input_style" v-model="dateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
                                range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                            </el-form-item>
                            <el-form-item label="数据来源" prop="dataSource">
                                <el-select class="set_radius" v-model="queryParams.dataSource" placeholder="请选择数据来源" clearable>
                                    <el-option label="人工采集" :value="1" />
                                    <el-option label="设备采集" :value="2" />
                                </el-select>
                            </el-form-item>
                            <el-form-item label="能源消耗量" prop="operator">
                                <el-select class="set_radius" v-model="queryParams.operator" style="width: 120px;" placeholder="请选择" clearable>
                                    <el-option label=">" value=">" />
                                    <el-option label="<" value="<" />
                                    <el-option label="=" value="=" />
                                    <el-option label=">=" value=">=" />
                                    <el-option label="<=" value="<=" />
                                </el-select>
                            </el-form-item>
                            <el-form-item prop="useVale" style="margin-left: 4px;">
                                <el-input v-model="queryParams.useVale" style="width: 90px;" placeholder="请输入" clearable />
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
                                <el-button type="info" plain :loading="exportLoading" @click="handleExport">
                                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                                    <span style="margin-left:6px">导出</span>
                                </el-button>
                            </el-col>
                            <el-col :span="1.5">
                                <div class="description">说明： 建议数据采集维度比数据统计更精细，数据统计结果将越精确</div>
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
                            <el-select style="width: 110px;" v-model="queryParams.unitSelect" @click="handleQuery">
                                <el-option label="一级单位" value="Unit" />
                                <el-option label="二级单位" value="LageUnit" />
                            </el-select>
                        </div>
                    </el-row>
                    <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                        <el-table-column label="单元名称" align="center" prop="FacilityName" />
                        <el-table-column label="能源类型" align="center" prop="FactorName" />
                        <el-table-column label="统计时间" align="center" prop="DDate" />
                        <el-table-column label="能源消耗量" align="center" prop="UseVale">
                            <template slot-scope="scope">
                                <div v-if="queryParams.unitSelect == 'Unit'">{{ scope.row.UseVale }}</div>
                                <div v-else>{{ (scope.row.UseVale/10000).toFixed(2) }}</div>
                            </template>
                        </el-table-column>
                        <el-table-column label="单位" align="center" :prop="queryParams.unitSelect" />
                        <el-table-column label="成本（元）" align="center" prop="CostVale" />
                         <el-table-column label="数据来源" align="center" prop="Memo">
                            <template slot-scope="scope">
                                <div v-if="scope.row.Memo == 1">人工采集</div>
                                <div v-else>设备采集</div>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" align="center" width="100" class-name="small-padding fixed-width">
                            <template slot-scope="scope">
                                <el-button class="primary" type="text" @click="handleView(scope.row)">明细</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
                </div>
             </div>
        </div>
    </div>
</template>
<script>
import factorType from './cmp/factorType.vue';
import { facilityEnergyPageDateList, exportFacilityList } from '@/api/energy/energyMeter';
import { selectFactorOrg } from '@/api/energy/gatherManage';
export default {
    name: 'energyCellStatis',
    components: {
        factorType
    },
    data() {
        return {
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                unitSelect: 'Unit',
                dateType: '日',
                operator: '',
                endDate: '',
                beginDate: '',
                factorId: '',
                useValue: '',
                facilityId: '',
                dataSource: ''
            },
            unitList: [
                { name: '电', unit: 'KW(千瓦)', lastUnit: 'MW(万瓦)' },
                { name: '天然气', unit: 'Nm³(标准m³)', lastUnit: '万Nm³' },
                { name: '金额', unit: '元', lastUnit: '万元' },
                { name: '标准煤', unit: 'kgce', lastUnit: 'tce' },
                { name: '碳排放', unit: 'kgCO₂e', lastUnit: 'tCO₂e' },
            ],
            dateRange: [],
            factorList: [],
            tableData: [],
            total: 0,
            loading: false,
            exportLoading: false,
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 获取数据
        getInitList() {
            selectFactorOrg({ OrgId: this.$store.getters.orgId }).then(res => {
                this.factorList = res.data;
            }) 
        },

        getList(currentKey) {
            this.loading = true;
            console.log(this.queryParams.facilityId)
            this.queryParams.facilityId = 
            (currentKey !== undefined && currentKey !== null && currentKey !== '' && typeof currentKey === 'string') 
                ? currentKey 
                : this.queryParams.facilityId;
            if (this.dateRange.length > 0) {
                this.queryParams.beginDate = this.dateRange[0];
                this.queryParams.endDate = this.dateRange[1];
            }
            facilityEnergyPageDateList(this.queryParams).then(res => {
                this.loading = false;
                this.tableData = res.data.List;
                this.total = res.data.Total;
            })
        },
        
        // 跳转到能源采集页面
        handleView(row) {
            this.$router.push({
                path: '/gatherManage/energyGather',
                query: row
            })
        },

        // 导出
        handleExport() {
            let query = { ...this.queryParams };

            delete query.pageNum;
            delete query.pageSize;
            
            exportFacilityList({ 
                ...query 
            });
        },

         /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.dateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
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
                font-weight: 400;
                font-size: 14px;
                color: rgba(255, 255, 255, 0.6);
                margin-left: 20px;
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
