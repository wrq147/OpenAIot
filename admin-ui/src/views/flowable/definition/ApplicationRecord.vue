<template>
  <div style="padding:20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form
          :model="queryParams"
          class="biaodan"
          ref="queryForm"
          :inline="true"
          label-position="left"
          @submit.native.prevent
        >
          <el-form-item label="提交时间" prop="dateRange">
            <el-date-picker
              v-model="dateRange"
              class="set_radius"
              size="small"
              style="width: 232px"
              value-format="yyyy/MM/dd"
              format="yyyy/MM/dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>
          <el-form-item label="完成时间" prop="dateSubmit">
            <el-date-picker
              v-model="dateSubmit"
              class="set_radius"
              size="small"
              style="width: 232px"
              value-format="yyyy/MM/dd"
              format="yyyy/MM/dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>
          <el-form-item label="审批状态" prop="Status">
            <el-select
              clearable
              v-model="queryParams.Status"
              class="set_radius"
              placeholder="请选择"
              @change="getRecordFun"
            >
              <el-option
                size="small"
                v-for="item in approvaList"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="部门" prop="Depts">
            <el-select
              class="set_radius"
              multiple
              v-model="queryParams.DeptsInfo"
              ref="selectDept"
              value-key="name"
              placeholder="请选择"
              @focus="getSelectFocus"
              @change="deptChange"
            >
            <el-option v-for="opitem in queryParams.DeptsInfo" 
            :key="opitem.id"
           :label="opitem.name"
           :value="opitem"></el-option>
          </el-select>
            <org-picker
              :multiple="multiple"
              ref="orgPicker"
              :selected="queryParams.DeptsInfo"
              @ok="selected"
            />
          </el-form-item>

          <el-form-item label="申请人" prop="Users">
            <el-select
              class="set_radius"
              multiple
              v-model="queryParams.userInfo"
              ref="selectUsersOne"
              value-key="name"
              placeholder="请选择申请人"
              @focus="getUsersFocus('申请人')"
              @change="creatorChange"
            >
            <el-option v-for="opitem in queryParams.userInfo" 
            :key="opitem.id"
           :label="opitem.name"
           :value="opitem"></el-option>
          </el-select>
            <org-picker
              :multiple="multiple"
              ref="userPicker"
              @ok="selectUsersed"
            />
          </el-form-item>

          <!-- <div style="display:flex; justify-content: space-between;width：100%;padding-right:15px;margin-bottom:20px;"> -->
          <!-- 将最后一个表单和按钮合并为一排，然后表单和按钮分别排在左边和右边 -->
          <el-form-item label="审批编号" prop="Id">
            <el-input
              class="set_radius"
              v-model="queryParams.Id"
              placeholder="请输入"
              clearable
              size="small"
              @keyup.enter.native="getRecordFun"
            />
          </el-form-item>
          <el-form-item label="过滤表单">
            <div style="display: flex;">
              <el-select style="margin-right: 10px" v-model="queryParams.Items[0].FieldId" filterable  clearable placeholder="请选择表单字段" @change="formChange">
                <el-option v-for="opitem in tableListSearch" :key="opitem.filed" :label="opitem.filedName" :value="opitem.filed" />
              </el-select>
              <div v-if="queryParams.Items[0].fieldType === 'TextInput' || queryParams.Items[0].fieldType === 'TextareaInput'">
                <el-input v-model="queryParams.Items[0].Value" placeholder="请输入" clearable size="small" />
              </div>
              <div style="display:flex" v-if="queryParams.Items[0].fieldType === 'NumberInput'">
                <el-input v-model="queryParams.Items[0].Min" type="number" placeholder="请输入" clearable size="small" />
                <span style="margin: 0 10px">~</span>
                <el-input v-model="queryParams.Items[0].Max" type="number" placeholder="请输入" clearable size="small" />
              </div>
              <div v-if="queryParams.Items[0].fieldType === 'UserPicker'">
                <el-select class="set_radius"
                    multiple
                    v-model="formUserInfo"
                    ref="selectUsersTwo"
                    value-key="name"
                    placeholder="请选择人员"
                    @focus="getUsersFocus('人员选择')"
                    @change="formUserChange"
                  >
                    <el-option v-for="opitem in formUserInfo" 
                      :key="opitem.id"
                      :label="opitem.name"
                      :value="opitem"
                      />
                  </el-select>
              </div>
              <div v-if="queryParams.Items[0].fieldType === 'DevicPicker'">
                <el-select class="set_radius"
                    multiple
                    v-model="devValue"
                    ref="selectDev"
                    value-key="name"
                    placeholder="请选择设备"
                    @focus="showDialog"
                    @change="formUserChange"
                  >
                    <el-option v-for="opitem in devValue" 
                      :key="opitem.id"
                      :label="opitem.name"
                      :value="opitem"
                      />
                  </el-select>
                  <dev-picker ref="devicePicker" :selected="devValue" @ok="devSelected" />
              </div>
            </div>
          </el-form-item>

          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" size="medium" @click="resetRecordQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" size="medium" @click="getRecordFun">搜索</el-button>
            <!-- <el-button
            type="warning"
            plain
            icon="el-icon-printer"
            size="medium"
            @click="exportRecordFun"
            :loading="exportLoading"
            v-hasPermi="['/FlowService/Flow/Record']"
            >导出</el-button>-->
          </el-form-item>
          <!-- </div> -->
        </el-form>
      </div>
      <div class="elbiaoge_elform" :style="{'min-height':tableConHeight+'px'}">
        <el-row :gutter="10" class="button_row record_num">
          <div style="display:flex;justify-content: flex-start;align-items: center;">
            <el-col :span="1.5">
              <div>共{{this.total}}条申请记录</div>
            </el-col>
            <el-col :span="1.5">
              <el-button
                type="warning"
                plain
                size="mini"
                @click="exportRecordFun"
                :loading="exportLoading"
                v-hasPermi="['/FlowService/Flow/Record']"
              >
                <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                <span style="margin-left:6px">导出</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar
            :showSearch.sync="showSearch"
            :columns="columns"
            @queryTable="getRecordFun"
          ></right-toolbar>
        </el-row>
        <!-- <el-row :gutter="20" class="mb8 record_num">
      <el-col :span="1.5">
        <div>共{{this.total}}条申请记录</div>
      </el-col>
        </el-row>-->

        <el-table ref="tables" v-loading="loading" class="data_table" :data="myRecordList" border :default-sort="defaultSort" @sort-change="handleSortChange">
          <el-table-column label="审批编号" align="left" prop="Id" width="160" v-if="columns[0].visible" :show-overflow-tooltip="true"/>
          <el-table-column label="提交时间" :sortable="true" align="center" prop="create_time" v-if="columns[1].visible" :show-overflow-tooltip="true" :sort-orders="['descending', 'ascending']"/>
          <el-table-column label="完成时间" align="center" prop="FinishTime" v-if="columns[2].visible" :show-overflow-tooltip="true"/>
          <el-table-column label="申请人" align="center" prop="RealName" v-if="columns[3].visible" :show-overflow-tooltip="true"></el-table-column>
          <el-table-column label="申请部门" align="center" prop="dept_name" v-if="columns[4].visible" :show-overflow-tooltip="true"></el-table-column>

          <template v-for="(ites,inx) in tableList">
            <el-table-column :key="inx" :label="ites.filedName" align="center" :prop="ites.filed" v-if="columns[inx+6].visible" :show-overflow-tooltip="true">
              <template slot-scope="scope" v-if="ites.isShow">
                <div v-if="ites.formType=='UserPicker'" style="display: flex;justify-content: center;flex-wrap:wrap;">
                  <div v-for="(pson,pidx) in scope.row[ites.filed]" :key="pidx" style="display: flex; align-items: center">
                    <div v-if="$isNotEmpty(pson.avatar)" style="display:flex;align-items:center;justify-content:center;">
                      <el-avatar :title="pson.name" :size="35" :src="pson.avatar"/>
                      <span style="margin-left:8px">{{pson.name}}</span>
                    </div>
                    <span :title="pson.name" v-else class="avatar">{{ getShortName(pson.name) }}</span>
                  </div>
                </div>
                <div v-else-if="ites.formType=='DevicPicker'" style="display: flex;justify-content: center;flex-wrap:wrap;">
                  <div v-for="(pson,pidx) in scope.row[ites.filed]" :key="pidx" style="display: flex; align-items: center">
                    <div v-if="$isNotEmpty(pson.photoUrl)" style="display:flex;align-items:center;justify-content:center;">
                      <el-avatar :title="pson.name" :size="35" :src="pson.photoUrl"/>
                      <span style="margin-left:8px">{{pson.name}}</span>
                    </div>
                    <span :title="pson.name" v-else >{{ pson.name }}</span>
                  </div>
                </div>
                <div v-else-if="ites.formType=='DeptPicker'" style="display: flex;justify-content: center;flex-wrap:wrap;">
                  <div v-for="(pson,pidx) in scope.row[ites.filed]" :key="pidx">
                    <span>{{ pson.name }}</span>
                  </div>
                </div>
                <div v-else-if="ites.formType=='ImageUpload'">
                  <template v-if="scope.row[ites.filed]&&scope.row[ites.filed].length>0">
                    <div style="display: flex; align-items: center;margin-right:-5px;justify-content:center;">
                      <template v-for="(pson,pidx) in scope.row[ites.filed]">
                        <div :title="scope.row[ites.filed]" :key="pidx" v-if="$isNotEmpty(pson.url)" style="display:flex;align-items:center;justify-content:center;margin-right:5px">
                          <!-- <el-avatar :title="pson.name" :size="35" :src="pson.url"/> -->
                          <img :src="pson.url" alt="" style="width: 30px;height:30px;border-radius:3px;">
                        </div>
                        <span :key="pidx" :title="scope.row[ites.filed]" v-else class="avatar">{{scope.row[ites.filed]}}</span>
                      </template>
                    </div>
                  </template>
                </div>
                <div v-else>{{scope.row[ites.filed]}}</div>
              </template>
            </el-table-column>
          </template>

          <el-table-column
            label="审批状态"
            prop="Status"
            align="center"
            width="100"
            v-if="columns[5].visible"
            :show-overflow-tooltip="true"
          >
            <template slot-scope="scope">
              <el-tag v-if="scope.row.Status == 0" size="mini">运行中</el-tag>
              <el-tag type="warning" v-if="scope.row.Status == 1" size="mini">保存中</el-tag>
              <el-tag type="success" v-if="scope.row.Status == 2" size="mini">已完成</el-tag>
              <el-tag type="info" v-if="scope.row.Status == 3" size="mini">已结束</el-tag>
            </template>
          </el-table-column>
          <!-- <el-table-column label="耗时" align="center" width="180">
        <template slot-scope="scope">
          <label>{{ caldurTime(scope.row) }}</label>
        </template>
          </el-table-column>-->
          <!-- <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
        <template slot-scope="scope">
          <el-dropdown>
            <span class="el-dropdown-link">
              更多操作
              <i class="el-icon-arrow-down el-icon--right"></i>
            </span>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item
                icon="el-icon-circle-close"
                v-if="scope.row.Status == 0 || scope.row.Status == 1"
                @click.native="handleStop(scope.row)"
              >取消</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </template>
          </el-table-column>-->
        </el-table>

        <pagination
          v-show="total > 0"
          :total="total"
          :page.sync="queryParams.pageNum"
          :limit.sync="queryParams.pageSize"
          @pagination="getRecordFun"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { getRecord, getFormDetail, exportRecord } from "@/api/flowable/design";
import { str2Date, deltaTime } from "@/utils/index";
import FormRender from "../common/form/FormRender";
import devPicker from "../common/DevicePickers";
import "../common/utlity.js";
import { getFormGroups } from "@/api/flowable/design";
import OrgPicker from "../common/OrgPicker";
import { resizeTableCon } from "@/mixins/resizeTableCon";
export default {
  name: "ProcessList",
  components: { FormRender, OrgPicker, devPicker },
  mixins: [resizeTableCon],
  data() {
    return {
      multiple: true, //单选与多选控制
      approvaList: [
        {
          value: 0,
          label: "运行"
        },
        {
          value: 1,
          label: "保存中"
        },
        {
          value: 2,
          label: "完成"
        },
        {
          value: 3,
          label: "结束"
        }
      ],
      // 遮罩层
      loading: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 弹出层标题
      // 完成时间范围
      dateRange: [],
      //提交时间范围
      dateSubmit: [],
      //流程运行时间范围
      funDAte: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        TemplateId: null,
        Status: null, //流程状态
        Depts: [],
        Users: [],
        DeptsInfo: [],
        userInfo: [],
        Id: null,
        showAll: false,
        orderByColumn: null,
        Items: [
          {
            FieldId: '',
            Value: '',
            ValueArray : [],
            fieldType: '',
            Min : null,
            Max: null
          }
        ],
        isAsc: null
      },
      peopleType: '', // 人员选择框标志
      devValue: [], // 选择设备标志
      formUserInfo: [], // 选择人员标志

      // 默认排序
      defaultSort: { prop: "create_time", order: "descending" },
      myRecordList: [], //记录列表
      tableList: [],
      tableListSearch: [], // 表单字段列表
      temList: [],
      exportLoading: false,
      columns: [
        { key: 0, label: `审批编号`, visible: true },
        { key: 1, label: `提交时间`, visible: true },
        { key: 2, label: `完成时间`, visible: true },
        { key: 3, label: `申请人`, visible: true },
        { key: 4, label: `申请部门`, visible: true },
        { key: 5, label: `审批状态状态`, visible: true }
      ]
    };
  },
  created() {
    // this.getList();
    // this.getGroups();
    this.getRecordFun(); //获取流程记录
  },
  methods: {
    // 选择表单字段
    formChange(e) {
      this.queryParams.Items[0].ValueArray = [];
      this.queryParams.Items[0].Value = '';
      this.queryParams.Items[0].Min = null;
      this.queryParams.Items[0].Max = null;
      this.devValue = [];
      this.formUserInfo = [];
      const data = this.tableList.find((item) => item.filed == e);
      if (data) {
        this.queryParams.Items[0].fieldType = data.formType;
      } else {
        this.queryParams.Items[0].fieldType = "";
      }
    },
    creatorChange(vals){
      this.queryParams.pageNum=1;
      this.queryParams.Users=vals.map(x=>x.id);
      this.getRecordFun();
    },
    // 获取表单提交人员下拉列表
    formUserChange(vals) {
      this.queryParams.pageNum = 1;
      this.queryParams.Items[0].ValueArray = vals.map(x=>x.id);
      this.getRecordFun();
    },
    // 打开设备选择框
    showDialog() {
      this.$refs.devicePicker.show(this.devValue, "device");
    },
    // 获取设备列表
    devSelected(values) {
      this.devValue = values;
      this.queryParams.Items[0].ValueArray = values.map(x=>x.id);
      this.getRecordFun();
    },
    deptChange(vals){
      this.queryParams.pageNum=1;
      this.queryParams.Depts=vals.map(x=>x.id);
      this.getRecordFun();
    },
    getUsersFocus(name) {
      //获取用户下拉列表的焦点
      this.peopleType = name;
      if (this.peopleType === '申请人') {
        this.$refs.selectUsersOne.blur();
      } else {
        this.$refs.selectUsersTwo.blur();
      }
      this.$refs.userPicker.show();
    },
    selectUsersed(values) {
      //选择申请人
      this.$refs.userPicker.show([], "user");
      this.queryParams.pageNum=1;
      if (this.peopleType === "申请人") {
        this.$set(this.queryParams,"userInfo",values);
        this.queryParams.Users = values.map(x=>x.id);
      } else {
        this.formUserInfo = values;
        this.queryParams.Items[0].ValueArray = values.map(x=>x.id);
      }
      
      this.getRecordFun();
    },
    getSelectFocus() {
      //获取部门下拉列表的焦点
      this.$refs.selectDept.blur();
      this.$refs.orgPicker.show();
    },
    exportRecordFun() {
      //导出到excel
      /** 导出按钮操作 */
      // const queryParams = this.queryParams;
      this.$modal
        .confirm("是否确认导出流程记录？")
        .then(() => {
          this.exportLoading = true;
          // console.log("确认");
          this.queryParams.TemplateId = parseInt(this.$route.query.code);
          let outForm=this.addDateRange2(this.queryParams, this.dateRange, this.dateSubmit)
          // console.log("打印传参", outForm);
          delete outForm.userInfo
          return exportRecord(outForm);
        })
        .then(() => {
          this.exportLoading = false;
        })
        .catch(() => {});
      // exportRecord({id:this.queryParams.TemplateId}).then((rsp)=>{
      //   console.log("导出",rsp);

      // })
    },
    /** 排序触发事件 */
    handleSortChange(column, prop, order) {
      this.queryParams.orderByColumn = column.prop;
      this.queryParams.isAsc = column.order;
      this.getRecordFun();
    },
    selected(values) {
      this.$set(this.queryParams,"DeptsInfo",values);
      this.queryParams.pageNum=1;
      this.queryParams.Depts=values.map(x=>x.id);
      this.$refs.orgPicker.show([], "dept");
      this.getRecordFun();
      // console.log("选中的部门", this.queryParams.Depts);
    },
    resetRecordQuery() {
      this.dateRange = [];
      this.dateSubmit = [];
      this.$refs.tables.sort(this.defaultSort.prop, this.defaultSort.order);
      this.resetForm("queryForm");
      this.queryParams = {
        pageNum: 1,
        pageSize: 10,
        TemplateId: null,
        Status: null, //流程状态
        Depts: [],
        Users: [],
        Items: [
          {
            FieldId: '',
            Value: '',
            ValueArray : [],
            fieldType: '',
            Min : null,
            Max: null
          }
        ],
        Id: null,
        showAll: false,
        orderByColumn: null,
        isAsc: null
      };

      this.getRecordFun();
    },
    addDateRange2(params, dateRange, dateSubmit) {
      //分割时间

      let search = params;
      if (dateRange && dateRange.length == 2) {
        search["beginTime"] = dateRange[0];
        search["endTime"] = dateRange[1];
      } else {
        search["beginTime"] = "";
        search["endTime"] = "";
      }
      if (dateSubmit && dateSubmit.length == 2) {
        search["StartFinish"] = dateSubmit[0];
        search["EndFinish"] = dateSubmit[1];
      } else {
        search["StartFinish"] = "";
        search["EndFinish"] = "";
      }

      return search;
    },
    getShortName(name) {
      if (name) {
        return name.length > 2 ? name.substring(1, 3) : name;
      }
      return "**";
    },
    async getRecordFun() {
      //获取流程记录
      this.myRecordList = [];
      this.temList = [];
      this.loading = true;
      this.queryParams.TemplateId = parseInt(this.$route.query.code);
      let dataPar = {};
      let rsp = await getFormDetail(this.queryParams.TemplateId); //获取模板信息
      let form = rsp.data;
      form.formItems = JSON.parse(form.Form.FormFields);
      this.temList = this.getTemplete(form.formItems); //获取模板组件信息
      let res = await getRecord(
        this.addDateRange2(this.queryParams, this.dateRange, this.dateSubmit)
      );
      // console.log("过滤条件", this.queryParams);

      if (res.code == 0) {
        if (rsp.code == 0) {
          this.loading = false;
        }
        //添加表头
        this.columns = [
          { key: 0, label: `审批编号`, visible: true },
          { key: 1, label: `提交时间`, visible: true },
          { key: 2, label: `完成时间`, visible: true },
          { key: 3, label: `申请人`, visible: true },
          { key: 4, label: `申请部门`, visible: true },
          { key: 5, label: `审批状态状态`, visible: true }
        ];
        this.tableList = [];
        this.temList.map((its, ix) => {
          let a = {
            filed: its.assemblyd,
            filedName: its.assemblyTitle,
            formType:its.formType,
            isShow: its.isShow
          };
  
          let colObj = {
            key: 5 + 1 + ix,
            label: its.assemblyTitle,
            visible: true
          };
          this.columns.push(colObj);
          this.tableList.push(a);
          
        });
        // console.log("流程记录",this.tableList);
        this.tableListSearch = this.tableList.filter(item => {
          const targetTypes = ['TextInput', 'TextareaInput', 'NumberInput', 'UserPicker', 'DevicPicker'];
          return targetTypes.includes(item.formType);
        });
        //添加表头
        this.total = res.data.Total;
        this.myRecordList = res.data.List;
        if (res.data.List.length > 0) {
          res.data.List.forEach((item, idx) => {
            // console.log("打印数组每组数据的键名", Object.keys(item),res);
            if (item.DataItems) {
              for (let tmp in item.DataItems) {
                this.temList.map((its, ix) => {
                  if (tmp == its.assemblyd) {
                    if(its.formType=='UserPicker'||its.formType=='DeptPicker'||its.formType=='DevicPicker'||its.formType=='ImageUpload'){
                      item[its.assemblyd] = JSON.parse(item.DataItems[its.assemblyd]);
                    }
                    else{
                      item[its.assemblyd] = item.DataItems[its.assemblyd];
                    }

                  }
                });
                // console.log("查询模板结果", this.temList);
                this.myRecordList = res.data.List;
                // console.log("纪录列表", this.myRecordList);
              }
            }
          });
        }
      }
    },

    getTemplete(formItems) {
      //获取模板组件
      let templateArry = [];
      if (formItems.length > 0) {
        formItems.map((ite, inx) => {
          if (ite.name === "SpanLayout") {
            if (ite.props.items.length > 0) {
              templateArry = [
                ...templateArry,
                ...this.getTemplete(ite.props.items)
              ];
            }
          } else {
            let objs = [
              {
                assemblyTitle: ite.title,
                isShow: ite.props.enablePrint,
                formType:ite.name,
                assemblyd: ite.id
              }
            ];
            templateArry = [...templateArry, ...objs];
          }
        });
      }
      return templateArry;
    }

    // caldurTime: function(row) {//计算耗时
    //   if (row.FinishTime == null) {
    //     return deltaTime(str2Date(row.createTime), new Date());
    //   } else {
    //     return deltaTime(str2Date(row.createTime), str2Date(row.FinishTime));
    //   }
    // }
  }
};
</script>
<style lang="less" scope>
th {
  font-weight: normal;
}

.record_num {
  margin-top: 10px;
  margin-bottom: -5px;
  font-size: 14px;
  color: #515a6e;
  padding-left: 10px;
}

.content_con {
  padding-top: 15px;
  .content_li_con:last-child {
    border-bottom: 1px solid #ebeef5;
  }
  .content_li_con {
    padding: 12px;
    border-top: 1px solid #ebeef5;

    .title {
      font-size: 16px;
      text-align: left;
    }
    .content_ul {
      width: 100%;
      display: flex;
      justify-content: flex-start;
      flex-wrap: wrap;
      margin: 0;
      padding: 0;
      margin-top: 20px;

      .li_con {
        padding-left: 20px;
        width: 20%;
        height: 70px;
        box-sizing: border-box;
        margin-top: 10px;
        display: flex;
        justify-content: flex-start;

        .content_li {
          cursor: pointer;
          width: 100%;
          height: 70px;
          padding: 0 10px;
          display: flex;
          justify-content: space-between;
          align-items: center;
          border: 1px solid #ebeef5;
          box-sizing: border-box;
          border-radius: 5px;
          &.active_li {
            border: 1px solid #448ed7;
          }
          .left_cont {
            display: flex;
            align-items: center;
            .li-icons {
              width: 30px;
              height: 30px;
              border-radius: 5px;
              font-size: 20px;
              text-align: center;
              line-height: 30px;
            }
            span {
              margin-left: 10px;
            }
          }
          .active_content {
            color: #38adff;
            font-size: 12px;
          }
        }
      }
    }
  }
}
</style>
