<template>
  <div class="login">
    <div class="logo-heard">
      <img src="../assets/images/name_logo.png" alt="">
    </div>
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form" >
      <div class=""></div>
      <div class="form_con">
        <div class="login-title">登录</div>
        <div class="LoginMethod" :style="{'border-bottom':!codeLogin&&!emailLogin&&!corpLogin?'none':''}">
        
          <div class="LoginMethod-router" v-if="codeLogin||corpLogin">
            <router-link :to="'/login'" class="routerLink activeRouterLink">账号登录</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="'/loginInterface'" class="routerLink">手机号登录</router-link>
          </div>
          <!-- <div class="LoginMethod-router" v-if="emailLogin">
            <router-link :to="'/emailLogin'" class="routerLink">邮箱登录</router-link>
          </div> -->
          <div class="LoginMethod-router" v-if="corpLogin">
            <router-link :to="'/wxworkLogin'" class="routerLink">企业微信</router-link>
          </div> 
        </div>
        <el-form-item prop="username">
          <el-input v-model="loginForm.username" type="text" auto-complete="off" :clearable="true" :placeholder="loginType=='default'?'请输入账号、邮箱或手机':'请输入账号或手机'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhanghao" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="loginForm.password" :show-password="true" :clearable="true" type="password" auto-complete="off" placeholder="请输入密码" @keyup.enter.native="handleLogin">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
          </el-input>
          
        </el-form-item>
        <el-form-item prop="code" v-if="captchaOnOff">
          <el-input v-model="loginForm.code" auto-complete="off" placeholder="请输入验证码" :style="{width: '50%'}" @keyup.enter.native="handleLogin">
             <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
          </el-input>
          <div class="login-code">
            <img :src="codeUrl" @click="getCode" class="login-code-img" />
          </div>
        </el-form-item>
        <div class="login-check">
          <el-checkbox v-model="loginForm.rememberMe" :style="{'color':'#999999'}" @change="changeRemember">记住密码</el-checkbox>
         
          <label  slot="label" v-if="loginType=='default'&&codeLogin||loginType=='default'&&emailLogin">
            <router-link class="forget_link" :to="'/loginInterface?forgetPass=100'" v-if="codeLogin">
              <span >忘记密码？</span>
            </router-link>
            <router-link class="forget_link" :to="'/emailLogin?forgetPass=100'" v-if="!codeLogin&&emailLogin">
              <span >忘记密码？</span>
            </router-link>
          </label>
        </div>
        <el-form-item style="width: 100%">
          <el-button :loading="loading" size="medium" type="primary" style="width: 100%" @click.native.prevent="handleLogin">
            <span v-if="!loading">登 录</span>
            <span v-else>登 录 中...</span>
          </el-button>
        </el-form-item>
         <!-- <div class="login-register">
            <router-link :to="'/InviteRegister'"><span>没有账号？</span>立即注册</router-link>
         </div> -->
      </div>
    </el-form>
    <el-dialog title="账号激活" :visible.sync="activeDialogVisible" width="590px" v-loading="activeLoading">
      <div class="active_email_con">
        请登录邮箱
        <span class="email_text">{{ activeEmailText }}</span> 激活账号
      </div>
      <span slot="footer" class="active_dialog-footer">
        <el-button class="cancel_btton" v-if="activeDaoji">重新发送激活邮件 {{ daojiNum }}s</el-button>
        <el-button class="cancel_btton noDaoji" v-if="!activeDaoji" @click="reSendEmail">重新发送激活邮件</el-button>
        <el-button class="confrim_button" type="primary" @click="confirm">已激活</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { getCodeImg, getUserInfo, sendEmailCode } from "@/api/login";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "@/utils/jsencrypt";
import { getConfigKeyList } from "@/api/system/config.js";
import { setToken, setRefreshToken } from "@/utils/auth";
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
        rememberMe: true,
        code: "",
        uuid: "",
      },
      loginRules: {
        username: [
          { required: true, trigger: "blur", message: "请输入您的账号" },
        ],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        code: [{ required: true, trigger: "change", message: "请输入验证码" }],
      },
      loading: false,
      // 验证码开关
      captchaOnOff: false,
      // 注册开关
      register: false,
      corpLogin:false,//企业微信登录开关
      codeLogin:false,//验证码登录开关
      emailLogin:false,//邮箱登录开关
      redirect: undefined,
      activeName: 'second',
      loginType: 'default',
      hasQuery:false
    };
  },
  watch: {
    $route: {
      handler: function (route) {
        // console.log(route,);
        if(route.query && route.query.redirect&&route.query.redirect.indexOf('/crm/yaoqing/choose')>-1){
          this.hasQuery=true
          let arr=route.query.redirect.split("?")
          this.redirect='/crm/yaoqing/hailogin?'+arr[1]
        }else{
          this.redirect = route.query && route.query.redirect;
        }
       
      },
      immediate: true,
    },
  },
  created() {
    //判断是否显示注册\是否短信登录\是否企业微信登录、是否邮箱登录
    getConfigKeyList(["sys.account.registerUser","login.code","login.corp","login.email"]).then((res) => {
      let tmpitem = res.data.find(x=>x.name=="sys.account.registerUser");
      this.register = tmpitem==null||tmpitem.value=='false'?false:true;
      tmpitem = res.data.find(x=>x.name=="login.code");
      this.codeLogin = tmpitem==null||tmpitem.value=='false'?true:false;
      tmpitem = res.data.find(x=>x.name=="login.corp");
      this.corpLogin = tmpitem==null||tmpitem.value=='false'?true:false;
      tmpitem = res.data.find(x=>x.name=="login.email");
      this.emailLogin = tmpitem==null||tmpitem.value=='false'?true:false;
    });
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
        this.captchaOnOff =res.data.captchaOnOff === undefined ? true : res.data.captchaOnOff;
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
          // console.log(err, "发送邮箱失败");
        });
    },
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if(this.loginForm.rememberMe){
          Cookies.set("username", this.loginForm.username, { expires: 30 });
          Cookies.set("password", encrypt(this.loginForm.password), { expires: 30 });
          Cookies.set("rememberMe", this.loginForm.rememberMe, { expires: 30 });
        }else{
          Cookies.remove("username");
          Cookies.remove("password");
          Cookies.remove("rememberMe");
        }
        if (valid) {
          this.loading = true;

          this.$store
            .dispatch("Login", this.loginForm)
            .then(async () => {
              this.$store.commit('SET_ROLES', [])
              this.$nextTick(async()=>{
                this.$store.commit("orgLis/SET_ORG_LIST", null);
                let list = await this.$store.dispatch("orgLis/setOrgList");
                if (list && list.length > 0||this.hasQuery) {
                  this.$router.push({ path: this.redirect || "/" }).catch(() => {});
                } else {
                  this.$router.push("/crm/yaoqing/choose_addorg");
                }
              })
            })
            .catch(async (e) => {
              // console.log(e, "登录报错信息");
              this.loading = false;
              if (e.code == 6) {
                //跳转至邮箱注册
                setToken(e.data.token);
                setRefreshToken(e.data.refresh_token);
                this.$store.commit('SET_ROLES', [])
                if (this.loginForm.username.indexOf("@") > 0) {
                  let rsp = await getUserInfo({
                    id: 0,
                  });
                  let infoRsp = rsp.data.user;
                  if (!infoRsp.EmailActive) {
                    this.activeEmailText = this.loginForm.username;
                    this.activeDialogVisible = true;
                    this.reSendEmail();
                  }
                  return;
                }
              } else {
                if(e.code==888){
                  this.captchaOnOff=true
                  this.getCode();
                }else if (this.captchaOnOff) {
                  this.getCode();
                }
                
              }
            });
        }
      });
    },
    async confirm() {
      let rsp = await getUserInfo({
        id: 0,
      });
      let infoRsp = rsp.data.user;
      if (!infoRsp.EmailActive) {
        this.$message.warning("邮箱还未激活");
        this.activeEmailText = this.loginForm.username;
        this.activeDialogVisible = true;
      } else {
        this.activeDialogVisible = false;
        this.$nextTick(async()=>{
          this.$store.commit("orgLis/SET_ORG_LIST", null);
          let list = await this.$store.dispatch("orgLis/setOrgList");
          if (list && list.length > 0 ||this.hasQuery) {
            this.$router.push({ path: "/" }).catch(() => {});
          } else {
            this.$router.push("/crm/yaoqing/choose_addorg");
          }
        })
      }
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
    --forminputbg: rgba(19, 25, 34, 1),
    --forminputcontentcolor:rgba(255, 255, 255, 1),
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px,
    --labelfontsize:18px,
  ),

  'jiayulogin': (//项目样式
    --formlabelcolor: rgba(153, 153, 153, 1), 
    --formconbg: rgba(255, 255, 255, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(19, 25, 34, 1),
    --forminputcontentcolor:rgba(255, 255, 255, 1),
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px,
    --labelfontsize:18px,
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

.active_email_con {
  font-size: 20px;
  color: #333333;
  .email_text {
    color: #EE7700;
  }
}
.active_dialog-footer {
  display: flex;
  justify-content: space-between;
  margin-top: 70px;
  .cancel_btton {
    height: 52px;
    width: calc(50% - 10px);
    background: #f6f9ff;
    color: #EE7700;
    font-size: 20px;
    border: 0;
    &.noDaoji {
      color: #EE7700;
    }
  }
  .confrim_button {
    height: 52px;
    font-size: 20px;
    width: calc(50% - 10px);
    background: rgba(61, 185, 143, 1);
    border: 0;
  }
}
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
  .logo-heard{
    // margin:30px 0 0 30px ;
    width: 360px;
    height: 46px;
    position: absolute;
    left: 40px;
    top: 42px;
    img{
      width: 100%;
      height: 100%;
    }
  }


  .login-form {
    width: 1440px;
    // max-width: 100%;
    height: 820px;
    // 应用默认主题
    @include apply-theme('default');
    // background: var(--formconbg);
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
      .login-title{
        font-weight: 600;
        font-size: 28px;
        line-height: 28px;
        color: rgba(255, 255, 255, 1);
        margin-bottom: 30px;
      }
      .login-code-img {
        width: 148px;
        height: 48px;
        cursor: pointer;
        margin-left: 10px;
      }
      .login-check{
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 60px;
        padding-top: 14px;
          span {
            font-size: 14px !important;
            color: var(--formlabelcolor) !important;
            font-weight: 400;
          }
          .el-checkbox.is-checked{
            ::v-deep .el-checkbox__label{
              color: rgba(61, 185, 143, 1);
            }
            ::v-deep .el-checkbox__input.is-checked .el-checkbox__inner, .el-checkbox__input.is-indeterminate .el-checkbox__inner{
              background-color: rgba(61, 185, 143, 1);
              border-color: rgba(61, 185, 143, 1);
            }
            ::v-deep .el-checkbox__input.is-focus .el-checkbox__inner{
              border-color: rgba(61, 185, 143, 1);
            }
          }
      }
      .login-register{
        margin-top: 70px;
        text-align: center;
        font-weight: 400;
        font-size: 14px;
        color: #EE7700;
        span{
          color: #999999;
        }
      }
      ::v-deep label.el-form-item__label::before {
        content: "" !important;
      }
      
      ::v-deep .el-form-item__content {
        color: var(--formcontentcolor) !important;
        
      }
      .forget_link{
        color:var(--formcontentcolor);
      }
      ::v-deep .el-form-item{
        margin-bottom: 16px !important;
      }
      ::v-deep .el-input {
        height: var(--inputHeight);
        border-radius: 6px;
        background-color: var(--forminputbg);
        input {
          height: var(--inputHeight);
          background-color:rgba(19, 25, 34, 1);
          color: var(--forminputcontentcolor);
          border: 1px solid rgba(19, 25, 34, 1);
          border-radius: 6px;
          font-size: var(--inputfontsize);
        }
      }
      ::v-deep .el-input__prefix{
        color: rgba(255, 255, 255, 0.30);
        font-size: 14px;
        line-height: 48px;
        left: 16px;
      }
      ::v-deep .el-input--prefix .el-input__inner{
        padding-left: 40px;
      }
      ::v-deep .el-button {
        height: 52px;
        border-radius: 4px;
        background: rgba(61, 185, 143, 1);
        font-size: 16px;
        border: 0;
      }
      ::v-deep .el-input__inner:focus {
        border: 1px solid rgba(61, 185, 143, 1);
      }
      .input-icon {
        height: 39px;
        width: 14px;
        margin-left: 2px;
      }
      .LoginMethod{
        display: flex;
        font-size: 14px !important;
        line-height: 14px;
        margin-bottom: 40px;
        width:100%;
        align-items: center;

        .LoginMethod-router{
          color: #999999;
          padding: 0 10px;
          border-right: 1px solid rgba(255, 255, 255, 0.60);
          .routerLink{
            display: block;
            text-align: center;
          }
          .activeRouterLink{
            color: rgba(61, 185, 143, 1);
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
  }
}



</style>
