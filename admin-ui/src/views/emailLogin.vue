<template>
  <div class="login">
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form">

      <div class="login_title" v-if="loginType=='default'">
        <h2 class="title">后台管理系统</h2>
      </div>
      <div class="form_con">
          <div class="LoginMethod" v-if="loginType=='default'">
            <div class="LoginMethod-router" v-if="$route.query.forgetPass==100&&!codeLogin" style="text-align:center;font-size:18px;color:#ffffff">
              <span>邮箱验证</span>
            </div>
           <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100">
            <router-link :to="'/login'" class="routerLink"> 密码</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="$route.query.forgetPass==100?'/loginInterface?forgetPass=100':'/loginInterface'" class="routerLink">手机号</router-link>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100||$route.query.forgetPass==100&&codeLogin">
            <router-link :to="$route.query.forgetPass==100?'/emailLogin?forgetPass=100':'/emailLogin'" class="routerLink activeRouterLink">邮箱</router-link>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100&&corpLogin">
            <router-link :to="'/wxworkLogin'" class="routerLink">企业微信</router-link>
          </div> 
        </div> 
        <el-form-item prop="email">
          <div class="form_label_con" slot="label" v-if="loginType=='default'">
            <div class="label_left">
              <img style="width:14px;height:16px;margin-right:12px;" src="../assets/images/userEmail.png" alt=""><span>邮箱</span>
            </div>
          </div>
          <el-input v-model="loginForm.email" type="text" auto-complete="off" placeholder="请输入邮箱">
          </el-input>
        </el-form-item>


        <el-form-item prop="code">
          <span slot="label" style="flex:1;" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" style="margin-right: 14px;color: #C1C1C1"></i>验证码
          </span>
          <el-input v-model="loginForm.code" type="text" auto-complete="off" placeholder="请输入验证码" :style="{width: '65%'}">
          </el-input>
          <el-button class="login-button" :disabled="disabled" @click="handGetCode" v-if="loginType=='default'">
              {{getCodeText}}
          </el-button>
        </el-form-item>
        <el-form-item style="width: 100%;padding-top:30px;">
          <el-button :loading="loading" size="medium" type="primary" style="width: 100%" @click.native.prevent="handleLogin">
            <span v-if="!loading">登 录</span>
            <span v-else>登 录 中...</span>
          </el-button>
        </el-form-item>
      </div>
    </el-form>

    <!--  底部  -->
  </div>
</template>

<script>
import { getUserInfo, sendEmailCode } from "@/api/login";
import {
  visitorEmailCode,
  loginEmails,
  sendImgCode,
} from "@/api/system/Employee";

import { setToken, setRefreshToken } from "@/utils/auth";
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
      loginForm: {
        email: "",
        username: "",
        password: "",
        rememberMe: false,
        code: "",
        uuid: "",
        imgCode: "",
      },
      loginRules: {
        email: [{ required: true, trigger: "blur", message: "请输入您的邮箱" }],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        code: [{ required: true, trigger: "change", message: "请输入验证码" }],
      },
      loading: false,
      // 验证码开关
      captchaOnOff: true,
      // 注册开关
      redirect: undefined,
      getCodeText: "获取验证码",
      disabled: false,
      getCodeBtnColor: "#ffffff",
      getCodeisWaiting: false,
      hideCode: false,
      uuid: "",
      imgImg: "",
      corpLogin:false,//企业微信登录开关
      codeLogin:false,//手机号登录开关
      loginType: 'default',
      yaoqingId:'',//邀请id
    };
  },
  watch: {
    $route: {
      handler: function (route) {
        this.redirect = route.query && route.query.redirect;
        this.yaoqingId = route.query && route.query.yaoqingId;
      },
      immediate: true,
    },
  },
  created() {
    //判断是否显示注册
    //配置是否需要企业微信登录
    getConfigKey("login.corp").then((res) => {
      this.corpLogin = res.data=='false'||res.data==''?true:false;
    });
    getConfigKey("login.code").then((res) => {
      this.codeLogin = res.data=='false'||res.data==''?true:false;
    });
  },
  methods: {
    handImgCode() {
      this.tuxingCode();
    },
    //发送邮箱验证码
    handGetCode() {
      if (!this.loginForm.email) {
        this.$message.warning("请输入邮箱"); //弹出提示框
        return;
      }
      visitorEmailCode({
        email: this.loginForm.email,
      })
        .then((res) => {
          if (res.code == 0) {
            this.disabled = true;
            this.getCodeText = "发送..."; //发送验证码
            this.getCodeisWaiting = true;
            this.getCodeBtnColor = "rgba(255,255,255,0.5)"; //追加样式，修改颜色
            //示例用定时器模拟请求效果
            //setTimeout(()用于在指定的毫秒数后调用函数或计算表达式
            setTimeout(() => {
              this.$message.success("邮件码已发送"); //弹出提示框
              this.setTimer(); //调用定时器方法
            }, 1000);
          }
        })
        .catch((err) => {
          //	if(err.code==113){
          // this.$message.warning(err);//弹出提示框
          this.tuxingCode();
          this.hideCode = true;
          //	}
          //	this.$message.warning(err);//弹出提示框
        });
    },
    setTimer() {
      let holdTime = 60; //定义变量并赋值
      this.getCodeText = "60s";
      //setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
      //setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
      this.Timer = setInterval(() => {
        if (holdTime <= 0) {
          this.disabled = false;
          this.getCodeisWaiting = false;
          this.getCodeBtnColor = "#ffffff";
          this.getCodeText = "重新获取";
          clearInterval(this.Timer); //清除该函数
          return; //返回前面
        }
        this.getCodeText = holdTime + "s";
        holdTime--;
      }, 1000);
    },
    tuxingCode() {
      sendImgCode().then((res) => {
        if (res.code == 0) {
          console.log(res.data, "图形验证码");

          this.uuid = res.data.uuid;
          this.imgImg = res.data.base64;
        }
      });
    },
    handloginInterface() {
      if (this.$route.query.forgetPass == 100) {
        this.$router.push("/loginInterface?forgetPass=100");
      } else {
        this.$router.push("/loginInterface");
      }
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
          this.loading = true;
          loginEmails({
            email: this.loginForm.email,
            code: this.loginForm.code,
          })
            .then(async (res) => {
              if (res.code == 0) {
                this.$modal.msgSuccess("登录成功");
                setToken(res.data.token);
                setRefreshToken(res.data.refresh_token);
                this.$store.commit('SET_ROLES', [])
                if (this.$route.query.forgetPass == 100) {
                  this.$router.push(
                    "/ForgotPassword?UpdatePasswordCode=" +
                      res.data.ext_info.extObj.UpdatePasswordCode
                  );
                  this.loading = false;
                } else {
                  try {
                    this.$nextTick(async()=>{
                      this.$store.commit("orgLis/SET_ORG_LIST", null);
                      let list = await this.$store.dispatch("orgLis/setOrgList");
                      if (list && list.length > 0) {
                        this.$router.push({ path: "/" }).catch(() => {});
                      } else {
                        if (this.$route.query.yaoqingId){
                          this.$router.push("/crm/yaoqing/choose_addorg?yaoqingId=" + this.$route.query.yaoqingId);
                        }else{
                          this.$router.push("/crm/yaoqing/choose_addorg");
                        }
                      }
                    })
                  } catch (error) {
                    console.log(error, "eee");
                  }
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

.active_email_con {
  font-size: 20rpx;
  color: #333333;
  .email_text {
    color: #3572ff;
  }
}
.active_dialog-footer {
  display: flex;
  justify-content: space-between;
  .cancel_btton {
    width: calc(50% - 10px);
    border: 1px solid #dfe2ea;
    background: #f6f9ff;
    color: #78829d;
    &.noDaoji {
      color: #3572ff;
    }
  }
  .confrim_button {
    width: calc(50% - 10px);
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
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
    --inputfontsize:14px,
    --labelfontsize:18px
  ),
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

.login {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  overflow: hidden;

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
      width: 150px;
      height: 50px;
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
    
    .form_con {
      background: var(--formconbg);
      width: 500px;
      height: 568px;
      border-radius: 20px;
      padding: 50px;
      padding-top:1px;
      ::v-deep label.el-form-item__label::before {
        content: "" !important;
      }
      ::v-deep label.el-form-item__label {
        display: flex;
        span {
          font-size: var(--labelfontsize) !important;
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
      }
      ::v-deep .el-input__inner:focus {
        border: 1px solid rgba(53, 114, 255, 1) !important;
      }
      .input-icon {
        height: 39px;
        width: 14px;
        margin-left: 2px;
      }
      .LoginMethod{
        display: flex;
        font-size:16px!important;
        margin:25px 0px;
        width:100%;
        align-items: center;
        justify-content:space-around;
        padding-bottom: 20px;
        text-align: center;
        border-bottom:1px solid rgba(255, 255, 255, .2);

        .LoginMethod-router{
          color:rgba(120, 130, 157, 1);
          width: 33%;
          .routerLink{
            padding:0px 10px;
            padding-bottom: 20px;
              
          }
          .activeRouterLink{
            border-bottom: 3px solid rgba(53, 114, 255, 1);
            color:#fff!important;
          }
        }
        .LoginMethod-router:first-child{
          text-align: left;
        }
        .LoginMethod-router:last-child{
          text-align: right;
        }
      }
    }
    .login-code {
      width: 33%;
      height: 38px;
      float: right;
      img {
        cursor: pointer;
        vertical-align: middle;
      }
    }
    .login-code-img {
      height: 38px;
    }
    .login-button {
      border: none;
      width: 30%;
      height: 48px !important;
      margin-left: 15px;
      color: #fff !important;
    }
    .login-code {
      width: 33%;
      height: 38px;
      float: right;
      img {
        cursor: pointer;
        vertical-align: middle;
      }
    }
    .form_label_con {
      display: inline;
      width: 100%;
      color: #fff;
      font-size: var(--labelfontsize);
      .label_left {
        // display: inline-block;
        float: left;
        display: flex;
        align-items: center;
      }
      .label_right {
        // display: inline-block;
        float: right;
      }
    }
    .form_label_con::after {
      clear: both;
    }
  }
}


</style>
