<template>
  <div class="login">
    <div class="logo-heard">
      <img src="../assets/images/logo.png" alt="">
    </div>
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form">
      <div class="form_con">
        <div class="login-title">登录</div>
          <div class="LoginMethod">
            <div class="LoginMethod-router" v-if="$route.query.forgetPass==100&&!codeLogin" style="text-align:center;font-size:18px;color:#ffffff">
              <span>邮箱验证</span>
            </div>
           <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100">
            <router-link :to="'/login'" class="routerLink"> 账号登录</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="$route.query.forgetPass==100?'/loginInterface?forgetPass=100':'/loginInterface'" class="routerLink">手机号验证</router-link>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100||$route.query.forgetPass==100&&codeLogin">
            <router-link :to="$route.query.forgetPass==100?'/emailLogin?forgetPass=100':'/emailLogin'" class="routerLink activeRouterLink">邮箱登录</router-link>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100&&corpLogin">
            <router-link :to="'/wxworkLogin'" class="routerLink">企业微信</router-link>
          </div> 
        </div> 
        <el-form-item prop="email">
          <el-input v-model="loginForm.email" type="text" auto-complete="off" :clearable="true" placeholder="请输入邮箱">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhanghao" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
          </el-input>
        </el-form-item>
        <el-form-item prop="code">
          <el-input v-model="loginForm.code" type="text" auto-complete="off" placeholder="请输入验证码" :style="{width: '100%'}">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
            <div slot="suffix" class="input_suffix_con">
              <div class="input_suffix_line"></div>
              <el-button class="input_suffix_button"  :class="disabled ? 'disabled-button' : ''" :disabled="disabled" @click="handGetCode">
                {{ getCodeText }}
              </el-button>
            </div>
          </el-input>
        </el-form-item>
        <el-form-item prop="imgCode" v-if="hideCode">
          <el-input v-model="loginForm.imgCode" auto-complete="off" placeholder="请输入图形验证码" :style="{width: '50%'}" @input="handInput">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
          </el-input>
          <div class="login-code">
            <img :src="imgImg" @click="handImgCode" class="login-code-img" />
          </div>
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
$themes: (//动态设置样式
  'default': (
    --formlabelcolor: #999999,
    --formconbg: rgba(255, 255, 255, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: #FAFAFA,
    --forminputcontentcolor: #000,
    --inputHeight: 44px,
    --opacity:0.5,
    --inputfontsize:14px,
    --labelfontsize:18px,
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
  height: 100vh;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  overflow: hidden;
  position: relative;
  .logo-heard{
    margin:30px 0 0 30px ;
    width: 360px;
    height: 46px;
    img{
      width: 100%;
      height: 100%;
    }
  }


  .login-form {
    width: 36.11%;
    height: 100%;
    position: absolute;
    right: 0;
    top: 0;
    // 应用默认主题
    @include apply-theme('default');
    background: var(--formconbg);
    display: flex;
    flex-direction: column;
    align-items: center;
    
    .form_con {
      width: 76.92%;
      margin-top: 140px;
      .login-title{
        font-weight: 600;
        font-size: 28px;
        color: #333333;
        margin-bottom: 30px;
      }
      .login-accountBox{
        display: flex;
        align-items: center;
      }
      .login-account{
        width: 100%;
        display: flex;
        align-items: center;
        background: #FAFAFA;
        height: 48px;
        img{
          width: 14px;
          height: 14px;
          margin-left: 12px;
        }
      }
      .login-code-img {
        width: 148px;
        height: 48px;
        cursor: pointer;
        margin-left: 10px;
      }
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
        border-radius: 0;
        background-color: var(--forminputbg);
        input {
          height: var(--inputHeight);
          background-color: rgba(32, 46, 87, 0);
          color: var(--forminputcontentcolor);
          border: 0;
          font-size: var(--inputfontsize);
        }
      }
      ::v-deep .el-button {
        height: 52px;
        border-radius: 4px;
        background: linear-gradient( 90deg, #EE7700 0%, #FF9E3D 100%);
        font-size: 16px;
        border: 0;
      }
      ::v-deep .el-input__inner:focus {
        border: 0 !important;
      }
      .input-icon {
        height: 39px;
        width: 14px;
        margin-left: 2px;
      }
      .LoginMethod{
        display: flex;
        font-size: 16px !important;
        margin-bottom: 40px;
        width:100%;
        align-items: center;

        .LoginMethod-router{
          color: #999999;
          padding: 0 10px;
          border-right: 1px solid #999999;
          .routerLink{
            display: block;
            text-align: center;
              
          }
          .activeRouterLink{
            color: #EE7700;
          }
        }
        .LoginMethod-router:first-child{
          padding-left: 0;
        }
        .LoginMethod-router:last-child{
          padding-right: 0;
          border-right: 0;
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
.login-button {
    border: none;
    background: #FAFAFA !important;
    width: 30%;
    height: 48px !important;
    margin-left: 15px;
    color: #EE7700;
}


</style>
