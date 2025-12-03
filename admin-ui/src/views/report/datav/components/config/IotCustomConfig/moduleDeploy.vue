<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="70px" class="custom_form_item">

        <el-collapse v-model="activeNames" accordion>

          <el-collapse-item title="图层" name="1">
            <el-form-item v-if="configData.layerName !== undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item>

          <el-collapse-item title="动画配置" name="2">

            <!-- <el-form-item v-if="configData.chartOption.animationList!==undefined" label="动画列表"> -->
            <div class="animate_con">
              <div class="title"><span class="title_text">动画列表</span><el-button type="text" @click="addMove()">+
                  添加动画</el-button></div>
              <div class="animate_li_con" v-for="(item, inx) in configData.chartOption.animationList" :key="'ani' + inx">
                <div class="name_con">
                  <div class="num">{{ inx + 1 }}</div>
                  <span class="text">名称：</span>
                  <el-input v-model="configData.chartOption.animationList[inx].name" placeholder="请输入动画名称" />
                  <el-button style="margin-left: 10px" size="mini" @click="delMove(inx)" type="danger"
                    icon="el-icon-delete" circle></el-button>
                </div>
                <div class="info_con">
                  <div><span>动画序列图：<span
                        style="color:red;font-size:12px;margin-right:10px;">请上传10M以内的图片</span></span><el-button
                      type="text" @click="addMoveImg(inx)">+ 添加序列图</el-button></div>
                  <div class="image_flex_con">
                    <div v-for="(ite, index) in configData.chartOption.animationList[inx].imglist" :key="'iotimg' + index"
                      class="image_flex">
                      <div class="image_con"><image-upload :isShowTip="false"
                          v-model="configData.chartOption.animationList[inx].imglist[index]" :limit="1"></image-upload>
                      </div>
                      <el-button style="margin-left: 10px" size="mini" @click="delMoveImg(index, inx)" type="danger"
                        icon="el-icon-delete" circle></el-button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <!-- </el-form-item> -->
            <!-- <el-form-item label="动画条件" prop="conditionJson" style="width: 100%"> -->
            <div class="title"><span class="title_text" style="margin-right:20px;">条件</span><el-button type="text"
                @click="addConditionList()">+ 添加条件</el-button></div>
            <el-row :gutter="3" :key="'condition' + inx" v-for="(ite, inx) in configData.chartOption.conditionList"
              style="margin-top:10px;display:flex;align-items:center;">
              <el-col :span="1" style="font-size:12px">{{ inx + 1 }}:</el-col>
              <el-col :span="7">
                <el-select v-model="configData.chartOption.conditionList[inx].field" placeholder="字段" filterable
                  clearable>
                  <el-option v-for="item in fieldOptions" :key="item.filed" :label="item.name"
                    :value="item.filed"></el-option>
                </el-select>
              </el-col>
              <el-col :span="8">
                <el-select v-model="configData.chartOption.conditionList[inx].symbol" placeholder="比较符">
                  <el-option v-for="item in compareList" :key="item.value" :label="item.label"
                    :value="item.value"></el-option>
                </el-select>
              </el-col>
              <!-- <el-col :span="5">
                <el-select v-model="configData.chartOption.conditionList[inx].valtype" placeholder="类型">
                    <el-option v-for="item in valtypeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                </el-select>
              </el-col> -->
              <el-col :span="5">
                <el-input type="text" v-model="configData.chartOption.conditionList[inx].value"
                  placeholder="值"></el-input>
              </el-col>
              <el-col :span="3">
                <el-button size="mini" @click="delConditionsItem(inx)" type="danger" icon="el-icon-delete"
                  circle></el-button>
              </el-col>
            </el-row>
            <div class="title"><span class="title_text" style="margin-right:20px;">条件组</span><el-button type="text"
                @click="addConditionGroupList()">+ 添加条件组</el-button></div>
            <template v-for="(item2, ix) in configData.chartOption.conditionGroup">
              <div class="num" :key="'group_num' + ix">
                <span style="margin-right:10px">条件组{{ ix + 1 }}:</span><el-button type="text"
                  @click="addConditionGroupCondition(ix)">+ 加条件</el-button>
                <el-button type="text" @click="delConditionGroupCondition(ix)"
                  v-if="configData.chartOption.conditionGroup[ix].condition.length > 0">- 减条件</el-button>
                <el-button size="mini" @click="delConditionGroupList(ix)" type="danger" icon="el-icon-delete"
                  circle></el-button>
              </div>
              <div style="display:flex;align-items:center;" :key="'groups' + ix">
                <el-row :gutter="3">
                  <el-col :span="18">
                    <el-row :gutter="3"
                      v-if="configData.chartOption.conditionGroup[ix] && configData.chartOption.conditionGroup[ix].condition.length > 0">
                      <template v-for="(row, i) in configData.chartOption.conditionGroup[ix].condition">
                        <el-col :span="9" v-if="i == 0" :key="'group_condition' + i">
                          <el-select v-model="configData.chartOption.conditionGroup[ix].condition[i]" placeholder="条件">
                            <el-option v-for="(item, dx) in configData.chartOption.conditionList" :key="'grCon' + dx"
                              :label="'第' + Number(dx + 1) + '个'" :value="dx"></el-option>
                          </el-select>
                        </el-col>
                        <el-col :span="7" v-if="i > 0" :key="'group_symbol' + i">
                          <el-select v-model="configData.chartOption.conditionGroup[ix].groups[i - 1]" placeholder="关系">
                            <el-option label="与" value="and"></el-option>
                            <el-option label="或" value="or"></el-option>
                          </el-select>
                        </el-col>
                        <el-col :span="8" v-if="i > 0" :key="'group_condition' + i">
                          <el-select v-model="configData.chartOption.conditionGroup[ix].condition[i]" placeholder="条件">
                            <el-option v-for="(item, dx) in configData.chartOption.conditionList" :key="'grCon' + dx"
                              :label="'第' + Number(dx + 1) + '个'" :value="dx"></el-option>
                          </el-select>
                        </el-col>
                      </template>
                    </el-row>
                  </el-col>
                  <el-col :span="6"
                    v-if="configData.chartOption.conditionGroup && configData.chartOption.conditionGroup[ix].condition.length > 0">
                    <el-select v-model="configData.chartOption.conditionGroup[ix].animation" placeholder="动画">
                      <el-option v-for="(item, dx) in configData.chartOption.animationList" :key="'grAn' + dx"
                        :label="item.name" :value="dx"></el-option>
                    </el-select>
                  </el-col>
                </el-row>
              </div>
            </template>
            <!-- </el-form-item> -->
          </el-collapse-item>


          <!-- <el-collapse-item title="动画" name="6">
          
          <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
            <el-select v-model="configData.chartOption.animate" placeholder="请选择">
              <el-option
                v-for="item in animateOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>

        </el-collapse-item> -->

        </el-collapse>

      </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { getLinkChart } from "../../../util/LinkageChart";
export default {
  props: ["costomData", "drawingList", "resultTableList", "filedSelected"],
  data() {
    return {
      fontFamilys: this.fontFamilys,
      fontWeights: ['normal', 'bold', 'bolder', 'lighter'],
      types: [{ label: '月范围', value: 'monthrange' }, { label: '日期范围', value: 'daterange' }, { label: '日期时间范围', value: 'datetimerange' }],
      activeNames: ['1'],
      chartList: this.drawingList,
      animateOptions,
      configData: this.costomData,
      // fieldOptions:[],
      compareList: [
        { label: '大于', value: '>' },
        { label: '小于', value: '<' },
        { label: '等于', value: '==' },
        { label: '不等于', value: '><' },
        { label: '大于等于', value: '>=' },
        { label: '小于等于', value: '<=' },
      ],
      valtypeList: [
        { label: '数字', value: 'number' }
      ],
      isUpdatingFromCostomData: false
    };
  },
  watch: {
    configData: {
      deep: true,
      handler(newVal, oldVal) {
        if (this.isUpdatingFromCostomData) {
          this.isUpdatingFromCostomData = false;
          return;
        }

        this.$emit("costom-change", newVal);
      },
    },
    costomData: {
      deep: true,
      handler(newVal) {
        this.isUpdatingFromCostomData = true;
        this.configData = newVal;
      },
    },
  },
  //页面加载完执行
  mounted() {
    let chartList = getLinkChart(this.drawingList);
    this.chartList = chartList;
  },
  computed: {
    fieldOptions() {
      let filedObj = this.filedSelected.find(rw => rw.key == 'filed')
      let filedNameObj = this.filedSelected.find(rw => rw.key == 'filedName')
      if (filedObj && filedNameObj) {
        let resArr = this.resultTableList.map(ro => {
          let obj = {
            filed: ro[filedObj.filed],
            name: ro[filedNameObj.filed],
          }
          return obj
        })
        return resArr
      }
      return []
    }
  },
  methods: {
    delConditionGroupList(ix) {
      //删除条件组
      this.configData.chartOption.conditionGroup.splice(ix, 1)
    },
    addConditionGroupList() {//添加条件组
      if (this.configData.chartOption.conditionGroup) {
        this.configData.chartOption.conditionGroup.push({
          condition: [],
          groups: [],
          animation: ''
        })
      } else {
        this.configData.chartOption.conditionGroup = []
        this.configData.chartOption.conditionGroup.push({
          condition: [],
          groups: [],
          animation: ''
        })
      }
    },
    addConditionGroupCondition(index) {//增加条件组条件
      this.configData.chartOption.conditionGroup[index].condition.push('')
    },
    delConditionGroupCondition(index) {//删除条件组条件
      this.configData.chartOption.conditionGroup[index].condition.splice(this.configData.chartOption.conditionGroup[index].condition.length - 1, 1)
    },
    addConditionList() {
      if (this.configData.chartOption.conditionList) {
        this.configData.chartOption.conditionList.push({ field: '', symbol: '', value: '', valtype: '', })
      } else {
        this.configData.chartOption.conditionList = []
        this.configData.chartOption.conditionList.push({ field: '', symbol: '', value: '', valtype: '', })
      }
      // if(this.configData.chartOption.conditionList.length>=2){
      //   this.configData.chartOption.conditionGroup.push('')
      // }
    },
    delConditionsItem(inx) {
      this.configData.chartOption.conditionList.splice(inx, 1)
    },
    handleConditionGroupData() {
      //处理条件与条件的相关
    },
    delMove(inx) {
      //删除图片
      this.configData.chartOption.animationList.splice(inx, 1)
    },
    addMove() {
      //添加图片个数
      this.configData.chartOption.animationList.push({
        name: '',
        imglist: [],
      })
    },
    delMoveImg(index, inx) {
      //删除图片
      this.configData.chartOption.animationList[inx].imglist.splice(index, 1)
    },
    addMoveImg(inx) {
      //添加图片个数
      this.configData.chartOption.animationList[inx].imglist.push('')
    },
    bindCharts(val) {
      this.$set(this.configData.chartOption, 'bindList', val);
    },
  },
};
</script>

<style lang="scss" scoped>
.animate_con {
  padding: 0 5px;

  .title_text {
    margin-right: 20px;
  }

  .animate_li_con {
    .name_con {
      display: flex;
      align-items: center;

      .num {
        width: 30px;
        height: 20px;
        border-radius: 20px;
        border: 1px solid #333333;
        display: flex;
        justify-content: center;
        align-items: center;
        margin-right: 10px;
      }

      .text {
        white-space: nowrap;
      }
    }

    // .info_con{
    // padding-left: 40px;
    // }
  }
}

.image_flex_con {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  flex-wrap: wrap;
  margin-right: -3px;
}

.image_flex {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  margin-right: 3px;
  margin-bottom: 3px;

  .image_con {
    width: 60px;
    height: 60px;

    ::v-deep .el-upload-list--picture-card .el-upload-list__item {
      width: 60px;
      height: 60px;
    }

    ::v-deep .hide {
      width: 60px;
      height: 60px;
    }

    .component-upload-image {
      height: 60px;
    }

    ::v-deep .el-upload.el-upload--picture-card {
      width: 60px;
      height: 60px;
      line-height: 60px;
    }
  }
}

::v-deep .center-tabs .el-tabs__item {
  width: 33%;
  text-align: center;
}

.dataProduct {
  margin-bottom: 10px;
  line-height: 45px;
  padding: 0 15px;
  background-color: #f5f5f5;
  color: #666;
}
</style>