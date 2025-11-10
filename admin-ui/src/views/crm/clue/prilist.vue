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
                  <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/CRMService/Clue/Add']">
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
              <el-table-column label="客户名称" v-if="columns[0].visible"
                :show-overflow-tooltip="true" >
                <template slot-scope="scope">
                  <el-link type="primary" @click="viewInfo(scope.row)">{{ scope.row.CompanyName }}<i class="el-icon-edit el-icon--right"></i></el-link>
                  <el-tag type="warning" v-if="scope.row.ItemOverday!=null&&scope.row.ItemOverday<=7">{{scope.row.ItemOverday}}天后跟进超期</el-tag>
                  <el-tag type="danger" v-if="scope.row.ItemOverday!=null&&scope.row.ItemOverday<=0">即将超期入公海</el-tag>
                </template>
              </el-table-column>
              <el-table-column label="联系人" align="center" key="RealName" prop="RealName" width="120" v-if="columns[1].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="手机号" align="center" key="Mobile" prop="Mobile" width="120" v-if="columns[2].visible"
                :show-overflow-tooltip="true" />
              <el-table-column label="部门" align="center" key="DeptName" prop="DeptName" width="140" v-if="columns[3].visible" />
              <el-table-column label="职务" align="center" key="PostName" prop="PostName" width="140" v-if="columns[4].visible" />
              <el-table-column label="协作人" align="center" key="HelperName" prop="HelperName" show-overflow-tooltip v-if="columns[5].visible" />
              <el-table-column label="线索来源" align="center" key="FromType" prop="FromType"  width="120" v-if="columns[6].visible">
                <template slot-scope="scope" v-if="scope.row.FromType">
                  <span>{{ fromMap.get(scope.row.FromType) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[7].visible" show-overflow-tooltip>
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" fixed="right" align="center" class-name="small-padding fixed-width" width="258">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-back" @click.stop="handleReturn(scope.row)"
                    v-hasPermi="['/CRMService/Clue/Return']">退回</el-button>
                  <el-button type="text" icon="el-icon-edit" @click.stop="handleUpdate(scope.row)"
                    v-hasPermi="['/CRMService/Clue/Edit']">修改</el-button>
                  <el-button type="text" icon="el-icon-delete" @click.stop="handleDelete(scope.row)"
                    v-hasPermi="['/CRMService/Clue/Remove']">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getPriList" />
          </div>
        </el-col>
      </el-row>

    </div>
    
    <clue-add ref="clueAdd" @errLoading="clueAddErrLoading" @finishLoading="clueAddfinishLoading"></clue-add>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";

import {
  privateClue,
  privateClueDel,
  clueReturn,
} from "@/api/crm/clue";

import OrgPicker from "@/views/flowable/common/OrgPicker";
import CustomerDialog from "@/views/crm/compontent/customer_dialog";
import ClueAdd from "@/views/crm/compontent/clue-add";
import {getCrmConfig} from "@/api/crm/config";
export default {
  name: "cluePrilist",
  components: { OrgPicker, CustomerDialog, ClueAdd },
  mixins: [resizeTableCon],
  dicts: ["follow_way"],
  data() {
    return {
      
      infoVisible: false, //详情的弹出层是否显示
      fromList: [
        {
          value: "weixin",
          label: "微信线索"
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
      priList: [], //线索列表
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
        { key: 7, label: `线索来源`, visible: true },
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

      fromMap: new Map(), //线索来源map
      employeeMap: new Map(), //员工map
      returnContent: "",//退回线索的描述内容
      overday:0,
    };
  },

  async mounted() {
    let tmpconfig=await getCrmConfig();
    this.overday=tmpconfig.data.FollowReturnDay;

    this.setFromMap();
    this.getPriList();
  },

  methods: {
    clueAddErrLoading() {
      //线索添加失败
      this.loading = false;
    },
    clueAddfinishLoading() {
      //线索添加成功
      this.loading = false;
      this.$refs.clueAdd.cancel()//关闭弹窗
      this.getPriList();
    },
    
    typeChange() { },
    
    
    // 取消按钮
    customerDialogClose() {
      this.addCustomDia = false;
    },
    
    viewInfo(val) {
      //跳转线索详情
      this.$router.push({ path: "/crm/clue/clueDetail", query: { id: val.Id, } });
    },
    handleReturn(row) {
      //退回线索
      this.returnContent = ""; //将退回原因置空
      var _this = this;
      const h = _this.$createElement;
      _this
        .$msgbox({
          title: "是否确认退回线索",
          message: h(
            "div",
            {
              attrs: {
                class: "el-textarea"
              }
            },
            [
              h("textarea", {
                attrs: {
                  class: "el-textarea__inner",
                  autocomplete: "off",
                  rows: 4,
                  placeholder: "请输入退回原因",
                  id: "returnContent"
                },
                value: _this.returnContent,
                on: { input: _this.onCommentInputChange }
              })
            ]
          ),
          showCancelButton: true,
          confirmButtonText: "退回",
          cancelButtonText: "取消",
          beforeClose: (action, instance, done) => {
            if (action === "confirm") {
              if (this.returnContent) {
                instance.confirmButtonLoading = true;
                clueReturn({ id: row.Id, reason: this.returnContent })
                  .then(res => {
                    if (res.code == 0) {
                      instance.confirmButtonLoading = false;
                      this.$message({
                        type: "success",
                        message: "退回成功"
                      });
                      this.getPriList();
                      done();
                    }
                  })
                  .catch(err => {
                    console.log("err", err);
                    instance.confirmButtonLoading = false;
                  });
              } else {
                this.$message({
                  type: "error",
                  message: "退回原因不能为空"
                });
              }
            } else {
              done();
            }
          }
        })
        .then(action => {
          console.log("action", action);

          // _this.$message({
          //   type: "info",
          //   message: "action: " + action
          // });
        });
    },
    onCommentInputChange() {
      this.returnContent = document.getElementById("returnContent").value;
    },
    handleUpdate(row) {
      //修改线索信息
      if (row.Id) {
        this.$refs.clueAdd.handleUpdate(row)
      }
    },
    handleDelete(row) {
      //删除线索
      this.$modal
        .confirm('是否确认删除客户名称为"' + row.CompanyName + '"的线索？')
        .then(() => {
          this.loading = true;
          return privateClueDel({ id: row.Id });
          // .then(res => {
          //   console.log("删除私海线索", res);
          //   this.loading = false;
          //   if (res.code == 0) {
          //   }
          // })
          // .catch(err => {
          //   console.log("错误", err);
          //   thsi.loading = false;
          // });
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
      //设置线索来源map
      this.fromList.map(row => {
        this.fromMap.set(row.value, row.label);
      });
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
      // console.log(selection, "选中的");

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
      //打开添加线索
      this.$refs.clueAdd.handleAdd()
    },
    getPriList() {
      //获取公海线索列表
      this.loading = true;
      // this.queryParams.orgId = this.$store.getters.orgId;
      privateClue(this.addDateRange(this.queryParams, this.dateRange)).then(
        response => {
          // console.log("线索列表", response);
          if (response.data && response.data.List) {
            response.data.List.map(async row => {
              if(this.overday>0){
                let followTime= row.LastFollowDate.replace(new RegExp(/-/gm), '/').replace('T', ' ').replace(new RegExp(/\.[\d]{3}/gm), '');
                let deta = new Date()-new Date(followTime);
                let detaday=deta / (1 * 24 * 60 * 60 * 1000);
                row.ItemOverday=this.overday-detaday;
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

<style lang="scss">
</style>