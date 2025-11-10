<template>
    <div>
        <el-dialog width="960px" title="配置告警" :visible.sync="isWarnOpen" append-to-body>
            <div style="background-color: #fff;padding:20px 20px;">
                <div class="box-row">
                    <div class="bx-hd">告警工单流程</div>
                    <div class="bx-bd">
                        <el-input style="width:420px;" v-model="form.WarnFlowName" readonly placeholder="请选择告警工单的处理流程模板"
                            @focus="openFlowPicker()">
                            <i slot="suffix" @click="onClearOut" v-if="form.WarnFlowName != ''"
                                class="el-icon-circle-close" style="font-size: 22px;cursor: pointer;"></i>
                        </el-input>
                    </div>
                </div>
                <div class="box-row" v-show="form.WarnFlowName != ''">
                    <div class="bx-hd">流程表单初始化</div>
                    <div class="bx-bd">
                        <el-table border v-loading="tbloading" :data="formInit" row-key="id" style="width:100%">
                            <el-table-column prop="title" label="表单字段" align="center" width="300"></el-table-column>
                            <el-table-column label="值类型" align="center" width="120">
                                <template slot-scope="scope">
                                    <el-select v-model="scope.row.way" placeholder="请选择" @change="scope.row.val = ''">
                                        <el-option label="自定义" :value="0"></el-option>
                                        <el-option label="系统值" :value="1"></el-option>
                                    </el-select>
                                </template>
                            </el-table-column>
                            <el-table-column label="初始值" align="center">
                                <template v-slot:default="scope">
                                    <el-input v-if="scope.row.way == 0" v-model="scope.row.val"
                                        placeholder="请输入内容"></el-input>
                                    <el-select v-else v-model="scope.row.val" placeholder="请选择">
                                        <el-option label="告警名称" value="告警名称"></el-option>
                                        <el-option label="告警描述" value="告警描述"></el-option>
                                        <el-option label="告警级别" value="告警级别"></el-option>
                                        <el-option label="设备来源" value="设备来源"></el-option>
                                        <el-option label="设备拥有者" value="设备拥有者"></el-option>
                                        <el-option label="拥有者地址" value="拥有者地址"></el-option>
                                        <el-option label="使用者地址" value="使用者地址"></el-option>
                                    </el-select>
                                </template>
                            </el-table-column>
                        </el-table>
                    </div>
                </div>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button @click="isWarnOpen = false">取 消</el-button>
                <el-button type="primary" @click="onSubmit">确 定</el-button>
            </div>
            <FlowPicker ref="flowPicker" @selected="onSelected"></FlowPicker>
        </el-dialog>

    </div>
</template>

<script>
import FlowPicker from "@/views/flowable/common/FlowPicker.vue";
import { getFormDetail } from "@/api/flowable/design";
import { getItems } from "@/views/flowable/common/utlity.js"
import { saveWarnConfig, getWarnConfig } from "@/api/rules/productModel.js";
export default {
    name: 'warningConfig',
    components: { FlowPicker },
    data() {
        return {
            form: {
                ProductId: null,
                WarnFlowId: 0,
                WarnFlowName: "",
                WarnFlowInitJson: ""
            },
            formInit: [],
            tbloading: false,
            isWarnOpen: false
        };
    },
    methods: {
        async openDlg(pid) {
            let res = await getWarnConfig(pid);
            this.form.ProductId = res.data.ProductId;
            this.form.WarnFlowId = res.data.WarnFlowId;
            this.form.WarnFlowName = res.data.WarnFlowName;
            this.form.WarnFlowInitJson = res.data.WarnFlowInitJson;
            this.isWarnOpen = true;
        },
        openFlowPicker() {
            this.$refs.flowPicker.OpenDialog();
        },
        async onSelected(item) {
            this.tbloading = true;
            this.form.WarnFlowId = item.Id;
            this.form.WarnFlowName = item.Name;
            let rsp = await getFormDetail(item.Id);
            let tformItems = JSON.parse(rsp.data.Form.FormFields);
            let newformItems = getItems(tformItems);
            this.formInit.length = 0;
            newformItems.forEach(element => {
                if (element.name == "TextInput" || element.name == "TextareaInput") {
                    this.formInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": "" });
                }
            });
            this.tbloading = false;
        },
        onClearOut() {
            this.form.WarnFlowId = 0;
            this.form.WarnFlowName = "";
            this.$set(this, "formInit", []);
        },
        async onSubmit() {
            try {
                for (let i = 0; i < this.formInit.length; i++) {
                    if (this.formInit[i].way == 1 && this.formInit[i].val == "") {
                        this.$modal.msgError(this.formInit[i].title + "未选择初始值");
                        return;
                    }
                }
                this.form.WarnFlowInitJson = JSON.stringify(this.formInit);
                await saveWarnConfig(this.form);
                this.$modal.msgSuccess("保存成功");
                this.isWarnOpen = false;
            }
            catch (err) {
                this.$message.error(err);
            }
        }
    }
}
</script>
<style lang="scss" scope>
.box-row {
    .bx-hd {
        font-size: 16px;
        color: #333;
        background-color: rgb(249, 250, 252);
        padding: 0 15px;
        height: 48px;
        display: flex;
        justify-content: space-between;
        flex-direction: row;
        align-items: center;
    }

    .bx-bd {
        padding: 15px 0px;

        .el-input__suffix {
            display: flex;
            align-items: center;
        }
    }
}
</style>