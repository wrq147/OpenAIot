<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <el-tabs v-model="queryParams.factorId" @tab-click="getList">
            <el-tab-pane v-for="item in factorList" :key="item.Id" :label="item.TypeName" :name="item.Id" />
        </el-tabs>
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item label="设备名称" prop="equipmentId">
                        <el-select class="set_radius" v-model="queryParams.equipmentId" placeholder="请选择设备名称" clearable>
                            <el-option v-for="item in devList" :key="item.Id" :label="item.EquipmentName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="统计日期">
                        <el-date-picker class="form_input_style" v-model="readDateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
                        range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="创建时间">
                        <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 228px" value-format="yyyy-MM-dd HH:mm:ss" type="datetimerange"
                        range-separator="-" start-placeholder="开始时间" end-placeholder="结束时间"></el-date-picker>
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
                <div>
                    <el-col :span="1.5">
                        <el-button type="success" plain @click="handleAdd">
                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                            <span style="margin-left:6px">新增</span>
                        </el-button>
                    </el-col>
                    <el-col :span="1.5">
                        <el-button type="info" plain @click="handleImport">
                            <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                            <span style="margin-left:6px">导入</span>
                        </el-button>
                    </el-col>
                    <el-col :span="1.5">
                        <el-button type="info" plain @click="handleGetHistoryData">
                            <i class="el-icon-s-promotion"></i>
                            <span style="margin-left:6px">获取历史采集数据</span>
                        </el-button>
                    </el-col>
                    
                    <!-- <el-col :span="1.5">
                        <el-button type="info" plain :loading="exportLoading" @click="handleExport">
                            <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                            <span style="margin-left:6px">导出</span>
                        </el-button>
                    </el-col> -->
                </div>
            </el-row>
            <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                <el-table-column label="设备名称" align="center" prop="EquipmentName" />
                <el-table-column label="因子名称" align="center" prop="FactorName" />
                <el-table-column label="统计日期" align="center">
                    <template slot-scope="scope">
                        {{ formatDate(scope.row.DDate) }}
                    </template>
                </el-table-column>
                <el-table-column label="统计时间" align="center" prop="Memo" />
                <el-table-column label="期初表码值" align="center" prop="InitVale" />
                <el-table-column label="期末表码值" align="center" prop="EndVale" />
                <el-table-column label="能源消耗量" align="center" prop="UseVale" />
                <el-table-column label="单位" align="center" prop="Unit" />
                <el-table-column label="成本（元）" align="center" prop="CostVale" />
                <el-table-column label="操作人" align="center" prop="updateName" />
                <el-table-column label="创建时间" align="center" width="150">
                    <template slot-scope="scope">
                        <el-tooltip class="item" effect="dark" :content="scope.row.createTime" placement="bottom">
                           <div> {{ scope.row.createTime }} </div>
                        </el-tooltip>
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
                    <template slot-scope="scope">
                        <el-button class="primary" type="text" @click="handleView(scope.row)">详情</el-button>
                        <div class="line"></div>
                        <el-button class="primary" type="text" @click="handleUpdate(scope.row)">修改</el-button>
                        <div class="line"></div>
                        <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
        </div>
        <energyAdd ref="energyAdd" :title="title" :equipmentList="devList" @getList="handleQuery" />
        <historyData ref="historyData" :factorList="factorList" @getList="handleQuery" />
        <uploadFild ref="uploadFild" @getList="handleQuery" @importTemplate="importTemplate" />
    </div>
</template>
<script>
import moment from 'moment';
import { selectEnergyPageList, removeEnergy, selectFactorOrg, importEnergyTemplate } from '@/api/energy/gatherManage';
import { equipmentPageList } from '@/api/energy/equip';
import uploadFild from '@/components/uploadFild/index.vue';
import energyAdd from './cmp/energyAdd.vue';
import historyData from './cmp/historyData.vue';
export default {
    name: 'energyGather',
    components: {
        energyAdd,
        historyData,
        uploadFild
    },
    data() {
        return {
            title: '新增',
            activeName: 'electricity',
            loading: false,
            exportLoading: false,
            single: true,
            ids: [],
            devList: [],
            factorList: [],
            readDateRange: [],
            createDateRange: [],
            tableData: [],
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                equipmentId: '',
                factorId: '',
                beginDate: '',
                endDate: '',
                beginTime: '',
                endTime: ''
            },
            total: 0,

        };
    },
    mounted() {
        const rowData = this.$route.query;
        this.getInitList(rowData);
    },
    methods: {
        // 时间格式化
        formatDate(date) {
            return moment(date).format('YYYY-MM-DD');
        },
        // 获取数据
        getInitList(rowData) {
            equipmentPageList({
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 9999,
            }).then(res => {
                this.devList = res.data.List;
            })
            selectFactorOrg({ OrgId: this.$store.getters.orgId }).then(res => {
                this.factorList = res.data;
                if(Object.keys(rowData).length === 0) {
                    this.queryParams.factorId = this.factorList.length > 0 ? this.factorList[0].Id : '';
                } else {
                    this.queryParams.factorId = rowData.FactorId;
                    this.readDateRange = [rowData.DDate, rowData.DDate];
                } 
                this.getList();
            }) 
        },
        getList() {
            this.loading = true;
            if(this.readDateRange.length > 0) {
                this.queryParams.beginDate = this.readDateRange[0];
                this.queryParams.endDate = this.readDateRange[1];
            }
            if(this.createDateRange.length > 0) {
                this.queryParams.beginTime = this.createDateRange[0];
                this.queryParams.endTime = this.createDateRange[1];
            }

            selectEnergyPageList(this.queryParams).then(res => {
                this.loading = false;
                this.tableData = res.data.List;
                this.total = res.data.Total;
            })
        },

         /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.readDateRange = [];
            this.importDateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },
        // 新增
        handleAdd() {
            this.title = '新增';
            this.$refs.energyAdd.openDialog();
        },
        // 修改
        handleUpdate(row) {
            this.title = '修改';
            this.$refs.energyAdd.openDialog(row);
        },

        // 详情
        handleView(row){
            this.$refs.energyAdd.openDialog(row, true)
        },
        // 获取历史采集数据
        handleGetHistoryData(row) {
            this.$refs.historyData.openDialog()
        },
        // 导入
        handleImport() {
            this.$refs.uploadFild.openDialog('EfficiencyService/Production/ImportEnergy');
        },
        // 导入能源数据模版
        importTemplate() {
            importEnergyTemplate();
        },
        // 导出
        handleExport() {},
        // 删除
        handleDelete(row) {
            this.$confirm('确定删除该采集数据吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeEnergy({ Id: row.Id }).then(res => {
                    this.$message({
                        message: '删除成功',
                        type: 'success'
                    });
                    this.getList();
                })
            }).catch(() => {
                this.$message({
                    type: 'info',
                    message: '已取消删除'
                });
            });
        },
    }
}
</script>
<style lang="less" scoped>
::v-deep{
    .el-tabs{
        padding: 0 20px;
    }
    .el-tabs__header{
        margin: 0;
        .el-tabs__nav{
            position: relative;
            padding-left: 8px;
        }
        .el-tabs__nav::before{
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
        .el-tabs__item{
            height: 56px;
            padding: 0 16px !important;
            font-weight: 400;
            font-size: 16px;
            line-height: 56px;
            color: rgba(255, 255, 255, 0.6) !important;
        }
        .el-tabs__item.is-active{
            font-weight: 500;
            color: #FFFFFF !important;
        }
    }
    .el-tabs .el-tabs__header .el-tabs__nav .el-tabs__active-bar{
        height: 0;
    }
    .el-tabs .el-tabs__header .el-tabs__nav-wrap::after{
        background-color: rgba(255, 255, 255, 0.1);
        height: 1px;
    }
}
</style>