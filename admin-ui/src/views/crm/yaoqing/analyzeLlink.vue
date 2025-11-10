<template>
  <div class="choose_bg jiayuchoose_bg">
    <div class="choose_con jiayuchoose_con">
      <div class="title" v-if="loginType=='default'">
        后台管理系统
      </div>
      <div class="cont_con">
        <div>
          <el-input type="textarea" :autosize="{ minRows: 4, maxRows: 4}" placeholder="请输入内容" v-model="linkUrl"></el-input>
        </div>
        <div class="btn_con">
          <div class="btn_view" @click.stop="nextClick">下一步</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  name: "AdminUiChoose", //邀请的代理商是否已有账号选择

  data() {
    return {
      linkUrl:'',
      loginType:'default',//登录类型
    };
  },
  computed: {
  },
  mounted() {
  },

  methods: {
    nextClick() {
				//跳转下一步
				if(this.linkUrl){
					if(this.linkUrl.indexOf('yaoqingId=')>-1){
						let list=this.linkUrl.split('yaoqingId=')
            this.$router.push("/crm/yaoqing/hailogin?yaoqingId="+list[1]);
					}else if(this.linkUrl.indexOf('yqcode=')>-1){
						let list=this.linkUrl.split('yqcode=')
            this.$router.push("/crm/yaoqing/hailogin?yaoqingId="+list[1]);
					}else if(this.linkUrl.indexOf('code=')>-1){
						let list=this.linkUrl.split('code=')
            this.$router.push("/InviteRegister?code="+ list[1]);
					}else{
            this.$message.warning("请输入正确的邀请链接");
					}
				}else{
          this.$message.warning("请输入邀请链接");
				}
			}
  },
};
</script>

<style lang="scss" scoped>
html {
  height: 100%;
  overflow: hidden;
  // background: red;
}
.choose_bg {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../../../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  &.jiayuchoose_bg {
    background-image: url("../../../assets/images/jiayu_login_bg1.png");
  }

  .choose_con {
    width: 600px;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    color: #ffffff;
    &.jiayuchoose_con {
      .title {
        color: #333333;
      }
      
      .btn_con {
        margin-top: 40px;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: row;
        height: 50px;
        .btn_view {
          width: 220px;
          height: 50px;
          line-height: 50px;
          background: #202e57;
          text-align: center;
          border-radius: 4px;
          cursor: pointer;
          font-size: 18px;
          background: linear-gradient( 90deg, #4C79FF 0%, #6DA8FF 100%);
          border:none;
        }
      }
    }
    .title {
      font-size: 36px;
      font-weight: 550;
    }
    .cont_con {
      margin-top: 50px;
      width: 600px;
      border-radius: 10px;
      background: #ffffff;
      padding: 50px;
      box-sizing: border-box;
    }
    .btn_con {
      margin-top: 40px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-direction: row;
      height: 50px;
      .btn_view {
        width: 220px;
        height: 50px;
        line-height: 50px;
        background: #202e57;
        text-align: center;
        border-radius: 4px;
        cursor: pointer;
        font-size: 16px;
      }
      .btn_view:hover {
        border: 1px solid #3572ff;
      }
    }
  }
}
</style>