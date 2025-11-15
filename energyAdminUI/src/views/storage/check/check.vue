<template>
    <div>
        <el-dialog :title="title" v-loading="isloading" :close-on-click-modal="false" :visible.sync="open" width="1280px"
            append-to-body>
            <el-row :gutter="20">
                <el-col :span="10">
                    <el-form ref="form" :model="item" :rules="rules" label-width="80px">
                        <el-form-item label="物品编号" prop="DeviceNumber">
                            <el-input v-model="item.DeviceNumber" @change="iptChange">
                                <el-button slot="append" type="primary">
                                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                    <span style="margin-left:6px">扫码添加</span>
                                </el-button>
                            </el-input>
                        </el-form-item>
                        <el-form-item label="物品数量" prop="Num">
                            <el-input v-model="item.Num" placeholder="请输入物品数量" style="width:250px;"></el-input>
                        </el-form-item>
                        <el-form-item style="text-align: right;">
                            <el-button type="primary" @click="onSubmit">确认</el-button>
                        </el-form-item>
                    </el-form>

                    <el-divider></el-divider>

                    <el-descriptions v-if="item.Id != null" class="margin-top" :column="2" border>
                        <el-descriptions-item>
                            <template slot="label">
                                物品名称
                            </template>
                            {{ item.Name }}
                        </el-descriptions-item>
                        <el-descriptions-item>
                            <template slot="label">
                                存储类型
                            </template>
                            {{ item.TargetType == 1 ? '设备' : '耗材' }}
                        </el-descriptions-item>
                    </el-descriptions>
                </el-col>
                <el-col :span="12" :offset="2">
                    <div class="pdtitle">
                        <span style="font-size: 16px;font-weight: bold;color: #409EFF;">待盘点清单</span>
                        <div style="display: flex;align-items: center;">
                            <span style="color:#999;">盘点进度：</span>
                            <div style="width: 150px;text-align: right;">
                                <el-progress :percentage="parseInt((AllTotal - total) * 100.0 / AllTotal)"></el-progress>
                            </div>

                        </div>
                    </div>
                    <el-table :data="tableData" border style="width: 100%">
                        <el-table-column prop="DeviceNumber" label="物品编号" width="180">
                        </el-table-column>
                        <el-table-column prop="Name" label="物品名称" width="180">
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
                        <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
                            <template slot-scope="scope">
                                <div>
                                    {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
                                </div>
                            </template>
                        </el-table-column>
                    </el-table>
                    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                        :limit.sync="queryParams.pageSize" @pagination="getList" />
                </el-col>
            </el-row>
        </el-dialog>
    </div>
</template>
      
<script>
import { invItemList, invInfo, confirmItem, itemInfo } from "@/api/storage/inventory";
export default {
    data() {
        return {
            isloading: false,
            open: false,
            title: "",
            form: {},
            item: {
                Id: undefined,
                DeviceNumber: "",
                Num: 0,
                Name: "",
                TargetType: 0,
            },
            tableData: [],
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 10,
            },
            // 表单校验
            rules: {
                DeviceNumber: [
                    { required: true, message: "物品编号不能为空", trigger: "change" }
                ],
                Num: [
                    { required: true, message: "物品数量不能为空", trigger: "change" }
                ]
            },
            // 总条数
            total: 0,
            AllTotal: 1,
        };
    },
    methods: {
        async openDlg(id) {
            this.open = true;
            this.isloading = true;
            this.resetItem();
            let invRsp = await invInfo(id);
            this.title = "'" + invRsp.data.Name + "'的盘点任务";
            this.form = invRsp.data;
            await this.getList();
            this.isloading = false;
        },
        async getList() {
            let allRsp = await invItemList({ Id: this.form.Id });
            this.AllTotal = allRsp.data.Total;
            this.queryParams.Id = this.form.Id;
            if (this.form.Status == 2) {
                this.queryParams.UnFirst = true;
            }
            else if (this.form.Status == 3) {
                this.queryParams.UnCheck = true;
            }
            else {
                return;
            }
            let itemRsp = await invItemList(this.queryParams);
            this.tableData = itemRsp.data.List;
            this.total = itemRsp.data.Total;
        },
        async onSubmit() {
            let valiRsp = await this.$refs["form"].validate();
            if (valiRsp) {
                let cc = parseInt(eval(this.item.Num));
                await confirmItem(this.form.Id, this.item.DeviceNumber, cc);

                this.$message.success("提交成功！开始盘点下一个物品");
                this.getList();
                this.resetItem();
            }

        },
        resetItem() {
            this.item = {
                Id: undefined,
                DeviceNumber: "",
                Num: 0,
                Name: ""
            }
        },
        async iptChange() {
            if (this.item.DeviceNumber != "") {
                let xx = await itemInfo(this.form.Id, this.item.DeviceNumber);
                if (xx.data == null) {
                    this.resetItem();
                    this.$message.error("物品不存在");
                    return;
                }
                this.item.Id = xx.data.Id;
                this.item.DeviceNumber = xx.data.DeviceNumber;
                this.item.Name = xx.data.Name;
            }

        }
    }
};
</script>
<style lang="scss">
.pdtitle {
    padding-bottom: 15px;
    display: flex;
    justify-content: space-between;
    background-color: #f9fafc;
    padding: 10px 20px;
    border-top: 1px solid #EBEEF5;
    border-left: 1px solid #EBEEF5;
    border-right: 1px solid #EBEEF5;
}
</style>