<template>
  <div style="width:800px;" class="orgForm">
    <el-form
      ref="orgForm"
      :model="org"
      :rules="orgRules"
      label-width="80px"
      label-position="top"
      v-if="orgRules"
    >
      <el-form-item label="企业名称" prop="OrgName">
        <el-input v-model="org.OrgName" maxlength="30" />
      </el-form-item>
      <el-form-item label="企业规模" prop="Size">
        <!-- <el-input v-model="org.Size" maxlength="11" /> -->
        <el-select v-model="org.Size" placeholder="请选择规模" style="width:100%" @change="choiceSize">
          <el-option
            v-for="item in scaleLis"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="行业类型" prop="Industry">
        <el-cascader v-model="org.industryArr" placeholder="请选择行业类型" :props="{value:'Id', label: 'Name', children: 'children'}" :options="industryLis" @change="choiceIndustry" style="width:100%"></el-cascader>
      </el-form-item>
      <el-form-item label="企业地址" prop="addressName">
        <el-input placeholder="请选择企业所在地址" v-model="org.AddressName" @focus="choiceMap"></el-input>
        <el-input placeholder="请选择企业所在地址" v-model="org.Lng" style="display:none"></el-input>
        <el-input placeholder="请选择企业所在地址" v-model="org.Lat" style="display:none"></el-input>
        <el-input placeholder="请选择企业所在地址" v-model="org.AddressCode" style="display:none"></el-input>
      </el-form-item>
      <el-form-item prop="addressDetail" label="地址详情">
        <el-input placeholder="请输入地址详情" v-model="org.AddressDetail" type="textarea"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="custom" class="saveInfoBtn" @click="submit">保存</el-button>
        <el-button class="closeInfoBtn" @click="close">关闭</el-button>
      </el-form-item>
    </el-form>
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
  </div>
</template>

<script>
import { editCompany } from "@/api/system/company";
import { initMap } from "@/utils/amap";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
export default {
  name:'orgManage',
  components: { mapSelectCompt },
  props: {
    org: {
      type: Object,
      default: () => ({})
    }
  },
  dicts: ["org_size"],
  data() {
    return {
      // 表单校验
      openSelectMap: false, //是否打开选择地址的弹窗
      industryLis: [], //行业类型
      scaleLis: [], //企业规模
      areaLis: [], //区域地址
      orgRules: {
        OrgName: [{ required: true, trigger: "blur", message: "请输入企业" }],
        Industry: [
          { required: true, trigger: "change", message: "请选择行业" }
        ],
        Size: [
          { required: true, trigger: "change", message: "请选择预计使用规模" }
        ],
        // AddressDetail: [
        //   { required: true, trigger: "blur", message: "请输入地址详情" },
        //   { required: true, trigger: "change", message: "请输入地址详情" }
        // ],
        // AddressName: [
        //   { required: true, trigger: "blur", message: "请选择企业所在地址" },
        //   { required: true, trigger: "change", message: "请选择企业所在地址" }
        // ]
      },
      loading: false,
      // 注册开关
      industryLis: [],
      scaleLis: [],
      center: null,
      map: null,
      suggestionList: [],
      search: null,
      suggest: null,
      markers: null,
      infoWindowList: Array(10),
      choiceAddress: {},
      isFirstDraw: true,
      geocoder: null
    };
  },
  beforeCreate() {
  },
  mounted() {
    this.getDatas();
  },
  methods: {
    closeDislog() {
      this.openSelectMap = false;
      this.isFirstDraw = true;
    },
    returnMapInfo(info){
      // console.log('infoinfo',info);
      if (info) {
        this.org.AddressName =info.AddressName;
        this.org.AddressDetail =info.AddressDetail;
        this.org.AddressCode =info.AddressCode;
        this.org.Lat =info.Lat;
        this.org.Lng =info.Lng;
      }
      // console.log("选择地址后的信息",this.org);
      this.$forceUpdate()
    },
    choiceMap() {
      //打开地址选择的弹窗
      this.$refs.mapSelectCompt.choiceMap()
    },
    async choiceIndustry() {
      //选择行业类型
      this.$set(this.org,"Industry",this.org.industryArr[this.org.industryArr.length - 1]);
      this.$store.commit("datas/SET_INDUSTRY",this.org.industryArr[this.org.industryArr.length - 1]);
      let rs = await this.$store.dispatch("datas/industryName");
      this.$set(this.org, "IndustryName", rs);
    },
    async choiceSize(val) {
      //选择行业类型
      let rt =this.dict.getName("org_size",val);
      this.$set(this.org, "SizeName", rt);
    },
    getDatas() {
      this.$store.dispatch("datas/industryTree").then(rt => {
        this.industryLis = rt;
      });

      this.scaleLis = this.dict.type.org_size;
    },
    submit() {
      this.$refs["orgForm"].validate(valid => {
        if (valid) {
          editCompany(this.org).then(response => {
            this.$modal.msgSuccess("修改成功");
            this.$emit("reLoadOrg");
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
.orgForm{
  .el-form-item {
      margin-bottom: 10px !important;
    }
}

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

#suggestionList {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

#suggestionList li .a {
  margin-top: -1px;
  background-color: #f6f6f6;
  text-decoration: none;
  font-size: 18px;
  color: black;
  display: block;
}

#suggestionList li .item_info {
  font-size: 12px;
  color: grey;
}

#suggestionList li .a:hover:not(.header) {
  background-color: #eee;
}
</style>