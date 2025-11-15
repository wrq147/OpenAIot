<template>
    <div style="width: 100%;border:solid 1px #dadada;padding-bottom: 15px;background-color: #fafafa;">
        <el-form ref="matchesForm" :model="matchForm" label-position="top" :inline="true" class="matches_form">
            <el-form-item label="从机地址" prop="SlaveId">
                <el-input-number v-model="matchForm.SlaveId" style="width:130px;" controls-position="right"
                    placeholder="请输入从机地址"></el-input-number>
            </el-form-item>
            <el-form-item label="功能码" prop="FuncCode">
                <el-select v-model="matchForm.FuncCode" style="width:180px;" @change="onCodeChange" placeholder="请输入功能码">
                    <el-option v-for="item in FuncCodeList" :key="item.value" :label="item.label"
                        :value="item.value"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="起始地址" prop="StartAddress">
                <el-input-number v-model="matchForm.StartAddress" style="width:130px;" controls-position="right"
                    placeholder="请输入起始地址"></el-input-number>
            </el-form-item>
        </el-form>
        <div class="flex_con">
            <div style="padding-left:20px;color:#97999C;font-size: 14px;margin-right:20px">写入数据</div>
            <el-checkbox v-model="waitreturn">是否等待返回</el-checkbox>
        </div>
        <template>
            <el-form v-for="(ite, inx) in this.matchForm.Items" :key="inx" ref="matchesItemsForm" :model="ite"
                label-position="top" :inline="true" class="matches_form2">
                <el-form-item :label="inx == 0 ? '字节序' : ''" prop="ByteOrder">
                    <el-select v-model="ite.ByteOrder" :disabled="matchForm.FuncCode == 5 || matchForm.FuncCode == 15"
                        style="width:100px" placeholder="请选择字节序">
                        <el-option v-for="item in ByteOrderList" :key="item.value" :label="item.label"
                        v-show="(item.value!='CDAB'&&item.value!='BADC')||ite.NumRegister>3"
                            :value="item.value"></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item :label="inx == 0 ? '数据长度' : ''" prop="NumRegister">
                    <template v-if="matchForm.FuncCode == 5 || matchForm.FuncCode == 15">
                        <el-input-number v-model="ite.NumRegister" controls-position="right" style="width:100px"
                            :disabled="true"></el-input-number>
                    </template>
                    <template v-else-if="ite.ByteOrder == 'C'">
                        <el-input-number v-model="ite.NumRegister" controls-position="right" style="width:100px" :min="1"
                            :max="255"></el-input-number>
                    </template>
                    <template v-else>
                        <el-select v-model="ite.NumRegister" style="width:100px" placeholder="请选择数据长度">
                            <el-option v-for="item in NumRegisterList" :key="item.value" :label="item.label"
                                :value="item.value"></el-option>
                        </el-select>
                    </template>

                </el-form-item>
                <el-form-item :label="inx == 0 ? '输入参数(线圈仅布尔型)' : ''" prop="PropertyCode">
                    <el-select v-model="ite.PropertyCode" style="width:160px" clearable placeholder="请选择对应的输入参数">
                        <el-option v-for="item in InputParamsList" :key="item.code" :label="item.name"
                            :value="item.code"></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item :label="inx == 0 ? '　　　' : ''" v-if="matchForm.FuncCode == 15 || matchForm.FuncCode == 16">
                    <el-button plain @click="delItemsFormList(ite, inx)">移除</el-button>
                </el-form-item>
            </el-form>
            <div style="margin-left:20px;margin-top:10px" v-if="matchForm.FuncCode == 15 || matchForm.FuncCode == 16">
                <el-button plain @click="addItemsFormList">+添加</el-button>
            </div>
        </template>

    </div>
</template>
  
<script>
export default {
    name: "FunModbus",
    props: {
        FunItem: {
            type: Object,
            default: () => {
                return {};
            }
        }
    },
    data() {
        return {
            matchForm: {
                SlaveId: 1,
                FuncCode: 16,
                StartAddress: 0,
                Items: [{
                    ByteOrder: "H",
                    NumRegister: "2",
                    PropertyCode: ""
                }]
            },
            waitreturn:false,//是否等待返回
            //功能码列表
            FuncCodeList: [
                { label: "05写单个线圈寄存器", value: 5 },
                { label: "06写单个保持寄存器", value: 6 },
                { label: "0F写多个线圈寄存器", value: 15 },
                { label: "10写多个保持寄存器", value: 16 }
            ],
            ByteOrderList: [
                { label: "大端", value: "H" },
                { label: "小端", value: "L" },
                { label: "字节", value: "C" },
                { label: "CDAB", value: "CDAB" },
                { label: "BADC", value: "BADC" },
            ],
            NumRegisterList: [
                { label: "8位", value: "1" },
                { label: "16位", value: "2" },
                { label: "32位", value: "4" }
            ],
        };
    },
    computed: {
        InputParamsList() {
            if (this.FunItem.inputs == null) return [];
            if (this.matchForm.FuncCode == 5 || this.matchForm.FuncCode == 15) {
                return this.FunItem.inputs.filter(item => item.type == "boolean");
            }
            return this.FunItem.inputs;
        }
    },
    mounted() {
        if (this.FunItem.downdata != null && this.FunItem.downdata != "") {
            try {
                let obj = JSON.parse(this.FunItem.downdata);
                this.matchForm = obj;
            }
            catch { }
        }
        if (this.FunItem.waitreturn != null && this.FunItem.waitreturn != ""&& this.FunItem.waitreturn != undefined) {
            try {
                this.waitreturn=this.FunItem.waitreturn
            }
            catch { }
        }
    },
    methods: {
        setInputData() {
            this.FunItem.downdata = JSON.stringify(this.matchForm);
            this.FunItem.waitreturn = this.waitreturn
        },
        onCodeChange() {
            this.matchForm.Items = [{
                ByteOrder: "H",
                NumRegister: "2",
                PropertyCode: ""
            }];
        },
        delItemsFormList(row, indexRow) {
            //删除提取规则增加
            this.matchForm.Items.splice(indexRow, 1);
        },
        addItemsFormList() {
            //数据提取规则增加
            this.matchForm.Items.push({
                ByteOrder: "H", //字节序
                NumRegister: "2", //数据长度
                PropertyCode: "" //对应的属性标识符
            });
        },
    }
};
</script>
  
<style lang="less" scoped>
.matches_form {

    .el-form-item.is-required:not(.is-no-asterisk)>.el-form-item__label:before,
    .el-form-item.is-required:not(.is-no-asterisk) .el-form-item__label-wrap>.el-form-item__label:before {
        content: "";
    }

    .el-form-item .el-form-item__label {
        padding: 0;
    }
    
}
.flex_con{
    display: flex;
    justify-content: flex-start;
    align-items: center;
}
</style>