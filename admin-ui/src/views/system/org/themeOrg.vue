<template>
  <div>
    <div class="cont_con">
      <div class="theme_con" v-if="form">
        <div class="theme_top">
          <div class="theme_title">
            <div class="img_con">
              <img :src="form.PhotoUrl+'?wh=500x500'" alt="">
            </div>
            <div class="text">{{form.themeName}}</div>
          </div>
          <div class="change_btn" @click="openFlowPicker">切换主题</div>
        </div>
        
        <div class="theme_bottom">
          <!-- <div class="theme_remark">{{form.Remark}}</div> -->
          <div class="switch_li">
            <div class="label">顶级菜单位置</div>
            <div class="switch_text">左边 <el-switch v-model="form.isTopNav" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch> 顶部</div>
          </div>
          <div class="switch_li">
            <div class="label">是否开启标签导航</div>
            <el-switch v-model="form.isTagsViews" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
          </div>
          <div class="switch_li">
            <div class="label">是否显示企业Logo</div>
            <el-switch v-model="form.isShowLogo" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
          </div>
          <div class="switch_li">
            <div class="label">是否开启动态标题</div>
            <el-switch v-model="form.isActiveTiltle" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
          </div>
        </div>
      </div>
      <div class="empty_con" style="height:200px" v-else>暂无数据</div>
    </div>
    
    <el-dialog :title="title" :close-on-click-modal="false" :visible.sync="open"  width="700px">
      <div class="hasbg_con" v-if="themeList&&themeList.length>0">
        <div class="cont_con" v-for="item in themeList" :key="item.Id" @click="finishChangeTheme(item)">
          <div class="theme_con" v-if="item">
            <div class="theme_top">
              <div class="theme_title" :class="item.Id==item.themeId?'active':''">
                <div class="img_con">
                  <img :src="item.PhotoUrl+'?wh=500x500'" alt="">
                </div>
                <div class="text">{{item.Name}}</div>
              </div>
              <div class="con_right" v-if="item.Id==form.themeId">
                <div class="unico"></div>
              </div>
            </div>
            
            <div class="theme_bottom">
              <!-- <div class="theme_remark">{{item.Remark}}</div> -->
              <div class="switch_li">
                <div class="label">是否开启TopNav</div>
                <!-- <el-switch v-model="item.isTopNav" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch> -->
                <div class="switch_text">左边 <el-switch v-model="item.isTopNav" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch> 顶部</div>
              </div>
              <div class="switch_li">
                <div class="label">是否开启标签导航</div>
                <el-switch v-model="item.isTagsViews" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
              </div>
              <div class="switch_li">
                <div class="label">是否显示企业Logo</div>
                <el-switch v-model="item.isShowLogo" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
              </div>
              <div class="switch_li">
                <div class="label">是否开启动态标题</div>
                <el-switch v-model="item.isActiveTiltle" :active-value="true" :inactive-value="false" :disabled="true" active-color="#3B6EF4" inactive-color="#E9EDEF"></el-switch>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="empty_con" v-else>暂无数据</div>
      </el-dialog>
  </div>
</template>

<script>
import {
  orgStyle,
  orgStyleList,
  changeOrgStyle
} from "@/api/system/StyleMan";
import {saveSetting} from '@/utils/theme'
export default {
  name: 'AdminUiThemeOrg',

  data() {
    return {
      title:'切换主题',
      form:null,
      org:null,
      themeList:[],
      open:false,
      isLoadTheme:false
    };
  },

  mounted() {
  },

  methods: {
    finishChangeTheme(row){
      //切换主题
      if(this.form.themeId!=row.Id){
        changeOrgStyle({styleId:row.Id}).then(res=>{
          // console.log("切换成功打印",res);
          this.$modal.msgSuccess("切换成功");
          this.isLoadTheme=true
          this.getOrgTheme()
          this.open=false
        })
      }
      
    },
    
    getOrgTheme(val){
      if(val){
        this.org=JSON.parse(JSON.stringify(val))
      }
      orgStyle({orgId:this.$store.state.user.orgId}).then(res=>{
        console.log(res.data,'主题数据');
        if(res.data){
          this.form={
            themeName:res.data.Name,
            PhotoUrl:(JSON.parse(res.data.StyleJson)).mobileLogo,
            Remark:res.data.Remark,
            themeId:res.data.Id,
            isTopNav:(JSON.parse(res.data.StyleJson)).isTopNav,
            isTagsViews:(JSON.parse(res.data.StyleJson)).isTagsViews,
            isShowLogo:(JSON.parse(res.data.StyleJson)).isShowLogo,
            isActiveTiltle:(JSON.parse(res.data.StyleJson)).isActiveTiltle,
            themeType:(JSON.parse(res.data.StyleJson)).themeType,
            isNotAutoCreate:(JSON.parse(res.data.StyleJson)).isNotAutoCreate,
            qywxAppId:(JSON.parse(res.data.StyleJson)).qywxAppId,
          }
          if(this.isLoadTheme){
            saveSetting(this.form)
          }
        }else{
          this.form=null
        }
        
      })
    },
    openFlowPicker(){
      this.open=true
      this.getorgStyleList()
    },
    getorgStyleList(){
      //获取企业的主题
      orgStyleList().then(res=>{
        res.data.map(row=>{
          row.isTopNav=(JSON.parse(row.StyleJson)).isTopNav
          row.isTagsViews=(JSON.parse(row.StyleJson)).isTagsViews
          row.isShowLogo=(JSON.parse(row.StyleJson)).isShowLogo
          row.isActiveTiltle=(JSON.parse(row.StyleJson)).isActiveTiltle
          row.PhotoUrl=(JSON.parse(row.StyleJson)).mobileLogo
        })
        this.themeList=res.data
      })
    }
  },
};
</script>
<style lang="less" scoped>
.empty_con{
  width: 100%;
  height: 300px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 16px;
  color: #999999;
}
.cont_con{
  padding: 30px;
  background: rgba(249, 250, 252, 1);
  border-radius: 4px;
  .theme_top{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 40px;
    line-height: 40px;
    .theme_title{
      display: flex;
      align-items: center;
      &.active{
        .text{
          color: rgba(53, 114, 255, 1);
        }
      }
    }
    .img_con{
      width: 48px;
      height: 40px;
      img{
        width: 48px;
        height: 40px;
      }
    }
    .text{
      font-size: 20px;
      color: rgba(51, 51, 51, 1);
      margin-left: 20px;
    }
    .change_btn{
      background: linear-gradient( 90deg, #4C79FF 0%, #6DA8FF 100%);
      width: 88px;
      height: 36px;
      display: flex;
      justify-content: center;
      align-items: center;
      color: rgba(255, 255, 255, 1);
      border-radius: 4px;
    }
  }
  .theme_bottom{
    .switch_li{
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 20px;
      line-height: 20px;
      width:100%;
      padding-left: 60px;
      margin-top: 30px;
      box-sizing: border-box;
      .label{
        font-size: 16px;
        color: rgba(120, 130, 157, 1);
      }
      .switch_text{
        font-size: 16px;
        color: rgba(120, 130, 157, 1);
      }
    }
  }
  .con_right{
    .unico {
      width: 28px;
      height: 28px;
      line-height: 26px;
      border: 1px solid #F9FAFC;
      background-color: #F9FAFC;
      position: relative;
      cursor: pointer;
    }

    .unico::before {
      position: absolute;
      content: " ";
      width: 7px;
      height: 13px;
      transform: rotate(45deg);
      border-right: 2px solid #067FD7;
      border-bottom: 2px solid #067FD7;
      top: 4px;
      left: 9px;
    }

  }
}
</style>