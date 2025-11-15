<template>
  <el-dialog :visible.sync="dialog" class="adddialog" :show-close="false" width="800px">
    <div slot="title" class="dialog_title">
      <div class="dialog_title_left">
        <img src="@/assets/images/zs.png" alt="" />
        <span>新增计费标准</span>
      </div>
      <div class="dialog_title_right" @click.stop="closeDialog">
        <i class="zhongtaiiconfont zhongtai-icon-guanbi"></i>
      </div>
    </div>

    <div>
      <el-form :model="addForm" ref="addForm" :rules="addRules" label-position="top" label-width="120px">
        <el-row :gutter="8">
          <el-col :span="12">
            <el-form-item label="计费标准名称" prop="PolicyName">
              <el-input :disabled="isOnlyView" type="text" v-model="addForm.PolicyName" placeholder="请输入计费标准名称"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="能源类型" prop="EnergyType">
              <el-select :disabled="isOnlyView" style="width: 100%" v-model="addForm.EnergyType" clearable placeholder="请选择能源类型" @change="EnergyTypeChange">
                <el-option :label="it.TypeName" :value="it.Id" v-for="(it,ix) in OrgEngryList" :key="'Type'+ix"/>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="碳因子" prop="FactorId">
              <el-select :disabled="isOnlyView" style="width: 100%" v-model="addForm.FactorId" clearable placeholder="请选择碳因子" @change="EnergyTypeChange">
                <el-option :label="it.FactorName" :value="it.Id" v-for="(it,ix) in OrgFactorList" :key="'factor'+ix"/>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="计量单位" prop="Unit">
              <el-select :disabled="isOnlyView" style="width: 100%" v-model="addForm.Unit" clearable placeholder="请选择计量单位" @change="UnitChange">
                <el-option :label="item.oneValue" :value="item.oneValue" v-for="(item,jx) in UnitList" :key="'Unit'+jx"/>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <el-row :gutter="8">
        <el-col :span="24">
          <div class="freight_list">
            <div class="freight_li" v-for="(itemform, inx) in addForm.PolicyDetils" :key="'PolicyDetils' + inx">
              <el-form ref="itemform" :model="itemform" label-width="80px" label-position="left" :key="'itemform' + inx">
                <!-- <div class="item_form_div" style="margin-bottom:20px">
                  <div class="item_btn_li add_item_btn" @click.stop="exportRow" style="position: relative;border-radius:0;">
                    <i class="zhongtaiiconfont zhongtai-icon-daochu"></i>
                    <span style="margin-left: 6px">导出</span>
                  </div>
                  <el-upload style="display:inline" accept=".json" :multiple="false" :show-file-list="false" action="#" :before-upload="handleImport">
                    <div class="item_btn_li del_item_btn" style="position: relative;border-radius:0;">
                      <i class="zhongtaiiconfont zhongtai-icon-daoru"></i>
                      <span style="margin-left: 6px">导入</span>
                    </div>
                  </el-upload>
                </div> -->
                <el-form-item label="有效月份:" prop="PolicyMonth">
                  <div class="item_form_div">
                    <el-select v-if="!isOnlyView" :disabled="isOnlyView" popper-class="PolicyMonth_choice" class="PolicyMonth_choice" style="width: 500px" v-model="addForm.PolicyDetils[inx].PolicyMonth" multiple placeholder="请选择">
                      <el-option v-for="item in loadMonthList(inx)" :key="item.value" :label="item.label" :value="item.value" :disabled="item.disabled"></el-option>
                    </el-select>
                    <div v-else class="month_ul">
                      <div class="month_li" v-for="it in addForm.PolicyDetils[inx].PolicyMonth" :key="'month'+it">{{it+'月'}}</div>
                    </div>
                    <span class="title_text">共{{addForm.PolicyDetils[inx].PolicyMonth.length}}个月</span>
                    <div class="item_btn_li add_item_btn" v-if="!isOnlyView&&inx == 0" @click.stop="addPolicyDetils">
                      <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                      <span style="margin-left: 6px">添加</span>
                    </div>
                    <div class="item_btn_li del_item_btn" v-else-if="!isOnlyView" @click.stop="delPolicyDetils(inx)">
                      <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                      <span style="margin-left: 6px">删除</span>
                    </div>
                    <!-- <div class="item_btn_li del_item_btn" v-if="inx == 0" @click.stop="delPolicyDetils(inx)" style="position: relative;border-radius:0;">
                      <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>
                      <span style="margin-left: 6px">删除</span>
                    </div> -->
                  </div>
                </el-form-item>
                <el-form-item label="计费方式:" prop="PolicyType">
                  <el-radio-group v-model="addForm.PolicyDetils[inx].PolicyType" class="policy_type_radio">
                    <el-radio label="1" v-if="!isOnlyView||isOnlyView&&addForm.PolicyDetils[inx].PolicyType=='1'">分时价</el-radio>
                    <el-radio label="2" v-if="!isOnlyView||isOnlyView&&addForm.PolicyDetils[inx].PolicyType=='2'">不分时</el-radio>
                    <el-radio label="3" v-if="!isOnlyView||isOnlyView&&addForm.PolicyDetils[inx].PolicyType=='3'">阶梯价</el-radio>
                  </el-radio-group>
                </el-form-item>
                <el-form-item label="单价设置:" prop="PolicyType" v-if="addForm.PolicyDetils[inx].PolicyType == 1">
                  <el-row :gutter="6">
                    <el-col :span="6" style="width: 160px">
                      <el-input :disabled="isOnlyView" type="number" style="width: 154px" placeholder="单价" v-model="addForm.PolicyDetils[inx].JianPrice">
                        <template slot="prepend">尖</template>
                        <template slot="append">元</template>
                      </el-input>
                    </el-col>
                    <el-col :span="6" style="width: 160px">
                      <el-input :disabled="isOnlyView" type="number" style="width: 154px" placeholder="单价" v-model="addForm.PolicyDetils[inx].FengPrice">
                        <template slot="prepend">峰</template>
                        <template slot="append">元</template>
                      </el-input>
                    </el-col>
                    <el-col :span="6" style="width: 160px">
                      <el-input :disabled="isOnlyView" type="number" style="width: 154px" placeholder="单价" v-model="addForm.PolicyDetils[inx].PingPrice">
                        <template slot="prepend">平</template>
                        <template slot="append">元</template>
                      </el-input>
                    </el-col>
                    <el-col :span="6" style="width: 160px">
                      <el-input :disabled="isOnlyView" type="number" style="width: 154px" placeholder="单价" v-model="addForm.PolicyDetils[inx].GuPrice">
                        <template slot="prepend">谷</template>
                        <template slot="append">元</template>
                      </el-input>
                    </el-col>
                  </el-row>
                </el-form-item>
                <el-form-item label="单价设置:" prop="PolicyType" v-if="addForm.PolicyDetils[inx].PolicyType == 2">
                  <el-row :gutter="10">
                    <el-col :span="6">
                      <el-input :disabled="isOnlyView" style="width: 154px" placeholder="单价" v-model="addForm.PolicyDetils[inx].DayPrice">
                        <template slot="append">元</template>
                      </el-input>
                    </el-col>
                  </el-row>
                </el-form-item>
                <el-form-item label="单价设置:" v-if="addForm.PolicyDetils[inx].PolicyType == 3">
                  <div class="Price_tiers">
                    <div class="Price_tiers_title">
                      <div class="tiers_title_interval">区间</div>
                      <div class="tiers_title_li">单价</div>
                      <div class="tiers_title_add" @click.stop="addPriceTiers(inx)" v-if="!isOnlyView">
                        <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                        <span style="margin-left: 6px">添加</span>
                      </div>
                    </div>
                    <div class="Price_tiers_ul">
                      <div class="tiers_ul_li" v-for="(PriceTiersForm, ix) in addForm.PolicyDetils[inx].PriceTiers" :key="'Price_tiers' + inx + ix">
                        <el-input :disabled="isOnlyView" style="width: 120px" type="number" v-model="addForm.PolicyDetils[inx].PriceTiers[ix].MinKwh" placeholder="最小值"></el-input>
                        <div class="tiers_ul_li_symbol"><span>≤</span><span>用量</span><span>≥</span></div>
                        <el-input :disabled="isOnlyView" style="width: 120px" type="number" v-model="addForm.PolicyDetils[inx].PriceTiers[ix].MaxKwh" placeholder="最大值"></el-input>
                        <el-input :disabled="isOnlyView" type="number" style="width: 120px;margin-left:24px;" placeholder="单价" v-model="addForm.PolicyDetils[inx].PriceTiers[ix].Price">
                          <template slot="append">元</template>
                        </el-input>
                        <div class="handle_text" v-if="!isOnlyView">
                          <span @click="delPriceTiers(inx, ix)">删除</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </el-form-item>
                <el-form-item label="时段设置:" prop="PolicyType" v-if="addForm.PolicyDetils[inx].PolicyType == 1">
                  <div class="Price_times_con">
                    <div class="Price_times_title">
                      <div class="times_title_li">起始时间</div>
                      <div class="times_title_li">截止时间</div>
                      <div class="times_title_li">峰谷属性</div>
                      <div class="times_title_li add" @click.stop="addPriceTimes(inx)" v-if="!isOnlyView">
                        <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
                        <span style="margin-left: 6px">添加</span>
                      </div>
                    </div>
                    <div class="Price_times_ul">
                      <div class="Price_times_li" v-for="(PriceTimesForm, i) in addForm.PolicyDetils[inx].PriceTimes" :key="'Price_times' + inx + i">
                        <el-time-select :disabled="isOnlyView" style="width: 140px" placeholder="起始时间" v-model="addForm.PolicyDetils[inx].PriceTimes[i].StartTime"
                          :picker-options="{start: '00:00',step: '00:01',end: '23:59',}"></el-time-select>
                        <div class="times_li_line"></div>
                        <el-time-select :disabled="isOnlyView" style="width: 140px" placeholder="截止时间" v-model="addForm.PolicyDetils[inx].PriceTimes[i].EndTime"
                          :picker-options="{start: '00:00',step: '00:01',end: '23:59',}"></el-time-select>
                        <div class="times_li_zhanwei"></div>
                        <el-select :disabled="isOnlyView" style="width: 140px" v-model="addForm.PolicyDetils[inx].PriceTimes[i].TimePeriod" clearable placeholder="峰谷">
                          <el-option label="尖" value="1" />
                          <el-option label="峰" value="2" />
                          <el-option label="平" value="3" />
                          <el-option label="谷" value="4" />
                        </el-select>
                        <div class="handle_text" v-if="!isOnlyView">
                          <span @click="delPriceTimes(inx, i)">删除</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </el-form-item>
              </el-form>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>

    <div slot="footer" class="dialog-footer" v-if="!isOnlyView">
      <el-button @click="dialog = false">取消</el-button>
      <el-button type="primary" @click="submitForm" v-loading="saveLoading">保存</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { addPolicy, editPolicy,policyInfo } from "@/api/energy/Policy";
import {selectFactorOrgByTypeId} from "@/api/energy/factorLibrary";
export default {
  name: "EnergyAdminUIFreightAdd",
  props:{
    OrgEngryList:{
      type:Array,
      default:()=>{
        return []
      }
    }
  },
  data() {
    return {
      UnitList:[{
        oneValue:'KWh',
        twoValue:'MWh',
      },{
        oneValue:'Nm³',
        twoValue:'万Nm³',
      }],
      dialog: false,
      isOnlyView: false,
      addForm: {
        PolicyName: "",
        EnergyType: "",
        FactorId:'',
        Unit: "",
        LageUnit: "",
        Memo: "",
        PolicyDetils: [
          {
            PolicyMonth: [],
            PolicyType: "1",
            JianPrice: 0.0,
            FengPrice: 0.0,
            PingPrice: 0.0,
            GuPrice: 0.0,
            PriceTimes: [
              {
                TimePeriod: "",
                StartTime: "",
                EndTime: "",
              },
            ],
            DayPrice: 0.0,
            CycleType: "1",
            PriceTiers: [
              {
                TierLevel: "1",
                MinKwh: 0,
                MaxKwh: 0,
                Price: 0,
              },
            ],
          },
        ],
      },
      addRules: {
        PolicyName: [
          { required: true, trigger: "blur", message: "请输入计费标准名称" },
        ],
        EnergyType: [
          { required: true, trigger: "change", message: "请选择能源类型" },
        ],
        FactorId: [
          { required: true, trigger: "change", message: "请选择能源类型" },
        ],
        Unit: [{ required: true, trigger: "blur", message: "请输入单位" }],
      },
      saveLoading: false,
      monthList: [[]],
      OrgFactorList:[],//已选的排放因子//展示是能源类型
      orgId:'',
    };
  },

  mounted() {
    this.orgId = this.$store.state.user.orgId;
  },
  computed:{
    
  },
  methods: {
    EnergyTypeChange(){
      if(this.addForm.EnergyType){
        selectFactorOrgByTypeId({OrgId:this.orgId,TypeId:this.addForm.EnergyType}).then(res=>{
          this.OrgFactorList=res.data
        })
      }else{
        this.addForm.FactorId=''
      }
      // if(this.addForm.FactorId){
      //   let findRow=this.OrgFactorList.find(row=>row.Id==this.addForm.FactorId)
      //   if(findRow){
      //     this.addForm.EnergyType=findRow.typeId
      //   }else{
      //     this.addForm.EnergyType=''
      //   }
      // }else{
      //   this.addForm.EnergyType=''
      // }
    },
    UnitChange(){
      if(this.addForm.Unit){
        let findRow=this.UnitList.find(row=>row.oneValue==this.addForm.Unit)
        if(findRow){
          this.addForm.LageUnit=findRow.twoValue
        }else{
          this.addForm.LageUnit=''
        }
      }else{
        this.addForm.LageUnit=''
      }
    },
    
    addPolicyDetils() {
      let rowNew={
        PolicyMonth: [],
        PolicyType: "1",
        JianPrice: 0.0,
        FengPrice: 0.0,
        PingPrice: 0.0,
        GuPrice: 0.0,
        PriceTimes: [
          {
            TimePeriod: "",
            StartTime: "",
            EndTime: "",
          },
        ],
        DayPrice: 0.0,
        CycleType: "1",
        PriceTiers: [
          {
            TierLevel: "1",
            MinKwh: 0,
            MaxKwh: 0,
            Price: 0,
          },
        ],
      }
      this.addForm.PolicyDetils.push(rowNew);
      this.monthList.push([])
      // this.loadMonthList()
    },
    delPolicyDetils(inx) {
      this.addForm.PolicyDetils.splice(inx, 1);
      // this.loadMonthList()
    },
    addPriceTimes(inx) {
      //添加价格时间段
      this.addForm.PolicyDetils[inx].PriceTimes.push({
        TimePeriod: "",
        StartTime: "",
        EndTime: "",
      });
    },
    delPriceTimes(inx, ix) {
      this.addForm.PolicyDetils[inx].PriceTimes.splice(ix, 1);
    },
    addPriceTiers(inx) {
      //添加阶段价格
      if (this.addForm.PolicyDetils[inx].PriceTiers.length < 3) {
        this.addForm.PolicyDetils[inx].PriceTiers.push({
          TierLevel: (this.addForm.PolicyDetils[inx].PriceTiers.length + 1)+'',
          MinKwh: 0,
          MaxKwh: 0,
          Price: 0,
        });
      } else {
        this.$message.error("最多只可设置3个阶段");
      }
    },
    delPriceTiers(inx, ix) {
      this.addForm.PolicyDetils[inx].PriceTiers.splice(ix, 1);
    },
    loadMonthList(inx) {
      let monthList = [];
        try {
          let PolicyDetils=JSON.parse(JSON.stringify(this.addForm.PolicyDetils))
          let allOtherChoice=PolicyDetils.filter((row,ix)=>ix!=inx)
          let allOtherChoicemonth=[]
          if(allOtherChoice&&allOtherChoice.length>0){
            allOtherChoice.map(row=>{
              if(allOtherChoicemonth&&allOtherChoicemonth.length>0){
                allOtherChoicemonth=[...allOtherChoicemonth,...row.PolicyMonth]
              }else{
                allOtherChoicemonth=row.PolicyMonth
              }
              
            })
          }
          for (let i = 1; i < 13; i++) {
            let obj = {
              value: i+'',
              label: i + "月",
            };
            if(allOtherChoicemonth.includes(i+'')){
              obj.disabled=true
            }else{
              obj.disabled=false
            }
            monthList.push(obj);
          }
        } catch (error) {
          console.log("error",error);
        }
        return monthList
      
    },
    async loadpolicyInfo(id){
      let res=await policyInfo(id)
      return res.data
    },
   async openDialog(row, isOnlyView) {
      if (row) {
        let info=await this.loadpolicyInfo(row.Id)
        this.addForm = {
          Id:info.Id,
          PolicyName: info.PolicyName,
          EnergyType: info.EnergyType,
          FactorId:info.FactorId,
          Unit: info.Unit,
          LageUnit: info.LageUnit,
          Memo: info.Memo,
          PolicyDetils: [],
        };
        if(info.PolicyDetils&&info.PolicyDetils.length>0){
          this.addForm.PolicyDetils=info.PolicyDetils.map(row=>{
            row.PolicyMonth=row.PolicyMonth.split(',')
            if(row.PriceTiers){
              row.PriceTiers=row.PriceTiers
            }else{
              row.PriceTiers=[
                {
                  TierLevel: "1",
                  MinKwh: 0,
                  MaxKwh: 0,
                  Price: 0,
                },
              ]
            }
            if(row.PriceTimes){
              row.PriceTimes=row.PriceTimes
            }else{
              row.PriceTimes=[
                {
                  TimePeriod: "",
                  StartTime: "",
                  EndTime: "",
                },
              ]
            }
            return row
          })
        }else{
          this.addForm.PolicyDetils=[
            {
              PolicyMonth: [],
              PolicyType: "1",
              JianPrice: 0.0,
              FengPrice: 0.0,
              PingPrice: 0.0,
              GuPrice: 0.0,
              PriceTimes: [
                {
                  TimePeriod: "",
                  StartTime: "",
                  EndTime: "",
                },
              ],
              DayPrice: 0.0,
              CycleType: "1",
              PriceTiers: [
                {
                  TierLevel: "1",
                  MinKwh: 0,
                  MaxKwh: 0,
                  Price: 0,
                },
              ],
            },
          ]
        }
        this.EnergyTypeChange()
      } else {
        this.addForm = {
          PolicyName: "",
          EnergyType: "",
          FactorId:'',
          Unit: "",
          LageUnit: "",
          Memo: "",
          PolicyDetils: [
            {
              PolicyMonth: [],
              PolicyType: "1",
              JianPrice: 0.0,
              FengPrice: 0.0,
              PingPrice: 0.0,
              GuPrice: 0.0,
              PriceTimes: [
                {
                  TimePeriod: "",
                  StartTime: "",
                  EndTime: "",
                },
              ],
              DayPrice: 0.0,
              CycleType: "1",
              PriceTiers: [
                {
                  TierLevel: "1",
                  MinKwh: 0,
                  MaxKwh: 0,
                  Price: 0,
                },
              ],
            },
          ],
        };
      }
      if (isOnlyView) {
        this.isOnlyView = isOnlyView;
      } else {
        this.isOnlyView = false;
      }
      // this.loadMonthList()
      this.dialog = true;
    },
    closeDialog() {
      this.dialog = false;
    },
    submitForm() {
      this.$refs["addForm"].validate((valid) => {
        if (valid) {
          // let submitTag=[]
          let submitForm = JSON.parse(JSON.stringify(this.addForm));
          // submitForm.MarketTime=this.addForm
          let iserr=false
          let iserrmes=''
          submitForm.PolicyDetils=submitForm.PolicyDetils.map(row=>{
            row.PolicyMonth=row.PolicyMonth.join(',')
            row.PriceTiers.map(rw=>{
              rw.MinKwh=Number(rw.MinKwh)
              rw.MaxKwh=Number(rw.MaxKwh)
              rw.Price=Number(rw.Price)
            })
            if(row.PriceTimes&&row.PriceTimes.length>0){
              for(let i=0;i<row.PriceTimes.length;i++){
                for(let timeKey in row.PriceTimes[i]){
                  if(timeKey=='StartTime'&&row.PriceTimes[i].StartTime!=null&&row.PriceTimes[i].StartTime!=''){
                  }else if(timeKey=='StartTime'){
                    iserr=true
                    iserrmes='请填写时间段开始时间'
                    return
                  }
                  if(timeKey=='EndTime'&&row.PriceTimes[i].EndTime!=null&&row.PriceTimes[i].EndTime!=''){
                  }else if(timeKey=='EndTime'){
                    iserr=true
                    iserrmes='请填写时间段结束时间'
                    return
                  }
                  if(timeKey=='TimePeriod'&&row.PriceTimes[i].TimePeriod!=null&&row.PriceTimes[i].TimePeriod!=''){
                  }else if(timeKey=='TimePeriod'){
                    iserr=true
                    iserrmes='请填写时间段开始时间'
                    return
                  }
                }
              }
              
            }
            
            return row
          })
          if(iserr&&iserrmes){
            this.$message.error(iserrmes);
            return
          }
          this.saveLoading = true;
          if (this.addForm.Id) {
            editPolicy(submitForm)
              .then((response) => {
                if (response.code == 0) {
                  this.$message.success("修改计费标准成功");
                  this.dialog = false;
                  this.$emit("loadList");
                  this.saveLoading = false;
                }
              })
              .catch((err) => {
                this.saveLoading = false;
              });
          } else {
            submitForm.OrgId=this.orgId
            addPolicy(submitForm)
              .then((response) => {
                if (response.code == 0) {
                  this.$message.success("添加计费标准成功");
                  this.dialog = false;
                  this.$emit("loadList");
                  this.saveLoading = false;
                }
              })
              .catch((err) => {
                this.saveLoading = false;
              });
          }
        }
      });
    },
    exportRow(row,isAll) {
      let tmploading = this.$loading({
        lock: true,
        text: "导出中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      let msgitem = {
        t: '计费时段数据',
        items: JSON.parse(JSON.stringify(this.addForm.PolicyDetils))
      };
      let tmname = '计费时段数据';
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
    handleImport(file){
      let tmploading = this.$loading({
        lock: true,
        text: "导入中...",
        background: "rgba(0, 0, 0, 0.7)",
      });
      const reader = new FileReader()
      reader.readAsText(file)
      reader.onload = async (e)=> {
        try {
          const str = e.target.result
          const jsonData = JSON.parse(str)
          let PolicyDetils=JSON.parse(JSON.stringify(jsonData.items))
          console.log("导入的数据",PolicyDetils);
          this.addForm.PolicyDetils=PolicyDetils.map(row=>{
            if(typeof row.PolicyMonth=='string'){
              row.PolicyMonth=row.PolicyMonth.split(',')
            }
            
            if(row.PriceTiers){
              row.PriceTiers=row.PriceTiers
            }else{
              row.PriceTiers=[
                {
                  TierLevel: "1",
                  MinKwh: 0,
                  MaxKwh: 0,
                  Price: 0,
                },
              ]
            }
            if(row.PriceTimes){
              row.PriceTimes=row.PriceTimes
            }else{
              row.PriceTimes=[
                {
                  TimePeriod: "",
                  StartTime: "",
                  EndTime: "",
                },
              ]
            }
            return row
          })
          tmploading.close();
        } catch (error) {
          tmploading.close();
        }
      }
    },
  },
};
</script>

<style lang="scss" scoped>
// .PolicyMonth_choice{
//   &.el-select{
//     // .el-select__tags{
//       ::v-deep .el-tag.el-tag--info{
//         background: rgba(34, 46, 64, 1);
//         color: #ffffff;
//         border-color: rgba(34, 46, 64, 1);
//         height: 26px;
//         .el-tag__close{
//           color: #ffffff;
//           background-color:transparent;
//         }
//       }
//     // }
    
//   }
// }
.PolicyMonth_choice.el-select-dropdown.is-multiple{
  
 .el-select-dropdown__item.is-disabled::after {
    position: absolute;
    right: 20px;
    font-family: element-icons;
    content: "\e6da";
    font-size: 12px;
    font-weight: 700;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }
  .el-select-dropdown__item.is-disabled{
    color: rgba(61, 185, 143, 1);
    opacity: 0.4;
  }
  .el-select-dropdown__item.is-disabled:hover{
    background: transparent;
  }
}
.freight_list {
  padding: 20px;
  width: 100%;
  box-sizing: border-box;
  background: rgba(19, 25, 34, 1);
  .freight_li {
    font-size: 14px;
    color: rgba(255, 255, 255, 1);
    .item_form_div {
      display: flex;
      align-items: center;
      width: 100%;
      position: relative;
      height: 36px;
      .month_ul{
        display: flex;
        flex-wrap: wrap;
        // align-items: center;
        max-width: calc(100% - 77px);
        .month_li{
          padding: 7px 15px;
          line-height: 14px;
          font-size: 14px;
          color: rgba(255, 255, 255, 1);
          background: rgba(34, 46, 64, 1);
          white-space: nowrap;
          margin-right: 2px;
          margin-top: 2px;
          border-radius: 2px;
        }
      }
      .title_text {
        color: rgba(255, 255, 255, 0.6);
        margin-left: 10px;
      }
      .item_btn_li {
        position: absolute;
        right: -20px;
        top: 2px;
        width: 70px;
        height: 32px;
        background: rgba(34, 46, 64, 1);
        display: flex;
        justify-content: center;
        align-items: center;
        border-radius: 20px 0 0 20px;
        cursor: pointer;
        &.add_item_btn {
          color: rgba(61, 185, 143, 1);
          
          i {
            color: rgba(61, 185, 143, 1);
            font-size: 10px;
          }
        }
        &.del_item_btn {
          color: rgba(255, 255, 255, 1);
          i {
            color: rgba(255, 255, 255, 1);
            font-size: 12px;
          }
        }
      }
    }
    .policy_type_radio {
    }
    .Price_tiers {
      .Price_tiers_title {
        width: 100%;
        
        display: flex;
        align-items: center;
        // justify-content: space-between;
        height: 36px;
        .tiers_title_interval {
          width: 372px;
          text-align: center;
          background: rgba(34, 46, 64, 1);
        }
        .tiers_title_li {
          width: 144px;
          text-align: center;
          background: rgba(34, 46, 64, 1);
        }
        .tiers_title_add {
          width: 124px;
          text-align: center;
          color: rgba(61, 185, 143, 1);
          cursor: pointer;
          background: rgba(34, 46, 64, 1);
          i{
            font-size: 10px;
          }
        }
      }
      .Price_tiers_ul {
        .tiers_ul_li {
          display: flex;
          align-items: center;
          margin-top:8px;
          .tiers_ul_li_symbol {
            width: 100px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            color: rgba(255, 255, 255, 1);
            margin: 0 16px;
          }
          .handle_text {
            width: 140px;
            text-align: center;
            color: rgba(241, 92, 92, 1);
          }
        }
      }
    }
    .Price_times_con {
      .Price_times_title {
        display: flex;
        // justify-content: space-between;
        align-items: center;
        
        height: 36px;
        width: 100%;
        .times_title_li {
          background: rgba(34, 46, 64, 1);
          width: 166.6666666666666666667px;
          color: rgba(255, 255, 255, 0.6);
          text-align: center;
          height: 36px;
          line-height: 36px;
          &.add {
            color: rgba(61, 185, 143, 1);
            cursor: pointer;
            width: 140px;
            i{
              font-size: 10px;
            }
          }
        }
      }
      .Price_times_ul {
        width: 100%;
        .Price_times_li {
          display: flex;
          align-items: center;
          // justify-content: space-between;
          width: 100%;
          height: 36px;
          margin-top: 8px;
          .times_li_line {
            width: 8px;
            height: 2px;
            background: rgba(151, 151, 151, 1);
            margin: 0 16px;
          }
          .times_li_zhanwei {
            width: 40px;
            height: 36px;
          }
          .handle_text {
            width: 140px;
            text-align: center;
            color: rgba(241, 92, 92, 1);
          }
        }
      }
    }
  }
}
.adddialog {
  ::v-deep .el-dialog__header {
    padding: 0;
    color: #ffffff;
  }
}

.dialog_title {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  .dialog_title_left {
    font-size: 16px;
    color: #ffffff;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    padding-left: 20px;
    line-height: 16px;
    height: 56px;
    .img {
      width: 16px;
      height: 16px;
    }
    span {
      margin-left: 6px;
    }
  }
  .dialog_title_right {
    margin-right: 20px;
    cursor: pointer;
    i.zhongtaiiconfont {
      color: rgba(255, 255, 255, 0.6);
      font-size: 12px;
    }
  }
}
</style>