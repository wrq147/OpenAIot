<template>
  <div class="login">
    <div class="logo-heard">
      <img src="../assets/images/name_logo.png" alt="">
    </div>
    <el-form ref="loginForm" label-position="top" class="login-form">
      <div class="form_con">
        <div class="login-title">登录</div>
        <div class="LoginMethod">
          <div class="LoginMethod-router">
            <router-link :to="'/login'" class="routerLink">账号登录</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="'/loginInterface'" class="routerLink">手机号登录</router-link>
          </div>
          <!-- <div class="LoginMethod-router" v-if="emailLogin">
            <router-link :to="'/emailLogin'" class="routerLink">邮箱登录</router-link>
          </div> -->
          <div class="LoginMethod-router">
            <router-link :to="$route.query.forgetPass == 100 ? '/emailLogin?forgetPass=100' : '/emailLogin'" class="routerLink activeRouterLink">企业微信</router-link>
          </div>
        </div>
        <div class="qrcode"><div id="wx_qrcode" /></div>
      </div>
    </el-form>
    <!--  底部  -->
  </div>
</template>
  
<script>
import { setToken, setRefreshToken } from "@/utils/auth";
import { LoginByWxCorp, wxBaseUrl,CorpWxConfigJson } from "@/api/login";
import { orgStyle } from "@/api/system/StyleMan";
import { getConfigKey } from "@/api/system/config.js";
var jWeixin =null
export default {
  name: "Login",
  data() {
    return {
      redirect: undefined,
      authCode: "",
      wwLogin: null, //记录组件的实例
      themeOrgId: "",
      wxWorkJobInfo: null,
      wxCode: null,
      codeLogin: false,
      emailLogin: false, //邮箱登录开关
      isWxAutomaticLogon:true,
    };
  },
  watch: {
    $route: {
      handler: function (route) {
        this.redirect = route.query && route.query.redirect;
        //   if (route.query.code) {
        //     this.scancodeLogin(route.query.code)
        //   }
      },
      immediate: true,
    },
    "$route.query": {
      handler(newVal, oldVal) {
        if (this.authCode && !this.wxCode&&!this.wxloginoutVal) {
          if (this.wxWorkJobInfo) {
            this.scancodeLogin(); // 根据企业微信code调用后台接口进行登录操作
          }
        }
      },
      deep: true,
      immediate: true,
    },
    authCode: {
      handler(newVal, oldVal) {
        // console.log("authCode发生改变",this.authCode);
        if (this.authCode && !this.wxCode&&!this.wxloginoutVal) {
          if (this.wxWorkJobInfo) {
            this.scancodeLogin(); // 根据企业微信code调用后台接口进行登录操作
          }
        }
      },
    },
  },
  computed:{
    wxloginoutVal(){//是否有退出登录
      let val=sessionStorage.getItem('wxloginout')
      return val;
    }
  },
  async created() {
    //判断是否显示注册
    
    let res=await getConfigKey("login.code")
    this.codeLogin = res.data == "false" || res.data == "" ? true : false;
    //配置是否需要邮箱登录
    let res2=await getConfigKey("login.email")
    this.emailLogin = res2.data == "false" || res2.data == "" ? true : false;
    
  },
  async mounted() {
    let orgId = this.$store.getters.orgId;
    if (orgId) {
      this.themeOrgId = orgId;
    } else {
      let res = await getConfigKey("org.style");
      // console.log(res,'默认的主题');
      this.$store.commit("SET_themeOrgId", res.data);
      this.themeOrgId = this.$store.getters.themeOrgId;
      // console.log("当前主题企业Id",this.themeOrgId,orgId);
    }
    
    await this.getWxworkInfo(this.themeOrgId);
    var ua = window.navigator.userAgent.toLowerCase();
    if (/wxwork/i.test(ua)) {
      jWeixin = require('jweixin-module')
      let cururl = location.href
      await this.requestJsApiConfig(this.wxWorkJobInfo.qywxAppId,cururl)
    }
    this.wxCode = this.getUrlParam("code");
    if (this.wxCode&&!this.wxloginoutVal) {
      var ua = window.navigator.userAgent.toLowerCase();
      if (/wxwork/i.test(ua)) {
        this.authCode = this.wxCode;
        this.scancodeLogin();
      } else {
        this.$message.warning("请在企业微信执行自动登录");
      }
    } else {
      if (!this.wxloginoutVal&&(/wxwork/i.test(ua))) {
        this.redirectBaseUrl(this.wxWorkJobInfo.qywxAppId); //初始在企业微信登录时自动登录
      }else{
        this.createCode();
      }
      
    }
  },
  methods: {
    async requestJsApiConfig(appid, url) {//处理企业微信自动登录相关参数
				try {
					let result = await CorpWxConfigJson(appid, url)
					var data = result.data;
					var wxdata = {
						debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
						appId: data.corpid, // 必填，公众号的唯一标识
						timestamp: data.timestamp, // 必填，生成签名的时间戳
						nonceStr: data.nonceStr, // 必填，生成签名的随机串
						signature: data.signature, // 必填，签名
						jsApiList: ['checkJsApi',
							'scanQRCode', // 微信扫一扫接口
							'chooseImage', // 微信拍照接口
							'uploadImage', //上传图片接口与
							'previewImage', //预览
							'getLocation', //获取位置
							'openLocation', //地图
							'chooseLocation', //选择地图
							'startRecord', 
							'stopRecord', 
							'onVoiceRecordEnd',
							'translateVoice'
						]
					};

					jWeixin.config(wxdata);
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'rrrrrr');
					alert("错误11111")
					alert(JSON.stringify(e))
				}
			},
    getUrlParam(name) {
      let reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
      let r = window.location.search.substr(1).match(reg);
      // console.log(r, 'rrrrr');
      if (r != null) return unescape(r[2]);
      return null;
    },
    async getWxworkInfo(orgId) {
      try {
        let res = await orgStyle({ orgId: orgId });
        let info = null;
        if (res.data) {
          info = {
            isNotAutoCreate: JSON.parse(res.data.StyleJson).isNotAutoCreate ? JSON.parse(res.data.StyleJson).isNotAutoCreate : true,
            qywxAppId: JSON.parse(res.data.StyleJson).qywxAppId ? JSON.parse(res.data.StyleJson).qywxAppId : "",
            qywxAgentid: JSON.parse(res.data.StyleJson).qywxAgentid ? JSON.parse(res.data.StyleJson).qywxAgentid : "",
          };
        }
        this.wxWorkJobInfo = info;
      } catch (error) {
        this.wxWorkJobInfo = null;
      }
    },
    scancodeLogin() {
      LoginByWxCorp({
        code: this.authCode,
        appid: this.wxWorkJobInfo.qywxAppId,
        created: this.wxWorkJobInfo.isNotAutoCreate,
      })
        .then(async (res) => {
          if (res.code == 0) {
            // console.log(res)
            sessionStorage.setItem('wxloginout', false);//清除退出登录判断变量
            this.$modal.msgSuccess("登录成功");
            setToken(res.data.token);
            setRefreshToken(res.data.refresh_token);
            this.$store.commit("SET_ROLES", []);
            try {
              this.$store.commit("orgLis/SET_ORG_LIST", null);
              this.$nextTick(async () => {
                this.$store.commit("orgLis/SET_ORG_LIST", null);
                let list = await this.$store.dispatch("orgLis/setOrgList");
                if (list && list.length > 0) {
                  this.$router.push({ path: "/" }).catch(() => {});
                } else {
                  this.$router.push("/crm/yaoqing/choose_addorg");
                }
              });
            } catch (error) {
              console.log(error, "eee");
            }
          }
        })
        .catch((err) => {
          this.loading = false;
          // this.tuxingCode();
          // if(err.code&&err.code==11){
          //   this.$message.warning("用户不存在，请先添加用户");
          //   this.createCode();
          // }else{
          //   this.redirectBaseUrl(this.wxWorkJobInfo.qywxAppId); //登录报错重新刷新code
          // }
          this.createCode();
        });
    },
    redirectBaseUrl(appId) {
      // var cururl = location.href.split('#')[0];
      var cururl = location.href;
      
      // return
      // let cururl=location.origin+location.hash
      wxBaseUrl({
        url: cururl,
        appId: appId,
      })
        .then((result) => {
          location.href = result.data;
        })
        .catch((err) => {});
    },
    createCode() {
      const that = this;
      let cururl = location.href;

      if (this.wxWorkJobInfo) {
        this.wwLogin = ww.createWWLoginPanel({
          el: "#wx_qrcode",
          params: {
            appid: this.wxWorkJobInfo.qywxAppId, //wwa972ed49001348f9
            agentid: this.wxWorkJobInfo.qywxAgentid,
            redirect_uri: cururl,
            panel_size: "small",
            redirect_type: "callback",
          },
          onCheckWeComLogin({ isWeComLogin }) {
            // console.log('isWeComLogin',isWeComLogin)
          },
          onLoginSuccess(val) {
            // console.log('onLoginSuccess',val.code)
            that.authCode = val.code; // 获取的code赋值
          },
          onLoginFail(err) {
            console.log("err", err);
          },
        });
      }
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
}
// .title {
//   margin: 0px auto 30px auto;
//   text-align: center;
//   color: #707070;
// }

.login-form {
  // background: var(--formconbg);
  width: 75%;
  height: 820px;
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
.qrcode {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 53.84%;
  height: auto;
  margin: 0 auto;
}
</style>
  