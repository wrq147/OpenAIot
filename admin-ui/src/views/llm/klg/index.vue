<template>
  <div class="knowledge-base-container">
    <!-- 顶部操作栏 -->
    <div class="header-bar">
      <div class="header-left">
        <h2 class="page-title">知识库管理</h2>
        <span class="page-desc">统一管理业务知识库，支持文档导入与检索</span>
      </div>
      <div class="header-right">
        <el-button type="primary" icon="el-icon-plus" @click="handleAdd">
          新建知识库
        </el-button>
      </div>
    </div>

    <!-- 搜索区域 -->
    <div class="search-wrapper">
      <el-input
        v-model="searchKeyword"
        placeholder="输入知识库名称快速检索"
        class="search-input"
        clearable
        @keyup.enter.native="handleSearch"
      >
        <el-button slot="append" icon="el-icon-search" @click="handleSearch">搜索</el-button>
      </el-input>
      <el-button @click="resetSearch">重置</el-button>
    </div>

    <!-- 知识库卡片网格 -->
    <div class="knowledge-grid">
      <div
        v-for="item in knowledgeList"
        :key="item.Id"
        class="knowledge-card"
        @click="goToDetail(item.Id)"
      >
        <div class="card-cover" :style="{ backgroundImage: `url(${item.Cover || defaultCover})` }">
          <!-- 权限标签：左上角 -->
          <span v-if="item.IsPublic === true" class="tag-public">公开</span>
          <span v-else class="tag-private">私有</span>
          <!-- 状态标签：右下角 -->
          <span class="status-tag" :class="statusClass(item.Status)">
            {{ statusText(item.Status) }}
          </span>
        </div>
        <div class="card-content">
          <h3 class="card-title">{{ item.Name }}</h3>
          <p class="card-desc">{{ item.Description || '暂无描述信息' }}</p>
          <div class="card-footer">
            <span class="permission-badge">{{ item.PermissionTypeName }}</span>
            <span class="doc-count">{{ item.DocCount }} 篇文档</span>
          </div>
        </div>
        <div class="card-actions" @click.stop>
          <el-button text type="primary" size="small" @click="handleEdit(item)">编辑</el-button>
          <!-- 草稿、审核失败 显示发布按钮 -->
          <el-button
            v-if="[0,3].includes(item.Status)"
            text
            type="success"
            size="small"
            @click="handlePublish(item)"
          >
            {{ item.Status === 0 ? '发布' : '重新提交' }}
          </el-button>
          <el-button text type="danger" size="small" @click="handleDelete(item)">删除</el-button>
        </div>
      </div>

      <!-- 空数据 -->
      <div v-if="knowledgeList.length === 0" class="empty-state">
        <i class="el-icon-folder-opened empty-icon"></i>
        <p class="empty-text">暂无知识库数据</p>
        <p class="empty-tip">还没有创建任何知识库，立即开始搭建你的第一个知识库吧</p>
        <el-button type="primary" icon="el-icon-plus" @click="handleAdd" style="margin-top:16px">
          创建知识库
        </el-button>
      </div>
    </div>

    <!-- 分页 -->
    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="queryParams.pageNum"
      :limit.sync="queryParams.pageSize"
      @pagination="getList"
      class="pagination-wrapper"
    />

    <!-- 新增/编辑弹窗 -->
    <el-dialog
      :title="dialogTitle"
      :visible.sync="dialogVisible"
      width="520px"
      top="3vh"
      append-to-body
      :close-on-click-modal="false"
    >
      <el-form ref="form" :model="form" :rules="rules" label-width="88px">
        <el-form-item label="封面图标" prop="Cover">
          <div class="cover-selector">
            <div class="cover-preview">
              <img :src="form.Cover || defaultCover" alt="文库封面" />
            </div>
            <div class="cover-tip">
              <image-upload v-model="form.Cover" :limit="1"></image-upload>
              <span class="tip-text">建议正方形图片，自动裁剪适配卡片</span>
            </div>
          </div>
        </el-form-item>

        <el-form-item label="文库名称" prop="Name">
          <el-input v-model="form.Name" placeholder="请输入文库名称" maxlength="50" show-word-limit />
        </el-form-item>

        <el-form-item label="文库描述" prop="Description">
          <el-input
            v-model="form.Description"
            type="textarea"
            placeholder="简单描述知识库用途、业务范围等"
            :rows="4"
            maxlength="200"
            show-word-limit
          />
        </el-form-item>

        <el-form-item label="文库权限" prop="IsPublic">
          <el-radio-group v-model="form.IsPublic">
            <el-radio :label="false">私有</el-radio>
            <el-radio :label="true">公开</el-radio>
          </el-radio-group>
        </el-form-item>

      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确认保存</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  listKnowledge,
  getKnowledge,
  addKnowledge,
  updateKnowledge,
  deleteKnowledge,
  enableKnowledge,
  disableKnowledge
} from '@/api/llm/knowledge'

export default {
  name: 'KnowledgeBase',
  data() {
    return {
      loading: false,
      total: 0,
      knowledgeList: [],
      searchKeyword: '',
      dialogVisible: false,
      dialogTitle: '',
      form: {
        Id: undefined,
        Cover: '',
        Name: '',
        Description: '',
        IsPublic: false
      },
      queryParams: {
        pageNum: 1,
        pageSize: 12,
        Name: undefined
      },
      defaultCover: '/knowicon.png',
      rules: {
        Name: [
          { required: true, message: '文库名称不能为空', trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    // 状态文本映射
    statusText(status) {
      const map = {
        0: '草稿',
        1: '已发布',
        2: '审核中',
        3: '审核失败'
      }
      return map[status] || '未知'
    },
    // 状态样式class
    statusClass(status) {
      const map = {
        0: 'status-draft',
        1: 'status-publish',
        2: 'status-auditing',
        3: 'status-reject'
      }
      return map[status] || ''
    },
    getList() {
      this.loading = true
      listKnowledge(this.queryParams).then(response => {
        this.knowledgeList = response.data.List
        this.total = response.data.Total
        this.loading = false
      })
    },
    handleSearch() {
      this.queryParams.Name = this.searchKeyword || undefined
      this.queryParams.pageNum = 1
      this.getList()
    },
    resetSearch() {
      this.searchKeyword = ''
      this.queryParams.Name = undefined
      this.queryParams.pageNum = 1
      this.getList()
    },
    goToDetail(id) {
      this.$router.push(`/report/klg/detail/${id}`)
    },
    handleAdd() {
      this.form = {
        Id: undefined,
        Cover: '',
        Name: '',
        Description: '',
        IsPublic: false
      }
      this.dialogTitle = '新建知识库'
      this.dialogVisible = true
      this.$nextTick(() => this.$refs.form?.clearValidate())
    },
    handleEdit(item) {
      this.form = {
        Id: item.Id,
        Cover: item.Cover,
        Name: item.Name,
        Description: item.Description,
        IsPublic: item.IsPublic
      }
      this.dialogTitle = '编辑知识库'
      this.dialogVisible = true
      this.$nextTick(() => this.$refs.form?.clearValidate())
    },
    handleDelete(item) {
      this.$modal.confirm(`确认要删除知识库「${item.Name}」？删除后内部文档数据将无法恢复！`)
        .then(() => deleteKnowledge(item.Id))
        .then(() => {
          this.getList()
          this.$modal.msgSuccess('删除成功')
        })
        .catch(() => {})
    },
    cancel() {
      this.dialogVisible = false
      this.$refs['form'].resetFields()
    },
    submitForm() {
      this.$refs['form'].validate(valid => {
        if (!valid) return
        if (this.form.Id) {
          updateKnowledge(this.form).then(() => {
            this.$modal.msgSuccess('修改成功')
            this.dialogVisible = false
            this.getList()
          })
        } else {
          addKnowledge(this.form).then(() => {
            this.$modal.msgSuccess('新增成功')
            this.dialogVisible = false
            this.getList()
          })
        }
      })
    }
  }
}
</script>

<style scoped>
.knowledge-base-container {
  padding: 24px;
  background-color: #f7f8fa;
  min-height: calc(100vh - 60px);
}

/* 顶部栏 */
.header-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 28px;
}
.header-left {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.page-title {
  margin: 0;
  color: #1d2129;
  font-size: 22px;
  font-weight: 600;
}
.page-desc {
  font-size: 13px;
  color: #86909c;
}
.header-right {
  display: flex;
  gap: 12px;
}

/* 搜索区域 */
.search-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 28px;
  padding: 16px 20px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
}
.search-input {
  width: 360px;
}

/* 卡片网格 */
.knowledge-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(282px, 1fr));
  gap: 24px;
  margin-bottom: 28px;
}

/* 卡片样式 */
.knowledge-card {
  background: #ffffff;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 1px 8px rgba(0, 0, 0, 0.06);
  cursor: pointer;
  transition: all 0.25s ease;
  border: 1px solid transparent;
}
.knowledge-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.1);
  border-color: #e5ebfa;
}

.card-cover {
  height: 132px;
  background-size: cover;
  background-position: center;
  position: relative;
}

.tag-public {
  position: absolute;
  top: 0px;
  left: 0px;
  background: linear-gradient(90deg, #d926e8, #8033e8);
  color: #fff;
  font-size: 12px;
  padding: 4px 10px;
  border-radius: 12px 4px 12px 4px;
  font-weight: 500;
}
.tag-private {
  position: absolute;
  top: 0px;
  left: 0px;
  background: linear-gradient(90deg, #7488a2, #4e5e72);
  color: #fff;
  font-size: 12px;
  padding: 4px 10px;
  border-radius: 12px 4px 12px 4px;
  font-weight: 500;
}

.status-tag {
  position: absolute;
  bottom: 10px;
  right: 10px;
  font-size: 12px;
  padding: 3px 10px;
  border-radius: 20px;
  color: #fff;
  box-shadow: 0 2px 6px rgba(0,0,0,0.12);
}
.status-draft {
  background-color: #909399;
}
.status-auditing {
  background-color: #e6a23c;
}
.status-publish {
  background-color: #67c23a;
}
.status-reject {
  background-color: #f56c6c;
}


.card-content {
  padding: 18px;
}
.card-title {
  margin: 0 0 8px;
  font-size: 16px;
  font-weight: 600;
  color: #1d2129;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.card-desc {
  margin: 0 0 14px;
  font-size: 13px;
  color: #86909c;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  min-height: 39px;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.permission-badge {
  font-size: 12px;
  padding: 2px 9px;
  border-radius: 12px;
  background-color: #f0f9eb;
  color: #67c23a;
}
.doc-count {
  font-size: 12px;
  color: #86909c;
}

.card-actions {
  padding: 14px 18px 18px;
  display: flex;
  gap: 8px;
  border-top: 1px solid #f2f3f5;
  margin-top: 8px;
  flex-wrap: wrap;
}
/* 空状态 */
.empty-state {
  grid-column: 1 / -1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  background: #fff;
  border-radius: 12px;
}
.empty-icon {
  font-size: 56px;
  color: #c0c4cc;
}
.empty-text {
  margin: 12px 0 4px;
  font-size: 16px;
  color: #606266;
}
.empty-tip {
  font-size: 13px;
  color: #909399;
}

/* 分页 */
.pagination-wrapper {
  display: flex;
  justify-content: flex-end;
  padding-top: 10px;
}

/* 弹窗封面选择 */
.cover-selector {
  display: flex;
  align-items: center;
  gap: 18px;
}
.cover-preview {
  width: 104px;
  height: 104px;
  border-radius: 10px;
  overflow: hidden;
  border: 1px solid #dcdfe6;
}
.cover-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.cover-tip {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.tip-text {
  font-size: 12px;
  color: #909399;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 14px;
}
</style>