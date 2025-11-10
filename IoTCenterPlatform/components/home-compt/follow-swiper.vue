<template>
	<view>
		<uni-swiper-dot class="uni-swiper-dot-box" @clickItem=clickItem :info="info" :current="current" :mode="mode"
			:dots-styles="dotsStyles" field="content" v-if="info&&info.length>0">
			<swiper class="swiper_view_con" @change="change" :current="swiperDotIndex">
				<swiper-item v-for="(item, index) in info" :key="index">
					<view class="swiper_item_con" :class="'swiper-item' + index">
						<view class="line_title">
							<view class="left_text">
								<text>Follow up plan</text>
							</view>
							<view class="right_cont">
								<text>Complete</text>
							</view>
							<view class="line"></view>
						</view>
						<view class="follow">
							<view class="name_label">Objective:</view>
							<view class="name_value">{{item.CustomerName}}</view>
						</view>
						<view class="follow nomargin">
							<view class="name_label">Content:</view>
							<view class="name_value">{{item.Remark}}</view>
						</view>
					</view>
				</swiper-item>
				
			</swiper>
			
		</uni-swiper-dot>
		<uni-load-more iconType="circle" :status="status" v-else-if="showLoading"/>
	</view>
	
</template>

<script>
	export default {
		name:"follow-swiper",//搜索主键
		props:{
			status:{
				type:String,
				default:'loading'
			},
			showLoading:{
				type:Boolean,
				default:true
			},
			info:{
				type:Array,
				default:()=>{
					return []
				}
			}
		},
		data() {
			return {
				//轮播图相关属性
				swiperDotIndex: 0,
				// info: [],
				mode: 'dot',
				dotsStyles: {
					bottom: 10,
					backgroundColor: 'rgba(120, 130, 157, 1)',
					border: '1px rgba(120, 130, 157, 1) solid',
					color: '#fff',
					selectedBackgroundColor: 'rgba(255, 53, 53, 1)',
					selectedBorder: '1px rgba(255, 53, 53, 1) solid'
				},
				indicatorDots: true,
				autoplay: false,
				interval: 2000,
				duration: 500,
				current: 0,
				//轮播图相关属性
			};
		},
		methods:{
			change(e) {//轮播页面发生变化
				this.current = e.detail.current;
			},
			clickItem(e) {//点击指示点
				this.swiperDotIndex = e
			},
		}
	}
</script>

<style lang="less" scoped>
	.swiper_view_con {
		width: 100%;
		height: 302rpx;
		padding: 20rpx 20rpx 0 20rpx;
		box-sizing: border-box;
	
		.swiper_item_con {
			width: 100%;
			// height: 234rpx;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			border-radius: 10rpx;
			box-sizing: border-box;
			padding: 30rpx;
			position: relative;
	
			.follow {
				font-size: 28rpx;
				line-height: 28rpx;
				margin-top: 20rpx;
				display: flex;
				justify-content: flex-start;
				font-family: Roboto-Regular, Roboto;
	
				&.nomargin {
					margin-top: 28rpx;
				}
	
				.name_label {
					color: rgba(255, 255, 255, 0.5);
					width: 134rpx;
					margin-right: 10rpx;
				}
	
				.name_value {
					color: rgba(255, 255, 255, 1);
					display: -webkit-box;
					-webkit-box-orient: vertical;
					-webkit-line-clamp: 2;
					overflow: hidden;
					text-overflow: ellipsis;
					width: calc(100% - 134rpx);
				}
			}
		}
	}
</style>