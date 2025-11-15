<template>
  <el-form v-loading="emailLoading" ref="form" :model="bindEmail" :rules="rules" label-width="80px">
    <el-form-item label="邮箱" prop="email">
      <el-input
        v-model="bindEmail.email"
        style="width:340px"
        placeholder="请输入邮箱"
        :disabled="isDisabled"
      />
    </el-form-item>
    <!-- <el-form-item label="邮箱" prop="Email">
      <el-input v-model="user.Email" maxlength="50" />
    </el-form-item>-->
    <el-form-item prop="emailCode" label="验证码">
      <div class="mobile_code_con">
        <el-input
          v-model="bindEmail.emailCode"
          auto-complete="off"
          placeholder="请输入验证码"
          style="width:340px"
        >
          <!-- <svg-icon slot="prefix" icon-class="validCode" class="el-input__icon input-icon" /> -->
        </el-input>
        <div :class="{'invite_register-code':true,'count_down':codeState!='normal'}">
          <!-- <img :src="codeUrl" @click="getCode" class="invite_register-code-img" /> -->
          <span v-if="codeState=='normal'" @click="getEmailCode">获取验证码</span>
          <span v-else>{{'重新发送('+currSecord+'s)'}}</span>
        </div>
      </div>
    </el-form-item>
    <el-form-item style="margin-top:35px">
      <el-button type="primary" class="saveInfoBtn" @click="submit">绑定</el-button>
      <el-button class="closeInfoBtn" @click="close">关闭</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import { updateUserProfile } from "@/api/system/user";
import { sendEmailCode, bindEmail } from "@/api/system/Employee";
export default {
  props: {
    user: {
      type: Object
    }
  },
  data() {
    return {
      emailLoading:false,
      // 表单校验
      rules: {
        email: [
          { required: true, message: "邮箱地址不能为空", trigger: "blur" },
          {
            type: "email",
            message: "请输入正确的邮箱地址",
            trigger: ["blur", "change"]
          }
        ],
        emailCode: [
          { required: true, message: "验证码不能为空", trigger: "blur" }
        ]
      },
      bindEmail: {
        email: null,
        emailCode: ""
      }, //绑定手机号的参数
      // 验证码开关
      //   captchaOnOff: true,
      currSecord: 0, //验证码倒计时
      codeState: "normal", //是否重新发送验证码
      isDisabled: false //是否禁用
    };
  },
  computed: {},
  mounted() {
    //   this.bindPhone.mobile=this.user.Mobile
  },
  // updated() {
  //   this.$set(this.bindEmail, "email", this.user.Email);
  // },
  methods: {
    setBindEmail(val) {
      // console.log(val.Email, "val.Emailuuuuuuu");

      this.$set(this.bindEmail, "email", val.Email);
    },
    async getEmailCode() {
      this.emailLoading=true;
      //获取邮箱验证码
      if (this.bindEmail.email) {
        //传邮箱就可以获取验证码
        try {
          let res = await sendEmailCode({
            email: this.bindEmail.email
          });
          this.emailLoading=false;
          if (res.code == 0) {
            this.currSecord = 120;
            this.codeState = "wait";
            this.beginInterval();
            this.isDisabled = true;
            this.$forceUpdate();
          }
        } catch (error) {
          this.emailLoading=false;
          console.log("error", error);
        }
      } else {
        this.emailLoading=false;
        this.$message.error("邮箱不能为空");
        return
      }
    },
    beginInterval() {
      //验证码重新发送倒计时
      this.currSecord = this.currSecord - 1;
      if (this.currSecord <= 0) {
        this.codeState = "normal";
      } else {
        setTimeout(this.beginInterval, 1000);
      }
    },
    submit() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          this.emailLoading = true;
          bindEmail({
            note:this.bindEmail.emailCode
          }).then(response => {
            this.emailLoading=false;
            this.$modal.msgSuccess("修改成功");
          }).catch(err=>{
            this.emailLoading=false;
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
.closeInfoBtn {
  background-color: #ffffff;
  border: 1px solid #dfe2ea;
  width: 130px;
  height: 40px;
}
.invite_register-code {
cursor: pointer;
  width: 115px;
  height: 28px;
  border-radius: 14px;
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  text-align: center;
  line-height: 28px;
  color: #ffffff;
  overflow: hidden;
  //   position: absolute;
  //   left: 240px;
  //   top: 3px;
  margin-left: 12px;
  span {
    display: block;
    width: 100%;
    height: 100%;
    text-align: center;
  }
  img {
    cursor: pointer;
    vertical-align: middle;
  }
}
.count_down {
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  padding: 0 5px;
  font-size: 14px;
}
.mobile_code_con {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}
.img_code_con {
  .el-form-item__content {
    display: flex;
    justify-content: flex-start;
    align-items: center;
  }
  .login-code {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 40px;
    margin: 0;
    padding: 0;
    margin-left: 12px;
  }
}
</style>