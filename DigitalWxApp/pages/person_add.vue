<template>
	<view class="common_wrap wrap">
		<view class="title">
			填写信息创建
		</view>
		<view class="row">
			<text>所在企业</text>
			<view class="right">
				<input type="text" value="爬虫科技" />
			</view>
		</view>
		<!-- <view class="row">
			<text>行业类型</text>
			<view class="right">
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		<view class="row">
			<text>员工规模</text>
			<view class="right">
				<text>100~500</text>
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view> -->
		<view class="row">
			<text>姓名</text>
			<view class="right">
				<input type="text" value="" placeholder="请输入名字"/>
			</view>
		</view>
		<view class="row">
			<text>职位</text>
			<view class="right">
				<input type="text" value="" placeholder="请输入职位"/>
			</view>
		</view>
		<view class="common_btn">
			开通数字名片
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {

			}
		},
		methods: {

		}
	}
</script>

<style lang="scss">
	.wrap {
		width: 100%;
		background-color: #FFFFFF;

		.title {
			height: 120rpx;
			line-height: 120rpx;
			padding-left: 30rpx;
			box-sizing: border-box;
			color: #333333;
			font-family: pfzho;
			border-bottom: 1px solid #F6F6F6;
			font-size: 34rpx;
		}

	}
	
	.common_btn{
		margin-top: 230rpx;
	}
</style>
