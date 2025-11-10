<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form :model="queryParams" class="biaodan" ref="queryForm" :inline="true">
          <el-form-item label="仓库名称" prop="Name">
            <el-input v-model="queryParams.Name" placeholder="请输入仓库名称"></el-input>
          </el-form-item>
          <el-form-item label="仓库状态" prop="Status">
            <el-select v-model="queryParams.Status" placeholder="请选择仓库状态">
              <el-option label="停用" value="0"></el-option>
              <el-option label="正常" value="1"></el-option>
            </el-select>
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
            <el-col :span="1.5" v-hasPermi="['/StorageService/House/Add']">
              <el-button type="primary" plain @click="handleAdd">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left:6px">新增</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table border v-loading="loading" :data="tbList" class="data_table" :header-cell-style="cellSty"
          style="width:100%" row-key="Id">
          <el-table-column label="仓库Id" prop="Id" width="180" align="center">
          </el-table-column>
          <el-table-column label="仓库名称" align="left">
            <template slot-scope="scope">
              <span>{{ scope.row.StoreName }}</span>
            </template>
          </el-table-column>
          <el-table-column label="仓库类型" align="center" width="120">
            <template slot-scope="scope">
              <el-tag type="success" v-if="scope.row.IsSystem == 1">系统</el-tag>
              <el-tag type="info" v-else>一般</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="仓库状态" align="center" width="120">
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 1">正常</el-tag>
              <el-tag v-else type="info">停用</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="负责人" align="center" prop="LeaderName" width="120"></el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="220">
            <template slot-scope="scope">
              <el-link icon="el-icon-edit" type="primary" @click="handleUpdate(scope.row)"
                v-hasPermi="['/StorageService/House/Edit']">修改</el-link>
              <el-link icon="el-icon-delete"
                v-if="scope.row.IsSystem != 1 && checkItemPermi(['/StorageService/House/Remove'])" type="danger"
                @click="handleDelete(scope.row)" style="margin-left:15px;">删除</el-link>
              <el-link icon="el-icon-video-pause" type="warning" v-if="scope.row.IsSystem != 1 && scope.row.Status == 1"
                @click="handleStop(scope.row)" style="margin-left:15px;">停用</el-link>

              <el-link icon="el-icon-video-play" type="success" v-if="scope.row.IsSystem != 1 && scope.row.Status == 0"
                @click="handleStart(scope.row)" style="margin-left:15px;">启用</el-link>

            </template>
          </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize" @pagination="getList" />
      </div>

      <!-- 添加编辑耗材对话框 -->
      <el-dialog :title="title" v-loading="houseloading" :close-on-click-modal="false" :visible.sync="open"
        width="650px" append-to-body>
        <el-form ref="form" :model="form" :rules="rules" label-width="120px" label-position="top">
          <el-row>
            <el-col :span="11">
              <el-form-item label="仓库名称" prop="StoreName">
                <el-input v-model="form.StoreName" placeholder="请输入仓库名称" />
              </el-form-item>
            </el-col>

            <el-col :span="11" style="margin-left:20px;">
              <el-form-item label="负责人" prop="LeaderId">
                <el-select filterable allow-create default-first-option v-model="form.LeaderName" ref="selectLeader"
                  placeholder="请选择负责人" @focus="getLeaderFocus" style="width:100%"></el-select>
                <org-picker :multiple="false" ref="leaderPicker" @ok="selectLeadered" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-tabs v-model="activeName" type="card">
            <el-tab-pane label="出库审核流程" name="first">
              <el-row>
                <el-col :span="24">
                  <el-form-item label="出库审核流程">
                    <div style="display:flex;align-items: center;width: 70%;">
                      <el-input v-model="form.LeaveTemplateName" readonly placeholder="请选择出库的审核模板" @focus="openFlowPicker(0)">
                        <i slot="suffix" @click="onClearOut" v-if="form.LeaveTemplateName != ''" class="el-icon-circle-close"
                          style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                      </el-input>
                    </div>
                  </el-form-item>
                </el-col>
              </el-row>
              <houseFlow v-show="form.LeaveTemplateName!=''" ref="LeaveTemplate" :formInitVal="formInit[0]" :tbloading="tbloading[0]" flowTitle="出库审核流程表单初始化" :index="0" @setformInitData="setformInitData"></houseFlow>
            </el-tab-pane>
            <el-tab-pane label="入库审核流程" name="second">
              <el-row>
                <el-col :span="24">
                  <el-form-item label="入库审核流程">
                    <div style="display:flex;align-items: center;width: 70%;">
                      <el-input v-model="form.EnterTemplateName" readonly placeholder="请选择出库的审核模板" @focus="openFlowPicker(1)">
                        <i slot="suffix" @click="onClearIn" v-if="form.EnterTemplateName != ''" class="el-icon-circle-close"
                          style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                      </el-input>
                    </div>
                  </el-form-item>
                </el-col>
              </el-row>
              <houseFlow v-show="form.EnterTemplateName!=''" ref="EnterTemplate" :formInitVal="formInit[1]" :tbloading="tbloading[1]" flowTitle="入库审核流程表单初始化" :index="1" @setformInitData="setformInitData"></houseFlow>
            </el-tab-pane>
            <el-tab-pane label="出库申请流程" name="third">
              <el-row>
                <el-col :span="24">
                  <el-form-item label="出库申请流程">
                    <div style="display:flex;align-items: center;width: 70%;">
                      <el-input v-model="form.LeaveApplyTemplateName" readonly placeholder="请选择出库申请的流程模板" @focus="openFlowPicker(2)">
                        <i slot="suffix" @click="onClearApply" v-if="form.LeaveApplyTemplateName != ''" class="el-icon-circle-close"
                          style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                      </el-input>
                    </div>
                  </el-form-item>
                </el-col>
              </el-row>
              <houseFlow v-show="form.LeaveApplyTemplateName!=''" ref="LeaveApplyTemplate" :formInitVal="formInit[2]" :tbloading="tbloading[2]" flowTitle="出库申请流程表单初始化" :index="2" @setformInitData="setformInitData"></houseFlow>
            </el-tab-pane>
          </el-tabs>
          <el-row>
            <el-col :span="24">
              <el-form-item label="备注说明" prop="Remark">
                <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="form.Remark"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>

        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </el-dialog>
    </div>

    <FlowPicker ref="flowPicker" @selected="onSelected"></FlowPicker>
  </div>
</template>

<script>
import {
  houseList,
  addHouse,
  editHouse,
  delHouse,
  houseInfo,
  changeStatus
} from "@/api/storage/house";
import OrgPicker from "@/views/flowable/common/OrgPicker";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { flowminix } from "./flowminix";
import { checkPermi } from '@/utils/permission.js'
import {
  getUser,
} from "@/api/system/user";
import FlowPicker from "@/views/flowable/common/FlowPicker.vue";
import houseFlow from "./houseFlow.vue"
export default {
  name: "HouseList",
  mixins: [resizeTableCon,flowminix],
  components: { OrgPicker,FlowPicker,houseFlow },
  data() {
    return {
      activeName:'first',
      title: '',
      houseloading: false,
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 表格树数据
      tbList: [],
      // 是否显示弹出层
      open: false,
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Status: undefined,
        Name: undefined
      },
      // 总条数
      total: 0,
      // 表单参数
      form: {},
      // 表单校验
      rules: {
        StoreName: [
          { required: true, message: "仓库名称不能为空", trigger: "blur" }
        ]
      },
      leaderInfo: undefined,
      activeIdx: 0,
    };
  },
  created() {
    this.getList();
  },
  methods: {
    openFlowPicker(idx) {
      this.activeIdx = idx;
      this.$refs.flowPicker.OpenDialog();
    },
    
    onClearOut() {
      this.form.LeaveTemplateId = 0;
      this.form.LeaveTemplateName = "";
      this.form.LeaveFlowInitJson = "";
      this.setformInitData([],0)
    },
    onClearIn() {
      this.form.EnterTemplateId = 0;
      this.form.EnterTemplateName = "";
      this.form.EnterFlowInitJson = "";
      this.setformInitData([],1)
    },
    onClearApply(){
      this.form.LeaveApplyTemplateId = 0;
      this.form.LeaveApplyTemplateName = "";
      this.form.LeaveApplyFlowInitJson = "";
      this.setformInitData([],2)
    },   
    checkItemPermi(value) {
      return checkPermi(value);
    },
    /** 查询列表 */
    getList() {
      this.loading = true;
      houseList(this.queryParams).then(response => {
        this.tbList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.form = {
        Id: undefined,
        StoreName: "",
        Remark: "",
        LeaveTemplateId:0,
        EnterTemplateId:0,
        LeaveApplyTemplateId:0,
        LeaveTemplateName:"",
        EnterTemplateName:"",
        LeaveApplyTemplateName:''
      };
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.resetForm("queryForm");
      this.handleQuery();
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.title = "新增仓库";
      this.reset();
      this.activeName='first'
      this.open = true;
    },
    async handleUpdate(row) {
      this.activeName='first'
      this.open = true;
      this.houseloading = true;
      this.title = "编辑仓库";
      let rsp = await houseInfo(row.Id,true);
      console.log("得到的仓库信息",rsp);
      this.form = rsp.data;
      let uii = await getUser(this.form.LeaderId);
      console.log(uii,'uii');
      this.form.LeaderName = uii.data.user.RealName;
      this.form.Avatar = uii.data.user.avatar
      if(rsp.data.LeaveTemplateId){
        this.activeIdx=0
        try {
          await this.onSelected({Id:rsp.data.LeaveTemplateId,Name:rsp.data.LeaveTemplateName},true,JSON.parse(rsp.data.LeaveFlowInitJson))
        } catch (error) {
        }
      }
      if(rsp.data.EnterTemplateId){
        this.activeIdx=1
        try {
          await this.onSelected({Id:rsp.data.EnterTemplateId,Name:rsp.data.EnterTemplateName},true,JSON.parse(rsp.data.EnterFlowInitJson))
        } catch (error) {
        }
      }
      if(rsp.data.LeaveApplyTemplateId){
        this.activeIdx=2
        try {
          await this.onSelected({Id:rsp.data.LeaveApplyTemplateId,Name:rsp.data.LeaveApplyTemplateName},true,JSON.parse(rsp.data.LeaveApplyFlowInitJson))
        } catch (error) {
        }
      }
      this.houseloading = false;

    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate(valid => {
        if (valid) {
          let leaveFlowInit=this.returnformInit(0)
          for (let i = 0; i < leaveFlowInit.length; i++) {
            if (leaveFlowInit[i].way == 1 && leaveFlowInit[i].val == "") {
              this.$modal.msgError('出库审核流程'+leaveFlowInit[i].title + "未选择初始值");
              return;
            }
          }
          let enterFlowInit=this.returnformInit(1)
          for (let i = 0; i < enterFlowInit.length; i++) {
            if (enterFlowInit[i].way == 1 && enterFlowInit[i].val == "") {
              this.$modal.msgError('入库审核流程'+enterFlowInit[i].title + "未选择初始值");
              return;
            }
          }
          let leaveApplyFlowInit=this.returnformInit(2)
          for (let i = 0; i < leaveApplyFlowInit.length; i++) {
            if (leaveApplyFlowInit[i].way == 1 && leaveApplyFlowInit[i].val == "") {
              this.$modal.msgError('出库申请流程'+leaveApplyFlowInit[i].title + "未选择初始值");
              return;
            }
          }
          this.form.LeaveFlowInitJson=JSON.stringify(leaveFlowInit)
          this.form.EnterFlowInitJson=JSON.stringify(enterFlowInit)
          this.form.LeaveApplyFlowInitJson=JSON.stringify(leaveApplyFlowInit)
          if (this.form.Id != undefined) {
            if (this.form.LeaderId == null) {
              this.form.LeaderId = 0;
              this.form.LeaderId = 0;
            }
            editHouse(this.form).then(response => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });

          } else {

            addHouse(this.form).then(response => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });

          }
        }
      });

    },
    /** 删除按钮操作 */
    handleDelete(row) {
      this.$modal
        .confirm('是否确认删除名称为"' + row.StoreName + '"的数据项？')
        .then(function () {
          return delHouse(row.Id);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
    handleStop(row) {
      this.$modal
        .confirm('是否确认停用"' + row.StoreName + '"？')
        .then(function () {
          return changeStatus(row.Id, "0");
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("操作成功");
        })
        .catch(() => { });
    },
    handleStart(row) {
      this.$modal
        .confirm('是否确认启用"' + row.StoreName + '"？')
        .then(function () {
          return changeStatus(row.Id, "1");
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("操作成功");
        })
        .catch(() => { });
    },
    //选择负责人
    getLeaderFocus() {
      //获取负责人选择下拉列表的焦点
      this.$refs.selectLeader.blur();
      if (this.form.LeaderId > 0) {
        this.leaderInfo = [{ id: this.form.LeaderId, name: this.form.LeaderName, avatar: this.form.Avatar, type: "user" }];
      }
      else {
        this.leaderInfo = [];
      }

      this.$refs.leaderPicker.show(this.leaderInfo, "user");
    },
    selectLeadered(values) {
      //选择负责人
      this.leaderInfo = values;
      if (values.length > 0) {
        this.form.LeaderId = values[0].id;
        this.form.LeaderName = values[0].name;
        this.form.Avatar = values[0].avatar;
      }
      else {
        this.form.LeaderId = 0;
        this.form.LeaderName = undefined;
        this.form.Avatar = undefined;
      }
      this.$forceUpdate();
    },
  }
};
</script>
<style lang="scss"></style>