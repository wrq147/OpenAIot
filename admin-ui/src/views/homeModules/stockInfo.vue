<template>
  <div class="stock_info">
    <div class="home_module_title">
      <i class="zhongtaiiconfont zhongtai-icon-kucunxinxi"></i>
      <span>库存信息</span>
    </div>
    <div class="device_sum">
      <div class="sum_li_con" v-for="item in stockInfo" :key="item.name">
        <div class="sum_li">
          <div class="li_left">
            <img
              src="@/assets/images/kucunzongshu.png"
              alt=""
              v-if="item.name == '库存总数'"
            />
            <img
              src="@/assets/images/jinriruku.png"
              alt=""
              v-if="item.name == '今日入库'"
            />
            <img
              src="@/assets/images/jinrichuku.png"
              alt=""
              v-if="item.name == '今日出库'"
            />
          </div>
          <div class="li_right">
            <div class="right_label">{{ item.name }}</div>
            <div class="right_val">{{ item.val }}</div>
          </div>
        </div>
        <div class="mingxi">
          <div class="mingxi_li">
            <span class="label">设备</span>
            <span class="val">{{ item.list.device }}</span>
          </div>
          <div class="mingxi_li">
            <span class="label">耗材</span>
            <span class="val">{{ item.list.parts }}</span>
          </div>
        </div>
      </div>
    </div>
    <div class="alarm_table">
      <div class="table_title">
        <div class="title_label">出入库记录</div>
        <el-tabs
          v-model="activeName"
          tab-position="top"
          type="card"
          @tab-click="handleClick"
        >
          <el-tab-pane label="今日" name="today"></el-tab-pane>
          <el-tab-pane label="昨日" name="yesterday"></el-tab-pane>
          <el-tab-pane label="近7天" name="seven"></el-tab-pane>
          <el-tab-pane label="近30天" name="thirty"></el-tab-pane>
        </el-tabs>
      </div>
      <el-table
        v-loading="loading"
        :data="alarmList"
        max-height="268"
        class="data_table"
        :header-cell-style="cellSty"
        style="width: 100%"
      >
        <el-table-column
          label="物品编号"
          align="center"
          key="DeviceNumber"
          prop="DeviceNumber"
          width="150"
          :show-overflow-tooltip="true"
        />
        <el-table-column label="名称" align="center" key="Name" prop="Name" />
        <el-table-column
          label="存储类型"
          align="center"
          key="TargetType"
          prop="TargetType"
        >
          <template slot-scope="scope">
            <div>
              {{ scope.row.TargetType == 1 ? "成品" : "半成品" }}
            </div>
          </template>
        </el-table-column>
        <el-table-column
          label="出入库"
          align="center"
          key="FormType"
          prop="FormType"
        >
          <template slot-scope="scope">
            <div v-if="scope.row.FormType == 0">出库</div>
            <div v-if="scope.row.FormType == 1">入库</div>
            <div v-if="scope.row.FormType == 2">盘亏修正</div>
            <div v-if="scope.row.FormType == 3">盘盈修正</div>
          </template>
        </el-table-column>
        <el-table-column
          label="数量"
          align="center"
          key="Quantity"
          prop="Quantity"
        />
        <el-table-column label="单位" align="center" key="Unit" prop="Unit" />
      </el-table>
    </div>
  </div>
</template>
  
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { StockStatisticsInfo, StockDetailRecords } from "@/api/home.js";
export default {
  name: "stockInfo",
  mixins: [resizeTableCon],
  data() {
    return {
      loading: false,
      activeName: "today",
      alarmList: [],
      stockInfo: [
        {
          name: "库存总数",
          val: 20,
          list: {
            parts: 10,
            device: 10,
          },
        },
        {
          name: "今日入库",
          val: 20,
          list: {
            parts: 10,
            device: 10,
          },
        },
        {
          name: "今日出库",
          val: 20,
          list: {
            parts: 10,
            device: 10,
          },
        },
      ],
    };
  },
  mounted() {
    this.getStockStatisticsInfo();
    this.getStockDetailRecords();
  },
  methods: {
    getStockDetailRecords() {
      //获取出入库记录
      let days = 0;
      if (this.activeName == "today") days = 0;
      if (this.activeName == "yesterday") days = 1;
      if (this.activeName == "seven") days = 7;
      if (this.activeName == "thirty") days = 30;
      StockDetailRecords({ Day: days, showAll: true }).then((res) => {
        // console.log("res出入库记录", res);
        if (res.data && res.data.List) {
          this.alarmList = res.data.List;
        }
      });
    },
    getStockStatisticsInfo() {
      //获取库存统计信息
      StockStatisticsInfo().then((res) => {
        // console.log(res, "库存统计信息");
        let data = res.data;
        this.stockInfo = [
          {
            name: "库存总数",
            val: data.TotalCount,
            list: {
              parts: data.PartsCount,
              device: data.DevCount,
            },
          },
          {
            name: "今日入库",
            val: data.TodayInCount,
            list: {
              parts: data.TodayPartsInCount,
              device: data.TodayDevInCount,
            },
          },
          {
            name: "今日出库",
            val: data.TodayOutCount,
            list: {
              parts: data.TodayPartsOutCount,
              device: data.TodayDevOutCount,
            },
          },
        ];
      });
    },
    handleClick() {
      //设备信息的报警列表的切换
      this.getStockDetailRecords();
    },
  },
};
</script>
  
<style lang="scss" scoped>
.stock_info {
  width: calc(50% - 10px);
  // height: 100%;
  margin-bottom: 30px;

  .device_sum {
    width: 100%;
    display: flex;
    justify-content: space-between;
    margin-bottom: 12px;

    .sum_li_con {
      width: calc(100% / 3 - 8px); //(100%-24)/3
      // height: 72px;
      padding: 0 20px;
      background: #ffffff;
      border-radius: 4px;

      .sum_li {
        display: flex;
        justify-content: flex-start;
        align-items: center;
        box-sizing: border-box;
        font-size: 4px;
        border-bottom: 1px solid #f4f5f9;
        padding: 16px 0 15px 0;
        height: 72px;

        .li_left {
          width: 40px;
          height: 40px;
          margin-right: 16px;

          img {
            width: 40px;
            height: 40px;
          }
        }

        .li_right {
          .right_label {
            font-size: 14px;
            color: #78829d;
            margin-bottom: 8px;
          }

          .right_val {
            color: #333333;
            font-size: 18px;
            font-weight: 550;
          }
        }
      }

      .mingxi {
        display: flex;
        justify-content: space-between;
        align-items: center;
        height: 42px;
        width: 100%;

        .mingxi_li {
          display: flex;
          justify-content: center;
          align-items: center;
          line-height: 42px;
          width: 50%;

          .label {
            font-size: 14px;
            color: #78829d;
            margin-right: 10px;
          }

          .val {
            font-size: 16px;
            color: #333333;
          }
        }
      }
    }
  }

  .alarm_table {
    background: #ffffff;
    padding: 16px 20px 10px;
    border-radius: 4px;
    height: 352px;
    box-sizing: border-box;

    .table_title {
      position: relative;

      .title_label {
        position: absolute;
        left: 0;
        bottom: 15px;
        font-size: 16px;
        color: #333333;
        font-weight: 550;
      }
    }
  }
}
</style>
<style lang="less">
.home_module_title {
  font-size: 14px;
  color: #333333;
  margin-bottom: 12px;

  span {
    margin-left: 6px;
  }
}

.alarm_table {
  background: #ffffff;

  .table_title {
    .el-tabs .el-tabs__header.is-top .el-tabs__nav-scroll {
      display: flex;
      justify-content: flex-end;
    }

    .el-tabs .el-tabs__header.is-top {
      margin-bottom: 0;
    }
  }

  .data_table {
    .el-table__body-wrapper {
      // background-color: #ddd;
      &::-webkit-scrollbar {
        width: 6px;
        height: 6px;
      }

      // 滚动条里面默认的小方块,自定义样式
      &::-webkit-scrollbar-thumb {
        background: #efefef;
        border-radius: 3px;
      }

      // 滚动条里面的轨道
      &::-webkit-scrollbar-track {
        background: transparent;
      }
    }

    // .el-table__body-wrapper::-webkit-scrollbar {
    //   width: 8px !important;
    //   height: 8px !important;
    // }
  }
}
</style>