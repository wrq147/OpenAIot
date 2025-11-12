<template>
  <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
    width="1100px" top="2vh" @close="cancel">
    <el-descriptions class="margin-top" title="父项物料信息" :column="2" :size="'medium'" border>
      <template slot="extra" v-if="!isReadonly">
        <el-button type="primary" @click="openDeviceDialog(true)">父项物料</el-button>
      </template>
      <el-descriptions-item>
        <template slot="label">父项物料编号</template>{{ ruleForm.SkuNumber }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">父项物料名称</template>{{ ruleForm.productName }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">规格型号</template>{{ ruleForm.specs }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">物料属性</template>{{ ruleForm.productFrom }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">单位</template>{{ ruleForm.unit }}
      </el-descriptions-item>
    </el-descriptions>
    <div style="margin: 10px 0" v-if="!isReadonly">
      <el-button type="primary" icon="el-icon-plus" plain @click="addProductList">子项物料</el-button>
    </div>
    <div style="margin: 30px 0" v-if="isReadonly"></div>
    <el-table :data="ruleForm.items" border tooltip-effect="dark" style="width: 100%">
      <el-table-column type="index" label="序号" align="center" width="50" />
      <el-table-column align="center" width="240">
        <template #header>
          <span>
            <span style="color: #f56c6c;">*</span>子项物料名称
          </span>
        </template>
        <template slot-scope="scope">
          <div style="display: flex;" v-if="!isReadonly">
            <el-input v-model="scope.row.ProductName" :disabled="true" placeholder="请选择物料" />
            <el-button type="primary" size="mini" @click="openDeviceDialogChild(false, scope.$index)">选择物料</el-button>
          </div>
          <div v-else>
            <span>{{scope.row.ProductName}}</span>
          </div>
        </template>
      </el-table-column>
      <!-- <el-table-column label="关联工序" align="center">
        <template slot-scope="scope">
          <div style="display: flex;">
            <el-select v-model="scope.row.ProcessStepId" placeholder="请选择工序">
              <el-option v-for="(item, index) in operListData" :key="index" :label="item.OperName" :value="item.Id" />
            </el-select>
          </div>
        </template>
      </el-table-column> -->
      <el-table-column label="规格" prop="Specs" align="center"></el-table-column>
      <el-table-column label="单位" prop="Unit" align="center"></el-table-column>
      <!-- <el-table-column label="产品属性" prop="ProductFrom" align="center"></el-table-column> -->
      <el-table-column label="数量" align="center">
        <template slot-scope="scope">
          <div style="display: flex;" v-if="!isReadonly">
            <el-input type="Number" v-model="ruleForm.items[scope.$index].Quantity" placeholder="请输入数量" />
          </div>
          <div v-else>
            <span>{{ruleForm.items[scope.$index].Quantity}}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="备注" align="center">
        <template slot-scope="scope">
          <div style="display: flex;" v-if="!isReadonly">
            <el-input v-model="ruleForm.items[scope.$index].Remark" placeholder="请输入备注" />
          </div>
          <div v-else>
            <span>{{ruleForm.items[scope.$index].Remark}}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" v-if="!isReadonly">
        <template slot-scope="scope">
          <div>
            <el-button type="text" icon="el-icon-delete" style="color:red"
              @click="delProductList(scope.$index)">删除</el-button>
          </div>
        </template>
      </el-table-column>
    </el-table>
    <span slot="footer" class="dialog-footer">
      <el-button @click="cancel" v-if="!isReadonly">取消</el-button>
      <el-button type="primary" @click="submitForm('ruleForm')" v-if="!isReadonly">确定</el-button>
      <el-button @click="cancel" v-if="isReadonly">关闭</el-button>
    </span>
    <slectProductList ref="slectProductList" :dialog-visible="deviceOpen" @cancelForm="cancelForm"
      @productSelect="productSelect" />
  </el-dialog>
</template>
<script>
// import { operList } from "@/api/mes/oper";
import { factoryProductInfo } from "@/api/factory/product";
import { bomAdd, bomEdit } from "@/api/mes/bom";
import slectProductList from './slectProductList.vue'
export default {
  name: 'addDefect',
  components: {
    slectProductList
  },
  props: {
    dialogVisible: {
      type: Boolean
    },
    title: {
      type: String
    }
  },
  data() {
    return {
      dialogFlag: false,
      deviceOpen: false,
      // 表单
      ruleForm: {},
      operListData: [],
      childrenIndex: '',
      // 存储每行的错误信息
      rowErrors: [],
      isReadonly:false,
    }
  },
  watch: {
    dialogVisible(newValue) {
      this.dialogFlag = newValue;
      // operList().then(res => {
      //   this.operListData = res.data.List
      // })
    }
  },
  methods: {
    // 获取父产品详情
    getProductInfo(id) {
      factoryProductInfo({ id: id }).then(res => {
        console.log(res,'resres');
        this.ruleForm.productFrom = res.data.ProductFrom;
        this.ruleForm.productName = res.data.ProductName;
        this.ruleForm.specs = res.data.Specs;
        this.ruleForm.quantity = res.data.Quantity;
        this.ruleForm.unit = res.data.Unit;
        this.ruleForm.SkuNumber=res.data.SkuNumber
      })
    },

    // 提交新增/修改按钮
    submitForm() {
      if (this.ruleForm.productId === '') {
        this.$message.warning('请先选择父项物料!')
        return false
      }
      if (!this.validateAllRows()) {
        this.$message.error('请完善表格中的必填项');
        return false;
      }
      if (this.title === '新增物料清单') {
        this.getAddRpt()
      } else {
        this.setEditRpt()
      }
    },

    getAddRpt() {
      bomAdd(this.ruleForm).then(res => {
        this.$message.success('添加成功!')
        this.$emit('getList')
      })
    },

    setEditRpt() {
      bomEdit(this.ruleForm).then(res => {
        this.$message.success('修改成功!')
        this.$emit('getList')
      })
    },

    // 添加子项物料
    addProductList() {
      if (this.ruleForm.productId === '') {
        this.$message.warning('请先选择父项物料!')
        return false
      }
      this.ruleForm.items.push({
        id: "",
        ParentProductId: this.ruleForm.productId,
        ProductId: "",
        ProductFrom: '',
        ProductName: '',
        Specs: '',
        OrgId: this.$store.state.user.orgId,
        Quantity: 1,
        Unit:'',
        ProcessStepId: "",
        Remark: "",
        Sort:this.ruleForm.items.length+1
      })
    },

    // 删除子项物料
    delProductList(index) {
      if (index >= 0 && index < this.ruleForm.items.length) {
        this.ruleForm.items.splice(index, 1);
      }
    },

    // 父物料点击弹窗
    openDeviceDialog(type) {
      this.$refs['slectProductList'].deviceList = [];
      this.$refs['slectProductList'].isFather = type;
      this.$refs['slectProductList'].deviceQuery.NoId = null;
      this.deviceOpen = true;
    },

    // 子物料点击弹窗
    openDeviceDialogChild(type, index) {
      this.$refs['slectProductList'].deviceList = [];
      this.$refs['slectProductList'].isFather = type;
      this.$refs['slectProductList'].deviceQuery.NoId = this.ruleForm.productId;
      this.childrenIndex = index;
      this.deviceOpen = true;
    },

    cancelForm() {
      this.deviceOpen = false;
    },

    // 选择的产品
    productSelect(data, isFather) {
      if (isFather) {
        this.ruleForm.productId = data.Id;
        this.ruleForm.productName = data.ProductName;
        this.ruleForm.productFrom = data.ProductFrom;
        this.ruleForm.specs = data.Specs;
        this.ruleForm.unit = data.Unit;
        this.ruleForm.SkuNumber=data.SkuNumber
      } else {
        this.ruleForm.items[this.childrenIndex].Sort=this.childrenIndex
        this.ruleForm.items[this.childrenIndex].ProductId = data.Id;
        this.ruleForm.items[this.childrenIndex].ProductName = data.ProductName;
        this.ruleForm.items[this.childrenIndex].Specs = data.Specs;
        this.ruleForm.items[this.childrenIndex].Quantity = 1;
        this.ruleForm.items[this.childrenIndex].Unit = data.Unit;
        this.ruleForm.items[this.childrenIndex].ProductFrom = data.ProductFrom;
      }
      let items=JSON.parse(JSON.stringify(this.ruleForm.items))
      this.ruleForm.items=JSON.parse(JSON.stringify(items))
    },

    // 验证单行
    validateRow(index) {
      const row = this.ruleForm.items[index];
      const errors = {
        ProductName: !row.ProductName,
      };

      this.rowErrors[index] = errors;

      // 返回是否验证通过
      return !errors.ProductName;
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
  .el-cascader {
    width: 100%;
  }
}

.addPeople>.box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
}

.addPeople>.btn {
  width: 100%;
  justify-content: flex-end;
  display: flex;
  align-items: center;
}
</style>