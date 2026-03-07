<template>
  <div>
    <el-form-item label="枚举值">
      <div>
        <div class="enum_list_con">
          <div class="enum_list">
            <template v-if="curVal.elements != null">
              <div class="enum_li" v-for="(it, kv) in curVal.elements" :key="kv">
                <span v-if="hasKey">{{ kv }}:{{ it }}</span>
                <span v-else>{{ it }}</span>
                <i class="zhongtaiiconfont zhongtai-icon-a-guanbihui meijuclose" @click="delEnum(kv, kv)"></i>
              </div>
            </template>

            <div class="enum_con" @click="showEnumInput" v-if="!(showEnumKey || showEnumVal)">
              +枚举值
            </div>
          </div>
          <div class="enum_btn" @click="exportEumVal()">导入</div>
        </div>

        <el-form ref="typeForm2" style="width: 250px; display: inline-block" label-width="65px" v-if="showEnumKey || showEnumVal">
          <el-form-item label="枚举key" prop="enumKey" class="enum_key" v-if="showEnumKey">
            <el-input
              v-model="typeForm.enumKey"
              ref="enumKeyAuto"
              placeholder="请输入枚举key"
              @blur="eumKeyBlur"
            />
          </el-form-item>
          <el-form-item label="枚举值" prop="enumValue" v-if="showEnumVal">
            <el-input
              v-model="typeForm.enumValue"
              ref="enumValAuto"
              placeholder="请输入枚举值"
              @blur="eumValBlur"
            />
          </el-form-item>
        </el-form>
        <div style="font-size: 14px; color: #909399" v-if="hasKey">
          <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="margin-right: 5px; font-size: 14px">
          </i>枚举的键名是原值，枚举的键值是映射的目标值
        </div>
      </div>
    </el-form-item>
    <el-form-item label="是否多选">
      <el-switch
        v-model="curVal.multi"
        active-color="#13ce66"
        inactive-color="#ff4949">
      </el-switch>
    </el-form-item>
    <el-dialog :close-on-click-modal="false" :title="upload.title" :visible.sync="upload.open" width="400px" append-to-body :destroy-on-close="true">
      <el-upload
        ref="uploadref"
        :limit="1"
        accept=".txt"
        :on-progress="handleFileUploadProgress"
        :on-change="handleImport"
        :auto-upload="false"
        action="#"
        drag
      >
        <i class="el-icon-upload"></i>
        <div class="el-upload__text">
          将文件拖到此处，或
          <em>点击上传</em>
        </div>
        <div class="el-upload__tip text-center" slot="tip">
          <span style="line-height: 33px;">仅允许导入txt格式文件。</span>
          <el-link
            type="primary"
            :underline="false"
            style="font-size:12px;vertical-align: baseline;"
            @click="onImportTemplate"
          >下载模板</el-link>
        </div>
      </el-upload>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="submitFileForm">确 定</el-button>
        <el-button @click="upload.open = false">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
export default {
  name: "enumItem",
  props: {
    value: {
      type: Object,
      default: () => {
        return {};
      },
    },
    hasKey: {
      type: Boolean,
      default: () => {
        return false;
      },
    },
  },
  data() {
    return {
      curVal: this.value,
      showEnumKey: false, //是否显示枚举key输入框
      showEnumVal: false, //是否显示枚举值输入框
      typeForm: { enumKey: "", enumValue: "" },
      upload:{
        title:'',
        open:false,
        file:null,
        isUploading:false
      }
    };
  },
  mounted() {},
  methods: {
    delEnum(val, kv) {
      //删除枚举
      this.$modal
        .confirm('是否确认删除枚举key为"' + kv + '"的枚举？')
        .then((rs) => {
          if (rs == "confirm") {
            this.$delete(this.curVal.elements, kv);
            this.$emit("input", JSON.parse(JSON.stringify(this.curVal)));
            this.$forceUpdate();
          }
        });
    },
    eumKeyBlur() {
      this.curVal.elements[this.typeForm.enumKey] = "";
      this.showEnumKey = false;
      this.showEnumVal = true;
    },
    eumValBlur() {
      if (!this.hasKey) {
        this.typeForm.enumKey = this.typeForm.enumValue;
      }
      this.curVal.elements[this.typeForm.enumKey] = this.typeForm.enumValue;
      this.$emit("input", JSON.parse(JSON.stringify(this.curVal)));
      this.showEnumVal = false;
    },
    showEnumInput() {
      if (this.curVal.elements == null) {
        this.curVal.elements = {};
      }
      //点击后显示枚举输入框
      if (this.hasKey) {
        this.showEnumKey = true;
        this.typeForm.enumKey = "";
        this.typeForm.enumValue = "";
        this.$nextTick(() => this.$refs.enumKeyAuto.focus());
      } else {
        this.showEnumVal = true;
        this.typeForm.enumKey = "";
        this.typeForm.enumValue = "";
      }
    },
    exportEumVal(){
      this.upload.title="导入枚举"
      this.upload.open=true
    },
    onImportTemplate() {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let tmname = new Date().getTime();;
      tmploading.close();
      const content = "键名  键值\r\n键名  键值"
      const blobData = new Blob([content], { type: 'text/plain;charset=utf-8' })
      const filename = `${tmname}.txt` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    
    handleImport(file, fileList) {
      this.upload.file=file.raw
    },
    // 提交上传文件
    submitFileForm(){
      if(this.upload.file){
        let tmploading = this.$loading({
          lock: true,
          text: "导入中...",
          background: "rgba(0, 0, 0, 0.7)",
        });
        const reader = new FileReader()
        reader.readAsText(this.upload.file)
        reader.onload = (e)=> {
          const str = e.target.result
          let list1=str.split('\r\n')
          for(let i=0;i<list1.length;i++){
            if(this.curVal.elements==undefined||this.curVal.elements==null){
              this.curVal.elements={}
            }
            let rowArr=list1[i].split(/\s+/)
            if(rowArr[0]&&rowArr[1]){
              this.curVal.elements[rowArr[0]]=rowArr[1]
            }
          }
          this.upload.open=false
          this.$forceUpdate()
          tmploading.close();
        }
      }
      
    },
  },
};
</script>

<style lang="scss" scope>
.enum_list_con{
  display: inline-flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: flex-start;
  justify-content: space-between;
}
.enum_btn{
  width: 60px;
  height: 26px;
  border-radius: 5px;
  background: #007ACC;
  color: #ffffff;
  font-size: 14px;
  display: flex;
  justify-content: center;
  align-items: center;
}
.enum_list {
  display: inline-flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: flex-start;
  flex-wrap: wrap;
  margin-right: -5px;
  width: 410px;

  .enum_li {
    // width: 100px;
    padding: 0 15px 0 8px;
    height: 30px;
    line-height: 30px;
    text-align: center;
    border-radius: 15px;
    border: 1px solid #dddddd;
    display: inline-block;
    margin: 0 5px 5px 0;
    position: relative;

    .svg-icon {
      font-size: 6px;
      position: absolute;
      right: 6px;
      top: 12px;
      cursor: pointer;
    }
    .zhongtaiiconfont.meijuclose {
      font-size: 6px;
      position: absolute;
      right: 6px;
      top: 0px;
      cursor: pointer;
    }
  }
}

.enum_con {
  width: 100px;
  height: 30px;
  line-height: 30px;
  text-align: center;
  border-radius: 15px;
  border: 1px dashed #dddddd;
  display: inline-block;
  cursor: pointer;
  margin-left: 5px;
}
</style>