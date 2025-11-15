<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div v-show="!isView">
            <div class="from_con" id="from_con">
                <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                    <div class="biaodan_input_con">
                        <el-form-item>
                            <el-select class="set_radius" style="width: 94px" v-model="queryParams.productType" placeholder="请选择">
                                <el-option label="产品名称" :value="1" />
                                <el-option label="产品型号" :value="2" />
                            </el-select>
                        </el-form-item>
                        <el-form-item v-if="queryParams.productType == 1" style="margin-left: 4px;" prop="productName">
                            <el-input style="width: 140px;" v-model="queryParams.productName" placeholder="请输入" clearable />
                        </el-form-item>
                        <el-form-item v-else style="margin-left: 4px;" prop="productModel">
                            <el-input style="width: 140px;" v-model="queryParams.productModel" placeholder="请输入" clearable />
                        </el-form-item>
                        <el-form-item prop="productBorder" label="生命周期边界">
                            <el-select class="set_radius" v-model="queryParams.productBorder" placeholder="请选择">
                                <el-option v-for="(item, key) in tabList" :key="key" :label="item" :value="key"  />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="创建日期">
                            <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
                            range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                        </el-form-item>
                    </div>
                    <el-form-item class="button_con">
                        <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="handleQuery">查询</el-button>
                    </el-form-item>
                </el-form>
            </div>
            <div class="elbiaoge_elform" style="padding: 16px 6px;">
                <el-row :gutter="10" style="padding-left: 10px;" class="mb8 button_row">
                    <div style="display: flex;align-items: center;">
                        <el-col :span="1.5">
                            <el-button type="success" plain @click="handleAdd">
                                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                <span style="margin-left:6px">新增产品碳足迹</span>
                            </el-button>
                        </el-col>
                    </div>
                </el-row>
                <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                    <el-table-column label="产品名称" align="center" prop="ProductName" />
                    <el-table-column label="产品型号" align="center" prop="ProductModel" />
                    <el-table-column label="产品类型" align="center">
                        <template slot-scope="scope">
                            {{ scope.row.ProductType == 1 ? '成品' : '半成品' }}
                        </template>
                    </el-table-column>
                    <el-table-column label="产量/单位" align="center">
                        <template slot-scope="scope">
                            {{ scope.row.OutPut }}/{{ scope.row.Unit }}
                        </template>
                    </el-table-column>
                    <el-table-column label="生命周期边界" align="center" prop="BorderTitle" />
                    <el-table-column label="数据统计时段" align="center" width="250">
                        <template slot-scope="scope">
                            <el-tooltip class="item" effect="dark" :content="formatDate(scope.row.BeginDate) + ' - ' + formatDate(scope.row.EndDate)" placement="bottom">
                            <span> {{ formatDate(scope.row.BeginDate) }} - {{ formatDate(scope.row.EndDate) }} </span>
                            </el-tooltip>
                        </template>
                    </el-table-column>
                    <el-table-column label="创建人" align="center" prop="createName" />
                    <el-table-column label="创建时间" align="center">
                        <template slot-scope="scope">
                            <el-tooltip class="item" effect="dark" :content="scope.row.create_time" placement="bottom">
                            <div> {{ scope.row.create_time }} </div>
                            </el-tooltip>
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" align="center" width="200" class-name="small-padding fixed-width">
                        <template slot-scope="scope">
                            <el-button class="primary" type="text" @click="handleView(scope.row)">详情</el-button>
                            <div class="line"></div>
                            <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                        </template>
                    </el-table-column>
                </el-table>
                <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
            </div>
            <carbonFootprintAdd ref="carbonFootprintAdd" @getList="getList" />
        </div>
        <div v-show="isView" class="detail_con">
            <cfDetail ref="cfDetail" @closeDialog="closeDialog" />
        </div>
    </div>
</template>
<script>
import moment from 'moment';
import { modelProductModelPage, removeProductModel } from '@/api/energy/cyclicalModel';
import carbonFootprintAdd from './cmp/carbonFootprintAdd.vue'
import cfDetail from './cmp/cfDetail.vue'

export default {
    name: 'carbonFootprint',
    components: {
        carbonFootprintAdd,
        cfDetail
    },
    data() {
        return {
            createDateRange: [],
            isView: false,
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                productType: 1,
                productBorder: '',
                productName: '',
                productModel: '',
                beginTime: '',
                endTime: ''
            },
            tabList: {
                '1': '从摇篮到大门',
                '2': '从大门到大门',
                '3': '从摇篮到坟墓',
            },
            tableData: [],
            total: 0,
            loading: false,
        };
    },
    mounted() {
        this.getList();
    },
    methods: {
        // 时间格式化
        formatDate(date) {
            return moment(date).format('YYYY-MM-DD');
        },
        // 获取数据
        getList() {
            if(this.createDateRange.length > 0) {
                this.queryParams.beginTime = this.createDateRange[0];
                this.queryParams.endTime = this.createDateRange[1];
            }
            modelProductModelPage(this.queryParams).then(res => {
                this.tableData = res.data.List;
                this.total = res.data.Total;
            })
        },

        // 新增
        handleAdd() {
            this.$refs.carbonFootprintAdd.openDialog();
        },

        // 详情
        handleView(row) {
            this.isView = true;
            this.$refs.cfDetail.openDialog(row.Id);
        },

        // 删除
        handleDelete(row) {
            this.$confirm('确定删除该碳足迹吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeProductModel({ Id: row.Id }).then(res => {
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

        // 关闭详情
        closeDialog(){
            this.isView = !this.isView;
        },

        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.createDateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },
    }
}
</script>
<style lang="less" scoped>
.detail_con {
   width: 100%;
   height: 100%;
}
</style>
