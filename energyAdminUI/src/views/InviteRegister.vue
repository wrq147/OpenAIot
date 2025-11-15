<template>
  <div class="invite_register jiayuinvite_register" :class="loginType=='cziot' ? 'cziotlogin' : ''">
    <el-form ref="inviteRegisterForm" :model="inviteRegisterForm" :rules="inviteRegisterRules" label-position="top" class="invite_register-form jiayu_form" :class="loginType=='cziot' ? 'cziotlogin_form' : ''">
      <div class="jiayulogin_title" v-if="loginType=='default'">
        <h2 class="title">后台管理系统</h2>
      </div>
      <div class="jianbianbg" v-if="loginType=='cziot'"></div>
      <div class="wel_con" v-if="loginType=='default'">WELCOME!</div>
      <div class="form_con jiayuform_con" :style="{ width: showOrgInfo&&loginType=='default'?'calc(50% + 50px)':'100%' }">
        <div v-if="showOrgInfo&&loginType=='cziot'||loginType=='default'" class="left_con" :class="loginType=='cziot' ? 'cziotleft_con' : ''" :style="{ width: showOrgInfo?'100%':'calc((100% - 50px) / 2)','margin-right':showOrgInfo?'0':'50px' }">
          <div class="login_logo" v-if="showOrgInfo&&loginType=='cziot'">
            <img src="../assets/images/logo.png" alt="">
          </div>
          <div class="invite_register_title" v-if="loginType=='default'||showOrgInfo&&loginType=='cziot'">
            <h2 class="title">{{showOrgInfo?'员工邀请':'注册账号'}}</h2>
          </div>
          <div class="invite_company" v-if="showOrgInfo&&loginType=='cziot'">
            {{ inviteRegisterForm.company }}
          </div>
          <div class="avatar_con" style="height:100%">
            <img v-if="loginType=='default'||loginType=='cziot'&&showOrgInfo" :src="inviteRegisterForm.avatar" style="width: 100px; height: 100px; margin-bottom: 10px" @click.stop="changeDiffentAvatar"/>
            <div class="invite_name" v-if="loginType=='cziot'">{{inviteRegisterForm.realName}}</div>
            <div v-if="!showOrgInfo" class="svg-wrap" style="width: 100px;height: 100px;border-radius: 50%;margin-bottom: 10px;position: fixed;left: 200%;top: 200%;">
              <avataaars :key="timer"></avataaars>
            </div>
            <el-upload v-if="loginType=='default'&&!showOrgInfo" class="avatar-uploader" ref="upload" action="/" :show-file-list="false" :before-upload="handleBeforeUpload" :http-request="handMove">
              <div><span class="upload_click">点击上传头像</span></div>
            </el-upload>
            <div style="width: 100%; height: 20px" v-if="loginType=='default'&&showOrgInfo"></div>
            <el-form-item prop="realName" v-if="loginType=='default'&&showOrgInfo">
              <el-input v-model="inviteRegisterForm.realName" type="text" auto-complete="off" placeholder="请输入姓名" disabled></el-input>
            </el-form-item>
            <el-form-item v-if="showOrgInfo" prop="deptId">
              <div v-if="loginType=='default'" style="font-size: 14px; margin-top: 0px; margin-bottom: 20px;color: #333333">{{ inviteRegisterForm.company }} </div>
              <el-select v-if="loginType=='default'" class="text_center" v-model="inviteRegisterForm.deptId" placeholder="请选择部门" clearable>
                <el-option v-for="item in deptOptions" :key="item.id" :label="item.label" :value="item.id"></el-option>
              </el-select>
              <div class="icon_content_con" v-if="loginType=='cziot'">
                <i class="zhongtaiiconfont zhongtai-icon-qiyedizhi" style="color: #C1C1C1;z-index:9;"></i>
                <el-select class="text_center" v-model="inviteRegisterForm.deptId" placeholder="请选择部门" clearable>
                  <el-option v-for="item in deptOptions" :key="item.id" :label="item.label" :value="item.id"></el-option>
                </el-select>
              </div>
            </el-form-item>
            <el-form-item v-if="showOrgInfo" prop="postName">
              <el-input class="text_center" v-model="inviteRegisterForm.postName" type="text" auto-complete="off" placeholder="请输入职位" style="min-width: 217px">
                <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhiwei" :style="{'color':'#C1C1C1'}" v-if="loginType=='cziot'"></i>
              </el-input>
            </el-form-item>
            <el-form-item style="width: 100%; padding-top: 0; margin-bottom: 0" v-if="showOrgInfo">
              <el-button :loading="loading" size="medium" type="primary" style="width: 100%" @click.native.prevent="handleJoinOrg('inviteRegisterForm')">
                <span v-if="!loading">加入邀请</span>
                <span v-else>加入中...</span>
              </el-button>
            </el-form-item>
          </div>
        </div>
        <div class="right_con" v-if="!showOrgInfo">
          <div class="login_logo" v-if="loginType=='cziot'">
            <img src="../assets/images/logo.png" alt="">
          </div>
          <div class="invite_register_title" v-if="loginType=='cziot'">
            <h2 class="title">{{showOrgInfo?'员工邀请':'注册账号'}}</h2>
          </div>
          <el-form-item prop="realName" v-if="loginType=='default'">
            <span slot="label">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhanghao" style="margin-right: 14px;color: #C1C1C1"></i>姓名
            </span>
            <el-input v-model="inviteRegisterForm.realName" type="text" auto-complete="off" placeholder="请输入姓名">
            </el-input>
          </el-form-item>
          <div class="avatar_name_con" v-if="loginType=='cziot'">
            <div v-if="!showOrgInfo" class="svg-wrap" style="width: 100px;height: 100px;border-radius: 50%;margin-bottom: 10px;position: fixed;left: 200%;top: 200%;">
              <avataaars :key="timer"></avataaars>
            </div>
            <div class="avatar_con">
              <img class="avatar_img" :src="inviteRegisterForm.avatar"  @click.stop="changeDiffentAvatar"/>
              <img class="xiangji" src="../assets/images/xiangji_icon.png" alt="">
            </div>
            <el-input v-model="inviteRegisterForm.realName" type="text" auto-complete="off" placeholder="请输入姓名">
            </el-input>
          </div>
          <el-form-item prop="mobile" label-width="100%" v-if="isphoneReg">
            <div class="form_label_con" slot="label" v-if="loginType=='default'">
              <div class="label_left">
                <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-shoujihao" style="margin-right: 14px;color: #C1C1C1"></i>手机号
              </div>
              <div v-if="canEmailReg" class="label_right" style="cursor: pointer" @click.stop="changeRegType(false)">
                <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-a-qiehuan-icon" style="margin-right: 14px;color: #C1C1C1"></i>切换为邮箱
              </div>
            </div>
            <el-input v-model="inviteRegisterForm.mobile" type="text" auto-complete="off" placeholder="请输入手机号">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-zhanghao" :style="{'color':'#C1C1C1'}" v-if="loginType=='cziot'"></i>
              <div slot="suffix" v-if="loginType=='cziot'" class="input_suffix_con">
                <i class="zhongtaiiconfont zhongtai-icon-a-qiehuan-icon"></i>
                <el-button class="input_suffix_button qiehuan" @click="changeRegType(false)">
                  <span style="color:rgba(153, 153, 153, 1);">使用邮箱</span>
                </el-button>
              </div>
            </el-input>
          </el-form-item>
          <el-form-item prop="email" label-width="100%" v-if="!isphoneReg && canPhoneReg">
            <div class="form_label_con" slot="label" v-if="loginType=='default'">
              <div class="label_left">
                <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-shoujihao" style="margin-right: 14px;color: #C1C1C1"></i>邮箱
              </div>
              <div v-if="canPhoneReg" class="label_right" style="cursor: pointer" @click.stop="changeRegType(true)">
                <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-a-qiehuan-icon" style="margin-right: 14px;color: #C1C1C1"></i>切换为手机号
              </div>
            </div>
            <el-input v-model="inviteRegisterForm.email" type="text" auto-complete="off" placeholder="请输入邮箱">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yonghuyouxiang" :style="{'color':'#C1C1C1'}" v-if="loginType=='cziot'"></i>
              <div slot="suffix" v-if="loginType=='cziot'" class="input_suffix_con">
                <i class="zhongtaiiconfont zhongtai-icon-a-qiehuan-icon"></i>
                <el-button class="input_suffix_button qiehuan" @click="changeRegType(true)">
                  <span style="color:rgba(153, 153, 153, 1);">使用手机号</span>
                </el-button>
              </div>
            </el-input>
          </el-form-item>
          <el-form-item prop="mobileCode" v-if="captchaOnOff && isphoneReg">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" style="margin-right: 14px;color: #C1C1C1"></i>验证码
            </span>
            <el-input v-model="inviteRegisterForm.mobileCode" auto-complete="off" placeholder="请输入验证码" :style="{width: loginType=='default'?'63%':'100%'}">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'#C1C1C1'}" v-if="loginType=='cziot'"></i>
              <div slot="suffix" v-if="loginType=='cziot'" class="input_suffix_con">
                <div class="input_suffix_line"></div>
                <el-button class="input_suffix_button"  :class="codeState!='normal' ? 'disabled-button' : ''" :disabled="codeState!='normal'" @click="getMobileCode">
                  <span v-if="codeState == 'normal'">获取验证码</span>
                  <span v-else>{{ "重新发送(" + currSecord + "s)" }}</span>
                </el-button>
              </div>
            </el-input>
            <div :class="{'invite_register-code': true,count_down: codeState != 'normal',}" v-if="loginType=='default'">
              <span v-if="codeState == 'normal'" @click="getMobileCode">获取验证码</span>
              <span v-else>{{ "重新发送(" + currSecord + "s)" }}</span>
            </div>
          </el-form-item>
          <el-form-item prop="password">
            <span slot="label" v-if="loginType=='default'">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" style="margin-right: 14px;color: #C1C1C1"></i>密码
            </span>
            <el-input v-model="inviteRegisterForm.password" type="password" auto-complete="off" placeholder="请输入密码">
              <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-mima" :style="{'color':'#C1C1C1'}" v-if="loginType=='cziot'"></i>
            </el-input>
          </el-form-item>
          <el-form-item prop="readAgreement">
            <el-checkbox-group v-model="inviteRegisterForm.readAgreement" style="height: 26px">
              <el-checkbox style="margin: 0px 0px 22px 0px">
                <span style="font-size: 12px">
                  已阅读或同意
                  <span style="color: rgba(24, 144, 255, 1)">服务协议</span>
                </span>
              </el-checkbox>
            </el-checkbox-group>
          </el-form-item>

          <el-form-item style="width: 100%; padding-top: 0; margin-bottom: 0">
            <el-button :loading="loading" size="medium" type="primary" style="width: 100%"
              @click.native.prevent="handleinviteRegister('inviteRegisterForm')">
              <span v-if="!loading">立即注册</span>
              <span v-else>注 册 中...</span>
            </el-button>
          </el-form-item>
        </div>
      </div>
      
    </el-form>
    <el-dialog title="填写图形验证码" class="imgCodeTips" :show-close="false" :visible.sync="imgCodeShow" width="400px" :modal-append-to-body="false"
        :close-on-click-modal="false" center>
        <div class="img_code_con">
          <el-input v-model="imgCode" type="text" placeholder="请输入图形验证码" style="width: 240px"></el-input>
          <div class="login-code" style="display: inline-flex;align-items: center;justify-content: center;height: 40px;">
            <img :src="imgImg" style="height: 40px" @click="getImgCode" class="login-code-img"/>
          </div>
        </div>
        <span slot="footer" class="dialog-footer">
          <el-button @click="dialogHide">取 消</el-button>
          <el-button type="primary" @click="getInputCode">确 定</el-button>
        </span>
      </el-dialog>
    <el-dialog title="账号激活" :visible.sync="activeDialogVisible" width="590px" v-loading="activeLoading">
      <div class="active_email_con">请登录邮箱<span class="email_text">{{ activeEmailText }}</span> 激活账号
      </div>
      <span slot="footer" class="active_dialog-footer">
        <el-button class="cancel_btton" v-if="activeDaoji">重新发送激活邮件 {{ daojiNum }}s</el-button>
        <el-button class="cancel_btton noDaoji" v-if="!activeDaoji" @click="reSendEmail">重新发送激活邮件</el-button>
        <el-button class="confrim_button" type="primary" @click="confirm">已激活</el-button>
      </span>
    </el-dialog>
    <!--  底部  -->
    <!-- <div class="el-invite_register-footer">
      <span>Copyright © 2021 悟空云 All Rights Reserved.</span>
    </div>-->
  </div>
</template>

<script>
import { getConfigKey } from "@/api/system/config.js";
import Avataaars from "vuejs-avataaars";
import {
  explainCode,
  sendMobileCode,
  memberLogon,
  emailReg,
  sendImgCode,
  sendEmailCode,
  staffJoinOrg
} from "@/api/system/Employee";
import { getUserInfo } from "@/api/login";
import { setToken, setRefreshToken } from "@/utils/auth";
export default {
  name: "inviteRegister",
  components: {
    Avataaars,
  },
  data() {
    return {
      isphoneReg: true, //是否是手机号注册
      fileList: [],
      // 大小限制(MB)
      fileSize: 10,
      // 文件类型, 例如['png', 'jpg', 'jpeg']
      fileType: ["png", "jpg", "jpeg", "gif"],
      // 部门树选项
      deptOptions: [],
      codeUrl: "",
      cookiePassword: "",
      inviteRegisterForm: {
        realName: "",
        password: "",
        readAgreement: [], //验证是否同意服务协议
        mobileCode: "",
        deptId: null,
        mobile: "",
        email: "",
        company: "",
        avatar: "",
        postName: "",
        code: "", //邀请码
      },
      inviteRegisterRules: {
        realName: [
          { required: true, trigger: "blur", message: "请输入您的姓名" },
        ],
        postName: [
          { required: true, trigger: "blur", message: "请输入您的职位" },
        ],
        mobile: [
          { required: true, trigger: "blur", message: "请输入您的手机号" },
        ],
        email: [{ required: true, trigger: "blur", message: "请输入您的邮箱" }],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        mobileCode: [
          { required: true, trigger: "blur", message: "请输入验证码" },
        ],
        readAgreement: [
          {
            type: "array",
            required: true,
            trigger: "change",
            message: "请阅读服务协议",
          },
        ],
        deptId: [{ required: true, trigger: "change", message: "请选择部门" }],
      },
      loading: false,
      // 验证码开关
      captchaOnOff: true,
      currSecord: 0, //验证码倒计时
      codeState: "normal", //是否重新发送验证码
      imgCode: "", //图形验证码
      uuid: "", //图形验证码编号
      imgImg: "", //图形链接
      imgCodeShow: false, //图形弹窗是否显示
      timer: "",
      showOrgInfo: false,
      canEmailReg: false,
      canPhoneReg: false,
      activeLoading: false,
      activeEmailText: "",
      activeDialogVisible: false,
      activeDaoji: false,
      daojiNum: 0,
      loginType:'cziot',
    };
  },
  watch: {},
  async created() {
    // if(process.env.VUE_APP_ICON == "/icons/cziot.ico"){
    //   this.loginType='cziot'
    // }else{
    //   this.loginType='default'
    // }
    let rsp = await getConfigKey("reg.way");
    console.log(rsp,'rsprsp');
    if (rsp.data != null && rsp.data != "") {
      this.isphoneReg = rsp.data.indexOf("phone") == 0;
      this.canEmailReg = rsp.data.indexOf("email") >= 0;
      this.canPhoneReg = rsp.data.indexOf("phone") >= 0;
    } else {
      this.canEmailReg = true;
      this.canPhoneReg = true;
    }
  },
  async mounted() {
    if (this.$route.query) {
      let pars = this.$route.query;
      // pars=decodeURIComponent(pars)

      // console.log("获取到的链接的参数", pars);
      if (pars.code) {
        if(this.$store.state.user.uid){
          this.inviteRegisterForm.avatar=this.$store.state.user.avatar
          this.inviteRegisterForm.realName=this.$store.state.user.name
        }else{
          
          try{
            let personInfo=await this.$store.dispatch("GetInfo")
            // console.log(personInfo,'返回的个人信息');
            this.inviteRegisterForm.avatar=personInfo.data.avatar
            this.inviteRegisterForm.realName=personInfo.data.name
          }catch(e){
            //TODO handle the exception
            
          }
        }
			
        let code = decodeURIComponent(pars.code);
        this.showOrgInfo = true;
        explainCode({ code: code }).then((res) => {
          // console.log("解释编码结果", res);
          if (res.code == 0) {
            this.inviteRegisterForm.code = code;
            this.inviteRegisterForm.company = res.data.OrgName;
            this.deptOptions = res.data.DeptList;
          }
        });
      } else {
        this.showOrgInfo = false;
      }
    }

    // console.log("头像", Avataaars);
    if (!this.showOrgInfo) {
      this.changeDiffentAvatar();
    }

    // this.getImgCode(); //获取图形验证码
  },
  methods: {
    changeRegType(val) {
      //切换注册方式
      this.isphoneReg = val;
    },
    // 上传图片前校检格式和大小
    handleBeforeUpload(file) {
      // 校检文件类型
      // console.log("上传地址", process.env.VUE_APP_BASE_API, file);

      if (this.fileType) {
        let fileExtension = "";
        if (file.name.lastIndexOf(".") > -1) {
          fileExtension = file.name.slice(file.name.lastIndexOf(".") + 1);
        }
        const isTypeOk = this.fileType.some((type) => {
          if (file.type.indexOf(type) > -1) return true;
          if (fileExtension && fileExtension.indexOf(type) > -1) return true;
          return false;
        });
        if (!isTypeOk) {
          this.$message.error(
            `文件格式不正确, 请上传${this.fileType.join("/")}格式文件!`
          );
          return false;
        }
      }
      // 校检文件大小
      if (this.fileSize) {
        const isLt = file.size / 1024 / 1024 < this.fileSize;
        if (!isLt) {
          this.$message.error(`上传文件大小不能超过 ${this.fileSize} MB!`);
          return false;
        }
      }

      return true;
    },
    handMove(file) {
      console.log("手动上传", file);
      // 第一步：先读取文件
      const base = new FileReader();
      // console.dir(base)
      // 第二步：将需要转化的图片放进去
      base.readAsDataURL(file.file);
      // 第三步：获取结果， 因为文件加载需要时间，因此是个异步的过程，需要使用onload去获取读取的结果，读取转化后的结果放在了result属性里了
      base.onload = () => {
        // this.baseURL = base.result
        console.log("上传图片", base.result);
        this.inviteRegisterForm.avatar = base.result;
      };
    },
    // 上传图片前校检格式和大小
    dialogHide() {
      //点击弹窗的取消按钮
      this.imgCode = "";
      this.imgCodeShow = false;
      // console.log("点击取消按钮", this.imgCode);
    },
    getInputCode() {
      //点击弹窗的确定按钮
      // console.log("点击确定按钮", this.imgCode);
      this.imgCodeShow = false;
      this.getMobileCode();
    },
    async getMobileCode() {
      //获取手机验证码
      if (this.uuid && this.uuid != "") {
        //如果有图形验证码需走需输入图形验证码的那条路
        try {
          let res = await sendMobileCode({
            phone: this.inviteRegisterForm.mobile,
            code: this.imgCode,
            imgid: this.uuid,
          });
          if (res.code == 0) {
            this.currSecord = 60;
            this.codeState = "wait";
            this.beginInterval(); //验证码重新发送倒计时
          }
        } catch (error) {
          if (error.message) {
            this.$message.error(error.message);
          }

          if (error.code == 2) {
            this.getImgCode();
            this.imgCodeShow = true;
          }
        }
      } else {
        //如果没有图形验证码只需要传手机号就可以获取验证码
        try {
          let res = await sendMobileCode({
            phone: this.inviteRegisterForm.mobile,
          });
          // console.log("短信验证码2", res);
          if (res.code == 0) {
            this.currSecord = 60;
            this.codeState = "wait";
            this.beginInterval();
          }
        } catch (error) {
          if (error.message) {
            this.$message.error(error.message);
          }
          if (error.code == 2) {
            this.getImgCode();
            this.imgCodeShow = true;
            return;
          }
        }
      }
    },
    getImgCode() {
      //获取图形验证码
      sendImgCode().then((res) => {
        if (res.code == 0) {
          this.uuid = res.data.uuid;
          this.imgImg = res.data.base64;
        }
      });
    },
    changeDiffentAvatar() {
      if (this.showOrgInfo) {
        return;
      }
      // //切换不同的头像
      this.timer = new Date().getTime(); //给子组件传不同的key可以更新子组件
      this.$nextTick(() => {
        let serializer = new XMLSerializer();
        let svg1 = document.querySelector(".svg-wrap>svg");
        // console.log("svg1", svg1);
        let toExport = svg1.cloneNode(true);
        let bb = svg1.getBBox();
        toExport.setAttribute(
          "viewBox",
          bb.x + " " + bb.y + " " + bb.width + " " + bb.height
        );
        toExport.setAttribute("width", bb.width);
        toExport.setAttribute("height", bb.height);
        let source =
          '<?xml version="1.0" standalone="no"?>\r\n' +
          serializer.serializeToString(toExport);
        let image = new Image();
        image.src =
          "data:image/svg+xml;charset=utf-8," + encodeURIComponent(source);
        let canvas = document.createElement("canvas");
        canvas.width = bb.width;
        canvas.height = (bb.width * 100) / 100;
        let context = canvas.getContext("2d");
        let _this = this;
        image.onload = function () {
          context.drawImage(image, 0, 0, bb.width, (bb.width * 100) / 100);
          _this.inviteRegisterForm.avatar = canvas.toDataURL("image/png");
          // console.log("base64位的头像", _this.inviteRegisterForm.avatar);
        };
      });
    },

    getBase64Image(img) {
      var canvas = document.createElement("canvas");
      canvas.width = img.width;
      canvas.height = img.height;
      var ctx = canvas.getContext("2d");
      ctx.drawImage(img, 0, 0, img.width, img.height);
      var ext = img.src.substring(img.src.lastIndexOf(".") + 1).toLowerCase();
      var dataURL = canvas.toDataURL("image/" + ext);
      return dataURL;
    },
    beginInterval() {
      //验证码重新发送倒计时
      this.currSecord = this.currSecord - 1;
      if (this.currSecord <= 0) {
        this.codeState = "normal";
      } else {
        setTimeout(this.beginInterval, 1000);
      }
    },
    handleinviteRegister(inviteRegisterForm) {
      this.$refs[inviteRegisterForm].validate((valid) => {
        // console.log("校验的值", valid);
        if (valid) {
          // alert("submit!");
          let submitQuey = JSON.parse(JSON.stringify(this.inviteRegisterForm));
          delete submitQuey.readAgreement;
          if (this.isphoneReg) {
            delete submitQuey.email;
            memberLogon(submitQuey).then((res) => {
              if (res.code == 0) {
                this.$modal.msgSuccess("注册成功");
                setToken(res.data.token);
                setRefreshToken(res.data.refresh_token);
                this.$nextTick(async()=>{
                  this.$store.commit("orgLis/SET_ORG_LIST", null);
                  let list = await this.$store.dispatch("orgLis/setOrgList");
                  if (list && list.length > 0) {
                    this.$router.push({ path: "/" }).catch(() => {});
                  } else {
                    this.$router.push("/crm/yaoqing/choose_addorg");
                  }
                })
              }
            });
          } else {
            submitQuey.mobile = "";
            submitQuey.mobileCode = "";
            emailReg(submitQuey).then((res) => {
              if (res.code == 0) {
                this.$modal.msgSuccess("注册成功，需要激活您的邮箱");
                setToken(res.data.token);
                setRefreshToken(res.data.refresh_token);
                this.activeEmailText = this.inviteRegisterForm.email;
                this.activeDialogVisible = true;
                this.reSendEmail();
              }
            });
          }
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    reSendEmail() {
      //重新发送激活邮件
      this.activeLoading = true;
      sendEmailCode({
        email: this.inviteRegisterForm.email,
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
        this.activeEmailText = this.inviteRegisterForm.email;
        this.activeDialogVisible = true;
      } else {
        this.activeDialogVisible = false;
        this.$nextTick(async()=>{
          this.$store.commit("orgLis/SET_ORG_LIST", null);
          let list = await this.$store.dispatch("orgLis/setOrgList");
          if (list && list.length > 0) {
            this.$router.push({ path: "/" }).catch(() => {});
          } else {
            this.$router.push("/crm/yaoqing/choose_addorg");
          }
        })
      }
    },
    handleJoinOrg(inviteRegisterForm) {
      this.$refs[inviteRegisterForm].validate((valid) => {
        // console.log("校验的值", valid);
        if (valid) {
          // alert("submit!");
          this.loading=true
          staffJoinOrg({
						code:this.inviteRegisterForm.code,
						postName:this.inviteRegisterForm.postName,
						deptId:this.inviteRegisterForm.deptId
					}).then(res=>{
						this.$modal.msgSuccess("加入邀请成功");
            this.$store.commit("SET_CHANGE_ORG", res.data);
            setTimeout(()=>{
              this.$router.push("/");
            },200)
					}).catch(err=>{
						this.loading=false
            console.log("加入邀请失败");
					})
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
  },
};
</script>

<style lang="scss" scoped>

.active_email_con {
  font-size: 20px;
  color: #333333;
  .email_text {
    color: #3572ff;
  }
}
.active_dialog-footer {
  display: flex;
  justify-content: space-between;
  margin-top: 70px;
  .cancel_btton {
    height: 52px;
    width: calc(50% - 10px);
    border: 1px solid #dfe2ea;
    background: #f6f9ff;
    color: #78829d;
    font-size: 20px;
    &.noDaoji {
      color: #3572ff;
    }
  }
  .confrim_button {
    height: 52px;
    font-size: 20px;
    width: calc(50% - 10px);
    background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  }
}
.imgCodeTips {
  height: 400px;
  top: calc((100% - 400px) / 2);
  overflow-y: hidden;
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
  .el-button.el-button--default {
    background: #ffffff !important;
  }
}
.invite_register {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-image: url("../assets/images/login-background.png");
  background-size: cover;
  width: 100vw;
  overflow: hidden;
  &.jiayuinvite_register {
    background-image: url("../assets/images/jiayu_login_bg1.png");
    justify-content: flex-start;
    @media (max-height: 800px) {
      align-items: flex-start;
    }
    overflow: auto;
    .invite_register_title {
      h2 {
        color: #333333;
      }
    }
  }
  &.cziotlogin{
    background-image: url("../assets/images/login-background.png");
    background-position: center;
    justify-content: flex-end;
  }
  .jiayulogin_title {
    display: flex;
    justify-content: center;
    align-items: center;
    img.sidebar-logo {
      width: 46px;
      height: 36px;
      margin-right: 10px;
    }
    h2 {
      font-size: 36px;
      line-height: 36px;
      color: rgba(51, 51, 51, 1);
    }
  }
  .wel_con {
    font-size: 24px;
    line-height: 24px;
    color: #999999;
    position: fixed;
    left: 50px;
    top: 40px;
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
    h2 {
      height: 20px;
      line-height: 20px;
      color: #ffffff;
      margin: 0;
      font-size: 20px;
    }
  }
}
// .title {
//   margin: 0px auto 30px auto;
//   text-align: center;
//   color: #707070;
// }
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
  ),
  'cziotlogin': (//LD项目样式
    --formlabelcolor: rgba(255, 255, 255, 0),
    --formconbg: rgba(255, 255, 255, 0),
    --formcontentcolor: rgba(193, 193, 193, 1),
    --forminputbg: rgba(248, 248, 248, 1),
    --forminputcontentcolor: #333333,
    --inputHeight: 60px,
    --opacity:1,
    --inputfontsize:16px,
    --formlabelconcolor:#fff,
    --formlabelfontsize:20px,
    --subbtnradius:10px,
    --submitbtnheight:60px,
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
  width: 60vw;
  position: fixed;
  right: 20%;
  top: 1%;
  // display: flex;
  // justify-content: center;
  // flex-direction: column;
  // align-items: flex-start;
  overflow-y: auto;
  height: 100vh;
  
  @include apply-theme('default');
  &.jiayu_form{
    @include apply-theme('jiayulogin');
  }
  &.cziotlogin_form{
    @include apply-theme('cziotlogin');
    background: rgba(233, 237, 250, 1);
    border-radius: 20px;
    width: 560px;
    height: 800px;
    padding: 60px;
    box-sizing: border-box;
    position: relative;
    right: 0;
    bottom: 0;
    left: 0;
    top: 0;
    margin-right: 160px;
    justify-content: flex-start;
    z-index: 0;
    .jianbianbg{
      height: 238px;
      width: 100%;
      background: linear-gradient(0deg, rgba(255,255,255,0) 0%, rgba(87,137,255,0.4) 100%);
      position: absolute;
      top: 0;
      left: 0;
      z-index: 0;
      border-radius: 20px 20px 0 0;
    }
    .login_logo{
      z-index: 2;
      position: relative;
    }
    .form_con{
      width: 440px;
      height: 564px;
      padding: 0;
      // margin-top: 60px;
      z-index: 2;
      justify-content: flex-start;
      padding-top: 0;
      &.jiayuform_con {
        box-shadow:none;
        ::v-deep .el-form-item{
          margin-bottom: 20px !important;
        }
      }
      .login_logo{
        margin-bottom: 60px;
        img{
          width: 150px;
          height: 50px;
        }
      }
      .invite_register_title{
        line-height: 24px;
        height: 24px;
        margin-bottom: 40px;
        .title{
          font-size: 24px;
          line-height: 24px;
          height: 24px;
          color: rgba(51, 51, 51, 1);
          font-weight: bold;
        }
      }
      .invite_company{
        font-size: 18px;
        color: rgba(51, 51, 51, 1);
        margin-bottom: 40px;
        font-weight: normal;
      }
      .invite_name{
        font-size: 16px;
        color: rgba(51, 51, 51, 1);
        margin-bottom: 30px;
        line-height: 16px;
      }
      .left_con{
        width: 0 !important;
        margin-right: 0 !important;
        &.cziotleft_con{
          width: 100% !important;
          text-align: left;
        }
        .invite_register_title{
          margin-bottom: 20px;
        }
        ::v-deep .el-button{
          margin-top: 40px;
        }
        .avatar_con {
          .text_center {
            ::v-deep input {
              text-align: left;
            }
          }
          ::v-deep .el-form-item__error {
            left: 0;
          }
        }
        ::v-deep .el-input {
          .el-input__prefix{
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 46px;
          }
          &.el-input--prefix .el-input__inner{
            padding-left: 46px;
          }
          .el-input__suffix{
            .el-input__suffix-inner{
              .input_suffix_con{
                .el-button.input_suffix_button{
                  margin-top: 0;
                }
              }
            }
          }
          
        }
        
      }
      .right_con {
        width: 100%;
        
        .avatar_name_con{
          display: flex;
          align-items: center;
          margin-bottom: 40px;
          .avatar_con{
            width: 100px;
            height: 100px;
            position: relative;
            margin-right: 40px;
            .avatar_img{
              width: 100px;
              height: 100px;
            }
            .xiangji{
              position: absolute;
              right: 0;
              bottom: 0;
              width: 36px;
              height: 36px;
            }
          }
          ::v-deep .el-input {
            // height: 38px;
            background: transparent;
            input {
              // height: 38px;
              background-color: transparent;
              border: none;
              border-bottom: 1px solid rgba(223, 223, 223, 1);
              border-radius: 0;
            }
          }
          ::v-deep .el-input__inner:focus {
            
            border: none !important;
            border-bottom: 1px solid rgba(53, 114, 255, 1) !important;
          }
        }
        ::v-deep .el-input {
          .el-input__prefix{
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 46px;
          }
          &.el-input--prefix .el-input__inner{
            padding-left: 46px;
          }
          &.el-input--suffix .el-input__inner {
            padding-right: 120px;
          }
          &.el-input--suffix{
            .el-input__suffix{
              .input_suffix_con{
                display: flex;
                justify-content: flex-start;
                align-items: center;
                .zhongtaiiconfont{
                  font-size: 16px;
                }
                .input_suffix_line{
                  height: 16px;
                  width: 1px;
                  background: rgba(193, 193, 193, 1);
                }
                .el-button.input_suffix_button{
                  width: inherit;
                  height: 60px;
                  background: transparent;
                  color: rgba(53, 114, 255, 1);
                  font-size: 16px;
                  border: none;
                  margin-top: 0;
                  &.qiehuan{
                    padding-left: 6px;
                  }
                  &.disabled-button{
                    color: #999999;
                    padding-left: 12px;
                    padding-right: 14px;
                  }
                }
              }
            }
          }
          
        }
      }
    }
  }
  .form_con {
    background: var(--formconbg);
    width: 1150px;
    height: 752px;
    border-radius: 20px;
    padding: 50px;
    box-sizing: border-box;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    
    &.jiayuform_con {
      border-radius: 10px;
      box-shadow: 0px 10px 12px 0px rgba(16, 26, 53, 0.04);
      
      .avatar_con {
        .upload_click {
          font-size: 14px;
          color: #3572ff;
          cursor: pointer;
        }
      }
      .count_down {
        background: #f8f8f8;
        padding: 0 5px;
        font-size: 14px;
        color: #999999;
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
    }
    .right_con {
      width: calc((100% - 50px) / 2);
    }
    .avatar_con {
      width: 100%;
      text-align: center;
      //height: 110px;
      .svg-wrap {
        margin-left: calc((100% - 100px) / 2);
        margin-bottom: 10px;
      }
      .el-form-item {
        width: 100%;
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
        ::v-deep input {
          text-align: center;
        }
      }
      ::v-deep .el-form-item__error {
        min-width: 200px;
        text-align: left;
        left: calc((100% - 200px) / 2);
      }
    }
    ::v-deep .el-form-item{
      margin-bottom: 30px !important;
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
    margin-top: 40px;
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



</style>
