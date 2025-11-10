<template>
    <div class="groupList">
        <div class="groupList-pd">
            <el-row class="mb8 button_row" style="margin-bottom: 5px;background-color: #f6f9ff;" type="flex" justify="space-between">
                <el-col :span="8" :offset="1">
                  <span style="font-size: 14px;color:#333;">报表分组</span>
                </el-col>
                <el-col :span="15" style="display: flex;justify-content: flex-end;margin-right: 10px;">
                  <el-button style="min-width:50px;padding:10px 5px;" plain @click="openAddCatetory('-1')">
                    <i class="el-icon-plus" style="font-size: 14px"></i><span style="margin-left: 6px">添加</span></el-button>
                  <el-button style="min-width:50px;padding:10px 5px;" plain @click="isEnableEditor = !isEnableEditor">
                    <i class="el-icon-edit" style="font-size: 14px"></i><span style="margin-left: 6px">{{isEnableEditor?"取消":"编辑"}}</span></el-button>
                </el-col>
            </el-row>
            <div style="padding:10px;">
              <el-tree ref="categoryTree" v-loading="configLoading" :data="catetoryTableData" node-key="Id" default-expand-all
                :highlight-current="true" :expand-on-click-node="true" :check-on-click-node="true" :props="{ label: 'Name', children: 'Children' }" @node-click="clickNode">
                  <span class="custom-tree-node" slot-scope="{ node, data }">
                  <span>{{ node.label }}</span>
                  <span v-if="node.level>1&&isEnableEditor">
                      <el-button type="text" size="mini" @click.stop="() => openAddCatetory(data.Id, data)">编辑</el-button>
                      <el-button type="text" size="mini" style="color: red;" @click.stop="() => deleteRowData(node, data)">删除</el-button>
                  </span>
                  </span>
              </el-tree>
            </div>
        </div>
        <el-dialog v-if="dialogFlag" :title="title + '分组'" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px" top="10vh" @close="dialogFlag = false">
            <el-form ref="catetoryFrom" :model="catetoryFrom" :rules="catetoryRules" label-width="80px" class="addPeople">
                <el-form-item label="父级分类" prop="parentId">
                    <el-cascader
                        v-model="catetoryFrom.parentId"
                        :options="roomCatetoryTreeList"
                        :props="cascaderProps"
                        clearable>
                    </el-cascader>
                </el-form-item> 
                <el-form-item label="分类名称" prop="name">
                    <el-input type="text" v-model="catetoryFrom.name" placeholder="请输入分类名称"></el-input>
                </el-form-item>
                <el-form-item label="分类序号" prop="sort">
                    <el-input type="number" v-model.number="catetoryFrom.sort" placeholder="请输入分类序号"></el-input>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
                <el-button @click="dialogFlag = false">取消</el-button>
                <el-button type="primary" @click="submitForm('catetoryFrom')">确定</el-button>
            </span>
        </el-dialog>
    </div>
</template>
<script>
import { treeSelectList, treeSelectAdd, treeSelectEdit, treeSelectRemove } from "@/api/report/report";
export default { 
  name: 'addDataOrigin',
  props: {
    dialogVisible: {
      type: Boolean
    }
  },
  data() {
    return {
      configLoading: true,
      roomCatetoryTreeList: [],
      catetoryTableData: [],//分类列表总数居
      dialogFlag: false,
      title: '添加',
      catetoryFrom: {
        id: "",
        name: "",
        sort: "",
        parentId: "",
        orgId: '',
        children: []
      },
      cascaderProps: {
        checkStrictly: true,
        value: 'Id',
        label: 'Name',
        children: 'Children'
      },
      catetoryRules: {
        parentId: [
          { required: true, trigger: "change", message: "请选择父级分类" },
        ],
        name: [{ required: true, trigger: "blur", message: "请输入分类名称" }],
        sort: [{ required: true, trigger: "blur", message: "请输入分类序号" }],
      },
      isEnableEditor:false
    }
  },
  async mounted() {
    this.activeOrgId = this.$store.getters.orgId;
    this.getCatetoryList();
  },
  methods: {
    // 点击编辑/添加操作
    openAddCatetory(parentId, data) {
        if (
            !this.roomCatetoryTreeList[0] ||
            this.roomCatetoryTreeList[0].Id != "-1"
            ) {
            this.roomCatetoryTreeList.unshift({
                Name: "作为一级分类",
                Id: "-1",
                ParentId: '',
                Children: []
            });
        }
        if (data) {
            this.title = '编辑';
            //编辑分类
            this.catetoryFrom = {
                parentId: data.ParentId === '' ? '-1' : data.ParentId,
                id: parentId,
                name: data.Name,
                sort: data.Sort,
                orgId: this.activeOrgId
            };
        } else {
            this.title = '添加';
            //打开添加分类
            this.catetoryFrom = {
                parentId: '-1',
                id: parentId,
                name: "",
                sort: 0,
                orgId: this.activeOrgId,
                children: []
            };
        }
        this.dialogFlag = true;
    },
    async getCatetoryList() {
      //获取设备分类列表
      this.configLoading = true;
      this.dialogFlag = false;
      let res = await treeSelectList();
      try {
        if (res.code == 0) {
          const data = JSON.parse(JSON.stringify(res.data));
          const resDate  = JSON.parse(JSON.stringify(res.data));
          this.setDisable (1, resDate, 2);
          this.roomCatetoryTreeList = resDate;
          this.catetoryTableData = [ {
            Name: "全部",
            Id: "",
            ParentId: '',
            Children: data
          }]
          this.configLoading = false;
        }
      } catch (error) {}
    },
    submitForm(formName) {
        this.$refs[formName].validate((valid) => {
        if (valid) {
            if (this.title === '添加' ) {
                this.getAddTree();
            } else {
                this.setEditTree();
            }
        } else {
          return false
        }
      })
    },
    // 添加分组
    getAddTree() {
        if (typeof this.catetoryFrom.parentId !== 'string') { 
            this.catetoryFrom.parentId = this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1] === "-1" ? '' : 
            this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1]
        } else {
            this.catetoryFrom.parentId = this.catetoryFrom.parentId === "-1" ? '' : this.catetoryFrom.parentId
        }
        treeSelectAdd(this.catetoryFrom).then((res) => {
            this.$message.success('添加成功!')
            this.getCatetoryList()
        })
    },
    setEditTree() {
        if (typeof this.catetoryFrom.parentId !== 'string') {
            this.catetoryFrom.parentId = this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1] === "-1" ? '' : 
            this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1]
        } else {
            this.catetoryFrom.parentId = this.catetoryFrom.parentId === "-1" ? '' : this.catetoryFrom.parentId
        }
        treeSelectEdit(this.catetoryFrom).then((res) => {
            this.$message.success('编辑成功!')
            this.getCatetoryList()
        })
    },
    //删除表格中的一行的数据
    deleteRowData(node, data) {
      this.$modal
        .confirm('是否确认移除名为"' + data.Name + '"的分组？')
        .then(function () {
          return treeSelectRemove({ id: data.Id });
        })
        .then(() => {
          this.getCatetoryList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch(() => {});
    },
    // 超过3级,不能选中,子级分类最多4级
      /**
       * count: 当前层级
       * data: 当前层级的数据
       * maxNum: 最多不能超过几级
      */
    setDisable(count, data, maxNum) {
        if (count > maxNum) { //最多几级就写几
          data.forEach(v => {
            v.disabled = true // 超过设定的最大级数,给这一集的数据添加disabled属性
          })
        } else {
          data.forEach(v => {
          v.count = count // 设置最外层数据的初始count

            if (v.Children && v.Children.length) {
              v.count++
              this.setDisable(v.count, v.Children, maxNum) // 子级循环时把这一层数据的count传入
            }
          })
        }
    },
    //点击分类节点
    clickNode(data) {
      this.$emit("clickNode", data,this.activeOrg);
    }
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
    .el-dialog__header{
        border-bottom: 1px solid #ccc;
    }
    .el-select, .el-input, .el-cascader{
        width: 100%;
    }
    .el-tree .custom-tree-node {
        //分类树的样式
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: space-between;
        font-size: 14px;
        padding-right: 8px;
        height: 45px;
    }
 }
 .groupList{
    width: 20%;
    height: 100%;
 }
 .groupList-pd{
    border-radius: 5px;
    background-color: #fff;
    border: 1px solid #f6f6f6;
 }
</style>