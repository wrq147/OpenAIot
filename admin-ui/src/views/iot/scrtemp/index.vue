<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--产品数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="搜索关键词" prop="SearchKey">
                <el-input
                  class="set_radius"
                  v-model="queryParams.SearchKey"
                  placeholder="请输入关键字"
                  clearable
                  @keyup.enter.native="handleQuery"
                />
              </el-form-item>
              <el-form-item label="创建日期">
                <el-date-picker
                  class="set_radius"
                  v-model="dateRange"
                  style="width: 232px"
                  value-format="yyyy-MM-dd"
                  type="daterange"
                  range-separator="-"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                ></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button
                  type="primary"
                  icon="el-icon-search"
                  @click="handleQuery"
                  >搜索</el-button
                >
              </el-form-item>
            </el-form>
          </div>
          <div
            class="elbiaoge_elform"
            :style="{ 'min-height': tableConHeight + 'px' }"
          >
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="primary" plain @click="handleAdd">
                    <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                    <span style="margin-left: 6px">新增</span>
                  </el-button>
                </el-col>
              </div>
            </el-row>
            <el-row :gutter="10" justify="start">
              <el-col
                :span="8"
                v-for="item in iotScriptDataArr"
                :key="item.Id"
                style="margin-bottom: 20px"
              >
                <div class="script_li" @click="editScript(item)">
                  <div class="script_label">名称：{{ item.Name }}</div>
                  <div
                    class="index_tags_info"
                    v-if="item.IsSystem && item.IsSystem == '1'"
                  ></div>
                  <div>
                    <el-input
                      type="textarea"
                      :autosize="{ minRows: 14, maxRows: 14 }"
                      placeholder="请输入内容"
                      v-model="item.ScriptContent"
                      disabled
                      resize="none"
                      style="background: #ffffff"
                    >
                    </el-input>
                  </div>
                  <div class="remark_cot" v-if="item.Remark">
                    备注：{{ item.Remark }}
                  </div>
                  <div class="del_con" @click.stop="delectScript(item)" v-if="!item.IsSystem ||item.IsSystem && item.IsSystem != '1'">
                    <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                  </div>
                </div>
              </el-col>
            </el-row>

            <pagination
              v-show="total > 0"
              :total="total"
              :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize"
              :pageSizes="pageSizes"
              @pagination="loadIotScriptList"
            />
          </div>
        </el-col>
      </el-row>
      <el-dialog
        :title="dialogTitle"
        :close-on-click-modal="false"
        :visible.sync="scriptVisible"
        width="800px"
        append-to-body
        :destroy-on-close="true"
        :before-close="handleClose"
      >
        <el-form
          ref="scriptForm"
          :model="scriptForm"
          class="scriptForm"
          :rules="scriptRules"
          label-width="80px"
          label-position="top"
        >
          <el-row>
            <el-col :span="24">
              <el-form-item label="脚本名称" prop="name">
                <el-input
                  v-model="scriptForm.name"
                  placeholder="请输入脚本名称"
                  maxlength="20"
                  :disabled="activeScriptType == '1'"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="24">
                <dataAnalysis
                v-if="showScriptEditor"
                  ref="scriptcontent"
                  activeSelect="dataAnalysis"
                  :content="scriptForm.scriptContent"
                  :isdialog="true"
                  :options="{readOnly:activeScriptType == '1'}"
                ></dataAnalysis>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="24">
              <el-form-item label="物模型初始化json" prop="remark">
                <div class="daoru_con">
                  <div class="rulesjson" v-if="initModelTSLName">{{ initModelTSLName }}</div>
                  <el-tooltip class="item" effect="dark" content="导入的json格式需和开发产品一致" placement="top-start">
                    <el-button type="primary" class="putbutton">物模型初始化json<input type="file" @change="importProcess" id="putbuttonFile"/></el-button>
                  </el-tooltip>
                </div>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="24">
              <el-form-item label="备注说明" prop="remark">
                <el-input
                  type="textarea"
                  :rows="2"
                  placeholder="请输入备注说明"
                  v-model="scriptForm.remark"
                  :disabled="activeScriptType == '1'"
                ></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitScriptForm">确 定</el-button>
          <el-button @click="scriptVisible = false">取 消</el-button>
        </div>
      </el-dialog>
      <!-- 添加或修改参数配置对话框 -->
    </div>
  </div>
</template>

<script>
import {
  iotScriptList,
  removeIotScriptt,
  addIotScript,
  editIotScript,
} from "@/api/scrtemp.js";
import { resizeTableCon } from "@/mixins/resizeTableCon";
let dataAnalysis = () => import("@/views/iot/physicalModel/dataAnalysis.vue");
export default {
  name: "ScrtempIndex",
  mixins: [resizeTableCon],
  components: {
    dataAnalysis,
  },
  data() {
    return {
      pageSizes: [6, 12, 18, 24, 30],
      dialogTitle: "", //弹窗标题
      scriptVisible: false,
      queryParams: {
        pageNum: 1,
        pageSize: 6,
        SearchKey:''
      },
      // 日期范围
      dateRange: [],
      total: 0,
      iotScriptDataArr: [],
      scriptForm: {
        name: "",
        remark: "",
        scriptContent: "",
      },
      scriptRules: {
        name: [{ required: true, trigger: "blur", message: "请填写脚本名称" }]
        //自定义校验器
      }, //产品分类添加验证
      activeScriptType:'',
      showScriptEditor:false,
      initModelTSLName:'',//初始化物模型的json文件名称
    };
  },

  mounted() {
    this.loadIotScriptList();
  },

  methods: {
    processReadFile(file) {
      //读取导入参数的值
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          let jsonArr = JSON.parse(e.target.result);
          let infoObj={}
          if(Array.isArray(jsonArr)){
            infoObj=jsonArr[0]
          }else{
            infoObj=jsonArr
          }
          if(infoObj.ModelTSL){
            this.scriptForm.initModelTSL=infoObj.ModelTSL
          }else{
            this.scriptForm.initModelTSL=''
          }
        } catch (error) {
          this.scriptForm.initModelTSL=''
          // this.$store.commit("rulesloadForm", this.setup);
        }
      };
      reader.readAsText(file);
    },
    importProcess() {
      //导入参数
      const file = event.target.files[0];
      if (!file) {
        return;
      }
      if (file.name) {
        this.initModelTSLName = file.name;
      }
      this.processReadFile(file);
    },
    handleClose(){
      this.showScriptEditor=false
      this.scriptVisible = false
    },
    submitScriptForm() {
      this.scriptForm.scriptContent = this.$refs.scriptcontent.returnCode();
      this.$refs["scriptForm"].validate((valid) => {
        if (valid) {
          if (this.scriptForm.id) {
            editIotScript(this.scriptForm)
              .then((rsp) => {
                // console.log("编辑后返回值", rsp);
                if (rsp.code == 0) {
                  this.$modal.msgSuccess("修改成功");
                  this.handleQuery();
                  this.scriptVisible = false;
                }
              })
              .catch((err) => {
                this.$message.error(err);
              });
          } else {
            addIotScript(this.scriptForm)
              .then((rsp) => {
                console.log("添加后返回值", rsp);
                if (rsp.code == 0) {
                  this.$modal.msgSuccess("添加成功");
                  this.handleQuery();
                  this.scriptVisible = false;
                }
              })
              .catch((err) => {
                this.$message.error(err);
              });
          }
        }
      });
    },
    loadIotScriptList() {
      //获取脚本模板列表
      iotScriptList(this.addDateRange(this.queryParams, this.dateRange)).then(
        (res) => {
          // console.log("脚本列表", res);
          this.iotScriptDataArr = res.data.List;
          this.total = res.data.Total;
        }
      );
    },
    editScript(item) {
      this.resetForm("scriptForm");
      this.initModelTSLName=''
      this.scriptForm = {
        name: item.Name,
        remark: item.Remark,
        scriptContent: item.ScriptContent,
        initModelTSL:item.InitModelTSL,
        id: item.Id,
      };
      this.activeScriptType=item.IsSystem
      this.dialogTitle = "编辑脚本模板";
      this.showScriptEditor=true
      this.scriptVisible = true;
    },
    delectScript(item) {
      //删除脚本模板
      this.$modal
        .confirm('是否确认删除名称为"' + item.Name + '"的脚本模板？')
        .then(function () {
          return removeIotScriptt({ ids: item.Id });
        })
        .then(() => {
          this.handleQuery();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    handleAdd() {
      //添加脚本
      this.resetForm("scriptForm");
      this.activeScriptType=''
      this.initModelTSLName
      this.scriptVisible = true;
      this.showScriptEditor=true
      this.scriptForm = {
        name: "",
        remark: "",
        scriptContent: "",
        initModelTSL:'',
      };
      this.dialogTitle = "添加脚本模板";
    },
    handleQuery() {
      //
      this.queryParams.pageNum = 1;
      this.loadIotScriptList();
    },
    resetQuery() {
      //重置搜索
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
  },
};
</script>
<style lang="less" scope>
.daoru_con{
  .rulesjson {
    display: inline-block;
    margin-right: 20px;
    color: rgba(50, 150, 250, 0.71);
  }
  .putbutton {
    background: #1890ff;
    color: #fff;
    position: relative;
    span {
      color: #ffffff;
    }
    i {
      margin-right: 5px;
    }
    #putbuttonFile {
      position: absolute;
      left: 0;
      top: 0;
      width: 100%;
      height: 100%;
      opacity: 0;
      filter: alpha(opacity=0);
    }
  }
}
.script_li {
  background: #f5f7fa;
  padding: 10px;
  border-radius: 10px;
  position: relative;
  cursor: pointer;
  height: 390px;
  .index_tags_info {
    position: absolute;
    right: 0;
    top: 0;
    font-size: 12px;
    z-index: 10;
    color: #ffffff;
    width: 30px;
    // text-align: right;
    word-break: break-all;
  }
  .index_tags_info::before {
    content: "";
    position: absolute;
    top: 0;
    right: 0;
    border-width: 0 40px 40px 0;
    border-style: solid;
    border-color: transparent #409eff transparent transparent;
    z-index: 2;
    border-radius: 2px;
  }
  .index_tags_info::after {
    content: "系统";
    position: absolute;
    right: 0;
    top: 0;
    font-size: 12px;
    z-index: 3;
    color: #ffffff;
    line-height: 30px;
    text-align: center;
    transform: rotate(45deg);
    transform-origin: center center;
    color: #ffffff;
    font-size: 12px;
  }
  .script_label {
    color: #666666;
    font-size: 16px;
    margin: 0 0 10px 0;
  }
  .remark_cot {
    color: #666666;
    font-size: 12px;
    margin-top: 10px;
    width: calc(100% - 20px);
    display: block;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  .del_con {
    position: absolute;
    right: 10px;
    bottom: 15px;
    color: #999999;
    width: 16px;
    height: 16px;
  }
}
</style>
<style lang="less">
.scriptForm {
  .el-form-item {
    margin-bottom: 10px;
    .el-form-item__label {
      padding-bottom: 0;
    }
  }
}
.script_li {
  .el-textarea.is-disabled .el-textarea__inner {
    background: #ffffff;
  }
}
</style>