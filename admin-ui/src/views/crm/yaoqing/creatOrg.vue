<template>
  <div class="invite_register jiayuinvite_register">
    <el-form ref="orgForm" :model="orgForm" :rules="orgRules" label-position="top" class="invite_register-form jiayu_form">
      <div class="form_con jiayuform_con">
        <div class="left_con" v-if="loginType=='default'">
          <div class="invite_register_title">
            <h2 class="title">{{yaoqingId?'输入团队或企业信息':'新建企业'}}</h2>
          </div>
          <div class="avatar_con">
            <el-form-item prop="logo" class="logoImg">
              <image-upload v-model="orgForm.logo" :limit="1">
                <template #tip>
                  <div style="text-align: center;margin-top: 30px;font-size: 16px;color: #3572FF">
                    <span style="text-decoration-line: underline">请上传企业logo</span>
                  </div>
                </template>
              </image-upload>
            </el-form-item>
          </div>
        </div>
        <div class="right_con">
          <el-form-item prop="orgName">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-qiyemingcheng" style="margin-right: 14px;color: #C1C1C1"></i>企业名称<span style="color:red !important;">*</span>
            </span>
            <el-input v-model="orgForm.orgName" type="text" auto-complete="off" placeholder="请输入企业名称">
            </el-input>
          </el-form-item>
          <el-form-item prop="industry">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-hangyeleixing" style="margin-right: 14px;color: #C1C1C1"></i>行业类型<span style="color:red !important;">*</span>
            </span>
            <el-cascader v-model="orgForm.industryArr" placeholder="请选择行业类型" :props="{ value: 'Id', label: 'Name', children: 'children' }"
              :options="industryLis" @change="choiceSize" style="width: 100%" v-if="loginType=='default'"></el-cascader>
          </el-form-item>
          <el-form-item prop="size">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-qiyeguimo" style="margin-right: 14px;color: #C1C1C1;"></i>使用规模<span style="color:red !important;">*</span>
            </span>
            <el-select v-model="orgForm.size" placeholder="请选择规模" style="width: 100%" v-if="loginType=='default'">
              <el-option v-for="item in scaleLis" :key="item.value" :label="item.label" :value="item.value"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item prop="addressName">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-qiyedizhi1" style="margin-right: 14px;color: #C1C1C1"></i>地址
            </span>
            <el-input placeholder="请选择企业所在地址" v-model="orgForm.addressName" @focus="choiceMap">
            </el-input>
            <el-input placeholder="请选择企业所在地址" v-model="orgForm.lng" style="display: none"></el-input>
            <el-input placeholder="请选择企业所在地址" v-model="orgForm.lat" style="display: none"></el-input>
            <el-input placeholder="请选择企业所在地址" v-model="orgForm.addressCode" style="display: none"></el-input>
            <el-input class="address_detail_inner" placeholder="请输入地址详情" v-model="orgForm.addressDetail" :style="{'margin-top':'20px'}"></el-input>
          </el-form-item>
          <!-- <el-form-item prop="addressDetail">
            <el-input class="address_detail_inner" placeholder="请输入地址详情" v-model="orgForm.addressDetail"></el-input>
          </el-form-item> -->
          <el-button :loading="loading" size="medium" type="primary" style="width: 100%" @click.native.prevent="creatOrg">
            <span v-if="!loading">完成创建</span>
            <span v-else>创 建 中...</span>
          </el-button>
        </div>
      </div>
    </el-form>
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
  </div>
</template>

<script>
import { creatCompany } from "@/api/system/company";
import { joinInvite } from "@/api/manufac/agentMansge.js";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
import {agentInviteInfo} from "@/api/manufac/agentMansge";
export default {
  name: "inviteRegistercreatOrg",
  components: { mapSelectCompt },
  dicts: ["org_size"],
  data() {
    const fileMustUpload = (rule, value, callback) => {
      if (this.orgForm.logo == null) {
        // 未上传文件
        callback("请上传企业logo");
      }
      callback();
    };
    return {
      openSelectMap: false, //是否打开选择地址的弹窗
      input3: "预计使用规模",
      codeUrl: "",
      cookiePassword: "",
      orgForm: {
        orgName: "",
        industryArr: null,
        industry: null,
        size: null,
        logo: '',
        addressName: "",
        addressCode: 0,
        lng: 0,
        lat: 0,
        addressDetail: "",
        intro: "",
      },
      orgRules: {
        orgName: [{ required: true, trigger: "blur", message: "请输入企业" }],
        industry: [
          { required: true, trigger: "change", message: "请选择行业" },
        ],
        size: [
          { required: true, trigger: "change", message: "请选择预计使用规模" },
        ],
        logo: [{ validator: fileMustUpload, trigger: "change" }],
        // addressDetail: [
        //   { required: true, trigger: "blur", message: "请输入地址详情" },
        //   { required: true, trigger: "change", message: "请输入地址详情" },
        // ],
        // addressName: [
        //   { required: true, trigger: "blur", message: "请选择企业所在地址" },
        //   { required: true, trigger: "change", message: "请选择企业所在地址" },
        // ],
      },
      loading: false,
      industryLis: [],
      scaleLis: [],
      center: null,
      map: null,
      search: null,
      suggest: null,
      markers: null,
      infoWindowList: Array(1),
      choiceAddress: {},
      isFirstDraw: true,
      geocoder: null,
      yaoqingId: 0, //邀请id
      loginType:'default',
    };
  },
  watch: {},
  beforeCreate() {
  },
  created() {
    
  },
  mounted() {
    let pars = this.$route.query;
    if (pars.yaoqingId) {
      this.yaoqingId = pars.yaoqingId;
      this.loadYaoqingInfo(this.yaoqingId)
    }
    this.getDatas();
  },
  methods: {
    loadYaoqingInfo(id){
      agentInviteInfo({id:id}).then(async res=>{
        console.log("邀请信息",res);
        let data=res.data
        if(data&&data.AddressCode){
          this.orgForm.addressName=data.AddressName;
          this.orgForm.addressCode=data.AddressCode;
          this.orgForm.lng=data.Lng;
          this.orgForm.lat=data.Lat;
          this.orgForm.addressDetail=data.AddressDetail;
        }
        if(data&&data.Industry){
          this.orgForm.industry=data.Industry
          this.orgForm.industryArr = await this.getIndustryArr(data.Industry);
        }
        if(data&&data.Size){
          this.orgForm.size=data.Size
        }
        if(data&&data.Logo){
          this.orgForm.logo=data.Logo
        }
        if(data&&data.OrgName){
          this.orgForm.orgName=data.OrgName
        }
      })
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
    choiceSize() {
      //选择行业类型
      // console.log("行业类型",this.orgForm.industry);
      this.orgForm.industry =
        this.orgForm.industryArr[this.orgForm.industryArr.length - 1];
    },
    returnMapInfo(info){
      if (info) {
        this.orgForm.addressName =info.AddressName;
        this.orgForm.addressDetail =info.AddressDetail;
        this.orgForm.addressCode =info.AddressCode;
        this.orgForm.lat =info.Lat;
        this.orgForm.lng =info.Lng;
      }
      // console.log("选择地址后的信息",this.orgForm);
      this.$forceUpdate()
    },
    choiceMap() {
      //打开地址选择的弹窗
      this.$refs.mapSelectCompt.choiceMap()
    },
    goOff() {
      this.$router.go(-1);
    },
    getDatas() {
      this.$store.dispatch("datas/industryTree").then((rt) => {
        // console.log("行业", rt);

        this.industryLis = rt;
      });
      this.scaleLis = this.dict.type.org_size;
    },
    creatOrg() {
      this.$refs.orgForm.validate((valid) => {
        if (valid) {
          this.loading = true;
          creatCompany(this.orgForm).then((res) => {
            console.log(res, "创建企业成功");
            if (res.code == 0) {
              this.loading = false;
              this.$modal.msgSuccess("创建成功");
              // this.$router.go(-1);
              if (res.data) {
                if (this.yaoqingId) {
                  joinInvite({
                    id: this.yaoqingId,
                    orgId: res.data,
                  }).then((rsp) => {
                    if (rsp.code == 0) {
                      let url =
                        window.location.protocol + "//" + window.location.host;
                      window.location.replace(url); //切换企业后刷新整个网站
                      return this.$store.dispatch("GetInfo");
                    }
                  });
                } else {
                  let url =
                    window.location.protocol + "//" + window.location.host;
                  window.location.replace(url); //切换企业后刷新整个网站
                  return this.$store.dispatch("GetInfo");
                }
              }

              // this.$router.push("/");
            }
          });
        }
      });
    },
  },
};
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
html {
  height: 100%;
  overflow: hidden;
  // background: red;
}


.imgCodeTips {
  height: 300px;
  top: calc((100% - 300px) / 2);
  .img_code_con {
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: row;
    ::v-deep .el-form-item__content {
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: row;
      ::v-deep .el-input {
        background-color: #ffffff;
        margin-right: 20px;
        color: #606266;
        input {
          border: 1px solid #5f6368;
          color: #606266;
        }
      }
    }
  }
  ::v-deep .el-button.el-button--default {
    background: #ffffff !important;
  }
}
.invite_register {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../../../assets/images/login-background.png");
  background-size: cover;
  width: 100%;
  &.jiayuinvite_register {
    background-image: url("../../../assets/images/jiayu_login_bg1.png");
  }

  .invite_register_title {
    width: 100%;
    display: flex;
    justify-content: flex-start;
    // position: fixed;
    // right: 17.2%;
    // top: 16%;
    height: 20px;
    line-height: 20px;
    margin-bottom: 10px;
    position: absolute;
    top: 50px;
    left: 50px;
    h2 {
      height: 20px;
      line-height: 20px;
      color: #ffffff;
      margin: 0;
      font-size: 25px;
    }
  }
}
$themes: (//动态设置样式
  'default': (
    --formlabelcolor: rgba(255, 255, 255, 0.8),//
    --formconbg: rgba(24, 36, 69, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(32, 46, 87, 1),
    --forminputcontentcolor: rgba(255, 255, 255, 1),
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:14px,
    --formlabelconcolor:#fff,
    --formlabelfontsize:20px,
    --subbtnradius:4px,
    --submitbtnheight:52px,
    --formitemmarginbottom:20px,
  ),
  'jiayulogin': (//LD项目样式
    --formlabelcolor: rgba(153, 153, 153, 1),
    --formconbg: rgba(255, 255, 255, 1),
    --formcontentcolor: rgba(120, 130, 157, 1),
    --forminputbg: rgba(248, 248, 248, 1),
    --forminputcontentcolor: #333333,
    --inputHeight: 48px,
    --opacity:0.5,
    --inputfontsize:16px,
    --formlabelconcolor:#999999,
    --formlabelfontsize:20px,
    --subbtnradius:4px,
    --submitbtnheight:52px,
    --formitemmarginbottom:20px,
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
.invite_register-form {
  width: 60%;
  
  @include apply-theme('default');
  &.jiayu_form{
    @include apply-theme('jiayulogin');
  }

  .form_con {
    background: var(--formconbg);
    width: 100%;
    height: 67%;
    border-radius: 10px;
    padding: 50px;
    box-sizing: border-box;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    align-items: center;
    position: relative;
    
    &.jiayuform_con {
      border-radius: 10px;
      box-shadow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04);
      .invite_register_title {
        h2 {
          color: #333333;
        }
      }
      .avatar_con {
        ::v-deep .el-upload--picture-card {
          background-color: #f8f8f8;
          border: none;
        }
        .upload_click {
          font-size: 14px;
          color: #3572ff;
        }
      }
      .count_down {
        background: #f8f8f8;
        padding: 0 5px;
        font-size: 14px;
        color: #999999;
      }
      .address_detail_inner {
        border-radius: 4px;
        .el-textarea__inner {
          background-color: #f8f8f8;
          border: none;
          border-radius: 4px;
          //   color: #9097AB;
          color: #333333;
          //   border: 1px solid #202e57;
          opacity: 1;
        }
      }
    }
    .invite_register-code {
      width: 33%;
      height: 48px;
      float: right;
      border-radius: 4px;
      background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
      text-align: center;
      line-height: 48px;
      color: #ffffff;
      overflow: hidden;
      span {
        display: block;
        width: 100%;
        height: 100%;
        text-align: center;
      }
      img {
        cursor: pointer;
        vertical-align: middle;
      }
    }
    .count_down {
      background: rgba(32, 46, 87, 1);
      padding: 0 5px;
      font-size: 14px;
    }
    .form_label_con {
      display: inline;
      width: 100%;
      color: var(--formlabelconcolor);
      font-size: var(--formlabelfontsize);
      .label_left {
        // display: inline-block;
        float: left;
      }
      .label_right {
        // display: inline-block;
        float: right;
      }
    }
    .form_label_con::after {
      clear: both;
    }
    .left_con {
      width: calc((100% - 50px) / 2);
      text-align: center;
      margin-right: 50px;
      height: 100%;
    }
    .right_con {
      width: calc((100% - 50px) / 2);
    }
    .avatar_con {
      width: 100%;
      text-align: center;
      .logoImg {
        .el-form-item__error {
          text-align: center;
        }
      }
      ::v-deep .el-upload--picture-card {
        background-color: #202e57;
        border: none;
      }
      .svg-wrap {
        margin-left: calc((100% - 100px) / 2);
        margin-bottom: 10px;
      }
      .el-form-item {
        display: flex;
        align-items: center;
        justify-content: center;
      }
      .upload_click {
        font-size: 14px;
        color: rgba(120, 130, 157, 1);
        text-decoration-line: underline;
        cursor: pointer;
      }
      .text_center {
        input {
          text-align: center;
        }
      }
      .el-form-item__error {
        min-width: 200px;
        text-align: left;
        left: calc((100% - 200px) / 2);
      }
    }
    ::v-deep .el-form-item{
      margin-bottom: var(--formitemmarginbottom) !important;
      .icon_content_con{
        display: flex;
        align-items: center;
        width: 100%;
        background-color: var(--forminputbg);
        border-radius: 10px;
        // padding-left: 20px;
        // box-sizing: border-box;
        position: relative;
        i.zhongtaiiconfont{
          position: absolute;
          left: 20px;
          z-index: 999999;
        }
      }
    }
    .icon_content_con{
      ::v-deep .el-cascader{
        width: 100%;
        box-sizing: border-box;
      }
      ::v-deep input.el-input__inner{
        padding-left: 46px !important;
        width: 100%;
        box-sizing: border-box;
      }
      // ::v-deep .el-input__inner:focus {
      //   border: none !important;
      // }
    }
    
    ::v-deep label.el-form-item__label::before {
      content: "" !important;
      height: 0 !important;
    }
    ::v-deep label.el-form-item__label {
      width: 100%;
      span {
        font-size: var(--formlabelfontsize) !important;
        color: var(--formlabelcolor) !important;
        font-weight: normal;
      }
    }
    ::v-deep .el-form-item__content {
      width: 100%;
      color: var(--formcontentcolor) !important;
      .el-select.text_center {
        width: 100%;
      }
    }
  }
  ::v-deep .el-input {
    height: var(--inputHeight);
    border-radius: 10px;
    background-color: var(--forminputbg);
    input {
      height: var(--inputHeight);
      border-radius: 10px;
      background-color: rgba(32, 46, 87, 0);
      color: var(--forminputcontentcolor);
      border: 1px solid var(--forminputbg);
      opacity: var(--opacity);
      font-size: var(--inputfontsize);
    }
  }
  ::v-deep .el-button {
    height: var(--submitbtnheight);
    border-radius: var(--subbtnradius);
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  }
  ::v-deep .el-input__inner:focus {
    border: 1px solid rgba(53, 114, 255, 1) !important;
  }
  ::v-deep .el-cascader:not(.is-disabled):hover .el-input__inner{
    border-color: var(--forminputbg);
  }
  .input-icon {
    height: 39px;
    width: 14px;
    margin-left: 2px;
  }
  .address_detail_inner {
    border-radius: 4px;
    .el-textarea__inner {
      background-color: #202e57;
      border: none;
      border-radius: 4px;
      //   color: #9097AB;
      color: rgba(255, 255, 255, 1);
      //   border: 1px solid #202e57;
      opacity: 0.5;
    }
  }
}
</style>
