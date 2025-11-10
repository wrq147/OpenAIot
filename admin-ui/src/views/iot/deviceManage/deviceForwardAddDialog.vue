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
                <el-form-item label="第三方编码" prop="DeviceNumber">
                  <el-input type="text" v-model="deviceAddFrom.DeviceNumber"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发设备第三方编码" prop="DeviceNumber2">
                  <el-input type="text" v-model="deviceAddFrom.DeviceNumber2"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="10">
              <el-col :span="12" style="padding-right:20px;">
                <el-form-item label="设备分组" prop="groupId">
                  <treeselect style="width:100%" v-model="deviceAddFrom.groupId" :options="groupTreeList" :show-count="true"
                    :normalizer="normalizer" placeholder="请选择设备分组" />
                </el-form-item>
              </el-col>
              <el-col :span="12" style="padding-right:20px;">
                <el-form-item label="转发设备分组" prop="groupId2">
                  <treeselect style="width:100%" v-model="deviceAddFrom.groupId2" :options="groupTreeList" :show-count="true"
                    :normalizer="normalizer" placeholder="请选择转发设备分组" />
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
                <el-form-item label="转发设备名称" prop="name2">
                  <el-input type="text" v-model="deviceAddFrom.name2" placeholder="请输入转发设备名称"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="产品名称" prop="productId">
                  <el-input type="text" v-model="deviceAddFrom.productName" placeholder="请输入产品名称" :disabled="true"
                    v-if="isProductDev"></el-input>
                  <el-select @change="getproductTagList" style="width:100%" v-model="deviceAddFrom.productId" placeholder="请选择" v-else
                    :clearable="true" filterable remote reserve-keyword :remote-method="remoteMethod">
                    <el-option v-for="item in productLists" :key="item.Id" :label="item.Name"
                      :value="item.Id"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="转发设备产品名称" prop="productId2">
                  <el-input type="text" v-model="deviceAddFrom.productName2" placeholder="请输入转发设备产品名称" :disabled="true"
                    v-if="isProductDev"></el-input>
                  <el-select @change="getproductTagList2" style="width:100%" v-model="deviceAddFrom.productId2" placeholder="请选择" v-else
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
    <mapSelectCompt ref="mapSelectCompt" @returnMapInfo="returnMapInfo"></mapSelectCompt>
  </div>
</template>

<script>
import {
  addDevice,
  editDevice,
  GenerateDeviceNumber,
  productTagList,
  DeviceTagList,
  DeviceInfo
} from "@/api/rules/device";
import {
  productInfo
} from "@/api/rules/productModel";
import Treeselect from "@riophae/vue-treeselect";
import "@riophae/vue-treeselect/dist/vue-treeselect.css";
import mapSelectCompt from "@/views/iot/deviceManage/mapSelectCompt";
import { productList } from "@/api/rules/productModel";
export default {
  name: 'AdminUiDeviceAddDialog',
  components: { Treeselect, mapSelectCompt },
  props: {
    groupTreeList: {
      type: Array,
      default: () => {
        return []
      }
    },
    isProductDev: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      stepNum: 1,
      deviceAddDialogTitle: '',
      saveDeviceLoading: false,
      deviceAddOpen: false, //添加设备弹窗
      deviceAddFrom: {
        productId: null,
        productName: "",
        productId2: null,
        productName2: "",
        dState: '',
        groupId: null,
        groupId2: null,
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
          { required: true, trigger: "change", message: "请选择产品" }
        ],
        productId2: [
          { required: true, trigger: "change", message: "请选择转发设备产品" }
        ],
        name: [{ required: true, trigger: "blur", message: "请输入设备名称" }],
        name2: [{ required: true, trigger: "blur", message: "请输入转发设备名称" }],
      },
      addloading: false,
      firstLabelIfo: [],//一开始的标签数据
      labelIfo: [],
      labelIfo2:[],
      maxStep: 2,
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
    getproductTagList() {
      if (this.deviceAddFrom.productId) {
        productTagList({ id: this.deviceAddFrom.productId }).then(res => {
          this.labelIfo = res.data
        })
      }else{
        this.labelIfo = []
      }
    },
    getproductTagList2() {
      if (this.deviceAddFrom.productId2) {
        productTagList({ id: this.deviceAddFrom.productId2 }).then(res => {
          this.labelIfo2 = res.data
        })
      }else{
        this.labelIfo2=[]
      }
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
        // pageSize:pageSize,
        pageNum: 1
      }
      productList(query).then(async response => {
        // console.log("查询到的产品", response);
        if (response.code == 0) {
          if (response.data && response.data.List)
            this.productLists = response.data.List;
        }
        // this.productLists = response.data.List;
        // this.total = response.data.Total;
        // this.loading = false;
      });
    },
    getproductList2(pageSize, key) {
      let query = {
        Name: key,
        // pageSize:pageSize,
        pageNum: 1
      }
      productList(query).then(async response => {
        // console.log("查询到的产品", response);
        if (response.code == 0) {
          if (response.data && response.data.List)
            this.productLists2 = response.data.List;
        }
        // this.productLists = response.data.List;
        // this.total = response.data.Total;
        // this.loading = false;
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
    returnStep() {
      //回到上一步
      this.stepNum = this.stepNum - 1
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
      this.stepNum = 1
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
        groupId: null,
        groupId2: null,
        name: "",
        name2: "",
        deviceId: "",
        deviceId2: "",
        PhotoUrl: '',
        pd: "",
        Price: 0,
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
      // console.log(row, 'rowrowrow');
      this.stateoptions = []
      this.isOpendevPosition = false
      this.resetForm("deviceAddFrom");
      this.addloading = true;
      this.stepNum = 1
      this.deviceAddDialogTitle = '编辑设备'
      this.deviceAddFrom = {
        id: row.Id,
        productId: row.ProductId,
        productName: row.ProductName,
        dState: row.DState,
        groupId: row.GroupId,
        name: row.Name,
        deviceId: row.DeviceId,
        PhotoUrl: row.PhotoUrl,
        pd: row.PD,
        Price: row.Price,
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
      this.$forceUpdate()
      this.addloading = false;
      this.deviceAddOpen = true;
    },
    saveDevice() {
      this.$refs["deviceAddFrom"].validate(valid => {
        if (valid) {
          // let submitTag=[]
          let hasEditlabel = false
          let originLabel = JSON.stringify(this.firstLabelIfo)
          let lastLabel = JSON.stringify(this.labelIfo)
          if (originLabel == lastLabel) {
            hasEditlabel = false
          } else {
            hasEditlabel = true
          }
          let submitTag = this.labelIfo.map(row => {
            let obj = {
              code: row.Code,
              value: row.Value
            }
            return obj
          })
          let submitTag2 = this.labelIfo2.map(row => {
            let obj = {
              code: row.Code,
              value: row.Value
            }
            return obj
          })
          // let submitForm = JSON.parse(JSON.stringify(this.deviceAddFrom))
          let submitForm={
            productId: this.deviceAddFrom.productId,
            productName: this.deviceAddFrom.productName,
            dState: this.deviceAddFrom.dState,
            DeviceNumber: this.deviceAddFrom.DeviceNumber,
            groupId: this.deviceAddFrom.groupId,
            name: this.deviceAddFrom.name,
            deviceId: this.deviceAddFrom.deviceId,
            PhotoUrl: this.deviceAddFrom.PhotoUrl,
            pd: this.deviceAddFrom.pd,
            Price: this.deviceAddFrom.Price,
            Remark: this.deviceAddFrom.Remark,
            lat: this.deviceAddFrom.lat,
            lng: this.deviceAddFrom.lng,
            addressName:this.deviceAddFrom.addressName
          }
          let submitForm2={
            productId: this.deviceAddFrom.productId2,
            productName: this.deviceAddFrom.productName2,
            dState: this.deviceAddFrom.dState,
            DeviceNumber: this.deviceAddFrom.DeviceNumber2,
            groupId: this.deviceAddFrom.groupId2,
            name: this.deviceAddFrom.name2,
            deviceId: this.deviceAddFrom.deviceId2,
            PhotoUrl: this.deviceAddFrom.PhotoUrl,
            pd: this.deviceAddFrom.pd,
            Price: this.deviceAddFrom.Price,
            Remark: this.deviceAddFrom.Remark,
            lat: this.deviceAddFrom.lat,
            lng: this.deviceAddFrom.lng,
            addressName:this.deviceAddFrom.addressName
          }
          submitForm.tags = JSON.parse(JSON.stringify(submitTag))
          submitForm2.tags = JSON.parse(JSON.stringify(submitTag2))
          // if(!this.isEditTags&&submitForm.id){
          //   delete submitForm.tags
          // }
          // if (!hasEditlabel && submitForm.id) {
          //   delete submitForm.tags
          // }
          // delete submitForm.tags
          // delete submitForm2.tags
          delete submitForm.addressName
          delete submitForm2.addressName
          delete submitForm.productName
          delete submitForm2.productName
          if (this.deviceAddFrom.groupId) {
            submitForm.groupId = this.deviceAddFrom.groupId
          } else {
            submitForm.groupId = ''
          }
          if (this.deviceAddFrom.groupId2) {
            submitForm2.groupId = this.deviceAddFrom.groupId2
          } else {
            submitForm2.groupId = ''
          }
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