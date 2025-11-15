<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :close-on-click-modal="false" :show-close="false" width="500px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>获取历史采集数据</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="90px">
        <div>
            <el-row :gutter="24">
                <el-col :span="24">
                    <el-form-item label="设备类型">
                        <el-select v-model="addForm.FactorId" style="width: 100%" disabled>
                            <el-option v-for="item in factorList" :key="item.Id" :label="item.TypeName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item label="获取历史数据时间" prop="beginDate">
                        <el-date-picker class="form_input_style" v-model="createDateRange" style="width: 100%" value-format="yyyy-MM-dd" type="daterange"
                        range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期" @change="getDateRange"></el-date-picker>
                    </el-form-item>
                </el-col>
            </el-row>
        </div>
    </el-form>
    <div slot="footer" class="dialog-footer">
        <el-button @click="dialog = false">取消</el-button>
        <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { reCalculationEnergyDay } from '@/api/energy/gatherManage'
export default {
    name: 'historyData',
    props: {
        factorList: {
            type: Array,
            default: () => [],
        },
    },
    data() {
        return {
            dialog: false,
            addForm: {},
            createDateRange: [],
            addRules: {
                beginDate: [{ required: true, trigger: "blur", message: "请选择历史数据时间" }],
            },
            saveLoading: false,
        };
    },

    methods: {
        openDialog() {
            // 筛选factorList中TypeName包含“电”的设备（不区分大小写）
            const electricDevices = this.factorList.filter(item => {
                // 确保TypeName存在，且包含“电”字（如“电力”“电能表”等）
                return item.TypeName && item.TypeName.includes('电');
            });
            this.createDateRange = [];
            this.addForm = {
                OrgId: this.$store.getters.orgId,
                FactorId: electricDevices.length > 0 ?  electricDevices[0].Id : '没有符合的设备',
                beginDate: '',
                endDate: '',
            }
            this.dialog = true
        },
        closeDialog(){
            this.dialog = false;
        },
        submitForm(){
            this.$refs["addForm"].validate(valid => {
                if (valid) {
                    this.saveLoading = true;
                    this.$confirm('请确认是否覆盖原有数据？将按最新计费标准重新计算。', '提示', {
                        confirmButtonText: '确定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        this.getHistoryData();
                    }).catch(() => {
                       this.saveLoading = false;
                    });
                }
            });
        },
        // 获取历史数据
        getHistoryData() {
            reCalculationEnergyDay(this.addForm).then(response => {
                this.$message.success("获取成功");
                this.dialog = false;
                this.$emit('getList');
                this.saveLoading = false;
            }).catch(err => {
                this.saveLoading = false;
            });
        },
        // 日期范围选择器改变事件
        getDateRange() {
            this.addForm.beginDate = this.createDateRange[0];
            this.addForm.endDate = this.createDateRange[1];
        },
    },
};
</script>

<style lang="less" scoped>
.adddialog{
    ::v-deep .el-dialog__header{
        padding: 0;
        color: #ffffff;
    }
}

.dialog_title{
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    .dialog_title_left{
        font-size: 16px;
        color: #ffffff;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        padding-left: 20px;
        line-height: 16px;
        height: 56px;
        .img{
            width: 16px;
            height: 16px;
        }
        span{
            margin-left: 6px;
        }
    }
    .dialog_title_right{
        margin-right: 20px;
        cursor: pointer;
        i.zhongtaiiconfont{
            color: rgba(255, 255, 255, 0.60);
            font-size: 12px;
        }
    }
}
</style>