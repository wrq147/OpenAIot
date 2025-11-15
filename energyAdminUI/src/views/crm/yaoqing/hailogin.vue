<template>
  <div class="choose_bg jiayuchoose_bg">
    <div class="choose_con jiayuchoose_con">
      <div class="title" v-if="loginType=='default'">
        后台管理系统
      </div>
      <div class="cont_con">
        <div>
          <el-select class="text_center" v-model="chooseCompany" placeholder="请选择要接受邀请的企业" clearable v-if="loginType=='default'">
            <el-option v-for="item in orgList" :key="item.Id" :label="item.OrgName" :value="item.Id"></el-option>
          </el-select>
        </div>
        <div class="btn_con">
          <div class="btn_view" @click.stop="joinOrg">接受邀请</div>
          <div class="btn_view" @click.stop="recreateNewOrg">创建新企业</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { joinInvite } from "@/api/manufac/agentMansge.js";
export default {
  name: "AdminUiChoose", //邀请的代理商是否已有账号选择

  data() {
    return {
      yaoqingId: 0,
      chooseCompany: "",
      orgList:[],
      loginType:'default',//登录类型
    };
  },
  computed: {
  },
  mounted() {
    let pars = this.$route.query;
    if(pars.code){
      this.$router.push("/InviteRegister?code=" + pars.code);
      return
    }
    if (pars.yaoqingId) {
      this.yaoqingId = pars.yaoqingId;
    }
    if (this.$store.getters.orgId > 0) {
      this.chooseCompany = this.$store.getters.orgId;
    }
    this.reqOrgLis()
  },

  methods: {
    async reqOrgLis() {
      this.$store.commit("orgLis/SET_ORG_LIST", null);
      let list = await this.$store.dispatch("orgLis/setOrgList");
      // console.log("企业列表111111", this.$store.state.orgLis.orgList);
      this.orgList= list;
      if(!this.orgList||this.orgList&&this.orgList.length==0){
        if (this.yaoqingId) {
          this.$router.push("/crm/yaoqing/creatOrg?yaoqingId=" + this.yaoqingId);
        }
      }
    },
    joinOrg() {
      //已有账号
      if (this.chooseCompany && this.chooseCompany != "") {
        joinInvite({
          id: Number(this.yaoqingId),
          orgId: this.chooseCompany,
        }).then(async (rsp) => {
          if (rsp.code == 0) {
            await this.$store.dispatch("GetInfo");
            let url = window.location.protocol + "//" + window.location.host;
            window.location.replace(url); //切换企业后刷新整个网站
          }
        });
      } else {
        this.$modal.msgError("请选择要接受邀请的企业");
      }
    },
    recreateNewOrg() {
      //重新创建账号
      if (this.yaoqingId) {
        this.$router.push("/crm/yaoqing/creatOrg?yaoqingId=" + this.yaoqingId);
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
      .cont_con {
        background: #ffffff;
        box-shadow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04);
        border-radius: 10px;
        opacity: 0.98;
        ::v-deep .el-select {
          width: 100%;
          background: #f8f8f8;
          height: 48px;
          // opacity: 0.5;
          line-height: 48px;
          border-radius: 10px;

          .el-input {
            input.el-input__inner {
              color: #333333;
              background: #f8f8f8;
              border: none;
              border-radius: 5px;
            }
          }
        }
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
          font-size: 18px;
          background: linear-gradient( 90deg, #4C79FF 0%, #6DA8FF 100%);
        }
        .btn_view:first-child {
          margin-top: 0;
          border: 1px solid #3572FF;
          background: #FFFFFF;
          color: #3572FF;
        }
        .btn_view:nth-child(2):hover {
          border: none;
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
      background: #182445;
      padding: 50px;
      box-sizing: border-box;
      ::v-deep .el-select {
        width: 100%;
        background: #202e57;
        height: 42px;
        // opacity: 0.5;
        line-height: 40px;

        .el-input {
          input.el-input__inner {
            color: #ffffff;
            background: #202e57;
            border: none;
            border-radius: 5px;
          }
        }
      }
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