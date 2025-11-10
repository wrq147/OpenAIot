<template>
    <el-dialog :title="title" :visible.sync="open" width="60%" :close-on-click-modal="false"
        append-to-body :list="iconsGroups">
        <el-form :model="queryProcessParams" ref="queryProcessForm" :inline="true" label-width="0">
          <el-form-item>
            <el-input style="width: 350px" v-model="queryProcessParams.name"
              placeholder="请输入名称" prefix-icon="el-icon-search" clearable @clear="handleProcessQuery"/>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" icon="el-icon-search" size="medium" @click="handleProcessQuery">搜索</el-button>
          </el-form-item>
        </el-form>
        <div class="content_con">
          <div class="content_li_con" v-for="(item, inx) in iconsGroups" :key="inx" v-show="item.Id > 0 || item.Items.length > 0">
            <div class="title">{{ item.Name }}</div>
            <ul class="content_ul">
              <li class="li_con" v-for="(it, ix) in item.Items" :key="ix">
                <div :class="[ 'content_li', activeProgess == it.Id ? 'active_li' : '',]"
                  @mouseover="startProcess(it.Id)" @mouseout="endProcess()" @click="handleStartProcess(it)">
                  <div class="left_cont">
                    <i :class="['li-icons', it.Icon]" :style="{ color: '#ffffff', background: it.Background }"></i>
                    <span>{{ it.Name }}</span>
                  </div>
                  <div v-show="activeProgess == it.Id" class="active_content">{{title}}</div>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </el-dialog>
</template>
<script>
import {
  definitionList
} from "@/api/flowable/process";
export default {
  name: 'AdminUiSelectProcess',
  props:{
    title:{
      type:String,
      default:''
    }
  },
  data() {
    return {
      open: false,
      // title:'',
      queryProcessParams: {
        IsEmbed:false,
        name: null
      },
      iconsGroups: [], //流程列表
      activeProgess: -1 //鼠标移到的流程
    };
  },

  mounted() {
    
  },

  methods: {
    endProcess() {
      //鼠标移开事件
      this.activeProgess = -1;
    },
    startProcess(id) {
      //鼠标移上去的事件
      this.activeProgess = id;
    },
    /** 打开弹窗操作 */
    handleOpen() {
      this.open = true;
      this.listDefinition();
    },
    /** 搜索按钮操作 */
    handleProcessQuery() {
      this.listDefinition();
    },
    listDefinition() {
      definitionList(this.queryProcessParams).then(response => {
        this.iconsGroups = response.data;
        this.processLoading = false;
      });
    },
    handleStartProcess(row) {
      this.$emit('handleStartProcess',row)
    },
  },
};
</script>
<style scoped>

</style>