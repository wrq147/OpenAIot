<template>
  <div :id="bindingDiv" :class="className" :style="{ height: height, width: width }" />
</template>

<script>

import '../../animate/animate.css'
import {parseQueryString, fIsUrL} from '../../util/urlUtil'
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },

  },
  data() {
    return {
      bindingDiv: "tiandiMap" + this.chartOption.bindingDiv
    };
  },
  watch: {
    width() {
    },
    height() {
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  methods: {
    setChartVal(result) {
      
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))

      const a = new Promise((resolve, reject) => {
        console.log(reject);
        // 如果已加载直接返回
        if (window.T) {
            console.log('地图脚本初始化成功...');
            resolve(window.T);
        }
        });
        // 第一种方式显示
        // a.then((T) => {
        //   const imageURL = 'http://t0.tianditu.gov.cn/img_c/wmts?tk=5228a6fb6f451e191672532de0a03ad6';
        //   const lay = new T.TileLayer(imageURL, { minZoom: 1, maxZoom: 18 });
        //   const config = { layers: [lay], name: 'TMAP_SATELLITE_MAP' };
        //   const map = new T.Map('yzMap', config);
        //   const ctrl = new T.Control.MapType();
        //   map.addControl(ctrl);
        //   const map = new T.Map('yzMap');
        //   map.centerAndZoom(new T.LngLat(116.401003, 39.903117), 12);
        // }).catch();
        // 第二种方式显示
          this.chart = new window.T.Map(this.bindingDiv);
          this.chart.centerAndZoom(new window.T.LngLat(116.401003, 39.903117), 12);
        
        console.log(a);
        //允许鼠标滚轮缩放地图
        this.chart.enableScrollWheelZoom();
        //创建比例尺控件对象
        var scale = new T.Control.Scale();
        //添加比例尺控件
        this.chart.addControl(scale);
        var miniMap = new T.Control.OverviewMap({
            isOpen: true,
            size: new T.Point(150, 150)
        });
        console.log(dataOption.isHawkEye);
        if(dataOption.isHawkEye) { //是否显示鹰眼
          this.chart.addControl(miniMap);
        }
       
        if(dataOption.isMapType) { //是否显示地图类型控件
          //创建对象
          var ctrl = new T.Control.MapType();
          //添加控件
          this.chart.addControl(ctrl);
        }

        if(dataOption.isBounds) { //是否显示标注
          // 随机向地图添加25个标注
          var bounds = this.chart.getBounds();
          var sw = bounds.getSouthWest();
          var ne = bounds.getNorthEast();
          var lngSpan = Math.abs(sw.lng - ne.lng);
          var latSpan = Math.abs(ne.lat - sw.lat);
          for (var i = 0; i < 25; i++) {
              var point = new T.LngLat(sw.lng + lngSpan * (Math.random() * 0.7), ne.lat - latSpan * (Math.random() * 0.7));
              var marker = new T.Marker(point);// 创建标注
              this.chart.addOverLay(marker);
          }
        }

    }
  }

}
</script>

<style>

</style>