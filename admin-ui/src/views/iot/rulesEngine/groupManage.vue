<template>
  <div class="groupList">
    <div class="groupList-pd">
      <!-- 分组头部 -->
      <el-row class="group-header">
        <!-- 标题+数量+新增按钮同一行 -->
        <el-col :span="24" class="header-title-row">
          <div class="header-left">
            <span class="group-title">规则分组</span>
            <el-tag v-if="catetoryTableData.length" size="mini" type="info">{{ treeTotal }} 个分组</el-tag>
          </div>
          <div class="header-right">
            <el-link icon="el-icon-plus" type="primary" @click="openAddCategory('-1')">新增一级</el-link>
          </div>
        </el-col>
      </el-row>

      <!-- 分组树容器 -->
      <div class="group-tree-container" v-loading="configLoading">
        <!-- 空白占位 -->
        <div v-if="!catetoryTableData.length" class="tree-empty">
          <i class="el-icon-folder-opened empty-icon"></i>
          <p>暂无规则分组，点击上方新增分组</p>
        </div>

        <el-tree v-else ref="categoryTree" :data="catetoryTableData" node-key="Id" default-expand-all
          :highlight-current="true" expand-on-click-node check-on-click-node
          :props="{ label: 'GroupName', children: 'Children' }" @node-click="clickNode"
          @node-collapse="handleNodeCollapse" class="custom-tree" :indent="24">
          <span class="custom-tree-node" slot-scope="{ node, data }">
            <!-- 文件夹图标区分层级 -->
            <i :class="[
              node.isLeaf ? 'el-icon-document' : node.expanded ? 'el-icon-folder-opened' : 'el-icon-folder',
              'node-icon'
            ]"></i>
            <!-- 分组名称，超长省略 -->
            <el-tooltip effect="dark" :content="node.label" placement="top-start" popper-class="tree-tooltip">
              <span class="node-label" :class="{ 'current-node': currentNodeId === data.Id }">
                {{ node.label }}
              </span>
            </el-tooltip>

            <!-- 右侧三点下拉菜单 新增添加子节点 -->
            <el-popover placement="bottom-end" width="100" trigger="click" :close-delay="150">
              <div class="tree-menu">
                <div class="menu-item" @click="openAddCategory(data.Id)">添加子节点</div>
                <div class="menu-item" @click="openAddCategory(data.Id, data)">编辑</div>
                <div class="menu-item danger" @click="deleteRowData(data, node)">删除</div>
              </div>
              <span slot="reference" class="tree-more-btn">
                <i class="el-icon-more"></i>
              </span>
            </el-popover>
          </span>
        </el-tree>
      </div>
    </div>

    <!-- 新增/编辑弹窗 -->
    <el-dialog :title="groupForm.id ? '编辑分组' : '新增分组'" :visible.sync="groupOpen" center width="620px"
      :close-on-click-modal="false" destroy-on-close class="group-dialog">
      <el-form :model="groupForm" ref="groupForm" :rules="groupRules" label-position="left" label-width="90px"
        class="group-form">
        <el-form-item label="父级分组" prop="parentIdData">
          <treeselect v-model="groupForm.parentIdData" :options="groupTreeList" :normalizer="normalizer"
            placeholder="请选择父级分组（一级分组选「作为一级分组」）" :show-count="true" searchable :disabled="groupFormDisable"
            class="group-treeselect" />
        </el-form-item>
        <el-form-item label="分组名称" prop="groupName">
          <el-input v-model="groupForm.groupName" placeholder="请输入分组名称" :disabled="groupFormDisable"
            @keyup.enter="submitForm" />
        </el-form-item>
        <el-form-item label="排序序号" prop="sort">
          <el-input v-model.number="groupForm.sort" type="number" min="0" placeholder="数字越小排序越靠前"
            :disabled="groupFormDisable" @keyup.enter="submitForm" />
        </el-form-item>
        <el-form-item label="分组描述" prop="remark">
          <el-input v-model="groupForm.remark" type="textarea" :rows="3" placeholder="选填，填写分组用途说明"
            :disabled="groupFormDisable" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="closeDialog">取消</el-button>
        <el-button type="primary" @click="submitForm">确定保存</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { addGroup, removeGroup, editGroup } from "@/api/rules/ruselSevic";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";

export default {
  name: "GroupManage",
  components: { Treeselect },
  props: {
    catetoryTableData: {
      type: Array,
      default: () => []
    },
    beforeGroupTree: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      configLoading: false,
      currentNodeId: "",
      groupOpen: false,
      groupFormDisable: false,
      // 表单
      groupForm: {
        parentIdData: "",
        id: "",
        groupName: "",
        sort: 0,
        remark: ""
      },
      groupRules: {
        parentIdData: [{ required: true, message: "请选择父级分组", trigger: "change" }],
        groupName: [{ required: true, message: "请输入分组名称", trigger: ["blur", "change"] }],
        sort: [{ required: true, message: "请输入排序序号", trigger: ["blur", "change"] }]
      },
      groupTreeList: [],
    };
  },
  computed: {
    treeTotal() {
      // 递归统计所有分组数量
      const countTree = (list) => {
        let count = 0;
        list.forEach(item => {
          count++;
          if (item.Children?.length) count += countTree(item.Children);
        });
        return count;
      };
      return countTree(this.catetoryTableData);
    }
  },
  mounted() { },
  beforeDestroy() { },
  methods: {
    // 获取所有节点ID，用于一键展开
    getAllNodeIds() {
      const ids = [];
      const loop = (list) => {
        list.forEach(item => {
          ids.push(item.Id);
          if (item.Children?.length) loop(item.Children);
        });
      };
      loop(this.catetoryTableData);
      return ids;
    },
    // 树形数据标准化
    normalizer(node) {
      const copy = { ...node };
      if (!copy.Children?.length) delete copy.Children;
      return {
        id: copy.Id,
        label: copy.GroupName,
        children: copy.Children
      };
    },
    // 打开弹窗统一入口
    openAddCategory(parentId, data) {
      this.resetGroupForm();
      this.groupTreeList = JSON.parse(JSON.stringify(this.beforeGroupTree));
      // 根节点选项前置
      if (!this.groupTreeList.some(i => i.Id === "-1")) {
        this.groupTreeList.unshift({
          GroupName: "作为一级分组",
          Id: "-1",
          ParentId: 0
        });
      }

      if (parentId === "-1") {
        // 新增一级分组
        this.groupForm.parentIdData = "-1";
        this.groupForm.id = "";
        this.groupFormDisable = false;
      } else if (data) {
        // 编辑当前节点
        this.groupForm.id = data.Id;
        this.groupForm.parentIdData = data.ParentId || "-1";
        this.groupForm.groupName = data.GroupName;
        this.groupForm.sort = data.Sort ?? 0;
        this.groupForm.remark = data.Remark ?? "";
        this.groupFormDisable = false;
      } else {
        // 新增子级（菜单添加子节点走这里）
        this.groupForm.parentIdData = parentId;
        this.groupForm.id = "";
        this.groupFormDisable = false;
      }
      this.groupOpen = true;
      this.$nextTick(() => this.$refs.groupForm.clearValidate());
    },
    // 重置表单
    resetGroupForm() {
      this.groupForm = {
        parentIdData: "",
        id: "",
        groupName: "",
        sort: 0,
        remark: ""
      };
    },
    // 关闭弹窗
    closeDialog() {
      this.groupOpen = false;
      this.resetGroupForm();
      this.$refs.groupForm.clearValidate();
    },
    // 表单提交
    submitForm() {
      this.$refs.groupForm.validate(async valid => {
        if (!valid) return;
        const params = { ...this.groupForm };
        params.parentId = params.parentIdData === "-1" ? "" : params.parentIdData;
        delete params.parentIdData;

        try {
          if (params.id) {
            await editGroup(params);
            this.$message.success("分组修改成功");
          } else {
            await addGroup(params);
            this.$message.success("分组创建成功");
          }
          this.groupOpen = false;
          this.$emit("getGroupList");
        } catch (err) {
          this.$message.error(err.message || "操作失败，请重试");
        }
      });
    },
    // 删除分组
    deleteRowData(row, node) {
      const hasChild = row.Children && row.Children.length > 0;
      const tip = hasChild
        ? `分组【${row.GroupName}】包含子分组，删除后子分组将一并移除，确认删除？`
        : `确认删除分组【${row.GroupName}】？`;
      this.$modal.confirm(tip).then(async () => {
        await removeGroup({ id: row.Id });
        this.$modal.msgSuccess("分组已删除");
        this.$emit("getGroupList");
      }).catch(() => { });
    },
    // 节点点击
    clickNode(data) {
      this.currentNodeId = data.Id;
      this.$emit("clickNode", data);
    },
    // 节点折叠
    handleNodeCollapse() {
      return false;
    },
    // treeselect选中回调
    selectGroupTree() { },
  }
};
</script>

<style lang="scss" scoped>
// 全局深度样式
::v-deep {
  // 树形基础美化 + 层级连接线 + 文字左移核心样式
  .el-tree {
    background: transparent;
    color: #303133;

    // 层级虚线连接线
    .el-tree-node {
      position: relative;
      padding-left: 4px;

      .el-tree-node__children {
        margin-left: 24px;
        position: relative;
      }

      .el-tree-node__children::before {
        content: "";
        position: absolute;
        left: -12px;
        top: 0;
        width: 1px;
        height: 100%;
        border-left: 1px dashed #dcdfe6;
      }

      .el-tree-node__content {
        height: 40px;
        align-items: center;
        transition: all 0.2s;
        border-radius: 6px;
        margin: 2px 0;
        padding: 0 4px !important; // 缩小左侧内边距，文字整体左靠
        cursor: pointer;

        &:hover {
          background-color: #f0f7ff;
        }
      }

      // 展开收起箭头缩小间距，叶子节点清除占位
      .el-tree-node__expand-icon {
        color: #909399;
        font-size: 14px;
        margin-right: 4px !important;

        &:hover {
          color: #409eff;
        }

        &.is-leaf {
          visibility: hidden;
          width: 0 !important;
          margin-right: 0 !important;
        }
      }
    }

    // 当前选中节点高亮
    .el-tree-node.is-current>.el-tree-node__content {
      background-color: #e6f2ff;
      border: 1px solid #409eff;

      .node-label {
        color: #409eff;
        font-weight: 500;
      }
    }
  }

  // treeselect 样式
  .vue-treeselect__control {
    height: 36px !important;
  }

  .vue-treeselect__list {
    max-height: 260px;
  }

  // 弹窗表单输入框统一高度
  .el-input__inner,
  .el-textarea__inner {
    border-radius: 6px;
    height: 36px;
    transition: border 0.2s;

    &:focus {
      border-color: #409eff;
      box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.15);
    }
  }

  .el-textarea__inner {
    height: auto;
  }

  // tooltip 宽度限制
  .tree-tooltip {
    max-width: 320px !important;
  }
}

.groupList {
  width: 100%;
  height: 100%;
}

.groupList-pd {
  background: #fff;
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  min-height: 420px;
  overflow: hidden;
}

.group-header {
  padding: 14px 16px;
  background: #fafafa;
  border-bottom: 1px solid #e4e7ed;

  .header-title-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 0px;
  }

  .header-left {
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .group-title {
    font-size: 16px;
    font-weight: 500;
    color: #303133;
  }
}

// 树形容器
.group-tree-container {
  padding: 12px 10px;
  max-height: calc(100vh - 240px);
  overflow-y: auto;
  box-sizing: border-box;

  // 自定义滚动条
  &::-webkit-scrollbar {
    width: 6px;
  }

  &::-webkit-scrollbar-track {
    background: #f5f7fa;
    border-radius: 3px;
  }

  &::-webkit-scrollbar-thumb {
    background: #dcdfe6;
    border-radius: 3px;

    &:hover {
      background: #c0c4cc;
    }
  }
}

// 空白状态
.tree-empty {
  height: 320px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #909399;

  .empty-icon {
    font-size: 48px;
    margin-bottom: 12px;
    color: #c0c4cc;
  }
}

// 自定义节点 缩小图标文字间距，文字靠左
.custom-tree {
  padding: 4px 0;
}

.custom-tree-node {
  display: flex;
  align-items: center;
  width: 100%;
  gap: 4px;
  position: relative;

  .node-icon {
    font-size: 16px;
    color: #909399;
  }

  .node-label {
    flex: 1;
    font-size: 14px;
    color: #606266;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  // 右侧三点按钮：默认隐藏
  .tree-more-btn {
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    cursor: pointer;
    color: #909399;
    font-size: 16px;
    opacity: 0; // 默认透明隐藏
    transition: opacity 0.2s ease;

    &:hover {
      background: #e4e7ed;
      color: #303133;
    }
  }

  // 父节点悬浮时显示三点按钮
  &:hover .tree-more-btn {
    opacity: 1;
  }
}

// 节点下拉菜单
.tree-menu {
  .menu-item {
    padding: 6px 12px;
    cursor: pointer;
    font-size: 13px;

    &:hover {
      background: #f0f7ff;
    }

    &.danger {
      color: #f56c6c;

      &:hover {
        background: #fef0f0;
      }
    }
  }
}

// 弹窗样式
.group-dialog {
  ::v-deep .el-dialog__body {
    padding: 20px 24px;
  }

  .group-form {
    .el-form-item {
      margin-bottom: 18px;
    }
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding-top: 10px;
  }
}
</style>