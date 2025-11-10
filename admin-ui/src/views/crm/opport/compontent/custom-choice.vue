<template>
    <div>
        <el-dialog width=" 900px" title="请选择客户" :visible.sync="customOpen" :close-on-click-modal="false" append-to-body>
            <el-form :model="customQuery" ref="customQuery" :inline="true"
                style="display: flex;justify-content: space-between;">
                <el-form-item label="创建日期">
                    <el-date-picker class="form_input_style" v-model="customDateRange" style="width:232px"
                        value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                        end-placeholder="结束日期"></el-date-picker>
                </el-form-item>
                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
            </el-form>
            <el-table ref="customOptions" :data="customOptions" tooltip-effect="dark" style="width: 100%"
                highlight-current-row v-loading="customLoading" @current-change="handleCustomChange">
                <el-table-column label="企业ID" align="center" key="OrgId" prop="OrgId" />
                <el-table-column label="客户名称" align="center" key="CustomerName" prop="CustomerName"
                    :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                        <p style="white-space: pre-line !important;">{{ scope.row.CustomerName }}</p>
                    </template>
                </el-table-column>
                <el-table-column label="客户类型" align="center" key="CustomerType" prop="CustomerType"
                    :show-overflow-tooltip="true">
                    <template slot-scope="scope">
                        <span>{{ scope.row.CustomerType == 0 ? '代理' : '直销' }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="行业类型" align="center" key="IndustryName" prop="IndustryName"
                    :show-overflow-tooltip="true" />
                <el-table-column label="公司电话" align="center" key="CompanyTel" prop="CompanyTel" />
                <!-- <el-table-column label="公司网址" align="center" key="CompanyUrl" prop="CompanyUrl" />
                <el-table-column label="公司地址" align="center" key="AddressName" prop="AddressName">
                    <template slot-scope="scope">
                        <p>{{ scope.row.AddressName + scope.row.AddressDetail }}</p>
                    </template>
                </el-table-column> -->
                <el-table-column label="负责人" align="center" key="LeaderName" prop="LeaderName" />
                <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName">
                    <template slot-scope="scope">
                        <p>{{ scope.row.HelperName }}</p>
                    </template>
                </el-table-column>
                <el-table-column label="线索来源" align="center" key="FromType" prop="FromType">
                    <template slot-scope="scope" v-if="scope.row.FromType">
                        <span>{{ fromMap.get(scope.row.FromType) }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="创建时间" align="center" prop="createTime" width="240">
                    <template slot-scope="scope">
                        <span>{{ parseTime(scope.row.createTime) }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="customTotal > 0" :total="customTotal" :page.sync="customQuery.pageNum"
                :limit.sync="customQuery.pageSize" @pagination="loadcustomList" />
        </el-dialog>
    </div>
</template>

<script>
import {
    privateCustomer
} from "@/api/crm/customer";
import { listMember } from "@/api/system/Employee";
export default {
    name: 'AdminUiCustomChoice',

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
            customQuery: {
                pageSize: 10,
                pageNum: 1,
            },
            customOpen: false,
            customOptions: [],
            customLoading: false,
            customTotal: 0,
            customDateRange: [],
            fromMap: new Map(), //线索来源map
            employeeMap: new Map(), //员工map
            //选择客户相关
        };
    },

    async mounted() {
        // this.loadcustomList()
        this.setFromMap();
    //    await this.getMemberList()
    },

    methods: {
        //客户、联系人
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
            this.customQuery.pageNum = 1;
            this.loadcustomList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.customDateRange = [];
            this.resetForm("customQuery");
            this.handleQuery();
        },
        handleCustomChange(val) {
            //完成选择
            if (val != null) {
                this.$emit('handleCustomChange', val)
                this.customOpen=false
            }
        },
        openCustomDialog() {
            this.customOpen = true;
            this.loadcustomList();
        },
        loadcustomList() {
            //获取客户列表
            // console.log("是否查询客户列表");
            this.customLoading = true
            privateCustomer(this.addDateRange(this.customQuery, this.customDateRange)).then(response => {
                // console.log("客户列表", response);
                if (response.data&&response.data.List) {
                    // response.data.List.map(async row => {
                    //     if (row.Helper) {
                    //         //根据id获取协作人名称
                    //         let helperArr = [];
                    //         let arr = row.Helper.split(",");
                    //         arr.map(it => {

                    //             helperArr.push(this.employeeMap.get(parseInt(it)).RealName);
                    //         });
                    //         row.HelperName = helperArr.join(",");
                    //     }
                    //     row.IndustryName = "";
                    //     if (row.Industry) {
                    //         //行业类型
                    //         row.IndustryName = await this.$store.dispatch(
                    //             "datas/industryName",
                    //             row.Industry
                    //         );
                    //     }
                    //     row.LeaderName = "";
                    //     if (row.LeaderId) {
                    //         //负责人姓名获取
                    //         row.LeaderName = this.employeeMap.get(
                    //             parseInt(row.LeaderId)
                    //         ).RealName;
                    //     }
                    // });
                    this.customOptions = response.data.List
                    this.customTotal = response.data.Total
                    this.customLoading = false
                }

            })
        },
        //客户、联系人
    },
};
</script>

<style lang="less" scoped></style>