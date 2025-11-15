<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
      width="1300px" top="2vh" @close="cancel">
        <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
            <el-row>
                <el-col :span="12">
                    <el-form-item label="计划名称" prop="planName">
                        <el-input v-model="ruleForm.planName" placeholder="请输入工艺路线名称" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="优先级" prop="priority">
                        <el-select v-model="ruleForm.priority" placeholder="请选择优先级">
                            <el-option label="优先安排" :value="1" />
                            <el-option label="加急处理" :value="2" />
                            <el-option label="正常排产" :value="3" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="超期时间" prop="overTime">
                        <el-date-picker v-model="ruleForm.overTime" format="yyyy-MM-dd HH:mm" value-format="yyyy-MM-dd HH:mm" type="datetime" placeholder="选择超期时间" />
                    </el-form-item>
                </el-col>
                <el-col v-show="PlanTemplateId !== ''" :span="24">
                    <AddEmbed ref="flowForm">
                        <div class="flow-title" style="font-size: 16px; color: #333">生产计划信息</div>
                    </AddEmbed>
                </el-col>
            </el-row>
        </el-form>
      <div style="margin: 10px 0">
        <el-button type="primary" icon="el-icon-plus" plain @click="deviceOpen = true;">添加产品</el-button>
      </div>
      <el-table :data="ruleForm.items" border tooltip-effect="dark" style="width: 100%">
        <el-table-column type="index" label="序号" align="center" width="50" />
        <el-table-column label="产品编号" prop="ProductId" align="center"></el-table-column>
        <el-table-column label="产品名称" prop="ProductName" align="center"></el-table-column>
        <el-table-column label="规格" prop="Specs" align="center"></el-table-column>
        <el-table-column label="单位" prop="Unit" align="center" width="80"></el-table-column>
        <el-table-column label="产品属性" prop="ProductFrom" align="center"></el-table-column>
        <el-table-column align="center">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>计划数量
            </span>
          </template>
          <template slot-scope="scope">
            <el-input v-model="scope.row.Quantity" type="number" placeholder="请输入计划数量" />
          </template>
        </el-table-column>
        <el-table-column align="center" width="200">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>计划开始时间
            </span>
          </template>
          <template slot-scope="scope">
            <el-date-picker v-model="scope.row.PlannedStartOn" format="yyyy-MM-dd HH:mm" value-format="yyyy-MM-dd HH:mm" type="datetime" placeholder="选择计划开始时间" />
          </template>
        </el-table-column>
        <el-table-column align="center" width="200">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>计划结束时间
            </span>
          </template>
          <template slot-scope="scope">
            <el-date-picker v-model="scope.row.PlannedEndOn" format="yyyy-MM-dd HH:mm" value-format="yyyy-MM-dd HH:mm" type="datetime" placeholder="选择计划结束时间" />
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="100">
          <template slot-scope="scope">
            <div>
              <el-button type="text" icon="el-icon-delete" style="color:red" @click="delProductList(scope.$index)">删除</el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm', 1)">保存</el-button>
        <el-button plain type="primary" @click="submitForm('ruleForm', 2)">提交</el-button>
      </span>
      <slectProductList ref="slectProductList" :dialog-visible="deviceOpen" @cancelForm="cancelForm" @productSelect="productSelect" />
    </el-dialog>
</template>
<script>
  import { operAdd, operEdit, planeFormData, planeSubmitModel, operInfo } from "@/api/mes/plan";
  import { factoryMesConfig } from "@/api/mes/config";
  import slectProductList from './slectProductList.vue'
  import AddEmbed from "@/views/flowable/task/record/AddEmbed";
  export default {
    name: 'addPlan',
    components: {
      slectProductList,
      AddEmbed
    },
    props: {
      dialogVisible: {
        type: Boolean
      },
      title: {
        type: String
      },
      routeList: {
        type: Array,
        default: () => []
      }
    },
    data() {
      return {
        dialogFlag: false,
        deviceOpen: false,
        // 表单
        ruleForm: {},
        // 校验
        rules: {
            planName: [
                { required: true, message: "请输入计划名称", trigger: "blur" },
            ],
            priority: [
                { required: true, message: "请选择优先级", trigger: "change" },
            ],
            overTime: [
                { required: true, message: "请选择超期时间", trigger: "change" },
            ]
        },
        PlanTemplateId: '',
        // 存储每行的错误信息
        rowErrors: []
      }
    },
    watch: {
      dialogVisible(newValue) {
        this.dialogFlag = newValue;
        if (newValue) {
            factoryMesConfig().then(res => {
                this.PlanTemplateId = res.data.PlanTemplateId;
                this.ruleForm.flowId = res.data.PlanTemplateId;
                planeFormData({ number: this.PlanTemplateId }).then(repont => {
                    this.$refs.flowForm.InitData(
                        this.PlanTemplateId,{
                        "@from": this.ruleForm.number,
                        "@fromtype": "生产计划",
                    }, 
                    this.ruleForm.id === '' ? null : this.ruleForm.number,
                    repont.data
                   )
                })
            });
        }
      }
    },
    methods: {
        // 编辑进来时获取子元素信息
        getDataInfo(id) {
            operInfo({ id }).then(res => {
                this.ruleForm.items = res.data.Items;
            })
        },
      // 提交新增/修改按钮
      submitForm(formName, st) {
        this.$refs[formName].validate(async (valid) => {
          if (valid) {
            // 验证表格行
            if (!this.validateAllRows()) {
              this.$message.error('请完善表格中的必填项');
              return false;
            }
            let response;
            if (this.title === '新增生产计划') {
                response = await operAdd(this.ruleForm)
            } else {
                response = await operEdit(this.ruleForm)
            }
            if (this.PlanTemplateId !== '') {
                if (st === 2) {
                    let tmpmodel = this.$refs.flowForm.getModel();
                    tmpmodel["id"] = this.ruleForm.id === '' ? response.data : this.ruleForm.id;
                    await planeSubmitModel(tmpmodel);
                } else {
                    this.$refs.flowForm.submitForm(st);
                }
            }
            else{
                await planeSubmitModel({ id: response.data });
            }
            this.$emit('getList');
          } else {
            return false
          }
        })
      },

      // 添加子项
      productSelect(data) {
        console.log(data)
        this.ruleForm.items.push({
            ProductId: data.Id,
            ProductName: data.ProductName,
            Specs: data.Specs,
            Unit: data.Unit,
            ProductFrom: data.ProductFrom,
            Quantity: '',
            PlannedStartOn: '',
            PlannedEndOn: '',
        })
      },

      // 删除子项
      delProductList(index){
        if (index >= 0 && index < this.ruleForm.items.length) {
          this.ruleForm.items.splice(index, 1);
        }
      },

      // 验证单行
      validateRow(index) {
        const row = this.ruleForm.items[index];
        const errors = {
            Quantity: !row.Quantity,
            PlannedStartOn: !row.PlannedStartOn && row.PlannedStartOn !== '',
            PlannedEndOn: !row.PlannedEndOn && row.PlannedEndOn !== ''
        };
        
        this.rowErrors[index] = errors;
        
        // 返回是否验证通过
        return !errors.Quantity && !errors.PlannedStartOn && !errors.PlannedEndOn;
      },

      // 验证所有行
      validateAllRows() {
        let isValid = true;
        
        this.ruleForm.items.forEach((_, index) => {
          if (!this.validateRow(index)) {
            isValid = false;
          }
        });
        
        return isValid;
      },

      cancelForm() {
        this.deviceOpen = false;
      },

      cancel() {
        this.$emit('cancelForm')
      }
    }
  }
</script>
  
<style lang="scss" scoped>
  ::v-deep {
    .el-dialog__header {
      border-bottom: 1px solid #ccc;
    }
    .el-select,
    .el-date-editor,
    .el-cascader {
      width: 100% !important;
    }
  }
  </style>