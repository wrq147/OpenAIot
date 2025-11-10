<template>
  <el-menu :default-active="activeMenu" mode="horizontal" @select="handleSelect" class="el-menu-demo reportRightnav" :key="'a'+elmenuKey">
    <template v-for="(item, index) in topMenus">
      <el-menu-item :index="item.path" :key="index" v-if="!item.list&&index <= visibleNumber||item.list&&item.list.length==0&&index <= visibleNumber" :disabled="item.isdisable">
        <!-- <svg-icon :icon-class="item.meta.icon" /> -->
        <i :class="item.icon"></i>
        {{ item.title }}
      </el-menu-item>
      <el-submenu :index="item.path" :key="index" v-if="item.list&&item.list.length>0&&index <= visibleNumber" :disabled="item.isdisable">
        <template slot="title">{{item.title}}</template>
        <el-menu-item :index="item.path+'-'+it.path" v-for="(it,ix) in item.list" :key="index+'k'+ix" :disabled="it.isdisable">
          <div class="putbutton">{{it.title}}<input v-if="it.path=='inJson'" type="file" @change="importProcess" id="putbuttonFile"/></div>
        </el-menu-item>
      </el-submenu>
    </template>

    <!-- 顶部菜单超出数量折叠 -->
    <el-submenu index="more" v-if="topMenus.length > visibleNumber+1" :key="'b'+elsubmenuKey">
      <template slot="title">更多菜单</template>
      <template v-for="(item, index) in topMenus">
        <el-menu-item :index="item.path" :key="index" v-if="!item.list&&index > visibleNumber||item.list&&item.list.length==0&&index> visibleNumber" :disabled="item.isdisable">
          <!-- <svg-icon :icon-class="item.meta.icon" /> -->
          <i :class="item.icon"></i>
          {{ item.title }}
        </el-menu-item>
        <el-submenu :index="item.path" :key="index" v-if="item.list&&item.list.length>0&&index > visibleNumber" :disabled="item.isdisable">
            <template slot="title">{{item.title}}</template>
            <el-menu-item :index="item.path+'-'+it.path" v-for="(it,ix) in item.list" :key="index+'k'+ix" :disabled="it.isdisable">
              <div class="putbutton">{{it.title}}<input v-if="it.path=='inJson'" type="file" @change="importProcess" id="putbuttonFile"/></div>
            </el-menu-item>
        </el-submenu>
      </template>
    </el-submenu>
  </el-menu>
</template>

<script>
export default {
  name: 'AdminUiRightnav',
  props:{
    undoStyle:{
        type:Boolean,
        default:false,
    },
    redoStyle:{
        type:Boolean,
        default:false,
    },
    justifywidth:{
        type:Number,
        default:0
    }
  },
  data() {
    return {
        elmenuKey:0,
        elsubmenuKey:0,
        activeMenu:'',
      // 顶部栏初始数
      visibleNumber: 2,
      // 当前激活菜单的 index
      currentIndex: undefined,
      topMenus:[],
      VisibleWidth:0
    };
  },
  watch:{
    undoStyle:{
      handler(to){//监听撤回是否禁用
        if(this.topMenus[0]){
          this.topMenus[0].isdisable=to
        }
            
      },
      immediate:true,
      deep:true,
    },
    redoStyle:{
      handler(to){//监听还原是否禁用
        if(this.topMenus[1]){
          this.topMenus[1].isdisable=to
        }
      },
      immediate:true,
      deep:true,
    },
    justifywidth:{
        handler(to){
            if(to){
                this.VisibleWidth=to*0.45
                this.setVisibleNumber();
            }
        },
        immediate:true,
        deep:true,
    }
  },
  beforeMount() {
    window.addEventListener("resize", this.setVisibleNumber);
  },
  beforeDestroy() {
    window.removeEventListener("resize", this.setVisibleNumber);
  },
  mounted() {
    this.setVisibleNumber();
    this.topMenus=[{
        title:'撤销',
        icon:'el-icon-arrow-left',
        path:'undo',
        isdisable:this.undoStyle
      },{
        title:'还原',
        icon:'el-icon-arrow-right',
        path:'redo',
        isdisable:this.redoStyle
      },{
        title:'预览',
        icon:'el-icon-view',
        path:'view',
        isdisable:false
      },{
        title:'预警',
        icon:'el-icon-coordinate',
        path:'warning',
        isdisable:false
      },{
        title:'导出',
        icon:'el-icon-edit-outline',
        path:'handleCommand',
        isdisable:false,
        list:[{
            path:'image',
            title:'导出图片',
            isdisable:false,
        },{
            path:'word',
            title:'导出word',
            isdisable:false,
        },{
            path:'ppt',
            title:'导出ppt',
            isdisable:false,
        },{
            path:'outJson',
            title:'导出json',
            isdisable:false,
        },{
            path:'inJson',
            title:'导入json',
            isdisable:false,
        }]
      },{
        title:'清空',
        icon:'el-icon-delete',
        path:'clear',
        isdisable:false
      },{
        title:'保存',
        icon:'el-icon-document',
        path:'saveScreen',
        isdisable:false
      },]
  },

  methods: {
    // 根据宽度计算设置显示栏数
    setVisibleNumber() {
      let width = document.body.getBoundingClientRect().width / 3;
      if(this.VisibleWidth){
        width=this.VisibleWidth
      }
      this.visibleNumber = parseInt(width / 82)-1;
      this.elmenuKey++
      this.elsubmenuKey++
    },
    // 菜单选择事件
    handleSelect(key, keyPath) {
      this.currentIndex = key;
      if (key == null) {
        return;
      }
      this.$emit('returnHandleSelect',key)
    },
    processReadFile(file) {
      //读取导入参数的值
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          let josnForm = JSON.parse(e.target.result);
          this.$emit('importFile',josnForm)
          

          // console.log(josnForm, "导入的json");
        } catch (error) {
          console.error("导入报表失败", error);
        }
      };
      reader.readAsText(file);
    },
    importProcess(event) {
      //导入参数
      const file = event.target.files[0];
      if (!file) {
        return;
      }
      this.processReadFile(file);
    },
  },
};
</script>
<style lang="scss" scoped>
.putbutton {
  // background: #1890ff;
  // color: #fff;
  position: relative;
  span {
    // color: #ffffff;
  }
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
}
.el-menu-demo.reportRightnav.el-menu--horizontal>.el-menu-item{
  padding: 0 10px;
  i{
    width: 18px;
    margin: 2px;
  }
}
.el-menu-demo.reportRightnav.el-menu--horizontal>.el-submenu>.el-submenu__title{
    padding: 0 3px;
    i{
    width: 18px;
    margin: 2px;
  }
}
.el-menu-demo.reportRightnav.el-menu--horizontal>.el-submenu{
  width: 100px;
}
::v-deep .el-submenu__title{
  padding: 0 10px;
}
</style>