<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="8" :xs="24">
        <!-- <el-card class="box-card"> -->
        <div class="infoList">
          <!-- <div slot="header" class="clearfix"> -->
          <div class="header">
            <span>个人信息</span>
          </div>
          <div>
            <div class="text-center">
              <userAvatar :user="user" />
            </div>
            <ul class="list-ul">
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-daichuli list-icon"></i>签名
                <div class="item-right" v-if="isEditSignature">
                  <el-input v-model="user.WaitSignature" placeholder="请输入工作签名"></el-input>
                  <el-button type="primary" icon="el-icon-close" @click="cancelChange" style="margin-left:2px;padding:12px;"></el-button>
                  <el-button type="primary" icon="el-icon-check" @click="finishChange" style="margin-left:2px;padding:12px;"></el-button>
                </div>
                <div class="item-right" style="padding:10px 0" v-else>{{ user.WaitSignature }}<i @click.stop="changeSignature" class="zhongtaiiconfont zhongtai-icon-xiugai list-edit-icon"></i></div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-yonghumingcheng list-icon"></i>用户名称
                <div class="item-right">{{ user.UserName }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-shoujihaoma1 list-icon"></i>手机号码
                <div class="item-right">{{ user.Mobile }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-yonghuyouxiang list-icon"></i>用户邮箱
                <div class="item-right">{{ user.Email }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-a-bumenguanli list-icon"></i>所属部门
                <div class="item-right" v-if="user.dept_id">{{ user.dept_name }} / {{ postGroup }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-suoshujuese list-icon"></i>所属角色
                <div class="item-right" style="padding:10px 0">{{ roleGroup }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-chuangjianriqi list-icon"></i>创建日期
                <div class="item-right">{{ user.create_time }}</div>
              </li>
            </ul>
          </div>
          <!-- </el-card> -->
        </div>
      </el-col>
      <el-col :span="16" :xs="24">
        <div class="infoList">
          <div class="header">
            <span>基本资料</span>
          </div>
          <el-tabs v-model="activeTab" class="userTabs">
            <el-tab-pane label="基本资料" name="userinfo" class="infoForm">
              <userInfo :user="user" />
            </el-tab-pane>
            <el-tab-pane label="绑定手机号" name="phonebind" class="infoForm">
              <bindPhone :user="user" ref="bindPhone"/>
            </el-tab-pane>
            <el-tab-pane label="绑定邮箱" name="emailbind" class="infoForm">
              <bindEmail :user="user" ref="bindEmail"/>
            </el-tab-pane>
            <el-tab-pane label="修改密码" name="resetPwd" class="infoForm">
              <resetPwd :user="user" />
            </el-tab-pane>
            <el-tab-pane label="开发者信息" name="developerInfo" class="infoForm" v-if="haiDevelope">
              <developerInfo :deveInfo="deveInfo" />
            </el-tab-pane>
          </el-tabs>
        </div>
      </el-col>
    </el-row>
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
export default {
  name: "Profile",
  components: { userAvatar, userInfo, resetPwd,bindPhone,bindEmail,developerInfo },
  data() {
    return {
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
        this.$refs.bindPhone.setBindPhone(val)
        this.$refs.bindEmail.setBindEmail(val)
      }
    }
  },
  methods: {
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
<style lang="scss" scope>
.ulabl {
  margin-right: 5px;
}
.infoList {
  background-color: #ffffff;
  padding: 15px 20px 20px;
  border-radius: 5px;
  font-size: var(--rightcon);
  .userTabs .el-tabs__item {
    font-size: var(--rightcon);
  }
  .userTabs .el-tabs__nav-wrap::after {
    background-color: #ffffff;
  }
  .infoForm .el-form {
    .el-form-item__label {
      font-size: var(--rightcon);
      padding-bottom: 0;
    }
  }
  .header {
    margin-bottom: 25px;
  }
  .list-ul {
    width: 100%;
    list-style: none;
    padding: 0;
    .list-li-item {
      width: 100%;
      text-align: left;
      color: #78829d;
      padding: 20px 0;
      line-height: var(--fsslde);
      border-bottom: 1px solid #f6f7fa;
      .list-icon {
        margin-right: 10px;
      }
      .list-edit-icon {
        margin-left: 10px;
        color:#999999;
        cursor: pointer;
      }
      .item-right {
        color: #333333;
        float: right;
        display: flex;
        justify-content: flex-end;
        align-items: center;
      }
    }
    .list-li-item::after {
      clear: both;
    }
  }
}
</style>