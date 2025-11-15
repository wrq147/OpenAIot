<template>
  <div>

    <el-dialog title="请选择流程模板" :visible.sync="open" width="60%" :close-on-click-modal="false" append-to-body
      :list="iconsGroups">
      <el-form :model="queryProcessParams" :inline="true" label-width="0">
        <el-form-item>
          <el-input style="width: 350px" v-model="queryProcessParams.name" placeholder="请输入名称"
            prefix-icon="el-icon-search" clearable @clear="listDefinition" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" size="medium" @click="listDefinition">搜索</el-button>
        </el-form-item>
      </el-form>
      <div class="content_con">
        <el-empty description="流程未定义，请先创建" v-if="!processLoading&&iconsGroups.length == 0">
          <el-link type="primary" @click="onCreateNew">创建新的流程模板</el-link>
        </el-empty>
        <template v-else>
          <div class="content_li_con" v-for="(item, inx) in iconsGroups" :key="inx"
            v-show="item.Id > 0 || item.Items.length > 0">
            <div class="title">{{ item.Name }}</div>
            <ul class="content_ul">
              <li class="li_con" v-for="(it, ix) in item.Items" :key="ix">
                <div :class="[
                  'content_li',
                  activeProgess == it.Id ? 'active_li' : '',
                ]" @mouseover="startProcess(it.Id)" @mouseout="endProcess()" @click="onSelected(it)">
                  <div class="center_cont">
                    <i :class="['li-icons', it.Icon]" :style="{ color: '#ffffff', background: it.Background }"></i>
                    <span>{{ it.Name }}</span>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </template>

      </div>
    </el-dialog>


  </div>
</template>

<script>
import {
  definitionList
} from "@/api/flowable/process";
export default {
  name: "FlowPicker",
  data() {
    return {
      open: false,
      processLoading: true,
      queryProcessParams: {
        name: null
      },
      iconsGroups: [], //流程列表
      activeProgess: -1 //鼠标移到的流程
    };
  },
  mounted() {

  },
  methods: {
    endProcess() {
      //鼠标移开事件
      this.activeProgess = -1;
    },
    startProcess(id) {
      //鼠标移上去的事件
      this.activeProgess = id;
    },

    OpenDialog() {
      this.open = true;
      this.listDefinition();
    },
    listDefinition() {
      definitionList(this.queryProcessParams).then(response => {
        this.iconsGroups = response.data;
        this.processLoading = false;
      });
    },
    onSelected(item) {
      this.$emit("selected", item);
      this.open = false;
    },
    onCreateNew(){
      this.$store.dispatch("tagsView/delView", this.$route);
      this.$router.push("/flowable/FormsPanel");
    }
  }
};
</script>

<style lang="scss" scoped>
.content_con {
  padding-top: 15px;

  .content_li_con:last-child {
    border-bottom: 1px solid #ebeef5;
  }

  .content_li_con {
    padding: 12px;
    border-top: 1px solid #ebeef5;

    .title {
      font-size: 16px;
      text-align: left;
    }

    .content_ul {
      width: 100%;
      display: flex;
      justify-content: flex-start;
      flex-wrap: wrap;
      margin: 0;
      padding: 0;
      margin-top: 20px;
      margin-left: -20px;

      .li_con {
        width: 20%;
        height: 70px;
        box-sizing: border-box;
        margin-top: 10px;
        display: flex;
        justify-content: flex-start;
        margin-left: 20px;

        .content_li {
          cursor: pointer;
          width: 100%;
          height: 70px;
          padding: 0 5px 0 10px;
          display: flex;
          justify-content: space-between;
          align-items: center;
          border: 1px solid #ebeef5;
          box-sizing: border-box;
          border-radius: 5px;

          &.active_li {
            border: 1px solid #448ed7;
          }

          .center_cont {
            display: flex;
            align-items: center;

            .li-icons {
              width: 30px;
              height: 30px;
              border-radius: 5px;
              font-size: 20px;
              text-align: center;
              line-height: 30px;
              flex-shrink: 0; //由于子元素宽度之和超出了弹性盒子宽度，因此两个子元素被等比例压缩，解决方法
            }

            span {
              margin-left: 5px;
            }
          }

        }
      }
    }
  }
}
</style>