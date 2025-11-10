<template>
  <div :class="animate" :id="chartOption.bindingDiv">
    <div v-if="this.chartOption.rotation == false" :style="{ height: height, width: width, overflow: 'hidden' }">
      <img v-if="this.chartOption.enlarge == true" @click="onPreview(imgURL)" :style="{ opacity: opacity, 'object-fit': fit }"
        class="autoImg" :src="imgURL" alt="" ref="img">
      <img :style="{ 'object-fit': fit }" class="autoImg" :src="imgURL" alt="" v-else ref="img">
    </div>

    <div id="grid" :style="{ height: height, width: width }" v-if="this.chartOption.rotation == true">
      <a class="card" href="#" ref="card" @mousemove="move" @mouseleave="leave" @mouseover="over">
        <div class="reflection" ref="refl"></div>
        <img :src="imgURL" :style="{ opacity: opacity, 'object-fit': fit }" ref="img" />
      </a>
    </div>
  </div>
</template>

<script>
import '../../animate/animate.css'
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
      fit: this.chartOption.fit,
      imgURL: "",
      opacity: this.chartOption.opacity,
      animate: this.className
    };
  },
  watch: {
    width() {

    },
    height() {

    },
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
  methods: {
    onPreview(file) {
      this.$openPreview([file]);
    },
    setChartVal(result) {
      this.imgURL = result;
      this.opacity = (this.dataOption.opacity).toString();
      this.fit = this.dataOption.fit;
    },
    over() {
      const refl = this.$refs.refl;
      refl.style.opacity = 1;
    },
    leave() {
      const card = this.$refs.card;
      const refl = this.$refs.refl;
      card.style.transform = `perspective(500px) scale(1)`;
      refl.style.opacity = 0;
    },

    move() {
      const card = this.$refs.card;
      const refl = this.$refs.refl;

      const relX = (event.offsetX + 1) / card.offsetWidth;
      const relY = (event.offsetY + 1) / card.offsetHeight;
      const rotY = `rotateY(${(relX - 0.5) * 60}deg)`;
      const rotX = `rotateX(${(relY - 0.5) * -60}deg)`;
      card.style.transform = `perspective(500px) scale(1.2) ${rotY} ${rotX}`;

      const lightX = this.scale(relX, 0, 1, 150, -50);
      const lightY = this.scale(relY, 0, 1, 30, -100);
      const lightConstrain = Math.min(Math.max(relY, 0.3), 0.7);
      const lightOpacity = this.scale(lightConstrain, 0.3, 1, 1, 0) * 255;
      const lightShade = `rgba(${lightOpacity}, ${lightOpacity}, ${lightOpacity}, 1)`;
      const lightShadeBlack = `rgba(0, 0, 0, 1)`;
      refl.style.backgroundImage = `radial-gradient(circle at ${lightX}% ${lightY}%, ${lightShade} 20%, ${lightShadeBlack})`;
    },
    scale: (val, inMin, inMax, outMin, outMax) =>
      outMin + (val - inMin) * (outMax - outMin) / (inMax - inMin)
  },
};
</script>

<style lang="scss" scoped>
#grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, 100%);
  grid-column-gap: 30px;
  grid-row-gap: 30px;
  align-items: center;
  justify-content: center;
}

#grid .card {
  background-color: #ccc;
  width: 100%;
  height: 100%;
  transition: all 0.1s ease;
  border-radius: 3px;
  position: relative;
  z-index: 1;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0);
  overflow: hidden;
  cursor: pointer;
}

#grid .card:hover {
  -webkit-transform: scale(1.2);
  transform: scale(1.2);
  z-index: 2;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.4);
}

#grid .card:hover img {
  -webkit-filter: grayscale(0);
  filter: grayscale(0);
}

#grid .card .reflection {
  position: absolute;
  width: 100%;
  height: 100%;
  z-index: 2;
  left: 0;
  top: 0;
  transition: all 0.1s ease;
  opacity: 0;
  mix-blend-mode: soft-light;
}

#grid .card img {
  width: 100%;
  height: 100%;
  -webkit-filter: grayscale(0.65);
  filter: grayscale(0.65);
  transition: all 0.3s ease;
}

.autoImg {
  width: 100%;
  height: 100%;
}
</style>
