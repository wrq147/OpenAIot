<template>
  <div class="app-container">
      <div style="margin-bottom: 20px;">
        <el-radio-group v-model="sourceType">
          <el-radio-button label="api">api数据源</el-radio-button>
          <el-radio-button label="sql">sql数据源</el-radio-button>
        </el-radio-group>
      </div>
      <div v-if="sourceType === 'api'" class="header-query">
        <el-form ref="queryForm" :model="queryParams" :inline="true">
          <el-form-item label="过滤链接名称">
            <el-input v-model.trim="queryApiParams.Name" placeholder="请输过滤链接名称" clearable />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
        </el-form>
        <div>
          <el-button v-hasPermi="['/ReportService/Source/Add']" type="primary" icon="el-icon-plus" @click="handleView('')">新增</el-button>
          <el-button v-hasPermi="['/ReportService/Source/Remove']" type="danger" icon="el-icon-delete" @click="handleDelete(undefined)">批量删除</el-button>
        </div>
      </div>
      <div v-if="sourceType === 'sql'" class="header-query">
        <el-form ref="queryForm" :model="queryParams" :inline="true">
          <el-form-item label="过滤链接名称">
            <el-input v-model.trim="queryParams.Name" placeholder="请输过滤链接名称" clearable />
          </el-form-item>
          <el-form-item label="数据库类型" prop="databaseType" label-width="100px">
            <el-select v-model="queryParams.dataType" placeholder="请选择数据库类型">
              <el-option label="全部" value=""></el-option>
              <el-option label="MySQL" value="mysql"></el-option>
              <el-option label="SQLServer" value="sqlserver"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
        </el-form>
        <div>
          <el-button v-hasPermi="['/ReportService/Source/Add']" type="primary" icon="el-icon-plus" @click="handleView('')">新增</el-button>
          <el-button v-hasPermi="['/ReportService/Source/Remove']" type="danger" icon="el-icon-delete" @click="handleDelete(undefined)">批量删除</el-button>
        </div>
      </div>
      
      <div v-if="sourceType === 'api'" style="background-color: rgb(255, 255, 255);">
        <el-table v-loading="loadingApi" stripe :data="sourseApiList" @selection-change="handleSelectionChangeApi">
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="序号" type="index" align="center" width="80px" sortable :show-overflow-tooltip="true" />
          <el-table-column label="接口类型" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.apiType === 0 ? 'BI报表' : '打印模板' }}</span>
            </template>
          </el-table-column>
          <el-table-column label="接口名称" prop="InterfaceName" align="center" />
          <el-table-column label="api地址" prop="Url" align="center" />
          <el-table-column label="接口方法" prop="Method" align="center" width="100px" />
          <el-table-column label="参数类型" prop="ParamType" align="center" width="100px" />
          <el-table-column label="创建人" prop="createName" align="center" />
          <el-table-column label="创建时间" align="center" prop="createTime">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button size="mini" type="text" icon="el-icon-edit" @click="handleView(scope.row)">编辑</el-button >
              <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="handleDelete(scope.row)">删除</el-button >
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="totalApi>0" :total="totalApi" :page.sync="queryApiParams.pageNum" :limit.sync="queryApiParams.pageSize" @pagination="getApiList" />
      </div>
      <div v-if="sourceType === 'sql'" style="background-color: rgb(255, 255, 255);">
        <el-table v-loading="loading" stripe :data="sourseList" @selection-change="handleSelectionChangeSql">
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="序号" type="index" align="center" width="80px" sortable :show-overflow-tooltip="true" />
          <el-table-column label="数据库类型" prop="DatabaseType" align="center" width="100" />
          <el-table-column label="IP地址" prop="IpAddress" align="center" />
          <el-table-column label="端口" prop="Port" align="center" width="80" />
          <el-table-column label="数据库名称" prop="DatabaseName" align="center" />
          <el-table-column label="链接名称" prop="LinkName" align="center" />
          <el-table-column label="用户名" prop="UserName" align="center" />
          <el-table-column label="密码" prop="Password" align="center" />
          <el-table-column label="创建人" prop="createName" align="center" />
          <el-table-column label="创建时间" align="center" prop="createTime">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
            <template slot-scope="scope">
              <el-button size="mini" type="text" icon="el-icon-edit" @click="handleView(scope.row)">编辑</el-button >
              <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="handleDelete(scope.row)" >删除</el-button >
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total>0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
      </div>
    <add-data-source ref="addDataSourse" :dialog-visible="isOpenDataSource" :title="title" @cancelForm="cancelForm" @getList="getList" />
    <add-api-source ref="addApiSource" :dialog-visible="isOpenApiSource" :title="title" @cancelForm="cancelForm" @getApiList="getApiList" />
  </div>
</template>
<script>
import { listSourse, delSourse } from "@/api/report/sourse";
import { listApiSource, delApiSource } from "@/api/report/apisource";
import addDataSource from "./addDataSource";
import addApiSource from "./addApiSource";
export default {
  name: "datasourse",
  components: {
    addDataSource,
    addApiSource
  },
  data() {
    return {
      // 标签切换
      sourceType: 'api',
      // 遮罩层
      loading: true,
      loadingApi: true,
      // 选中数组
      idsSql: [],
      idsApi: [],
      // 总条数
      total: 0,
      totalApi: 0,
      // 表格数据
      sourseList: [],
      sourseApiList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      isOpenDataSource: false,
      isOpenApiSource: false,
      // 查询参数
      queryApiParams: {
        pageNum: 1,
        pageSize: 10,
        Name: undefined,
        apiType: 0
      },
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Name: undefined,
        dataType: ''
      },
    };
  },
  created() {
    this.getList();
    this.getApiList();
  },
  methods: {
    /** 查询数据源列表 */
    getList() {
      this.loading = true;
      this.isOpenDataSource = false;
      listSourse(this.queryParams).then((response) => {
        this.sourseList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    /** 查询数据源列表 */
    getApiList() {
      this.loadingApi = true;
      this.isOpenApiSource = false;
      listApiSource(this.queryApiParams).then((response) => {
        this.sourseApiList = response.data.List;
        this.totalApi = response.data.Total;
        this.loadingApi = false;
      });
    },
    // 取消按钮
    cancelForm() {
      this.isOpenDataSource = false;
      this.isOpenApiSource = false;
    },

    /** 搜索按钮操作 */
    handleQuery() {
      if (this.sourceType === 'api') {
        this.queryApiParams.pageNum = 1;
        this.getApiList();
      } else {
        this.queryParams.pageNum = 1;
        this.getList();
      }
    },
    
    /** 新增/编辑操作 */
    handleView(data) {
      if (this.sourceType === 'api') {
        this.addApiFunction(data)
      } else {
        this.addSqlFunction(data)
      }
    },
    addSqlFunction(data) {
      if (data === '') { 
        this.title = "添加数据源";
        this.$refs['addDataSourse'].ruleForm={
          databaseType: '',
          ipAddress: '',
          linkName: '',
          port: '',
          databaseName: '',
          userName: '',
          password: ''
        };
      } else {
        this.title = "编辑数据源";
        this.$refs['addDataSourse'].ruleForm={
          id: data.Id,
          databaseType: data.DatabaseType,
          ipAddress: data.IpAddress,
          linkName: data.LinkName,
          port: data.Port,
          databaseName: data.DatabaseName,
          userName: data.UserName,
          password: data.Password
        };
      }
      this.isOpenDataSource = true;
    },
    addApiFunction(data) {
      if (data === '') { 
        this.title = "添加数据源";
        this.$refs['addApiSource'].ruleForm={
          apiType: 0,
          interfaceName: '',
          url: '',
          method: '',
          paramJson: '',
          paramType: '',
          headerJson: ''
        };
      } else {
        this.title = "编辑数据源";
        this.$refs['addApiSource'].ruleForm={
          id: data.Id,
          apiType: data.ApiType,
          interfaceName: data.InterfaceName,
          url: data.Url,
          method: data.Method,
          paramJson: data.ParamJson,
          paramType: data.ParamType,
          headerJson: data.HeaderJson
        };
      }
      this.isOpenApiSource = true;
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      if (this.sourceType === 'api') {
        this.apiDeleteFunction(row)
      } else {
        this.sqlDeleteFunction(row)
      }
    },
    apiDeleteFunction(row){
      const ids = row ? row.Id : this.idsApi.toString();;
      if (ids.length < 1) {
        this.$message.error('请选择要删除的数据')
        return
      }
      this.$confirm(
        '是否确认删除编号为"' + ids + '"的数据项?', "警告", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }).then(function () {
          return delApiSource({ id: ids });
        }).then(() => {
          this.getApiList();
          this.msgSuccess("删除成功");
        }).catch(function () {});
    },
    sqlDeleteFunction(row){
      const ids = row ? row.Id : this.idsSql.toString();
      if (ids.length < 1) {
        this.$message.error('请选择要删除的数据')
        return
      }
      this.$confirm(
        '是否确认删除编号为"' + ids + '"的数据项?', "警告", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        }).then(function () {
          return delSourse({ id: ids });
        }).then(() => {
          this.getList();
          this.msgSuccess("删除成功");
        }).catch(function () {});
    },
    // 多选框选中数据
    handleSelectionChangeSql(selection) {
      this.idsSql = selection.map((item) => item.Id);
    },
    // 多选框选中数据
    handleSelectionChangeApi(selection) {
      this.idsApi = selection.map((item) => item.Id);
    }
  },
};
</script>