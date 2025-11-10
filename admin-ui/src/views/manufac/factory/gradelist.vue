<template>
  <div class="conf-container">
    <div style="background-color: #fff;padding: 10px;">
      <el-tabs v-model="activeTab">
        <el-tab-pane label="代理配置" name="gradeMan">
          <el-row :gutter="20">
            <el-col :span="24" :xs="24">
              <div v-if="hasFactory" class="confbox" style="margin-bottom:25px;">
                <div class="header" style="margin-bottom: 25px;">
                  <span>代理设置</span>
                </div>
                <div>
                  <el-form ref="form" v-loading="loading" :model="form" label-width="120px">
                    <el-form-item>
                      <template v-slot:label>
                        <span style="margin-right: 5px;">证书模板</span>
                      </template>
                      <el-col :span="22">
                        <PrintSelector dataId="$PrintData1" v-model="form.CertTemplateId"></PrintSelector>
                      </el-col>
                    </el-form-item>
                    <el-form-item style="padding-top:15px;">
                      <el-button type="primary" @click="onSubmit">保存</el-button>
                    </el-form-item>
                  </el-form>
                </div>
              </div>
              <div v-else style="margin-bottom: 25px;">
                <el-alert title="您还没有成为生产商，暂无法设置代理" :closable="false" type="warning" center show-icon>
                </el-alert>
              </div>
            </el-col>
            <el-col :span="24" :xs="24">
              <div class="confbox">
                <div class="header" style="margin-bottom: 15px;">
                  <span>代理级别</span>
                  <div>
                    <el-button type="primary" size="small" icon="el-icon-plus" @click="openAddGrade"
                      plain>添加</el-button>
                  </div>
                </div>
                <el-table border class="airlineDeviceTable" v-loading="tbloading" :data="tbList" row-key="Id"
                  style="width:100%">
                  <el-table-column label="序号" align="center" width="100">
                    <template slot-scope="scope">
                      {{ scope.$index + 1 }}
                    </template>
                  </el-table-column>
                  <el-table-column prop="GradeName" label="级别名称">
                  </el-table-column>
                  <el-table-column label="操作" align="center" width="300">
                    <template v-slot:default="scope">
                      <el-button class="handle" size="mini"><i class="el-icon-rank" /> 移动</el-button>
                      <el-button @click="openEditGrade(scope.row)" size="mini"><i class="el-icon-edit" /> 修改</el-button>
                      <el-button v-if="scope.row.IsSystem == 0" type="danger" @click="delGrade(scope.row)"
                        size="mini"><i class="el-icon-delete" /> 删除</el-button>
                    </template>
                  </el-table-column>
                </el-table>

              </div>
            </el-col>
          </el-row>
        </el-tab-pane>
        <el-tab-pane v-if="checkPermission(['/MES/'])" label="生产配置" name="mesMan">
          <div style="padding:20px;">
            <el-form :model="mesform" label-width="120px" label-position="top">
              <el-form-item label="生产计划审核流程">
                <div style="display:flex;align-items: center;width: 70%;">
                  <el-input v-model="mesform.PlanTemplateName" readonly placeholder="请选择生产计划审核模板"
                    @focus="openFlowPicker(0)">
                    <i slot="suffix" @click="onClearOut" v-if="mesform.PlanTemplateName != ''"
                      class="el-icon-circle-close" style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                  </el-input>
                </div>
              </el-form-item>
              <div class="box-row" v-if="mesform.PlanTemplateName != ''">
                <div class="bx-hd">生产计划流程初始化</div>
                <div class="bx-bd">
                  <el-table border v-loading="planloading" :data="planFormInit" row-key="id" style="width:100%">
                    <el-table-column prop="title" label="表单字段" align="center" width="300"></el-table-column>
                    <el-table-column label="值类型" align="center" width="120">
                      <template slot-scope="scope">
                        <el-select v-model="scope.row.way" placeholder="请选择"
                          @change="changeLoadVal($event, scope.row, scope.$index)">
                          <el-option label="自定义" :value="0"
                            v-if="scope.row.eltype != 'DeptPicker' && scope.row.eltype != 'UserPicker'"></el-option>
                          <el-option label="系统值" :value="1"></el-option>
                        </el-select>
                      </template>
                    </el-table-column>
                    <el-table-column label="初始值" align="center">
                      <template v-slot:default="scope">
                        <template v-if="scope.row.eltype != 'TableList'">
                          <el-input v-if="scope.row.way == 0" v-model="scope.row.val" placeholder="请输入内容"></el-input>
                          <el-select v-else v-model="scope.row.val" placeholder="请选择">
                            <el-option label="发起人" value="发起人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                            <el-option label="发起人所属部门" value="发起人所属部门"
                              v-if="scope.row.eltype == 'DeptPicker'"></el-option>
                          </el-select>
                        </template>
                      </template>
                    </el-table-column>
                  </el-table>
                </div>
              </div>
              <el-form-item label="生产报工审核流程">
                <div style="display:flex;align-items: center;width: 70%;">
                  <el-input v-model="mesform.ReportTemplateName" readonly placeholder="请选择生产报工审核模板"
                    @focus="openFlowPicker(1)">
                    <i slot="suffix" @click="onClearOutReport" v-if="mesform.ReportTemplateName != ''"
                      class="el-icon-circle-close" style="vertical-align: middle;font-size: 22px;cursor: pointer;"></i>
                  </el-input>
                </div>
              </el-form-item>
              <div class="box-row" v-if="mesform.ReportTemplateName != ''">
                <div class="bx-hd">生产报工流程初始化</div>
                <div class="bx-bd">
                  <el-table border v-loading="reportloading" :data="reportFormInit" row-key="id" style="width:100%">
                    <el-table-column prop="title" label="表单字段" align="center" width="300"></el-table-column>
                    <el-table-column label="值类型" align="center" width="120">
                      <template slot-scope="scope">
                        <el-select v-model="scope.row.way" placeholder="请选择"
                          @change="changeLoadVal($event, scope.row, scope.$index)">
                          <el-option label="自定义" :value="0"
                            v-if="scope.row.eltype != 'DeptPicker' && scope.row.eltype != 'UserPicker'"></el-option>
                          <el-option label="系统值" :value="1"></el-option>
                        </el-select>
                      </template>
                    </el-table-column>
                    <el-table-column label="初始值" align="center">
                      <template v-slot:default="scope">
                        <template v-if="scope.row.eltype != 'TableList'">
                          <el-input v-if="scope.row.way == 0" v-model="scope.row.val" placeholder="请输入内容"></el-input>
                          <el-select v-else v-model="scope.row.val" placeholder="请选择">
                            <el-option label="发起人" value="发起人" v-if="scope.row.eltype == 'UserPicker'"></el-option>
                            <el-option label="发起人所属部门" value="发起人所属部门"
                              v-if="scope.row.eltype == 'DeptPicker'"></el-option>
                          </el-select>
                        </template>
                      </template>
                    </el-table-column>
                  </el-table>
                </div>
              </div>
              <el-form-item style="padding-top:15px;">
                <el-button type="primary" @click="onSaveMes">保存</el-button>
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>
      </el-tabs>

    </div>


    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open" width="500px">
      <el-form ref="gradeform" :model="gradeform" :rules="graderules" label-width="120px">
        <el-form-item label="级别名称" prop="GradeName">
          <el-input v-model="gradeform.GradeName" placeholder="请输入代理级别名称" maxlength="20" />
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="onGradeSubmit">确 定</el-button>
        <el-button @click="open = false">取 消</el-button>
      </div>
    </el-dialog>

    <FlowPicker ref="flowPicker" @selected="onSelected"></FlowPicker>
  </div>

</template>


<script>
import { checkPermi } from "@/utils/permission";
import {
  factoryInfo,
  setFactoryConfig
} from "@/api/manufac/factory";
import {
  factoryMesConfig,
  saveFactoryMesConfig
} from "@/api/mes/config";
import {
  gradeList,
  addGrade,
  editGrade,
  gradeSort,
  delGrade
} from "@/api/manufac/grade";
import { getFormDetail } from "@/api/flowable/design";
import FlowPicker from "@/views/flowable/common/FlowPicker.vue";
import PrintSelector from "@/views/report/print/printselector";
import Sortable from 'sortablejs';
import { getItems } from "@/views/flowable/common/utlity.js"
export default {
  name: "GradeList",
  components: { PrintSelector, FlowPicker },
  data() {
    return {
      loading: true,
      tbloading: true,
      form: {},
      tbList: [],
      title: "",
      open: false,
      // 表单参数
      gradeform: {},
      // 表单校验
      graderules: {
        GradeName: [
          { required: true, message: '请输入代理级别名称', trigger: "blur" }
        ]
      },
      hasFactory: false,
      activeTab: "gradeMan",
      activeIdx: 0,
      mesform: {
        PlanTemplateName: '',
        ReportTemplateName: '',
      },
      planloading: true,
      planFormInit: [],
      reportloading: true,
      reportFormInit: []
    };
  },
  mounted() {
    this.initList();
  },
  methods: {
    initList() {
      this.loading = true;
      factoryInfo(0).then(response => {
        this.form = response.data;
        this.hasFactory = this.form != null;
        this.loading = false;
      });

      this.initGradeList();

      let ismes = checkPermi(['/MES/']);
      if (ismes) {
        this.initMesConfig();
      }
    },
    async initMesConfig() {
      let response = await factoryMesConfig();
      // console.log("初始化配置信息",response);
      this.mesform.PlanTemplateName = response.data.PlanTemplateName;
      this.mesform.PlanTemplateId = response.data.PlanTemplateId;

      this.mesform.ReportTemplateName = response.data.ReportTemplateName;
      this.mesform.ReportTemplateId = response.data.ReportTemplateId;

      if (this.$isNotEmpty(response.data.PlanFlowInitJson)) {
        let planFormInit = JSON.parse(response.data.PlanFlowInitJson);
        await this.onInitPlanForm(this.mesform.PlanTemplateId, true, planFormInit);
      }
      if (this.$isNotEmpty(response.data.ReportFlowInitJson)) {
        let reportFormInit = JSON.parse(response.data.ReportFlowInitJson);
        await this.onInitReportForm(this.mesform.ReportTemplateId, true, reportFormInit);
      }
    },
    openFlowPicker(idx) {
      this.activeIdx = idx;
      this.$refs.flowPicker.OpenDialog();
    },
    onClearOut() {
      this.mesform.PlanTemplateName = "";
      this.mesform.PlanTemplateId = 0;
      this.$forceUpdate();
    },
    async onSelected(item) {
      if (this.activeIdx == 0) {
        this.mesform.PlanTemplateId = item.Id;
        this.mesform.PlanTemplateName = item.Name;
        await this.onInitPlanForm(this.mesform.PlanTemplateId);
      }
      else if (this.activeIdx == 1) {
        this.mesform.ReportTemplateId = item.Id;
        this.mesform.ReportTemplateName = item.Name;
        await this.onInitReportForm(this.mesform.ReportTemplateId);
      }
    },

    async onInitPlanForm(templateId, issetVal, setVal) {
      this.planloading = true;
      let rsp = await getFormDetail(templateId);
      let tformItems = JSON.parse(rsp.data.Form.FormFields);
      let newformItems = getItems(tformItems);
      this.planFormInit.length = 0;
      newformItems.map(element => {
        if (element.name == "TextInput" || element.name == "TextareaInput") {
          let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": "" }
          if (issetVal && setVal) {
            let rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) {
              obj.val = rowval.val
              obj.way = rowval.way
            }
          }
          this.planFormInit.push(obj);
        }
        else if (element.name == "DeptPicker") {
          this.planFormInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "发起人所属部门" });
        }
        else if (element.name == "UserPicker") {
          this.planFormInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "发起人" });
        }
      });
      this.planloading = false;
    },

    async onInitReportForm(templateId, issetVal, setVal) {
      this.reportloading = true;
      let rsp = await getFormDetail(templateId);
      let tformItems = JSON.parse(rsp.data.Form.FormFields);
      let newformItems = getItems(tformItems);
      this.reportFormInit.length = 0;
      newformItems.map(element => {
        if (element.name == "TextInput" || element.name == "TextareaInput") {
          let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": "" }
          if (issetVal && setVal) {
            let rowval = setVal.find(rw => rw.id == element.id)
            if (rowval) {
              obj.val = rowval.val
              obj.way = rowval.way
            }
          }
          this.reportFormInit.push(obj);
        }
        else if (element.name == "DeptPicker") {
          this.reportFormInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "发起人所属部门" });
        }
        else if (element.name == "UserPicker") {
          this.reportFormInit.push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "发起人" });
        }
      });
      this.reportloading = false;
    },
    onClearOutReport() {
      this.mesform.ReportTemplateName = "";
      this.mesform.ReportTemplateId = 0;
      this.$forceUpdate();
    },
    changeLoadVal(event, row, index) {
      //切换流程初始化默认值设置
      if (event == 1 && row.eltype == "TableList") {
        row.val = {}
      } else {
        row.val = ''
      }
    },
    initGradeList() {
      this.tbloading = true;
      gradeList().then(response => {
        this.tbList = response.data;
        this.tbloading = false;
        setTimeout(() => {
          this.rowInitDrop();
        }, 200)

      });
    },
    //行拖拽,排序方法
    rowInitDrop() {

      // 获取对象
      const tbody = document.querySelector(".airlineDeviceTable .el-table__body-wrapper tbody");
      const _this = this;
      Sortable.create(tbody, {
        animation: 50,
        draggable: ".el-table__row",
        ghostClass: "ghost",
        handle: ".handle",
        onEnd({ newIndex, oldIndex }) {
          if (newIndex == oldIndex) return;
          const currRow = _this.tbList.splice(oldIndex, 1)[0]
          _this.tbList.splice(newIndex, 0, currRow)
          let tarr = _this.tbList.map(x => x.Id);
          gradeSort(tarr).then(rsp => {
            _this.$modal.msgSuccess("保存成功");
          });
        }
      });
    },
    openAddGrade() {
      this.title = "添加代理级别";
      this.gradeform = {
        GradeName: ""
      };
      this.open = true;
    },
    openEditGrade(item) {
      this.title = "编辑代理级别";
      this.gradeform = {
        Id: item.Id,
        GradeName: item.GradeName
      };
      this.open = true;
    },
    checkPermission(perms) {
      return checkPermi(perms);
    },
    delGrade(item) {
      this.$modal
        .confirm('是否确认删除级别为"' + item.GradeName + '"的数据项？')
        .then(function () {
          return delGrade(item.Id);
        })
        .then(() => {
          this.initList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
    onSubmit() {
      setFactoryConfig(this.form).then(rsp => {
        this.$modal.msgSuccess("保存成功");
      }).catch(err => {
        this.$message.error(err);
      });
    },
    onGradeSubmit() {
      this.$refs["gradeform"].validate(valid => {
        if (valid) {
          if (this.gradeform.Id) {
            editGrade(this.gradeform)
              .then(rsp => {
                this.$modal.msgSuccess("修改成功");
                this.initGradeList();
                this.open = false;
              });
          } else {
            addGrade(this.gradeform)
              .then(rsp => {
                this.$modal.msgSuccess("添加成功");
                this.initGradeList();
                this.open = false;
              });
          }
        } else {
          return false;
        }
      });
    },
    onSaveMes() {
      if(this.mesform.PlanTemplateId){
        this.mesform.planFlowInitJson=JSON.stringify(this.planFormInit)
      }else{
        this.mesform.PlanTemplateId=''
        this.mesform.planFlowInitJson=''
      }
      if(this.mesform.ReportTemplateId){
        this.mesform.ReportFlowInitJson=JSON.stringify(this.reportFormInit)
      }else{
        this.mesform.ReportTemplateId=''
        this.mesform.ReportFlowInitJson=''
      }
      saveFactoryMesConfig(this.mesform).then(rsp => {
        this.$modal.msgSuccess("保存成功");
      });
    }
  }
};
</script>
<style lang="scss" scope>
.conf-container {
  padding: 20px;

  .confbox {
    background-color: #ffffff;
    border-radius: 5px;
    padding: 0px !important;
    font-size: var(--rightcon);

    .header {
      font-size: 16px;
      color: #333;
      background-color: rgb(249, 250, 252);
      padding: 0 15px;
      height: 48px;
      display: flex;
      justify-content: space-between;
      flex-direction: row;
      align-items: center;
    }

    .ghost {
      background-color: #e8f4ff;
    }

    .handle {
      cursor: move;
    }

  }
}


.box-row {
  .bx-hd {
    font-size: 16px;
    color: #333;
    background-color: rgb(249, 250, 252);
    padding: 0 15px;
    height: 48px;
    display: flex;
    justify-content: space-between;
    flex-direction: row;
    align-items: center;
  }

  .bx-bd {
    padding: 15px 0px;

    .el-input__suffix {
      display: flex;
      align-items: center;
    }
  }
}
</style>