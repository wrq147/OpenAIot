<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
        <div>
            <el-row :gutter="20">
                <el-col :span="24" :xs="24">
                    <div class="from_con" id="from_con">
                        <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                            <el-form-item prop="Key" label="关键字">
                                <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable />
                            </el-form-item>
                            <el-form-item class="submit_button_con">
                                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                            </el-form-item>
                        </el-form>
                    </div>
                    <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
                        <el-row :gutter="10" class="mb8 button_row">
                            <div>
                                <el-col :span="1.5">
                                    <el-button type="primary" icon="el-icon-plus" plain
                                        @click="handleAdd('')">新增</el-button>
                                </el-col>
                            </div>
                        </el-row>

                        <el-table v-loading="loading" :data="configList" class="data_table" :header-cell-style="cellSty"
                            style="width:100%" :fit="true">
                            <el-table-column label="策略编号" prop="Id" align="center" width="200">
                            </el-table-column>
                            <el-table-column label="策略名称" prop="Name" width="260" />
                            <el-table-column label="操作" align="center" fixed="right" class-name="small-padding"
                                width="250">
                                <template slot-scope="scope">
                                    <el-button type="text" icon="el-icon-edit"
                                        @click="handleAdd(scope.row)">编辑</el-button>
                                    <el-button type="text" icon="el-icon-delete" style="color:red"
                                        @click="handleDelete(scope.row.Id)">删除</el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                            :limit.sync="queryParams.pageSize" @pagination="getList" />
                    </div>
                </el-col>
            </el-row>
        </div>

        <confadd ref="addConfig" />
    </div>
</template>

<script>
import { removeVideoConfig,videoConfigList } from "@/api/rules/video";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import confadd from './confadd.vue'

export default {
    components: {
        confadd
    },
    mixins: [resizeTableCon],
    data() {
        return {
            loading: false,
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 20,
                Key: ''
            },
            total: 0,
            configList: [],
        }
    },
    created() {
        this.getList();
    },
    methods: {
        getList() {
            this.loading = true;
            videoConfigList(this.queryParams).then(response => {
                this.sourceList = response.data.List;
                this.total = response.data.Total;
                this.loading = false;
            })
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.resetForm("queryForm");
            this.handleQuery();
        },
        /** 新增按钮操作 */
        handleAdd(data) {
            if (data == '') {
                this.$refs.addConfig.showDlg(null);
            }
            else {
                this.$refs.addConfig.showDlg(data.Id);
            }
        },
        /** 删除按钮操作 */
        handleDelete(row) {
            this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeVideoConfig({ id: row.Id }).then(res => {
                    this.$message.success('删除成功!')
                    this.getList()
                })
            }).catch(() => { })
        },
    }
}
</script>
