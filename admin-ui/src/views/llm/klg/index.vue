<template>
  <div class="knowledge-base-container">
    <div class="header-bar">
      <div class="header-left">
        <h2 class="page-title">知识库管理</h2>
      </div>
      <div class="header-right">
        <el-button type="primary" icon="el-icon-plus" @click="handleAdd">
          新建
        </el-button>
        <el-button icon="el-icon-upload">
          导入
        </el-button>
      </div>
    </div>

    <div class="search-container">
      <el-input
        v-model="searchKeyword"
        placeholder="请输入关键词"
        class="search-input"
        clearable
        @keyup.enter.native="handleSearch"
      >
        <el-button slot="append" icon="el-icon-search" @click="handleSearch"></el-button>
      </el-input>
    </div>

    <div class="knowledge-grid">
      <div
        v-for="item in knowledgeList"
        :key="item.Id"
        class="knowledge-card"
        @click="goToDetail(item.Id)"
      >
        <div class="card-cover" :style="{ backgroundImage: `url(${item.Cover || defaultCover})` }">
          <span v-if="item.PermissionType === 3" class="public-tag">公开</span>
        </div>
        <div class="card-content">
          <h3 class="card-title">{{ item.Name }}</h3>
          <p class="card-desc">{{ item.Description }}</p>
          <div class="card-footer">
            <span class="permission-type">{{ item.PermissionTypeName }}</span>
            <span class="doc-count">{{ item.DocCount }} 篇文档</span>
          </div>
        </div>
        <div class="card-actions">
          <el-button type="text" size="small" @click.stop="handleEdit(item)">编辑</el-button>
          <el-button type="text" size="small" @click.stop="handleDelete(item)">删除</el-button>
        </div>
      </div>

      <div v-if="knowledgeList.length === 0" class="empty-state">
        <i class="el-icon-folder-opened" style="font-size: 48px; color: #ccc;"></i>
        <p style="margin-top: 16px; color: #999;">暂无知识库，点击新建创建第一个知识库</p>
      </div>
    </div>

    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="queryParams.pageNum"
      :limit.sync="queryParams.pageSize"
      @pagination="getList"
    />

    <el-dialog
      :title="dialogTitle"
      :visible.sync="dialogVisible"
      width="500px"
      append-to-body
    >
      <el-form ref="form" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="封面" prop="Cover">
          <div class="cover-selector">
            <div class="cover-preview">
              <img :src="form.Cover || defaultCover" alt="封面" />
            </div>
            <div class="cover-options">
              <div class="cover-grid">
                <div
                  v-for="(cover, index) in coverOptions"
                  :key="index"
                  class="cover-item"
                  :class="{ active: form.Cover === cover }"
                  @click="form.Cover = cover"
                >
                  <img :src="cover" alt="" />
                </div>
              </div>
              <el-button type="text" size="small" @click="uploadCover">上传封面</el-button>
            </div>
          </div>
        </el-form-item>

        <el-form-item label="文库名称" prop="Name">
          <el-input v-model="form.Name" placeholder="请输入文库名称" maxlength="50" />
        </el-form-item>

        <el-form-item label="文库描述" prop="Description">
          <el-input
            v-model="form.Description"
            type="textarea"
            placeholder="请输入文库描述"
            :rows="3"
            maxlength="200"
          />
        </el-form-item>

        <el-form-item label="文库权限">
          <el-radio-group v-model="form.PermissionType">
            <el-radio label="1">私有文库(查看、修改管理需要相应授权)</el-radio>
            <el-radio label="2">企业级文库(企业员工可查看,修改管理需要相应授权)</el-radio>
            <el-radio label="3">全网级文库(无需登录即可查看,修改管理需要相应授权)</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
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
  deleteKnowledge
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
        PermissionType: 1
      },
      queryParams: {
        pageNum: 1,
        pageSize: 12,
        Name: undefined
      },
      coverOptions: [
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=business%20knowledge%20library%20cover%20abstract%20blue&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=modern%20office%20workspace%20professional&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=tech%20network%20data%20connection&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=books%20library%20education%20study&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=nature%20landscape%20peaceful%20mountains&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=city%20skyline%20night%20modern&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=abstract%20geometric%20shapes%20colorful&image_size=square',
        'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=coffee%20shop%20cozy%20workspace&image_size=square'
      ],
      defaultCover: 'https://neeko-copilot.bytedance.net/api/text_to_image?prompt=business%20knowledge%20library%20cover%20abstract%20blue&image_size=square',
      rules: {
        Name: [
          { required: true, message: '文库名称不能为空', trigger: 'blur' }
        ],
        PermissionType: [
          { required: true, message: '请选择权限类型', trigger: 'change' }
        ]
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
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
    goToDetail(id) {
      this.$router.push(`/llm/klg/detail/${id}`)
    },
    handleAdd() {
      this.form = {
        Id: undefined,
        Cover: '',
        Name: '',
        Description: '',
        PermissionType: 1
      }
      this.dialogTitle = '新建文库'
      this.dialogVisible = true
    },
    handleEdit(item) {
      this.form = {
        Id: item.Id,
        Cover: item.Cover,
        Name: item.Name,
        Description: item.Description,
        PermissionType: item.PermissionType
      }
      this.dialogTitle = '编辑文库'
      this.dialogVisible = true
    },
    handleDelete(item) {
      this.$modal.confirm(`是否确认删除文库"${item.Name}"？`).then(() => {
        return deleteKnowledge(item.Id)
      }).then(() => {
        this.getList()
        this.$modal.msgSuccess('删除成功')
      }).catch(() => {})
    },
    cancel() {
      this.dialogVisible = false
      this.$refs['form'].resetFields()
    },
    submitForm() {
      this.$refs['form'].validate(valid => {
        if (valid) {
          if (this.form.Id !== undefined) {
            updateKnowledge(this.form).then(response => {
              this.$modal.msgSuccess('修改成功')
              this.dialogVisible = false
              this.getList()
            })
          } else {
            addKnowledge(this.form).then(response => {
              this.$modal.msgSuccess('新增成功')
              this.dialogVisible = false
              this.getList()
            })
          }
        }
      })
    },
    uploadCover() {
      this.$message.info('上传功能开发中')
    }
  }
}
</script>

<style scoped>
.knowledge-base-container {
  padding: 20px;
}

.header-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  margin: 0;
  color: #303133;
  font-size: 18px;
  font-weight: 600;
}

.header-right {
  display: flex;
  gap: 12px;
}

.search-container {
  margin-bottom: 24px;
}

.search-input {
  width: 300px;
}

.knowledge-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.knowledge-card {
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  cursor: pointer;
  transition: all 0.3s ease;
}

.knowledge-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.card-cover {
  height: 120px;
  background-size: cover;
  background-position: center;
  position: relative;
}

.public-tag {
  position: absolute;
  top: 8px;
  right: 8px;
  background: #1890ff;
  color: #fff;
  font-size: 12px;
  padding: 2px 8px;
  border-radius: 4px;
}

.card-content {
  padding: 16px;
}

.card-title {
  margin: 0 0 8px 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.card-desc {
  margin: 0 0 12px 0;
  font-size: 14px;
  color: #909399;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.permission-type {
  font-size: 12px;
  color: #67c23a;
  background: #f0f9eb;
  padding: 2px 8px;
  border-radius: 4px;
}

.doc-count {
  font-size: 12px;
  color: #909399;
}

.card-actions {
  padding: 0 16px 16px;
  display: flex;
  gap: 16px;
}

.card-actions .el-button {
  font-size: 12px;
  padding: 0;
}

.empty-state {
  grid-column: 1 / -1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 0;
  color: #999;
}

.cover-selector {
  display: flex;
  gap: 16px;
}

.cover-preview {
  width: 100px;
  height: 100px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e4e7ed;
}

.cover-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.cover-options {
  flex: 1;
}

.cover-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 8px;
  margin-bottom: 8px;
}

.cover-item {
  width: 50px;
  height: 50px;
  border-radius: 4px;
  overflow: hidden;
  border: 2px solid transparent;
  cursor: pointer;
}

.cover-item.active {
  border-color: #1890ff;
}

.cover-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>