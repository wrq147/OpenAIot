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
          <el-form-item label="关键字" prop="userName">
            <el-input
              class="set_radius"
              v-model="queryParams.Key"
              placeholder="请输入关键字"
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
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button
                type="primary"
                plain
                @click="openSelectUser"
              >
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">添加企业</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button
                style="font-size:12px"
                type="warning"
                plain
                icon="el-icon-close"
                @click="handleClose"
              >关闭</el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table
          v-loading="loading"
          border
          :data="orgList"
          :row-style="isRed"
          class="data_table"
          @selection-change="handleSelectionChange"
          :header-cell-style="cellSty"
          style="width:100%"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="企业编号" prop="Id" :show-overflow-tooltip="true" />
          <el-table-column label="企业名称" prop="OrgName" :show-overflow-tooltip="true" />
          <el-table-column label="Logo" align="center" prop="Email" :show-overflow-tooltip="true" >
            <template slot-scope="scope">
              <div style="width: 100%;display:flex;justify-content:center;align-items:center">
                <div class="imgwrap" style="max-width: 60px;max-height:60px;">
                    <el-image fit="cover" :src="scope.row.Logo + '?wh=500x500'"
                        :preview-src-list="[scope.row.Logo]"></el-image>
                </div>
              </div>
            </template>
          </el-table-column>
          <el-table-column label="状态" align="center" prop="status">
            <template slot-scope="scope">
              <dict-tag :options="dict.type.org_status" :value="scope.row.status" />
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center" prop="createTime" width="180">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button
                type="text"
                icon="el-icon-circle-close"
                @click="cancelAuthUser(scope.row)"
              >取消授权</el-button>
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
      <select-org ref="select" :styleId="queryParams.StyleId" @ok="handleQuery" />
    </div>
  </div>
</template>

<script>
import {
  allStyleOrgList,
  delOrgStyle,
} from "@/api/system/StyleMan";
import selectOrg from "./selectOrg";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "AuthUser",
  dicts: ["org_status"],
  components: { selectOrg },
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
      orgList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        StyleId: undefined,
        Key: undefined,
      }
    };
  },
  created() {
    const themeId = this.$route.params && this.$route.params.themeId;
    if (themeId) {
      this.queryParams.StyleId = themeId;
      this.getList();
    }
  },
  methods: {
    /** 查询授权用户列表 */
    getList() {
      this.loading = true;
      allStyleOrgList(this.queryParams).then(response => {
        this.orgList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    // 返回按钮
    handleClose() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/system/theme/index" });
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
          backgroundColor: "#F6F9FF"
        };
      }
    },
    /** 打开授权用户表弹窗 */
    openSelectUser() {
      this.$refs.select.show();
    },
    /** 取消授权按钮操作 */
    cancelAuthUser(row) {
      const styleId = this.queryParams.StyleId;
      this.$modal
        .confirm('确认要删除该企业"' + row.OrgName + '"的主题吗？')
        .then(function() {
          return delOrgStyle({ orgId: row.Id, styleId: styleId });
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
  }
};
</script>