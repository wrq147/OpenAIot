<template>
  <div class="app-container">
    <el-row>
      <el-col :span="12" class="card-box">
        <el-card>
          <div slot="header"><span>CPU</span></div>
          <div class="el-table el-table--enable-row-hover el-table--medium">
            <table cellspacing="0" style="width: 100%;">
              <thead>
                <tr>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">属性</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">值</div>
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">核心数</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.cpu">{{ server.cpu.CpuNum }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">CPU架构</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.cpu">{{ server.cpu.CpuBit }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">CPU使用率</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.cpu">{{ server.cpu.CpuUse }}%</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">线程数</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.cpu">{{ server.cpu.ThreadCount }}</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-card>
      </el-col>

      <el-col :span="12" class="card-box">
        <el-card>
          <div slot="header"><span>内存</span></div>
          <div class="el-table el-table--enable-row-hover el-table--medium">
            <table cellspacing="0" style="width: 100%;">
              <thead>
                <tr>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">属性</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">内存</div>
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">总内存</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.mem">{{ server.mem.total }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">已用内存</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.mem">{{ server.mem.used }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">剩余内存</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.mem">{{ server.mem.free }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">使用率</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.mem"
                      :class="{ 'text-danger': parseFloat(server.mem.usage.replace('%', '')) > 80 }">{{ server.mem.usage
                      }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-card>
      </el-col>

      <el-col :span="24" class="card-box">
        <el-card>
          <div slot="header">
            <span>服务器信息</span>
          </div>
          <div class="el-table el-table--enable-row-hover el-table--medium">
            <table cellspacing="0" style="width: 100%;">
              <tbody>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">服务器名称</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.computerName }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">操作系统</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.osName }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">服务器IP</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.RemoteIp }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">系统架构</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.osArch }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">项目路径</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.userDir }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">运行时间</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.sys">{{ server.sys.SysRunTime }}</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-card>
      </el-col>

      <el-col :span="24" class="card-box">
        <el-card>
          <div slot="header">
            <span>CLR信息</span>
          </div>
          <div class="el-table el-table--enable-row-hover el-table--medium">
            <table cellspacing="0" style="width: 100%;">
              <tbody>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">.net名称</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.clr">{{ server.clr.FrameworkDescription }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">已使用内存</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.clr">{{ server.clr.UseMem }}</div>
                  </td>
                </tr>
                <tr>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">启动时间</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.clr">{{ server.clr.startTime }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">运行时长</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" v-if="server.clr">{{ server.clr.runTime }}</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-card>
      </el-col>

      <el-col :span="24" class="card-box">
        <el-card>
          <div slot="header">
            <span>磁盘状态</span>
          </div>
          <div class="el-table el-table--enable-row-hover el-table--medium">
            <table cellspacing="0" style="width: 100%;">
              <thead>
                <tr>
                  <th class="el-table__cell el-table__cell is-leaf">
                    <div class="cell">盘符路径</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">盘符类型</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">总大小</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">可用大小</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">已用大小</div>
                  </th>
                  <th class="el-table__cell is-leaf">
                    <div class="cell">已用百分比</div>
                  </th>
                </tr>
              </thead>
              <tbody v-if="server.sysFiles">
                <tr v-for="(sysFile, index) in server.sysFiles" :key="index">
                  <td class="el-table__cell is-leaf">
                    <div class="cell">{{ sysFile.dirName }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">{{ sysFile.typeName }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">{{ sysFile.total }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">{{ sysFile.free }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell">{{ sysFile.used }}</div>
                  </td>
                  <td class="el-table__cell is-leaf">
                    <div class="cell" :class="{ 'text-danger': sysFile.usage > 80 }">{{ sysFile.usage }}%</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { getServer } from "@/api/monitor/server";

export default {
  name: "Server",
  data() {
    return {
      // 服务器信息
      server: [],
      // 定时器 ID
      timerId: null
    };
  },
  created() {
    this.getList();
    this.openLoading();
    // 设置定时任务，每 5 秒调用一次 getList 方法
    this.timerId = setInterval(() => {
      this.getList();
    }, 5000);
  },
  beforeDestroy() {
    // 在组件销毁前清除定时任务
    if (this.timerId) {
      clearInterval(this.timerId);
    }
  },
  methods: {
    /** 查询服务器信息 */
    getList() {
      getServer().then(response => {
        this.server = response.data;
      }).finally(() => {
        this.$modal.closeLoading();
      });
    },
    // 打开加载层
    openLoading() {
      this.$modal.loading("正在加载服务监控数据，请稍后！");
    }
  }
};
</script>