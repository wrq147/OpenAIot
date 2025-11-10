<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con">
        <!-- <el-col :span="24" style="margin-bottom:20px;"> -->
        <el-form :model="queryParams" ref="queryForm" class="biaodan" :inline="true">
          <el-form-item label="用户名称" prop="userName">
            <el-input
              class="set_radius"
              v-model="queryParams.userName"
              placeholder="请输入用户名称"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          </el-form-item>
        </el-form>
      </div>
      <!-- </el-col> -->
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-table
          border
          v-loading="loading"
          :data="list.slice((queryParams.pageNum-1)*queryParams.pageSize,queryParams.pageNum*queryParams.pageSize)"
          :header-cell-style="cellSty"
          style="width:100%"
          class="data_table"
        >
          <el-table-column label="序号" type="index" align="center">
            <template slot-scope="scope">
              <span>{{(queryParams.pageNum - 1) * queryParams.pageSize + scope.$index + 1}}</span>
            </template>
          </el-table-column>
          <el-table-column label="会话编号" align="center" prop="UserId" :show-overflow-tooltip="true" />
          <el-table-column
            label="登录名称"
            align="center"
            prop="UserName"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="主机" align="center" prop="Ipaddr" :show-overflow-tooltip="true" />
          <el-table-column
            label="登录地点"
            align="center"
            prop="Location"
            :show-overflow-tooltip="true"
          />
          <el-table-column label="浏览器" align="center" prop="Browser" />
          <el-table-column label="操作系统" align="center" prop="OSName" />
          <el-table-column label="登录时间" align="center" prop="LoginTime" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.LoginTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                @click="handleForceLogout(scope.row)"
                v-hasPermi="['/MonitorService/Online/ForceLogout']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-qiangtui"></i>
                <span style="margin-left:6px">强退</span>
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination
          v-show="total>0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { list, forceLogout } from "@/api/monitor/online";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "Online",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 总条数
      total: 0,
      // 表格数据
      list: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 100,
        userName: undefined
      }
    };
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询登录日志列表 */
    getList() {
      this.loading = true;
      list(this.queryParams).then(response => {
        this.list = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
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
    /** 强退按钮操作 */
    handleForceLogout(row) {
      this.$modal
        .confirm('是否确认强退名称为"' + row.userName + '"的数据项？')
        .then(function() {
          return forceLogout(row.UserId);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("强退成功");
        })
        .catch(() => {});
    }
  }
};
</script>

