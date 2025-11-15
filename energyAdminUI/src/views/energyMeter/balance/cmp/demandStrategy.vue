<template>
  <div>
    <strategyComponent :originalHtml="originalHtml"></strategyComponent>
    <!-- <div class="content_ai">
        <div class="content_con">
            <div class="content_text" ref="contentTextContainer">
                <div class="title">用能策略推荐</div>
                <div class="text">
                    <p>经数据模型分析</p>
                    <p>目前企业最大需求主要分布在{{info.TTime}} 时段，高峰需求量电力消费为 {{info.UseVale}} kwh。结合当地电力计费标准如下：</p>
                    <div class="strategy_table_con">
                      <div class="strategy_table_ul" v-for="(ite,inx) in strategyTableData" :key="'ul'+inx">
                        <div class="strategy_table_li" v-for="(item,ix) in ite" :key="'li'+ix">
                          <div class="table_li_val" v-for="(ro,i) in item" :key="'lival'+i">{{ro}}</div>
                        </div>
                      </div>
                    </div>
                    <p>建议调整策略</p>
                    <p>根据分时电价，建议可将高峰时段转移到平段或谷段，电力成本每月将缩减大约{{info.SaveCost}}元。</p>
                </div>
                <div class="hide">隐藏</div>
            </div>
            <div class="ai_img">
                <img class="img" src="@/assets/images/strategy_ai.png" alt="">
            </div>
        </div>
    </div> -->
  </div>
</template>

<script>
import strategyComponent from './strategyComponent.vue'
import {selectEnergyStrategy} from '@/api/energy/energyMeter'
export default {
  name: 'DemandStrategy',
  components:{strategyComponent},
  props:{
    strategyList:{
      type:Array,
      default:()=>{
        return []
      }
    },
    factorName:{
      type:String,
      default:''
    },
    queryParams:{
      type:Object,
      default:()=>{
        return {}
      }
    },
  },
  data() {
    return {
      originalHtml:'',
      strategyTableData:[],
      info:{},
      isTopTime:false,//判断最大需量是否是高峰时间段
      topTimeType:'',//判断最大需量高峰时间段
    };
  },
  watch:{
    queryParams:{
        handler(newval){
          let queryVal=JSON.parse(JSON.stringify(newval))
          if(queryVal&&queryVal.OrgId&&queryVal.facilityId){
            this.loadSelectEnergyStrategy(queryVal)
          }
          
        },
        deep:true
    },
  },
  mounted() {
    
  },
  methods: {
    loadSelectEnergyStrategy(query){
      this.strategyTableData=[[['峰谷属性'],['时间段'],['电力单价']]]
      this.isTopTime=false
      this.topTimeType=''
      selectEnergyStrategy(query).then(res=>{
          res.data.UseVale=Number(Number(res.data.UseVale).toFixed(2))
          this.info=JSON.parse(JSON.stringify(res.data))
          if(this.info.Policy){
            let hasSharpTime=this.info.Policy.find(rw=>rw.PolicyType=='尖')
            this.info.Policy.map(row=>{
              let rowArr=[]
              for(let keVal in row){
                if(Array.isArray(row[keVal])){
                  rowArr[1]=row[keVal]
                }else{
                  if(keVal=='PolicyType'){
                    rowArr[0]=[row[keVal]]
                  }
                  if(keVal=='Price'){
                    rowArr[2]=[row[keVal]]
                  }
                }
              }
              if(hasSharpTime){
                if(row.PolicyType=='尖'){
                  this.topTimeType='尖'
                  if(row.TTime.includes(this.info.TTime)){
                    this.isTopTime=true
                  }else{
                    this.isTopTime=false
                  }
                }
              }else{
                if(row.PolicyType=='峰'){
                  this.topTimeType='峰'
                  if(row.TTime.includes(this.info.TTime)){
                    this.isTopTime=true
                  }else{
                    this.isTopTime=false
                  }
                }
              }
              this.strategyTableData.push(rowArr)
            })
          }
          this.generateHtml()
      })
      
    },
    generateHtml(){
      let htl='<p style="margin:0;">经数据模型分析：</p>'
      // if(this.topTimeType&&this.isTopTime){
        htl=htl+`<p style="margin:0;">目前企业最大需求主要分布在${this.info.TTime} 时段，${this.topTimeType}值需求量电力消费为 ${this.info.UseVale} kwh。结合当地电力计费标准如下：</p>`
        if(this.strategyTableData&&this.strategyTableData.length>0){
          htl=htl+`<div class="strategy_table_con" style="width: 100%;border: 1px solid rgba(255, 255, 255, 0.50);margin-top: 12px;margin-bottom: 12px;">`
          for(let i=0;i<this.strategyTableData.length;i++){
            let ite=this.strategyTableData[i]
            htl=htl+`<div class="strategy_table_ul" style="display: flex;width: 100%;">`
            for(let j=0;j<ite.length;j++){
              let item=ite[j]
              htl=htl+`<div class="strategy_table_li" style="width: calc(100% / 3);border: 1px solid rgba(255, 255, 255, 0.50);padding-bottom: 9px;">`
              for(let k=0;k<item.length;k++){
                let ro=item[k]
                htl=htl+`<div class="table_li_val" style="text-align: center;margin-top: 9px;line-height: 14px;">${ro}</div>`
              }
              htl=htl+`</div>`
            }
            htl=htl+`</div>` 
          }  
          htl=htl+`</div>`
        }     
        
        htl=htl+'<p style="margin:0;">　　　　</p>'
        htl=htl+'<p style="margin:0;">　　　　</p>'
        htl=htl+`<p style="margin:0;">建议调整策略</p>
        <p style="margin:0;">根据分时电价，建议可将${this.topTimeType}值时段转移到平段或谷段，电力成本每月将缩减大约${this.info.SaveCost}元。</p>`
      // }else{
      //   htl=htl+`最大需量分布时间段合理。`
      // }
      htl=htl+'<p style="margin:0;">　　　　</p>'
      htl=htl+'<p style="margin:0;">　　　　</p>'
      htl=htl+'<p style="margin:0;">　　　　</p>'
      this.originalHtml=htl
    }
  },
};
</script>

<style lang="less" scoped>
</style>