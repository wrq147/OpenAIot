<template>
    <el-dialog title="工单详情" :visible.sync="detailDialogVisible" width="80%" append-to-body @close="handleClose">
        <el-tabs v-if="taskData" v-model="activeTab" type="card">
            <!-- 基本信息标签页 -->
            <el-tab-pane label="基本信息" name="baseInfo">
                <el-descriptions :column="2" border :span="16" class="detail-descriptions">
                    <el-descriptions-item label="工单编号">{{ taskData.WorkNumber }}</el-descriptions-item>
                    <el-descriptions-item label="产品编号">{{ taskData.SkuNumber }}</el-descriptions-item>
                    <el-descriptions-item label="产品名称">{{ taskData.ProductName }}</el-descriptions-item>
                    <el-descriptions-item label="状态">
                        <el-tag v-if="taskData.IsFinish == true" type="success">已完成</el-tag>
                        <el-tag v-else>进行中</el-tag>
                    </el-descriptions-item>
                    <el-descriptions-item label="计划数">{{ taskData.PlanNum }}</el-descriptions-item>
                    <el-descriptions-item label="良品数">{{ taskData.GoodNum }}</el-descriptions-item>
                    <el-descriptions-item label="不良品数">{{ taskData.DefectNum }}</el-descriptions-item>
                    <el-descriptions-item label="完成率">
                        {{ taskData.PlanNum ? ((taskData.GoodNum / taskData.PlanNum) * 100).toFixed(2) + '%' : '0%' }}
                    </el-descriptions-item>
                    <el-descriptions-item label="允许报工人员" :span="2">
                        <div class="assigned-users-container">
                            <div v-for="(user, index) in parseAssignedUsers(taskData.AssignedUser)" :key="index"
                                :class="['assigned-user-tag', user.type === 'user' ? 'user-tag' : 'dept-tag']">
                                <i
                                    :class="['el-icon', user.type === 'user' ? 'el-icon-user' : 'el-icon-office-building']"></i>
                                <span class="tag-text">{{ user.name || (user.type === "user" ? "未知人员" : "未知部门")
                                }}</span>
                            </div>
                            <span
                                v-if="!taskData.AssignedUser || parseAssignedUsers(taskData.AssignedUser).length === 0"
                                class="no-user-text">
                                无指定人员
                            </span>
                        </div>
                    </el-descriptions-item>
                </el-descriptions>
            </el-tab-pane>

            <!-- 工艺信息标签页 -->
            <el-tab-pane label="工艺信息" name="processInfo">
                <el-descriptions :column="2" border :span="16" class="detail-descriptions">
                    <el-descriptions-item label="工序名称">{{ taskData.OperName }}</el-descriptions-item>
                    <el-descriptions-item label="工序编号">{{ taskData.OperCode || '无' }}</el-descriptions-item>
                    <el-descriptions-item label="报工数配比">{{ taskData.PropOf }}</el-descriptions-item>
                    <el-descriptions-item label="预计平均工时">{{ taskData.WorkTime + "分钟" }}</el-descriptions-item>
                    <el-descriptions-item label="实际总工时">{{ taskData.WorkTimeTotal + "分钟" }}</el-descriptions-item>
                    <el-descriptions-item label="预计总工时">{{ (taskData.WorkTime * taskData.PlanNum) + "分钟"
                    }}</el-descriptions-item>
                    <template v-for="(item, ix) in filedTableList">
                        <el-descriptions-item :label="item.name" :key="'custom_des' + ix">
                            <div v-html="getFieldShow(taskData.RouteOper, item)"></div>
                        </el-descriptions-item>
                    </template>
                </el-descriptions>
            </el-tab-pane>

            <!-- 报工记录标签页 - 表格形式 -->
            <el-tab-pane label="报工记录" name="reportRecord">
                <div>
                    <!-- 查询条件 -->
                    <el-form :inline="true" :model="reportQueryParams" class="report-search-form">
                        <el-form-item label="报工人员">
                            <el-input v-model="reportQueryParams.UserName" placeholder="请输入报工人员" clearable
                                style="width: 180px;"></el-input>
                        </el-form-item>
                        <el-form-item label="报工时间">
                            <el-date-picker v-model="reportQueryParams.dateRange" type="daterange" range-separator="至"
                                start-placeholder="开始日期" end-placeholder="结束日期" value-format="yyyy-MM-dd"
                                style="width: 240px;" clearable></el-date-picker>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="el-icon-search" @click="loadReportRecords">查询</el-button>
                            <el-button icon="el-icon-refresh" @click="resetReportQuery">重置</el-button>
                        </el-form-item>
                    </el-form>

                    <!-- 报工记录表格 -->
                    <el-table :data="reportRecords" border stripe style="width: 100%;" v-loading="recordLoading">
                        <el-table-column label="序号" type="index" width="60" align="center" />
                        <el-table-column label="报工单号" prop="ReportNumber" align="center" min-width="120" />
                        <el-table-column label="报工人员" prop="ReportUserName" align="center" min-width="100" />
                        <el-table-column label="报工数量" prop="ReportNum" align="center" width="100">
                            <template slot-scope="scope">
                                <span style="color: #1989fa; font-weight: bold;">{{ scope.row.ReportNum }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="良品数" prop="GoodNum" align="center" width="100">
                            <template slot-scope="scope">
                                <span style="color: #52c41a;">{{ scope.row.GoodNum }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="不良品数" prop="DefectNum" align="center" width="100">
                            <template slot-scope="scope">
                                <span style="color: #f5222d;">{{ scope.row.DefectNum }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="不良原因" prop="DefectReason" align="center" min-width="120"
                            show-overflow-tooltip />
                        <el-table-column label="报工时间" prop="ReportTime" align="center" min-width="180" />
                        <el-table-column label="备注" prop="Remark" align="center" min-width="150"
                            show-overflow-tooltip />
                    </el-table>

                    <!-- 分页控件 -->
                    <el-pagination @size-change="handleSizeChange" @current-change="handleCurrentChange"
                        :current-page="reportQueryParams.pageNum" :page-sizes="[10, 20, 50, 100]"
                        :page-size="reportQueryParams.pageSize" :total="reportTotal"
                        layout="total, sizes, prev, pager, next, jumper" style="margin-top: 15px; text-align: right;">
                    </el-pagination>

                </div>
            </el-tab-pane>
        </el-tabs>

        <div slot="footer" class="dialog-footer">
            <el-button @click="detailDialogVisible = false">关闭</el-button>
        </div>
    </el-dialog>
</template>

<script>
import { ReportList } from "@/api/mes/report";
import { taskInfo } from "@/api/mes/task";
import { orgField } from "@/api/factory/customFields";
import { getFieldShow } from '@/utils/field.js'
export default {
    name: 'TaskDetail',
    data() {
        return {
            detailDialogVisible: false,
            activeTab: 'baseInfo',

            // 报工记录查询参数
            reportQueryParams: {
                pageNum: 1,
                pageSize: 10,
                TaskId: '',
                UserName: '',
                dateRange: []
            },

            // 报工记录数据
            recordLoading: false,
            reportRecords: [],
            reportTotal: 0,

            taskData: null,
            filedTableList: []
        }
    },
    watch: {
        activeTab(newVal) {
            if (newVal === 'reportRecord' && this.detailDialogVisible) {
                this.initReportQuery();
                this.loadReportRecords();
            }
        }
    },
    methods: {
        async getCustomFiled() {
            //获取自定义的字段
            this.filedTableList = [];
            let orgId = this.$store.state.user.orgId;
            let res = await orgField({ orgId: orgId, field: "报工" });
            if (res.data) {
                let filedList = JSON.parse(res.data.ExtValue);
                this.filedTableList = filedList;
            } else {
                this.filedTableList = [];
            }
        },
        // 解析分配用户数据
        parseAssignedUsers(assignedUsers) {
            if (!assignedUsers || assignedUsers === "") return [];
            try {
                return JSON.parse(assignedUsers);
            } catch (e) {
                console.error('解析分配用户数据失败:', e);
                return [];
            }
        },
        // 初始化报工记录查询参数
        initReportQuery() {
            this.reportQueryParams.TaskId = this.taskData.Id;
            this.reportQueryParams.pageNum = 1;
        },

        // 加载报工记录数据
        async loadReportRecords() {
            if (!this.reportQueryParams.TaskId) return;

            this.recordLoading = true;
            try {
                // 构建查询参数
                const params = {
                    ...this.reportQueryParams,
                    beginTime: this.reportQueryParams.dateRange[0] || '',
                    endTime: this.reportQueryParams.dateRange[1] || ''
                };
                delete params.dateRange;

                let res = await ReportList(params);
                this.reportRecords = res.data.List;
                this.reportTotal = res.data.Total;

            } catch (error) {
                this.$message.error('加载报工记录失败');
                console.error(error);
            } finally {
                this.recordLoading = false;
            }
        },

        // 重置报工记录查询
        resetReportQuery() {
            this.reportQueryParams = {
                pageNum: 1,
                pageSize: 10,
                taskId: this.taskData.id || this.taskData.WorkNumber,
                UserName: '',
                dateRange: []
            };
            this.reportRecords = [];
            this.reportTotal = 0;
        },


        // 分页大小改变
        handleSizeChange(val) {
            this.reportQueryParams.pageSize = val;
            this.loadReportRecords();
        },

        // 当前页改变
        handleCurrentChange(val) {
            this.reportQueryParams.pageNum = val;
            this.loadReportRecords();
        },
        async openDialog(id) {
            let res = await taskInfo({ "id": id });
            this.taskData = res.data;
            this.detailDialogVisible = true;
        },
        // 关闭弹窗
        handleClose() {
            this.detailDialogVisible = false;
        }
    }
}
</script>

<style scoped>
/* 详情描述列表样式 */
.detail-descriptions {
    margin-top: 10px;
}

::v-deep .detail-descriptions .el-descriptions__label {
    font-weight: bold;
}

::v-deep .detail-descriptions .el-descriptions__content {
    word-break: break-all;
}

/* 标签页样式 */
::v-deep .el-tabs--card {
    --el-tabs-card-border-color: var(--el-border-color);
}


/* 报工记录搜索表单 */
.report-search-form {
    padding-top: 20px;
    padding-left: 15px;
    margin-bottom: 10px;
    background-color: #f5f7fa;
}


/* 允许报工人员容器样式 */
.assigned-users-container {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
    padding: 2px 0;
    min-height: 26px;
    align-items: center;
}

/* 标签通用样式 */
.assigned-user-tag {
    display: inline-flex;
    align-items: center;
    padding: 2px 8px;
    border-radius: 4px;
    font-size: 12px;
    line-height: 1.4;
    white-space: nowrap;
}

/* 用户标签样式 */
.user-tag {
    background-color: #e6f7ff;
    color: #1890ff;
    border: 1px solid #91d5ff;
}

/* 部门标签样式 */
.dept-tag {
    background-color: #f6ffed;
    color: #52c41a;
    border: 1px solid #b7eb8f;
}

/* 图标样式 */
.assigned-user-tag .el-icon {
    margin-right: 4px;
    font-size: 12px;
}

/* 标签文本样式 */
.tag-text {
    max-width: 100px;
    overflow: hidden;
    text-overflow: ellipsis;
}

/* 无人员提示文本 */
.no-user-text {
    color: #999;
    font-size: 12px;
    font-style: italic;
}
</style>