<template>
  <div class="login">
    <el-form ref="loginForm" label-position="top" class="login-form">
      <div class="login_title">
        <h2 class="title">后台管理系统</h2>
      </div>
      <div class="form_con">
        <div class="LoginMethod">
          <div class="LoginMethod-router">
            <router-link :to="'/login'" class="routerLink">密码</router-link>
          </div>
          <div class="LoginMethod-router" v-if="codeLogin">
            <router-link :to="'/loginInterface'" class="routerLink">手机号</router-link>
          </div>
          <div class="LoginMethod-router" v-if="emailLogin">
            <router-link :to="'/emailLogin'" class="routerLink">邮箱</router-link>
          </div>
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
.LoginMethod {
  display: flex;
  font-size: 16px !important;
  margin: 25px 0px;
  width: 100%;
  align-items: center;
  justify-content: space-around;
  padding-bottom: 20px;
  text-align: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);

  .LoginMethod-router {
    color: rgba(120, 130, 157, 1);
    width: 33%;
    .routerLink {
      padding: 0px 10px;
      padding-bottom: 20px;
    }
    .activeRouterLink {
      border-bottom: 3px solid rgba(53, 114, 255, 1);
      color: #fff !important;
    }
  }
  .LoginMethod-router:first-child {
    text-align: left;
  }
  .LoginMethod-router:last-child {
    text-align: right;
  }
}
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
.login {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
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
    margin-left: 24px;
    font-size: 20px;
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
}
// .title {
//   margin: 0px auto 30px auto;
//   text-align: center;
//   color: #707070;
// }

.login-form {
  width: 500px;
  padding: 25px 25px 5px 25px;
  position: fixed;
  right: 12.3%;
  bottom: 7%;
  .form_con {
    background: rgba(24, 36, 69, 1);
    width: 500px;
    height: 568px;
    border-radius: 20px;
    padding: 50px;
    padding-top: 1px;

    ::v-deep label.el-form-item__label::before {
      content: "" !important;
    }
    ::v-deep label.el-form-item__label {
      display: flex;
      span {
        font-size: 18px !important;
        color: rgba(255, 255, 255, 0.8) !important;
        font-weight: normal;
      }
      .LoginMethod {
        display: flex;
        color: #78829d !important;
        font-size: 14px !important;
        .LoginMethod_label {
          display: flex;
          align-items: center;
          .PhoneNumberLogo {
            width: 14px;
            height: 14px;
            margin-right: 5px;
          }
        }
      }
    }
    ::v-deep .el-form-item__content {
      color: rgba(120, 130, 157, 1) !important;
    }
    ::v-deep .el-input {
      height: 48px;
      border-radius: 10px;
      background-color: rgba(32, 46, 87, 1);
      input {
        height: 48px;
        border-radius: 10px;
        background-color: rgba(32, 46, 87, 0);
        color: rgba(255, 255, 255, 1);
        border: 1px solid #202e57;
        opacity: 0.5;
      }
    }
    ::v-deep .el-button {
      height: 52px;
      border-radius: 4px;
      background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
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
.login-code {
  width: 33%;
  height: 38px;
  float: right;
  img {
    cursor: pointer;
    vertical-align: middle;
  }
}
.login-button {
  border: none;
  width: 30%;
  height: 48px !important;
  margin-left: 15px;
  color: #fff !important;
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
}
.login-code-img {
  height: 38px;
}
.qrcode {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
  