<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="100px" class="addPeople">
        <el-form-item label="数据库类型" prop="databaseType">
          <el-select v-model="ruleForm.databaseType" readonly placeholder="请选择数据库类型">
            <el-option label="MySQL" value="mysql" :disabled="!isBaseaName"></el-option>
            <el-option label="SQLServer" value="sqlserver" :disabled="!isBaseaName"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="IP地址" prop="ipAddress">
          <el-input v-model="ruleForm.ipAddress" :readonly="!isBaseaName" placeholder="请输入IP地址" />
        </el-form-item>
        <el-form-item label="端口" prop="port">
          <el-input v-model="ruleForm.port" :readonly="!isBaseaName" placeholder="请输入端口号" />
        </el-form-item>
        <!-- <el-form-item label="数据库名称" prop="databaseName">
          <el-input v-model="ruleForm.databaseName" :readonly="!isBaseaName" placeholder="请输入数据库名称" />
        </el-form-item> -->
        <el-form-item label="链接名称" prop="linkName">
          <el-input v-model="ruleForm.linkName" :readonly="!isBaseaName" placeholder="请输入链接名称" />
        </el-form-item>
        <el-form-item label="用户名" prop="userName">
          <el-input v-model="ruleForm.userName" :readonly="!isBaseaName" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="密码" prop="password">
          <el-input v-model="ruleForm.password" :readonly="!isBaseaName" show-password placeholder="请输入密码" />
        </el-form-item>
      </el-form>
      <el-form v-if="!isBaseaName" ref="ruleFormName" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
        <el-form-item label="数据库名称" prop="databaseName">
          <el-select v-model="ruleForm.databaseName" placeholder="请选择数据库类型">
            <el-option v-for="(item, index) in showDatabases" :key="index" :label="item.name" :value="item.name" />
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button v-if="isBaseaName" type="primary" :disabled="isVerification" @click="nextStep('ruleForm')">{{ isVerification ? '验证中...' : '下一步' }}</el-button>
        <el-button v-if="!isBaseaName" type="primary" @click="submitForm('ruleFormName')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addSourse, updateSourse, showDatabases } from "@/api/report/sourse";
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
      ruleForm: {},
      list: [],
      isBaseaName: true,
      isVerification: false,
      // 表单校验
      rules: {
        databaseType: [
          { required: true, message: "数据库类型不能为空", trigger: "change" }
        ],
        ipAddress: [
          { required: true, message: "ip地址不能为空", trigger: "blur" }
        ],
        port: [
          { required: true, message: "端口号不能为空", trigger: "blur" }
        ],
        databaseName: [
          { required: true, message: "数据库名称不能为空", trigger: "blur" }
        ],
        linkName: [
          { required: true, message: "链接名称不能为空", trigger: "blur" }
        ],
        userName: [
          { required: true, message: "用户名不能为空", trigger: "blur" }
        ],
        password: [
          { required: true, message: "密码不能为空", trigger: "blur" }
        ]
      },
      showDatabases: [],
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
    }
  },
  methods: {
    nextStep(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.isVerification = true
          this.getShowDatabases()
        } else {
          return false
        }
      })
    },
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
        addSourse(this.ruleForm).then(res => {
            this.$message.success('添加成功!')
            this.$emit('getList')
        })
    },
    getUpdateSourse() {
        updateSourse(this.ruleForm).then(res => {
            this.$message.success('修改成功!')
            this.$emit('getList')
        })
    },
    getShowDatabases() {
      const data = {
        type: this.ruleForm.databaseType,
        ipAdress: this.ruleForm.ipAddress,
        port: this.ruleForm.port,
        userName: this.ruleForm.userName,
        password: this.ruleForm.password
      }
      showDatabases(data).then(res => {
        this.showDatabases = res.data
        this.isBaseaName = false
      })
      this.isVerification = false
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