<template>
    <div class="app-container">
        <!-- 建模库列表视图 -->
        <div v-if="currentView === 'libraryList'">
            <!-- 页面标题和操作栏 -->
            <div class="page-header">
                <div class="title-and-desc">
                    <h2 class="page-title">人脸建模库管理</h2>
                    <p class="page-desc">管理所有的人脸建模库及其包含的人脸数据</p>
                </div>
                <div class="page-actions">
                    <el-button type="primary" icon="el-icon-plus" @click="showAddLibraryDialog = true">
                        新建建模库
                    </el-button>
                </div>
            </div>

            <!-- 搜索和筛选区域 -->
            <div class="filter-container">
                <el-row :gutter="20">
                    <el-col :span="8">
                        <el-input placeholder="请输入建模库名称" v-model="librarySearchQuery" prefix-icon="el-icon-search"
                            clearable class="search-input"></el-input>
                    </el-col>
                    <el-col :span="6">
                        <el-select placeholder="请选择状态" v-model="libraryStatusFilter" clearable class="search-select">
                            <el-option label="启用" value="1"></el-option>
                            <el-option label="禁用" value="0"></el-option>
                        </el-select>
                    </el-col>
                    <el-col :span="4">
                        <el-button type="primary" @click="searchLibraries" class="search-btn">
                            <i class="el-icon-search"></i> 搜索
                        </el-button>
                    </el-col>
                    <el-col :span="4">
                        <el-button type="default" @click="resetLibraryFilters" class="reset-btn">
                            <i class="el-icon-refresh"></i> 重置
                        </el-button>
                    </el-col>
                </el-row>
            </div>

            <!-- 建模库列表 -->
            <div class="main-content" v-loading="libloading">
                <!-- 空状态 -->
                <div v-if="filteredLibraries.length === 0" class="empty-state">
                    <el-empty description="暂无建模库数据">
                        <el-button type="primary" size="small" @click="showAddLibraryDialog = true">
                            <i class="el-icon-plus"></i> 新建建模库
                        </el-button>
                    </el-empty>
                </div>

                <!-- 方格布局 -->
                <div v-else class="library-grid">
                    <el-card v-for="library in filteredLibraries" :key="library.Id" class="library-card" shadow="hover">
                        <div slot="header" class="library-card-header">
                            <div class="library-name">
                                <i class="el-icon-folder-opened text-primary"></i>
                                <span>{{ library.HouseName }}</span>
                            </div>
                            <el-dropdown trigger="click">
                                <i class="el-icon-more text-gray-400"></i>
                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item @click="enterFaceManagement(library)">
                                        <i class="el-icon-picture"></i> 管理人脸
                                    </el-dropdown-item>
                                    <el-dropdown-item @click="showEditLibraryDialog(library)">
                                        <i class="el-icon-edit"></i> 编辑
                                    </el-dropdown-item>
                                    <el-dropdown-item divided @click="deleteLibrary(library.id)">
                                        <i class="el-icon-delete text-danger"></i> 删除
                                    </el-dropdown-item>
                                </el-dropdown-menu>
                            </el-dropdown>
                        </div>

                        <div class="library-card-body">
                            <p class="library-desc">{{ library.description || '无描述信息' }}</p>

                            <div class="library-meta">
                                <div class="meta-item">
                                    <i class="el-icon-user"></i>
                                    <span>人脸数量: {{ library.faceCount }}</span>
                                </div>
                                <div class="meta-item">
                                    <i class="el-icon-time"></i>
                                    <span>{{ formatDate(library.createdAt) }}</span>
                                </div>
                            </div>

                            <div class="library-actions">
                                <el-tag :type="library.Status === '1' ? 'success' : 'danger'">
                                    {{ library.Status === '1' ? '启用' : '禁用' }}
                                </el-tag>
                                <el-button type="text" @click="enterFaceManagement(library)" class="enter-btn">
                                    进入管理 <i class="el-icon-arrow-right"></i>
                                </el-button>
                            </div>
                        </div>
                    </el-card>
                </div>

            </div>
        </div>

        <!-- 人脸建模管理视图 -->
        <div v-if="currentView === 'faceManagement'">
            <!-- 页面标题和操作栏 -->
            <div class="page-header">
                <div class="title-and-desc">
                    <h2 class="page-title">{{ currentLibrary.name }} - 人脸建模管理</h2>
                    <p class="page-desc">共 {{ currentFaces.length }} 个人脸建模</p>
                </div>
                <div class="page-actions">
                    <el-button type="default" icon="el-icon-back" @click="backToLibraryList">
                        返回列表
                    </el-button>
                    <el-button type="primary" icon="el-icon-upload" @click="showUploadDialog = true">
                        上传人脸建模
                    </el-button>
                </div>
            </div>

            <!-- 搜索和批量操作 -->
            <div class="filter-container">
                <el-row :gutter="20">
                    <el-col :span="10">
                        <el-input placeholder="请输入人脸名称或ID" v-model="faceSearchQuery" prefix-icon="el-icon-search"
                            clearable class="search-input"></el-input>
                    </el-col>
                    <el-col :span="4">
                        <el-button type="primary" @click="searchFaces" class="search-btn">
                            <i class="el-icon-search"></i> 搜索
                        </el-button>
                    </el-col>
                    <el-col :span="4">
                        <el-button type="default" @click="resetFaceFilters" class="reset-btn">
                            <i class="el-icon-refresh"></i> 重置
                        </el-button>
                    </el-col>
                    <el-col :span="6">
                        <el-button type="warning" @click="batchDeleteFaces" :disabled="selectedFaceIds.length === 0"
                            class="batch-delete-btn">
                            <i class="el-icon-delete"></i> 批量删除 ({{ selectedFaceIds.length }})
                        </el-button>
                    </el-col>
                </el-row>
            </div>

            <!-- 人脸列表 -->
            <div class="main-content">
                <div class="face-grid">
                    <el-card v-for="face in filteredFaces" :key="face.id" class="face-card" shadow="hover">
                        <div class="face-image-container">
                            <img :src="face.imageUrl" alt="人脸建模" class="face-image">
                            <el-checkbox v-model="selectedFaceIds" :label="face.id" class="face-checkbox"></el-checkbox>
                            <div class="face-info-overlay">
                                <p class="face-name">{{ face.name }}</p>
                                <p class="face-id">{{ face.id }}</p>
                            </div>
                        </div>

                        <div class="face-card-footer">
                            <span class="upload-time">{{ formatDate(face.uploadTime) }}</span>
                            <el-dropdown trigger="click">
                                <i class="el-icon-more text-gray-400"></i>
                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item @click="showFaceDetail(face)">
                                        <i class="el-icon-view"></i> 查看详情
                                    </el-dropdown-item>
                                    <el-dropdown-item @click="editFace(face)">
                                        <i class="el-icon-edit"></i> 编辑信息
                                    </el-dropdown-item>
                                    <el-dropdown-item divided @click="deleteFace(face.id)">
                                        <i class="el-icon-delete text-danger"></i> 删除
                                    </el-dropdown-item>
                                </el-dropdown-menu>
                            </el-dropdown>
                        </div>
                    </el-card>
                </div>

                <!-- 空状态 -->
                <div v-if="filteredFaces.length === 0" class="empty-state">
                    <el-empty description="暂无人脸建模数据">
                        <el-button type="primary" size="small" @click="showUploadDialog = true">
                            <i class="el-icon-upload"></i> 上传人脸建模
                        </el-button>
                    </el-empty>
                </div>

                <!-- 分页 -->
                <div v-if="filteredFaces.length > 0" class="pagination-container">
                    <el-pagination @size-change="handleFaceSizeChange" @current-change="handleFaceCurrentChange"
                        :current-page="currentFacePage" :page-sizes="[12, 24, 36, 48]" :page-size="facePageSize"
                        layout="total, sizes, prev, pager, next, jumper" :total="filteredFaces.length"></el-pagination>
                </div>
            </div>
        </div>

        <!-- 添加/编辑建模库对话框 -->
        <el-dialog :title="isEditingLibrary ? '编辑建模库' : '新建建模库'" :visible.sync="showAddLibraryDialog" width="500px"
            :close-on-click-modal="false" border>
            <el-form :model="currentLibraryForm" :rules="libraryRules" ref="libraryForm" label-width="100px">
                <el-form-item label="建模库名称" prop="HouseName">
                    <el-input v-model="currentLibraryForm.HouseName" placeholder="请输入建模库名称"></el-input>
                </el-form-item>
                <el-form-item label="描述" prop="Remark">
                    <el-input v-model="currentLibraryForm.Remark" placeholder="请输入建模库描述" type="textarea"
                        rows="3"></el-input>
                </el-form-item>
                <el-form-item label="状态" prop="Status">
                    <el-radio-group v-model="currentLibraryForm.Status">
                        <el-radio label="1">启用</el-radio>
                        <el-radio label="0">禁用</el-radio>
                    </el-radio-group>
                </el-form-item>
            </el-form>
            <div slot="footer">
                <el-button @click="showAddLibraryDialog = false">取消</el-button>
                <el-button type="primary" @click="saveLibrary">
                    保存
                </el-button>
            </div>
        </el-dialog>

        <!-- 上传人脸建模对话框 -->
        <el-dialog title="上传人脸建模" :visible.sync="showUploadDialog" width="600px" :close-on-click-modal="false" border>
            <el-upload class="upload-demo" drag action="" :on-change="handleFileChange" :before-upload="beforeUpload"
                :file-list="uploadFileList" multiple :auto-upload="false">
                <i class="el-icon-upload text-4xl"></i>
                <div class="el-upload__text">
                    拖放文件到此处，或<em>点击上传</em>
                </div>
                <div class="el-upload__tip">
                    支持 JPG、PNG 格式，单文件不超过 5MB，最多上传 10 个文件
                </div>
            </el-upload>

            <!-- 上传文件列表 -->
            <div v-if="uploadFileList.length > 0" class="upload-file-list">
                <div v-for="(file, index) in uploadFileList" :key="file.uid" class="file-item">
                    <i class="el-icon-picture text-primary"></i>
                    <div class="file-info">
                        <p class="file-name">{{ file.name }}</p>
                        <el-progress :percentage="file.percentage || 0" stroke-width="2"></el-progress>
                    </div>
                    <el-button type="text" size="small" class="file-remove" @click="removeUploadFile(index)">
                        <i class="el-icon-delete text-danger"></i>
                    </el-button>
                </div>
            </div>

            <div slot="footer">
                <el-button @click="cancelUpload">取消</el-button>
                <el-button type="primary" @click="confirmUpload" :disabled="uploadFileList.length === 0 || isUploading">
                    <i class="el-icon-loading" v-if="isUploading"></i>
                    {{ isUploading ? '上传中...' : '开始上传' }}
                </el-button>
            </div>
        </el-dialog>

        <!-- 人脸详情对话框 -->
        <el-dialog title="人脸详情" :visible.sync="showFaceDetailDialog" width="500px" :close-on-click-modal="false" border>
            <div v-if="currentFaceDetail" class="face-detail">
                <div class="face-detail-image">
                    <img :src="currentFaceDetail.imageUrl" alt="人脸建模">
                </div>
                <el-descriptions column="1" border class="face-detail-info">
                    <el-descriptions-item label="人脸ID">{{ currentFaceDetail.id }}</el-descriptions-item>
                    <el-descriptions-item label="名称">{{ currentFaceDetail.name }}</el-descriptions-item>
                    <el-descriptions-item label="上传时间">{{ formatDate(currentFaceDetail.uploadTime)
                    }}</el-descriptions-item>
                    <el-descriptions-item label="文件大小">{{ formatFileSize(currentFaceDetail.fileSize)
                    }}</el-descriptions-item>
                    <el-descriptions-item label="文件格式">{{ currentFaceDetail.fileType }}</el-descriptions-item>
                    <el-descriptions-item label="备注">{{ currentFaceDetail.remark || '无' }}</el-descriptions-item>
                </el-descriptions>
            </div>
            <div slot="footer">
                <el-button @click="showFaceDetailDialog = false">关闭</el-button>
            </div>
        </el-dialog>

        <!-- 编辑人脸信息对话框 -->
        <el-dialog title="编辑人脸信息" :visible.sync="showEditFaceDialog" width="400px" :close-on-click-modal="false" border>
            <el-form :model="currentFaceForm" :rules="faceRules" ref="faceForm" label-width="80px">
                <el-form-item label="名称" prop="name">
                    <el-input v-model="currentFaceForm.name" placeholder="请输入人脸名称"></el-input>
                </el-form-item>
                <el-form-item label="备注">
                    <el-input v-model="currentFaceForm.remark" placeholder="请输入备注信息" type="textarea"
                        rows="3"></el-input>
                </el-form-item>
            </el-form>
            <div slot="footer">
                <el-button @click="showEditFaceDialog = false">取消</el-button>
                <el-button type="primary" @click="saveFaceInfo">
                    保存
                </el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
import {
    getFaceHouseList,addFaceHouse
} from "@/api/ai/face";

export default {
    data() {
        return {
            currentView: 'libraryList',
            currentLibrary: null,
            currentFaces: [],
            faces: {},
            // 筛选和分页
            librarySearchQuery: '',
            libraryStatusFilter: '',
            filteredLibraries: [],
            libloading: false,

            faceSearchQuery: '',
            filteredFaces: [],
            currentFacePage: 1,
            facePageSize: 12,
            selectedFaceIds: [],

            // 对话框状态
            showAddLibraryDialog: false,
            isEditingLibrary: false,
            currentLibraryForm: {
                HouseName: '',
                Status: '1',
                Remark:""
            },

            showUploadDialog: false,
            uploadFileList: [],
            isUploading: false,

            showFaceDetailDialog: false,
            currentFaceDetail: null,

            showEditFaceDialog: false,
            currentFaceForm: {
                id: '',
                name: '',
                remark: ''
            },

            // 表单验证规则
            libraryRules: {
                HouseName: [
                    { required: true, message: '请输入建模库名称', trigger: 'blur' },
                    { min: 2, max: 50, message: '名称长度在 2 到 50 个字符', trigger: 'blur' }
                ],
                description: [
                    { max: 200, message: '描述不能超过 200 个字符', trigger: 'blur' }
                ]
            },

            faceRules: {
                name: [
                    { required: true, message: '请输入人脸名称', trigger: 'blur' },
                    { min: 1, max: 50, message: '名称长度在 1 到 50 个字符', trigger: 'blur' }
                ]
            }
        };
    },
    async created() {
        await searchLibraries();
    },
    methods: {
        // 格式化日期
        formatDate(dateString) {
            if (!dateString) return '';
            const date = new Date(dateString);
            return date.toLocaleString('zh-CN', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit'
            }).replace(',', '');
        },

        // 格式化文件大小
        formatFileSize(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        },

        // 建模库搜索和筛选
        async searchLibraries() {
            let res = await getFaceHouseList({ "Key": this.librarySearchQuery, "Status": this.libraryStatusFilter });
            this.filteredLibraries = res.data;
        },

        resetLibraryFilters() {
            this.librarySearchQuery = '';
            this.libraryStatusFilter = '';
            this.searchLibraries();
        },

        // 显示编辑建模库对话框
        showEditLibraryDialog(library) {
            this.isEditingLibrary = true;
            this.currentLibraryForm = { ...library };
            this.showAddLibraryDialog = true;
        },

        // 保存建模库
        saveLibrary() {
            this.$refs.libraryForm.validate(async valid => {
                if (valid) {
                    if (this.isEditingLibrary) {
                        // 更新现有建模库

                    } else {
                        // 添加新建模库
                        await addFaceHouse(this.currentLibraryForm);
                    }

                    this.searchLibraries();
                    this.showAddLibraryDialog = false;
                    this.$message.success(this.isEditingLibrary ? '建模库更新成功' : '建模库创建成功');
                }
            });
        },

        // 删除建模库
        deleteLibrary(id) {
            this.$confirm('确定要删除这个建模库吗？删除后所有关联的人脸数据也将被删除。', '确认删除', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                const index = this.libraries.findIndex(lib => lib.id === id);
                if (index !== -1) {
                    this.libraries.splice(index, 1);
                    delete this.faces[id];
                    if (this.currentLibrary && this.currentLibrary.id === id) {
                        this.currentView = 'libraryList';
                        this.currentLibrary = null;
                    }
                    this.searchLibraries();
                    this.$message.success('建模库已删除');
                }
            }).catch(() => { });
        },

        // 进入人脸管理
        enterFaceManagement(library) {
            this.currentLibrary = { ...library };
            this.currentView = 'faceManagement';
            this.loadFaces();
        },

        // 返回建模库列表
        backToLibraryList() {
            this.currentView = 'libraryList';
            this.selectedFaceIds = [];
        },

        // 加载人脸数据
        loadFaces() {
            if (!this.currentLibrary) return;
            this.currentFacePage = 1;
            this.faceSearchQuery = '';
            this.filteredFaces = this.faces[this.currentLibrary.id] || [];
        },

        // 人脸搜索
        searchFaces() {
            this.currentFacePage = 1;
            if (!this.currentLibrary) return;
            const allFaces = this.faces[this.currentLibrary.id] || [];
            this.filteredFaces = allFaces.filter(face => {
                return face.name.toLowerCase().includes(this.faceSearchQuery.toLowerCase()) ||
                    face.id.toLowerCase().includes(this.faceSearchQuery.toLowerCase());
            });
        },

        resetFaceFilters() {
            this.faceSearchQuery = '';
            this.currentFacePage = 1;
            this.filteredFaces = this.faces[this.currentLibrary.id] || [];
        },

        // 人脸分页处理
        handleFaceSizeChange(size) {
            this.facePageSize = size;
            this.currentFacePage = 1;
        },

        handleFaceCurrentChange(page) {
            this.currentFacePage = page;
        },

        // 文件上传处理
        handleFileChange(file, fileList) {
            this.uploadFileList = fileList;
        },

        beforeUpload(file) {
            const isImage = file.type === 'image/jpeg' || file.type === 'image/png';
            const isLt5M = file.size / 1024 / 1024 < 5;

            if (!isImage) {
                this.$message.error('只能上传 JPG/PNG 格式的图片');
                return false;
            }
            if (!isLt5M) {
                this.$message.error('图片大小不能超过 5MB');
                return false;
            }

            return true;
        },

        removeUploadFile(index) {
            this.uploadFileList.splice(index, 1);
        },

        cancelUpload() {
            this.uploadFileList = [];
            this.isUploading = false;
            this.showUploadDialog = false;
        },

        confirmUpload() {
            if (!this.currentLibrary) {
                this.$message.error('请先选择一个建模库');
                return;
            }

            this.isUploading = true;

            // 模拟上传进度
            let completed = 0;
            const totalFiles = this.uploadFileList.length;
            const interval = setInterval(() => {
                this.uploadFileList.forEach(file => {
                    if (file.percentage < 100) {
                        file.percentage = Math.min(100, (file.percentage || 0) + 10);
                    } else {
                        completed++;
                    }
                });

                if (completed === totalFiles) {
                    clearInterval(interval);
                    this.isUploading = false;

                    // 添加新上传的人脸到库中
                    this.uploadFileList.forEach(file => {
                        const faceId = 'face-' + Date.now() + '-' + Math.floor(Math.random() * 1000);
                        const fileName = file.name.split('.').slice(0, -1).join('.');
                        const fileType = file.name.split('.').pop().toLowerCase();
                        const imageUrl = URL.createObjectURL(file.raw);

                        const newFace = {
                            id: faceId,
                            name: fileName,
                            imageUrl: imageUrl,
                            uploadTime: new Date().toLocaleString('zh-CN', {
                                year: 'numeric',
                                month: '2-digit',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit',
                                second: '2-digit'
                            }).replace(',', ' '),
                            fileSize: file.size,
                            fileType: fileType,
                            remark: ''
                        };

                        if (!this.faces[this.currentLibrary.id]) {
                            this.faces[this.currentLibrary.id] = [];
                        }
                        this.faces[this.currentLibrary.id].unshift(newFace);

                        // 更新建模库的人脸数量
                        const libIndex = this.libraries.findIndex(lib => lib.id === this.currentLibrary.id);
                        if (libIndex !== -1) {
                            this.libraries[libIndex].faceCount++;
                        }
                        if (this.currentLibrary) {
                            this.currentLibrary.faceCount++;
                        }
                    });

                    this.loadFaces();
                    this.uploadFileList = [];
                    this.showUploadDialog = false;
                    this.$message.success(`成功上传 ${totalFiles} 个人脸建模`);
                }
            }, 300);
        },

        // 显示人脸详情
        showFaceDetail(face) {
            this.currentFaceDetail = { ...face };
            this.showFaceDetailDialog = true;
        },

        // 编辑人脸信息
        editFace(face) {
            this.currentFaceForm = {
                id: face.id,
                name: face.name,
                remark: face.remark || ''
            };
            this.showEditFaceDialog = true;
        },

        // 保存人脸信息
        saveFaceInfo() {
            this.$refs.faceForm.validate(valid => {
                if (valid && this.currentLibrary) {
                    const faceList = this.faces[this.currentLibrary.id] || [];
                    const index = faceList.findIndex(face => face.id === this.currentFaceForm.id);

                    if (index !== -1) {
                        faceList[index].name = this.currentFaceForm.name;
                        faceList[index].remark = this.currentFaceForm.remark;
                        this.filteredFaces = [...faceList];
                        this.showEditFaceDialog = false;
                        this.$message.success('人脸信息更新成功');
                    }
                }
            });
        },

        // 删除单个人脸
        deleteFace(faceId) {
            this.$confirm('确定要删除这个人脸建模吗？', '确认删除', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                if (!this.currentLibrary) return;

                const faceList = this.faces[this.currentLibrary.id] || [];
                const index = faceList.findIndex(face => face.id === faceId);

                if (index !== -1) {
                    faceList.splice(index, 1);
                    this.filteredFaces = [...faceList];

                    // 更新建模库的人脸数量
                    const libIndex = this.libraries.findIndex(lib => lib.id === this.currentLibrary.id);
                    if (libIndex !== -1) {
                        this.libraries[libIndex].faceCount--;
                    }
                    if (this.currentLibrary) {
                        this.currentLibrary.faceCount--;
                    }

                    this.selectedFaceIds = this.selectedFaceIds.filter(id => id !== faceId);
                    this.$message.success('人脸建模已删除');
                }
            }).catch(() => { });
        },

        // 批量删除人脸
        batchDeleteFaces() {
            if (this.selectedFaceIds.length === 0) return;

            this.$confirm(`确定要删除选中的 ${this.selectedFaceIds.length} 个人脸建模吗？`, '确认删除', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                if (!this.currentLibrary) return;

                let deletedCount = 0;
                const faceList = this.faces[this.currentLibrary.id] || [];

                // 过滤掉要删除的人脸
                this.faces[this.currentLibrary.id] = faceList.filter(face => {
                    const isDeleted = this.selectedFaceIds.includes(face.id);
                    if (isDeleted) deletedCount++;
                    return !isDeleted;
                });

                this.filteredFaces = [...this.faces[this.currentLibrary.id]];

                // 更新建模库的人脸数量
                const libIndex = this.libraries.findIndex(lib => lib.id === this.currentLibrary.id);
                if (libIndex !== -1) {
                    this.libraries[libIndex].faceCount -= deletedCount;
                }
                if (this.currentLibrary) {
                    this.currentLibrary.faceCount -= deletedCount;
                }

                this.selectedFaceIds = [];
                this.$message.success(`成功删除 ${deletedCount} 个人脸建模`);
            }).catch(() => { });
        }
    }
};
</script>
<style lang="scss" scoped>
.app-container {
    padding: 24px;
    background-color: #ffffff;
    min-height: 100vh;
    margin-top: 4px;
}

.page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 24px;
    padding-bottom: 16px;
    border-bottom: 1px solid #e8e8e8;
    /* 增加分割线 */
}

.title-and-desc {
    flex: 1;
}

.page-title {
    font-size: 18px;
    color: #1f2d3d;
    margin: 0 0 4px 0;
    font-weight: 500;
}

.page-desc {
    font-size: 14px;
    color: #8392a5;
    margin: 0;
}

.page-actions {
    display: flex;
    gap: 10px;
}

/* 筛选区域调整 */
.filter-container {
    padding: 16px;
    margin-bottom: 24px;
    background-color: #fafafa;
    /* 浅灰色背景突出筛选区 */
    border: 1px solid #e8e8e8;
    border-radius: 4px;
}

.search-input,
.search-select {
    width: 100%;
}

.search-btn,
.reset-btn {
    width: 100%;
}

/* 主内容区调整 */
.main-content {
    padding: 24px;
    background-color: #ffffff;
    border: 1px solid #e8e8e8;
    border-radius: 4px;
}

/* 建模库样式 */
.library-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 20px;
}

.library-card {
    transition: all 0.3s ease;
    height: 100%;
    display: flex;
    flex-direction: column;
    border: 1px solid #e8e8e8;
    /* 卡片边框 */
}

.library-card:hover {
    border-color: #1890ff;
    /* 悬停时边框变色 */
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.library-card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 14px 16px;
    border-bottom: 1px solid #f0f0f0;
}

.library-name {
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: 500;
    color: #1f2d3d;
}

.library-card-body {
    padding: 16px;
    flex: 1;
    display: flex;
    flex-direction: column;
}

.library-desc {
    color: #4e5969;
    font-size: 14px;
    line-height: 1.5;
    margin-bottom: 16px;
    flex: 1;
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: 2;
    overflow: hidden;
}

.library-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
    margin-bottom: 16px;
    font-size: 13px;
    color: #8392a5;
}

.meta-item {
    display: flex;
    align-items: center;
    gap: 4px;
}

.library-actions {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: auto;
}

.enter-btn {
    color: #1890ff;
    padding: 0;
    height: auto;
}

/* 人脸样式 */
.face-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
    gap: 20px;
}

.face-card {
    transition: all 0.3s ease;
    overflow: hidden;
    border: 1px solid #e8e8e8;
}

.face-card:hover {
    border-color: #1890ff;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.face-image-container {
    position: relative;
    height: 180px;
    overflow: hidden;
}

.face-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.3s ease;
}

.face-card:hover .face-image {
    transform: scale(1.05);
}

.face-checkbox {
    position: absolute;
    top: 8px;
    right: 8px;
    z-index: 2;
}

.face-info-overlay {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    padding: 8px 12px;
    background: linear-gradient(transparent, rgba(0, 0, 0, 0.7));
    color: #fff;
    z-index: 1;
}

.face-name {
    font-weight: 500;
    margin: 0 0 4px 0;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.face-id {
    font-size: 12px;
    opacity: 0.9;
    margin: 0;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.face-card-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px;
    border-top: 1px solid #f0f0f0;
    font-size: 13px;
    color: #8392a5;
}

/* 空状态 */
.empty-state {
    padding: 40px 0;
    text-align: center;
    background-color: #fafafa;
    border-radius: 4px;
    margin-bottom: 16px;
}

/* 分页 */
.pagination-container {
    margin-top: 24px;
    text-align: right;
}

/* 上传文件列表 */
.upload-file-list {
    margin-top: 20px;
}

.file-item {
    display: flex;
    align-items: center;
    padding: 10px;
    border: 1px solid #e8e8e8;
    border-radius: 4px;
    margin-bottom: 10px;
    background-color: #fafafa;
}

.file-info {
    flex: 1;
    margin: 0 10px;
    overflow: hidden;
}

.file-name {
    font-size: 13px;
    margin-bottom: 6px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.file-remove {
    padding: 0;
    height: auto;
}

/* 人脸详情 */
.face-detail {
    padding: 10px 0;
}

.face-detail-image {
    text-align: center;
    margin-bottom: 20px;
    padding: 16px;
    background-color: #fafafa;
    border-radius: 4px;
}

.face-detail-image img {
    max-height: 200px;
    border-radius: 4px;
    border: 1px solid #e8e8e8;
}

.face-detail-info {
    margin-top: 16px;
}

/* 批量删除按钮 */
.batch-delete-btn {
    width: 100%;
}

/* 对话框样式增强 */
::v-deep .el-dialog__body {
    padding: 20px;
}

::v-deep .el-dialog--border {
    border: 1px solid #e8e8e8;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}
</style>