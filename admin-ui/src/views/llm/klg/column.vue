<template>
  <div class="column-management-container">
    <div class="management-header">
      <div class="header-left" v-if="currentKb != null">
        <h2 class="page-title">{{ currentKb.Name }}</h2>
        <span v-if="currentKb" class="kb-status-tag" :class="{
          waitPublish: currentKb.Status === 0,
          published: currentKb.Status === 1,
          auditing: currentKb.Status === 2,
          auditFail: currentKb.Status === 3
        }">
          {{
            currentKb.Status === 0 ? '待发布' :
              currentKb.Status === 1 ? '已发布' :
                currentKb.Status === 2 ? '审核中' :
                  currentKb.Status === 3 ? '审核失败' : '未知状态'
          }}
        </span>
      </div>
      <div class="header-right">
        <el-dropdown>
          <el-button type="primary" icon="el-icon-plus" plain>
            新建 <i class="el-icon-arrow-down el-icon--right"></i>
          </el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item @click.native="handleAddColumn">
              <i class="el-icon-folder"></i> 新建栏目
            </el-dropdown-item>
            <el-dropdown-item @click.native="handleAddArticle">
              <i class="el-icon-document"></i> 新建文章
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
        <el-button icon="el-icon-upload2" plain>导入</el-button>
      </div>
    </div>


    <div class="management-body">
      <div class="tree-sidebar">
        <div class="sidebar-actions">
          <el-input v-model="filterText" placeholder="搜索..." prefix-icon="el-icon-search" size="small" clearable
            style="margin-bottom: 12px;">
          </el-input>
        </div>

        <div class="tree-container">
          <el-tree v-if="hasContent" :data="treeData" :props="treeProps" node-key="id" :expand-on-click-node="false"
            :filter-node-method="filterNode" ref="tree" @node-click="handleNodeClick"
            :default-expanded-keys="expandedKeys" :default-selected-keys="selectedKeys" highlight-current>
            <span class="custom-tree-node" slot-scope="{ node, data }">
              <span class="node-icon">
                <i v-if="data.isColumn" class="el-icon-folder"></i>
                <i v-else class="el-icon-document"></i>
              </span>
              <span class="node-label">{{ node.label }}</span>
              <span v-if="!data.isColumn && data.articleData" class="node-status-tag"
                :class="{ draft: data.articleData.Status === 0, published: data.articleData.Status === 1 }">
                {{ data.articleData.Status === 1 ? '已发布' : '草稿' }}
              </span>
              <span class="node-actions">
                <el-dropdown trigger="click" @click.native.stop>
                  <span class="el-dropdown-link">
                    <i class="el-icon-more"></i>
                  </span>
                  <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item v-if="data.isColumn" @click.native.stop="handleRenameColumn(data)">
                      <i class="el-icon-edit"></i> 重命名
                    </el-dropdown-item>
                    <el-dropdown-item v-if="data.isColumn" @click.native.stop="handleAddSubColumn(data)">
                      <i class="el-icon-folder-add"></i> 新建子栏目
                    </el-dropdown-item>
                    <el-dropdown-item v-if="data.isColumn" @click.native.stop="handleAddArticleUnderColumn(data)">
                      <i class="el-icon-document"></i> 新建文章
                    </el-dropdown-item>
                    <el-dropdown-item v-if="!data.isColumn" @click.native.stop="handleEditArticle(data)">
                      <i class="el-icon-edit"></i> 编辑
                    </el-dropdown-item>
                    <el-dropdown-item divided @click.native.stop="handleDeleteNode(data)">
                      <i class="el-icon-delete" style="color: #f56c6c;"></i>
                      <span style="color: #f56c6c;">删除</span>
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </el-dropdown>
              </span>
            </span>
          </el-tree>

          <div v-else class="empty-tree">
            <i class="el-icon-folder-opened" style="font-size: 48px; color: #dcdfe6;"></i>
            <p style="margin: 16px 0; color: #909399;">暂无内容</p>
            <div class="empty-buttons">
              <el-button size="small" @click="handleAddColumn">创建栏目</el-button>
              <el-button size="small" type="primary" @click="handleAddArticle">创建文章</el-button>
            </div>
          </div>
        </div>
      </div>

      <div class="content-editor">
        <div v-if="isEditingArticle" class="editor-panel">
          <div class="editor-header">
            <h3>{{ editingArticle.Id ? '编辑文章' : '新建文章' }}</h3>
            <div class="editor-actions">
              <el-button @click="cancelEdit">取消</el-button>
              <el-button type="primary" @click="saveDraft">保存</el-button>
            </div>
          </div>

          <div class="editor-bd">
            <el-form :model="editingArticle" :rules="articleRules" ref="articleForm" label-width="80px">
              <el-form-item label="标题" prop="Title">
                <el-input v-model="editingArticle.Title" placeholder="请输入文章标题" maxlength="200"></el-input>
              </el-form-item>
              <el-form-item label="所属栏目">
                <el-select v-model="editingArticle.ColumnId" placeholder="选择栏目（可选）" clearable style="width: 300px;">
                  <el-option v-for="col in allColumns" :key="col.Id" :label="col.Name" :value="col.Id">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="正文" prop="Content">
                <div class="wangeditor-container">
                  <div class="editor-mode-switch">
                    <el-button :class="{ active: editorMode === 'edit' }" @click="switchEditorMode('edit')">
                      编辑
                    </el-button>
                    <el-button :class="{ active: editorMode === 'preview' }" @click="switchEditorMode('preview')">
                      预览
                    </el-button>
                  </div>
                  <div v-show="editorMode === 'edit'" class="editor-wrapper">
                    <Toolbar style="border-bottom: 1px solid #ccc" :editor="editor" :defaultConfig="toolbarConfig"
                      :mode="editorModeType" />
                    <Editor v-model="editorContent" :defaultConfig="editorConfig" :mode="editorModeType" style="height: 350px; overflow-y: hidden;"
                      @onCreated="handleEditorCreated" />
                  </div>
                  <div v-show="editorMode === 'preview'" class="preview-wrapper" v-html="renderedMarkdown"></div>
                </div>
              </el-form-item>
            </el-form>
          </div>

        </div>

        <div v-else-if="isEditingColumn" class="editor-panel">
          <div class="editor-header">
            <h3>{{ editingColumn.Id ? '编辑栏目' : '新建栏目' }}</h3>
            <div class="editor-actions">
              <el-button @click="cancelEdit">取消</el-button>
              <el-button type="primary" @click="saveColumn">保存</el-button>
            </div>
          </div>
          <div class="editor-bd">
            <el-form :model="editingColumn" :rules="columnRules" ref="columnForm" label-width="80px">
              <el-form-item label="栏目名称" prop="Name">
                <el-input v-model="editingColumn.Name" placeholder="请输入栏目名称" maxlength="100"
                  style="width:340px;"></el-input>
              </el-form-item>
              <el-form-item label="父级栏目">
                <el-select v-model="editingColumn.ParentId" placeholder="无（顶级栏目）" clearable style="width: 300px;">
                  <el-option label="无（顶级栏目）" value=""></el-option>
                  <el-option v-for="col in availableParentColumns" :key="col.Id" :label="col.Name" :value="col.Id">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-form>
          </div>

        </div>

        <div v-else class="editor-empty">
          <div class="empty-state-box">
            <div class="status-list">
              <h4>了解知识库状态</h4>
              <div class="status-item">
                <span class="status-dot draft"></span>
                <span><b>待发布：</b>内容暂未提交，AI助手与用户无法查看</span>
              </div>
              <div class="status-item">
                <span class="status-dot auditing"></span>
                <span><b>审核中：</b>内容已提交，等待管理员审核</span>
              </div>
              <div class="status-item">
                <span class="status-dot published"></span>
                <span><b>发布：</b>已完成审核，根据公开设置区分权限，未公开仅企业内部可见</span>
              </div>
              <div class="status-item">
                <span class="status-dot auditFail"></span>
                <span><b>审核失败：</b>内容未通过审核，需修改后重新提交</span>
              </div>
            </div>
            <p class="tip-text">请从左侧目录树选择一个栏目或文章进行编辑，或点击上方按钮新建内容</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getKnowledge } from '@/api/llm/knowledge'
import { listColumn, addColumn, updateColumn, deleteColumn } from '@/api/llm/column'
import { getArticleItems, addArticle, updateArticle, deleteArticle } from '@/api/llm/article'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import markdownModule from '@wangeditor/plugin-md'
import marked from 'marked'
Boot.registerModule(markdownModule)
export default {
  name: 'ColumnManagement',
  components: { Editor, Toolbar },
  data() {
    return {
      editor: null,
      kbList: [],
      currentKb: null,
      columns: [],
      articles: [],
      allColumns: [],
      treeData: [],
      treeProps: { children: 'children', label: 'name' },
      expandedKeys: [],
      selectedKeys: [],
      filterText: '',
      isEditingColumn: false,
      isEditingArticle: false,
      editingColumn: { Id: undefined, Name: '', ParentId: "", SortOrder: 1 },
      editingArticle: { Id: undefined, Title: '', Content: '', ColumnId: null },
      articleRules: { Title: [{ required: true, message: '请输入文章标题', trigger: 'blur' }] },
      columnRules: { Name: [{ required: true, message: '请输入栏目名称', trigger: 'blur' }] },
      editorMode: 'edit',
      editorModeType: 'markdown',
      editorContent: '',
      editorConfig: {
        placeholder: '开始编写文章内容...',
        mode: 'markdown',
        autoFocus: false,
        maxLength: 50000
      },
      toolbarConfig: {},
    }
  },
  computed: {
    hasContent() {
      return this.columns.length > 0 || this.articles.length > 0
    },
    availableParentColumns() {
      return this.allColumns.filter(c => c.Id !== this.editingColumn.Id)
    },
    renderedMarkdown() {
      if (!this.editorContent) return '<p>暂无内容</p>'
      return marked(this.editorContent)
    }
  },
  watch: {
    filterText(val) {
      this.$refs.tree.filter(val)
    },
    isEditingArticle(val) {
      if (val && this.editingArticle.Content) {
        this.editorContent = this.editingArticle.Content
      }
    }
  },
  created() {
    this.loadKbDetail()
  },
  methods: {
    filterNode(value, data) {
      if (!value) return true
      return data.name.indexOf(value) !== -1
    },
    async loadKbDetail() {
      const kbId = this.$route.query.id
      const [kbRes, columnsRes, articlesRes] = await Promise.all([
        getKnowledge(kbId),
        listColumn(kbId),
        getArticleItems(kbId)
      ])
      this.currentKb = kbRes.data
      this.columns = columnsRes.data
      this.articles = articlesRes.data
      this.allColumns = this.columns
      this.buildTreeData()
    },
    buildTreeData() {
      const tree = []
      const columnMap = {}
      this.columns.forEach(col => {
        columnMap[col.Id] = { id: col.Id, name: col.Name, isColumn: true, children: [] }
      })
      this.columns.forEach(col => {
        if (col.ParentId && columnMap[col.ParentId]) {
          columnMap[col.ParentId].children.push(columnMap[col.Id])
        } else {
          tree.push(columnMap[col.Id])
        }
      })
      this.articles.forEach(article => {
        const articleNode = {
          id: 'article-' + article.Id,
          name: article.Title,
          isColumn: false,
          articleData: article
        }
        if (article.ColumnId && columnMap[article.ColumnId]) {
          columnMap[article.ColumnId].children.push(articleNode)
        } else {
          tree.push(articleNode)
        }
      })
      this.treeData = tree
    },
    handleNodeClick(data) {
      if (data.isColumn) {
        this.editorMode = 'edit'
        this.isEditingArticle = false
        this.isEditingColumn = true
        this.editingColumn = { Id: data.id, Name: data.name, ParentId: this.getParentId(data.id), SortOrder: 1 }
      } else if (data.articleData) {
        this.editorMode = 'edit'
        this.isEditingColumn = false
        this.isEditingArticle = true
        const article = data.articleData
        this.editingArticle = { Id: article.Id, Title: article.Title, Content: article.Content || '', ColumnId: article.ColumnId || null }
        this.editorContent = article.Content || ''
      }
    },
    getParentId(id) {
      const col = this.columns.find(c => c.Id === id)
      return col ? col.ParentId : ""
    },
    handleAddColumn() {
      this.isEditingArticle = false
      this.isEditingColumn = true
      this.editingColumn = { Id: undefined, Name: '', ParentId: "", SortOrder: 1 }
    },
    handleAddSubColumn(data) {
      this.isEditingArticle = false
      this.isEditingColumn = true
      this.editingColumn = { Id: undefined, Name: '', ParentId: data.id, SortOrder: 1 }
    },
    handleAddArticleUnderColumn(data) {
      this.editorMode = 'edit'
      this.isEditingColumn = false
      this.isEditingArticle = true
      this.editingArticle = {
        Id: undefined,
        Title: '',
        Content: '',
        ColumnId: data.id
      }
      this.editorContent = ''
    },
    handleAddArticle() {
      this.editorMode = 'edit'
      this.isEditingColumn = false
      this.isEditingArticle = true
      this.editingArticle = { Id: undefined, Title: '', Content: '', ColumnId: null }
      this.editorContent = ''
    },
    handleRenameColumn(data) {
      this.handleNodeClick(data)
    },
    handleEditArticle(data) {
      this.handleNodeClick(data)
    },
    handleDeleteNode(data) {
      if (data.isColumn) {
        this.$modal.confirm(`确认删除栏目"${data.name}"？该栏目下的子栏目和文章不会被自动删除。`).then(() => {
          deleteColumn(data.id).then(() => {
            this.$modal.msgSuccess('删除成功')
            this.loadKbDetail()
          })
        }).catch(() => { })
      } else {
        this.$modal.confirm(`确认删除文章"${data.name}"？`).then(() => {
          deleteArticle(data.articleData.Id).then(() => {
            this.$modal.msgSuccess('删除成功')
            this.loadKbDetail()
          })
        }).catch(() => { })
      }
    },
    cancelEdit() {
      this.isEditingColumn = false
      this.isEditingArticle = false
      this.editorMode = 'edit'
    },
    saveColumn() {
      this.$refs.columnForm.validate(valid => {
        if (!valid) return
        if (this.editingColumn.Id) {
          updateColumn(this.editingColumn).then(() => {
            this.$modal.msgSuccess('保存成功')
            this.loadKbDetail()
            this.isEditingColumn = false
          })
        } else {
          const payload = { ...this.editingColumn, KbId: this.currentKb.Id, Status: 1 }
          addColumn(payload).then(() => {
            this.$modal.msgSuccess('创建成功')
            this.loadKbDetail()
            this.isEditingColumn = false
          })
        }
      })
    },
    saveDraft() {
      this.submitArticle(0)
    },
    submitArticle(status) {
      this.$refs.articleForm.validate(valid => {
        if (!valid) return
        const payload = {
          ...this.editingArticle,
          KbId: this.currentKb.Id,
          Status: status,
          Content: this.editorContent,
          DocType: 2
        }
        if (this.editingArticle.Id) {
          updateArticle(payload).then(() => {
            this.$modal.msgSuccess(status === 1 ? '发布成功' : '草稿已保存')
            this.loadKbDetail()
            this.isEditingArticle = false
          })
        } else {
          addArticle(payload).then(() => {
            this.$modal.msgSuccess(status === 1 ? '发布成功' : '草稿已保存')
            this.loadKbDetail()
            this.isEditingArticle = false
          })
        }
      })
    },
    switchEditorMode(mode) {
      this.editorMode = mode
    },
    handleEditorCreated(editor) {
      this.editor = editor;
      editor.getConfig().set('markdown', true)
    }
  }
}
</script>
<style src="@wangeditor/editor/dist/css/style.css"></style>
<style scoped>
.column-management-container {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 80px);
  background: #f5f7fa;
}

.management-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
  background: #fff;
  box-shadow: 0 2px 3px -2px rgba(0, 0, 0, 0.12);
  border-top: 1px solid #e4e7ed;
  z-index: 99;
  position: relative;
}

.header-left {
  display: flex;
  align-items: center;
}

/* 知识库状态标签 */
.kb-status-tag {
  margin-left: 12px;
  padding: 2px 8px;
  font-size: 12px;
  border-radius: 10px;
}

/* 待发布 */
.kb-status-tag.waitPublish {
  background: #f4f4f5;
  color: #909399;
}

/* 已发布 */
.kb-status-tag.published {
  background: #f0f9eb;
  color: #67c23a;
}

/* 审核中 */
.kb-status-tag.auditing {
  background: #fdf6ec;
  color: #e6a23c;
}

/* 审核失败 */
.kb-status-tag.auditFail {
  background: #fef0f0;
  color: #f56c6c;
}

.page-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.create-dropdown {
  position: absolute;
  top: 50px;
  right: 24px;
  background: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  padding: 4px 0;
  z-index: 100;
}

.create-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  cursor: pointer;
  color: #606266;
}

.create-item:hover {
  background: #f5f7fa;
}

.management-body {
  display: flex;
  flex: 1;
  overflow: hidden;
  padding: 16px;
  gap: 16px;
}

.tree-sidebar {
  width: 320px;
  background: #fff;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.sidebar-actions {
  padding: 12px;
  border-bottom: 1px solid #ebeef5;
}

.tree-container {
  flex: 1;
  overflow-y: auto;
  padding: 8px;
  position: relative;
}

.custom-tree-node {
  flex: 1;
  display: flex;
  align-items: center;
  padding: 4px 0;
}

.node-icon {
  color: #909399;
  margin-right: 4px;
}

.node-label {
  flex: 1;
  font-size: 13px;
  color: #606266;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.node-status-tag {
  font-size: 10px;
  padding: 1px 6px;
  border-radius: 3px;
  margin-left: 6px;
}

.node-status-tag.draft {
  background: #f4f4f5;
  color: #909399;
}

.node-status-tag.published {
  background: #f0f9eb;
  color: #67c23a;
}

.node-actions {
  opacity: 0;
  transition: opacity 0.2s;
}

.custom-tree-node:hover .node-actions {
  opacity: 1;
}

.node-actions .el-dropdown-link {
  cursor: pointer;
  padding: 2px 4px;
  border-radius: 3px;
}

.node-actions .el-dropdown-link:hover {
  background: #f5f7fa;
}

.empty-tree {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.empty-buttons {
  display: flex;
  gap: 8px;
}

.content-editor {
  flex: 1;
  background: #fff;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.editor-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #ebeef5;
}

.editor-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
}

.editor-bd {
  padding: 20px;
}

.editor-actions {
  display: flex;
  gap: 8px;
}


.wangeditor-container {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
}

.editor-mode-switch {
  display: flex;
  border-bottom: 1px solid #dcdfe6;
}

.editor-mode-switch .el-button {
  border: none;
  border-radius: 0;
  margin: 0;
}

.editor-mode-switch .el-button.active {
  background: #fff;
  border-bottom: 2px solid #1890ff;
  color: #1890ff;
}

.preview-wrapper {
  padding: 20px;
  background: #fff;
  overflow-y: auto;
}

.editor-empty {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px;
}

.empty-state-box {
  max-width: 500px;
  width: 100%;
}

.status-list {
  background: #fff;
  border: 1px solid #ebeef5;
  border-radius: 8px;
  padding: 24px;
  margin-bottom: 24px;
}

.status-list h4 {
  margin: 0 0 16px 0;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}

.status-item {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 12px;
  font-size: 13px;
  color: #606266;
}

.status-item:last-child {
  margin-bottom: 0;
}

.status-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
}

.status-dot.draft {
  background: #909399;
}

.status-dot.published {
  background: #67c23a;
}

.status-dot.auditing {
  background: #e6a23c;
}

.status-dot.auditFail {
  background: #f56c6c;
}

.tip-text {
  text-align: center;
  color: #909399;
  font-size: 13px;
}
</style>