<template>
    <div>
        <el-row :gutter="10">
          <!--用户数据-->
          <el-col :span="24" :xs="24">
            <div id="from_con">
              <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
                <el-form-item prop="timeName">
                    <el-input v-model="queryParams.timeName" placeholder="请输入时段名称" clearable />
                </el-form-item>
                <el-form-item class="submit_button_con">
                  <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="elbiaoge_elform">
              <el-table v-loading="loading" :data="batchList" class="data_table"  style="width:100%" highlight-current-row @current-change="onDeviceChange">
                <el-table-column label="序号" align="center" type="index" width="50" />
                <el-table-column label="时段名称" align="center" prop="TimeName" />
                <el-table-column label="上班时间" align="center" prop="StartTime" />
                <el-table-column label="下班时间" align="center" prop="EndTime" />
              </el-table>
              <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                :limit.sync="queryParams.pageSize" @pagination="getList" />
            </div>
          </el-col>
        </el-row>
      </div>
</template>
<script>
  import { timeQuantumList } from "@/api/scheduling/timeQuantum";
  export default {
    name: "Index",
    data() {
      return {
        loading: false,
        // 查询参数
        queryParams: {
            pageNum: 1,
            pageSize: 10,
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
        onDeviceChange(val) {
          this.$emit('getTimeInfo', val)
        }
    }
  };
  </script>
  
  <style lang="scss" scoped>
  .home_com {
    width: 100%;
    padding: 20px;
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;
    align-items: flex-start;
    background: #F4F5F9;
    margin-bottom: -30px;
  }
  .biaodan .el-form-item{
    margin-bottom: 0 !important;
  }
  .biaodan.el-form--inline{
    margin-left: 0;
  }
  .biaodan .submit_button_con{
    float: none !important;
    margin-left: 20px;
  }
  </style>