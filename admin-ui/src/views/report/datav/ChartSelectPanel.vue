<template>
  <div>
    <el-dialog width="700px" title="请选择新增的组件" :visible.sync="isOpen">
      <div>
        <el-tabs tab-position="left">
          <el-tab-pane>
            <span slot="label"><i class="el-icon-pie-chart"></i> 图表组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild1 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild1=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild1"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in chartsComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild1=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-tickets"></i> 文本组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild2 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild2=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild2"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in textComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild2=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-picture-outline"></i> 媒体组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild3 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild3=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild3"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in mediaComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild3=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-s-grid"></i> 表格组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild4 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild4=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild4"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in tableComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild4=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-news"></i> 条件组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild5 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild5=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild5"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in factorComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild5=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-news"></i> 组态组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild6 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild6=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild6"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in configurationComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild6=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
          <el-tab-pane>
            <span slot="label"><i class="el-icon-news"></i> 其他组件</span>
            <div class="pan-item">
              <el-row :gutter="20">
                <template v-if="selectChild6 != null">
                  <el-col :span="6">
                    <div class="grid-item" @click="selectChild6=null">
                      <div class="grid-icon">
                        <i class="el-icon-back"></i>
                      </div>
                      <div class="grid-title">返回</div>
                    </div>
                  </el-col>

                  <el-col
                    :span="6"
                    v-for="(item, idx) in selectChild6"
                    :key="idx"
                  >
                    <div class="grid-item" @click="onSelectItem(item)">
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
                <template v-else>
                  <el-col
                    :span="6"
                    v-for="(item, idx) in otherComponents"
                    :key="idx"
                  >
                    <div
                      class="grid-item"
                      v-if="item.child"
                      @click="selectChild6=item.child"
                    >
                      <div class="grid-icon">
                        <i class="el-icon-folder"></i>
                      </div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                    <div class="grid-item" @click="onSelectItem(item)" v-else>
                      <div class="grid-icon"><i :class="item.icon"></i></div>
                      <div class="grid-title">{{ item.layerName }}</div>
                    </div>
                  </el-col>
                </template>
              </el-row>
            </div>
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-dialog>
  </div>
</template> 

<script>
import {
  chartsComponents,
  textComponents,
  mediaComponents,
  tableComponents,
  factorComponents,
  configurationComponents,
  otherComponents
} from "./ComponentsConfig";

export default {
  data() {
    return {
      chartsComponents,
      textComponents,
      mediaComponents,
      tableComponents,
      factorComponents,
      configurationComponents,
      otherComponents,
      isOpen: false,
      selectRect: null,
      selectChild1: null,
      selectChild2: null,
      selectChild3: null,
      selectChild4: null,
      selectChild5: null,
      selectChild6: null,
    };
  },
  mounted() {},
  computed: {},
  methods: {
    showDialog(rect) {
      this.selectRect = rect;
      this.isOpen = true;
    },
    onSelectItem(item) {
      this.$emit("add", item, this.selectRect);
      this.isOpen = false;
    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep {
  .el-dialog__header{
    border-bottom: 1px solid #ccc;
  }
}
.pan-item {
  padding: 10px 10px;
}
.grid-item {
  cursor: pointer;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 15px 0;
  .grid-icon {
    font-size: 36px;
  }
}
.grid-item:hover {
  transform: scale(1.05);
  box-shadow: 0 0 6px 1px rgba(0, 0, 0, 0.15);
}
::v-deep .el-dialog__body {
  overflow: hidden;
  overflow-y: auto;
  z-index: 1;
}
</style>
