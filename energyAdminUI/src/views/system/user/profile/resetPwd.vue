<template>
  <el-form ref="form" :model="user" :rules="rules" label-width="80px" label-position="top">
    <el-form-item label="旧密码" prop="oldPassword">
      <el-input style="width:100%" v-model="user.oldPassword" placeholder="请输入旧密码" type="password" show-password/>
    </el-form-item>
    <el-form-item label="新密码" prop="newPassword">
      <el-input v-model="user.newPassword" placeholder="请输入新密码" type="password" show-password/>
    </el-form-item>
    <el-form-item label="确认密码" prop="confirmPassword">
      <el-input v-model="user.confirmPassword" placeholder="请确认密码" type="password" show-password/>
    </el-form-item>
    <div style="display:flex;justify-content: flex-end;margin-top:40px">
      <el-button class="closeInfoBtn" @click="close">取消</el-button>
      <el-button type="primary" class="saveInfoBtn" @click="submit">保存</el-button>
    </div>
  </el-form>
</template>

<script>
import { updateUserPwd } from "@/api/system/user";

export default {
  data() {
    const equalToPassword = (rule, value, callback) => {
      if (this.user.newPassword !== value) {
        callback(new Error("两次输入的密码不一致"));
      } else {
        callback();
      }
    };
    return {
      test: "1test",
      user: {
        oldPassword: undefined,
        newPassword: undefined,
        confirmPassword: undefined
      },
      // 表单校验
      rules: {
        oldPassword: [
          { required: true, message: "旧密码不能为空", trigger: "blur" }
        ],
        newPassword: [
          { required: true, message: "新密码不能为空", trigger: "blur" },
          { min: 6, max: 20, message: "长度在 6 到 20 个字符", trigger: "blur" }
        ],
        confirmPassword: [
          { required: true, message: "确认密码不能为空", trigger: "blur" },
          { required: true, validator: equalToPassword, trigger: "blur" }
        ]
      }
    };
  },
  methods: {
    submit() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          updateUserPwd(this.user.oldPassword, this.user.newPassword).then(
            response => {
              this.$modal.msgSuccess("修改成功");
              this.$emit('successDialog')
            }
          );
        }
      });
    },
    close() {
      this.$emit('closeDialog')
    }
  }
};
</script>
<style lang="less" scoped>
.saveInfoBtn {
  background: rgba(61, 185, 143, 1);
  border: none;
  width: 80px;
  height: 36px;
  color: rgba(255, 255, 255, 1);
}
.closeInfoBtn {
  background-color: rgba(34, 46, 64, 1);
  border: none;
  width: 80px;
  height: 36px;
  color: rgba(255, 255, 255, 0.6);
  &:focus, &:hover{
    background: rgba(34, 46, 64, 1);
    color: rgba(255, 255, 255, 0.6);
    border: none;
  }
}
</style>