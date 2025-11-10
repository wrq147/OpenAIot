<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="text"
    class="swiper-no-swiping"
  >
    <swiper ref="mySwiper" :key="key" :options="swiperOption">
			<swiper-slide v-for="(item, index) in banners" :key="index" :style="{ height: height, width: width }">
				<img :src="item.value" :style="{ height: height, width: width }" />
			</swiper-slide>
			<div class="swiper-pagination" slot="pagination"></div>
		</swiper>
  </div>
</template>

<script>

import { Swiper, SwiperSlide } from "vue-awesome-swiper";
import "swiper/css/swiper.css";
import "../../animate/animate.css";
import dataChart from '../mixins/dataChart.js'
export default {
  mixins: [dataChart],
  components: {
    Swiper,
    SwiperSlide
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
    }
  },
  data() {
    return {
      key: 1,
      banners: this.chartOption.staticDataValue,
      swiperOption: {
        preventClicksPropagation: false,
        //swiper3
        // 分页器配置
        pagination: {
          el: ".swiper-pagination",
          clickable: false,
          type: "bullets",
          hideOnClick: false
        },
        // 设定初始化时slide的索引
        initialSlide: 0,
        //Slides的滑动方向，可设置水平(horizontal)或垂直(vertical)
        direction: 'horizontal',
        // 自动切换图配置
        autoplay: {
          delay: 3000,
          stopOnLastSlide: false,
          disableOnInteraction: false
        },
        effect: "slide",
        speed: 800,
        // 箭头配置
        navigation: {
          nextEl: ".swiper-button-next",
          prevEl: ".swiper-button-prev"
        },
        // 环状轮播
        loop: true,
        loopedSlides: 3,
        loopAdditionalSlides: 0,
        // 一个屏幕展示的数量
        slidesPerView: 1,
        // 间距
        // spaceBetween: 26,
        // 修改swiper自己或子元素时，自动初始化swiper
        observer: true,
        // 修改swiper的父元素时，自动初始化swiper
        observeParents: true
      },
      animate: this.className
    };
  },
  watch: {
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
    if (!this.chart) {
      return;
    }
    this.chart = null;
  },
  computed: {},
  methods: {
    setChartVal(result) {
      this.banners = result;
      this.swiperOption.direction = this.dataOption.direction;
      this.swiperOption.autoplay.delay = parseInt(this.dataOption.playSpeed);
      this.swiperOption.effect = this.dataOption.isEffect;
      this.swiperOption.pagination.hideOnClick = this.dataOption.isPagination;
      this.swiperOption.pagination.type = this.dataOption.paginationType;
      this.$nextTick(() => {
        this.key++
      });
    },
  
  }
};
</script>

<style lang="scss" scoped>
.demo-input-suffix {
  display: flex;
  float: left;
}
</style>
