<template>
  <div style="position: relative;">
    <div style="position: absolute;top: 0;left: 0;width:100%;z-index:1;">
      <div style="height: 2px;width:100%;background-color: #dadada;"></div>
      <div class="header">
        <el-menu :default-active="activeSelect" active-text-color="#409eff" class="el-menu-demo shejiqi"
          mode="horizontal" @select="handleSelect">
          <el-menu-item index="configInfo" @click="to('configInfo')">配置信息</el-menu-item>
          <el-menu-item index="physicalModel" @click="to('physicalModel')">物模型</el-menu-item>
          <el-menu-item index="deviceManagement" @click="to('deviceManagement')">设备管理</el-menu-item>
          <el-menu-item index="onlineDebug" @click="to('onlineDebug')">在线调试</el-menu-item>
          <el-menu-item index="dataAnalysis" @click="to('dataAnalysis')">数据解析</el-menu-item>
          <el-menu-item index="warning" @click="to('warning')">报警工单</el-menu-item>
          <el-menu-item index="notice" @click="to('notice')">配置事件通知</el-menu-item>
          <el-menu-item v-show="NetworkWayName != ''" index="protocolList"
            @click="to('protocolList')">协议介绍</el-menu-item>
        </el-menu>
        <div class="name_text">{{ productInfos == null ? "" : productInfos.Name }}</div>
      </div>
    </div>
    <div style="padding-top:50px">
      <div style="padding:20px 20px 0 20px" id="big_con">
        <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }"
          v-if="activeSelect == 'configInfo'">
          <el-row :gutter="10" class="mb8 button_row" style="justify-content:space-between;margin-bottom:10px">
            <div>
              <el-col :span="1.5">
                <el-button type="primary" plain @click="openEditProductDrawer">
                  <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                  <span style="margin-left:6px">修改</span>
                </el-button>
              </el-col>
            </div>
          </el-row>
          <product-info :productInfos="productInfos" :configLoading="configLoading" :NetworkWayName="NetworkWayName"
            :className="className"></product-info>
        </div>
        <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }"
          v-if="activeSelect == 'physicalModel'">
          <div>
            <div class="header" style="border-top:none;padding: 0;border-bottom: 1px solid #dadada;">
              <el-menu :default-active="activeDefinition" active-text-color="#409eff" class="el-menu-demo shejiqi"
                mode="horizontal" @select="showsort = false">
                <el-menu-item index="attribute" @click="definitonSelect('attribute')">属性定义</el-menu-item>
                <el-menu-item index="function" @click="definitonSelect('function')">功能定义</el-menu-item>
                <el-menu-item index="event" @click="definitonSelect('event')">事件定义</el-menu-item>
                <el-menu-item index="expands" @click="definitonSelect('expands')">标签</el-menu-item>
                <el-menu-item v-if="enableStore == true" index="proprules"
                  @click="definitonSelect('proprules')">统计规则</el-menu-item>
                <el-menu-item index="firmwareFiles" @click="definitonSelect('firmwareFiles')">固件文件</el-menu-item>
              </el-menu>
            </div>
            <div style="padding:20px;">
              <div v-if="activeDefinition != 'firmwareFiles' && activeDefinition != 'proprules'">
                <el-row :gutter="10" class="mb8 button_row">
                  <div v-if="activeDefinition != 'expands'">
                    <el-col :span="1.5">
                      <DynamicAddDropdown :treeType="activeDefinition" @menu-click="handleMenuClick">
                      </DynamicAddDropdown>

                    </el-col>
                    <el-col :span="1.5">
                      <el-upload style="display:inline" accept=".json" :multiple="false" :show-file-list="false"
                        action="#" :before-upload="handleImport">
                        <el-button type="primary" plain>
                          <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                          <span style="margin-left:6px">导入</span>
                        </el-button>
                      </el-upload>
                    </el-col>
                    <el-col :span="1.5">
                      <el-button type="primary" plain @click="exportRow(tableData, true)">
                        <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                        <span style="margin-left:6px">导出</span>
                      </el-button>
                    </el-col>
                    <el-col :span="1.5">
                      <el-button v-if="!showsort" type="primary" plain @click="startSort">
                        <span style="margin-left:6px">点这里开始拖动排序</span>
                      </el-button>
                      <el-button v-else type="warning" plain @click="showsort = false">
                        <span style="margin-left:6px">点击这里关闭排序</span>
                      </el-button>
                    </el-col>
                    <el-col :span="1.5">
                      <el-input v-model="searchTxt" placeholder="请输入要搜索的名称" clearable>
                        <i slot="suffix" class="el-input__icon el-icon-search"></i>
                      </el-input>
                    </el-col>

                  </div>

                  <div v-if="activeDefinition == 'expands'">
                    <el-col :span="1.5">
                      <div style="height:36px;line-height:36px;background-color:#F6F9FF;color:#3572FF;padding:0 15px">
                        <el-dropdown trigger="click" @command="handleSetType">
                          <div style="color:#3572FF;cursor: pointer;">
                            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                            <span style="margin-left:6px;margin-right:6px">添加标签</span>
                            <i class="el-icon-arrow-down"></i>
                          </div>
                          <el-dropdown-menu slot="dropdown">
                            <el-dropdown-item v-for="item of typeList" :key="item.value" :command="item.value"
                              v-show="!(activeDefinition == 'expands' && item.value == 'file')">{{
                              item.alabel}}</el-dropdown-item>
                          </el-dropdown-menu>
                        </el-dropdown>
                      </div>
                    </el-col>
                    <el-col :span="1.5">
                      <el-upload style="display:inline" accept=".json" :multiple="false" :show-file-list="false"
                        action="#" :before-upload="handleImport">
                        <el-button type="primary" plain>
                          <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                          <span style="margin-left:6px">导入</span>
                        </el-button>
                      </el-upload>
                    </el-col>
                    <el-col :span="1.5">
                      <el-button type="primary" plain @click="exportRow(tableData, true)">
                        <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                        <span style="margin-left:6px">导出</span>
                      </el-button>
                    </el-col>
                    <el-col :span="1.5">
                      <el-button v-if="!showsort" type="primary" plain @click="startSort">
                        <span style="margin-left:6px">点这里开始拖动排序</span>
                      </el-button>
                      <el-button v-else type="warning" plain @click="showsort = false">
                        <span style="margin-left:6px">点击这里关闭排序</span>
                      </el-button>
                    </el-col>
                    <el-col :span="1.5">
                      <el-input v-model="searchTxt" placeholder="请输入要搜索的名称" clearable>
                        <i slot="suffix" class="el-input__icon el-icon-search"></i>
                      </el-input>
                    </el-col>
                  </div>
                  <right-toolbar :showSearch.sync="showSearch" :isShowSearch="false"
                    @queryTable="getProductInfo()"></right-toolbar>
                </el-row>
                <el-table v-if="activeDefinition != 'firmwareFiles'" v-loading="configLoading" :data="tableData"
                  style="width: 100%;" row-key="code" class="data_table" :row-class-name="sortFilterClass"
                  :header-cell-style="cellSty" border>
                  <el-table-column :prop="its.filed" :label="its.filedName" v-for="its in tableColumnsList"
                    :key="its.filed">
                    <template slot-scope="scope">
                      <div v-if="its.filed == 'type'">{{ typeListMap.get(scope.row.option.type).alabel }}</div>
                      <div v-if="its.filed == 'unit'">{{ scope.row.option.unit }}</div>
                      <div v-else-if="its.filed == 'enable'">
                        <el-switch v-model="scope.row.enable" disabled></el-switch>
                      </div>
                      <div v-else>{{ scope.row[its.filed] }}</div>
                    </template>
                  </el-table-column>
                  <el-table-column label="操作" align="center" width="280" class-name="small-padding fixed-width">
                    <template slot-scope="scope">
                      <el-button type="text" icon="el-icon-edit"
                        @click="editRowData(scope.row, scope.$index)">修改</el-button>
                      <el-button type="text" icon="el-icon-delete"
                        @click="deleteRowData(scope.row, scope.$index)">删除</el-button>
                      <el-button type="text" icon="el-icon-copy-document"
                        @click="copyRowData(scope.row, scope.$index)">拷贝</el-button>
                    </template>
                  </el-table-column>
                </el-table>
              </div>
              <div v-else-if="activeDefinition == 'proprules'">
                <propsRules :productInfos="productInfos"></propsRules>
              </div>
              <div v-else-if="activeDefinition == 'firmwareFiles'">
                <firmwareFiles :productInfos="productInfos" @changeSrc="filesUnload"></firmwareFiles>
              </div>
            </div>
          </div>
        </div>

        <device-manage v-if="activeSelect == 'deviceManagement'" :activeSelect="activeSelect"
          :productInfos="productInfos"></device-manage>
        <online-debug v-show="activeSelect == 'onlineDebug'" :productInfos="productInfos"></online-debug>
        <data-analysis ref="scriptEditor" v-if="activeSelect == 'dataAnalysis'" :productInfo="productInfos"
          :ChannelData="ChannelData" :content="productInfos.InterScripts" @saveCode="saveCode"
          @saveSetData="saveSetData"></data-analysis>
        <div class="elbiaoge_elform" :style="{ 'min-height': 'calc(100vh - 194px' }" v-if="activeSelect == 'warning'">
          <warn-list ref="warning-list" :isComponent="true" :filProductId="productId"></warn-list>
        </div>
        <topicList v-if="activeSelect == 'protocolList'"></topicList>
        <noticeList v-if="activeSelect == 'notice'" :noticeStr="productInfos.NoticeWay" @saveNotice="onSaveNotice">
        </noticeList>

      </div>
    </div>
    <el-drawer
      :title="activeDefinition == 'attribute' ? '属性定义' : (activeDefinition == 'function' ? '功能定义' : (activeDefinition == 'event' ? '事件定义' : '标签'))"
      ref="attrDra" :visible.sync="attrDrawer" :wrapperClosable="false" direction="rtl" :size="attrDrawerSize"
      :destroy-on-close="true">
      <el-form class="attrFrom_con" label-width="100px" :rules="attrRules" ref="attrFrom"
        :model="activeDefinition == 'attribute' ? attrFrom : (activeDefinition == 'function' ? funcFrom : (activeDefinition == 'event' ? eventFrom : expandsForm))">
        <el-form-item label="字段名称" prop="name">
          <el-input v-if="activeDefinition == 'attribute'" v-model="attrFrom.name" placeholder="请输入字段名称" />
          <el-input v-if="activeDefinition == 'expands'" v-model="expandsForm.name" placeholder="请输入字段名称" />
          <el-input v-if="activeDefinition == 'function'" v-model="funcFrom.name" placeholder="请输入字段名称" />
          <el-input v-if="activeDefinition == 'event'" v-model="eventFrom.name" placeholder="请输入字段名称" />
        </el-form-item>
        <el-form-item label="标识符" prop="code">
          <el-input v-if="activeDefinition == 'attribute'" v-model="attrFrom.code" placeholder="请输入标识符"
            @input="attrFrom.code = attrFrom.code.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')"
            :disabled="(isEditCode && attrFrom.code != '') || attrFrom.isfixed == true" />
          <el-input v-if="activeDefinition == 'expands'" :disabled="(isEditCode && expandsForm.code != '')"
            v-model="expandsForm.code" placeholder="请输入标识符"
            @input="expandsForm.code = expandsForm.code.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')" />
          <el-input v-if="activeDefinition == 'function'" v-model="funcFrom.code" placeholder="请输入标识符"
            @input="funcFrom.code = funcFrom.code.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')"
            :disabled="(isEditCode && funcFrom.code != '') || funcFrom.isfixed == true" />
          <el-input v-if="activeDefinition == 'event'" v-model="eventFrom.code" placeholder="请输入标识符"
            @input="eventFrom.code = eventFrom.code.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')"
            :disabled="(isEditCode && eventFrom.code != '') || eventFrom.isfixed == true" />
          <span style="font-size:14px;color:#909399">
            <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="font-size:14px;margin-right:5px;"></i>1到50位字母，数字，下划线
          </span>
        </el-form-item>
        <el-form-item label="标识符前缀" prop="prefixcode"
          v-if="activeDefinition == 'function' || activeDefinition == 'attribute'">
          <el-input v-if="activeDefinition == 'attribute'" v-model="attrFrom.prefixcode" placeholder="表示边缘的设备地址,没有则不填"
            @input="attrFrom.prefixcode = attrFrom.prefixcode.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')" />
          <el-input v-if="activeDefinition == 'function'" v-model="funcFrom.prefixcode" placeholder="表示边缘的设备地址,没有则不填"
            @input="funcFrom.prefixcode = funcFrom.prefixcode.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')" />
        </el-form-item>
        <el-form-item label="默认值" prop="value" v-if="activeDefinition == 'expands'">
          <template v-if="expandsForm.option.type == 'geo'">
            <div>经度<el-input-number style="margin-left:10px;" v-model="expandsForm.value.lng" :min="0"
                :max="180"></el-input-number></div>
            <div style="margin-top:15px;">纬度<el-input-number style="margin-left:10px;" v-model="expandsForm.value.lat"
                :min="0" :max="90"></el-input-number></div>
          </template>
          <template v-else-if="expandsForm.option.type == 'date'">
            <el-date-picker v-model="expandsForm.value" value-format="timestamp" type="datetime"
              placeholder="选择默认日期时间"></el-date-picker>
          </template>
          <template v-else>
            <el-input v-model="expandsForm.value" placeholder="请输入默认值" style="width:280px;"></el-input>
          </template>

        </el-form-item>
        <el-form-item label="属性标识" prop="mapcode" v-if="activeDefinition == 'expands'">
          <el-select filterable v-model="expandsForm.mapcode" placeholder="请选择属性标识" style="width:100%" clearable>
            <el-option v-for="item in attrTableData" v-show="item.option.type == curType" :key="item.code"
              :label="item.name" :value="item.code"></el-option>
          </el-select>
          <span style="font-size:14px;color:#909399">
            <i class="zhongtaiiconfont zhongtai-icon-zhuyi"
              style="font-size:14px;margin-right:5px;"></i>请先在属性定义中创建属性，仅支持对应的数据类型。
          </span>
        </el-form-item>
        <el-form-item label="属性使用者" prop="propshowway" v-if="activeDefinition == 'attribute'">
          <el-checkbox-group v-model="propshowway">
            <el-checkbox label="org" name="showway">来源组织</el-checkbox>
            <el-checkbox label="own" name="showway">拥有者组织</el-checkbox>
            <el-checkbox label="use" name="showway">使用者组织</el-checkbox>
            <el-checkbox label="person" name="showway">其它人员</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="报警级别" prop="Level" v-if="activeDefinition == 'event'">
          <el-select v-model="eventFrom.Level" placeholder="请选择报警级别" style="width:100%"
            @change="eventFrom.SilenceTime = 86400">
            <el-option v-for="item in LevelList" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="告警目标" v-if="activeDefinition == 'event' && eventFrom.Level != -1">
          <el-checkbox-group v-model="eventFrom.Targets">
            <el-checkbox label="org" name="evttarget">来源组织</el-checkbox>
            <el-checkbox label="own" name="evttarget">拥有者组织</el-checkbox>
            <el-checkbox label="use" name="evttarget">使用者组织</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="沉默周期" prop="SilenceTime" v-if="activeDefinition == 'event' && eventFrom.Level != -1">
          <el-select v-model="eventFrom.SilenceTime" placeholder="请选择沉默周期" style="width:100%">
            <el-option label="无" :value="0"></el-option>
            <el-option label="5分钟" :value="300"></el-option>
            <el-option label="1小时" :value="3600"></el-option>
            <el-option label="6小时" :value="21600"></el-option>
            <el-option label="12小时" :value="43200"></el-option>
            <el-option label="1天" :value="86400"></el-option>
            <el-option label="7天" :value="604800"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="是否启用" prop="enable" v-if="activeDefinition == 'expands'">
          <el-switch v-model="expandsForm.enable"></el-switch>
        </el-form-item>
        <el-form-item label="功能使用者" prop="showway" v-if="activeDefinition == 'function'">
          <el-checkbox-group v-model="funshowway">
            <el-checkbox label="org" name="funshowway">来源组织</el-checkbox>
            <el-checkbox label="own" name="funshowway">拥有者组织</el-checkbox>
            <el-checkbox label="use" name="funshowway">使用者组织</el-checkbox>
            <el-checkbox label="person" name="funshowway">其它人员</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="屏蔽方式" v-if="activeDefinition == 'function'">
          <el-radio-group v-model="funcFrom.actionway">
            <el-radio :label="0">隐藏</el-radio>
            <el-radio :label="1">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="启用条件" v-if="activeDefinition == 'function'">
          <div>
            <el-button type="text" @click="onAddCondition">+ 添加</el-button>
          </div>
          <div class="dlg-param-bg" v-if="funcFrom.conditions != null && funcFrom.conditions.length > 0">
            <div class="paramrow" v-for="(ccitem, cidx) in funcFrom.conditions" :key="cidx">
              <span style="margin-right: 10px;width:45px;">条件{{ getCondName(cidx) }}</span>
              <el-select v-model="ccitem.code" placeholder="属性" style="width: 120px;margin-right: 10px;"
                @change="condiChange($event, ccitem)">
                <el-option :label="opx.name" :value="opx.code" v-for="opx in attrTableData" :key="opx.code"></el-option>
              </el-select>
              <el-select placeholder="判断符" v-model="ccitem.compare" style="width: 100px;margin-right: 10px;">
                <el-option label="等于" value="=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="不等于" value="!=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="大于" value=">"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于" value="<"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="大于等于" value=">="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于等于" value="<="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
              </el-select>

              <el-input v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long'" v-model="ccitem.val"
                style="width: 120px" type="number" placeholder="输入比较值" />
              <el-input v-else-if="ccitem.valtype == 'String'" v-model="ccitem.val" type="text" placeholder="输入比较值"
                style="width: 120px" />
              <el-select v-else-if="ccitem.valtype == 'Enum'" placeholder="请选择比较值" v-model="ccitem.val"
                @visible-change="enumVisibleChange($event, ccitem)" style="width: 120px;">
                <el-option v-for="(option, oi) in enumArr" :key="oi" :label="option.key"
                  :value="option.value"></el-option>
              </el-select>
              <el-select v-else-if="ccitem.valtype == 'Bool'" placeholder="请选择布尔值" v-model="ccitem.val"
                style="width: 120px;">
                <el-option label="真" value="True"></el-option>
                <el-option label="假" value="False"></el-option>
              </el-select>
              <el-date-picker v-else-if="ccitem.valtype == 'Date'" style="width: 120px;" v-model="ccitem.val"
                value-format="timestamp" type="datetime" placeholder="请选择日期和时间">
              </el-date-picker>
              <i class="el-icon-delete" style="color: #ff0000;cursor: pointer;margin-left:10px;"
                @click="onDelCondition(cidx)"></i>
            </div>
          </div>
        </el-form-item>
        <el-form-item label="条件组合"
          v-if="activeDefinition == 'function' && funcFrom.conditions != null && funcFrom.conditions.length > 1">
          <div class="dlg-gg-row">
            <el-input v-model="funcFrom.GroupTxt" placeholder="输入条件组关系表达式  &为与，|为或" />
            <el-alert title="使用表达式构建复杂逻辑，例如: (A & B) | C" type="warning" :closable="false"></el-alert>
          </div>
        </el-form-item>
        <el-form-item label="触发方式" v-if="activeDefinition == 'event'">
          <div>
            <el-select placeholder="请选择触发方式" v-model="eventFrom.CondType" style="width: 220px;">
              <el-option label="规则触发" :value="0"></el-option>
              <el-option label="在线触发" :value="1"></el-option>
              <el-option label="离线触发" :value="2"></el-option>
              <el-option label="属性触发" :value="3"></el-option>
            </el-select>
          </div>
        </el-form-item>
        <el-form-item v-if="activeDefinition == 'event' && eventFrom.CondType == 3" label="触发条件">
          <div>
            <el-button type="text" @click="onAddEvtCondition">+ 添加</el-button>
          </div>
          <div class="dlg-param-bg" v-if="eventFrom.PropConditions != null && eventFrom.PropConditions.length > 0">
            <div class="paramrow" v-for="(ccitem, cidx) in eventFrom.PropConditions" :key="cidx">
              <span style="margin-right: 10px;width:45px;">条件{{ getCondName(cidx) }}</span>
              <el-select v-model="ccitem.code" placeholder="属性" style="width: 120px;margin-right: 10px;"
                @change="condiChange($event, ccitem)">
                <el-option :label="opx.name" :value="opx.code" v-for="opx in attrTableData" :key="opx.code"></el-option>
              </el-select>
              <el-select placeholder="判断符" v-model="ccitem.compare" style="width: 100px;margin-right: 10px;">
                <el-option label="等于" value="=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="不等于" value="!=" v-if="ccitem.valtype != 'Date'"></el-option>
                <el-option label="大于" value=">"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于" value="<"
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="大于等于" value=">="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
                <el-option label="小于等于" value="<="
                  v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long' || ccitem.valtype == 'Date'"></el-option>
              </el-select>

              <el-input v-if="ccitem.valtype == 'Double' || ccitem.valtype == 'Long'" v-model="ccitem.val"
                style="width: 120px" type="number" placeholder="输入比较值" />
              <el-input v-else-if="ccitem.valtype == 'String'" v-model="ccitem.val" type="text" placeholder="输入比较值"
                style="width: 120px" />
              <el-select v-else-if="ccitem.valtype == 'Enum'" placeholder="请选择比较值" v-model="ccitem.val"
                @visible-change="enumVisibleChange($event, ccitem)" style="width: 120px;">
                <el-option v-for="(option, oi) in enumArr" :key="oi" :label="option.key"
                  :value="option.value"></el-option>
              </el-select>
              <el-select v-else-if="ccitem.valtype == 'Bool'" placeholder="请选择布尔值" v-model="ccitem.val"
                style="width: 120px;">
                <el-option label="真" value="True"></el-option>
                <el-option label="假" value="False"></el-option>
              </el-select>
              <el-date-picker v-else-if="ccitem.valtype == 'Date'" style="width: 120px;" v-model="ccitem.val"
                value-format="timestamp" type="datetime" placeholder="请选择日期和时间">
              </el-date-picker>
              <i class="el-icon-delete" style="color: #ff0000;cursor: pointer;margin-left:10px;"
                @click="onDelEvtCondition(cidx)"></i>
            </div>
          </div>
        </el-form-item>
        <el-form-item
          v-if="activeDefinition == 'event' && eventFrom.CondType == 3 && eventFrom.PropConditions != null && eventFrom.PropConditions.length > 1"
          label="条件组合">
          <div class="dlg-gg-row">
            <el-input v-model="eventFrom.GroupTxt" placeholder="输入条件组关系表达式  &为与，|为或" />
            <el-alert title="使用表达式构建复杂逻辑，例如: (A & B) | C" type="warning" :closable="false"></el-alert>
          </div>
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-if="activeDefinition == 'attribute'" type="textarea" :rows="2" placeholder="请输入备注说明"
            v-model="attrFrom.description"></el-input>
          <el-input v-if="activeDefinition == 'expands'" type="textarea" :rows="2" placeholder="请输入备注说明"
            v-model="expandsForm.description"></el-input>
          <el-input v-if="activeDefinition == 'function'" type="textarea" :rows="2" placeholder="请输入备注说明"
            v-model="funcFrom.description"></el-input>
          <el-input v-if="activeDefinition == 'event'" type="textarea" :rows="2" placeholder="请输入备注说明"
            v-model="eventFrom.description"></el-input>
          <p v-if="activeDefinition == 'event'" style="color:#999;line-height: 22px;">提示：可用 $标识符 来替换为输入事件的数据</p>
        </el-form-item>
        <el-form-item label="输入参数" v-if="activeDefinition == 'function'">
          <div v-if="inputsList.length > 0">
            <div class="param_list" v-for="(item, inx) in inputsList" :key="inx">
              <div class="list_left">
                <span>{{ item.name }}</span>
                <span class="params_type">{{ typeListMap.get(item.type).label }}</span>
              </div>
              <div class="list_right">
                <i class="el-icon-edit" @click="editParams('inputs', item, inx)"></i>
                <i class="el-icon-delete" @click="deleteParams('inputs', item, inx)"></i>
              </div>
            </div>
          </div>
          <span style="color:#0055FF;cursor: pointer;" @click="openParamsDrawer('inputs')">+输入参数</span>
        </el-form-item>
        <el-form-item label="输出参数" v-if="activeDefinition == 'event'">
          <div v-if="outputsList.length > 0">
            <div class="param_list" v-for="(item, inx) in outputsList" :key="inx">
              <div class="list_left">
                <span>{{ item.name }}</span>
                <span class="params_type">{{ typeListMap.get(item.type).label }}</span>
              </div>
              <div class="list_right">
                <i class="el-icon-edit" @click="editParams('outputs', item, inx)"></i>
                <i class="el-icon-delete" @click="deleteParams('outputs', item, inx)"></i>
              </div>
            </div>
          </div>
          <span style="color:#0055FF;cursor: pointer;" @click="openParamsDrawer('outputs')">+输出参数</span>
        </el-form-item>
        <type-form ref="typeFormAssembly" v-if="activeDefinition == 'attribute' || activeDefinition == 'expands'"
          :activeDefinition="activeDefinition" :enumKeyList="enumKeyList" :paramsForm="paramsForm"
          :attrTableData="attrTableData" @changeMapcode="changeMapcode" @setCurType="setCurType"></type-form>
        <el-alert v-if="activeDefinition == 'expands' && expandsForm.code == 'state'" title="只有设备运行状态在枚举值里时，属性才能变更设备的运行状态"
          type="warning" :closable="false"></el-alert>
        <div v-if="activeDefinition == 'function'">
          <span style="color: #72767b;line-height: 45px;margin-top: 10px;">功能执行</span>
          <div style="padding: 10px 10px;">
            <div style="padding: 15px 10px;border: solid 1px #dadada;border-bottom: none;line-height: 26px;">
              <el-radio-group :value="funcFrom.downway" @input="onDownWayChange">
                <el-radio :label="0">功能脚本</el-radio>
                <el-radio :label="1">下发modbus写入</el-radio>
                <el-radio :label="2">下发modbus读取</el-radio>
                <el-radio :label="3">下发等待属性回复</el-radio>
                <el-radio style="margin-top: 20px;" :label="4">图形化脚本</el-radio>
              </el-radio-group>
            </div>
            <div v-if="funcFrom.downway == 0">
              <fun-analysis v-model="funcFrom.downdata"></fun-analysis>
            </div>
            <div v-if="funcFrom.downway == 1">
              <funModbus ref="funRef" :FunItem="funcFrom"></funModbus>
            </div>
            <div v-if="funcFrom.downway == 2"
              style="padding:15px 20px 15px 20px;border:solid 1px #dadada;background-color: #fafafa;">
              <el-select v-model="funcFrom.downdata" style="width:320px;" placeholder="请选择">
                <el-option v-for="item in modbusData.Matches" :key="item.Name" :label="item.Name" :value="item.Name">
                </el-option>
              </el-select>
            </div>
            <div v-if="funcFrom.downway == 3"
              style="padding:15px 20px 15px 20px;border:solid 1px #dadada;background-color: #fafafa;">
              <el-select v-model="propfuncdata" multiple style="width:320px;" placeholder="请选择">
                <el-option v-for="item in attrTableData" :key="item.code" :label="item.name" :value="item.code">
                </el-option>
              </el-select>
            </div>
            <div v-if="funcFrom.downway == 4"
              style="padding:15px 20px 15px 20px;border:solid 1px #dadada;background-color: #fafafa;text-align: center;">
              <el-button type="primary" plain @click="onEditGraph">打开图形化编程器</el-button>
            </div>
          </div>
        </div>
      </el-form>
      <div class="demo-drawer__footer" style="text-align: center;margin-top:40p;padding-bottom:20px">
        <el-button @click="attrDrawer = false">取 消</el-button>
        <el-button type="primary" @click="saveProductInfo" :loading="saveLoading">{{ saveLoading ? '提交中 ...' : '保 存'}}</el-button>
      </div>
    </el-drawer>
    <el-drawer title="添加参数" ref="funcDra" :wrapperClosable="false" :visible.sync="funcDrawer" @close="closeParamDrawer"
      direction="rtl" size="600px" :destroy-on-close="true">
      <el-form ref="paramsForm" :rules="paramsRules" :model="paramsForm" label-width="100px" class="params_form">
        <el-form-item label="字段名称" prop="name">
          <el-input v-model="paramsForm.name" placeholder="请输入字段名称" />
        </el-form-item>
        <el-form-item label="标识符" prop="code">
          <el-input v-model="paramsForm.code" placeholder="请输入标识符"
            @input="paramsForm.code = paramsForm.code.replace(/[^a-zA-Z0-9_]{1,50}$/g, '')"
            :disabled="isEditCode && paramsForm.code != ''" />
          <span style="font-size:14px;color:#909399">
            <i class="zhongtaiiconfont zhongtai-icon-zhuyi" style="margin-right:5px;font-size:14px;"></i>1到50位字母，数字，下划线
          </span>
        </el-form-item>
        <el-form-item label="数据类型" prop="type">
          <el-select v-model="paramsForm.type" placeholder="请选择数据类型" style="width:100%" @change="onIptChg">
            <el-option v-for="item in typeList" v-show="item.paramshow" :key="item.value" :label="item.label"
              :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <enum-item v-model="paramsForm" v-if="paramsForm.type == 'enum'" :hasKey="true"></enum-item>
        <el-form-item v-if="activeParams == 'inputs'" label="默认值" prop="defval">
          <el-row>
            <el-col :span="14">
              <param-item ref="defParamVal" :Item="paramsForm" :disabled="paramsForm.disabledDef"></param-item>
            </el-col>
            <el-col :span="8" :offset="1">
              <el-checkbox v-model="paramsForm.disabledDef" @change="onParamEnabel">无</el-checkbox>
              <el-checkbox v-model="paramsForm.readOnly">只读</el-checkbox>
            </el-col>
          </el-row>
        </el-form-item>
        <el-form-item label="备注" prop="remark" v-if="activeParams == 'inputs'">
          <el-input type="textarea" :rows="2" placeholder="请输入备注说明" v-model="paramsForm.remark"></el-input>
        </el-form-item>
      </el-form>
      <div class="demo-drawer__footer" style="text-align: center;margin-top:40p;padding-bottom:20px">
        <el-button @click="closeParamDrawer">取 消</el-button>
        <el-button type="primary" @click="joinParams" :loading="paramsLoading">{{ paramsLoading ? '提交中 ...' : '确 定'}}</el-button>
      </div>
    </el-drawer>


    <edit-product-info ref="editProduct" :productInfo="productInfos" @saveInfo="saveInfo"></edit-product-info>

    <copy-item ref="cpyRef" @ok="confirmCopy"></copy-item>
    <graph-code-dialog v-if="funcFrom.downway == 4" ref="graphDlg" :initval="funcFrom.downdata"
      @confirm="funcFrom.downdata = $event"></graph-code-dialog>
  </div>
</template>
<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import {
  classInfo,
  editProduct,
  productInfo,
  channelInfo
} from "@/api/rules/productModel";

let topicList = () => import("@/views/iot/deviceManage/topicList.vue")
let deviceManage = () => import("@/views/iot/deviceManage/deviceCom.vue")
let onlineDebug = () => import("@/views/iot/deviceManage/onlineDebug.vue")
let dataAnalysis = () => import("@/views/iot/physicalModel/dataAnalysis.vue")
let ProductInfo = () => import("./components/productInfo")
let EditProductInfo = () => import("./components/editProductInfo")
let TypeForm = () => import("./components/typeForm")
let warnList = () => import("@/views/iot/physicalModel/warnList.vue")
let noticeList = () => import("@/views/iot/physicalModel/noticeList.vue")
let firmwareFiles = () => import("@/views/iot/physicalModel/components/firmwareFiles.vue")
let propsRules = () => import("@/views/iot/physicalModel/components/propsRules.vue")

let funAnalysis = () => import("@/views/iot/deviceManage/funAnalysis.vue")
let copyItem = () => import("./components/copyItem.vue")
let funModbus = () => import("./components/funModbus.vue")
let paramItem = () => import("../funInput/paramItem.vue")
let enumItem = () => import("../funInput/enumItem.vue")
let GraphCodeDialog = () => import("./components/GraphCodeDialog")
let DynamicAddDropdown = () => import("./components/DynamicAddDropdown.vue");
import Sortable from 'sortablejs';

export default {
  name: "productAdd",
  mixins: [resizeTableCon],
  components: {
    topicList,
    deviceManage,
    onlineDebug,
    dataAnalysis,
    funAnalysis,
    ProductInfo,
    EditProductInfo,
    TypeForm,
    warnList,
    noticeList,
    copyItem,
    funModbus,
    paramItem,
    enumItem,
    firmwareFiles,
    propsRules,
    GraphCodeDialog,
    DynamicAddDropdown
  },
  data() {
    const noRepeat = (rule, value, callback) => {
      if (this.activeSelect == "physicalModel") {
        if (this.activeDefinition == "attribute") {
          if (
            this.attrCodeList.includes(this.attrFrom.code) &&
            this.activeModelLine == -1
          ) {
            // 未上传文件
            callback("该标识符已存在，请重新输入");
          }
        }
        if (this.activeDefinition == "function") {
          if (
            this.funcCodeList.includes(this.funcFrom.code) &&
            this.activeModelLine == -1
          ) {
            // 未上传文件
            callback("该标识符已存在，请重新输入");
          }
        }
        if (this.activeDefinition == "event") {
          if (
            this.eventCodeList.includes(this.eventFrom.code) &&
            this.activeModelLine == -1
          ) {
            // 未上传文件
            callback("该标识符已存在，请重新输入");
          }
        }
        if (this.activeDefinition == "expands") {
          if (
            this.expandsCodeList.includes(this.expandsForm.code) &&
            this.activeModelLine == -1
          ) {
            // 未上传文件
            callback("该标识符已存在，请重新输入");
          }
        }
      }

      callback();
    };
    const noRepeat2 = (rule, value, callback) => {
      if (this.activeSelect == "physicalModel") {
        if (
          this.activeCodeList.includes(this.paramsForm.code) &&
          this.activeParamsLine == -1
        ) {
          // 未上传文件
          callback("该标识符已存在，请重新输入");
        }
      }

      callback();
    };
    return {
      curType: null, //当前物模型数据类型
      typeList: [
        { alabel: "信号", label: "信号强度", value: "signal", paramshow: false },
        { alabel: "状态", label: "运行状态", value: "state", paramshow: false },
        { alabel: "整型", label: "整型(Int)", value: "int", paramshow: true },
        { alabel: "浮点", label: "浮点型(Float)", value: "float", paramshow: true },
        { alabel: "字符", label: "字符型(String)", value: "string", paramshow: true },
        { alabel: "时间", label: "时间型(Date)", value: "date", paramshow: true },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean", paramshow: true },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum", paramshow: true },
        { alabel: "文件", label: "文件类型(File)", value: "file", paramshow: false },
        { alabel: "位置", label: "设备位置(Geo)", value: "geo", paramshow: false }
      ], //数据类型列表
      attrCodeList: [], //用于判断是否包含了该属性定义
      expandsCodeList: [], //用于判断是否包含了该标签
      funcCodeList: [], //用于判断是否包含了该功能定义
      eventCodeList: [], //用于判断是否包含了该事件定义
      inputsCodeList: [],
      outputsCodeList: [],
      activeCodeList: [],
      NetworkWayName: "",
      enumKeyList: [], //枚举key的列表
      enableStore: false,
      isEdit: false, //判断是否是修改数据
      activeParams: "", //inputs,outputs
      inputsList: [], //输入参数列表
      outputsList: [], //输出参数列表
      attrDrawerSize: "600px",
      saveLoading: false, //是否在保存数据
      paramsLoading: false, //是否处于添加参数中
      configLoading: false, //配置信息是否处于
      productInfos: {
        PhotoUrl: null
      },
      tableData: [], //表格数据
      attrTableData: [], //属性定义表格数据
      funcTableData: [], //功能定义表格数据
      eventTableData: [], //事件定义表格数据
      expandsTableData: [], //标签表格数据
      modbusData: {}, //modbus数据
      isEditCode: false,
      attrFrom: {
        //属性定义
        name: "", //名称
        code: "", //标识符
        prefixcode: "",//标识符前缀
        description: "", //描述
        option: {}
      },
      expandsForm: {
        //标签
        name: "", //名称
        code: "", //标识符
        value: 0, //默认值
        mapcode: "", //属性定义绑定
        enable: true, //是否启用
        option: {}
      },
      funcFrom: {
        //功能定义
        name: "", //名称
        code: "", //标识符
        prefixcode: "",//标识符前缀
        description: "", //描述
        showway: "org,own,use,person",
        downway: 0,
        downdata: "",
        inputs: [], //输入参数
        outputs: [] //输出参数
      },
      eventFrom: {
        //事件定义
        name: "", //名称
        code: "", //标识符
        Level: null,
        CondType: 0,
        Targets: ["org"],
        SilenceTime: 86400,
        description: "" //描述
      },
      paramsForm: {
        name: "", //名称
        code: "", //标识符
        option: {}
      },
      attrRules: {
        name: [{ required: true, trigger: "blur", message: "请输入字段名称" }],
        code: [
          { required: true, trigger: "blur", message: "请输入标识符" },
          {
            min: 1,
            max: 50,
            message: "长度在 1 到 50 个字符",
            trigger: "blur"
          },
          { validator: noRepeat, trigger: "blur" }
        ],
        Level: [
          { required: true, trigger: "change", message: "请选择报警级别" }
        ],
        SilenceTime: [
          { required: true, trigger: "change", message: "请选择沉默周期" }
        ]
      },
      paramsRules: {
        name: [{ required: true, trigger: "blur", message: "请输入字段名称" }],
        code: [
          { required: true, trigger: "blur", message: "请输入标识符" },
          {
            min: 1,
            max: 50,
            message: "长度在 1 到 50 个字符",
            trigger: "blur"
          },
          { validator: noRepeat2, trigger: "blur" }
        ]
      },
      attrDrawer: false,
      funcDrawer: false,

      multipleList: [
        { label: 0, value: 0 },
        { label: 1, value: 1 },
        { label: 2, value: 2 },
        { label: 3, value: 3 }
      ], //倍数列表
      activeSelect: "configInfo", //切换页面
      bodyConHei: 0,
      activeDefinition: "attribute",
      // 遮罩层
      loading: false,
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        key: undefined,
        status: undefined
      },
      tableColumnsList: [
        { filed: "name", filedName: "名称" },
        { filed: "code", filedName: "标识符" },
        { filed: "type", filedName: "类型" },
        { filed: "unit", filedName: "单位" },
        { filed: "description", filedName: "说明" }
      ],
      expandsTableColumnsList: [
        { filed: "name", filedName: "名称" },
        { filed: "code", filedName: "标识符" },
        { filed: "type", filedName: "类型" },
        { filed: "unit", filedName: "单位" },
        { filed: "value", filedName: "默认值" },
        { filed: "enable", filedName: "启用" }
      ], // 显示搜索条件
      showSearch: true,
      // 日期范围
      dateRange: [],
      typeListMap: new Map(),
      productId: "",
      classId: "",
      className: "",
      activeModelLine: -1, //当前修改的行是
      activeParamsLine: -1, //当前修改的参数是哪一行
      isFirstGet: true, //是不是第一次获取协议数据
      ChannelData: null,
      fileType: ["png", "jpg", "jpeg", "gif"],
      // 大小限制(MB)
      fileSize: 10,
      LevelList: [
        { label: "忽略（无通知和工单）", value: -1 },
        { label: "普通", value: 0 },
        { label: "告警", value: 1 },
        { label: "紧急", value: 2 }
      ], //告警级别列表
      showarr: [],
      showpparr: [],
      showsort: false,
      searchTxt: "",
      enumArr: [],
    };
  },

  mounted() {
    // let div3 = document.getElementById("app-main");
    // this.bodyConHei = div3.offsetHeight;
    this.setTypeMap();
    let pars = this.$route.query;
    if (pars.classId) {
      this.classId = pars.classId;
      this.getProductClassInfo(); //获取协议分类名称
    }

    if (this.$route.params.id) {
      this.productId = this.$route.params.id;
      this.getProductInfo();
    }
  },
  computed: {
    propshowway: {
      get() {
        return this.showpparr;
      },
      set(val) {
        this.attrFrom.showway = val.join(',');
        this.showpparr = val;
      }
    },
    funshowway: {
      get() {
        return this.showarr;
      },
      set(val) {
        this.funcFrom.showway = val.join(',');
        this.showarr = val;
      }
    },
    propfuncdata: {
      get() {
        if (this.funcFrom.downway == 3) {
          if (this.funcFrom.downdata == null || this.funcFrom.downdata == "") {
            return [];
          }
          return JSON.parse(this.funcFrom.downdata) || [];
        }
        else {
          return [];
        }
      },
      set(val) {
        if (this.funcFrom.downway == 3) {
          this.funcFrom.downdata = JSON.stringify(val);
        }
      }
    }
  },
  methods: {
    handleMenuClick(params) {
      // 根据type处理不同逻辑
      switch (params.type) {
        case "custom":
          // 自定义项逻辑
          this.openAttrDrawer();
          break;
        case "codeItem":
          if (this.activeDefinition == "attribute") {
            this.openAttrDrawer();
            let tmpdata = JSON.parse(params.codeItem.OptionData) || {};
            for (let propName in tmpdata) {
              this.attrFrom[propName] = tmpdata[propName]
            }
            this.attrFrom.name = params.codeItem.Name || "";
            this.attrFrom.code = params.codeItem.Code || "";
            this.attrFrom.prefixcode = "";
            this.attrFrom.isfixed = true;
            this.curType = this.attrFrom.option.type;
            this.$nextTick(() => {
              if (this.$refs["typeFormAssembly"]) {
                this.$refs["typeFormAssembly"].setNOtInputsType(this.attrFrom); //执行子组件typeForm的setNOtInputsType方法
              }
            });
          }
          else if (this.activeDefinition == "function") {
            this.openAttrDrawer();
            let tmpdata = JSON.parse(params.codeItem.OptionData) || {};
            for (let propName in tmpdata) {
              this.funcFrom[propName] = tmpdata[propName]
            }
            this.funcFrom.name = params.codeItem.Name || "";
            this.funcFrom.code = params.codeItem.Code || "";
            this.funcFrom.prefixcode = "";
            this.funcFrom.isfixed = true;
            this.funcFrom.showway = "org,own,use,person";
          }
          else if (this.activeDefinition == "event") {
            this.openAttrDrawer();
            let tmpdata = JSON.parse(params.codeItem.OptionData) || {};
            for (let propName in tmpdata) {
              this.eventFrom[propName] = tmpdata[propName]
            }
            this.eventFrom.name = params.codeItem.Name || "";
            this.eventFrom.code = params.codeItem.Code || "";
            this.eventFrom.Targets = ["org"];
            this.eventFrom.isfixed = true;
          }

          break;
      }
    },

    //修改固件列表
    filesUnload(val) {
      let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL)
      tmpmodeltsl.firmwares = val;
      console.info(tmpmodeltsl);
      this.productInfos.ModelTSL = JSON.stringify(tmpmodeltsl);
      this.saveLoading = true;
      editProduct({
        id: this.productInfos.Id,
        ModelTSL: this.productInfos.ModelTSL
      }).then(rsp => {
        if (rsp.code == 0) {
          this.$modal.msgSuccess("保存成功");
          this.saveLoading = false;
          this.getProductInfo();
        }
      }).catch(err => {
        this.saveLoading = false;
      });

    },
    exportRow(row, isAll) {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let msgitem = {
        t: this.activeDefinition,
        items: []
      };
      if (isAll) {
        msgitem.items = JSON.parse(JSON.stringify(row))
      } else {
        msgitem.items.push(row);
      }
      let tmname = this.activeDefinition;
      tmploading.close();
      const content = JSON.stringify(msgitem)
      const blobData = new Blob([content], { type: 'application/json' })
      const filename = `${tmname}.json` //可以自定义后缀名

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(blobData, filename)
      } else {
        const anchor = document.createElement('a')
        anchor.href = window.URL.createObjectURL(blobData)
        anchor.download = filename
        anchor.click()
        window.URL.revokeObjectURL(blobData)
      }
    },
    handleImport(file) {
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = async (e) => {
        const str = e.target.result
        const jsonData = JSON.parse(str)

        if (jsonData.t != this.activeDefinition) {
          this.$modal.msgError("导入类型错误");
          tmploading.close();
          return;
        }
        let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL);
        switch (this.activeDefinition) {
          case "attribute":
            jsonData.items.forEach(item => {
              let hasfindIndex = tmpmodeltsl.properties.findIndex(rw => rw.code == item.code)
              if (hasfindIndex == -1) {
                tmpmodeltsl.properties.push(item);
              } else if (hasfindIndex !== undefined) {
                tmpmodeltsl.properties[hasfindIndex] = JSON.parse(JSON.stringify(item))
              }

            });
            break;
          case "function":
            jsonData.items.forEach(item => {
              let hasfindIndex = tmpmodeltsl.functions.findIndex(rw => rw.code == item.code)
              if (hasfindIndex == -1) {
                tmpmodeltsl.functions.push(item);
              } else if (hasfindIndex !== undefined) {
                tmpmodeltsl.functions[hasfindIndex] = JSON.parse(JSON.stringify(item))
              }
            });
            break;
          case "event":
            jsonData.items.forEach(item => {
              let hasfindIndex = tmpmodeltsl.events.findIndex(rw => rw.code == item.code)
              if (hasfindIndex == -1) {
                tmpmodeltsl.events.push(item);
              } else if (hasfindIndex !== undefined) {
                tmpmodeltsl.events[hasfindIndex] = JSON.parse(JSON.stringify(item))
              }
            });
            break;
          case "expands":
            jsonData.items.forEach(item => {
              let hasfindIndex = tmpmodeltsl.tags.findIndex(rw => rw.code == item.code)
              if (hasfindIndex == -1) {
                tmpmodeltsl.tags.push(item);
              } else if (hasfindIndex !== undefined) {
                tmpmodeltsl.tags[hasfindIndex] = JSON.parse(JSON.stringify(item))
              }
            });
            break;
        }

        this.productInfos.ModelTSL = JSON.stringify(tmpmodeltsl);
        editProduct({
          id: this.productInfos.Id,
          ModelTSL: this.productInfos.ModelTSL
        }).then(rsp => {
          if (rsp.code == 0) {
            this.$modal.msgSuccess("导入成功");
            this.getProductInfo();
          }
        }).catch(er => {
          tmploading.close();
          return
        });
        tmploading.close();
      }
    },
    sortFilterClass({ row, rowIndex }) {
      if (this.showsort) {
        return "item-sort" + (this.searchTxt.length > 0 && row.name.indexOf(this.searchTxt) == -1 ? " hidden-row" : "");
      }
      else {
        return "no-sort" + (this.searchTxt.length > 0 && row.name.indexOf(this.searchTxt) == -1 ? " hidden-row" : "");
      }
    },
    startSort() {
      this.showsort = true;
      //初始化排序
      const tbody = document.querySelector(".data_table .el-table__body-wrapper tbody");
      new Sortable(tbody, {
        animation: 150,
        filter: ".no-sort",
        // 需要在odEnd方法中处理原始eltable数据，使原始数据与显示数据保持顺序一致
        onEnd: ({ newIndex, oldIndex }) => {
          let tmparr = [];
          let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL);
          if (this.activeDefinition == "attribute") {
            tmparr = tmpmodeltsl.properties;
          }
          if (this.activeDefinition == "function") {
            tmparr = tmpmodeltsl.functions;
          }
          if (this.activeDefinition == "event") {
            tmparr = tmpmodeltsl.events;
          }
          if (this.activeDefinition == "expands") {
            tmparr = tmpmodeltsl.tags;
          }
          const targetRow = tmparr[oldIndex];
          tmparr.splice(oldIndex, 1);
          tmparr.splice(newIndex, 0, targetRow);

          this.productInfos.ModelTSL = JSON.stringify(tmpmodeltsl);
          this.saveLoading = true;
          editProduct({
            id: this.productInfos.Id,
            ModelTSL: this.productInfos.ModelTSL
          }).then(rsp => {
            if (rsp.code == 0) {
              this.$modal.msgSuccess("保存成功");
              this.attrDrawer = false;
              this.saveLoading = false;
              this.getProductInfo();
            }
          }).catch(err => {
            this.saveLoading = false;
          });
        },
      });
    },
    confirmCopy(copyObj) {
      let tmpmodeltsl = JSON.parse(this.productInfos.ModelTSL);
      if (this.activeDefinition == "attribute") {
        tmpmodeltsl.properties.push(copyObj);
      }
      if (this.activeDefinition == "expands") {
        tmpmodeltsl.tags.push(copyObj);
      }
      if (this.activeDefinition == "function") {
        tmpmodeltsl.functions.push(copyObj);
      }
      if (this.activeDefinition == "event") {
        tmpmodeltsl.events.push(copyObj);
      }
      this.productInfos.ModelTSL = JSON.stringify(tmpmodeltsl);
      this.saveLoading = true;
      editProduct({
        id: this.productInfos.Id,
        ModelTSL: this.productInfos.ModelTSL
      }).then(rsp => {
        if (rsp.code == 0) {
          this.$modal.msgSuccess("保存成功");
          this.attrDrawer = false;
          this.saveLoading = false;
          this.getProductInfo();
        }
      }).catch(err => {
        this.saveLoading = false;
      });
    },
    onDownWayChange(val) {
      this.funcFrom.downdata = "";
      this.funcFrom.downway = val;
      if (val == 4) {
        this.$nextTick(() => {
          setTimeout(() => {
            this.$refs.graphDlg.open();
          }, 200)
        })
      }
    },
    onEditGraph() {
      this.$refs.graphDlg.open();
    },
    setCurType(curType) {

      if (this.curType != curType) {
        if (this.activeDefinition == 'expands') {
          this.expandsForm.option.type = curType;
          if (curType == "geo") {
            if (this.expandsForm.value == 0) {
              this.expandsForm.value = { "lng": 0, "lat": 0 };
            }
          }
          else if (curType == "boolean") {
            this.expandsForm.value = "false";
          }
          else if (curType == "date") {
            this.expandsForm.value = new Date().getTime();
          }
          else if (curType == "int" || curType == "float") {
            this.expandsForm.value = 0;
          }
          else {
            this.expandsForm.value = "";
          }
        }
        else if (this.activeDefinition == 'attribute') {
          this.attrFrom.option.type = curType;
        }
      }
      this.curType = curType;

    },
    setTypeMap() {
      //设置数据类型的map
      this.typeList.forEach(item => {
        this.typeListMap.set(item.value, item);
      });

      // console.log("设置的值", this.typeListMap);
    },
    saveInfo(classId) {
      //协议详情编辑组件
      this.classId = classId;
      this.getProductClassInfo();
      this.getProductInfo();
    },
    reloadProduct() {
      this.getProductInfo();
    },
    handleSetType(type) {
      //根据选择的数据类型添加数据
      this.openAttrDrawer();
      this.$nextTick(() => {
        if (this.$refs["typeFormAssembly"]) {
          if (type == 'state') {
            this.expandsForm = {
              code: 'state',
              enable: true,
              mapcode: '',
              name: '运行状态',
              option: {
                elements: {
                  '正常': '正常',
                  '维修': '维修',
                  '保养': '保养'
                },
                type: "enum"
              },
              value: '正常'
            }
            this.$refs["typeFormAssembly"].setNOtInputsType(this.expandsForm); //直接赋值已有值
          } else if (type == 'signal') {
            this.expandsForm = {
              code: 'signal',
              enable: true,
              mapcode: '',
              name: '信号强度',
              option: {
                max: 9999,
                min: 0,
                unit: 'dBm',
                decimals: 2,
                type: "float"
              },
              value: 0
            }
            this.$refs["typeFormAssembly"].setNOtInputsType(this.expandsForm); //直接赋值已有值
          }
          else {
            this.$refs["typeFormAssembly"].setTypeVal(type);
            this.curType = type;
            this.expandsForm.option.type = this.curType;
            if (this.curType == "geo") {
              if (this.expandsForm.value == 0) {
                this.expandsForm.value = { "lng": 0, "lat": 0 };
              }
            }
            else if (this.curType == "boolean") {
              this.expandsForm.value = "false";
            }
            else if (this.curType == "date") {
              this.expandsForm.value = new Date().getTime();
            }
            else if (this.curType == "int" || this.curType == "float") {
              this.expandsForm.value = 0;
            }
            else {
              this.expandsForm.value = "";
            }
          }

        }
      });
    },

    getProductClassInfo() {
      //获取协议分类信息
      classInfo({ id: this.classId }).then(rs => {
        this.className = rs.data.Name;
      });
    },

    openEditProductDrawer() {
      //打开协议修改编辑弹出层
      // this.editProductDrawer = true;

      this.$refs["editProduct"].openEditProductDrawer();
    },

    getChannelInfo() {
      channelInfo({ code: this.productInfos.NetworkWay }).then(res => {
        if (res.code == 0) {
          this.NetworkWayName = res.data.Name;
          this.ChannelData = res.data;
        }
      });
    },
    onSaveNotice(arr) {
      this.productInfos.NoticeWay = JSON.stringify(arr);
      editProduct({
        id: this.productInfos.Id,
        NoticeWay: this.productInfos.NoticeWay
      });
    },
    saveCode(code) {
      //保存脚本
      this.productInfos.InterScripts = code;
      editProduct({
        id: this.productInfos.Id,
        interScripts: this.productInfos.InterScripts
      }).then(rsp => {
        // console.log("数据更新后返回", rsp);
        if (rsp.code == 0) {
          this.$modal.msgSuccess("保存成功");
          this.getProductInfo();
          // console.log("删除后表格数据", this.tableData);
        }
      });
    },

    editParams(statusType, row, rowIndex) {
      //编辑添加的参数
      this.openParamsDrawer(statusType);
      this.paramsForm = row;
      if (statusType == "inputs") {
        this.paramsForm.disabledDef = this.paramsForm.defval == null;
        this.$nextTick(() => {
          if (this.$refs["typeFormAssembly"]) {
            this.$refs["typeFormAssembly"].setInputsType(this.paramsForm);
          }
        });
      }
      this.isEditCode = true;
      this.activeParamsLine = rowIndex;
    },
    deleteParams(statusType, row, rowIndex) {
      //删除参数
      this.$modal
        .confirm('是否确认删除字段名称为"' + row.name + '"的数据项？')
        .then(rs => {
          if (rs == "confirm") {
            if (statusType == "inputs") {
              this.inputsList.splice(rowIndex, 1);
              this.inputsCodeList.splice(rowIndex, 1);
            } else if (statusType == "outputs") {
              this.outputsList.splice(rowIndex, 1);
              this.outputsCodeList.splice(rowIndex, 1);
            }
          }
        });
    },
    copyRowData(row, indexRow) {
      this.$refs.cpyRef.openDlg(row);
    },
    editRowData(row, indexRow) {
      //修改一行的数据
      // console.log("点击的那一行的数据", row, indexRow);
      this.openAttrDrawer();
      this.activeModelLine = indexRow;
      this.isEdit = true;
      this.isEditCode = true;
      if (this.activeDefinition == "attribute") {
        this.attrFrom = JSON.parse(JSON.stringify(row));
        if (this.attrFrom.showway == null) {
          this.attrFrom.showway = "org,own,use,person";
          this.showpparr = ["org", "own", "use", "person"];
        }
        else {
          this.showpparr = this.attrFrom.showway.split(',');
        }
        this.curType = this.attrFrom.option.type;
        if (this.attrFrom.prefixcode == null) {
          this.$set(this.attrFrom, "prefixcode", "");
        }
        this.$nextTick(() => {
          if (this.$refs["typeFormAssembly"]) {
            this.$refs["typeFormAssembly"].setNOtInputsType(this.attrFrom); //执行子组件typeForm的setNOtInputsType方法
          }
        });
      }
      else if (this.activeDefinition == "expands") {
        this.expandsForm = JSON.parse(JSON.stringify(row));
        if (this.expandsForm.option.type == "geo") {
          this.$set(this.expandsForm, "value", { "lng": 0, "lat": 0 });
        }
        this.curType = this.expandsForm.option.type;
        this.$nextTick(() => {
          if (this.$refs["typeFormAssembly"]) {
            this.$refs["typeFormAssembly"].setNOtInputsType(this.expandsForm); //执行子组件typeForm的setNOtInputsType方法
          }
        });
      }
      else if (this.activeDefinition == "function") {
        this.funcFrom = JSON.parse(JSON.stringify(row));
        if (this.funcFrom.showway == null) {
          this.funcFrom.showway = "";
          this.showarr.length = 0;
        }
        else {
          this.showarr = this.funcFrom.showway.split(',');
        }
        if (this.funcFrom.prefixcode == null) {
          this.$set(this.funcFrom, "prefixcode", "");
        }

        if (this.funcFrom.actionway == null) {
          this.$set(this.funcFrom, "actionway", 0);
        }
        this.inputsList = this.funcFrom.inputs;
        this.inputsCodeList = Array.from(this.funcFrom.inputs, ({ code }) => code);
      }
      else if (this.activeDefinition == "event") {
        this.eventFrom = JSON.parse(JSON.stringify(row));
        this.outputsList = this.eventFrom.outputs;
        this.outputsCodeList = Array.from(this.eventFrom.outputs, ({ code }) => code);
        if (this.eventFrom.Targets == null) {
          this.$set(this.eventFrom, "Targets", []);
        }
        if (this.eventFrom.CondType == null) {
          this.$set(this.eventFrom, "CondType", 0);
        }
      }
    },
    deleteRowData(row, indexRow, type) {
      //删除表格中的一行的数据
      let modelTSL = JSON.parse(this.productInfos.ModelTSL);
      if (type) {
        this.$modal.confirm("是否确认删除该行数据？").then(rs => {
          if (rs == "confirm") {
            this.productInfos.ModelTSL = JSON.stringify(modelTSL);
            editProduct({
              id: this.productInfos.Id,
              ModelTSL: this.productInfos.ModelTSL
            }).then(rsp => {
              // console.log("数据更新后返回", rsp);
              if (rsp.code == 0) {
                this.$modal.msgSuccess("删除成功");
                this.attrDrawer = false;
                this.getProductInfo();
                // console.log("删除后表格数据", this.tableData);
              }
            });
          }
        });
      } else {
        this.$modal
          .confirm('是否确认删除字段名称为"' + row.name + '"的数据项？')
          .then(rs => {
            if (rs == "confirm") {
              if (this.activeDefinition == "attribute") {
                modelTSL.properties.splice(indexRow, 1);
              }
              if (this.activeDefinition == "expands") {
                modelTSL.tags.splice(indexRow, 1);
              }
              if (this.activeDefinition == "function") {
                modelTSL.functions.splice(indexRow, 1);
              }
              if (this.activeDefinition == "event") {
                modelTSL.events.splice(indexRow, 1);
              }
              this.productInfos.ModelTSL = JSON.stringify(modelTSL);
              editProduct({
                id: this.productInfos.Id,
                ModelTSL: this.productInfos.ModelTSL
              }).then(rsp => {
                // console.log("数据更新后返回", rsp);
                if (rsp.code == 0) {
                  this.$modal.msgSuccess("删除成功");
                  this.attrDrawer = false;
                  this.getProductInfo();
                  // console.log("删除后表格数据", this.tableData);
                }
              });
            }
          });
      }
    },
    closeParamDrawer() {
      //关闭参数填写弹窗
      this.attrDrawerSize = "600px";
      this.funcDrawer = false;
    },
    onIptChg() {
      if (this.activeParams == "outputs") return;
      this.$refs.defParamVal.reset();
    },
    onParamEnabel(val) {
      this.$nextTick(() => {
        this.$forceUpdate();
      });
    },
    joinParams() {
      //添加输入输出参数
      this.$refs["paramsForm"].validate(val1 => {
        if (val1) {
          this.paramsLoading = true;
          if (this.activeParams == 'inputs') {
            this.paramsForm.defval = this.$refs.defParamVal.getVal();
            delete this.paramsForm.option;
            delete this.paramsForm.disabledDef;
            
            if (this.activeParamsLine > -1) {
              this.inputsList[this.activeParamsLine] = this.paramsForm;
            } else {
              this.inputsList.push(this.paramsForm);
              this.inputsCodeList.push(this.paramsForm.code);
            }
          }
          else{
           if (this.activeParamsLine > -1) {
              this.outputsList[this.activeParamsLine] = this.paramsForm;
            } else {
              this.outputsList.push(this.paramsForm);
              this.outputsCodeList.push(this.paramsForm.code);
            }
          }

          this.funcDrawer = false; //关闭弹窗
          this.paramsLoading = false;
        }


      });
    },
    openParamsDrawer(statusType) {
      //打开填写参数的弹出层
      this.funcDrawer = true;
      this.attrDrawerSize = "700px";
      this.activeParams = statusType;
      this.paramsForm = {
        name: "", //名称
        code: "", //标识符
        option: {}
      };
      if (statusType != "inputs") {
        this.activeCodeList = JSON.parse(JSON.stringify(this.outputsCodeList));
      } else {
        this.paramsForm["disabledDef"] = true;
        this.activeCodeList = JSON.parse(JSON.stringify(this.inputsCodeList));
      }
      this.activeParamsLine = -1;
      this.isEditCode = false;
      this.enumKeyList = [];
      if (this.$refs["typeFormAssembly"]) {
        this.$refs["typeFormAssembly"].reloadTypeForm();
      }

      this.resetForm("paramsForm");
    },
    changeMapcode() {
      this.expandsForm.mapcode = null;
    },
    openAttrDrawer() {
      //打开属性定义弹出层
      this.attrDrawer = true;
      this.isEditCode = false;
      this.showarr = ['org', 'own', 'use', 'person'];
      this.showpparr = ["org", "own", "use", "person"];
      this.attrFrom = {
        //属性定义
        name: "", //名称
        code: "", //标识符
        prefixcode: "",//标识符前缀
        description: "", //描述
        option: {}
      };
      this.expandsForm = {
        //标签
        name: "", //名称
        code: "", //标识符
        value: 0, //默认值
        mapcode: "", //属性定义绑定
        enable: true, //是否启用
        option: {}
      };
      this.funcFrom = {
        //功能定义
        name: "", //名称
        code: "", //标识符
        prefixcode: "",//标识符前缀
        description: "", //描述
        showway: "org,own,use,person",
        downway: 0,
        actionway: 0,
        downdata: "",
        inputs: [], //输入参数
        outputs: [] //输出参数
      };
      this.eventFrom = {
        //功能定义
        name: "", //名称
        code: "", //标识符
        Level: null,
        CondType: 0,
        Targets: ["org"],
        SilenceTime: 60,
        description: "", //描述
      };
      this.$nextTick(() => {
        if (this.$refs["typeFormAssembly"]) {
          this.$refs["typeFormAssembly"].reloadTypeForm();
        }
      });
      this.inputsList = this.funcFrom.inputs;
      this.outputsList = [];
      this.enumKeyList = [];
      this.activeModelLine = -1; //当前修改的行是
      this.activeParamsLine = -1; //当前修改的参数是哪一行
      this.resetForm("attrFrom");
      this.resetForm("paramsForm");
    },
    saveSetData(params, editInfo) {
      //修改和添加数据对数据进行处理
      let proModelTSL = JSON.parse(this.productInfos.ModelTSL)
      let modelTSL = JSON.parse(JSON.stringify(proModelTSL));
      if (params == "modbus") {
        if (!modelTSL.modbus) {
          modelTSL.modbus = {};
        }
        if (editInfo) {
          modelTSL.modbus = JSON.parse(JSON.stringify(editInfo));
        }
      }
      else {
        if (this.activeModelLine > -1) {
          if (this.activeDefinition == "attribute") {
            let option = this.$refs["typeFormAssembly"].setOptionsData();
            this.attrFrom.option = JSON.parse(JSON.stringify(option));
            modelTSL.properties[this.activeModelLine] = JSON.parse(
              JSON.stringify(this.attrFrom)
            );
          }
          if (this.activeDefinition == "expands") {
            let option = this.$refs["typeFormAssembly"].setOptionsData();
            this.expandsForm.option = JSON.parse(JSON.stringify(option));
            modelTSL.tags[this.activeModelLine] = JSON.parse(
              JSON.stringify(this.expandsForm)
            );
          }
          if (this.activeDefinition == "function") {
            this.funcFrom.inputs = this.inputsList;
            modelTSL.functions[this.activeModelLine] = JSON.parse(
              JSON.stringify(this.funcFrom)
            );
          }
          if (this.activeDefinition == "event") {
            this.eventFrom.outputs = this.outputsList;
            modelTSL.events[this.activeModelLine] = JSON.parse(
              JSON.stringify(this.eventFrom)
            );
          }

        } else {
          if (this.activeDefinition == "attribute") {
            let option = this.$refs["typeFormAssembly"].setOptionsData();
            if (!modelTSL.properties) {
              modelTSL.properties = [];
            }
            this.attrFrom.option = option;
            modelTSL.properties.push(JSON.parse(JSON.stringify(this.attrFrom)));
          }
          if (this.activeDefinition == "expands") {
            let option = this.$refs["typeFormAssembly"].setOptionsData();
            if (!modelTSL.tags) {
              modelTSL.tags = [];
            }
            this.expandsForm.option = JSON.parse(JSON.stringify(option));
            modelTSL.tags.push(JSON.parse(JSON.stringify(this.expandsForm)));
          }
          if (this.activeDefinition == "function") {
            if (!modelTSL.functions) {
              modelTSL.functions = [];
            }
            this.funcFrom.inputs = JSON.parse(JSON.stringify(this.inputsList));
            modelTSL.functions.push(JSON.parse(JSON.stringify(this.funcFrom)));
          }
          if (this.activeDefinition == "event") {
            this.eventFrom.outputs = JSON.parse(JSON.stringify(this.outputsList));
            if (!modelTSL.events) {
              modelTSL.events = [];
            }
            modelTSL.events.push(JSON.parse(JSON.stringify(this.eventFrom)));
          }
        }
      }

      let proModelTSLStr = JSON.stringify(modelTSL);
      this.saveLoading = true;
      editProduct({
        id: this.productInfos.Id,
        ModelTSL: proModelTSLStr
      }).then(rsp => {
        // console.log("数据更新后返回", rsp);
        if (rsp.code == 0) {
          this.$modal.msgSuccess("操作成功");
          this.attrDrawer = false;
          this.saveLoading = false;
          this.getProductInfo();
          // console.log("新增修改后表格数据", this.tableData);
        }
      }).catch(err => {
        this.saveLoading = false;
      });
    },
    saveProductInfo() {
      //保存协议信息
      if (this.funcFrom.downway == 1) {
        this.$refs.funRef.setInputData();
      }

      this.$refs["attrFrom"].validate(valid => {
        if (valid) {
          if (
            this.$refs["typeFormAssembly"] &&
            this.$refs["typeFormAssembly"].$refs["typeForm"]
          ) {
            this.$refs["typeFormAssembly"].$refs["typeForm"].validate(valid2 => {
              if (valid2) {
                if (this.$refs["paramsForm"]) {
                  //效验每个参数的name和code
                  this.$refs["paramsForm"].validate(valid3 => {
                    if (valid3) {
                      this.saveSetData();
                    } else {
                      return false;
                    }
                  });
                } else {
                  this.saveSetData();
                }
              } else {
                return false;
              }
            });
          } else {
            this.saveSetData();
            // console.log("submit3!");
          }
        } else {
          console.log("error submit1!!");
          return false;
        }
      });
    },

    getProductInfo() {
      this.configLoading = true;

      productInfo({ id: this.productId }).then(rsp => {
        if (rsp.code == 0) {
          this.productInfos = rsp.data;
          this.enableStore = false;
          if (this.productInfos.StorageConfig) {
            let tmpstorageConfig = JSON.parse(this.productInfos.StorageConfig);
            if (tmpstorageConfig.enable == '1') {
              this.enableStore = true;
            }
          }

          let jsonLis = JSON.parse(this.productInfos.ModelTSL);

          if (jsonLis.properties) {
            this.attrTableData = jsonLis.properties;
            this.attrCodeList = Array.from(
              jsonLis.properties,
              ({ code }) => code
            );
          }
          if (jsonLis.functions) {
            this.funcTableData = jsonLis.functions;
            this.funcCodeList = Array.from(
              jsonLis.functions,
              ({ code }) => code
            );
          }
          if (jsonLis.events) {
            this.eventTableData = jsonLis.events;
            this.eventCodeList = Array.from(jsonLis.events, ({ code }) => code);
          }
          if (jsonLis.tags) {
            this.expandsTableData = jsonLis.tags;
            this.expandsCodeList = Array.from(jsonLis.tags, ({ code }) => code);
            if (this.expandsCodeList.includes('state')) {
              this.typeList = this.typeList.filter(row => row.value != 'state')
            }
            else {
              this.typeList = this.typeList.filter(row => row.value != 'state')
              this.typeList = [...[{ alabel: "状态", label: "运行状态", value: "state" }], ...this.typeList]
            }

            if (this.expandsCodeList.includes('signal')) {
              this.typeList = this.typeList.filter(row => row.value != 'signal')
            }
            else {
              this.typeList = this.typeList.filter(row => row.value != 'signal')
              this.typeList = [...[{ alabel: "信号", label: "信号强度", value: "signal" }], ...this.typeList]
            }
            // console.log(this.typeList,'this.typeListthis.typeList');
          }
          if (jsonLis.modbus) {
            this.modbusData = jsonLis.modbus;
          }

          if (this.activeSelect == "physicalModel") {
            if (this.activeDefinition == "attribute") {
              this.tableData = JSON.parse(JSON.stringify(this.attrTableData));
            }
            if (this.activeDefinition == "function") {
              this.tableData = JSON.parse(JSON.stringify(this.funcTableData));
            }
            if (this.activeDefinition == "event") {
              this.tableData = JSON.parse(JSON.stringify(this.eventTableData));
            }
            if (this.activeDefinition == "expands") {
              this.tableData = JSON.parse(
                JSON.stringify(this.expandsTableData)
              );
              this.tableColumnsList = JSON.parse(
                JSON.stringify(this.expandsTableColumnsList)
              );
            } else {
              if (this.activeDefinition == "attribute") {
                this.tableColumnsList = [
                  { filed: "name", filedName: "名称" },
                  { filed: "code", filedName: "标识符" },
                  { filed: "type", filedName: "类型" },
                  { filed: "unit", filedName: "单位" },
                  { filed: "description", filedName: "说明" }
                ];
              } else {
                this.tableColumnsList = [
                  { filed: "name", filedName: "名称" },
                  { filed: "code", filedName: "标识符" },
                  { filed: "description", filedName: "说明" }
                ];
              }

            }
            // console.log("表格数据",this.tableData);
          }
          if (this.isFirstGet) {
            this.getChannelInfo();
          }

          this.configLoading = false;
          this.isFirstGet = false;
        }
      });
    },

    to(path) {
      if (!this.configLoading) {
        this.activeSelect = path;
      }

    },
    definitonSelect(path) {
      this.activeDefinition = path;

      if (path == "attribute") {
        this.tableData = JSON.parse(JSON.stringify(this.attrTableData));
      }
      if (path == "function") {
        this.tableData = JSON.parse(JSON.stringify(this.funcTableData));
      }
      if (path == "event") {
        this.tableData = JSON.parse(JSON.stringify(this.eventTableData));
      }
      if (this.activeDefinition == "expands") {
        this.tableData = JSON.parse(JSON.stringify(this.expandsTableData));
        this.tableColumnsList = JSON.parse(
          JSON.stringify(this.expandsTableColumnsList)
        );
      } else {
        if (this.activeDefinition == "attribute") {
          this.tableColumnsList = [
            { filed: "name", filedName: "名称" },
            { filed: "code", filedName: "标识符" },
            { filed: "type", filedName: "类型" },
            { filed: "unit", filedName: "单位" },
            { filed: "description", filedName: "说明" }
          ];
        } else {
          this.tableColumnsList = [
            { filed: "name", filedName: "名称" },
            { filed: "code", filedName: "标识符" },
            { filed: "description", filedName: "说明" }
          ];
        }
      }
      // console.log("表格数据",this.tableData);
    },
    handleSelect(path) {
      // console.log(path);
      this.activeSelect = path;
      if (path == "physicalModel") {
        if (this.activeDefinition == "attribute") {
          this.tableData = this.attrTableData;
        }
        if (this.activeDefinition == "function") {
          this.tableData = this.funcTableData;
        }
        if (this.activeDefinition == "event") {
          this.tableData = this.eventTableData;
        }
        if (this.activeDefinition == "expands") {
          this.tableData = JSON.parse(JSON.stringify(this.expandsTableData));
          this.tableColumnsList = JSON.parse(
            JSON.stringify(this.expandsTableColumnsList)
          );
        } else {
          if (this.activeDefinition == "attribute") {
            this.tableColumnsList = [
              { filed: "name", filedName: "名称" },
              { filed: "code", filedName: "标识符" },
              { filed: "type", filedName: "类型" },
              { filed: "unit", filedName: "单位" },
              { filed: "description", filedName: "说明" }
            ];
          } else {
            this.tableColumnsList = [
              { filed: "name", filedName: "名称" },
              { filed: "code", filedName: "标识符" },
              { filed: "description", filedName: "说明" }
            ];
          }
        }
      }
      if (path == "deviceManagement") {
        this.activeDevice = "deviceManage";
      }
      if (path == "warning") {
        this.$nextTick(() => {
          this.setTableCon()
        });
      }
    },
    onAddCondition() {
      if (this.funcFrom.conditions == null) {
        this.$set(this.funcFrom, "conditions", []);
        this.$set(this.funcFrom, "GroupTxt", "");
      }
      this.funcFrom.conditions.push({ code: "", valtype: "String", compare: "=", val: "" });
    },

    onDelCondition(idx) {
      this.$delete(this.funcFrom.conditions, idx);
    },
    enumVisibleChange(visible, item) {
      if (visible) {
        let newttt;
        let newlist = this.attrTableData.filter(x => x.code == item.code);
        if (newlist.length > 0) {
          newttt = newlist[0].option;
        }
        else {
          return;
        }
        this.enumArr = [];
        for (let key in newttt.elements) {
          this.enumArr.push({ "key": newttt.elements[key], "value": newttt.elements[key] });
        }
      }
    },
    condiChange(code, item) {
      let newttt;
      let newlist = this.attrTableData.filter(x => x.code == code);
      if (newlist.length > 0) {
        newttt = newlist[0].option;
      }
      else {
        return;
      }

      if (newttt.type == "string") {
        item.valtype = "String";
        item.compare = "=";
        item.val = "";
      }
      else if (newttt.type == "boolean") {
        item.valtype = "Bool";
        item.compare = "=";
        item.val = "";
      }
      else if (newttt.type == "enum") {
        item.val = "";
        item.compare = "=";
        item.valtype = "Enum";
      }
      else if (newttt.type == "date") {
        item.val = new Date().getTime();
        item.compare = ">";
        item.valtype = "Date";
      }
      else if (newttt.type == "int") {
        item.val = 0;
        item.valtype = "Long";
      }
      else {
        item.val = 0;
        item.valtype = "Double";
      }
    },

    onAddEvtCondition() {
      if (this.eventFrom.PropConditions == null) {
        this.$set(this.eventFrom, "PropConditions", []);
        this.$set(this.eventFrom, "GroupTxt", "");
      }
      if (this.eventFrom.PropConditions.length >= 26) {
        this.$modal.msgError("超过条件数量");
        return;
      }
      this.eventFrom.PropConditions.push({ code: "", valtype: "String", compare: "=", val: "" });
    },
    onDelEvtCondition(idx) {
      this.$delete(this.eventFrom.PropConditions, idx);
    },
    getCondName(idx) {
      var tarrnames = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
      return tarrnames[idx];
    }
  }
};
</script>
<style lang="less" scope>
.script_li {
  background: #f5f7fa;
  padding: 10px;
  border-radius: 10px;
  position: relative;
  cursor: pointer;
  height: 390px;

  .tags_info {
    position: absolute;
    right: 0;
    top: 0;
    font-size: 12px;
    z-index: 10;
    color: #ffffff;
    width: 30px;
    // text-align: right;
    word-break: break-all;
  }

  .tags_info::before {
    content: "";
    position: absolute;
    top: 0;
    right: 0;
    border-width: 0 40px 40px 0;
    border-style: solid;
    border-color: transparent #409eff transparent transparent;
    z-index: 2;
    border-radius: 2px;
  }

  .tags_info::after {
    content: '\2713';
    position: absolute;
    right: 0;
    top: 0;
    font-size: 16px;
    z-index: 3;
    color: #ffffff;
    width: 20px;
    line-height: 30px;
    text-align: center;
    transform: rotate(0deg);
    transform-origin: center center;
    color: #ffffff;
  }

  .script_label {
    color: #666666;
    font-size: 16px;
    margin: 0 0 10px 0;
  }

  .remark_cot {
    color: #666666;
    font-size: 12px;
    margin-top: 10px;
    width: calc(100% - 20px);
    display: block;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .del_con {
    position: absolute;
    right: 10px;
    bottom: 15px;
    color: #999999;
    width: 16px;
    height: 16px;
  }
}
</style>
<style lang="less">
.script_li {
  .el-textarea.is-disabled .el-textarea__inner {
    background: #ffffff;
  }
}

.data_table {
  .item-sort {
    box-shadow: 5px 5px 5px -2px rgba(0, 0, 0, .3);
  }

  .hidden-row {
    display: none;
  }
}

.attrFrom_con {
  padding: 20px;
}

.param_list {
  color: #272e3b;
  width: 100%;
  box-sizing: border-box;
  padding: 12px;
  height: 36px;
  background-color: #fafafa;
  display: flex;
  justify-content: space-between;
  align-items: center;

  .list_left {
    .params_type {
      padding: 0 6px;
      border: 1px solid #595959;
      border-radius: 3px;
      margin-left: 10px;
    }
  }

  .list_right {
    i {
      padding: 0 5px;
      cursor: pointer;
    }
  }
}

.params_form {
  padding: 20px;
}

.header {
  //   min-width: 980px;
  background-color: #ffffff;
  border-top: 1px solid #dadada;
  width: 100%;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  // justify-content: space-between;
  //   line-height: 70px;
  //   height: 70px;
  position: relative;
  padding: 10px 0;

  .el-menu {
    top: 0;
    // z-index: 999;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    width: 100%;
  }

  .el-menu.el-menu--horizontal {
    border-bottom: none;
  }

  .shejiqi {
    height: 38px;
    line-height: 38px;
    border: none;
  }

  .shejiqi .el-menu-item {
    padding: 0;
    margin: 0 25px;
    height: 38px;
    line-height: 38px;
    font-size: 16px;
  }

  .name_text {
    display: flex;
    justify-content: center;
    align-items: center;
    padding-right: 30px;
    white-space: nowrap;
    font-size: 20px;
    color: rgba(76, 121, 255, 1);
    font-weight: bold;
  }

}


.dlg-param-bg {
  .paramrow {
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-bottom: 10px;
  }
}

.dlg-gg-row {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: center;
}
</style>