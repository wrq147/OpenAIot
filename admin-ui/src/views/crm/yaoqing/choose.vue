<template>
  <div class="choose_bg jiayuchoose_bg">
    <div class="choose_con jiayuchoose_con">
      <div class="title" v-if="loginType=='default'">
        后台管理系统
      </div>
      <div class="btn_con">
        <div class="btn_view" @click.stop="haiLoginID">已有账号</div>
        <div class="btn_view" @click.stop="recreateID">重新创建</div>
      </div>
    </div>
  </div>
</template>

<script>
import { getToken,setToken,setRefreshToken } from "@/utils/auth";
import store from "@/store";
import router from "@/router";
import { getAppDown } from "@/api/code.js";
import {loginThemeInfo} from '@/utils/theme'
import {
  explainCode
} from "@/api/system/Employee";
import {agentInviteInfo,agentInviteLogin } from '@/api/manufac/agentMansge'
export default {
  name: "AdminUiChoose", //邀请的代理商是否已有账号选择

  data() {
    return {
      yaoqingId: 0,
      isMobile: false,
      isNotJunmp: true,
      code:null,
      node:'',
      loginType:'default',//登录类型
    };
  },

  async mounted() {
    let pars = this.$route.query;
    if (pars.yaoqingId) {
      this.yaoqingId = pars.yaoqingId;
    }
    if (pars.code) {
      this.code = pars.code;
    }
    if (pars.node) {
      this.node = pars.node;
    }
    if(this.yaoqingId&&this.node){
      this.handleLoginCheck()
    }
  },

  methods: {
    handleLoginCheck(){
      agentInviteLogin({mobileCode:this.node,yqCode:this.yaoqingId}).then(async res=>{
        // console.log("res邀请登录",res);
        setToken(res.data.token)
        setRefreshToken(res.data.refresh_token)
        this.$store.commit("orgLis/SET_ORG_LIST", null);
        let list = await this.$store.dispatch("orgLis/setOrgList");
        let jumpUrl='/crm/yaoqing/hailogin?yaoqingId='+this.yaoqingId
        this.$router.push(jumpUrl);
      })
    },
    haiLoginID() {
      //已有账号
      if (getToken() && getToken() != undefined) {
        if (this.yaoqingId) {
          this.$router.push(
            "/crm/yaoqing/hailogin?yaoqingId=" + this.yaoqingId
          );
        }
        if (this.code) {
          this.$router.push(
            "/crm/yaoqing/hailogin?code=" + this.code
          );
        }
      } else {
        store.dispatch("FedLogOut").then(async () => {
          // router.replace(`/login?redirect=${router.app.$route.fullPath}`);
          if(this.code){
            explainCode({ code: decodeURIComponent(this.code) }).then(async(rsp)=>{
                // console.log("员工邀请",rsp);
                if(rsp.data&&rsp.data.OrgId){
                  let loginUrl=await loginThemeInfo(rsp.data.OrgId)
                  if(loginUrl){
                      router.replace(`${loginUrl}&redirect=${router.app.$route.fullPath}`);
                  }else{
                      router.replace(`/login?redirect=${router.app.$route.fullPath}`);
                  }
                }
            })
          }else if(this.yaoqingId){
              agentInviteInfo({id:this.yaoqingId}).then(async (rsp)=>{
                  if(rsp.data&&rsp.data.ParentOrgId){
                      let loginUrl=await loginThemeInfo(rsp.data.ParentOrgId)
                      if(loginUrl){
                          router.replace(`${loginUrl}&redirect=${router.app.$route.fullPath}`);
                      }else{
                          router.replace(`/login?redirect=${router.app.$route.fullPath}`);
                      }
                  }
                  
              }).catch(err=>{
                router.replace(`/login?redirect=${router.app.$route.fullPath}`);
              })
          }
        });
      }
    },
    recreateID() {
      //重新创建账号
      if (this.yaoqingId) {
        if(this.node){
          this.$router.push("/crm/yaoqing/creatUser?yaoqingId=" + this.yaoqingId+"&node=" + this.node);
        }else{
          this.$router.push("/crm/yaoqing/creatUser?yaoqingId=" + this.yaoqingId);
        }
        
      }
      if (this.code) {
        this.$router.push("/crm/yaoqing/creatUser?code=" + this.code);
      }
    },
  },
};
</script>

<style lang="scss" scoped>
html {
  height: 100%;
  overflow: hidden;
  // background: red;
}
$themes: (//动态设置样式
  'default': (
    --allfontcolor: #ffffff,
    --titlecolor:#ffffff,
    --btnconboxshow: none,
    --btnconbg: #182445,
    --btnviewradius:4px,
    --btnviewbg: #202e57,
    --btnmargintop:40px,
    --btnHeight: 80px,
    --btnhoverborder:#3572ff,
    --btnfontsize:20px,
    --btnconpadding:50px,
  ),
  'jiayulogin': (//LD项目样式
    --allfontcolor: #ffffff,
    --titlecolor:#333333,
    --titlefontsize:36px,
    --btnconbg: rgba(255, 255, 255, 1),
    --btnconboxshow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04),
    --btnviewradius:4px,
    --btnviewbg: linear-gradient( 90deg, #4C79FF 0%, #6DA8FF 100%),
    --btnmargintop:40px,
    --btnHeight: 80px,
    --btnhoverborder:none,
    --btnfontsize:20px,
    --btnconpadding:50px,
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
.choose_bg {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../../../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  @include apply-theme('default');
  &.jiayuchoose_bg {
    background-image: url("../../../assets/images/jiayu_login_bg1.png");
    @include apply-theme('jiayulogin');
  }

  .choose_con {
    width: 31.25vw;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    color: var(--allfontcolor);
    &.jiayuchoose_con {
      .btn_con {
        opacity: 0.98;
        .btn_view:first-child {
          border: 1px solid #3572FF;
          background: #FFFFFF;
          color: #3572FF;
        }
        .btn_view:nth-child(2):hover{
          border: none;
        }
      }
    }
    .title {
      font-size: var(--titlefontsize);
      font-weight: 550;
      color: var(--titlecolor);
    }
    .btn_con {
      margin-top: 50px;
      width: 31.25vw;
      height: 300px;
      border-radius: 10px;
      background: var(--btnconbg);
      padding: var(--btnconpadding);
      box-sizing: border-box;
      box-shadow: var(--btnconboxshow);
      .btn_view {
        width: 26vw;
        height: var(--btnHeight);
        line-height: var(--btnHeight);
        background: var(--btnviewbg);
        margin-top: var(--btnmargintop);
        text-align: center;
        border-radius: var(--btnviewradius);
        cursor: pointer;
        font-size: var(--btnfontsize);
      }
      .btn_view:first-child {
        margin-top: 0;
      }
      .btn_view:hover {
        border: 1px solid #3572ff;
      }
    }
  }
}
</style>