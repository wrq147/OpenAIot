<template>
    <div>
        <el-dialog title="库存差异修正" v-loading="dialogLoading" :close-on-click-modal="false" :visible.sync="open"
            width="1280px" append-to-body>
            <el-descriptions class="margin-top" title="盘点信息" :column="3" border>
                <el-descriptions-item>
                    <template slot="label">
                        盘点单号
                    </template>
                    {{ form.Id }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">
                        初盘人员
                    </template>
                    {{ FirstUserName }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">
                        复盘人员
                    </template>
                    {{ CheckUserName }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">
                        仓库
                    </template>
                    {{ form.House != null ? form.House.StoreName : "" }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template slot="label">
                        备注
                    </template>
                    {{ form.Remark }}
                </el-descriptions-item>
            </el-descriptions>
            <div class="inntitle">盘点差异明细</div>
            <el-table ref="wupinTable" :data="wupinList" tooltip-effect="dark" style="width: 100%">
                <el-table-column type="index" width="50">
                </el-table-column>
                <el-table-column prop="DeviceNumber" label="物品编号" align="center" width="150"></el-table-column>
                <el-table-column prop="Name" label="物品名称"></el-table-column>
                <el-table-column label="预览图片" align="center" width="150">
                    <template slot-scope="scope">
                        <div class="imgwrap">
                            <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]">
                            </el-image>
                        </div>
                    </template>
                </el-table-column>
                <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
                    <template slot-scope="scope">
                        <div>
                            {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
                        </div>
                    </template>
                </el-table-column>
                <el-table-column align="center" label="差异数量" width="140">
                    <template slot-scope="scope">
                        <div v-html="DiffNumber(scope.row)"></div>
                    </template>
                </el-table-column>
                <el-table-column align="center" label="库存数量" width="140">
                    <template slot-scope="scope">
                        <span>{{ scope.row.SnapQuantity }}</span>
                    </template>
                </el-table-column>
                <el-table-column align="center" label="初盘数量" width="140">
                    <template slot-scope="scope">
                        <span>{{ scope.row.FirstCount }}</span>
                    </template>
                </el-table-column>
                <el-table-column align="center" label="复盘数量" width="140">
                    <template slot-scope="scope">
                        <span>{{ scope.row.CheckCount }}</span>
                    </template>
                </el-table-column>
                <el-table-column align="center" label="最终数量" width="160" fixed="right">
                    <template slot-scope="scope">
                        <el-input-number v-if="scope.row.TargetType == 1" v-model="scope.row.Count" :min="0" :max="1"
                            style="width:140px;"></el-input-number>
                        <el-input-number v-else v-model="scope.row.Count" :min="0" :max="999999"
                            style="width:140px;"></el-input-number>
                    </template>
                </el-table-column>
            </el-table>
            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="submitForm()">修 正</el-button>
                <el-button @click="open = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>
      
<script>
import { invInfo, invItemList, repairInv } from "@/api/storage/inventory";

export default {
    components: {},
    data() {
        return {
            open: false,
            dialogLoading: false,
            form: {},
            wupinList: [],
        };
    },
    computed: {
        FirstUserName: function () {
            if (this.form.UserList == null) return [];
            let uss = this.form.UserList.filter(x => x.TimeIn == 0);
            return uss.map(x => {
                return x.UserInfo.RealName;
            }).join();
        },
        CheckUserName: function () {
            if (this.form.UserList == null) return [];
            let uss = this.form.UserList.filter(x => x.TimeIn == 1);
            return uss.map(x => {
                return x.UserInfo.RealName;
            }).join();
        },
    },
    methods: {
        async openDlg(id) {
            this.open = true;
            this.dialogLoading = true;
            this.form = (await invInfo(id)).data;
            await this.loadWupinList();
            this.dialogLoading = false;
        },
        async loadWupinList() {
            this.dialogLoading = true;
            let rsp = await invItemList({ Id: this.form.Id, OnlyRevise: true, showAll: true });
            rsp.data.List.forEach(element => {
                if (element.Count == null) {
                    if (element.CheckUserId == 0 && element.FirstUserId == 0) {
                        element.Count = element.SnapQuantity;
                    }
                    else {
                        if (element.CheckUserId != 0) {
                            element.Count = element.CheckCount;
                        }
                        else {
                            element.Count = element.FirstCount;
                        }
                    }
                }
            });
            this.wupinList = rsp.data.List;
            this.dialogLoading = false;
        },
        DiffNumber(row) {
            let diffnum = 0;
            if (row.CheckUserId == 0) {
                if (row.FirstUserId == 0) {
                    return '<span style="color:#c40001;">未盘点</span>';
                }
                diffnum = row.FirstCount - row.Quantity;
            }
            else {
                diffnum = row.CheckCount - row.Quantity;
            }

            if (diffnum == 0) {
                return "0";
            }
            else if (diffnum > 0) {
                return '<span style="color:#67c23a;">+' + diffnum + '</span>';
            }
            else {
                return '<span style="color:#c40001;">' + diffnum + '</span>';
            }
        },
        submitForm() {
            this.$modal
          .confirm('是否确认修正库存？')
          .then(() => {
            return repairInv(this.form.Id, this.wupinList);
          })
          .then((rsp) => {
            this.$modal.msgSuccess("操作成功");
            this.$emit("reload");
            this.open = false;
          })
          .catch(() => { });

        }
    }
};
</script>
<style lang="scss">
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

.inntitle {
    font-size: 16px;
    font-weight: bold;
    padding-top: 25px;
    padding-bottom: 20px;
}
</style>