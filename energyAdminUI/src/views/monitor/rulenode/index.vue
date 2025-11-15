<template>
    <div style="padding:10px 10px 0 10px" id="big_con">
        <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
                <div>
                    <el-col :span="1.5">
                        <el-button type="primary" plain @click="popAddDlg">
                            <i class="el-icon-plus"></i>
                            <span style="margin-left:6px">注册规则节点服务器</span>
                        </el-button>
                    </el-col>

                </div>
            </el-row>
            <el-table border v-loading="loading" :data="list" :header-cell-style="cellSty" style="width:100%"
                class="data_table">
                <el-table-column label="序号" type="index" align="center">
                    <template slot-scope="scope">
                        <span>{{ scope.$index + 1 }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="节点名称" align="center" prop="name" :show-overflow-tooltip="true" />
                <el-table-column label="过期时间" align="center" prop="time" :show-overflow-tooltip="true" />
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
                    <template slot-scope="scope">
                        <el-button type="text" @click="handleForceDown(scope.row)">
                            <i class="zhongtaiiconfont zhongtai-icon-qiangtui"></i>
                            <span style="margin-left:6px">下线</span>
                        </el-button>
                    </template>
                </el-table-column>
            </el-table>

        </div>

        <!-- 添加节点对话框 -->
        <el-dialog :visible.sync="addDialogVisible" width="300px" title="注册节点" center>
            <el-form :model="addForm" ref="addFormRef" :rules="addFormRules">
                <el-form-item label="节点名称" prop="name">
                    <el-input v-model="addForm.name" placeholder="请输入节点名称"></el-input>
                </el-form-item>
            </el-form>
            <div class="dialog-footer">
                <el-button @click="addDialogVisible = false">取消</el-button>
                <el-button type="primary" @click="handleAddNode">确定</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
import { nodeList, forcedDown, regNode } from "@/api/rules/ruselSevic";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
    name: "rulenode",
    mixins: [resizeTableCon],
    data() {
        return {
            // 遮罩层
            loading: true,
            // 表格数据
            list: [],
            // 控制添加节点对话框的显示与隐藏
            addDialogVisible: false,
            // 存储添加节点表单的数据
            addForm: {
                name: ''
            },
            // 表单验证规则
            addFormRules: {
                name: [
                    { required: true, message: '节点名称为必填项', trigger: 'blur' }
                ]
            },
            // 定时器 ID
            timerId: null
        };
    },
    created() {
        this.loading = true;
        this.getList();
        // 设置定时任务，每 5 秒调用一次 getList 方法
        this.timerId = setInterval(() => {
            this.getList();
        }, 5000);
    },
    beforeDestroy() {
        // 在组件销毁前清除定时任务
        if (this.timerId) {
            clearInterval(this.timerId);
        }
    },
    methods: {
        getList() {
            nodeList().then(response => {
                this.list = response.data;
                this.loading = false;
            });
        },
        handleForceDown(row) {
            this.$confirm("是否确认下线该节点?", "警告", {
                confirmButtonText: "确定",
                cancelButtonText: "取消",
                type: "warning"
            })
                .then(async () => {
                    await forcedDown(row.name);
                    this.msgSuccess("下线成功");
                    this.getList();
                })
                .catch(function () { });
        },
        popAddDlg() {
            // 显示添加节点对话框
            this.addDialogVisible = true;
        },
        handleAddNode() {
            // 对表单进行验证
            this.$refs.addFormRef.validate(async (valid) => {
                if (valid) {
                    // 调用注册节点的 API
                    await regNode(this.addForm.name);
                    this.$message.success('节点添加成功');
                    // 关闭对话框
                    this.addDialogVisible = false;
                    // 清空表单
                    this.addForm.name = '';
                    // 刷新节点列表
                    this.getList();
                }
            });
        }
    }
};
</script>