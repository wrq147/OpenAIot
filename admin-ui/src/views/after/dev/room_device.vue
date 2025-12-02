<template>
  <div>
    <el-dialog title="房间内的设备列表" :visible.sync="roomDeviceOpen" center width="1000px" :close-on-click-modal="false">
      <div>
        <div class="from_con" id="from_con" style="margin-bottom: 0; padding-bottom: 0; padding-left: 0">
          <el-form :model="deviceForm" ref="deviceForm" :inline="true" class="biaodan">
            <!-- <el-form-item label="设备名称" prop="name">
              <el-input v-model="deviceForm.Name" placeholder="请输入设备名称"/>
            </el-form-item> -->
            <el-form-item label="联网状态" prop="status">
              <el-select class="set_radius" v-model="deviceForm.Online" placeholder="设备联网状态" clearable>
                <el-option v-for="dict in statusList" :key="dict.value" :label="dict.label" :value="dict.value"/>
              </el-select>
            </el-form-item>
            <el-form-item class="submit_button_con">
              <el-button icon="el-icon-refresh" @click="resetLoad">重置</el-button>
              <el-button type="primary" icon="el-icon-search" @click="getDeviceList(false)">搜索</el-button>
            </el-form-item>
          </el-form>
        </div>
        <el-row :gutter="10" class="mb8 button_row" style="margin-top:10px">
          <el-col :span="1.5">
            <el-button type="primary" plain @click="openAddDevice">
              <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
              <span style="margin-left: 6px">加入设备</span>
            </el-button>
          </el-col>
        </el-row>
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
                    <div class="airCompressors-item-top-see" @click="toDeleteDevice(its)">
                        <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                    <span class="airCompressors-item-top-see-txt">移除</span>
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
        <div class="empty_li" v-else-if="configLoading != 'loading'">
          暂无数据
        </div>
        <pagination v-show="total > 0" :total="total" :page.sync="deviceForm.pageNum" :limit.sync="deviceForm.pageSize" :pageSizes="pageSizes" @pagination="getDeviceList()"/>
      </div>
      <!-- <div slot="footer" class="dialog-footer">
        <el-button @click="roomDeviceOpen = false">取 消</el-button>
        <el-button type="primary" @click="addRoomDevice">确 定</el-button>
      </div> -->
    </el-dialog>
    <el-dialog width="960px" title="请选择分配的设备" :visible.sync="deviceOpen" append-to-body>
      <el-form :model="deviceQuery" ref="deviceForm" :inline="true" style="display: flex; justify-content: space-between">
        <div>
          <el-form-item label="查询关键字" prop="Key">
            <el-input v-model="deviceQuery.Key" placeholder="请输入设备名称、通讯编码或编号" clearable></el-input>
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker class="form_input_style" v-model="devDateRange" style="width: 232px" value-format="yyyy-MM-dd" type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
        </div>

        <el-form-item>
          <el-button icon="el-icon-refresh" @click="resetSelectDevice">重置</el-button>
          <el-button type="primary" icon="el-icon-search" @click="getDeviceList(true)">搜索</el-button>
        </el-form-item>
      </el-form>
      <el-table ref="devTable" :data="selectdeviceList" tooltip-effect="dark" v-loading="deviceLoading"
        style="width: 100%" @selection-change="onDeviceChange" @row-click="clickDevRow" @select="devBoxSelect" row-key="Id">
        <el-table-column type="selection" width="55"> </el-table-column>
        <el-table-column prop="DeviceNumber" label="设备编号" align="center" width="150">
        </el-table-column>
        <el-table-column prop="DeviceId" label="通讯编码" align="center" width="150">
        </el-table-column>
        <el-table-column label="预览图片" align="center" width="150">
          <template slot-scope="scope">
            <div class="imgwrap">
              <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'" :preview-src-list="[scope.row.PhotoUrl]"></el-image>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="Name" label="设备名称"> </el-table-column>
        <el-table-column prop="ProductName" label="所属产品"></el-table-column>
      </el-table>
      <pagination v-show="deviceTotal > 0" :total="deviceTotal" :page.sync="deviceQuery.pageNum" :limit.sync="deviceQuery.pageSize" @pagination="getDeviceList(true)"/>
      <div slot="footer" class="dialog-footer">
        <el-button @click="deviceOpen = false">取 消</el-button>
        <el-button type="primary" @click="addRoomDevice">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { myDeviceList } from "@/api/after/dev";
import { removeRoomDevice,addRoomDevice  } from "@/api/after/room";
export default {
  name: "AdminUiRoomDevice",
  props: {
    // roomval:{
    //     type:Object,
    //     default:()=>{
    //         return []
    //     }
    // }
  },
  data() {
    return {
      pageSizes: [9, 18, 27, 36],
      defaultProps: {
        children: "Children",
        label: "TreeName",
      },
      tableData: [],//房间里的设备
      total: 0,
      configLoading: false,
      roomDeviceOpen: false,
      roomval: {},
      deviceForm: {
        pageNum: 1,
        pageSize: 9,
        Online: null,
        // type: null,
      }, //设备查询form
      statusList: [
        { label: "离线", value: 0 },
        { label: "在线", value: 1 },
        { label: "未知", value: 2 },
      ],
      selectdeviceList: [], //选择设备
      deviceLoading: false,
      deviceQuery: {
        pageNum: 1,
        pageSize: 20,
        Key: "",
      },
      deviceTotal: 0,
      devDateRange:[],//时间日期
      afterSelectDevice:[],
      deviceOpen:false
    };
  },

  mounted() {},

  methods: {
    onDeviceChange(val) {
        this.afterSelectDevice=JSON.parse(JSON.stringify(val))
    },
    devBoxSelect(arr, row) {
      //点击多选框
      const selected = this.afterSelectDevice.some(
        (item) => item.Id === row.Id
      );
      if (selected) {
        this.afterSelectDevice = this.afterSelectDevice.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    clickDevRow(row) {
      const selected = this.afterSelectDevice.some(
        (item) => item.Id === row.Id
      );
      if (!selected) {
        // 选择
        this.$refs.devTable.toggleRowSelection(row, true);
      } else {
        // 取消
        this.$refs.devTable.toggleRowSelection(row, false);
        this.afterSelectDevice = this.afterSelectDevice.filter(
          (rw) => rw.Id !== row.Id
        );
      }
    },
    openAddDevice() {
      //添加设备进房间
      this.deviceOpen=true
      this.deviceQuery={
        pageNum: 1,
        pageSize: 20,
      },
      this.getDeviceList(true)
    },
    toDeleteDevice(row) {
        let that=this
      this.$modal
        .confirm('是否确认删除名为"' + row.Name + '"的房间？')
        .then(function (res) {
            let data2=[{roomId:that.roomval.Id,deviceId:row.Id}]
          return removeRoomDevice(data2);
        })
        .then(() => {
          this.deviceForm.pageNum = 1;
          this.getDeviceList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch((err) => {
            console.log(err,'errerr');
        });
    },
    resetLoad() {
      //重置
      this.deviceForm.Name = "";
      this.deviceForm.Online = null;
      this.deviceForm.pageNum = 1;
      this.getDeviceList();
    },
    resetSelectDevice(){
        this.deviceQuery.Key=''
        this.devDateRange=[]
        this.deviceQuery.pageNum=1
        this.getDeviceList(true);
    },
    addRoomDevice() {
      //添加房间的设备
      let addParams=[]
    //   this.afterSelectDevice.map(row=>{
    //     let obj={
    //         roomId:this.roomval.Id,
    //         deviceId:row.Id
    //     }
    //     addParams.push(obj)
    //   })
      addParams=this.afterSelectDevice.map(row=>{
        let obj={
            roomId:this.roomval.Id,
            deviceId:row.Id
        }
        return obj
      })
      addRoomDevice(addParams).then(res=>{
        this.$modal.msgSuccess("加入成功");
        this.getDeviceList()
        this.deviceOpen=false
      }).catch(err=>{
        console.log("err",err);
      })
    },
    openDialog(data) {
      //打开
      this.roomval = JSON.parse(JSON.stringify(data));
      this.getDeviceList();
      this.roomDeviceOpen = true;
    },
    getDeviceList(isSel) {
      //获取设备列表

      let deviceForm = {};
      if (isSel) {
        this.deviceLoading = true;
        // deviceForm = JSON.parse(JSON.stringify(this.deviceQuery));
        deviceForm=this.addDateRange(this.deviceQuery, this.devDateRange)
        deviceForm.FilterRoom=true
      } else {
        this.configLoading = true;
        deviceForm = JSON.parse(JSON.stringify(this.deviceForm));
        deviceForm.RoomId=this.roomval.Id
        deviceForm.RoomCategory=this.roomval.CategoryId
        deviceForm.TargetOrgId=this.roomval.TargetOrgId
      }

      myDeviceList(deviceForm).then((response) => {
          //   console.log("查询到的设备", response);

          if (isSel) {
            this.selectdeviceList = response.data.List;
            this.deviceTotal = response.data.Total;
          } else {
            this.tableData = response.data.List;
            this.total = response.data.Total;
          }
          this.configLoading = false;
          this.deviceLoading = false;
        })
        .catch((err) => {
          this.configLoading = false;
          this.deviceLoading = false;
        });
    },
  },
};
</script>
<style lang="scss" scoped>
.empty_li {
  width: 100%;
  text-align: center;
  font-size: 16px;
  color: #999999;
  margin-top: 60px;
  margin-bottom: 60px;
}
.airCompressors {
  display: flex;
  justify-content: left;
  width: 100%;
  flex-wrap: wrap;
  align-items: center;
  margin-right: -15px;
  margin-top: -10px;
  .airCompressors-item:nth-child(3n) {
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
