<template>
  <el-form class="bind_form" ref="form" :model="bindPhone" :rules="rules" label-width="100px" label-position="top">
    <el-form-item label="手机号码" prop="mobile">
      <el-input  v-model="bindPhone.mobile" maxlength="11" style="width:360px" placeholder="请输入手机号码"/>
    </el-form-item>
    <el-form-item prop="imgCode" v-if="imgCodeShow" label="图形验证码">
      <el-input v-model="bindPhone.imgCode" auto-complete="off" placeholder="请输入图形验证码" :style="{width: '50%'}" @input="handInput">
        <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i>
      </el-input>
      <div class="login-code">
        <img :src="imgImg" @click="handImgCode" class="login-code-img" />
      </div>
    </el-form-item>
    <el-form-item prop="code" label="验证码">
      <el-input v-model="bindPhone.mobileCode" type="text" auto-complete="off" placeholder="请输入验证码" :style="{width: '100%'}">
        <!-- <i slot="prefix" class="zhongtaiiconfont zhongtai-icon-yanzhengma" :style="{'color':'rgba(255, 255, 255, 0.30)'}"></i> -->
        <div slot="suffix" class="input_suffix_con">
          <div class="input_suffix_line"></div>
          <el-button class="input_suffix_button"  :class="codeState!='normal' ? 'disabled-button' : ''" :disabled="codeState!='normal'">
            <span v-if="codeState=='normal'" @click="getMobileCode">获取验证码</span>
            <span v-else>{{'重新发送('+currSecord+'s)'}}</span>
          </el-button>
        </div>
      </el-input>
    </el-form-item>
    
    <div style="display:flex;justify-content: flex-end;margin-top:40px">
      <el-button class="closeInfoBtn" @click="close">取消</el-button>
      <el-button type="primary" class="saveInfoBtn" @click="submit">绑定</el-button>
    </div>
  </el-form>
</template>

<script>
import { updateUserProfile } from "@/api/system/user";
import {
  explainCode,
  sendMobileCode,
  memberLogon,
  sendImgCode,
  bindTel
} from "@/api/system/Employee";
export default {
  props: {
    user: {
      type: Object
    }
  },
  data() {
    return {
      // 表单校验
      rules: {
        imgCode: [
          { required: true, message: "图形验证码不能为空", trigger: "blur" }
        ],
        mobileCode: [
          { required: true, message: "验证码不能为空", trigger: "blur" },
        ],
        mobile: [
          { required: true, message: "手机号码不能为空", trigger: "blur" },
          {
            pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
            message: "请输入正确的手机号码",
            trigger: "blur"
          }
        ]
      },
      bindPhone: {
        mobile: null,
        imgCode: "",
        mobileCode: ""
      }, //绑定手机号的参数
      // 验证码开关
      //   captchaOnOff: true,
      currSecord: 0, //验证码倒计时
      codeState: "normal", //是否重新发送验证码
      uuid: "", //图形验证码编号
      imgImg: "", //图形链接
      imgCodeShow: false //图形弹窗是否显示
    };
  },
  mounted() {
    //   this.bindPhone.mobile=this.user.Mobile
  },
  // updated() {
  //   this.$set(this.bindPhone, "mobile", this.user.Mobile);
  // },
  methods: {
    setBindPhone(val){
      this.$set(this.bindPhone, "mobile", val.Mobile);
    },
    async getMobileCode() {
      //获取手机验证码
      // console.log("点击了获取手机验证码", this.uuid, this.bindPhone.imgCode);

      if (this.uuid) {
        //如果有图形验证码需走需输入图形验证码的那条路
        try {
          let res = await sendMobileCode({
            phone: this.bindPhone.mobile,
            code: this.bindPhone.imgCode,
            imgid: this.uuid
          });
          // console.log("传值",{
          // phone: this.bindPhone.mobile,
          //   code: this.bindPhone.imgCode,
          //   imgid: this.uuid
          // });
          if (res.code == 0) {
            this.currSecord = 120;
            this.codeState = "wait";
            this.beginInterval(); //验证码重新发送倒计时
          }
        } catch (error) {
          if (error.code == 102||error.code == 2) {
            this.getImgCode();
            this.imgCodeShow = true;
          }
        }
      } else {
        //如果没有图形验证码只需要传手机号就可以获取验证码
        try {
          // console.log(this.bindPhone.mobile, "hhdhdhdhdh");

          let res = await sendMobileCode({
            phone: this.bindPhone.mobile
          });
          // console.log("短信验证码2", res);
          if (res.code == 0) {
            this.currSecord = 60;
            this.codeState = "wait";
            this.beginInterval();
          }
        } catch (error) {
          // console.log("error", error);

          if (error.code == 102||error.code == 2) {
            this.getImgCode();
            this.imgCodeShow = true;
            return;
          }
        }
      }
    },
    getImgCode() {
      //获取图形验证码
      sendImgCode().then(res => {
        if (res.code == 0) {
          // console.log(res,'图形验证码');
          
          this.uuid = res.data.uuid;
          this.imgImg = res.data.base64;
        }
      });
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
    submit() {
      this.$refs["form"].validate(valid => {
        if (valid) {
          bindTel({
            phone: this.bindPhone.mobile,
            code: this.bindPhone.mobileCode
          }).then(response => {
            this.$modal.msgSuccess("修改成功");
            this.$emit('successDialog')
          });
        }
      });
    },
    close() {
      // this.$store.dispatch("tagsView/delView", this.$route);
      // this.$router.push({ path: "/index" });
      this.$emit('closeDialog')
    }
  }
};
</script>
<style lang="less" scoped>
.bind_form{
  ::v-deep .el-input {
    height: 36px;
    border-radius: 6px;
    input {
      height: 36px;
      color: rgba(255, 255, 255, 1);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: 6px;
      font-size: 14px;
    }
    &.el-input--prefix .el-input__inner{
      padding-left: 40px;
    }
    &.el-input--suffix{
      .el-input__suffix{
        .input_suffix_con{
          display: flex;
          justify-content: flex-start;
          align-items: center;
          .input_suffix_line{
            height: 14px;
            width: 1px;
            background:rgba(255, 255, 255, 0.2);
          }
          .el-button.input_suffix_button{
            width: 102px;
            height: 36px;
            background: transparent;
            color: rgba(61, 185, 143, 1);
            font-size: 14px;
            border: none;
            padding-left: 16px;
            padding-right: 16px;
            &.disabled-button{
              width: 122px;
              color: #999999;
            }
          }
        }
      }
    }
  }
  ::v-deep .el-input__prefix{
    color: rgba(255, 255, 255, 0.30);
    font-size: 14px;
    line-height: 36px;
    left: 16px;
  }
  ::v-deep .el-input--prefix .el-input__inner{
    padding-left: 40px;
  }
  .login-code {
    width: calc(50% - 30px);
    height: 36px;
    float: right;
    img {
      width: 100%;
      cursor: pointer;
      height: 36px;
      vertical-align: middle;
    }
  }
  ::v-deep .el-input__inner:focus {
    border: 1px solid rgba(61, 185, 143, 1);
  }
  .input-icon {
    height: 39px;
    width: 14px;
    margin-left: 2px;
  }
}
.saveInfoBtn {
  background: rgba(61, 185, 143, 1);
  border: none;
  width: 80px;
  height: 36px;
  color: rgba(255, 255, 255, 1);
}
.closeInfoBtn {
  background-color: rgba(34, 46, 64, 1);
  border: none;
  width: 80px;
  height: 36px;
  color: rgba(255, 255, 255, 0.6);
  &:focus, &:hover{
    background: rgba(34, 46, 64, 1);
    color: rgba(255, 255, 255, 0.6);
    border: none;
  }
}
</style>