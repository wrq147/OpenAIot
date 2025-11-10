<template>
  <div class="box-row">
    <div class="bx-hd">{{ flowTitle }}</div>
    <div class="bx-bd">
      <el-table border v-loading="tbloading" :data="formInit" row-key="id" style="width:100%">
        <el-table-column prop="title" label="表单字段" align="center" width="300"></el-table-column>
        <el-table-column label="值类型" align="center" width="120">
          <template slot-scope="scope">
            <el-select v-model="scope.row.way" placeholder="请选择"
              @change="changeLoadVal($event, scope.row, scope.$index)">
              <el-option label="自定义" :value="0"
                v-if="scope.row.eltype != 'DevicPicker' && scope.row.eltype != 'UserPicker'"></el-option>
              <el-option label="系统值" :value="1" v-if="scope.row.eltype != 'TableList'"></el-option>
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="初始值" align="center">
          <template v-slot:default="scope">
            <template v-if="scope.row.eltype != 'TableList'">
              <el-input v-if="scope.row.way == 0" v-model="scope.row.val" placeholder="请输入内容"></el-input>
              <el-select v-else v-model="scope.row.val" placeholder="请选择">
                <el-option label="申请单编号" value="申请单编号"
                  v-if="index == 2 && (scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput')"></el-option>
                <el-option label="出库方式" value="出库方式"
                  v-if="index == 1 && (scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput')"></el-option>
                <el-option label="领用人员" value="领用人员" v-if="index == 1 && (scope.row.eltype == 'UserPicker')"></el-option>
                <el-option label="入库方式" value="入库方式"
                  v-if="index == 0 && (scope.row.eltype == 'TextInput' || scope.row.eltype == 'TextareaInput')"></el-option>
              </el-select>
            </template>
            <template v-else>
              <TableList :valueModel="{}" v-model="scope.row.val" mode="mode" v-bind="scope.row.props"></TableList>
            </template>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import TableList from '@/views/flowable/common/form/components/TableList.vue'
export default {
  name: 'AdminUiHouseFlow',
  props: ['formInitVal', 'tbloading', 'flowTitle', 'index'],
  components: { TableList },
  data() {
    return {
      formInit: this.formInitVal
    };
  },
  watch: {
    formInit: {
      deep: true,
      handler(newVal) {
        // console.log(newVal,'newValnewVal');
        this.$emit('setformInitData', newVal, this.index)
      },
    },
    formInitVal: {
      deep: true,
      handler(newVal) {
        this.formInit = JSON.parse(JSON.stringify(newVal))
      },
    },
  },
  mounted() {

  },

  methods: {
    changeLoadVal(event, row, index) {
      //切换流程初始化默认值设置
      if (event == 1 && row.eltype == "TableList") {
        this.formInit[index].val = {}
      } else {
        this.formInit[index].val = ''
      }
    },
  },
};
</script>
<style lang="less" scoped>
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