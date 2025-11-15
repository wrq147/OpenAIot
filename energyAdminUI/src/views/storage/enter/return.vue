<template>
    <div class="returnpage">
        <el-dialog title="入库物品退货" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1050px"
            top="2vh">
            <el-form ref="form" :model="form" :rules="rules" label-width="120px">
                <div class="base-title">基本信息</div>
                <el-row>
                    <el-col :span="12">
                        <el-form-item label="出库单号" prop="StockNumber">
                            <el-input v-model="form.StockNumber" :readonly="true"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="退货时间" prop="OutDate">
                            <el-date-picker v-model="form.OutDate" type="datetime" placeholder="选择日期时间">
                            </el-date-picker>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="16">
                        <el-form-item label="物流单号" prop="ExpressNumber">
                            <el-input placeholder="请输入物流单号" v-model="form.ExpressNumber" @change="expressSelected">
                                <el-select v-if="form.ExpressNumber != ''" style="width:130px" v-model="form.ExpressCompany"
                                    slot="prepend" placeholder="请选择物流公司">
                                    <el-option v-for="kditem in kdcompanys" :key="kditem.Code" :label="kditem.Name"
                                        :value="kditem.Code"></el-option>
                                </el-select>
                            </el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row v-if="form.ExpressCompany == 'shunfeng'">
                    <el-col :span="16">
                        <el-form-item label="联系电话" prop="ExpressPhone">
                            <el-input placeholder="请输入物流单上的联系电话" type="tel" v-model="form.ExpressPhone"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="备注说明" prop="Remark">
                            <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="form.Remark"
                                style="width:80%"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>

            <div>
                <div class="items-title">
                    <div style="font-size:16px;color:#333;">
                        退货物品
                        <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
                        <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
                    </div>
                </div>
                <el-table :data="form.List" stripe style="width: 100%">
                    <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
                    </el-table-column>
                    <el-table-column prop="TargetName" label="物品名称">
                    </el-table-column>
                    <el-table-column label="存储类型" align="center" width="100">
                            <template slot-scope="scope">
                                <span v-if="scope.row.TargetType==1">成品</span>
                                <span v-else-if="scope.row.TargetType==0">半成品</span>
                            </template>
                        </el-table-column>
                    <el-table-column label="预览图片" align="center" width="150">
                        <template slot-scope="scope">
                            <div class="imgwrap">
                                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'"
                                    :preview-src-list="[scope.row.PhotoUrl]">
                                </el-image>
                            </div>
                        </template>
                    </el-table-column>
                    <el-table-column prop="Quantity" align="center" label="数量" width="140">
                    </el-table-column>
                    <el-table-column prop="Price" align="center" label="单价（元）" width="140">
                    </el-table-column>
                </el-table>
            </div>

            <div style="margin-top:10px;">
                <AddEmbed ref="flowForm">
                    <div class="flow-title" style="font-size:16px;color:#333;">出库审核</div>
                </AddEmbed>
            </div>

            <div slot="footer" class="dialog-footer">
                <el-button plain type="primary" @click="submitForm()">提 交</el-button>
                <el-button plain @click="open = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>
      
<script>
import {
    getEnterInfo,
    generateCKNumber,
    cancelEnterModel,
    leaveFormData
} from "@/api/storage/stock";
import {
    autoCompany
} from '@/api/code.js'

import AddEmbed from "@/views/flowable/task/record/AddEmbed";
export default {
    name: "EnterReturn",
    components: { AddEmbed },
    data() {
        return {
            kdcompanys: [],
            allloading: false,
            open: false,
            form: {
                Id: undefined,
                StockNumber: "",
                OutDate: undefined,
                Remark: "",
                ExpressNumber: "",
                ExpressCompany: "",
                ExpressPhone: "",
                List: []
            },
            // 表单校验
            rules: {
                OutDate: [
                    { required: true, message: "退货时间不能为空", trigger: "change" }
                ]
            },
            leaveTemplateId: 0,
        };
    },
    computed: {
        FlowParams: function () {
            return {
                "@from": this.form.StockNumber,
                "@fromtype": "出库单",
            };
        },
        finishedTotal(){
            let total=0
            if(this.form&&this.form.List){
                this.form.List.map(ro=>{
                    if(ro.TargetType==1){
                    total=total+Number(ro.Quantity)
                    }
                })
            }
            return total
        },
        useTotal(){
            let total=0
            if(this.form&&this.form.List){
                this.form.List.map(ro=>{
                    if(ro.TargetType==0){
                    total=total+Number(ro.Quantity)
                    }
                })
            }
            return total
        }
    },
    methods: {
        async openDialog(id) {
            this.open = true;
            this.allloading = true;
            let ssp = await generateCKNumber();
            this.form.StockNumber = ssp.data;
            this.form.Id = id;
            this.form.OutDate = this.parseTime(Date.now());
            let enterInfo = await getEnterInfo(id);
            this.form.List = enterInfo.data.List;
            this.kdcompanys = await this.$store.dispatch("datas/kuaiDiList");

            this.leaveTemplateId = enterInfo.data.FromHouseLeaveTemplateId;
            if (this.leaveTemplateId > 0) {
                let fromInfo=await leaveFormData({
                "applyNumber": this.form.StockNumber,
                "fromHouseId": enterInfo.data.ToHouseId,
                })
                await this.$refs.flowForm.InitData(
                this.leaveTemplateId,
                this.FlowParams,
                this.leaveId == null ? null : this.form.StockNumber,
                fromInfo.data
                );
                // await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, null);
            }


            this.allloading = false;
        },
        async expressSelected(val) {
            if (val.length < 8) return;
            this.form.ExpressCompany = (await autoCompany(val)).data;
        },
        submitForm() {
            this.$refs["form"].validate(valid => {
                if (valid) {
                    let tmpmodel = this.$refs.flowForm.getModel();
                    let mergedObj1 = Object.assign({}, tmpmodel, this.form);
                    cancelEnterModel(mergedObj1).then(response => {
                        this.$modal.msgSuccess("操作成功");
                        this.open = false;
                        this.$emit("confirm");
                    });

                }
            });
        }
    }
};
</script>
<style lang="scss">
.num_li{
    font-weight: normal;
    font-size: 14px;
    color: #666666;
    margin-left: 10px;
}
.imgwrap {
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;

    .el-image {
        display: flex;
        width: 80px;
        height: 80px;
        justify-content: center;
        align-items: center;
    }
}

.returnpage {
    padding: 20px 20px 0 20px;
    height: 100%;

    .el-table .el-table__header-wrapper th {
        background: none;
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
        margin-bottom: 20px;
    }

    .items-title,
    .flow-title {
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: space-between;
        height: 48px;
        background-color: rgb(249, 250, 252);
        padding: 0 15px;
        margin-bottom: 5px;
    }
}
</style>