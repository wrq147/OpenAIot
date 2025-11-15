<template>
  <div class="upload-file">
    <div style="margin-bottom: 15px;">
      <!-- 添加固件按钮 -->
      <el-button type="primary" plain @click="openAddFirmwareDialog">添加固件</el-button>
    </div>

    <!-- 添加固件对话框 -->
    <el-dialog :visible.sync="addFirmwareDialogVisible" title="添加固件" width="680px">
      <el-form :model="newFirmware" ref="addFirmwareForm" label-width="80px" style="margin-left: 100px;">
        <el-form-item label="版本号">
          <el-input v-model="newFirmware.name" style="width:350px"></el-input>
        </el-form-item>
        <el-form-item label="固件类型">
          <el-input v-model="newFirmware.tag" style="width:350px"></el-input>
        </el-form-item>
        <el-form-item label="选择文件">
          <el-upload :action="uploadFileUrl" :before-upload="handleBeforeUpload" :file-list="fileList" :limit="1"
            :on-error="handleUploadError" :on-exceed="handleExceed" :on-success="handleUploadSuccessForDialog"
            :show-file-list="false" :headers="headers" ref="uploadForDialog" drag class="upload-file-uploader" :disabled="isUploading">
            <div class="upload_file_con file_loading" v-if="isUploading">
              <i class="el-icon-loading" style="font-size:20px;color:#409EFF;"></i>
              <div class="upload_file_text" style="color:#409EFF;">上传中</div>
            </div>
            <div class="upload_file_con" v-else>
              <i class="el-icon-upload"></i>
              <div class="upload_file_text">将文件拖到此处，或<em>点击上传</em></div>
              <div class="el-upload__tip" slot="tip">只能上传zip/rar/bin文件，且不超过100MB</div>
            </div>
          </el-upload>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="addFirmwareDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitAddFirmware">提交</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 固件列表 -->
    <el-table :data="firmwareList" :header-cell-style="{background: 'rgb(249, 250, 252)',color: 'rgb(120, 130, 157)','font-weight':'normal'}" stripe>
      <el-table-column prop="name" label="版本号"></el-table-column>
      <el-table-column prop="tag" label="固件类型"></el-table-column>
      <el-table-column prop="url" label="地址">
        <template #default="scope">
          <el-link :href="`${scope.row.url}`" :underline="false" target="_blank">
            <span class="el-icon-document"> 点击下载 </span>
          </el-link>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="scope">
          <el-link :underline="false" @click="handleDelete(scope.$index)" type="danger">
            删除
          </el-link>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getToken } from "@/utils/auth";
import request from '@/utils/request'
export default {
  name: "FileUpload2",
  props: {
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  watch: {
    productInfos: {
      handler(newVal) {
        this.fetchFirmwareList();
      },
      immediate: true
    }
  },
  data() {
    return {
      uploadFileUrl:
        process.env.VUE_APP_BASE_API == "/"
          ? "/AuthService/File/Upload?withDomain=true"
          : process.env.VUE_APP_BASE_API +
          "/AuthService/File/Upload?withDomain=true", // 上传的图片服务器地址
      headers: {
        Authorization: getToken(),
      },
      fileList: [],
      isUploading: false,
      firmwareList: [], // 存储固件列表数据
      addFirmwareDialogVisible: false,
      newFirmware: {
        name: '',
        tag: 'firmware',
        url: ''
      }
    };
  },
  methods: {
    // 获取固件列表
    fetchFirmwareList() {
      let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL);
      if (tmpmodeltsl != null && tmpmodeltsl.firmwares != null) {
        this.firmwareList = tmpmodeltsl.firmwares;
      }
    },
    // 打开添加固件对话框
    openAddFirmwareDialog() {
      this.addFirmwareDialogVisible = true;
      this.newFirmware = {
        name: '',
        tag: 'firmware',
        url: ''
      };
      setTimeout(()=>{
        this.$refs.uploadForDialog.clearFiles();
      },1000);

    },
    // 上传前校检
    handleBeforeUpload(file) {
      const allowedTypes = ['zip', 'rar', 'bin'];
      const fileType = file.name.split('.').pop().toLowerCase();
      if (this.isUploading) {
        return false;
      }
      if (!allowedTypes.includes(fileType)) {
        this.$message.error('只能上传 zip/rar/bin 文件');
        this.isUploading = false;
        return false;
      }

      this.isUploading = true;
      return true;
    },
    // 文件个数超出
    handleExceed() {
      this.$message.error(`上传文件数量不能超过 1 个!`);
    },
    // 上传失败
    handleUploadError(err) {
      this.isUploading = false;
      const errorMessage = err.response ? err.response.data.message : '上传失败，请重试';
      this.$message.error(errorMessage);
    },
    // 对话框中上传成功回调
    handleUploadSuccessForDialog(res, file) {
      if (this.newFirmware.url != null && this.newFirmware.url != "") {
        request({
          url: "/AuthService/File/Delete",
          method: "get",
          params: { id: this.newFirmware.url },
        });
      }
      this.$message.success("文件上传成功");
      this.newFirmware.url = res.data;
      this.isUploading = false;
    },
    // 提交添加固件信息
    async submitAddFirmware() {
      if (!this.newFirmware.name || !this.newFirmware.tag || !this.newFirmware.url) {
        this.$message.error('请填写完整信息');
        return;
      }
      this.firmwareList.unshift(this.newFirmware);
      console.info(this.firmwareList);
      this.$emit('changeSrc', this.firmwareList);
      this.addFirmwareDialogVisible = false;
    },
    // 删除固件
    async handleDelete(idx) {
      this.$modal
        .confirm('是否确认删除固件"' + this.firmwareList[idx].name + '"？')
        .then(rs => {
          if (rs == "confirm") {
            if (this.firmwareList[idx].url != null && this.firmwareList[idx].url != "") {
              request({
                url: "/AuthService/File/Delete",
                method: "get",
                params: { id: this.firmwareList[idx].url },
              });
            }
            this.firmwareList.splice(idx, 1);
            this.$emit('changeSrc', this.firmwareList);
          }
        });

    },
  },
};
</script>

<style scoped lang="scss">
.upload-file-uploader {
  margin-bottom: 5px;
  ::v-deep .el-upload-dragger{
    height: auto !important;
  }
}

.upload-file-list .el-upload-list__item {
  // border: 1px solid #e4e7ed;
  line-height: 2;
  margin-bottom: 10px;
  position: relative;
}

.upload-file-list .el-upload-list__item.ele-upload-list__item-content {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  color: inherit;
}

.upload-file-list .el-upload-list__item.ele-upload-list__item-content .ele-upload-list__item-content-action {
  display: inline-block;
  padding: 0 10px;
  border: 1px solid #F76C84;
  text-align: center;
  margin-left: 10px;
  border-radius: 3px;
}

.ele-upload-list__item-content-action .el-link {
  margin-right: 10px;
}

.upload_file_con {
  color: #606266;
  font-size: 14px;
  .upload_file_text em {
    color: #409EFF;
  }
}

.upload_file_con.file_loading {
  display: flex;
  height: 100%;
  align-items: center;
  flex-direction: column;
  justify-content: center;
}


</style>