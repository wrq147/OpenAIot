<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
        <el-form-item label="班组名称" prop="banZuName">
          <el-input v-model="ruleForm.banZuName" placeholder="请输入班组名称" />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addGroup, editGroup } from "@/api/scheduling/timeGroup";
export default { 
  name: 'addBatch',
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
      deviceOpen: false,
      deptList: [],
      // 表单
      ruleForm: {},
      // 校验
      rules: {
        banZuName: [
          { required: true, message: "班组名称不能为空", trigger: "blur" },
        ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
    }
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
            if (this.title === '新增班组') {
                this.getAddRpt()
            } else {
                this.setEditRpt()
            }
        } else {
          return false
        }
      })
    },
    getAddRpt() {
        addGroup(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
    },
    setEditRpt() {
        editGroup(this.ruleForm).then(res => {
          this.$message.success('修改成功!')
          this.$emit('getList')
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
  .el-date-editor.el-input {
    width: 100%;
  }
  .el-select, .el-cascader{
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