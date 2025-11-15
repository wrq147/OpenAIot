<template>
  <div class="login">
    <div class="logo-heard">
      <img src="../assets/images/name_logo.png" alt="">
    </div>
    <el-form ref="loginForm" :model="loginForm" :rules="loginRules" label-position="top" class="login-form">

      <div class="form_con">
        <div class="login-title">登录</div>
        <div class="LoginMethod">
          <div class="LoginMethod-router" v-if="$route.query.forgetPass==100&&!emailLogin" style="text-align:center;font-size:18px;color:#ffffff">
            <span>手机号验证</span>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100">
            <router-link :to="'/login'" class="routerLink">账号登录</router-link>
          </div>
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100||$route.query.forgetPass==100&&emailLogin">
            <router-link :to="$route.query.forgetPass==100?'/loginInterface?forgetPass=100':'/loginInterface'" class="routerLink activeRouterLink">手机号登录</router-link>
          </div>
          <!-- <div class="LoginMethod-router" v-if="emailLogin">
            <router-link :to="$route.query.forgetPass==100?'/emailLogin?forgetPass=100':'/emailLogin'" class="routerLink">邮箱登录</router-link>
          </div> -->
          <div class="LoginMethod-router" v-if="$route.query.forgetPass!=100&&corpLogin">
            <router-link :to="'/wxworkLogin'" class="routerLink">
              企业微信
            </router-link>
          </div>
        </div>
        <el-form-item prop="phone">
          <el-input v-model="loginForm.phone" type="text" auto-complete="off" :clearable="true" placeholder="请输入手机号">
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

        <el-form-item style="width: 100%; padding-top: 30px">
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
import { sendMobileCode, sendImgCode, loginPhone } from "@/api/system/Employee";
import { setToken, setRefreshToken } from "@/utils/auth";
import { getConfigKey,getConfigKeyList } from "@/api/system/config.js";
export default {
  name: "Login",
  data() {
    return {
      loginForm: {
        phone: "",
        username: "",
        password: "",
        rememberMe: false,
        code: "",
        uuid: "",
        imgCode: "",
      },
      uuid: "",
      imgImg: "",
      loginRules: {
        phone: [
          { required: true, trigger: "blur", message: "请输入您的手机号" },
        ],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        code: [{ required: true, trigger: "blur", message: "请输入验证码" }],
      },
      loading: false,
      // 验证码开关
      redirect: undefined,
      corpLogin:false,//企业微信登录开关
      codeLogin:false,//手机号验证码登录开关
      emailLogin:false,//邮箱登录开关
      getCodeText: "获取验证码",
      disabled: false,
      getCodeBtnColor: "#ffffff",
      getCodeisWaiting: false,
      hideCode: false,
      loginType: 'default',
      yaoqingId:''
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
    // getConfigKey("login.corp").then((res) => {
    //   this.corpLogin = res.data=='false'||res.data==''?true:false;
    // });
    // //配置是否需要邮箱登录
    // getConfigKey("login.email").then((res) => {
    //   this.emailLogin = res.data=='false'||res.data==''?true:false;
    // });
    // //配置是否需要邮箱登录
    // getConfigKey("login.code").then((res) => {
    //   this.codeLogin = res.data=='false'||res.data==''?true:false;
    // });
    getConfigKeyList(["login.code","login.corp","login.email"]).then((res) => {
      console.log(res,'resres');
      let tmpitem = res.data.find(x=>x.name=="login.code");
      this.codeLogin = tmpitem==null||tmpitem.value=='false'?true:false;
      tmpitem = res.data.find(x=>x.name=="login.corp");
      this.corpLogin = tmpitem==null||tmpitem.value=='false'?true:false;
      tmpitem = res.data.find(x=>x.name=="login.email");
      this.emailLogin = tmpitem==null||tmpitem.value=='false'?true:false;
    });
  },
  methods: {
    handEmailLogin() {
      if (this.$route.query.forgetPass == 100) {
        this.$router.push("/emailLogin?forgetPass=100");
      } else {
        this.$router.push("/emailLogin");
      }
    },
    handImgCode() {
      this.tuxingCode();
    },
    handInput(e) {
      if (this.loginForm.imgCode.length == 4) {
        //this.tuxingCode();
        this.handGetCode();
      }
    },
    //发送手机验证码登录
    handGetCode() {
      if (!this.loginForm.phone) {
        this.$message.warning("请输入手机号"); //弹出提示框
        return;
      }
      sendMobileCode({
        phone: this.loginForm.phone,
        code: this.loginForm.imgCode,
        imgid: this.uuid,
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
              this.$message.success("验证码已发送"); //弹出提示框
              this.setTimer(); //调用定时器方法
            }, 1000);
          }
        })
        .catch((err) => {
          if (err.code == 2) {
            // this.$message.warning(err);//弹出提示框
            this.tuxingCode();
            this.hideCode = true;
          }
          //	this.$message.warning(err);//弹出提示框
        });
    },

    setTimer() {
      let holdTime = 60; //定义变量并赋值
      this.getCodeText = "重新获取(60)s";
      //setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
      //setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
      this.Timer = setInterval(() => {
        holdTime--;
        if (holdTime <= 0) {
          this.disabled = false;
          this.getCodeisWaiting = false;
          this.getCodeBtnColor = "#ffffff";
          this.getCodeText = "重新获取";
          clearInterval(this.Timer); //清除该函数
          return; //返回前面
        }
        this.getCodeText = "重新获取("+holdTime + ")s";
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
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.loading = true;
          loginPhone({
            tel: this.loginForm.phone,
            code: this.loginForm.code,
          })
            .then(async (res) => {
              if (res.code == 0) {
                // console.log(res)
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
                    this.$store.commit("orgLis/SET_ORG_LIST", null);
                    this.$nextTick(async()=>{
                      this.$store.commit("orgLis/SET_ORG_LIST", null);
                      let list = await this.$store.dispatch("orgLis/setOrgList");
                      if (list && list.length > 0) {
                        this.$router.push({ path: "/" }).catch(() => {});
                      } else {
                        if (this.yaoqingId){
                          this.$router.push("/crm/yaoqing/choose_addorg?yaoqingId=" + this.yaoqingId);
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
            .catch(() => {
              this.loading = false;
              // this.tuxingCode();
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
    --forminputbg: rgba(19, 25, 34, 1),
    --forminputcontentcolor:rgba(255, 255, 255, 1),
    --inputHeight: 48px,
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
    width: 75%;
    height: 820px;
    // position: absolute;
    // right: 0;
    // top: 0;
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
      // .login-code-img {
      //   width: 148px;
      //   height: 48px;
      //   cursor: pointer;
      //   margin-left: 10px;
      // }
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
        &.el-input--prefix .el-input__inner{
          padding-left: 40px;
        }
        // &.el-input--suffix .el-input__inner {
        //   padding-right: 120px;
        // }
        &.el-input--suffix{
          .el-input__suffix{
            .input_suffix_con{
              display: flex;
              justify-content: flex-start;
              align-items: center;
              .input_suffix_line{
                height: 14px;
                width: 1px;
                background:rgba(255, 255, 255, 0.2);
              }
              .el-button.input_suffix_button{
                width: 102px;
                height: 48px;
                background: transparent;
                color: rgba(61, 185, 143, 1);
                font-size: 14px;
                border: none;
                padding-left: 16px;
                padding-right: 16px;
                &.disabled-button{
                  width: 122px;
                  color: #999999;
                }
              }
            }
          }
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
      .login-code {
        width: calc(50% - 30px);
        height: 48px;
        float: right;
        img {
          width: 100%;
          cursor: pointer;
          height: 48px;
          vertical-align: middle;
        }
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
