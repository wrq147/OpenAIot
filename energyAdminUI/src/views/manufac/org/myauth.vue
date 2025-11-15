<template>
  <div style="padding:10px 10px 0 10px;height:100%" id="big_con" v-loading="isLoading">
    <el-empty v-if="authList.length == 0" description="暂无授权证书"></el-empty>
    <div v-else class="myauth-el-table">

      <el-row class="myauth-el-row">
        <el-col :span="5" v-for="item in authList" :key="item.Id">
          <el-card shadow="hover" :body-style="{ padding: '0px' }">
            <div class="image-wrap">
              <div class="image">
                <el-image style="height:100%;width:100%;" fit="cover" :src="item.Logo + '?wh=500x500'" lazy>
                </el-image>
              </div>
              <div class="tip" v-if="item.RegionsName&&item.RegionsName!=''">代理区域：{{ item.RegionsName }}</div>
            </div>

            <div style="padding: 14px;height: 86px;">
              <div>
                <el-tag>{{ item.GradeName }}</el-tag>
                <span class="name">{{ item.ParentOrgName }}</span>
              </div>
              <div class="bottom clearfix">
                <time class="time">{{ parseTime(item.createTime) }}</time>
                <el-button v-if="item.CertTemplateId!=''" type="text" @click="downloadCert(item)" class="button">下载证书</el-button>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>

    </div>
    <EmbedPrint v-if="showPrintParams" ref="printDlg" @closePrint="closePrint"></EmbedPrint>
  </div>
</template>
<script>

import {
  AuthorizationList,
} from "@/api/manufac/myauth";
import EmbedPrint from "../../report/print/EmbedPrint";
export default {
  name: '',
  components: {
    EmbedPrint
  },
  mixins: [],
  props: {

  },
  data() {
    return {
      isLoading:false,
      authList: [],
      showPrintParams:false
    }
  },
  computed: {

  },
  watch: {

  },
  created() {
    this.listInit();
  },
  methods: {
    closePrint(){
      //关闭打印弹窗
      this.showPrintParams=false
    },
    listInit() {
      this.isLoading=true;
      AuthorizationList().then((res) => {
        this.authList = res.data;
        this.isLoading=false;
      })
    },
    downloadCert(item){
      this.showPrintParams=true
      this.$nextTick(()=>{
        this.$refs.printDlg.showPrint(item.CertTemplateId,{id:item.Id})
      })
      
    }
  }
};
</script>
<style lang='scss' scoped>
.myauth-el-table {
  .image-wrap {
    position: relative;
    width: 100%;
    padding-top: 70%;
    margin: 0 auto;
    .image{
      position: absolute;
      top: 0;
      left:0;
      width: 100%;
      height: 100%;
    }
  }
  .name{
    margin-left: 15px;font-size: 14px;font-weight: bold;
  }
  .tip{
    position: absolute;
    background-color: #409EFF;
    bottom: 0px;
    right: 0px;
    padding:3px 5px;
    font-size: 12px;
    color: #fff;
    margin-top:10px;
    text-align: right;
  }
}

.time {
  font-size: 13px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
}

.button {
  padding: 0;
  float: right;
}


.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both
}

.myauth-el-row{
  .el-col{
    margin-left: 50px;
  }
}
</style>