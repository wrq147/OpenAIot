<template>
    <div class="app-container">
        <div class="header-query">
            <el-form ref="queryForm" :model="queryParams" :inline="true">
                <el-form-item>
                    <el-input v-model.trim="queryParams.SearchKey" clearable placeholder="请输入搜索关键字" />
                </el-form-item>
                <!-- <el-form-item>
                    <el-select v-model="queryParams.Id" clearable placeholder="请选择大屏报表">
                    <el-option v-for="d in screenList" :key="d.Id" :label="d.Name" :value="d.Id" />
                    </el-select>
                </el-form-item> -->
                <el-form-item>
                   <el-date-picker v-model="time" type="daterange" value-format="yyyy-MM-dd" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="el-icon-search" @click="getList">搜索</el-button>
                </el-form-item>
            </el-form>
            <div>
              <el-button type="danger" icon="el-icon-delete" @click="remove('')">批量删除</el-button>
            </div>
        </div>
        <div style="background-color: rgb(255, 255, 255);">
            <el-table v-loading="loading" stripe :data="list"  @selection-change="handleSelectionChange">
                <el-table-column type="selection" align="center" width="50" />
                <el-table-column label="序号" type="index" align="center" width="80px" sortable :show-overflow-tooltip="true" />
                <el-table-column label="创建时间" prop="createTime" align="center" />
                <el-table-column label="大屏名称" prop="ReportName" align="center" />
                <el-table-column label="推送时间" prop="TimerStatus" align="center" >
                  <template  v-slot="scope">
                    <div>
                      <el-tag type="info" v-if="scope.row.TimerStatus==1">不推送</el-tag>
                      <div v-else>
                        <el-tag type="success">{{scope.row.CronName}}</el-tag>
                      </div>
                    </div>
                  </template>
                </el-table-column>
                <el-table-column label="有效时间" align="center" width="80">
                  <template  v-slot="scope">
                    <div>
                       {{ scope.row.EffectiveTime === 0 ? '永久' : scope.row.EffectiveTime + '天' }}
                    </div>
                  </template>
                </el-table-column>
                <el-table-column label="到期时间" prop="ExpirationTime" align="center" />
                <el-table-column label="操作" width="200" align="center">
                    <template v-slot="scope">
                        <el-button size="mini" type="text" icon="el-icon-edit-outline" @click="editShare(scope.row)">编辑</el-button>
                        <el-button size="mini" type="text" icon="el-icon-copy-document" class="copy-button" @click="handleView(scope.row)">复制链接</el-button>
                        <el-button size="mini" type="text" icon="el-icon-delete" style="color:red;" @click="remove(scope.row.Id)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="total>0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
        </div>
        <share-screen ref="shareScreen" :dialog-visible="shareOpen" :title="`分享“ ${shareScreenName} ”`" @cancelForm="cancelForm" @getList="getList" />
    </div>
</template>
<script>
import { rptList } from "@/api/report/report";
import { shareList, shareRemove,infoShare } from "@/api/report/share";
import shareScreen from "@/views/report/list/shareScreen";
export default {
  components: {
    shareScreen
  },
  data() {
    return {
      // 搜索参数
      queryParams: {
        SearchKey: '',
        beginTime: '',
        endTime: '',
        pageNum: 1,
        pageSize: 20,
        showAll: true
      },
      time: [],
      // 报表数据
      list: [],
      screenList: [],
      ids: [],
      // 总数
      total: 0,
      shareScreenName:'',
      shareOpen:false,
      loading:false,
    }
  },
  created() {
    // this.dataVList()
    this.getList();
  },
  methods: {
    cancelForm() {
      this.shareOpen = false;
    },
    editShare(row) {
      
      infoShare(row.Id).then(res=>{
        console.log(res.data);
        let item=res.data
        // console.log("编辑行的数据",item);
        let nameList=[]
        let avatarList=[]
        if(item.NoticeUserType==0&&item.NoticeUserList&&item.NoticeUserList.length>0){
          nameList=item.NoticeUserList.map(rw=>rw.RealName)
          avatarList=item.NoticeUserList.map(rw=>rw.Avatar)
        }else if(item.NoticeUserType==1&&item.NoticeRoleList&&item.NoticeRoleList.length>0){
          nameList=item.NoticeRoleList.map(rw=>rw.roleName)
        }
        console.info(item)
        let link = '';
        if (item.ReportType === 'table') {
          link = "http://"+window.location.host+"/#/report/spreadSheet/viewDataReport?tokenId="+item.Id
        } else {
          link = "http://"+window.location.host+"/#/report/datav/datavRelease?tokenId="+item.Id
        }
        this.$refs['shareScreen'].ruleForm = {
          ReportId: item.ReportId,
          EffectiveTime: item.EffectiveTime,
          link: link,
          timerStatus:item.TimerStatus?parseInt(item.TimerStatus):item.TimerStatus,//是否启用推送
          noticeWay:item.NoticeWay.split(','),//推送通知方式
          noticeUserType:item.NoticeUserType,//0人员//1角色
          timerCron:item.TimerCron,
          cronName:item.CronName,
          noticeUsers:item.NoticeUsers,
          noticeUsersName:nameList,
          noticeUsersAvatar:avatarList,
          id:item.Id,
          UsingPassword:item.UsingPassword
        }
        if(item.noticeUserType==0&&item.NoticeUserList&&item.NoticeRoleList.length>0){
          this.$refs['shareScreen'].noticeUsers =item.NoticeUserList.map(rw=>{
            return {
              id:rw.Id,
              name:rw.RealName,
              avatar: rw.Avatar,
              type: "user",
            }
          })
        }else if(item.noticeUserType==0&&item.NoticeRoleList&&item.NoticeRoleList.length>0){
          this.$refs['shareScreen'].noticeUsers =item.NoticeRoleList.map(rw=>{
            return {
              id:rw.roleId,
              name:rw.roleName,
              type: "role",
            }
          })
        }
        
        this.shareOpen = true;
        this.shareScreenName = item.ReportName;
        this.$forceUpdate()
      })
      
    },
    /**
     * 获取数据
     */
    getList(query) {
      if(query&&query=='close'){
        this.shareOpen = false;
      }
      this.loading = true
      this.queryParams.beginTime = this.time[0] !==undefined ? this.time[0] : ''
      this.queryParams.endTime = this.time[1] !==undefined ? this.time[1] : ''
      shareList(this.queryParams).then(res => {
        this.list = res.data.List
        this.total = res.data.Total;
        this.loading = false
      })
    },
    // 获取大屏报表数据
    dataVList() {
        rptList({ pageNum: 1,  pageSize: 999}).then(response => {
        this.screenList = response.data.List;
      })
    },
    handleView(row) {
      let link = '';
      if (row.ReportType === 'table') {
        link = "http://"+window.location.host+"/#/report/spreadSheet/viewDataReport?tokenId="+row.Id
      } else {
        link = "http://"+window.location.host+"/#/report/datav/datavRelease?tokenId="+row.Id
      }
        const input = document.createElement('input');
        input.value = link; // 设置要复制的文本
        document.body.appendChild(input); // 添加input元素到DOM
        input.select(); // 选中文本
        document.execCommand('copy'); // 执行复制操作
        document.body.removeChild(input); // 移除input元素
        this.$message.success("复制成功")
    },
    remove(id) {
      if (id === '' && this.ids.length < 1) {
        this.$message.error('请选择要删除的数据')
        return
      }
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        shareRemove({ ids: id == '' ? this.ids : id }).then(res => {
          this.$message.success('删除成功!')
          this.getList()
        })
      }).catch(() => {})
    },
    handleSelectionChange(val) {
      this.ids = []
      this.ids = val.map(item => item.Id)
    }
  }
}
</script>