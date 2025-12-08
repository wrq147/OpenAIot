<template>
    <el-dialog title="工单详情" v-if="detailDialogVisible" top="2vh" :visible.sync="detailDialogVisible" width="70%"
        append-to-body @close="handleClose">
        <div style="margin-bottom: 20px;">
            <el-descriptions :column="2" border :span="16" class="detail-descriptions">
                <el-descriptions-item label="工单编号">{{ orderData.WorkNumber }}</el-descriptions-item>
                <el-descriptions-item label="计划名称">{{ orderData.PlanInfo ? orderData.PlanInfo.PlanName : ''
                }}</el-descriptions-item>
                <el-descriptions-item label="产品名称">{{ orderData.ProdInfo ? orderData.ProdInfo.ProductName : ''
                }}</el-descriptions-item>
                <el-descriptions-item label="状态">
                    <el-tag v-if="orderData.Status == 0" type="warning">待生产</el-tag>
                    <el-tag v-if="orderData.Status == 1">生产中</el-tag>
                    <el-tag v-if="orderData.Status == 2" type="success">已完成</el-tag>
                    <el-tag v-if="orderData.Status == 3" type="danger">已取消</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="生产数量/计划数量">{{ orderData.BatchCount }}/{{ orderData.Quantity
                }}</el-descriptions-item>
                <el-descriptions-item label="当前进度">
                    <template v-if="orderData.Status == 1">
                        <el-progress type="line"
                            :percentage="Math.round((orderData.BatchCount / orderData.Quantity) * 100)"
                            :text-inside="true" :stroke-width="24" />
                    </template>
                    <template v-else-if="orderData.Status == 2">
                        <el-progress type="line" :percentage="100" :text-inside="true" :stroke-width="24"
                            status="success" />
                    </template>
                    <template v-else>
                        <span style="color: #999;">无数据</span>
                    </template>
                </el-descriptions-item>
                <el-descriptions-item label="优先级">
                    <span v-if="orderData.Priority == 1">优先安排</span>
                    <span v-if="orderData.Priority == 2">加急处理</span>
                    <span v-if="orderData.Priority == 3">正常排产</span>
                </el-descriptions-item>
                <el-descriptions-item label="超期时间">{{ orderData.OverTime }}</el-descriptions-item>
                <el-descriptions-item label="计划开始时间">{{ orderData.PlannedStartOn }}</el-descriptions-item>
                <el-descriptions-item label="计划结束时间">{{ orderData.PlannedEndOn }}</el-descriptions-item>
                <el-descriptions-item label="实际开始时间">{{ orderData.StartOn }}</el-descriptions-item>
                <el-descriptions-item label="实际结束时间">{{ orderData.EndOn }}</el-descriptions-item>

            </el-descriptions>
        </div>
        <el-tabs v-if="orderData" v-model="activeTab" type="card">
            <!-- 子工单标签页 -->
            <el-tab-pane label="子工单" name="subOrderList">
                <div>
                    <!-- 子工单查询条件 -->
                    <el-form :inline="true" :model="subOrderQueryParams" class="work-search-form">
                        <el-form-item label="子工单编号">
                            <el-input v-model="subOrderQueryParams.Key" placeholder="请输入子工单编号" clearable
                                style="width: 180px;"></el-input>
                        </el-form-item>
                        <el-form-item label="状态">
                            <el-select v-model="subOrderQueryParams.Status" placeholder="全部" clearable
                                style="width: 120px;">
                                <el-option label="待生产" value="0"></el-option>
                                <el-option label="生产中" value="1"></el-option>
                                <el-option label="已完成" value="2"></el-option>
                                <el-option label="已取消" value="3"></el-option>
                            </el-select>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="el-icon-search" @click="loadSubOrders">查询</el-button>
                            <el-button icon="el-icon-refresh" @click="resetSubOrderQuery">重置</el-button>
                        </el-form-item>
                    </el-form>

                    <!-- 子工单表格 -->
                    <el-table :data="subOrders" border stripe style="width: 100%;" v-loading="subOrderLoading">
                        <el-table-column label="子工单编号" prop="WorkNumber" align="center" width="160" />
                        <el-table-column label="产品名称" align="center" :show-overflow-tooltip="true">
                            <template slot-scope="scope">
                                <span>{{ scope.row.ProdInfo ? scope.row.ProdInfo.ProductName : '' }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="计划数量" prop="Quantity" align="center" width="100" />
                        <el-table-column label="完成数量" prop="BatchCount" align="center" width="100" />
                        <el-table-column label="状态" align="center" width="100">
                            <template slot-scope="scope">
                                <el-tag v-if="scope.row.Status == 0" type="warning">待生产</el-tag>
                                <el-tag v-if="scope.row.Status == 1">生产中</el-tag>
                                <el-tag v-if="scope.row.Status == 2" type="success">已完成</el-tag>
                                <el-tag v-if="scope.row.Status == 3" type="danger">已取消</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="优先级" align="center" width="100">
                            <template slot-scope="scope">
                                <span v-if="scope.row.Priority == 1">优先安排</span>
                                <span v-if="scope.row.Priority == 2">加急处理</span>
                                <span v-if="scope.row.Priority == 3">正常排产</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="创建时间" prop="CreatedOn" align="center" width="180" />
                    </el-table>

                    <!-- 分页控件 -->
                    <el-pagination @size-change="handleSubOrderSizeChange" @current-change="handleSubOrderCurrentChange"
                        :current-page="subOrderQueryParams.pageNum" :page-sizes="[10, 20, 50, 100]"
                        :page-size="subOrderQueryParams.pageSize" :total="subOrderTotal"
                        layout="total, sizes, prev, pager, next, jumper" style="margin-top: 15px; text-align: right;">
                    </el-pagination>
                </div>
            </el-tab-pane>

            <!-- 生产记录标签页 -->
            <el-tab-pane label="生产记录" name="workRecord">
                <div>
                    <!-- 查询条件 -->
                    <el-form :inline="true" :model="workQueryParams" class="work-search-form">
                        <el-form-item label="关键词">
                            <el-input v-model="workQueryParams.Key" placeholder="请输入搜索关键词" clearable
                                style="width: 180px;"></el-input>
                        </el-form-item>
                        <el-form-item label="创建时间">
                            <el-date-picker v-model="workQueryParams.dateRange" type="daterange" range-separator="至"
                                start-placeholder="开始日期" end-placeholder="结束日期" value-format="yyyy-MM-dd"
                                style="width: 240px;" clearable></el-date-picker>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="el-icon-search" @click="loadWorkRecords">查询</el-button>
                            <el-button icon="el-icon-refresh" @click="resetWorkQuery">重置</el-button>
                        </el-form-item>
                    </el-form>

                    <!-- 生产记录表格 -->
                    <el-table :data="workRecords" border stripe style="width: 100%;" v-loading="workLoading">
                        <el-table-column label="批次编号" prop="Id" align="center" width="160" />
                        <el-table-column label="通讯编号" prop="LNumber" align="center" width="160" />
                        <template v-for="ite in filedTableList">
                            <el-table-column :label="ite.name" align="center" :key="ite.mapid" :prop="ite.mapid"
                                :show-overflow-tooltip="true">
                                <template slot-scope="scope">
                                    <div v-html="ingetFieldShow(scope.row, ite)"></div>
                                </template>
                            </el-table-column>
                        </template>
                        <el-table-column label="创建时间" prop="CreatedOn" align="center" />
                        <el-table-column label="更新时间" prop="UpdatedOn" align="center" />
                        <el-table-column label="操作" align="center" fixed="right" class-name="small-padding fixed-width"
                            width="120">
                            <template slot-scope="scope">
                                <!-- 报工记录按钮 + Popover 时间线 -->
                                <el-popover ref="recordPopover" placement="left" width="400" trigger="click"
                                    @show="loadRecordTimeline(scope.row)">
                                    <div v-loading="timelineLoading" class="timeline-container">
                                        <el-timeline>
                                            <el-timeline-item v-for="(item, index) in timelineData" :key="index"
                                                :timestamp="item.createTime">
                                                <span v-if="item.Status == 0" style="color:#909399">
                                                    {{ item.ReportMem.RealName + "创建了一条未提交的报工" }}
                                                </span>
                                                <span v-else-if="item.Status == 1" style="color:#E6A23C">
                                                    {{ item.ReportMem.RealName + "提交了一条未审核的报工" }}
                                                </span>
                                                <span v-else-if="item.Status == 2" style="color:#67C23A">
                                                    {{ item.ReportMem.RealName + "提交了一条已审核的报工" }}
                                                </span>
                                                <span v-else-if="item.Status == 3" style="color:#909399">
                                                    {{ item.ReportMem.RealName + "取消了一条报工" }}
                                                </span>
                                                <span v-else-if="item.Status == 4" style="color:#F56C6C">
                                                    {{ item.ReportMem.RealName + "被驳回了一条报工" }}
                                                </span>
                                            </el-timeline-item>
                                        </el-timeline>
                                        <div v-if="timelineData.length === 0" class="empty-timeline">
                                            暂无报工记录
                                        </div>
                                    </div>

                                    <el-button slot="reference" type="text" icon="el-icon-notebook-2">
                                        报工记录
                                    </el-button>
                                </el-popover>
                            </template>
                        </el-table-column>
                    </el-table>

                    <!-- 分页控件 -->
                    <el-pagination @size-change="handleSizeChange" @current-change="handleCurrentChange"
                        :current-page="workQueryParams.pageNum" :page-sizes="[10, 20, 50, 100]"
                        :page-size="workQueryParams.pageSize" :total="reportTotal"
                        layout="total, sizes, prev, pager, next, jumper" style="margin-top: 15px; text-align: right;">
                    </el-pagination>

                </div>
            </el-tab-pane>
            <el-tab-pane label="生产物料" name="workBomList">
                <div>
                    <el-table :data="workBomList" border stripe style="width: 100%;" v-loading="workBomLoading">

                    </el-table>
                </div>
            </el-tab-pane>
        </el-tabs>

        <div slot="footer" class="dialog-footer">
            <el-button @click="detailDialogVisible = false">关闭</el-button>
        </div>
    </el-dialog>
</template>

<script>
import { WorkBatchList, mesOrderInfo, mesOrderList, mesBomList } from "@/api/mes/report";
import { orgFormFields } from "@/api/factory/customFields";
import { getFieldShow } from '@/utils/field.js'
import { ReportList } from '@/api/mes/report'
export default {
    data() {
        return {
            detailDialogVisible: false,
            activeTab: 'subOrderList',

            // 生产记录查询参数
            workQueryParams: {
                pageNum: 1,
                pageSize: 10,
                WorkOrderId: '',
                Key: '',
                dateRange: []
            },

            // 生产记录数据
            workLoading: false,
            workRecords: [],
            reportTotal: 0,

            // 子工单查询参数
            subOrderQueryParams: {
                pageNum: 1,
                pageSize: 10,
                ParentId: '',
                Key: '',
                Status: ''
            },

            // 子工单数据
            subOrderLoading: false,
            subOrders: [],
            subOrderTotal: 0,

            orderData: null,
            filedTableList: [],

            timelineLoading: false,
            timelineData: [],
            currentRecordId: '',

            workBomLoading: false,
            workBomList: [],
            WorkOrderId: null
        }
    },
    watch: {
        activeTab: {
            handler(newVal) {
                this.tabInit(newVal);
            }
        }
    },
    methods: {
        tabInit(tabval) {
            if (tabval === 'workRecord') {
                this.initWorkQuery();
                this.loadWorkRecords();
            }
            if (tabval === 'subOrderList') {
                this.initSubOrderQuery();
                this.loadSubOrders();
            }
            else if (tabval == 'workBomList') {
                this.loadBomList();
            }
        },
        async loadBomList() {
            this.workBomLoading = true;
            try {
                let res = await mesBomList(this.WorkOrderId);
                this.workBomList = res.data;

            } catch (error) {
                this.$message.error('加载物料信息失败');
            } finally {
                this.workBomLoading = false;
            }
        },
        ingetFieldShow(obj, field) {
            return getFieldShow(obj, field);
        },
        async getCustomFiled() {
            //获取自定义的字段
            this.filedTableList = [];
            let res = await orgFormFields({ field: "报工", ext: true, isfixed: false });
            this.filedTableList = res.data;
        },
        // 初始化生产记录查询参数
        initWorkQuery() {
            this.workQueryParams.WorkOrderId = this.orderData.Id;
            this.workQueryParams.pageNum = 1;
        },

        // 加载生产记录数据
        async loadWorkRecords() {
            this.workLoading = true;
            try {
                // 构建查询参数
                const params = {
                    ...this.workQueryParams,
                    beginTime: this.workQueryParams.dateRange[0] || '',
                    endTime: this.workQueryParams.dateRange[1] || ''
                };
                delete params.dateRange;

                let res = await WorkBatchList(params);
                this.workRecords = res.data.List;
                this.reportTotal = res.data.Total;

            } catch (error) {
                this.$message.error('加载生产记录失败');
            } finally {
                this.workLoading = false;
            }
        },

        // 重置生产记录查询
        resetWorkQuery() {
            this.workQueryParams = {
                pageNum: 1,
                pageSize: 10,
                WorkOrderId: this.orderData.Id,
                Key: '',
                dateRange: []
            };
            this.workRecords = [];
            this.reportTotal = 0;
        },

        // 初始化子工单查询参数
        initSubOrderQuery() {
            this.subOrderQueryParams.ParentOrderId = this.orderData.Id;
            this.subOrderQueryParams.pageNum = 1;
        },

        // 加载子工单数据
        async loadSubOrders() {
            this.subOrderLoading = true;
            try {
                const params = { ...this.subOrderQueryParams };

                // 调用API获取子工单列表
                let res = await mesOrderList(params);
                this.subOrders = res.data.List;
                this.subOrderTotal = res.data.Total;

            } catch (error) {
                this.$message.error('加载子工单失败');
            } finally {
                this.subOrderLoading = false;
            }
        },

        // 重置子工单查询
        resetSubOrderQuery() {
            this.subOrderQueryParams = {
                pageNum: 1,
                pageSize: 10,
                ParentId: this.orderData.Id,
                Key: '',
                Status: ''
            };
            this.subOrders = [];
            this.subOrderTotal = 0;
        },

        // 分页大小改变
        handleSizeChange(val) {
            this.workQueryParams.pageSize = val;
            this.loadWorkRecords();
        },

        // 当前页改变
        handleCurrentChange(val) {
            this.workQueryParams.pageNum = val;
            this.loadWorkRecords();
        },

        // 子工单分页大小改变
        handleSubOrderSizeChange(val) {
            this.subOrderQueryParams.pageSize = val;
            this.loadSubOrders();
        },

        // 子工单当前页改变
        handleSubOrderCurrentChange(val) {
            this.subOrderQueryParams.pageNum = val;
            this.loadSubOrders();
        },

        async openDialog(id) {
            this.WorkOrderId = id;
            await this.getCustomFiled();
            let res = await mesOrderInfo({ "id": id });
            this.orderData = res.data;
            this.detailDialogVisible = true;
            this.subOrderQueryParams.ParentId = id;
            await this.tabInit(this.activeTab);
        },
        // 关闭弹窗
        handleClose() {
            this.detailDialogVisible = false;
        },
        async loadRecordTimeline(record) {
            this.timelineLoading = true;
            this.currentRecordId = record.Id;
            this.timelineData = [];
            let res = await ReportList({ "pageSize": 0, "BatchNo": record.Id });
            this.timelineData = res.data.List;
            this.timelineLoading = false;
        }
    }
}
</script>

<style scoped>
/* 详情描述列表样式 */
.detail-descriptions {
    margin-top: 10px;
}

::v-deep .detail-descriptions .el-descriptions-item__label {
    font-weight: bold;
    width: 150px !important;
    flex: none !important;
}

::v-deep .detail-descriptions .el-descriptions-item__content {
    word-break: break-all;
    padding-left: 15px;
}

/* 标签页样式 */
::v-deep .el-tabs--card {
    --el-tabs-card-border-color: var(--el-border-color);
}


/* 搜索表单 */
.work-search-form {
    padding-top: 20px;
    padding-left: 15px;
    margin-bottom: 10px;
    background-color: #f5f7fa;
}

.timeline-container {
    padding-top: 10px;
}
</style>