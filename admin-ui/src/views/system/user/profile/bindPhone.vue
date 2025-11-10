<template>
  <el-form ref="form" :model="bindPhone" :rules="rules" label-width="100px">
    <el-form-item label="手机号码" prop="mobile">
      <el-input
        v-model="bindPhone.mobile"
        maxlength="11"
        style="width:340px"
        placeholder="请输入手机号码"
      />
    </el-form-item>
    <!-- <el-form-item label="邮箱" prop="Email">
      <el-input v-model="user.Email" maxlength="50" />
    </el-form-item>-->
    <el-form-item class="img_code_con" prop="imgCode" label="图形验证码" v-if="imgCodeShow">
      <el-input
        v-model="bindPhone.imgCode"
        type="text"
        auto-complete="off"
        placeholder="请输入图形验证码"
        style="width:340px"
      >
        <!-- <svg-icon slot="prefix" icon-class="password" class="el-input__icon input-icon" /> -->
      </el-input>
      <div class="login-code">
        <img :src="imgImg" style="height:40px" @click="getImgCode" class="login-code-img" />
      </div>
    </el-form-item>
    <el-form-item prop="mobileCode" label="验证码">
      <div class="mobile_code_con">
        <el-input
          v-model="bindPhone.mobileCode"
          auto-complete="off"
          placeholder="请输入验证码"
          style="width:340px"
        >
          <!-- <svg-icon slot="prefix" icon-class="validCode" class="el-input__icon input-icon" /> -->
        </el-input>
        <div :class="{'invite_register-code':true,'count_down':codeState!='normal'}">
          <!-- <img :src="codeUrl" @click="getCode" class="invite_register-code-img" /> -->
          <span v-if="codeState=='normal'" @click="getMobileCode">获取验证码</span>
          <span v-else>{{'重新发送('+currSecord+'s)'}}</span>
        </div>
      </div>
    </el-form-item>
    <el-form-item style="margin-top:35px">
      <el-button type="primary" class="saveInfoBtn" @click="submit">绑定</el-button>
      <el-button class="closeInfoBtn" @click="close">关闭</el-button>
    </el-form-item>
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
          });
        }
      });
    },
    close() {
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push({ path: "/index" });
    }
  }
};
</script>
<style lang="scss" scope>
.saveInfoBtn {
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  border: none;
  width: 130px;
  height: 40px;
}
.closeInfoBtn {
  background-color: #ffffff;
  border: 1px solid #dfe2ea;
  width: 130px;
  height: 40px;
}
.invite_register-code {
  width: 115px;
  height: 28px;
  border-radius: 14px;
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  text-align: center;
  line-height: 28px;
  color: #ffffff;
  overflow: hidden;
  //   position: absolute;
  //   left: 240px;
  //   top: 3px;
  margin-left: 12px;
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
  background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
  padding: 0 5px;
  font-size: 14px;
}
.mobile_code_con {
  display: flex;
  justify-content: flex-start;
  align-items: center;
}
.img_code_con {
  .el-form-item__content {
    display: flex;
    justify-content: flex-start;
    align-items: center;
  }
  .login-code {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 40px;
    margin: 0;
    padding: 0;
    margin-left: 12px;
  }
}
</style>