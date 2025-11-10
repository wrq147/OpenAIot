<template>
    <div :id="id"></div>
</template>

<script>

import VueEvent from '../../VueEvent'


export default {
  components: {

  },
  props: ["chartOption","dragchartdata", "drawingList"],
  data() {
    return {
      id: this.dragchartdata.customId,
      width: this.dragchartdata.width,
      height: this.dragchartdata.height,
      left: this.dragchartdata.x,
      top: this.dragchartdata.y,
      prevOffsetX: 0,
      prevOffsetY: 0,
      flag: true,
    };
  },
  watch: {
    dragchartdata: {
      deep: true,
      handler(newVal) {
        
        this.width = newVal.width;
        this.height = newVal.height;
        this.x = newVal.x;
        this.y = newVal.y;
        
      }
    },
    chartOption: {
      deep: true,
      handler(newVal) {
        this.setSize(newVal);
        
      }
    },
    
  },
  mounted(){
    VueEvent.$on("combine_move", (cid,x,y)=>{
      if (cid == this.id) {
        this.move(x,y)
        this.flag = false;
      }
    });
    VueEvent.$on("combine_stop", ()=>{
      this.prevOffsetX = 0;
      this.prevOffsetY = 0;
      this.flag = true;
    })
  },
  created(){
    this.setSize(this.dragchartdata.chartOption);
    
  },
  methods: {
    setSize(newVal){

      if(this.flag){

        let leftArr = [];
        let topArr = [];
        let rightArr = [];
        let downArr = [];
        
        newVal.forEach(chart => {
          leftArr.push(chart.x);
          topArr.push(chart.y);
          rightArr.push(chart.x + chart.width);
          downArr.push(chart.y + chart.height);
        });

        leftArr = leftArr.sort((a,b)=>{return a-b}); 
        topArr = topArr.sort((a,b)=>{return a-b});
        rightArr = rightArr.sort((a,b)=>{return a-b});
        downArr = downArr.sort((a,b)=>{return a-b});

        this.$set(this.dragchartdata, 'width', rightArr[rightArr.length-1] - leftArr[0]);
        this.$set(this.dragchartdata, 'height', downArr[downArr.length-1] - topArr[0]);
        this.$set(this.dragchartdata, 'x', leftArr[0]);
        this.$set(this.dragchartdata, 'y', topArr[0]);
      }

    },
    move(left,top){
      
      const offsetX = left - this.dragchartdata.x;
      const offsetY = top - this.dragchartdata.y;
      
      const deltaX = this.deltaX(offsetX);
      const deltaY = this.deltaY(offsetY);

      this.dragchartdata.chartOption.map(el => {
        
          el.x += deltaX;
          el.y += deltaY;

        return el;
      });

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
  },
  
};
</script>

<style lang="scss">

</style>