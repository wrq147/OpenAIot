<template>
	<view class="uni-navbar" :class="{'uni-dark':dark, 'uni-nvue-fixed': fixed}" :style="{'opacity':topopacity}">
		<view class="uni-navbar__content"
			:class="{ 'uni-navbar--fixed': fixed, 'uni-navbar--shadow': shadow, 'uni-navbar--border': border,'top_bg':isBg }"
			:style="{ 'background-color': themeBgColor, 'border-bottom-color':themeColor,'z-index':zIndex,height:navbarHeight }">
			<status-bar v-if="statusBar" :bgc="themeBgColor"/>
			<view :style="{'height':seatHeight,'width':'100%'}" v-if="hasSeat"></view>
			<slot name="allslot">
				<view :style="{ color: themeColor,backgroundColor: themeBgColor ,height:navbarHeight}" class="uni-navbar__header">
					<!--#ifndef MP-WEIXIN -->
					<view @tap="onClickLeft" class="uni-navbar__header-btns uni-navbar__header-btns-left"
						:style="{width:leftIconWidth}">
						<template v-if="isLeftSlot">
							<slot name="left">
								<view class="uni-navbar__content_view" v-if="leftIcon.length > 0&&!isCustomize">
									<uni-icons :color="themeColor" :type="leftIcon" size="40rpx" />
								</view>
								<view class="uni-navbar__content_view" v-if="leftIcon.length > 0&&isCustomize">
									<uni-icons custom-prefix="iconfont" :color="themeColor" :type="leftIcon"
										:size="iconSize*2+'rpx'"></uni-icons>
								</view>
								<view
									:class="{ 'uni-navbar-btn-icon-left': !leftIcon.length > 0 ,'customize_left':isCustomize}"
									class="uni-navbar-btn-text" v-if="leftText.length">
									<text :style="{ color: themeColor, fontSize: leftFontSize }">{{ leftText }}</text>
								</view>
							</slot>
						</template>
						<template v-else>
							<view class="uni-navbar__content_view" v-if="leftIcon.length > 0&&!isCustomize">
								<uni-icons :color="themeColor" :type="leftIcon" size="40rpx" />
							</view>
							<view class="uni-navbar__content_view" v-if="leftIcon.length > 0&&isCustomize">
								<uni-icons custom-prefix="iconfont" :color="themeColor" :type="leftIcon"
									:size="iconSize*2+'rpx'"></uni-icons>
							</view>
							<view
								:class="{ 'uni-navbar-btn-icon-left': !leftIcon.length > 0 ,'customize_left':isCustomize}"
								class="uni-navbar-btn-text" v-if="leftText.length">
								<text :style="{ color: themeColor, fontSize: leftFontSize }">{{ leftText }}</text>
							</view>
						</template>
					</view>
					<!--#endif -->
					<!--#ifdef MP-WEIXIN -->
					<view class="uni-navbar__header-btns uni-navbar__header-btns-left"
						:style="{width:hasSeat?leftIconWidth:'calc('+rightIconWidth+' + '+leftIconWidth+')',backgroundColor:isShowAbs?'rgba(0,0,0, 0.5)':''}">
						<template v-if="isLeftSlot">
							<slot name="left">
								<view @tap="onClickLeft" class="uni-navbar__content_view" v-if="leftIcon.length > 0&&!isCustomize">
									<uni-icons :color="themeColor" :type="leftIcon" size="40rpx" />
								</view>
								<view @tap="onClickLeft" class="uni-navbar__content_view" v-if="leftIcon.length > 0&&isCustomize">
									<uni-icons custom-prefix="iconfont" :color="themeColor" :type="leftIcon"
										:size="iconSize*2+'rpx'"></uni-icons>
								</view>
								<view @tap="onClickLeft" :class="{ 'uni-navbar-btn-icon-left': !leftIcon.length > 0 ,'customize_left':isCustomize}"
									class="uni-navbar-btn-text" v-if="leftText.length">
									<text :style="{ color: themeColor, fontSize: leftFontSize }">{{ leftText }}</text>
								</view>
							</slot>
						</template>
						<template v-if="!isLeftSlot">
							<view @tap="onClickLeft" class="uni-navbar__content_view" v-if="leftIcon.length > 0&&!isCustomize">
								<uni-icons :color="themeColor" :type="leftIcon" size="40rpx" />
							</view>
							<view @tap="onClickLeft" class="uni-navbar__content_view" v-if="leftIcon.length > 0&&isCustomize">
								<uni-icons custom-prefix="iconfont" :color="themeColor" :type="leftIcon"
									:size="iconSize*2+'rpx'"></uni-icons>
							</view>
							<view @tap="onClickLeft" :class="{ 'uni-navbar-btn-icon-left': !leftIcon.length > 0 ,'customize_left':isCustomize}"
								class="uni-navbar-btn-text" v-if="leftText.length">
								<text :style="{ color: themeColor, fontSize: leftFontSize }">{{ leftText }}</text>
							</view>
						</template>
						<view v-if="!hasSeat&&isRightSlot||!hasSeat&&rightIcon.length>0||!hasSeat&&rightText.length" class="view_line" :style="{'height':iconSize*2+'rpx','opacity':isShowAbs?'0.5':'1'}" style="width: 1rpx;margin: 0 24rpx;background-color: rgba(224, 224, 224, 1);"></view>
						<template v-if="!hasSeat&&isRightSlot">
							<slot name="right">
								<view @click="onClickRight" v-if="rightIcon.length&&!isCustomize">
									<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
								</view>
								<view @click="onClickRight" v-if="rightIcon.length > 0&&isCustomize">
									<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
										:size="iconSize*2+'rpx'"></uni-icons>
								</view>
								<view @click="onClickRight" class="uni-navbar-btn-text" v-if="rightText.length">
									<text class="uni-nav-bar-right-text"
										:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
								</view>
							</slot>
						</template>
						<template v-if="!hasSeat&&!isRightSlot">
							<view @click="onClickRight" v-if="rightIcon.length > 0&&!isCustomize">
								<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
							</view>
							<view @click="onClickRight" v-if="rightIcon.length > 0&&isCustomize">
								<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
									:size="iconSize*2+'rpx'"></uni-icons>
							</view>
							<view @click="onClickRight" class="uni-navbar-btn-text" v-if="rightText.length">
								<text class="uni-nav-bar-right-text"
									:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
							</view>
						</template>
					</view>
					<view class="abs_con abs_left" v-if="!hasSeat&&isShowAbs&&absList&&absList.length>0" :style="{left:leftIconWidth}">
						<view class="abs_li" v-for="item in absList" @click="comfirmClick(item)">
							{{item.name}}
						</view>
					</view>
					<!--#endif -->
					<view class="uni-navbar__header-container " @tap="onClickTitle"
						:class="{'noLeftPadding':isNoLeftPadding}">
						<template v-if="isCenterSlot">
							<slot>
								<view class="uni-navbar__header-container-inner" v-if="title.length>0"
									:class="{'alginLeft':titleIsLeft}">
									<text class="uni-nav-bar-text uni-ellipsis-1" :style="{color: themeColor }"
										:class="{'customTitlestyle':isCustom}">{{ title }}</text>
								</view>
							</slot>
						</template>
						<template v-else>
							<view class="uni-navbar__header-container-inner" v-if="title.length>0"
								:class="{'alginLeft':titleIsLeft}">
								<text class="uni-nav-bar-text uni-ellipsis-1" :style="{color: themeColor }"
									:class="{'customTitlestyle':isCustom}">{{ title }}</text>
							</view>
						</template>
					</view>
					<!--#ifndef MP-WEIXIN -->
					<view @click="onClickRight" class="uni-navbar__header-btns uni-navbar__header-btns-right"
						:style="{width:rightIconWidth,backgroundColor:isShowAbs?'rgba(0,0,0, 0.5)':''}">
						<template v-if="isRightSlot">
							<slot name="right">
								<view v-if="rightIcon.length&&!isCustomize">
									<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
								</view>
								<view v-if="rightIcon.length > 0&&isCustomize">
									<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
										:size="iconSize*2+'rpx'"></uni-icons>
								</view>
								<view class="uni-navbar-btn-text" v-if="rightText.length && !rightIcon.length">
									<text class="uni-nav-bar-right-text"
										:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
								</view>
							</slot>
						</template>
						<template v-else>
							<view v-if="rightIcon.length&&!isCustomize">
								<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
							</view>
							<view v-if="rightIcon.length > 0&&isCustomize">
								<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
									:size="iconSize*2+'rpx'"></uni-icons>
							</view>
							<view class="uni-navbar-btn-text" v-if="rightText.length && !rightIcon.length">
								<text class="uni-nav-bar-right-text"
									:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
							</view>
						</template>
					</view>
					<view class="abs_con" v-if="isShowAbs&&absList&&absList.length>0">
						<view class="abs_li" v-for="item in absList" @click="comfirmClick(item)">
							{{item.name}}
						</view>
					</view>
					<!--#endif -->
					<!--#ifdef MP-WEIXIN -->
					<view v-if="!hasSeat" class="uni-navbar__header-btns uni-navbar__header-btns-right"
						:style="{width:'calc('+rightIconWidth+' + '+leftIconWidth+')'}">
					</view>
					<view v-if="hasSeat" @click="onClickRight" class="uni-navbar__header-btns uni-navbar__header-btns-right"
						:style="{width:rightIconWidth,backgroundColor:isShowAbs?'rgba(0,0,0, 0.5)':''}">
						<template v-if="isRightSlot">
							<slot name="right">
								<view v-if="rightIcon.length&&!isCustomize">
									<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
								</view>
								<view v-if="rightIcon.length > 0&&isCustomize">
									<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
										:size="iconSize*2+'rpx'"></uni-icons>
								</view>
								<view class="uni-navbar-btn-text" v-if="rightText.length && !rightIcon.length">
									<text class="uni-nav-bar-right-text"
										:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
								</view>
							</slot>
						</template>
						<template v-else>
							<view v-if="rightIcon.length&&!isCustomize">
								<uni-icons :color="themeColor" :type="rightIcon" size="44rpx" />
							</view>
							<view v-if="rightIcon.length > 0&&isCustomize">
								<uni-icons custom-prefix="iconfont" :color="rightIconColor?rightIconColor:themeColor" :type="rightIcon"
									:size="iconSize*2+'rpx'"></uni-icons>
							</view>
							<view class="uni-navbar-btn-text" v-if="rightText.length && !rightIcon.length">
								<text class="uni-nav-bar-right-text"
									:style="{ color: rightTextColor?rightTextColor:themeColor, fontSize: leftFontSize}">{{ rightText }}</text>
							</view>
						</template>
					</view>
					<view class="abs_con" v-if="hasSeat&&isShowAbs&&absList&&absList.length>0">
						<view class="abs_li" v-for="item in absList" @click="comfirmClick(item)">
							{{item.name}}
						</view>
					</view>
					<!--#endif -->
					
				</view>
			</slot>
		</view>
		<!-- #ifndef APP-NVUE -->
		<view class="uni-navbar__placeholder" v-if="fixed">
			<status-bar v-if="statusBar" :bgc="themeBgColor"/>
			<view v-if="showNavPla" class="uni-navbar__placeholder-view" :style="{ height:navbarHeight}" />
		</view>
		<!-- #endif -->
	</view>
</template>

<script>
	import statusBar from "./uni-status-bar.vue";
	const getVal = (val) => typeof val === 'number' ? val + 'px' : val;

	/**
	 * 
	 * 
	 * NavBar 自定义导航栏
	 * @description 导航栏组件，主要用于头部导航
	 * @tutorial https://ext.dcloud.net.cn/plugin?id=52
	 * @property {Boolean} dark 开启黑暗模式
	 * @property {String} title 标题文字
	 * @property {String} leftText 左侧按钮文本
	 * @property {String} rightText 右侧按钮文本
	 * @property {String} leftIcon 左侧按钮图标（图标类型参考 [Icon 图标](http://ext.dcloud.net.cn/plugin?id=28) type 属性）
	 * @property {String} rightIcon 右侧按钮图标（图标类型参考 [Icon 图标](http://ext.dcloud.net.cn/plugin?id=28) type 属性）
	 * @property {String} color 图标和文字颜色
	 * @property {String} backgroundColor 导航栏背景颜色
	 * @property {Boolean} fixed = [true|false] 是否固定顶部
	 * @property {Boolean} statusBar = [true|false] 是否包含状态栏
	 * @property {Boolean} shadow = [true|false] 导航栏下是否有阴影
	 * @property {Boolean} stat 是否开启统计标题上报
	 * @event {Function} clickLeft 左侧按钮点击时触发
	 * @event {Function} clickRight 右侧按钮点击时触发
	 * @event {Function} clickTitle 中间标题点击时触发
	 */
	export default {
		name: "UniNavBar",
		components: {
			statusBar
		},
		emits: ['clickLeft', 'clickRight', 'clickTitle'],
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
			showNavPla: {
				type: Boolean,
				default: true
			},
			isCustomize: {
				type: Boolean,
				default: false
			},
			dark: {
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
				default: false
			},
			color: {
				type: String,
				default: ""
			},
			backgroundColor: {
				type: String,
				default: ""
			},
			statusBar: {
				type: [Boolean, String],
				default: false
			},
			shadow: {
				type: [Boolean, String],
				default: false
			},
			border: {
				type: [Boolean, String],
				default: true
			},
			height: {
				type: [Number, String],
				default: 44
			},
			leftWidth: {
				type: [Number, String],
				default: 60
			},
			rightWidth: {
				type: [Number, String],
				default: 60
			},
			stat: {
				type: [Boolean, String],
				default: ''
			},
			leftFontSize: { //左边字体大小
				type: [String],
				default: '24rpx'
			},
			iconSize: { //图标的大小
				type: Number,
				default: 18
			},
			topopacity:{
				type: Number,
				default: 1
			},
			titleIsLeft: { //顶部标题是否左对齐
				type: [Boolean, String],
				default: false
			},
			isCustom: { //顶部标题是否左对齐
				type: Boolean,
				default: false
			},
			isNoLeftPadding: { //标题中间是否没有左边的padding
				type: Boolean,
				default: false
			},
			isTouming: { //背景是否是透明的
				type: Boolean,
				default: false
			},
			isBg:{//是否是为了设置背景
				type: Boolean,
				default: false
			},
			zIndex:{
				type:Number,
				default:998
			},
			isShowAbs:{ //头部是否显示弹出定位
				type: Boolean,
				default: false
			},
			absList:{
				type:Array,
				default:()=>{
					return []
				}
			},
			rightIconColor: {
				type: String,
				default: ""
			},
			rightTextColor: {
				type: String,
				default: ""
			},
		},
		computed: {
			themeBgColor() {
				if (this.dark) {
					// 默认值
					if (this.backgroundColor) {
						return this.backgroundColor
					} else {
						return this.dark ? '#333' : ''
					}
				}
				return this.backgroundColor || ''
			},
			themeColor() {
				if (this.dark) {
					// 默认值
					if (this.color) {
						return this.color
					} else {
						return this.dark ? '#fff' : '#333'
					}
				}
				return this.color || '#fff'
			},
			navbarHeight() {
				return getVal(this.height)
			},
			leftIconWidth() {
				return getVal(this.leftWidth)
			},
			rightIconWidth() {
				return getVal(this.rightWidth)
			}
		},
		mounted() {
			if (uni.report && this.stat && this.title !== '') {
				uni.report('title', this.title)
			}
		},
		methods: {
			comfirmClick(row){
				this.$emit('comfirmClick',row)
			},
			onClickLeft() {
				this.$emit("clickLeft");
			},
			onClickRight() {
				this.$emit("clickRight");
			},
			onClickTitle() {
				this.$emit("clickTitle");
			}
		}
	};
</script>

<style lang="scss" scoped>
	$nav-height: 88rpx;
	// $nav-height: var(--nav-height);
	.uni-nvue-fixed {
		/* #ifdef APP-NVUE */
		position: sticky;
		/* #endif */
	}

	.uni-navbar {
		// box-sizing: border-box;
		
	}

	.uni-nav-bar-text {
		/* #ifdef APP-PLUS */
		font-size: 34rpx;
		/* #endif */
		/* #ifndef APP-PLUS */
		font-size: 28rpx;

		/* #endif */
		&.customTitlestyle {
			font-size: 36rpx;
			font-weight: 550;
		}
	}

	.uni-nav-bar-right-text {
		font-size: 24rpx;
	}

	.uni-navbar__content {
		position: relative;
		// background-color: #fff;
		// box-sizing: border-box;
		background-color: transparent;
		width: 100%;
	}

	.uni-navbar__content_view {
		// box-sizing: border-box;
	}

	.uni-navbar-btn-text {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		flex-direction: column;
		justify-content: flex-start;
		align-items: center;
		line-height: 24rpx;

	}

	.customize_left {
		line-height: 32rpx;
		margin-left: 16rpx;
	}

	.uni-navbar-btn-icon-left {
		line-height: 32rpx;
		margin-left: 16rpx;
	}

	.uni-navbar__header {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		padding: 0 20rpx;
		flex-direction: row;
		height: $nav-height;
		font-size: 24rpx;
		position: relative;
	}
	.uni-navbar__header .abs_con{
		position: absolute;
		right: 20rpx;
		top: 88rpx;
		background-color: #fff;
		padding: 5rpx 30rpx;
		color: #333;
		border-radius: 10rpx;
		width: 240rpx;
		box-sizing: border-box;
		&.abs_left{
			right: auto;
		}
		.abs_li{
			width: 100%;
			padding: 25rpx 0;
			text-align: center;
			font-size: 28rpx;
			line-height: 28rpx;
		}
	}
	.uni-navbar__header-btns {
		/* #ifndef APP-NVUE */
		// overflow: hidden;
		display: flex;
		/* #endif */
		flex-wrap: nowrap;
		flex-direction: row;
		width: 120rpx;
		// padding: 0 6px;
		justify-content: center;
		align-items: center;
		/* #ifdef H5 */
		cursor: pointer;
		/* #endif */
	}

	.uni-navbar__header-btns-left {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		width: 120rpx;
		justify-content: flex-start;
		align-items: center;
	}

	.uni-navbar__header-btns-right {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		flex-direction: row;
		// width: 150rpx;
		// padding-right: 30rpx;
		justify-content: flex-end;
		align-items: center;
	}

	.uni-navbar__header-container {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		flex: 1;
		padding: 0 20rpx;
		// overflow: hidden;
		justify-content: center;
		align-items: center;
		&.noLeftPadding {
			padding-left: 0;
		}
	}

	.uni-navbar__header-container-inner {
		/* #ifndef APP-NVUE */
		display: flex;
		/* #endif */
		flex: 1;
		flex-direction: row;
		align-items: center;
		justify-content: center;
		font-size: 34rpx;
		overflow: hidden;

		// box-sizing: border-box;
		&.alginLeft {
			text-align: left;
			/* #ifndef APP-NVUE */
			justify-content: flex-start;
			/* #endif */
			font-size: 38rpx;
		}
	}


	.uni-navbar__placeholder-view {
		height: $nav-height;
	}

	.uni-navbar--fixed {
		position: fixed;
		z-index: 998;
		/* #ifdef H5 */
		left: var(--window-left);
		right: var(--window-right);
		/* #endif */
		/* #ifndef H5 */
		left: 0;
		right: 0;
		/* #endif */
		&.top_bg{
			position: absolute;
			top: 0;
			left: 0;
		}

	}

	.uni-navbar--shadow {
		box-shadow: 0 2rpx 12rpx #ccc;
	}

	.uni-navbar--border {
		border-bottom-width: 1rpx;
		border-bottom-style: solid;
		border-bottom-color: #eee;
	}

	.uni-ellipsis-1 {
		overflow: hidden;
		/* #ifndef APP-NVUE */
		white-space: nowrap;
		text-overflow: ellipsis;
		/* #endif */
		/* #ifdef APP-NVUE */
		lines: 1;
		text-overflow: ellipsis;
		/* #endif */
		height: 100%;
		align-content: center;
		display: flex;
		align-items: center;
	}

	// 暗主题配置
	.uni-dark {}
</style>