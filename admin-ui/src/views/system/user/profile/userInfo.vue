<template>
  <el-form ref="form" :model="user" :rules="rules" label-width="80px">
    <el-form-item label="用户昵称" prop="RealName">
      <el-input v-model="user.RealName" maxlength="30" style="width:320px;" />
    </el-form-item>
    <!-- <el-form-item label="手机号码" prop="Mobile">
      <el-input v-model="user.Mobile" maxlength="11" />
    </el-form-item>
    <el-form-item label="邮箱" prop="Email">
      <el-input v-model="user.Email" maxlength="50" />
    </el-form-item> -->
    <el-form-item label="性别">
      <el-radio-group v-model="user.Sex">
        <el-radio label="0">男</el-radio>
        <el-radio label="1">女</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" class="saveInfoBtn" @click="submit">保存</el-button>
      <el-button  class="closeInfoBtn" @click="close">关闭</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import { updateUserProfile } from "@/api/system/user";

export default {
  props: {
    user: {
      type: Object
    }
  },
  data() {
    return {
      // 表单校验
      rules: {
        RealName: [
          { required: true, message: "用户昵称不能为空", trigger: "blur" }
        ],
        Email: [
          { required: true, message: "邮箱地址不能为空", trigger: "blur" },
          {
            type: "email",
            message: "'请输入正确的邮箱地址",
            trigger: ["blur", "change"]
          }
        ],
        Mobile: [
          { required: true, message: "手机号码不能为空", trigger: "blur" },
          {
            pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
            message: "请输入正确的手机号码",
            trigger: "blur"
          }
        ]
      }
    };
  },
  methods: {
    submit() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          updateUserProfile(this.user).then(response => {
            this.$modal.msgSuccess("修改成功");
          });
        }
      });
    },
    close() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/index" });
    }
  }
};
</script>
<style lang="scss" scope>
.saveInfoBtn {
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  border: none;
  width: 130px;
  height: 40px;
}
.closeInfoBtn{
  background-color: #ffffff;
  border: 1px solid #DFE2EA;
  width: 130px;
  height: 40px;
}
</style>