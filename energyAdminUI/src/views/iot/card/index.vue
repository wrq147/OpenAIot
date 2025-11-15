<template>
  <div style="padding:10px 10px 0 10px" id="big_con">
    <div>
      <el-row :gutter="20">
        <!--产品数据-->
        <el-col :span="24" :xs="24">
          <div class="from_con" id="from_con" v-show="showSearch">
            <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="物联卡关键词" prop="Name">
                <el-input class="set_radius" v-model="queryParams.Key" placeholder="请输入物联卡关键词" clearable
                  @keyup.enter.native="handleQuery" />
              </el-form-item>
              <el-form-item label="设备关键词" prop="Name">
                <el-input class="set_radius" v-model="queryParams.DeviceKey" placeholder="请输入设备关键词" clearable
                  @keyup.enter.native="handleQuery" />
              </el-form-item>
              <el-form-item label="状态" prop="Status">
                <el-select class="set_radius" v-model="queryParams.Status" placeholder="网卡状态" clearable
                  @change="changeStatus">
                  <el-option :label="item.text" :value="item.value" v-for="item in cardStatusList" :key="item.value" />
                </el-select>
              </el-form-item>
              <el-form-item label="卡来源" prop="CardFrom">
                <el-select class="set_radius" v-model="queryParams.CardFrom" placeholder="过滤卡来源" clearable>
                  <el-option label="SimBoss" value="SimBoss" />
                  <el-option label="硕软" value="Sohan" />
                  <el-option label="移动" value="YiDong" />
                  <el-option label="联通" value="Unicom" />
                  <el-option label="未知" value="Unknow" />
                </el-select>
              </el-form-item>
              <el-form-item label="是否绑定设备" prop="Status">
                <el-select class="set_radius" v-model="queryParams.IsBindDev" placeholder="是否绑定设备" clearable
                  @change="changeStatus">
                  <el-option label="是" :value="true" />
                  <el-option label="否" :value="false" />
                </el-select>
              </el-form-item>
              <el-form-item label="到期时间">
                <el-date-picker class="set_radius" v-model="dateRange" style="width: 232px" value-format="yyyy-MM-dd"
                  type="daterange" range-separator="-" start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
              </el-form-item>
              <el-form-item class="submit_button_con">
                <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
                <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="elbiaoge_elform" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-row :gutter="10" class="mb8 button_row">
              <div>
                <el-col :span="1.5">
                  <el-button type="info" @click="handleImport" plain v-hasPermi="['/IoTService/IotCard/ListPage']">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left: 6px">SimBoss卡导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="info" @click="handleImport2" plain v-hasPermi="['/IoTService/IotCard/ListPage']">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left: 6px">移动卡导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="info" @click="handleImport3" plain v-hasPermi="['/IoTService/IotCard/ListPage']">
                    <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                    <span style="margin-left: 6px">硕软卡导入</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="success" plain :disabled="multiple" @click="handleRenew">
                    <span style="margin-left: 6px">批量续费</span>
                  </el-button>
                </el-col>
                <el-col :span="1.5">
                  <el-button type="danger" :disabled="multiple" icon="el-icon-delete" plain
                    @click="handleDelete('')">批量删除</el-button>
                </el-col>
              </div>
              <right-toolbar :showSearch.sync="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
            </el-row>

            <el-table v-loading="loading" border :data="productTableList" :row-style="isRed" :cell-style="isRed"
              @selection-change="handleSelectionChange" @sort-change="handleSortChange" class="data_table"
              :header-cell-style="cellSty" style="width: 100%" :fit="true">
              <el-table-column type="selection" width="55"> </el-table-column>
              <el-table-column label="ICCID号" align="center" key="ICCID" prop="ICCID" width="180"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <el-link type="primary" @click="viewInfo(scope.row)">
                    <span>{{ scope.row.ICCID }}</span></el-link>
                </template>
              </el-table-column>
              <el-table-column label="IMSI号" align="center" key="IMSI" prop="IMSI" width="140"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.IMSI }}</span>
                </template>
              </el-table-column>
              <el-table-column label="对应手机号" align="center" key="MSISDN" prop="MSISDN" width="140"
                v-if="columns[2].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.MSISDN }}</span>
                </template>
              </el-table-column>
              <el-table-column label="卡来源" align="center" key="CardFrom" prop="CardFrom" v-if="columns[3].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span v-if="scope.row.CardFrom == 'SimBoss'">SimBoss</span>
                  <span v-else-if="scope.row.CardFrom == 'Sohan'">硕软</span>
                  <span v-else-if="scope.row.CardFrom == 'YiDong'">移动</span>
                  <span v-else-if="scope.row.CardFrom == 'Unicom'">联通</span>
                  <span v-else>未知</span>
                </template>
              </el-table-column>
              <el-table-column label="网络限速值" align="center" key="speedLimit" prop="speedLimit" v-if="columns[4].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.speedLimit }}Kbps</span>
                </template>
              </el-table-column>
              <el-table-column label="运营商" align="center" key="Carrier" prop="Carrier" v-if="columns[5].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.Carrier }}</span>
                </template>
              </el-table-column>
              <el-table-column label="网卡类型" align="center" key="CardType" prop="CardType" v-if="columns[6].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{
                    scope.row.CardType == "POOL"
                      ? "流量池卡"
                      : scope.row.CardType == "SINGLE"
                        ? "单卡"
                        : "其他"
                  }}</span>
                </template>
              </el-table-column>
              <el-table-column label="流量池Id" align="center" key="CardPoolId" prop="CardPoolId" v-if="columns[7].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.CardPoolId }}</span>
                </template>
              </el-table-column>
              <el-table-column label="卡当前套餐名称" align="center" key="RatePlanId" prop="RatePlanId" width="150"
                v-if="columns[8].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.RatePlanName }}</span>
                </template>
              </el-table-column>
              <el-table-column label="卡套餐大小" align="center" key="TotalDataVolume" prop="TotalDataVolume" width="120"
                v-if="columns[9].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.TotalDataVolume }}M</span>
                </template>
              </el-table-column>
              <el-table-column label="卡套餐用量" align="center" key="UsedDataVolume" prop="UsedDataVolume" width="150"
                v-if="columns[10].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.UsedDataVolume
                  }}{{ scope.row.UsedCountAsVolume ? "次" : "MB" }}</span>
                </template>
              </el-table-column>
              <el-table-column label="激活时间" align="center" key="StartDate" prop="StartDate" width="150"
                v-if="columns[11].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.StartDate }}</span>
                </template>
              </el-table-column>
              <el-table-column label="到期时间" align="center" key="ExpirationDate" prop="ExpirationDate" width="150"
                sortable="custom" :sort-orders="['descending', 'ascending']" v-if="columns[12].visible"
                :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.ExpirationDate }}</span>
                </template>
              </el-table-column>
              <el-table-column label="使用的设备" align="center" key="UsingDeviceName" prop="UsingDeviceName" width="140"
                v-if="columns[13].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.UsingDeviceName }}</span>
                </template>
              </el-table-column>
              <el-table-column label="设备绑定时间" align="center" key="UsingOn" prop="UsingOn" width="150"
                v-if="columns[14].visible" :show-overflow-tooltip="true">
                <template slot-scope="scope">
                  <span>{{ scope.row.UsingOn }}</span>
                </template>
              </el-table-column>
              <el-table-column label="网卡状态" align="center" key="Status" v-if="columns[15].visible" width="100"
                :filters="cardStatusList" :filter-method="filterSatus" class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  {{ getStatusText(scope.row.Status) }}
                </template>
              </el-table-column>
              <el-table-column label="最后一次同步时间" align="center" prop="CreatedOn" width="150"
                class-name="small-padding fixed-width" v-if="columns[16].visible">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.LastSyncDate) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="创建时间" align="center" prop="CreatedOn" width="150"
                class-name="small-padding fixed-width" v-if="columns[17].visible">
                <template slot-scope="scope">
                  <span>{{ parseTime(scope.row.CreatedOn) }}</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" align="center" width="300" fixed="right"
                class-name="small-padding fixed-width">
                <template slot-scope="scope">
                  <el-button type="text" icon="el-icon-refresh" @click="handleSynchronous(scope.row)">立即同步</el-button>
                  <el-button type="text" icon="el-icon-video-pause" @click="handleUnActive(scope.row)"
                    v-if="scope.row.UsingDevice">取消绑定</el-button>
                  <el-button type="text" icon="el-icon-video-play" @click="handleActive(scope.row)"
                    v-else>绑定</el-button>

                  <el-button type="text" icon="el-icon-switch-button" @click="handleStop(scope.row)">停卡</el-button>

                  <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row.Id)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>

            <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
              :limit.sync="queryParams.pageSize" @pagination="getList" />
          </div>
        </el-col>
      </el-row>
      <!-- 物联网卡导入对话框 -->
      <el-dialog :close-on-click-modal="false" :title="upload.title" :visible.sync="upload.open" width="400px"
        append-to-body>
        <el-upload ref="uploadref" :limit="1" accept=".xlsx, .xls, .txt" :headers="upload.headers" :action="upload.url +
          '?updateSupport=' +
          upload.updateSupport +
          '&cardFrom=' +
          importType
          " :disabled="upload.isUploading" :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess"
          :auto-upload="false" drag>
          <i class="el-icon-upload"></i>
          <div class="el-upload__text">
            将文件拖到此处，或
            <em>点击上传</em>
          </div>
          <div class="el-upload__tip text-center" slot="tip">
            <div class="el-upload__tip" slot="tip">
              <el-checkbox v-model="upload.updateSupport" />是否更新已经存在的设备数据
            </div>
            <span style="line-height: 33px">仅允许导入xls、xlsx、txt格式文件。</span>
            <el-link type="primary" :underline="false" style="font-size: 12px; vertical-align: baseline"
              @click="onImportTemplate">下载模板</el-link>
          </div>
        </el-upload>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="submitFileForm">确 定</el-button>
          <el-button @click="upload.open = false">取 消</el-button>
        </div>
      </el-dialog>
      <!-- 添加或修改参数配置对话框 -->
      <el-dialog :close-on-click-modal="false" title="选择设备" :visible.sync="choiceDevicevisible" width="400px"
        append-to-body>
        <el-select v-model="cardDevice" filterable remote reserve-keyword placeholder="请输入关键词"
          :remote-method="remoteMethod" :loading="optionLoading" clearable @change="choiceDeviceVal">
          <el-option v-for="item in deviceOption" :key="item.Id" :label="item.Name" :value="item.Id"></el-option>
        </el-select>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="fisishBindDevice">确 定</el-button>
          <el-button @click="choiceDevicevisible = false">取 消</el-button>
        </div>
      </el-dialog>
      <el-dialog :close-on-click-modal="false" title="网卡续费" :visible.sync="renewvisible" width="400px" append-to-body>
        <el-input-number v-model="renewMonth" :min="1" :max="36" :precision="0" :step="1"
          label="续费月份"></el-input-number>
        <div slot="footer" class="dialog-footer">
          <el-button type="primary" @click="fisishRenew">确 定</el-button>
          <el-button @click="renewvisible = false">取 消</el-button>
        </div>
      </el-dialog>
      <el-dialog :close-on-click-modal="false" title="网卡详情" :visible.sync="cardInfoVisable" width="1000px"
        append-to-body>
        <div slot="title">
          <div class="titile_num">
            <div class="num_tips">物联卡</div>
            <div class="num">{{ cardInfo.ICCID }}</div>
          </div>
          <div class="title_ul">
            <div class="num_li">
              <div class="label">IMSI号：</div>
              <div class="value">{{ cardInfo.IMSI }}</div>
            </div>
            <div class="num_li">
              <div class="label">对应手机号：</div>
              <div class="value">{{ cardInfo.MSISDN }}</div>
            </div>
          </div>
        </div>
        <div>
          <div class="nav_li">
            <div class="nav_text">基本信息</div>
            <div class="line"></div>
          </div>
          <div class="info_ul">
            <div class="info_li" v-for="(item, inx) in cardInfoList" :key="inx">
              <div class="label">{{ item.label }}</div>
              <div class="value">{{ item.value }}</div>
            </div>
          </div>
        </div>
      </el-dialog>
    </div>
  </div>
</template>

<script>
import {
  iotCardUnbind,
  iotCardList,
  exportemplateCard,
  iotCardRemove,
  iotCardUsingDevice,
  iotCardRecharge,
  iotCardInfo,
  iotCardSyncCard,
  DeviceList,
  iotStopCard
} from "@/api/rules/device";
import { orgInfo } from "@/api/system/company";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { getToken } from "@/utils/auth";
export default {
  name: "cardList",
  mixins: [resizeTableCon],
  data() {
    return {
      cardInfoVisable: false,
      //网卡续费
      renewvisible: false,
      renewMonth: 0, //续费月份
      //网卡续费
      //绑定设备
      activeBindId: 0,
      deviceOptionList: [],
      optionLoading: false,
      deviceForm: {
        pageNum: 1,
        pageSize: 100,
        orderByColumn: "CreateOn",
        isAsc: "desc",
        GroupId: null,
        Online: null,
        type: null,
      }, //设备查询form
      deviceOption: [], //设备列表
      cardDevice: "", //绑定的设备id
      choiceDevicevisible: false, //物联网卡绑定设备选择设备弹窗
      //绑定设备
      // 物联网卡导入参数
      upload: {
        // 是否显示弹出层（设备导入）
        open: false,
        // 弹出层标题（设备导入）
        title: "",
        // 是否禁用上传
        isUploading: false,
        // 是否更新已经存在的用户数据
        updateSupport: false,
        // 设置上传的请求头部
        headers: { Authorization: getToken() },
        // 上传的地址
        url: process.env.VUE_APP_BASE_API + "IoTService/IotCard/Import",
      },
      classmap: new Map(),
      // 遮罩层
      loading: true,
      // 导出遮罩层
      exportLoading: false,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      // 产品表格数据
      productTableList: null,
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 表单参数
      form: {},
      // 列信息
      columns: [
        // { key: 0, label: `所属组织ID`, visible: true },
        { key: 0, label: `ICCID号`, visible: true },
        { key: 1, label: `IMSI号`, visible: true },
        { key: 2, label: `对应手机号`, visible: true },
        { key: 3, label: `卡来源`, visible: true },
        { key: 4, label: `网络限速值`, visible: false },
        { key: 5, label: `运营商`, visible: false },
        { key: 6, label: `网卡类型`, visible: true },
        { key: 7, label: `流量池Id`, visible: false },
        { key: 8, label: `卡当前套餐名称`, visible: true },
        { key: 9, label: `卡套餐大小`, visible: true },
        { key: 10, label: `卡套餐用量`, visible: true },
        { key: 11, label: `激活时间`, visible: true },
        { key: 12, label: `到期时间`, visible: true },
        { key: 13, label: `使用设备Id`, visible: true },
        { key: 14, label: `设备绑定时间`, visible: true },
        { key: 15, label: `网卡状态`, visible: true },
        { key: 16, label: `最后一次同步时间`, visible: true },
        { key: 17, label: `创建时间`, visible: false },
      ],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        Key: "",
        DeviceKey: "",
        Status: null,
        IsBindDev: null,
        CardFrom: null
      },
      cardStatusList: [
        { text: "测试中", value: "testing" },
        { text: "库存", value: "inventory" },
        { text: "待激活", value: "pending-activation" },
        { text: "已激活", value: "activation" },
        { text: "已停卡", value: "deactivation" },
        { text: "已销卡", value: "retired" },
      ],
      importType: "", //导入卡的类型
      cardInfo: {}, //指定网卡信息
      cardInfoList: [],
    };
  },
  mounted() {
    this.$nextTick(() => {
      this.getList();
      this.getDeviceList();
    });
  },
  methods: {
    handleSynchronous(row) {
      this.$modal
        .confirm('确认要立即同步ICCID为"' + row.ICCID + '"物联网卡吗？')
        .then(function () {
          return iotCardSyncCard({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("同步成功");
          this.queryParams.pageNum = 1;
          this.getList();
        })
        .catch(function () { });
    },
    viewInfo(row) {
      //获取指定网卡信息
      this.cardInfoList = [];
      iotCardInfo({ id: row.Id }).then((res) => {
        // console.log("指定网卡信息",res);
        this.cardInfo = res.data;
        orgInfo({ id: res.data.OrgId }).then((rs) => {
          // console.log('制定企业信息',rs);
          this.cardInfo.OrgName = rs.data.OrgName;

          for (var keys in this.cardInfo) {
            console.log("循环", keys);
            let obj = {
              label: "",
              value: this.cardInfo[keys],
            };
            if (keys == "OrgName") {
              obj.label = "所属企业";
              this.cardInfoList[0] = obj;
            }
            if (keys == "CardFrom") {
              obj.label = "卡来源";
              this.cardInfoList[1] = obj;
            }
            if (keys == "SpeedLimit") {
              obj.label = "网络限速值";
              this.cardInfoList[2] = obj;
            }
            if (keys == "Carrier") {
              obj.label = "运营商";
              this.cardInfoList[3] = obj;
            }
            if (keys == "CardType") {
              obj.label = "网卡类型";
              if (this.cardInfo[keys] == "POOL") {
                obj.value = "流量池卡";
              } else if (this.cardInfo[keys] == "SINGLE") {
                obj.value = "单卡";
              } else {
                obj.value = "其他";
              }
              this.cardInfoList[4] = obj;
            }
            if (keys == "CardPoolId") {
              obj.label = "流量池ID";
              this.cardInfoList[5] = obj;
            }
            if (keys == "RatePlanName") {
              obj.label = "当前套餐";
              this.cardInfoList[6] = obj;
            }
            if (keys == "RatePlanId") {
              obj.label = "套餐ID";
              this.cardInfoList[7] = obj;
            }
            if (keys == "TotalDataVolume") {
              obj.label = "套餐大小";
              this.cardInfoList[8] = obj;
            }
            if (keys == "UsedDataVolume") {
              obj.label = "套餐用量";
              this.cardInfoList[9] = obj;
            }
            if (keys == "UseCountAsVolume") {
              obj.label = "套餐单位";
              this.cardInfoList[10] = obj;
            }
            if (keys == "StartDate") {
              obj.label = "激活时间";
              this.cardInfoList[11] = obj;
            }
            if (keys == "ExpirationDate") {
              obj.label = "到期时间";
              this.cardInfoList[12] = obj;
            }
            if (keys == "LastSyncDate") {
              obj.label = "最后一次同步时间";
              this.cardInfoList[13] = obj;
            }
            if (keys == "UsingDevice") {
              obj.label = "使用中的设备ID";
              this.cardInfoList[14] = obj;
            }
            if (keys == "Status") {
              obj.label = "网卡状态";
              obj.value = this.getStatusText(this.cardInfo[keys]);
              this.cardInfoList[15] = obj;
            }
            if (keys == "UsingOn") {
              obj.label = "设备绑定时间";
              this.cardInfoList[16] = obj;
            }
            if (keys == "CreatedOn") {
              obj.label = "创建时间";
              this.cardInfoList[17] = obj;
            }
          }
          this.$forceUpdate();
          this.cardInfoVisable = true;
        });
      });
    },
    fisishRenew() {
      //完成续费
      if (this.renewMonth) {
        // console.log("续费相关参数", this.ids, this.renewMonth);
        iotCardRecharge({ Ids: this.ids, Month: this.renewMonth }).then(
          (res) => {
            this.$modal.msgSuccess("续费成功");
          }
        );
      } else {
        this.$modal.msgError("请输入续费时间");
      }
    },
    handleRenew() {
      //批量续费
      this.renewvisible = true;
    },
    fisishBindDevice() {
      if (this.cardDevice) {
        iotCardUsingDevice({
          id: this.activeBindId,
          devId: this.cardDevice,
        }).then((res) => {
          // console.log("绑定成功", res);
          this.$modal.msgSuccess("绑定成功");
        });
      } else {
        this.$modal.msgError("请选择要绑定的设备");
      }
    },
    remoteMethod(query) {
      if (query !== "") {
        this.optionLoading = true;
        setTimeout(() => {
          this.deviceForm.key = query;
          this.getDeviceList();
          this.optionLoading = false;
          this.deviceOption = this.deviceOptionList.filter((item) => {
            return item.Name.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });
        }, 200);
      } else {
        this.deviceOption = this.deviceOptionList;
        delete this.deviceForm.key;
        this.getDeviceList();
      }
    },
    choiceDeviceVal(val) {
      // console.log("选项变化", val);
      if (!val) {
        this.deviceOption = this.deviceOptionList;
      }
      this.$nextTick(() => { });
    },
    getDeviceList() {
      //获取设备列表
      DeviceList(this.deviceForm).then(async (response) => {
        // console.log("查询到的设备", response);
        this.deviceOptionList = response.data.List;
        this.deviceOption = JSON.parse(JSON.stringify(response.data.List));
      });
    },
    /** 排序触发事件 */
    handleSortChange(column, prop, order) {
      this.queryParams.orderByColumn = column.prop;
      this.queryParams.isAsc = column.order;
      this.getList();
    },
    getStatusText(val) {
      let text = "";
      switch (val) {
        case "testing":
          text = "测试中";
          break;
        case "inventory":
          text = "库存";
          break;
        case "pending-activation":
          text = "待激活";
          break;
        case "activation":
          text = "已激活";
          break;
        case "deactivation":
          text = "已停卡";
          break;
        case "retired":
          text = "已销卡";
          break;
      }
      return text;
    },
    // 文件上传中处理
    handleFileUploadProgress(event, file, fileList) {
      this.upload.isUploading = true;
    },
    // 文件上传成功处理
    handleFileSuccess(response, file, fileList) {
      this.upload.open = false;
      this.upload.isUploading = false;
      this.$refs.uploadref.clearFiles();
      this.$alert(response.message, "导入结果", {
        dangerouslyUseHTMLString: true,
      });
      this.searchCardList();
    },
    // 提交上传文件
    submitFileForm() {
      this.$refs.uploadref.submit();
    },
    /** 导入按钮操作 */
    handleImport() {
      // console.log("777777777");
      this.upload.title = "SimBoss卡导入";
      this.upload.open = true;
      this.importType = "SimBoss";
    },
    handleImport2() {
      // console.log("888888888");
      this.upload.title = "移动卡导入";
      this.upload.open = true;
      this.importType = "YiDong";
    },
    handleImport3() {
      this.upload.title = "硕软卡导入";
      this.upload.open = true;
      this.importType = "Sohan";
    },
    /** 下载模板操作 */
    onImportTemplate() {
      exportemplateCard();
    },
    searchCardList() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    changeStatus() {
      //改变查询的状态的值
      if (this.queryParams.Status == "") {
        this.queryParams.Status = null;
      }
    },
    filterSatus(value, row) {
      return row.Status === value;
    },
    /** 查询物联网卡列表 */
    getList() {
      this.loading = true;
      // console.log(this.queryParams, "传的参数");
      iotCardList(
        this.addDateRange(this.queryParams, this.dateRange, [
          "ExpirBeginTime",
          "ExpirEndTime",
        ])
      ).then(async (response) => {
        // console.log("查询到的物联网卡", response.data.List);
        this.productTableList = response.data.List;
        this.total = response.data.Total;
        this.loading = false;
      });
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.queryParams.IsBindDev = null;
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.Id);
      this.single = selection.length != 1;
      this.multiple = !selection.length;
    },
    isRed({ row }) {
      let checkIdList = this.ids;
      // console.log("选中的",checkIdList,this.ids,row);
      if (checkIdList.includes(row.Id)) {
        return {
          backgroundColor: "rgba(32, 63, 65, 1)",
          "text-align": "center",
        };
      }
    },
    handleUnActive(row) {
      //取消绑定
      this.$modal
        .confirm('确认要取消绑定ICCID为"' + row.ICCID + '"物联网卡吗？')
        .then(function () {
          return iotCardUnbind({ id: row.Id });
        })
        .then(() => {
          this.$modal.msgSuccess("取消绑定成功");
          this.queryParams.pageNum = 1;
          this.getList();
        })
        .catch(function () { });
    },
    handleActive(row) {
      //绑定
      this.choiceDevicevisible = true;
      this.activeBindId = row.Id;
    },
    /** 删除网卡操作 */
    handleDelete(id) {
      if (id === '' && this.ids.length < 1) {
        this.$message.error('请选择要删除的数据')
        return
      }
      this.$modal
        .confirm('是否确认删除ICCID为"' + row.ICCID + '"的物联网卡？')
        .then(function () {
          return iotCardRemove({ ids: id === '' ? this.ids : id });
        })
        .then((rsp) => {
          console.log("删除返回值", rsp);
          this.queryParams.pageNum = 1;
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
    handleStop(row) {
      this.$modal
        .confirm('是否确认停用物联网卡"' + row.ICCID + '"？')
        .then(async function () {
          let rsp = await iotStopCard({ id: row.Id });
          if (rsp.code == 0) {
            await iotCardSyncCard({ id: row.Id });
            this.queryParams.pageNum = 1;
            this.getList();
            this.$modal.msgSuccess("停用成功，请等待运营商处理！");
          }
          else {
            this.$modal.msgError(rsp.message);
          }
          return;
        })
        .catch(() => { });
    }
  },
};
</script>
<style rel="stylesheet/scss" lang="scss">
@import "~@/assets/styles/element-variables.scss";

// @import "~@/assets/icons/iconfont.css";
// .app-container {
//   padding-right: 30px;
// }
.titile_num {
  display: flex;
  justify-content: flex-start;
  align-items: center;

  .num_tips {
    width: 62px;
    height: 24px;
    background: #f6f9ff;
    font-size: 14px;
    color: #3572ff;
    text-align: center;
    line-height: 24px;
    margin-right: 12px;
  }

  .num {
    font-size: 20px;
    color: #333333;
  }
}

.title_ul {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  margin-top: 22px;

  .num_li {
    font-size: 16px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    margin-right: 50px;
    line-height: 16px;

    .label {
      color: #78829d;
    }

    .value {
      color: #333333;
    }
  }
}

.nav_li {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  flex-direction: column;
  color: #3572ff;
  font-size: 16px;

  .line {
    width: 64px;
    height: 2px;
    background: #3572ff;
  }

  .nav_text {
    line-height: 16px;
    margin-bottom: 10px;
  }
}

.info_ul {
  display: flex;
  justify-content: flex-start;
  flex-wrap: wrap;

  .info_li {
    width: 250px;
    height: 102px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    margin-left: -20px;
    padding-left: 30px;

    .label {
      color: #78829d;
      font-size: 14px;
      line-height: 14px;
    }

    .value {
      color: #333333;
      font-size: 16px;
      line-height: 16px;
      margin-top: 12px;
    }
  }
}

.data_table {
  th div.cell .el-table__column-filter-trigger .el-icon-arrow-down::before {
    font-family: "zhongtaiiconfont" !important;
    // font-size: 24px;
    font-size: var(--shaixuan);
    font-style: normal;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    content: "\e6d2";
  }

  .col_con {
    width: 320px;
    display: flex;
    justify-content: flex-start;

    .col_left {
      display: flex;
      align-items: center;

      img {
        width: 54px;
        height: 54px;
      }
    }

    .col_right {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-left: 12px;

      .col_right_top {
        line-height: 22px;
        text-align: left;
        width: 100%;
        color: #0054fc;
      }

      .col_right_bottom {
        line-height: 22px;

        .right_bottom1 {
          display: flex;
          align-items: flex-start;
          justify-content: flex-start;
          flex-direction: column;

          .proName {
            color: #0054fc;
          }

          // .proName::after {
          //   content: "|";
          //   display: inline;
          //   color: #976697;
          //   padding: 0;
          //   margin: 0 8px;
          //   vertical-align: 0;
          // }
        }

        .right_bottom2 {
          display: flex;
          align-items: center;
          justify-content: flex-start;

          .data_icon {
            display: block;
            width: 44px;
            height: 16px;
            color: #0054fc;

            .el-tooltip__popper .popper__arrow {
              border-width: 6px;
              border-color: rgb(0, 0, 0, 1);
            }

            i {
              display: none;
              margin-left: 8px;
            }
          }
        }
      }
    }
  }
}

.data_table.el-table table .fixed-width .cell .el-button {
  margin-right: 6px;
}

.data_table.el-table table {
  .fixed-width .cell {
    margin-right: 0;
  }
}

.data_table.el-table table tr.el-table__row:hover {
  .col_con .col_right .col_right_bottom .right_bottom2 .data_icon i {
    display: inline;
    cursor: pointer;
  }
}
</style>