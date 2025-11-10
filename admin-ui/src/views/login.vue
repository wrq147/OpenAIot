<template>
  <div class="login">
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form" >

      <div class="login_title" v-if="loginType=='default'">
        <h2 class="title"> 后台管理系统 </h2>
      </div>
      <div class="form_con">
        <div class="LoginMethod" :style="{'border-bottom':!codeLogin&&!emailLogin&&!corpLogin?'none':''}" v-if="loginType=='default'">
          <!-- <div class="LoginMethod-router" v-if="!codeLogin&&!emailLogin&&!corpLogin" style="text-align:center;font-size:18px;color:#ffffff">
            <span></span>
          </div> -->
          <div class="LoginMethod-router" v-if="codeLogin||emailLogin||corpLogin">
            <router-link :to="'/login'" class="routerLink activeRouterLink">密码</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="'/loginInterface'" class="routerLink">手机号</router-link>
          </div>
          <div class="LoginMethod-router" v-if="emailLogin">
            <router-link :to="'/emailLogin'"  class="routerLink">邮箱</router-link>
          </div>
          <div class="LoginMethod-router" v-if="corpLogin">
            <router-link :to="'/wxworkLogin'" class="routerLink">企业微信</router-link>
          </div> 
        </div>
        <el-form-item prop="username">
          <span slot="label" style="flex:1;" v-if="loginType=='default'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhanghao" style="margin-right: 14px;" :style="{'color':'#78829d'}"></i>账号
          </span>
          
          <el-input v-model="loginForm.username" type="text" auto-complete="off" :clearable="true" :placeholder="loginType=='default'?'请输入账号、邮箱或手机':'请输入账号或手机'">
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <span slot="label" style="flex:1;" v-if="loginType=='default'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" style="margin-right: 14px;" :style="{'color':'#78829d'}"></i>密码
          </span>
          <label  slot="label" v-if="loginType=='default'&&codeLogin||loginType=='default'&&emailLogin">
            <router-link class="forget_link" :to="'/loginInterface?forgetPass=100'" v-if="codeLogin">
              <label ><label>忘记密码？</label></label>
            </router-link>
            <router-link class="forget_link" :to="'/emailLogin?forgetPass=100'" v-if="!codeLogin&&emailLogin">
              <label ><label>忘记密码？</label></label>
            </router-link>
          </label>
          <el-input v-model="loginForm.password" :show-password="true" :clearable="true" type="password" auto-complete="off" placeholder="请输入密码" @keyup.enter.native="handleLogin">
          </el-input>
        </el-form-item>
        <el-form-item prop="code" v-if="captchaOnOff">
          <span slot="label" v-if="loginType=='default'">
            <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" style="margin-right: 14px;" :style="{'color':'#78829d'}"></i>验证码
          </span>
          <el-input v-model="loginForm.code" auto-complete="off" placeholder="请输入验证码" :style="{width: '63%'}" @keyup.enter.native="handleLogin">
          </el-input>
          <div class="login-code">
            <img :src="codeUrl" @click="getCode" class="login-code-img" />
          </div>
        </el-form-item>
        <div v-if="loginType=='default'">
          <el-checkbox v-model="loginForm.rememberMe" style="margin: 0px 0px 25px 0px" :style="{'color':'#FFFFFF'}" @change="changeRemember">记住密码</el-checkbox>
          <div style="float: right" v-if="register">
            <router-link class="link-type" :to="'/InviteRegister'">立即注册</router-link>
          </div>
          <div style="clear: both; height: 0; overflow: hidden"></div>
        </div>
        <el-form-item style="width: 100%">
          <el-button :loading="loading" size="medium" type="primary" style="width: 100%" @click.native.prevent="handleLogin">
            <span v-if="!loading">登 录</span>
            <span v-else>登 录 中...</span>
          </el-button>
        </el-form-item>
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
    <!--  底部  -->
    <div class="el-login-footer" v-if="loginType=='default'">
      <div style="width: 280px;text-align: center;float: right;">推荐分辨率1920*1080</div>
      <div style="clear: both;"></div>
    </div>
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
      console.log(res,'resres');
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
    --formlabelcolor: rgba(255, 255, 255, 0.8),
    --formconbg: rgba(24, 36, 69, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(32, 46, 87, 1),
    --forminputcontentcolor: rgba(255, 255, 255, 1),
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px,
    --labelfontsize:18px,
  ),

  'jiayulogin': (//项目样式
    --formlabelcolor: rgba(153, 153, 153, 1),
    --formconbg: rgba(255, 255, 255, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(248, 248, 248, 1),
    --forminputcontentcolor: #333333,
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
    color: #3572ff;
  }
}
.active_dialog-footer {
  display: flex;
  justify-content: space-between;
  margin-top: 70px;
  .cancel_btton {
    height: 52px;
    width: calc(50% - 10px);
    border: 1px solid #dfe2ea;
    background: #f6f9ff;
    color: #78829d;
    font-size: 20px;
    &.noDaoji {
      color: #3572ff;
    }
  }
  .confrim_button {
    height: 52px;
    font-size: 20px;
    width: calc(50% - 10px);
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
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
  .el-login-footer {
    height: 55px;
    line-height: 20px;
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
    &.jiayu_footer{
      color: #999999;
    }
  }
}



</style>
