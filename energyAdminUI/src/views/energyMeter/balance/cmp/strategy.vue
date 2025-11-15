<template>
  <div>
    <strategyComponent :originalHtml="originalHtml"></strategyComponent>
  </div>
</template>

<script>
import strategyComponent from './strategyComponent.vue'
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
    }
  },
  data() {
    return {
      originalHtml:'',
    };
  },
  watch:{
    strategyList:{
      handler(newval){
        
        if(newval&&newval.length>0){
          let htl='<p style="margin:0;">经数据分析：</p>'
          let maxRow=null
          let minRow=null
          newval.map((rw,ix)=>{
            if(ix==0){
              htl=htl+`<p style="margin:0;">目前${rw.name}${this.factorName}${rw.value}${rw.unit}，占比${rw.pence}%，</p>`
              maxRow=JSON.parse(JSON.stringify(rw))
              minRow=JSON.parse(JSON.stringify(rw))
            }else{
              htl=htl+`<p style="margin:0;">${rw.name}${this.factorName}${rw.value}${rw.unit}，占比${rw.pence}%，</p>`
              if(Number(maxRow.pence)<Number(rw.pence)){
                maxRow=JSON.parse(JSON.stringify(rw))
              }
              if(Number(minRow.pence)>Number(rw.pence)){
                minRow=JSON.parse(JSON.stringify(rw))
              }
            }
          })
          if(maxRow&&minRow){
            if(Number(maxRow.pence)-Number(minRow.pence)>20){
              htl=htl+'<p style="margin:0;">整体分布数据占比不平均。</p>'
              htl=htl+'<p style="margin:0;">　　　　</p>'
              htl=htl+'<p style="margin:0;">　　　　</p>'
              htl=htl+'<p style="margin:0;">建议策略：</p>'
              htl=htl+'<p style="margin:0;">建议根据产能平均车间投入与用能平衡，达到均衡。</p>'
            }else{
              htl=htl+'<p style="margin:0;">整体分布数据占比比较平均。</p>'
            }
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