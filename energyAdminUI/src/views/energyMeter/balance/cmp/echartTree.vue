<template>
  <div class="hello">
    <div class="org_tree_div" ref="org_tree_content">
      <vue2-org-tree
        :data="allData"
        :horizontal="true"
        :label-class-name="labelClassName"
        :collapsable="false"
        @on-expand="onExpand"
        @on-node-click="NodeClick"
        :renderContent="renderContent"
      />
    </div>
  </div>
</template>

<script>
export default {
  name: "echartTree",
  data() {
    return {
      labelClassName: "bg-color-orange",
      data: {
        id: 0,
        label: "XXX科技有限公司",
        number: 40,
        children: [
          {
            id: 2,
            label: "产品研发部",
            number: 25,
            children: [
              {
                id: 5,
                label: "研发-前端",
                number: 4,
              },
              {
                id: 6,
                label: "研发-后端",
                number: 4,
              },
              {
                id: 9,
                label: "UI设计",
                number: 2,
              },
              {
                id: 10,
                label: "产品经理",
                number: 15,
              },
            ],
          },
          {
            id: 3,
            label: "销售部",
            number: 10,
            children: [
              {
                id: 7,
                label: "销售一部",
                number: 5,
              },
              {
                id: 8,
                label: "销售二部",
                number: 5,
              },
            ],
          },
          {
            id: 4,
            label: "财务部",
            number: 3,
          },
          {
            id: 9,
            label: "HR人事",
            number: 2,
          },
        ],
      },
      currentUnit: "人",
    };
  },
  props: {
    allData: {
        type: Object,
        default: () => {
            return {};
        }
    },
  },
  methods: {
    renderContent(h, data) {
        
        if(data.parentTotal){
            let pence=0
            if(data.value>0){
                pence=(Number((Number(data.value)/Number(data.parentTotal)).toFixed(4))*100).toFixed(2)
            }
            return (
                <div class="content_div_con">
                    <div class="content_div">
                        <div class="title">{data.name}</div>
                        <div class="detail">
                            {data.value}
                            <span class="unit">{data.unit}</span>
                        </div>
                    </div>
                    <div class="content_div2">占比<div class="pence_con">{pence}%</div></div>
                </div>
            );
        }else{
            return (
                <div class="content_div_con">
                    <div class="content_div">
                        <div class="title">{data.name}</div>
                        <div class="detail">
                            {data.value}
                            <span class="unit">{data.unit}</span>
                        </div>
                    </div>
                </div>
            );
        }
      
    },
    //点击节点
    NodeClick(e, data) {
      // console.log(e, data);
    },
    //默认展开
    toggleExpand(data, val) {
      if (Array.isArray(data)) {
        data.forEach((item) => {
          this.$set(item, "expand", val);
        });
      } else {
        this.$set(data, "expand", val);
      }
    },
    collapse(list) {
      list.forEach((child) => {
        if (child.expand) {
          child.expand = false;
        }
        child.children && this.collapse(child.children);
      });
    },
    //展开
    onExpand(e, data) {
      if ("expand" in data) {
        data.expand = !data.expand;
        if (!data.expand && data.children) {
          this.collapse(data.children);
        }
      } else {
        this.$set(data, "expand", true);
      }
    },
  },
};
</script>
<style lang="scss" scoped>
// .hello {
//   width: 100vw;
//   height: 100vh;
// }

.org_tree_div {
  width: 100%;
  height: 100%;
}

.org_tree_content {
  width: 100%;
  height: calc(100vh - 180px);
  overflow: auto;

  .org_tree_div {
    display: inline-block;
    min-width: 100%;
    min-height: calc(100vh - 180px);
  }
}
.org_tree_content {
  width: 100%;
  height: calc(100vh - 180px);
  overflow: auto;
  .org_tree_div {
    display: inline-block;
    min-width: 100%;
    min-height: calc(100vh - 180px);
  }
}

.org-tree-container {
  background: transparent;
  padding-top: 30px;
}

::v-deep .org-tree-node{
    padding-left: 50px;
    padding-top: 30px;
    padding-bottom: 30px;
}
::v-deep .org-tree.horizontal .org-tree-node.is-leaf, ::v-deep .org-tree.horizontal .org-tree-node.collapsed {
  padding-top: 30px;
  padding-bottom: 30px;
}
::v-deep .org-tree-node::before{
    width: 50px;
    border-color: rgba(61, 185, 143, 1) !important;
    border-width: 2px !important;
}
::v-deep .org-tree-node::after{
    border-color: rgba(61, 185, 143, 1) !important;
    border-width: 2px !important;
    width: 50px;
}
::v-deep .org-tree.horizontal>.org-tree-node{
    padding-top: 0;
}
::v-deep .org-tree.horizontal .org-tree-node:only-child:before{
    border-color: rgba(61, 185, 143, 1) !important;
    border-width: 2px !important;
    margin-top: 1px;
}
::v-deep .org-tree.horizontal .org-tree-node-children{
    padding-left: 50px;
}
::v-deep .org-tree.horizontal .org-tree-node-children>.org-tree-node:first-child{
    padding-top: 0;
}
::v-deep .org-tree.horizontal .org-tree-node-children:before{
    width: 50px;
    border-color: rgba(61, 185, 143, 1);
    border-width: 2px;
}
::v-deep .org-tree-node .org-tree-node-children::after{
    border-color: rgba(61, 185, 143, 1);
    border-width: 2px;
}
::v-deep .org-tree-node-label .org-tree-node-label-inner {
  min-width: 160px;
  height: 100%;
  background-color: rgba(34, 46, 64, 1);
  border:2px solid rgba(61, 185, 143, 1);
  border-radius: 4px;
  position: relative;
  padding:12px;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  .content_div_con{
    font-size: 20px;
    line-height: 20px;
    .content_div{
        .title{
            font-size: 14px;
            line-height: 14px;
        }
        .detail{
            margin-top: 12px;
            .unit{
                font-size: 14px;
                margin-left: 8px;
            }
        }
    }
    .content_div2{
        position: absolute;
        top: calc(100% + 16px);
        left: 0;
        font-size: 14px;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        height: 24px;
        color: rgba(255, 255, 255, 1);
        .pence_con{
            border-radius: 4px;
            border: 1px solid rgba(255, 255, 255, 0.2);
            margin-left: 16px;
            padding:0 10px;
            height: 24px;
            box-sizing: border-box;
            display: flex;
            justify-content: flex-start;
            align-items: center;
        }
    }
  }
  
    .title {
        color: #ffffff;
        font-weight: bold;
        overflow: hidden; //溢出内容隐藏
        text-overflow: ellipsis; //文本溢出部分用省略号表示
        line-clamp: 2;
    }
    .detail {
        color: #ffffff;
        word-break: break-all;
    }
}
</style>
