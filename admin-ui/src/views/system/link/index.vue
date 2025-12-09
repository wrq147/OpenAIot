<template>
    <div style="padding:20px 20px 0 20px" id="big_con">
        <div>
            <div class="from_con" id="from_con" v-show="showSearch">
                <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
                    <el-form-item label="关键词" prop="Key">
                        <el-input class="set_radius" v-model="queryParams.Key" placeholder="请输入搜索关键词" clearable
                            @keyup.enter.native="handleQuery" />
                    </el-form-item>
                    <el-form-item label="创建日期">
                        <el-date-picker class="set_radius" v-model="dateRange" style="width: 250px"
                            value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                            end-placeholder="结束日期"></el-date-picker>
                    </el-form-item>
                    <el-form-item class="submit_button_con">
                        <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                        <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                    </el-form-item>
                </el-form>
            </div>
            <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
                <el-row :gutter="10" class="mb8 button_row">
                    <div>
                        <el-col :span="1.5">
                            <el-button type="primary" plain @click="handleAdd"
                                v-hasPermi="['/DictService/DictType/Add']">
                                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                                <span style="margin-left:6px">新增</span>
                            </el-button>
                        </el-col>

                        <el-col :span="1.5">
                            <el-button type="danger" plain :disabled="multiple" @click="handleDelete"
                                v-hasPermi="['/DictService/DictType/Remove']">
                                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                                <span style="margin-left:6px">删除</span>
                            </el-button>
                        </el-col>
                    </div>
                    <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
                </el-row>

                <el-table v-loading="loading" border :data="linkList" :row-style="isRed" class="data_table"
                    :header-cell-style="cellSty" style="width:100%" @selection-change="handleSelectionChange">
                    <el-table-column type="selection" width="55" align="center" />
                    <el-table-column label="短链接Id" align="center" prop="Id" width="150" />
                    <el-table-column label="跳转的url地址" align="center" prop="Url" :show-overflow-tooltip="true" />
                    <el-table-column label="所属组织" align="center" prop="OrgName" :show-overflow-tooltip="true" width="200" />
                    <el-table-column label="创建时间" align="center" width="180">
                        <template slot-scope="scope">
                            <span>{{ parseTime(scope.row.CreatedOn) }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
                        <template slot-scope="scope">
                            <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)">删除</el-button>
                        </template>
                    </el-table-column>
                </el-table>

                <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                    :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
            <!-- 添加或修改参数配置对话框 -->
            <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="500px" append-to-body>
                <el-form ref="form" :model="form" label-width="80px">
                    <el-form-item label="链接地址" prop="url">
                        <el-input v-model="form.url" placeholder="请输入链接url" />
                    </el-form-item>
                </el-form>
                <div slot="footer" class="dialog-footer">
                    <el-button type="primary" @click="submitForm">确 定</el-button>
                    <el-button @click="cancel">取 消</el-button>
                </div>
            </el-dialog>
        </div>
    </div>
</template>

<script>
import {
    getLinkList,
    addLink,
    delLink
} from "@/api/system/link";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
    mixins: [resizeTableCon],
    data() {
        return {
            // 遮罩层
            loading: true,
            // 导出遮罩层
            exportLoading: false,
            // 选中数组
            ids: [],
            // 非单个禁用
            single: true,
            // 非多个禁用
            multiple: true,
            // 显示搜索条件
            showSearch: true,
            // 总条数
            total: 0,
            // 链接表格数据
            linkList: [],
            // 弹出层标题
            title: "",
            // 是否显示弹出层
            open: false,
            // 日期范围
            dateRange: [],
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 10,
                Key: undefined,
            },
            // 表单参数
            form: {
                url: ""
            }
        };
    },
    created() {
        this.getList();
    },
    methods: {
        /** 查询链接列表 */
        getList() {
            this.loading = true;
            getLinkList(this.addDateRange(this.queryParams, this.dateRange)).then(
                response => {
                    this.linkList = response.data.List;
                    this.total = response.data.Total;
                    this.loading = false;
                }
            );
        },
        // 取消按钮
        cancel() {
            this.open = false;
            this.reset();
        },
        // 表单重置
        reset() {
            this.form = {
                url: ""
            };
            this.resetForm("form");
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.dateRange = [];
            this.resetForm("queryForm");
            this.handleQuery();
        },
        /** 新增按钮操作 */
        handleAdd() {
            this.reset();
            this.open = true;
            this.title = "添加链接";
        },
        // 多选框选中数据
        handleSelectionChange(selection) {
            this.ids = selection.map(item => item.Id);
            this.single = selection.length != 1;
            this.multiple = !selection.length;
        },
        isRed({ row }) {
            //设置表格中选中行的背景颜色
            let checkIdList = this.ids;
            // console.log("选中的",checkIdList,this.ids,row);
            if (checkIdList.includes(row.Id)) {
                return {
                    backgroundColor: "#F6F9FF"
                };
            }
        },
        /** 提交按钮 */
        submitForm: function () {
            if (this.form.url == "") {
                this.$modal.msgError("请输入链接url");
                return;
            }
            addLink(this.form).then(response => {
                this.$modal.msgSuccess("新增成功");
                this.open = false;
                this.getList();
            });
        },
        /** 删除按钮操作 */
        handleDelete(row) {
            const tmpIds = row.Id || this.ids;
            this.$modal
                .confirm('是否确认删除链接编号为"' + tmpIds + '"的数据项？')
                .then(function () {
                    return delLink({ id: tmpIds });
                })
                .then(() => {
                    this.getList();
                    this.$modal.msgSuccess("删除成功");
                })
                .catch(() => { });
        },
    }
};
</script>