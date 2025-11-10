<template>
  <div class="login jiayulogin">
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form jiayulogin-form" >
      <div class="login_title jiayulogin_title" v-if="loginType=='default'">
        <h2 class="title">后台管理系统</h2>
      </div>
      <div class="form_con jiayuform_con">
        <div class="login-title">登录成功，请设置新密码！</div>
        <el-form-item prop="password">
          <span slot="label" style="flex: 1" v-if="loginType=='default'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" style="margin-right: 14px" :style="{ color: '#C1C1C1' }"></i>密码
          </span>
          <el-input :show-password="true" :clearable="true" v-model="loginForm.password" type="password" auto-complete="off" placeholder="请输入密码" @keyup.enter.native="handleLogin">
          </el-input>
        </el-form-item>

        <el-form-item prop="newPassword">
          <span slot="label" style="flex: 1" v-if="loginType=='default'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" style="margin-right: 14px" :style="{ color: '#C1C1C1' }"></i>确认密码
          </span>
          <el-input :show-password="true" :clearable="true" v-model="loginForm.newPassword" type="password" auto-complete="off" placeholder="请输入确认密码" @keyup.enter.native="handleLogin">
          </el-input>
        </el-form-item>
        <el-form-item style="width: 100%">
          <el-button :loading="loading" size="medium" type="primary" style="width: 100%;" @click.native.prevent="handleLogin">
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
      loginType: 'default',
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
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  &.jiayulogin {
    background-image: url("../assets/images/jiayu_login_bg.png");
  }

  .login_title {
    width: 100%;
    display: flex;
    justify-content: center;
    // position: fixed;
    // right: 17.2%;
    // top: 16%;
    height: 30px;
    line-height: 30px;
    margin-bottom: 50px;
    margin-left:24px;
    font-size:20px;
    &.jiayulogin_title {
      img.sidebar-logo {
        width: 46px;
        height: 36px;
        margin-right: 10px;
      }
      h2 {
        font-size: 36px;
        line-height: 36px;
        color: rgba(51, 51, 51, 1);
      }
    }
    img.sidebar-logo {
      width: 54px;
      height: 30px;
      margin-right: 20px;
    }
    h2 {
      height: 30px;
      line-height: 30px;
      color: #ffffff;
      margin: 0;
    }
  }
  .login_logo{
    img{
      width: 180px;
      height: 60px;
    }
  }
}
$themes: (//动态设置样式
  'default': (
    --formlabelcolor: rgba(255, 255, 255, 0.8),
    --formconbg: rgba(24, 36, 69, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(32, 46, 87, 1),
    --forminputcontentcolor: rgba(255, 255, 255, 1),
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px
  ),

  'jiayulogin': (//LD项目样式
    --formlabelcolor: rgba(153, 153, 153, 1),
    --formconbg: rgba(255, 255, 255, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(248, 248, 248, 1),
    --forminputcontentcolor: #333333,
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px
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
  width: 500px;
  padding: 25px 25px 5px 25px;
  position: fixed;
  right: 12.3%;
  bottom: 7%;
  // 应用默认主题
  @include apply-theme('default');
  &.jiayulogin-form{
    @include apply-theme('jiayulogin');
    
  }

  
  .form_con {
    background: var(--formconbg);
    width: 500px;
    height: 568px;
    border-radius: 20px;
    padding: 50px;
    padding-top:1px;
    .login-title{
      text-align:center;
      font-size:16px;
      color:#78829D;
      margin-bottom: 30px;
      padding-top: 20px;
    }
    &.jiayuform_con{
      box-shadow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04);
    }
    ::v-deep label.el-form-item__label::before {
      content: "" !important;
    }
    ::v-deep label.el-form-item__label {
      display: flex;
      span {
        font-size: 18px !important;
        color: var(--formlabelcolor) !important;
        font-weight: normal;
        
      }
   
    }
    ::v-deep .el-form-item__content {
      color: var(--formcontentcolor) !important;
      
    }
    .forget_link{
      color:var(--formcontentcolor);
    }
    ::v-deep .el-input {
      height: var(--inputHeight);
      border-radius: 10px;
      background-color: var(--forminputbg);
      input {
        height: var(--inputHeight);
        border-radius: 10px;
        background-color: rgba(32, 46, 87, 0);
        color: var(--forminputcontentcolor);
        border: 1px solid var(--forminputbg);
        opacity: var(--opacity);
        font-size: var(--inputfontsize);
      }
      
      
    }
    ::v-deep .el-button {
      height: 52px;
      border-radius: 4px;
      background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
      font-size: 16px;
      margin-top: 50px;
    }
    ::v-deep .el-input__inner:focus {
      border: 1px solid rgba(53, 114, 255, 1) !important;
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
