<template>
    <div>
        <el-dialog width=" 900px" title="请选择线索" :visible.sync="clueOpen" :close-on-click-modal="false" append-to-body>
            <el-form :model="clueQuery" ref="clueQuery" :inline="true"
                style="display: flex;justify-content: space-between;">
                <el-form-item label="创建日期">
                    <el-date-picker class="form_input_style" v-model="clueDateRange" style="width:232px"
                        value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                        end-placeholder="结束日期"></el-date-picker>
                </el-form-item>
                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
            </el-form>
            <el-table ref="clueOptions" :data="clueOptions" tooltip-effect="dark" style="width: 100%" highlight-current-row
                v-loading="clueLoading" @current-change="handleClueChange">
                <el-table-column label="企业ID" align="center" key="OrgId" prop="OrgId" />
                <el-table-column label="客户名称" align="center" key="CompanyName" prop="CompanyName"
                    :show-overflow-tooltip="true" >
                    <template slot-scope="scope">
                        <p style="white-space: pre-line !important;">{{ scope.row.CompanyName }}</p>
                    </template>
                </el-table-column>
                <el-table-column label="联系人" align="center" key="RealName" prop="RealName" :show-overflow-tooltip="true" />
                <el-table-column label="手机号" align="center" key="Mobile" prop="Mobile" :show-overflow-tooltip="true" >
                    <template slot-scope="scope">
                        <p style="white-space: pre-line !important;">{{ scope.row.Mobile }}</p>
                    </template>
                </el-table-column>
                <el-table-column label="部门" align="center" key="DeptName" prop="DeptName" />
                <el-table-column label="职务" align="center" key="PostName" prop="PostName" />
                <el-table-column label="协作人名称" align="center" key="HelperName" prop="HelperName" />
                <el-table-column label="线索来源" align="center" key="FromType" prop="FromType">
                    <template slot-scope="scope" v-if="scope.row.FromType">
                        <span>{{ fromMap.get(scope.row.FromType) }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="创建时间" align="center" prop="createTime" width="100">
                    <template slot-scope="scope">
                        <span>{{ parseTime(scope.row.createTime) }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="clueTotal > 0" :total="clueTotal" :page.sync="clueQuery.pageNum"
                :limit.sync="clueQuery.pageSize" @pagination="loadclueList" />
        </el-dialog>
    </div>
</template>

<script>
import {
    privateClue
} from "@/api/crm/clue";
import { listMember } from "@/api/system/Employee";
export default {
    name: 'AdminUiClueChoice',

    data() {
        return {
            //选择线索相关
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
            clueQuery: {
                pageSize: 10,
                pageNum: 1,
            },
            clueOpen: false,
            clueOptions: [],
            clueLoading: false,
            clueTotal: 0,
            clueDateRange: [],
            fromMap: new Map(), //线索来源map
            employeeMap: new Map(), //员工map
            //选择线索相关
        };
    },

    async mounted() {

    },

    methods: {
        //线索、联系人
        async getMemberList() {
            try {
                let rsp = await listMember({ deptIdWithChildren: true, showAll: true,isPrimaryDept:true });
                if (rsp.data && rsp.data.List) {
                    rsp.data.List.map(row => {
                        this.employeeMap.set(row.Id, row);
                    });
                }
            } catch (err) {
                console.log("报错", err);

                this.$message.error(err.data);
            }
        },
        setFromMap() {
            //设置线索来源map
            this.fromList.map(row => {
                this.fromMap.set(row.value, row.label);
            });
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.clueQuery.pageNum = 1;
            this.loadclueList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.clueDateRange = [];
            this.resetForm("clueQuery");
            this.handleQuery();
        },
        handleClueChange(val) {
            //完成选择
            if (val != null) {
                this.$emit('handleClueChange', val)
                this.clueOpen = false
            }
        },
        async openClueDialog() {
            this.clueOpen = true;
            this.loadclueList();
            this.setFromMap();
            await this.getMemberList();
        },
        loadclueList() {
            //获取线索列表
            console.log("是否查询线索列表");
            this.clueLoading = true
            privateClue(this.addDateRange(this.clueQuery, this.clueDateRange)).then(response => {
                console.log("线索列表", response);
                if (response.data && response.data.List) {
                    response.data.List.map(async row => {
                        if (row.Helper) {
                            //根据id获取协作人名称
                            let helperArr = [];
                            let arr = row.Helper.split(",");
                            arr.map(it => {

                                helperArr.push(this.employeeMap.get(parseInt(it)).RealName);
                            });
                            row.HelperName = helperArr.join(",");
                        }
                        row.LeaderName = "";
                        if (row.LeaderId) {
                            //负责人姓名获取
                            row.LeaderName = this.employeeMap.get(
                                parseInt(row.LeaderId)
                            ).RealName;
                        }
                    });
                    this.clueOptions = response.data.List
                    this.clueTotal = response.data.Total
                    this.clueLoading = false
                }

            })
        },
        //线索、联系人
    },
};
</script>

<style lang="less" scoped></style>