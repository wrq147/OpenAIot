<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
        <div>
            <el-row :gutter="20">
                <el-col :span="24" :xs="24">
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
                    <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
                        <el-row :gutter="10" class="mb8 button_row">
                            <div>
                                <el-col :span="1.5">
                                    <el-button type="primary" icon="el-icon-plus" plain
                                        @click="handleAdd('')">新增</el-button>
                                </el-col>
                            </div>
                        </el-row>

                        <el-table v-loading="loading" :data="sourceList" class="data_table" 
                            :header-cell-style="cellSty" style="width:100%" :fit="true">
                            <el-table-column label="视频源Id" align="center" :show-overflow-tooltip="true">
                                <template slot-scope="scope">
                                    <el-link @click.stop="handleAdd(scope.row)">{{ scope.row.Id }}</el-link>
                                </template>
                            </el-table-column>
                            <el-table-column label="安装位置" prop="Position" :show-overflow-tooltip="true" />
                            <el-table-column label="VideoType" align="center" width="80">
                                <template slot-scope="scope">
                                    {{ scope.row.VideoType == 0 ? "固定地址" : "GB28181" }}
                                </template>
                            </el-table-column>
                            <el-table-column label="视频Key" align="center" prop="VideoKey"
                                :show-overflow-tooltip="true" />
                            <el-table-column label="视频信息" width="260">
                                <template slot-scope="scope">
                                    <div v-if="scope.row.VideoType == 0">
                                        <span>推流地址：{{ scope.row.PullAddr }},拉流节点：{{ scope.row.PullNode }}</span>
                                    </div>
                                    <div v-else>
                                        <span>注册用户名：{{ scope.row.UserName }},注册密码：{{ scope.row.UserPwd }},码流类型：{{
                                            scope.row.BitType==0?"主码流":"子码流" }}</span>
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="操作" align="center" class-name="small-padding fixed-width"
                                width="150">
                                <template slot-scope="scope">
                                    <el-button type="text" icon="el-icon-edit"
                                        @click="handleAdd(scope.row)">编辑</el-button>
                                    <el-button type="text" icon="el-icon-setting"
                                        @click="handleSet(scope.row)">配置</el-button>
                                    <el-button type="text" icon="el-icon-delete" style="color:red"
                                        @click="handleDelete(scope.row.Id)">删除</el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                            :limit.sync="queryParams.pageSize" @pagination="getList" />
                    </div>
                </el-col>
            </el-row>
        </div>
        <!-- 新增/编辑视频源弹窗 -->
        <addvsource ref="addVideo" />
        <!-- 配置视频源弹窗 -->
        <aconfig ref="setAconfig" />
    </div>
</template>

<script>
import { videoSourceList } from "@/api/rules/video";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import addvsource from './addvsource.vue'
import aconfig from './aconfig.vue'
import { removeVideoSource } from "@/api/rules/video";

export default {
    components: {
        addvsource,
        aconfig
    },
    mixins: [resizeTableCon],
    data() {
        return {
            loading: false,
            open: false,
            title: '新增视频源',
            // 查询参数
            queryParams: {
                pageNum: 1,
                pageSize: 20,
                Key: ''
            },
            total: 0,
            sourceList: [],
        }
    },
    created() {
        this.getList();
    },
    methods: {
        getList() {
            this.open = false;
            this.loading = true;
            videoSourceList(this.queryParams).then(response => {
                this.sourceList = response.data.List;
                this.total = response.data.Total;
                this.loading = false;
            })
        },
        /** 搜索按钮操作 */
        handleQuery() {
            this.queryParams.pageNum = 1;
            this.getList();
        },
        /** 重置按钮操作 */
        resetQuery() {
            this.resetForm("queryForm");
            this.handleQuery();
        },
        /** 新增按钮操作 */
        handleAdd(data) {
            if (data == '') {
                this.$refs.addVideo.showDlg(null);
            }
            else {
                this.$refs.addVideo.showDlg(data.Id);
            }
        },
        handleSet(data){

        },
        /** 删除按钮操作 */
        handleDelete(row) {
            this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                removeVideoSource({ id: row.Id }).then(res => {
                    this.$message.success('删除成功!')
                    this.getList()
                })
            }).catch(() => { })
        },
        cancelForm() {
            this.open = false;
        },
    }
}
</script>