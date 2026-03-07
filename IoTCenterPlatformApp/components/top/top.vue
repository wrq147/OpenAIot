<template>
	<view>
		<uni-nav-bar :hasSeat="hasSeat" :seatHeight="seatHeight" :isLeftSlot="isLeftSlot" :isCenterSlot="isCenterSlot" :isRightSlot="isRightSlot" :statusBar="statusBar" :isCustomize="true" :isCustom="true" :fixed="fixed" :leftIcon="leftIcon"
			:leftText="leftText" :backgroundColor="backgroundColor" :color="color" :border="false"
			:leftFontSize="leftFontSize" :rightIcon="rightIcon" :rightText="rightText" :title="title"
			:leftWidth="leftWidth" :rightWidth="rightWidth" :titleIsLeft="titleIsLeft"
			:isNoLeftPadding="isNoLeftPadding" height="88rpx" @clickLeft="clickLeft" @clickRight="clickRight"
			:zIndex="996" :dark="false" :iconSize="iconSize" @comfirmClick="comfirmClick" :isShowAbs="isShowAbs"
			:absList="absList" :topopacity="topopacity" :rightIconColor="rightIconColor" :rightTextColor="rightTextColor" @clickTitle="clickTitle">
			<template v-slot:left v-if="isLeftSlot">
				<slot name="top_left">
				</slot>
			</template>
			<template v-if="isCenterSlot">
				<slot name="top_center">
				</slot>
			</template>
			<template v-slot:right v-if="isRightSlot">
				<slot name="top_right">
				</slot>
			</template>
		</uni-nav-bar>
	</view>
</template>

<script>
	export default {
		name: "top",
		props: {
			seatHeight:{
				type: String,
				default: "0rpx"
			},
			hasSeat:{
				type: Boolean,
				default: false
			},
			isLeftSlot:{
				type: Boolean,
				default: false
			},
			isRightSlot:{
				type: Boolean,
				default: false
			},
			isCenterSlot:{
				type: Boolean,
				default: false
			},
			title: {
				type: String,
				default: ""
			},
			leftText: {
				type: String,
				default: ""
			},
			rightText: {
				type: String,
				default: ""
			},
			leftIcon: {
				type: String,
				default: ""
			},
			rightIcon: {
				type: String,
				default: ""
			},
			fixed: {
				type: [Boolean, String],
				default: true
			},
			color: {
				type: String,
				default: "rgba(51, 51, 51, 1)"
			},
			backgroundColor: {
				type: String,
				default: ""
			},
			statusBar: {
				type: [Boolean, String],
				default: true
			},
			leftFontSize: {
				type: String,
				default: '32rpx'
			},
			iconSize: {
				type: Number,
				default: 18
			},
			title: {
				type: String,
				default: ""
			},
			leftWidth: {
				type: [Number, String],
				default: 60
			},
			rightWidth: {
				type: [Number, String],
				default: 60
			},
			titleIsLeft: { //顶部标题是否左对齐
				type: [Boolean, String],
				default: false
			},
			isCustom: { //标题文字是否是自定义
				type: Boolean,
				default: true
			},
			isNoLeftPadding: { //标题中间是否没有左边的padding
				type: Boolean,
				default: false
			},
			isleftBack: { //左边是否是返回标签
				type: Boolean,
				default: false
			},
			isShowAbs: { //头部是否显示弹出定位
				type: Boolean,
				default: false
			},
			absList: {
				type: Array,
				default: () => {
					return []
				}
			},
			topopacity: {
				type: Number,
				default: 1
			},
			rightIconColor: {
				type: String,
				default: ''
			},
			rightTextColor: {
				type: String,
				default: "#999999"
			},
		},
		data() {
			return {
				isReturnHome: false, //是否是需要返回首页
			};
		},
		mounted() {
			this.getCurrenPage()
			// console.log("是否显示左侧插槽",this.isLeftSlot);
		},
		methods: {
			clickTitle(){
				this.$emit('clickTitle')
			},
			comfirmClick(row) {
				this.$emit('comfirmClick', row)
			},
			getCurrenPage() {
				//获取当前页面，查看是否有上一页
				let pages = getCurrentPages(); // 获取当前页面栈的实例
				let prevPage = pages[pages.length - 2]; //上一个页面
				let curPage = pages[pages.length - 1].route; //当前页面
				if (!prevPage || prevPage == undefined || prevPage == null) {
					this.isReturnHome = true
				}
			},
			clickLeft() {
				//点击左侧的单独的图标
				if (this.isleftBack) {
					if (this.isReturnHome) {
						uni.switchTab({
							url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
						})
						return
					}
					// #ifdef H5
					history.back();
					// #endif
					// #ifndef H5
					uni.navigateBack()
					// #endif
				} else {
					this.$emit('clickLeft')
				}
			},
			clickRight() {
				//点击右侧的单独的图标
				this.$emit('clickRight')
			}
		}
	}
</script>

<style>

</style>