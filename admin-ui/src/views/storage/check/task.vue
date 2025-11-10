<template>
    <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
        <div>
            <div class="from_con" id="from_con">
                <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
                    <el-form-item label="盘点名称" prop="Name">
                        <el-input v-model="queryParams.Name" placeholder="请输入盘点名称"></el-input>
                    </el-form-item>
                    <el-form-item label="初盘时间">
                        <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px"
                            value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                            end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                    <!-- <el-col class="float_right" :span="24"> -->
                    <el-form-item class="submit_button_con">
                        <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                    </el-form-item>
                    <!-- </el-col> -->
                </el-form>
            </div>
            <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">

                <el-table border v-loading="loading" highlight-current-row :data="tbList" class="data_table"
                    :header-cell-style="cellSty" style="width:100%" row-key="Id" @row-click="onRowClick">
                    <el-table-column type="index" align="center" width="50">
                    </el-table-column>
                    <el-table-column label="盘点状态" align="center" width="120">
                        <template slot-scope="scope">
                            <el-tag v-if="scope.row.Status == 0" type="warning">待提交</el-tag>
                            <el-tag v-if="scope.row.Status == 1" type="warning">待开始</el-tag>
                            <el-tag v-else-if="scope.row.Status == 2">初盘中</el-tag>
                            <el-tag v-else-if="scope.row.Status == 3">复盘中</el-tag>
                            <el-tag v-else-if="scope.row.Status == 4" type="danger">已结束</el-tag>
                            <el-tag v-else-if="scope.row.Status == 5" type="success">已修正</el-tag>
                            <el-tag v-else-if="scope.row.Status == 6" type="info">已取消</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column label="盘点名称" prop="Name" align="left"></el-table-column>
                    <el-table-column label="盘点仓库" align="center">
                        <template slot-scope="scope">
                            <span v-if="scope.row.House != null">{{ scope.row.House.StoreName }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="初盘人员" align="center">
                        <template slot-scope="scope">
                            <span>{{ getInventoryUsers(scope.row, 0) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="初盘时间" align="center" width="160">
                        <template slot-scope="scope">
                            <span v-if="scope.row.StartOn != null">{{ parseTime(scope.row.StartOn) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="复盘人员" align="center">
                        <template slot-scope="scope">
                            <span>{{ getInventoryUsers(scope.row, 1) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="复盘时间" align="center" width="160">
                        <template slot-scope="scope">
                            <span v-if="scope.row.CheckOn != null">{{ parseTime(scope.row.CheckOn) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="创建时间" align="center" width="160">
                        <template slot-scope="scope">
                            <span>{{ parseTime(scope.row.createTime) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column align="center" label="操作" width="160" fixed="right">
                        <template slot-scope="scope">
                            <el-link icon="el-icon-video-play" v-if="scope.row.Status == 2 || scope.row.Status == 3"
                                type="primary" @click="onRowClick(scope.row)">开始盘点</el-link>
                        </template>
                    </el-table-column>
                </el-table>
                <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                    :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
            <CheckTask ref="checkDlg" @reload="getList()"></CheckTask>
        </div>
    </div>
</template>
      
<script>
import {
    invTask
} from "@/api/storage/inventory";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import CheckTask from "./check.vue";
export default {
    mixins: [resizeTableCon],
    components: { CheckTask },
    data() {
        return {
            // 遮罩层
            loading: true,
            // 表格树数据
            tbList: [],
            // 日期范围
            dateRange: [],
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 10,
                Status: 9,
                Name: undefined
            },
            total: 0,
        };
    },
    created() {
        this.getList();
    },
    methods: {
        getList() {
            this.loading = true;
            invTask(this.addDateRange(this.queryParams, this.dateRange)).then(response => {
                this.tbList = response.data.List;
                this.total = response.data.Total;
                this.loading = false;
            });
        },
        onRowClick(row) {
            this.$refs.checkDlg.openDlg(row.Id);
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.dateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },
        getInventoryUsers(row, ti) {
            if (row.UserList == null) {
                return "";
            }
            var tmpusers = "";
            row.UserList.filter(x => x.TimeIn == ti).forEach(element => {
                tmpusers += "," + element.UserInfo.RealName;
            });
            if (tmpusers != "") {
                tmpusers = tmpusers.substring(1);
            }
            return tmpusers;
        },
    }
};
</script>
<style lang="scss"></style>