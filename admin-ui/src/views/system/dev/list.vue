<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          ref="queryForm"
          class="biaodan"
          :inline="true"
        >
          <el-form-item label="开发者名称" prop="configName">
            <el-input
              class="set_radius"
              v-model="queryParams.configName"
              placeholder="请输入开发者名称"
              clearable
              @keyup.enter.native="handleQuery"
            />
          </el-form-item>
          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
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
                v-hasPermi="['/DeveloperService/Developer/Add']"
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
                v-hasPermi="['/DeveloperService/Developer/Edit']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left:6px">修改</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="danger"
                plain
                :disabled="multiple"
                @click="handleDelete"
                v-hasPermi="['/DeveloperService/Developer/Remove']"
              >
              <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left:6px">删除</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          v-loading="loading"
          border
          :data="datalist"
          class="data_table"
          :row-style="isRed"
          @selection-change="handleSelectionChange"
          :header-cell-style="cellSty"
          style="width:100%"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="开发者Id" align="center" prop="DevId" width="250" />
          <el-table-column
            label="开发者SecKey"
            align="center"
            prop="SecKey"
            width="320"
          />
         <el-table-column label="加密方式" align="center">
            <template slot-scope="scope">
              <el-tag type="success" v-if="scope.row.KeyType==0" >简单验证</el-tag>
              <el-tag type="info" v-else >OAuth验证</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="开发者类型" align="center">
            <template slot-scope="scope">
              <span v-if="scope.row.UserType==0">个人开发者</span>
              <span v-else>企业开发者</span>
            </template>
          </el-table-column>
          <el-table-column
            label="开发者名称"
            align="center"
            prop="DeveloperName"
          />
          <el-table-column
            label="开发者组织"
            align="center"
            prop="DeveloperOrgName"
          />
          <el-table-column label="创建时间" align="center" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.CreateOn) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['/DeveloperService/Developer/Edit']"
              >修改</el-button>
              <el-button
                type="text"
                icon="el-icon-delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['/DeveloperService/Developer/Remove']"
              >删除</el-button>
            </template>
          </el-table-column>
        </el-table>

        <pagination
          v-show="total>0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getList"
        />
      </div>

      <!-- 添加或修改参数配置对话框 -->
      <el-dialog
        :title="title"
        :close-on-click-modal="false"
        :visible.sync="open"
        width="500px"
        append-to-body
      >
        <el-form ref="form" :model="form" label-width="120px">
          <el-form-item label="加密方式" prop="KeyType">
            <el-select v-model="form.KeyType" placeholder="请选择">
              <el-option label="简单验证" :value="0"></el-option>
              <el-option label="OAuth验证" :value="1"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item v-if="form.DevId==null" label="开发者类型" prop="UserType">
            <el-radio-group v-model="form.UserType">
                <el-radio :label="0">个人开发者</el-radio>
                <el-radio :label="1">企业开发者</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="form.DevId==null" label="关联用户" prop="UserId">

            <el-select
            v-if="form.UserType==0"
            v-model="form.UserId"
              filterable
              remote
              reserve-keyword
              placeholder="通过手机、邮箱查找"
              :remote-method="selectSearchMember"
              :loading="uidloading"
              ref="selectMemberDept">
              <el-option
                v-for="member in searchMemberList"
                :key="member.Id"
                :label="member.Name"
                :value="member.Id"
                style="height:50px;line-height:50px;background:#ffffff">
                <div style="display:flex;justify-content: flex-start;align-items: center;height:50px;line-height:50px">
                  <img :src="member.Avatar" style="float: left;width: 28px;height: 28px;margin-right: 10px;"/>
                  <div style="float: left; color: #8492a6; font-size: 13px">{{ member.Name }}</div>
                </div>
              </el-option>
            </el-select>
           <el-select
        v-else
              v-model="form.OrgId"
              filterable
              remote
              reserve-keyword
              placeholder="请输入企业名称"
              :remote-method="searchOrg"
              :loading="uidloading">
              <el-option
                v-for="item in UserOptions"
                :key="item.Id"
                :label="item.OrgName"
                :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  listDev,
  addDev,
  updateDev,
  delDev,
  devInfo
} from "@/api/dev";
import {
  searchMember,
  searchOrg
} from "@/api/system/Employee";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "DevList",
  mixins: [resizeTableCon],
  data() {
    return {
      // 遮罩层
      loading: true,
      uidloading:false,
      UserOptions:[],
      searchMemberList:[],
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
      // 表格数据
      datalist: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        configName: undefined,
        configKey: undefined,
        configType: undefined
      },
      // 表单参数
      form: {},
    };
  },
  created() {
    this.getList();
  },
  methods: {
    searchOrg(query) {
        if (query !== '') {
          this.uidloading = true;
          searchOrg({key:query}).then(res => {
              this.UserOptions = res.data;
              this.uidloading = false;
          });

        } else {
          this.UserOptions = [];
        }
    },
    /** 查询参数列表 */
    getList() {
      this.loading = true;
      listDev(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          this.datalist = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    selectSearchMember(query) {
      //全局搜索指定用户
      if (query !== "") {
        this.uidloading = true;
        searchMember({ key: query }).then(res => {
            this.searchMemberList = res.data;
            this.uidloading = false;
        });
      } else {
        this.searchMemberList = [];
      }
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        KeyType: 0,
        UserType:0
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
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加开发者";
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.DevId);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      if (checkIdList.includes(row.configId)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    /** 修改按钮操作 */
    async handleUpdate(row) {
      this.reset();
      const dataId = row.DevId || this.ids;
      let response=await devInfo(dataId);
      this.form = response.data;
      this.open = true;
      this.title = "修改参数";
    },
    /** 提交按钮 */
    submitForm: function() {
      if (this.form.DevId != undefined) {
            updateDev(this.form).then(response => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });
          } else {
            addDev(this.form).then(response => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });
          }
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const dataId = row.DevId || this.ids;
      this.$modal
        .confirm('是否确认删除参数编号为"' + dataId + '"的数据项？')
        .then(function() {
          return delDev(dataId);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 刷新缓存按钮操作 */
    handleRefreshCache() {
      refreshCache().then(() => {
        this.$modal.msgSuccess("刷新成功");
      });
    }
  }
};
</script>
