<template>
  <div class="groupList">
    <div class="groupList-pd">
      <!-- 分组头部 -->
      <el-row class="group-header" type="flex" justify="space-between" align="top">
        <el-col :span="24" style="margin-bottom:0">
          <span class="group-title">规则分组</span>
        </el-col>
        <el-col :span="15" class="group-actions">
          <el-button  type="text"  icon="el-icon-plus" @click="openAddCatetory('-1')" class="action-btn add-btn">
            添加
          </el-button>
          <el-button  type="text"  :icon="isEnableEditor ? 'el-icon-close' : 'el-icon-edit'" @click="isEnableEditor = !isEnableEditor" class="action-btn edit-btn">
            {{ isEnableEditor ? "取消" : "编辑" }}
          </el-button>
        </el-col>
      </el-row>

      <!-- 分组树 -->
      <div class="group-tree-container">
        <el-tree @node-collapse="handleNodeCollapse" ref="categoryTree" v-loading="configLoading" :data="catetoryTableData" node-key="Id" default-expand-all :highlight-current="true"
          :expand-on-click-node="true" :check-on-click-node="true" :props="{ label: 'GroupName', children: 'Children' }" @node-click="clickNode" class="custom-tree" :class="{'edit_tree':isEnableEditor}">
          <span class="custom-tree-node" slot-scope="{ node, data }">
            <el-tooltip class="item" effect="dark" :content="node.label" placement="top">
              <span class="node-label" :class="{ 'current-node': currentNodeId === data.Id }">
                {{ node.label }}
              </span>
            </el-tooltip>
            <span v-if="node.level > 1 && isEnableEditor" class="node-actions">
              <el-button  type="text"  size="mini"  @click.stop="() => openAddCatetory(data.Id, data)" class="node-action-btn edit">
                编辑
              </el-button>
              <el-button  type="text"  size="mini"  @click.stop="() => deleteRowData(data,node)" class="node-action-btn delete">
                删除
              </el-button>
            </span>
          </span>
        </el-tree>
      </div>
    </div>
    <el-dialog title="添加分组" :visible.sync="groupOpen" center width="600px" :close-on-click-modal="false">
      <el-form :model="groupFrom" ref="groupFrom" :rules="groupRules" label-position="left" class="groupFrom" :inline="true" label-width="80px">
        <el-form-item label="父级分组" prop="parentIdData">
          <treeselect class="groupSet" v-model="groupFrom.parentIdData" :options="groupTreeList" :show-count="true" :normalizer="normalizer" placeholder="请选择规则分组" @select="selectGroupTree" :disabled="groupFormDisable"/>
        </el-form-item>
        <el-form-item label="分组名称" prop="groupName">
          <el-input type="text" v-model="groupFrom.groupName" placeholder="请输入分组名称" :disabled="groupFormDisable"></el-input>
        </el-form-item>
        <el-form-item label="分组序号" prop="sort">
          <el-input type="number" v-model.number="groupFrom.sort" placeholder="请输入分组序号" :disabled="groupFormDisable"></el-input>
        </el-form-item>
        <el-form-item label="分组描述" prop="remark">
          <el-input type="text" v-model="groupFrom.remark" placeholder="请输入分组描述" :disabled="groupFormDisable"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="groupOpen = false">取 消</el-button>
        <el-button type="primary" @click="addDeviceGroup">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  addGroup,
  removeGroup,
  editGroup,
} from "@/api/rules/ruselSevic";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
export default {
  name: "groupManage",
  components: {
    Treeselect
  },
  props: {
    dialogVisible: {
      type: Boolean,
    },
    catetoryTableData:{
      type:Array,
      default:()=>{
        return []
      }
    },
    beforeGroupTree:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      configLoading: false,
      title: "添加",
      currentNodeId: "", // 当前选中节点ID
      isEnableEditor:false,
      groupOpen: false, //添加分组弹窗
      groupFrom: {
        parentIdData: "",
        id: "",
        groupName: "",
        sort: "",
        remark: "",
      },
      groupRules: {
        parentIdData: [
          { required: true, trigger: "change", message: "请选择父级分组" },
        ],
        groupName: [
          { required: true, trigger: "blur", message: "请输入分组名称" },
        ],
        sort: [{ required: true, trigger: "blur", message: "请输入分组序号" }],
      },
      groupFormDisable:false,
      groupTreeList:[]
    };
  },
  async mounted() {
  },
  methods: {
    deleteRowData(row) {
      //删除表格中的一行的数据
      this.$modal.confirm('是否确认移除名为"' + row.GroupName + '"的设备分组？')
      .then(function () {
        return removeGroup({ id: row.Id });
      })
      .then(() => {
        // this.getGroupList();
        this.$emit('getGroupList')
        this.$modal.msgSuccess("移除成功");
      })
      .catch(() => {});
    },
    selectGroupTree() {},
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.GroupName,
        children: node.Children,
      };
    },
    editRowData(row, isdisable) {
      //修改一行的数据
      this.openAddGroup();
      this.groupFrom = {
        parentIdData: row.ParentId == "" ? -1 : row.ParentId,
        id: row.Id,
        groupName: row.GroupName,
        sort: row.Sort,
        remark: row.Remark,
      };
      // console.log(isdisable, "是否禁用");
      if (isdisable) {
        this.groupFormDisable = true;
      } else {
        this.groupFormDisable = false;
      }
    },
    addDeviceGroup() {
      //添加设备分组
      this.$refs["groupFrom"].validate((valid) => {
        if (valid) {
          if (this.groupFrom.parentIdData == -1) {
            this.groupFrom.parentId = "";
          } else {
            this.groupFrom.parentId = this.groupFrom.parentIdData;
          }
          if (this.groupFrom.id) {
            editGroup(this.groupFrom)
              .then((response) => {
                this.$message.success("修改分组成功");
                if (response.code == 0) {
                  // this.getGroupList();
                  this.$emit('getGroupList')
                }
                this.groupOpen = false;
              })
              .catch((err) => {
                this.$message.error(err.message);
              });
          } else {

            addGroup(this.groupFrom)
              .then((response) => {
                this.$message.success("创建分组成功");
                if (response.code == 0) {
                  // this.getGroupList();
                  this.$emit('getGroupList')
                }
                this.groupOpen = false;
              })
              .catch((err) => {
                this.$message.error(err.message);
              });
          }
        }
      });
    },
    openAddGroup() {
      //打开添加分组
      this.resetForm("groupFrom");
      this.groupTreeList=JSON.parse(JSON.stringify(this.beforeGroupTree))
      this.groupFrom = {
        parentIdData: null,
        id: "",
        groupName: "",
        sort: "",
        remark: "",
      };
      if (!this.groupTreeList[0] || this.groupTreeList[0].Id != "-1") {
        this.groupTreeList.unshift({
          GroupName: "作为一级分组",
          Id: "-1",
          ParentId: 0,
        });
      }
      this.groupOpen = true;
      this.groupFormDisable = false;
    },
    openAddCatetory(parentId, data) {
      if(parentId=='-1'){
        this.openAddGroup()
      }else{
        this.editRowData(data,false)
      }
    },
    handleNodeCollapse(){
      return false;
    },
    // 点击分类节点
    clickNode(data) {
      this.currentNodeId = data.Id;
      this.$emit("clickNode", data);
    },
  },
};
</script>
  
<style lang="scss" scoped>
::v-deep {
  .el-tree {
    background-color: transparent;
    &.edit_tree{
      .el-tree-node {
        .el-tree-node__children{
          overflow-x: auto;
        }
      }
    }
    .el-tree-node {
      .el-tree-node__content {
        height: 38px;
        align-items: center;
        transition: background-color 0.2s;

        &:hover {
          background-color: #f0f7ff;
        }
      }
  
      .el-tree-node__expand-icon {
        color: #909399;

        &:hover {
          color: #409eff;
        }
        &.is-leaf{
          color: transparent;
        }
      }

      .el-tree-node__label {
        font-size: 14px;
        color: #606266;
      }
    }

    .el-tree-node.is-current > .el-tree-node__content {
      background-color: #f0f7ff;

      .el-tree-node__label {
        color: #409eff;
        font-weight: 500;
      }
    }

    .el-tree-node.is-disabled {
      .el-tree-node__label {
        color: #909399;
        cursor: not-allowed;
      }
    }
  }

  .el-cascader {
    .el-input__inner {
      height: 36px;
      border-radius: 4px;
    }
  }

  .el-input__inner {
    height: 36px;
    border-radius: 4px;
    transition: all 0.3s;

    &:focus {
      border-color: #409eff;
      box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.2);
    }
  }
}

.groupList {
  height: 100%;
  width: 100%;
}

.groupList-pd {
  background-color: #fff;
  border: 1px solid #e4e7ed;
  min-height: 395px;
  overflow: hidden;
  width: 100%;
}

// 分组头部
.group-header {
  padding: 12px 6px 12px 10px;
  background-color: #fafafa;
  border-bottom: 1px solid #e4e7ed;
  .group-title {
    font-size: 15px;
    font-weight: 500;
    color: #303133;
  }

  .group-actions {
    display: flex;
    justify-content: flex-end;
  }

  .action-btn {
    font-size: 14px;
    padding: 4px 2px;
    transition: all 0.2s;

    &:hover {
      color: #409eff;
      background-color: #f0f7ff;
      border-radius: 4px;
    }
  }

  .add-btn {
    color: #409eff;
  }

  .edit-btn {
    color: #606266;
    margin-left: 2px;
  }
}

// 分组树容器
.group-tree-container {
  padding: 12px 8px;
  max-height: calc(100vh - 220px);
  overflow-y: auto;
  width: 100%;
  box-sizing: border-box;
  &::-webkit-scrollbar {
    width: 6px;
  }

  &::-webkit-scrollbar-track {
    background: #f5f7fa;
    border-radius: 3px;
  }

  &::-webkit-scrollbar-thumb {
    background: #e4e7ed;
    border-radius: 3px;

    &:hover {
      background: #c0c4cc;
    }
  }
}

.custom-tree {
  padding: 4px 0;
}

.custom-tree-node {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 0 8px;

}

.node-label {
  flex: 1;
  font-size: 14px;
  color: #606266;
  transition: all 0.2s;

  &.current-node {
    color: #409eff;
    font-weight: 500;
  }
}

.node-actions {
  display: flex;
  gap: 8px;
}

.node-action-btn {
  font-size: 12px;
  padding: 2px 6px;
  height: auto;

  &.edit {
    color: #409eff;

    &:hover {
      background-color: rgba(64, 158, 255, 0.1);
    }
  }

  &.delete {
    color: #f56c6c;
    margin-left: 2px;
    &:hover {
      background-color: rgba(245, 108, 108, 0.1);
    }
  }
}

// 对话框样式
.group-dialog {
  ::v-deep .el-dialog__body {
    padding: 20px;
  }

  .add-form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .form-control {
    width: 100%;
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
  }

  .cancel-btn {
    border-color: #e4e7ed;
    color: #606266;

    &:hover {
      background-color: #f5f7fa;
    }
  }

  .confirm-btn {
    background-color: #409eff;
    border-color: #409eff;

    &:hover {
      background-color: #66b1ff;
      border-color: #66b1ff;
    }
  }
}
</style>