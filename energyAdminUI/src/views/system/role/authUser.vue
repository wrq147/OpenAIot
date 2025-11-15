<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" ref="queryForm" class="biaodan" :inline="true">
          <el-form-item label="用户名字" prop="key">
            <el-input class="set_radius" v-model="queryParams.key" placeholder="请输入用户名字" clearable @keyup.enter.native="handleQuery"/>
          </el-form-item>
          <el-form-item label="手机号码" prop="phonenumber">
            <el-input v-model="queryParams.phonenumber" class="set_radius" placeholder="请输入手机号码" clearable @keyup.enter.native="handleQuery"/>
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="openSelectUser" v-hasPermi="['/AuthService/Role/Edit']">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">添加用户</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button style="font-size:12px" type="danger" plain icon="el-icon-circle-close" :disabled="multiple" @click="cancelAuthUserAll" v-hasPermi="['/AuthService/Role/Edit']">批量取消授权</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button style="font-size:12px" type="warning" plain icon="el-icon-close" @click="handleClose">关闭</el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" border :data="userList" :row-style="isRed" :cell-style="isRed" class="data_table"
          @selection-change="handleSelectionChange" :header-cell-style="cellSty" style="width:100%">
          <el-table-column type="selection" width="55" align="center" />
          <!-- <el-table-column label="用户名称" prop="UserName" :show-overflow-tooltip="true" /> -->
          <el-table-column label="用户名字" prop="RealName" :show-overflow-tooltip="true" />
          <el-table-column label="邮箱" prop="Email" :show-overflow-tooltip="true" />
          <el-table-column label="手机" prop="Mobile" :show-overflow-tooltip="true" />
          <el-table-column label="状态" align="center" prop="status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.sys_normal_disable" :value="scope.row.status" />
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center" prop="createTime" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-circle-close" @click="cancelAuthUser(scope.row)" v-hasPermi="['/AuthService/Role/Edit']">取消授权</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total>0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
      </div>
      <select-user ref="select" :roleId="queryParams.roleId" @ok="handleQuery" />
    </div>
  </div>
</template>

<script>
import {
  allocatedUserList,
  authUserCancel,
  authUserCancelAll
} from "@/api/system/role";
import selectUser from "./selectUser";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "AuthUser",
  dicts: ["sys_normal_disable"],
  components: { selectUser },
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 选中用户组
      userIds: [],
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 用户表格数据
      userList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        roleId: undefined,
        key: undefined,
        phonenumber: undefined
      }
    };
  },
  created() {
    const roleId = this.$route.params && this.$route.params.roleId;
    if (roleId) {
      this.queryParams.roleId = roleId;
      this.getList();
    }
  },
  methods: {
    /** 查询授权用户列表 */
    getList() {
      this.loading = true;
      allocatedUserList(this.queryParams).then(response => {
        this.userList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    // 返回按钮
    handleClose() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/org/role" });
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
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.userIds = selection.map(item => item.Id);
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      //设置表格中选中行的背景颜色
      let checkIdList = this.userIds;
      // console.log("选中的",checkIdList,this.userIds,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    /** 打开授权用户表弹窗 */
    openSelectUser() {
      this.$refs.select.show();
    },
    /** 取消授权按钮操作 */
    cancelAuthUser(row) {
      console.log("改行的数据",row);
      
      const roleId = this.queryParams.roleId;
      this.$modal
        .confirm('确认要取消该用户"' + row.RealName + '"角色吗？')
        .then(function() {
          return authUserCancel({ UserId: row.Id, RoleID: roleId });
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("取消授权成功");
        })
        .catch(() => {});
    },
    /** 批量取消授权按钮操作 */
    cancelAuthUserAll(row) {
      const roleId = this.queryParams.roleId;
      const userIds = this.userIds.join(",");
      console.log("选中的用户",userIds);
      
      this.$modal
        .confirm("是否取消选中用户授权数据项？")
        .then(function() {
          return authUserCancelAll({ roleId: roleId, userIds: userIds });
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("取消授权成功");
        })
        .catch(() => {});
    }
  }
};
</script>