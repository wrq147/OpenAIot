<template>
  <div>
    <el-form label-position="top" label-width="90px">
      <el-form-item label="1、执行动作">
        <el-select v-model="config.action" placeholder="请选择">
          <el-option label="修改已有数据" value="Update">
          </el-option>
          <el-option label="删除已有数据" value="Delete">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="2、目标表单">
        <el-select v-model="config.targetform" @change="chgTarget" placeholder="请选择">
          <el-option
            v-for="item in formlist"
            :key="item.code"
            :label="item.name"
            :value="item.code">
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="3、为目标表单添加过滤条件" v-if="SelectedTable!=null">
        <div>
          <div style="margin-bottom: 15px;" v-if="ConditionFields.length>0">
            <el-button @click="onCondiAddClick" type="primary" icon="el-icon-plus" size="small" plain>添加过滤条件</el-button>
          </div>
          <el-row v-for="(item,idx) in ConditionList" :key="idx" style="margin-bottom: 10px;">
            <el-col :span="6">
              <el-select v-model="item.TargetField" placeholder="请选择过虑字段">
                <el-option
                  v-for="condi in ConditionFields" :key="condi.code"
                  :label="condi.name"
                  :value="condi.code">
                </el-option>
              </el-select>
            </el-col>
            <el-col :span="3">
              <div style="text-align: center;">等于</div>
            </el-col>
            <el-col :span="5">
              <el-select v-model="item.ValueType" placeholder="请选择值来源" @change="clearItemVal(item)">
                <el-option label="变量" value="Form"></el-option>
                <el-option label="常量" value="Const" v-if="item.type=='Enum'||item.type=='Text'||item.type=='Number'"></el-option>
              </el-select>
            </el-col>
            <el-col :span="7" style="padding-left:5px;">
              <template v-if="item.ValueType=='Form'">
                <el-select v-model="item.Value" placeholder="请选择值">
                    <el-option v-for="formitem in item.form" :key="formitem.id" :label="formitem.title" :value="formitem.id"></el-option>
                </el-select>
              </template>
              <template v-else>
                <template v-if="item.type=='Enum'">
                  <el-select v-model="item.Value" placeholder="请选择值">
                    <el-option v-for="enitem in item.options" :key="enitem.val" :label="enitem.name" :value="enitem.val"></el-option>
                  </el-select>
                </template>
                <template v-else-if="item.type=='Number'">
                  <el-input type="number" v-model="item.Value" placeholder="请输入内容"></el-input>
                </template>
                <template v-else>
                  <el-input v-model="item.Value" placeholder="请输入内容"></el-input>
                </template>
              </template>
            </el-col>
            <el-col :span="3">
              <el-row type="flex" justify="end">
                <el-button @click="onCondiDelClick(idx)" type="danger" icon="el-icon-delete" size="small" circle></el-button>
              </el-row>
            </el-col>
          </el-row>
        </div>
      </el-form-item>

      <el-form-item label="4、对符合过滤条件的数据，进行以下字段的修改" v-if="config.action=='Update'&&SelectedTable!=null">
        <div>
          <div style="margin-bottom: 15px;" v-if="FieldsFields.length>0">
            <el-button @click="onFieldAddClick" type="primary" icon="el-icon-plus" size="small" plain>添加字段</el-button>
          </div>
          <el-row v-for="(item,idx) in FieldList" :key="idx" style="margin-bottom: 10px;">
            <el-col :span="6">
              <el-select v-model="item.TargetField" placeholder="请选择过虑字段">
                <el-option
                  v-for="fdi in FieldsFields" :key="fdi.code"
                  :label="fdi.name"
                  :value="fdi.code">
                </el-option>
              </el-select>
            </el-col>
            <el-col :span="3">
              <div style="text-align: center;">修改为</div>
            </el-col>
            <el-col :span="5">
              <el-select v-model="item.ValueType" placeholder="请选择值来源" @change="clearItemVal(item)">
                <el-option label="变量" value="Form"></el-option>
                <el-option label="常量" value="Const"></el-option>
              </el-select>
            </el-col>
            <el-col :span="7" style="padding-left:5px;">
              <template v-if="item.ValueType=='Form'">
                <el-select v-model="item.Value" placeholder="请选择值">
                    <el-option v-for="formitem in item.form" :key="formitem.id" :label="formitem.title" :value="formitem.id"></el-option>
                </el-select>
              </template>
              <template v-else>
                <template v-if="item.type=='Enum'">
                  <el-select v-model="item.Value" placeholder="请选择值">
                    <el-option v-for="enitem in item.options" :key="enitem.val" :label="enitem.name" :value="enitem.val"></el-option>
                  </el-select>
                </template>
                <template v-else-if="item.type=='Number'">
                  <el-input type="number" v-model="item.Value" placeholder="请输入内容"></el-input>
                </template>
                <template v-else>
                  <el-input v-model="item.Value" placeholder="请输入内容"></el-input>
                </template>
              </template>
            </el-col>
            <el-col :span="3">
              <el-row type="flex" justify="end">
                <el-button @click="onFieldDelClick(idx)" type="danger" icon="el-icon-delete" size="small" circle></el-button>
              </el-row>
            </el-col>
          </el-row>
        </div>
      </el-form-item>
    </el-form>

  </div>
</template>

<script>
import { getDataList } from "@/api/flowable/design";
import {getItems} from "../../utlity"
export default {
  name: "DataxeConfig",
  components: {},
  props: {
    config: {
      type: Object,
      default: () => {
        return {};
      },
    },
  },
  data() {
    return {
      formlist:[]
    };
  },
  computed:{
    SelectedTable(){
      let sels=this.formlist.filter(item=>item.code==this.config.targetform);
      if(sels.length>0){
        return sels[0];
      }
      else{
        return null;
      }
    },
    ConditionFields(){
      if(this.SelectedTable.fields==null){
        return [];
      }
      return this.SelectedTable.fields.filter(x=>x.used!=2);
    },
    FieldsFields(){
      return this.SelectedTable.fields.filter(x=>x.used!=1);
    },
    ConditionList(){
      this.config.conditions.forEach(item=>{
          let selfield=this.ConditionFields.filter(x=>x.code==item.TargetField);
          item["type"]=selfield[0].type;
          let formarr=[];
          if(selfield[0].formlist!=null){
            selfield[0].formlist.forEach(it=>{
              formarr.push({"title":it.name,"id":it.val});
            });
          }
          if(item["type"]=='Enum'){
            item['options']=selfield[0].options;
            let tmparr=getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='SelectInput');
            formarr.push(...tmparr);
          }
          else if(item["type"]=='Text'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='TextInput');
              formarr.push(...tmparr);
          }
          else if(item["type"]=='Number'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='AmountInput'||x.name=="NumberInput");
              formarr.push(...tmparr);
          }
          else if(item["type"]=='User'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='UserPicker');
              formarr.push(...tmparr);
          }
          item["form"]=formarr;
      });
      return this.config.conditions;
    },
    FieldList(){
      this.config.fields.forEach(item=>{
          let selfield=this.FieldsFields.filter(x=>x.code==item.TargetField);
          item["type"]=selfield[0].type;
          let formarr=[];
          if(selfield[0].formlist!=null){
            selfield[0].formlist.forEach(it=>{
              formarr.push({"title":it.name,"id":it.val});
            });
          }
          if(item["type"]=='Enum'){
            item['options']=selfield[0].options;
            let tmparr=getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='SelectInput');
            formarr.push(...tmparr);
          }
          else if(item["type"]=='Text'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='TextInput'||x.name=='TextareaInput');
              formarr.push(...tmparr);
          }
          else if(item["type"]=='Number'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='AmountInput'||x.name=="NumberInput");
              formarr.push(...tmparr);
          }
          else if(item["type"]=='User'){
              let tmparr= getItems(this.$store.state.flowable.design.formItems).filter(x=>x.name=='UserPicker');
              formarr.push(...tmparr);
          }
          item["form"]=formarr;
      });
      return this.config.fields;
    },
  },
  created(){
    getDataList().then(rsp=>{
      this.formlist=rsp.data;
    })
  },
  methods: {
    chgTarget(){
      this.$set(this.config,"conditions",[]);
      this.$set(this.config,"fields",[]);
    },
    onCondiAddClick(){
      let selitem=this.ConditionFields[0];
      this.config.conditions.push({
        "TargetField":selitem.code,
        "ValueType":"Form",
        "Value":""
      })

    },
    onCondiDelClick(idx){
      this.config.conditions.splice(idx,1);
    },
    onFieldAddClick(){
      let selitem=this.FieldsFields[0];
      this.config.fields.push({
        "TargetField":selitem.code,
        "ValueType":"Const",
        "Value":""
      })
    },
    onFieldDelClick(idx){
      this.config.fields.splice(idx,1);
    },
    clearItemVal(item){
      item.Value="";
    }
  },
};
</script>

<style scoped>
</style>
