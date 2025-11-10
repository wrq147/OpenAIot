<!--
* @description 图片组件
* @filename RoyImage.vue
* @author ROYIANS
* @date 2022/12/6 14:49
!-->
<template>
    <div class="RoyQrcode">
      <StyledQrcode v-bind="style">
        <img :alt="element.title || 'RoyQrcode'" :src="codeSrc" />
      </StyledQrcode>
    </div>
  </template>
  
  <script>
  import commonMixin from '@/mixins/print/commonMixin'
  import { StyledQrcode } from '@/components/print/PageComponents/style'

  import { mapState } from 'vuex';
  /**
   * 图片组件
   */
  export default {
    name: 'RoyQrcode',
    mixins: [commonMixin],
    components: {
      StyledQrcode
    },
    props: {
      element: {
        type: Object,
        default: () => {}
      },
      propValue: {
        type: Object,
        default: () => {
          return {}
        }
      }
    },
    computed: {
      style() {
        return this.element.style || {}
      },
      ...mapState({
        dataSource: (state) => state.printTemplateModule.dataSource,
        dataSet: (state) => state.printTemplateModule.dataSet
      })
    },
    data() {
      return {
        codeSrc:''
      }
    },
    watch:{
        element:{
            handler(newVal){
                this.createCode(newVal)
            },
            deep:true,
            immediate:true
        }
    },
    methods: {
      initMounted() {
      },
      
    },
    created() {},
    mounted() {
      this.initMounted()
    },
  }
  </script>
  
  <style lang="scss">
  .RoyImage {
    height: 100%;
    padding: 0;
  }
  </style>
  