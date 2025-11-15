<template>
  <div>
    <div class="airCompressors" v-loading="configLoading" v-if="tableData&&tableData.length>0">
        <div class="airCompressors-item" v-for="its in tableData" :key="its.Id">
            <div class="airCompressors-item-top">
            <div class="airCompressors-item-top-left">
                <el-image fit="cover" class="airCompressors-item-top-logo" :src="its.PhotoUrl + '?wh=500x500'">
                <img class="airCompressors-item-top-logo" slot="error" src="../../../assets/images/shebei.png" alt/>
                </el-image>
                <div class="airCompressors-item-top-left-text">
                <p class="title">{{ its.Name }}</p>
                <p class="number">{{ its.DeviceNumber }}</p>
                <p class="onLine">
                    <span v-if="its.Online != 2" class="yuan" :class="[its.Online == 0 ? 'on1' : 'active1']"></span>
                    <span :class="[its.Online == 0 ? 'on' : its.Online == 1 ? 'active' : 'on',]">{{ its.Online == 0 ? "离线" : its.Online == 1 ? "在线" : "未知" }}</span>
                </p>
                </div>
            </div>
            <div class="airCompressors-item-top-see" @click="toDeviceDetails(its)">
                <svg-icon icon-class="todetails"></svg-icon>
                <span class="airCompressors-item-top-see-txt">查看</span>
            </div>
            </div>
            <div class="airCompressors-item-bottom">
            <div class="airCompressors-item-bottom-left">
                产品：{{ its.ProductName }}
            </div>
            <div class="airCompressors-item-bottom-right">
                类型：{{ its.GroupName }}
            </div>
            </div>
        </div>
        
    </div>
    <div class="empty_li" v-else-if="!configLoading">暂无数据</div>
  </div>
</template>

<script>
export default {
  name: 'AdminUiDevPiece',

  data() {
    return {
      
    };
  },
  props:{
    tableData:{
        type:Array,
        default:()=>{
            return []
        }
    },
    configLoading:{
        type:Boolean,
        default:true
    }
  },
  mounted() {
    
  },

  methods: {
    toDeviceDetails(it){
        this.$emit('toDeviceDetails',it)
    }
  },
};
</script>
<style lang="scss" scoped>
.empty_li{
    width: 100%;
    text-align: center;
    font-size: 16px;
    color: #999999;
    margin-top: 100px;
  }
.airCompressors {
  display: flex;
  justify-content: left;
  width: 100%;
  flex-wrap: wrap;
  align-items: center;
  margin-right: -15px;
  .airCompressors-item:nth-child(3n){
    margin-right: 0;
  }
  .airCompressors-item {
    background: rgba(249, 250, 252, 1);
    width: calc(33% - 10px);
    // height:180px;
    border-radius: 10px;
    margin-top: 25px;
    display: flex;
    margin-right: 14px;
    flex-direction: column;
    padding: 15px 20px;
    box-sizing: border-box;
    .airCompressors-item-bottom {
      color: rgba(153, 153, 153, 1);
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-top: 15px;
      font-size: 13px;
      .airCompressors-item-bottom-left {
        width: 48%;
        border-right: 1px solid rgba(234, 234, 234, 1);
      }
      .airCompressors-item-bottom-right {
        width: 48%;
        text-align: center;
      }
    }
    .airCompressors-item-top {
      display: flex;
      border-bottom: 1px solid rgba(234, 234, 234, 1);
      padding-bottom: 15px;
      justify-content: space-between;
      .airCompressors-item-top-see {
        // margin-left: auto;
        align-self: flex-end;
        color: rgba(153, 153, 153, 1);
        font-size: 13px;
        display: flex;
        align-items: center;
        cursor: pointer;
        .airCompressors-item-top-see-txt {
          display: inline-block;
          margin-left: 6px;
        }
      }
      .airCompressors-item-top-see:hover {
        color: rgba(53, 114, 255, 1);
      }
      .airCompressors-item-top-left {
        display: flex;
        align-items: center;

        .airCompressors-item-top-left-text {
          line-height: 12px;
          margin-left: 20px;
          .number {
            color: rgba(153, 153, 153, 1);
            font-size: 14px;
          }
          .title {
            font-weight: bold;
            line-height: 25px;
          }
          .onLine {
            display: flex;
            font-size: 13px;
            align-items: center;
            .yuan {
              display: inline-block;
              width: 8px;
              height: 8px;
              border-radius: 50%;

              margin-right: 6px;
            }
          }
          .active1 {
            background: rgba(13, 179, 166, 1);
          }
          .on1 {
            background: rgba(186, 186, 186, 1);
          }
          .active {
            color: rgba(13, 179, 166, 1);
          }
          .on {
            color: rgba(102, 102, 102, 1);
          }
        }
        .airCompressors-item-top-logo {
          width: 75px;
          height: 75px;
          border-radius: 10px;
        }
      }
    }
  }
}
</style>