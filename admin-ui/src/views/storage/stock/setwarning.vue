<template>
  <el-dialog title="批量设置库存预警" :visible.sync="warnOpen" center width="600px" :close-on-click-modal="false" :destroy-on-close="true">
      <div v-loading="loading" class="warn_con">
        <div class="label">选中库存</div>
        <div class="form_item">
            <el-radio-group v-model="selectType" @input="changeselectType">
                <el-radio :label="1">只修改选中库存({{selectStock.length}})</el-radio>
                <el-radio :label="2">修改当前查询条件下所有库存({{allStock.length}})</el-radio>
            </el-radio-group>
        </div>
        <div class="label">
            <span>设置预警值</span>
        </div>
        <div class="form_item">
            <el-table :data="stockWarnOptions" tooltip-effect="dark" style="width: 100%" v-loading="warnLoading">
                <el-table-column prop="Name" align="left" label="库存名称"></el-table-column>
                <el-table-column label="库存下限" align="center" width="120">
                    <template slot-scope="scope">
                        <el-input type="number" placeholder="请输入库存下限" v-model="scope.row.MinNum" :min="-1" :step="1"></el-input>
                    </template>
                </el-table-column>
                <el-table-column label="库存上线" align="center" width="120">
                    <template slot-scope="scope">
                        <el-input type="number" placeholder="请输入库存上线" v-model="scope.row.MaxNum" :min="-1" :step="1"></el-input>
                    </template>
                </el-table-column>
            </el-table>
        </div>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="warnOpen = false">取 消</el-button>
        <el-button type="primary" @click="finishSetWarning" v-loading="submitLoading" :disabled="submitLoading">确定</el-button>
      </div>
   </el-dialog>
</template>

<script>
import {
    setPileWarn
} from "@/api/storage/stock";
export default {
  name: 'stockSetwarning',
  props:{
    allStock:{
        type:Array,
        default:()=>{
            return []
        }
    },
    selectStock:{
        type:Array,
        default:()=>{
            return []
        }
    },
  },
  data() {
    return {
        submitLoading:false,
        warnOpen:false,
        loading:false,
        selectType:1,//选择的方式，1表示只修改选中库存，修改当前查询条件下所有库存
        warnLoading:false,
        stockWarnOptions:[]
    };
  },

  mounted() {
    
  },

  methods: {
    finishSetWarning(){
        //完成预警值设置
        this.submitLoading=true
        let subForm=[]
        subForm=this.stockWarnOptions.map(row=>{
            let obj={
                houseId:row.HouseId,
                targetType:row.TargetType,
                targetId:row.TargetId,
                minNum:Number(row.MinNum),
                maxNum:Number(row.MaxNum)
            }
            return obj
        })
        setPileWarn(subForm).then(res=>{
            this.$modal.msgSuccess("设置成功");
            this.warnOpen=false
        }).catch(err=>{
            console.log("报错");
            this.submitLoading=false
        })
    },
    changeselectType(val){
        //
        console.log("类型值发生改变",val);
        this.warnLoading=true
        if(val==1){
            this.stockWarnOptions=JSON.parse(JSON.stringify(this.selectStock))
        }else if(val==2){
            this.stockWarnOptions=JSON.parse(JSON.stringify(this.allStock))
        }
        this.$forceUpdate()
        this.warnLoading=false
    },
    setWarnOption(val){
        this.stockWarnOptions=JSON.parse(JSON.stringify(val))
        this.selectType=1
        this.submitLoading=false
        this.$forceUpdate()
        this.warnOpen=true
    }
  },
};
</script>
<style lang="less" scoped>
.warn_con{
    .label{
        color: #333333;
        font-size: 16px;
        font-weight: bold;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
    .form_item{
        padding: 10px 0;
    }
}
</style>