<template>
    <div>
        <el-dialog width=" 900px" title="请选择客户" :visible.sync="contactOpen" :close-on-click-modal="false" append-to-body>
            <el-form :model="contactQuery" ref="contactQuery" :inline="true"
                style="display: flex;justify-content: space-between;">
                <el-form-item label="创建日期">
                    <el-date-picker class="form_input_style" v-model="contactDateRange" style="width:232px"
                        value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                        end-placeholder="结束日期"></el-date-picker>
                </el-form-item>
                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
            </el-form>
            <el-table ref="contactOptions" :data="contactOptions" tooltip-effect="dark" style="width: 100%"
                highlight-current-row v-loading="contactLoading" @current-change="handleContactChange">
                <el-table-column label="客户名称" align="center" key="CustomerId" prop="CustomerName"
                    :show-overflow-tooltip="true" />
                <el-table-column label="联系人" align="center" key="RealName" prop="RealName" :show-overflow-tooltip="true" />
                <el-table-column label="手机号" align="center" key="Mobile" prop="Mobile" :show-overflow-tooltip="true" />
                <el-table-column label="邮箱" align="center" key="Email" prop="Email" :show-overflow-tooltip="true" />
                <el-table-column label="微信号" align="center" key="WxNumber" prop="WxNumber" :show-overflow-tooltip="true" />
                <el-table-column label="部门" align="center" key="DeptName" prop="DeptName" />
                <el-table-column label="职务" align="center" key="PostName" prop="PostName" />
                <el-table-column label="协作人名称" align="center" key="HelperName" prop="HelperName" />
                <el-table-column label="创建时间" align="center" prop="createTime" width="240">
                    <template slot-scope="scope">
                        <span>{{ parseTime(scope.row.createTime) }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="contactTotal > 0" :total="contactTotal" :page.sync="contactQuery.pageNum"
                :limit.sync="contactQuery.pageSize" @pagination="loadcontactList" />
        </el-dialog>
    </div>
</template>

<script>
import {
    contactList,
} from "@/api/crm/contact";
export default {
    name: 'AdminUiContactChoice',

    data() {
        return {
            //选择客户相关
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
            contactQuery: {
                pageSize: 10,
                pageNum: 1,
            },
            contactOpen: false,
            contactOptions: [],
            contactLoading: false,
            contactTotal: 0,
            contactDateRange: [],
            customerMap: new Map(), //客户列表Map
            employeeMap: new Map(), //员工map
            //选择客户相关
        };
    },

    async mounted() {
    },

    methods: {
        /** 搜索按钮操作 */
        handleQuery() {
            this.contactQuery.pageNum = 1;
            this.loadcontactList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.contactDateRange = [];
            this.resetForm("contactQuery");
            this.handleQuery();
        },
        handleContactChange(val) {
            //完成选择
            if (val != null) {
                this.$emit('handleContactChange', val)
                this.contactOpen = false
            }
        },
        openContactDialog(CustomerId) {
            this.contactOpen = true;
            if(CustomerId){
                this.contactQuery.CustomerId=CustomerId
            }
            this.loadcontactList();
        },
        loadcontactList() {
            //获取客户列表
            console.log("是否查询客户列表");
            this.contactLoading = true
            contactList(this.addDateRange(this.contactQuery, this.contactDateRange)).then(response => {
                console.log("客户列表", response);
                if (response.data && response.data.List) {
                    response.data.List.map(row => {
                        if (row.Helper) {
                
                            row.HelperName = (row.HelperUsers.map(row=>row.RealName)).join(',')
                        }
                        row.LeaderName = "";
                        if (row.LeaderId) {
                            //负责人姓名获取
                            row.LeaderName = row.LeaderUser.RealName
                        }
                    });
                    this.contactOptions = response.data.List
                    this.contactTotal = response.data.Total
                    this.contactLoading = false
                }

            })
        },
        //客户、联系人
    },
};
</script>

<style lang="less" scoped></style>