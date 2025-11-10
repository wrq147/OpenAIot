<template>
  <div>
    <el-dialog title="发起任务" :visible.sync="planeListOpen" center width="1100px" :close-on-click-modal="false" :destroy-on-close="true">
      <div v-loading="loading">
        <!-- <div class="from_con" id="from_con" style="padding-left:0;padding-right:0;">
            <el-form :model="planeForm" ref="planeForm" :inline="true" class="biaodan">
                <el-form-item label="创建时间">
                    <el-date-picker class="set_radius" v-model="dateRange" style="width:232px" value-format="yyyy-MM-dd" 
                        type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
                </el-form-item>
                <el-form-item class="submit_button_con">
                    <el-button icon="el-icon-refresh">重置</el-button>
                    <el-button type="primary" icon="el-icon-search" @click="getList">搜索</el-button>
                </el-form-item>
            </el-form>
        </div> -->
        <div class="plane_ul">
            <div class="plane_li" v-for="item in tbList" :key="item.Id" @click="choicePlane(item)">
              <div class="templete">
                <i :class="item.Icon" :style="'background: ' + item.Background"></i>
              </div>
              <div class="name_type">
                <div class="name">{{item.FlowTemplateName}}</div>
                <div class="type">{{item.Name}}</div>
              </div>
                
                <!-- <div class="remark" v-if="item.Remark">备注：{{item.Remark}}</div> -->
            </div>
        </div>
      </div>
    
    </el-dialog>
  </div>
</template>

<script>
import { devPlaneList } from "@/api/after/devplane";
export default {
  name: 'AdminUiPlaneDialog',

  data() {
    return {
        planeListOpen:false,
        dateRange:[],
        tbList:[],
        total:0,
        loading:false,
        planeForm:{
            pageNum: 1,
            pageSize: 0,
            StartWay:0,
            CanStart:true
        }
    };
  },

  mounted() {
    
  },

  methods: {
    choicePlane(item){
        //
        this.planeListOpen=false
        this.$emit('openTaskAdd',item)
    },
    openDialog(deviceId){
      if(deviceId){
        this.planeForm.DeviceId=deviceId
      }
      this.getList()
      this.planeListOpen=true
    },
    getList(){
      this.loading=true
      devPlaneList(this.addDateRange(this.planeForm, this.dateRange)).then(res=>{
        // console.log(res,'计划类型');
        this.tbList=JSON.parse(JSON.stringify(res.data.List))
        // for(let i=0;i<5;i++){
        //     this.tbList=[...this.tbList,...this.tbList]
        // }
        this.total=res.data.Total
        this.loading=false
      }).catch(err=>{
        this.loading=false
      })
    },
  },
};
</script>
<style lang="less" scoped>
.plane_ul{
    display: flex;
    flex-wrap: wrap;
    .plane_li{
        width: calc(25% - 10px);
        // height: 120px;
        padding: 20px;
        margin-right: 10px;
        box-sizing: border-box;
        background: #F9FAFC;
        border-radius: 4px;
        margin-bottom: 20px;
        cursor: pointer;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        .templete{
          width: 50px;
          height: 50px;
          background: rgba(255, 133, 61, 1);
          border-radius: 10px;
          margin-right: 20px;
          
          i{
            width: 50px;
            height: 50px;
            background: rgba(255, 133, 61, 1);
            border-radius: 10px;
            font-size: 25px;
            display: flex;
            justify-content: center;
            align-items: center;
            color: #ffffff;
          }
        }
        .name_type{
            .name{
              line-height: 24px;
              font-size: 16px;
              color: rgba(51, 51, 51, 1);
              font-weight: bold;
            }
            .type{
              font-size: 14px;
              color: rgba(153, 153, 153, 1);
              line-height: 22px;
              margin-top: 4px;
            }
          }
        .remark{
            margin: 38px 0 0 20px;
            font-size: 14px;
            display: -webkit-box;
            -webkit-box-orient: vertical;
            -webkit-line-clamp: 2; /* 定义显示的行数 */
            overflow: hidden;
            text-overflow: ellipsis;
            color: #999999;
        }
    }
    .plane_li:nth-child(4n){
        margin-right: 0;
    }
}
</style>