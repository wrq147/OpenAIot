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
        <el-input v-model="sourceForm.VideoKey" :readonly="true" />
      </el-form-item>
      <el-form-item v-if="sourceForm.VideoType == 0" label="拉流地址" prop="PullAddr">
        <el-input v-model="sourceForm.PullAddr" placeholder="请输入拉流地址" />
      </el-form-item>
      <el-form-item v-if="sourceForm.VideoType == 1" label="注册用户名" prop="UserName">
        <el-input v-model="sourceForm.UserName" placeholder="请输入注册用户名" />
      </el-form-item>
      <el-form-item v-if="sourceForm.VideoType == 1" label="注册密码" prop="UserPwd">
        <el-input v-model="sourceForm.UserPwd" placeholder="请输入注册密码" />
      </el-form-item>
      <el-form-item v-if="sourceForm.VideoType == 1" label="码流类型">
        <el-radio-group v-model="sourceForm.BitType">
          <el-radio :label="0">主码流</el-radio>
          <el-radio :label="1">子码流</el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="cancel">取消</el-button>
      <el-button type="primary" @click="submitForm('sourceForm')">确定</el-button>
    </span>
  </el-dialog>
</template>
<script>
import { addVideoSource, editVideoSource, getVideoDetail } from "@/api/rules/video";
export default {
  data() {
    return {
      dialogFlag: false,
      // 表单
      sourceForm: {},
      title: '',
      // 校验
      rules: {
        Position: [
          { required: true, message: "请输入视频源位置", trigger: "blur" },
        ]
      },
      formId: null
    }
  },
  methods: {
    showDlg(tid) {
      this.formId = tid;
      this.dialogFlag = true;
      if (this.formId == null) {
        this.title = "新增视频源";
        this.sourceForm = {
          Id: null,
          Position: '',
          VideoType: 0,
          VideoKey: '',
          PullAddr: '',
          UserName: '',
          UserPwd: '',
          BitType: 0
        };
      } else {
        this.title = "编辑视频源";
        getVideoDetail({ id: tid }).then((res) => {
          this.sourceForm = {
            id: tid,
            Position: res.data.Position,
            VideoType: res.data.VideoType,
            VideoKey: res.data.VideoKey,
            PullAddr: res.data.PullAddr,
            UserName: res.data.UserName,
            UserPwd: res.data.UserPwd,
            BitType: res.data.BitType
          };
        })
      }
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          if (this.formId == null) {
            addVideoSource(this.sourceForm);
          } else {
            editVideoSource(this.sourceForm);
          }
          this.$message.success('操作成功!')
        } else {
          return false
        }
      })
    },
    cancel() {
      this.dialogFlag = false;
    }
  }
}
</script>

<style lang="scss" scoped></style>