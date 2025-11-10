<template>
    <div>
        <el-dialog title="扫码入库" :visible.sync="dialogFlag" :close-on-click-modal="false" width="800px" top="2vh"
            @close="cancel">
            <div class="base-title">基本信息</div>
            <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="80px" class="addPeople">
                <el-form-item label="入库单号">
                    <el-input v-model.trim="ruleForm.StockNumber" placeholder="请输入入库单号" />
                </el-form-item>
                <el-form-item label="入库时间" prop="InDate">
                    <el-date-picker v-model="ruleForm.InDate" type="datetime" placeholder="选择日期时间" />
                </el-form-item>
                <el-form-item label="目标仓库" prop="HouseName">
                    <el-input class="houseipt" v-model="ruleForm.HouseName" readonly placeholder="请选择目标仓库"
                        @focus="openHouseDialog">
                        <i slot="suffix" @click="onClear" v-if="ruleForm.HouseId != null" class="el-icon-circle-close"
                            style=" vertical-align: middle; font-size: 22px; cursor: pointer; " />
                    </el-input>
                </el-form-item>
                <el-form-item label="备注说明" prop="Remark">
                    <el-input type="textarea" :rows="2" v-model.trim="ruleForm.Remark" placeholder="请输入备注说明" />
                </el-form-item>
            </el-form>
            <div class="base-title">
                <div>
                    入库物品
                <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
                <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
                </div>
                <div>
                    <el-input v-model.trim="scanOrderId" placeholder="请输入单号" @keyup.enter.native="scanOrderClick" />
                    <el-button style="margin-left: 20px;" type="warning" @click="scanOrderClick">添加</el-button>
                </div>
            </div>
            <el-table :data="ruleForm.List" stripe>
                <el-table-column prop="DeviceNumber" label="物品编号" />
                <el-table-column prop="Name" label="物品名称" />
                <el-table-column label="数量">
                    <template slot-scope="scope">
                        <span v-if="scope.row.TargetType == 1">{{ scope.row.Quantity }}</span>
                        <el-input-number v-else v-model="scope.row.Quantity" :min="1" :max="9999" size="mini" />
                    </template>
                </el-table-column>
                <el-table-column label="金额（元）">
                    <template slot-scope="scope">
                        <el-input-number v-model="scope.row.Price" :precision="2" size="mini" :min="0" />
                    </template>
                </el-table-column>
                <el-table-column label="操作" align="center">
                    <template slot-scope="scope">
                        <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;"
                            @click="ruleForm.List.splice(scope.$index, 1)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <span slot="footer" class="dialog-footer">
                <el-button type="primary" @click="submitForm('ruleForm')">提交</el-button>
                <el-button @click="cancel">取消</el-button>
            </span>
        </el-dialog>
        <HouseSelecter ref="houseDlg" @ok="handleCurrentChange"></HouseSelecter>
    </div>
</template>

<script>
import { houseList } from "@/api/storage/house";
import {
    scanEnterPile,
    generateRKNumber,
    getInStockByKey
} from "@/api/storage/stock";
import HouseSelecter from "../house/HouseSelecter.vue";
export default {
    name: 'addScanPut',
    components: { HouseSelecter },
    data() {
        return {
            dialogFlag: false,
            // 表单
            ruleForm: {},
            enterId: null,
            // 校验
            rules: {
                HouseName: [
                    { required: true, message: "目标仓库不能为空", trigger: "change" },
                ],
                InDate: [
                    { required: true, message: "入库时间不能为空", trigger: "change" },
                ],
            },
            scanOrderId: '', // 用于扫码接口调用
        }
    },
    computed:{
        finishedTotal(){
        let total=0
        if(this.ruleForm&&this.ruleForm.List){
            this.ruleForm.List.map(ro=>{
                if(ro.TargetType==1){
                total=total+Number(ro.Quantity)
                }
            })
        }
        return total
        },
        useTotal(){
        let total=0
        if(this.ruleForm&&this.ruleForm.List){
            this.ruleForm.List.map(ro=>{
                if(ro.TargetType==0){
                total=total+Number(ro.Quantity)
                }
            })
        }
        return total
        }
    },
    methods: {
        submitForm(formName) {
            this.$refs[formName].validate(async (valid) => {
                if (valid) {
                    if (this.ruleForm.List.length === 0) {
                        this.$message.error("请选择要入库的物品");
                        return;
                    }
                    await scanEnterPile(this.ruleForm);
                    this.$modal.msgSuccess("操作成功");
                    this.dialogFlag = false;
                } else {
                    return false
                }
            })
        },
        async openDialog() {
            this.dialogFlag = true;
            let RKNumber = await generateRKNumber();
            let hslist = (await houseList({ IsSystem: true })).data.List;
            this.ruleForm = {
                StockNumber: RKNumber.data,
                HouseName: hslist.length > 0 ? hslist[0].StoreName : '',
                HouseId: hslist.length > 0 ? hslist[0].Id : undefined,
                InDate: this.parseTime(Date.now()),
                Remark: '',
                List: []
            }
        },
        openHouseDialog() {
            this.$refs.houseDlg.openHouseDialog("请选择目标仓库");
        },
        async handleCurrentChange(val) {
            this.ruleForm.HouseName = val.StoreName;
            this.ruleForm.HouseId = val.Id;
        },
        scanOrderClick() {
            if (this.ruleForm.List.some(x => x.DeviceNumber == this.scanOrderId)) {
                this.$message.error('请不要重复添加!');
                return;
            }
            if (this.scanOrderId === '') {
                this.$message.error('请输入单号!');
                return;
            }
            getInStockByKey({ key: this.scanOrderId }).then(res => {
                this.ruleForm.List.push(res.data);
                this.scanOrderId = '';
            })
        },
        onClear() {
            this.ruleForm.HouseName = "";
            this.ruleForm.HouseId = null;
        },
        cancel() {
            this.dialogFlag = false
        }
    }
}
</script>

<style lang="scss" scoped>
.num_li{
    font-weight: normal;
    font-size: 14px;
    color: #666666;
    margin-left: 10px;
}
::v-deep {
    .el-dialog__header {
        border-bottom: 1px solid #ccc;
    }

    .el-date-editor {
        width: 100%;
    }
}

.base-title {
    font-size: 16px;
    color: #333;
    background-color: rgb(249, 250, 252);
    padding: 0 15px;
    height: 48px;
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.base-title>div {
    display: flex;
    align-items: center;
}
</style>