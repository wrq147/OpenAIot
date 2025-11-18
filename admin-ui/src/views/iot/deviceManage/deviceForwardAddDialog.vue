<template>
  <div>
    <el-dialog top="5vh" :title="deviceAddDialogTitle" :visible.sync="deviceAddOpen" v-loading="addloading" center
      width="850px" class="deviceAddDialog deviceForwardAddDialog" ref="deviceAddDialogs" :close-on-click-modal="false">
      <el-form :model="deviceAddFrom" ref="deviceAddFrom" :rules="deviceAddRules" label-position="top"
        label-width="90px">
        <div>
          <div>
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="批次编号" prop="DeviceNumber">
                  <el-input type="text" v-model="deviceAddFrom.DeviceNumber"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发的批次编号" prop="DeviceNumber2">
                  <el-input type="text" v-model="deviceAddFrom.DeviceNumber2"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="设备名称" prop="name">
                  <el-input type="text" v-model="deviceAddFrom.name" placeholder="请输入设备名称"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发的名称" prop="name2">
                  <el-input type="text" v-model="deviceAddFrom.name2" placeholder="请输入转发设备名称"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="协议名称" prop="productId">
                  <el-input type="text" v-model="deviceAddFrom.productName" placeholder="请输入协议名称" :disabled="true"
                    v-if="isProductDev"></el-input>
                  <el-select style="width:100%" v-model="deviceAddFrom.productId" placeholder="请选择" v-else
                    :clearable="true" filterable remote reserve-keyword :remote-method="remoteMethod">
                    <el-option v-for="item in productLists" :key="item.Id" :label="item.Name"
                      :value="item.Id"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发设备协议名称" prop="productId2">
                  <el-input type="text" v-model="deviceAddFrom.productName2" placeholder="请输入转发设备协议名称" :disabled="true"
                    v-if="isProductDev"></el-input>
                  <el-select style="width:100%" v-model="deviceAddFrom.productId2" placeholder="请选择" v-else
                    :clearable="true" filterable remote reserve-keyword :remote-method="remoteMethod2">
                    <el-option v-for="item in productLists2" :key="item.Id" :label="item.Name"
                      :value="item.Id"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="通讯编码" prop="deviceId">
                  <el-input type="text" v-model="deviceAddFrom.deviceId" placeholder="请输入通讯编码"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发设备通讯编码" prop="deviceId2">
                  <el-input type="text" v-model="deviceAddFrom.deviceId2" placeholder="请输入转发设备通讯编码"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </div>
          
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="deviceAddOpen = false">取 消</el-button>
        <el-button type="primary" @click="saveDevice" :loading="saveDeviceLoading">{{saveDeviceLoading ? '提交中 ...' : '确定'}}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  addDevice,
  GenerateDeviceNumber
} from "@/api/rules/device";
import { productList } from "@/api/rules/productModel";
export default {
  name: 'AdminUiDeviceAddDialog',
  components: { },
  props: {
    isProductDev: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      deviceAddDialogTitle: '',
      saveDeviceLoading: false,
      deviceAddOpen: false, //添加设备弹窗
      deviceAddFrom: {
        productId: null,
        productName: "",
        productId2: null,
        productName2: "",
        dState: '',
        name: "",
        name2: "",
        deviceId: "",
        deviceId2: "",
        PhotoUrl: '',
        pd: "",
        Price: 0,
        Remark: ""
      },
      deviceAddRules: {
        productId: [
          { required: true, trigger: "change", message: "请选择协议" }
        ],
        productId2: [
          { required: true, trigger: "change", message: "请选择转发设备协议" }
        ],
        name: [{ required: true, trigger: "blur", message: "请输入设备名称" }],
        name2: [{ required: true, trigger: "blur", message: "请输入转发设备名称" }],
      },
      addloading: false,
      productLists: [],
      productLists2:[],
      stateoptions: [],//设备运行状态列表
      isOpendevPosition: false,//是否可以修改位置
    };
  },

  mounted() {
    this.getproductList();
    this.getproductList2();
  },

  methods: {
  
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
    remoteMethod2(query) {
      if (query !== "") {
        this.loading = true;
        setTimeout(() => {
          this.getproductList2(30, query);
        }, 200);
      } else {
        setTimeout(() => {
          this.getproductList2(30, undefined);
        }, 200);
      }
    },
    getproductList(pageSize, key) {
      let query = {
        Name: key,
        pageNum: 1
      }
      productList(query).then(async response => {
        if (response.code == 0) {
          if (response.data && response.data.List)
            this.productLists = response.data.List;
        }
      });
    },
    getproductList2(pageSize, key) {
      let query = {
        Name: key,
        pageNum: 1
      }
      productList(query).then(async response => {
        if (response.code == 0) {
          if (response.data && response.data.List)
            this.productLists2 = response.data.List;
        }
      });
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
      let rsp2 = await GenerateDeviceNumber()
      this.deviceAddDialogTitle = '添加转发设备'
      this.deviceAddFrom = {
        productId: null,
        productName: "",
        productId2: null,
        productName2: "",
        dState: '',
        DeviceNumber: rsp.data,
        DeviceNumber2: rsp2.data,
        name: "",
        name2: "",
        deviceId: "",
        deviceId2: "",
        PhotoUrl: '',
        Remark: "",
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
      // console.log(row, 'rowrowrow');
      this.stateoptions = []
      this.isOpendevPosition = false
      this.resetForm("deviceAddFrom");
      this.addloading = true;
      this.deviceAddDialogTitle = '编辑设备'
      this.deviceAddFrom = {
        id: row.Id,
        productId: row.ProductId,
        productName: row.ProductName,
        dState: row.DState,
        name: row.Name,
        deviceId: row.DeviceId,
        PhotoUrl: row.PhotoUrl,
        DeviceNumber: row.DeviceNumber,
        SkuNumber: row.SkuNumber,
        Remark: row.Remark,
        addressName: ''
      };
      this.$forceUpdate()
      this.addloading = false;
      this.deviceAddOpen = true;
    },
    saveDevice() {
      this.$refs["deviceAddFrom"].validate(valid => {
        if (valid) {

          let submitForm={
            productId: this.deviceAddFrom.productId,
            productName: this.deviceAddFrom.productName,
            dState: this.deviceAddFrom.dState,
            DeviceNumber: this.deviceAddFrom.DeviceNumber,
            name: this.deviceAddFrom.name,
            deviceId: this.deviceAddFrom.deviceId,
            PhotoUrl: this.deviceAddFrom.PhotoUrl,
            Remark: this.deviceAddFrom.Remark,
            addressName:this.deviceAddFrom.addressName
          }
          let submitForm2={
            productId: this.deviceAddFrom.productId2,
            productName: this.deviceAddFrom.productName2,
            dState: this.deviceAddFrom.dState,
            DeviceNumber: this.deviceAddFrom.DeviceNumber2,
            name: this.deviceAddFrom.name2,
            deviceId: this.deviceAddFrom.deviceId2,
            PhotoUrl: this.deviceAddFrom.PhotoUrl,
            Remark: this.deviceAddFrom.Remark,
            addressName:this.deviceAddFrom.addressName
          }

          delete submitForm.productName
          delete submitForm2.productName
          this.saveDeviceLoading = true;
          addDevice(submitForm).then(response => {
            if (response.code == 0) {
              addDevice(submitForm2).then(response => {
                if (response.code == 0) {
                  this.$message.success("添加设备成功");
                  this.deviceAddOpen = false;
                  this.$emit('loadDeviceList')
                  this.saveDeviceLoading = false;
                }
              }).catch(err => {
                this.$message.warning("添加转发设备失败");
                this.saveDeviceLoading = false;
              });
              
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
<style lang="less" scoped>
.deviceForwardAddDialog{
  ::v-deep .el-form-item{
    margin-bottom: 12px;
    .el-form-item__label{
      padding-bottom: 0;
    }
  }
}
</style>