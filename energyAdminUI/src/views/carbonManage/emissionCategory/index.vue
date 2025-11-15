<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan biaodan_flex" :model="queryParams" ref="queryForm" :inline="true">
                <div class="biaodan_input_con">
                    <el-form-item label="子类别" prop="subClassName">
                        <el-input v-model="queryParams.subClassName" placeholder="请输入关键字查询" clearable />
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
                </div>
            </el-row>
            <el-table v-loading="loading" :data="tableData" class="data_table" style="width:100%">
                <el-table-column label="序号" align="center" prop="ClassNo" sortable width="60"  />
                <el-table-column label="排放类别" align="center" prop="ClassName" />
                <el-table-column label="所属范围" align="center">
                    <template slot-scope="scope">
                        <el-tooltip class="item" effect="dark" :content="rangeList[scope.row.RangeId]" placement="bottom">
                            <div class="text-ellipsis">{{ rangeList[scope.row.RangeId] }}</div>
                        </el-tooltip>
                    </template>
                </el-table-column>
                <el-table-column label="子类别" align="center" prop="SubClassName">
                    <template slot-scope="scope">
                        <el-tooltip class="item" effect="dark" :content="scope.row.SubClassName" placement="bottom">
                            <div class="text-ellipsis">{{ scope.row.SubClassName }}</div>
                        </el-tooltip>
                    </template>
                </el-table-column>
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
                        <el-button class="primary" type="text" @click="handleUpdate(scope.row)">编辑</el-button>
                        <div class="line"></div>
                        <el-button class="danger" type="text" @click="handleDelete(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
        </div>
        <categoryAdd ref="categoryAdd" :rangeList="rangeList" :title="title" @getList="handleQuery" />
    </div>
</template>
<script>
import { classPageList, classRemove } from '@/api/energy/emissionCategory';
import categoryAdd from './cmp/categoryAdd.vue'
export default {
    name: 'energyGather',
    components: {
        categoryAdd
    },
    data() {
        return {
            title: '新增',
            loading: false,
            tableData: [],
            queryParams: {
                OrgId: this.$store.getters.orgId,
                pageNum: 1,
                pageSize: 10,
                subClassName: '',
            },
            total: 0,
            rangeList: {
                1: '范围1（直接排放）：企业直接控制排放源（如燃料燃烧、工艺排放等）',
                2: '范围2（间接排放）：企业通过控制间接排放源（如通过其他企业排放）间接排放',
                3: '范围3（综合排放）：企业同时控制直接和间接排放源',
            },
        };
    },
    mounted() {
        this.getList();
    },
    methods: {
        // 获取列表
        getList() {
            this.loading = true;
            classPageList(this.queryParams).then(res => {
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
            this.resetForm("queryForm");
            this.handleQuery();
        },

        // 新增
        handleAdd() {
            this.title = '添加';
            this.$refs.categoryAdd.openDialog();
        },

        // 修改
        handleUpdate(row) {
            this.title = '编辑';
            this.$refs.categoryAdd.openDialog(row);
        },
        
        // 详情
        handleView(row){
            this.$refs.categoryAdd.openDialog(row, true)
        },
        
        // 删除
        handleDelete(row) {
            this.$confirm('确定删除该排放类别吗？', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                classRemove({ Id: row.Id }).then(res => {
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
.text-ellipsis{
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}
</style>