<template>
  <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
    width="700px" top="2vh" @close="cancel">
    <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
      <el-form-item label="批次编号" prop="Number">
        <el-input v-model="ruleForm.Number" placeholder="请输入批次编号" />
      </el-form-item>
      <el-form-item label="关联编号" prop="LNumber">
        <el-input v-model="ruleForm.LNumber" placeholder="请输入关联编号" />
      </el-form-item>
      <el-form-item label="所属产品" prop="ProductId">
        <el-button type="primary" size="mini" plain @click="openDeviceDialog">
          <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
          <span style="margin-left: 6px">选择产品</span>
        </el-button>
        <el-tag v-if="ruleForm.ProductName !== ''" style="margin-left: 10px">{{ ruleForm.ProductName }}</el-tag>
      </el-form-item>
    </el-form>
    <slectProductList :dialog-visible="deviceOpen" @cancelForm="cancelForm" @productSelect="productSelect" />
    <span slot="footer" class="dialog-footer">
      <el-button @click="cancel">取消</el-button>
      <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { addBatch, editBatch } from "@/api/manufac/batchList";
import slectProductList from './slectProductList.vue'
export default {
  name: 'addBatch',
  components: {
    slectProductList
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
      // 表单
      ruleForm: {},
      // 校验
      rules: {
        Number: [
          { required: true, message: "唯一编号不能为空", trigger: "blur" },
        ],
        LNumber: [
          { required: true, message: "其它编号不能为空", trigger: "blur" },
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
          if (this.title === '新增批次') {
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
      addBatch(this.ruleForm).then(res => {
        this.$message.success('添加成功!')
        this.$emit('getList')
      })
    },
    setEditRpt() {
      editBatch(this.ruleForm).then(res => {
        this.$message.success('修改成功!')
        this.$emit('getList')
      })
    },
    productSelect(data) {
      // console.log("选择产品",data);
      this.ruleForm.ProductId = data.Id;
      this.ruleForm.ProductName = data.ProductName;
    },
    openDeviceDialog() {
      this.deviceOpen = true;
    },
    cancelForm() {
      this.deviceOpen = false;
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