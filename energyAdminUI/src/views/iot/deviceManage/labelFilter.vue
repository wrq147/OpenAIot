<template>
  <el-dialog class="label_filter_dialog" title="标签筛选" :visible.sync="dialogVisible" width="650px" :before-close="handleClose">
    <div class="filter_con">
        <div class="filter_title">
            <span>筛选项</span>
            <i class="el-icon-refresh-right" @click="resetLabelFilter"></i>
        </div>
        <div class="filter_ul">
            <el-row :gutter="10" class="filter_li" v-for="(item,inx) in labelConditions" :key="'filterLabel'+inx">
                <el-col :span="2">
                    <div class="del_li" @click="delFilterLi(inx)">
                        <i class="el-icon-delete"></i>
                    </div>
                </el-col>
                <el-col :span="6">
                    <el-select filterable v-model="labelConditions[inx].code" placeholder="请选择标识符" style="width: 100%" @change="fieldSelectChange($event,inx)">
                        <el-option v-for="item in allLabelList" :key="item.code" :label="item.name" :value="item.code"></el-option>
                    </el-select>
                </el-col>
                <!-- <el-col :span="3">
                    <el-select filterable v-model="labelConditions[inx].optionType" placeholder="请选择数据类型" style="width: 100%">
                      <el-option v-for="item in typeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                    </el-select>
                </el-col> -->
                <el-col :span="5">
                    <el-select class="time_mini" placeholder="请选择比较符" v-model="labelConditions[inx].compare">
                        <template v-for=" it in compareList">
                            <el-option :label="it" :value="it" :key="it"></el-option>
                        </template>
                    </el-select>
                </el-col>
                <el-col :span="6" v-if="labelConditions[inx]&&labelConditions[inx].optionType=='int'||labelConditions[inx]&&labelConditions[inx].optionType=='float'">
                    <el-input type="number" v-model="labelConditions[inx].val" placeholder="请输入值"></el-input>
                </el-col>
                <el-col :span="8" v-else-if="labelConditions[inx]&&labelConditions[inx].optionType=='date'">
                    <el-date-picker style="width:100%" v-model="labelConditions[inx].val" type="datetime" placeholder="请选择值" value-format="yyyy-MM-dd HH:mm:ss" format="yyyy-MM-dd HH:mm:ss"></el-date-picker>
                </el-col>
                <el-col :span="6" v-else-if="labelConditions[inx]&&labelConditions[inx].optionType=='enum'">
                    <!-- <el-input type="text" v-model="labelConditions[inx].val" placeholder="请输入值"></el-input> -->
                    <el-select class="time_mini" placeholder="请选择" v-model="labelConditions[inx].val">
                        <template v-for=" it in enumListReturn(inx)">
                            <el-option :label="it.value" :value="it.key" :key="it.key"></el-option>
                        </template>
                    </el-select>
                </el-col>
                <el-col :span="6" v-else>
                    <el-input type="text" v-model="labelConditions[inx].val" placeholder="请输入值"></el-input>
                </el-col>
                <el-col :span="2">
                    <div class="add_li" @click="addFilterLi(inx)">
                        <i class="el-icon-plus"></i>
                    </div>
                </el-col>
            </el-row>
        </div>
    </div>
    <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="finishLabelFilter">确 定</el-button>
    </span>
  </el-dialog>
</template>

<script>
import {
    orgStyle,
} from "@/api/system/StyleMan";
export default {
  name: 'AdminUiLabelFilter',
  props:{

  },
  data() {
    return {
      typeList: [
        { alabel: "整型", label: "整型(Int)", value: "int" },
        { alabel: "浮点", label: "浮点型(Float)", value: "float" },
        { alabel: "字符", label: "字符型(String)", value: "string" },
        { alabel: "时间", label: "时间型(Date)", value: "date" },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean" },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum" },
        { alabel: "文件", label: "文件类型(File)", value: "file" },
        { alabel: "位置", label: "设备位置(Geo)", value: "geo" },
      ], //数据类型列表
      dialogVisible:false,
      labelConditions:[],
      allLabelList:[],//所有的标签
      compareList:['大于','小于','大于等于','小于等于','不等于','等于','包含','不包含'],
    };
  },

  mounted() {
    
  },

  methods: {
    resetLabelFilter(){
        this.$modal.confirm('是否确认清除所有标签过滤？').then( ()=>{
            this.labelConditions=[{
                code: "",
                optionType: "",
                compare: "",
                val: ""
            }]
            this.$modal.msgSuccess("清除成功");
        }).catch(() => { });
    },
    enumListReturn(inx){
        //返回枚举列表
        let find=this.allLabelList.find(row=>row.code==this.labelConditions[inx].code)
        if(find.elementsLis){
            return find.elementsLis
        }else{
            return []
        }
    },
    addFilterLi(inx){
        let obj={
                code: "",
                optionType: "",
                compare: "",
                val: ""
            }
        this.labelConditions.splice(inx+1,0,obj)
    },
    delFilterLi(inx){
        //删除
        this.labelConditions.splice(inx,1)
    },
    loadallLabelList(){
        //加载标签列表
        orgStyle({ orgId: this.$store.state.user.orgId }).then(response => {
            // console.log("主题详情",response);
            if(response.data&&response.data.StyleJson){
                let conditionLabel=(JSON.parse(response.data.StyleJson)).conditionLabel?(JSON.parse(response.data.StyleJson)).conditionLabel:[]
                this.allLabelList=JSON.parse(JSON.stringify(conditionLabel))
            }
            
        });
    },
    fieldSelectChange(val,inx){
      if(this.labelConditions[inx].code){
        let rowObj=this.allLabelList.find(row=>row.code==this.labelConditions[inx].code)
        this.labelConditions[inx].optionType=rowObj.type
        if(rowObj&&rowObj.type=='int'||rowObj.type=='date'||rowObj.type=='float'){
          this.compareList=['大于','小于','大于等于','小于等于','不等于','等于']
        }else{
          this.compareList=['等于','包含','不包含']
        }
      }
      this.$forceUpdate()
    },
    openFilter(TagConditions){
        this.loadallLabelList()
        if(TagConditions&&TagConditions.length>0){
            this.labelConditions=JSON.parse(JSON.stringify(TagConditions))
        }else{
            let obj={
                code: "",
                optionType: "",
                compare: "",
                val: ""
            }
            this.labelConditions=[]
            this.labelConditions.push(obj)
        }
        this.dialogVisible=true
    },
    handleClose(){
        //筛选关闭
        this.dialogVisible=false
    },
    finishLabelFilter(){
        let afterList=this.labelConditions.filter(row=>row.code)
        this.handleClose()
        this.$emit('finishLabelFilter',afterList)
    }
  },
};
</script>

<style lang="less" scoped>
.label_filter_dialog{
    ::v-deep .el-dialog__body{
        padding: 20px;
    }
}
.filter_con{
    .filter_title{
        font-size: 16px;
        color: #333333;
        i{
            margin-left: 10px;
        }
    }
    .filter_ul{
        .filter_li{
            margin-top: 10px;
            .add_li,.del_li{
                width: 100%;
                display: flex;
                justify-content: center;
                align-items: center;
                border: 1px solid #D9D9D9;
                background: #f5f5f5;
                width: 40px;
                height: 40px;
                margin-right: 10px;
                border-radius: 5px;
                
            }
        }
    }
}
</style>