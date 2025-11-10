<template>
  <div style="padding:20px 20px 0 20px;height:100%" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--用户数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="创建日期">
                <el-date-picker class="form_input_style" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd"
                  type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
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
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Follow/Add']">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left:6px">添加</span>
                  </el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getPriList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" :data="priList" :row-style="isRed" @selection-change="handleSelectionChange"
              class="data_table" :header-cell-style="cellSty" style="width:100%">
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column label="跟进类型" align="center" key="TargetType" prop="TargetType" v-if="columns[0].visible">
                <template slot-scope="scope">
                  <span>{{ scope.row.TargetType==0?'客户':'线索' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="跟进目标" align="center" key="TargetName" prop="TargetName" v-if="columns[1].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="联系人" align="center" key="ContactName" prop="ContactName" v-if="columns[2].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="跟进方式" align="center" key="FollowWayName" prop="FollowWayName" v-if="columns[3].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="商机名称" align="center" key="OpportName" prop="OpportName" v-if="columns[4].visible" />
              <el-table-column label="跟进人" align="center" key="FollowUserName" prop="FollowUserName" v-if="columns[5].visible" />
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[8].visible" width="240">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="258">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)"
                    v-hasPermi="['/CRMService/Follow/Edit']">修改</el-button>
                  <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)"
                    v-hasPermi="['/CRMService/Follow/Remove']">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getPriList" />
          </div>
        </el-col>
      </el-row>
      <el-dialog :title="diaTitle" :close-on-click-modal="false" :visible.sync="open" width="900px" append-to-body
        class="add_dialog_border">
        <el-form class="add_clue_form" ref="form" :model="form" :rules="rules" label-width="80px" label-position="left">
          <el-row :gutter="20">
            <el-col :span="24">
              <el-form-item label="跟进内容" prop="remark">
                  <text-editor :value="form.remark" @input="editorInput" :fileSize="10" :height="280" v-if="open" :editOpen="editOpen"></text-editor>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="跟进目标" prop="targetId">
                <el-input placeholder="请选择跟进目标" v-model="form.targetName" class="target-choice" @focus="choiceTarget"
                  ref="targetInput" clearable="">
                  <el-select v-model="form.targetType" slot="prepend" placeholder="">
                    <el-option label="客户" :value="0"></el-option>
                    <el-option label="线索" :value="1"></el-option>
                  </el-select>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="跟进商机" prop="opportName">
                <el-input class="form_input_style" v-model="form.opportName" placeholder="请选择商机" clearable
                  style="width:100%" @focus="choiceOpport" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="跟进时间" prop="followTime">
                <el-date-picker class="form_input_style" v-model="form.followTime" type="datetime" placeholder="选择日期时间" style="width:100%">
                </el-date-picker>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="跟进方式" prop="followWay">
                <el-select class="form_input_style" v-model="form.followWay" placeholder="请选择" style="width:100%">
                  <el-option :label="item.label" :value="item.value" v-for="item in followWayList"
                    :key="item.value"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="联系人" prop="realName">
                <el-input class="form_input_style" v-model="form.realName" placeholder="请选择联系人" clearable size="small"
                  style="width:100%" @focus="choiceContact" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="跟进人" prop="followUser">
                <el-select class="form_input_style" filterable allow-create default-first-option
                  v-model="form.followUserName" ref="selectFollow" placeholder="请选择跟进人" @focus="getFollowFocus"
                  style="width:100%"></el-select>
                <org-picker :multiple="false" ref="followPicker" :selected="form.userInfo" @ok="selectLeadered" />
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
    <clue-choice ref="culeOptions" @handleClueChange="handleClueChange"></clue-choice>
    <custom-choice ref="customOptions" @handleCustomChange="handleCustomChange"></custom-choice>
    <opport-choice ref="opportOptions" @handleOpportChange="handleOpportChange"></opport-choice>
    <contact-choice ref="contactOptions" @handleContactChange="handleContactChange"></contact-choice>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  followList,
  FollowInfo,
  addFollow,
  editFollow,
  delFollow
} from "@/api/crm/follow";

import OrgPicker from "@/views/flowable/common/OrgPicker";
import CustomChoice from "@/views/crm/compontent/custom-choice.vue"
import ClueChoice from "@/views/crm/compontent/clue-choice.vue"
import OpportChoice from "@/views/crm/compontent/opport-choice.vue"
import ContactChoice from "@/views/crm/compontent/contact-choice.vue"
import TextEditor from '@/components/Editor/index.vue'//富文本编辑器
var dayjs = require('@/utils/day.js')
export default {
  name: "cluePrilist",
  components: { OrgPicker, CustomChoice, ClueChoice, OpportChoice, ContactChoice,TextEditor },
  mixins: [resizeTableCon],
  dicts: ["follow_way"],
  data() {
    return {
      diaTitle: "添加跟进记录",
      fromList: [
        {
          value: "weixin",
          label: "微信跟进记录"
        },
        {
          value: "form",
          label: "流程表单"
        },
        {
          value: "other",
          label: "其他"
        }
      ],
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
      priList: [], //跟进记录列表
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
        { key: 7, label: `跟进记录来源`, visible: true },
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
        Belong: 2 //为1表示公海，为2表示私海，其它为全部（不用传）
      },
      rules: {
        followTime: [
          {
            required: true,
            message: "跟进时间不能为空",
            trigger: ["blur", "change"]
          }
        ],
        followUser: [
          {
            required: true,
            message: "跟进人不能为空",
            trigger: ["blur"]
          }
        ],
        followWay: [
          {
            required: true,
            message: "跟进方式不能为空",
            trigger: ["blur", "change"]
          },
        ],
        targetId: [
          {
            required: true,
            message: "跟进目标不能为空",
            trigger: ["change"]
          }
        ],
        remark: [
          {
            required: true,
            message: "跟进内容不能为空",
            trigger: ["blur", "change"]
          }
        ]
      },
      form: {
        targetType: 0,//0是客户，1是线索
        targetId: '',
        targetName: '',
        opportId: '',//商机id
        opportName: '',//商机名称
        followTime: null,
        realName: "", //联系人
        contactId: '',
        userInfo: [], //已经选择的跟进人
        followUserName: [], //
        followUser: null,//跟进人
        followWay: '',//跟进方式
        remark: "" //跟进记录详情
      },
      fromMap: new Map(), //跟进记录来源map
      returnContent: "",
      followWayList: [],//跟进方式列表
      editOpen:false,//是否显示富文本编辑器
    };
  },

  async mounted() {
    this.setFromMap();
    this.getPriList();
    this.followWayList = this.dict.type.follow_way;
  },
  methods: {
    editorInput(value){
      this.form.remark=value
    },
    handleContactChange(val) {
      //完成联系人选择
      // console.log(val, 'val完成选择联系人',val);
      if (val != null) {
        this.form.contactId = val.Id
        this.form.realName = val.RealName
      }
      this.$forceUpdate()
    },
    choiceContact() {
      //选择联系人
      this.$refs.contactOptions.openContactDialog()
    },
    handleOpportChange(val) {
      //选择商机
      // console.log(val, 'val完成选择商机');
      if (val != null) {
        this.form.opportId = val.Id
        this.form.opportName = val.OpportName
      }
      this.$forceUpdate()
    },
    choiceOpport() {
      this.$refs.opportOptions.openOpportDialog()
    },
    handleCustomChange(val) {
      //完成客户选择
      // console.log(val, 'val完成客户选择');
      if (val != null) {
        this.form.targetId = val.Id
        this.form.targetName = val.CustomerName
      }
      this.$forceUpdate()
      console.log(this.form, 'this.form');
    },
    handleClueChange(val) {
      //选择线索后
      // console.log(val, 'val选择线索后');
      if (val != null) {
        this.form.targetId = val.Id
        this.form.targetName = val.CompanyName
      }
      this.$forceUpdate()
      // console.log(this.form, 'this.form');
    },
    choiceTarget() {
      //选择跟进目标主题
      if (this.form.targetType != null) {
        if (this.form.targetType == 0) {
          this.$refs.customOptions.openCustomDialog()
        } else if (this.form.targetType == 1) {
          this.$refs.culeOptions.openClueDialog()
        }
      } else {
        this.$message.error("请先选择目标类型");
        this.$refs.targetInput.blur()
      }
    },
    async choiceSize(val) {
      //获取跟进方式列表
      let rt = this.dict.getName("follow_way", val);
      this.$set(this.org, "SizeName", rt);
    },
    getFollowFocus() {
      //选择跟进人
      this.$refs.selectFollow.blur();
      this.$refs.followPicker.show(this.form.userInfo, "user");
    },
    selectLeadered(values) {
      //选择跟进人
      // console.log("选中的跟进人", values);

      this.form.userInfo = values;
      if (values.length > 0) {
        this.form.followUser = values[0].id;
        this.form.followUserName = values[0].name;
      }
      else {
        this.form.followUser = 0;
        this.form.followUserName = undefined;
      }
      this.$forceUpdate();
    },
    onCommentInputChange() {
      this.returnContent = document.getElementById("returnContent").value;
    },
    handleUpdate(row) {
      //修改跟进记录信息
      if (row.Id) {
        FollowInfo({ id: row.Id }).then(res => {
          // console.log("点击的跟进记录详情", res);
          if (res.data) {
            let data = res.data;
            let useList = [];
            let followUserName = "";
            if (data.FollowUser) {
              //获取责任人相关信息
              useList=[{
                 id: data.FollowUserInfo.Id,
                 name: data.FollowUserInfo.RealName,
                 avatar: data.FollowUserInfo.Avatar, 
                 type: "user" 
                }]
              followUserName = data.FollowUserInfo.RealName;
            }
            
            this.form = {
              targetType: data.TargetType,//0是客户，1是线索
              targetId: data.TargetId,
              targetName: data.TargetName,
              opportId: data.OpportId,//商机id
              opportName: data.OpportName,//商机名称
              followTime: data.FollowTime,
              realName: data.ContactInfo?data.ContactInfo.RealName:'', //联系人
              contactId: data.ContactId,
              userInfo: useList, //已经选择的跟进人
              followUserName:followUserName, //
              followUser: data.FollowUser,//跟进人
              followWay: data.FollowWay,//跟进方式
              remark: data.Remark, //跟进记录详情
              id:data.Id
            };
           this.$nextTick(()=>{
            this.open = true;
            this.editOpen=true
           })
            this.diaTitle = "编辑跟进记录";
          }
        });
      }
    },
    handleDelete(row) {
      //删除跟进记录
      this.$modal
        .confirm('是否确认删除该跟进记录？')
        .then(() => {
          this.loading = true;
          return delFollow({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("删除成功");
          this.getPriList();
          this.loading = false;
        })
        .catch(() => {
          this.loading = false;
        });
    },
    setFromMap() {
      //设置跟进记录来源map
      this.fromList.map(row => {
        this.fromMap.set(row.value, row.label);
      });
    },
    submitForm() {
      //提交数据
      this.$refs.form.validate(valid => {
        if (valid) {
          let submitForm = {};
          submitForm = JSON.parse(JSON.stringify(this.form));
          delete submitForm.userInfo;
          delete submitForm.targetName;
          delete submitForm.opportName;
          delete submitForm.realName;
          delete submitForm.followUserName;
          this.loading = true;
          if (submitForm.id) {
            editFollow(submitForm)
              .then(res => {
                console.log(res, "修改跟进记录成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("修改成功");
                  this.open = false;
                  this.editOpen=false
                  this.getPriList();
                }
              })
              .catch(err => {
                console.log("err", err);
                this.loading = false;
              });
          } else {
            addFollow(submitForm)
              .then(res => {
                console.log(res, "添加跟进记录成功");
                if (res.code == 0) {
                  this.loading = false;
                  this.$modal.msgSuccess("添加成功");
                  this.open = false;
                  this.editOpen=false
                  this.getPriList();
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

    // 取消按钮
    cancel() {
      this.open = false;
      this.editOpen=false
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getPriList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      console.log(selection, "选中的");

      this.ids = selection.map(item => item.Id);
      // console.log("选中的",this.ids);

      this.single = selection.length != 1;
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
      //打开添加跟进记录
      this.diaTitle = "添加跟进记录";
      let now = new Date();
      let nowTime=dayjs(now).format('YYYY-MM-DD HH:mm:ss')
      this.form = {
        targetType: 0,//0是客户，1是线索
        targetId: '',
        opportId: '',//商机id
        opportName: '',//商机名称
        followTime: nowTime,
        realName: "", //联系人
        contactId: '',
        userInfo: [{ id: parseInt(this.$store.state.user.uid), name: this.$store.state.user.name, avatar: this.$store.state.user.avatar,type:'user'}], //已经选择的跟进人
        followUserName: this.$store.state.user.name, //
        followUser: parseInt(this.$store.state.user.uid),//跟进人
        followWay: this.followWayList[0].value,//跟进方式
        remark: "" //跟进记录详情
      };
      this.resetForm("form");
      this.open = true;
      this.editOpen=true
    },
    getPriList() {
      //获取公海跟进记录列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      followList(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          // console.log("跟进记录列表", response);
          if (response.data && response.data.List) {
            response.data.List.map(row => {
              if (row.FollowUser) {
                row.FollowUserName=row.FollowUserInfo.RealName
              }
              if (row.ContactId) {
                row.ContactName=row.ContactInfo.RealName
              }
              if(row.FollowWay){
                row.FollowWayName=this.dict.getName("follow_way",row.FollowWay);
                // console.log("跟进方式",row.FollowWayName);
              }
            });
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
.target-choice.el-input {
  ::v-deep .el-select .el-input {
    width: 80px;
    height: 48px;
    line-height: 48px;
  }
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