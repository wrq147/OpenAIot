<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
        <el-form-item label="时段名称" prop="timeName">
          <el-input v-model="ruleForm.timeName" placeholder="请输入时段名称" />
        </el-form-item>
        <el-form-item label="上班时间" prop="startTime">
            <el-time-picker
                v-model="ruleForm.startTime"
                format="HH:mm"
                value-format="HH:mm"
                placeholder="上班时间"
            />
        </el-form-item>
        <el-form-item label="下班时间" prop="endTime">
            <el-time-picker
                v-model="ruleForm.endTime"
                format="HH:mm"
                value-format="HH:mm"
                placeholder="下班时间"
            />
        </el-form-item>
        <!-- <el-form-item label="归属单位" prop="deptId">
            <el-select v-model="ruleForm.deptId" placeholder="请选择单位">
                <el-option v-for="item in deptList" :key="item.deptId" :label="item.deptName" :value="item.deptId">
                </el-option>
            </el-select>
        </el-form-item> -->
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addTimeQuantum, editTimeQuantum } from "@/api/scheduling/timeQuantum";
import { listDept } from "@/api/system/dept";
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
        timeName: [
          { required: true, message: "时段名称不能为空", trigger: "blur" },
        ],
        startTime: [
          { required: true, message: "上班时间不能为空", trigger: "change" },
        ],
        endTime: [
          { required: true, message: "下班时间不能为空", trigger: "change" },
        ],
        // deptId: [
        //   { required: true, message: "归属单位不能为空", trigger: "change" },
        // ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
      this.getListdept();
    }
  },
  methods: {
    getListdept() {
        listDept().then(res => {
            this.deptList = res.data
        })
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
            if (this.title === '新增时段') {
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
        addTimeQuantum(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
    },
    setEditRpt() {
        editTimeQuantum(this.ruleForm).then(res => {
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