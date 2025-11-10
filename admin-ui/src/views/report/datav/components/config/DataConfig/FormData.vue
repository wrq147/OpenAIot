<template>
  <div>
      <el-form-item  label="表格文件">
        <el-select v-model="formSource.id" placeholder="请选择" @change="formChange">
          <el-option
            v-for="item in formSourceList"
            :key="item.id"
            :label="item.fileName"
            :value="item.id">
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item  label="表格模板">
        <a href="../../template/表格模板.xlsx" style="color:#1890ff;cursor:pointer" download="表格模板.xlsx">表格模板下载</a>
      </el-form-item>

      <el-form-item label="*格式说明">
        <div class="el-form-item__content">
          <span style="word-wrap: break-word;">首行为静态数据中json对象的key值，其余各行数据源数据与列名对应</span>
        </div>
      </el-form-item> 

      <el-form-item  label="">            
        <el-button type="success" @click="addClick" >新增表格数据</el-button>
        <el-button type="primary" @click="updateClick" >更新表格数据</el-button>
      </el-form-item>

      <el-form-item v-show="formSourceFlag === 'update'" label="表格名称">
        <el-input placeholder="请输入内容" v-model="formSource.fileName" :disabled="true"></el-input>
      </el-form-item>

      <el-form-item v-show="formSourceFlag === 'update'" label="文件连接">
        <a :href="formSource.fileUrl" style="color:#1890ff;cursor:pointer" download="基本柱图表格模板.xlsx">{{formSource.fileName}}</a>
      </el-form-item>

      <el-form-item v-show="formSourceFlag === 'add'" label="表格名称">
        <el-input placeholder="请输入内容" v-model="addFileName" ></el-input>
      </el-form-item>

      <el-form-item v-show="formSourceFlag !== null" label="上传文件">
        <el-upload
          class="upload-demo"
          drag
          action
          ref="upload"
          :limit="1"
          :http-request="httpRequest"
          :show-file-list="true" 
          :before-upload="beforeUpload"
          :on-exceed="handleExceed"
          :on-remove="handleRemove"
          >
          <i class="el-icon-upload"></i>
        </el-upload>
      </el-form-item>

      <el-form-item v-show="formSourceFlag !== null" >
        <el-button type="primary" @click="formSubmit">确定</el-button>
        <el-button type="info" @click="formCancel" >取消</el-button>
      </el-form-item>

  </div>
</template>

<script>
import { listFormsource, addFormsource, updateFormsource, getFormsource } from '@/api/report/formsource'

export default {
  props:["costomData","chartType"],
  data(){
    return{
      formSourceFlag: null,
      formSource: (this.costomData.chartOption.formSource != undefined && this.costomData.chartOption.formSource != null) ? this.costomData.chartOption.formSource:JSON.parse(JSON.stringify( {id: '',fileName: '',fileUrl: '',chartType: this.chartType} )),
      formSourceList: [],
      formQueryParam: {
        chartType: "bar"
      },
      fileList: [],
      addFileName: '',
    }
  },
  mounted() {
  
    this.getFormsourceList();
  },
  methods:{
    //获取表格数据下拉框
    getFormsourceList() {
      listFormsource(this.formQueryParam).then((response) => {      
        this.formSourceList = response.rows;
      });
    },
    formChange(option) {
      getFormsource(option).then(response => {
        if (response.code == 200) {
          this.formSource.id = response.data.id;
          this.formSource.fileName = response.data.fileName;
          this.formSource.fileUrl = response.data.fileUrl;
          
          this.$emit("changeForm", this.formSource);
          // this.$set(this.configData.chartOption, 'formSource', JSON.parse(JSON.stringify( this.formSource )));
        }
      })
    },
    updateClick() {
      this.formSourceFlag = 'update';
    },
    addClick() {
      this.formSourceFlag = 'add';
    },
    //自定义上传,重写文件上传方法,覆盖原有的上传方法，将上传的文件依次添加到fileList数组中
    httpRequest(option) {
        this.fileList.push(option);
        this.msgSuccess("成功上传至浏览器缓存");
    },
    // 上传预处理
    beforeUpload(file) {
      const Xls = file.name.split('.'); 
      if(Xls[1] === 'xls'||Xls[1] === 'xlsx'){
        return file;
      }else {
        this.$message.error('请上传excel格式的文件!')
        return false;
      }
    },
    handleExceed(files, fileList) {
      this.$message.warning(`当前限制选择 1 个文件，本次选择了 ${files.length} 个文件，共选择了 ${files.length + fileList.length} 个文件`);
    },
    handleRemove(file, fileList) {
      this.fileList = [];
    },
    formCancel() {
      this.formSourceFlag = null;
      this.fileList = [];
      this.$refs.upload.clearFiles()
    },
    formSubmit() {
      if(this.fileList.length == 0) {
        this.$message({
          message: '请上传文件在进行提交！',
          type: 'warning'
        });
        return false;
      }
      // 使用form表单的数据格式
      let paramsData = new FormData()
      // 将上传文件数组依次添加到参数paramsData中
      this.fileList.forEach((it, index) => {
        //paramsData.append(`files[${index}]`, it.file)
        paramsData.append('file', it.file); // 因为要上传多个文件，所以需要遍历一下才行
      });
      paramsData.append('chartType', this.formSource.chartType)
      // 将表单数据添加到参数paramsData中
      if(this.formSourceFlag == 'update') {
        paramsData.append('id', this.formSource.id)
        paramsData.append('fileName', this.formSource.fileName)
        paramsData.append('fileUrl', this.formSource.fileUrl)
        updateFormsource(paramsData).then(response => {
          //  console.log(response);
          if (response.code == 200) {
              this.formSourceFlag = null;
              this.fileList = [];
              this.$refs.upload.clearFiles()
              this.msgSuccess("更新成功");
              //this.getFormsourceList();
              this.formSource.id = response.data.id;
              this.formSource.fileName = response.data.fileName;
              this.formSource.fileUrl = response.data.fileUrl;
              // this.$set(this.configData.chartOption, 'formSource', this.formSource);
              this.$emit("changeForm", this.formSource);
          }
        });
      } else if(this.formSourceFlag == 'add') {
        paramsData.append('fileName', this.addFileName)
        addFormsource(paramsData).then(response => {
          //  console.log(response);
          if (response.code == 200) {
            this.addFileName = '';
            this.formSourceFlag = null;
            this.fileList = [];
            this.$refs.upload.clearFiles()
            this.getFormsourceList();
            this.msgSuccess("新增成功");
            this.formSource.id = response.data.id;
            this.formSource.fileName = response.data.fileName;
            this.formSource.fileUrl = response.data.fileUrl;
            // this.$set(this.configData.chartOption, 'formSource', this.formSource);
            this.$emit("changeForm", this.formSource);
          }
        });
      }
      
    },
  }
}
</script>