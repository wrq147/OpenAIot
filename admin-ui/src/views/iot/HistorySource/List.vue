<template>
    <div style="padding: 20px 20px 0 20px" id="big_con">
        <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item prop="Key" label="关键字">
                    <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable />
                </el-form-item>

                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
            </el-form>
        </div>
        <el-row :gutter="20">
            <el-col :span="24" :xs="24">
                <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
                    <el-row :gutter="10" class="mb8 button_row">
                        <el-col :span="1.5">
                            <el-button type="primary" plain @click="createdSource">
                                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                <span style="margin-left: 6px">创建存储数据源</span>
                            </el-button>
                        </el-col>
                    </el-row>
                    <el-table v-loading="loading" border :data="dateTableList" :row-style="isRed"
                        @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty"
                        style="width: 100%">
                        <el-table-column label="数据源名称" align="center" key="Name" prop="Name"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="数据源类型" align="center" key="StorageType" prop="StorageType" />
                        <el-table-column label="备注" align="center" key="Remark" prop="Remark"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                            <template slot-scope="scope">
                                <el-button type="text" icon="el-icon-edit"
                                    @click="handleUpdate(scope.row)">修改</el-button>
                                <el-button type="text" icon="el-icon-delete"
                                    @click="handleDelete(scope.row)">删除</el-button>
                                <el-button type="text" icon="el-icon-close"
                                    @click="handleDelHis(scope.row)">清除数据</el-button>
                            </template>
                        </el-table-column>
                    </el-table>

                    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                        :limit.sync="queryParams.pageSize" @pagination="getList" />
                </div>
            </el-col>
        </el-row>
        <!-- 添加或修改数据源 -->
        <el-dialog title="添加存储数据源" :close-on-click-modal="false" :visible.sync="open" width="650px" append-to-body>
            <el-form ref="form" :model="form" label-width="120px">
                <el-form-item label="数据源名称" prop="Name">
                    <el-input style="width:260px" v-model="form.Name" placeholder="请输入数据源名称"></el-input>
                </el-form-item>
                <el-form-item label="数据源类型" prop="StorageType">
                    <el-radio-group v-model="form.StorageType">
                        <el-radio label="influx">influx</el-radio>
                    </el-radio-group>
                </el-form-item>

                <div v-if="form.StorageType == 'influx'">
                    <el-form-item label="存储组织" prop="org">
                        <el-input type="text" style="width:400px" v-model="DBConfig.org"
                            placeholder="请输入存储组织"></el-input>
                    </el-form-item>
                    <el-form-item label="连接的url" prop="url">
                        <el-input type="text" style="width:400px" v-model="DBConfig.url"
                            placeholder="请输入连接的url"></el-input>
                    </el-form-item>
                    <el-form-item label="连接令牌" prop="token">
                        <el-input type="text" style="width:400px" v-model="DBConfig.token"
                            placeholder="请输入连接令牌"></el-input>
                    </el-form-item>
                    <el-form-item label="数据库名(bucket)" prop="bucket">
                        <el-input type="text" style="width:400px" v-model="DBConfig.bucket"
                            placeholder="请输入存储的数据库名"></el-input>
                    </el-form-item>
                </div>
                <el-form-item label="备注" prop="Remark">
                    <el-input style="width:370px" type="textarea" :rows="2" v-model="form.Remark"
                        placeholder="请输入备注"></el-input>
                </el-form-item>
            </el-form>

            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="submitForm">确 定</el-button>
                <el-button @click="cancel">取 消</el-button>
            </div>
        </el-dialog>
        <el-dialog title="执行删除历史数据" :visible.sync="deldlgvis" width="600px">
            <el-form ref="form" :model="delform" label-width="80px">
                <el-form-item label="时间范围">
                    <el-date-picker v-model="deldateRange" :picker-options="pickerOptions"
                        value-format="yyyy-MM-dd HH:mm:ss" type="datetimerange" range-separator="至"
                        start-placeholder="开始日期" end-placeholder="结束日期">
                    </el-date-picker>
                </el-form-item>
            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button type="primary" @click="submitDelForm">执 行</el-button>
                <el-button @click="deldlgvis = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { historySourceList, delHistorySource, historySourceInfo, addHistorySource, editHistorySource,delAllHistory } from "@/api/rules/historysource";
export default {
    name: "HistoryList",
    mixins: [resizeTableCon],
    data() {
        return {
            total: 0,
            queryParams: {
                Key: "",
                pageNum: 1,
                pageSize: 25,
            },
            dateTableList: [],
            // 列信息
            loading: false,
            ids: [], //选择的数据源
            form: {
                Name: "",
                StorageType: "influx",
                Remark: ""
            },
            DBConfig: {
                org: "",
                url: "",
                token: "",
                bucket: "",
            },
            editId: "",
            open: false,
            deldlgvis: false,
            delform: {
            },
            deldateRange: [],
            pickerOptions: {
                disabledDate(time) {
                    const tomorrow = new Date();
                    tomorrow.setDate(tomorrow.getDate() + 1);
                    return time.getTime() > tomorrow.getTime();
                }
            },
        };
    },
    mounted() {
        this.getList();
    },

    methods: {
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.resetForm("queryForm");
            this.handleQuery();
        },
        createdSource() {
            this.open = true;
            this.form = {
                Name: "",
                StorageType: "influx",
                Remark: ""
            };
            this.DBConfig = {
                org: "",
                url: "",
                token: "",
                bucket: ""
            };
        },
        // 多选框选中数据
        handleSelectionChange(selection) {
            this.ids = selection.map((item) => item.Id);

            this.single = selection.length != 1;
            this.multiple = !selection.length;
        },
        isRed({ row }) {
            let checkIdList = this.ids;
            // console.log("选中的",checkIdList,this.ids,row);
            if (checkIdList.includes(row.Id)) {
                return {
                    backgroundColor: "#F6F9FF",
                };
            }
        },
        async getList() {
            let res = await historySourceList(this.queryParams);
            this.dateTableList = res.data.List;
            this.total = res.data.Total;
        },
        async handleUpdate(row) {
            //修改
            this.open = true;
            this.editId = row.Id;
            let res = await historySourceInfo(row.Id);
            this.form = res.data;
            this.DBConfig = JSON.parse(res.data.StorageConfig);
        },
        handleDelete(row) {
            let that = this;
            //删除
            this.$modal
                .confirm('是否确认删除存储数据源"' + row.Name + '"？')
                .then(function () {
                    return delHistorySource(row.Id);
                })
                .then(() => {
                    that.getList();
                    that.$modal.msgSuccess("移除成功");
                })
                .catch((err) => {
                    console.log("错误", err);
                });
        },
        handleDelHis(row) {
            this.$set(this, "delform", {"SourceId":row.Id});
            this.$set(this, "deldateRange", []);
            this.deldlgvis = true;
        },
        submitDelForm() {
            if (this.deldateRange.length < 2) {
                this.$message({
                    message: "请选择删除的日期范围",
                    type: "error",
                });
                return;
            }
            this.addDateRange(this.delform, this.deldateRange, ['BeginTime', 'EndTime']);
            delAllHistory(this.delform).then((rsp) => {
                this.$modal.msgSuccess("发送成功");
            });
        },
        async submitForm() {
            const loading = this.$loading({
                lock: true,
                text: '保存中...',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            this.form.StorageConfig = JSON.stringify(this.DBConfig);
            if (this.editId == "") {
                let res = await addHistorySource(this.form);
                if (res.code == 0) {
                    this.$modal.msgSuccess("添加成功");
                    this.getList();
                    this.open = false;
                }
            } else {
                let res = await editHistorySource(this.form);
                if (res.code == 0) {
                    this.$modal.msgSuccess("修改成功");
                    this.getList();
                    this.open = false;
                }
            }

            loading.close();
        },
        cancel() {
            this.open = false;
        },
    },
};
</script>
<style lang="less" scoped></style>