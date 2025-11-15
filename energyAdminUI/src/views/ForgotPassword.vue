<template>
  <div class="login">
    <div class="logo-heard">
      <img src="../assets/images/name_logo.png" alt="" />
    </div>
    <el-form
      ref="loginForm"
      :model="loginForm"
      :rules="loginRules"
      label-position="top"
      class="login-form"
    >
      <div class="form_con jiayuform_con">
        <div class="login-title">
          重置密码
          <div class="login-titleChild">登录成功，请设置新密码！</div>
        </div>
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            :show-password="true"
            :clearable="true"
            type="password"
            auto-complete="off"
            placeholder="请输入密码"
            @keyup.enter.native="handleLogin"
          >
            <i
              slot="prefix"
              class="zhongtaiiconfont zhongtai-icon-mima"
              :style="{ color: 'rgba(255, 255, 255, 0.30)' }"
            ></i>
          </el-input>
        </el-form-item>
        <el-form-item prop="newPassword">
          <el-input
            v-model="loginForm.newPassword"
            :show-password="true"
            :clearable="true"
            type="password"
            auto-complete="off"
            placeholder="请输入确认密码"
            @keyup.enter.native="handleLogin"
          >
            <i
              slot="prefix"
              class="zhongtaiiconfont zhongtai-icon-mima"
              :style="{ color: 'rgba(255, 255, 255, 0.30)' }"
            ></i>
          </el-input>
        </el-form-item>
        <el-form-item style="width: 100%">
          <el-button
            :loading="loading"
            size="medium"
            type="primary"
            style="width: 100%"
            @click.native.prevent="handleLogin"
          >
            <span v-if="!loading">确 定</span>
            <span v-else>提 交 中...</span>
          </el-button>
        </el-form-item>
      </div>
    </el-form>
  </div>
</template>

<script>
import { forgetNewPass } from "@/api/system/Employee";
import { getCodeImg, sendEmailCode } from "@/api/login";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "@/utils/jsencrypt";
import { getConfigKey } from "@/api/system/config.js";
export default {
  name: "Login",
  data() {
    return {
      activeLoading: false,
      activeEmailText: "",
      activeDialogVisible: false,
      activeDaoji: false,
      daojiNum: 0,
      codeUrl: "",
      cookiePassword: "",
      loginForm: {
        username: "",
        password: "",
        rememberMe: false,
        code: "",
        uuid: "",
        newPassword: "",
      },
      loginRules: {
        newPassword: [
          { required: true, trigger: "blur", message: "请确认您的密码" },
        ],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
      },
      loading: false,
      // 验证码开关
      captchaOnOff: true,
      redirect: undefined,
      loginType: "default",
    };
  },
  watch: {
    $route: {
      handler: function (route) {
        this.redirect = route.query && route.query.redirect;
      },
      immediate: true,
    },
  },
  created() {
    this.getCode();
    this.getCookie();
  },
  methods: {
    changeRemember(val) {
      if (val) {
        Cookies.set("username", this.loginForm.username, { expires: 30 });
        Cookies.set("password", encrypt(this.loginForm.password), {
          expires: 30,
        });
        Cookies.set("rememberMe", this.loginForm.rememberMe, { expires: 30 });
      } else {
        Cookies.remove("username");
        Cookies.remove("password");
        Cookies.remove("rememberMe");
      }
    },
    getCode() {
      getCodeImg().then((res) => {
        this.captchaOnOff =
          res.data.captchaOnOff === undefined ? true : res.data.captchaOnOff;
        if (this.captchaOnOff) {
          this.codeUrl = "data:image/gif;base64," + res.data.img;
          this.loginForm.uuid = res.data.uuid;
        }
      });
    },
    getCookie() {
      const username = Cookies.get("username");
      const password = Cookies.get("password");
      const rememberMe = Cookies.get("rememberMe");
      this.loginForm = {
        username: username === undefined ? this.loginForm.username : username,
        password:
          password === undefined ? this.loginForm.password : decrypt(password),
        rememberMe: rememberMe === undefined ? false : Boolean(rememberMe),
      };
    },
    reSendEmail() {
      //重新发送激活邮件
      this.activeLoading = true;
      sendEmailCode({
        email: this.loginForm.username,
      })
        .then((res) => {
          this.activeLoading = false;
          this.activeDaoji = true;
          this.daojiNum = 120;
          let timer = setInterval(() => {
            if (this.daojiNum > 0) {
              this.daojiNum--;
            } else {
              clearInterval(timer);
              this.activeDaoji = false;
            }
          }, 1000);
        })
        .catch((err) => {
          this.activeLoading = false;
          console.log(err, "发送邮箱失败");
        });
    },
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          if (this.loginForm.password != this.loginForm.newPassword) {
            this.$message.warning("两次密码输入不一致！");
            return;
          }
          this.loading = true;
          forgetNewPass({
            newPassword: this.loginForm.newPassword,
            code: this.$route.query.UpdatePasswordCode,
          })
            .then((res) => {
              if (res.code == 0) {
                this.$message.success("修改成功！");
                if (this.$route.query && this.$route.query.loginJumpUrl) {
                  window.location.replace(this.$route.query.loginJumpUrl); //切换企业后刷新整个网站
                } else {
                  this.$router.push("/index");
                }
              }
            })
            .catch((err) => {
              this.$message.warning(err);
              this.loading = false;
            });
        }
      });
    },
  },
};
</script>

<style lang="scss" scoped>
.login {
  height: 100vh;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  overflow: hidden;
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  .logo-heard {
    // margin:30px 0 0 30px ;
    width: 360px;
    height: 46px;
    position: absolute;
    left: 40px;
    top: 42px;
    img {
      width: 100%;
      height: 100%;
    }
  }
}
$themes: (
  //动态设置样式
  "default":
    (
      --formlabelcolor: #999999,
      --formconbg: rgba(255, 255, 255, 1),
      --formcontentcolor: rgba(120, 130, 157, 1),
      --forminputbg: rgba(19, 25, 34, 1),
      --forminputcontentcolor: rgba(255, 255, 255, 1),
      --inputHeight: 48px,
      --opacity: 0.5,
      --inputfontsize: 14px,
      --labelfontsize: 18px,
    )
);

// 主题应用混合器（设置 CSS 变量）
@mixin apply-theme($theme-name) {
  $theme: map-get($themes, $theme-name);
  @if $theme {
    // 将主题值映射到 CSS 变量
    @each $var, $value in $theme {
      #{$var}: $value;
    }
  }
}

.login-form {
  width: 75%;
  height: 820px;
  // 应用默认主题
  @include apply-theme("default");
  display: flex;
  // flex-direction: column;
  align-items: center;
  justify-content: flex-end;
  background-image: url("../assets/images/nenghao_bg.png");
  background-size: cover;

  .form_con {
    width: 440px;
    // margin-top: 140px;
    height: 620px;
    background: rgba(25, 33, 45, 1);
    border-radius: 10px;
    padding: 60px 40px;
    box-sizing: border-box;
    .login-title {
      font-weight: 600;
      font-size: 28px;
      line-height: 28px;
      color: rgba(255, 255, 255, 1);
      margin-bottom: 30px;
      .login-titleChild {
        font-size: 16px;
        color: rgba(255, 255, 255, 0.6);
        padding-top: 20px;
      }
    }
    ::v-deep .el-form-item {
      margin-bottom: 16px !important;
    }
    ::v-deep .el-input {
      height: var(--inputHeight);
      border-radius: 6px;
      background-color: var(--forminputbg);
      input {
        height: var(--inputHeight);
        background-color: rgba(19, 25, 34, 1);
        color: var(--forminputcontentcolor);
        border: 1px solid rgba(19, 25, 34, 1);
        border-radius: 6px;
        font-size: var(--inputfontsize);
      }
    }
    ::v-deep .el-input__prefix {
      color: rgba(255, 255, 255, 0.3);
      font-size: 14px;
      line-height: 48px;
      left: 16px;
    }
    ::v-deep .el-input--prefix .el-input__inner {
      padding-left: 40px;
    }
    ::v-deep .el-button {
      height: 52px;
      border-radius: 4px;
      background: rgba(61, 185, 143, 1);
      font-size: 16px;
      border: 0;
      margin-top: 60px;
    }
    ::v-deep .el-input__inner:focus {
      border: 1px solid rgba(61, 185, 143, 1);
    }
    .input-icon {
      height: 39px;
      width: 14px;
      margin-left: 2px;
    }
  }
}
.login-tip {
  font-size: 13px;
  text-align: center;
  color: #bfbfbf;
}
.el-login-footer {
  height: 40px;
  line-height: 40px;
  position: fixed;
  bottom: 0;
  width: 100%;
  text-align: right;
  box-sizing: border-box;
  padding-right: 16.3%;
  color: #fff;
  font-family: Arial;
  font-size: 12px;
  letter-spacing: 1px;
  &.jiayu_footer {
    color: #999999;
  }
}
</style>
