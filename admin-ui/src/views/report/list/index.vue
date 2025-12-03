<template>
  <div class="app-container">
    <div class="header-query">
      <el-form ref="queryForm" :model="queryParams" :inline="true">
        <el-form-item>
          <el-input v-model="queryParams.Key" placeholder="请输入搜索的关键字" clearable />
        </el-form-item>
        <el-form-item>
          <el-select v-model="queryParams.Status" clearable placeholder="请选择状态">
            <el-option label="正常" value="0"></el-option>
            <el-option label="停用" value="1"></el-option>
            <el-option label="已发布" value="2"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" @click="getList">搜索</el-button>
        </el-form-item>
      </el-form>
      <div v-hasPermi="['/ReportService/Report/Add']">
        <el-button type="primary" icon="el-icon-plus" @click="add('')">新增</el-button>
      </div>
    </div>
    <div style="background-color: rgb(255, 255, 255);">
      <div class="content" v-loading="loading">
        <groupManage @clickNode="clickNode"></groupManage>
        <div class="content__item-list">
          <div class="content__item" v-for="item in screenList" :key="item.Id">
            <el-card shadow="hover" :body-style="{ padding: '0px' }">
              <img v-if="item.Thumbnail !== ''" :src="item.Thumbnail" class="image" style="width: 100%;height: 156px;"
                @click="view(item.Id)" />
              <div v-else @click="viewReport(item.Id)">
                <el-empty :imageSize="113" />
              </div>
              <div style="padding: 2px 10px;display: flex;justify-content: space-between;align-items: center;">
                <span class="content__name">{{ item.Name }}</span>
                <div style="display: flex; align-items: center;">
                  <el-switch @change="releaseStatus(item, $event)" v-model="item.Status" active-color="#13ce66"
                    inactive-color="#DCDFE6" class="switchStyle" active-value="2" inactive-value="0" active-text="已发布"
                    inactive-text="未发布">
                  </el-switch>
                  <el-dropdown>
                    <span class="el-dropdown-link">
                      <img style="width: 48px;height: 48px;" src="@/assets/images/more.png" alt="">
                    </span>
                    <el-dropdown-menu slot="dropdown">
                      <el-dropdown-item @click.native="edit(item)" v-hasPermi="['/ReportService/Report/Edit']">
                        <i class="el-tooltip el-icon-edit" style="cursor: pointer"> 编辑</i>
                      </el-dropdown-item>
                      <el-dropdown-item @click.native="add(item)" v-hasPermi="['/ReportService/Report/Edit']">
                        <i class="el-tooltip el-icon-connection" style="cursor: pointer"> 分组</i>
                      </el-dropdown-item>
                      <el-dropdown-item @click.native="copy(item)" v-hasPermi="['/ReportService/Report/Add']">
                        <i class="el-tooltip el-icon-document-copy" style="cursor: pointer"> 复制</i>
                      </el-dropdown-item>
                      <el-dropdown-item @click.native="del(item.Id)" v-hasPermi="['/ReportService/Report/Remove']">
                        <i class="el-tooltip el-icon-delete" style="cursor: pointer"> 删除</i>
                      </el-dropdown-item>
                      <el-dropdown-item @click.native="share(item.Id, item.Name, item.ReportType)">
                        <i class="el-tooltip el-icon-share" style="cursor: pointer"> 分享</i>
                      </el-dropdown-item>
                      <el-dropdown-item @click.native="release(item)">
                        <i :class="'el-tooltip ' + (item.Status == '2' ? 'el-icon-star-on' : 'el-icon-star-off')"
                          style="cursor: pointer"> 发布</i>
                      </el-dropdown-item>
                    </el-dropdown-menu>
                  </el-dropdown>
                </div>
              </div>
            </el-card>
          </div>
          <div v-if="screenList.length === 0 && !loading" class="empty-list">
            <el-empty description="暂无报表数据">
              <el-button type="primary" icon="el-icon-plus" @click="add('')" v-hasPermi="['/ReportService/Report/Add']">
                新增报表
              </el-button>
            </el-empty>
          </div>

        </div>
      </div>
      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize"
        @pagination="getList" :pageSizes="[12, 24, 36, 48, 60]" />
    </div>
    <!-- 数据大屏保存参数配置对话框 -->
    <add-screen ref="addScreen" :title="title" :dialog-visible="open" @cancelForm="cancelForm" @getList="getList" />
    <!-- 数据大屏分享对话框 -->
    <share-screen ref="shareScreen" :dialog-visible="shareOpen" :title="`分享“ ${shareScreenName} ”`"
      @cancelForm="cancelForm" @getList="getList" />
  </div>
</template>
<script>
import { rptList, rptCopy, deleteRpt } from "@/api/report/report";
import { editRpt } from "@/api/report/report";
import groupManage from "./groupManage";
import addScreen from "./addScreen";
import shareScreen from "./shareScreen";

export default {
  name: "ReportList",
  components: {
    addScreen,
    shareScreen,
    groupManage
  },
  filters: {
    switchFilter(status) {
      const statusMap = {
        0: false,
        2: true,
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      // 遮罩层
      loading: true,
      // 总条数
      total: 0,
      // 数据大屏表格数据
      screenList: [],
      title: '新增',
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 12,
        Key: '',
        GroupId: '',
        Status: undefined,
      },
      // 是否显示弹出层
      open: false,
      shareOpen: false,
      shareScreenName: ''
    }
  },
  created() {
    this.getList();
  },
  methods: {
    /** 查询数据大屏列表 */
    getList() {
      this.open = false;
      this.loading = true;
      rptList(this.queryParams).then(response => {
        this.screenList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      })
    },
    // 点击树形返回值
    clickNode(data) {
      this.queryParams.GroupId = data.Id;
      this.getList();
    },
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 大屏模板添加 */
    add(data) {
      if (data === '') {
        this.title = '新增';
        this.$refs['addScreen'].ruleForm = {
          Name: '',
          DesInfo: '',
          DeviceType: 'pc',
          GroupId: '',
          reportType: 'screen'
        };
      } else {
        this.title = '编辑';
        this.$refs['addScreen'].ruleForm = {
          Id: data.Id,
          Name: data.Name,
          DesInfo: data.DesInfo,
          DeviceType: data.DeviceType,
          GroupId: data.GroupId ? data.GroupId : '',
          reportType: data.ReportType
        };
      }
      this.open = true;
    },
    view(id) {
      const viewRuter = this.$router.resolve({ path: "datav/datavRelease", query: { screenId: id } })
      window.open(viewRuter.href, '_blank')
    },
    viewReport(id) {
      const viewRuter = this.$router.resolve({ path: "spreadSheet/viewDataReport", query: { screenId: id } })
      window.open(viewRuter.href, '_blank')
    },
    copy(item) {
      this.$confirm("是否复制“" + item.Name + "”", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        rptCopy(item.Id).then(res => {
          this.$message.success("复制成功")
          this.getList()
        })
      })
    },
    release(item) {
      if (item.Status === '2') {
        this.$message("该大屏已发布")
        return false
      } else {
        this.$confirm('是否发布“' + item.Name + '”?', "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning"
        }).then(() => {
          let params = { Status: 2, Id: item.Id }
          editRpt(params).then(response => {
            this.getList();
          })
        })
      }
    },
    releaseStatus(item, e) {
      const text = e == 0 ? '取消发布' : '发布'
      this.$confirm('是否' + text + '“' + item.Name + '”?', "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        let params = { Status: e, Id: item.Id }
        editRpt(params).then(response => {
          this.getList();
        })
      }).catch(() => {
        item.Status = e == 0 ? 2 : 0
      })
    },
    // 取消按钮
    cancelForm() {
      this.shareOpen = false;
      this.open = false;
    },
    edit(item) {
      this.loading = true;
      setTimeout(() => {
        if (item.ReportType === 'screen') {
          this.$router.push({ path: 'datav/datavDraw', query: { screenId: item.Id } });
        } else {
          this.$router.push({ path: 'spreadSheet', query: { screenId: item.Id } });
        }
        this.loading = false;
      }, 50);
    },
    del(id) {
      this.$confirm('是否确认删除报表项?', "警告", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(function () {
        return deleteRpt(id);
      }).then(() => {
        this.getList();
        this.$message.success('删除成功!')
      }).catch(function () { });
    },
    share(id, name, ReportType) {
      this.$refs['shareScreen'].ruleForm = {
        ReportId: id,
        EffectiveTime: '',
        link: '',
        ReportType: ReportType,
        timerStatus: 1,//是否启用推送
        noticeWay: [],//推送通知方式
        noticeUserType: 0,//0人员//1角色
        timerCron: '',
        UsingPassword: ''
      }
      this.shareOpen = true;
      this.shareScreenName = name;
    }
  }
};
</script>

<style lang="scss" scoped>
::v-deep {
  .el-dialog__header {
    border-bottom: 1px solid #ccc;
  }

  .el-select {
    width: 100%;
  }

  .inputColor {
    color: #000;
  }

  .el-icon-star-on {
    color: #fcbc40;
  }

  .effectSpan .el-input__inner {
    color: #FFFFFF;
  }

  .el-card {
    width: 280px;
    margin: 0 6px 12px 6px;
  }

  .el-empty {
    padding: 0;
  }
}

::v-deep .el-switch__label {
  position: absolute;
  display: none;
  color: #fff;
}

/*打开时文字位置设置*/
::v-deep .el-switch__label--right {
  z-index: 1;
  right: 20px;
}

/*关闭时文字位置设置*/
::v-deep .el-switch__label--left {
  z-index: 1;
  left: 20px;
}

/*显示文字*/
::v-deep .el-switch__label.is-active {
  display: block;
}

::v-deep .el-switch .el-switch__core,
.el-switch .el-switch__label {
  width: 72px !important;
}

.content {
  padding: 15px;
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
}

.content__item-list {
  width: calc(100% - 21%);
  display: flex;
  align-items: center;
  flex-wrap: wrap;
}

.content__name {
  width: 100px;
  font-size: 14px;
  font-weight: bold;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.el-tooltip {
  color: #3DD2B4;
}

.tips {
  font-size: 12px;
  color: rgb(75, 75, 75);
}

.el-tag+.el-tag {
  margin-right: 10px;
}

.el-tag--medium {
  margin-right: 10px;
}

.button-new-tag {
  margin-right: 10px;
  height: 32px;
  line-height: 30px;
  padding-top: 0;
  padding-bottom: 0;
}

.input-new-tag {
  width: 90px;
  margin-right: 10px;
  vertical-align: bottom;
}
.empty-list {
  width: 100%;
  padding: 60px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  
}
</style>