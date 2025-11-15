<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div class="elbiaoge_elform tab_con" :style="{'min-height':tableConHeight-20+'px'}">
      <div class="tabs_ul">
        <div class="tabs_li" :class="{'active':activeTabs=='baseInfo'}" @click="setActiveTabs('baseInfo')">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='baseInfo'">
          <span>基础信息</span>
        </div>
        <div class="tabs_li" :class="{'active':activeTabs=='setting'}" @click="setActiveTabs('setting')">
          <img src="@/assets/images/zs.png" alt="" v-if="activeTabs=='setting'">
          <span>安全设置</span>
        </div>
      </div>
      <div class="cont_con" :style="{'min-height':tableConHeight-75+'px'}">
        <div class="base_con" :style="{'min-height':tableConHeight-119+'px'}" v-show="activeTabs=='baseInfo'">
          <div class="base_left">
            <div class="titile_text">头像</div>
            <div class="text-center">
              <userAvatar ref="userAvatar" :user="user" />
            </div>
            <div class="upload_bth" @click="editAvatar">
              <i class="zhongtaiiconfont zhongtai-icon-shangchuan"></i>
              <span>上传头像</span>
            </div>
          </div>
          <div class="base_right">
            <userInfo :user="user"></userInfo>
          </div>
        </div>
        <div class="setting_con" v-show="activeTabs=='setting'">
          <div class="setting_li">
            <div class="setting_li_left">
              <div class="setting_icon">
                <i class="zhongtaiiconfont zhongtai-icon-mima"></i>
              </div>
              <div class="setting_text">
                <div class="text_title">账号密码</div>
                <div class="text_content">为了您的账号安全，建议使用包含字母，符号或数字且长度超过8位的密码</div>
              </div>
            </div>
            <div class="setting_li_right">
              <el-button type="primary" class="saveInfoBtn" @click="openChangePassword">修改密码</el-button>
            </div>
          </div>
          <div class="setting_li">
            <div class="setting_li_left">
              <div class="setting_icon">
                <i class="zhongtaiiconfont zhongtai-icon-shoujihao"></i>
              </div>
              <div class="setting_text">
                <div class="text_title">绑定手机号</div>
                <div class="text_content" v-if="user.Mobile">当前绑定手机：{{user.Mobile}}</div>
              </div>
            </div>
            <div class="setting_li_right">
              <el-button type="primary" class="saveInfoBtn" @click="changeBinding">更换绑定</el-button>
            </div>
          </div>
        </div>
      </div>
      <el-dialog :visible.sync="bindingDialog" class="adddialog" :show-close="false" width="400px">
        <div slot="title" class="dialog_title">
          <div class="dialog_title_left">
            <img src="@/assets/images/zs.png" alt="">
            <span>更换绑定</span>
          </div>
          <div class="dialog_title_right" @click.stop="bindingDialog=false">
            <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
          </div>
        </div>
        <bindPhone @closeDialog="bindingDialog=false" @successDialog="successDialog"></bindPhone>
        <!-- <div slot="footer" class="dialog-footer">
          <el-button @click="passwordDialog = false">取消</el-button>
          <el-button type="primary" @click="finishPasswordSubmit" v-loading="submintLoad">保存</el-button>
        </div> -->
      </el-dialog>
      <el-dialog :visible.sync="passwordDialog" class="adddialog" :show-close="false" width="400px">
        <div slot="title" class="dialog_title">
          <div class="dialog_title_left">
            <img src="@/assets/images/zs.png" alt="">
            <span>修改密码</span>
          </div>
          <div class="dialog_title_right" @click.stop="passwordDialog=false">
            <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
          </div>
        </div>
        <resetPwd @closeDialog="passwordDialog=false" @successDialog="successDialog"></resetPwd>
        <!-- <div slot="footer" class="dialog-footer">
          <el-button @click="passwordDialog = false">取消</el-button>
          <el-button type="primary" @click="finishPasswordSubmit" v-loading="submintLoad">保存</el-button>
        </div> -->
      </el-dialog>
    </div>
  </div>
</template>

<script>
import userAvatar from "./userAvatar";
import userInfo from "./userInfo";
import resetPwd from "./resetPwd";
import { getUserProfile,updateUserProfile } from "@/api/system/user";
import bindPhone from "./bindPhone"
import bindEmail from "./bindEmail"
import developerInfo from './developerInfo'
import {devProfile} from '@/api/dev'
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "Profile",
  mixins: [resizeTableCon],
  components: { userAvatar, userInfo, resetPwd,bindPhone,bindEmail,developerInfo },
  data() {
    return {
      bindingDialog:false,
      submintLoad:false,
      passwordDialog:false,
      activeTabs:'baseInfo',
      user: {},
      roleGroup: {},
      postGroup: {},
      activeTab: "userinfo",
      haiDevelope:false,
      deveInfo:{},//开发者信息
      isEditSignature:false,//是否是在修改签名
    };
  },
  created() {
    if(this.$route.query.target!=null){
      this.activeTab=this.$route.query.target;
    }
    this.getUser();
    this.getDevProfile()
  },
  watch:{
    user:{
      handler(val,oldVal){
        // console.log(val,oldVal);
        // this.$refs.bindPhone.setBindPhone(val)
        // this.$refs.bindEmail.setBindEmail(val)
      }
    }
  },
  methods: {
    successDialog(){
      this.getUser();
    },
    changeBinding(){
      this.bindingDialog=true
    },
    openChangePassword(){
      this.passwordDialog=true
    },
    editAvatar(){
      this.$refs.userAvatar.editCropper()
    },
    setActiveTabs(val){
      this.activeTabs=val
    },
    finishChange(){
      //完成修改签名操作
      updateUserProfile({waitSignature:this.user.WaitSignature,signature:this.user.WaitSignature}).then(response => {
          this.$modal.msgSuccess("修改成功");
          this.isEditSignature=false
      });
    },
    cancelChange(){
      //取消修改签名操作
      this.isEditSignature=false
    },
    changeSignature(){
      //修改签名
      this.isEditSignature=true
    },
    getDevProfile(){
      //获取开发者的信息
      devProfile().then(res=>{
        // console.log('开发者的信息',res);
        if(res.data){
          if(res.data.UserType==0){
            this.haiDevelope=true
          }
          else{
            this.haiDevelope=false
          }
          this.deveInfo=res.data
        }else{
          this.haiDevelope=false
        }
        
      }).catch(err=>{ 
        this.haiDevelope=false
      })
    },
    getUser() {
      getUserProfile().then(response => {
        this.user = response.data.user;
        this.roleGroup = response.data.roleGroup;
        this.postGroup = response.data.postGroup;
        // console.log("用户信息", this.user);
      });
    }
  }
};
</script>
<style lang="less" scoped>
.adddialog{
  ::v-deep .el-dialog__header{
    padding: 0;
    color: #ffffff;
  }
  ::v-deep .el-dialog__body{
    padding: 24px 20px 20px;
  }
}

.dialog_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  .dialog_title_left{
    font-size: 16px;
    color: #ffffff;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    padding-left: 20px;
    line-height: 16px;
    height: 56px;
    .img{
      width: 16px;
      height: 16px;
    }
    span{
      margin-left: 6px;
    }
  }
  .dialog_title_right{
    margin-right: 20px;
    cursor: pointer;
    i.zhongtaiiconfont{
      color: rgba(255, 255, 255, 0.60);
      font-size: 12px;
    }
  }
}
.ulabl {
  margin-right: 5px;
}
.elbiaoge_elform.tab_con{
  padding: 0 20px;
  .tabs_ul{
    width: 100%;
    display: flex;
    align-items: center;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    height: 55px;
    
    .tabs_li{
      line-height: 16px;
      display: flex;
      align-items: center;
      margin-right: 32px;
      cursor: pointer;
      img{
        width: 16px;
        height: 16px;
        margin-right: 8px;
      }
      span{
        font-size: 16px;
        color: rgba(255, 255, 255, 0.6);
      }
      &.active{
        span{
          color: rgba(255, 255, 255, 1);
        }
      }
    }
  }
  .cont_con{
    // width: calc(100% - 170px);
    width: 100%;
    padding-top: 24px;
    height: 100%;
    .base_con{
      width: 100%;
      display: flex;
      height: 100%;
      .base_left{
        width: 228px;
        border-right: 1px solid rgba(255, 255, 255, 0.1);
        display: flex;
        flex-direction: column;
        align-items: center;
        // justify-content: center;
        .titile_text{
          width: 100%;
          text-align: left;
          font-size: 14px;
          color: rgba(255, 255, 255, 0.60);
        }
        .upload_bth{
          width: 96px;
          height: 32px;
          background: rgba(34, 46, 64, 1);
          font-size: 14px;
          color: rgba(255, 255, 255, 1);
          text-align: center;
          line-height: 32px;
          margin-top: 24px;
          cursor: pointer;
          i{
            font-size: 14px;
            margin-right: 6px;
          }
        }
      }
      .base_right{
        width: calc(100% - 228px);
        padding-left: 40px;
        box-sizing: border-box;
      }
    }
    .setting_con{
      margin-top: -24px;
      .setting_li{
        display: flex;
        justify-content: space-between;
        align-items: center;
        height: 88px;
        border-bottom:1px solid rgba(255, 255, 255, 0.1);
        .setting_li_right{
          .saveInfoBtn.el-button {
            background: rgba(61, 185, 143, 1);
            // background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
            border: none;
            width: 130px;
            height: 40px;
            &.el-button--primary:focus, &.el-button--primary:hover{
              background: rgba(61, 185, 143, 1);
              border: none;
            }
          }
        }
        
        .setting_li_left{
          display: flex;
          justify-content: space-between;
          align-items: center;
          .setting_icon{
            width: 40px;
            height: 40px;
            background: rgba(34, 46, 64, 1);
            border-radius: 50%;
            color: rgba(255, 255, 255, 0.6);
            display: flex;
            align-items: center;
            justify-content: center;
          }
          .setting_text{
            font-size: 14px;
            margin-left: 16px;
            .text_title{
              font-weight: bold;
              color: rgba(255, 255, 255, 1);
              line-height: 14px;
            }
            .text_content{
              color: rgba(255, 255, 255, 0.6);
              line-height: 14px;
              margin-top: 12px;
            }
          }
        }
      }
    }
  }
}
</style>