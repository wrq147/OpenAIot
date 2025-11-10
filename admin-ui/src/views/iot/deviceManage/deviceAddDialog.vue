<template>
  <div>
    <el-dialog top="5vh" :title="deviceAddDialogTitle" :visible.sync="deviceAddOpen" v-loading="addloading" center
      width="850px" class="deviceAddDialog" ref="deviceAddDialogs" :close-on-click-modal="false">
      <el-form :model="deviceAddFrom" ref="deviceAddFrom" :rules="deviceAddRules" label-position="right"
        label-width="90px">
        <div>
          <div v-if="stepNum == 1">
            <el-row :gutter="10">
              <el-col :span="12">
                <el-form-item label="第三方编码" prop="DeviceNumber">
                  <el-input type="text" v-model="deviceAddFrom.DeviceNumber"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="设备分组" prop="groupId">
                  <treeselect v-model="deviceAddFrom.groupId" :options="groupTreeList" :show-count="true"
                    :normalizer="normalizer" placeholder="请选择设备分组" />
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
                <el-form-item label="产品名称" prop="productId">
                  <el-input type="text" v-model="deviceAddFrom.productName" placeholder="请输入产品名称" :disabled="true"
                    v-if="isProductDev"></el-input>
                  <el-select @change="productChange" v-model="deviceAddFrom.productId" placeholder="请选择" v-else
                    :clearable="true" filterable remote reserve-keyword :remote-method="remoteMethod">
                    <el-option v-for="item in productLists" :key="item.Id" :label="item.Name"
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
          <div v-if="stepNum == 2">
            <el-row :gutter="10">
              <el-col :span="12" v-for="item in labelIfo" :key="item.Code">
                <el-form-item :label="item.Name" :prop="item.Code">
                  <el-switch v-if="item.Option.type == 'boolean'" v-model="item.Value"
                    :active-text="item.Option.trueText" :inactive-text="item.Option.falseText">
                  </el-switch>
                  <el-input-number v-else-if="item.Option.type == 'float'" v-model="item.Value"
                    :precision="item.Option.decimals" :step="0.1" :min="item.Option.min"
                    :max="item.Option.max"></el-input-number>
                  <el-input-number v-else-if="item.Option.type == 'int'" v-model="item.Value" :min="item.Option.min"
                    :max="item.Option.max"></el-input-number>
                  <el-select v-else-if="item.Option.type == 'enum'" v-model="item.Value" placeholder="请选择">
                    <el-option v-for="(elii, kii) in item.Option.elements" :key="kii" :label="elii"
                      :value="kii"></el-option>
                  </el-select>
                  <el-date-picker v-else-if="item.Option.type == 'date'" v-model="item.Value" value-format="timestamp"
                    type="datetime" placeholder="选择日期时间">
                  </el-date-picker>
                  <el-input v-else v-model="item.Value" placeholder="请输入内容"></el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </div>
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="deviceAddOpen = false" v-if="stepNum == 1">取 消</el-button>
        <el-button @click="returnStep" v-else>上一步</el-button>
        <el-button type="primary" @click="saveDevice" :loading="saveDeviceLoading" v-if="stepNum == maxStep">{{saveDeviceLoading ? '提交中 ...' : '确定'}}</el-button>
        <el-button type="primary" @click="nextStep" v-else>下一步</el-button>
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
        dState: '',
        groupId: null,
        name: "",
        deviceId: "",
        PhotoUrl: '',
        pd: "",
        Price: 0,
        Remark: ""
      },
      deviceAddRules: {
        productId: [
          { required: true, trigger: "change", message: "请选择产品" }
        ],
        name: [{ required: true, trigger: "blur", message: "请输入设备名称" }],
      },
      addloading: false,
      firstLabelIfo: [],//一开始的标签数据
      labelIfo: [],
      maxStep: 2,
      productLists: [],
      stateoptions: [],//设备运行状态列表
      isOpendevPosition: false,//是否可以修改位置
    };
  },

  mounted() {
    this.getproductList();
  },

  methods: {
    productChange() {
      this.getproductTagList();
      this.productInit();
    },
    productInit() {
      //产品切换
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
    getproductTagList() {
      if (this.deviceAddFrom.productId) {
        productTagList({ id: this.deviceAddFrom.productId }).then(res => {
          this.labelIfo = res.data
          // console.log(res, 'res产品标签结果');
          if (this.labelIfo.length == 0) {
            this.maxStep = 1
          } else {
            this.maxStep = 2
          }
        })
      }
    },
    nextStep() {
      //到下一步
      this.$refs["deviceAddFrom"].validate(valid => {
        if (valid) {
          this.stepNum = this.stepNum + 1
        }
      });
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
      this.deviceAddDialogTitle = '添加设备'
      this.deviceAddFrom = {
        productId: null,
        productName: "",
        dState: '',
        DeviceNumber: rsp.data,
        groupId: null,
        name: "",
        deviceId: "",
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
      console.log(row, 'rowrowrow');
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
      this.productInit()
      this.getDeviceTagList(row.Id)
      this.addloading = false;
      this.deviceAddOpen = true;
    },
    getDeviceTagList(id) {
      //获取标签信息
      DeviceTagList({ id: id }).then((res) => {
        // console.log("标签信息", res);
        this.labelIfo = JSON.parse(JSON.stringify(res.data));
        this.firstLabelIfo = JSON.parse(JSON.stringify(res.data));
        if (this.labelIfo.length == 0) {
          this.maxStep = 1
        } else {
          this.maxStep = 2
        }
      });
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
          let submitForm = JSON.parse(JSON.stringify(this.deviceAddFrom))
          submitForm.tags = JSON.parse(JSON.stringify(submitTag))
          // if(!this.isEditTags&&submitForm.id){
          //   delete submitForm.tags
          // }
          if (!hasEditlabel && submitForm.id) {
            delete submitForm.tags
          }
          delete submitForm.addressName
          delete submitForm.productName
          if (this.deviceAddFrom.groupId) {
            submitForm.groupId = this.deviceAddFrom.groupId
          } else {
            submitForm.groupId = ''
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
        }
      });
    },
  },
};
</script>