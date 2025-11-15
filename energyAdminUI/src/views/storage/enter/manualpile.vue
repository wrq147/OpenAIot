<template>
  <div style="padding: 20px 20px 0 20px; height: 100%" class="manualpage">
    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1050px"
      top="2vh">
      <el-form ref="form" :model="form" :rules="rules" label-width="120px">
        <div class="base-title">基本信息</div>
        <el-row>
          <el-col :span="12">
            <el-form-item label="入库单号" prop="StockNumber">
              <el-input v-model="form.StockNumber" :readonly="true"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="入库时间" prop="InDate">
              <el-date-picker v-model="form.InDate" type="datetime" placeholder="选择日期时间">
              </el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="16">
            <el-form-item label="物流单号" prop="ExpressNumber">
              <el-input placeholder="请输入物流单号" v-model="form.ExpressNumber" @change="expressSelected">
                <el-select v-if="form.ExpressNumber != ''" style="width: 130px" v-model="form.ExpressCompany"
                  slot="prepend" placeholder="请选择物流公司">
                  <el-option v-for="kditem in kdcompanys" :key="kditem.Code" :label="kditem.Name"
                    :value="kditem.Code"></el-option>
                </el-select>
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row v-if="form.ExpressCompany == 'shunfeng'">
          <el-col :span="16">
            <el-form-item label="联系电话" prop="ExpressPhone">
              <el-input placeholder="请输入物流单上的联系电话" type="tel" v-model="form.ExpressPhone"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="目标仓库" prop="HouseName">
              <div style="display: flex; align-items: center">
                <el-input class="houseipt" v-model="form.HouseName" readonly placeholder="请选择目标仓库"
                  @focus="openHouseDialog">
                  <i slot="suffix" @click="onClear" v-if="form.HouseId != null" class="el-icon-circle-close" style="
                      vertical-align: middle;
                      font-size: 22px;
                      cursor: pointer;
                    "></i>
                </el-input>
              </div>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row>
          <el-col :span="24">
            <el-form-item label="备注说明" prop="Remark">
              <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="form.Remark"
                style="width: 80%"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <div>
          <div class="items-title">
            <div style="font-size: 16px; color: #333">
              入库物品
              <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
              <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
            </div>
            <div>
              <el-row :gutter="15" type="flex" justify="end">
                <el-col :span="1.5">
                  <el-button type="warning" size="mini" plain @click="openChangePrice">
                    <i class="el-icon-edit"></i>
                    <span style="margin-left: 6px">批量改价</span>
                  </el-button>
                </el-col>

                <el-col :span="1.5">
                  <el-button type="primary" size="mini" plain @click="openDeviceDialog">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">选择物品</span>
                  </el-button>
                </el-col>

              </el-row>
            </div>
          </div>
          <el-table :data="form.List" stripe style="width: 100%">
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
            </el-table-column>
            <el-table-column prop="TargetName" label="名称">
            </el-table-column>
            <el-table-column label="类型" align="center" width="100">
              <template slot-scope="scope">
                <span v-if="scope.row.TargetType == 1">成品</span>
                <span v-else-if="scope.row.TargetType == 0">半成品</span>
              </template>
            </el-table-column>
            <el-table-column align="center" label="数量" width="140">
              <template slot-scope="scope">
                <span v-if="scope.row.TargetType == 1">{{
                  scope.row.Quantity
                }}</span>
                <el-input-number v-else v-model="scope.row.Quantity" :min="1" :max="9999" size="mini"></el-input-number>
              </template>
            </el-table-column>
            <el-table-column align="center" label="单价（元）" width="140">
              <template slot-scope="scope">
                <el-input-number v-model="scope.row.Price" :precision="2" size="mini" :min="0"></el-input-number>
              </template>
            </el-table-column>
            <el-table-column prop="address" align="center" label="操作" width="110">
              <template slot-scope="scope">
                <el-link type="danger" icon="el-icon-delete" @click="form.List.splice(scope.$index, 1)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <div style="margin-top: 10px" v-show="enterTemplateId > 0">
          <AddEmbed ref="flowForm">
            <div class="flow-title" style="font-size: 16px; color: #333">
              入库审核
            </div>
          </AddEmbed>
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button plain type="primary" @click="submitForm(2)">提 交</el-button>
        <el-button plain @click="submitForm(1)">保 存</el-button>
        <el-button plain @click="open = false">取 消</el-button>
      </div>


      <el-dialog top="2vh" width="960px" title="请选择需要入库的产品批次" :visible.sync="deviceOpen" append-to-body>
        <el-form :model="deviceQuery" ref="deviceForm" :inline="true"
          style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="查询关键字" prop="Key">
              <el-input v-model="deviceQuery.Key" placeholder="请输入名称、编号" clearable></el-input>
            </el-form-item>
            <el-form-item label="创建时间">
              <el-date-picker class="form_input_style" v-model="devDateRange" style="width: 232px"
                value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
          </div>

          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetDevice">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadDeviceList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table ref="devTable" :data="deviceList" tooltip-effect="dark" v-loading="deviceLoading" style="width: 100%"
          @selection-change="onDeviceChange" @row-click="clickDevRow" @select="devBoxSelect" row-key="Id">
          <el-table-column type="selection" width="55"> </el-table-column>
          <el-table-column prop="Number" label="物品编号" align="center" width="150">
          </el-table-column>
          <el-table-column label="产品标签" align="center">
            <template slot-scope="scope">
              <div>{{ scope.row.ProductLabel === 'U' ? '半成品' : '成品' }}</div>
            </template>
          </el-table-column>
          <el-table-column label="预览图片" align="center" width="150">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'"
                  :preview-src-list="[scope.row.PhotoUrl]">
                </el-image>
              </div>
            </template>
          </el-table-column>
          <el-table-column prop="BatchName" label="名称" />
          <el-table-column prop="TypeName" label="产品分组" />
          <el-table-column prop="SupplierName" label="供应商" />
        </el-table>
        <pagination v-show="deviceTotal > 0" :total="deviceTotal" :page.sync="deviceQuery.pageNum"
          :limit.sync="deviceQuery.pageSize" @pagination="loadDeviceList" />
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="deviceOpen = false">确 定</el-button>
        </div>
      </el-dialog>

      <HouseSelecter ref="houseDlg" @ok="handleCurrentChange"></HouseSelecter>
    </el-dialog>
  </div>
</template>

<script>
import { houseList, houseInfo } from "@/api/storage/house";

import {
  enterDevList,
  manualPile,
  submitManualPileModel,
  generateRKNumber,
  getEnterInfo,
  enterFormData
} from "@/api/storage/stock";

import { autoCompany } from "@/api/code.js";

import HouseSelecter from "../house/HouseSelecter.vue";
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
export default {
  name: "ManualPile",
  components: { AddEmbed, HouseSelecter },
  data() {
    return {
      title: "",
      kdcompanys: [],
      allloading: false,
      enterId: undefined,
      deviceOpen: false,
      // partsOpen: false,
      partsLoading: false,
      deviceLoading: false,
      open: false,
      form: {
        StockNumber: "",
        HouseName: "",
        HouseId: undefined,
        InDate: undefined,
        Remark: "",
        ExpressNumber: "",
        ExpressCompany: "",
        ExpressPhone: "",
        List: [],
      },
      // 表单校验
      rules: {
        HouseName: [
          { required: true, message: "目标仓库不能为空", trigger: "change" },
        ],
        InDate: [
          { required: true, message: "入库时间不能为空", trigger: "change" },
        ],
      },
      // 日期范围
      devDateRange: [],
      deviceQuery: {
        pageNum: 1,
        pageSize: 10,
        Key: "",
      },
      deviceTotal: 0,
      deviceList: [],
      enterTemplateId: 0,
      thisHandleDevSelectList: [],
    };
  },
  computed: {
    FlowParams: function () {
      return { "@from": this.form.StockNumber, "@fromtype": "入库单" };
    },
    finishedTotal(){
      let total=0
      this.form.List.map(ro=>{
        if(ro.TargetType==1){
          total=total+Number(ro.Quantity)
        }
      })
      return total
    },
    useTotal(){
      let total=0
      this.form.List.map(ro=>{
        if(ro.TargetType==0){
          total=total+Number(ro.Quantity)
        }
      })
      return total
    }
  },
  methods: {
    devBoxSelect(arr, row) {
      //点击多选框
      // console.log(row, "多选框点击事件");
      const selected = this.form.List.some(
        (item) => item.TargetType == 1 && item.TargetId === row.Id
      );
      if (selected) {
        this.form.List = this.form.List.filter(
          (rw) => rw.TargetType == 0 || rw.TargetType == 1 && rw.TargetId !== row.Id
        );
      }
    },
    clickDevRow(row) {
      // console.log("点击单行", row);
      const selected = this.form.List.some(
        (item) => item.TargetType == 1 && item.TargetId === row.Id
      );
      if (!selected) {
        // 选择
        this.$refs.devTable.toggleRowSelection(row, true);
      } else {
        // 取消
        this.$refs.devTable.toggleRowSelection(row, false);
        this.form.List = this.form.List.filter(
          (rw) => rw.TargetType == 0 || rw.TargetType == 1 && rw.TargetId !== row.Id
        );
      }
    },
    async expressSelected(val) {
      if (val.length < 8) return;
      this.form.ExpressCompany = (await autoCompany(val)).data;
    },
    async openDialog(id, hsid) {
      this.allloading = true;
      this.open = true;

      this.enterId = id;
      if (this.enterId == null) {
        this.title = "新增入库单";
        let ssp = await generateRKNumber();
        this.form.StockNumber = ssp.data;
        this.form.HouseName = "";
        this.form.InDate = this.parseTime(Date.now());
        this.form.HouseId = undefined;
        this.form.Remark = "";
        this.form.ExpressNumber = "";
        this.form.ExpressCompany = "";
        this.form.ExpressPhone = "";
        this.form.List = [];

        if (hsid != null) {
          let hsInfo = await houseInfo(hsid, false);
          this.form.HouseName = hsInfo.data.StoreName;
          this.form.HouseId = hsInfo.data.Id;
          this.enterTemplateId = hsInfo.data.EnterTemplateId;
        } else {
          let hslist = (await houseList({ IsSystem: true })).data.List;
          if (hslist.length > 0) {
            this.form.HouseName = hslist[0].StoreName;
            this.form.HouseId = hslist[0].Id;
            this.enterTemplateId = hslist[0].EnterTemplateId;
          } else {
            this.enterTemplateId = 0;
          }
        }
      } else {
        this.title = "编辑入库单";
        let enterInfo = await getEnterInfo(this.enterId);
        this.enterTemplateId = enterInfo.data.ToHouseEnterTemplateId;
        this.form.StockNumber = enterInfo.data.StockNumber;
        this.form.HouseName = enterInfo.data.ToHouseName;
        this.form.InDate = enterInfo.data.InDate;
        this.form.HouseId = enterInfo.data.ToHouseId;
        this.form.Remark = enterInfo.data.Remark;
        this.form.ExpressNumber = enterInfo.data.ExpressNumber;
        this.form.ExpressCompany = enterInfo.data.ExpressCompany;
        this.form.ExpressPhone = enterInfo.data.ExpressPhone;
        this.form.List = enterInfo.data.List;
      }

      if (this.enterTemplateId > 0) {
        let fromInfo = await enterFormData({
          "applyNumber": this.form.StockNumber,
          "toHouseId": this.form.HouseId,
        })
        await this.$refs.flowForm.InitData(
          this.enterTemplateId,
          this.FlowParams,
          this.enterId == null ? null : this.form.StockNumber,
          fromInfo.data
        );
      }

      this.kdcompanys = await this.$store.dispatch("datas/kuaiDiList");
      this.allloading = false;
    },
    openChangePrice() {
      this.$prompt("请输入要更改的价格", "新的价格", {
        confirmButtonText: "提交",
        cancelButtonText: "取消",
        inputPattern: /(^[1-9]\d*(\.\d{1,2})?$)|(^0(\.\d{1,2})?$)/,
        inputErrorMessage: "格式错误",
        inputPlaceholder: "请输入新的价格",
        inputValue: "0",
        closeOnClickModal: false,
      }).then(({ value }) => {
        if (this.form.List != null) {
          this.form.List.forEach((item) => {
            item.Price = Number(value);
          });
        }
      });
    },
    openHouseDialog() {
      this.$refs.houseDlg.openHouseDialog("请选择目标仓库");
    },
    openDeviceDialog() {
      this.deviceOpen = true;
      this.loadDeviceList();
    },
    onClear() {
      this.form.HouseName = "";
      this.form.HouseId = null;
    },
    resetDevice() {
      this.devDateRange = [];
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    async handleCurrentChange(val) {
      this.form.HouseName = val.StoreName;
      this.form.HouseId = val.Id;
      this.enterTemplateId = val.EnterTemplateId;
      if (this.enterTemplateId > 0) {
        let fromInfo = await enterFormData({"applyNumber": this.form.StockNumber,"toHouseId": this.form.HouseId,})
        await this.$refs.flowForm.InitData(
          this.enterTemplateId,
          this.FlowParams,
          this.enterId == null ? null : this.form.StockNumber,
          fromInfo.data
        );
      }
    },
    loadDeviceList() {
      this.deviceLoading = true;
      enterDevList(this.addDateRange(this.deviceQuery, this.devDateRange)).then(
        (rsp) => {
          this.deviceList = rsp.data.List;
          this.deviceTotal = rsp.data.Total;

          this.$nextTick(() => {
            //设置选中状态
            this.deviceList.forEach((element) => {
              if (
                this.form.List.some((item) => {
                  return item.TargetType == 1 && item.TargetId == element.Id;
                })
              ) {
                this.$refs.devTable.toggleRowSelection(element);
              }
            });
            this.deviceLoading = false;
          });
        }
      );
    },
    onDeviceChange(val) {
      if (this.deviceLoading) return;
      // this.form.List = this.form.List.filter((x) => x.TargetType == 0);
      let IdList = this.form.List.map((item) => {
        return item.TargetId;
      });
      val.forEach((element) => {
        if (IdList.includes(element.Id)) {
        } else {
          this.form.List.push({
            TargetType: element.ProductLabel == 'U' ? 0 : 1,
            TargetId: element.Id,
            TargetNumber: element.Number,
            Quantity: 1,
            Price: element.Price?element.Price:0,
            TargetName: element.BatchName,
          });
        }
      });
    },
    submitForm(st) {
      this.$refs["form"].validate(async (valid) => {
        if (valid) {
          if (this.form.List.length == 0) {
            this.$message.error("请选择要入库的产品");
            return;
          }
          this.form.Id = this.enterId;

          let response = await manualPile(this.form);
          //提交审批表单
          if (this.enterTemplateId > 0) {
            if (st == 2) {
              let tmpmodel = this.$refs.flowForm.getModel();
              tmpmodel["id"] = response.data;
              await submitManualPileModel(tmpmodel);
            }
            else {
              await this.$refs.flowForm.submitForm(st);
            }
          }
          else {
            await submitManualPileModel({ id: response.data });
          }

          this.$modal.msgSuccess("操作成功");
          this.open = false;
          this.$emit("confirm");
        }
      });
    },
  },
};
</script>
<style lang="scss">
.num_li{
    font-weight: normal;
    font-size: 14px;
    color: #666666;
    margin-left: 10px;
}
.imgwrap {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;

  .el-image {
    display: flex;
    width: 80px;
    height: 80px;
    justify-content: center;
    align-items: center;
  }
}

.base-title {
  font-size: 16px;
  color: #333;
  background-color: rgb(249, 250, 252);
  padding: 0 15px;
  height: 48px;
  display: flex;
  flex-direction: row;
  align-items: center;
  margin-bottom: 20px;
}

.items-title,
.flow-title {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  height: 48px;
  background-color: rgb(249, 250, 252);
  padding: 0 15px;
  margin-bottom: 5px;
}

.manualpage .el-table .el-table__header-wrapper th {
  background: none;
}

.manualpage {
  &>.el-dialog__wrapper {
    &>.el-dialog {
      &>.el-dialog__footer {
        background-color: #fafafa;
      }
    }
  }
}

.my-autocomplete {
  li {
    line-height: normal;
    padding: 7px;

    .name {
      text-overflow: ellipsis;
      overflow: hidden;
    }
  }
}

.houseipt {
  .el-input__inner {
    cursor: pointer;
  }
}
</style>