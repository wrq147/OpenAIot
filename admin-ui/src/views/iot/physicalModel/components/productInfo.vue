<template>
  <div>
    <table border="1" class="configInfo" v-loading="configLoading">
      <tr>
        <th colspan="1">
          <span>ID</span>
        </th>
        <td colspan="1">
          <span>
            <div>
              <div style="width: 90%;">{{productInfos.Id}}</div>
              <div style="display: none"></div>
            </div>
          </span>
        </td>
        <th colspan="1">
          <span>产品分类</span>
        </th>
        <td colspan="1">
          <span>
            <div>
              <div style="width: 90%;">{{className}}</div>
              <div style="display: none"></div>
            </div>
          </span>
        </td>
        <th colspan="1">
          <span>通讯方式</span>
        </th>
        <td colspan="1">
          <span>{{productInfos.PhysicsWay}}</span>
        </td>
      </tr>
      <tr>
        <th colspan="1">
          <span>接入方式</span>
        </th>
        <td colspan="1">
          <span>{{NetworkWayName}}</span>
        </td>
        <th colspan="1">
          <span>创建时间</span>
        </th>
        <td colspan="1">
          <span>{{productInfos.createTime}}</span>
        </td>
        <th colspan="1">
          <span>更新时间</span>
        </th>
        <td colspan="1">
          <span>{{productInfos.updateTime}}</span>
        </td>
      </tr>
      <tr>
        <th colspan="1">
          <span>说明</span>
        </th>
        <td class colspan="5">{{productInfos.Remark}}</td>
      </tr>
    </table>
  </div>
</template>

<script>
export default {
  name: "AdminUiProductinfo",
  props: {
    productInfos: {
      type: Object,
      default: () => {
        return {};
      }
    },
    configLoading: {
      type: Boolean,
      default: false
    },
    NetworkWayName:{//接入方式
        type:String,
        default:''
    },
    className:{
        type:String,
        default:''
    }
  },
  data() {
    return {
    };
  },

  mounted() {},

  methods: {
      
  }
};
</script>

<style lang="less" scoped>
table.configInfo {
  border-spacing: 0px;
  border-collapse: collapse;
  width: 100%;
  border: none;
  border-top: solid 1px #efefef;
  border-left: solid 1px #efefef;
  tr {
    height: 60px;
    line-height: 60px;
    color: rgba(0, 0, 0, 0.85);
    font-size: 14px;

    th {
      background-color: #fafafa;
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;
    }
    td {
      padding: 0 15px;
      border-bottom: solid 1px #efefef;
      border-right: solid 1px #efefef;
    }
  }
}
</style>