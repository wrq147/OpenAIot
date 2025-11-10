<template>
    <div>
        <el-dialog width=" 900px" title="请选择商机" :visible.sync="opportOpen" :close-on-click-modal="false" append-to-body>
            <el-form :model="opportQuery" ref="opportQuery" :inline="true"
                style="display: flex;justify-content: space-between;">
                <el-form-item label="创建日期">
                    <el-date-picker class="form_input_style" v-model="opportDateRange" style="width:232px"
                        value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                        end-placeholder="结束日期"></el-date-picker>
                </el-form-item>
                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
            </el-form>
            <el-table ref="opportOptions" :data="opportOptions" tooltip-effect="dark" style="width: 100%"
                highlight-current-row v-loading="opportLoading" @current-change="handleOpportChange">
                <el-table-column label="企业ID" align="center" key="OrgId" prop="OrgId" />
                <el-table-column label="商机名称" align="center" key="OpportName" prop="OpportName"
                    :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                        <p style="white-space: pre-line !important;">{{ scope.row.OpportName }}</p>
                    </template>
                </el-table-column>
                <el-table-column label="商机类型" align="center" key="OpporterType" prop="OpporterType"
                    :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                        <span>{{ scope.row.OpporterType == 0 ? '代理' : '直销' }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="客户名称" align="center" key="CustomerName" prop="CustomerName"
                    :show-overflow-tooltip="true" />
                <el-table-column label="联系人" align="center" key="ContactId" prop="ContactId" >
                    <template slot-scope="scope" v-if="scope.row.ContactId">
                        <span>{{ scope.row.ContactUser.RealName }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="负责人" align="center" key="LeaderName" prop="LeaderName" >
                    <template slot-scope="scope" v-if="scope.row.LeaderId">
                        <span>{{ scope.row.LeaderUser.RealName }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName">
                    <template slot-scope="scope" v-if="scope.row.Helper">
                        <span>{{ returnRowHelperName(scope.row) }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="销售阶段" align="center" key="Period" prop="Period">
                <template slot-scope="scope" v-if="scope.row.Period">
                  <span>{{ periodTypeMap.get(scope.row.Period) ? periodTypeMap.get(scope.row.Period).PeriodName : '' }}</span>
                </template>
              </el-table-column>
                <el-table-column label="创建时间" align="center" prop="createTime" width="100">
                    <template slot-scope="scope">
                        <span>{{ parseTime(scope.row.createTime) }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="opportTotal > 0" :total="opportTotal" :page.sync="opportQuery.pageNum"
                :limit.sync="opportQuery.pageSize" @pagination="loadopportList" />
        </el-dialog>
    </div>
</template>

<script>
import {
    opportList,
    periodList
} from "@/api/crm/opport";
export default {
    name: 'AdminUiOpportChoice',

    data() {
        return {
            //选择商机相关
            fromList: [
                {
                    value: "weixin",
                    label: "微信线索"
                },
                {
                    value: "form",
                    label: "流程表单"
                },
                {
                    value: "other",
                    label: "其他"
                }
            ],
            opportQuery: {
                pageSize: 10,
                pageNum: 1,
            },
            opportOpen: false,
            opportOptions: [],
            opportLoading: false,
            opportTotal: 0,
            opportDateRange: [],
            fromMap: new Map(), //线索来源map
            employeeMap: new Map(), //员工map
            periodTypeMap: new Map(),//销售阶段列表
            //选择商机相关
        };
    },

    async mounted() {
    },

    methods: {
        returnRowHelperName(row){
            if(row.Helper){
                let namelist=row.HelperUsers.map(ro=>ro.RealName)
                return namelist.join(',')
            }
        },
        async getPeriodList() {
            //获取销售阶段
            try {
                let res = await periodList()
                console.log("销售阶段", res);
                if (res.data) {
                    res.data.map(row => {
                        this.periodTypeMap.set(row.Id, row.PeriodName)
                    })

                }
            } catch (error) {
                console.log("销售阶段查询错误", error);
            }

        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.opportQuery.pageNum = 1;
            this.loadopportList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.opportDateRange = [];
            this.resetForm("opportQuery");
            this.handleQuery();
        },
        handleOpportChange(val) {
            //完成选择
            if (val != null) {
                this.$emit('handleOpportChange', val)
                this.opportOpen = false
            }
        },
       async openOpportDialog() {
            this.opportOpen = true;
            this.loadopportList();
            await this.getPeriodList();
        },
        loadopportList() {
            //获取商机列表
            console.log("是否查询商机列表");
            this.opportLoading = true
            opportList(this.addDateRange(this.opportQuery, this.opportDateRange)).then(response => {
                console.log("商机列表", response);
                if (response.data && response.data.List) {
                    this.opportOptions = response.data.List
                    this.opportTotal = response.data.Total
                    this.opportLoading = false
                }

            })
        },
        //商机、联系人
    },
};
</script>

<style lang="less" scoped></style>