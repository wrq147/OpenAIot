<template>
  <div>
    <el-dialog class="label_filter_dialog" :title="dialogTitle" :visible.sync="dialogVisible" width="650px" :before-close="handleClose">
      <el-form class="label_filter_form" ref="labelForm" :model="labelForm" :rules="rules" label-width="170px" label-position="top">
          <el-form-item label="标签名称" prop="name">
              <el-input v-model="labelForm.name" placeholder="请输入标签名称" />
          </el-form-item>
          <el-form-item label="标签名称" prop="code">
              <el-input v-model="labelForm.code" placeholder="请输入标签名称" />
          </el-form-item>
          <el-form-item label="标签名称" prop="type">
              <el-select filterable v-model="labelForm.type" placeholder="请选择数据类型" style="width: 100%">
                  <el-option v-for="item in typeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
              </el-select>
          </el-form-item>
          <el-form-item label="" prop="elementsLis" v-if="labelForm.type == 'enum'">
            <div class="item_flex_con" style="padding-bottom:14px">
              <div class="label_text">
                <span>枚举值</span>
                <span class="tips_text">
                  （枚举的键名是原值，枚举的键值是转换的目标值）
                </span>
              </div>
              <div class="handle_con">
                  <div class="guide_text" @click="exportEumVal(labelForm.elementsLis)">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left:6px">导入</span>
                  </div>
              </div>
            </div>
          <div class="enum_list_con">
            <div class="enum_list">
              <div class="enum_li" v-for="(it, inx) in labelForm.elementsLis" :key="it.key">
                <span>{{ it.key }}:{{ it.value }}</span>
                <div class="meijuclose">
                  <i class="zhongtaiiconfont zhongtai-icon-a-guanbihui" @click="delEnum(inx,it)"></i>
                </div>
              </div>
              <div class="enum_con" @click="showEnumInput" v-if="!(showEnumKey || showEnumVal)">
                + 枚举值
              </div>
            </div>
          </div>

          <el-form ref="labelForm2" style="width: 250px; display: inline-block"
            label-width="65px" v-if="showEnumKey || showEnumVal" label-position="top">
            <el-form-item label="" prop="enumKey" class="enum_key" v-if="showEnumKey">
              <div class="item_flex_con">
                <div class="label_text" style="width:75px">
                  <span>枚举key</span>
                </div>
                <el-input v-model="labelForm.enumKey" ref="enumKeyAuto" placeholder="请输入枚举key" @blur="eumKeyBlur" />
              </div>
            </el-form-item>
            <el-form-item label="" prop="enumValue" v-if="showEnumVal">
              <div class="item_flex_con">
                <div class="label_text" style="width:75px">
                  <span>枚举值</span>
                </div>
                <el-input v-model="labelForm.enumValue" ref="enumValAuto" placeholder="请输入枚举值" @blur="eumValBlur" />
              </div>
              
            </el-form-item>
          </el-form>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
          <el-button class="add_label_btn" @click="dialogVisible = false">取 消</el-button>
          <el-button class="add_label_btn" type="primary" @click="finishLabelAdd">确 定</el-button>
      </span>
    </el-dialog>
    <el-dialog :close-on-click-modal="false" :title="upload.title" :visible.sync="upload.open" width="400px" append-to-body :destroy-on-close="true">
        <el-upload ref="uploadref" :limit="1" accept=".txt" :on-progress="handleFileUploadProgress" :on-change="handleImport" :auto-upload="false" action="#" drag>
          <i class="el-icon-upload"></i>
          <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
          <div class="el-upload__tip text-center" slot="tip">
            <span style="line-height: 33px;">仅允许导入txt格式文件。</span>
            <el-link type="primary" :underline="false" style="font-size:12px;vertical-align: baseline;" @click="onImportTemplate">下载模板</el-link>
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
  name: 'AdminUiConditionLabel',

  data() {
    return {
      typeList: [
        { alabel: "整型", label: "整型(Int)", value: "int" },
        { alabel: "浮点", label: "浮点型(Float)", value: "float" },
        { alabel: "字符", label: "字符型(String)", value: "string" },
        { alabel: "时间", label: "时间型(Date)", value: "date" },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean" },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum" },
        { alabel: "文件", label: "文件类型(File)", value: "file" },
        { alabel: "位置", label: "设备位置(Geo)", value: "geo" },
      ], //数据类型列表
      dialogVisible:false,
      dialogTitle:'添加标签',
      labelForm:{
        name:'',
        code:'',
        type:'',
      },
      enumKeyList:[],
      rules: {
        name: [
          { required: true, message: "标签名称不能为空", trigger: "blur" },
        ],
        code: [
          { required: true, message: "标签标识符不能为空", trigger: "blur" },
        ],
        type: [
          { required: true, message: "标签类型不能为空", trigger: "change" },
        ],
      },
      expressOpen: false,
      expressEnable: false,
      showEnumKey: false, //是否显示枚举key输入框
      showEnumVal: false, //是否显示枚举值输入框
      upload:{
        title:'',
        open:false,
        file:null,
        isUploading:false
      }
    };
  },

  mounted() {
    
  },

  methods: {
    handleClose(){
      this.dialogVisible=false
    },
    finishLabelAdd(){
      let subForm=JSON.parse(JSON.stringify(this.labelForm))
      if(subForm.type=='enum'){
        subForm.elementsLis=subForm.elementsLis.filter(row=>row.key&&row.value)
      }else{
        if(subForm.elementsLis){
          delete subForm.elementsLis
        }
      }
      this.handleClose()
      this.$emit('finishLabelAdd',subForm)
    },
    openLabelAddDialog(form){
        if(form){
            this.labelForm=JSON.parse(JSON.stringify(form))
            if(this.labelForm.elementsLis){
              this.enumKeyList=this.labelForm.elementsLis.map(rw=>rw.key)
            }
        }else{
          this.labelForm={
            name:'',
            code:'',
            type:'',
          }
        }
        this.dialogVisible=true
    },
    changeType(val){
        if(val=='enum'){
            this.labelForm.elementsLis= []; //添加的枚举列表
            this.labelForm.enumKey= ""; //枚举key
            this.labelForm.enumValue= ""; //枚举value
        }else{
            delete this.labelForm.elementsLis //添加的枚举列表
            delete this.labelForm.enumKey //枚举key
            delete this.labelForm.enumValue //枚举value
        }
        
    },
    delEnum(inx, item) {
      this.labelForm.elementsLis.splice(inx, 1);
      this.enumKeyList.splice(this.enumKeyList.findIndex((item) => item == item.key),1);
      this.$forceUpdate();
    },
    eumKeyBlur() {
      //失去枚举Key输入框的焦点
      if (
        this.labelForm &&
        this.labelForm.enumKey &&
        this.labelForm.enumKey != undefined &&
        this.labelForm.enumKey != null &&
        this.labelForm.enumKey != ""
      ) {
        if (!this.enumKeyList.includes(this.labelForm.enumKey)) {
          this.showEnumKey = false;
          this.showEnumVal = true;
          let objs = {
            key: this.labelForm.enumKey,
            value: "",
          };
          if(this.labelForm.elementsLis){}else{
            this.labelForm.elementsLis=[]
          }
          this.labelForm.elementsLis.push(objs);
          this.enumKeyList.push(this.labelForm.enumKey);
          this.$nextTick(() => this.$refs.enumValAuto.focus());
        } else {
          this.$message.error("该枚举已存在");
          this.labelForm.enumKey = "";
          this.$nextTick(() => this.$refs.enumKeyAuto.focus());
        }
      } else {
        this.showEnumKey = false;
        this.showEnumVal = false;
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
      console.log(file,'filefile');
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
          // console.log("str导入的数据",str);
          let list1=str.split('\r\n')
          for(let i=0;i<list1.length;i++){
            let rowArr=list1[i].split(/\s+/)
            if(rowArr[0]&&rowArr[1]){
              this.enumKeyList.push(rowArr[0]);
              let obj = {
                key: rowArr[0],
                value: rowArr[1],
              };
              if(this.labelForm.elementsLis){}else{
                this.labelForm.elementsLis=[]
              }
              this.labelForm.elementsLis.push(obj);
            }
          }
          this.upload.open=false
          this.$forceUpdate()
          tmploading.close();
        }
      }
      
    },
    eumValBlur() {
      //失去枚举值输入框的焦点
      if (this.labelForm.enumValue) {
        this.showEnumVal = false;
        this.labelForm.elementsLis[this.labelForm.elementsLis.length - 1].value = this.labelForm.enumValue;
        this.labelForm.enumKey = "";
        this.labelForm.enumValue = "";
      }
    },
    showEnumInput() {
      //点击后显示枚举输入框
        this.showEnumKey = true;
        this.$nextTick(() => this.$refs.enumKeyAuto.focus());
    },
  },
};
</script>

<style lang="less" scoped>
.label_filter_form{
  ::v-deep .el-form-item__label{
    line-height: 14px;
    font-size: 14px;
    color: rgba(51, 51, 51, 1);
    padding-bottom: 14px;
  }
  ::v-deep .el-form-item{
    margin-bottom: 30px;
  }
  ::v-deep .el-input{
    height: 40px;
  }
  ::v-deep .el-select{
    height: 40px;
  }
}
.add_label_btn.el-button{
  width: 100px;
  height: 40px;
}
.item_flex_con{
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  .handle_con{
    display: flex;
    justify-content: flex-end;
    align-items: center;
    .guide_text{
      color: rgba(99, 101, 122, 1);
      font-size: 14px;
      margin-right: 30px;
    }
  }
  .tips_text{
      font-size: 12px;
      color: rgba(187, 192, 206, 1);
      // margin-left: 20px;
    }
}
.enum_list_con{
  display: inline-flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: flex-start;
  justify-content: space-between;
  width: 100%;
  .enum_con {
    width: 100px;
    height: 40px;
    line-height: 40px;
    text-align: center;
    border-radius: 15px;
    border: 1px dashed rgba(223, 226, 234, 1);
    display: inline-block;
    cursor: pointer;
    margin-left: 5px;
  }
  .enum_list {
    display: inline-flex;
    flex-direction: row;
    justify-content: flex-start;
    align-items: flex-start;
    flex-wrap: wrap;
    margin-right: -10px;
    width: 100%;
    .enum_li {
      padding:0 20px 0 20px;
      height: 40px;
      line-height: 40px;
      text-align: center;
      border-radius: 20px;
      border: 1px solid rgba(223, 226, 234, 1);
      display: inline-block;
      margin: 0 10px 5px 0;
      position: relative;
      .meijuclose{
        display: flex;
        justify-content: center;
        align-items: center;
        position: absolute;
        right: -2px;
        top: -2px;
        cursor: pointer;
        width: 16px;
        height: 16px;
        border-radius: 50%;
        background: rgba(239, 239, 239, 1);
        i.zhongtaiiconfont{
          font-size: 6px;
          color: #78829D;
        }
      }
    }
  }
}
</style>