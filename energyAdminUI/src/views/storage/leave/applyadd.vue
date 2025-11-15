<template>
  <div style="padding: 20px 20px 0 20px; height: 100%" class="manualpage">
    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" v-loading="allloading" width="1050px" top="2vh">
      <el-form ref="form" :model="form" :rules="rules" label-width="120px">
        <div class="base-title">基本信息</div>
        <el-row>
          <el-col :span="12">
            <el-form-item label="申请单号" prop="ApplyNumber">
              <el-input v-model="form.ApplyNumber" :readonly="true"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="申请时间" prop="ApplyOn">
              <el-date-picker v-model="form.ApplyOn" type="datetime" placeholder="选择日期时间"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="出库仓库" prop="HouseId">
              <div style="display: flex; align-items: center">
                <el-input class="houseipt" v-model="form.HouseName" readonly placeholder="请选择出库仓库" @focus="openHouseDialog">
                  <i slot="suffix" @click="onClear" v-if="form.HouseId != null"
                    class="el-icon-circle-close"
                    style="font-size: 22px;cursor: pointer;vertical-align: middle;"></i>
                </el-input>
              </div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="申请类型" prop="ApplyType">
              <div style="display: flex; align-items: center">
                <el-select v-model="form.ApplyType" placeholder="请选择">
                  <el-option
                    v-for="item in applytypelist"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
                  </el-option>
                </el-select>
              </div>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="申请人" prop="ApplyUserId">
              <el-select filterable allow-create default-first-option v-model="form.ApplyUserName" ref="selectApplyUser"
                placeholder="请选择申请人" @focus="getApplyUserFocus" style="width:100%"></el-select>
              <org-picker :multiple="false" ref="applyUserPicker" @ok="selectApplyUser" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="申请原因" prop="Reason">
              <el-input type="textarea" :rows="2" placeholder="请输入申请原因" v-model="form.Reason" style="width: 80%"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <div>
          <div class="items-title">
            <div style="font-size: 16px; color: #333">
              工单明细
              <span v-if="finishedTotal>0" class="num_li">成品：{{finishedTotal}}</span>
              <span v-if="useTotal>0" class="num_li">半成品：{{useTotal}}</span>
            </div>
            <div>
              <el-row :gutter="15" type="flex" justify="end">

                <el-col :span="1.5">
                  <el-button type="primary" size="mini" plain @click="openWupinDialog">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">选择物品</span>
                  </el-button>
                </el-col>
              </el-row>
            </div>
          </div>
          <el-table :data="form.List" stripe style="width: 100%">
            <el-table-column prop="TargetNumber" align="center" label="物品编号" width="180"></el-table-column>
            <el-table-column prop="TargetName" label="物品名称"></el-table-column>
            <el-table-column label="存储类型" align="center" width="100">
                <template slot-scope="scope">
                    <span v-if="scope.row.TargetType==1">成品</span>
                    <span v-else-if="scope.row.TargetType==0">半成品</span>
                </template>
            </el-table-column>
            <el-table-column align="center" label="数量" width="140">
              <template slot-scope="scope">
                <span v-if="scope.row.TargetType == 1">{{ scope.row.Quantity }}</span>
                <el-input-number v-else v-model="scope.row.Quantity" :min="1" :max="9999" size="mini"></el-input-number>
              </template>
            </el-table-column>
            <el-table-column prop="address" align="center" label="操作" width="110" >
              <template slot-scope="scope">
                <el-link type="danger" icon="el-icon-delete" @click="form.List.splice(scope.$index, 1)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <div style="margin-top: 10px" v-show="leaveApplyTemplateId > 0">
          <AddEmbed ref="flowForm">
            <div class="flow-title" style="font-size: 16px; color: #333">
              审批信息
            </div>
          </AddEmbed>
        </div>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button plain type="primary" @click="submitForm(2)">提 交</el-button>
        <el-button plain @click="submitForm(1)">保 存</el-button>
        <el-button plain @click="open = false">取 消</el-button>
      </div>

      <el-dialog width="960px" title="请选择导入的物品" :visible.sync="wupinOpen" :close-on-click-modal="false" append-to-body>
        <el-form :model="wupinQuery" :inline="true" ref="wupinForm" style="display: flex; justify-content: space-between">
          <div>
            <el-form-item label="搜索关键字" prop="Key">
              <el-input v-model="wupinQuery.Key" placeholder="搜索物品名称或物品编号" clearable></el-input>
            </el-form-item>
            <el-form-item label="创建日期">
              <el-date-picker class="form_input_style" v-model="wupinDateRange"
                style="width: 232px" value-format="yyyy-MM-dd" type="daterange"
                range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
          </div>
          <el-form-item>
            <el-button icon="el-icon-refresh" @click="resetWupin">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="loadWupinList">搜索</el-button>
          </el-form-item>
        </el-form>
        <el-table ref="wupinTable" :data="wupinList" tooltip-effect="dark" v-loading="wupinLoading" style="width: 100%"
          @selection-change="onWupinChange" @row-click="clickWupinRow" @select="wupinBoxSelect" row-key="TargetId">
          <el-table-column type="selection" width="55"></el-table-column>
          <el-table-column prop="DeviceNumber" label="物品编号" align="center" width="150"></el-table-column>
          <el-table-column label="预览图片" align="center" width="150">
            <template slot-scope="scope">
              <div class="imgwrap">
                <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]">
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
                {{ scope.row.TargetType == 1 ? "成品" : "半成品" }}
              </div>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="wupinTotal > 0" :total="wupinTotal" :page.sync="wupinQuery.pageNum"
          :limit.sync="wupinQuery.pageSize" @pagination="loadWupinList" :pageSizes="[2, 4, 6, 8, 10]"/>

        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="onWupinConfirm">确 定</el-button>
          <el-button @click="wupinOpen = false">取 消</el-button>
        </div>
      </el-dialog>

      <HouseSelecter ref="houseDlg" @ok="handleCurrentChange"></HouseSelecter>
    </el-dialog>
  </div>
</template>
    
<script>
import { houseList} from "@/api/storage/house";
import HouseSelecter from "../house/HouseSelecter.vue";
import {
  stockList,
} from "@/api/storage/stock";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import { applyAdd,applyEdit,applySubmitModel,ApplyInfo,generateCKSQNumber,applyFormData } from "@/api/storage/apply";
import AddEmbed from "@/views/flowable/task/record/AddEmbed";
import KfSelecter from "@/components/KFSelecter/kfselecter";
export default {
  name: "applyadd",
  components: { AddEmbed, KfSelecter, HouseSelecter,OrgPicker },
  dicts: ["apply_type"],
  data() {
    return {
      title: "",
      allloading: false,
      applyId: undefined,
      wupinOpen: false,
      wupinLoading: false,
      open: false,
      applytypelist:[],//申请类别列表
      form: {
        ApplyNumber: "",
        HouseId: undefined,
        HouseName: "",
        ApplyType:undefined,
        ApplyOn:undefined,
        Reason:'',
        // ApplyDeptId:undefined,
        ApplyUserId:undefined,
        List: [],
      },
      // 表单校验
      rules: {
        ApplyNumber: [
          { required: true, message: "申请编号不能为空", trigger: "blur" },
        ],
        HouseId: [
          { required: true, message: "出库仓库不能为空", trigger: "change" },
        ],
        ApplyType: [
          { required: true, message: "请选择申请类型", trigger: "change" },
        ],
        ApplyOn: [
          { required: true, message: "申请时间不能为空", trigger: "blur" },
        ],
        ApplyUserId: [
          { required: true, message: "请选择申请人", trigger: "change" },
        ],
      },
      // 日期范围
      wupinDateRange: [],
      wupinQuery: {
        pageNum: 1,
        pageSize: 10,
        HouseId: "",
      },
      wupinTotal: 0,
      wupinList: [],
      wupinSelectArr: [],
      showFlow: false,
      leaveApplyTemplateId: 0,
      ApplyUserInfo:[],//申请人信息
      ApplyDeptInfo:[],//申请部门
    };
  },
  computed: {
    FlowParams: function () {
      return {
        "@from": this.form.ApplyNumber,
        "@fromtype": "出库申请单",
      };
    },
    finishedTotal(){
      let total=0
      if(this.form&&this.form.List){
        this.form.List.map(ro=>{
            if(ro.TargetType==1){
            total=total+Number(ro.Quantity)
            }
        })
      }
      return total
    },
    useTotal(){
      let total=0
      if(this.form&&this.form.List){
        this.form.List.map(ro=>{
            if(ro.TargetType==0){
            total=total+Number(ro.Quantity)
            }
        })
      }
      return total
    }
  },
  methods: {
    //选择负责人
    getApplyUserFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectApplyUser.blur();
      if (this.form.ApplyUserId > 0) {
        this.ApplyUserInfo = [{ id: this.form.ApplyUserId, name: this.form.ApplyUserName, avatar: this.form.Avatar, type: "user" }];
      }
      else {
        this.ApplyUserInfo = [];
      }

      this.$refs.applyUserPicker.show(this.ApplyUserInfo, "user");
    },
    selectApplyUser(values) {
      //选择负责人
      this.ApplyUserInfo = values;
      if (values.length > 0) {
        this.form.ApplyUserId = values[0].id;
        this.form.ApplyUserName = values[0].name;
        this.form.Avatar = values[0].avatar;
      }
      else {
        this.form.ApplyUserId = 0;
        this.form.ApplyUserName = undefined;
        this.form.Avatar = undefined;
      }
      this.$forceUpdate();
    },
    getApplyDeptFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectApplyDept.blur();
      if (this.form.ApplyDeptId > 0) {
        this.ApplyDeptInfo = [{ id: this.form.ApplyDeptId, name: this.form.ApplyDeptName, type: "dept" }];
      }
      else {
        this.ApplyDeptInfo = [];
      }

      this.$refs.applyDeptPicker.show(this.ApplyDeptInfo, "dept");
    },
    selectApplyDept(values) {
      //选择负责人
      this.ApplyDeptInfo = values;
      if (values.length > 0) {
        this.form.ApplyDeptId = values[0].id;
        this.form.ApplyDeptName = values[0].name;
      }
      else {
        this.form.ApplyDeptId = 0;
        this.form.ApplyDeptName = undefined;
      }
      this.$forceUpdate();
    },
    wupinBoxSelect(arr, row) {
      //点击多选框
      console.log(row, "多选框点击事件");
      const selected = this.wupinSelectArr.some(
        (item) => item.TargetId === row.TargetId
      );
      if (selected) {
        this.wupinSelectArr = this.wupinSelectArr.filter((rw) => rw.TargetId !== row.TargetId);
      }
    },
    clickWupinRow(row) {
      console.log("点击单行", row);
      const selected = this.wupinSelectArr.some((item) => item.TargetId === row.TargetId);
      if (!selected) {
        // 选择
        this.$refs.wupinTable.toggleRowSelection(row, true);
      } else {
        // 取消
        this.$refs.wupinTable.toggleRowSelection(row, false);
        this.wupinSelectArr = this.wupinSelectArr.filter((rw) => rw.TargetId !== row.TargetId);
      }
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
    async openDialog(id) {
      this.allloading = true;
      this.open = true;
      this.applytypelist=this.dict.type.apply_type
      this.$nextTick(async () => {
        this.applyId = id;
        if (this.applyId == null) {
          this.title = "新增出库申请单";
          let ssp = await generateCKSQNumber();
          this.form={
            ApplyNumber: ssp.data,
            HouseId: undefined,
            HouseName: "",
            ApplyType:undefined,
            ApplyOn:this.parseTime(Date.now()),
            Reason:'',
            ApplyUserId:parseInt(this.$store.state.user.uid),
            ApplyUserName:this.$store.state.user.name,
            Avatar:this.$store.state.user.avatar,
            List: [],
          }
          let hslist = (await houseList({ IsSystem: true })).data.List;
          // let hsInfo = await houseInfo(fromHsId, false);
          //   this.form.FromHouseName = hsInfo.data.StoreName;
          //   this.form.FromHouseId = hsInfo.data.Id;
          //   this.leaveApplyTemplateId = hsInfo.data.LeaveTemplateId;
            if (hslist.length > 0) {
              this.form.HouseName = hslist[0].StoreName;
              this.form.HouseId = hslist[0].Id;
              this.leaveApplyTemplateId = hslist[0].LeaveApplyTemplateId;
            } else {
              this.leaveApplyTemplateId = 0;
            }
        } else {
          this.title = "编辑出库申请单";
          let leaveInfo = await ApplyInfo(this.applyId);
          this.form={
            Id:this.applyId,
            ApplyNumber: leaveInfo.data.ApplyNumber,
            HouseId: leaveInfo.data.HouseId,
            HouseName: leaveInfo.data.House.StoreName,
            ApplyType:leaveInfo.data.ApplyType,
            ApplyOn:leaveInfo.data.ApplyOn,
            Reason:leaveInfo.data.Reason,
            ApplyUserId:leaveInfo.data.ApplyUserId,
            ApplyUserName:leaveInfo.data.ApplyUserInfo.RealName,
            Avatar:leaveInfo.data.ApplyUserInfo.RealName.Avatar,
            List: leaveInfo.data.List,
          }
          this.leaveApplyTemplateId = leaveInfo.data.House.LeaveApplyTemplateId;
        }

        if (this.leaveApplyTemplateId > 0) {
          let fromInfo=await applyFormData({
            "applyNumber": this.form.ApplyNumber,
            "houseId": this.form.HouseId,
          })
          await this.$refs.flowForm.InitData(
            this.leaveApplyTemplateId,
            this.FlowParams,
            this.applyId == null ? null : this.form.ApplyNumber,
            fromInfo.data
          );
        }

        this.allloading = false;
      });
    },
    openHouseDialog() {
      this.$refs.houseDlg.openHouseDialog("请选择要出库的仓库");
    },
    openWupinDialog() {
      if (this.form.HouseId) {
        this.wupinOpen = true;
        this.wupinSelectArr=[]
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
      this.wupinSelectArr=[]
      this.loadWupinList();
    },
    async handleCurrentChange(val) {
      this.form.HouseName = val.StoreName;
      this.form.HouseId = val.Id;
      this.leaveApplyTemplateId = val.LeaveApplyTemplateId;
      if (this.leaveApplyTemplateId > 0) {
        let fromInfo=await applyFormData({
          "applyNumber": this.form.ApplyNumber,
          "houseId": this.form.HouseId,
        })
        await this.$refs.flowForm.InitData(
          this.leaveApplyTemplateId,
          this.FlowParams,
          this.applyId == null ? null : this.form.StockNumber,
          fromInfo.data
        );
      }
    },
    loadWupinList() {
      this.wupinLoading = true;
      // this.wupinSelectArr = [];
      this.wupinQuery.HouseId = this.form.FromHouseId;
      stockList(this.addDateRange(this.wupinQuery, this.wupinDateRange)).then(
        (rsp) => {
          this.wupinList = rsp.data.List;
          // console.log("物品列表", this.wupinList);
          this.wupinTotal = rsp.data.Total;
          this.$nextTick(() => {
            //设置选中状态
            this.wupinList.forEach((element) => {
              if (
                this.wupinSelectArr.some((item) => {
                  return item.TargetId == element.TargetId;
                })
              ) {
                this.$refs.wupinTable.toggleRowSelection(element);
              }
            });
            this.wupinLoading = false;
          });
        }
      );
    },

    onWupinChange(val) {
      // this.wupinSelectArr = val;
      console.log("选项变了",val,this.wupinSelectArr);
      let IdList = this.wupinSelectArr.map((item) => {
        return item.TargetId;
      });
      console.log(IdList,'IdListIdListIdList');
      val.forEach((element) => {
        if (IdList.includes(element.TargetId)) {
        } else {
          this.wupinSelectArr.push(element);
        }
      });
    },
    onWupinConfirm() {
      if (this.wupinSelectArr.length > 0) {
        let filteritems = this.form.List.filter((x) =>
          this.wupinSelectArr.some((w) => w.TargetId == x.TargetId)
        );
        if (filteritems.length > 0) {
          this.$message.error(
            "不可以重复添加物品 '" +
              filteritems.map((x) => x.TargetName).join() +
              "'"
          );
          return;
        }
        this.wupinSelectArr.forEach((element) => {
          this.form.List.push({
            TargetType: element.TargetType,
            TargetId: element.TargetId,
            TargetNumber: element.DeviceNumber,
            Quantity: element.Quantity,
            TargetName: element.Name,
          });
        });
      }
      this.wupinOpen = false;
    },
    submitForm(st) {
      this.$refs["form"].validate(async (valid) => {
        if (valid) {
          if (this.form.List.length == 0) {
            this.$message.error("请选择要出库的物品");
            return;
          }
          this.form.FlowId=this.leaveApplyTemplateId
          this.form.Id = this.applyId;
          let response;
          if (this.applyId == null) {
            response = await applyAdd(this.form);
          } else {
            response = await applyEdit(this.form);
          }
          
          if (this.leaveApplyTemplateId > 0) {
            if (st == 2) {
              let tmpmodel = this.$refs.flowForm.getModel();
              if(this.applyId){
                tmpmodel["id"] = this.applyId;
              }else{
                tmpmodel["id"] = response.data;
              }
              
              await applySubmitModel(tmpmodel);
            }
            else {
              await this.$refs.flowForm.submitForm(st);
            }
          }
          else{
            await applySubmitModel({ id: response.data });
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
  & > .el-dialog__wrapper {
    & > .el-dialog {
      & > .el-dialog__footer {
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