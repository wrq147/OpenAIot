<template>
  <div class="grid-style" ref="dragcontnet" @mousedown.left="onmousedownClick" @mouseup.stop="handMouseUp">

    <drag-chart v-for="item in notConfigurationDraw" :key="'ch' + item.customId" :dragchartdata="item"
      v-if="item.isShow" :scale="scale" :activeId="activeId" :drawingList="notConfigurationDraw" :onCtrl="onCtrl"
      :ctrlSelectArr="ctrlSelectArr" :theme="theme" @getRefLineParams="getRefLineParams" @dragging="dragging"
      @dragstop="dragstop" @actived="onActived"></drag-chart>
    <div id="configurationDraw">
      <!-- <svg id="svg" overflow="visible" width="400" height="400" > -->
      <drag-chart-config v-for="item in configurationDraw" :key="'kch' + item.customId" :dragchartdata="item"
        v-if="item.isShow" :scale="scale" :activeId="activeId" :drawingList="configurationDraw" :onCtrl="onCtrl"
        :ctrlSelectArr="ctrlSelectArr" :theme="theme" @getRefLineParams="getRefLineParams" @dragging="dragging"
        @dragstop="dragstop" @actived="onActived" ref="zutaiconfig"></drag-chart-config>
      <!-- </svg> -->
    </div>
    <!--辅助线-->
    <span class="ref-line v-line" v-for="(item, idx) in vLine" :key="'v' + idx" v-show="item.display" :style="{
      left: item.position,
      top: item.origin,
      height: item.lineLength,
    }" />
    <span class="ref-line h-line" v-for="(item, idx) in hLine" :key="'h' + idx" v-show="item.display" :style="{
      top: item.position,
      left: item.origin,
      width: item.lineLength,
    }" />
    <!--辅助线END-->
    <div id="rectangular"></div>
    <div style="height: 110%; width: 110%; visibility: hidden"></div>
  </div>
</template>

<script>

import DragChart from './DragChart'
import DragChartConfig from './DragChartConfiguration'
import VueEvent from './VueEvent'
import { addEvent, removeEvent } from './util/dom'
export default {
  components: {
    DragChart,
    DragChartConfig
  },
  props: ["drawingList", "scale", "activeId", "theme"],
  data() {
    return {
      //dragchartdatas: this.drawingList,
      vLine: [],
      hLine: [],
      draggingId: null,
      prevOffsetX: 0,
      prevOffsetY: 0,
      onCtrl: false,
      ctrlSelectArr: [],  //所有选中的组件的id（数组）
      isClickMove: false,
      moveId: 0,
      container: null,
      path: null,
      img: null,
      isDrawing: false,
    };
  },
  created() {
    this.resetMouseState();
  },
  mounted() {
    VueEvent.$on("clear_ctrl", data => {

      this.ctrlSelectArr = [];
      this.resetMouseState();
    });
  },
  methods: {

    // 辅助线回调事件
    getRefLineParams(params) {
      const { vLine, hLine } = params;
      let id = 0;
      this.vLine = vLine.map((item) => {
        item["id"] = ++id;
        return item;
      });
      this.hLine = hLine.map((item) => {
        item["id"] = ++id;
        return item;
      });

    },
    onActived(activeId) {
      // console.log("活动的",activeId);
      this.moveId = activeId;
    },
    // 在页面钩子 mounted() 处调用此函数，增加按键监听事件
    watchKeyEvent() {
      const setKeyStatus = (keyCode, status) => {
        //status==true才执行，解决每次监听键盘声
        switch (keyCode) {
          case 16:
            if (this.onShfit === status) return
            // console.log('shift', status ? '按下' : '抬起')
            this.onShfit = status
            break
          case 17:
            if (this.onCtrl === status) return
            // console.log('ctrl', status ? '按下' : '抬起')
            this.onCtrl = status
            break
          case 65: //左对齐
            if (this.onShfit === true) {
              if (this.onA === status) return
              //console.log('A', status ? '按下' : '抬起');
              this.onA = status;
              if (status) {
                this.alignLeft();
              }
            }

            break
          case 68: //右对齐
            if (this.onShfit === true) {
              if (this.onD === status) return
              //console.log('D', status ? '按下' : '抬起');
              this.onD = status;
              if (status) {
                this.alignRight();
              }
            }
            break
          case 87: //上对齐
            if (this.onShfit === true) {
              if (this.onW === status) return
              //console.log('W', status ? '按下' : '抬起');
              this.onW = status;
              if (status) {
                this.alignUp();
              }
            }
            break
          case 83: //下对齐
            if (this.onShfit === true) {
              if (this.onS === status) return
              //console.log('S', status ? '按下' : '抬起');
              this.onS = status;
              if (status) {
                this.bottomJustify();
              }
            }
            break
          case 37:
            if (this.onShfit === true) {
              if (status) {
                event.preventDefault();
                //获取框选组件列表
                if (this.ctrlSelectArr.length > 0) {
                  //分别向左移动位置
                  this.ctrlSelectArr.forEach(element => {
                    const chart = this.drawingList.filter(function (item) {
                      return item.customId == element;
                    })[0]
                    chart.x--;
                  });
                } else {
                  //向左移动当前组件位置
                  const id = this.moveId;
                  const chart = this.drawingList.filter(function (item) {
                    return item.customId == id;
                  })[0]
                  if (chart != null) {
                    chart.x--;
                  }
                }
              }
            }
            break
          case 38:
            if (this.onShfit === true) {
              if (status) {
                event.preventDefault();
                //获取框选组件列表
                if (this.ctrlSelectArr.length > 0) {
                  //分别向上移动位置
                  this.ctrlSelectArr.forEach(element => {
                    const chart = this.drawingList.filter(function (item) {
                      return item.customId == element;
                    })[0]
                    chart.y--;
                  });
                } else {
                  //向上移动当前组件位置
                  const id = this.moveId;
                  const chart = this.drawingList.filter(function (item) {
                    return item.customId == id;
                  })[0]
                  if (chart != null) {
                    chart.y--;
                  }
                }
              }
            }
            break
          case 39:
            if (this.onShfit === true) {
              if (status) {
                event.preventDefault();
                //获取框选组件列表
                if (this.ctrlSelectArr.length > 0) {
                  //分别向右移动位置
                  this.ctrlSelectArr.forEach(element => {
                    const chart = this.drawingList.filter(function (item) {
                      return item.customId == element;
                    })[0]
                    chart.x++;
                  });
                } else {
                  //向右移动当前组件位置
                  const id = this.moveId;
                  const chart = this.drawingList.filter(function (item) {
                    return item.customId == id;
                  })[0]
                  if (chart != null) {
                    chart.x++;
                  }
                }
              }
            }
            break
          case 40:
            if (this.onShfit === true) {
              if (status) {
                event.preventDefault();
                //获取框选组件列表
                if (this.ctrlSelectArr.length > 0) {
                  //分别向下移动位置
                  this.ctrlSelectArr.forEach(element => {
                    const chart = this.drawingList.filter(function (item) {
                      return item.customId == element;
                    })[0]
                    chart.y++;
                  });
                } else {
                  //向下移动当前组件位置
                  const id = this.moveId;
                  const chart = this.drawingList.filter(function (item) {
                    return item.customId == id;
                  })[0]
                  if (chart != null) {
                    chart.y++;
                  }
                }
              }
            }
            break
        }
      }
      document.onkeydown = (e) => {
        setKeyStatus(e.keyCode, true)
      }
      document.onkeyup = (e) => {
        setKeyStatus(e.keyCode, false)
      }
    },
    dragging(id, left, top) {
      this.draggingId = id;

      //if (! this.onCtrl) return;

      const offsetX = left - this.draggingElement.x;
      const offsetY = top - this.draggingElement.y;

      const deltaX = this.deltaX(offsetX);
      const deltaY = this.deltaY(offsetY);

      let elements = this.drawingList.filter((item) => {
        //console.log(item.customId);
        //console.log(this.ctrlSelectArr.indexOf(item.customId));
        return this.ctrlSelectArr.indexOf(item.customId) > -1;
      })

      elements.map(el => {
        // console.log(el.customId,id,'idid');
        if (el.customId !== id) {
          el.x += deltaX;
          el.y += deltaY;
        }

        return el;
      });
    },
    dragstop() {
      this.draggingId = null;
      this.prevOffsetX = 0;
      this.prevOffsetY = 0;
    },
    deltaX(offsetX) {
      const ret = offsetX - this.prevOffsetX;

      this.prevOffsetX = offsetX;

      return ret;
    },
    deltaY(offsetY) {
      const ret = offsetY - this.prevOffsetY;

      this.prevOffsetY = offsetY;

      return ret;
    },
    // 重置鼠标状态
    resetMouseState() {
      //console.log("重置鼠标状态");
      this.mouseClickPosition = { mouseX: 0, mouseY: 0, x: 0, y: 0, w: 0, h: 0 };
      this.start_x = 0;
      this.start_y = 0;
      this.end_x = 0;
      this.end_y = 0;
    },
    //鼠标左键按下方法
    onmousedownClick(e) {
      this.$emit("bgClick");
      //console.log("鼠标左键按下方法");
      //console.log(e);
      this.mouseClickPosition.mouseX = e.touches ? e.touches[0].pageX : e.pageX;
      this.mouseClickPosition.mouseY = e.touches ? e.touches[0].pageY : e.pageY;
      this.mouseClickPosition.mouseX = parseInt(this.mouseClickPosition.mouseX / 20) * 20;
      this.mouseClickPosition.mouseY = parseInt(this.mouseClickPosition.mouseY / 20) * 20;

      this.mouseClickPosition.x = e.touches ? e.touches[0].offsetX : e.offsetX;
      this.mouseClickPosition.y = e.touches ? e.touches[0].offsetY : e.offsetY;
      this.mouseClickPosition.x = parseInt(this.mouseClickPosition.x / 20) * 20;
      this.mouseClickPosition.y = parseInt(this.mouseClickPosition.y / 20) * 20;

      this.start_x = this.mouseClickPosition.mouseX;
      this.start_y = this.mouseClickPosition.mouseY;

      addEvent(document.documentElement, "mousemove", this.boxSelecting)
      addEvent(document.documentElement, "mouseup", this.boxSelected)

    },
    boxSelecting(e) {
      const mouseClickPosition = this.mouseClickPosition;
      const end_x = e.touches ? e.touches[0].pageX : e.pageX;
      const end_y = e.touches ? e.touches[0].pageY : e.pageY;
      const tmpDeltaX = mouseClickPosition.mouseX - end_x;
      const tmpDeltaY = mouseClickPosition.mouseY - end_y;
      let [deltaX, deltaY] = this.snapToscale(tmpDeltaX, tmpDeltaY, this.scale);
      deltaX = parseInt(deltaX / 20) * 20;
      deltaY = parseInt(deltaY / 20) * 20;
      let divElement = document.getElementById("rectangular");
      divElement.style.display = 'block'
      divElement.className = 'rectangular';
      //从左往右
      // 画矩形
      if (mouseClickPosition.mouseX < end_x) {
        divElement.style.width = 0 - deltaX + "px";
        divElement.style.height = (mouseClickPosition.mouseY > end_y > 0 ? deltaY : (0 - deltaY)) + "px";
        divElement.style.left = mouseClickPosition.x + "px";
        //divElement.style.right = end_x+"px";
        //从下往上
        if (mouseClickPosition.mouseY > end_y) {
          divElement.style.top = mouseClickPosition.y - deltaY + "px";
          //divElement.style.bottom = mouseClickPosition.mouseY + "px";
        } else {
          divElement.style.top = mouseClickPosition.y + "px";
          //divElement.style.bottom = end_y+"px";
        }
      } else {
        divElement.style.width = deltaX + "px";
        divElement.style.height = (mouseClickPosition.mouseY > end_y > 0 ? deltaY : (0 - deltaY)) + "px";
        divElement.style.left = mouseClickPosition.x - deltaX + "px";
        //divElement.style.right = mouseClickPosition.x+"px";
        //从下往上
        if (mouseClickPosition.mouseY > end_y) {
          divElement.style.top = mouseClickPosition.y - deltaY + "px";
          //divElement.style.bottom=that.start_y+"px";
        } else {
          divElement.style.top = mouseClickPosition.y + "px";
          //divElement.style.bottom=end_y+"px";
        }
      }
      this.isClickMove = true;
      this.ctrlSelectArr = [];
    },
    boxSelected(e) {
      // 移动样式清空
      let divElement = document.getElementById("rectangular");
      divElement.style.display = 'none';
    },
    //鼠标左键抬起方法
    handMouseUp(event) {
      this.end_x = parseInt(event.pageX / 20) * 20;
      this.end_y = parseInt(event.pageY / 20) * 20;

      if (this.isClickMove) {

        //核心内容，根据你的鼠标移动矩形区域来判断div是否在里面
        let files = document.getElementsByClassName("test3");
        let a = 0;
        for (let i = 0; i < files.length; i++) {
          const file = files[i];
          let id = file.lastChild.id;
          let el = file.getBoundingClientRect();
          let top = el.top + 1 //上边界
          let left = el.left + 1;//左边界
          let right = el.right - 1;//右边界
          let bottom = el.bottom - 1;//下边界
          //判断如果有组合组件，去除组合只保留内部组件
          let tarNode = this.drawingList.filter((item) => {
            return item.customId == id;
          })[0];

          if (tarNode == null || tarNode.chartType == 'group') {
            continue;
          }

          //原地点击
          if (this.start_x == this.end_x) {
            if (!(this.end_x >= left && this.end_x <= right && this.end_y > top && this.end_y < bottom)) {
              a += 1;
            }
          } else {
            //从左往右
            if (this.start_x < this.end_x) {
              //从下到上
              if (this.start_y > this.end_y) {
                if (top < this.start_y && bottom > this.end_y) {
                  if (left < this.end_x && right > this.start_x) {
                    if (this.ctrlSelectArr.indexOf(parseInt(id)) === -1) {
                      //console.log("添加节点");
                      this.ctrlSelectArr.push(parseInt(id));
                    }
                  }
                }
              }
              //从上到下
              else {
                if (top < this.end_y && bottom > this.start_y) {
                  if (left < this.end_x && right > this.start_x) {
                    if (this.ctrlSelectArr.indexOf(parseInt(id)) === -1) {
                      //console.log("添加节点");
                      this.ctrlSelectArr.push(parseInt(id));
                    }
                  }
                }
              }
              //从右往左
            } else {
              //从下到上
              if (this.start_y > this.end_y) {
                if (top < this.start_y && bottom > this.end_y) {
                  if (left < this.start_x && right > this.end_x) {
                    if (this.ctrlSelectArr.indexOf(parseInt(id)) === -1) {
                      //console.log("添加节点");
                      this.ctrlSelectArr.push(parseInt(id));
                    }
                  }
                }
              }
              //从上到下
              else {
                if (top < this.end_y && bottom > this.start_y) {
                  if (left < this.start_x && right > this.end_x) {
                    if (this.ctrlSelectArr.indexOf(parseInt(id)) === -1) {
                      //console.log("添加节点");
                      this.ctrlSelectArr.push(parseInt(id));
                    }
                  }
                }
              }

            }
          }
        }

        if (this.ctrlSelectArr.length > 0 && this.ctrlSelectArr.indexOf(this.activeId) == -1) {
          //激活选中的
          this.$emit("selected", this.ctrlSelectArr[0]);
        }


        if (this.ctrlSelectArr.length == 0) {
          let divElement = document.getElementById("rectangular");
          let selectedRect = {
            x: parseInt(divElement.style.left.replace("px", "")),
            y: parseInt(divElement.style.top.replace("px", "")),
            w: parseInt(divElement.style.width.replace("px", "")),
            h: parseInt(divElement.style.height.replace("px", ""))
          };

          this.$emit("boxSelected", selectedRect);
        }
      }


      this.resetMouseState();
      this.isClickMove = false;
      removeEvent(document.documentElement, "mousemove", this.boxSelecting)
      removeEvent(document.documentElement, "mouseup", this.boxSelected)
    },
    snapToscale(pendingX, pendingY, scale) {
      const x = Math.round(pendingX / scale);
      const y = Math.round(pendingY / scale);
      return [x, y]
    },

    //左对齐
    alignLeft() {
      console.log("OvO：← 是左对齐哦~");
      const chart = this.drawingList.filter(item => {
        return this.ctrlSelectArr.indexOf(item.customId) > -1
      })
      //console.log(chart[0].x);
      for (let i = 0; chart[i] != null; i++) {
        chart[i].x = chart[0].x;
      }
    },
    //右对齐
    alignRight() {
      console.log("OvO：→ 是右对齐哦~");
      const chart = this.drawingList.filter(item => {
        return this.ctrlSelectArr.indexOf(item.customId) > -1
      })
      //console.log(chart[0].width);
      if (chart.length == 1) {
        //alert("OvO");
        chart[i].x = chart[0].x;
      }
      for (let i = 0; chart[i] != null; i++) {
        //console.log("x", chart[0].x);
        //console.log("height", chart[0].height);
        chart[i].x = chart[0].x + (chart[0].width - chart[i].width);
      }
    },
    //上对齐
    alignUp() {
      console.log("OvO：↑ 是上对齐哦~");
      const chart = this.drawingList.filter(item => {
        return this.ctrlSelectArr.indexOf(item.customId) > -1
      })
      //console.log(chart[0].y);
      for (let i = 0; chart[i] != null; i++) {
        chart[i].y = chart[0].y;
      }
    },
    //下对齐
    bottomJustify() {
      console.log("OvO：↓ 是下对齐哦~");
      const chart = this.drawingList.filter(item => {
        return this.ctrlSelectArr.indexOf(item.customId) > -1
      })
      // console.log(chart);
      for (let i = 0; chart[i] != null; i++) {
        //console.log("y", chart[0].y);
        //console.log("height", chart[0].height);
        chart[i].y = chart[0].y + (chart[0].height - chart[i].height);
      }
    },

  },
  computed: {
    draggingElement: function () {
      if (!this.draggingId) return;
      return this.drawingList.find(el => el.customId === this.draggingId);
    },
    configurationDraw() {//组态组件
      let list = this.drawingList.filter(row => row.chartTypeGroup && row.chartTypeGroup == 'configuration')
      return list
    },
    notConfigurationDraw() {//非组态组件
      let list = this.drawingList.filter(row => !row.chartTypeGroup || row.chartTypeGroup && row.chartTypeGroup != 'configuration')
      return list
    },
  },
  beforeDestroy() {
    removeEvent(document.documentElement, "mousemove", this.boxSelecting)
    removeEvent(document.documentElement, "mouseup", this.boxSelected)
  }
};
</script>

<style lang="scss">
.grid-style {
  height: 100%;
  width: 100%;
  background: linear-gradient(-90deg, rgba(0, 0, 0, 0.1) 1px, transparent 1px) 0% 0% / 20px 20px, linear-gradient(rgba(0, 0, 0, 0.1) 1px, transparent 1px) 0% 0% / 20px 20px;
}

#configurationDraw {
  height: 100%;
  width: 100%;
}

#rectangular {
  background-color: rgba(235, 239, 243, 0.45);
  position: fixed;
  border: 1px solid rgba(255, 255, 255, 0);
  z-index: 99999;
}
</style>