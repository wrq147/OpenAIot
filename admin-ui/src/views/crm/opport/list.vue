<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>

      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建日期">
                <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px"
                  value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期"
                  end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <!-- <el-col class="float_right" :span="24"> -->
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
              <!-- </el-col> -->
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Clue/Add']">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getOpportList"
                :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="priList" :row-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%;position:relative;">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="商机名称" align="center" key="OpportName" prop="OpportName"
                v-if="columns[0].visible" />
              <el-table-column label="客户名称" align="center" key="CustomerName" prop="CustomerName"
                v-if="columns[1].visible"></el-table-column>
              <el-table-column label="联系人" align="center" key="ContactId" prop="ContactId"
                v-if="columns[2].visible"></el-table-column>
              <el-table-column label="成交几率" align="center" key="Probability" prop="Probability"
                v-if="columns[3].visible">
                <template slot-scope="scope" v-if="scope.row.Probability">
                  <span>{{ scope.row.Probability + '%' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="负责人" align="center" key="LeaderId" prop="LeaderName" v-if="columns[5].visible">
                <template slot-scope="scope" v-if="scope.row.LeaderId">
                  <span>{{ scope.row.LeaderUser.RealName }}</span>
                </template>
              </el-table-column>
              <el-table-column label="协作人" align="center" v-if="columns[6].visible">
                <template slot-scope="scope" v-if="scope.row.Helper">
                  <span>{{ returnRowHelperName(scope.row) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="销售阶段" align="center" key="Period" prop="Period" v-if="columns[7].visible">
                <template slot-scope="scope" v-if="scope.row.Period">
                  <span>{{ periodTypeMap.get(scope.row.Period) ? periodTypeMap.get(scope.row.Period).PeriodName : ''
                  }}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[8].visible" width="240">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="258">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)"
                    v-hasPermi="['/CRMService/Opportunity/Edit']">修改</el-button>
                  <el-button @click="handleOpenDetails(scope.row)" type="text" icon="">详情</el-button>
                  <el-button @click="handDelete(scope.row)" type="text" icon="">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getOpportList" />
          </div>
        </el-col>
      </el-row>


      <el-dialog top="2vh" :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="800px" append-to-body
        class="add_dialog_border opport_add" @close="cancel">
        <el-form class="add_clue_form" ref="opportForm" :model="opportForm" :rules="rules" label-width="96px" label-position="left">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="商机名称" required prop="opportName">
                <el-input class="form_input_style" v-model="opportForm.opportName" placeholder="请输入商机名称" clearable
                  style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="商机编号" required prop="opportNumber">
                <el-input class="form_input_style" v-model="opportForm.opportNumber" placeholder="请输入商机编号" clearable
                  style="width:100%" :disabled="true" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户名称" required prop="customerId">
                <el-input class="form_input_style" v-model="opportForm.customerName" placeholder="请选择客户" clearable
                  style="width:100%" @focus="openCustomDialog" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="客户联系人" required prop="contactId">
                <el-input class="form_input_style" v-model="opportForm.contactName" placeholder="请选择联系人" clearable
                  style="width:100%" @focus="openContactDialog" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="销售阶段" required prop="period">
                <el-select class="form_input_style" v-model="opportForm.period" placeholder="请选择客户类型" style="width:100%"
                  @change="periodChange">
                  <el-option :key="item.Id" :label="item.PeriodName" :value="item.Id"
                    v-for="item in periodSelectlis"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="赢率" required prop="probability">
                <el-input class="form_input_style" v-model="opportForm.probability" placeholder="请输入预计成交几率" clearable
                  style="width:100%" type="number" :disabled="true" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="负责人" prop="leaderId">
                <el-select class="form_input_style" filterable allow-create default-first-option
                  v-model="opportForm.leaderName" ref="selectLeader" placeholder="请选择负责人" @focus="getLeaderFocus"
                  style="width:100%"></el-select>
                <org-picker2 :multiple="false" ref="leaderPicker" :selected="opportForm.leaderInfo"
                  @ok="selectLeadered" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="协作人" prop="helperName">
                <el-select class="form_input_style" multiple v-model="opportForm.helperName" ref="selectUsers"
                  placeholder="请选择协作人" @focus="getUsersFocus" style="width:100%"></el-select>
                <org-picker :multiple="true" ref="userPicker" :selected="opportForm.userInfo" @ok="selectUsersed" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="商机详情" prop="remark">
                <el-input type="textarea" v-model="opportForm.remark" placeholder="请输入商机详情"
                  :autosize="{ minRows: 2, maxRows: 4 }"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <div>
            <div class="items-title">
              <div style="font-size:16px;color:#333;">商机明细</div>
              <div>
                <el-row :gutter="15" type="flex" justify="end">
                  <el-col :span="1.5">
                    <el-button type="primary" size="mini" plain @click="openWupinDialog">
                      <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                      <span style="margin-left:6px">选择产品</span>
                    </el-button>
                  </el-col>
                </el-row>
              </div>
            </div>
            <el-table :data="opportForm.detailList" stripe style="width: 100%" v-loading="choiceLoading">
              <el-table-column prop="number" align="center" label="产品编号" width="180"></el-table-column>
              <el-table-column prop="name" label="产品名称"></el-table-column>
              <el-table-column prop="label" align="center" label="标签" width="220"></el-table-column>
              <el-table-column prop="quantity" align="center" label="数量" width="140">
                <template slot-scope="scope">
                  <span v-if="scope.row.label == '成品'">{{
                    scope.row.quantity
                  }}</span>
                  <el-input-number v-else v-model="scope.row.quantity" :min="1" :max="9999" size="mini"></el-input-number>
                </template>
              </el-table-column>
              <el-table-column prop="price" align="center" label="价格" width="140">
                <template slot-scope="scope">
                  <el-input-number v-model="scope.row.price" :precision="2" size="mini" :min="0"></el-input-number>
                </template>
              </el-table-column>
              <el-table-column align="center" label="操作" width="110">
                <template slot-scope="scope">
                  <el-link type="danger" icon="el-icon-delete" @click="deleteProductLis(scope.$index)">删除</el-link>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
      <wupin-choice ref="wupinChoice" @onWupinConfirm="onWupinConfirm"
        :detailList="opportForm.detailList2"></wupin-choice>
      <custom-choice ref="customOptions" @handleCustomChange="handleCustomChange"></custom-choice>
      <contact-choice ref="contactOptions" @handleContactChange="handleContactChange"></contact-choice>
    </div>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  opportList,
  opportAdd,
  opportEdit,
  GenerateNumber,
  opportInfo,
  periodList,
  AddComment,
  opportDel
} from "@/api/crm/opport";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import OrgPicker2 from "@/views/flowable/common/OrgPicker";
import WupinChoice from "../compontent/wupin-choice.vue"
import CustomChoice from "@/views/crm/compontent/custom-choice.vue"
import ContactChoice from "@/views/crm/compontent/contact-choice.vue"
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
export default {
  name: "cluePrilist",
  components: { OrgPicker, OrgPicker2, WupinChoice, CustomChoice, ContactChoice, TextEditor },
  mixins: [resizeTableCon],
  dicts: ["follow_way"],
  data() {
    return {
      diaTitle: "添加商机", //弹出层的标题
      open: false, //弹出层
      //表格样式设计
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      //表格样式设计
      // 日期范围
      dateRange: [],
      priList: [], //商机列表
      // 显示搜索条件
      showSearch: true,

      // 列信息
      columns: [
        { key: 0, label: `企业ID`, visible: true },
        { key: 1, label: `客户名称`, visible: true },
        { key: 2, label: `联系人`, visible: true },
        { key: 3, label: `手机号`, visible: true },
        { key: 4, label: `部门`, visible: true },
        { key: 5, label: `职务`, visible: true },
        { key: 6, label: `协作人名称`, visible: true },
        { key: 7, label: `商机来源`, visible: true },
        { key: 8, label: `创建时间`, visible: true }
      ],
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 列表总条数
      total: 0,
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        targetType: '商机',
        Belong: 1 //为1表示公海，为2表示私海，其它为全部（不用传）
      },
      rules: {
        opportName: [
          {
            required: true,
            message: "商机名称不能为空",
            trigger: ["blur", "change"]
          }
        ],
        probability: [
          {
            required: true,
            message: "预计成交几率不能为空",
            trigger: ["blur", "change"]
          }
        ],
        period: [
          {
            required: true,
            message: "销售阶段不能为空",
            trigger: ["blur", "change"]
          }
        ],
        customerId: [
          {
            required: true,
            message: "客户名称不能为空",
            trigger: ["blur", "change"]
          }
        ],
        leaderId: [
          {
            required: true,
            message: "负责人不能为空",
            trigger: ["blur", "change"]
          }
        ],
        contactId: [
          {
            required: true,
            message: "联系人不能为空",
            trigger: ["blur", "change"]
          }
        ],
      },
      opportForm: {
        opportNumber: "", //商机编码
        opportName: "", //商机名称
        customerId: "", //客户编码
        customerName: "", //客户编码（不传）
        contactId: "", //联系人id
        contactName: "", //联系人名称（不传）
        probability: null, //预计成交几率
        period: "", //销售阶段
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helper: "",
        remark: "", //线索详情
        leaderId: 0, //负责人
        leaderName: [], //负责人
        leaderInfo: [], //负责人
        detailList: [], //商机明细
        detailList2: [], //商机明细
      },
      fromMap: new Map(), //商机来源map
      employeeMap: new Map(), //员工map
      periodTypeMap: new Map(),//销售阶段列表
      periodSelectlis: [], //销售阶段列表
      //商机明细相关变量
      choiceLoading: false,
      //商机明细相关变量
      //客户、联系人
      customerMap: new Map(),//客户
      contactMap: new Map(),//联系人
      //客户、联系人
      EditOpportId: 0,//是否是编辑
      AdvanceRow: [],//选取的数据
      detailsOped: false,//详情数据
      detailsRow: [],//详情

    };
  },
  async mounted() {
    await this.getPeriodList()
    this.getOpportList();

  },
  methods: {
    returnRowHelperName(row) {
      if (row.Helper) {
        let namelist = row.HelperUsers.map(ro => ro.RealName)
        return namelist.join(',')
      }
    },
    handDelete(row) {
      // console.log(row,'row');
      // return
      opportDel({
        id: row.Id
      }).then((res) => {
        if (res.code == 0) {
          this.$modal.msgSuccess("删除成功");
          this.getOpportList()
        }
      })
    },
    periodChange(event) {
      this.opportForm.probability = this.periodTypeMap.get(event).Probability
    },
    //提交评论
    handComment() {
      // console.log(this.detailsRow, 'this.detailsRow');
      // return
      AddComment({
        targetId: this.detailsRow.CustomerId,
        content: this.textareaComment,
        targetType: '商机',
        // parentCommentUserId: parentUserId,
        // parentCommentid: parentid,
      }).then((res) => {
        console.log(res)

      })
    },
    handleContactChange(val) {
      //完成联系人选择
      // console.log(val, 'val');
      if (val != null) {
        this.opportForm.contactId = val.Id
        this.opportForm.contactName = val.RealName
      }
    },
    openContactDialog() {
      //选择联系人
      this.$refs.contactOptions.openContactDialog()
    },
    handleCustomChange(val) {
      //完成客户选择
      // console.log(val, 'val');
      if (val != null) {
        this.opportForm.customerId = val.Id
        this.opportForm.customerName = val.CustomerName
      }
    },
    openCustomDialog() {
      //打开客户列表选择弹窗
      this.$refs.customOptions.openCustomDialog()
    },
    deleteProductLis(index) {
      //删除商机明细
      this.opportForm.detailList.splice(index, 1)
      this.opportForm.detailList2.splice(index, 1)
    },
    //选择商机明细
    openWupinDialog() {
      this.$refs.wupinChoice.openWupinDialog()
    },
    onWupinConfirm(selectArr) {
      this.choiceLoading = true
      if (selectArr.length > 0) {
        this.opportForm.detailList = []
        selectArr.forEach(element => {
          if (this.EditOpportId) {
            this.opportForm.detailList.push({
              label: element.ProductLabel == "U" ? "半成品" : "成品",
              productId: element.Id,
              quantity: 1,
              price: element.Price,
              opportId: this.EditOpportId,
              name: element.ProductName,
              number: element.SkuNumber
            });
          } else {
            this.opportForm.detailList.push({
              label: element.ProductLabel == "U" ? "半成品" : "成品",
              productId: element.Id,
              quantity: 1,
              price: element.Price,
              name: element.ProductName,
              number: element.SkuNumber
            });
          }

        })
        this.opportForm.detailList2 = JSON.parse(JSON.stringify(selectArr))
      }
      this.choiceLoading = false
    },

    //选择商机明细
    async getPeriodList() {
      //获取销售阶段
      try {
        let res = await periodList()
        // console.log("销售阶段", res);
        if (res.data) {
          this.periodSelectlis = res.data;
          res.data.map(row => {
            this.periodTypeMap.set(row.Id, row)
          })

        }
      } catch (error) {
        console.log("销售阶段查询错误", error);
      }

    },
    async getGenerateNumber() {
      //生成商机唯一编号
      try {
        let res = await GenerateNumber();
        // console.log("生成商机编码", res);

        this.opportForm.opportNumber = res.data;
        // console.log("表单信息", this.opportForm);
      } catch (error) {
        console.log("生成编码报错", err);
      }
    },
    //点击打开详情
    handleOpenDetails(row) {
      this.$router.push({ path: '/crm/opport/details', query: { id: row.Id } })
    },
    handleUpdate(row) {
      //修改商机信息
      if (row.Id) {
        this.EditOpportId = row.Id
        opportInfo({ id: row.Id }).then(res => {
          // console.log("点击的商机详情", res);
          if (res.data) {
            let data = res.data;
            let useList = [];
            let helperName = []
            let leaderList = [];
            let leaderName = "";
            if (data.Helper && data.HelperUsers) {
              helperName = data.HelperUsers.map(row => row.RealName)
              useList = data.HelperUsers.map(row => {
                let obj = {
                  id: row.Id,
                  name: row.RealName,
                  avatar: row.Avatar,
                  type: 'user'
                }
                return obj
              })
              let noticeUsers = data.HelperUsers.map(row => row.Id)
              data.Helper = noticeUsers.join(',')
            }
            if (data.LeaderId) {
              //获取责任人相关信息
              leaderName = data.LeaderUser.RealName;
              leaderList = [{
                id: data.LeaderUser.Id,
                name: data.LeaderUser.RealName,
                avatar: data.LeaderUser.Avatar,
                type: 'user'
              }]
            }
            let detailList = []
            let detailList2 = []
            if (data.DetailList && data.DetailList.length > 0) {
              data.DetailList.map(row => {
                let obj1 = {
                  label: row.ProductInfo.ProductLabel == "U" ? "半成品" : "成品",
                  productId: row.ProductId,
                  opportId: data.Id,
                  quantity: row.Quantity,
                  price: row.Price,
                  name: row.ProductInfo.ProductName,
                  number: row.ProductInfo.SkuNumber
                }
             
                detailList.push(obj1)
                detailList2.push(row.ProductInfo)
              })
            }
            this.opportForm = {
              opportNumber: data.OpportNumber, //商机编码
              opportName: data.OpportName, //商机名称
              customerId: data.CustomerId, //客户编码
              customerName: data.CustomerName, //客户名称（不传）
              contactId: data.ContactId, //联系人id
              contactName: data.ContactUser ? data.ContactUser.RealName : '', //联系人名称（不传）
              probability: data.Probability, //预计成交几率
              period: data.Period, //销售阶段
              userInfo: useList, //已经选择的协作人员工
              helperName: helperName, //
              helper: data.Helper,
              remark: data.Remark, //线索详情
              leaderId: data.LeaderId, //负责人
              leaderName: leaderName, //负责人
              leaderInfo: leaderList, //负责人
              detailList: detailList, //商机明细
              detailList2: detailList2, //商机明细
              id: data.Id
            };
            this.$nextTick(() => {
              const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
              const arr = Array.from(closeIcons);
              arr.forEach((item) => {
                item.style.display = "none";
              });
            });
            this.open = true;
            this.diaTitle = "编辑商机";
          }
        });
      }
    },
    submitForm() {
      //提交数据
      this.$refs.opportForm.validate(valid => {
        if (valid) {
          if (!this.opportForm.leaderId) {
            this.$modal.error("负责人不能为空");
            return
          }
          let submitForm = {};
          submitForm = JSON.parse(JSON.stringify(this.opportForm));
          submitForm.helperName = submitForm.helperName.join(",");
          delete submitForm.userInfo;
          delete submitForm.leaderName;
          delete submitForm.leaderInfo;
          delete submitForm.detailList2;
          delete submitForm.customerName;
          delete submitForm.contactName;
          this.loading = true;
          if (submitForm.id) {
            opportEdit(submitForm)
              .then(res => {
                // console.log(res, "编辑商机成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("编辑成功");
                  this.open = false;
                  this.getOpportList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            opportAdd(submitForm)
              .then(res => {
                // console.log(res, "添加商机成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  this.getOpportList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          }
        }
      });
    },
    //选择负责人和协作人
    getLeaderFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectLeader.blur();
      this.$refs.leaderPicker.show(this.opportForm.leaderInfo, "user");
    },
    getUsersFocus() {
      //获取协作人选择下拉列表的焦点
      this.$refs.selectUsers.blur();
      this.$refs.userPicker.show(this.opportForm.userInfo, "user");
    },
    selectUsersed(values) {
      //选择协作人
      this.opportForm.userInfo = values;
      if (values.length > 0) {
        let li = [];
        let li2 = [];
        let li3 = [];
        values.map((it, ix) => {
          // console.log(it.name);
          li.push(parseInt(it.id));
          li2.push(it.name);
          li3.push(it.avatar);
          // console.log("li", li);
        });
        this.opportForm.helper = li.join(",");
        this.opportForm.helperName = li2;
      }
      else {
        this.opportForm.helper = undefined;
        this.opportForm.helperName = undefined;
      }
      this.$nextTick(() => {
        const closeIcons = this.$refs.selectUsers.$el.querySelector(".el-select__tags").querySelectorAll(".el-icon-close");
        const arr = Array.from(closeIcons);
        arr.forEach((item) => {
          item.style.display = "none";
        });
      });
      this.$forceUpdate();
    },
    selectLeadered(values) {

      //选择负责人
      this.opportForm.leaderInfo = values;
      if (values.length > 0) {
        this.opportForm.leaderId = values[0].id;
        this.opportForm.leaderName = values[0].name;
        this.opportForm.leaderAvatar = values[0].avatar
      }
      else {
        this.opportForm.leaderId = 0;
        this.opportForm.leaderName = undefined;
        this.opportForm.leaderAvatar = undefined;
      }
      this.$forceUpdate();
    },
    //选择负责人和协作人
    // 取消按钮
    cancel() {
      this.open = false;
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getOpportList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      //选中行的样式设置
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "#F6F9FF"
        };
      }
    },
    handleAdd() {
      //打开添加商机
      this.opportForm = {
        opportNumber: "", //商机编码
        opportName: "", //商机名称
        customerId: "", //客户编码
        customerName: "", //客户编码（不传）
        contactId: "", //联系人id
        contactName: "", //联系人名称（不传）
        probability: null, //预计成交几率
        period: "", //销售阶段
        userInfo: [], //已经选择的协作人员工
        helperName: [], //
        helper: "",
        remark: "", //线索详情
        leaderId: 0, //负责人
        leaderName: [], //负责人
        leaderInfo: [], //负责人
        detailList: [], //商机明细
        detailList2: [], //商机明细
      };
      this.diaTitle = "添加商机";
      this.resetForm("opportForm");
      this.getGenerateNumber()
      this.EditOpportId = 0
      this.open = true;
    },
    getOpportList() {
      //获取公海商机列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      opportList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          console.log("商机列表", response);
          if (response.data && response.data.List) {
            this.priList = response.data.List;
          }

          // for(let i=0;i<3;i++){
          //   this.agentList=[...this.agentList,...this.agentList]
          // }
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    }
  }
};
</script>

<style lang="scss" scoped>
::v-deep::-webkit-scrollbar {
  display: none;
  /* Chrome Safari */
}

.el-tabs__nav-wrap::after {
  height: 0px;
}


.items-title {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  height: 48px;
  background-color: rgb(249, 250, 252);
  padding: 0 15px;
  margin-bottom: 5px;
}

.add_clue_form {
  ::v-deep .el-form-item {
    margin-bottom: 18px;
  }
  ::v-deep .el-form-item__label {
    padding-bottom: 0;
    text-align: right;
  }
  ::v-deep .form_input_style.el-input {
    height: 48px;
    line-height: 48px;
    input {
      height: 48px;
      line-height: 48px;
    }
  }
  .form_input_style {
    ::v-deep .el-input {
      height: 48px;
      line-height: 48px;
      input {
        height: 48px;
        line-height: 48px;
      }
    }
  }
}
</style>
