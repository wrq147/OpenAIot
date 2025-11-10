<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <div class="design">
        <i class="el-icon-plus"></i>
      </div>
      <p>{{ placeholder }} {{ sizeTip }}</p>
    </div>
    <div v-else>
      <div v-if="disabled">
        <el-image
          :preview-src-list="previewArr"
          v-for="(img, index) in showarr"
          :key="index"
          style="width: 80px; height: 80px; margin-right: 10px; cursor: pointer"
          :src="img.url"
          fit="cover"
        ></el-image>
      </div>
      <el-upload
        v-else
        :file-list="showarr"
        :on-success="handleSuccess"
        :action="uploadFileUrl"
        :headers="headers"
        :limit="maxSize"
        with-credentials
        :multiple="maxSize > 0"
        list-type="picture-card"
        :before-upload="beforeUpload"
      >
        <i slot="default" class="el-icon-plus"></i>
        <div slot="file" slot-scope="{ file }">
          <img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
          <span class="el-upload-list__item-actions">
            <span
              class="el-upload-list__item-preview"
              @click="handlePictureCardPreview(file)"
            >
              <i class="el-icon-zoom-in"></i>
            </span>
            <span
              v-if="!disabled"
              class="el-upload-list__item-delete"
              @click="handleDownload(file)"
            >
              <i class="el-icon-download"></i>
            </span>
            <span
              v-if="!disabled"
              class="el-upload-list__item-delete"
              @click="handleRemove(file)"
            >
              <i class="el-icon-delete"></i>
            </span>
          </span>
        </div>
        <div slot="tip" class="el-upload__tip">
          {{ placeholder }} {{ sizeTip }}
        </div>
      </el-upload>
    </div>
  </div>
</template>

<script>
import componentMinxins from "../ComponentMinxins";
import { getToken } from "@/utils/auth";
import request from '@/utils/request'
export default {
  mixins: [componentMinxins],
  name: "ImageUpload",
  components: {},
  props: {
    value: {
      type: Array,
      default: () => {
        return [];
      },
    },
    placeholder: {
      type: String,
      default: "请选择图片",
    },
    maxSize: {
      type: Number,
      default: 10,
    },
    maxNumber: {
      type: Number,
      default: 5,
    },
    enableZip: {
      type: Boolean,
      default: true,
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  computed: {
    sizeTip() {
      return this.maxSize > 0 ? `| 每张图不超过${this.maxSize}MB` : "";
    },
  },
  data() {
    return {
      uploadFileUrl:
        process.env.VUE_APP_BASE_API == "/"
          ? "/AuthService/File/Upload?withDomain=true"
          : process.env.VUE_APP_BASE_API + "/AuthService/File/Upload?withDomain=true",
      headers: {
        Authorization: getToken(),
      },
      showarr: [],
      previewArr: [],
    };
  },
  created() {
    this.showarr = Object.assign([], this.value);
    this.previewArr = this.showarr.map((x) => x.url);
  },
  methods: {
    handleSuccess(response, file, fileList, idx) {
      if(this.value==null){
        this.value=[];
      }
      this.value.push({ name: file.name, url: response.data });
      this.$emit('input',  this.value);
    },
    beforeUpload(file) {
      const alows = ["image/jpeg", "image/png", "image/gif", "image/jpg"];
      if (alows.indexOf(file.type) === -1) {
        this.$message.warning("存在不支持的图片格式");
      } else if (this.maxSize > 0 && file.size / 1024 / 1024 > this.maxSize) {
        this.$message.warning(`单张图片最大不超过 ${this.maxSize}MB`);
      } else {
        return true;
      }
      return false;
    },
    handleRemove(file, fileList) {
      console.log(file, fileList);
      if(file&&file.response&&file.response.length>0){
          request({
            url: '/AuthService/File/Delete',
            method: 'get',
            params: {id:file.response[0].data}
          }).then(res=>{
            let valArr=this.showarr.filter(row=>row.url!=file.response[0].data)
            this.showarr=JSON.parse(JSON.stringify(valArr))
            this.$emit("input", valArr);
            this.$forceUpdate()
          });
          
       }else if(file&&file.url){
          request({
            url: '/AuthService/File/Delete',
            method: 'get',
            params: {id:file.url}
          }).then(res=>{
            let valArr=this.showarr.filter(row=>row.url!=file.url)
            this.showarr=JSON.parse(JSON.stringify(valArr))
            this.$emit("input", valArr);
            this.$forceUpdate()
          });
          // let valArr=this.showarr.filter(row=>row.url!=file.response[0].data)
          // this.$emit("input", valArr);
       }
       
    },
    handlePictureCardPreview(file) {
      let newarr = this.value.filter((x) => x.url != file.url);
      newarr = newarr.map((x) => x.url);
      newarr.unshift(file.url);
      this.$openPreview(newarr);
    },
    handleDownload(file) {
      console.log(file);
    },
  },
};
</script>

<style lang="less" scoped>
.design {
  i {
    padding: 10px;
    font-size: xx-large;
    background: white;
    border: 1px dashed #8c8c8c;
  }
}
/deep/ .el-upload--picture-card {
  width: 80px;
  height: 80px;
  line-height: 87px;
}
/deep/ .el-upload-list__item {
  width: 80px;
  height: 80px;
  .el-upload-list__item-actions {
    & > span + span {
      margin: 1px;
    }
  }
}
</style>
