<template>
  <!-- 授权用户 -->
  <el-dialog
    title="选择企业"
    :visible.sync="visible"
    width="800px"
    top="5vh"
    :close-on-click-modal="false"
    append-to-body
  >
    <el-form :model="queryParams" ref="queryForm" :inline="true">
      <el-form-item label="关键字" prop="Key">
        <el-input
          v-model="queryParams.Key"
          placeholder="请输入关键字"
          clearable
          @keyup.enter.native="handleQuery"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
        <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>
    <el-row>
      <el-table
        @row-click="clickRow"
        border
        ref="table"
        :data="orgList"
        @selection-change="handleSelectionChange"
        height="260px"
      >
        <el-table-column type="selection" width="55"></el-table-column>
        <el-table-column label="企业编号" prop="Id" :show-overflow-tooltip="true" width="140"/>
        <el-table-column label="企业名称" prop="OrgName" :show-overflow-tooltip="true" />
        <el-table-column label="Logo" align="center" prop="Email" :show-overflow-tooltip="true" >
          <template slot-scope="scope">
            <div style="width: 100%;display:flex;justify-content:center;align-items:center">
              <div class="imgwrap" style="max-width: 30px;max-height:30px;">
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
      </el-table>
      <pagination
        v-show="total>0"
        :total="total"
        :page.sync="queryParams.pageNum"
        :limit.sync="queryParams.pageSize"
        @pagination="getList"
      />
    </el-row>
    <div slot="footer" class="dialog-footer">
      <el-button type="primary" @click="handleSelectUser">确 定</el-button>
      <el-button @click="visible = false">取 消</el-button>
    </div>
  </el-dialog>
</template>

<script>
import {
  allStyleOrgList,
  setOrgStyle,
} from "@/api/system/StyleMan";
export default {
  dicts: ["org_status"],
  props: {
    // 角色编号
    styleId: {
      type: [Number, String]
    }
  },
  data() {
    return {
      // 遮罩层
      visible: false,
      // 选中数组值
      orgIds: [],
      // 总条数
      total: 0,
      // 未授权用户数据
      orgList: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        StyleId: undefined,
        Key: undefined,
        NoExist:true
      }
    };
  },
  methods: {
    // 显示弹框
    show() {
      this.queryParams.StyleId = this.styleId;
      this.getList();
      this.visible = true;
    },
    clickRow(row) {
      this.$refs.table.toggleRowSelection(row);
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      console.log("选中的用户",selection);
      
      this.orgIds = selection.map(item => item.Id);
    },
    // 查询表数据
    getList() {
      allStyleOrgList(this.queryParams).then(res => {
        this.orgList = res.data.List;
        this.total = res.data.Total;
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
    /** 选择授权用户操作 */
    handleSelectUser() {
      const styleId = this.queryParams.StyleId;
      if(this.orgIds&&this.orgIds.length>0){
        this.orgIds.map(row=>{
          setOrgStyle({ styleId: styleId, orgId: row }).then(res => {
            this.$modal.msgSuccess("添加成功");
            this.visible = false;
            this.$emit("ok");
          });
        })
        
      }
      
    }
  }
};
</script>
