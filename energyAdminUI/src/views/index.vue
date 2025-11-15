<template>
  <div class="home_com" v-loading="isLoading" :class="{'factory_con':activeLi==1}">
    <div class="home_top_nav_con">
      <div class="home_top_nav">
        <div class="top_nav_li" :class="{'active':activeLi==1}" @click="setActive(1)">厂区概览图</div>
        <div class="top_nav_li" :class="{'active':activeLi==2}" @click="setActive(2)">设备分布图</div>
      </div>
      <div class="select_con" v-if="activeLi==1">
        <el-select popper-class="home_select" class="home_select" style="width:56px;" v-model="countType" placeholder="请选择">
          <el-option  key="月份" label="月份" value="月"></el-option>
          <el-option  key="年度" label="年度" value="年"></el-option>
        </el-select>
        <el-date-picker :clearable="false" prefix-icon="el-icon-arrow-down" class="home_picker" v-if="countType=='月'" style="width:100px" v-model="countDate" type="month" format="yyyy年MM月" placeholder="统计月份" @change="startChange"></el-date-picker>
        <el-date-picker :clearable="false" prefix-icon="el-icon-arrow-down" class="home_picker" v-if="countType=='年'" style="width:100px" v-model="countDate" type="year" format="yyyy年" placeholder="统计年度" @change="startChange"></el-date-picker>
      </div>
    </div>
    <equipmentDistribution v-if="activeLi==2" style="width:100%;height:calc(100vh - 60px);"></equipmentDistribution>
    <factoryOverview :countType="countType" :countDate="countDate" v-else style="width:100%;min-height:calc(100vh - 60px);"></factoryOverview>
  </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import equipmentDistribution from '@/views/homeModules/bulletinBoard/equipmentDistribution.vue'
import factoryOverview from '@/views/homeModules/bulletinBoard/factoryOverview.vue'
import { checkPermi } from '@/utils/permission.js';
import { initMap } from "@/utils/amap";
import dayjs from 'dayjs';
export default {
  name: "Index",
  components: {
    equipmentDistribution,
    factoryOverview
  },
  mixins: [resizeTableCon],
  data() {
    return {
      isLoading: false,
      checkNum: 0,
      activeLi:1,
      countType:'月',
      countDate:''
    };
  },
  mounted() {
    this.countDate=dayjs().subtract(1,'month').format('YYYY-MM-DD')
    this.getCheckNum()

  },
  beforeCreate() {
    // console.log('iiiiiiii');
    initMap().catch((ex) => {
      // this.$message.error(ex);
    });
  },
  methods: {
    startChange(){
      //统计的年月
    },
    setActive(val){
      this.activeLi=val
    },
    isCheckPermi(val) {
      return checkPermi(val)
    },
    getCheckNum() {
      //获取权限数量
      if (checkPermi(['/AfterService/Dev/List'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/CRMMan/'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/FlowService/Flow'])) {
        this.checkNum = this.checkNum + 1
      }
      if (checkPermi(['/Stock/'])) {
        this.checkNum = this.checkNum + 1
      }
    }
  }
};
</script>
<style lang="less">
.home_com{
  &::-webkit-scrollbar {
      width: 6px;
      height: 6px;
  }
  
  /* 修改滚动条轨道 */
  &::-webkit-scrollbar-track {
      background: rgba(19, 104, 96, 1);
  }
  
  /* 修改滚动条滑块 */
  &::-webkit-scrollbar-thumb {
      background: rgba(255, 255, 255, 0.2);
      border-radius: 5px;
  }
  
  /* 修改滑块在鼠标悬浮时的样式 */
  &::-webkit-scrollbar-thumb:hover {
      background: rgba(255, 255, 255, 0.2);
  }
}
</style>
<style lang="scss" scoped>
.home_select.el-select-dropdown.el-popper{
  .el-scrollbar{
    .el-select-dropdown__wrap{
      .el-select-dropdown__list{
        .el-select-dropdown__item{
          padding: 0 10px !important;
        }
      }
    }
  } 
}
.home_com {
  width: 100%;
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  align-items: flex-start;
  position: relative;
  z-index: 3;
  // &.factory_con{
  //   background: url('~@/assets/images/data_bg.png') no-repeat;
  //   background-size: cover;
  //   z-index: 3;
  // }
  // background: rgba(25, 33, 45, 1);
  // background: #000000;
  // margin-bottom: -30px;
  .home_top_nav_con{
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: absolute;
    width: 100%;
    padding: 6px 10px;
    box-sizing: border-box;
    height: 42px;
    border-bottom: 2px solid rgba(34, 46, 64, 1);
    background: rgba(19, 25, 34, 1);
    top: 0;
    left: 0;
    z-index: 2;

    .select_con{
      .home_select{
        margin-right: 4px;
        ::v-deep .el-input{
          font-size: 12px;
        }
        ::v-deep .el-input__inner{
          height: 28px;
          line-height: 28px;
        }
        ::v-deep .el-select__caret{
          font-size: 12px;
          
        }
        ::v-deep .el-input__icon{
          line-height:28px;
          width:18px
        }
      }
      .home_picker{
        width: 120px;
        ::v-deep .el-input__inner{
          height: 28px;
          line-height: 28px;
          text-align: right;
          // border:none;
          font-size: 12px;
          padding: 0;
          padding-right: 30px;
        }
        ::v-deep .el-input__prefix{
          right: 18px;
          left: initial;
          width: 8px;
        }
        ::v-deep .el-input__prefix .el-input__icon{
          line-height: 28px;
          
        }
      }
    }
  }
  .home_top_nav{
    display: flex;
    align-items: center;
    z-index: 999;
    
    .top_nav_li{
      cursor: pointer;
      width: 120px;
      height: 28px;
      background: url('~@/assets/images/not_choice.png') no-repeat;
      font-size: 14px;
      color: rgba(255, 255, 255, 0.6);
      display: flex;
      justify-content: center;
      align-items: center;
      &.active{
        background: url('~@/assets/images/choice.png') no-repeat;
        color: rgba(255, 255, 255, 1);
      }
    }
  }
  
}
</style>
