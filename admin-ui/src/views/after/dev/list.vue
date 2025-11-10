<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 136px)' }">
      <div class="device_con" v-if="!configLoading">
        <div class="device_con_left">
          <div class="left_top">
            <div class="name">{{ activeOrginfo.OrgName }}</div>
            <div class="top_btn">
              <div class="handle_my" @click="switchMyOrgers" v-if="activeOrginfo.Id != myOrgId">取消</div>
              <div class="change_btn" @click.stop="switchOrgers">切换</div>
            </div>
          </div>
          <div class="custom_info">
            <div class="custom_logo">
              <img class="logo" :src="activeOrginfo.Logo" alt="" />
            </div>
            <div class="custom_name hui">
              {{ activeOrginfo.Creator ? activeOrginfo.Creator.RealName : "" }}{{ activeOrginfo.Creator&&activeOrginfo.Creator.Mobile ? '（'+activeOrginfo.Creator.Mobile+'）' : "" }}
            </div>
            <div class="custom_name hui">{{ activeOrginfo.AddressName }}</div>
            <div class="custom_name hui">{{ activeOrginfo.AddressDetail }}</div>
            <div class="line"></div>
          </div>
          <div class="device_total">
            <div class="total_li" v-for="(item,inx) in totalDeviceInfo" :key="'state'+inx">
              <div class="li_top">
                <div class="samll_block green"></div>
                <span>{{item.state}}</span>
              </div>
              <div class="li_bottom">
                <span class="num">{{item.count}}</span><span class="unit">台</span>
              </div>
            </div>
            <!-- <div class="total_li">
              <div class="li_top">
                <div class="samll_block"></div>
                <span>离线</span>
              </div>
              <div class="li_bottom">
                <span class="num">{{totalDeviceInfo.OfflineCount}}</span><span class="unit">台</span>
              </div>
            </div> -->
          </div>
        </div>
        <div class="PlanList-table">
          <div class="from_con" id="from_con" style="margin-bottom: 0; padding-bottom: 0; padding-left: 0">
            <el-form :model="deviceForm" ref="deviceForm" :inline="true" class="biaodan">
              <el-form-item label="车间">
                <select_tree ref="selectTree" class="set_radius groupSet" :defaultProps="defaultProps" nodeKey="TreeId" :treeData='roomTreeList' @select="selectRoomCatetoryTree"/>
              </el-form-item>
              <el-form-item label="关键字" prop="Key">
                <el-input v-model="deviceForm.Key" placeholder="请输入关键字" clearable/>
              </el-form-item>
              <el-form-item label="联网状态" prop="Online">
                <el-select style="width:100px;" v-model="deviceForm.Online" placeholder="联网状态" clearable>
                  <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value"/>
                </el-select>
              </el-form-item>
             <el-form-item label="运行状态" prop="DState" v-if="isShowDStateSerch">
               <el-select v-model="deviceForm.DState" style="width:100px;" filterable reserve-keyword allow-create
                :clearable="true" placeholder="运行状态">
                  <el-option v-for="item in DStatelist" :key="item.value" :label="item.label" :value="item.value">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="getDeviceList">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          
          <!--新修改的样式-->
          <!-- <dev_piece :configLoading="configLoading" :tableData="tableData" @toDeviceDetails="toDeviceDetails"></dev_piece> -->
          <dev_table :configLoading="configLoading2" :tableData="tableData" @toDeviceDetails="toDeviceDetails"></dev_table>

          <pagination v-show="total > 0" :total="total" :page.sync="deviceForm.pageNum" :limit.sync="deviceForm.pageSize" :pageSizes="pageSizes" @pagination="getDeviceList"/>
        </div>
      </div>
    </div>
    <KfSelecter
      ref="kfDlg"
      @ok="onTargetChange"
      :isFilterInvite="true"
      :IsInvite="true"
      title="请选择切换的企业"
    ></KfSelecter>
    
  </div>
</template>
<script>
import { myDeviceList } from "@/api/after/dev";
import { orgInfo } from "@/api/system/company";
import KfSelecter from "@/components/KFSelecter/kfselecter";
import select_tree from "./select_tree";
import { devRunStatisticsInfo } from "@/api/rules/device";
import { roomCatetoryTree, deviceRoomList } from "@/api/after/room";

// import dev_piece from "./dev_piece";
import dev_table from "./dev_table";
import { getConfigKey } from "@/api/system/config.js";
export default {
  name: "deviceList",
  components: { KfSelecter,select_tree,dev_table },
  dicts: ["device_run"],
  data() {
    return {
      //设备
      defaultProps:{
        children: "Children",
        label: "TreeName",
      },
      roomTreeList: [], //车间树结构
      roomval: {},
      pageSizes: [10, 20, 30, 50, 100],//增大每页数据量，方便批量打印设备二维码
      total: 0,
      deviceForm: {
        pageNum: 1,
        pageSize: 10,
        Online: null,
        Key: "",
        DState: '',
        RoomId: '',
        RoomCategory: '',
      }, //设备查询form
      statusList: [
        { label: "离线", value: 0 },
        { label: "在线", value: 1 },
        { label: "未知", value: 2 },
      ],
      tableData: [], //我的设备列表
      configLoading: true, //配置信息是否处于
      configLoading2:true,
      activeOrgId: "",
      activeOrginfo: {}, //活动的企业信息
      myOrgInfo:{},//我的登录企业信息
      totalDeviceInfo:{},//设备统计信息
      DStatelist:[],
      isShowDStateSerch:false
    };
  },
  watch: {
    '$route.query': {
      handler(newVal, oldVal) {
        const { id, name } = this.$route.query;
        this.deviceForm.Online = id ? Number(id) : this.deviceForm.Online;
      },
      deep: true,
      immediate: true 
    },
  },
  async mounted() {
    this.configLoading = true;
    let Configres = await getConfigKey("device.runstate");
    if(Configres.data&&Configres.data!=='false'){
      this.isShowDStateSerch=Configres.data
      if(this.dict.type&&this.dict.type.device_run){
        this.DStatelist=this.dict.type.device_run
      }
    }else{
      this.isShowDStateSerch=false
    }
    try {
      // this.activeOrgId = this.$store.getters.orgId;
      let res = await orgInfo({ id: this.myOrgId });
      // console.log("我的企业信息", res);
      this.myOrgInfo= res.data
      this.activeOrginfo = res.data;
    } catch (error) {
      
    }
    await this.getCatetoryList();
    this.getDeviceStatistics()
    this.getDeviceList();
    
  },
  computed:{
    myOrgId(){
      return this.$store.getters.orgId
    }
  },
  methods: {
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.$refs.selectTree.handleClear();
      this.deviceForm.RoomCategory = '';
      this.deviceForm.RoomId = '';
      this.resetForm("deviceForm");
      this.deviceForm.pageNum=1
      this.deviceForm.Online = undefined
      this.deviceForm.DState = undefined
      this.getDeviceList();
    },
    getDeviceStatistics(orgId) {
      //获取设备统计信息
      let query={}
      if(orgId){
        query.orgId=orgId
      }else{
        query.orgId=this.myOrgId
      }
      if(this.isShowDStateSerch&&this.dict.type.device_run&&this.dict.type.device_run.length>0){
        query.StateList=this.dict.type.device_run.map(row=>row.value)
      }
      devRunStatisticsInfo(query).then((res) => {
        // console.log("设备统计信息res", res);
        let data = res.data;
        this.totalDeviceInfo=res.data
        // this.totalDeviceInfo = JSON.parse(JSON.stringify(data))
      });
    },
    selectRoomCatetoryTree(val){//选择车间分类或者车间后
      this.roomval=JSON.parse(JSON.stringify(val))
    },
    async switchMyOrgers(){
      this.activeOrginfo=JSON.parse(JSON.stringify(this.myOrgInfo))
      this.getDeviceStatistics(this.activeOrginfo.Id)
      await this.getCatetoryList();
      this.getDeviceList()
    },
    switchOrgers() {
      //打开切换客户
      this.$refs.kfDlg.openAgentDialog();
    },
    async onTargetChange(val) {//选择切换客户
      let res = await orgInfo({ id: val.OrgId });
      this.activeOrginfo = res.data;
      this.getDeviceStatistics(this.activeOrginfo.Id)
      await this.getCatetoryList();
      this.getDeviceList()
    },
    async getCatetoryList() {
      //获取设备分类列表
      this.configLoading = true;
      let res = await roomCatetoryTree({ orgid: this.activeOrginfo.Id });
      try {
        if (res.code == 0) {
          let lists = [];
          lists = JSON.parse(JSON.stringify(res.data));
          let allroom=await this.loadDeviceRoomList({ TargetOrgId: this.activeOrginfo.Id })
          let nocateRoom=allroom.filter(row=>row.CategoryId==''||row.CategoryId==null)
          this.roomTreeList = await this.initCatetoryRoomTree(lists,allroom,nocateRoom); //选择分类时分类树
          this.configLoading = false;
        }
      } catch (error) {
        // console.log("error报错了",error);
      }
    },
    async initCatetoryRoomTree(nodes,allroom,nocateRoom) {
      for (let idx = 0; idx < nodes.length; idx++) {
        let curnode = nodes[idx];
        nodes[idx].TreeId = curnode.Id;
        nodes[idx].TreeName = curnode.Name;
        nodes[idx].TreeType = "catetory";
        let rooms = allroom.filter(row=>row.CategoryId==curnode.Id);
        if (curnode.hasOwnProperty("Children") &&curnode.Children &&curnode.Children.length > 0) {
          let curnodeChildren = JSON.parse(JSON.stringify(curnode.Children));
          curnode.Children = await this.initCatetoryRoomTree(curnodeChildren,allroom);
          curnode.Children = [...curnode.Children, ...rooms];
        }else{
          curnode.Children =rooms
        }
      }
      if(nocateRoom&&nocateRoom.length>0){
        nodes=[...nodes,...nocateRoom]
      }
      return nodes;
    },
    async loadDeviceRoomList(activeCategoryInfo) {
      //加载车间列表
      try {
        let response = await deviceRoomList({
          TargetOrgId: activeCategoryInfo.TargetOrgId,
        });
        if (response.data && response.data.length > 0) {
          let roomTableData=response.data.map(row=>{
            row.TreeId= row.Id
            row.TreeName= row.Name
            row.TreeType= "room"
            return row
          })
          return roomTableData;
        } else {
          return [];
        }
      } catch (error) {
        // console.log(error,'errorerror');
        return [];
      }
    },
    getDeviceList() {
      //获取设备列表
      this.configLoading2 = true;
      if(this.activeOrginfo.Id==this.$store.getters.orgId){
        delete this.deviceForm.TargetOrgId
      }else{
        this.deviceForm.TargetOrgId=this.activeOrginfo.Id
      }
      if(this.roomval.TreeId){
        if(this.roomval.TreeType=="catetory"){
          delete this.deviceForm.RoomId
          this.deviceForm.RoomCategory=this.roomval.TreeId
          this.deviceForm.TargetOrgId=this.roomval.TargetOrgId
        }
        if(this.roomval.TreeType=="room"){
          this.deviceForm.RoomId=this.roomval.TreeId
          this.deviceForm.RoomCategory=this.roomval.CategoryId
          this.deviceForm.TargetOrgId=this.roomval.TargetOrgId
        }
      }else{
        delete this.deviceForm.RoomCategory
        delete this.deviceForm.RoomId
      }
      console.log(this.deviceForm)
      myDeviceList(this.deviceForm)
        .then(async (response) => {
          // console.log("查询到的设备", response);
          this.tableData = response.data.List;
          this.total = response.data.Total;
          this.configLoading2 = false;
        })
        .catch((err) => {
          this.configLoading2 = false;
        });
    },
    toDeviceDetails(row) {
      this.$router.push({
        path: "/iot/deviceManage/deviceDetail",
        query: { id: row.Id, isCustom: true },
      });
    },
  },
};
</script>

<style lang="scss" scoped>
.device_con {
  display: flex;
  justify-content: space-between;
  .device_con_left {
    width: 20%;
    display: flex;
    justify-content: flex-start;
    flex-direction: column;
    align-items: center;
    border: 1px solid #f6f6f6;
    font-size: 14px;
    .left_top {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 10px;
      width: 100%;
      box-sizing: border-box;
      color: #333333;
      height: 50px;
      background: #f6f9ff;
      .top_btn{
        display: flex;
        justify-content: flex-end;
        align-items: center;
        .handle_my {
          font-size: 14px;
          margin-right: 10px;
          color: #2878ff;
          cursor: pointer;
        }
      }
      .change_btn {
        width: 50px;
        height: 28px;
        color: #fff;
        display: flex;
        justify-content: center;
        align-items: center;
        background: #3572ff;
        border-radius: 15px;
        line-height: 28px;
      }
    }
    .expand {
      width: 100%;
      text-align: right;
      color: #96a0a2;
      padding: 12px 10px 0;
    }
    .custom_info {
      display: flex;
      flex-direction: column;
      align-items: center;
      width: 100%;
      padding: 0 5px;
      box-sizing: border-box;
      padding-top: 12px;
      .custom_logo {
        width: 120px;
        height: 120px;
        border-radius: 50%;
        .logo {
          width: 120px;
          height: 120px;
          border-radius: 50%;
        }
      }
      .custom_name {
        margin-top: 20px;
        color: #333;
        &.hui {
          color: #4b4b4b;
          margin-top: 10px;
        }
      }
      .line {
        margin-top: 30px;
        width: 100%;
        height: 1px;
        background: -webkit-linear-gradient(
          left,
          #fff 0%,
          #dddddd 50%,
          #fff 100%
        );
      }
    }
    .device_total {
      display: flex;
      justify-content: flex-start;
      align-items: flex-start;
      flex-wrap: wrap;
      margin-top: 20px;
      width: 100%;
      .total_li {
        display: flex;
        flex-direction: column;
        align-items: center;
        font-size: 14px;
        color: #333;
        width: 50%;
        margin-bottom: 10px;
        .li_top {
          display: flex;
          justify-content: center;
          align-items: center;
          .samll_block {
            width: 8px;
            height: 8px;
            border-radius: 2px;
            background: #9e9e9e;
            margin-right: 2px;
            &.green {
              background: #00afaa;
            }
            &.red {
              background: #e93030;
            }
            &.yellow {
              background: #e0b309;
            }
          }
        }
        .li_bottom {
          margin-top: 3px;
          .unit {
            color: #9a9acf;
            margin-left: 2px;
          }
        }
      }
    }
  }
}
.PlanList-table {
  width: 80%;
  padding: 0 0 0 20px;
}
</style>
