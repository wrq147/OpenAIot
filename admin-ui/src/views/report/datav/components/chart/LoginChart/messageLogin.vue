<template>
  <el-form
    ref="loginForm"
    :model="loginForm"
    :rules="loginRules"
    label-position="top"
    class="login-form"
  >
    <el-form-item prop="phone">
      <span slot="label" style="flex: 1; display: flex; align-items: center">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-shoujihaoma"
          style="margin-right: 14px"
        ></i
        >手机号
      </span>
      <el-input
        v-model="loginForm.phone"
        type="text"
        auto-complete="off"
        placeholder="请输入手机号"
      >
      </el-input>
    </el-form-item>
    <el-form-item prop="code" label="验证码">
      <span slot="label" style="flex: 1">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-yanzhengma"
          style="margin-right: 14px"
        ></i
        >验证码
      </span>
      <el-input
        v-model="loginForm.code"
        type="text"
        auto-complete="off"
        placeholder="请输入验证码"
        style="width: 65%"
      >
      </el-input>
      <el-button class="login-button" :disabled="disabled" @click="handGetCode" :style="{'--height':this.chartOption.input.inputHeight+'px','--fontSize':this.chartOption.input.fontSize+'px'}">
        {{ getCodeText }}
      </el-button>
    </el-form-item>
    <el-form-item prop="imgCode" label="图形验证码" v-if="hideCode">
      <span slot="label">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-yanzhengma"
          style="margin-right: 14px"
        ></i
        >图形验证码
      </span>
      <el-input
        v-model="loginForm.imgCode"
        auto-complete="off"
        placeholder="请输入图形验证码"
        style="width: 63%"
        @input="handInput"
      >
        <!-- <svg-icon slot="prefix" icon-class="validCode" class="el-input__icon input-icon" /> -->
      </el-input>
      <div class="login-code" :style="{'--height':this.chartOption.input.inputHeight+'px'}">
        <img :src="imgImg" @click="handImgCode" class="login-code-img" />
      </div>
    </el-form-item>

    <el-form-item style="width: 100%; padding-top: 30px">
      <el-button
        :loading="loading"
        size="medium"
        type="primary"
        style="width: 100%"
        @click.native.prevent="handleLogin"
      >
        <span v-if="!loading">登 录</span>
        <span v-else>登 录 中...</span>
      </el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import { getUserInfo } from "@/api/login";
import { sendMobileCode, sendImgCode, loginPhone } from "@/api/system/Employee";
import { setToken, setRefreshToken } from "@/utils/auth";
export default {
  name: "reportMessageLogin",
  props: {
    register: {
        type:Boolean,
        default:false
    },
    loginType:{
        type:Boolean,
        default:false
    },
    isNeedJumpForget:{
        type:Boolean,
        default:false
    },
    redirect:{
      type:String
    },
    chartOption: {
        type: Object
    },
  },
  data() {
    return {
      loginForm: {
        phone: "",
        username: "",
        password: "",
        rememberMe: false,
        code: "",
        uuid: "",
        imgCode: "",
      },
      uuid: "",
      imgImg: "",
      loginRules: {
        phone: [
          { required: true, trigger: "blur", message: "请输入您的手机号" },
        ],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        code: [{ required: true, trigger: "blur", message: "请输入验证码" }],
      },
      loading: false,
      // 验证码开关
      getCodeText: "获取验证码",
      disabled: false,
      getCodeBtnColor: "#ffffff",
      getCodeisWaiting: false,
      hideCode: false,
    };
  },

  mounted() {},

  methods: {
    handImgCode() {
      this.tuxingCode();
    },
    handInput(e) {
      if (this.loginForm.imgCode.length == 4) {
        //this.tuxingCode();
        this.handGetCode();
      }
    },
    //发送手机验证码登录
    handGetCode() {
      if (!this.loginForm.phone) {
        this.$message.warning("请输入手机号"); //弹出提示框
        return;
      }
      sendMobileCode({
        phone: this.loginForm.phone,
        code: this.loginForm.imgCode,
        imgid: this.uuid,
      })
        .then((res) => {
          if (res.code == 0) {
            this.disabled = true;
            this.getCodeText = "发送..."; //发送验证码
            this.getCodeisWaiting = true;
            this.getCodeBtnColor = "rgba(255,255,255,0.5)"; //追加样式，修改颜色
            //示例用定时器模拟请求效果
            //setTimeout(()用于在指定的毫秒数后调用函数或计算表达式
            setTimeout(() => {
              this.$message.success("验证码已发送"); //弹出提示框
              this.setTimer(); //调用定时器方法
            }, 1000);
          }
        })
        .catch((err) => {
          if (err.code == 2) {
            // this.$message.warning(err);//弹出提示框
            this.tuxingCode();
            this.hideCode = true;
          }
          //	this.$message.warning(err);//弹出提示框
        });
    },

    setTimer() {
      let holdTime = 60; //定义变量并赋值
      this.getCodeText = "60s";
      //setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
      //setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
      this.Timer = setInterval(() => {
        if (holdTime <= 0) {
          this.disabled = false;
          this.getCodeisWaiting = false;
          this.getCodeBtnColor = "#ffffff";
          this.getCodeText = "重新获取";
          clearInterval(this.Timer); //清除该函数
          return; //返回前面
        }
        this.getCodeText = holdTime + "s";
        holdTime--;
      }, 1000);
    },
    tuxingCode() {
      sendImgCode().then((res) => {
        if (res.code == 0) {
          //   console.log(res.data, "图形验证码");

          this.uuid = res.data.uuid;
          this.imgImg = res.data.base64;
        }
      });
    },
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.loading = true;
          loginPhone({
            tel: this.loginForm.phone,
            code: this.loginForm.code,
          })
            .then(async (res) => {
              if (res.code == 0) {
                // console.log(res)
                this.$modal.msgSuccess("登录成功");
                setToken(res.data.token);
                setRefreshToken(res.data.refresh_token);
                this.$store.commit('SET_ROLES', [])
                if (this.isNeedJumpForget) {
                  this.$emit('forgotPassword',false)
                  if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.loginJumpUrl){
                    this.$router.push("/ForgotPassword?UpdatePasswordCode=" +res.data.ext_info.extObj.UpdatePasswordCode+'&loginJumpUrl='+this.chartOption.buttonSetting.loginJumpUrl);
                  }else{
                    this.$router.push("/ForgotPassword?UpdatePasswordCode=" +res.data.ext_info.extObj.UpdatePasswordCode);
                  }
                  this.loading = false;
                } else {
                  try {
                    if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.isDefalutUrl){
                      this.$store.commit("orgLis/SET_ORG_LIST", null);
                      let list = await this.$store.dispatch("orgLis/setOrgList");
                      this.$nextTick(()=>{
                        if (list && list.length > 0) {
                          this.$router.push({ path: this.redirect || "/" }).catch(() => {});
                        } else {
                          this.$router.push("/crm/yaoqing/choose_addorg");
                        }
                      })
                    }else{
                      if(this.chartOption.buttonSetting&&this.chartOption.buttonSetting.loginJumpUrl){
                        window.location.replace(this.chartOption.buttonSetting.loginJumpUrl); //切换企业后刷新整个网站
                      }
                    }
                  } catch (error) {
                    console.log(error, "eee");
                    this.loading = false;
                  }
                }
              }
            })
            .catch(() => {
              this.loading = false;
              // this.tuxingCode();
            });
        }
      });
    },
  },
};
</script>
<style lang="less" scoped>
.login-button {
  border: none;
  width: 30%;
  height: var(--height) !important;
  margin-left: 15px;
  font-size: var(--fontSize) !important;
  font-weight: normal !important;
//   color: #fff !important;
}
.login-code {
  width: 33%;
  height: var(--height);
  float: right;
  img {
    cursor: pointer;
    vertical-align: middle;
  }
  .login-code-img {
    height: var(--height);
  }
}

</style>
