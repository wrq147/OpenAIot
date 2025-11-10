<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" ref="queryForm" class="biaodan" :inline="true">
          <el-form-item label="应用所属">
            <el-select v-model="queryParams.OrgId" filterable remote reserve-keyword @change="chgOrgId"
              :clearable="true" placeholder="请输入要搜索的企业名称" :remote-method="searchToolOrg" :loading="xxloading">
              <el-option v-for="item in SearchUserOrgList" :key="item.Id" :label="item.OrgName" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="应用主题">
            <el-select v-model="queryParams.StyleId" filterable remote reserve-keyword @change="chgThemeId"
              :clearable="true" placeholder="请输入要搜索的主题名称" :remote-method="searchToolTheme" :loading="themeloading">
              <el-option label="系统" value=" "></el-option>
              <el-option v-for="item in SearchUserThemeList" :key="item.Id" :label="item.Name" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="搜索内容" prop="SearchKey">
            <el-input class="set_radius" v-model="queryParams.SearchKey" placeholder="请输入搜索版本、标题、内容" clearable />
          </el-form-item>

          <el-form-item label="创建日期">
            <el-date-picker class="set_radius" v-model="dateRange" style="width: 250px" value-format="yyyy-MM-dd"
              type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-dropdown @command="onNewPublic">
                <el-button type="primary">
                  <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                  发布新版<i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="app">原生App安装包</el-dropdown-item>
                  <el-dropdown-item command="wgt" divided>wgt资源包</el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain @click="handleDelete">
                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left: 6px">批量删除</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" border :data="dataList" class="data_table"
          @selection-change="handleSelectionChange" :header-cell-style="cellSty" style="width: 100%">
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="编号" width="80" align="center" prop="Id" />
          <el-table-column label="应用所属" align="center" width="220">
            <template slot-scope="scope">
              <span v-if="scope.row.OrgId == 0">系统</span>
              <span v-else>{{ scope.row.OrgName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="更新标题" align="center" prop="Title" :show-overflow-tooltip="true" />
          <el-table-column label="安装包类型" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.PackageType == 0">原生App安装包</el-tag>
              <el-tag v-else-if="scope.row.PackageType == 1" type="success">wgt资源包</el-tag>
              <el-tag v-else type="danger">未知类型</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="平台" align="center" prop="Platform" width="180" />
          <el-table-column label="版本号" align="center" prop="UpVersion" width="180" />
          <el-table-column label="安装包状态" align="center">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.IsPublish == true" type="success">已上线</el-tag>
              <el-tag v-else type="info">待上线</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="上传时间" align="center" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.CreatedOn) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-document" @click="handleDetail(scope.row)">详情</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize" @pagination="getList" />
      </div>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="600px" append-to-body>
        <el-form ref="form" :disabled="readOnly" :model="form" :rules="rules" label-width="80px" style="width: 500px;">
          <el-form-item label="应用所属">
            <el-select :disabled="FilterOrg != null" style="width:260px;" v-model="form.OrgId" :clearable="true" @change="searchOrg"
              filterable remote reserve-keyword placeholder="请输入要搜索的企业名称" :remote-method="searchOrg"
              :loading="yyloading">
              <el-option v-for="item in UserOrgList" :key="item.Id" :label="item.OrgName" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="应用主题" prop="StyleId">
            <el-select style="width:260px;" v-model="form.StyleId" :clearable="true"  @change="searchTheme"
              filterable remote reserve-keyword placeholder="请输入要搜索的主题名称" :remote-method="searchTheme"
              :loading="themeloading">
              <el-option label="系统" value=" "></el-option>
              <el-option v-for="item in UserThemeList" :key="item.Id" :label="item.Name" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="包类型">
            <span v-if="form.PackageType == 0" style="color: #999;">原生App安装包</span>
            <span v-else-if="form.PackageType == 1" style="color: #999;">wgt资源包</span>
          </el-form-item>
          <el-form-item label="更新标题" prop="Title">
            <el-input v-model="form.Title" placeholder="请输入更新标题" />
          </el-form-item>
          <el-form-item label="更新内容" prop="UpContent">
            <el-input type="textarea" :rows="2" placeholder="请输入更新内容" v-model="form.UpContent">
            </el-input>
          </el-form-item>
          <el-form-item label="发布平台" prop="Platform">
            <el-radio-group @input="$forceUpdate()" v-if="form.PackageType == 0" v-model="form.Platform">
              <el-radio label="android">android</el-radio>
              <el-radio label="ios">ios</el-radio>
            </el-radio-group>
            <el-checkbox-group v-else @change="$forceUpdate()" v-model="form.PlatformArrary">
              <el-checkbox label="android"></el-checkbox>
              <el-checkbox label="ios"></el-checkbox>
            </el-checkbox-group>
          </el-form-item>
          <el-form-item label="当前版本" prop="UpVersion">
            <el-input v-model="form.UpVersion" placeholder="当前包版本号，必须大于当前线上发行版本号" />
          </el-form-item>
          <el-form-item v-if="form.PackageType == 1" label="原生App最低版本" prop="MinAppVersion">
            <el-input v-model="form.MinAppVersion" placeholder="请输入原生App最低版本" />
          </el-form-item>
          <el-form-item v-if="form.PackageType != 0 || form.Platform != 'ios'" label="上传包">
            <el-upload ref="upload" :limit="1" :accept="acceptStr" :headers="upload.headers" :action="upload.url"
              :disabled="upload.isUploading" :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess"
              drag>
              <i class="el-icon-upload"></i>
              <div class="el-upload__text">
                将文件拖到此处，或
                <em>点击上传</em>
              </div>
              <div slot="tip" style="font-size: 12px;color: #999;margin-top: -10px;">
                仅允许上传{{ acceptStr }}格式文件。
              </div>
            </el-upload>
          </el-form-item>
          <el-form-item label="包地址" prop="UpUrl">
            <el-input v-model="form.UpUrl" placeholder="可下载安装包地址" />
          </el-form-item>

          <el-form-item v-if="form.PackageType == 1" label="静默更新">
            <el-switch v-model="form.IsSilently"> </el-switch>
            <el-tooltip style="margin-left:15px;" effect="dark" content="静默更新：App升级时会在后台下载wgt包并自行安装。新功能在下次启动App时生效"
              placement="right">
              <i class="el-icon-warning-outline"></i>
            </el-tooltip>
          </el-form-item>

          <el-form-item label="强制更新">
            <el-switch v-model="form.IsMandatory"> </el-switch>
            <el-tooltip style="margin-left:15px;" effect="dark" content="强制更新：升级弹出框不可取消" placement="right">
              <i class="el-icon-warning-outline"></i>
            </el-tooltip>
          </el-form-item>

          <el-form-item label="上线发行">
            <el-switch v-model="form.IsPublish"> </el-switch>
            <el-tooltip style="margin-left:15px;" effect="dark"
              content="同时只可有一个未发行版，线上发行不可更设为下线。\n未上线可以设为上线发行并自动替换当前线上发行版" placement="right">
              <i class="el-icon-warning-outline"></i>
            </el-tooltip>
          </el-form-item>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button v-if="!readOnly" type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  listUpgrade,
  getUpgrade,
  addUpgrade,
  delUpgrade,
} from "@/api/system/upgrade";
import {
  styleManList,
} from "@/api/system/StyleMan";
import { getToken } from "@/utils/auth";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  searchOrg
} from "@/api/system/Employee";
export default {
  name: "UpgradeList",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
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
      // 参数表格数据
      dataList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        orderByColumn: "Id",
        isAsc: "desc",
        pageNum: 1,
        pageSize: 10,
        SearchKey: undefined,
        OrgId: 0,
        StyleId:' '
      },
      readOnly: false,
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        Title: [
          { required: true, message: "更新标题不能为空", trigger: "blur" },
        ],
        UpContent: [
          { required: true, message: "更新内容不能为空", trigger: "blur" },
        ],
        UpVersion: [
          { required: true, message: "当前版本不能为空", trigger: "blur" },
        ],
        MinAppVersion: [
          { required: true, message: "App最低版本不能为空", trigger: "blur" },
        ],
        Platform: [
          { required: true, message: '请选择发布平台', trigger: ['change', 'input'] }
        ],
        UpUrl: [
          { required: true, message: '包地址不能为空', trigger: "blur" }
        ],
        // StyleId: [
        //   { required: true, message: '所属主题不能为空', trigger: 'change' }
        // ],
      },
      // 文件上传参数
      upload: {
        // 是否禁用上传
        isUploading: false,
        // 设置上传的请求头部
        headers: { Authorization: getToken() },
        // 上传的地址
        url:
          process.env.VUE_APP_BASE_API == "/"
            ? "/AuthService/File/Upload?withDomain=true"
            : process.env.VUE_APP_BASE_API +
            "/AuthService/File/Upload?withDomain=true",
      },
      SearchUserOrgList: [{ "Id": 0, "OrgName": "系统" }],
      UserOrgList: [{ "Id": 0, "OrgName": "系统" }],
      FilterOrg: { "Id": 0, "OrgName": "系统" },
      SearchUserThemeList: [],
      UserThemeList: [],
      FilterTheme: null,
      xxloading: false,
      yyloading: false,
      themeloading: false,
    };
  },
  computed: {
    acceptStr() {
      if (this.form.PackageType == 0) {
        return ".apk";
      } else {
        return ".wgt";
      }
    },
  },
  created() {
    this.getList();
  },
  methods: {
    chgOrgId(val) {
      this.$nextTick(()=>{
        if (val !== "") {
          this.FilterOrg = this.SearchUserOrgList.find(x => x.Id == val);
        }
        else {
          this.FilterOrg = null;
        }
        this.searchToolOrg(val);
        this.getList();
      })
    },
    searchToolOrg(query) {
        this.xxloading = true;
        searchOrg({ key: query }).then(res => {
          this.SearchUserOrgList = res.data;
          this.SearchUserOrgList.unshift({ "Id": 0, "OrgName": "系统" });
          this.xxloading = false;
        });
    },
    searchOrg(query) {
        this.yyloading = true;
        searchOrg({ key: query }).then(res => {
          this.UserOrgList = res.data;
          this.UserOrgList.unshift({ "Id": 0, "OrgName": "系统" });
          this.yyloading = false;
        });
    },
    chgThemeId(val) {
      this.$nextTick(()=>{
        this.searchToolTheme(val);
        this.getList();
      })
    },
    searchToolTheme(query) {
        this.themeloading = true;
        styleManList({ Name: query }).then(res => {
          this.SearchUserThemeList = res.data.List;
          this.themeloading = false;
        });

    },
    searchTheme(query) {//主题列表
        this.themeloading = true;
        styleManList({ Name: query }).then(res => {
          this.UserThemeList = res.data.List;
          this.themeloading = false;
        });
    },
    /** 查询参数列表 */
    getList() {
      this.loading = true;
      listUpgrade(this.addDateRange(this.queryParams, this.dateRange)).then(
        (response) => {
          this.dataList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    // 文件上传成功处理
    handleFileSuccess(response, file, fileList) {
      if (response.code != 0) {
        this.$message.error(response.message);
        return;
      }
      this.upload.isUploading = false;
      this.$refs.upload.clearFiles();
      this.form.UpUrl = response.data;
      this.$message({
        message: "上传成功，已填入包地址",
        type: "success",
      });
    },
    onNewPublic(cmd) {
      this.open = true;
      this.readOnly = false;
      this.form = {
        UpContent: "",
        UpVersion: "",
        MinAppVersion: "",
        UpUrl: "",
        IsPublish: true,
      };
      if (this.FilterOrg != null) {
        this.form["OrgId"] = this.FilterOrg.Id;
      }
      else {
        this.form["OrgId"] = 0;
      }
      if (this.FilterTheme != null) {
        this.form["StyleId"] = this.FilterTheme.Id;
      }
      else {
        this.form["StyleId"] = ' ';
      }
      if (cmd == "app") {
        this.form["PackageType"] = 0;
        this.form["Title"] = "发布App更新";
        this.form["Platform"] = "android";
        this.form["PlatformArrary"] = ["android"];
        this.form["IsSilently"] = false;
        this.form["IsMandatory"] = true;
      } else if (cmd == "wgt") {
        this.form["PackageType"] = 1;
        this.form["Title"] = "发布wgt更新";
        this.form["Platform"] = "android,ios";
        this.form["PlatformArrary"] = ["android", "ios"];
        this.form["IsSilently"] = true;
        this.form["IsMandatory"] = false;
      }
      let form=JSON.parse(JSON.stringify(this.form))
      this.form=JSON.parse(JSON.stringify(form))
      this.resetForm("form");
    },
    // 取消按钮
    cancel() {
      this.open = false;
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
      this.ids = selection.map((item) => item.Id);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },

    /** 详情按钮操作 */
    handleDetail(row) {
      getUpgrade(row.Id).then((response) => {
        this.form = response.data;
        if(this.form.OrgId==0){
          this.UserOrgList = [{ "Id": 0, "OrgName": "系统" }];
        }
        else{
          this.UserOrgList = [{ "Id": this.form.OrgId, "OrgName": this.form.OrgName }];
        }

        console.info(  this.UserOrgList)
        this.form["PlatformArrary"] = this.form.Platform.split(",");
        this.open = true;
        this.readOnly = true;
        this.title = "发行信息";
      });
    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          addUpgrade(this.form).then((response) => {
            this.$modal.msgSuccess("发布成功");
            this.open = false;
            this.getList();
          });
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const tmpIds = row.Id || this.ids;
      this.$modal
        .confirm('是否确认删除编号为"' + tmpIds + '"的数据项？')
        .then(function () {
          return delUpgrade(tmpIds);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
  },
};
</script>
