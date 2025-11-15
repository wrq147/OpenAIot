<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="80px">
        <el-form-item label="班次名称" prop="banCiName">
          <el-input v-model="ruleForm.banCiName" placeholder="请输入班次名称" />
        </el-form-item>
        <el-form-item label="开始日期" prop="startDate">
            <el-date-picker
                v-model="ruleForm.startDate"
                value-format="yyyy-MM-dd"
                type="date"
                placeholder="开始日期"
            />
        </el-form-item>
        <el-form-item label="结束日期" prop="endTDate">
            <el-date-picker
                v-model="ruleForm.endTDate"
                value-format="yyyy-MM-dd"
                type="date"
                placeholder="结束日期"
            />
        </el-form-item>
        <!-- <el-form-item label="所属单位" prop="deptId">
            <el-select v-model="ruleForm.deptId" placeholder="请选择所属单位">
                <el-option v-for="item in deptList" :key="item.deptId" :label="item.deptName" :value="item.deptId" />
            </el-select>
        </el-form-item> -->
        <el-form-item label="周期数" prop="zhouQiShu">
            <el-input type="number" :disabled="title==='编辑班次'" v-model="ruleForm.zhouQiShu" placeholder="请输入班次名称" />
        </el-form-item>
        <el-form-item label="周期单位" prop="zhouQiDanWei">
            <el-select v-model="ruleForm.zhouQiDanWei" :disabled="title==='编辑班次'" placeholder="请选择单位">
                <el-option v-for="(item, index) in unitList" :key="index" :label="item" :value="item" />
            </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">保存</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addClasses, editClasses, classesPeriod } from "@/api/scheduling/timeClasses";
import { listDept } from "@/api/system/dept";
import timeInfo from './timeInfo.vue'
import periodInfo from './periodInfo.vue'
export default { 
  name: 'addBatch',
  components: {
    timeInfo,
    periodInfo
  },
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
      unitList: [],
      // 表单
      ruleForm: {},
      // 校验
      rules: {
        banCiName: [
          { required: true, message: "时段名称不能为空", trigger: "blur" },
        ],
        startDate: [
          { required: true, message: "上班时间不能为空", trigger: "change" },
        ],
        endTDate: [
          { required: true, message: "下班时间不能为空", trigger: "change" },
        ],
        // deptId: [
        //   { required: true, message: "所属单位不能为空", trigger: "change" },
        // ],
        zhouQiShu: [
          { required: true, message: "周期数不能为空", trigger: "blur" },
        ],
        zhouQiDanWei: [
          { required: true, message: "周期单位不能为空", trigger: "change" },
        ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
      if (newValue) {
        this.getListdept();
        this.getUnitList();
      }
    }
  },
  methods: {
    getListdept() {
        listDept().then(res => {
            this.deptList = res.data
        })
    },
    getUnitList() {
        classesPeriod().then(res => {
            this.unitList = res.data
        })
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
            if (this.title === '新增班次') {
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
        addClasses(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
    },
    setEditRpt() {
        editClasses(this.ruleForm).then(res => {
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
.tableList{
    margin-top: 10px;
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
}
.tableList .left{
  width: 45% !important;
  padding: 10px;
  border: 1px solid #ccc;
}
.tableList .right{
  width: 53% !important;
  padding: 10px;
  border: 1px solid #ccc;
}
</style>