<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="800px" height="700px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="">
        <span>各种能源折标准煤系数（参考值）</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>
    <div class="img_con">
        <div class="tips_text">如下数据取至国家标准  ２５８９—２０２０GB/T 《综合能耗计算通则》,将根据国家发布不定期更新。</div>
        <img style="width:760px;height:auto" src="@/assets/images/coefficient.png" alt="">
    </div>
  </el-dialog>
</template>

<script>
export default {
  name: 'EnergyAdminUICoefficient',

  data() {
    return {
      dialog:false,
    };
  },

  mounted() {
    
  },

  methods: {
    closeDialog(){
        this.dialog=false
    },
    openDialog(){
        this.dialog=true
    }
  },
};
</script>

<style lang="less" scoped>
.adddialog{
    ::v-deep .el-dialog__body{
        padding: 14px 20px;
        height: 586px;
        overflow-y: auto;
        overflow-x: hidden;
        .img_con{
            // height: 100%;
            // overflow-y: auto;
            // overflow-x: hidden;
            .tips_text{
                margin-bottom: 20px;
                font-size: 14px;
                color: rgba(255, 255, 255, 1);
            }
        }
    }
}
.dialog_title{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  .dialog_title_left{
    font-size: 16px;
    color: #ffffff;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    padding-left: 20px;
    line-height: 16px;
    height: 56px;
    .img{
      width: 16px;
      height: 16px;
    }
    span{
      margin-left: 6px;
    }
  }
  .dialog_title_right{
    margin-right: 20px;
    cursor: pointer;
    i.zhongtaiiconfont{
      color: rgba(255, 255, 255, 0.60);
      font-size: 12px;
    }
  }
}
</style>