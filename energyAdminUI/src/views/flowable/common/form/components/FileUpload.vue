<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-button size="small" icon="el-icon-paperclip" round
        >选择文件</el-button
      >
      <ellipsis
        :row="1"
        :content="placeholder + sizeTip"
        hoverTip
        slot="tip"
        class="el-upload__tip"
      />
    </div>
    <div v-else>
      <div style="display: flex;flex-direction: column;" v-if="disabled">
        <el-link
         style="width:100%;display: flex;justify-content: start;"
          icon="el-icon-download"
          type="primary"
          v-for="(file, index) in showarr"
          :key="index"
          :href="file.url"
          target="_blank"
          >{{ file.name }}</el-link
        >
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
        :before-upload="beforeUpload"
      >
        <el-button size="small" icon="el-icon-paperclip" round
          >选择文件</el-button
        >
        <ellipsis
          :row="1"
          :content="placeholder + sizeTip"
          hoverTip
          slot="tip"
          class="el-upload__tip"
        />
      </el-upload>
    </div>
  </div>
</template>

<script>
import componentMinxins from "../ComponentMinxins";
import Ellipsis from "../../Ellipsis.vue";
import { getToken } from "@/utils/auth";
export default {
  mixins: [componentMinxins],
  name: "FileUpload",
  components: { Ellipsis },
  props: {
    placeholder: {
      type: String,
      default: "请选择附件",
    },
    value: {
      type: Array,
      default: () => {
        return [];
      },
    },
    maxSize: {
      type: Number,
      default: 10,
    },
    maxNumber: {
      type: Number,
      default: 5,
    },
    fileTypes: {
      type: Array,
      default: () => {
        return [];
      },
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  computed: {
    sizeTip() {
      if (this.fileTypes.length > 0) {
        return ` | 只允许上传[${String(this.fileTypes).replaceAll(
          ",",
          "、"
        )}]格式的文件，且单个附件不超过${this.maxSize}MB`;
      }
      return this.maxSize > 0 ? ` | 单个附件不超过${this.maxSize}MB` : "";
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
    };
  },
  created() {
    this.showarr = Object.assign([], this.value);
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
      let fileExtension = "";
      if (file.name.lastIndexOf(".") > -1) {
        fileExtension = file.name.slice(file.name.lastIndexOf(".") + 1);
      }
      if (this.fileTypes.indexOf(fileExtension) === -1) {
        this.$message.warning("存在不支持的格式");
      } else if (this.maxSize > 0 && file.size / 1024 / 1024 > this.maxSize) {
        this.$message.warning(`文件最大不超过 ${this.maxSize}MB`);
      } else {
        return true;
      }
      return false;
    },
    handleRemove(file, fileList) {
      console.log(file, fileList);
    },
    handlePictureCardPreview(file) {
      console.log(file);
    },
    handleDownload(file) {
      console.log(file);
    },
  },
};
</script>

<style lang="less" scoped>
</style>
