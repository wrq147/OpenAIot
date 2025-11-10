<template>
  <div style="background-color:#EDEFF2;" class="org-manage-con" ref="orgManage">
    <el-row :gutter="20">
      <el-col :span="8" :xs="24">
        <div class="infoList white-con">
          <div class="header">
            <span>企业信息</span>
          </div>
          <div>
            <div class="text-center">
              <orgLogo :org="org" ref="orglogo" @reLoadOrg="getOrgInfo"></orgLogo>
            </div>
            <ul class="list-ul">
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-qiyemingcheng list-icon"></i>企业名称
                <div class="item-right">{{ org.OrgName }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-qiyeguimo list-icon"></i>企业规模
                <div class="item-right">{{ org.SizeName }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-hangyeleixing list-icon"></i>行业类型
                <div class="item-right">{{ org.IndustryName }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-qiyedizhi list-icon"></i>企业地址
                <div
                  class="item-right address-li"
                  style="padding:10px 0 0 10px;float: none !important;text-align:right;"
                >{{ org.AddressName+org.AddressDetail }}</div>
              </li>
              <li class="list-li-item">
                <i class="zhongtaiiconfont zhongtai-icon-chuangjianriqi list-icon"></i>创建日期
                <div class="item-right">{{ org.createTime }}</div>
              </li>
              <li class="list-li-item" style="border-top:none">
                <i class="zhongtaiiconfont zhongtai-icon-gengxinriqi list-icon"></i>更新日期
                <div class="item-right">{{ org.updateTime }}</div>
              </li>
            </ul>
          </div>
        </div>
      </el-col>
      <el-col :span="16" :xs="24">
        <div class="infoList white-con">
          <el-tabs v-model="activeTab" class="userTabs">
            <el-tab-pane label="基本资料" name="userinfo" class="infoForm">
              <orgInfoAsse :org="org" @reLoadOrg="getOrgInfo" />
            </el-tab-pane>

            <el-tab-pane v-if="haiDevelope" label="开发者信息" name="developer" class="infoForm">
              <developerInfo :deveInfo="deveInfo" />
            </el-tab-pane>
            <el-tab-pane label="主题设置" name="theme" class="infoForm">
              <themeOrg
                ref="orgtheme"
                :org="org"
                @reLoadOrg="getOrgInfo"
                @reLoadsystem="reLoadsystem"
              />
            </el-tab-pane>
            <el-tab-pane v-if="checkPermission(['/AuthService/Holiday/List'])" label="假期设置" name="vacation" class="infoForm">
              <vacationConfig />
            </el-tab-pane>
            <el-tab-pane label="扩展操作" name="resetPwd" class="infoForm">
              <orgOtherFun
                ref="otherFun"
                :org="org"
                @reLoadOrg="getOrgInfo"
                @reLoadsystem="reLoadsystem"
              />
            </el-tab-pane>

          </el-tabs>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { checkPermi } from "@/utils/permission";
import orgInfoAsse from "./orgManage";
import orgOtherFun from "./orgOtherFun";
import themeOrg from "./themeOrg";
import vacationConfig from "./vacationConfig";
import { orgInfo, editCompany } from "@/api/system/company";
import navbar from "@/layout/components/Navbar";
import deptList from "@/views/system/dept/index.vue";
import employeeList from "@/views/system/Employee/index.vue";
import orgLogo from "./orgLogo";
import developerInfo from './developerInfo'
import {devProfile} from '@/api/dev'
export default {
  name: "Profile",
  components: {
    orgInfoAsse,
    navbar,
    employeeList,
    deptList,
    orgOtherFun,
    orgLogo,
    developerInfo,
    themeOrg,
    vacationConfig
  },
  dicts: ["org_size"],
  data() {
    return {
      isLoadingOrg: false,
      openeds: ["1", "2"],
      org: {},
      roleGroup: {},
      postGroup: {},
      activeTab: "userinfo",
      orgId: 0,
      activePath: "",
      haiDevelope:false,
      deveInfo:{}//开发者信息
    };
  },
  mounted() {
    this.orgId = this.$store.state.user.orgId;
    this.getOrgInfo();
    this.activePath = "1-1";
    this.getDevProfile();
  },
  watch: {
    "$store.getters.size"() {

      this.setFontChange();
    },
    "$store.state.user.orgId"() {
      this.orgId = this.$store.state.user.orgId; //切换企业后，企业发生变化
      this.getOrgInfo();
    },
    org: {
      handler(val, oldVal) {
        this.$refs.otherFun.getBelongUser(val);
        this.$refs.orglogo.setOrgInfo(val);
        this.$refs.orgtheme.getOrgTheme(val);
        this.$store.commit("orgLis/SET_ORG_LIST", null);
      }
    }
  },
  updated() {
    this.setFontChange();
  },
  methods: {
    getDevProfile(){
      //获取开发者的信息
      devProfile().then(res=>{
        // console.log('开发者的信息',res);
        if(res.data){
          if(res.data.UserType==1){
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
    checkPermission(perms) {
      return checkPermi(perms);
    },
    reLoadsystem() {
      //删除整个企业后直接刷新整个网站
      let url = window.location.protocol + "//" + window.location.host;
      window.location.replace(url); //切换企业后刷新整个网站
    },
    async getIndustryArr(Industry) {
      //获取行业规模数组
      let arr = await this.$store.dispatch("datas/industryTree");
      let isFinish = false;
      let rsArray = [];
      for (let index = 0; index < arr.length; index++) {
        if (arr[index].children && arr[index].children.length > 0) {
          for (let ix = 0; ix < arr[index].children.length; ix++) {
            if (arr[index].children[ix].Id == Industry) {
              rsArray = [];
              rsArray.push(arr[index].children[ix].ParentId);
              rsArray.push(arr[index].children[ix].Id);
              return rsArray;
              // this.org.industryArr = JSON.parse(JSON.stringify(rsArray));
            }
          }
          if (isFinish) {
            return rsArray;
          }
        }
      }
    },
    getOrgInfo() {
      this.isLoadingOrg = true;
      orgInfo({ id: this.orgId })
        .then(async response => {
          if (response.code == 0 && response.data) {
            if (response.data.Industry) {
              this.$store.commit("datas/SET_INDUSTRY", response.data.Industry);
              response.data.IndustryName = await this.$store.dispatch(
                "datas/industryName"
              );
              response.data.industryArr = await this.getIndustryArr(
                response.data.Industry
              ); //获取行业类型的值
            } else {
              response.data.IndustryName = "";
              response.data.industryArr = [];
            }
            if (response.data.Size) {
              response.data.Size = response.data.Size + "";
              response.data.SizeName = this.dict.getName("org_size",response.data.Size);
            } else {
              response.data.SizeName = "";
            }
            this.org = JSON.parse(JSON.stringify(response.data));
          }
          this.isLoadingOrg = false;
        })
        .catch(err => {
          console.log(err);
          this.isLoadingOrg = false;
        });
    },
    setFontChange() {
      if (
        this.$store.getters.size == "default" ||
        this.$store.getters.size == "medium"
      ) {
        // console.log("进来medium");
        this.$refs.orgManage.style.setProperty("--fsslde2", "14px");
        this.$refs.orgManage.style.setProperty("--rightcon2", "14px");
        this.$refs.orgManage.style.setProperty("--fsmini2", "16px");
      } else if (this.$store.getters.size == "small") {
        // console.log("进来small");
        this.$refs.orgManage.style.setProperty("--fsslde2", "12px");
        this.$refs.orgManage.style.setProperty("--rightcon2", "12px");
        this.$refs.orgManage.style.setProperty("--fsmini2", "14px");
      } else if (this.$store.getters.size == "mini") {
        // console.log("进来mini");
        this.$refs.orgManage.style.setProperty("--fsslde2", "10px");
        this.$refs.orgManage.style.setProperty("--rightcon2", "10px");
        this.$refs.orgManage.style.setProperty("--fsmini2", "10px");
      } else {
        // console.log("进来其他");
        this.$refs.orgManage.style.setProperty("--fsslde2", "14px");
        this.$refs.orgManage.style.setProperty("--rightcon2", "14px");
        this.$refs.orgManage.style.setProperty("--fsmini2", "16px");
      }
    }
  }
};
</script>
<style lang="scss" scope>
.org-manage-con {
  //   padding-left: 20px;
  box-sizing: border-box;
  --fsmini2: 16px;
  --rightcon2: 14px;
  --fsslde2: 14px;
  padding: 15px 0 0 15px;
}

.text-center {
  display: flex;
  justify-content: center;
  align-items: center;
}
.list-icon {
  margin-right: 5px;
  clear: both !important;
}
.logo-uploader {
  width: 120px;
  height: 120px;
  position: relative;
  .orgLogo {
    width: 120px;
    height: 120px;
    border-radius: 50%;
  }
  .upload-up {
    display: none;
    border-radius: 50%;
    position: absolute;
    top: 0;
    left: 0;
    width: 120px;
    height: 120px;
    line-height: 120px;
    text-align: center;
    background-color: rgba(222, 225, 230, 0.5);
    color: #ffffff;
  }
}
.logo-uploader:hover {
  .upload-up {
    display: block;
  }
}
.infoList {
  background-color: #ffffff;
  padding: 15px 20px 20px;
  border-radius: 5px;
  font-size: var(--rightcon2);
  .userTabs .el-tabs__item {
    font-size: var(--rightcon2);
  }
  .userTabs .el-tabs__nav-wrap::after {
    background-color: #ffffff;
  }
  .infoForm {
    font-size: var(--rightcon2);
  }
  .infoForm .el-form {
    .el-form-item__label {
      font-size: var(--rightcon2);
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
      font-size: var(--fsslde1);
      border-bottom: 1px solid #f6f7fa;
      .list-icon {
        margin-right: 10px;
      }
      .item-right {
        color: #333333;
        float: right;
      }
    }
    .list-li-item::after {
      clear: both;
    }
  }
}
</style>