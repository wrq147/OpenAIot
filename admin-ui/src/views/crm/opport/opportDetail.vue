<template>
  <div style="padding:20px 20px 0 20px;height:100%" class="manualpage">
    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="950px"
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
            <el-form-item label="目标仓库" prop="HouseName">
              <div style="display:flex;align-items: center;">
                <el-input class="houseipt" v-model="form.HouseName" readonly placeholder="请选择目标仓库"
                  @focus="openHouseDialog">
                  <i slot="suffix" @click="onClear" v-if="form.HouseId != null" class="el-icon-circle-close"
                    style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
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
            <div style="font-size:16px;color:#333;">入库物品</div>
            <div v-if="enterMethod == 3">
              <el-row :gutter="15" type="flex" justify="end">

                <el-col :span="1.5" v-hasPermi="['/IoTService/']">
                  <el-button type="primary" size="mini" plain @click="openDeviceDialog">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">选择产品</span>
                  </el-button>
                </el-col>

                <el-col :span="1.5">
                  <el-button type="primary" size="mini" plain>
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">扫码添加</span>
                  </el-button>
                </el-col>
              </el-row>
            </div>
          </div>
          <el-table :data="form.List" stripe style="width: 100%">
            <el-table-column prop="TargetNumber" align="center" label="批次编号" width="180">
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
                <span v-if="scope.row.TargetType == 1">{{ scope.row.Quantity }}</span>
                <el-input-number v-else v-model="scope.row.Quantity" :min="1" :max="9999" size="mini"></el-input-number>
              </template>
            </el-table-column>
            <el-table-column align="center" label="单价（元）" width="140">
              <template slot-scope="scope">
                <el-input-number v-model="scope.row.Price" :precision="2" size="mini" :min="0"></el-input-number>
              </template>
            </el-table-column>
            <el-table-column prop="address" align="center" label="操作" width="110" v-if="enterMethod == 3">
              <template slot-scope="scope">
                <el-link type="danger" icon="el-icon-delete" @click="form.List.splice(scope.$index, 1)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>

        </div>

        <div v-show="showFlow" style="margin-top:10px;">
          <div class="flow-title" style="font-size:16px;color:#333;">入库审核</div>
          <div>
            <AddEmbed ref="flowForm"></AddEmbed>
          </div>
        </div>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button plain type="primary" @click="submitForm(2)">提 交</el-button>
        <el-button plain @click="submitForm(1)">保 存</el-button>
        <el-button plain @click="open = false">取 消</el-button>
      </div>


      <el-dialog width="620px" title="请选择目标仓库" :visible.sync="houseOpen" :close-on-click-modal="false" append-to-body>
        <el-form :model="houseQuery" ref="queryForm" :inline="true"
          style="display: flex;justify-content: space-between;">
          <el-form-item label="仓库名称" prop="Name">
            <el-input v-model="houseQuery.Name" placeholder="请输入仓库名称" clearable></el-input>
          </el-form-item>
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadHouseList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table :data="houseOptions" tooltip-effect="dark" style="width: 100%" highlight-current-row
          v-loading="houseLoading" @current-change="handleCurrentChange">
          <el-table-column prop="StoreName" align="left" label="仓库名称">
          </el-table-column>
          <el-table-column label="仓库类型" align="center" width="120">
            <template slot-scope="scope">
              <el-tag type="success" v-if="scope.row.IsSystem == 1" effect="plain">系统</el-tag>
              <el-tag type="info" v-else effect="plain">一般</el-tag>
            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="houseTotal > 0" :total="houseTotal" :page.sync="houseQuery.pageNum"
          :limit.sync="houseQuery.pageSize" @pagination="loadHouseList" />
      </el-dialog>


      <el-dialog top="2vh" width="960px" title="请选择需要入库的产品批次" :visible.sync="deviceOpen" append-to-body>
        <el-form :model="deviceQuery" ref="deviceForm" :inline="true"
          style="display: flex;justify-content: space-between;">
          <div>
            <el-form-item label="查询关键字" prop="Key">
              <el-input v-model="deviceQuery.Key" placeholder="搜索产品名称或批次编号" clearable></el-input>
            </el-form-item>
            <el-form-item label="创建时间">
              <el-date-picker class="form_input_style" v-model="devDateRange" style="width:232px"
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
          @selection-change="onDeviceChange">
          <el-table-column type="selection" width="55">
          </el-table-column>
          <el-table-column prop="Number" label="批次编号" align="center" width="150">
          </el-table-column>
          <el-table-column label="预览图片" align="center" width="150">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]">
                  <div slot="error" class="image-slot">
                    <i class="el-icon-picture-outline"></i>
                  </div>
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


    </el-dialog>
  </div>
</template>

<script>
import {
  houseList
} from "@/api/storage/house";

import {
  enterDevList,
  manualPile,
  generateRKNumber,
  getEnterInfo
} from "@/api/storage/stock";

import {
  autoCompany,
  kuaiDiCompanys
} from '@/api/code.js'

import { getQuery } from "@/api/flowable/process";
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
export default {
  name: "ManualPile",
  components: { AddEmbed },
  data() {
    return {
      title: '',
      kdcompanys: [],
      allloading: false,
      enterId: undefined,
      houseOpen: false,
      deviceOpen: false,
      partsOpen: false,
      houseLoading: false,
      deviceLoading: false,
      open: false,
      houseOptions: [],
      houseTotal: 0,
      houseQuery: { 'Name': undefined, 'Status': '1', 'PageNum': 1, 'PageSize': 20 },
      form: {
        StockNumber: "",
        HouseName: "",
        HouseId: undefined,
        InDate: undefined,
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
        ]
      },
      // 日期范围
      devDateRange: [],
      deviceQuery: {
        pageNum: 1,
        pageSize: 20,
        Key: ""
      },
      deviceTotal: 0,
      deviceList: [],
      showFlow: false,
      enterTemplateId: 0,
      enterMethod: 0,
    };
  },
  computed: {
    FlowParams: function () {
      return { "@from": this.form.StockNumber, "@fromtype": "入库单" };
    }
  },
  methods: {
    async expressSelected(val) {
      if (val.length < 8) return;
      this.form.ExpressCompany = (await autoCompany(val)).data;
    },
    async openDialog(id) {
      this.allloading = true;
      this.open = true;

      this.enterId = id;
      let flowId = undefined;
      if (this.enterId == null) {
        this.enterMethod = 3;
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
      }
      else {
        this.title = "编辑入库单";
        let enterInfo = await getEnterInfo(this.enterId);
        this.enterMethod = enterInfo.data.EnterMethod;
        this.form.StockNumber = enterInfo.data.StockNumber;
        this.form.HouseName = enterInfo.data.ToHouseName;
        this.enterTemplateId = enterInfo.data.ToHouseEnterTemplateId;
        this.form.InDate = enterInfo.data.InDate;
        this.form.HouseId = enterInfo.data.ToHouseId;
        this.form.Remark = enterInfo.data.Remark;
        this.form.ExpressNumber = enterInfo.data.ExpressNumber;
        this.form.ExpressCompany = enterInfo.data.ExpressCompany;
        this.form.ExpressPhone = enterInfo.data.ExpressPhone;
        this.form.List = enterInfo.data.List;
        let sdddx = await getQuery({ Name: "@from", Value: this.form.StockNumber });
        if (sdddx.data.length > 0) {
          flowId = sdddx.data[0].FlowId;
        }
      }

      if (this.enterTemplateId > 0) {
        await this.$refs.flowForm.InitData(this.enterTemplateId, this.FlowParams, flowId);
        this.showFlow = this.$refs.flowForm.getFormLength() > 0;
      }
      else {
        this.showFlow = false;
      }

      let tmpkdrsp = await kuaiDiCompanys();
      this.kdcompanys = tmpkdrsp.data;

      this.allloading = false;
    },
    openHouseDialog() {
      this.houseOpen = true;
      this.loadHouseList();
    },
    openDeviceDialog() {
      this.deviceOpen = true;
      this.loadDeviceList();
    },
    onClear() {
      this.form.HouseName = "";
      this.form.HouseId = null;
    },
    resetQuery() {
      this.resetForm("queryForm");
      this.loadHouseList();
    },
    resetDevice() {
      this.devDateRange = [];
      this.resetForm("deviceForm");
      this.loadDeviceList();
    },
    async handleCurrentChange(val) {
      if (val != null) {
        this.form.HouseName = val.StoreName;
        this.form.HouseId = val.Id;
        this.houseOpen = false;
        this.enterTemplateId = val.EnterTemplateId;
        if (this.enterTemplateId > 0) {
          await this.$refs.flowForm.InitData(this.enterTemplateId, this.FlowParams, flowId);
          this.showFlow = this.$refs.flowForm.getFormLength() > 0;
        }
        else {
          this.showFlow = false;
        }
      }

    },
    loadHouseList() {
      this.houseLoading = true;
      houseList(this.houseQuery).then(rsp => {
        this.houseOptions = rsp.data.List;
        this.houseTotal = rsp.data.Total;
        this.houseLoading = false;
      })
    },
    loadDeviceList() {
      this.deviceLoading = true;
      enterDevList(this.addDateRange(this.deviceQuery, this.devDateRange)).then(rsp => {
        this.deviceList = rsp.data.List;
        this.deviceTotal = rsp.data.Total;

        this.$nextTick(() => {
          //设置选中状态
          this.deviceList.forEach(element => {
            if (this.form.List.some(item => {
              return item.TargetType == 1 && item.TargetId == element.Id;
            })) {
              this.$refs.devTable.toggleRowSelection(element);
            }
          });
          this.deviceLoading = false;
        })


      })
    },

    onDeviceChange(val) {
      if (this.deviceLoading) return;
      this.form.List = this.form.List.filter(x => x.TargetType == 0);
      val.forEach(element => {
        this.form.List.push({
          TargetType: element.ProductLabel == 'F' ? 1 : 0,
          TargetId: element.Id,
          TargetNumber: element.Number,
          Quantity: 1,
          Price: element.Price,
          TargetName: element.BatchName
        });
      })

    },

    submitForm(st) {
      this.$refs["form"].validate(valid => {
        if (valid) {
          if (this.form.List.length == 0) {
            this.$message.error("请选择要入库的物品");
            return;
          }
          this.form.Id = this.enterId;
          this.form.IsSubmit = st == 2;
          manualPile(this.form).then(response => {
            if (this.enterTemplateId > 0 || response.data == 1) {
              //提交审批表单
              this.$refs.flowForm.submitForm(st).then(rss => {
                this.$modal.msgSuccess("操作成功");
                this.open = false;
                this.$emit("confirm");
              })
            }
            else {
              this.$modal.msgSuccess("操作成功");
              this.open = false;
              this.$emit("confirm");
            }

          });

        }
      });
    }
  }
};
</script>
<style lang="scss">
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