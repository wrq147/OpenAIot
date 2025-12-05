<template>
  <div class="groupList">
    <div class="groupList-pd">
      <!-- 分组头部 -->
      <el-row class="group-header" type="flex" justify="space-between" align="middle">
        <el-col :span="8" :offset="1">
          <span class="group-title">报表分组</span>
        </el-col>
        <el-col :span="15" class="group-actions">
          <el-button 
            type="text" 
            icon="el-icon-plus"
            @click="openAddCatetory('-1')"
            class="action-btn add-btn"
          >
            添加
          </el-button>
          <el-button 
            type="text" 
            :icon="isEnableEditor ? 'el-icon-close' : 'el-icon-edit'"
            @click="isEnableEditor = !isEnableEditor"
            class="action-btn edit-btn"
          >
            {{ isEnableEditor ? "取消" : "编辑" }}
          </el-button>
        </el-col>
      </el-row>
      
      <!-- 分组树 -->
      <div class="group-tree-container">
        <el-tree 
          ref="categoryTree" 
          v-loading="configLoading" 
          :data="catetoryTableData" 
          node-key="Id" 
          default-expand-all
          :highlight-current="true" 
          :expand-on-click-node="true" 
          :check-on-click-node="true" 
          :props="{ label: 'Name', children: 'Children' }" 
          @node-click="clickNode"
          class="custom-tree"
        >
          <span class="custom-tree-node" slot-scope="{ node, data }">
            <span class="node-label" :class="{ 'current-node': currentNodeId === data.Id }">
              {{ node.label }}
            </span>
            <span v-if="node.level > 1 && isEnableEditor" class="node-actions">
              <el-button 
                type="text" 
                size="mini" 
                @click.stop="() => openAddCatetory(data.Id, data)"
                class="node-action-btn edit"
              >
                编辑
              </el-button>
              <el-button 
                type="text" 
                size="mini" 
                @click.stop="() => deleteRowData(node, data)"
                class="node-action-btn delete"
              >
                删除
              </el-button>
            </span>
          </span>
        </el-tree>
      </div>
    </div>
    
    <!-- 编辑/添加分组对话框 -->
    <el-dialog 
      :title="title + '分组'" 
      :visible.sync="dialogFlag" 
      :close-on-click-modal="false" 
      width="500px" 
      top="10vh" 
      @close="dialogFlag = false"
      class="group-dialog"
    >
      <el-form 
        ref="catetoryFrom" 
        :model="catetoryFrom" 
        :rules="catetoryRules" 
        label-width="80px" 
        class="add-form"
      >
        <el-form-item label="父级分类" prop="parentId">
          <el-cascader
            v-model="catetoryFrom.parentId"
            :options="roomCatetoryTreeList"
            :props="cascaderProps"
            clearable
            class="form-control"
          >
          </el-cascader>
        </el-form-item> 
        <el-form-item label="分类名称" prop="name">
          <el-input 
            type="text" 
            v-model="catetoryFrom.name" 
            placeholder="请输入分类名称"
            class="form-control"
          ></el-input>
        </el-form-item>
        <el-form-item label="分类序号" prop="sort">
          <el-input 
            type="number" 
            v-model.number="catetoryFrom.sort" 
            placeholder="请输入分类序号"
            class="form-control"
          ></el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogFlag = false" class="cancel-btn">取消</el-button>
        <el-button type="primary" @click="submitForm('catetoryFrom')" class="confirm-btn">确定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { treeSelectList, treeSelectAdd, treeSelectEdit, treeSelectRemove } from "@/api/report/report";

export default { 
  name: 'groupManage',
  props: {
    dialogVisible: {
      type: Boolean
    }
  },
  data() {
    return {
      configLoading: true,
      roomCatetoryTreeList: [],
      catetoryTableData: [], // 分类列表总数居
      dialogFlag: false,
      title: '添加',
      currentNodeId: '', // 当前选中节点ID
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
      isEnableEditor: false,
      activeOrgId: '',
      activeOrg: {}
    }
  },
  async mounted() {
    this.activeOrgId = this.$store.getters.orgId;
    this.activeOrg = this.$store.getters.activeOrg;
    await this.getCatetoryList();
  },
  methods: {
    // 点击编辑/添加操作
    openAddCatetory(parentId, data) {
      // 添加顶级分类选项
      if (!this.roomCatetoryTreeList.some(item => item.Id === '-1')) {
        this.roomCatetoryTreeList.unshift({
          Name: "作为一级分类",
          Id: "-1",
          ParentId: '',
          Children: [],
          disabled: false
        });
      }
      
      if (data) {
        this.title = '编辑';
        // 编辑分类
        this.catetoryFrom = {
          parentId: data.ParentId === '' ? '-1' : data.ParentId,
          id: parentId,
          name: data.Name,
          sort: data.Sort,
          orgId: this.activeOrgId
        };
      } else {
        this.title = '添加';
        // 打开添加分类
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
      // 获取设备分类列表
      this.configLoading = true;
      this.dialogFlag = false;
      try {
        const res = await treeSelectList();
        if (res.code === 0) {
          const data = JSON.parse(JSON.stringify(res.data));
          const resDate = JSON.parse(JSON.stringify(res.data));
          this.setDisable(1, resDate, 2);
          this.roomCatetoryTreeList = resDate;
          this.catetoryTableData = [{
            Name: "全部",
            Id: "",
            ParentId: '',
            Children: data
          }];
        }
      } catch (error) {
        console.error('获取分类列表失败:', error);
      } finally {
        this.configLoading = false;
      }
    },
    
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          if (this.title === '添加') {
            this.getAddTree();
          } else {
            this.setEditTree();
          }
        } else {
          return false;
        }
      });
    },
    
    // 添加分组
    async getAddTree() {
      try {
        // 处理父级ID
        if (typeof this.catetoryFrom.parentId !== 'string') {
          this.catetoryFrom.parentId = this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1] === "-1" 
            ? '' 
            : this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1];
        } else {
          this.catetoryFrom.parentId = this.catetoryFrom.parentId === "-1" ? '' : this.catetoryFrom.parentId;
        }
        
        const res = await treeSelectAdd(this.catetoryFrom);
        this.$message.success('添加成功!');
        this.dialogFlag = false;
        await this.getCatetoryList();
      } catch (error) {
        this.$message.error('添加失败: ' + (error.message || '网络错误'));
      }
    },
    
    // 编辑分组
    async setEditTree() {
      try {
        // 处理父级ID
        if (typeof this.catetoryFrom.parentId !== 'string') {
          this.catetoryFrom.parentId = this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1] === "-1" 
            ? '' 
            : this.catetoryFrom.parentId[this.catetoryFrom.parentId.length - 1];
        } else {
          this.catetoryFrom.parentId = this.catetoryFrom.parentId === "-1" ? '' : this.catetoryFrom.parentId;
        }
        
        const res = await treeSelectEdit(this.catetoryFrom);
        this.$message.success('编辑成功!');
        this.dialogFlag = false;
        await this.getCatetoryList();
      } catch (error) {
        this.$message.error('编辑失败: ' + (error.message || '网络错误'));
      }
    },
    
    // 删除分组
    deleteRowData(node, data) {
      this.$confirm('是否确认移除名为"' + data.Name + '"的分组？', "警告", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(async () => {
        try {
          await treeSelectRemove({ id: data.Id });
          this.$message.success("移除成功");
          await this.getCatetoryList();
        } catch (error) {
          this.$message.error('删除失败: ' + (error.message || '网络错误'));
        }
      }).catch(() => {});
    },
    
    // 超过3级不能选中，子级分类最多4级
    setDisable(count, data, maxNum) {
      if (count > maxNum) {
        data.forEach(v => {
          v.disabled = true;
        });
      } else {
        data.forEach(v => {
          v.count = count;
          if (v.Children && v.Children.length) {
            v.count++;
            this.setDisable(v.count, v.Children, maxNum);
          }
        });
      }
    },
    
    // 点击分类节点
    clickNode(data) {
      this.currentNodeId = data.Id;
      this.$emit("clickNode", data, this.activeOrg);
    }
  }
};
</script>
  
<style lang="scss" scoped>
::v-deep {
  .el-tree {
    background-color: transparent;
    
    .el-tree-node {
      .el-tree-node__content {
        height: 38px;
        align-items: center;
        transition: background-color 0.2s;
        
        &:hover {
          background-color: #F0F7FF;
        }
      }
      
      .el-tree-node__expand-icon {
        color: #909399;
        
        &:hover {
          color: #409EFF;
        }
      }
      
      .el-tree-node__label {
        font-size: 14px;
        color: #606266;
      }
    }
    
    .el-tree-node.is-current > .el-tree-node__content {
      background-color: #F0F7FF;
      
      .el-tree-node__label {
        color: #409EFF;
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
      border-color: #409EFF;
      box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.2);
    }
  }
}

.groupList {
  width: 20%;
  height: 100%;
}

.groupList-pd {
  background-color: #fff;
  border: 1px solid #E4E7ED;
  min-height: 395px;
  overflow: hidden;
}

// 分组头部
.group-header {
  padding: 12px 16px;
  background-color: #FAFAFA;
  border-bottom: 1px solid #E4E7ED;
  
  .group-title {
    font-size: 15px;
    font-weight: 500;
    color: #303133;
  }
  
  .group-actions {
    display: flex;
    justify-content: flex-end;
    gap: 16px;
  }
  
  .action-btn {
    font-size: 14px;
    padding: 4px 8px;
    transition: all 0.2s;
    
    &:hover {
      color: #409EFF;
      background-color: #F0F7FF;
      border-radius: 4px;
    }
  }
  
  .add-btn {
    color: #409EFF;
  }
  
  .edit-btn {
    color: #606266;
  }
}

// 分组树容器
.group-tree-container {
  padding: 12px 8px;
  max-height: calc(100vh - 220px);
  overflow-y: auto;
  
  &::-webkit-scrollbar {
    width: 6px;
  }
  
  &::-webkit-scrollbar-track {
    background: #F5F7FA;
    border-radius: 3px;
  }
  
  &::-webkit-scrollbar-thumb {
    background: #E4E7ED;
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
    color: #409EFF;
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
    color: #409EFF;
    
    &:hover {
      background-color: rgba(64, 158, 255, 0.1);
    }
  }
  
  &.delete {
    color: #F56C6C;
    
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
    border-color: #E4E7ED;
    color: #606266;
    
    &:hover {
      background-color: #F5F7FA;
    }
  }
  
  .confirm-btn {
    background-color: #409EFF;
    border-color: #409EFF;
    
    &:hover {
      background-color: #66b1ff;
      border-color: #66b1ff;
    }
  }
}
</style>