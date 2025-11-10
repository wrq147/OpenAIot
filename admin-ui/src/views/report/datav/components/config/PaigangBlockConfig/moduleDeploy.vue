<template>
  <div class="field-box">
    <el-scrollbar class="right-scrollbar">
      <el-form size="small" label-width="100px" label-position="top" class="custom_form_item">
        <el-collapse  v-model="activeNames" accordion>
          
          <el-collapse-item title="图层" name="1" class="nopaddingbottom">
            <el-form-item v-if="configData.layerName!==undefined" label="图层名称">
              <el-input v-model="configData.layerName" placeholder="请输入图层名称" />
            </el-form-item>
          </el-collapse-item> 
          <el-collapse-item title="文本设置" name="2" class="nopaddingbottom">

            <el-form-item v-if="configData.chartOption.liText.lefttext!==undefined" label="左侧文本对齐方式">
              <el-select v-model="configData.chartOption.liText.lefttext" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.righttext!==undefined" label="右侧对齐方式">
              <el-select v-model="configData.chartOption.liText.righttext" placeholder="请选择">
                <el-option label="居左" value="left"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="right"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.size!==undefined" label="文本字号">
              <el-slider v-model="configData.chartOption.liText.size" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.colorType!==undefined" label="文本字色显示类型">
              <el-select v-model="configData.chartOption.liText.colorType" placeholder="请选择" @change="liTextColorTypeChange">
                <el-option label="单个颜色" value="1"></el-option>
                <el-option label="根据字段控制" value="2"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.color!==undefined&&configData.chartOption.liText.colorType==1||configData.chartOption.liText.color!==undefined&&configData.chartOption.liText.colorType===undefined" label="文本字色">
              <el-color-picker v-model="configData.chartOption.liText.color" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.color!==undefined&&configData.chartOption.liText.colorType==2">
              <div slot="label">
                <span style="margin-right: 10px">文本字色</span>
              </div>
              <div style="">
                <el-button type="text" @click="addLiTextcolorItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.liText.color" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-input v-model="configData.chartOption.liText.color[index].value" placeholder="请输入对应颜色的值" />
                <el-color-picker v-model="configData.chartOption.liText.color[index].color" show-alpha style="width: 32px;"></el-color-picker>
                <el-button style="margin-left: 10px" size="mini" @click="delLiTextcolorItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.weight!==undefined" label="文字粗细">
              <el-select v-model="configData.chartOption.liText.weight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            
            <el-form-item label="是否溢出隐藏" v-if="configData.chartOption.liText.isOverHide!==undefined">
              <el-radio-group v-model="configData.chartOption.liText.isOverHide">
                <el-radio :label="false">否</el-radio>
                <el-radio :label="true">是</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="是否显示省略号" v-if="configData.chartOption.liText.isOverHide">
              <el-radio-group v-model="configData.chartOption.liText.isShowEllipsis">
                <el-radio :label="false">否</el-radio>
                <el-radio :label="true">是</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.family!==undefined" label="文本字体">
              <el-select v-model="configData.chartOption.liText.family" placeholder="请选择">
                <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.lineHeight!==undefined" :label="configData.chartOption.isRoll?'文本行高px':'文本行高%'">
              <el-slider v-model="configData.chartOption.liText.lineHeight" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.minwidth!==undefined" label="单个文本最小宽度比%">
              <el-slider v-model="configData.chartOption.liText.minwidth" :min="0" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.isThousand!==undefined" label="千位分隔符">
              <el-select v-model="configData.chartOption.isThousand" placeholder="请选择">
                <el-option label="无分隔符，例：1000" value="false"></el-option>
                <el-option label="空格，例：1 000" value=" "></el-option>
                <el-option label="逗号，例：1,000" value=","></el-option>
                <el-option label="点号，例：1.000" value="."></el-option>
              </el-select>
            </el-form-item>

          </el-collapse-item>
          <el-collapse-item title="表格单行样式设置" name="22">
              <div>
                <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addStyleList">添加样式列表</el-button>
                <draggable :animation="340" group="selectItem" handle=".option-drag">
                  <div style="display: flex;margin-bottom:10px" class="lableText">
                    <div style="width: 40px;">序号</div>
                    <div style="width: 55px;">字体</div>
                    <div style="width: 55px;">加粗</div>
                    <div style="width: 45px;">颜色</div>
                    <div style="width: 45px;">背景</div>
                    <div style="width: 75px;">对齐</div>
                  </div>

                  <div v-for="(item, index) in configData.chartOption.allStyleList" :key="index" class="select-item" style="align-items: center;">
                    <div style="padding:0 6px;border-radius:50%;border:1px solid #999999;font-size:12px;text-align:center;color:#38A3FB;margin-right:5px;">{{index+1}}</div>
                    <el-select v-model="configData.chartOption.allStyleList[index].family" placeholder="请选择">
                      <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
                    </el-select>
                    <el-select v-model="configData.chartOption.allStyleList[index].weight" placeholder="请选择">
                      <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
                    </el-select>
                    <el-color-picker v-model="configData.chartOption.allStyleList[index].textColor" show-alpha size="small" style="margin:0 3px"></el-color-picker>
                    <el-color-picker v-model="configData.chartOption.allStyleList[index].textBgColor" show-alpha size="small" style="margin:0 3px"></el-color-picker>
                    <el-select v-model="configData.chartOption.allStyleList[index].isLeft" placeholder="请选择">
                      <el-option label="居左" value="left" />
                      <el-option label="居中" value="center" />
                      <el-option label="居右" value="right" />
                    </el-select>
                    <div class="close-btn select-line-icon" @click="removeStyleList(index)">
                      <i class="el-icon-remove-outline" />
                    </div>
                  </div>
                </draggable>
              </div>
              <div>
                <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addliTextStyleList(1)">添加条件</el-button>
                <el-button icon="el-icon-circle-plus-outline" type="text"  @click="addliTextStyleList(2)">添加多选条件</el-button>
              </div>
              <template v-for="(item, index) in configData.chartOption.liTextStyleList">
                <div :key="index" v-if="configData.chartOption.liTextStyleList!==undefined" style="margin-top:10px">
                  <el-select v-model="configData.chartOption.liTextStyleList[index].rowFiledIndex" clearable multiple filterable placeholder="列" v-if="configData.chartOption.liTextStyleList[index].type&&configData.chartOption.liTextStyleList[index].type==2">
                    <template v-for="(value , inx) in filedListArr">
                      <el-option :key="'type'+inx" :value="inx" :label="value"/>
                    </template>
                  </el-select>
                  <el-select v-model="configData.chartOption.liTextStyleList[index].rowFiledIndex" filterable placeholder="列" v-else>
                    <template v-for="(value , inx) in filedListArr">
                      <el-option :key="'b'+inx" :value="inx" :label="value"/>
                    </template>
                  </el-select>
                  <div class="select-item" style="align-items: center;margin-top:10px">
                    <div class="color_list_con">
                      <el-form-item label="文本字号px">
                        <el-slider v-model="configData.chartOption.liTextStyleList[index].fontSize" :min="0" :step="1" :max="200" show-input></el-slider>
                      </el-form-item>
                      <el-form-item label="文本圆弧px">
                        <el-slider v-model="configData.chartOption.liTextStyleList[index].borderRadio" :min="0" :step="1" :max="200" show-input></el-slider>
                      </el-form-item>
                      <el-form-item :label="configData.chartOption.isRoll?'文本行高px':'文本行高%'">
                        <el-slider v-model="configData.chartOption.liTextStyleList[index].lineHeight" :min="0" :step="1" :max="200" show-input></el-slider>
                      </el-form-item>
                      <el-form-item label="单个文本最小宽度比%">
                        <el-slider v-model="configData.chartOption.liTextStyleList[index].minwidth" :min="0" :max="100" :step="1" show-input></el-slider>
                      </el-form-item>
                      <el-form-item label="内边距%">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>上：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].paddingTop" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>右：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].paddingRight" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>下：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].paddingBottom" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>左：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].paddingLeft" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                      </el-form-item>
                      <el-form-item label="外边距%">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>上：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].marginTop" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>右：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].marginRight" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>下：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].marginBottom" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>左：</span><el-slider style="width: 88%;" v-model="configData.chartOption.liTextStyleList[index].marginLeft" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                      </el-form-item>
                      <el-form-item label="上边框(px)" v-if="configData.chartOption.liTextStyleList[index].borderTop!==undefined">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>宽度：</span><el-slider style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderTop.width" :min="0" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>颜色：</span><el-color-picker v-model="configData.chartOption.liTextStyleList[index].borderTop.color" show-alpha size="small" style="margin:0 3px"></el-color-picker></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>类型：</span>
                          <el-select style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderTop.type" placeholder="请选择">
                            <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                          </el-select>
                        </div>
                      </el-form-item>
                      <el-form-item label="右边框(px)" v-if="configData.chartOption.liTextStyleList[index].borderRight!==undefined">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>宽度：</span><el-slider style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderRight.width" :min="0" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>颜色：</span><el-color-picker v-model="configData.chartOption.liTextStyleList[index].borderRight.color" show-alpha size="small" style="margin:0 3px"></el-color-picker></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>类型：</span>
                          <el-select style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderRight.type" placeholder="请选择">
                            <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                          </el-select>
                        </div>
                      </el-form-item>
                      <el-form-item label="下边框(px)" v-if="configData.chartOption.liTextStyleList[index].borderBottom!==undefined">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>宽度：</span><el-slider style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderBottom.width" :min="0" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>颜色：</span><el-color-picker v-model="configData.chartOption.liTextStyleList[index].borderBottom.color" show-alpha size="small" style="margin:0 3px"></el-color-picker></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>类型：</span>
                          <el-select style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderBottom.type" placeholder="请选择">
                            <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                          </el-select>
                        </div>
                      </el-form-item>
                      <el-form-item label="左边框(px)" v-if="configData.chartOption.liTextStyleList[index].borderLeft!==undefined">
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>宽度：</span><el-slider style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderLeft.width" :min="0" :max="100" :step="1" show-input></el-slider></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>颜色：</span><el-color-picker v-model="configData.chartOption.liTextStyleList[index].borderLeft.color" show-alpha size="small" style="margin:0 3px"></el-color-picker></div>
                        <div style="display:flex;justify-content:space-between;align-items:center;"><span>类型：</span>
                          <el-select style="width: 80%;" v-model="configData.chartOption.liTextStyleList[index].borderLeft.type" placeholder="请选择">
                            <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                          </el-select>
                        </div>
                      </el-form-item>
                      <el-form-item label="文本样式控制方式">
                        <el-select v-model="configData.chartOption.liTextStyleList[index].styleType" placeholder="请选择">
                          <el-option label="单个样式" value="1"></el-option>
                          <el-option label="根据字段控制" value="2"></el-option>
                        </el-select>
                      </el-form-item>
                      <el-form-item v-if="configData.chartOption.allStyleList&&configData.chartOption.allStyleList.length>0">
                        <div slot="label">
                          <span style="margin-right: 10px">对应文本行样式设置</span>
                        </div>
                        <template v-if="configData.chartOption.liTextStyleList[index].styleType&&configData.chartOption.liTextStyleList[index].styleType==2">
                          <div style="margin-top: -10px">
                            <el-button type="text" @click="addStyleStatusList(index)">+ 添加</el-button>
                          </div>
                          <div v-for="(tmpitem, idx) in configData.chartOption.liTextStyleList[index].styleStatusList" :key="'c' + idx" style="margin-bottom: 10px;display:flex;align-items:center;">
                            <el-input v-model="configData.chartOption.liTextStyleList[index].styleStatusList[idx].value" placeholder="请输入对应样式的值" />
                            <el-select v-model="configData.chartOption.liTextStyleList[index].styleStatusList[idx].index" placeholder="请选择对应样式序号">
                              <el-option v-for="(item,index2) in configData.chartOption.allStyleList" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                            </el-select>
                            <el-button style="margin-left: 10px" size="mini" @click="delStyleStatusList(index,idx)" type="danger" icon="el-icon-delete" circle></el-button>
                          </div>
                        </template>
                        <template v-else>
                          <el-select v-model="configData.chartOption.liTextStyleList[index].styleStatusIndex" placeholder="请选择对应样式序号">
                            <el-option v-for="(item,index2) in configData.chartOption.allStyleList" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                          </el-select>
                        </template>
                      </el-form-item>
                    </div>
                    <div class="close-btn select-line-icon" @click="removeliTextStyleList(index)">
                      <i class="el-icon-remove-outline" />
                    </div>
                  </div>
                </div>
              </template>
              
            
          </el-collapse-item>
          <el-collapse-item title="进度条设置" name="4" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.progressbar.baralign!==undefined" label="标题对齐方式">
              <el-select v-model="configData.chartOption.progressbar.baralign" placeholder="请选择">
                <el-option label="居左" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="flex-end"></el-option>
                <el-option label="两边对齐" value="space-between"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.alignItems!==undefined" label="标题对齐方式">
              <el-select v-model="configData.chartOption.progressbar.alignItems" placeholder="请选择">
                <el-option label="居上" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居下" value="flex-end"></el-option>
              </el-select>
            </el-form-item>
            
            <el-form-item v-if="configData.chartOption.progressbar.conBgJianbianList!==undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条容器背景颜色列表</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addBarConBgItem()">+ 添加</el-button>
              </div>
              <div v-for="(barbgitem,inx) in configData.chartOption.progressbar.conBgJianbianList" :key="'bar'+inx" style="margin-bottom: 10px;display:flex;align-items:center;">
                <div class="color_list_con">
                  <div class="color_list_number">{{inx+1}}</div>
                  <div>
                    <el-button type="text" @click="addBarConcolorItem(inx)">+ 添加</el-button>
                  </div>
                  <div v-for="(tmpitem, index) in configData.chartOption.progressbar.conBgJianbianList[inx]" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                    <el-color-picker v-model="configData.chartOption.progressbar.conBgJianbianList[inx][index].color" show-alpha style="width: 32px;"></el-color-picker>
                    <el-slider v-model="configData.chartOption.progressbar.conBgJianbianList[inx][index].percentage" :min="0" :step="1" show-input  style="width: calc(100% - 70px)"></el-slider>
                    <el-button style="margin-left: 10px" size="mini" @click="delBarConcolorItem(inx,index)" type="danger" icon="el-icon-delete" circle></el-button>
                  </div>
                </div>
                <el-button style="margin-left: 10px" size="mini" @click="delBarConBgItem(inx)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.conBgJianbian!==undefined&&configData.chartOption.progressbar.conBgJianbianList!==undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条容器背景颜色值设置</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addBarConValueItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.progressbar.conBgJianbian" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-input v-model="configData.chartOption.progressbar.conBgJianbian[index].value" placeholder="请输入对应颜色的值" />
                <el-select v-model="configData.chartOption.progressbar.conBgJianbian[index].index" placeholder="请选择对应颜色序号">
                  <el-option v-for="(item,index) in configData.chartOption.progressbar.conBgJianbianList" :key="index" :label="'第'+(index+1)+'条'" :value="index"></el-option>
                </el-select>
                <el-button style="margin-left: 10px" size="mini" @click="delBarConValueItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.conBgJianbian!==undefined&&configData.chartOption.progressbar.conBgJianbianList===undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条容器背景</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addConBgItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.progressbar.conBgJianbian" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-color-picker v-model="configData.chartOption.progressbar.conBgJianbian[index].color" show-alpha style="width: 32px;"></el-color-picker>
                <el-slider v-model="configData.chartOption.progressbar.conBgJianbian[index].percentage" :min="0" :step="1" show-input  style="width: calc(100% - 70px)"></el-slider>
                <el-button style="margin-left: 10px" size="mini" @click="delConBgItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.bgJianbianlist!==undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条颜色列表</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addBarBgItem()">+ 添加</el-button>
              </div>
              <div v-for="(baritem,inx) in configData.chartOption.progressbar.bgJianbianlist" :key="'bar'+inx" style="margin-bottom: 10px;display:flex;align-items:center;">
                <div class="color_list_con">
                  <div class="color_list_number">{{inx+1}}</div>
                  <div>
                    <el-button type="text" @click="addBarcolorItem(inx)">+ 添加</el-button>
                  </div>
                  <div v-for="(tmpitem, index) in configData.chartOption.progressbar.bgJianbianlist[inx]" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                    <el-color-picker v-model="configData.chartOption.progressbar.bgJianbianlist[inx][index].color" show-alpha style="width: 32px;"></el-color-picker>
                    <el-slider v-model="configData.chartOption.progressbar.bgJianbianlist[inx][index].percentage" :min="0" :step="1" show-input  style="width: calc(100% - 70px)"></el-slider>
                    <el-button style="margin-left: 10px" size="mini" @click="delBarcolorItem(inx,index)" type="danger" icon="el-icon-delete" circle></el-button>
                  </div>
                </div>
                <el-button style="margin-left: 10px" size="mini" @click="delBarBgItem(inx)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.bgJianbian!==undefined&&configData.chartOption.progressbar.bgJianbianlist!==undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条颜色值设置</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addBarValueItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.progressbar.bgJianbian" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-input v-model="configData.chartOption.progressbar.bgJianbian[index].value" placeholder="请输入对应颜色的值" />
                <el-select v-model="configData.chartOption.progressbar.bgJianbian[index].index" placeholder="请选择对应颜色序号">
                  <el-option v-for="(item,index2) in configData.chartOption.progressbar.bgJianbianlist" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                </el-select>
                <el-button style="margin-left: 10px" size="mini" @click="delBarValueItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.bgJianbian!==undefined&&configData.chartOption.progressbar.bgJianbianlist===undefined">
              <div slot="label">
                <span style="margin-right: 10px">进度条颜色</span>
              </div>
              <div style="margin-top: -10px">
                <el-button type="text" @click="addConBgItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.progressbar.bgJianbian" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-color-picker v-model="configData.chartOption.progressbar.bgJianbian[index].color" show-alpha style="width: 32px;"></el-color-picker>
                <el-slider v-model="configData.chartOption.progressbar.bgJianbian[index].percentage" :min="0" :step="1" show-input  style="width: calc(100% - 70px)"></el-slider>
                <el-button style="margin-left: 10px" size="mini" @click="delConBgItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.size!==undefined" label="进度条字号">
              <el-slider v-model="configData.chartOption.progressbar.size" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.height!==undefined" :label="configData.chartOption.isRoll?'进度条高度px':'进度条高度%'">
              <el-slider v-model="configData.chartOption.progressbar.height" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item label="内边距%">
              <div style="display:flex;justify-content:space-between;align-items:center;"><span>上：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.paddingTop" :min="-100" :max="100" :step="1" show-input></el-slider></div>
              <div style="display:flex;justify-content:space-between;align-items:center;"><span>右：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.paddingRight" :min="-100" :max="100" :step="1" show-input></el-slider></div>
              <div style="display:flex;justify-content:space-between;align-items:center;"><span>下：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.paddingBottom" :min="-100" :max="100" :step="1" show-input></el-slider></div>
              <div style="display:flex;justify-content:space-between;align-items:center;"><span>左：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.paddingLeft" :min="-100" :max="100" :step="1" show-input></el-slider></div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.colorType!==undefined" label="进度条字色显示类型">
              <el-select v-model="configData.chartOption.progressbar.colorType" placeholder="请选择" @change="barcolorTypeChange">
                <el-option label="单个颜色" value="1"></el-option>
                <el-option label="根据字段控制" value="2"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.color!==undefined&&configData.chartOption.progressbar.colorType==1||configData.chartOption.progressbar.color!==undefined&&configData.chartOption.progressbar.colorType===undefined" label="进度条字色">
              <el-color-picker v-model="configData.chartOption.progressbar.color" show-alpha></el-color-picker>
            </el-form-item>           
            <el-form-item v-if="configData.chartOption.progressbar.color!==undefined&&configData.chartOption.progressbar.colorType==2">
              <div slot="label">
                <span style="margin-right: 10px">进度条字色</span>
              </div>
              <div style="">
                <el-button type="text" @click="addbarTextItem()">+ 添加</el-button>
              </div>
              <div v-for="(tmpitem, index) in configData.chartOption.progressbar.color" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                <el-input v-model="configData.chartOption.progressbar.color[index].value" placeholder="请输入对应颜色的值" />
                <el-color-picker v-model="configData.chartOption.progressbar.color[index].color" show-alpha style="width: 32px;"></el-color-picker>
                <el-button style="margin-left: 10px" size="mini" @click="delbarTextItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
              </div>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.weight!==undefined" label="进度条粗细">
              <el-select v-model="configData.chartOption.progressbar.weight" placeholder="请选择">
                <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>

           <el-form-item v-if="configData.chartOption.progressbar.family!==undefined" label="进度条字体">
              <el-select v-model="configData.chartOption.progressbar.family" placeholder="请选择">
                <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.conRadius!==undefined" label="进度条容器圆角角度">
              <el-slider v-model="configData.chartOption.progressbar.conRadius" :min="0" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.progressbar.radius!==undefined" label="进度条圆角角度">
              <el-slider v-model="configData.chartOption.progressbar.radius" :min="0" :step="1" show-input></el-slider>
            </el-form-item>
            
          </el-collapse-item>

          <el-collapse-item title="状态设置" name="41" class="nopaddingbottom">
            <el-form-item label="是否显示状态">
              <el-radio-group v-model="configData.chartOption.progressbar.isshowstatus">
                <el-radio :label="false">否</el-radio>
                <el-radio :label="true">是</el-radio>
              </el-radio-group>
            </el-form-item>
            <div v-if="configData.chartOption.progressbar.isshowstatus">
              <el-form-item v-if="configData.chartOption.progressbar!==undefined" label="排列方式">
                <el-select v-model="configData.chartOption.progressbar.statusflexDirection" placeholder="请选择">
                  <el-option label="左右" value="row"></el-option>
                  <el-option label="上下" value="column"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.barStatusalign!==undefined" label="标题对齐方式">
                <el-select v-model="configData.chartOption.progressbar.barStatusalign" placeholder="请选择">
                  <el-option label="居左" value="flex-start"></el-option>
                  <el-option label="居中" value="center"></el-option>
                  <el-option label="居右" value="flex-end"></el-option>
                  <el-option label="两边对齐" value="space-between"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusAlignItems!==undefined" label="标题对齐方式">
                <el-select v-model="configData.chartOption.progressbar.statusAlignItems" placeholder="请选择">
                  <el-option label="居上" value="flex-start"></el-option>
                  <el-option label="居中" value="center"></el-option>
                  <el-option label="居下" value="flex-end"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusFontSize!==undefined" label="状态字号">
                <el-slider v-model="configData.chartOption.progressbar.statusFontSize" :min="0" :step="1" :max="200" show-input></el-slider>
              </el-form-item>
              <el-form-item label="内边距px">
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>上：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statuspaddingTop" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>右：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statuspaddingRight" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>下：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statuspaddingBottom" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>左：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statuspaddingLeft" :min="-100" :max="100" :step="1" show-input></el-slider></div>
              </el-form-item>
              <el-form-item label="外边距px">
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>上：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statusmarginTop" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>右：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statusmarginRight" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>下：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statusmarginBottom" :min="-100" :max="100" :step="1" show-input></el-slider></div>
                <div style="display:flex;justify-content:space-between;align-items:center;"><span>左：</span><el-slider style="width: 88%;" v-model="configData.chartOption.progressbar.statusmarginLeft" :min="-100" :max="100" :step="1" show-input></el-slider></div>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusbgJianbianlist!==undefined">
                <div slot="label">
                  <span style="margin-right: 10px">状态背景颜色列表</span>
                </div>
                <div style="margin-top: -10px">
                  <el-button type="text" @click="addBarStatusBgItem()">+ 添加</el-button>
                </div>
                <div v-for="(baritem,inx) in configData.chartOption.progressbar.statusbgJianbianlist" :key="'bar'+inx" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <div class="color_list_con">
                    <div class="color_list_number">{{inx+1}}</div>
                    <div>
                      <el-button type="text" @click="addBarStatuscolorItem(inx)">+ 添加</el-button>
                    </div>
                    <div v-for="(tmpitem, index) in configData.chartOption.progressbar.statusbgJianbianlist[inx]" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                      <el-color-picker v-model="configData.chartOption.progressbar.statusbgJianbianlist[inx][index].color" show-alpha style="width: 32px;"></el-color-picker>
                      <el-slider v-model="configData.chartOption.progressbar.statusbgJianbianlist[inx][index].percentage" :min="0" :step="1" show-input  style="width: calc(100% - 70px)"></el-slider>
                      <el-button style="margin-left: 10px" size="mini" @click="delBarStatuscolorItem(inx,index)" type="danger" icon="el-icon-delete" circle></el-button>
                    </div>
                  </div>
                  <el-button style="margin-left: 10px" size="mini" @click="delBarStatusBgItem(inx)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusbgJianbian!==undefined&&configData.chartOption.progressbar.statusbgJianbianlist!==undefined">
                <div slot="label">
                  <span style="margin-right: 10px">状态背景颜色值设置</span>
                </div>
                <div style="margin-top: -10px">
                  <el-button type="text" @click="addBarStatusBgValueItem()">+ 添加</el-button>
                </div>
                <div v-for="(tmpitem, index) in configData.chartOption.progressbar.statusbgJianbian" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <el-input v-model="configData.chartOption.progressbar.statusbgJianbian[index].value" placeholder="请输入对应颜色的值" />
                  <el-select v-model="configData.chartOption.progressbar.statusbgJianbian[index].index" placeholder="请选择对应颜色序号">
                    <el-option v-for="(item,index2) in configData.chartOption.progressbar.statusbgJianbianlist" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                  </el-select>
                  <el-button style="margin-left: 10px" size="mini" @click="delBarStatusBgValueItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statuscolorType!==undefined" label="状态字色显示类型">
                <el-select v-model="configData.chartOption.progressbar.statuscolorType" placeholder="请选择" @change="barstatuscolorTypeChange">
                  <el-option label="单个颜色" value="1"></el-option>
                  <el-option label="根据字段控制" value="2"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusFontColor!==undefined&&configData.chartOption.progressbar.statuscolorType==1||configData.chartOption.progressbar.statusFontColor!==undefined&&configData.chartOption.progressbar.statuscolorType===undefined" label="进度条字色">
                <el-color-picker v-model="configData.chartOption.progressbar.statusFontColor" show-alpha></el-color-picker>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusFontColor!==undefined&&configData.chartOption.progressbar.statuscolorType==2">
                <div slot="label">
                  <span style="margin-right: 10px">状态字色</span>
                </div>
                <div style="">
                  <el-button type="text" @click="addbarStatusTextItem()">+ 添加</el-button>
                </div>
                <div v-for="(tmpitem, index) in configData.chartOption.progressbar.statusFontColor" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <el-input v-model="configData.chartOption.progressbar.statusFontColor[index].value" placeholder="请输入对应颜色的值" />
                  <el-color-picker v-model="configData.chartOption.progressbar.statusFontColor[index].color" show-alpha style="width: 32px;"></el-color-picker>
                  <el-button style="margin-left: 10px" size="mini" @click="delbarStatusTextItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
              <el-form-item label="是否显示边框">
                <el-radio-group v-model="configData.chartOption.progressbar.isshowstatusBorder">
                  <el-radio :label="false">否</el-radio>
                  <el-radio :label="true">是</el-radio>
                </el-radio-group>
              </el-form-item>
              <div v-if="configData.chartOption.progressbar.isshowstatusBorder">
                <el-form-item v-if="configData.chartOption.progressbar.statusBorderWidth!==undefined" label="边框宽度">
                  <el-slider v-model="configData.chartOption.progressbar.statusBorderWidth" :min="0" :step="1" :max="200" show-input></el-slider>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.progressbar.statusBorderShowType!==undefined" label="状态边框显示类型">
                  <el-select v-model="configData.chartOption.progressbar.statusBorderShowType" placeholder="请选择" @change="barstatusBorderTypeChange">
                    <el-option label="单个颜色" value="1"></el-option>
                    <el-option label="根据字段控制" value="2"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.progressbar.statusBorderType!==undefined" label="状态边框类型">
                  <el-select style="width: 80%;" v-model="configData.chartOption.progressbar.statusBorderType" placeholder="请选择">
                    <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.progressbar.statusBorderColor!==undefined&&configData.chartOption.progressbar.statusBorderShowType==1||configData.chartOption.progressbar.statusBorderColor!==undefined&&configData.chartOption.progressbar.statusBorderShowType===undefined" label="状态边框颜色">
                  <el-color-picker v-model="configData.chartOption.progressbar.statusBorderColor" show-alpha></el-color-picker>
                </el-form-item>
                <el-form-item v-if="configData.chartOption.progressbar.statusBorderColor!==undefined&&configData.chartOption.progressbar.statusBorderShowType==2">
                  <div slot="label">
                    <span style="margin-right: 10px">状态字色</span>
                  </div>
                  <div style="">
                    <el-button type="text" @click="addbarStatusBorderItem()">+ 添加</el-button>
                  </div>
                  <div v-for="(tmpitem, index) in configData.chartOption.progressbar.statusBorderColor" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                    <el-input v-model="configData.chartOption.progressbar.statusBorderColor[index].value" placeholder="请输入对应颜色的值" />
                    <el-color-picker v-model="configData.chartOption.progressbar.statusBorderColor[index].color" show-alpha style="width: 32px;"></el-color-picker>
                    <el-button style="margin-left: 10px" size="mini" @click="delbarStatusBorderItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                  </div>
                </el-form-item>
              </div>
              <el-form-item v-if="configData.chartOption.progressbar.statusweight!==undefined" label="状态字体粗细">
                <el-select v-model="configData.chartOption.progressbar.statusweight" placeholder="请选择">
                  <el-option v-for="(item,index) in fontWeights" :key="index" :label="item" :value="item"></el-option>
                </el-select>
              </el-form-item>

            <el-form-item v-if="configData.chartOption.progressbar.statusfamily!==undefined" label="状态字体">
                <el-select v-model="configData.chartOption.progressbar.statusfamily" placeholder="请选择">
                  <el-option v-for="(item,index) in fontFamilys" :key="index" :label="item" :value="item"></el-option>
                </el-select>
              </el-form-item>
              <el-form-item v-if="configData.chartOption.progressbar.statusconRadius!==undefined" label="状态容器圆角角度">
                <el-slider v-model="configData.chartOption.progressbar.statusconRadius" :min="0" :step="1" show-input></el-slider>
              </el-form-item>
            </div>
          </el-collapse-item>

          <el-collapse-item title="颜色块总体属性设置" name="5" class="nopaddingbottom">
            
            <el-form-item label="循环方向">
              <el-radio-group v-model="configData.chartOption.loopDirection">
                <el-radio label="1">横向</el-radio>
                <el-radio label="2">纵向</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="是否隐藏图标">
              <el-radio-group v-model="configData.chartOption.ishideIcon">
                <el-radio :label="0">否</el-radio>
                <el-radio :label="1">是</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="图标">
              <!-- <el-color-picker v-model="configData.chartOption.blockBgColor" show-alpha></el-color-picker> -->
              <image-upload v-model="configData.chartOption.progressbar.iconUrl" :limit="1"></image-upload>
            </el-form-item> 
            <el-form-item>
              <el-checkbox v-model="configData.chartOption.isRoll">是否自动滚动</el-checkbox>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.rollSpeed!==undefined" label="滚动速度">
              <el-slider v-model="configData.chartOption.rollSpeed" :min="0" :max="10000" :step="0.1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.rollDirection!==undefined" label="滚动方向">
              <el-select v-model="configData.chartOption.rollDirection" placeholder="请选择">
                <el-option label="向下" :value="0"></el-option>
                <el-option label="向上" :value="1"></el-option>
              </el-select>
            </el-form-item>
            
            <el-form-item v-if="configData.chartOption.blockalignContent!==undefined" label="多行对齐方式">
              <el-select v-model="configData.chartOption.blockalignContent" placeholder="请选择">
                <el-option label="居上" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居下" value="flex-end"></el-option>
                <el-option label="两边对齐" value="space-between"></el-option>
                <el-option label="默认" value="stretch"></el-option>
                <el-option label="均匀分布" value="space-around"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.blockBgColorType!==undefined" label="颜色块背景设置类型">
              <el-select v-model="configData.chartOption.blockBgColorType" placeholder="请选择" @change="blockBgColorTypeChange">
                <el-option label="单个颜色" value="1"></el-option>
                <el-option label="根据字段控制" value="2"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="容器背景图片">
              <image-upload v-model="configData.chartOption.containerBgImage" :limit="1"></image-upload>
            </el-form-item>
            <el-form-item label="容器背景颜色">
              <el-color-picker v-model="configData.chartOption.containerBgColor" show-alpha></el-color-picker>
            </el-form-item>
            <el-form-item  label="容器弧度">
              <el-slider v-model="configData.chartOption.containerRadius"  :step="1" show-input/>
            </el-form-item>
            <el-form-item label="颜色块背景设置类型">
              <el-select v-model="configData.chartOption.blockBgType" placeholder="请选择" @change="blockBgTypeChange">
                <el-option label="颜色" value="0"></el-option>
                <el-option label="图片" value="1"></el-option>
              </el-select>
            </el-form-item>
            <div v-if="!configData.chartOption.blockBgType||configData.chartOption.blockBgType=='0'">
              <el-form-item v-if="configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType==1" label="内容块背景颜色">
                <el-color-picker v-model="configData.chartOption.blockBgColor" show-alpha></el-color-picker>
              </el-form-item>    
              <el-form-item v-if="configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType==2||configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType===undefined">
                <div slot="label">
                  <span style="margin-right: 10px">内容块背景颜色</span>
                </div>
                <div style="">
                  <el-button type="text" @click="addBlockBgItem()">+ 添加</el-button>
                </div>
                <div v-for="(tmpitem, index) in configData.chartOption.blockBgColor" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <el-input v-model="configData.chartOption.blockBgColor[index].value" placeholder="请输入对应颜色的值"/>
                  <el-color-picker v-model="configData.chartOption.blockBgColor[index].color" show-alpha style="width: 32px;"></el-color-picker>
                  <el-button style="margin-left: 10px" size="mini" @click="delBlockBgItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
            </div>
            <div v-if="configData.chartOption.blockBgType&&configData.chartOption.blockBgType=='1'">
              <el-form-item v-if="configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType==1" label="内容块背景图片">
                <!-- <el-color-picker v-model="configData.chartOption.blockBgColor" show-alpha></el-color-picker> -->
                <image-upload v-model="configData.chartOption.blockBgColor" :limit="1"></image-upload>
              </el-form-item>    
              <el-form-item v-if="configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType==2||configData.chartOption.blockBgColor!==undefined&&configData.chartOption.blockBgColorType===undefined">
                <div slot="label">
                  <span style="margin-right: 10px">内容块背景图片</span>
                </div>
                <div style="">
                  <el-button type="text" @click="addBlockBgItem()">+ 添加</el-button>
                </div>
                <div v-for="(tmpitem, index) in configData.chartOption.blockBgColor" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <el-input v-model="configData.chartOption.blockBgColor[index].value" placeholder="请输入对应背景的值"/>
                  <!-- <el-color-picker v-model="configData.chartOption.blockBgColor[index].color" show-alpha style="width: 32px;"></el-color-picker> -->
                  <image-upload v-model="configData.chartOption.blockBgColor[index].image" :limit="1"></image-upload>
                  <el-button style="margin-left: 10px" size="mini" @click="delBlockBgItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
            </div>
            <el-form-item v-if="configData.chartOption.align!==undefined" label="对齐方式">
              <el-select v-model="configData.chartOption.align" placeholder="请选择">
                <el-option label="居左" value="flex-start"></el-option>
                <el-option label="居中" value="center"></el-option>
                <el-option label="居右" value="flex-end"></el-option>
                <el-option label="两边对齐" value="space-between"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.liText.marginLeft!==undefined" label="文本内容左右边距">
              <el-slider v-model="configData.chartOption.liText.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.liText.marginTop!==undefined" label="文本内容上下边距">
              <el-slider v-model="configData.chartOption.liText.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.marginLeft!==undefined" label="文本块左右边距">
              <el-slider v-model="configData.chartOption.marginLeft" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.marginTop!==undefined" label="文本块上下边距">
              <el-slider v-model="configData.chartOption.marginTop" :min="0" :step="1" :max="200" show-input></el-slider>
            </el-form-item>

            <el-form-item v-if="configData.chartOption.bradius!==undefined" label="圆角角度">
              <el-slider v-model="configData.chartOption.bradius" :min="0" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="configData.chartOption.blockwidth!==undefined" label="文本块宽度占比%">
              <el-slider v-model="configData.chartOption.blockwidth" :min="0" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item v-if="!configData.chartOption.isRoll&&configData.chartOption.blockheight!==undefined" label="文本块高度占比%">
              <el-slider v-model="configData.chartOption.blockheight" :min="0" :max="100" :step="1" show-input></el-slider>
            </el-form-item>
            <el-form-item label="设置边框">
              <el-switch v-model="configData.chartOption.isetBorder" />
            </el-form-item>
            <el-form-item v-if="configData.chartOption.isetBorder" label="边框设置类型">
              <el-select v-model="configData.chartOption.blockBorderColorType" placeholder="请选择" @change="blockBorderColorTypeChange">
                <el-option label="单个设置" value="1"></el-option>
                <el-option label="根据字段控制" value="2"></el-option>
              </el-select>
            </el-form-item>
            <div style="" v-if="configData.chartOption.isetBorder">
              <el-button type="text" @click="addBorderInfoItem()">+ 添加</el-button>
            </div>
            <template v-if="configData.chartOption.isetBorder&&configData.chartOption.borderInfolist!==undefined&&configData.chartOption.borderInfolist.length" >
              <el-form-item :label="'边框'+(index+1)" v-for="(it,index) in configData.chartOption.borderInfolist" :key="'bo'+index">
                  <div style="margin-bottom: 10px;display:flex;align-items:center;">
                    <div>
                      <div style="display:flex;justify-content:space-between;align-items:center;"><span>宽度：</span><el-slider style="width: 80%;" v-model="configData.chartOption.borderInfolist[index].width" :min="0" :max="100" :step="1" show-input></el-slider></div>
                      <div style="display:flex;justify-content:space-between;align-items:center;"><span>颜色：</span><el-color-picker v-model="configData.chartOption.borderInfolist[index].color" show-alpha size="small" style="margin:0 3px"></el-color-picker></div>
                      <div style="display:flex;justify-content:space-between;align-items:center;"><span>类型：</span>
                        <el-select style="width: 80%;" v-model="configData.chartOption.borderInfolist[index].type" placeholder="请选择">
                          <el-option v-for="ite in borderTypeList" :key="ite.value" :label="ite.label" :value="ite.value"></el-option>
                        </el-select>
                      </div>
                    </div>
                    <el-button style="margin-left: 10px" size="mini" @click="delBorderInfoItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                  </div>
              </el-form-item>
            </template>
            <div>
              <el-form-item v-if="configData.chartOption.borderInfoVallist!==undefined&&configData.chartOption.blockBorderColorType!==undefined&&configData.chartOption.blockBorderColorType==1" label="边框显示">
                <el-select v-model="configData.chartOption.borderInfoVallist[0].sort" placeholder="请选择对应样式序号">
                  <el-option v-for="(item,index2) in configData.chartOption.borderInfolist" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                </el-select>
              </el-form-item>    
              <el-form-item v-if="configData.chartOption.blockBorderColorType!==undefined&&configData.chartOption.blockBorderColorType==2">
                <div slot="label">
                  <span style="margin-right: 10px">边框显示设置值</span>
                </div>
                <div style="">
                  <el-button type="text" @click="addBorderInfoValItem()">+ 添加</el-button>
                </div>
                <div v-for="(tmpitem, index) in configData.chartOption.borderInfoVallist" :key="'a' + index" style="margin-bottom: 10px;display:flex;align-items:center;">
                  <el-input v-model="configData.chartOption.borderInfoVallist[index].value" placeholder="请输入对应边框的值"/>
                  <el-select v-model="configData.chartOption.borderInfoVallist[index].sort" placeholder="请选择对应样式序号">
                    <el-option v-for="(item,index2) in configData.chartOption.borderInfolist" :key="index2" :label="'第'+(index2+1)+'条'" :value="index2"></el-option>
                  </el-select>
                  <el-button style="margin-left: 10px" size="mini" @click="delBorderInfoValItem(index)" type="danger" icon="el-icon-delete" circle></el-button>
                </div>
              </el-form-item>
            </div>
          </el-collapse-item>

          <el-collapse-item title="动画" name="6" class="nopaddingbottom">
            <el-form-item v-if="configData.chartOption.animate!==undefined" label="载入动画">
              <el-select v-model="configData.chartOption.animate" placeholder="请选择">
                <el-option v-for="item in animateOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
              </el-select>
            </el-form-item>
          </el-collapse-item>
        </el-collapse>

        </el-form>
    </el-scrollbar>
  </div>
</template>

<script>
import { animateOptions } from "../../../animate/animate";
import { fontFamilys } from "../../../ComponentsConfig";
import draggable from "vuedraggable";
export default {
  props: ["costomData","tableColum","filedListArr"],
  data() {
    return {
      fontFamilys:fontFamilys,
      fontWeights:['normal','bold','bolder','lighter'],
      activeNames: ['1'],
      animateOptions,
      configData: this.costomData,
      borderTypeList:[
      {value:'solid',label:'实线'},
      {value:'dotted',label:'点线'},
      {value:'dashed',label:'虚线'},
      {value:'double',label:'双实线'},
      {value:'groove',label:'3D凹槽'},
      {value:'ridge',label:'3D垄状'},
      {value:'inset',label:'3D内嵌'},
      {value:'outset',label:'3D外凸'},
    ]
    };
  },
  //页面加载完执行
  mounted() {},
  computed: {},
  components: {
    draggable
  },
  methods: {
    addStyleList(){
      //添加总样式列表
      
      if(!this.configData.chartOption.allStyleList||this.configData.chartOption.allStyleList==undefined){
        this.configData.chartOption.allStyleList=[]
      }
      this.configData.chartOption.allStyleList.push({
        textColor:'',//
        textBgColor:'',
        isLeft:'',
        weight:'normal',
        family:'黑体',
        fontSize:14
      })
    },
    removeStyleList(inx){
      //删除总样式列表
      this.configData.chartOption.allStyleList.splice(inx,1)
    },
    addliTextStyleList(val){
      //添加单行的文本样式设置
      if(!this.configData.chartOption.liTextStyleList||this.configData.chartOption.liTextStyleList==undefined){
        this.configData.chartOption.liTextStyleList=[]
      }
      this.configData.chartOption.liTextStyleList.push({
        rowFiledIndex:val==2?[0]:0,//
        type:val,
        lineHeight:this.configData.chartOption.liText.lineHeight,
        minwidth:this.configData.chartOption.liText.minwidth,
        paddingTop:this.configData.chartOption.liText.paddingTop,
        paddingRight:this.configData.chartOption.liText.paddingRight,
        paddingBottom:this.configData.chartOption.liText.paddingBottom,
        paddingLeft:this.configData.chartOption.liText.paddingLeft,
        marginTop:this.configData.chartOption.liText.marginTop,
        marginRight:this.configData.chartOption.liText.marginRight,
        marginBottom:this.configData.chartOption.liText.marginBottom,
        marginLeft:this.configData.chartOption.liText.marginLeft,
        borderRadio:5,
        styleStatusList:[],
        styleType:'1',
        styleStatusIndex:0,
        borderTop:{
          color:'#fff',
          width:0,
          type:'solid'
        },
        borderRight:{
          color:'#fff',
          width:0,
          type:'solid'
        },
        borderBottom:{
          color:'#fff',
          width:0,
          type:'solid'
        },
        borderLeft:{
          color:'#fff',
          width:0,
          type:'solid'
        },
      })
    },
    removeliTextStyleList(inx){
      //删除添加单行的文本样式设置
      this.configData.chartOption.liTextStyleList.splice(inx,1)
    },
    addStyleStatusList(index){
      this.configData.chartOption.liTextStyleList[index].styleStatusList.push({
        value:'',
        index:0
      })
    },
    delStyleStatusList(index,idx){
      this.configData.chartOption.liTextStyleList[index].styleStatusList.splice(idx,1)
    },
    addBarConValueItem(){
      //添加进度条容器设置的值
      this.configData.chartOption.progressbar.conBgJianbian.push({
        value:'',
        index:0
      })
    },
    delBarConValueItem(inx){
      //删除进度条容器值
      this.configData.chartOption.progressbar.conBgJianbian.splice(inx, 1);
    },
    addBarConBgItem(){
      //添加进度条容器颜色列表
      this.configData.chartOption.progressbar.conBgJianbianList.push([{
          color:'#1E35BA',
          percentage:100
        }])
      // this.configData.chartOption.blockBgColor.push({
      //   value:'',
      //   color:'#071869',
        
      // });
    },
    delBarConBgItem(inx){
      //删除进度条容器颜色列表
      this.configData.chartOption.progressbar.conBgJianbianList.splice(inx, 1);
    },
    addBarConcolorItem(inx){
      //添加进度条容器颜色的每一条颜色
      this.configData.chartOption.progressbar.conBgJianbianList[inx].push({
          color:'#1E35BA',
          percentage:100
        })
    },
    delBarConcolorItem(inx,index){
      //删除进度条容器颜色的每一条颜色
      this.configData.chartOption.progressbar.conBgJianbianList[inx].splice(index, 1);
    },
    addBarValueItem(){
      //添加进度条的值
      this.configData.chartOption.progressbar.bgJianbian.push({
        value:'',
        index:0
      })
    },
    addBarStatusBgValueItem(){
      //添加进度条背景颜色的值
      this.configData.chartOption.progressbar.statusbgJianbian.push({
        value:'',
        index:0
      })
    },
    delBarValueItem(inx){
      //删除进度条值
      this.configData.chartOption.progressbar.bgJianbian.splice(inx, 1);
    },
    delBarStatusBgValueItem(inx){
      //删除进度条背景颜色的值
      this.configData.chartOption.progressbar.statusbgJianbian.splice(inx, 1);
    },
    addBarBgItem(){
      //添加进度条颜色列表
      this.configData.chartOption.progressbar.bgJianbianlist.push([{
          color:'#1E35BA',
          percentage:100
        }])
    },
    addBarStatusBgItem(){
      //添加进度条状态背景颜色列表
      this.configData.chartOption.progressbar.statusbgJianbianlist.push([{
        color:'#1E35BA',
        percentage:100
      }])
    },
    delBarBgItem(inx){
      //删除进度条颜色列表
      this.configData.chartOption.progressbar.bgJianbianlist.splice(inx, 1);
    },
    delBarStatusBgItem(inx){
      //删除进度条状态背景颜色列表
      this.configData.chartOption.progressbar.statusbgJianbianlist.splice(inx, 1);
    },
    addBarcolorItem(inx){
      //添加进度条颜色的每一条颜色
      this.configData.chartOption.progressbar.bgJianbianlist[inx].push({
          color:'#1E35BA',
          percentage:100
        })
    },
    addBarStatuscolorItem(inx){
      //添加进度条状态背景颜色的每一条颜色
      this.configData.chartOption.progressbar.statusbgJianbianlist[inx].push({
          color:'#1E35BA',
          percentage:100
        })
    },
    delBarcolorItem(inx,index){
      //删除进度条颜色的每一条颜色
      this.configData.chartOption.progressbar.bgJianbianlist[inx].splice(index, 1);
    },
    delBarStatuscolorItem(inx,index){
      //删除进度条状态背景颜色的每一条颜色
      this.configData.chartOption.progressbar.statusbgJianbianlist[inx].splice(index, 1);
    },
    blockBgColorTypeChange(val){
      //背景颜色展示类型变化
      if(this.configData.chartOption.blockBgType&&this.configData.chartOption.blockBgType=='1'){
        if(val==1){
          this.configData.chartOption.blockBgColor=''
        }else if(val==2){
          this.configData.chartOption.blockBgColor=[{
            value:'',
            image:''
          }]
        }
      }else{
        if(val==1){
          this.configData.chartOption.blockBgColor='rgba(7, 24, 105, 1)'
        }else if(val==2){
          this.configData.chartOption.blockBgColor=[{
            value:'',
            color:'rgba(7, 24, 105, 1)'
          }]
        }
      }
      
    },
    blockBorderColorTypeChange(val){
      //背景颜色展示类型变化
      if(this.configData.chartOption.borderInfolist==undefined){
        this.configData.chartOption.borderInfolist=[{
          width:1,
          type:'solid',
          color:'#fff'
        }]
      }
    },
    blockBgTypeChange(val){
      //背景颜色展示类型变化
      if(val=='0'){
        if(this.configData.chartOption.blockBgColorType==1){
          this.configData.chartOption.blockBgColor='rgba(7, 24, 105, 1)'
        }else if(this.configData.chartOption.blockBgColorType==2){
          this.configData.chartOption.blockBgColor=[{
            value:'',
            color:'rgba(7, 24, 105, 1)',
            image:''
          }]
        }
      }else{
        if(this.configData.chartOption.blockBgColorType==1){
          this.configData.chartOption.blockBgColor=''
        }else if(this.configData.chartOption.blockBgColorType==2){
          this.configData.chartOption.blockBgColor=[{
            value:'',
            image:'',
            color:'',
          }]
        }
      }
      
    },
    addBorderInfoItem(){
      if(this.configData.chartOption.borderInfolist===undefined){
        this.$set(this.configData.chartOption,'borderInfolist',[])
      }
      this.configData.chartOption.borderInfolist.push({
        width:1,
        type:'solid',
        color:'#fff'
      })
    },
    delBorderInfoItem(idx){
      this.configData.chartOption.borderInfolist.splice(idx, 1);
    },
    addBorderInfoValItem(){
      if(this.configData.chartOption.borderInfoVallist===undefined){
        this.$set(this.configData.chartOption,'borderInfoVallist',[])
      }
      this.configData.chartOption.borderInfoVallist.push({
        value:'',
        sort:0,
      })
    },
    delBorderInfoValItem(idx){
      this.configData.chartOption.borderInfoVallist.splice(idx, 1);
    },
    addBlockBgItem(){
      this.configData.chartOption.blockBgColor.push({
        value:'',
        color:'#071869',
        image:''
      });
    },
    delBlockBgItem(idx){
      this.configData.chartOption.blockBgColor.splice(idx, 1);
    },
    barcolorTypeChange(val){
      //进度条字体颜色展示类型变化
      if(val==1){
        this.configData.chartOption.progressbar.color='#ffffff'
      }else if(val==2){
        this.configData.chartOption.progressbar.color=[{
          value:'',
          color:'#ffffff'
        }]
      }
    },
    barstatuscolorTypeChange(val){
      //进度条状态字体颜色展示类型变化
      if(val==1){
        this.configData.chartOption.progressbar.statusFontColor='#ffffff'
      }else if(val==2){
        this.configData.chartOption.progressbar.statusFontColor=[{
          value:'',
          color:'#ffffff'
        }]
      }
    },
    barstatusBorderTypeChange(val){
      //进度条状态边框颜色展示类型变化
      if(val==1){
        this.configData.chartOption.progressbar.statusBorderColor='#E74032'
      }else if(val==2){
        this.configData.chartOption.progressbar.statusBorderColor=[{
          value:'',
          color:'#E74032'
        }]
      }
    },
    addbarTextItem(){
      this.configData.chartOption.progressbar.color.push({
        value:'',
        color:'#ffffff',
      });
    },
    addbarStatusTextItem(){
      //进度条状态字体颜色
      this.configData.chartOption.progressbar.statusFontColor.push({
        value:'',
        color:'#ffffff',
      });
    },
    addbarStatusBorderItem(){
      //进度条状态边框颜色
      this.configData.chartOption.progressbar.statusBorderColor.push({
        value:'',
        color:'#ffffff',
      });
    },
    delbarTextItem(idx){
      this.configData.chartOption.progressbar.color.splice(idx, 1);
    },
    delbarStatusTextItem(idx){
      this.configData.chartOption.progressbar.statusFontColor.splice(idx, 1);
    },
    delbarStatusBorderItem(idx){
      //删除进度条状态边框颜色
      this.configData.chartOption.progressbar.statusBorderColor.splice(idx, 1);
    },
    liTextColorTypeChange(val){
      //内容字体颜色展示类型变化
      if(val==1){
        this.configData.chartOption.liText.color='#ffffff'
      }else if(val==2){
        this.configData.chartOption.liText.color=[{
          value:'',
          color:'#ffffff'
        }]
      }
    },
    addLiTextcolorItem(){
      this.configData.chartOption.liText.color.push({
        value:'',
        color:'#ffffff',
      });
    },
    delLiTextcolorItem(idx){
      this.configData.chartOption.liText.color.splice(idx, 1);
    },
  },
};
</script>

<style lang="scss" scoped>
::v-deep {
  .el-input--medium .el-input__inner {
    height: 32px;
    width: 100%;
  }
  .inputFontSize{
    width: 100%;
  }
  .el-select{
    height: 32px;
    width: 100%;
  }
  .el-input--suffix, .el-input__inner{
    height: 32px;
  }
  .el-select .el-input__icon {
    line-height: 32px; //el-select 改了多高，这边多高
  }
  .lableText .el-form-item__label{
    float: none;
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
.color_list_con{
  border:1px dashed #999999;
  position: relative;
  border-radius: 5px;
  width: 100%;
  padding: 0 5px;
  box-sizing: border-box;
  .color_list_number{
    width: 18px;
    height: 18px;
    border-radius: 5px;
    background: rgba(67,198,249, 0.5);
    line-height: 16px;
    position: absolute;
    right: 0;
    top: 0;
    text-align: center;
    align-content: center;
    color: #ffffff;
    font-size: 12px;
  }
}
</style>