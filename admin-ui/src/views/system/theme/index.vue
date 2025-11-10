<template>
  <div style="padding: 20px 20px 0 20px" id="big_con">
    <div>
      <div class="from_con" id="from_con" v-show="showSearch">
        <el-form class="biaodan" :model="queryParams" ref="queryForm" :inline="true">
          <el-form-item label="主题名称" prop="roleName">
            <el-input class="set_radius" v-model="queryParams.roleName" placeholder="请输入主题名称" clearable @keyup.enter.native="handleQuery"/>
          </el-form-item>
          <el-form-item label="应用所属">
            <el-select
              v-model="queryParams.OrgId"
              filterable
              remote
              reserve-keyword
              @change="chgOrgId"
              :clearable="true"
              placeholder="请输入要搜索的企业名称"
              :remote-method="searchToolOrg"
              :loading="xxloading"
            >
              <el-option v-for="item in SearchUserOrgList" :key="item.Id" :label="item.OrgName" :value="item.Id">
              </el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="创建日期">
            <el-date-picker
              class="set_radius"
              v-model="dateRange"
              style="width: 250px"
              value-format="yyyy-MM-dd"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
            ></el-date-picker>
          </el-form-item>

          <!-- <el-col class="float_right" :span="24"> -->
          <el-form-item class="submit_button_con">
            <el-button icon="el-icon-refresh" @click="resetQuery">重置</el-button>
            <el-button type="primary" icon="el-icon-search" @click="handleQuery">搜索</el-button>
          </el-form-item>
          <!-- </el-col> -->
        </el-form>
      </div>
      <div
        class="elbiaoge_elform"
        :style="{ 'min-height': tableConHeight + 'px' }"
      >
        <el-row :gutter="10" class="mb8 button_row">
          <div>
            <el-col :span="1.5">
              <el-button type="primary" plain @click="handleAdd" v-hasPermi="['/AuthService/Role/Add']">
                <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                <span style="margin-left: 6px">新增</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" plain :disabled="single" @click="handleUpdate" v-hasPermi="['/AuthService/Role/Edit']">
                <i class="zhongtaiiconfont zhongtai-icon-xiugai"></i>
                <span style="margin-left: 6px">修改</span>
              </el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain :disabled="single" @click="handleDelete" v-hasPermi="['/AuthService/Role/Remove']">
                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                <span style="margin-left: 6px">删除</span>
              </el-button>
            </el-col>
          </div>
          <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" border :data="themeList" :row-style="isRed" class="data_table"
          @selection-change="handleSelectionChange" :header-cell-style="cellSty" style="width: 100%">
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column label="主题名称" prop="Name" :show-overflow-tooltip="true" align="center"/>
          <el-table-column label="是否公开" prop="IsPublic" align="center" width="100">
            <template slot-scope="scope">
              <el-switch
                v-model="scope.row.IsPublic"
                :active-value="true"
                :inactive-value="false"
                :disabled="true"
              ></el-switch>
            </template>
          </el-table-column>
          <el-table-column label="主题展示图" align="center" width="150">
            <template slot-scope="scope">
                <div style="width: 100%;display:flex;justify-content:center;align-items:center">
                    <div class="imgwrap" style="max-width: 60px;max-height:60px;">
                        <el-image fit="cover" :src="scope.row.PhotoUrl + '?wh=500x500'"
                            :preview-src-list="[scope.row.PhotoUrl]"></el-image>
                    </div>
                </div>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center" prop="createTime">
            <template slot-scope="scope">
              <span>{{ parseTime(scope.row.createTime) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="248">
            <template slot-scope="scope">
              <el-button type="text" icon="el-icon-edit" @click="handleUpdate(scope.row)">修改</el-button>
              <el-button type="text" icon="el-icon-delete" @click="handleDelete(scope.row)" v-show="scope.row.IsSystem != 1">删除</el-button>
              <el-dropdown @command="(command) => handleCommand(command, scope.row)">
                <el-button type="text" class="el-dropdown-link">
                  <i class="el-icon-d-arrow-right el-icon--right"></i>更多
                </el-button>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="handleAuthOrg" icon="el-icon-user">分配企业</el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>

        <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList"/>
      </div>
      <!-- 添加或修改主题配置对话框 -->
      <el-dialog :title="title" :visible.sync="open" :close-on-click-modal="false" width="940px" top="6vh" append-to-body>
        <el-form class="dialog_form" ref="form" :model="form" :rules="rules" label-width="170px" label-position="top">
          <el-row :gutter="10">
            <el-col :span="24">
              <el-form-item label="主题名称" prop="name">
                <el-input v-model="form.name" placeholder="请输入主题名称" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="PC端主题颜色" prop="pcThemeColor">
                <div class="item_flex_con color_flex">
                  <div class="color_con"><el-color-picker v-model="form.pcThemeColor" show-alpha></el-color-picker></div>
                  <div class="tips_text">请选择PC端主题颜色</div>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="主题展示图" prop="photoUrl">
                
                <div class="avatar_con">
                  <image-upload v-model="form.photoUrl" :limit="1" :isFlexStart="true"></image-upload>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="移动端LOGO" prop="mobileLogo">
                
                <div class="avatar_con">
                  <image-upload v-model="form.mobileLogo" :limit="1" :isFlexStart="true"></image-upload>
                </div>
              </el-form-item>
            </el-col>
            
            <el-col :span="7">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">是否开启标签导航</div>
                  <el-switch v-model="form.isTagsViews" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="7">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">是否开启动态标题</div>
                  <el-switch v-model="form.isActiveTiltle" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">顶级菜单位置</div>
                  <!-- <el-switch v-model="form.isTopNav" :active-value="true" :inactive-value="false"></el-switch> -->
                  <el-radio-group v-model="form.isTopNav">
                    <el-radio :label="false">左边</el-radio>
                    <el-radio :label="true">顶部</el-radio>
                  </el-radio-group>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="7">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">是否显示Logo</div>
                  <el-switch v-model="form.isShowLogo" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="7">
              <el-form-item label="" prop="isOblong">
                <div class="item_flex_con">
                  <div class="label_text">logo是否长方形比例</div>
                  <el-switch v-model="form.isOblong" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isOblong">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">主题类型</div>
                  <el-radio-group v-model="form.themeType">
                    <el-radio label="theme-dark">暗系</el-radio>
                    <el-radio label="theme-light">亮系</el-radio>
                  </el-radio-group>
                </div>
                
              </el-form-item>
            </el-col>
            <el-col :span="7">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                  <div class="label_text">是否公开</div>
                  <el-switch v-model="form.isPublic" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isPublic">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="7">
              <el-form-item label="" prop="isNotAutoCreate">
                <div class="item_flex_con">
                  <div class="label_text">是否不存在自动创建账号</div>
                  <el-switch v-model="form.isNotAutoCreate" :active-value="true" :inactive-value="false"></el-switch>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="1">
              <el-form-item label="" prop="isNotAutoCreate">
                <div class="item_flex_con">
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="" prop="mobileNavType">
                <div class="item_flex_con">
                  <div class="label_text">移动端导航类型</div>
                  <el-radio-group v-model="form.mobileNavType">
                    <el-radio label="default">默认</el-radio>
                    <el-radio label="workOrder">工单</el-radio>
                  </el-radio-group>
                </div>
              </el-form-item>
            </el-col>
            
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="">
                <!-- <span  slot="label"> -->
                  <div class="item_flex_con" style="padding-bottom:14px">
                    <div class="label_text">条件标签</div>
                    <div class="handle_con">
                        <el-upload class="upload_json" style="display:inline" accept=".json" :multiple="false" :show-file-list="false" action="#" :before-upload="handleImport">
                          <div class="guide_text">
                            <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                            <span style="margin-left:6px">导入</span>
                          </div>
                        </el-upload>
                      <div class="guide_text" @click.stop="handleExport"><i class="zhongtaiiconfont zhongtai-icon-daochu"></i><span style="margin-left:6px">导出</span></div>
                      <div class="add_text" @click.stop="addLabelLi"><i class="el-icon-plus"></i><span>添加标签</span></div>
                    </div>
                  </div>
                <!-- </span> -->
                <el-table v-loading="loading" border :data="form.conditionLabel" :row-style="isRed" class="data_table" :header-cell-style="cellSty" style="width: 100%">
                  <el-table-column label="标签名称" prop="name" :show-overflow-tooltip="true" align="center"/>
                  <el-table-column label="标签标识符" prop="code" :show-overflow-tooltip="true" align="center"/>
                  <el-table-column label="标签类型" prop="type" :show-overflow-tooltip="true" align="center">
                    <template slot-scope="scope">
                      {{returnTypeText(scope.row.type)}}
                    </template>
                  </el-table-column>

                  <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="248">
                    <template slot-scope="scope">
                      <el-button type="text" icon="el-icon-edit" @click="editLabelLi(scope.row)">修改</el-button>
                      <el-button type="text" icon="el-icon-delete" @click="delLabelLi(scope.row.$index)" v-show="scope.row.IsSystem != 1">删除</el-button>
                    </template>
                  </el-table-column>
                </el-table>

                <!-- <el-row :gutter="20" v-for="(ite,inx) in form.conditionLabel" :key="'codlabel'+inx" :style="{'margin-bottom':inx==form.conditionLabel.length-1?'0':'10px'}">
                  <el-col :span="8">
                    <el-input v-model="form.conditionLabel[inx].name" type="text" placeholder="请输入标签名称"></el-input>
                  </el-col>
                  <el-col :span="8">
                    <el-input v-model="form.conditionLabel[inx].code" type="text" placeholder="请输入标签标识符"></el-input>
                  </el-col>
                  <el-col :span="7">
                    <el-select filterable v-model="form.conditionLabel[inx].type" placeholder="请选择数据类型" style="width: 100%">
                      <el-option v-for="item in typeList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                    </el-select>
                  </el-col>
                  <el-col :span="1">
                    <i @click="delLabelLi(inx)" class="el-icon-delete" style="font-size:16px;color:#78829D;"></i>
                  </el-col>
                </el-row> -->
                
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <div slot="footer" class="dialog-footer">
          <el-button class="cancel" @click="cancel">取 消</el-button>
          <el-button class="confirm" type="primary" @click="submitForm">确 定</el-button>
        </div>
      </el-dialog>
      <conditionLabel ref="conditionLabel" @finishLabelAdd="finishLabelAdd"></conditionLabel>
    </div>
  </div>
</template>

<script>
import {
  styleManList,
  getStyleMan,
  addStyleMan,
  updateStyleMan,
  delStyleMan,
} from "@/api/system/StyleMan";
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { searchOrg } from "@/api/system/Employee";
import conditionLabel from './conditionLabel'
export default {
  name: "themeIndex",
  dicts: ["sys_normal_disable"],
  mixins: [resizeTableCon],
  components:{conditionLabel},
  data() {
    return {
      typeList: [
        { alabel: "整型", label: "整型(Int)", value: "int" },
        { alabel: "浮点", label: "浮点型(Float)", value: "float" },
        { alabel: "字符", label: "字符型(String)", value: "string" },
        { alabel: "时间", label: "时间型(Date)", value: "date" },
        { alabel: "布尔", label: "布尔型(Boolean)", value: "boolean" },
        { alabel: "枚举", label: "枚举型(Enum)", value: "enum" },
        { alabel: "文件", label: "文件类型(File)", value: "file" },
        { alabel: "位置", label: "设备位置(Geo)", value: "geo" },
      ], //数据类型列表
      // 遮罩层
      loading: true,
      // 选中数组
      ids: [],
      // 非单个禁用
      single: true,
      // 显示搜索条件
      showSearch: true,
      // 总条数
      total: 0,
      //主题表格数据
      themeList: [],
      // 弹出层标题
      title: "",
      // 是否显示弹出层
      open: false,
      // 日期范围
      dateRange: [],
      // 查询参数
      queryParams: {
        pageNum: 1,
        pageSize: 10,
        roleName: undefined,
        status: undefined,
      },
      // 表单参数
      form: {
        name: "",
        photoUrl: "",
        mobileLogo: "",
        pcThemeColor: "",
        remark: "",
        isPublic: true,
        isTopNav:true,
        isTagsViews:true,
        isShowLogo:true,
        isActiveTiltle:false,
        themeType:'theme-dark',
        isNotAutoCreate:true,//是否不存在自动创建账号
        qywxAppId:'',//企业微信AppId
        isOblong:false,
        mobileNavType:'default',
        conditionLabel:[]
      },
      // 表单校验
      rules: {
        name: [
          { required: true, message: "主题名称不能为空", trigger: "blur" },
        ],
        mobileLogo: [
          { required: true, message: "移动端logo不能为空", trigger: "change" },
        ],
      },
      SearchUserOrgList: [{ Id: 0, OrgName: "系统" }],
      xxloading: false,
    };
  },
  created() {
    this.getList();
  },
  methods: {
    returnTypeText(typeVal){
      let find=this.typeList.find(rw=>rw.value==typeVal)
      if(find){
        return find.alabel
      }else{
        return ''
      }
      
    },
    finishLabelAdd(form){
      let findIndex=this.form.conditionLabel.findIndex(rw=>rw.code==form.code)
      if(findIndex!=undefined&&findIndex>-1){
        this.form.conditionLabel[findIndex]=JSON.parse(JSON.stringify(form))
      }else{
        if(!this.form.conditionLabel){
          this.form.conditionLabel=[]
        }
        this.form.conditionLabel.push(form)
      }
    },
    async handleExport(){
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let prolist=this.form.conditionLabel;
      let tmname=this.form.name;
      tmploading.close();
      const content =JSON.stringify(prolist)
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
    handleImport(file){
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = async (e)=> {
        const str = e.target.result
        const jsonData = JSON.parse(str)
        if(Array.isArray(jsonData)){
          for(let i=0;i<jsonData.length;i++){
            let row=jsonData[i]
            let keysArr=Object.keys(row)
            if(keysArr.includes('name')&&keysArr.includes('code')&&keysArr.includes('type')){
              this.form.conditionLabel.push(row)
            }else{
              this.$modal.msgError("导入格式不对");
              break
            }
          }
        }else{
          this.$modal.msgError("导入格式不对");
        }
        
        tmploading.close();
        this.$forceUpdate()
      }
    },

    delLabelLi(inx){//删除条件标签
      this.form.conditionLabel.splice(inx,1)
    },
    addLabelLi(){//添加条件标签
      this.$refs.conditionLabel.openLabelAddDialog()
      // let obj={
      //   name:'',
      //   code:'',
      //   type:''
      // }
      // if(!this.form.conditionLabel){
      //   this.form.conditionLabel=[]
      // }
      // this.form.conditionLabel.push(obj)
    },
    editLabelLi(item){
      this.$refs.conditionLabel.openLabelAddDialog(item)
    },
    chgOrgId(val) {
      if (val !== "") {
        this.FilterOrg = this.SearchUserOrgList.find((x) => x.Id == val);
        this.UserOrgList = [
          { Id: this.FilterOrg.Id, OrgName: this.FilterOrg.OrgName },
        ];
      } else {
        this.FilterOrg = null;
        this.UserOrgList = [{ Id: 0, OrgName: "系统" }];
      }
      this.getList();
    },
    searchToolOrg(query) {
      if (query !== "") {
        this.xxloading = true;
        searchOrg({ key: query }).then((res) => {
          this.SearchUserOrgList = res.data;
          this.SearchUserOrgList.unshift({ Id: 0, OrgName: "系统" });
          this.xxloading = false;
        });
      } else {
        this.SearchUserOrgList = [{ Id: 0, OrgName: "系统" }];
      }
    },
    /** 查询主题列表 */
    getList() {
      this.loading = true;
      styleManList(this.addDateRange(this.queryParams, this.dateRange)).then(
        (response) => {
          this.themeList = response.data.List;
          this.total = response.data.Total;
          this.loading = false;
        }
      );
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 表单重置
    reset() {
      this.resetForm("form");
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 重置按钮操作 */
    resetQuery() {
      this.dateRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.Id);
      this.single = selection.length != 1;
    },
    isRed({ row }) {
      //获取表格中选中行的背景颜色
      let checkIdList = this.ids;
      if (checkIdList.includes(row.themeId)) {
        return {
          backgroundColor: "#F6F9FF",
        };
      }
    },
    // 更多操作触发
    handleCommand(command, row) {
      switch (command) {
        case "handleAuthOrg":
          this.handleAuthOrg(row);
          break;
        default:
          break;
      }
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加主题";
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const themeId = row.Id || this.ids;
      getStyleMan(themeId).then(response => {
        // console.log("主题详情",response);
        this.form ={
            name: response.data.Name,
            photoUrl: response.data.PhotoUrl,
            mobileLogo: (JSON.parse(response.data.StyleJson)).mobileLogo,
            pcThemeColor: (JSON.parse(response.data.StyleJson)).pcThemeColor,
            isTopNav:(JSON.parse(response.data.StyleJson)).isTopNav,
            isTagsViews:(JSON.parse(response.data.StyleJson)).isTagsViews,
            isShowLogo:(JSON.parse(response.data.StyleJson)).isShowLogo,
            isActiveTiltle:(JSON.parse(response.data.StyleJson)).isActiveTiltle,
            themeType:(JSON.parse(response.data.StyleJson)).themeType,
            isNotAutoCreate:(JSON.parse(response.data.StyleJson)).isNotAutoCreate,
            isOblong:(JSON.parse(response.data.StyleJson)).isOblong?(JSON.parse(response.data.StyleJson)).isOblong:false,
            mobileNavType:(JSON.parse(response.data.StyleJson)).mobileNavType?(JSON.parse(response.data.StyleJson)).mobileNavType:'default',
            remark: response.data.Remark,
            isPublic: response.data.IsPublic,
            id:response.data.Id,
            conditionLabel:(JSON.parse(response.data.StyleJson)).conditionLabel?(JSON.parse(response.data.StyleJson)).conditionLabel:[]
        }
        this.open = true;
        this.title = "修改主题";
      });
    },
    /** 分配用户操作 */
    handleAuthOrg: function (row) {
      const themeId = row.Id;
      console.log();
      this.$router.push("/org/theme-auth/org/" + themeId);
    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          let submitForm = {};
          let dataJson = {
            mobileLogo: this.form.mobileLogo,
            pcThemeColor: this.form.pcThemeColor,
            isTopNav:this.form.isTopNav,
            isTagsViews:this.form.isTagsViews,
            isShowLogo:this.form.isShowLogo,
            isActiveTiltle:this.form.isActiveTiltle,
            themeType:this.form.themeType,
            isNotAutoCreate:this.form.isNotAutoCreate,
            isOblong:this.form.isOblong,
            mobileNavType:this.form.mobileNavType,
            conditionLabel:this.form.conditionLabel
          };
          submitForm = {
            name: this.form.name,
            photoUrl: this.form.photoUrl,
            styleJson: JSON.stringify(dataJson),
            remark: this.form.remark,
            isPublic: this.form.isPublic,
          };
          // console.log("保存主题",dataJson);
          if (this.form.id != undefined) {
            submitForm.id=this.form.id
            updateStyleMan(submitForm).then((response) => {
              this.$modal.msgSuccess("修改成功");
              this.open = false;
              this.getList();
            });
          } else {
            addStyleMan(submitForm).then((response) => {
              this.$modal.msgSuccess("新增成功");
              this.open = false;
              this.getList();
            });
          }
        }
      });
    },
    /** 删除按钮操作 */
    handleDelete(row) {
        console.log("单条数据",row);
      const themeId = row.Id || this.ids;
      this.$modal
        .confirm('是否确认删除编号为"' + themeId + '"的数据项')
        .then(function () {
          return delStyleMan(themeId);
        })
        .then(() => {
          this.getList();
          this.$modal.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
  },
};
</script>
<style lang="scss" scoped>
::v-deep .el-dialog__body{
  padding-left: 30px;
  padding-right: 30px;
  padding-top: 10px;
}
::v-deep .el-dialog__footer{
  padding-bottom: 30px;
}
.dialog_form{
  ::v-deep .el-form-item__label{
    font-size:14px;
    line-height: 14px;
    color: rgba(51, 51, 51, 1);
    width: 100%;
  }
  ::v-deep .el-form--label-top .el-form-item__label{
    padding-bottom: 14px;
  }

}

.cancel.el-button{
  width: 260px;
  height: 52px;
  font-size: 16px;
  color: rgba(120, 130, 157, 1);
}
.confirm.el-button{
  width: 260px;
  height: 52px;
  font-size: 16px;
  color: rgba(255, 255, 255, 1);
  background: linear-gradient( 90deg, #4C79FF 0%, #6DA8FF 100%);
}
.avatar_con {
    width: 100%;
    text-align: center;
    display: flex;
    align-items: flex-start;
    .label_tip{
      height: 68px;
      text-align: left;
    }
    .tip_con{
      width: 182px;
      height: 40px;
      border: 1px solid rgba(223, 226, 234, 1);
      color: rgba(120, 130, 157, 1);
      line-height: 40px;
      margin-right: 10px;
      text-align: center;
      border-radius: 4px;
      .zhongtaiiconfont{
        font-size: 10px;
      }
    }
    .el-upload--picture-card {
      background-color: #202e57;
      border: none;
    }
    ::v-deep .el-upload.el-upload--picture-card{
      width: 68px;
      height: 68px;
      line-height: 68px;
    }
    ::v-deep .component-upload-image{
      height: 68px;
      margin-bottom: 20px;
      .el-upload__tip{
        margin-top: 0;
        font-size: 12px;
        line-height: 20px;
        color: rgba(187, 192, 206, 1);
        text-align: left;
      }
    }
    ::v-deep .el-upload-list--picture-card .el-upload-list__item{
      width: 68px;
      height: 68px;
    }
  }
.item_flex_con{
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  &.color_flex{
    justify-content: flex-start;
  }
  .handle_con{
    display: flex;
    justify-content: flex-end;
    align-items: center;
    .guide_text{
      color: rgba(99, 101, 122, 1);
      font-size: 14px;
      margin-right: 30px;
    }
  }
  .add_text{
    font-size: 14px;
    color: rgba(53, 114, 255, 1);
    line-height: 14px;
    span{
      margin-left: 4px;
    }
  }
  .color_con{
    padding: 10px;
    width: 68px;
    height: 68px;
    box-sizing: border-box;
    display: flex;
    justify-content: center;
    align-items: center;
    border: 1px dashed rgba(223, 226, 234, 1);
    border-radius: 4px;
  }
  .tips_text{
      font-size: 12px;
      color: rgba(187, 192, 206, 1);
      margin-left: 20px;
    }
}
</style>
