<template>
  <div :style="{ height: height, width: width,'overflow-y': chartOption.isRoll?'hidden':'auto' }" :id="chartOption.bindingDiv">
    <SeamlessScroll :data="rows" :loop="true" v-if="chartOption.isRoll" :class-option="{direction: loopDirection, step: loopSeep,waitTime: 1000}">
      <div class="con_con" ref="text" :style="{...containerBgStyle,'flex-direction': chartOption.loopDirection&&chartOption.loopDirection=='2'?'column':'row','align-content':!chartOption.isRoll&&chartOption.blockalignContent?chartOption.blockalignContent:''}">
        <template v-for="(col, colindex) in rows">
          <div :style="block_conStyle" :key="colindex" class="block_con_con">
            <div :class="animate" class="block_con" :style="getBlockStyle(col)">
              <div class="jindu_con" :style="getBarConStyle(col)">
                <div class="jindu" :style="getBarStyle(col)"></div>
                <div class="jindutext">
                  <img v-if="!chartOption.ishideIcon&&chartOption.progressbar.iconUrl" :src="chartOption.progressbar.iconUrl" alt=""/>
                  <img v-if="!chartOption.ishideIcon&&!chartOption.progressbar.iconUrl" src="@/assets/images/paigangicon.png" alt=""/>
                  <ul class="jindutext_con" :style="jindutextFlexStyle">
                    <li class="jindutext_li">{{col.name}}</li>
                    <li class="jindutext_li" v-if="col.status" :style="getStatusStyle(col)">{{col.status}}</li>
                  </ul>
                </div>
              </div>
              <div class="block_ul" :style="blockStyle">
                <template v-for="(row, rowindex) in col.list">
                  <div :style="getliTextColor(col)" class="block_li" :key="rowindex+','+colindex">{{row}}</div>
                </template>
              </div>
            </div>
          </div>
        </template>
      </div>
    </SeamlessScroll>
    <div class="con_con" ref="text" v-else :style="{...containerBgStyle,'flex-direction': chartOption.loopDirection&&chartOption.loopDirection=='2'?'column':'row','align-content':!chartOption.isRoll&&chartOption.blockalignContent?chartOption.blockalignContent:''}">
      <template v-for="(col, colindex) in rows">
        <div :style="block_conStyle" :key="colindex" class="block_con_con">
          <div :class="animate" class="block_con" :style="getBlockStyle(col)">
            <div class="jindu_con" :style="getBarConStyle(col)">
              <div class="jindu" :style="getBarStyle(col)"></div>
              <div class="jindutext">
                <img v-if="!chartOption.ishideIcon&&chartOption.progressbar.iconUrl" :src="chartOption.progressbar.iconUrl" alt=""/>
                <img v-if="!chartOption.ishideIcon&&!chartOption.progressbar.iconUrl" src="@/assets/images/paigangicon.png" alt=""/>
                <ul class="jindutext_con" :style="jindutextFlexStyle">
                  <li class="jindutext_li">{{col.name}}</li>
                  <li class="jindutext_li" v-if="col.status" :style="getStatusStyle(col)">{{col.status}}</li>
                </ul>
              </div>
            </div>
            <div class="block_ul" :style="blockStyle">
              <template v-for="(row, rowindex) in col.list">
                <div :style="getliTextColor(col,rowindex)" class="block_li" :key="rowindex+','+colindex">{{row}}</div>
              </template>
            </div>
          </div>
        </div>
      </template>
    </div>
  </div>
</template>
  
  <script>
  import "../../animate/animate.css";
  import dataChart from '../mixins/dataChart.js'
  import {objectPaigangBlockDataHandle} from '../../util/commonChartChange'
  import SeamlessScroll from 'vue-seamless-scroll'
  var dayjs = require('@/utils/day.js')
  export default {
    mixins: [dataChart],
    components: {
      SeamlessScroll,
    },
    props: {
      className: {
        type: String,
        default: "chart"
      },
      width: {
        type: String,
        default: "100%"
      },
      height: {
        type: String,
        default: "100%"
      },
      drawingList: {
        type: Array
      }
    },
    data() {
      return {
        rows: [],
        colorheight: "",
        colorwidth: "",
        animate: this.className
      };
    },
    watch: {
      width() {},
      height() {},
      className: {
        handler(value) {
          this.animate = value;
        }
      }
    },
    mounted() {
      this.valUpdate = this.setChartVal;
    },
    beforeDestroy() {
      
    },
    computed: {
      containerBgStyle(){
        //背景图片的样式
        let styleobj={}
        if(this.chartOption.containerBgImage){
          styleobj.backgroundColor='transparent'
          styleobj.backgroundImage = `url(${this.chartOption.containerBgImage}) `;
          styleobj.backgroundSize = "100% 100%";
          styleobj.backgroundRepeat = "no-repeat";
        }else{
          if(this.chartOption.containerBgColor){
            styleobj.background=this.chartOption.containerBgColor
          }
        }
        if(this.chartOption.containerRadius){
          styleobj.borderRadius=this.chartOption.containerRadius+'px'
        }
        return styleobj
      },
      jindutextFlexStyle(){
        if(this.chartOption.progressbar.isshowstatus){
          let style={
            'flex-direction': this.chartOption.progressbar.statusflexDirection?this.chartOption.progressbar.statusflexDirection:'row',
            'justify-content': this.chartOption.progressbar.barStatusalign?this.chartOption.progressbar.barStatusalign:'flex-start',
            'align-items': this.chartOption.progressbar.statusAlignItems?this.chartOption.progressbar.statusAlignItems:'center',
          }
          return style
        }
      },
      loopSeep(){
        if(this.chartOption.rollSpeed!=undefined){
          return this.chartOption.rollSpeed
        }else{
          return 1
        }
      },
      loopDirection(){
        if(this.chartOption.rollDirection!=undefined){
          return this.chartOption.rollDirection
        }else{
          return 1
        }
      },
      block_conStyle(){
        let style = {
          width:this.chartOption.blockwidth + "%",
          height:this.chartOption.isRoll?'auto':this.chartOption.blockheight + "%",
          'padding-top': this.chartOption.marginTop+'px',
          'padding-bottom': this.chartOption.marginTop+'px',
          'padding-left': this.chartOption.marginLeft+'px',
          'padding-right': this.chartOption.marginLeft+'px',
        };
        return style;
      },
      blockStyle(){
        let style = {
          justifyContent: this.chartOption.align,
          'align-content': 'flex-start',
          'margin-top': this.chartOption.liText.marginTop+'px',
          'margin-bottom': this.chartOption.liText.marginTop+'px',
          'padding-left': this.chartOption.liText.marginLeft+'px',
          'padding-right': this.chartOption.liText.marginLeft+'px',
          borderRadius: this.chartOption.bradius+'px',
          'height':this.chartOption.isRoll?'auto':'calc(100% - '+this.chartOption.progressbar.height+'%)'
        };
        return style;
      },
    },
    methods: {
      setVW(range) {
        //设置vw值
        if (range) {
          return (range / window.innerWidth) * 100 + "vw";
        }
      },
      getliTextColor(col,rowindex) {//内容样式
        let textColor='#ffffff'
        let rowtextAlign=''
        let rowfontSize=''
        let rowfontFamily=''
        let rowfontWeight=''
        let rowminWidtht=''
        let rowheight=''
        let textBgColor=''
        let paddingTop=0
        let paddingRight=0
        let paddingBottom=0
        let paddingLeft=0
        let marginTop=0
        let marginRight=0
        let marginBottom=0
        let marginLeft=0
        let rowborderRadio=0
        let borderTop=''
        let borderRight=''
        let borderBottom=''
        let borderLeft=''
        if(this.chartOption.liText.colorType&&this.chartOption.liText.colorType==2){
          if(col.listTextstatus!==undefined&&col.listTextstatus!==null&&this.chartOption.liText.color&&this.chartOption.liText.color.length>0){
            let statusval=this.chartOption.liText.color.find(rw=>col.listTextstatus==rw.value)
            if(statusval){
              textColor= statusval.color
            }
          }
        }else{
          textColor=this.chartOption.liText.color
        }
        if(this.chartOption.liTextStyleList&&this.chartOption.liTextStyleList.length>0){
          let activeLi=null
          activeLi=this.chartOption.liTextStyleList.find(row=>row.rowFiledIndex==rowindex||row.type&&row.type==2&&row.rowFiledIndex.includes(rowindex))
          if(activeLi){
            if(activeLi.paddingTop){
              paddingTop=activeLi.paddingTop
            }
            if(activeLi.paddingRight){
              paddingRight=activeLi.paddingRight
            }
            if(activeLi.paddingBottom){
              paddingBottom=activeLi.paddingBottom
            }
            if(activeLi.paddingLeft){
              paddingLeft=activeLi.paddingLeft
            }
            if(activeLi.marginTop){
              marginTop=activeLi.marginTop
            }
            if(activeLi.marginRight){
              marginRight=activeLi.marginRight
            }
            if(activeLi.marginBottom){
              marginBottom=activeLi.marginBottom
            }
            if(activeLi.marginLeft){
              marginLeft=activeLi.marginLeft
            }
            if(activeLi.borderRadio){
              rowborderRadio=activeLi.borderRadio
            }
            if(activeLi.lineHeight){
              rowheight=activeLi.lineHeight
            }
            if(activeLi.minwidth){
              rowminWidtht=activeLi.minwidth
            }
            if(activeLi.fontSize){
              rowfontSize=activeLi.fontSize
            }
            let activeStatusStyle=null
            if(activeLi.styleType&&activeLi.styleType==2){
              if(col.listTextstatus!==undefined&&col.listTextstatus!==null){
                let statusval=activeLi.styleStatusList.find(rw=>col.listTextstatus==rw.value)
                if(statusval){
                  activeStatusStyle=this.chartOption.allStyleList[statusval.index]
                }
              }
            }else{
              activeStatusStyle=this.chartOption.allStyleList[activeLi.styleStatusIndex]
            }
            if(activeStatusStyle){
              rowtextAlign=activeStatusStyle.isLeft
              textColor=activeStatusStyle.textColor
              rowfontFamily=activeStatusStyle.family
              rowfontWeight=activeStatusStyle.weight
              textBgColor=activeStatusStyle.textBgColor
            }
            if(activeLi.borderTop){
              borderTop=activeLi.borderTop.width+'px '+activeLi.borderTop.type+' '+activeLi.borderTop.color
            }
            if(activeLi.borderRight){
              borderRight=activeLi.borderRight.width+'px '+activeLi.borderRight.type+' '+activeLi.borderRight.color
            }
            if(activeLi.borderBottom){
              borderBottom=activeLi.borderBottom.width+'px '+activeLi.borderBottom.type+' '+activeLi.borderBottom.color
            }
            if(activeLi.borderLeft){
              borderLeft=activeLi.borderLeft.width+'px '+activeLi.borderLeft.type+' '+activeLi.borderLeft.color
            }
          }
        }
        let style = {
          'border-top':borderTop,
          'border-right':borderRight,
          'border-bottom':borderBottom,
          'border-left':borderLeft,
          borderRadius:rowborderRadio+ "px",
          'padding-top':paddingTop?paddingTop+ "%":'',
          'padding-right':paddingRight?paddingRight+ "%":'',
          'padding-bottom':paddingBottom?paddingBottom+ "%":'',
          'padding-left':paddingLeft?paddingLeft+ "%":'',
          'margin-top':marginTop?marginTop+ "%":'',
          'margin-right':marginRight?marginRight+ "%":'',
          'margin-bottom':marginBottom?marginBottom+ "%":'',
          'margin-left':marginLeft?marginLeft+ "%":'',
          background:textBgColor?textBgColor:'',
          textAlign: this.chartOption.liText.left,
          fontSize: rowfontSize?rowfontSize + "px":this.chartOption.liText.size + "px",
          fontFamily: rowfontFamily?rowfontFamily:this.chartOption.liText.family,
          fontWeight: rowfontWeight?rowfontWeight:this.chartOption.liText.weight,
          color: textColor,//内容文本颜色
          '--left-textalign': rowtextAlign?rowtextAlign:this.chartOption.liText.lefttext,
          '--right-textalign': rowtextAlign?rowtextAlign:this.chartOption.liText.righttext,
          'min-width': rowminWidtht?rowminWidtht+'%':this.chartOption.liText.minwidth+'%',
          'height': this.chartOption.isRoll?(rowheight?rowheight + "px":this.chartOption.liText.lineHeight + "px"):(rowheight?rowheight + "%":this.chartOption.liText.lineHeight + "%"),
        };
        if(this.chartOption.liText.isOverHide){
          style['max-width']='100%';
          style['overflow']='hidden';
          if(this.chartOption.liText.isShowEllipsis){
            style['text-overflow']='ellipsis';
          }
          
        }
        return style;
      },
      getBarConStyle(col){//进度条容器样式
        let bgJianbianVal=[]
        if(this.chartOption.progressbar.conBgJianbianList&&this.chartOption.progressbar.conBgJianbianList.length>0){
          if(col.jindubgstatus!==undefined&&col.jindubgstatus!==null&&this.chartOption.progressbar.conBgJianbian&&this.chartOption.progressbar.conBgJianbian.length>0){
            let statusval=this.chartOption.progressbar.conBgJianbian.find(rw=>col.jindubgstatus==rw.value)
            if(statusval){
              bgJianbianVal=this.chartOption.progressbar.conBgJianbianList[statusval.index]
            }else{
              bgJianbianVal=this.chartOption.progressbar.conBgJianbianList[0]
            }
          }else{
            bgJianbianVal=this.chartOption.progressbar.conBgJianbianList[0]
          }
        }else{
          bgJianbianVal=this.chartOption.progressbar.conBgJianbian
        }
        let bgSrt=''
        if(bgJianbianVal&&bgJianbianVal.length>1){
          bgSrt='linear-gradient(90deg, '
          for(let inx=0;inx<bgJianbianVal.length;inx++){
            let row=bgJianbianVal[inx]
            if(inx==bgJianbianVal.length-1){
              bgSrt+=row.color+' '+row.percentage+'%'
            }else{
              bgSrt+=row.color+' '+row.percentage+'%, '
            }
          }
          bgSrt+=')'
        }else if(bgJianbianVal&&bgJianbianVal.length==1){
          bgSrt=bgJianbianVal[0].color
        }
        let style = {
          '--background':bgSrt,
          color:this.getBarTextColor(col),
          fontSize: this.chartOption.progressbar.size + "px",
          fontFamily: this.chartOption.progressbar.family,
          fontWeight: this.chartOption.progressbar.weight,
          borderRadius:this.chartOption.progressbar.conRadius+ "px",
          height:this.chartOption.isRoll?this.chartOption.progressbar.height + "px":this.chartOption.progressbar.height + "%",
          'padding-top':this.chartOption.progressbar.paddingTop?this.chartOption.progressbar.paddingTop+ "%":'',
          'padding-right':this.chartOption.progressbar.paddingRight?this.chartOption.progressbar.paddingRight+ "%":'',
          'padding-bottom':this.chartOption.progressbar.paddingBottom?this.chartOption.progressbar.paddingBottom+ "%":'',
          'padding-left':this.chartOption.progressbar.paddingLeft?this.chartOption.progressbar.paddingLeft+ "%":'',
        };
        if(this.chartOption.progressbar.baralign){
          style.justifyContent=this.chartOption.progressbar.baralign
        }
        if(this.chartOption.progressbar.alignItems){
          style.alignItems=this.chartOption.progressbar.alignItems
        }
        return style;
      },
      getBarTextColor(col){
        //设置进度条字体颜色
        let textColor='#ffffff'
        if(this.chartOption.progressbar.colorType&&this.chartOption.progressbar.colorType==2){
          if(col.jinduTextstatus!==undefined&&col.jinduTextstatus!==null&&this.chartOption.progressbar.color&&this.chartOption.progressbar.color.length>0){
            let statusval=this.chartOption.progressbar.color.find(rw=>col.jinduTextstatus==rw.value)
            if(statusval){
              textColor= statusval.color
            }
          }
        }else{
          textColor= this.chartOption.progressbar.color
        }
        return textColor
      },
      getBarStatusTextColor(col){
        //设置进度条状态字体颜色
        let textColor='#ffffff'
        if(this.chartOption.progressbar.statuscolorType&&this.chartOption.progressbar.statuscolorType==2){
          if(col.statusColor!==undefined&&col.statusColor!==null&&this.chartOption.progressbar.statusFontColor&&this.chartOption.progressbar.statusFontColor.length>0){
            let statusval=this.chartOption.progressbar.statusFontColor.find(rw=>col.statusColor==rw.value)
            if(statusval){
              textColor= statusval.color
            }
          }
        }else{
          textColor= this.chartOption.progressbar.statusFontColor
        }
        return textColor
      },
      getBarStatusBorder(col){
        //设置进度条状态边框设置
        let textColor='#E74032'
        if(this.chartOption.progressbar.statusBorderShowType&&this.chartOption.progressbar.statusBorderShowType==2){
          if(col.statusBorder!==undefined&&col.statusBorder!==null&&this.chartOption.progressbar.statusBorderColor&&this.chartOption.progressbar.statusBorderColor.length>0){
            let statusval=this.chartOption.progressbar.statusBorderColor.find(rw=>col.statusBorder==rw.value)
            if(statusval){
              textColor= statusval.color
            }
          }
        }else{
          textColor= this.chartOption.progressbar.statusBorderColor
        }
        let valStr=this.chartOption.progressbar.statusBorderWidth+'px'+' '+this.chartOption.progressbar.statusBorderType+' '+textColor
        return valStr
      },
      getStatusStyle(col){
        //进度条状态设置
        if(this.chartOption.progressbar.isshowstatus){
          let bgJianbianVal=[]
          if(this.chartOption.progressbar.statusbgJianbianlist&&this.chartOption.progressbar.statusbgJianbianlist.length>0){
            if(col.statusBg!==undefined&&col.statusBg!==null&&this.chartOption.progressbar.statusbgJianbian&&this.chartOption.progressbar.statusbgJianbian.length>0){
              let statusval=this.chartOption.progressbar.statusbgJianbian.find(rw=>col.statusBg==rw.value)
              if(statusval){
                bgJianbianVal=this.chartOption.progressbar.statusbgJianbianlist[statusval.index]
              }else{
                bgJianbianVal=this.chartOption.progressbar.statusbgJianbianlist[0]
              }
            }else{
              bgJianbianVal=this.chartOption.progressbar.statusbgJianbianlist[0]
            }
          }else{
            bgJianbianVal=this.chartOption.progressbar.statusbgJianbian
          }
          let bgSrt=''
          if(bgJianbianVal&&bgJianbianVal.length>1){
            bgSrt='linear-gradient(90deg, '
            for(let inx=0;inx<bgJianbianVal.length;inx++){
              let row=bgJianbianVal[inx]
              if(inx==bgJianbianVal.length-1){
                bgSrt+=row.color+' '+row.percentage+'%'
              }else{
                bgSrt+=row.color+' '+row.percentage+'%, '
              }
            }
            bgSrt+=')'
          }else if(bgJianbianVal&&bgJianbianVal.length==1){
            bgSrt=bgJianbianVal[0].color
          }
          let style = {
            'background':bgSrt,
            color:this.getBarStatusTextColor(col),
            border:this.chartOption.progressbar.isshowstatusBorder?this.getBarStatusBorder(col):'',
            fontSize: this.chartOption.progressbar.statusFontSize + "px",
            fontFamily: this.chartOption.progressbar.statusfamily,
            fontWeight: this.chartOption.progressbar.statusweight,
            borderRadius:this.chartOption.progressbar.statusconRadius+ "px",
            'padding-top':this.chartOption.progressbar.statuspaddingTop?this.chartOption.progressbar.statuspaddingTop+ "px":'',
            'padding-right':this.chartOption.progressbar.statuspaddingRight?this.chartOption.progressbar.statuspaddingRight+ "px":'',
            'padding-bottom':this.chartOption.progressbar.statuspaddingBottom?this.chartOption.progressbar.statuspaddingBottom+ "px":'',
            'padding-left':this.chartOption.progressbar.statuspaddingLeft?this.chartOption.progressbar.statuspaddingLeft+ "px":'',
            'margin-top':this.chartOption.progressbar.statusmarginTop?this.chartOption.progressbar.statusmarginTop+ "px":'',
            'margin-right':this.chartOption.progressbar.statusmarginRight?this.chartOption.progressbar.statusmarginRight+ "px":'',
            'margin-bottom':this.chartOption.progressbar.statusmarginBottom?this.chartOption.progressbar.statusmarginBottom+ "px":'',
            'margin-left':this.chartOption.progressbar.statusmarginLeft?this.chartOption.progressbar.statusmarginLeft+ "px":'',
          };
          return style
        }else{
          return ''
        }
        
      },
      getBarStyle(col){//进度条样式
        let bgJianbianVal=[]
        if(this.chartOption.progressbar.bgJianbianlist&&this.chartOption.progressbar.bgJianbianlist.length>0){
          if(col.jindubarstatus!==undefined&&col.jindubarstatus!==null&&this.chartOption.progressbar.bgJianbian&&this.chartOption.progressbar.bgJianbian.length>0){
            let statusval=this.chartOption.progressbar.bgJianbian.find(rw=>col.jindubarstatus==rw.value)
            if(statusval){
              bgJianbianVal=this.chartOption.progressbar.bgJianbianlist[statusval.index]
            }else{
              bgJianbianVal=this.chartOption.progressbar.bgJianbianlist[0]
            }
          }else{
            bgJianbianVal=this.chartOption.progressbar.bgJianbianlist[0]
          }
        }else{
          bgJianbianVal=this.chartOption.progressbar.bgJianbian
        }
        let bgSrt=''
        if(bgJianbianVal&&bgJianbianVal.length>1){
          bgSrt='linear-gradient(90deg, '
          for(let inx=0;inx<bgJianbianVal.length;inx++){
            let row=bgJianbianVal[inx]
            if(inx==bgJianbianVal.length-1){
              bgSrt+=row.color+' '+row.percentage+'%'
            }else{
              bgSrt+=row.color+' '+row.percentage+'%, '
            }
          }
          bgSrt+=')'
        }else if(bgJianbianVal&&bgJianbianVal.length==1){
          bgSrt=bgJianbianVal[0].color
        }
        let style = {
          'background':bgSrt,
          borderRadius:this.chartOption.progressbar.radius+ "px",
        };
        let barSize=null
        if(col.jindutype&&col.jindutype==2){
          if(col.barTotalValue<=col.barNow){
            barSize=100
          }else{
            barSize=Number(col.barNow)/Number(col.barTotalValue)*100
          }
          
        }else{
          if(col.barStart&&col.barEnd){
            if(dayjs(col.barStart).isValid()&&dayjs(col.barEnd).isValid()){
              let nowtime=new Date()
              if(col.barNow){
                nowtime=col.barNow
              }
              if(dayjs(col.barEnd).isBefore(dayjs(nowtime))||dayjs(col.barEnd).isSame(dayjs(nowtime))){
                barSize=100
              }else{
                let totalData=dayjs(col.barEnd).diff(dayjs(col.barStart))
                let totalData2=dayjs(nowtime).diff(dayjs(col.barStart))
                barSize=Number(totalData2)/Number(totalData)
              }
            }else{
              if(col.barEnd<=col.barNow){
                barSize=100
              }else{
                let totalData=Number(col.barEnd)-Number(col.barStart)
                let totalData2=Number(nowtime)-Number(col.barStart)
                if(totalData2){
                  barSize=totalData2/totalData*100
                }else{
                  barSize=0
                }
                
              }
            }
            
          }
        }
        
        if(barSize){
          style={
            'background':bgSrt,
            borderRadius:this.chartOption.progressbar.radius+ "px",
            width:barSize+'%'
          }
        }else{
          style={
            'background':bgSrt,
            borderRadius:this.chartOption.progressbar.radius+ "px",
            width:'0%'
          }
        }
        return style
      },
      getBlockStyle(col){//整体内容样式
        let bgobj=null
        if(this.chartOption.blockBgColorType!==undefined){
          if(this.chartOption.blockBgColorType==1){
            bgobj={}
            bgobj.color=this.chartOption.blockBgColor
            bgobj.image=this.chartOption.blockBgColor
          }else if(this.chartOption.blockBgColorType==2){
            if(col.bgstatus!=undefined&&col.bgstatus!=null){
              bgobj=this.chartOption.blockBgColor.find(rw=>col.bgstatus==rw.value)
            }
          }
        }else{
          if(this.chartOption.blockBgColor&&this.chartOption.blockBgColor.length>0){
            if(col.bgstatus!=undefined&&col.bgstatus!=null){
              bgobj=this.chartOption.blockBgColor.find(rw=>col.bgstatus==rw.value)
            }
          }
        }
        let borderobj=null
        let borderString=null
        if(this.chartOption.isetBorder&&this.chartOption.blockBorderColorType!==undefined){
          if(this.chartOption.blockBorderColorType==1){
            if(this.chartOption.borderInfoVallist&&this.chartOption.borderInfoVallist.length>0){
              borderobj=this.chartOption.borderInfolist[this.chartOption.borderInfoVallist[0].sort]
            }
            
          }else if(this.chartOption.blockBorderColorType==2){
            if(col.borderstatus!=undefined&&col.borderstatus!=null&&this.chartOption.borderInfoVallist&&this.chartOption.borderInfoVallist.length>0){
              let valobj=this.chartOption.borderInfoVallist.find(rw=>col.borderstatus==rw.value)
              if(valobj){
                borderobj=this.chartOption.borderInfolist[valobj.sort]
              }
            }
          }
          if(borderobj){
            borderString=borderobj.width+'px '+borderobj.type+' '+borderobj.color
          }
        }
        if(bgobj){
          if(this.chartOption.blockBgType&&this.chartOption.blockBgType=='1'){
            let styleobj={}
            styleobj.backgroundColor='transparent'
            styleobj.backgroundImage = `url(${bgobj.image}) `;
            styleobj.backgroundSize = "100% 100%";
            styleobj.backgroundRepeat = "no-repeat";
            styleobj.height = this.chartOption.isRoll?'auto':'100%';
            if(borderString){
              styleobj.border=borderString
            }
            return styleobj
          }
          return {'background':bgobj.color,'height':this.chartOption.isRoll?'auto':'100%',border:borderString}
        }else{
          return {'height':this.chartOption.isRoll?'auto':'100%',border:borderString}
        }
      },
      setChartVal(resData,rowGlobal) {
        let result=[]
        if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
          result =objectPaigangBlockDataHandle(rowGlobal,this.chartOption)?objectPaigangBlockDataHandle(rowGlobal,this.chartOption):[]
        }else{
          result = this.chartOption.staticDataValue;
        }
        let dataOption = JSON.parse(JSON.stringify(this.dataOption))
        //根据千位分隔符格式化数字
        if (dataOption.isThousand != "false") {
          result.map(element => {
            element.value = this.thousandDevide(
              element.value,
              dataOption.isThousand
            );
          });
        }
        this.rows = result;
        this.$forceUpdate()
      },
      thousandDevide(data, devide) {
        //格式化数字
        if (typeof data != "number") {
          return data;
        } else {
          if (!devide) {
            devide = ",";
          }
          var res = data.toString().replace(/\d+/, function(n) {
            // 先提取整数部分
            return n.replace(/(\d)(?=(\d{3})+$)/g, function($1) {
              return $1 + devide;
            });
          });
          return res;
        }
      },
     
    }
  };
  </script>
  <style lang="scss" scoped>
  ul{
    padding: 0;
    margin: 0;
    li {
            list-style: none;
        }
  }
  .con_con{
    display: flex;
    justify-content: flex-start;
    flex-wrap: wrap;
    height: 100%;
    
    // overflow: hidden;
  }
  .color:hover {
    cursor: default;
  }
  .block_con_con{
    box-sizing: border-box;
  }
  .block_con{
    --left-textalign: left;
    --right-textalign: right;
    // width: 180px;
    // height: 168px;
    background: rgba(16, 27, 120, 0.75);
    border-radius: 6px;
    .jindu_con{
      --background:linear-gradient( 90deg, #1E35BA 0%, rgba(30,53,186,0.5) 58%, rgba(30,53,186,0) 100%, rgba(10,2,43,0) 100%);
      width: 100%;
      height: 34px;
      display: flex;
      align-items: center;
      background: var(--background);
      color: rgba(255, 255, 255, 1);
      border-radius: 6px;
      position: relative;
      box-sizing: border-box;
      .jindu{
        display: flex;
        align-items: center;
        height: 100%;
        width: 80%;
        background: linear-gradient( 270deg, #1FF565 0%, #3CC7FF 100%);
        border-radius: 6px;
        
      }
      .jindutext{
        position: absolute;
        display: flex;
        align-items: center;
        width: 100%;
        .jindutext_con{
          display: flex;
          align-items: center;
        }
        img{
          width: 12px;
          height: 18px;
          margin-left: 10px;
          margin-right: 8px;
        }
      }
    }
    .block_ul{
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      flex-wrap: wrap;
      margin-top: 7px;
      padding: 0 12px;
      width: 100%;
      box-sizing: border-box;
      .block_li{
        // line-height: 28px;
        font-size: 14px;
        min-width: 48%;
        color: #ffffff;
        align-content: center;
        box-sizing: border-box;
      }
      .block_li:nth-child(2n+1){
        text-align: var(--left-textalign);
        // padding-left: 12px;
      }
      .block_li:nth-child(2n){
        text-align: var(--right-textalign);
        // padding-right: 12px;
      }
    }
  }
  </style>
  