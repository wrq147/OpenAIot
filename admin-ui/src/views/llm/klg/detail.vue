<template>
  <div class="knowledge-detail-container">
    <div class="detail-header">
      <div class="header-top">
        <div class="breadcrumb">
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/llm/klg/index' }">知识库管理</el-breadcrumb-item>
            <el-breadcrumb-item>{{ knowledge.Name }}</el-breadcrumb-item>
            <template v-for="(item, index) in currentPath">
              <el-breadcrumb-item :key="index">{{ item.name }}</el-breadcrumb-item>
            </template>
          </el-breadcrumb>
        </div>
        <div class="header-actions">
          <el-input v-model="searchKeyword" placeholder="搜索文档..." style="width: 240px;">
            <i slot="prefix" class="el-icon-search"></i>
          </el-input>
          <el-button icon="el-icon-setting" circle></el-button>
          <el-button icon="el-icon-user" circle></el-button>
        </div>
      </div>
    </div>

    <div class="detail-body">
      <div class="sidebar">
        <div class="sidebar-header">
          <div class="sidebar-title">
            <i class="el-icon-collection"></i>
            <span>{{ knowledge.Name }}</span>
          </div>
        </div>

        <div class="nav-tree">
          <el-tree
            :data="treeData"
            :props="treeProps"
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            ref="tree"
            @node-click="handleNodeClick"
            :default-expanded-keys="expandedKeys"
            :default-selected-keys="selectedKeys"
          >
            <span class="custom-tree-node" slot-scope="{ node, data }">
              <span class="node-icon">
                <i v-if="data.isColumn" class="el-icon-folder"></i>
                <i v-else class="el-icon-document"></i>
              </span>
              <span>{{ node.label }}</span>
            </span>
          </el-tree>
        </div>
      </div>

      <div class="content-area">
        <div v-if="selectedArticle" class="article-content">
          <div class="article-header">
            <h1>{{ selectedArticle.Title }}</h1>
            <div class="article-meta">
              <span class="author">{{ selectedArticle.createName || '管理员' }}</span>
              <span class="dot">·</span>
              <span>{{ parseTime(selectedArticle.createTime) }}</span>
              <span class="dot">·</span>
              <span>修改</span>
              <span class="dot">·</span>
              <span><i class="el-icon-view"></i> {{ selectedArticle.ViewCount || 0 }} 次阅读</span>
            </div>
          </div>

          <div class="article-body" v-html="renderedContent"></div>

          <div class="article-footer">
            <div class="action-buttons">
              <el-button size="small"><i class="el-icon-thumbs-up"></i> 点赞</el-button>
              <el-button size="small"><i class="el-icon-star-off"></i> 收藏</el-button>
              <el-button size="small"><i class="el-icon-share"></i> 分享</el-button>
            </div>
          </div>
        </div>

        <div v-else class="empty-content">
          <div class="empty-box">
            <i class="el-icon-document" style="font-size: 64px; color: #dcdfe6;"></i>
            <p style="margin: 16px 0 0 0; color: #909399; font-size: 16px;">请从左侧选择一篇文章进行阅读</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getKnowledge } from '@/api/llm/knowledge'
import { getColumnsByKbId } from '@/api/llm/column'
import { getArticlesByKbId } from '@/api/llm/article'
import marked from 'marked'

export default {
  name: 'KnowledgeDetail',
  data() {
    return {
      knowledge: {},
      columns: [],
      articles: [],
      treeData: [],
      treeProps: {
        children: 'children',
        label: 'name'
      },
      expandedKeys: [],
      selectedKeys: [],
      selectedArticle: null,
      currentPath: [],
      searchKeyword: ''
    }
  },
  computed: {
    renderedContent() {
      if (!this.selectedArticle?.Content) {
        return '<p style="color: #909399;">暂无内容</p>'
      }
      return marked(this.selectedArticle.Content)
    }
  },
  watch: {
    searchKeyword(val) {
      this.$refs.tree.filter(val)
    }
  },
  created() {
    this.getDetail()
  },
  methods: {
    filterNode(value, data) {
      if (!value) return true
      return data.name.indexOf(value) !== -1
    },
    async getDetail() {
      const kbId = this.$route.params.id
      const [kbRes, columnsRes, articlesRes] = await Promise.all([
        getKnowledge(kbId),
        getColumnsByKbId(kbId),
        getArticlesByKbId(kbId)
      ])

      this.knowledge = kbRes.data
      this.columns = columnsRes.data
      this.articles = articlesRes.data
      this.buildTreeData()
    },
    buildTreeData() {
      const tree = []
      const columnMap = {}

      this.columns.forEach(col => {
        columnMap[col.Id] = {
          id: col.Id,
          name: col.Name,
          isColumn: true,
          children: []
        }
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
          id: article.Id,
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
      if (!data.isColumn && data.articleData) {
        this.selectedArticle = data.articleData
        this.selectedKeys = [data.id]
        this.updatePath(data)
      }
    },
    updatePath(data) {
      const path = []
      let current = data
      while (current) {
        path.unshift({ id: current.id, name: current.name })
        current = this.findParent(current)
      }
      this.currentPath = path
    },
    findParent(data) {
      for (const item of this.treeData) {
        const found = this.findParentRecursive(item, data.id)
        if (found) return found
      }
      return null
    },
    findParentRecursive(node, childId) {
      if (node.children) {
        for (const child of node.children) {
          if (child.id === childId) return node
          const found = this.findParentRecursive(child, childId)
          if (found) return found
        }
      }
      return null
    },
    parseTime(time) {
      if (!time) return '刚刚'
      const date = new Date(time)
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      return `${year}年${month}月${day}日`
    }
  }
}
</script>

<style scoped>
.knowledge-detail-container {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 80px);
  background: #f5f7fa;
}

.detail-header {
  background: #fff;
  padding: 12px 24px;
  border-bottom: 1px solid #e4e7ed;
}

.header-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.breadcrumb {
  font-size: 14px;
  color: #606266;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.detail-body {
  display: flex;
  flex: 1;
  overflow: hidden;
  max-width: 1400px;
  margin: 0 auto;
  width: 100%;
}

.sidebar {
  width: 280px;
  background: #fff;
  border-right: 1px solid #e4e7ed;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.sidebar-header {
  padding: 16px;
  border-bottom: 1px solid #e4e7ed;
}

.sidebar-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  color: #303133;
}

.nav-tree {
  flex: 1;
  overflow-y: auto;
  padding: 8px;
}

.custom-tree-node {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #606266;
}

.node-icon {
  font-size: 14px;
  color: #909399;
}

.content-area {
  flex: 1;
  overflow-y: auto;
  background: #fff;
}

.article-content {
  padding: 40px 60px;
  max-width: 900px;
  margin: 0 auto;
}

.article-header {
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #ebeef5;
}

.article-header h1 {
  font-size: 32px;
  font-weight: 700;
  color: #303133;
  margin: 0 0 16px 0;
  line-height: 1.3;
}

.article-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #909399;
}

.article-meta .author {
  color: #606266;
  font-weight: 500;
}

.article-meta .dot {
  color: #dcdfe6;
}

.article-body {
  font-size: 16px;
  line-height: 2;
  color: #303133;
}

.article-body :deep(h1),
.article-body :deep(h2) {
  font-size: 24px;
  font-weight: 600;
  color: #303133;
  margin: 32px 0 16px 0;
  padding-bottom: 8px;
  border-bottom: 1px solid #ebeef5;
}

.article-body :deep(h3) {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
  margin: 24px 0 12px 0;
}

.article-body :deep(p) {
  margin: 16px 0;
}

.article-body :deep(ul),
.article-body :deep(ol) {
  margin: 16px 0;
  padding-left: 24px;
}

.article-body :deep(li) {
  margin: 8px 0;
}

.article-body :deep(code) {
  background: #f5f7fa;
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 14px;
  color: #e83e8c;
}

.article-body :deep(pre) {
  background: #282c34;
  padding: 16px;
  border-radius: 8px;
  overflow-x: auto;
  margin: 16px 0;
}

.article-body :deep(pre code) {
  background: transparent;
  color: #abb2bf;
  padding: 0;
}

.article-footer {
  margin-top: 48px;
  padding-top: 24px;
  border-top: 1px solid #ebeef5;
}

.action-buttons {
  display: flex;
  justify-content: center;
  gap: 16px;
}

.empty-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
}

.empty-box {
  text-align: center;
}
</style>