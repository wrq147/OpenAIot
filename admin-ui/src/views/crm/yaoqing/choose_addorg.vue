<template>
  <div class="choose_bg jiayuchoose_bg">
    <div class="choose_con jiayuchoose_con">
      <div class="title" v-if="loginType=='default'">
        后台管理系统
      </div>
      <div class="btn_con">
        <div class="btn_view" @click.stop="haiLoginID">加入企业</div>
        <div class="btn_view" @click.stop="recreateID">新建企业</div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "AdminUiChoose", //邀请的代理商是否已有账号选择

  data() {
    return {
      code:null,
      loginType:'default',//登录类型
    };
  },

  async mounted() {
  },

  methods: {
    haiLoginID() {
      this.$router.push("/crm/yaoqing/analyzeLlink");
    },
    recreateID() {
      this.$router.push("/crm/yaoqing/creatOrg");
    },
  },
};
</script>

<style lang="scss" scoped>
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