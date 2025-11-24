<template>
    <div class="elbiaoge_elform">
        <el-row :gutter="10" class="mb8 button_row">
            <div>
                <el-col :span="1.5">
                    <el-button type="primary" plain @click="addNotices">
                        <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                        <span style="margin-left:6px">添加消息方式</span>
                    </el-button>
                </el-col>
            </div>
        </el-row>

        <el-table :data="dataList" style="width: 100%" v-loading="loadingTable">
            <el-table-column prop="NoticeWay" label="通知方式" align="center">
                <template slot-scope="scope">
                    <div>
                        {{ scope.row.NoticeWay ? (noticeMap.get(scope.row.NoticeWay) ?
                            noticeMap.get(scope.row.NoticeWay).label : '') : '' }}
                    </div>
                </template>
            </el-table-column>
            <el-table-column prop="TargetType" label="目标类型" align="center">
                <template slot-scope="scope">
                    <div>
                        {{ (scope.row.TargetType || scope.row.TargetType == 0) ? (typeMap.get(scope.row.TargetType) ?
                            typeMap.get(scope.row.TargetType).label : '') : '' }}
                    </div>
                </template>
            </el-table-column>
            <el-table-column prop="TargetValue" label="目标内容" align="center">
                <template slot-scope="scope">
                    <div v-if="scope.row.TargetType == 0">
                        {{ scope.row.TargetValue ? (userMap.get(scope.row.TargetValue) ?
                            userMap.get(scope.row.TargetValue).RealName : '该用户不存在') : '' }}
                    </div>
                    <div v-else-if="scope.row.TargetType == 1">
                        {{ scope.row.TargetValue ? (roleMap.get(scope.row.TargetValue) ?
                            roleMap.get(scope.row.TargetValue).roleName : '该角色不存在') : '' }}
                    </div>
                    <div v-else>{{ scope.row.TargetValue }}</div>
                </template>
            </el-table-column>
            <el-table-column label="操作" align="center" width="160">
                <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row, scope.$index)">修改</el-button>
                    <el-button type="text" icon="el-icon-delete" @click="deleteTarget(scope.row, scope.$index)"
                        style="margin-left:15px;">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-dialog :title="title + '消息通知方式'" :visible.sync="isOpenDialog" @close="closeDialog" :close-on-click-modal="false"
            :destroy-on-close="true" width="460px" center>
            <el-form :model="form" class="center_form" label-width="120px">
                <el-form-item label="通知方式：">
                    <el-select v-model="form.NoticeWay" placeholder="请选择通知方式" @change="changeNoticeWay">
                        <el-option v-for="item in noticeList" :key="item.value" :label="item.label"
                            :value="item.value"></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="目标类型：" v-if="form.NoticeWay && form.NoticeWay != ''">
                    <el-select v-model="form.TargetType" placeholder="请选择目标类型" @change="changeTargetType">
                        <el-option v-for="item in targetTypeList" :key="item.value" :label="item.label"
                            :value="item.value"></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item :label="valueLabel + '：'"
                    v-if="form.NoticeWay && form.NoticeWay != '' && (form.TargetType == 1 || form.TargetType == 0 || form.TargetType == 4)">
                    <el-select v-model="form.TargetValue" v-if="form.TargetType == 0" placeholder="请选择设备来源的用户">
                        <el-option v-for="item in userLists" :key="item.Id" :label="item.RealName"
                            :value="item.Id"></el-option>
                    </el-select>
                    <el-select v-model="form.TargetValue" v-if="form.TargetType == 1" placeholder="请选择设备来源的角色">
                        <el-option v-for="item in roleLists" :key="item.roleId" :label="item.roleName"
                            :value="item.roleId"></el-option>
                    </el-select>
                    <el-input style="width:227px" v-if="form.TargetType == 4 && form.NoticeWay == 'SMS'"
                        v-model="form.TargetValue" placeholder="请输入固定手机号" maxlength="11"
                        @input="form.TargetValue = form.TargetValue.replace(/[^\d]/g, '')"></el-input>
                    <el-input style="width:227px" v-if="form.TargetType == 4 && form.NoticeWay == 'EMAIL'"
                        v-model="form.TargetValue" placeholder="请输入固定邮箱"
                        @input="form.TargetValue = form.TargetValue.replace(/[\u4e00-\u9fa5]/g, '')"></el-input>
                </el-form-item>
            </el-form>
            <div slot="footer">
                <el-button @click="closeDialog">取 消</el-button>
                <el-button type="primary" @click="saveNotices">确 定</el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
import { listMembers } from "@/api/system/member";
import { listRole } from "@/api/system/role";
export default {
    props: {
        noticeStr: String,
    },
    data() {
        return {
            noticeList: [
                { label: "APP站内通知", value: "APP" },
                { label: "EMAIL邮件通知", value: "EMAIL" },
                { label: "SMS短信通知", value: "SMS" },
                { label: "WX微信通知", value: "WX" }
            ],
            userLists: [],
            roleLists: [],
            activeIndex: -1,
            isOpenDialog: false,
            userMap: new Map(),
            roleMap: new Map(),
            noticeMap: new Map(),
            typeMap: new Map(),
            loadingTable: true,
            title: "添加",
            dataList: [],
            form: {}
        };
    },
    computed: {
        selectedNode() {
            return this.$store.state.rulesFlowable.rulesSelectedNode;
        },
        valueLabel() {
            if (this.form.TargetType == 0) {
                return "目标用户";
            } else if (this.form.TargetType == 1) {
                return "目标角色";
            } else if (
                this.form.TargetType == 2 ||
                this.form.TargetType == 3
            ) {
                return "";
            } else if (this.form.TargetType == 4) {
                if (this.form.NoticeWay == "EMAIL") {
                    return "固定邮箱";
                }
                if (this.form.NoticeWay == "SMS") {
                    return "固定手机号";
                }
            }
        },
        setup() {
            return this.$store.state.rulesFlowable.rulesDesign;
        },
        targetTypeList() {
            if (
                this.form.NoticeWay == "EMAIL" ||
                this.form.NoticeWay == "SMS"
            ) {
                return [
                    { label: "设备来源的用户", value: 0 },
                    { label: "设备来源的角色", value: 1 },
                    { label: "设备拥有者", value: 2 },
                    { label: "设备使用者", value: 3 },
                    { label: "固定目标", value: 4 }
                ];
            } else {
                return [
                    { label: "设备来源的用户", value: 0 },
                    { label: "设备来源的角色", value: 1 },
                    { label: "设备拥有者", value: 2 },
                    { label: "设备使用者", value: 3 }
                ];
            }
        }
    },
    async mounted() {
        this.loadingTable = true;
        await Promise.all([this.getUserList(), this.getRoleList()]);
        this.initNoticeMap(this.noticeList);
        this.initTypeMap();

        if (this.noticeStr == null || this.noticeStr == "") {
            this.dataList = [];
        }
        else {
            this.dataList = JSON.parse(this.noticeStr);
        }

        this.loadingTable = false;
    },
    methods: {
        handleUpdate(row, inx) {
            //获取该行的数据
            this.activeIndex = inx;
            this.title = "修改";
            this.form=this.dataList[inx];
            this.isOpenDialog = true;
        },
        deleteTarget(row, inx) {
            this.$modal
                .confirm('是否确认删除？')
                .then(() => {
                    this.dataList.splice(inx, 1);
                    this.$emit("saveNotice", this.dataList);
                    this.$modal.msgSuccess("删除成功");
                })

        },
        saveNotices() {
            this.isOpenDialog = false;
            if (this.activeIndex < 0) {
                this.dataList.push(this.form);
            }
            else {
                this.dataList[this.activeIndex] = this.form;
            }
            this.$emit("saveNotice", this.dataList);
        },
        closeDialog() {
            this.isOpenDialog = false;
        },
        addNotices() {
            this.title = "添加";
            this.isOpenDialog = true;
            this.form = {
                NoticeWay: "", //通知方式:APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知
                TargetType: null, // 目标类型：0为用户，1为角色，2为设备拥有者，3为设备使用者（可能是组织的管理员或设备使用者）,4为固定目标     （注：定时器触发和http触发无2和3）
                TargetValue: "" // TargetType为4时传邮件通知时传固定邮箱，短信通知时传固定手机号，0时传用户Id,1时传角色Id，其它传空
            };
            this.activeIndex = -1;
        },
        changeNoticeWay() {
            this.form.TargetType = null;
            this.form.TargetValue = "";
        },
        changeTargetType() {
            this.form.TargetValue = "";
        },
        async getUserList() {
            //获取用户列表
            let rsp = await listMembers({ showAll: true,isPrimaryDept:true });
            this.userLists = rsp.data.List;
            this.initUserMap(rsp.data.List);
            this.$forceUpdate();
        },
        async getRoleList() {
            //获取角色列表
            let rsp = await listRole({ showAll: true });
            this.roleLists = rsp.data.List;
            this.initRoleMap(rsp.data.List);
            this.$forceUpdate();
        },
        initUserMap(node) {
            for (let idx = 0; idx < node.length; idx++) {
                let curnode = node[idx];
                // console.log("组合时用户列表curnode",curnode);
                this.userMap.set(node[idx].Id, curnode);
            }
        },
        initRoleMap(node) {
            for (let idx = 0; idx < node.length; idx++) {
                let curnode = node[idx];
                // console.log("组合时用户列表curnode",curnode);
                this.roleMap.set(node[idx].roleId, curnode);
            }
        },
        initNoticeMap(node) {
            for (let idx = 0; idx < node.length; idx++) {
                let curnode = node[idx];
                // console.log("组合时用户列表curnode",curnode);
                this.noticeMap.set(node[idx].value, curnode);
            }
        },
        initTypeMap() {
            let node = [
                { label: "用户", value: 0 },
                { label: "角色", value: 1 },
                { label: "设备拥有者", value: 2 },
                { label: "设备使用者", value: 3 },
                { label: "固定目标", value: 4 }
            ];
            for (let idx = 0; idx < node.length; idx++) {
                let curnode = node[idx];
                // console.log("组合时用户列表curnode",curnode);
                this.typeMap.set(node[idx].value, curnode);
            }
        }
    },
};
</script>
  
<style lang="scss" scoped>
.item-desc {
    color: rgba(50, 150, 250, 0.71);
    display: block;
    width: 80%;
    height: 36px;
    line-height: 36px;
    background-color: #f5f7fa;
    text-align: left;
    margin-bottom: 10px;
    font-size: 14px;
    border-radius: 5px;
    border: 1px solid #dcdfe6;
    padding-left: 30px;
    box-sizing: border-box;
}

.choose {
    border-radius: 5px;
    margin-top: 2px;
    background: #f4f4f4;
    border: 1px dashed #1890ff !important;
}

.drag-hover {
    color: #1890ff;
}

.drag-no-choose {
    cursor: move;
    background: #f8f8f8;
    border-radius: 5px;
    margin: 5px 0;
    height: 25px;
    line-height: 25px;
    padding: 5px 10px;
    border: 1px solid #ffffff;

    div {
        display: inline-block;
        font-size: small !important;
    }

    div:nth-child(2) {
        float: right !important;
    }
}
</style>