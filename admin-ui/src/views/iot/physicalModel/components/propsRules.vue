<template>
  <div class="props-rules">
    <div style="margin-bottom: 15px;">
      <el-row class="button_row" :gutter="20">
        <el-col>
          <el-button type="primary" plain @click="openDialog(null)">添加规则</el-button>
        </el-col>
        <el-col :span="2">
          <el-button type="primary" plain @click="exportRow()">
            <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
            <span style="margin-left: 6px">导出</span>
          </el-button>
        </el-col>
        <el-col :span="2">
          <el-button type="primary" plain class="putbutton">
            <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
            <span style="margin-left: 6px">导入<input type="file" @change="importRow" id="putbuttonFile" /></span>
          </el-button>
        </el-col>
      </el-row>
    </div>

    <!-- 添加规则对话框 -->
    <el-dialog :visible.sync="dialogVisible" title="添加规则" width="680px">
      <el-form :model="ruleForm" label-width="80px" style="margin-left: 100px;">
        <el-form-item label="赋值属性">
          <el-select :disabled="ruleForm.Id != ''" placeholder="请选择关联属性" v-model="ruleForm.PropCode"
            style="width: 320px;">
            <el-option v-for="item in propList" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="统计时间">
          <el-select placeholder="请选择统计时间" v-model="ruleForm.WindowWay" style="width: 320px;">
            <el-option label="每时" :value="0"></el-option>
            <el-option label="每日" :value="1"></el-option>
            <el-option label="每月" :value="2"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="统计方式">
          <el-select placeholder="请选择统计方式" v-model="ruleForm.MergeWay" style="width: 320px;">
            <el-option label="最大值" value="max"></el-option>
            <el-option label="最小值" value="min"></el-option>
            <el-option label="平均值" value="mean"></el-option>
            <el-option label="合计值" value="sum"></el-option>
            <el-option label="期初值" value="first"></el-option>
            <el-option label="期末值" value="last"></el-option>
            <el-option label="区间值" value="range"></el-option>
            <el-option label="计数" value="count"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="统计属性">
          <el-select placeholder="请选择统计属性" v-model="ruleForm.MergeCode" style="width: 320px;">
            <el-option v-for="item in propList" :key="item.code" :label="item.name" :value="item.code"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm">保存</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 列表 -->
    <el-table :data="itemList"
      :header-cell-style="{ background: 'rgb(249, 250, 252)', color: 'rgb(120, 130, 157)', 'font-weight': 'normal' }"
      stripe>
      <el-table-column prop="PropName" label="赋值属性" align="center"></el-table-column>
      <el-table-column label="统计时间" align="center">
        <template slot-scope="scope">
          <span v-if="scope.row.WindowWay == 0">每时</span>
          <span v-else-if="scope.row.WindowWay == 1">每日</span>
          <span v-else-if="scope.row.WindowWay == 2">每月</span>
        </template>
      </el-table-column>
      <el-table-column label="统计方式" align="center">
        <template slot-scope="scope">
          <span v-if="scope.row.MergeWay == 'max'">最大值</span>
          <span v-else-if="scope.row.MergeWay == 'min'">最小值</span>
          <span v-else-if="scope.row.MergeWay == 'mean'">平均值</span>
          <span v-else-if="scope.row.MergeWay == 'sum'">合计值</span>
          <span v-else-if="scope.row.MergeWay == 'first'">期初值</span>
          <span v-else-if="scope.row.MergeWay == 'last'">期末值</span>
          <span v-else-if="scope.row.MergeWay == 'range'">区间值</span>
          <span v-else-if="scope.row.MergeWay == 'count'">计数</span>
        </template>
      </el-table-column>
      <el-table-column prop="MergePropName" label="统计属性" align="center"></el-table-column>
      <el-table-column label="操作" align="center" width="248" class-name="small-padding fixed-width">
        <template slot-scope="scope">
          <el-button type="text" icon="el-icon-edit" @click="openDialog(scope.row.Id)">修改</el-button>
          <el-button type="text" icon="el-icon-delete" @click="delRule(scope.$index)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import {
  getPropRuleList, getPropRuleInfo, addPropRule, editPropRule, removePropRule
} from "@/api/rules/productModel";

export default {
  name: "propsRules",
  props: {
    productInfos: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      itemList: [],
      propList: [],
      dialogVisible: false,
      ruleForm: {
        Id: '',
        ProductId: '',
        PropCode: '',
        WindowWay: 0,
        MergeWay: 'sum',
        MergeCode: '',
        Priority: 0
      }
    };
  },
  mounted() {
    this.fetchRuleList();
  },
  methods: {
    // 获取规则列表
    async fetchRuleList() {
      let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL);
      if (tmpmodeltsl != null && tmpmodeltsl.properties != null) {
        this.propList = tmpmodeltsl.properties.filter(x=>x.option.type=="float"||x.option.type=="int");
      }
      let res = await getPropRuleList(this.productInfos.Id);
      res.data.forEach(xitem => {
        let abpp = this.propList.find(x => x.code == xitem.PropCode);
        if (abpp != null) {
          xitem["PropName"] = abpp.name
        }
        else {
          xitem["PropName"] = "";
        }
        let merpp = this.propList.find(x => x.code == xitem.MergeCode);
        if (merpp != null) {
          xitem["MergePropName"] = merpp.name
        }
        else {
          xitem["MergePropName"] = "";
        }
      })
      this.itemList = res.data;
    },
    // 打开添加对话框
    async openDialog(oldId) {
      this.dialogVisible = true;
      if (oldId == null) {
        this.ruleForm = {
          Id: '',
          ProductId: this.productInfos.Id,
          PropCode: '',
          WindowWay: 0,
          MergeWay: 'sum',
          MergeCode: '',
          Priority: 0
        };
      }
      else {
        let rsp = await getPropRuleInfo(oldId);
        this.ruleForm = rsp.data;
      }

    },
    // 提交信息
    async submitForm() {
      if (this.ruleForm.Id == "") {
        if (this.ruleForm.PropCode == "") {
          this.$message.error('请选择关联属性');
          return;
        }
        if (this.ruleForm.MergeCode == "") {
          this.$message.error('请选择统计属性');
          return;
        }
        await addPropRule(this.ruleForm);
        this.$message.success("保存成功");
      }
      else {
        await editPropRule(this.ruleForm);
        this.$message.success("保存成功");
      }
      this.fetchRuleList();
      this.dialogVisible = false;
    },
    // 删除
    async delRule(idx) {
      this.$modal
        .confirm('是否确认删除统计规则"' + this.itemList[idx].PropName + '"？')
        .then(() => {
          return removePropRule(this.itemList[idx].Id);
        })
        .then(() => {
          this.fetchRuleList();
          this.$modal.msgSuccess("移除成功");
        })
        .catch((err) => {
          console.log("错误", err);
        });
    },
    exportRow() {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      tmploading.close();
      let newitems = this.itemList.map(x => {
        return {
          PropCode: x.PropCode,
          WindowWay: x.WindowWay,
          MergeWay: x.MergeWay,
          MergeCode: x.MergeCode,
          Priority: x.Priority
        };
      })
      const content = JSON.stringify(newitems);
      const blobData = new Blob([content], { type: "application/json" });
      const filename = `${this.productInfos.Name}-rule.json`; //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename);
      } else {
        const anchor = document.createElement("a");
        anchor.href = window.URL.createObjectURL(blobData);
        anchor.download = filename;
        anchor.click();
        window.URL.revokeObjectURL(blobData);
      }
    },
    importRow() {
      //导入功能
      const file = event.target.files[0];
      if (!file) {
        return;
      }

      const reader = new FileReader();
      reader.onload = async (e) => {
        try {
          let jsonArr = JSON.parse(e.target.result);
          for (let i = 0; i < jsonArr.length; i++) {
            jsonArr[i]["ProductId"] = this.productInfos.Id;
            await addPropRule(jsonArr[i]);
          }
          this.fetchRuleList();
        } catch (error) {
          console.error("Error parsing JSON", error);
        }
      };
      reader.readAsText(file);
    },
  },
};
</script>

<style scoped lang="scss">
.putbutton {
  background: #ecf5ff;
  color: #409eff;
  position: relative;
  border: 1px solid #B3D8FF;

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

  &::before {
    color: #409eff;
  }
}
</style>