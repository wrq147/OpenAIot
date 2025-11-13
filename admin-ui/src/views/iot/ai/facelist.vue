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
                            <el-dropdown trigger="click" @command="handleLibraryCommand($event, library)">
                                <i class="el-icon-more text-gray-400"></i>
                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item command="man">
                                        <i class="el-icon-picture"></i> 管理人脸
                                    </el-dropdown-item>
                                    <el-dropdown-item command="edit">
                                        <i class="el-icon-edit"></i> 编辑
                                    </el-dropdown-item>
                                    <el-dropdown-item command="del" divided>
                                        <i class="el-icon-delete text-danger"></i> 删除
                                    </el-dropdown-item>
                                </el-dropdown-menu>
                            </el-dropdown>
                        </div>

                        <div class="library-card-body">
                            <p class="library-desc">{{ library.Remark || '无描述信息' }}</p>

                            <div class="library-meta">
                                <div class="meta-item">
                                    <i class="el-icon-user"></i>
                                    <span>人脸数量: {{ library.FaceCount }}</span>
                                </div>
                                <div class="meta-item">
                                    <i class="el-icon-time"></i>
                                    <span>{{ library.CreatedOn }}</span>
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
                    <h2 class="page-title">人脸建模管理 - {{ currentLibrary.HouseName }}</h2>
                    <p class="page-desc">共 {{ currentLibrary.FaceCount }} 个人脸建模</p>
                </div>
                <div class="page-actions">
                    <el-button type="default" icon="el-icon-back" @click="backToLibraryList">
                        返回列表
                    </el-button>
                    <el-button type="primary" icon="el-icon-upload" @click="createFace">
                        创建人脸建模
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
                <div class="face-grid" v-loading="loading">
                    <el-card v-for="face in filteredFaces" :key="face.Id" class="face-card" shadow="hover">
                        <div class="face-image-container">
                            <img :src="face.FaceImg" alt="人脸建模" class="face-image">
                            <el-checkbox v-model="selectedFaceIds" :label="face.Id" class="face-checkbox"></el-checkbox>
                            <div class="face-info-overlay">
                                <p class="face-name">{{ face.MemInfo.RealName }}</p>
                                <p class="face-id">{{ face.Id }}</p>
                            </div>
                        </div>

                        <div class="face-card-footer">
                            <span class="upload-time">{{ face.CreatedOn }}</span>
                            <el-dropdown trigger="click" @command="handleFaceCommand($event, face)">
                                <i class="el-icon-more text-gray-400"></i>
                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item command="detail">
                                        <i class="el-icon-view"></i> 查看详情
                                    </el-dropdown-item>
                                    <el-dropdown-item divided command="del">
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
                        <el-button type="primary" size="small" @click="createFace">
                            <i class="el-icon-upload"></i> 创建人脸建模
                        </el-button>
                    </el-empty>
                </div>

                <!-- 分页 -->
                <div v-if="filteredFaces.length > 0" class="pagination-container">
                    <pagination v-show="total > 0" :total="total" :page.sync="currentFacePage"
                        :limit.sync="facePageSize" @pagination="loadFaces"></pagination>
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

        <!-- 人脸详情对话框 -->
        <el-dialog title="人脸详情" :visible.sync="showFaceDetailDialog" width="500px" :close-on-click-modal="false" border>
            <div v-if="currentFaceDetail" class="face-detail">
                <div class="face-detail-image">
                    <img :src="currentFaceDetail.FaceImg" alt="人脸建模">
                </div>
                <el-descriptions column="1" border class="face-detail-info">
                    <el-descriptions-item label="人脸ID">{{ currentFaceDetail.Id }}</el-descriptions-item>
                    <el-descriptions-item label="姓名">{{ currentFaceDetail.MemInfo.RealName }}</el-descriptions-item>
                    <el-descriptions-item label="建模状态">
                        <el-tag type="info" v-if="currentFaceDetail.FStatus == 0">未建模</el-tag>
                        <el-tag type="success" v-if="currentFaceDetail.FStatus == 1">建模成功</el-tag>
                        <el-tag type="danger" v-if="currentFaceDetail.FStatus == 2">建模失败</el-tag>
                    </el-descriptions-item>
                    <el-descriptions-item label="创建时间">{{ currentFaceDetail.CreatedOn }}</el-descriptions-item>
                </el-descriptions>
            </div>
            <div slot="footer">
                <el-button @click="showFaceDetailDialog = false">关闭</el-button>
            </div>
        </el-dialog>

        <!-- 创建人脸信息对话框 -->
        <el-dialog title="创建人脸信息" :visible.sync="showCreateFaceDialog" width="450px" :close-on-click-modal="false"
            border>
            <el-form :model="currentFaceForm" ref="faceForm" label-width="80px">
                <el-form-item label="员工" prop="MemId">
                    <el-select v-model="currentFaceForm.MemInfo.RealName" ref="selectUser" placeholder="请选择员工"
                        @focus="getUsersFocus" style="width:100%"></el-select>
                    <org-picker :multiple="true" ref="userPicker" :selected="currentFaceForm.userInfo"
                        @ok="selectUsersed" />
                </el-form-item>
                <el-form-item label="建模库" prop="HouseId">
                    <el-input v-model="currentFaceForm.HouseInfo.HouseName" :readonly="true">
                    </el-input>
                </el-form-item>
                <el-form-item label="建模头像" prop="FaceImg">
                    <image-upload v-model="currentFaceForm.FaceImg" :limit="1"></image-upload>
                </el-form-item>
            </el-form>
            <div slot="footer">
                <el-button @click="showCreateFaceDialog = false">取消</el-button>
                <el-button type="primary" @click="saveFaceInfo">
                    保存
                </el-button>
            </div>
        </el-dialog>
    </div>
</template>
<script>
import {
    getFaceHouseList, addFaceHouse, removeFaceHouse, updateFaceHouse, getHouseInfo, getFacePage, addFace, removeFace
} from "@/api/ai/face";
import OrgPicker from "@/views/flowable/common/OrgPicker";

export default {
    components: { OrgPicker },
    data() {
        return {
            currentView: 'libraryList',
            currentLibrary: null,
            faces: {},
            // 筛选和分页
            librarySearchQuery: '',
            libraryStatusFilter: '',
            filteredLibraries: [],
            libloading: false,

            faceSearchQuery: '',
            filteredFaces: [],
            currentFacePage: 1,
            facePageSize: 30,
            total: 0,
            loading: false,
            selectedFaceIds: [],

            // 对话框状态
            showAddLibraryDialog: false,
            isEditingLibrary: false,
            currentLibraryForm: {
                HouseName: '',
                Status: '1',
                Remark: ""
            },


            showFaceDetailDialog: false,
            currentFaceDetail: null,

            showCreateFaceDialog: false,
            currentFaceForm: {
                Id: '',
                HouseId: '',
                MemId: '',
                MemInfo: { "RealName": "" },
                HouseInfo: { "HouseName": "" },
                FaceImg: '',
                userInfo: null
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
        };
    },
    async created() {
        await this.searchLibraries();
    },
    methods: {
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
        handleLibraryCommand(command, libitem) {
            if (command == "man") {
                this.enterFaceManagement(libitem);
            }
            else if (command == "edit") {
                this.showEditLibraryDialog(libitem.Id);
            }
            else if (command == "del") {
                this.deleteLibrary(libitem.Id);
            }
        },
        handleFaceCommand(command, faceitem) {
            if (command == "detail") {
                this.showFaceDetail(faceitem);
            }
            else if (command == "del") {
                this.deleteFace(faceitem.Id);
            }
        },
        // 显示编辑建模库对话框
        async showEditLibraryDialog(id) {
            this.isEditingLibrary = true;
            let tmprsp = await getHouseInfo(id);
            this.currentLibraryForm = tmprsp.data;
            this.showAddLibraryDialog = true;
        },

        // 保存建模库
        saveLibrary() {
            this.$refs.libraryForm.validate(async valid => {
                if (valid) {
                    if (this.isEditingLibrary) {
                        // 更新现有建模库
                        await updateFaceHouse(this.currentLibraryForm);
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
            }).then(async () => {
                await removeFaceHouse({ ids: id });
                this.searchLibraries();
                this.$message.success('建模库已删除');
            }).catch(() => { });
        },

        // 进入人脸管理
        enterFaceManagement(library) {
            this.currentLibrary = { ...library };
            this.currentView = 'faceManagement';
            this.currentFacePage = 1;
            this.faceSearchQuery = '';
            this.loadFaces();
        },

        // 返回建模库列表
        backToLibraryList() {
            this.currentView = 'libraryList';
            this.selectedFaceIds = [];
        },

        // 加载人脸数据
        async loadFaces() {
            if (!this.currentLibrary) return;
            let res = await getFacePage({ "Name": this.faceSearchQuery, "pageNum": this.currentFacePage, "pageSize": this.facePageSize })
            this.total = res.data.Total;
            this.loading = false;
            this.filteredFaces = res.data.List || [];
        },

        // 人脸搜索
        searchFaces() {
            this.currentFacePage = 1;
            this.loadFaces();
        },

        resetFaceFilters() {
            this.faceSearchQuery = '';
            this.currentFacePage = 1;
            this.loadFaces();
        },
        // 显示人脸详情
        showFaceDetail(face) {
            this.currentFaceDetail = { ...face };
            this.showFaceDetailDialog = true;
        },

        // 创建人脸信息
        createFace() {

            this.currentFaceForm = {
                Id: null,
                HouseId: this.currentLibrary.Id,
                MemId: null,
                MemInfo: { "RealName": "" },
                FaceImg: "",
                HouseInfo: this.currentLibrary,
                userInfo: null
            };
            this.showCreateFaceDialog = true;
        },

        // 保存人脸信息
        async saveFaceInfo() {
            if (this.currentFaceForm.MemId == null) {
                this.$message.error("请选择员工");
                return;
            }
            if (this.currentFaceForm.FaceImg == "") {
                this.$message.error("请上传建模头像");
                return;
            }

            await addFace(this.currentFaceForm);
            this.loadFaces();
            this.showCreateFaceDialog = false;
            this.$message.success('人脸信息更新成功');
        },

        // 删除单个人脸
        deleteFace(faceId) {
            this.$confirm('确定要删除这个人脸建模吗？', '确认删除', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(async () => {
                await removeFace({ ids: faceId });
                this.loadFaces();
                this.currentLibrary.FaceCount -= 1;
                this.$message.success('人脸建模已删除');
            }).catch(() => { });
        },

        // 批量删除人脸
        batchDeleteFaces() {
            if (this.selectedFaceIds.length === 0) return;

            this.$confirm(`确定要删除选中的 ${this.selectedFaceIds.length} 个人脸建模吗？`, '确认删除', {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(async () => {
                await removeFace({ ids: this.selectedFaceIds });
                this.loadFaces();
                this.currentLibrary.FaceCount -= this.selectedFaceIds.length;
                this.selectedFaceIds = [];
                this.$message.success(`成功删除 ${this.selectedFaceIds.length} 个人脸建模`);
            }).catch(() => { });
        },
        getUsersFocus() {
            //获取员工选择下拉列表的焦点
            this.$refs.selectUser.blur();
            if (this.currentFaceForm.userInfo == null) {
                if (this.currentFaceForm.MemId != null) {
                    let arr = [{ id: this.currentFaceForm.MemId, name: this.currentFaceForm.MemInfo.RealName, avatar: this.currentFaceForm.MemInfo.Avatar, type: "user" }]
                    this.currentFaceForm.userInfo = JSON.parse(JSON.stringify(arr));
                }
                else {
                    this.currentFaceForm.userInfo = [];
                }
            }
            this.$refs.userPicker.show(this.currentFaceForm.userInfo, "user");
        },
        selectUsersed(values) {
            this.currentFaceForm.userInfo = values;

            if (values.length > 0) {
                this.currentFaceForm.MemInfo = { "Id": values[0].id, "RealName": values[0].name, "Avatar": values[0].avatar }
                this.currentFaceForm.MemId = values[0].id;
            }
            else {
                this.currentFaceForm.MemInfo = { "RealName": "" };
                this.currentFaceForm.MemId = undefined;
            }
            this.$forceUpdate();

        },

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