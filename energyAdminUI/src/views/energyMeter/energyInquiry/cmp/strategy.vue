<template>
  <div>
    <strategyComponent :originalHtml="originalHtml" :key="'strate'+comptKey"></strategyComponent>
  </div>
</template>

<script>
import strategyComponent from '@/views/energyMeter/balance/cmp/strategyComponent.vue'
export default {
  name: 'EnergyAdminUIStrategy',
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
    equipName:{
      type:String,
      default:''
    },
    startTime:{
      type:String,
      default:''
    },
    endTime:{
      type:String,
      default:''
    },
    timeType:{
      type:String,
      default:'月'
    },
  },
  data() {
    return {
      originalHtml:'',
      comptKey:123
    };
  },
  watch:{
    strategyList:{
      handler(newval){
        this.comptKey = new Date().getTime();
        if(newval&&newval.length>0){
          let htl='<p style="margin:0;">经数据分析：</p>'
          let maxRow=null
          let minRow=null

          newval.map((rw,ix)=>{
            if(rw.TimePeriod=='尖'&&Number(rw.UseVale)>0){
              maxRow=JSON.parse(JSON.stringify(rw))
            }else if(maxRow==null&&rw.TimePeriod=='峰'&&Number(rw.UseVale)>0){
              maxRow=JSON.parse(JSON.stringify(rw))
            }
            if(rw.TimePeriod=='谷'&&Number(rw.UseVale)>0){
              minRow=JSON.parse(JSON.stringify(rw))
            }else if(minRow==null&&rw.TimePeriod=='平'&&Number(rw.UseVale)>0){
              minRow=JSON.parse(JSON.stringify(rw))
            }
            if(ix==0){
              htl=htl+`<p style="margin:0;">目前${this.equipName}统计${this.startTime}至${this.endTime}时间周期内统计尖峰${this.factorName}情况如下：</p>`
            }
            htl=htl+`<p style="margin:0;">${rw.TimePeriod}值${this.factorName}占比总体${this.factorName}的${rw.pence}%（${rw.TimePeriod}值${this.factorName}/总${this.factorName}），总成本${rw.CostVale}元。</p>`
            
          })
          if(maxRow&&minRow){
            let diff=Number(maxRow.Price)-Number(minRow.Price)
            let diffPrice=Number(diff*maxRow.UseVale).toFixed(2)
            htl=htl+'<p style="margin:0;">　　　　</p>'
            htl=htl+'<p style="margin:0;">　　　　</p>'
            htl=htl+'<p style="margin:0;">建议调整策略：</p>'
            htl=htl+`<p style="margin:0;">根据分时电价，建议可将高峰时段转移到平段或谷段，${this.factorName}成本每${this.timeType}将缩减${diffPrice}元。</p>`
          }
          htl=htl+'<p style="margin:0;">　　　　</p>'
          htl=htl+'<p style="margin:0;">　　　　</p>'
          htl=htl+'<p style="margin:0;">　　　　</p>'
          this.originalHtml=htl
          
        }else{
          this.originalHtml='无数据可做分析！'
        }
      },
      deep:true,
    }
  },
  mounted() {
    
  },
  methods: {
    
  },
};
</script>

<style lang="less" scoped>
</style>