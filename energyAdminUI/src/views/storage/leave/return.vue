<template>
  <div style="padding:20px 20px 0 20px;height:100%" class="manualpage">
    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1050px"
      top="2vh">
      <el-form ref="form" :model="form" :rules="rules" label-width="120px">
        <div class="base-title">基本信息</div>
        <el-row>
          <el-col :span="12">
            <el-form-item label="出库单号" prop="StockNumber">
              <el-input v-model="form.StockNumber" :readonly="true"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="出库时间" prop="OutDate">
              <el-date-picker v-model="form.OutDate" type="datetime" placeholder="选择日期时间"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="16">
            <el-form-item label="物流单号" prop="ExpressNumber">
              <el-input placeholder="请输入物流单号" v-model="form.ExpressNumber" @change="expressSelected">
                <el-select v-if="form.ExpressNumber != ''" style="width:130px" v-model="form.ExpressCompany"
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
            <el-form-item label="所出仓库" prop="FromHouseId">
              <div style="display:flex;align-items: center;">
                <el-input class="houseipt" v-model="form.FromHouseName" readonly placeholder="请选择所出仓库" :disabled="true">
                  <!-- <i slot="suffix" @click="onClear" v-if="form.FromHouseId != null" class="el-icon-circle-close"
                  style="font-size: 22px;cursor: pointer;vertical-align: middle;"></i> -->
                </el-input>
              </div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="关联的入库单" prop="sourceEnterId">
              <div style="display:flex;align-items: center;">
                <el-input class="houseipt" v-model="form.sourceEnterNumber" readonly placeholder="请选择关联的入库单"
                  @focus="onOpenTarget">
                  <i slot="suffix" @click="onAgentClear" v-if="form.sourceEnterId != null" class="el-icon-circle-close"
                    style="font-size: 22px;cursor: pointer;vertical-align: middle;"></i>
                </el-input>
              </div>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注说明" prop="Remark">
              <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="form.Remark"
                style="width:80%"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <div>
          <div class="items-title">
            <div style="font-size:16px;color:#333;">出库物品
              <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
              <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
            </div>
            <div>
              <el-row :gutter="15" type="flex" justify="end">
                <el-col :span="1.5">
                  <el-button type="warning" size="mini" plain @click="openChangePrice">
                    <i class="el-icon-edit"></i>
                    <span style="margin-left:6px">批量改价</span>
                  </el-button>
                </el-col>

                <!-- <el-col :span="1.5">
                  <el-button type="primary" size="mini" plain @click="openWupinDialog">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">选择物品</span>
                  </el-button>
                </el-col> -->

                <!-- <el-col :span="1.5">
                  <el-button type="primary" size="mini" plain>
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">扫码添加</span>
                  </el-button>
                </el-col> -->
              </el-row>
            </div>
          </div>
          <el-table :data="form.List" stripe style="width: 100%">
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180"></el-table-column>
            <el-table-column prop="TargetName" label="物品名称"></el-table-column>
            <el-table-column label="存储类型" align="center" width="100">
              <template slot-scope="scope">
                <span v-if="scope.row.TargetType == 1">成品</span>
                <span v-else-if="scope.row.TargetType == 0">半成品</span>
              </template>
            </el-table-column>
            <el-table-column align="center" label="数量" width="140">
              <template slot-scope="scope">
                <span v-if="scope.row.TargetType == 1">{{ scope.row.Quantity }}</span>
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


        <div style="margin-top:10px;" v-show="leaveTemplateId > 0">
          <AddEmbed ref="flowForm">
            <div class="flow-title" style="font-size:16px;color:#333;">出库审核</div>
          </AddEmbed>
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button plain type="primary" @click="submitForm(2)">提 交</el-button>
        <el-button plain @click="submitForm(1)">保 存</el-button>
        <el-button plain @click="open = false">取 消</el-button>
      </div>


      <el-dialog width="960px" title="请选择导入的物品" :visible.sync="wupinOpen" :close-on-click-modal="false" append-to-body>
        <el-form :model="wupinQuery" :inline="true" ref="wupinForm"
          style="display: flex;justify-content: space-between;">
          <div>
            <el-form-item label="搜索关键字" prop="Key">
              <el-input v-model="wupinQuery.Key" placeholder="搜索物品名称或物品编号" clearable></el-input>
            </el-form-item>
            <el-form-item label="创建日期">
              <el-date-picker class="form_input_style" v-model="wupinDateRange" style="width:232px"
                value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
          </div>
          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetWupin">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadWupinList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table ref="wupinTable" :data="wupinList" tooltip-effect="dark" v-loading="wupinLoading" style="width: 100%"
          @selection-change="onWupinChange">
          <el-table-column type="selection" width="55"></el-table-column>
          <el-table-column prop="DeviceNumber" label="物品编号" align="center" width="150"></el-table-column>
          <el-table-column label="预览图片" align="center" width="150">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'"
                  :preview-src-list="[scope.row.PhotoUrl]">
                </el-image>
              </div>
            </template>
          </el-table-column>
          <el-table-column prop="Name" label="物品名称"></el-table-column>
          <el-table-column align="center" label="数量" width="140">
            <template slot-scope="scope">
              <span>{{ scope.row.Quantity }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="TargetType" align="center" label="存储类型" width="220">
            <template slot-scope="scope">
              <div>
                {{ scope.row.TargetType == 1 ? '成品' : '半成品' }}
              </div>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="wupinTotal > 0" :total="wupinTotal" :page.sync="wupinQuery.pageNum"
          :limit.sync="wupinQuery.pageSize" @pagination="loadWupinList" />

        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="onWupinConfirm">确 定</el-button>
          <el-button @click="wupinOpen = false">取 消</el-button>
        </div>
      </el-dialog>

      <Enterlist ref="kfDlg" @ok="onTargetChange" :enterMethod="21"></Enterlist>
      <HouseSelecter ref="houseDlg" @ok="handleCurrentChange"></HouseSelecter>
    </el-dialog>

  </div>
</template>

<script>
import { houseInfo } from "@/api/storage/house";
import HouseSelecter from "../house/HouseSelecter.vue";
import {
  leaveAdd,
  leaveEdit,
  generateCKNumber,
  leaveSubmitModel,
  stockList,
  getLeaveInfo,
  getEnterInfo,
  leaveFormData
} from "@/api/storage/stock";

import { autoCompany, kuaiDiCompanys } from "@/api/code.js";
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
import Enterlist from "../enter/enterlist"
export default {
  name: "ManualPile",
  components: { AddEmbed, Enterlist, HouseSelecter },
  data() {
    return {
      title: "",
      kdcompanys: [],
      allloading: false,
      leaveId: undefined,
      wupinOpen: false,
      wupinLoading: false,
      open: false,
      form: {
        LeaveMethod: 1,
        StockNumber: "",
        ToOrgId: undefined,
        ToName: "",
        sourceEnterNumber: '',
        sourceEnterId: '',
        FromHouseId: undefined,
        FromHouseName: "",
        OutDate: undefined,
        Remark: "",
        ExpressNumber: "",
        ExpressCompany: "",
        ExpressPhone: "",
        List: []
      },
      // 表单校验
      rules: {
        HouseName: [
          { required: true, message: "目标仓库不能为空", trigger: "change" }
        ],
        InDate: [
          { required: true, message: "入库时间不能为空", trigger: "change" }
        ],
        sourceEnterId: [
          { required: true, message: "关联的入库单不能为空", trigger: "change" }
        ],
        FromHouseId: [
          { required: true, message: "所出仓库不能为空", trigger: "change" }
        ]
      },
      // 日期范围
      wupinDateRange: [],
      wupinQuery: {
        pageNum: 1,
        pageSize: 20,
        HouseId: ''
      },
      wupinTotal: 0,
      wupinList: [],
      wupinSelectArr: [],
      showFlow: false,
      leaveTemplateId: 0,
    };
  },
  computed: {
    FlowParams: function () {
      return {
        "@from": this.form.StockNumber,
        "@fromtype": "出库单"
      };
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
    openChangePrice() {
      this.$prompt("请输入要更改的价格", "新的价格", {
        confirmButtonText: "提交",
        cancelButtonText: "取消",
        inputPattern: /(^[1-9]\d*(\.\d{1,2})?$)|(^0(\.\d{1,2})?$)/,
        inputErrorMessage: "格式错误",
        inputPlaceholder: "请输入新的价格",
        inputValue: "0",
        closeOnClickModal: false
      }).then(({ value }) => {
        if (this.form.List != null) {
          this.form.List.forEach(item => {
            item.Price = Number(value);
          })
        }
      });
    },
    async expressSelected(val) {
      if (val.length < 8) return;
      this.form.ExpressCompany = (await autoCompany(val)).data;
    },
    async openDialog(id) {
      this.allloading = true;
      this.open = true;

      this.$nextTick(async () => {
        this.resetForm("form");
        this.leaveId = id;
        if (this.leaveId == null) {
          this.title = "新增部分退货单";
          this.form.FromHouseName = "";
          this.form.FromHouseId = undefined;
          this.form.Remark = "";
          this.form.ExpressNumber = "";
          this.form.ExpressCompany = "";
          this.form.ExpressPhone = "";
          this.form.ToOrgId = undefined;
          this.form.ToName = "";
          this.form.sourceEnterNumber = '';
          this.form.sourceEnterId = '';
          this.form.List = [];
          this.resetForm("form");
          let ssp = await generateCKNumber();
          this.form.StockNumber = ssp.data;
          this.form.OutDate = this.parseTime(Date.now());
        } else {
          try {
            this.title = "编辑退货单";
            let leaveInfo = await getLeaveInfo(this.leaveId);
            console.log("退货单信息", leaveInfo);
            this.form.StockNumber = leaveInfo.data.StockNumber;
            this.form.FromHouseName = leaveInfo.data.FromHouseName;
            this.leaveTemplateId = leaveInfo.data.FromHouseLeaveTemplateId;
            this.form.OutDate = leaveInfo.data.OutDate;
            this.form.FromHouseId = leaveInfo.data.FromHouseId;
            this.form.Remark = leaveInfo.data.Remark;
            this.form.ExpressNumber = leaveInfo.data.ExpressNumber;
            this.form.ExpressCompany = leaveInfo.data.ExpressCompany;
            this.form.ExpressPhone = leaveInfo.data.ExpressPhone;
            this.form.ToOrgId = leaveInfo.data.ToOrgId;
            this.form.List = leaveInfo.data.List;
            this.form.ToName = leaveInfo.data.ToName;
            let res = await getEnterInfo(leaveInfo.data.SourceEnterId)
            this.form.sourceEnterNumber = res.data.StockNumber;
            this.form.sourceEnterId = res.data.Id;
          } catch (error) {
            console.log("报错了", error);
          }

        }
        if (this.leaveTemplateId > 0) {
          let fromInfo=await leaveFormData({
            "applyNumber": this.form.StockNumber,
            "fromHouseId": this.form.FromHouseId ,
            })
            await this.$refs.flowForm.InitData(
            this.leaveTemplateId,
            this.FlowParams,
            this.leaveId == null ? null : this.form.StockNumber,
            fromInfo.data
            );
          // await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, this.leaveId == null ? null : this.form.StockNumber);
        }

        let tmpkdrsp = await kuaiDiCompanys();
        this.kdcompanys = tmpkdrsp.data;
        this.allloading = false;
      });
    },
    openHouseDialog() {
      this.$refs.houseDlg.openHouseDialog("请选择要出库的仓库");
    },
    openWupinDialog() {
      if (this.form.FromHouseId) {
        this.wupinOpen = true;
        this.loadWupinList();
      } else {
        this.$message.error("请选择要出库的仓库");
      }

    },
    onClear() {
      this.form.FromHouseName = "";
      this.form.FromHouseId = null;
    },
    resetWupin() {
      this.wupinDateRange = [];
      this.resetForm("wupinForm");
      this.loadWupinList();
    },
    async handleCurrentChange(val) {
      this.form.FromHouseName = val.StoreName;
      this.form.FromHouseId = val.Id;
      this.leaveTemplateId = val.LeaveTemplateId;
      if (this.leaveTemplateId > 0) {
        let fromInfo=await leaveFormData({
        "applyNumber": this.form.StockNumber,
        "fromHouseId": this.form.FromHouseId ,
        })
        await this.$refs.flowForm.InitData(
        this.leaveTemplateId,
        this.FlowParams,
        this.leaveId == null ? null : this.form.StockNumber,
        fromInfo.data
        );
        // await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, this.leaveId == null ? null : this.form.StockNumber);
      }
    },
    loadWupinList() {
      this.wupinLoading = true;
      this.wupinSelectArr = [];
      this.wupinQuery.HouseId = this.form.FromHouseId
      stockList(this.addDateRange(this.wupinQuery, this.wupinDateRange)).then(
        rsp => {
          this.wupinList = rsp.data.List;
          // console.log('物品列表', this.wupinList);
          this.wupinTotal = rsp.data.Total;
          this.wupinLoading = false;
        }
      );
    },

    onWupinChange(val) {
      this.wupinSelectArr = val;
    },
    onWupinConfirm() {
      if (this.wupinSelectArr.length > 0) {
        let filteritems = this.form.List.filter(x => this.wupinSelectArr.some(w => w.TargetId == x.TargetId));
        if (filteritems.length > 0) {
          this.$message.error("不可以重复添加物品 '" + filteritems.map(x => x.TargetName).join() + "'");
          return;
        }
        this.wupinSelectArr.forEach(element => {
          this.form.List.push({
            TargetType: element.TargetType,
            TargetId: element.TargetId,
            TargetNumber: element.DeviceNumber,
            Quantity: element.Quantity,
            TargetName: element.Name,
            Price: element.Price
          });
        });
      }
      this.wupinOpen = false;
    },
    onAgentClear() {
      this.form.sourceEnterId = undefined;
      this.form.sourceEnterNumber = "";
    },
    onOpenTarget() {
      this.$refs.kfDlg.openAgentDialog();
    },
    async onTargetChange(val) {
      // console.log("选择入库单", val);
      this.form.sourceEnterId = val.Id;
      this.form.sourceEnterNumber = val.StockNumber;
      await this.loadEnterInfo(val.Id)
      let hsInfo = await houseInfo(val.ToHouseId, false);
      let obj = {
        StoreName: hsInfo.data.StoreName,
        Id: hsInfo.data.Id,
        LeaveTemplateId: hsInfo.data.LeaveTemplateId ? hsInfo.data.LeaveTemplateId : 0
      }
      this.handleCurrentChange(obj)
    },
    async loadEnterInfo(id) {
      let res = await getEnterInfo(id)
      console.log(res, '入库单详情');
      this.form.List = res.data.List
    },
    submitForm(st) {
      this.$refs["form"].validate(async valid => {
        if (valid) {
          if (this.form.List.length == 0) {
            this.$message.error("请选择要出库的物品");
            return;
          }
          this.form.Id = this.leaveId;
          let response;
          if (this.leaveId == null) {
            response = await leaveAdd(this.form);
          }
          else {
            response = await leaveEdit(this.form);
          }
          if (this.leaveTemplateId > 0) {
            if (st == 2) {
              let tmpmodel = this.$refs.flowForm.getModel();
              tmpmodel["id"] = response.data;
              await leaveSubmitModel(tmpmodel);
            }
            else {
              await this.$refs.flowForm.submitForm(st);
            }
          }
          else {
            await leaveSubmitModel({ id: response.data });
          }
          this.$modal.msgSuccess("操作成功");
          this.open = false;
          this.$emit("confirm");
        }
      });
    }
  }
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