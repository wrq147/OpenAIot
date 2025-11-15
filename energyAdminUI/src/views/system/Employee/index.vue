<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="归属部门" prop="deptId">
                <treeselect class="set_radius" v-model="queryParams.deptId" :options="deptOptions" :show-count="true"
                  placeholder="请选择归属部门" />
              </el-form-item>
              <el-form-item label="关键字" prop="key">
                <el-input class="set_radius" v-model="queryParams.key" placeholder="请输入关键字" clearable
                  @keyup.enter.native="handleQuery" />
              </el-form-item>
              <el-form-item label="手机号码" prop="phonenumber">
                <el-input class="set_radius" v-model="queryParams.phonenumber" placeholder="请输入手机号码" clearable
                  @keyup.enter.native="handleQuery" />
              </el-form-item>
              <el-form-item label="状态" prop="status">
                <el-select class="set_radius" v-model="queryParams.status" placeholder="用户状态" clearable>
                  <el-option v-for="dict in dict.type.sys_normal_disable" :key="dict.value" :label="dict.label"
                    :value="dict.value" />
                </el-select>
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker class="set_radius" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
                  type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row" style="justify-content:flex-start">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/AuthService/Member/YaoQing']">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加员工</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="primary" plain :loading="exportLoading" @click="handleExport"
                    v-hasPermi="['/AuthService/Member/Export']">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="danger" plain :disabled="single" @click="handleDelete"
                    v-hasPermi="['/AuthService/Member/Remove']">
                    <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    <span style="margin-left:6px">移除</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="userList" :row-style="isRed" :cell-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="账号" align="center" key="UserName" prop="UserName" v-if="columns[0].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="员工昵称" align="center" key="RealName" prop="RealName" v-if="columns[1].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="部门" align="center" key="dept_name" prop="dept_name" v-if="columns[2].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="手机号码" align="center" key="Mobile" prop="Mobile" v-if="columns[3].visible" />
              <el-table-column label="邮箱" align="center" key="Email" prop="Email" v-if="columns[4].visible" />
              <el-table-column label="工作签名" align="center" key="Signature" prop="Signature" v-if="columns[5].visible"></el-table-column>
              <el-table-column label="状态" align="center" key="status" v-if="columns[6].visible" width="118">
                <template slot-scope="scope">
                  <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" disabled></el-switch>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[7].visible" width="240">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-circle-plus-outline" @click="handleBilocation(scope.row)"
                    v-hasPermi="['/AuthService/Member/Edit']">分身</el-button>
                  <el-button type="text" icon="el-icon-circle-check" @click="handleAuthRole(scope.row)"
                    v-hasPermi="['/AuthService/Member/Edit']">分配角色</el-button>
                    <el-button type="text" icon="el-icon-key" @click="handleResetPwd(scope.row)" v-hasPermi="['/AuthService/Member/Edit']">重置密码</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="600px" append-to-body
        class="add_dialog_border">
        <div class="dialog_content_con">
          <div>
            <span>通过链接邀请</span>
          </div>
          <div style="height:40px;margin-top:18px">
            <el-input v-model="tipsValue" class="tipsSelect" placeholder="邀请码"></el-input>
            <el-button class="link_button" type="primary" @click="onCopy">复制链接</el-button>
          </div>
          <div class="date_prompt">
            <span>
              链接有效期：
              <span class="blue_color" style="color:#3572FF">7天</span>后邀请链接过期
            </span>
          </div>
        </div>
        <div class="dialog_content_con" style="margin-top:30px">
          <div>
            <span>通过账号邀请</span>
          </div>
          <div style="height:40px;margin-top:18px;margin-bottom:5px;">
            <el-input placeholder="通过用户昵称、用户名、手机号码邀请组织外成员" class="account_add" @focus="openAccountInvite" value
              style="width:100%">
            </el-input>
          </div>
        </div>
      </el-dialog>

      <!-- 用户导入对话框 -->
      <el-dialog :close-on-click-modal="false" :title="invitation.title" :visible.sync="invitation.open" width="600px"
        append-to-body :show-close="false" class="add_invitation_border">
        <div slot="title" class="dialog-title" style="margin-bottom:10px">
          <div>
            <i class="el-icon-arrow-left" style="margin-right:6px;cursor: pointer;" @click="invitation.open = false"></i>
            <span>邀请组织成员</span>
          </div>
        </div>
        <div>
          <el-select v-model="searchValue" class="account_add" multiple filterable remote reserve-keyword
            placeholder="通过用户昵称、用户名、手机号码邀请组织外成员" :remote-method="selectSearchMember" :loading="searchLoading"
            ref="selectMemberDept">
            <el-option v-for="member in searchMemberList" :key="member.Id" :value="member.Name"
              style="height:50px;line-height:50px;">
              <div style="display:flex;justify-content: flex-start;align-items: center;height:50px;line-height:50px"
                @click="choiceSearchMember(member)">
                <img :src="member.Avatar" style="float: left;width: 28px;height: 28px;margin-right: 10px;" />
                <div style="float: left; color: #8492a6; font-size: 13px">{{ member.Name }}</div>
              </div>
            </el-option>
          </el-select>
          <div class="invite_tips">
            <ul class="daiyaoqing" v-if="yaoqingList.length > 0">
              <li v-for="it in yaoqingList" :key="it.Id">
                <div style="position:absolute;top:10px;right:10px;font-size:20px;cursor: pointer;" @click="closeYaoQing">
                  <i class="el-icon-close"></i>
                </div>
                <div class="input_float_top">
                  <img :src="it.Avatar"
                    style="float: left;width: 100px;height: 100px;margin-bottom: 10px;border-radius:50%;" />
                  <span>{{ it.Name }}</span>
                </div>
                <div class="input_float_bottom">
                  <treeselect class="set_radius" v-model="it.deptId" :options="deptOptions" :show-count="true"
                    placeholder="请选择部门" ref="deptTree" style="width:240px;height:100%;margin-bottom:30px" />
                  <el-input style="width:240px;height:100%;" v-model.lazy="it.post_name" placeholder="请输入职位"
                    ref="postInput"></el-input>
                </div>
              </li>
            </ul>
            <div v-else class="tips_con">点击输入框邀请成员，快来试试吧！</div>
          </div>
        </div>
        <div slot="footer" class="dialog-footer" style="margin-top:10px">
          <el-button @click="invitation.open = false">取 消</el-button>
          <el-button v-if="yaoqingList.length > 0" type="primary" @click="submitForm">邀 请</el-button>
          <el-button v-else type="primary" style="background:#9BCAFF;border:none;height:36px;">邀 请</el-button>
        </div>
      </el-dialog>
      <org-picker :multiple="multiple" ref="orgPicker" selected @ok="selected" />
      <bilocation ref="bilocation" :dialog-visible="dialogVisible" @cancelForm="cancelForm"/>
    </div>
  </div>
</template>

<script>

import {
  listMember,
  delMember,
  searchMember,
  memberJionOrg,
  createCode,
  memberResetPwd
} from "@/api/system/Employee";
import { getToken } from "@/utils/auth";
import { treeselect } from "@/api/system/dept";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import OrgPicker from "../../flowable/common/OrgPicker";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import bilocation from './bilocation'
export default {
  name: "User",
  dicts: ["sys_normal_disable", "sys_user_sex"],
  components: { Treeselect, OrgPicker, bilocation },
  mixins: [resizeTableCon],
  data() {
    return {
      searchValue: '',
      searchMemberList: [], //全局搜索用户列表
      searchLoading: false, //全局搜素用户的状态
      yaoqingList: [], //待邀请用户列表
      tipsValue: "复制的链接", //链接
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
      // 用户表格数据
      userList: null,
      // 弹出层标题
      title: "",
      // 部门树选项
      deptOptions: [],
      // 是否显示弹出层
      open: false,
      dialogVisible: false,
      // 日期范围
      dateRange: [],
      // 角色选项
      roleOptions: [],
      // 表单参数
      form: {},
      // 员工邀请参数
      invitation: {
        // 是否显示弹出层（员工邀请）
        open: false,
        // 弹出层标题（员工邀请）
        title: ""
      },
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        userName: undefined, //用户名
        phonenumber: undefined, //手机号
        status: undefined, //状态
        deptId: undefined, //部门
        deptIdWithChildren: true
      },
      // 列信息
      columns: [
        { key: 0, label: `账号`, visible: true },
        { key: 1, label: `用户昵称`, visible: true },
        { key: 2, label: `部门`, visible: true },
        { key: 3, label: `手机号码`, visible: true },
        { key: 4, label: `邮箱`, visible: true },
        { key: 5, label: `工作签名`, visible: true },
        { key: 6, label: `状态`, visible: true },
        { key: 7, label: `创建时间`, visible: true }
      ],
      // 表单校验
      rules: {
        UserName: [
          { required: true, message: "用户名称不能为空", trigger: "blur" },
          {
            min: 2,
            max: 20,
            message: "用户名称长度必须介于 2 和 20 之间",
            trigger: "blur"
          }
        ],
        RealName: [
          { required: true, message: "用户昵称不能为空", trigger: "blur" }
        ],
        Email: [
          {
            type: "email",
            message: "'请输入正确的邮箱地址",
            trigger: ["blur", "change"]
          }
        ],
        Mobile: [
          {
            pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
            message: "请输入正确的手机号码",
            trigger: "blur"
          }
        ]
      }
    };
  },
  watch: {
    // 根据名称筛选部门树
  },
  created() {
    this.getList();
    this.getTreeselect();
  },
  methods: {
    /** 重置密码按钮操作 */
    handleResetPwd(row) {
      this.$prompt('请输入员工"' + row.RealName + '"的新密码', "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnClickModal: false,
        inputPattern: /^.{5,20}$/,
        inputErrorMessage: "用户密码长度必须介于 5 和 20 之间",
        closeOnClickModal:false
      })
        .then(({ value }) => {
          memberResetPwd({userId:row.Id,password:value}).then(response => {
            this.$modal.msgSuccess("修改成功，新密码是：" + value);
          });
        })
        .catch(() => {});
    },
    /** 分配角色操作 */
    handleAuthRole: function (row) {
      const userId = row.Id;
      this.$router.push("/org/user-auth/role/" + userId);
    },
    // 设置分身
    handleBilocation(row) {
      this.$refs['bilocation'].getPeople(row);
      this.dialogVisible = true
    },
    closeYaoQing() {
      //关闭邀请用户展示
      this.yaoqingList.shift();
    },
    onCopy() {
      let oInput = document.createElement("input");
      oInput.value = this.tipsValue;
      document.body.appendChild(oInput);
      oInput.select(); // 选择对象;
      document.execCommand("Copy"); // 执行浏览器复制命令
      this.$message({
        message: "复制成功",
        type: "success"
      });
      oInput.remove();
    },
    choiceSearchMember(member) {
      //点击查询结果的一条数据
      this.$refs.selectMemberDept.blur();
      this.searchMemberList = [];
      member.deptId = null;
      // member.post_name = '';
      this.$set(member, "post_name", "");
      if (this.yaoqingList && this.yaoqingList.length > 0) {
        this.yaoqingList.shift();
      }
      this.yaoqingList.push(member);
    },
    selectSearchMember(query) {
      //全局搜索指定用户
      if (query !== "") {
        this.searchLoading = true;
        searchMember({ key: query }).then(res => {
          setTimeout(() => {
            this.searchLoading = false;
            this.searchMemberList = res.data.filter(item => {
              return item;
            });
          }, 200);
        });
      } else {
        this.searchMemberList = [];
      }
    },
    selected(val) {
      //获取选中的用户
    },
    //打开部门成员列表
    //打开搜索账号邀请
    openAccountInvite() {
      this.invitation.open = true;
    },
    /** 查询用户列表 */
    getList() {
      this.loading = true;
      listMember(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.userList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    /** 查询部门下拉树结构 */
    getTreeselect() {
      treeselect({ OrgId: this.$store.getters.orgId }).then(response => {
        this.deptOptions = response.data;
      });
    },
    // 表单重置
    reset() {
      this.form = {
        Id: undefined,
        dept_id: undefined,
        UserName: undefined,
        RealName: undefined,
        Mobile: undefined,
        Email: undefined,
        Sex: undefined,
        status: "0",
        Introduction: undefined,
        post_name: undefined,
        roleIds: []
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
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)"
        };
      }
    },
    /** 添加员工按钮操作 */
    handleAdd() {
      this.reset();
      this.getTreeselect();
      this.open = true;
      this.title = "添加员工";
      createCode().then(res => {
        // console.log("生成的邀请码", res);
        if (res.code == 0) {
          if(process.env.VUE_APP_LANG=='CN'){
            this.tipsValue = window.location.origin + "/jump.html?lang=CN&code=" + encodeURIComponent(res.data);
          }else{
            this.tipsValue = window.location.origin + "/jump.html?code=" + encodeURIComponent(res.data);
          }
          
        }
      });
      // });
    },
    /** 提交按钮 */
    submitForm: function () {
      // console.log("邀请的用户列表", this.yaoqingList);
      if (this.yaoqingList[0].post_name == "") {
        this.$message.error("请填写邀请人的职位");
        this.$nextTick(() => {
          this.$refs.postInput.focus();
        });
        return;
      }
      if (
        this.yaoqingList[0].deptId == "" ||
        this.yaoqingList[0].deptId == null
      ) {
        this.$message.error("请填写邀请人的部门");
        this.$nextTick(() => {
          this.$refs.deptTree.focus();
        });
        return;
      }
      memberJionOrg({
        uid: this.yaoqingList[0].Id,
        depId: this.yaoqingList[0].deptId,
        postName: this.yaoqingList[0].post_name
      }).then(res => {
        // console.log("加入后返回值", res);
        if (res.code == 0) {
          this.$modal.msgSuccess("邀请成功");
          this.$nextTick(() => {
            this.invitation.open = false;
            this.open = false;
            this.getList();
          });
        }
      });
    },
    /** 移除员工操作 */
    handleDelete(row) {
      const userIds = row.Id || this.ids;
      this.$modal.confirm('是否确认移除用户编号为"' + userIds + '"的员工？')
        .then(function () {
          return delMember({ id: userIds });
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch(() => { });
    },
    /** 导出按钮操作 */
    handleExport() {
      const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出所有用户数据项？")
        .then(() => {
          this.exportLoading = true;
          return exportUser(queryParams);
        })
        .then(() => {
          this.exportLoading = false;
        })
        .catch(() => { });
    },
    cancelForm() {
      this.dialogVisible = false
      this.getList();
    }
  }
};
</script>
<style rel="stylesheet/scss" lang="scss" scoped>

@import "~@/assets/styles/element-variables.scss";

.app-container {
  padding-right: 30px;
}

.el-tree--highlight-current .el-tree-node.is-current>.el-tree-node__content {
  background-color: $--color-primary !important;
  color: #fff !important;
}

.set_radius {
  height: 36px;
  line-height: 36px;
  border-radius: 4px;
  vertical-align: middle;
  // width: 232px;

  input {
    // width: 232px;
    border-radius: 4px;
  }
}

// .set_radius.vue-treeselect--focused{
// border: 1px solid #1890FF;
// }
.set_radius .vue-treeselect__control {
  border-radius: 4px;
  height: 38px;
  line-height: 38px;
}

.vue-treeselect__menu {
  font-weight: normal !important;
}

::v-deep .el-form-item__content {
  line-height: normal;
}

::v-deep .set_radius {
  // width: 204px;
  line-height: 38px;

  .vue-treeselect__placeholder {
    line-height: 38px;
  }

  .vue-treeselect__control {
    height: 38px;
  }
}

.set_radius .vue-treeselect--single .vue-treeselect__input {
  height: 38px;
}

.set_radius .vue-treeselect__label-container .vue-treeselect__label {
  font-weight: normal;
  color: #606266;
}

.add_dialog_border>.el-dialog {
  border: 1px solid #e4e4e5;

  .el-dialog__body {
    border-top: 1px solid #e4e4e5;
  }

  .tipsSelect {
    width: 73%;

    input {
      height: 40px;
      line-height: 40px;
    }
  }

  .link_button {
    background: rgba(61, 185, 143, 1);
    width: 24%;
    height: 40px;
    margin-left: 3%;
    border: none;
  }

  .dialog_content_con {
    .date_prompt {
      background-color: transparent;
      border: 1px solid rgba(255, 255, 255, 0.6);
      padding: 15px 26px 15px 16px;
      border-radius: 4px;
      margin-top: 18px;
    }

    .account_add {
      border: 1px solid rgba(61, 185, 143, 1) !important;
      border-radius: 4px;

      input.el-input__inner {
        border: none;
        border-right: 1px solid rgba(255, 255, 255, 0.2);
        // border-radius: 10px 0 0 10px;
        border-radius: 4px;
      }

      // .el-input-group__append {
      //   background-color: #fff;
      //   border: none;
      //   border-radius: 0 10px 10px 0;
      // }
    }
  }
}

.add_invitation_border {
  .el-dialog__body {
    border-top: 1px solid rgba(255, 255, 255, 0.2);
    border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    // background-color: #f0f2f5;
  }

  .dialog-title {
    display: flex;
    justify-content: space-between;
    color: #ffffff;
  }

  .account_add {
    border: 1px solid rgba(61, 185, 143, 1) !important;
    border-radius: 4px;
    width: 100%;
    // background-color: #ffffff;

    input.el-input__inner {
      border: none;
      border-radius: 4px 0 0 4px;
      // border-radius: 4px;
      width: 85%;
    }

    .el-input-group__append {
      // background-color: #fff;
      border: none;
      border-radius: 0 4px 4px 0;
      width: 15%;

      .el-input {
        text-align: center;
      }

      input {
        padding: 0;
        width: 85%;
      }
    }
  }

  .invite_tips {
    height: 302px;
    // display: flex;
    // align-items: center;
    // justify-content: center;
    width: 100%;

    .tips_con {
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .daiyaoqing {
      padding: 0;
      margin: 0;
      margin-top: 10px;

      li {
        list-style: none;
        padding-top: 30px;
        position: relative;
        font-size: 14px;
        // background-color: #ffffff;
        border-radius: 10px;
        border: 1px solid rgba(255, 255, 255, 0.2);

        div.input_float_top {
          display: flex;
          align-items: center;
          flex-direction: column;
          justify-content: center;
          margin-bottom: 30px;
        }

        div.input_float_bottom {
          display: flex;
          align-items: center;
          justify-content: center;
          flex-direction: column;
          text-align: center;

          .el-input {
            margin-bottom: 30px;

            input {
              // border: none;
              // border-left: 1px solid #E4E4E5;
              border-radius: 4px;
              text-align: center;
            }

            input.el-input__inner {
              padding: 0 30px 0 10px;
            }
          }

          .vue-treeselect {
            display: flex;
            align-items: center;
            justify-content: center;
          }

          .vue-treeselect__input {
            display: flex;
            align-items: center;
            text-align: center;
            height: 34px;
            line-height: 34px;
          }

          .vue-treeselect__single-value,
          .vue-treeselect__placeholder {
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100%;
            line-height: 100%;
          }

          .vue-treeselect__placeholder.vue-treeselect-helper-hide {
            display: none;
          }

          .vue-treeselect__label-container .vue-treeselect__label {
            font-weight: normal;
            color: #ffffff;
          }
        }
      }
    }
  }
}
</style>