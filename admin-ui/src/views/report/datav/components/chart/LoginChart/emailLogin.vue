<template>
  <el-form
    ref="loginForm"
    :model="loginForm"
    :rules="loginRules"
    label-position="top"
    class="login-form"
  >
    <el-form-item prop="email">
      <span slot="label" style="flex: 1; display: flex; align-items: center">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-yonghuyouxiang"
          style="margin-right: 14px"
        ></i>
        邮箱
      </span>

      <el-input
        v-model="loginForm.email"
        type="text"
        auto-complete="off"
        placeholder="请输入邮箱"
      >
      </el-input>
    </el-form-item>

    <el-form-item prop="code" label="验证码">
      <span slot="label" style="flex: 1">
        <i
          slot="prefix"
          class="zhongtaiiconfont zhongtai-icon-yanzhengma"
          style="margin-right: 14px"
        ></i>验证码
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
import { sendEmailCode } from "@/api/login";
import {
  visitorEmailCode,
  loginEmails,
} from "@/api/system/Employee";
import { setToken, setRefreshToken } from "@/utils/auth";
import { getUserInfo } from "@/api/login";
export default {
  name: "reportEmailLogin",
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
        email: "",
        username: "",
        password: "",
        rememberMe: false,
        code: "",
        uuid: "",
      },
      loginRules: {
        email: [{ required: true, trigger: "blur", message: "请输入您的邮箱" }],
        password: [
          { required: true, trigger: "blur", message: "请输入您的密码" },
        ],
        code: [{ required: true, trigger: "change", message: "请输入验证码" }],
      },
      getCodeText: '获取验证码',
      disabled:false,
      getCodeBtnColor: "#ffffff",
      getCodeisWaiting: false,
      loading:false,
    };
  },

  mounted() {},

  methods: {
    //发送邮箱验证码
    handGetCode() {
      if (!this.loginForm.email) {
        this.$message.warning("请输入邮箱"); //弹出提示框
        return;
      }
      visitorEmailCode({
        email: this.loginForm.email,
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
              this.$message.success("邮件码已发送"); //弹出提示框
              this.setTimer(); //调用定时器方法
            }, 1000);
          }
        })
        .catch((err) => {
          this.hideCode = true;
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
    reSendEmail() {
      //重新发送激活邮件
      this.activeLoading = true;
      sendEmailCode({
        email: this.loginForm.username,
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
          console.log(err, "发送邮箱失败");
        });
    },
    handleLogin() {
      this.$refs.loginForm.validate((valid) => {
        if (valid) {
          this.loading = true;
          loginEmails({
            email: this.loginForm.email,
            code: this.loginForm.code,
          })
            .then(async (res) => {
              if (res.code == 0) {
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
                        console.log(error,'eee');
                    }
                //   this.$router.push("/");
                }
              }
            })
            .catch((err) => {
              this.$message.warning(err);
              this.loading = false;
            });
        }
      });
    },
  },
};
</script>
<style lang="less" scoped>
.login-button{
    border:none;
    width: 30%;
    height: var(--height) !important;
    margin-left:15px;
    font-size: var(--fontSize) !important;
    font-weight: normal !important;
    // color:#fff!important;
}
</style>
