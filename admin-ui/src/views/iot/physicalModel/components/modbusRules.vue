<template>
  <div>
    <el-drawer title="规则匹配" :visible="matchesDrawer" direction="rtl" @close="closeMatchesDrawer" size="900px" :destroy-on-close="true">
      <div style="padding-left: 20px; color: #97999c">匹配规则</div>
      <el-form ref="matchesForm" :model="matchesForm" :rules="matchesRules" label-position="top" :inline="true" class="matches_form">
        <el-form-item label="规则名称" prop="Name">
          <el-input v-model="matchesForm.Name" placeholder="请输入规则名称" />
        </el-form-item>
        <el-form-item label="从机地址" prop="SlaveId">
          <el-input-number
            v-model="matchesForm.SlaveId"
            controls-position="right"
            placeholder="请输入从机地址"
          ></el-input-number>
        </el-form-item>
        <el-form-item label="功能码" prop="FuncCode">
          <el-select v-model="matchesForm.FuncCode" style="width: 200px" placeholder="请输入功能码">
            <el-option
              v-for="item in FuncCodeList"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="起始地址" prop="StartAddress">
          <el-input-number
            v-model="matchesForm.StartAddress"
            controls-position="right"
            placeholder="请输入起始地址"
          ></el-input-number>
        </el-form-item>
      </el-form>
      <div style="padding-left: 20px; color: #97999c" class="matches_title">
        <span>数据读取规则</span>
        <div class="tongji_con">
          <div class="tongji_li" v-for="it in NumRegisterList" :key="it.label+'_sum_'+it.value">
            <span class="label">{{it.label}}寄存器：</span>
            <span class="num">{{countByteNum(it.value)}}个</span>
          </div>
        </div>
      </div>
      <template v-if="matchesForm.FuncCode != 5 && matchesForm.FuncCode != 6">
        <el-form v-for="(ite, inx) in matchesItemsForm" :key="inx" ref="matchesItemsForm" :model="ite" label-position="top" :inline="true" class="matches_form2">
          <el-form-item :label="inx == 0 ? '　　' : ''">
            <div class="xuhao" v-if="xuhaoArr">{{xuhaoArr[inx]}}</div>
          </el-form-item>
          <el-form-item :label="inx == 0 ? '字节序' : ''" prop="ByteOrder">
            <el-select v-model="ite.ByteOrder" placeholder="请选择字节序" style="width:160px" @change="byteOrderChange($event,inx)">
              <el-option v-for="item in ByteOrderList" :key="item.value" :label="item.label" :value="item.value" v-show="(item.value != 'CDAB' && item.value != 'BADC') || ite.NumRegister == '4'"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item :label="inx == 0 ? '数据长度' : ''" prop="NumRegister">
            <template>
              <el-select v-model="ite.NumRegister" style="width: 200px" placeholder="请选择数据长度">
                <el-option v-for="item in NumRegisterList" :key="item.value" :label="item.label" :value="item.value"></el-option>
              </el-select>
            </template>
          </el-form-item>
          <el-form-item :label="inx == 0 ? '对应的属性标识符' : ''" prop="PropertyCode">
            <div slot="label" v-if="inx == 0" class="rules_form_label">
              <span class="text">对应的属性标识符</span>
            </div>
            <el-select v-model="ite.PropertyCode" clearable placeholder="请选择对应的属性标识符" filterable>
              <el-option v-for="item in attrTableData" :key="item.code" :label="item.name" :value="item.code"></el-option>
            </el-select>
          </el-form-item>
          <el-form-item :label="inx == 0 ? '　　　' : ''">
            <el-button plain @click="delItemsFormList(ite, inx)">移除</el-button>
          </el-form-item>
          <el-form-item :label="inx == 0 ? '　　　' : ''">
            <el-button plain @click="appointAddItemsFormList(inx)">插入</el-button>
          </el-form-item>
        </el-form>
        <div style="margin-left: 20px; margin-top: 10px">
          <el-button plain @click="addItemsFormList">+添加</el-button>
        </div>
      </template>

      <div class="demo-drawer__footer" style="text-align: center; margin-top: 40p; padding-bottom: 20px">
        <el-button @click="closeMatchesDrawer">取 消</el-button>
        <el-button type="primary" @click="joinMatchesForm" :loading="matchesLoading">{{ matchesLoading ? "提交中 ..." : "保 存" }}</el-button>
      </div>
    </el-drawer>
  </div>
</template>

<script>
export default {
  name: "AdminUiModbusrules",
  props: {
    modbussForm: {
      type: Object,
      default: () => {
        return {};
      },
    },
    matchesForm: {
      type: Object,
      default: () => {
        return {};
      },
    },
    matchesItemsForm: {
      //规则
      type: Array,
      default: () => {
        return [];
      },
    },
    attrTableData: {
      //属性定义的列表
      type: Array,
      default: () => {
        return [];
      },
    },
    activeParamsLine: {
      type: Number,
      default: -1,
    },
    matchesDrawer: {
      type: Boolean,
      default: false,
    },
    activeAddParamsIndex: {
      type: Number,
      default: -1,
    },
  },
  data() {
    return {
      //功能码列表
      FuncCodeList: [
        { label: "01读线圈状态", value: 1 },
        { label: "02读离散输入状态", value: 2 },
        { label: "03读保持寄存器", value: 3 },
        { label: "04读输入寄存器", value: 4 },
      ],
      matchesLoading: false, //控制提交规则按钮是否可点击

      matchesRules: {
        Name: [{ required: true, trigger: "blur", message: "请输入规则名称" }],
        SlaveId: [
          { required: true, trigger: "blur", message: "请输入从机地址" },
        ],
        FuncCode: [
          { required: true, trigger: "blur", message: "请输入功能码" },
        ],
        StartAddress: [
          { required: true, trigger: "blur", message: "请输入起始地址" },
        ],
      },

      ByteOrderList: [
        { label: "大端", value: "H" },
        { label: "小端", value: "L" },
        { label: "字节", value: "C" },
        { label: "CDAB", value: "CDAB" },
        { label: "BADC", value: "BADC" },
      ],
      NumRegisterList: [
        { label: "1位", value: "b" },
        { label: "8位", value: "1" },
        { label: "16位", value: "2" },
        { label: "32位", value: "4" },
        { label: "64位", value: "8" },
      ],
      xuhaoArr:[]
    };
  },
  watch: {
    matchesItemsForm: {
      handler(val) {
          let mapObj = {};
          this.xuhaoArr=[]
          // console.log(this.matchesItemsForm,'this.matchesItemsForm');
          for (let i = 0; i < this.matchesItemsForm.length; i++) {
            let xunum=0
            if(i==0){
              xunum=Number(this.matchesForm.StartAddress)
            }else{
              if(this.matchesItemsForm[i-1]&&this.matchesItemsForm[i-1].NumRegister){
                let curreg = this.matchesItemsForm[i - 1].NumRegister;
                if (curreg == 'b') {
                  xunum = this.xuhaoArr[i - 1] + 0.0625;
                }
                else {
                  xunum = this.xuhaoArr[i - 1] + (Number(curreg) / 2);
                }
              }
            }
            this.xuhaoArr.push(xunum)
          }
          // console.log("this.config.Maping",this.xuhaoArr);
      },
      deep: true,
      immediate: true,
    },
  },
  mounted() {},

  methods: {
    countByteNum(type){
      let filter=this.matchesItemsForm.filter(rw=>rw.NumRegister==type)
      return filter.length
    },
    byteOrderChange(value,inx){
      if(value=='H'||value=='L'){
        this.matchesItemsForm[inx].NumRegister="2"
      }
    },
    delItemsFormList(row, indexRow) {
      //删除提取规则增加
      this.matchesItemsForm.splice(indexRow, 1);
    },
    appointAddItemsFormList(inx){
      this.matchesItemsForm.splice(inx,0,{
        ByteOrder: "H", //字节序
        NumRegister: "2", //数据长度
        PropertyCode: "", //对应的属性标识符
      });
    },
    addItemsFormList() {
      //数据提取规则增加
      this.matchesItemsForm.push({
        ByteOrder: "H", //字节序
        NumRegister: "2", //数据长度
        PropertyCode: "", //对应的属性标识符
      });
    },
    joinMatchesForm() {
      //规则匹配验证提交
      this.$refs["matchesForm"].validate((val1) => {
        let succNum = 0;
        for (let i = 0; i < this.matchesItemsForm.length; i++) {
          succNum += 1;
          if (succNum == this.matchesItemsForm.length &&i == this.matchesItemsForm.length - 1) {
            if (this.activeParamsLine > -1) {
              this.matchesForm.Items = JSON.parse(JSON.stringify(this.matchesItemsForm));
              let maFrom = JSON.parse(JSON.stringify(this.matchesForm));
              this.modbussForm.Matches[this.activeParamsLine] = maFrom;
            } else {
              this.matchesForm.Items = JSON.parse(JSON.stringify(this.matchesItemsForm));
              let maFrom = JSON.parse(JSON.stringify(this.matchesForm));
              if(this.activeAddParamsIndex>-1){
                this.modbussForm.Matches.splice(this.activeAddParamsIndex + 1, 0, maFrom);
              }else{
                // console.log(maFrom,'hshhshsyst');
                this.modbussForm.Matches.push(maFrom);
              }
              
            }
            // this.saveSetData();
            this.$emit("saveSetDataFun", this.modbussForm);
            this.closeMatchesDrawer();
          }
        }
      });
    },
    closeMatchesDrawer() {
      //关闭匹配规则填写弹窗
      this.$emit("closeMatchesDrawer");
    },
  },
};
</script>

<style lang="less">
.matches_form {
  padding: 0 20px 20px;
  margin-bottom: -20px;
}

.matches_form2 {
  padding-left: 20px;
  padding-top: 10px;
  margin-bottom: -20px;
}
.matches_form2 .el-form-item .el-form-item__label{
    width: 100%;
}
.rules_form_label{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  flex: 1;
  .text{

  }
  .btn{
    width: 80px;
    height: 30px;
    display: flex;
    justify-content: center;
    align-items: center;
    background: #F6F9FF;
    border-radius: 4px;
    color: #3572FF;
    font-size: 12px;
    line-height: 30px;
  }
}
.matches_form {
  .el-form-item.is-required:not(.is-no-asterisk) > .el-form-item__label:before,
  .el-form-item.is-required:not(.is-no-asterisk)
    .el-form-item__label-wrap
    > .el-form-item__label:before {
    content: "";
  }
  .el-form-item .el-form-item__label {
    padding: 0;
  }
}
.matches_form2 {
  .xuhao {
    width: 32px;
    height: 32px;
    border-radius: 16px;
    border: 1px solid #606266;
    display: flex;
    justify-content: center;
    align-items: center;
    color: #606266;
    font-size: 14px;
    margin-top: 3px;
  }
  .el-form-item.is-required:not(.is-no-asterisk) > .el-form-item__label:before,
  .el-form-item.is-required:not(.is-no-asterisk)
    .el-form-item__label-wrap
    > .el-form-item__label:before {
    content: "";
  }
  .el-form-item .el-form-item__label {
    padding: 0;
  }
}
</style>
<style lang="less" scoped>
.matches_title{
  display: flex;
  align-items: center;
  .tongji_con{
    margin-left: 20px;
    display: flex;
    align-items: center;
    .tongji_li{
      margin-right: 10px;
    }
  }
}

</style>