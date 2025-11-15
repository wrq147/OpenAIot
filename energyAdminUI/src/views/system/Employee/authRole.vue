<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <div class="from_con" id="from_con">
        <h4 class="form-header h4">基本信息</h4>
        <el-form ref="form" :model="form" class="biaodan" label-width="80px">
          <el-row>
            <el-col :span="8" :offset="2">
              <el-form-item label="用户昵称" prop="RealName">
                <el-input class="set_radius" v-model="form.RealName" disabled />
              </el-form-item>
            </el-col>
            <el-col :span="8" :offset="2">
              <el-form-item label="登录账号" prop="UserName">
                <el-input class="set_radius" v-model="form.UserName" disabled />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <h4 class="form-header h4">角色信息</h4>
        <el-table :row-style="isRed" :cell-style="isRed" :header-cell-style="cellSty" style="width:100%" class="data_table"
          v-loading="loading" border :row-key="getRowKey" @row-click="clickRow" ref="table"
          @selection-change="handleSelectionChange" :data="roles.slice((pageNum - 1) * pageSize, pageNum * pageSize)">
          <el-table-column label="序号" type="index" align="center">
            <template slot-scope="scope">
              <span>{{ (pageNum - 1) * pageSize + scope.$index + 1 }}</span>
            </template>
          </el-table-column>
          <el-table-column type="selection" :reserve-selection="true" width="55" :selectable="selectFn"></el-table-column>
          <el-table-column label="角色名称" align="center" prop="roleName" />
          <el-table-column label="角色说明" prop="remark" />
          <el-table-column label="创建时间" align="center" prop="createTime" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total > 0" :total="total" :page.sync="pageNum" :limit.sync="pageSize" />

        <el-form label-width="100px" class="submint_button">
          <el-form-item style="text-align: center; margin-left: -120px; margin-top: 30px">
            <el-button type="primary" @click="submitForm()">提交</el-button>
            <el-button @click="close()">返回</el-button>
          </el-form-item>
        </el-form>
      </div>
    </div>
  </div>
</template>

<script>
import { listRole } from "@/api/system/role";
import { getAuthRole } from "@/api/system/user";
import { updateAuthRole } from "@/api/system/member";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "AuthRole",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      // 分页信息
      total: 0,
      pageNum: 1,
      pageSize: 10,
      // 选中角色编号
      roleIds: [],
      // 角色信息
      roles: [],
      // 用户信息
      form: {},
      canList:[]
    };
  },
  async created() {
    const userId = this.$route.params && this.$route.params.userId;
    if (userId) {
      this.loading = true;
      let rrps = await listRole({ showAll: true });
      this.canList = rrps.data.List;
      let response = await getAuthRole(userId);
      this.form = response.data.user;
      this.roles = response.data.roles;
      this.total = this.roles.length;
      let checkedIds = response.data.roleIds;
      this.roleIds = checkedIds;
      this.$nextTick(() => {
        this.roles.forEach(row => {
          if (checkedIds.includes(row.roleId)) {
            this.$refs.table.toggleRowSelection(row);
          }
        });
      });
      this.loading = false;
    }
  },
  methods: {
    /** 单击选中行数据 */
    clickRow(row) {
      if(!this.canList.some(x=>x.roleId==row.roleId)){
        return;
      }
      this.$refs.table.toggleRowSelection(row);
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.roleIds = selection.map(item => item.roleId);
    },
    selectFn(row,index){
      if(!this.canList.some(x=>x.roleId==row.roleId)){
        return false;
      }
      else{
        return true;
      }
    },
    isRed({ row }) {
      //设置表格中选中行的背景颜色
      let checkIdList = this.roleIds;
      if (checkIdList.includes(row.roleId)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    // 保存选中的数据编号
    getRowKey(row) {
      return row.roleId;
    },
    /** 提交按钮 */
    submitForm() {
      const userId = this.form.Id;
      const roleIds = this.roleIds.join(",");
      updateAuthRole({ userId: userId, roleIds: roleIds }).then(response => {
        this.$modal.msgSuccess("授权成功");
        this.close();
      });
    },
    /** 关闭按钮 */
    close() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/org/Employee" });
    }
  }
};
</script>
<style lang="less">
.submint_button {
  .el-button {
    border-radius: 4px;
    height: 40px;
    width: 100px;
    font-size: 16px;
    cursor: pointer;
  }
}
</style>