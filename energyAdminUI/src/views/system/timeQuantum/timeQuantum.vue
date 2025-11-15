<template>
      <div class="big_con" id="big_con">
      <div>
        <el-row :gutter="20">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div class="from_con" id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item prop="timeName">
                    <el-input v-model="queryParams.timeName" placeholder="请输入时段名称" clearable />
                </el-form-item>
                <el-form-item class="submit_button_con">
                  <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                  <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="elbiaoge_elform">
              <el-row :gutter="10" class="mb8 button_row">
                <div>
                  <el-col :span="1.5">
                    <el-button type="primary" icon="el-icon-plus" plain @click="handleAdd('')">新增时段</el-button>
                  </el-col>
                </div>
              </el-row>
  
              <el-table v-loading="loading" :data="batchList" class="data_table"  style="width:100%" :border="false">
                <el-table-column label="序号" align="center" type="index" width="50" />
                <el-table-column label="时段名称" align="center" prop="TimeName" />
                <el-table-column label="工作时间" align="center">
                    <el-table-column label="上班时间" align="center" prop="StartTime" />
                    <el-table-column label="下班时间" align="center" prop="EndTime" />
                </el-table-column>
                <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="150">
                  <template slot-scope="scope">
                    <el-button type="text" icon="el-icon-edit" @click="handleAdd(scope.row)">编辑</el-button>
                    <el-button type="text" icon="el-icon-delete" style="color:red" @click="handleCancel(scope.row.Id)">删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
          </el-col>
        </el-row>
      </div>
      <!-- 新增/编辑产品批次弹窗 -->
      <add-Time ref="addTime" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    </div>
  </template>
  
  <script>
  import { timeQuantumList, delTimeQuantum } from "@/api/scheduling/timeQuantum";
  import addTime from './cmp/addTime.vue'
  export default {
    components: {
        addTime
    },
    name: "Index",
    data() {
      return {
        loading: false,
        open: false,
        title: '新增时段',
        // 查询参数
        queryParams: {
            pageNum: 1,
            pageSize: 20,
            timeName:'',
        },
        total: 0,
        batchList: []
      };
    },
    created() {
        this.getList();
    },
    methods: {
        getList() {
            this.open = false;
            this.loading = true;
            timeQuantumList(this.queryParams).then(response => {
                this.batchList = response.data.List;
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
            if (data === '') {
                this.title = '新增时段';
                this.$refs['addTime'].ruleForm={
                    timeName: '',
                    deptId: '',
                    startTime: '',
                    endTime: '',
                };
            } else {
                this.title = '编辑时段';
                this.$refs['addTime'].ruleForm={
                    Id: data.Id,
                    deptId: data.DeptId,
                    timeName: data.TimeName,
                    startTime: data.StartTime,
                    endTime: data.EndTime,
                };
            }
            this.open = true;
        },
        /** 删除按钮操作 */
        handleCancel(id) {
            this.$confirm('此操作将永久删除该时段, 是否继续?', '提示', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                delTimeQuantum({ id: id }).then(response => {
                    this.$message({
                        type: 'success',
                        message: '删除成功!'
                    });
                    this.getList();
                })
            })
        },
        cancelForm() {
            this.open = false;
        }
    }
  };
  </script>
  
  <style lang="scss" scoped>
    .big_con{
      background-color: transparent;
    }
    .from_con{
      margin-bottom: 5px;
    }
  </style>
  