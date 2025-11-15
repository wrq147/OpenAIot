<template>
  <div class="invite_org">
    <el-form ref="orgForm" :model="orgForm" :rules="orgRules" label-position="top" class="org-form">
      <div class="form_con">
        <div class="title">
          <div class="title_icon" @click="goOff">
            <i class="el-icon-back"></i>
          </div>
          <span>输入团队或企业名称</span>
        </div>
        <el-form-item prop="logo" class="logoImg">
          <image-upload v-model="orgForm.logo" :limit="1">
            <template #tip>
              <div style="text-align:center;">请上传营业执照</div>
            </template>
          </image-upload>
        </el-form-item>
        <el-form-item prop="orgName">
          <el-input v-model="orgForm.orgName" type="text" auto-complete="off" placeholder="请输入企业名称">
          </el-input>
        </el-form-item>
        <!-- <el-form-item prop="industry">
          <el-cascader
            v-model="orgForm.industryArr"
            placeholder="请选择行业类型"
            :props="{value:'Id', label: 'Name', children: 'children'}"
            :options="industryLis"
            @change="choiceSize"
          ></el-cascader>
        </el-form-item> -->
        <el-form-item prop="size">
          <div style="display: flex;justify-content: space-between;">
            <div style="color:#C0C4CC">
              预计使用规模
              <el-tooltip class="item" content="你预计将与多少人一起使用" placement="top">
                <span>
                  <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="margin-left:12px;cursor: pointer;"></i>
                </span>
              </el-tooltip>
            </div>
            <el-select style="width:280px;" v-model="orgForm.size" slot="append" placeholder="请选择规模">
              <el-option v-for="item in scaleLis" :key="item.value" :label="item.label" :value="item.value"></el-option>
            </el-select>
          </div>
        </el-form-item>
        <el-form-item prop="addressName">
          <el-input placeholder="请选择企业所在地址" v-model="orgForm.addressName" @focus="choiceMap"></el-input>
        </el-form-item>
        <el-form-item prop="addressDetail">
          <el-input placeholder="请输入地址详情" v-model="orgForm.addressDetail" type="textarea"></el-input>
        </el-form-item>
        <el-form-item style="margin-top:35px;margin-bottom:20px">
          <el-button :loading="loading" type="primary" style="width:100%;" @click.native.prevent="creatOrg">
            <span v-if="!loading">完成创建</span>
            <span v-else>创 建 中...</span>
          </el-button>
        </el-form-item>
      </div>
    </el-form>
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
  </div>
</template>

<script>
import { creatCompany } from "@/api/system/company";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
export default {
  name: "newOrg",
  components: { mapSelectCompt },
  dicts:['org_size'],
  data() {
    // const fileMustUpload = (rule, value, callback) => {
    //   if (this.orgForm.logo == null) {
    //     // 未上传文件
    //     callback("请上传企业logo");
    //   }
    //   callback();
    // };
    return {
      openSelectMap: false, //是否打开选择地址的弹窗
      input3: "预计使用规模",
      codeUrl: "",
      cookiePassword: "",
      orgForm: {
        orgName: "",
        // industryArr: '',
        industry: '',
        size: null,
        logo: null,
        addressName: "",
        addressCode: 0,
        lng: 0,
        lat: 0,
        addressDetail: "",
        intro: ""
      },
      orgRules: {
        orgName: [{ required: true, trigger: "blur", message: "请输入企业" }],
        // industry: [
        //   { required: true, trigger: "change", message: "请选择行业" }
        // ],
        size: [
          { required: true, trigger: "change", message: "请选择预计使用规模" }
        ],
        // logo: [{ validator: fileMustUpload, trigger: "change" }],
        // addressDetail: [
        //   { required: true, trigger: "blur", message: "请输入地址详情" },
        //   { required: true, trigger: "change", message: "请输入地址详情" }
        // ],
        // addressName: [
        //   { required: true, trigger: "blur", message: "请选择企业所在地址" },
        //   { required: true, trigger: "change", message: "请选择企业所在地址" }
        // ]
      },
      loading: false,
      // industryLis: [],
      scaleLis: [],
      center: null,
      map: null,
      search: null,
      suggest: null,
      markers: null,
      infoWindowList: Array(1),
      choiceAddress: {},
      isFirstDraw: true,
      geocoder: null
    };
  },
  watch: {},
  created() {
  },
  mounted() {
    this.getDatas();
  },
  methods: {
    // choiceSize() {
    //   //选择行业类型
    //   // console.log("行业类型",this.orgForm.industry);
    //   this.orgForm.industry = this.orgForm.industryArr[
    //     this.orgForm.industryArr.length - 1
    //   ];
    // },
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
      // this.$store.dispatch("datas/industryTree").then(rt => {
      //   this.industryLis = rt;
      // });

      this.scaleLis = this.dict.type.org_size;
    },
    creatOrg() {
      this.$refs.orgForm.validate(valid => {
        if (valid) {
          this.loading = true;
          creatCompany(this.orgForm).then(res => {
            // console.log(res, "创建企业成功");
            if (res.code == 0) {
              this.loading = false;
              this.$modal.msgSuccess("创建成功");
              this.$router.go(-1);
            }
          });
        }
      });
    },
  }
};
</script>

<style rel="stylesheet/scss" lang="scss">
.select_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
#container {
  overflow: hidden;
  width: 100%;
  height: 100%;
  margin: 0;
  font-family: "微软雅黑";
}

.anchorBL {
  display: none;
}

#panel {
  position: absolute;
  background: #fff;
  width: 350px;
  padding: 20px;
  z-index: 9999;
  top: 30px;
  left: 30px;
}
.invite_org {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  background-image: url("../assets/images/neworg_bg.png");
  background-size: cover;
  width: 100%;
  .org_title {
    width: 100%;
    display: flex;
    justify-content: center;
    // position: fixed;
    // right: 17.2%;
    // top: 16%;
    height: 30px;
    line-height: 30px;
    margin-bottom: 50px;
    img.sidebar-logo {
      width: 54px;
      height: 30px;
      margin-right: 20px;
    }
    h2 {
      height: 30px;
      line-height: 30px;
      color: #ffffff;
      margin: 0;
    }
  }
}
.org-form {
  // width: 600px;
  padding: 25px 15px 5px 15px;
  .form_con {
    box-shadow: -3px 3px 20px rgba(0, 29, 97, 0.1),
      3px -3px 20px rgba(0, 29, 97, 0.1);
    border-radius: 12px;
    overflow: auto;
    background: rgba(255, 255, 255, 1);
    width: 520px;
    // height: 548px;
    border-radius: 20px;
    padding: 20px 35px 15px;
    .logoImg {
      display: flex;
      justify-content: center;
      .el-form-item__content {
        padding-right: 10px;
      }
    }
    .product-uploader .el-upload {
      border: 1px dashed #d9d9d9;
      border-radius: 6px;
      cursor: pointer;
      position: relative;
      overflow: hidden;
      width: 80px;
      height: 80px;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-content: center;
    }
    .product-uploader .el-upload:hover {
      border-color: #409eff;
    }
    .product-uploader-icon {
      font-size: 16px;
      color: #c4c4d6;
      // width: 60px;
      // height: 60px;
      // line-height: 60px;
      text-align: center;
    }
    .tishi {
      display: inline;
      color: #c4c4d6;
      font-size: 14px;
    }
    .product {
      width: 80px;
      height: 80px;
      display: block;
    }
    .title {
      display: flex;
      justify-content: flex-start;
      align-items: center;
      color: #262626;
      margin-bottom: 16px;
      .title_icon {
        cursor: pointer;
        width: 44px;
        height: 44px;
        border-radius: 50%;
        border: 2px solid #efefef;
        text-align: center;
        line-height: 44px;
        font-size: 20px;
        color: #2b2b2b;
        margin-right: 16px;
      }
      .title_icon:hover {
        border: 2px solid #b0b0b0;
      }
      span {
        font-weight: 500px;
        font-size: 22px;
      }
    }

    .el-form-item {
      margin-bottom: 20px;
      .el-button {
        height: 50px;
        border-radius: 10px;
      }
      // .el-select {
      //   width: 100%;
      // }
      .el-textarea {
        textarea {
          border-left: none;
          border-top: none;
          border-right: none;
          padding: 0px;
          border-radius: 0;
          font-family: "";
        }
      }
      .el-cascader {
        width: 100%;
      }
      .input-with-select {
        width: 100%;
      }
      .input-with-select .el-input-group__prepend {
        background-color: #fff;
        width: 30%;
        border: none;
        border-bottom: 1px solid #dcdfe6;
        padding-left: 5px;
      }
      .input-with-select .el-input-group__append {
        background-color: #fff;
        width: 70%;
        padding: 5px 0;
        border: none;
        border-bottom: 1px solid #dcdfe6;
        padding-left: 5px;
        .el-select {
          width: 100%;
          margin-left: 0;
          padding-left: 10px;
          height: 36px;
          border-left: 1px solid #dcdfe6;
          input {
            height: 36px;
          }
        }
      }
    }
    .el-input--medium .el-input__inner {
      height: 50px;
      line-height: 50px;
      border-radius: 0;
      border-left: 0;
      border-top: 0;
      border-right: 0;
      padding: 0;
    }
  }
}
</style>
