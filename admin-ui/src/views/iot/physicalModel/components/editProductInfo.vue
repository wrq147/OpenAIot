<template>
  <div>
    <el-drawer title="修改产品" :visible.sync="editProductDrawer" direction="rtl" :wrapperClosable="false" :close-on-press-escape="false" @close="closeProductDrawer" size="600px"
      :destroy-on-close="true">
      <el-form ref="productInfos" :model="productInfos" :rules="productInfosRules" label-width="80px"
        style="padding:20px">
        <el-form-item label="产品名称" prop="Name">
          <el-input v-model="productInfos.Name" placeholder="请输入产品名称"></el-input>
        </el-form-item>
        <el-form-item label="产品分类" prop="ClassifiedId">
          <el-cascader ref="productCascader" v-model="productInfos.ClassifiedId" :options="productClassList"
            :props="{ children: 'Children', label: 'Name', value: 'Id' }" :show-all-levels="false" :checkStrictly="false"
            style="width:100%" @change="changeProductClass">
            <template slot-scope="{ node, data }">
              <span>{{ data.Name }}</span>
              <span v-if="data.Children && data.Children.length > 0">({{ data.Children.length }})</span>
            </template>
          </el-cascader>
        </el-form-item>
        <el-form ref="StorageConfig" :model="StorageConfig" :rules="StorageConfigRules" label-width="80px"
          class="storage_config2">
          <el-form-item label="存储方式" prop="enable" v-if="productInfos.NetworkWay">
            <el-select v-model="StorageConfig.enable" placeholder="请选择存储方式" @change="choiceStorageConfig">
              <el-option v-for="item in StorageConfigList" :key="item.value" :label="item.label"
                :value="item.value"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="存储组织" prop="org" v-if="StorageConfig.enable == '1'">
            <el-input v-model="StorageConfig.org" placeholder="请输入存储的组织名"></el-input>
          </el-form-item>
          <el-form-item label="连接的url" prop="url" v-if="StorageConfig.enable == '1'">
            <el-input v-model="StorageConfig.url" placeholder="请输入连接的url"></el-input>
          </el-form-item>
          <el-form-item label="连接令牌" prop="token" v-if="StorageConfig.enable == '1'">
            <el-input v-model="StorageConfig.token" placeholder="请输入连接令牌"></el-input>
          </el-form-item>
          <el-form-item label="数据库名(bucket)" prop="bucket" v-if="StorageConfig.enable == '1'">
            <el-input type="text" v-model="StorageConfig.bucket" placeholder="请输入存储的数据库名"></el-input>
          </el-form-item>
        </el-form>
        <el-form-item label="通讯方式" prop="PhysicsWay" v-if="productInfos.NetworkWay">
          <el-select filterable v-model="ProPhysicsWay" multiple :multiple-limit="4" placeholder="placeholder" style="width:100%" @change="PhysicsWayChange">
            <el-option :label="its.label" :value="its.value" v-for="its in physicsWayList" :key="its.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="接入方式" prop="NetworkWay">
          <el-select v-model="productInfos.NetworkWay" placeholder="placeholder" style="width:100%" @change="workWayChange">
            <el-option :label="its.Name" :value="its.Code" v-for="its in workWayList" :key="its.Code"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="产品图片" prop="PhotoUrl" class="is-required">
          <image-upload v-model="productInfos.PhotoUrl" :limit="1"></image-upload>
        </el-form-item>
        <el-form-item label="控制面板">
          <el-radio-group v-model="productInfos.isCustomMonitor">
            <el-radio :label="false">默认控制面板</el-radio>
            <el-radio :label="true">自定义控制面板</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="productInfos.isCustomMonitor" label="控制面板">
          <el-input type="text" v-model="productInfos.MonitorReportToken" placeholder="请输入自定义的控制面板"/>
        </el-form-item>
        <el-form-item label="备注说明" prop="Remark">
          <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="productInfos.Remark"></el-input>
        </el-form-item>
      </el-form>
      <div class="demo-drawer__footer" style="text-align: center;margin-top:40p;padding-bottom:20px">
        <el-button @click="editProductDrawer = false">取 消</el-button>
        <el-button type="primary" @click="editProductInfo" :loading="matchesLoading">{{ matchesLoading ? '提交中 ...' : '保 存'
        }}</el-button>
      </div>
    </el-drawer>
  </div>
</template>

<script>
import {
  classTree,
  editProduct,
  channelList
} from "@/api/rules/productModel";
export default {
  name: "AdminUiEditinfo",
  props: {
    productInfo: {
      type: Object,
      default: () => {
        return {};
      }
    },
  },
  data() {
    const fileMustUpload = (rule, value, callback) => {
      if (this.productInfos.PhotoUrl == null) {
        // 未上传文件
        callback("请上传封面");
      }
      callback();
    };
    return {
      matchesLoading: false,//控制提交按钮是否可以被点击
      editProductDrawer: false,//控制修改产品信息的弹窗打开和关闭
      //存储所有标识符，用于判定标识符是否重复
      StorageConfig: {
        enable: '0', //存储方式
        org: "hd.mz", //存储组织
        url: "", //连接的URL
        token: "", //连接令牌
        bucket: "MZIoT" //bucket
      }, //存储方式
      StorageConfigRules: {
        enable: [
          { required: true, trigger: "change", message: "请选则存储方式" }
        ], //存储方式
        org: [{ required: true, trigger: "blur", message: "请输入存储组织" }], //存储组织
        url: [{ required: true, trigger: "blur", message: "请输入连接的URL" }], //连接的URL
        token: [{ required: true, trigger: "blur", message: "请输入连接令牌" }], //连接令牌
        bucket: [{ required: true, trigger: "blur", message: "请输入bucket" }]
      }, //存储方式
      StorageConfigList: [
        { label: "不启用", value: "0" },
        { label: "启用", value: "1" }
      ],
      productClassList: [],//产品分类列表
      productInfosRules: {
        Name: [{ required: true, trigger: "blur", message: "请输入产品名称" }],
        ClassifiedId: [
          { required: true, trigger: "change", message: "请选择产品分类" }
        ],
        PhysicsWay: [
          { required: true, trigger: "change", message: "请选择通讯方式" }
        ],
        // NetworkWay: [
        //   { required: true, trigger: "change", message: "请选择接入方式" }
        // ],
        PhotoUrl: [{ validator: fileMustUpload, trigger: "change" }]
      },
      workWayList: [], //设备接入方式列表
      physicsWayList: [
        { label: "WiFi", value: "WiFi" },
        { label: "以太网", value: "以太网" },
        { label: "2G网络", value: "2G网络" },
        { label: "3G网络", value: "3G网络" },
        { label: "4G网络", value: "4G网络" },
        { label: "5G网络", value: "5G网络" },
        { label: "NB-IoT", value: "NB-IoT" },
        { label: "无", value: "None" }
      ], //通讯方式列表
      ProPhysicsWay:[],
      productInfos:{}
    };
  },

  mounted() {
    this.getchannelList();
    this.getProductClassList();
  },

  methods: {
    PhysicsWayChange(val){
      console.log(val,'val',this.ProPhysicsWay);
      if(this.ProPhysicsWay&&this.ProPhysicsWay.length>0){
        this.$set(this.productInfos,'PhysicsWay',this.ProPhysicsWay.join(','))
      }else{
        this.$set(this.productInfos,'PhysicsWay','')
      }
      // console.log(this.productInfos);
    },
    workWayChange(){
      // console.log("接入方式改变",this.productInfos);
      this.$forceUpdate()
    },
    getchannelList() {
      //获取设备接入方式
      channelList().then(rsp => {
        // console.log("设备接入方式", rsp);
        this.workWayList = rsp.data;
      });
    },
    openEditProductDrawer() {
      this.productInfo.isCustomMonitor=false
      this.productInfos=JSON.parse(JSON.stringify(this.productInfo))
      if(this.productInfo.PhysicsWay==null||this.productInfo.PhysicsWay==""){
        this.ProPhysicsWay=[];
      }
      else{
        this.ProPhysicsWay=this.productInfo.PhysicsWay.split(",");
      }
      if(this.productInfo.MonitorReportToken){
        this.productInfos.isCustomMonitor=true
      }else{
        this.productInfos.isCustomMonitor=false
      }

      //打开产品修改编辑弹出层
      this.editProductDrawer = true;
      if (this.productInfo.StorageConfig) {
        this.StorageConfig = JSON.parse(this.productInfo.StorageConfig);
      } else {
        this.StorageConfig = {
          enable: '0', //存储方式
          org: "hd.mz", //存储组织
          url: "", //连接的URL
          token: "", //连接令牌
          bucket: "MZIoT" //bucket
        };
      }
      this.$forceUpdate()
    },
    editProductInfo() {
      //保存修改的产品信息
      this.$refs["productInfos"].validate(val1 => {
        this.$refs["StorageConfig"].validate(val2 => {
          if (val1 && val2) {
            let StorageConfig = {};
            if (this.StorageConfig.enable == '1') {
              StorageConfig = JSON.parse(JSON.stringify(this.StorageConfig));
            }
            else{
              StorageConfig = {
                enable: '0'
              };
            }
            editProduct({
              id: this.productInfos.Id,
              classifiedId: this.productInfos.ClassifiedId,
              name: this.productInfos.Name,
              networkWay: this.productInfos.NetworkWay,
              photoUrl: this.productInfos.PhotoUrl,
              storageConfig: JSON.stringify(StorageConfig),
              physicsWay: this.ProPhysicsWay.join(),
              MonitorReportToken:this.productInfos.isCustomMonitor?this.productInfos.MonitorReportToken:'',
              remark: this.productInfos.Remark
            }).then(rsp => {
              // console.log("数据更新后返回", rsp);
              if (rsp.code == 0) {
                this.$modal.msgSuccess("修改成功");
                this.editProductDrawer = false;

                this.$emit('saveInfo', this.productInfos.ClassifiedId);
              }
            });
          }

        })

      });
    },
    getProductClassList() {
      //获取产品分类信息
      classTree().then(response => {
        if (response.data.length > 0) {
          // this.classmap.set(0, response.data);
          // this.initClassList(response.data);
          this.productClassList = response.data;
          this.initClassList2();
        }
      });
    },
    initClassList2() {
      //如果一级产品分类的子级列表为空，那么设置其Children为null
      for (let idx = 0; idx < this.productClassList.length; idx++) {
        if (
          this.productClassList[idx].Children &&
          this.productClassList[idx].Children.length == 0
        ) {
          this.productClassList[idx].Children = null;
        }
      }
    },
    choiceStorageConfig() {

      if (this.StorageConfig.enable == "1" && !this.StorageConfig.bucket) {
        this.StorageConfig = {
          enable: "1", //存储方式
          org: "hd.mz", //存储组织
          url: "", //连接的URL
          token: "", //连接令牌
          bucket: "MZIoT" //bucket
        };
      }
    },
    changeProductClass(value) {
      this.productInfos.ClassifiedId = this.$refs[
        "productCascader"
      ].getCheckedNodes()[0].value;
    },
    closeProductDrawer() {
      //关闭产品编辑弹出层
      // this.editProductDrawer = false;
      this.$emit("closeProductDrawer", false);
    }
  }
};
</script>

<style lang="less">
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
</style>