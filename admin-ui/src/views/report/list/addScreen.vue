<template>
    <el-dialog v-if="dialogFlag" :title="title + '大屏'" :visible.sync="dialogFlag" :close-on-click-modal="false" width="700px" top="2vh" @close="cancel">
      <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="80px" class="addPeople">
        <el-form-item label="报表名称" prop="Name">
          <el-input v-model="ruleForm.Name" placeholder="请输入报表名称" />
        </el-form-item>
        <el-form-item label="报表类型">
          <el-select v-model="ruleForm.reportType" :disabled="title==='编辑'? true : false" placeholder="请选择">
            <el-option label="大屏" value="screen"></el-option>
            <el-option label="电子报表" value="table"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="所属分组">
          <el-cascader
              v-model="ruleForm.GroupId"
              :options="roomCatetoryTreeList"
              :props="cascaderProps"
              clearable>
          </el-cascader>
        </el-form-item> 
        <el-form-item v-if="ruleForm.reportType !==''" label="设备类型">
          <el-select v-model="ruleForm.DeviceType" :disabled="title==='编辑'? true : false" placeholder="请选择">
            <el-option label="pc端" value="pc"></el-option>
            <el-option v-if="ruleForm.reportType ==='screen'" label="双端" value="double"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="报表简介">
          <el-input v-model="ruleForm.DesInfo" type="textarea" placeholder="请输入内容"></el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
import { addRpt, editRpt, treeSelectList } from "@/api/report/report";
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
      roomCatetoryTreeList: [],
      list: [],
      cascaderProps: {
        checkStrictly: true,
        value: 'Id',
        label: 'Name',
        children: 'Children'
      },
      // 校验
      rules: {
        Name: [
          { required: true, message: "名称不能为空", trigger: "blur" },
        ]
      }
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue
      this.getCatetoryList();
    }
  },
  methods: {
    //获取设备分组列表
    getCatetoryList() {
      treeSelectList().then(res => {
        this.roomCatetoryTreeList = JSON.parse(JSON.stringify(res.data));
      })
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          if (this.title === '新增') {
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
        this.ruleForm.GroupId = this.ruleForm.GroupId === '' ? '' : this.ruleForm.GroupId[this.ruleForm.GroupId.length - 1];
        addRpt(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
    },
    setEditRpt() {
        this.ruleForm.GroupId = this.ruleForm.GroupId === '' ? '' : this.ruleForm.GroupId[this.ruleForm.GroupId.length - 1];
        editRpt(this.ruleForm).then(res => {
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