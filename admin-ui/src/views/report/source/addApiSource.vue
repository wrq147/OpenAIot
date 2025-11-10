<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
        <el-form-item label="接口类型" prop="apiType">
          <el-select v-model="ruleForm.apiType" placeholder="请选择接口类型" :disabled="true">
            <el-option label="BI报表" :value="0"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="接口名称" prop="interfaceName">
          <el-input v-model="ruleForm.interfaceName" placeholder="请输入接口名称" />
        </el-form-item>
        <el-form-item label="接口地址" prop="url">
          <el-input v-model="ruleForm.url" placeholder="请输入接口地址" />
        </el-form-item>
        <el-form-item label="接口方法" prop="method">
          <el-select v-model="ruleForm.method" placeholder="请选择接口方法">
            <el-option label="GET" value="GET"></el-option>
            <el-option label="POST" value="POST"></el-option>
            <el-option label="PUT" value="PUT"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="参数类型" prop="method">
          <el-select v-model="ruleForm.method" placeholder="请选择参数类型">
            <el-option label="PARAM" value="PARAM"></el-option>
            <el-option label="JSON" value="JSON"></el-option>
            <el-option label="FORM" value="FORM"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="请求的Param" prop="paramJson">
          <el-input v-model="ruleForm.paramJson" placeholder="请输入请求的Param" />
        </el-form-item>
        <el-form-item label="请求的Header" prop="headerJson">
          <el-input v-model="ruleForm.headerJson" placeholder="请输入请求的Header" />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addApiSource, updateApiSource } from "@/api/report/apisource";
export default { 
  name: 'addDataOrigin',
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
        type: String
    }
  },
  data() {
    return {
      dialogFlag: false,
      // 表单
      ruleForm: {
        apiType:0
      },
      list: [],
      // 表单校验
      rules: {
        apiType: [
          { required: true, message: "接口类型不能为空", trigger: "change" }
        ],
        interfaceName: [
          { required: true, message: "接口名称不能为空", trigger: "blur" }
        ],
        url: [
          { required: true, message: "接口地址不能为空", trigger: "blur" }
        ],
        method: [
          { required: true, message: "接口方法不能为空", trigger: "change" }
        ],
        paramType: [
          { required: true, message: "参数类型不能为空", trigger: "change" }
        ],
        paramJson: [
          { required: true, message: "请求的Param不能为空", trigger: "blur" }
        ],
        headerJson: [
          { required: true, message: "请求的Header不能为空", trigger: "blur" }
        ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
    }
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
            if (this.title === '添加数据源') {
            this.getAddSoursen()
          } else {
            this.getUpdateSourse()
          }
        } else {
          return false
        }
      })
    },
    getAddSoursen() {
      addApiSource(this.ruleForm).then(res => {
        this.$message.success('添加成功!')
        this.$emit('getApiList')
      })
    },
    getUpdateSourse() {
      updateApiSource(this.ruleForm).then(res => {
        this.$message.success('修改成功!')
        this.$emit('getApiList')
      })
    },
    cancel() {
      this.$emit('cancelForm')
    }
  }
}
</script>
  
<style lang="scss" scoped>
 ::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
  .el-select{
    width: 100%;
  }
}
.addPeople>.box{
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}
.addPeople>.btn{
  width:100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>