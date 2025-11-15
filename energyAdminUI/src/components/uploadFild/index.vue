<template>
    <!-- 设备导入对话框 -->
    <el-dialog
        :close-on-click-modal="false"
        :title="upload.title"
        :visible.sync="upload.open"
        width="640px"
        append-to-body
      >
        <el-upload
            ref="uploadref"
            :limit="1"
            accept=".xlsx, .xls"
            :headers="upload.headers"
            :action="upload.url"
            :disabled="upload.isUploading"
            :on-progress="handleFileUploadProgress"
            :on-success="handleFileSuccess"
            :auto-upload="false"
            drag
        >
        <img src="~@/assets/images/uploadIcon.png" alt="">
        <div class="el-upload__text">
            将文件拖到此处，或
            <em>点击上传</em>
        </div>
        <div class="el-upload__tip text-center" slot="tip">
            <!-- <div class="el-upload__tip" slot="tip">
              <el-checkbox v-model="upload.updateSupport" />是否更新已经存在的设备数据
            </div> -->
            <span style="line-height: 33px;color: rgba(255, 255, 255, 0.6);">仅允许导入xls、xlsx格式文件。</span>
            <el-link
                type="primary"
                :underline="false"
                style="font-size:12px;vertical-align: baseline;color:#3DB98F"
                @click="onImportTemplate"
            >下载模板</el-link>
        </div>
        </el-upload>
        <div slot="footer" class="dialog-footer">
            <el-button type="primary" @click="submitFileForm">确 定</el-button>
            <el-button @click="upload.open = false">取 消</el-button>
        </div>
    </el-dialog>
</template>
<script>
import { getToken } from "@/utils/auth";
export default {
    name: 'uploadFild',
    components: {
      
    },
    data() {
        return {
            // 设备导入参数
            upload: {
                // 是否显示弹出层（设备导入）
                open: false,
                // 弹出层标题（设备导入）
                title: "导入",
                // 是否禁用上传
                isUploading: false,
                // 是否更新已经存在的用户数据
                updateSupport: false,
                // 设置上传的请求头部
                headers: { Authorization: getToken() },
                // 上传的地址
                url: ''
            },
        };
    },
    methods: {
        // 打开弹出层
        openDialog(url) {
            this.upload.open = true;
            this.upload.url = process.env.VUE_APP_BASE_API+'/' + url;
        },
        // 文件上传中处理
        handleFileUploadProgress(event, file, fileList) {
            this.upload.isUploading = true;
        },

        // 文件上传成功处理
        handleFileSuccess(response, file, fileList) {
            this.upload.open = false;
            this.upload.isUploading = false;
            this.$refs.uploadref.clearFiles();
            let message=response.message?response.message:'导入成功'
            this.$alert(message, "导入结果", { dangerouslyUseHTMLString: true });
            this.$emit('getList');
        },
        // 提交上传文件
        submitFileForm() {
            this.$refs.uploadref.submit();
        },
        /** 下载模板操作 */
        onImportTemplate() {
            this.$emit('importTemplate');
        },
    }
}
</script>
<style lang="less" scoped>
::v-deep {
    .el-dialog__header{
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        padding: 20px 20px 19px;
    }
    .el-dialog__title{
        font-weight: 500;
        font-size: 16px;
        color: #FFFFFF;
        padding: 0;
        margin: 0;
        line-height: 0;
        position: relative;
        padding-left: 24px;
        &::before{
            content: '';
            position: absolute;
            left: 0;
            top: 50%;
            transform: translateY(-50%);
            background: url('~@/assets/images/zs.png') no-repeat;
            width: 16px;
            height: 16px;
        }
    }
    .el-dialog__body{
        padding-top: 20px;
    }
    .el-upload{
        width: 100%;
         .el-upload-dragger{
            width: 100%;
            background-color: transparent;
            border: 1px dashed rgba(255, 255, 255, 0.2);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            >img{
                width: 60px;
                height: 64px;
            }
            .el-upload__text{
                font-weight: 400;
                font-size: 14px;
                color: #FFFFFF;
                margin-top: 20px;
                >em{
                    color:#3DB98F;
                }
            }
        }  
    }
    .el-upload__tip{
        font-weight: 400;
        font-size: 12px;
        color: rgba(255, 255, 255, 0.6);
        .el-checkbox{
            margin-right: 10px;
        }
    }
}
</style>