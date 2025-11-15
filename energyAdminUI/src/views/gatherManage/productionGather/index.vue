<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item label="设施名称" prop="facilityId">
                        <treeselect v-model="queryParams.facilityId" :options="facilityList" :show-count="true" :normalizer="normalizer" placeholder="请选择设施名称"
                        clearable />
                    </el-form-item>
                    <el-form-item label="产品类型" prop="productType">
                       <el-select class="set_radius" v-model="queryParams.productType" placeholder="产品类型" clearable>
                            <el-option label="成品" :value="1" />
                            <el-option label="半成品" :value="2" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="产品名称" prop="productId">
                        <el-select class="set_radius" v-model="queryParams.productId" filterable placeholder="请选择产品名称" clearable>
                            <el-option v-for="item in productList" :key="item.Id" :label="item.ProductName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="创建时间">
                        <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 228px" value-format="yyyy-MM-dd" type="daterange"
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
                    <!-- 
                    <el-col :span="1.5">
                        <el-button type="info" plain :loading="exportLoading" @click="handleExport">
                            <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                            <span style="margin-left:6px">导出</span>
                        </el-button>
                    </el-col>
                    <el-col :span="1.5">
                        <el-button type="info" plain :disabled="single" @click="handleDelete">
                            <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                            <span style="margin-left:6px">删除</span>
                        </el-button>
                    </el-col> -->
                </div>
            </el-row>
            <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                <el-table-column label="设施名称" align="center" prop="FacilityName" />
                <el-table-column label="产品类型" align="center" prop="ProductType" >
                  <template slot-scope="scope">
                    <span>{{ scope.row.ProductType==1?'成品':'半成品'}}</span>
                  </template>
                </el-table-column>
                <el-table-column label="产品名称" align="center" prop="ProductName" />
                <el-table-column label="产品型号" align="center" prop="ProductModel" />
                <el-table-column label="统计日期" align="center">
                    <template slot-scope="scope">
                        {{ formatDate(scope.row.DDate) }}
                    </template>
                </el-table-column>
                <el-table-column label="产量" align="center" prop="OutPut" />
                <el-table-column label="单位" align="center" prop="Unit" />
                <el-table-column label="单价" align="center" prop="Price" />
                <el-table-column label="产值（元）" align="center" prop="OutValue" />
                <el-table-column label="操作人" align="center" prop="updateName" />
                <el-table-column label="创建时间" align="center">
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
        <productionAdd ref="productionAdd" :title="title" :facilityList="facilityList" :productList="productList" @getList="handleQuery" />
        <uploadFild ref="uploadFild" @getList="handleQuery" @importTemplate="importTemplate" />
    </div>
</template>
<script>
import moment from 'moment'
import { productPageList } from '@/api/energy/product';
import { facilityTree } from '@/api/energy/facility';
import { selectProductionPageList, removeProduction, importProductionTemplate } from '@/api/energy/gatherManage'
import productionAdd from './cmp/productionAdd.vue'
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import uploadFild from '@/components/uploadFild/index.vue'
export default {
    name: 'energyGather',
    components: {
        Treeselect,
        productionAdd,
        uploadFild
    },
    data() {
        return {
            title: '新增',
            loading: false,
            exportLoading: false,
            single: true,
            ids: [],
            facilityList: [],
            productList: [],
            createDateRange: [],
            tableData: [],
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                productType: '',
                productId: '',
                facilityId: null,
                beginTime: '',
                endTime: ''
            },
            total: 0,
        };
    },
    mounted() {
        this.getInitList();
    },
    methods: {
        // 时间格式化
        formatDate(date) {
            return moment(date).format('YYYY-MM-DD');
        },
        // 获取数据
        getInitList() {
            productPageList({ OrgId: this.$store.getters.orgId,pageNum: 1, pageSize: 9999 }).then(res => {
                this.productList = res.data.List;
            })
            facilityTree({ OrgId: this.$store.getters.orgId }).then(res => {
                this.facilityList = res.data;
            })

            this.getList();
        },
        // 获取列表
        getList() {
            this.loading = true;
            if (this.createDateRange.length > 0) {
                this.queryParams.beginTime = this.createDateRange[0];
                this.queryParams.endTime = this.createDateRange[1];
            }
            selectProductionPageList(this.queryParams).then(res => {
                this.loading = false;
                this.tableData = res.data.List;
                this.total = res.data.Total;
            })
        },

        normalizer(node) {
            if (node.Children == null || !node.Children.length) {
                delete node.Children;
            }
            return {
                id: node.Id,
                label: node.FacilityName,
                children: node.Children,
            };
        },

         /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },

        /** 重置按钮操作 */
        resetQuery() {
            this.statisticDateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },

        // 新增
        handleAdd() {
            this.title = '添加';
            this.$refs.productionAdd.openDialog();
        },

        // 修改
        handleUpdate(row) {
            this.title = '编辑';
            this.$refs.productionAdd.openDialog(row);
        },
        
        // 详情
        handleView(row){
            this.$refs.productionAdd.openDialog(row, true)
        },
        // 导入
        handleImport() {
            this.$refs.uploadFild.openDialog('EfficiencyService/Production/ImportProduction');
        },
        // 导入能源数据模版
        importTemplate() {
            importProductionTemplate();
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
                removeProduction({ Id: row.Id }).then(res => {
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
.vue-treeselect{
    width: 140px;
}
</style>