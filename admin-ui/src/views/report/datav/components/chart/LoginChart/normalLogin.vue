<template>
  <el-form
    ref="loginForm"
    :model="loginForm"
    :rules="loginRules"
    label-position="top"
  >
    <el-form-item prop="username">
      <span slot="label" style="flex: 1">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-zhanghao"
          style="margin-right: 14px"
        ></i
        >账号
      </span>

      <el-input
        v-model="loginForm.username"
        type="text"
        auto-complete="off"
        placeholder="请输入账号、邮箱或手机"
      >
      </el-input>
    </el-form-item>
    <el-form-item prop="password" label="密码">
      <span slot="label" style="flex: 1">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-mima"
          style="margin-right: 14px"
        ></i
        >密码
      </span>
      <label slot="label" @click="forgotPassword">
        <span style="color: rgba(120, 130, 157, 1);font-size:14px;cursor: pointer;">
          <label>
            <label>忘记密码？</label>
          </label>
        </span>
      </label>
      <el-input
        v-model="loginForm.password"
        type="password"
        auto-complete="off"
        placeholder="请输入密码"
        @keyup.enter.native="handleLogin"
      >
      </el-input>
    </el-form-item>
    <el-form-item prop="code" v-if="captchaOnOff" label="验证码">
      <span slot="label">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-yanzhengma"
          style="margin-right: 14px;"
        ></i
        >验证码
      </span>
      <el-input
        v-model="loginForm.code"
        auto-complete="off"
        placeholder="请输入验证码"
        style="width: 63%"
        @keyup.enter.native="handleLogin"
      >
      </el-input>
      <div class="login-code" :style="{'--height':this.chartOption.input.inputHeight+'px'}">
        <img :src="codeUrl" @click="getCode" class="login-code-img" />
      </div>
    </el-form-item>
    <div>
      <el-checkbox
        v-model="loginForm.rememberMe"
        style="margin: 0px 0px 25px 0px;"
        :style="{ color: this.chartOption.label.fontColor }"
        @change="changeRemember"
        >记住密码</el-checkbox
      >
      <div style="float: right" v-if="register">
        <router-link class="link-type" :to="'/InviteRegister'"
          >立即注册</router-link
        >
      </div>
      <div style="clear: both; height: 0; overflow: hidden"></div>
    </div>

    <el-form-item style="width: 100%">
      <el-button
        :loading="loading"
        size="medium"
        type="primary"
        style="width: 100%"
        @click.native.prevent="handleLogin"
      >
        <span v-if="!loading">登 录</span>
        <span v-else>登 录 中...</span>
      </el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import Cookies from "js-cookie";
import { encrypt, decrypt } from "@/utils/jsencrypt";
import { setToken, setRefreshToken } from "@/utils/auth";
import { getCodeImg, getUserInfo } from "@/api/login";
export default {
  name: "reportNormalLogin",
  props: {
    register: {
        type:Boolean,
        default:false
    },
    loginType:{
        type:Boolean,
        default:false
    },
    redirect:{
      type:String
    },
    chartOption: {
        type: Object
    },
  },
  data() {
    return {
      loading: false,
      codeUrl: "",
      loginForm: {
        username: "",
        password: "",
        rememberMe: false,
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
      // 验证码开关
      captchaOnOff: false,
    };
  },
  created(){
    // this.getCode();
    this.getCookie();
  },
  mounted() {},

  methods: {
    forgotPassword(){
      this.$emit('forgotPassword',true)
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
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.loading = true;

          this.$store
            .dispatch("Login", this.loginForm)
            .then(async () => {
              try {
                if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.isDefalutUrl){
                  this.$store.commit('SET_ROLES', [])
                  this.$store.commit("orgLis/SET_ORG_LIST", null);
                  let list = await this.$store.dispatch("orgLis/setOrgList");
                  this.$nextTick(()=>{
                    if (list && list.length > 0) {
                      this.$router.push({ path: this.redirect || "/" }).catch(() => {});
                    } else {
                      this.$router.push("/crm/yaoqing/choose_addorg");
                    }
                  })
                }else{
                  if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.loginJumpUrl){
                    window.location.replace(this.chartOption.buttonSetting.loginJumpUrl); //切换企业后刷新整个网站
                  }
                }
                
              } catch (error) {
                console.log(error,'eee');
              }
              
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
                    
                    this.$emit('reloadActiveEmail',this.loginForm.username)
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
  },
};
</script>
<style lang="less" scoped>
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
</style>
