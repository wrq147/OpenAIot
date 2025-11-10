<template>
    <el-dialog v-if="dialogFlag" title="生产计划详情" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
      width="1300px" top="2vh" @close="cancel">
      <el-descriptions class="margin-top" :column="3" size="medium" border>
            <el-descriptions-item>
                <template slot="label">计划名称</template>
                {{ ruleForm.planName }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">优先级</template>
                {{ ruleForm.priority === 1 ? '优先安排' : ruleForm.priority === 2 ? '加急处理' : '正常排产'  }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">超期时间</template>
                {{ ruleForm.overTime }}
            </el-descriptions-item>
        </el-descriptions>
        <DetailEmbed ref="nodelDet" curStyle="width:320px;margin:20px 20px;">
            <div class="wp-title">审批信息</div>
        </DetailEmbed>
      <el-table :data="ruleForm.items" border tooltip-effect="dark" style="width: 100%;">
        <el-table-column type="index" label="序号" align="center" width="50" />
        <el-table-column label="产品编号" prop="ProductId" align="center"></el-table-column>
        <el-table-column label="产品名称" prop="ProdInfo.ProductName" align="center"></el-table-column>
        <el-table-column label="规格" prop="ProdInfo.Specs" align="center"></el-table-column>
        <el-table-column label="单位" prop="ProdInfo.Unit" align="center" width="80"></el-table-column>
        <el-table-column label="产品属性" prop="ProdInfo.ProductFrom" align="center"></el-table-column>
        <el-table-column label="计划数量" prop="Quantity" align="center"></el-table-column>
        <el-table-column label="计划开始时间" prop="PlannedStartOn" align="center" width="200"></el-table-column>
        <el-table-column label="计划结束时间" prop="PlannedEndOn" align="center" width="200"></el-table-column>
      </el-table>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
      </span>
    </el-dialog>
</template>
<script>
  import { operInfo } from "@/api/mes/plan";
  import DetailEmbed from "@/views/flowable/task/record/DetailEmbed";
  export default {
    name: 'addPlan',
    components: {
        DetailEmbed
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
        // 表单
        ruleForm: {},
      }
    },
    watch: {
      dialogVisible(newValue) {
        this.dialogFlag = newValue;
      }
    },
    methods: {
        // 详情进来时获取子元素信息
        getDataInfo(id) {
            operInfo({ id }).then(res => {
                this.ruleForm.items = res.data.Items;
                this.$refs.nodelDet.initNodes(res.data.FlowId);
            })
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