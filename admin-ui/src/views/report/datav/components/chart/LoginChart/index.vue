<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
  >
    <div class="form_con jiayuform_con" :style="levelSettingStyle">
      <div class="LoginMethod" :style="navSettingStyle">
        <div class="LoginMethod-router" @click="changeLoginMethod(1)">
          <div class="routerLink" :class="loginMethod==1?'activeRouterLink':''">
            账号登录
          </div>
        </div>
        <div class="LoginMethod-router" @click="changeLoginMethod(2)">
          <div class="routerLink" :class="loginMethod==2?'activeRouterLink':''">
            手机号登录
          </div>
        </div>
        <div class="LoginMethod-router" @click="changeLoginMethod(3)">
          <div class="routerLink" :class="loginMethod==3?'activeRouterLink':''">
            邮箱登录
          </div>
        </div>
      </div>
      <normalLogin :chartOption="chartOption" :register="register" :loginType="loginType" v-if="loginMethod==1" @reloadActiveEmail="reloadActiveEmail" :redirect="redirect" @forgotPassword="forgotPassword"></normalLogin>
      <messageLogin :chartOption="chartOption" :register="register" :loginType="loginType" v-if="loginMethod==2" @reloadActiveEmail="reloadActiveEmail" :redirect="redirect" @forgotPassword="forgotPassword"></messageLogin>
      <emailLogin :chartOption="chartOption" :register="register" :loginType="loginType" v-if="loginMethod==3" :redirect="redirect" :isNeedJumpForget="isNeedJumpForget" @forgotPassword="forgotPassword"></emailLogin>
    </div>
    <el-dialog
      title="账号激活"
      :visible.sync="activeDialogVisible"
      width="30%"
      v-loading="activeLoading"
      :modal="false"
    >
      <div class="active_email_con">
        请登录邮箱
        <span class="email_text">{{ activeEmailText }}</span> 激活账号
      </div>
      <span slot="footer" class="active_dialog-footer">
        <el-button class="cancel_btton" v-if="activeDaoji"
          >重新发送激活邮件 {{ daojiNum }}s</el-button
        >
        <el-button
          class="cancel_btton noDaoji"
          v-if="!activeDaoji"
          @click="reSendEmail"
          >重新发送激活邮件</el-button
        >
        <el-button class="confrim_button" type="primary" @click="confirm"
          >已激活</el-button
        >
      </span>
    </el-dialog>
    <div class="login_modal" :style="{'z-index':zIndex+1}" v-if="activeDialogVisible"></div>
  </div>
</template>

<script>
import "../../../animate/animate.css";
import dataChart from "../../mixins/dataChart.js";
import normalLogin from "./normalLogin"
import emailLogin from "./emailLogin"
import messageLogin from "./messageLogin"
import { getConfigKey } from "@/api/system/config.js";
import { getUserInfo, sendEmailCode } from "@/api/login";
export default {
  mixins: [dataChart],
  components: {
    normalLogin,
    emailLogin,
    messageLogin
  },
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
    zIndex:{
      type:Number,
      default:0
    },
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      loginForm: "",
      input: "",
      animate: this.className,
      // 注册开关
      register: false,
      redirect: undefined,
      loginType: true,
      activeLoading: false,
      activeEmailText: "",
      activeDialogVisible: false,
      activeDaoji: false,
      daojiNum: 0,
      loginMethod:1,//导航类型即登录方式
      username:'',//用户名
      isNeedJumpForget:false,
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      },
    },
    $route: {
      handler: function (route) {
        if(route.query && route.query.redirect&&route.query.redirect.indexOf('/crm/yaoqing/choose')>-1){
          let arr=route.query.redirect.split("?")
          this.redirect='/crm/yaoqing/hailogin?'+arr[1]
        }else{
          this.redirect = route.query && route.query.redirect;
        }
       
      },
      immediate: true,
    },
  },
  created(){
    getConfigKey("sys.account.registerUser").then((res) => {
      this.register = res.data == "true";
    });
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.input;
      };
    }
  },
  beforeDestroy() {},
  computed: {
    navSettingStyle(){
      const style={
        '--bordercolor':this.chartOption.navsetting.borderColor,
        '--activebordercolor':this.chartOption.navsetting.activeBorderColor,
        '--fontSize':this.chartOption.navsetting.fontSize+'px',
        '--fontWeight':this.chartOption.navsetting.fontWeight,
        '--fontColor':this.chartOption.navsetting.fontColor,
        '--activefontColor':this.chartOption.navsetting.activeColor,
        '--lineHeight':this.chartOption.navsetting.lineHeight+'px',
        '--fontPadding':this.chartOption.navsetting.fontPadding+'px',
      }
      return style
    },
    levelSettingStyle(){
      const style={
        '--bgColor':this.chartOption.levelsetting.bgColor,
        '--bgRadius':this.chartOption.levelsetting.bgRadius+'px',
        '--labelFontSize':this.chartOption.label.fontSize+'px',
        '--labelfontColor':this.chartOption.label.fontColor,
        '--labelfontWeight':this.chartOption.label.fontWeight,
        '--inputFontSize':this.chartOption.input.fontSize+'px',
        '--inputRadius':this.chartOption.input.inputRadius+'px',
        '--inputHeight':this.chartOption.input.inputHeight+'px',
        '--inputfontColor':this.chartOption.input.fontColor,
        '--inputfocusBorderColor':this.chartOption.input.focusBorderColor,
        '--inputBgColor':this.chartOption.input.inputBgColor,
        '--inputBorderColor':this.chartOption.input.inputBorderColor,
        '--btnBgStart':this.chartOption.buttonSetting.btnBgStart,
        '--btnBgEnd':this.chartOption.buttonSetting.btnBgEnd,
        '--btnfontColor':this.chartOption.buttonSetting.fontColor,
        '--btnFontSize':this.chartOption.buttonSetting.fontSize+'px',
        '--btnHeight':this.chartOption.buttonSetting.btnHeight+'px',
        '--btnRadius':this.chartOption.buttonSetting.btnRadius+'px',
        '--btnfontWeight':this.chartOption.buttonSetting.fontWeight,
      }
      return style
    }
  },
  methods: {
    forgotPassword(val){
      //忘记密码
      this.isNeedJumpForget=val
      if(val){
        this.loginMethod=3
      }
    },
    changeLoginMethod(val){
      this.loginMethod=val
      if(val==1){
        this.isNeedJumpForget=false
      }
    },
    reloadActiveEmail(username){
      this.activeEmailText = username;
      this.activeDialogVisible = true;
      this.reSendEmail(username);
    },
    reSendEmail(username) {
      this.username=username
      //重新发送激活邮件
      this.activeLoading = true;
      sendEmailCode({
        email: username,
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
    async confirm() {
      let rsp = await getUserInfo({
        id: 0,
      });
      let infoRsp = rsp.data.user;
      if (!infoRsp.EmailActive) {
        this.$message.warning("邮箱还未激活");
        this.activeEmailText = this.username;
        this.activeDialogVisible = true;
      } else {
        this.activeDialogVisible = false;
        if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.isDefalutUrl){
          this.$store.commit('SET_ROLES', [])
          this.$store.commit("orgLis/SET_ORG_LIST", null);
          let list = await this.$store.dispatch("orgLis/setOrgList");
          this.$nextTick(()=>{
            if (list && list.length > 0) {
              this.$router.push({ path: "/" }).catch(() => {});
            } else {
              this.$router.push("/crm/yaoqing/choose_addorg");
            }
          })
        }else{
          if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.loginJumpUrl){
            window.location.replace(this.chartOption.buttonSetting.loginJumpUrl); //切换企业后刷新整个网站
          }
        }
      }
    },
    setChartVal(result) {},
  },
};
</script>

<style lang="scss" scoped>

.login_modal {
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    opacity: .5;
    background: #000;
}
.LoginMethod {
  $border-color:var(--bordercolor);
  $border-activecolor:var(--activebordercolor);
  $fontColor:var(--fontColor);
  $activefontColor:var(--activefontColor);
  $lineHeight:var(--lineHeight);
  $fontPadding:var(--fontPadding);
  $fontSize:var(--fontSize);
  $fontWeight:var(--fontWeight);
  display: flex;
  font-size: $fontSize;
  // margin: 25px 0px;
  // padding-top: 25px;
  margin: 0;
  width: 100%;
  align-items: center;
  justify-content: space-around;
  // padding-bottom: 20px;
  text-align: center;
  border-bottom: 1px solid $border-color;
  margin-bottom: 25px;

  .LoginMethod-router {
    color: $fontColor;
    width: 33%;
    line-height: $lineHeight;
    padding: $fontPadding 0;
    .routerLink {
      padding: 0px 10px;
      padding-bottom: 24px;
      display: inline;
      font-weight: $fontWeight;
    }
    .activeRouterLink {
      border-bottom: 3px solid $border-activecolor;
      color: $activefontColor !important;
    }
  }
  .LoginMethod-router:first-child {
    text-align: left;
  }
  .LoginMethod-router:last-child {
    text-align: right;
  }
}
.form_con {
  $bgColor:var(--bgColor);
  $bgRadius:var(--bgRadius);
  $labelFontSize:var(--labelFontSize);
  $labelfontColor:var(--labelfontColor);
  $labelfontWeight:var(--labelfontWeight);
  $inputFontSize:var(--inputFontSize);
  $inputRadius:var(--inputRadius);
  $inputHeight:var(--inputHeight);
  $inputfontColor:var(--inputfontColor);
  $inputfocusBorderColor:var(--inputfocusBorderColor);
  $inputBorderColor:var(--inputBorderColor);
  $inputBgColor:var(--inputBgColor);
  $btnBgStart:var(--btnBgStart);
  $btnBgEnd:var(--btnBgEnd);
  $btnfontColor:var(--btnfontColor);
  $btnFontSize:var(--btnFontSize);
  $btnHeight:var(--btnHeight);
  $btnRadius:var(--btnRadius);
  $btnfontWeight:var(--btnfontWeight);
  
  background: $bgColor;
  width: 100%;
  height: 100%;
  border-radius: $bgRadius;
  padding: 50px;
  padding-top: 1px;
  
  &.jiayuform_con {
    background: $bgColor;
    box-shadow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04);
    ::v-deep label.el-form-item__label {
      span {
        color: $labelfontColor !important;
      }
    }
    // ::v-deep .el-form-item__content {
    //   color: rgba(120, 130, 157, 1) !important;
    // }
    ::v-deep .el-input {
      height: $inputHeight;
      border-radius: $inputRadius;
      background-color: $inputBgColor;
      input {
        font-size: $inputFontSize;
        height: $inputHeight;
        border-radius: $inputRadius;
        background-color: $inputBgColor;
        color: $inputfontColor;
        border: 1px solid $inputBorderColor;
        opacity: 1;
      }
    }
  }
  ::v-deep label.el-form-item__label::before {
      content: "" !important;
    }
  ::v-deep label.el-form-item__label {
    display: flex;
    span {
      font-size: $labelFontSize;
      color: $labelfontColor;
      font-weight: $labelfontWeight;
      i{
        color: $labelfontColor;
        font-size: $labelFontSize;
      }
    }
  
  }
  ::v-deep .el-form-item__content {
    .el-button {
      height:$btnHeight;
      border-radius: $btnRadius;
      background: linear-gradient(90deg, $btnBgStart 0%, $btnBgEnd 100%);
      color: $btnfontColor;
      font-size: $btnFontSize;
      font-weight: $btnfontWeight;
    }
  }

  ::v-deep .el-input__inner:focus {
    border: 1px solid $inputfocusBorderColor !important;
  }
}
</style>
