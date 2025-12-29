<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
      width="700px" top="2vh">
      <el-form :model="sourceForm" :rules="rules" label-width="120px">
        <el-form-item label="视频源位置" prop="Position">
          <el-input v-model="sourceForm.Position" placeholder="请输入视频源位置" />
        </el-form-item>
        <el-form-item label="视频源类型">
            <el-radio-group v-model="sourceForm.VideoType">
                <el-radio :label="0">固定地址</el-radio>
                <el-radio :label="1">GB28181</el-radio>
            </el-radio-group>
        </el-form-item>
        <el-form-item label="视频Key" prop="VideoKey">
          <el-input v-model="sourceForm.VideoKey" :readonly="true"/>
        </el-form-item>
        <el-form-item v-if="sourceForm.VideoType==0" label="拉流地址" prop="PullAddr">
          <el-input v-model="sourceForm.PullAddr" placeholder="请输入拉流地址"/>
        </el-form-item>
        <el-form-item v-if="sourceForm.VideoType==0" label="AI检测帧间隔" prop="FrameInterval">
          <el-input-number v-model="sourceForm.FrameInterval" :min="1" :max="999"></el-input-number>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
  import { addVideoSource, editVideoSource } from "@/api/rules/video";
  export default {
    props: {
      title: {
        type: String
      }
    },
    data() {
      return {
        dialogFlag: false,
        // 表单
        sourceForm: {},
        // 校验
        rules: {
          Position: [
            { required: true, message: "请输入视频源位置", trigger: "blur" },
          ]
        }
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
      }
    }
  }
</script>
  
<style lang="scss" scoped>
</style>