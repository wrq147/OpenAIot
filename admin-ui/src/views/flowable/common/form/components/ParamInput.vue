<template>
  <div>
    <div v-if="mode === 'DESIGN'">
      <el-button type="info" icon="el-icon-link" round disabled>关联的{{ UsingFormType }}</el-button>
    </div>
    <div v-else v-loading="formloading">
      <template v-if="form == null">
        <div style="text-align: center;">暂无关联表单</div>
      </template>
      <template v-else>
        <div v-if="UsingFormType == '入库单'">
          <el-descriptions title="入库单信息" :column="3" border>
            <el-descriptions-item label="入库单号">
              {{ form.StockNumber }}
            </el-descriptions-item>
            <el-descriptions-item label="入库方式">
              {{ EnterMethodName(form.EnterMethod) }}
            </el-descriptions-item>
            <el-descriptions-item label="状态">
              <el-tag v-if="form.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-else-if="form.Status == 1" type="warning">待审批</el-tag>
              <el-tag v-else-if="form.Status == 2" type="success">入库成功</el-tag>
              <el-tag v-else-if="form.Status == 3" type="danger">待退货</el-tag>
              <el-tag v-else-if="form.Status == 4" type="info">已退货</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="所出仓库">
              <template v-if="form.FromHouseName != ''">
                <el-tag>{{ form.FromName }}</el-tag><span style="margin-left:10px;">{{
                  form.FromHouseName }}</span>
              </template>
              <template v-else>
                无
              </template>
            </el-descriptions-item>
            <el-descriptions-item label="所入仓库">
              {{ form.ToHouseName }}
            </el-descriptions-item>
            <el-descriptions-item label="入库时间">
              {{ parseTime(form.InDate) }}
            </el-descriptions-item>
            <el-descriptions-item label="物流信息" span="3">
              <template v-if="this.form.ExpressNumber != ''">
                <a type="primary" :href="'https://www.kuaidi100.com/?nu=' + this.form.ExpressNumber" target="_blank">
                  <el-tag>{{ this.form.ExpressCompanyName }}</el-tag>
                  <span style="margin-left:15px;">{{ this.form.ExpressNumber }}</span>
                  <span v-if="this.form.ExpressPhone != ''">（{{ this.form.ExpressPhone }}）</span>
                  <span style="margin-left:25px;color:#409EFF;"><i class="el-icon-view"
                      style="margin-right:5px;"></i>点击查询快递详情</span>
                </a>
              </template>
            </el-descriptions-item>

            <el-descriptions-item label="备注" span="3">
              {{ form.Remark }}
            </el-descriptions-item>
          </el-descriptions>

          <div class="wp-title">入库物品</div>
          <el-table :data="form.List" style="width: 100%"
            :header-cell-style="{ 'color': '#78829D', 'background': '#F9FAFC' }" border stripe>
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
            </el-table-column>
            <el-table-column prop="TargetName" label="物品名称">
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
            <el-table-column prop="Quantity" align="center" label="数量" width="140">
            </el-table-column>
            <el-table-column prop="Price" align="center" label="单价（元）" width="140">
            </el-table-column>
          </el-table>
        </div>
        <div v-if="UsingFormType == '出库单'">
          <el-descriptions title="出库单信息" :column="3" border>
            <el-descriptions-item label="出库单号">
              {{ form.StockNumber }}
            </el-descriptions-item>
            <el-descriptions-item label="出库方式">
              {{ LeaveMethodName(form.LeaveMethod) }}
            </el-descriptions-item>
            <el-descriptions-item label="状态">
              <el-tag v-if="form.Status == 0" type="warning">待提交</el-tag>
              <el-tag v-else-if="form.Status == 1" type="warning">待审批</el-tag>
              <el-tag v-else-if="form.Status == 2" type="success">出库成功</el-tag>
              <el-tag v-else-if="form.Status == 3" type="danger">出库失败</el-tag>
              <el-tag v-else-if="form.Status == 4" type="info">已取消</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="所出仓库">
              {{ form.FromHouseName }}
            </el-descriptions-item>
            <el-descriptions-item label="所入仓库">
              <template v-if="form.ToHouseName != ''">
                <el-tag>{{ form.ToName }}</el-tag>
                <span style="margin-left:10px;">{{ form.ToHouseName }}</span>
              </template>
              <template v-else>
                无
              </template>
            </el-descriptions-item>

            <el-descriptions-item label="出库时间">
              {{ parseTime(form.OutDate) }}
            </el-descriptions-item>
            <el-descriptions-item label="物流信息" span="3">
              <template v-if="this.form.ExpressNumber != ''">
                <a type="primary" :href="'https://www.kuaidi100.com/?nu=' + this.form.ExpressNumber" target="_blank">
                  <el-tag>{{ this.form.ExpressCompanyName }}</el-tag>
                  <span style="margin-left:15px;">{{ this.form.ExpressNumber }}</span>
                  <span v-if="this.form.ExpressPhone != ''">（{{ this.form.ExpressPhone }}）</span>
                  <span style="margin-left:25px;color:#409EFF;"><i class="el-icon-view"
                      style="margin-right:5px;"></i>点击查询快递详情</span>
                </a>
              </template>
            </el-descriptions-item>

            <el-descriptions-item label="备注" span="3">
              {{ form.Remark }}
            </el-descriptions-item>
          </el-descriptions>

          <div class="wp-title">出库物品</div>
          <el-table :data="form.List" style="width: 100%" border stripe
            :header-cell-style="{ 'color': '#78829D', 'background': '#F9FAFC !important' }">
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
            </el-table-column>
            <el-table-column prop="TargetName" label="物品名称">
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
            <el-table-column prop="Quantity" align="center" label="数量" width="140">
            </el-table-column>
            <el-table-column prop="Price" align="center" label="单价（元）" width="140">
            </el-table-column>
          </el-table>
        </div>
        <div v-if="UsingFormType == '告警工单'">
          <el-descriptions title="告警工单" :column="3" border>
            <el-descriptions-item label="告警工单号">
              {{ form.WarnNumber }}
            </el-descriptions-item>
            <el-descriptions-item label="工单状态">
              <el-tag v-if="form.Status == 0">待处理</el-tag>
              <el-tag type="success" v-if="form.Status == 1">已处理</el-tag>
              <el-tag v-if="form.Status == 2">待派工</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="报警级别">
              <el-tag type="info" v-if="form.Level == 0">普通</el-tag>
              <el-tag type="warning" v-else-if="form.Level == 1">告警</el-tag>
              <el-tag type="danger" v-else-if="form.Level == 2">紧急</el-tag>
            </el-descriptions-item>
            <el-descriptions-item :column="2" label="告警名称">
              {{ form.Name }}
            </el-descriptions-item>
            <el-descriptions-item label="告警设备">
              {{ form.DeviceName }}
            </el-descriptions-item>
            <el-descriptions-item :column="2" label="告警描述">
              {{ form.Description }}
            </el-descriptions-item>
          </el-descriptions>
        </div>
        <div v-if="UsingFormType == '出库申请单'">
          <el-descriptions title="申请单信息" :column="3" border>
            <el-descriptions-item label="申请单号">
              {{ form.ApplyNumber }}
            </el-descriptions-item>
            <el-descriptions-item label="申请状态">
              {{ LeaveApplyStatus(form.Status) }}
            </el-descriptions-item>
            <el-descriptions-item label="领取仓库">
              {{form.House.StoreName}}
            </el-descriptions-item>
            <el-descriptions-item label="申请人">
              {{form.ApplyUserInfo.RealName}}
            </el-descriptions-item>
            <el-descriptions-item label="申请时间">
              {{ parseTime(form.ApplyOn) }}
            </el-descriptions-item>
            <el-descriptions-item label="申请原因" span="3">
              {{ form.Reason }}
            </el-descriptions-item>
          </el-descriptions>
          <div class="wp-title">领取物品</div>
          <el-table :data="form.List" style="width: 100%" border stripe
            :header-cell-style="{ 'color': '#78829D', 'background': '#F9FAFC !important' }">
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180">
            </el-table-column>
            <el-table-column prop="TargetName" label="物品名称">
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
            <el-table-column prop="Quantity" align="center" label="数量" width="140">
            </el-table-column>
          </el-table>
        </div>
      </template>

    </div>
  </div>
</template>

<script>
import componentMinxins from "../ComponentMinxins";
import {
  getEnterInfoByNumber, getLeaveInfoByNumber
} from "@/api/storage/stock";
import {ApplyInfoByNumber} from "@/api/storage/apply";
import { getWarnInfoByNumber } from "@/api/rules/productModel"
export default {
  mixins: [componentMinxins],
  name: "ParamInput",
  components: {},
  props: {
    value: {
      type: String,
      default: null,
    },
    formType: {
      type: String,
      default: "",
    },
    formId: {
      type: String,
      default: "",
    },
    disabled: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      targetId: undefined,
      form: null,
      formloading: false,
      UsingFormType: ""
    };
  },
  async mounted() {
    if (this.mode != 'DESIGN') {
      if (this.formType == "") {
        this.UsingFormType= this.valueModel["@fromtype"];
        this.targetId = this.valueModel["@from"];
      }
      else {
        this.UsingFormType = this.formType;
        if (this.formId == "@from") {
          this.targetId = this.valueModel["@from"];
        }
        else {
          this.targetId = this.valueModel[this.formId];
        }
      }


      if (this.targetId == null) {
        return;
      }
      this.formloading = true;
      if (this.UsingFormType == '入库单') {
        try {
          let enterInfo = await getEnterInfoByNumber(this.targetId);
          this.form = enterInfo.data;
          this.form.ExpressCompanyName = await this.$store.dispatch("datas/kuaiName", this.form.ExpressCompany);
        }
        catch (ex) { }
      }
      else if (this.UsingFormType == '出库单') {
        try {
          let leaveInfo = await getLeaveInfoByNumber(this.targetId);
          this.form = leaveInfo.data;
          this.form.ExpressCompanyName = await this.$store.dispatch("datas/kuaiName", this.form.ExpressCompany);
        }
        catch (ex) { }
      }
      else if (this.UsingFormType == '告警工单') {
        try {
          let warnInfo = await getWarnInfoByNumber(this.targetId);
          this.form = warnInfo.data;
        }
        catch (ex) { }
      }
      else if (this.UsingFormType == '出库申请单') {
        try {
          let applyInfo = await ApplyInfoByNumber(this.targetId);
          this.form = applyInfo.data;
        }
        catch (ex) { }
      }
      
      this.formloading = false;
    }

  },
  methods: {
    EnterMethodName(way) {
      switch (way) {
        case 0:
          return "出库";
        case 1:
          return "退货";
        case 2:
          return "调拨";
        case 3:
          return "手动";
      }
      return "";
    },
    LeaveMethodName(way) {
      switch (way) {
        case 0:
          return "出库";
        case 1:
          return "退货";
        case 2:
          return "调拨";
        case 3:
          return "领用";
      }
      return "";
    },
    LeaveApplyStatus(status){
      switch (status) {
        case 0:
          return "待提交";
        case 1:
          return "待审批";
        case 2:
          return "申请成功";
        case 3:
          return "申请失败";
        case 4:
          return "已取消";
      }
      return "";
    }
  },
};
</script>

<style lang="scss" scoped>
.wp-title {
  margin-top: 20px;
  font-size: 16px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 20px;
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
</style>
