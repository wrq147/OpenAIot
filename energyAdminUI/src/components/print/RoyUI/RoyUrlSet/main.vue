<template>
  <div>
    <el-dropdown trigger="click" @command="onNewData" style="width: 100%">
        <div style=" color: #3572ff;cursor: pointer;text-align: center;width: 100%;">
            <i class="el-icon-plus"></i>
            <span style="margin-left: 6px; margin-right: 6px">添加链接</span>
        </div>
        <el-dropdown-menu slot="dropdown">
            <el-dropdown-item command="text">文本</el-dropdown-item>
            <el-dropdown-item command="source">数据源</el-dropdown-item>
        </el-dropdown-menu>
    </el-dropdown>
    <div class="data-bd">
        <el-row style="margin-bottom: 15px" v-for="(params, idx) in urlParamsList" :key="idx">
            <el-col :span="16">
                <vxe-input style="width: 120%;" v-if="params.type=='text'" v-model="params.value" placeholder="请输入链接" type="text"></vxe-input>
                <vxe-select style="width: 120%;" v-if="params.type=='source'" v-model="params.value" placeholder="请选择数据源">
                    <vxe-option v-for="opt in dataSource" :key="opt.field" :label="opt.title" :value="opt.field"></vxe-option>
                </vxe-select>
            </el-col>
            <el-col :span="8" class="op">
                <el-button style="margin-left: 10px" size="mini" @click="delDataItem(idx)" type="danger" icon="el-icon-delete" circle/>
            </el-col>
        </el-row>
    </div>
  </div>
</template>

<script>
import { mapState } from 'vuex'
export default {
  name: 'RoyUrlSetMain',
  props: {
    modelValue: Array,
    defaultColor: {
      type: String,
      default: '#000000'
    },
    disabled: Boolean
  },
  data() {
    return {
      urlParamsList:this.modelValue
    };
  },
  computed: {
    ...mapState({
      dataSource: (state) => state.printTemplateModule.dataSource
    })
  },
  mounted() {
  },
  watch:{
    urlParamsList:{
        handler(newVal){
           this.updateValue(newVal) 
        },
        deep:true
    }
  },
  methods: {
    delDataItem(index){
        this.urlParamsList.splice(index,1)
    },
    updateValue(value) {
      this.$emit('update:modelValue', value)
      this.$emit('change', value)
      this.openStatus = false
    },
    onNewData(val){
        if(val&&val=='text'){
            this.urlParamsList.push({
                type:'text',
                value:''
            })
        }else if(val&&val=='source'){
            this.urlParamsList.push({
                type:'source',
                value:''
            })
        }
    }
  },
};
</script>
<style lang="less" scoped>
.data-title {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
  display: flex;
  justify-content: space-between;
  height: 45px;
  align-items: center;

  .fz {
    font-size: 14px;
  }
}
.data-bd {
    margin-top: 10px;
  .rw {
    margin-bottom: 15px;
  }

  .t {
    font-size: 12px;
    color: #999;
    text-align: center;
  }

  .tp {
    display: flex;
    align-items: center;
    font-size: 12px;
    height: 40px;
    justify-content: center;
  }

  .op {
    display: flex;
    align-items: center;
    height: 40px;
    justify-content: flex-end;
  }
}
</style>