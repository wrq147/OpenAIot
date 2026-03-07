<template>
  <view style="width: 100%;line-height: initial;" :style="{'border-top':disabled?'1rpx solid rgba(255, 255, 255, 0.2)':'none'}">
	<text class="form_text" :style="{'color': color,'width': '100%',}" :class="{'dis_text':disabled}">
		<text class="iconfont #icon-icon icon-baojing"
			:style="{'color':color,'font-size':'32rpx','margin-right': '8rpx','width': '100%'}"></text>{{placeholder}}
	</text>
  </view>

</template>

<script>
import componentMinxins from '../ComponentMinxins'

export default {
  mixins: [componentMinxins],
  name: "Description",
  components: {},
  props: {
    color:{
      type: String,
      default: '#868686'
    },
    placeholder: {
      type: String,
      default: '只是一段说明文字'
    },
	disabled: {
		default: false,
		type: Boolean
	},
  },
  data() {
    return {}
  },
  methods: {}
}
</script>

<style scoped>

</style>
