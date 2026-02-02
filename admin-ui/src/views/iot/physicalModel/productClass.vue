<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
 
    <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
      <el-row :gutter="10" class="mb8 button_row">
        <div>
          <el-col :span="1.5">
            <el-button
              type="primary"
              plain
              @click="openClassDialog('-1')"
            >
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left:6px">新增</span>
            </el-button>
          </el-col>
        </div>
        <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
      </el-row>
      <el-table
        v-if="refreshTable"
        v-loading="loading"
        :data="classListData"
        style="width: 100%;"
        row-key="Id"
        class="data_table"
        :header-cell-style="cellSty"
        border
        :default-expand-all="isExpandAll"
        :tree-props="{children: 'Children', hasChildren: 'hasChildren'}"
      >
        <el-table-column
          v-if="columns[0].visible"
          key="Id"
          prop="Id"
          label="分类编号"
          sortable
          width="180"
        ></el-table-column>
        <el-table-column v-if="columns[1].visible" key="Name" prop="Name" label="分类名称" width="180"></el-table-column>
        <el-table-column
          v-if="columns[2].visible"
          key="Sort"
          prop="Sort"
          label="排序值"
          sortable
          width="180"
        ></el-table-column>
        <el-table-column v-if="columns[3].visible" key="Remark" prop="Remark" label="备注说明"></el-table-column>
        <el-table-column
          label="操作"
          align="center"
          width="248"
          class-name="small-padding fixed-width"
        >
          <template slot-scope="scope">
            <el-button
              type="text"
              icon="el-icon-edit"
              @click="openClassDialog(scope.row.Id, scope.row)"
            >修改</el-button>
            <!-- <el-button
              type="text"
              icon="el-icon-circle-plus-outline"
              @click="openClassDialog(scope.row.ParentId, scope.row)"
            >添加</el-button> -->
            <el-button type="text" icon="el-icon-delete" @click="delClass(scope.row.Id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
    <el-dialog
      :title="dialogTitle"
      :close-on-click-modal="false"
      :visible.sync="addClassOpen"
      width="600px"
      append-to-body
    >
      <el-form
        ref="classForm"
        :model="classForm"
        class="classFrom"
        :rules="classRules"
        label-width="80px"
      >
            <el-form-item label="父级分类" prop="parentId">
                <el-cascader
                    style=" width: 100%;"
                    v-model="classForm.parentId"
                    :options="groupTreeList"
                    :props="cascaderProps"
                    clearable>
                </el-cascader>
            </el-form-item> 
            <el-form-item label="分类名称" prop="name">
              <el-input v-model="classForm.name" placeholder="请输入分类名称" maxlength="50" />
            </el-form-item>
            <el-form-item label="排序值" prop="sort">
              <el-input-number
                v-model="classForm.sort"
                controls-position="right"
                placeholder="请输入排序值"
                :min="0"
              ></el-input-number>
              <span class="item-desc">
                <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="font-size:14px;margin-right:5px;color:#E74032;"></i>排序值越小越靠前
              </span>
            </el-form-item>
            <el-form-item label="分类封面" prop="photoUrl" class="is-required">
              <image-upload v-model="classForm.photoUrl" :limit="1"></image-upload>
            </el-form-item>
            <el-form-item label="备注说明" prop="remark">
              <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="classForm.remark"></el-input>
            </el-form-item>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitClassForm">确 定</el-button>
        <el-button @click="addClassOpen=false">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  classTree,
  classInfo,
  removeClass,
  addClass,
  classSort,
  editClass
} from "@/api/rules/productModel";
export default {
  name: "proclassClass",
  mixins: [resizeTableCon],
  data() {
    const fileMustUpload = (rule, value, callback) => {
      if (this.classForm.photoUrl == null) {
        // 未上传文件
        callback("请上传封面");
      }
      callback();
    };
    return {
      dialogTitle: "", //弹出层的标题
      classForm: {
        name: "",
        sort: 0,
        remark: "",
        parentId: "",
        children: [],
        photoUrl: null
      },
      classRules: {
        parentId: [
          { required: true, trigger: "change", message: "请选择父级分类" },
        ],
        name: [{ required: true, trigger: "blur", message: "请输入分类名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入排序值" }],
        //自定义校验器
        // photoUrl: [{ validator: fileMustUpload, trigger: "change" }]
        // photoUrl: [{ required: true, trigger: "change", message: "请上传封面" }]
      }, //协议分类添加验证
      addClassOpen: false, //添加分类的弹出层
      // 遮罩层
      loading: false,
      // 是否展开，默认全部折叠
      isExpandAll: false,
      // 重新渲染表格状态
      refreshTable: true,
      // 列信息
      columns: [
        { key: 0, label: `分类编号`, visible: true },
        { key: 1, label: `分类名称`, visible: true },
        { key: 2, label: `排序值`, visible: true },
        { key: 3, label: `备注说明`, visible: true }
      ],
      // 显示搜索条件
      showSearch: true,
      // 日期范围
      dateRange: [],
      classListData: [], //协议分类树形列表
      groupTreeList: [],
      cascaderProps: {
        checkStrictly: true,
        value: 'Id',
        label: 'Name',
        children: 'Children'
      },
    };
  },
  computed: {
    btnObj() {
      return this.classForm.photoUrl;
    }
  },
  mounted() {
    this.getList();
  },
  methods: {
    delClass(id) {
      this.$modal
        .confirm('是否确认删除协议编号为"' + id + '"的数据项？')
        .then(function() {
          return removeClass({ id: id });
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    openClassDialog(parentId, data) {
      //打开添加分类的弹窗
      if (
          !this.groupTreeList[0] ||
          this.groupTreeList[0].Id != "-1"
          ) {
          this.groupTreeList.unshift({
              Name: "作为一级分类",
              Id: "-1",
              ParentId: '',
              Children: []
          });
      }
      if (data) {
          this.dialogTitle = '修改分类';
          //编辑分类
          this.classForm = {
              parentId: data.ParentId === '' ? '-1' : data.ParentId,
              id: parentId,
              name: data.Name,
              sort: data.Sort,
              remark: data.Remark,
              photoUrl: data.PhotoUrl
          };
      } else {
          this.dialogTitle = '添加分类';
          //打开添加分类
          this.classForm = {
              parentId: '-1',
              id: parentId,
              name: "",
              sort: 0,
              remark: '',
              photoUrl: null,
              children: []
          };
      }
      this.addClassOpen = !this.addClassOpen;
    },
    submitClassForm() {
      //提交保存分类
      this.$refs["classForm"].validate(valid => {
        if (valid) {
          if (typeof this.classForm.parentId !== 'string') { 
              this.classForm.parentId = this.classForm.parentId[this.classForm.parentId.length - 1] === "-1" ? '' : 
              this.classForm.parentId[this.classForm.parentId.length - 1]
          } else {
              this.classForm.parentId = this.classForm.parentId === "-1" ? '' : this.classForm.parentId
          }
          if (this.dialogTitle === '修改分类') {
            editClass(this.classForm)
              .then(rsp => {
                console.log("编辑后返回值", rsp);
                if (rsp.code == 0) {
                  this.$modal.msgSuccess("修改成功");
                  this.getList();
                  this.addClassOpen = false;
                }
              })
              .catch(err => {
                this.$message.error(err);
              });
          } else {
            addClass(this.classForm)
              .then(rsp => {
                console.log("添加后返回值", rsp);
                if (rsp.code == 0) {
                  this.$modal.msgSuccess("添加成功");
                  this.getList();
                  this.addClassOpen = false;
                }
              })
              .catch(err => {
                this.$message.error(err);
              });
          }
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    // 表单重置
    reset() {
      this.classForm = {
        name: "",
        sort: undefined,
        remark: ""
      };
      this.resetForm("classForm");
    },
    /** 查询用户列表 */
    getList() {
      this.loading = true;
      classTree().then(response => {
        this.classListData = response.data;
        this.groupTreeList = JSON.parse(JSON.stringify(response.data));
        this.loading = false;
      });
    },


  }
};
</script>
<style lang="less">
.proclass-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.proclass-uploader .el-upload:hover {
  border-color: #409eff;
}
.proclass-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}
.proclass {
  width: 178px;
  height: 178px;
  display: block;
}
.classFrom {
  .item-desc {
    color: rgba(50, 150, 250, 0.71);
    display: inline-block;
    width: 200px;
    height: 36px;
    line-height: 36px;
    background-color: #f5f7fa;
    text-align: left;
    margin-bottom: 10px;
    // font-size: 14px;
    border-radius: 5px;
    // border: 1px solid #dcdfe6;
    padding-left: 15px;
    box-sizing: border-box;
    margin-left: 5px;
  }
}
</style>