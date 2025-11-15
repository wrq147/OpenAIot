<template>
  <div style="padding:10px 10px 0 10px; height: 100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          class="biaodan"
          ref="queryForm"
          :inline="true"
        >
          <el-form-item label="应用名称" prop="Name">
            <el-input
              class="set_radius"
              v-model="queryParams.Name"
              placeholder="请输入应用名称"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery"
              >重置</el-button
            >
            <el-button type="primary" icon="el-icon-search" @click="handleQuery"
              >搜索</el-button
            >
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div
        class="elbiaoge_elform"
        :style="{ 'min-height': tableConHeight + 'px' }"
      >
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left: 6px">新增</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar
            :showSearch.sync="showSearch"
            @queryTable="getList"
          ></right-toolbar>
        </el-row>

        <el-table v-if="refreshTable" border v-loading="loading" :data="dataList" class="data_table"
          :header-cell-style="cellSty" style="width: 100%" row-key="AppId">
          <el-table-column prop="Name" label="应用名称" align="left" width="260"></el-table-column>
          <el-table-column prop="AppId" label="AppId" align="center"></el-table-column>
          <el-table-column prop="AppSecret" label="AppSecret" align="center"></el-table-column>
          <el-table-column label="账号类型" align="center">
            <template slot-scope="scope">
              <span v-if="scope.row.AccType == 'js'">公众号</span>
              <span v-else-if="scope.row.AccType == 'applet'">小程序</span>
              <span v-else-if="scope.row.AccType == 'corp'">企业微信</span>
            </template>
          </el-table-column>
          <el-table-column prop="OrgName" label="绑定企业" align="center" width="260"></el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="258">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-view" @click="handleLook(scope.row)">查看</el-button>
              <el-button type="text" icon="el-icon-refresh" @click="handleSynchronous(scope.row)">同步</el-button>
              <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- 添加或修改部门对话框 -->
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="600px"
        append-to-body
      >
        <el-form ref="form" :model="form" :rules="rules" label-width="120px">
          <el-row>
            <el-col :span="24">
              <el-form-item label="应用名称" prop="Name">
                <el-input v-model="form.Name" placeholder="请输入应用名称" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="AppId" prop="AppId">
                <el-input v-model="form.AppId" placeholder="请输入AppId" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="AppSecret" prop="AppSecret">
                <el-input
                  v-model="form.AppSecret"
                  placeholder="请输入AppSecret"
                />
              </el-form-item>
            </el-col>
            <el-col :span="24" v-if="form.AccType == 'corp'">
              <el-form-item label="AgentId" prop="AgentId">
                <el-input type="number" v-model="form.AgentId" placeholder="请输入AgentId" />
              </el-form-item>
            </el-col>
            <el-col :span="24" v-if="form.AccType == 'corp'">
              <el-form-item label="绑定组织" prop="OrgId">
                <el-select
                  v-model="form.OrgId"
                  filterable
                  remote
                  reserve-keyword
                  placeholder="请输入绑定组织"
                  :remote-method="searchOrg"
                  :loading="uidloading"
                  :clearable="true"
                  @change="choiceBindOrg"
                >
                  <el-option
                    v-for="item in UserOptions"
                    :key="item.Id"
                    :label="item.OrgName"
                    :value="item.Id"
                  >
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="账号类型" prop="AccType">
                <el-radio-group v-model="form.AccType">
                  <el-radio label="js">公众号</el-radio>
                  <el-radio label="applet">小程序</el-radio>
                  <el-radio label="corp">企业微信</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
      <syncWx ref="syncWx" @handleMoveSync="handleMoveSync" @handleInfo="handleInfo"></syncWx>
      <handSync ref="handSync"></handSync>
      <lookDetail ref="lookDetail" :dialog-visible="dialogVisible" :lookData="lookData" @cancelForm="cancelForm"></lookDetail>
    </div>
  </div>
</template>
  
  <script>
import { listWx, AddWx, delWx } from "@/api/system/wx";
import { searchOrg } from "@/api/system/Employee";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import syncWx from './syncWx.vue'
import handSync from './handSync.vue'
import lookDetail from './lookDetail'
export default {
  name: "WxList",
  mixins: [resizeTableCon],
  components:{ syncWx, handSync, lookDetail },
  data() {
    return {
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 表格树数据
      dataList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 重新渲染表格状态
      refreshTable: true,
      // 是否展开
      expand: false,
      // 查询参数
      queryParams: {
        Name: undefined,
      },
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        Name: [
          { required: true, message: "应用名称不能为空", trigger: "blur" },
        ],
        AppId: [{ required: true, message: "AppId不能为空", trigger: "blur" }],
        AppSecret: [
          { required: true, message: "AppSecret不能为空", trigger: "blur" },
        ],
        AccType: [
          { required: true, message: "账号类型不能为空", trigger: "blur" },
        ],
      },
      //绑定企业
      uidloading: false,
      UserOptions: [],
      dialogVisible: false,  // 查看详情
      lookData: {} // 查看详情数据
    };
  },
  created() {
    this.getList();
  },
  methods: {
    handleMoveSync(id){
      this.$refs.handSync.openDialog(id)
    },
    handleInfo(info){
      this.$refs.handSync.handleInfo(info)
    },
    handleSynchronous(row){
      //同步企业微信
      this.$refs.syncWx.openDialog(row.AppId)
    },
    choiceBindOrg(value){
      console.log('选中的值',value);
      let filterArr=this.UserOptions.filter(row=>row.Id==value)
      if(filterArr&&filterArr.length>0){
        this.form.OrgName=filterArr[0].OrgName
      }
      
    },
    searchOrg(query) {
      if (query !== "") {
        this.uidloading = true;
        searchOrg({ key: query }).then((res) => {
          this.UserOptions = res.data;
          this.uidloading = false;
        });
      } else {
        this.UserOptions = [];
      }
    },
    /** 查询微信应用列表 */
    getList() {
      this.loading = true;
      listWx().then((response) => {
        if (this.$isNotEmpty(this.queryParams.Name)) {
          this.dataList = response.data.filter((x) => {
            return x.Name.indexOf(this.queryParams.Name) != -1;
          });
        } else {
          this.dataList = response.data;
        }
        this.loading = false;
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
        Name: "",
        AppId: "",
        AppSecret: "",
        AgentId: "",
        OrgId: undefined,
        OrgName: "",
        AccType: "applet",
      };
      this.resetForm("form");
    },
    /** 搜索按钮操作 */
    handleLook(item) {
      this.lookData = item;
      this.dialogVisible = true;
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    /** 新增按钮操作 */
    handleAdd(row) {
      this.reset();
      this.open = true;
      this.title = "添加微信应用";
    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          if(this.form){}else{
            this.form=0
          }
          AddWx(this.form).then((response) => {
            this.$modal.msgSuccess("新增成功");
            this.open = false;
            this.getList();
          });
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      this.$modal
        .confirm('是否确认删除名称为"' + row.Name + '"的数据项？')
        .then(function () {
          return delWx(row.AppId);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    cancelForm() {
      this.dialogVisible = false;
    }
  },
};
</script>
  