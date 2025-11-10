<template>
  <div style="padding:20px 20px 0 20px;" id="big_con" v-loading="loading">
    <el-tabs tab-position="left" style="background-color: #fff;padding: 40px;">
      <el-tab-pane label="物联卡接入">

        <el-descriptions style="margin-left: 15px;" title="SimBoss卡配置" :column="1" border>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者AppId
            </template>
            <el-input style="width:350px" placeholder="请输入AppId" v-model="cardform.SimBossOption.app_id" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者Secret
            </template>
            <el-input style="width:350px" placeholder="请输入Secret" v-model="cardform.SimBossOption.app_secret" clearable>
            </el-input>
          </el-descriptions-item>
        </el-descriptions>

        <el-descriptions class="desTop" title="移动卡配置" :column="1" border>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者AppId
            </template>
            <el-input style="width:350px" placeholder="请输入AppId" v-model="cardform.YiDongOption.app_id" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者密码
            </template>
            <el-input style="width:350px" placeholder="请输入Password" v-model="cardform.YiDongOption.password" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              自动到期停卡
            </template>
            <el-switch v-model="cardform.YiDongOption.enable_expire" active-text="启用" inactive-text="禁止"></el-switch>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              初始可用月份
            </template>
            <el-input-number style="width:350px" v-model="cardform.YiDongOption.expire_month" :min="1"
              :max="60"></el-input-number>
          </el-descriptions-item>
        </el-descriptions>

        <el-descriptions class="desTop" title="硕软卡配置" :column="1" border>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              用户名
            </template>
            <el-input style="width:350px" placeholder="请输入用户名" v-model="cardform.SohanOption.app_id" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              key
            </template>
            <el-input style="width:350px" placeholder="请输入key" v-model="cardform.SohanOption.app_secret" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              自动到期停卡
            </template>
            <el-switch v-model="cardform.SohanOption.enable_expire" active-text="启用" inactive-text="禁止"></el-switch>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              初始可用月份
            </template>
            <el-input-number style="width:350px" v-model="cardform.SohanOption.expire_month" :min="1"
              :max="60"></el-input-number>
          </el-descriptions-item>
        </el-descriptions>


        <el-descriptions class="desTop" title="联通卡配置" :column="1" border>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              服务器地址
            </template>
            <el-input style="width:350px" placeholder="请输入接口服务器地址" v-model="cardform.UnicomOption.appUrl" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者AppId
            </template>
            <el-input style="width:350px" placeholder="请输入AppId" v-model="cardform.UnicomOption.app_id" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              开发者AppSecret
            </template>
            <el-input style="width:350px" placeholder="请输入AppSecret" v-model="cardform.UnicomOption.app_secret"
              clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              产品Id
            </template>
            <el-input style="width:350px" placeholder="请输入tenantId" v-model="cardform.UnicomOption.tenantId" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              encodeKey
            </template>
            <el-input style="width:350px" placeholder="请输入encodeKey" v-model="cardform.UnicomOption.encodeKey"
              clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              ivKey
            </template>
            <el-input style="width:350px" placeholder="请输入ivKey" v-model="cardform.UnicomOption.ivKey" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              username
            </template>
            <el-input style="width:350px" placeholder="请输入username" v-model="cardform.UnicomOption.username" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              password
            </template>
            <el-input style="width:350px" placeholder="请输入password" v-model="cardform.UnicomOption.password" clearable>
            </el-input>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              自动到期停卡
            </template>
            <el-switch v-model="cardform.UnicomOption.enable_expire" active-text="启用" inactive-text="禁止"></el-switch>
          </el-descriptions-item>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              初始可用月份
            </template>
            <el-input-number style="width:350px" v-model="cardform.UnicomOption.expire_month" :min="1"
              :max="60"></el-input-number>
          </el-descriptions-item>
        </el-descriptions>


        <el-descriptions class="desTop" title="自动添加物联卡" :column="1" border>
          <el-descriptions-item label-class-name="iot-setlabel">
            <template slot="label">
              是否启用<br />
              <div style="color: #c40001;font-size: 14px;margin-top: 10px;">注意：需先配置上面的开发者信息</div>
            </template>
            <el-switch :disabled="enableAuto" v-model="cardform.EnableAutoAdd" active-text="启用"
              inactive-text="禁止"></el-switch>
          </el-descriptions-item>
        </el-descriptions>

        <div style="display: flex;align-items: center;justify-content: center;margin-top: 30px;">
          <el-button type="primary" @click="SaveCardConfig">保存</el-button>
        </div>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
<script>
import {
  saveConfig,
  getConfig
} from "@/api/rules/device";

export default {
  data() {
    return {
      loading: true,
      cardform: {
        EnableAutoAdd: false,
        SimBossOption: {
          app_id: "",
          app_secret: ""
        },
        YiDongOption: {
          app_id: "",
          password: "",
          expire_month: 12,
          enable_expire: false
        },
        SohanOption: {
          app_id: "",
          app_secret: "",
          expire_month: 12,
          enable_expire: false
        },
        UnicomOption: {
          appUrl: "",
          app_id: "",
          app_secret: "",
          tenantId: "",
          encodeKey: "",
          ivKey: "",
          username: "",
          password: "",
          expire_month: 12,
          enable_expire: false
        }
      }
    };
  },
  computed: {
    enableAuto: function () {
      return this.cardform.SimBossOption.app_id == '' && this.cardform.YiDongOption.app_id == '' && this.cardform.SohanOption.app_id == '' && this.cardform.UnicomOption.app_id == '';
    }
  },
  async created() {
    this.loading = true;
    let res = await getConfig();
    if (res.data != null) {
      this.cardform.EnableAutoAdd = res.data.EnableAutoAdd;
      if (res.data.SimBossOption != null && res.data.SimBossOption != "") {
        this.cardform.SimBossOption = JSON.parse(res.data.SimBossOption);
      }
      if (res.data.YiDongOption != null && res.data.YiDongOption != "") {
        this.cardform.YiDongOption = JSON.parse(res.data.YiDongOption);
        if (this.cardform.YiDongOption.enable_expire == null) {
          this.cardform.YiDongOption.enable_expire = false;
        }
      }
      if (res.data.SohanOption != null && res.data.SohanOption != "") {
        this.cardform.SohanOption = JSON.parse(res.data.SohanOption);
        if (this.cardform.SohanOption.enable_expire == null) {
          this.cardform.SohanOption.enable_expire = false;
        }
      }
      if (res.data.UnicomOption != null && res.data.UnicomOption != "") {
        this.cardform.UnicomOption = JSON.parse(res.data.UnicomOption);
        if (this.cardform.UnicomOption.enable_expire == null) {
          this.cardform.UnicomOption.enable_expire = false;
        }
      }
      console.info(this.cardform)
    }
    this.loading = false;
  },
  methods: {
    async SaveCardConfig() {
      const loading = this.$loading({
        lock: true,
        text: '保存中...',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });
      let iptdata = { EnableAutoAdd: this.cardform.EnableAutoAdd, SimBossOption: JSON.stringify(this.cardform.SimBossOption), YiDongOption: JSON.stringify(this.cardform.YiDongOption), SohanOption: JSON.stringify(this.cardform.SohanOption), UnicomOption: JSON.stringify(this.cardform.UnicomOption) };
      let res = await saveConfig(iptdata);
      loading.close();
      if (res.code == 0) {
        this.$message.success("保存成功");
      }
      else {
        this.$message.error(res.message);
      }
    },

  }
};
</script>
<style lang="scss">
.desTop {
  margin-top: 25px;
  margin-left: 15px;
}

.iot-setlabel {
  width: 240px;
}
</style>