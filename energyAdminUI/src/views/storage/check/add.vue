<template>
    <div>
        <!-- 添加编辑盘点对话框 -->
        <el-dialog :title="title" v-loading="houseloading" :close-on-click-modal="false" :visible.sync="open" width="1280px"
            append-to-body>

            <div style="margin-bottom:30px;">
                <el-steps :active="active" simple finish-status="success">
                    <el-step title="基本信息"></el-step>
                    <el-step title="选择盘点物品"></el-step>
                    <el-step title="创建完成"></el-step>
                </el-steps>
            </div>
            <div v-loading="checkloading">
                <div v-if="active == 0">
                    <el-form ref="form" :model="form" :rules="rules" label-width="120px">
                        <el-row>

                            <el-col :span="12">
                                <el-form-item label="盘点名称" prop="Name">
                                    <el-input v-model="form.Name" placeholder="请输入盘点名称" />
                                </el-form-item>
                            </el-col>
                            <el-col :span="12">
                                <el-form-item label="盘点仓库" prop="HouseName">
                                    <div style="display:flex;align-items: center;">
                                        <el-input class="houseipt" v-model="form.HouseName" readonly placeholder="请选择盘点仓库"
                                            @focus="openHouseDialog">
                                            <i slot="suffix" @click="onClear" v-if="form.HouseId != null" class="el-icon-circle-close"
                                            style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                                        </el-input>
                                    </div>

                                </el-form-item>
                            </el-col>
                            <HouseSelecter ref="houseDlg" @ok="handleCurrentChange"></HouseSelecter>

                            <el-col :span="12">
                                <el-form-item label="初盘人员" :rules="[
                                    { required: true, validator: this.validateFirst, message: '请选择初盘人员', trigger: ['blur', 'change'] }
                                ]" prop="FirstUserIds">
                                    <el-select multiple filterable v-model="FirstUserIds" ref="selectFirst"
                                        placeholder="请选择初盘人员" @focus="getFirstFocus" @remove-tag="removeFirst"
                                        style="width:100%">
                                        <el-option v-for="item in FirstUser" :key="item.UserId"
                                            :label="item.UserInfo.RealName" :value="item.UserId">
                                        </el-option>
                                    </el-select>
                                    <org-picker :multiple="true" ref="firstPicker" @ok="onSelectFirst" />
                                </el-form-item>
                            </el-col>

                            <el-col :span="12">
                                <el-form-item label="复盘人员">
                                    <el-select multiple filterable v-model="CheckUserIds" ref="selectChecker"
                                        placeholder="请选择复盘人员" @focus="getCheckFocus" @remove-tag="removeCheck"
                                        style="width:100%">
                                        <el-option v-for="item in CheckUser" :key="item.UserId"
                                            :label="item.UserInfo.RealName" :value="item.UserId">
                                        </el-option>
                                    </el-select>
                                    <org-picker :multiple="true" ref="checkerPicker" @ok="onSelectCheck" />
                                </el-form-item>
                            </el-col>
                        </el-row>

                        <el-row>
                            <el-col :span="24">
                                <el-form-item label="备注说明" prop="Remark">
                                    <el-input type="textarea" :rows="2" placeholder="请输入备注说明"
                                        v-model="form.Remark"></el-input>
                                </el-form-item>
                            </el-col>
                        </el-row>
                    </el-form>

                    <div slot="footer" class="dialog-footer" style="text-align: right;">
                        <el-button type="primary" @click="NextForm">下一步</el-button>
                    </div>
                </div>

                <div v-else-if="active == 1">
                    <el-form :model="wupinQuery" :inline="true" ref="wupinForm"
                        style="display: flex;justify-content: space-between;">
                        <div>
                            <el-form-item label="搜索关键字" prop="Key">
                                <el-input v-model="wupinQuery.Key" placeholder="搜索物品名称或物品编号" clearable></el-input>
                            </el-form-item>
                        </div>
                        <el-form-item>
                            <el-button icon="el-icon-refresh" @click="resetWupin">重置</el-button>
                            <el-button type="primary" icon="el-icon-search" @click="loadWupinList">搜索</el-button>
                        </el-form-item>
                    </el-form>

                    <div class="wuping-it">
                        <div class="sel">
                            <div style="margin-bottom:10px;">
                                <el-tag>待选库存</el-tag>
                            </div>
                            <el-table ref="wupinTable" :data="wupinList" tooltip-effect="dark" style="width: 100%"
                                @selection-change="onWupinWaitChange">
                                <el-table-column type="selection" width="55"></el-table-column>
                                <el-table-column prop="DeviceNumber" label="物品编号" align="center"
                                    width="150"></el-table-column>
                                <el-table-column prop="Name" label="物品名称"></el-table-column>
                                <el-table-column align="center" label="库存数量" width="140">
                                    <template slot-scope="scope">
                                        <span>{{ scope.row.Quantity }}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
                                    <template slot-scope="scope">
                                        <div>
                                            {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
                                        </div>
                                    </template>
                                </el-table-column>
                            </el-table>
                            <pagination v-show="wupinTotal > 0" :total="wupinTotal" :page.sync="wupinQuery.pageNum"
                                :limit.sync="wupinQuery.pageSize" @pagination="loadWupinList" />
                        </div>
                        <div class="sec">
                            <div style="margin-bottom:15px;"><el-button :disabled="multipleWaitSelection.length == 0"
                                    icon="el-icon-d-arrow-right" @click="addSel" style="width:120px;">添加</el-button></div>
                            <div style="margin-bottom:15px;"><el-button :disabled="multipleCheckSelection.length == 0"
                                    icon="el-icon-d-arrow-left" @click="delSel" style="width:120px;">移除</el-button></div>
                            <div style="margin-bottom:15px;"><el-button icon="el-icon-d-arrow-right" @click="addAll"
                                    style="width:120px;">添加全部</el-button></div>
                            <div style="margin-bottom:15px;"><el-button icon="el-icon-d-arrow-left" @click="delAll"
                                    style="width:120px;">移除全部</el-button></div>
                        </div>
                        <div class="ser">
                            <div style="margin-bottom:10px;">
                                <el-tag>已选库存</el-tag>
                            </div>
                            <el-table ref="selTable" :data="selList" tooltip-effect="dark" style="width: 100%"
                                @selection-change="onWupinCheckedChange">
                                <el-table-column type="selection" width="55"></el-table-column>
                                <el-table-column prop="DeviceNumber" label="物品编号" align="center"
                                    width="150"></el-table-column>
                                <el-table-column prop="Name" label="物品名称"></el-table-column>
                                <el-table-column align="center" label="库存数量" width="140">
                                    <template slot-scope="scope">
                                        <span>{{ scope.row.Quantity}}</span>
                                    </template>
                                </el-table-column>
                                <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
                                    <template slot-scope="scope">
                                        <div>
                                            {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
                                        </div>
                                    </template>
                                </el-table-column>
                            </el-table>
                            <pagination v-show="selTotal > 0" :total="selTotal" :page.sync="selQuery.pageNum"
                                :limit.sync="selQuery.pageSize" @pagination="loadWupinList" />
                        </div>
                    </div>


                    <div slot="footer" class="dialog-footer" style="text-align: right;">
                        <el-button @click="PreForm">上一步</el-button>
                        <el-button type="primary" @click="submitForm">完成</el-button>
                    </div>
                </div>
                <div v-else-if="active == 2">
                    <el-result icon="success" title="成功提示" subTitle="恭喜您成功创建一张盘点单">
                        <template slot="extra">
                            <el-button type="primary" size="medium" @click="open = false;">返回</el-button>
                        </template>
                    </el-result>
                </div>
            </div>


        </el-dialog>
    </div>
</template>
      
<script>
import OrgPicker from "@/views/flowable/common/OrgPicker";
import HouseSelecter from "../house/HouseSelecter.vue";

import { invInfo, addInv, editInv, delItems, addItems, addItemsAll, delItemsAll, submitInv, invItemList } from "@/api/storage/inventory";
import {
    stockList
} from "@/api/storage/stock";
export default {
    components: { HouseSelecter, OrgPicker },
    data() {
        return {
            active: 0,
            title: "",
            checkloading: false,
            houseloading: false,
            // 表单参数
            form: {},
            // 是否显示弹出层
            open: false,
            // 表单校验
            rules: {
                Name: [
                    { required: true, message: "盘点名称不能为空", trigger: "blur" }
                ],
                HouseName: [
                    { required: true, message: "盘点仓库不能为空", trigger: ["blur", "change"] }
                ],
            },
            wupinQuery: {
                pageNum: 1,
                pageSize: 20,
                HouseId: ''
            },
            wupinTotal: 0,
            wupinList: [],

            selQuery: {
                pageNum: 1,
                pageSize: 20
            },
            selTotal: 0,
            selList: [],
            multipleWaitSelection: [],
            multipleCheckSelection: []
        };
    },
    computed: {
        FirstUserIds: {
            get() {
                if (this.form.UserList == null) return [];
                let uss = this.form.UserList.filter(x => x.TimeIn == 0);
                return uss.map(x => {
                    return x.UserId;
                });
            },
            set(val) {
            }
        },
        FirstUser: function () {
            if (this.form.UserList == null) return [];
            return this.form.UserList.filter(x => x.TimeIn == 0);
        },
        CheckUserIds: {
            get() {
                if (this.form.UserList == null) return [];
                let uss = this.form.UserList.filter(x => x.TimeIn == 1);
                return uss.map(x => {
                    return x.UserId;
                });
            },
            set(val) {
            }
        },
        CheckUser: function () {
            if (this.form.UserList == null) return [];
            return this.form.UserList.filter(x => x.TimeIn == 1);
        }
    },
    methods: {
        async openDlg(title, id) {
            this.title = title;
            this.active = 0;
            this.open = true;
            if (id == null) {
                this.reset();
            }
            else {
                this.form.Id = id;
                this.form = (await invInfo(id)).data;
                this.form.HouseName = this.form.House.StoreName;
            }
        },
        validateFirst(rule, value, callback) {
            if (this.FirstUserIds.length <= 0) {
                callback('请选择初盘人员')
            } else {
                callback()
            }
        },
        addSel() {
            let mapsel = this.multipleWaitSelection.map(x => {
                return { TargetType: x.TargetType, TargetId: x.TargetId };
            });
            addItems(this.form.Id, mapsel).then(rsp => {
                this.loadWupinList();
                this.loadSelList();
            })
        },
        delSel() {
            let mapsel = this.multipleCheckSelection.map(x => {
                return x.TargetId;
            });
            delItems(this.form.Id, mapsel).then(rsp => {
                this.loadWupinList();
                this.loadSelList();
            })
        },
        addAll() {
            addItemsAll({ InventId: this.form.Id, HouseId: this.form.HouseId, Key: this.wupinQuery.Key }).then(rsp => {
                this.loadWupinList();
                this.loadSelList();
            })
        },
        delAll() {
            delItemsAll(this.form.Id).then(rsp => {
                this.loadWupinList();
                this.loadSelList();
            });
        },
        onWupinWaitChange(val) {
            this.multipleWaitSelection = val;
        },
        onWupinCheckedChange(val) {
            this.multipleCheckSelection = val;
        },
        resetWupin() {
            this.wupinDateRange = [];
            this.resetForm("wupinForm");
            this.loadWupinList();
        },
        loadWupinList() {
            this.checkloading = true;
            this.wupinQuery.HouseId = this.form.HouseId;
            this.wupinQuery.InventId = this.form.Id;
            stockList(this.wupinQuery).then(
                rsp => {
                    this.wupinList = rsp.data.List;
                    this.wupinTotal = rsp.data.Total;
                    this.checkloading = false;
                }
            );
        },
        loadSelList() {
            this.checkloading = true;
            this.selQuery.Id = this.form.Id;
            invItemList(this.selQuery).then(
                rsp => {
                    this.selList = rsp.data.List;
                    this.selTotal = rsp.data.Total;
                    this.checkloading = false;
                }
            );
        },
        getFirstFocus() {
            this.$refs.selectFirst.blur();
            let info = [];
            this.form.UserList.forEach(x => {
                if (x.TimeIn == 0) {
                    info.push({ id: x.UserId, name: x.UserInfo.RealName, avatar: x.UserInfo.Avatar, type: "user" });
                }

            });
            this.$refs.firstPicker.show(info, "user");
        },
        removeFirst(val) {
            this.form.UserList = this.form.UserList.filter(x => x.TimeIn == 1 || (x.TimeIn == 0 && x.UserId != val));
        },
        onSelectFirst(values) {
            this.form.UserList = this.form.UserList.filter(x => x.TimeIn == 1);
            values.forEach((item) => {
                this.form.UserList.push({
                    TimeIn: 0,
                    UserId: item.id,
                    UserInfo: {
                        Id: item.id,
                        RealName: item.name,
                        Avatar: item.avatar
                    }
                });
            });

            this.$forceUpdate();
        },
        getCheckFocus() {
            this.$refs.selectChecker.blur();
            let info = [];
            this.form.UserList.forEach(x => {
                if (x.TimeIn == 1) {
                    info.push({ id: x.UserId, name: x.UserInfo.RealName, avatar: x.UserInfo.Avatar, type: "user" });
                }
            });
            this.$refs.checkerPicker.show(info, "user");
        },
        removeCheck(val) {
            this.form.UserList = this.form.UserList.filter(x => x.TimeIn == 0 || (x.TimeIn == 1 && x.UserId != val));
        },
        onSelectCheck(values) {
            this.form.UserList = this.form.UserList.filter(x => x.TimeIn == 0);
            values.forEach((item) => {
                this.form.UserList.push({
                    TimeIn: 1,
                    UserId: item.id,
                    UserInfo: {
                        Id: item.id,
                        RealName: item.name,
                        Avatar: item.avatar
                    }
                });
            });
            this.$forceUpdate();
        },
        handleCurrentChange(val) {
            this.form.HouseName = val.StoreName;
            this.form.HouseId = val.Id;
        },
        onClear() {
            this.form.HouseName = "";
            this.form.HouseId = null;
        },
        openHouseDialog() {
            this.$refs.houseDlg.openHouseDialog("请选择盘点仓库");
        },
        // 表单重置
        reset() {
            this.form = {
                Id: undefined,
                Name: "",
                HouseId: undefined,
                HouseName: undefined,
                UserList: [],
                StartOn: undefined,
                EndOn: undefined,
                Remark: ""
            };
        },
        openWupinDialog() {

        },
        NextForm() {
            this.$refs["form"].validate(valid => {
                if (valid) {
                    this.checkloading = true;
                    if (this.form.Id != undefined) {
                        editInv(this.form).then(response => {
                            this.$emit("reload");
                            this.loadWupinList();
                            this.loadSelList();
                            this.active = 1;
                            this.checkloading = false;
                        });

                    } else {

                        addInv(this.form).then(response => {
                            this.form.Id = response.data;
                            this.$emit("reload");
                            this.loadWupinList();
                            this.loadSelList();
                            this.active = 1;
                            this.checkloading = false;
                        });

                    }


                }
            });

        },
        PreForm() {
            this.active = 0;
        },
        /** 提交按钮 */
        submitForm() {
            if (this.selTotal <= 0) {
                this.$message.error("请添加盘点物品");
                return;
            }
            this.checkloading = true;
            submitInv(this.form.Id).then(response => {
                this.$emit("reload");
                this.active = 2;
                this.checkloading = false;
            });
        },
    }
};
</script>
<style lang="scss">
.wuping-it {
    display: flex;

    .sel {
        flex: 1;
        width: 0;
    }

    .sec {
        width: 150px;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 15px 0px;
    }

    .ser {
        flex: 1;
        width: 0;
    }
}
</style>