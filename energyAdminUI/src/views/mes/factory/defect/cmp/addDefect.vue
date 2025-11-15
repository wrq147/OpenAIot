<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
      width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
        <el-form-item label="不良品项名称" prop="defectName">
          <el-input v-model="ruleForm.defectName" placeholder="请输入不良品项名称" />
        </el-form-item>
        <el-form-item label="不良类别" prop="defectCategory">
          <el-select v-model="ruleForm.defectCategory" placeholder="请选择不良类别">
            <el-option :label="'外观'" :value="'外观'"></el-option>
            <el-option :label="'功能'" :value="'功能'"></el-option>
            <el-option :label="'性能'" :value="'性能'"></el-option>
            <el-option :label="'其它'" :value="'其它'"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
  import { defectAdd, defectEdit } from "@/api/mes/defect";
  export default {
    name: 'addDefect',
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
        // 校验
        rules: {
          defectName: [
            { required: true, message: "请输入不良品项名称", trigger: "blur" },
          ],
          defectCategory: [
            { required: true, message: "请选择不良类别", trigger: "change" },
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
            if (this.title === '新增不良品项') {
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
        defectAdd(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
      },
      setEditRpt() {
        defectEdit(this.ruleForm).then(res => {
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
    .el-dialog__header {
      border-bottom: 1px solid #ccc;
    }
  
    .el-select,
    .el-cascader {
      width: 100%;
    }
  }
  
  .addPeople>.box {
    display: flex;
    align-items: center;
    justify-content: space-between;
    flex-wrap: wrap;
  }
  
  .addPeople>.btn {
    width: 100%;
    justify-content: flex-end;
    display: flex;
    align-items: center;
  }
  </style>