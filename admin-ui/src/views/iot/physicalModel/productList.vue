<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--协议数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="搜索关键词" prop="Name">
                <el-input class="set_radius" v-model="queryParams.Name" placeholder="请输入协议名称" clearable @keyup.enter.native="handleQuery"/>
              </el-form-item>
              <el-form-item label="状态" prop="Status">
                <el-select class="set_radius" v-model="queryParams.Status" placeholder="协议状态" clearable @change="changeStatus">
                  <el-option label="未发布" value="0" />
                  <el-option label="已发布" value="1" />
                </el-select>
              </el-form-item>
              <el-form-item label="创建时间">
                <el-date-picker class="set_radius" v-model="dateRange" style="width:200px" value-format="yyyy-MM-dd" type="daterange"
                  range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <el-form-item label="协议分类" prop="ClassId">
                <treeselect class="groupSet" style="width:150px;" v-model="queryParams.ClassId" :options="proClassTree"
                  :show-count="true" :normalizer="normalizer" placeholder="请选择协议分类" />
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
                  <el-button type="primary" plain @click="handleAdd">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">新增</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button :disabled="multiple" type="primary" plain @click="handleExport">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left:6px">导出</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-upload style="display:inline" accept=".json" :multiple="false" :show-file-list="false" action="#" :before-upload="handleImport">
                    <el-button  type="primary" plain>
                      <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                      <span style="margin-left:6px">导入</span>
                    </el-button>
                  </el-upload>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleClsAdd" v-hasPermi="['/IoTService/IotClass/ListTree']">
                    <i class="el-icon-folder"></i>
                    <span style="margin-left:6px">协议分类</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" border :data="productTableList" :row-style="isRed"
              @selection-change="handleSelectionChange" class="data_table" :header-cell-style="cellSty" style="width:100%" :fit="true">
            <el-table-column type="selection" width="55">
            </el-table-column>
              <el-table-column width="320" align="center" label="协议信息" key="proInfo" prop="Name" v-if="columns[0].visible">
                <template slot-scope="scope">
                  <div class="col_con">
                    <div class="col_left">
                      <el-image fit="cover" style="width:54px;height:54px" :src="scope.row.PhotoUrl + '?wh=500x500'">
                        <div slot="error" class="image-slot">
                          <i class="el-icon-picture-outline"></i>
                        </div>
                      </el-image>
                    </div>
                    <div class="col_right">
                      <div class="col_right_bottom">
                        <div class="right_bottom1">
                          <div class="proName">{{scope.row.Name}}</div>
                          <div v-if="scope.row.ClassifiedId">协议分类：{{classmap.get(scope.row.ClassifiedId)}}</div>
                        </div>
                        <div class="right_bottom2">
                          <span style="text-align: left;">协议编号：{{scope.row.Id}}</span>
                          <span class="data_icon">
                            <el-tooltip
                              class="item"
                              content="复制协议编号"
                              placement="top"
                            >
                              <i class="el-icon-document-copy" @click="copyId(scope.row.Id)"></i>
                            </el-tooltip>
                            <el-tooltip
                              class="item2"
                              placement="top"
                              style="border:none;"
                            >
                              <div slot="content" style="line-height:20px">
                                品类：{{classmap.get(scope.row.ClassifiedId)}}
                                <br />
                                创建：{{parseTime(scope.row.createTime,'{y}-{m}-{d}')}}
                                <br />
                                更新：{{parseTime(scope.row.updateTime,'{y}-{m}-{d}')}}
                              </div>
                              <i class="el-icon-more"></i>
                            </el-tooltip>
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                </template>
              </el-table-column>

              <el-table-column label="创建者" align="center" key="createId" prop="createId" v-if="columns[1].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.createName}}</span>
                </template>
              </el-table-column>
              <el-table-column label="更新者" align="center" key="updateId" prop="updateId" v-if="columns[2].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.updateName}}</span>
                </template>
              </el-table-column>
              <el-table-column label="接入方式" align="center" key="NetworkWay" prop="NetworkWay" v-if="columns[3].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.NetworkWay}}</span>
                </template>
              </el-table-column>
              <el-table-column label="通讯方式" align="center" key="PhysicsWay" prop="PhysicsWay" v-if="columns[4].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.PhysicsWay}}</span>
                </template>
              </el-table-column>
              <el-table-column label="发布状态" align="center" key="Status" v-if="columns[5].visible" width="118"
                :filters="[{text: '未发布', value: '0'}, {text: '已发布', value: '1'}, {text: '未初始化', value: '2'}]"
                :filter-method="filterSatus"
              >
                <template slot-scope="scope">
                  <el-switch v-model="scope.row.Status" active-value="1" inactive-value="0" @change="handleProductStatus(scope.row)"></el-switch>
                </template>
              </el-table-column>
              <el-table-column label="当前版本" align="center" key="Version" prop="Version" v-if="columns[6].visible">
                <template slot-scope="scope">
                  <span>{{ scope.row.Version}}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建日期" align="center" prop="createTime" width="180" v-if="columns[7].visible">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column
                label="操作"
                align="center"
                width="248"
                fixed="right"
                class-name="small-padding fixed-width"
              >
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-copy-document" @click="handleCopy(scope.row)">拷贝</el-button>
                  <template v-if="scope.row.Status=='0'">
                    <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
                    <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)">删除</el-button>
                  </template>
                  <template v-if="scope.row.Status=='1'">
                    <el-button
                      type="text"
                      icon="el-icon-video-pause"
                      @click="handleActive(scope.row)"
                    >取消发布</el-button>
                  </template>
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
    </div>


  </div>
</template>

<script>
import {
  classTree,
  editProduct,
  addProduct,
  copyProduct,
  productList,
  removeProduct,
  productInfo
} from "@/api/rules/productModel";

import { resizeTableCon } from "@/mixins/resizeTableCon";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
  name: "ProductList",
  mixins: [resizeTableCon],
  components: { Treeselect },
  data() {
    return {
      classmap: new Map(),
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
      // 协议表格数据
      productTableList: null,
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 表单参数
      form: {},
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Name: null,
        Status: null
      },
      // 列信息
      columns: [
        { key: 0, label: `协议信息`, visible: true },
        { key: 1, label: `创建者ID`, visible: true },
        { key: 2, label: `更新者ID`, visible: true },
        { key: 3, label: `接入方式`, visible: true },
        { key: 4, label: `通讯方式`, visible: true },
        { key: 5, label: `开发状态`, visible: true },
        { key: 6, label: `当前版本`, visible: true },
        { key: 7, label: `创建时间`, visible: true },
      ],
      proClassTree:[],//协议分类列表
    };
  },
  mounted() {
    this.getClassList();
    this.$nextTick(() => {
      this.getList();
    });
  },
  watch: {
    $route(to, from) {
      if (
        from.path.indexOf("productAddSteps") ||
        from.path.indexOf("productAdd")
      ) {
        this.getList();
      } else if (from.path.indexOf("productClass")) {
        this.getClassList();
        this.getList();
      }
    }
  },
  methods: {
    async handleExport(){
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let prolist=[];
      let tmname="";
      for(let i=0;i<this.ids.length;i++){
        let prorsp = await productInfo({id:this.ids[i]});
        delete prorsp.data.Id;
        prolist.push(prorsp.data);
        tmname=prorsp.data.Name;
      }
      tmploading.close();
      const content =JSON.stringify(prolist)
      const blobData = new Blob([content], { type: 'application/json' })
      const filename = `${tmname}.json` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    handleImport(file){
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = async (e)=> {
        const str = e.target.result
        const jsonData = JSON.parse(str)
        for(let i=0;i<jsonData.length;i++){
          await addProduct(jsonData[i]);
        }
        tmploading.close();
        this.handleQuery();
      }
    },
    changeStatus() {
      //改变查询的状态的值
      if (this.queryParams.Status == "") {
        this.queryParams.Status = null;
      }
    },
    copyId(id) {
      //复制协议id
      let oInput = document.createElement("input");
      oInput.value = id;
      document.body.appendChild(oInput);
      oInput.select(); // 选择对象;
      document.execCommand("Copy"); // 执行浏览器复制命令
      this.$message({
        message: "复制成功",
        type: "success"
      });
      oInput.remove();
    },
    filterSatus(value, row) {
      return row.Status === value;
    },
    /** 查询协议列表 */
    getList() {
      this.loading = true;
      productList(this.addDateRange(this.queryParams, this.dateRange)).then(
        async response => {
          // console.log("查询到的协议", response);
          this.productTableList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    initClassMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        // console.log("组合时用户列表curnode",curnode);
        this.classmap.set(node[idx].Id, curnode.Name);
        if (
          curnode.hasOwnProperty("Children") &&
          curnode.Children &&
          curnode.Children.length > 0
        ) {
          this.initClassMap(curnode.Children);
        }
      }
    },
    getClassList() {
      //获取分类列表
      classTree().then(response => {
        if (response.data.length > 0) {
          // this.classmap.set(0, response.data);
          this.proClassTree=response.data
          this.initClassMap(response.data);
        }
      });
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.Name,
        children: node.Children
      };
    },
    // 协议状态修改
    handleProductStatus(row) {

      let text = "";
      if (row.Status == "0") {
        text = "取消发布";
      } else {
        text = "发布";
      }
      this.$modal
        .confirm('确认要"' + text + '""' + row.Name + '"协议吗？')
        .then(function() {
          return editProduct({ Id: row.Id, Status: row.Status });
        })
        .then(() => {
          this.$modal.msgSuccess(text + "成功");
        })
        .catch(function() {
          row.Status = row.Status == "0" ? "1" : "0";
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
        dept_id: undefined,
        UserName: undefined,
        RealName: undefined,
        Password: undefined,
        Mobile: undefined,
        Email: undefined,
        Sex: "2",
        Status: "0",
        Introduction: undefined,
        post_name: undefined
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
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    handleCopy(row){
      copyProduct(row.Id).then(rsp=>{
        this.$modal.msgSuccess("拷贝成功");
        this.getList();
      });
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.$router.push({ path: "/iot/physicalModel/productAddSteps" });
    },
    handleClsAdd(){
      this.$router.push({ path: "/iot/physicalModel/productClass" });
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.$router.push({
        path: "/iot/physicalModel/productAdd/"+row.Id,
        query: { classId: row.ClassifiedId }
      });
    },
    handleActive(row) {
      //启用
      row.Status = row.Status == "0" ? "1" : "0";
      this.$modal
        .confirm('确认要取消发布"' + row.Name + '"协议吗？')
        .then(function() {
          return editProduct({ Id: row.Id, Status: row.Status });
        })
        .then(() => {
          this.$modal.msgSuccess("取消发布成功");
        })
        .catch(function() {
          row.Status = row.Status == "0" ? "1" : "0";
        });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const ids = row.Id;
      this.$modal
        .confirm('是否确认删除协议编号为"' + ids + '"的数据项？')
        .then(function() {
          return removeProduct({ id: ids });
        })
        .then(rsp => {
          // console.log("删除返回值", rsp);
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    }
  }
};
</script>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";
// @import "~@/assets/icons/iconfont.css";
// .app-container {
//   padding-right: 30px;
// }
.data_table {
  th div.cell .el-table__column-filter-trigger .el-icon-arrow-down::before {
    font-family: "zhongtaiiconfont" !important;
    // font-size: 24px;
    font-size: var(--shaixuan);
    font-style: normal;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    content: "\e6d2";
  }
  .col_con {
    width: 320px;
    display: flex;
    justify-content: flex-start;
    .col_left {
      display: flex;
      align-items: center;
      img {
        width: 54px;
        height: 54px;
      }
    }
    .col_right {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-left: 12px;
      .col_right_top {
        line-height: 22px;
        text-align: left;
        width: 100%;
        color: #0054fc;
      }
      .col_right_bottom {
        line-height: 22px;
        .right_bottom1 {
          display: flex;
          align-items: flex-start;
          justify-content: flex-start;
          flex-direction: column;
          .proName {
            color: #0054fc;
          }

          // .proName::after {
          //   content: "|";
          //   display: inline;
          //   color: #976697;
          //   padding: 0;
          //   margin: 0 8px;
          //   vertical-align: 0;
          // }
        }
        .right_bottom2 {
          display: flex;
          align-items: center;
          justify-content: flex-start;
          .data_icon {
            display: block;
            width: 44px;
            height: 16px;
            color: #0054fc;
            .el-tooltip__popper .popper__arrow {
              border-width: 6px;
              border-color: rgb(0, 0, 0, 1);
            }
            i {
              display: none;
              margin-left: 8px;
            }
          }
        }
      }
    }
  }
}
.data_table.el-table table tr.el-table__row:hover {
  .col_con .col_right .col_right_bottom .right_bottom2 .data_icon i {
    display: inline;
    cursor: pointer;
  }
}
.groupSet {
  .vue-treeselect__input-container {
    display: flex;
    align-items: center;
  }
}
</style>
<style lang="scss" scoped>
::v-deep .vue-treeselect__menu{
  overflow: auto;
  width: 160px;
}
::v-deep .vue-treeselect__label{
  overflow: unset;
  text-overflow: unset;
}
::v-deep .vue-treeselect div, .vue-treeselect span{
  box-sizing:content-box;
}
</style>