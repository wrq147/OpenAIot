<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form
              class="biaodan"
              :model="queryParams"
              ref="queryForm"
              :inline="true"
            >
              <el-form-item label="搜索关键词" prop="key">
                <el-input
                  class="set_radius"
                  v-model="queryParams.key"
                  placeholder="请输入要搜索的用户名、手机、昵称或企业名"
                  clearable
                  @keyup.enter.native="handleQuery"
                />
              </el-form-item>
              <el-form-item label="状态" prop="status">
                <el-select
                  class="set_radius"
                  v-model="queryParams.status"
                  placeholder="用户状态"
                  clearable
                >
                  <el-option
                    v-for="dict in dict.type.sys_normal_disable"
                    :key="dict.value"
                    :label="dict.label"
                    :value="dict.value"
                  />
                </el-select>
              </el-form-item>
              <el-form-item label="创建时间">
                <el-date-picker
                  class="set_radius"
                  v-model="dateRange"
                  style="width:232px"
                  value-format="yyyy-MM-dd"
                  type="daterange"
                  range-separator="-"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                ></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button
                    type="primary"
                    plain
                    @click="handleAdd"
                    v-hasPermi="['/AuthService/User/Add']"
                  >
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">新增</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button
                    type="success"
                    plain
                    :disabled="single"
                    @click="handleUpdate"
                    v-hasPermi="['/AuthService/User/Edit']"
                  >
                  <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                    <span style="margin-left:6px">修改</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button
                    type="danger"
                    plain
                    :disabled="single"
                    @click="handleDelete"
                    v-hasPermi="['/AuthService/User/Remove']"
                  >
                  <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    <span style="margin-left:6px">删除</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button
                    type="info"
                    plain
                    @click="handleImport"
                    v-hasPermi="['/AuthService/User/Import']"
                  >
                  <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left:6px">导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button
                    type="warning"
                    plain
                    :loading="exportLoading"
                    @click="handleExport"
                    v-hasPermi="['/AuthService/User/Export']"
                  >
                  <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table
              v-loading="loading"
              :data="userList"
              :row-style="isRed"
              @selection-change="handleSelectionChange"
              class="data_table"
              :header-cell-style="cellSty"
              style="width:100%"
            >
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column
                label="用户编号"
                align="center"
                key="Id"
                prop="Id"
                v-if="columns[0].visible"
                width="200"
              />
              <el-table-column
                label="用户名称"
                align="center"
                key="UserName"
                prop="UserName"
                v-if="columns[1].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="用户昵称"
                align="center"
                key="RealName"
                prop="RealName"
                v-if="columns[2].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="所在企业"
                align="center"
                key="OrgNames"
                prop="OrgNames"
                width="250"
                v-if="columns[3].visible"
                :show-overflow-tooltip="true"
              />
              <el-table-column
                label="手机号码"
                align="center"
                key="Mobile"
                prop="Mobile"
                v-if="columns[4].visible"
                width="150"
              />
              <el-table-column label="邮箱" align="center" key="Email" prop="Email" v-if="columns[5].visible" />
              <el-table-column
                label="状态"
                align="center"
                key="status"
                v-if="columns[6].visible"
                width="118"
              >
                <template slot-scope="scope">
                  <el-switch
                    v-model="scope.row.status"
                    active-value="0"
                    inactive-value="1"
                    @change="handleStatusChange(scope.row)"
                  ></el-switch>
                </template>
              </el-table-column>
              <el-table-column
                label="创建时间"
                align="center"
                prop="createTime"
                v-if="columns[7].visible"
                width="240"
              >
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                align="center"
                width="248"
                class-name="small-padding fixed-width"
              >
                <template slot-scope="scope" v-if="scope.row.Id>2">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)" v-hasPermi="['/AuthService/User/Edit']">修改</el-button>
                  <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" v-hasPermi="['/AuthService/User/Remove']">删除</el-button>
                  <el-dropdown @command="(command) => handleCommand(command, scope.row)" v-hasPermi="['/AuthService/User/ResetPwd', '/AuthService/User/Edit']">
                    <el-button type="text" class="el-dropdown-link">
                      <i class="zhongtaiiconfont zhongtai-icon-gengduo" style="margin-right2px"></i>更多
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                      <el-dropdown-item command="handleResetPwd" icon="el-icon-key" v-hasPermi="['/AuthService/User/ResetPwd']">重置密码</el-dropdown-item>
                      <el-dropdown-item command="handleRole" icon="el-icon-s-custom">分配角色</el-dropdown-item>
                      <el-dropdown-item command="handleLoginBySys" icon="el-icon-bank-card">登录</el-dropdown-item>
                    </el-dropdown-menu>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>

            <pagination
              v-show="total > 0"
              :total="total"
              :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize"
              @pagination="getList"
            />
          </div>
        </el-col>
      </el-row>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="600px"
        append-to-body
      >
        <el-form ref="form" :model="form" :rules="rules" label-width="80px">
          <el-row>
            <el-col :span="12">
              <el-form-item label="用户昵称" prop="RealName">
                <el-input v-model="form.RealName" placeholder="请输入用户昵称" maxlength="20" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item v-if="form.Id == undefined" label="用户名称" prop="UserName">
                <el-input v-model="form.UserName" placeholder="请输入用户名称" maxlength="30" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item v-if="form.Id == undefined" label="用户密码" prop="Password">
                <el-input
                  v-model="form.Password"
                  placeholder="请输入用户密码"
                  type="password"
                  maxlength="20"
                  show-password
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="12">
              <el-form-item label="用户性别">
                <el-select v-model="form.Sex" placeholder="请选择">
                  <el-option
                    v-for="dict in dict.type.sys_user_sex"
                    :key="dict.value"
                    :label="dict.label"
                    :value="dict.value"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="状态">
                <el-radio-group v-model="form.status">
                  <el-radio
                    v-for="dict in dict.type.sys_normal_disable"
                    :key="dict.value"
                    :label="dict.value"
                  >{{ dict.label }}</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.Introduction" type="textarea" placeholder="请输入内容"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>

      <!-- 用户导入对话框 -->
      <el-dialog
        :close-on-click-modal="false"
        :title="upload.title"
        :visible.sync="upload.open"
        width="400px"
        append-to-body
      >
        <el-upload
          ref="upload"
          :limit="1"
          accept=".xlsx, .xls"
          :headers="upload.headers"
          :action="upload.url + '?updateSupport=' + upload.updateSupport"
          :disabled="upload.isUploading"
          :on-progress="handleFileUploadProgress"
          :on-success="handleFileSuccess"
          :auto-upload="false"
          drag
        >
          <i class="el-icon-upload"></i>
          <div class="el-upload__text">
            将文件拖到此处，或
            <em>点击上传</em>
          </div>
          <div class="el-upload__tip text-center" slot="tip">
            <div class="el-upload__tip" slot="tip">
              <el-checkbox v-model="upload.updateSupport" />是否更新已经存在的用户数据
            </div>
            <span style="line-height: 33px;">仅允许导入xls、xlsx格式文件。</span>
            <el-link
              type="primary"
              :underline="false"
              style="font-size:12px;vertical-align: baseline;"
              @click="onImportTemplate"
            >下载模板</el-link>
          </div>
        </el-upload>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitFileForm">确 定</el-button>
          <el-button @click="upload.open = false">取 消</el-button>
        </div>
      </el-dialog>

 <!-- 角色分配对话框 -->
  <el-dialog
    title="分配角色"
    :close-on-click-modal="false"
    :visible.sync="roleDialogOpen"
    width="900px"
    append-to-body
  >
    <div style="padding: 15px;">
      <!-- 已分配角色表格 -->
      <el-table
        :data="assignedRoles"
        border
        stripe
        style="width: 100%; margin-bottom: 15px"
      >
        <el-table-column prop="RoleName" label="角色名称"></el-table-column>
        <el-table-column prop="OrgName" label="所属企业"></el-table-column>
        <el-table-column prop="RoleDesc" label="角色描述"></el-table-column>
        <el-table-column label="操作" width="100" align="center">
          <template slot-scope="scope">
            <el-button
              type="text"
              size="small"
              @click="removeRole(scope.row)"
              v-if="scope.row.IsSystem=='1'"
            >
              <i class="el-icon-delete"></i>删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-form :inline="true" :model="formInline">
        <el-form-item label="系统角色" required>
          <el-select v-model="formInline.roleId" placeholder="请选择">
            <el-option
              v-for="item in systemRoles"
              :key="item.roleId"
              :label="item.roleName"
              :value="item.roleId">
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="所属企业">
          <el-select v-model="formInline.orgId" placeholder="请选择">
            <el-option v-for="item in userOrgs"
            :key="item.Id"
              :label="item.OrgName"
              :value="item.Id"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="addRole">添加</el-button>
        </el-form-item>
      </el-form>
    
    </div>
    
  </el-dialog>

    </div>
  </div>
</template>

<script>
import {
  listUser,
  listUserRole,
  listUserOrg,
  listSystemRoles,
  AddSysRole,
  delUserRole,
  getUser,
  delUser,
  addUser,
  updateUser,
  exportUser,
  resetUserPwd,
  changeUserStatus,
  importTemplate,
  LoginBySys
} from "@/api/system/user";
import { getToken,setToken,setRefreshToken } from "@/utils/auth";
import { resizeTableCon } from "@/mixins/resizeTableCon";

export default {
  name: "User",
  dicts: ["sys_normal_disable", "sys_user_sex"],
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
      // 用户表格数据
      userList: null,
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 默认密码
      initPassword: undefined,
      // 日期范围
      dateRange: [],
      // 表单参数
      form: {},
      defaultProps: {
        children: "children",
        label: "label"
      },
      // 用户导入参数
      upload: {
        // 是否显示弹出层（用户导入）
        open: false,
        // 弹出层标题（用户导入）
        title: "",
        // 是否禁用上传
        isUploading: false,
        // 是否更新已经存在的用户数据
        updateSupport: false,
        // 设置上传的请求头部
        headers: { Authorization: getToken() },
        // 上传的地址
        url: process.env.VUE_APP_BASE_API + "AuthService/User/Import"
      },
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        key: undefined,
        status: undefined
      },
      // 列信息
      columns: [
        { key: 0, label: `用户编号`, visible: true },
        { key: 1, label: `用户名称`, visible: true },
        { key: 2, label: `用户昵称`, visible: true },
        { key: 3, label: `部门`, visible: true },
        { key: 4, label: `手机号码`, visible: true },
        { key: 5, label: `邮箱`, visible: true },
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
        roleIds: [{ required: true, message: "请选择角色", trigger: "change" }],
        RealName: [
          { required: true, message: "用户昵称不能为空", trigger: "blur" }
        ],
        Password: [
          { required: true, message: "用户密码不能为空", trigger: "blur" },
          {
            min: 5,
            max: 20,
            message: "用户密码长度必须介于 5 和 20 之间",
            trigger: "blur"
          }
        ]
      },
      roleDialogUserId:0,
      roleDialogOpen:false,
      assignedRoles:[],
      userOrgs:[],
      systemRoles:[],
      formInline:{
        roleId:null,
        orgId:null
      }
    };
  },
  created() {
    this.getList();
    this.getConfigKey("sys.user.initPassword").then(response => {
      this.initPassword = response.msg;
    });
  },
  computed: {
    CurOrgId() {
      return this.$store.getters.orgId;
    }
  },
  methods: {
    /** 查询用户列表 */
    getList() {
      this.loading = true;
      listUser(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.userList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 用户状态修改
    handleStatusChange(row) {
      let text = row.status === "0" ? "启用" : "停用";
      this.$modal
        .confirm('确认要"' + text + '""' + row.UserName + '"用户吗？')
        .then(function() {
          return changeUserStatus(row.Id, row.status);
        })
        .then(() => {
          this.$modal.msgSuccess(text + "成功");
        })
        .catch(function() {
          row.status = row.status === "0" ? "1" : "0";
        });
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        Id: undefined,
        UserName: undefined,
        RealName: undefined,
        Password: undefined,
        Sex: "2",
        status: "0",
        Introduction: undefined
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
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    
    // 更多操作触发
    handleCommand(command, row) {
      switch (command) {
        case "handleResetPwd":
          this.handleResetPwd(row);
          break;
        case "handleLoginBySys":
          this.handleLoginBySys(row);
          break;
        case "handleRole":
          this.handleRole(row);
          break;
        default:
          break;
      }
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      getUser().then(response => {
        this.open = true;
        this.title = "添加用户";
        this.form.Password = this.initPassword;
      });
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const userId = row.Id || this.ids;
      getUser(userId).then(response => {
        this.form = response.data.user;
        this.form.postIds = response.data.user.postIds;
        this.open = true;
        this.title = "修改用户";
        this.form.Password = "";
      });
    },
    /** 重置密码按钮操作 */
    handleResetPwd(row) {
      this.$prompt('请输入"' + row.UserName + '"的新密码', "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnClickModal: false,
        inputPattern: /^.{5,20}$/,
        inputErrorMessage: "用户密码长度必须介于 5 和 20 之间",
        closeOnClickModal:false
      })
        .then(({ value }) => {
          resetUserPwd(row.Id, value).then(response => {
            this.$modal.msgSuccess("修改成功，新密码是：" + value);
          });
        })
        .catch(() => {});
    },
    handleLoginBySys(row){
      LoginBySys({id:row.Id}).then(res=>{
        // console.log("登录数据",res);
        setToken(res.data.token)
        setRefreshToken(res.data.refresh_token)
        let ext_info=res.data.ext_info
        this.$store.commit('SET_UID', ext_info.Id.toString());
        this.$store.commit('SET_PERMISSIONS', ext_info.permissions)
        this.$store.commit('SET_ROLES', ['ROLE_DEFAULT'])
        this.$store.commit('SET_NAME', ext_info.name)
        this.$store.commit('SET_AVATAR', ext_info.avatar)
        this.$store.commit('SET_ORGID',ext_info.OrgId)
        let url = window.location.protocol + "//" + window.location.host;
          window.location.replace(url); //登录后刷新
      })
    },
    async handleRole(row){
      this.formInline={
        roleId:null,
        orgId:null
      };
      this.roleDialogOpen=true;
      this.roleDialogUserId=row.Id;
      let tmpRes=await listUserRole(row.Id);
      this.assignedRoles=tmpRes.data;
      tmpRes=await listUserOrg(row.Id);
      this.userOrgs=tmpRes.data;
      tmpRes=await listSystemRoles();
      this.systemRoles=tmpRes.data;
    },
    removeRole(row){
       this.$modal.confirm('确认要删除"' + row.RoleName + '"吗？')
        .then(()=>{
          return delUserRole({userId:this.roleDialogUserId,roleId:row.RoleID,orgId:row.OrgId});
        })
        .then(() => {
          this.$modal.msgSuccess("操作成功");
          listUserRole(this.roleDialogUserId).then((tmpRes)=>{
            this.assignedRoles=tmpRes.data;
          });
        });
    },
    async addRole(){
      this.formInline.userId=this.roleDialogUserId;
      if(this.formInline.roleId==null){
        this.$message.error("请选择角色");
        return;
      }
      if(this.formInline.orgId==null){
        this.formInline.orgId=0;
      }
      let tmpRes=await AddSysRole(this.formInline);
      this.$message.success("添加成功");
      tmpRes=await listUserRole(this.roleDialogUserId);
      this.assignedRoles=tmpRes.data;
      this.formInline={
        roleId:null,
        orgId:null
      };
    },
    /** 提交按钮 */
    submitForm: function() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          if (this.form.Id != undefined) {
            updateUser(this.form).then(response => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });
          } else {
            addUser(this.form).then(response => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });
          }
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const userIds = row.Id || this.ids;
      this.$modal
        .confirm('是否确认删除用户编号为"' + userIds + '"的数据项？')
        .then(function() {
          return delUser(userIds);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
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
        .catch(() => {});
    },
    /** 导入按钮操作 */
    handleImport() {
      this.upload.title = "用户导入";
      this.upload.open = true;
    },
    /** 下载模板操作 */
    onImportTemplate() {
      importTemplate();
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    // 文件上传成功处理
    handleFileSuccess(response, file, fileList) {
      this.upload.open = false;
      this.upload.isUploading = false;
      this.$refs.upload.clearFiles();
      this.$alert(response.message, "导入结果", { dangerouslyUseHTMLString: true });
      this.getList();
    },
    // 提交上传文件
    submitFileForm() {
      this.$refs.upload.submit();
    }
  }
};
</script>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";
// .app-container {
//   padding-right: 30px;
// }
.el-tree--highlight-current .el-tree-node.is-current > .el-tree-node__content {
  background-color: $--color-primary !important;
  color: #fff !important;
}

.set_radius {
  height: 40px;
  line-height: 40px;
  border-radius: 4px;
  vertical-align: middle;
  width: 232px;

  input {
    width: 232px;
    border-radius: 4px;
  }
}

</style>