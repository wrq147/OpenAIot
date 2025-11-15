<template>
    <div>
        <el-dialog width="960px" :title="title" :visible.sync="agentOpen" highlight-current-row
            :close-on-click-modal="false" append-to-body>
            <el-tabs v-model="activeName" @tab-click="handleClick" v-if="showAgent && showKf">
                <el-tab-pane label="我的代理" name="first">
                </el-tab-pane>
                <el-tab-pane label="我的客户" name="second">
                </el-tab-pane>
            </el-tabs>
            <div v-if="activeName == 'first'">
                <el-form :model="agentQuery" ref="agentForm" :inline="true"
                    style="display: flex;justify-content: space-between;">
                    <div>
                        <el-form-item label="代理商" prop="Key">
                            <el-input v-model="agentQuery.OrgName" placeholder="请输入代理名称" clearable></el-input>
                        </el-form-item>
                        <el-form-item label="创建日期">
                            <el-date-picker class="form_input_style" v-model="agentDateRange" style="width:232px"
                                value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                                end-placeholder="结束日期"></el-date-picker>
                        </el-form-item>
                    </div>

                    <el-form-item>
                        <el-button icon="el-icon-refresh" @click="resetAgent">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="loadAgentList">搜索</el-button>
                    </el-form-item>
                </el-form>
                <el-table ref="agentTable" :data="agentList" tooltip-effect="dark" v-loading="agentLoading"
                    style="width: 100%" @current-change="onAgentChange">
                    <el-table-column label="代理商ID" align="center" key="OrgId" prop="OrgId" width="150" />
                    <el-table-column label="代理商名称" align="center" key="OrgName" prop="OrgName" :show-overflow-tooltip="true"
                        width="150" />
                    <el-table-column label="代理级别" align="center" key="GradeName" prop="GradeName" width="150"
                        :show-overflow-tooltip="true" />
                    <el-table-column label="上级企业名称" align="center" key="ParentOrgName" prop="ParentOrgName"
                        :show-overflow-tooltip="true" />
                    <el-table-column label="代理区域名称" align="center" key="RegionsName" prop="RegionsName" />
                </el-table>
                <pagination v-show="agentTotal > 0" :total="agentTotal" :page.sync="agentQuery.pageNum"
                    :limit.sync="agentQuery.pageSize" @pagination="loadAgentList" />
            </div>
            <div v-if="activeName == 'second'">
                <el-form :model="agentQuery" ref="customerForm" :inline="true"
                    style="display: flex;justify-content: space-between;">
                    <div>
                        <el-form-item label="创建日期">
                            <el-date-picker class="form_input_style" v-model="customDateRange" style="width:232px"
                                value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                                end-placeholder="结束日期"></el-date-picker>
                        </el-form-item>
                    </div>
                    <el-form-item>
                        <el-button icon="el-icon-refresh" @click="resetAgent">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="loadKfList">搜索</el-button>
                    </el-form-item>
                </el-form>
                <el-table ref="agentTable" :data="customerList" tooltip-effect="dark" v-loading="agentLoading"
                    style="width: 100%" @current-change="onCustomChange">
                    <el-table-column label="客户编号" align="center" key="CustomerNumber" prop="CustomerNumber" width="150" />
                    <el-table-column label="客户名称" align="center" key="CustomerName" prop="CustomerName"
                        :show-overflow-tooltip="true" width="150" />
                    <el-table-column label="客户类型" align="center" key="CustomerType" prop="CustomerType" width="150"
                        :show-overflow-tooltip="true">
                        <template slot-scope="scope">
                            <span>{{ scope.row.CustomerType == 0 ? '代理' : '直销' }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="客户行业" align="center" key="IndustryName" prop="IndustryName"
                        :show-overflow-tooltip="true" />
                    <el-table-column label="线索来源" align="center" key="FromType" prop="FromType">
                        <template slot-scope="scope" v-if="scope.row.FromType">
                            <span>{{ fromMap.get(scope.row.FromType) }}</span>
                        </template>
                    </el-table-column>
                </el-table>
                <pagination v-show="customerTotal > 0" :total="customerTotal" :page.sync="customerQuery.pageNum"
                    :limit.sync="customerQuery.pageSize" @pagination="loadKfList" />
            </div>
        </el-dialog>
    </div>
</template>
      
<script>
import { factorygetAgent } from "@/api/manufac/agentMansge";
import { checkPermi } from '@/utils/permission.js'
import {
    privateCustomer,
} from "@/api/crm/customer";
export default {
    name: "KfSelecter",
    props: {
      isFilterInvite: {
        type: Boolean,
        default:false,
      },
      IsInvite:{
        type: Boolean,
        default:false,
      },
      title:{
        type: String,
        default:'请选择目标企业',
      },
    },
    data() {
        return {
            // title:'请选择目标企业',
            showAgent: false,
            showKf: false,
            activeName: 'first',
            agentOpen: false, //选择代理商
            agentLoading: false,
            agentDateRange: [],
            agentQuery: {
                pageNum: 1,
                pageSize: 20,
                OrgName: ""
            },
            agentTotal: 0,
            agentList: [],
            agentSelectArr: [],
            customerQuery: {
                pageNum: 1,
                pageSize: 10,
                Belong: 2, //为1表示公海，为2表示私海，其它为全部（不用传）
            },
            customDateRange: [],
            customerList: [],
            customerTotal: 0,
            fromMap: new Map(), //线索来源map
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
        };
    },
    mounted() {
        if (checkPermi(['/ProducerService/Agent/AllList'])) {
            this.showAgent = true;
        }
        if (checkPermi(['/CRMService/Customer/List'])) {
            this.showKf = true;
        }
        this.setFromMap()
    },
    methods: {
        handleClick(){

        },
        setFromMap() {
            //设置线索来源map
            this.fromList.map(row => {
                this.fromMap.set(row.value, row.label);
            });
        },
        resetCustom() {
            this.customerDateRange = [];
            this.resetForm("customerForm");
            this.loadKfList()
        },
        resetAgent() {
            this.agentDateRange = [];
            this.resetForm("agentForm");
            this.loadAgentList();
        },
        onAgentChange(val) {
            if (val != null) {
                this.$emit("ok", val,false);
                this.agentOpen = false;
            }
        },
        onCustomChange(val) {
            if (val != null) {
                let obj={
                    OrgId:val.BindOrgId,
                    OrgName:val.CustomerName,
                    Id:val.Id
                }
                this.$emit("ok", obj,true);
                this.agentOpen = false;
            }
        },
        async loadAgentList() {
            //获取生产商的代理商的列表
            this.agentQuery.ParentOrgId = this.$store.getters.orgId;
            let res = await factorygetAgent(
                this.addDateRange(this.agentQuery, this.agentDateRange)
            );
            this.agentList = res.data.List;
            this.agentTotal = res.data.Total;
        },
        async loadKfList() {
            if(this.isFilterInvite){
                this.customerQuery.IsInvite=this.IsInvite
            }
            privateCustomer(this.addDateRange(this.customerQuery, this.customDateRange)).then(
                response => {
                    console.log("客户列表", response);
                    if (response.data && response.data.List) {
                        response.data.List.map(async row => {
                            row.IndustryName = "";
                            if (row.Industry) {
                                //行业类型
                                row.IndustryName = await this.$store.dispatch(
                                    "datas/industryName",
                                    row.Industry
                                );
                            }
                            row.LeaderName = "";
                        });
                        this.customerList = response.data.List;
                    }
                    this.customerTotal = response.data.Total;
                }
            );
        },
        async openAgentDialog() {
            this.agentOpen = true;
            this.agentLoading = true;
            if (this.showAgent) {
                await this.loadAgentList();
                this.activeName = "first"
            }
            if (this.showKf) {
                await this.loadKfList();
                this.activeName = "second"
            }
            this.agentLoading = false;
        },
    },
};
</script>
<style lang="scss" scoped></style>
      