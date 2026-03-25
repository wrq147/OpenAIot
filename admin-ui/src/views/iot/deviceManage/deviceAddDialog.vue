<template>
  <div>
    <el-dialog top="5vh" :title="deviceAddDialogTitle" :visible.sync="deviceAddOpen" v-loading="addloading" center
      width="850px" class="deviceAddDialog" ref="deviceAddDialogs" :close-on-click-modal="false">
      <el-form :model="deviceAddFrom" ref="deviceAddFrom" :rules="formRules" label-position="right" label-width="90px">
        <div>
          <el-row :gutter="10">
            <el-col :span="12">
              <el-form-item label="批次编号" prop="DeviceNumber">
                <el-input type="text" v-model="deviceAddFrom.DeviceNumber"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="通讯编码" prop="deviceId">
                <el-input type="text" v-model="deviceAddFrom.deviceId" placeholder="请输入通讯编码"></el-input>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="10">
            <el-col :span="12" v-if="enableMes">
              <el-form-item label="所属产品" prop="MesProductInfo">
                <el-select v-model="deviceAddFrom.MesProductInfo" style="width:100%;" value-key="Id" filterable remote
                  reserve-keyword placeholder="请选择产品" :clearable="true" @clear="clearEvt"
                  :remote-method="remoteProductList" :loading="mesloading">
                  <el-option v-for="item in MesProList" :key="item.Id" :label="item.ProductName" :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12" v-if="deviceAddFrom.MesProductInfo == null || deviceAddFrom.MesProductInfo.Id == '1'">
              <el-form-item label="协议名称" prop="productId">
                <el-input type="text" v-model="deviceAddFrom.productName" placeholder="请输入协议名称" :disabled="true"
                  v-if="isProductDev"></el-input>
                <el-select @change="productChange" style="width:100%;" v-model="deviceAddFrom.productId"
                  placeholder="请选择" v-else :clearable="true" filterable remote reserve-keyword
                  :remote-method="remoteMethod">
                  <el-option v-for="item in productLists" :key="item.Id" :label="item.Name"
                    :value="item.Id"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="10">
            <el-col :span="12">
              <el-form-item label="设备名称" prop="name">
                <el-input type="text" v-model="deviceAddFrom.name" placeholder="请输入设备名称"></el-input>
              </el-form-item>
            </el-col>
          </el-row>


          <el-row :gutter="10"
            v-if="deviceAddFrom.productId && stateoptions && stateoptions.length > 0 || deviceAddFrom.productId && isOpendevPosition">
            <el-col :span="12">
              <el-form-item label="运行状态" prop="dState"
                v-if="deviceAddFrom.productId && stateoptions && stateoptions.length > 0">
                <el-select v-model="deviceAddFrom.dState" clearable placeholder="请选择运行状态">
                  <el-option v-for="item in stateoptions" :key="item" :label="item" :value="item"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item prop="addressName" label="设备位置" v-if="deviceAddFrom.productId && isOpendevPosition">
                <el-input placeholder="请选择设备位置" v-model="deviceAddFrom.addressName" @focus="choiceMap"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="10">
            <el-col :span="24">
              <el-form-item label="设备图片" prop="PhotoUrl">
                <image-upload v-model="deviceAddFrom.PhotoUrl" :limit="1"></image-upload>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="10">
            <el-col :span="24">
              <el-form-item label="备注" prop="Remark">
                <el-input type="textarea" :rows="4" v-model="deviceAddFrom.Remark" placeholder="请输入备注信息"
                  maxlength="500"></el-input>
              </el-form-item>
            </el-col>
          </el-row>

        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="deviceAddOpen = false">取 消</el-button>
        <el-button type="primary" @click="saveDevice" :loading="saveDeviceLoading">{{ saveDeviceLoading ? '提交中 ...' :
          '确定' }}</el-button>
      </div>
    </el-dialog>
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
  </div>
</template>

<script>
import {
  addDevice,
  editDevice,
  GenerateDeviceNumber,
  DeviceInfo
} from "@/api/rules/device";
import {
  productInfo
} from "@/api/rules/productModel";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
import { productList } from "@/api/rules/productModel";
import { factoryProductListPost, factoryProductInfo } from '@/api/factory/product'
import { checkPermi } from "@/utils/permission";
export default {
  name: 'AdminUiDeviceAddDialog',
  components: { mapSelectCompt },
  props: {
    isProductDev: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      MesProList: [],
      mesloading: false,
      enableMes: false,
      deviceAddDialogTitle: '',
      saveDeviceLoading: false,
      deviceAddOpen: false, //添加设备弹窗
      deviceAddFrom: {
        productId: null,
        productName: "",
        dState: '',
        name: "",
        deviceId: "",
        PhotoUrl: '',
        Remark: ""
      },
      formRules: {
        MesProductInfo: [
          { required: true, message: '请选择所属产品', trigger: 'change' }
        ],
        productId: [
          { required: true, message: '请选择协议名称', trigger: 'change' }
        ],
        name: [
          { required: true, message: '请输入设备名称', trigger: 'blur' }
        ]
      },
      addloading: false,
      productLists: [],
      stateoptions: [],//设备运行状态列表
      isOpendevPosition: false,//是否可以修改位置
    };
  },

  mounted() {
    this.getproductList();
    if (checkPermi(["/ProducerService/Product/List"])) {
      this.remoteProductList("");
      this.enableMes = true;
    }
  },

  methods: {
    clearEvt() {
      this.deviceAddFrom.MesProductInfo = null;
    },
    async InitMesProduct(mesProdId) {
      if (mesProdId == null || mesProdId == "1" || mesProdId == "") {
        this.deviceAddFrom.MesProductInfo = { "Id": "1", "ProductName": "物联设备" };
        return;
      }
      let res = await factoryProductInfo({ "id": mesProdId });
      this.deviceAddFrom.MesProductInfo = res.data;
    },
    async remoteProductList(query) {
      this.mesloading = true
      let obj = {
        IsIot: true,
        pageNum: 1,
        pageSize: 30
      }
      if (query == "") {
        let res = await factoryProductListPost(obj);
        this.MesProList = res.data.List;
        this.MesProList.unshift({ "Id": "1", "ProductName": "物联设备" });
      }
      else {
        obj.Key = query;
        let res = await factoryProductListPost(obj);
        this.MesProList = res.data.List;
      }
      this.mesloading = false
    },
    productChange() {
      this.productInit();
    },
    productInit() {
      productInfo({ id: this.deviceAddFrom.productId }).then(rsp => {
        let ModelTSL = JSON.parse(rsp.data.ModelTSL)
        let tags = ModelTSL.tags
        let position = tags.find(row => row.code == "position")
        let state = tags.find(row => row.code == "state")
        if (position) {
          this.isOpendevPosition = position.enable
        }
        if (state) {
          if (state.option && state.option.elements) {
            this.stateoptions = Object.values(state.option.elements)
            if (state.value) {
              this.deviceAddFrom.dState = state.value
            }
          }
        }
      })
    },

    remoteMethod(query) {
      if (query !== "") {
        this.loading = true;
        setTimeout(() => {
          this.getproductList(30, query);
        }, 200);
      } else {
        setTimeout(() => {
          this.getproductList(30, undefined);
        }, 200);
      }
    },
    getproductList(pageSize, key) {
      let query = {
        Name: key,
        pageNum: 1,
        WithSys:true
      }
      productList(query).then(async response => {
        if (response.code == 0) {
          if (response.data && response.data.List)
            this.productLists = response.data.List;
        }
      });
    },
    choiceMap() {
      //选择位置
      this.$refs.mapSelectCompt.choiceMap()
    },
    returnMapInfo(info) {
      this.deviceAddFrom.lat = info.Lat
      this.deviceAddFrom.lng = info.Lng
      this.deviceAddFrom.addressName = info.AddressName
      this.$forceUpdate()
    },
    normalizer(node) {
      if (node.Children == null || !node.Children.length) {
        delete node.Children;
      }
      return {
        id: node.Id,
        label: node.GroupName,
        children: node.Children
      };
    },
    async openAddDevice(productInfo) {
      //打开添加设备
      this.stateoptions = []
      this.isOpendevPosition = false
      this.resetForm("deviceAddFrom");
      this.addloading = true;
      let rsp = await GenerateDeviceNumber()
      this.deviceAddDialogTitle = '添加设备'
      this.deviceAddFrom = {
        productId: null,
        MesProductInfo: { "Id": "1", "ProductName": "物联设备" },
        productName: "",
        dState: '',
        DeviceNumber: rsp.data,
        name: "",
        deviceId: "",
        PhotoUrl: '',
        Remark: "",
        lat: 0,
        lng: 0,
        addressName: ''
      };
      if (productInfo) {
        this.deviceAddFrom.productName = productInfo.Name;
        this.deviceAddFrom.productId = productInfo.Id;
        this.productChange();
      }
      this.addloading = false;
      this.deviceAddOpen = true;

    },
    async editRowData(row) {
      //修改一行的数据
      this.stateoptions = []
      this.isOpendevPosition = false
      this.resetForm("deviceAddFrom");
      this.addloading = true;
      this.deviceAddDialogTitle = '编辑设备'
      this.deviceAddFrom = {
        id: row.Id,
        productId: row.ProductId,
        MesProductInfo: null,
        productName: row.ProductName,
        dState: row.DState,
        name: row.Name,
        deviceId: row.DeviceId,
        PhotoUrl: row.PhotoUrl,
        DeviceNumber: row.DeviceNumber,
        SkuNumber: row.SkuNumber,
        Remark: row.Remark,
        lat: row.Lat,
        lng: row.Lng,
        addressName: ''
      };
      if (row.Lat && row.Lng) {
        let deinfo = await DeviceInfo({ id: row.Id });
        this.deviceAddFrom.addressName = deinfo.data.AreaCodeName
      }
      this.InitMesProduct(row.MesProductId);
      this.$forceUpdate()
      this.productInit()
      this.addloading = false;
      this.deviceAddOpen = true;
    },
    saveDevice() {
      this.$refs.deviceAddFrom.validate((valid) => {
        if (!valid) {
          return false;
        }
        let submitForm = JSON.parse(JSON.stringify(this.deviceAddFrom))
        if (submitForm.MesProductInfo != null && submitForm.MesProductInfo.Id != "1") {
          submitForm.ProductId = submitForm.MesProductInfo.IOTProductId;
          submitForm.MesProductId = submitForm.MesProductInfo.Id;
        }
        else {
          submitForm.MesProductId = "1";
        }
        this.saveDeviceLoading = true;
        if (this.deviceAddFrom.id) {
          editDevice(submitForm).then(response => {

            if (response.code == 0) {
              this.$message.success("修改设备成功");
              this.deviceAddOpen = false;
              this.$emit('loadDeviceList')
              this.saveDeviceLoading = false;
            }
          }).catch(err => {
            this.saveDeviceLoading = false;
          });
        } else {
          addDevice(submitForm).then(response => {

            if (response.code == 0) {
              this.$message.success("添加设备成功");
              this.deviceAddOpen = false;
              this.$emit('loadDeviceList')
              this.saveDeviceLoading = false;
            }
          }).catch(err => {
            this.saveDeviceLoading = false;
          });
        }
      });

    },
  },
};
</script>