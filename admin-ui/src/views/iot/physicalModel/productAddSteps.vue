<template>
  <div>
    <div style="padding: 20px 20px 50px 20px" id="big_con">
      <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 186px' }">
        <el-form :model="addData" :rules="addRules" ref="addParams">
          <div style="height: 100%; padding: 0 0 0 20px; padding-top: 10px" class="steps_con">
            <el-steps direction="vertical" :active="addActive" finish-status="success">
              <el-step title="请选择你想创建的协议分类">
                <div slot="title">
                  <span v-if="addActive == 0 || selectedClass.Id == 'undefine'">请选择你想创建的协议分类</span>
                  <span v-if="addActive != 0 && selectedClass.Id">已选择协议分类</span>
                  <span style="color: #0f73e6;font-size: 12px;margin-left: 30px;cursor: pointer;" @click="reselectClass"
                    v-if="selectedClass.Id != null">重新选择</span>
                </div>
                <div slot="description" style="padding: 20px 10px 32px">
                  <div class="selected_class" v-if="selectedClass.Id">
                    <div class="class_info">
                      <img style="width: " :src="selectedClass.PhotoUrl" alt />
                      <span class="label">{{ selectedClass.Name }}</span>
                    </div>
                  </div>
                  <el-card v-else class="class_box_card" v-loading="cardLoading">
                    <div class="class_item_con">
                      <template v-if="activeClassList.length > 0">
                        <div class="class_item" v-for="ite in activeClassList" :key="ite.Id" @click="choiceClass(ite)">
                          <img :src="ite.PhotoUrl" alt />
                          <span>{{ ite.Name }}</span>
                        </div>
                      </template>
                      <div style="width: 100%" v-else>
                        <el-empty>
                          <el-link type="primary"
                            @click="$router.push('/iot/physicalModel/productClass')">点击此处创建</el-link>
                        </el-empty>
                      </div>
                    </div>
                  </el-card>
                </div>
              </el-step>
              <el-step title="请选择设备接入方式">
                <div slot="title">
                  <span v-if="addActive == 1">请选择设备接入方式</span>
                  <span v-if="addActive != 1">已选设备接入方式</span>
                  <span style="color: #0f73e6;font-size: 12px;margin-left: 30px;cursor: pointer;"
                    @click="reselecWorkWay" v-if="networkWay != null">重新选择</span>
                </div>
                <div slot="description" style="padding: 20px 10px 32px"
                  v-if="(networkWay && networkWay.Code) || addActive == 1">
                  <div class="selected_class"
                    v-if="(networkWay && networkWay.Code && networkWay.Code == 'mqtt_modbus' && addData.interScripts) || (networkWay && networkWay.Code && networkWay.Code != 'mqtt_modbus')"
                    style="justify-content: flex-start">
                    {{ networkWay.Name }}<span v-if="addData.interScripts">（{{ scriptValue.Name }}）</span>
                  </div>
                  <el-card style="width: 100%" v-else>
                    <div class="workWay_con">
                      <div class="workWay_item_con" v-for="ites in workWayList" :key="ites.Code">
                        <div class="workWay_item" @click="choiceWorkWay(ites)">
                          {{ ites.Name }}<div class="workWaySelectDiv"
                            v-if="networkWay && networkWay.Code && networkWay.Code == ites.Code"></div>
                        </div>
                        <div class="workWay_remark">* {{ ites.Remark }}</div>
                      </div>
                    </div>
                    <el-row v-if="networkWay && networkWay.Code == 'mqtt_modbus'" style="margin-top: 20px">
                      <el-col :span="24">
                        <el-form-item label="DTU模板" style="margin-bottom: 0">
                          <el-select v-model="addData.interScripts" filterable remote reserve-keyword
                            placeholder="请输入关脚本模板名称" :remote-method="remoteMethod" :loading="optionLoading" clearable
                            @change="choiceScriptVal">
                            <el-option v-for="item in scriptOption" :key="item.Id" :label="item.Name"
                              :value="item.Id"></el-option>
                          </el-select>
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-card>
                </div>
              </el-step>

              <el-step title="完善协议信息">
                <div slot="title">
                  <span>完善协议信息</span>
                </div>
                <div slot="description" style="padding: 20px 10px 32px" v-if="addActive == 2">
                  <el-card style="width: 100%; margin-bottom: -15px">
                    <el-row>
                      <el-col :span="24">
                        <el-form-item label="协议名称" prop="name">
                          <el-input v-model="addData.name" @input.native="inputName" placeholder="请输入协议名称"
                            maxlength="20" />
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row v-if="networkWay.Code != ''">
                      <el-col :span="24">
                        <el-form-item label="存储方式" prop="enable">
                          <el-select v-model="StorageConfig.enable" filterable remote placeholder="请选择存储方式"
                            :remote-method="storageRemoteMethod" :loading="storageloading" @change="storageChange">
                            <el-option v-for="item in StorageConfigList" :key="item.value" :label="item.label"
                              :value="item.value"></el-option>
                          </el-select>
                        </el-form-item>
                      </el-col>
                    </el-row>

                    <el-row v-if="networkWay.Code != ''">
                      <el-col :span="24">
                        <el-form-item label="通讯方式" prop="physicsWay">
                          <el-select v-model="addData.physicsWay" multiple :multiple-limit="4" placeholder="请选择通讯方式">
                            <el-option v-for="item in physicsWayList" :key="item.value" :label="item.label"
                              :value="item.value"></el-option>
                          </el-select>
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row>
                      <el-col :span="24">
                        <el-form-item label="协议图片" prop="productImageUrl" class="is-required">
                          <image-upload v-model="addData.productImageUrl" :limit="1"></image-upload>
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row>
                      <el-col :span="24">
                        <el-form-item label="备注说明" prop="remark">
                          <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="addData.remark"></el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-card>
                </div>
              </el-step>
            </el-steps>
          </div>
        </el-form>
      </div>
    </div>
    <div class="save_con">
      <el-button type="primary" @click="saveAddProduct" :disabled="addActive != 2">创建协议</el-button>
    </div>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { classTree, addProduct, channelList } from "@/api/rules/productModel";
import { iotScriptList } from "@/api/scrtemp.js";
import { historySourceList, historySourceInfo } from "@/api/rules/historysource";
export default {
  mixins: [resizeTableCon],
  data() {
    const fileMustUpload = (rule, value, callback) => {
      if (
        this.addData.productImageUrl == null ||
        this.addData.productImageUrl == ""
      ) {
        // 未上传文件
        callback("请上传封面");
      }
      callback();
    };
    return {
      canAddDone: true,
      addData: {
        name: "",
        remark: "",
        productImageUrl: null,
        physicsWay: [], //通讯方式
        interScripts: "",
      },
      addRules: {
        name: [{ required: true, trigger: "blur", message: "请输入协议名称" }],
        productImageUrl: [{ validator: fileMustUpload, trigger: "change" }],
      },
      isStorageConfig: false,
      StorageConfig: {
        enable: "", //存储方式
        org: "hd.mz", //存储组织
        url: "", //连接的URL
        token: "", //连接令牌
        bucket: "MZIoT", //bucket
      }, //存储方式
      StorageConfigList: [],
      storageloading: false,
      physicsWayList: [
        { label: "WiFi", value: "WiFi" },
        { label: "以太网", value: "以太网" },
        { label: "2G网络", value: "2G网络" },
        { label: "3G网络", value: "3G网络" },
        { label: "4G网络", value: "4G网络" },
        { label: "5G网络", value: "5G网络" },
        { label: "NB-IoT", value: "NB-IoT" },
        { label: "无", value: "None" },
      ],
      networkWay: null, //设备接入方式
      workWayList: [], //设备接入方式列表

      productImageUrl: null, //协议照片
      cardLoading: true, //判断卡片展示的分类列表内容是否在加载中
      parClassId: 0, //当前展示的分类父级id
      activeClassList: [], //展示的分类列表
      classmap: new Map(), //将分类分组
      classListData: [], //分类树数据
      selectedClass: {}, //选中的分类
      addActive: 0,
      //脚本模板相关参数
      queryScriptForm: {
        pageNum: 1,
        pageSize: 6,
        SearchKey: "",
      },
      scriptOption: [],
      scriptOptionList: [],
      scriptListMap: new Map(),
      optionLoading: false,
      scriptValue: {},
      // //脚本模板相关参数
    };
  },
  mounted() {
    this.storageConfigList();
    this.getClassList(); //查询分类
    this.getchannelList(); //查询设备接入方式
    this.getScriptList();
  },
  methods: {
    async storageChange(val) {
      if (val == "") {
        this.StorageConfig.enable = "";
        this.StorageConfig.org = "";
        this.StorageConfig.url = "";
        this.StorageConfig.token = "";
        this.StorageConfig.bucket = "";
      }
      else {
        let res = await historySourceInfo(val);
        this.StorageConfig.enable = val;
        if (res.data.StorageType == "influx") {
          let dataobj = JSON.parse(res.data.StorageConfig);
          this.StorageConfig.org = dataobj.org;
          this.StorageConfig.url = dataobj.url;
          this.StorageConfig.token = dataobj.token;
          this.StorageConfig.bucket = dataobj.bucket;
        }
      }

    },
    async storageRemoteMethod(query) {
      this.storageloading = true;
      await this.storageConfigList(query);
      this.storageloading = false;
    },
    async storageConfigList(query) {
      let res = await historySourceList({ pageNum: 1, pageSize: 25, Key: query });
      let newhistlist = res.data.List.map((item) => {
        let newitem = {}
        newitem.label = item.Name;
        newitem.value = item.Id;
        return newitem;
      })
      newhistlist.unshift({ label: "不启用", value: "" });
      this.StorageConfigList = newhistlist;

    },
    //脚本模板选择相关函数
    async remoteMethod(query) {
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(async () => {
          this.queryScriptForm.SearchKey = query;
          await this.getScriptList();
          this.optionLoading = false;
          this.scriptOption = this.scriptOptionList.filter((item) => {
            return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
        }, 200);
      } else {
        this.scriptOption = this.scriptOptionList;
        delete this.queryScriptForm.SearchKey;
        await this.getScriptList();
      }
    },
    choiceScriptVal(val) {
      console.log("选项变化", val);
      if (!val) {
        this.scriptOption = this.scriptOptionList;
        this.addData.interScripts = "";
        this.scriptValue = {};
      } else {
        this.addData.interScripts = val;
        if (this.addData.interScripts) {
          this.scriptValue = this.scriptListMap.get(this.addData.interScripts);
        }
        console.log(this.scriptValue, "this.scriptValue脚本模板");
        this.addActive = 2;
      }
    },
    async getScriptList() {
      //获取设备列表
      let response = await iotScriptList(this.queryScriptForm);
      if (!this.queryScriptForm.SearchKey) {
        this.scriptOptionList = response.data.List;
      }
      this.scriptOption = JSON.parse(JSON.stringify(response.data.List));
      this.initScriptMap(response.data.List);
    },
    initScriptMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx];
        // console.log("组合时用户列表curnode",curnode);
        this.scriptListMap.set(node[idx].Id, curnode);
        if (
          curnode.hasOwnProperty("Children") &&
          curnode.Children &&
          curnode.Children.length > 0
        ) {
          this.initScriptMap(curnode.Children);
        }
      }
    },
    //脚本模板选择相关函数
    getchannelList() {
      //获取设备接入方式
      channelList().then((rsp) => {
        // console.log("设备接入方式", rsp);
        this.workWayList = rsp.data;
      });
    },
    inputName() {
      if (this.addData.name) {
        this.canAddDone = false;
      } else {
        this.canAddDone = true;
      }
    },
    addPro() {
      let ModelTSL = {};
      if (this.scriptValue.InitModelTSL) {
        ModelTSL = JSON.parse(this.scriptValue.InitModelTSL)
      } else {
        let poObj = {
          name: "设备位置",
          code: "position",
          value: { lng: 0, lat: 0 },
          mapcode: "",
          enable: true,
          option: { type: "geo", lng: 0, lat: 0 },
          description: "",
        };
        let tags = [];

        tags.push(poObj);
        ModelTSL.tags = tags;
        ModelTSL.properties = [];
        ModelTSL.functions = [];
        ModelTSL.events = [];
      }

      let StorageConfig = {};
      if (this.StorageConfig.enable == "") {
        StorageConfig = {
          enable: "0",
        };
      } else{
        StorageConfig = JSON.parse(JSON.stringify(this.StorageConfig));
        StorageConfig.enable = "1";
      }

      let addData = {
        name: this.addData.name,
        remark: this.addData.remark,
        networkWay: this.networkWay.Code,
        physicsWay: this.addData.physicsWay.join(),
        photoUrl: this.addData.productImageUrl,
        classifiedId: this.selectedClass.Id,
        status: 0,
        storageConfig: JSON.stringify(StorageConfig),
        InterScripts: this.scriptValue.ScriptContent ? this.scriptValue.ScriptContent : '',
        ModelTSL: JSON.stringify(ModelTSL),
      };

      addProduct(addData)
        .then((response) => {
          if (response.code == 0) {
            this.$message.success("创建协议成功");
            this.$store.dispatch("tagsView/delView", this.$route);
            this.$router.push({
              path: "/iot/physicalModel/productAdd/" + response.data,
              query: {
                classId: this.selectedClass.Id,
              },
            });
          }
        })
        .catch((err) => {
          console.log("错误打印", err);
          this.$message.error(err.message);
        });
    },
    saveAddProduct() {
      //创建协议
      if (this.$refs["addParams"]) {
        this.$refs["addParams"].validate((val) => {
          if (this.networkWay.Code != "" && this.$refs["StorageConfig"]) {
            this.$refs["StorageConfig"].validate((val1) => {
              if (val && val1) {
                this.addPro();
              }
            });
          } else {
            this.addPro();
          }
        });
      }
    },
    choiceWorkWay(item) {
      //选择设备接入方式
      // console.log("接入方式", item);

      this.networkWay = item;
      if (item.Code != "mqtt_modbus") {
        this.addActive = 2;
      }
      // this.addActive = 2;

      if (this.networkWay.Code == "") {
        this.physicsWayList = [{ label: "无网络", value: "" }];
        this.addData.physicsWay = [];
        this.StorageConfig.enable = "";
      } else {
        this.physicsWayList = [
          { label: "WiFi", value: "WiFi" },
          { label: "以太网", value: "以太网" },
          { label: "2G网络", value: "2G网络" },
          { label: "3G网络", value: "3G网络" },
          { label: "4G网络", value: "4G网络" },
          { label: "5G网络", value: "5G网络" },
          { label: "NB-IoT", value: "NB-IoT" },
        ];
        this.addData.physicsWay = ["WiFi"];
      }
    },
    reselecWorkWay() {
      //重新选择设备接入方式
      this.addActive = 1;
      this.StorageConfig = {
        enable: "", //存储方式
        url: "", //连接的URL
        token: "", //连接令牌
        bucket: "MZIoT", //bucket
      };
      this.networkWay = null;
      this.addData.interScripts = "";
      this.scriptValue = {}
      this.addData.name = "";
      this.addData.remark = "";
      this.addData.productImageUrl = null;
      this.addData.physicsWay = []; //通讯方式

      this.resetForm("classForm");
    },
    reselectClass() {
      //重新选择分类
      this.StorageConfig = {
        enable: "", //存储方式
        url: "", //连接的URL
        token: "", //连接令牌
        bucket: "MZIoT", //bucket
      };
      this.selectedClass = {};
      this.networkWay = null;
      this.addData.interScripts = "";
      this.scriptValue = {}
      this.addData.name = "";
      this.addData.remark = "";
      this.addData.productImageUrl = null;
      this.addData.physicsWay = []; //通讯方式
      this.parClassId = 0;
      this.activeClassList = this.classmap.get(0);
      this.resetForm("classForm");
      this.addActive = 0;
    },
    choiceClass(item) {
      //选择分类
      if (item.Children && item.Children.length > 0) {
        this.cardLoading = true;
        this.activeClassList = this.classmap.get(item.Id);
        this.cardLoading = false;
        console.log(
          "当前选中的协议分类的子级列表",
          this.classmap,
          this.activeClassList
        );
      } else {
        this.selectedClass = item;
        this.addActive = 1;
      }
    },
    initClassMap(node) {
      for (let idx = 0; idx < node.length; idx++) {
        let curnode = node[idx].Children;
        // console.log("组合时协议分类列表curnode", curnode);
        this.classmap.set(node[idx].Id, curnode);
        if (
          node[idx].hasOwnProperty("Children") &&
          node[idx].Children &&
          curnode.length > 0
        ) {
          this.initClassMap(curnode);
        }
      }
    },
    getClassList() {
      //获取分类列表
      this.cardLoading = true;
      this.selectedClass = {};
      classTree().then((response) => {
        this.classListData = response.data;
        if (response.data.length > 0) {
          this.classmap.set(0, response.data);
          this.initClassMap(response.data);
          this.activeClassList = this.classmap.get(this.parClassId);
        }
        this.cardLoading = false;
        this.loading = false;
      });
    },
    next() {
      //步骤条
      if (this.addActive++ > 4) this.addActive = 0;
    },
  },
};
</script>
<style lang="less">
.save_con {
  width: 100%;
  height: 60px;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #ffffff;
  position: absolute;
  bottom: 0;
  left: 0;
  box-shadow: 0px -5px 5px 0px #e1e1e1;
}

.workWay_con {
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;
  margin-right: -15px;
  margin-bottom: -15px;

  .workWay_item_con {
    width: 288px;
    margin-bottom: 20px;
    margin-right: 15px;

    .workWay_remark {
      font-size: 12px;
      color: #B4B4B4;
      width: 100%;
      padding: 0 5px;
    }
  }

  .workWay_item {
    width: 100%;
    height: 60px;
    border: 1px solid #e1e1e1;
    line-height: 60px;
    text-align: left;
    box-sizing: border-box;
    padding-left: 20px;
    cursor: pointer;
    margin-bottom: 10px;
    font-size: 16px;
    border-radius: 3px;
  }
}

.read_js_func_con {
  border: 1px solid #dddddd;
  border-left: none;
}

.image_card {
  width: 220px;
  height: 210px;
}

.product-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.product-uploader .el-upload:hover {
  border-color: #409eff;
}

.product-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}

.product {
  width: 178px;
  height: 178px;
  display: block;
}

.steps_con .el-step:last-of-type .el-step__main,
.el-step:last-of-type .el-step__description {
  padding-right: 0;
}

.steps_con .el-step .el-step__description {
  padding-right: 0;
}

.steps_con .el-step__title.is-success {
  color: #4a4a4a;
}

.steps_con .el-step__head.is-wait {
  color: #4a4a4a;
  height: 70px;

  .el-step__line {
    top: 28px;
    padding: 5px 0;
  }
}

.steps_con .el-step__head.is-process {
  .el-step__line {
    top: 28px;
    padding: 5px 0;
  }
}

.steps_con .el-step.is-vertical .el-step__line {
  width: 2px;
  bottom: 5px;
  left: 11px;
}

.steps_con .el-step__head.is-success {
  color: #409eff;
  border-color: #409eff;

  .el-step__line {
    border-color: #0055ff;
    top: 28px;
    background-color: #0055ff;
  }
}
</style>
<style lang="less" scoped>
.workWay_con {
  .workWay_item {
    position: relative;

    .workWaySelectDiv::after {
      content: "\2713";
      position: absolute;
      right: 0;
      top: 0;
      font-size: 18px;
      z-index: 3;
      color: #409eff;
      width: 36px;
      line-height: 46px;
      text-align: center;
      transform: rotate(0deg);
      transform-origin: center center;
      font-weight: bold;
    }
  }
}

.selected_class {
  width: 100%;
  // height: 46px;
  border: 2px solid #f1f1f1;
  line-height: 46px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-sizing: border-box;
  padding: 0 10px;
  border-radius: 5px;
  color: #4a4a4a;

  .class_info {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    box-sizing: border-box;

    img {
      width: 38px;
      height: 38px;
      border-radius: 5px;
      margin-right: 12px;
    }
  }

  span {
    // font-weight: 700;
    font-size: 14px;
  }
}

.class_box_card {
  .class_item_con {
    display: flex;
    flex-wrap: wrap;
    justify-content: flex-start;
    margin-right: -10px;
    margin-bottom: -15px;

    .class_item {
      width: 219px;
      height: 46px;
      border: 1px solid #e1e1e1;
      line-height: 46px;
      text-align: left;
      display: flex;
      justify-content: flex-start;
      align-items: center;
      box-sizing: border-box;
      padding-left: 10px;
      margin-right: 10px;
      cursor: pointer;
      margin-bottom: 10px;

      img {
        width: 38px;
        height: 38px;
        border-radius: 5px;
        margin-right: 12px;
      }
    }

    .class_item:hover {
      border: 1px solid #0f73e6;
    }
  }
}

.attrFrom_con {
  padding: 20px;
}
</style>