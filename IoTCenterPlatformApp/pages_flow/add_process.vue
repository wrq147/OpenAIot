<template>
	<view style="min-height: 100vh;background-color: #fff;">
		<top :title="topTitle" leftText="选择流程" leftWidth="300rpx" rightIcon="icon-crmtianjiaguanbi"
			rightWidth="157rpx" :isleftBack="false" backgroundColor="#FFFFFF" @clickRight="closeAdd" :iconSize="18"
			leftFontSize="36rpx"></top>

		<search-compt @searching="searching" pal="请输入流程名称"
			:isOnlySearch="true" backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>

		<view class="process_list">
			<view class="group_li" v-for="row in iconsGroups">
				<view class="group_title">{{row.Name}}</view>
				<view class="group_pro_lis">
					<view class="pro_li" v-for="item in row.Items" v-if="row.Items&&row.Items.length>0" @click="startProcess(item)">
						<view class="icon_con" :style="{'background':item.Background}">
							<custom-icons :iconsName="iconSubStr(item.Icon)" iconsSize="40rpx" iconsColor="#ffffff"></custom-icons>
						</view>
						<view class="name">
							{{item.Name}}
						</view>
					</view>
				</view>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
	  definitionList
	} from "@/api/process.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: '',
				iconsGroups:[],
				queryProcessParams:{
					IsEmbed:false
				},
				isSelect:false
			};
		},
		onLoad(options) {
			this.listDefinition()
			if(options.isSel){
				this.isSelect=true
			}else{
				this.isSelect=false
			}
		},
		methods: {
			searching(val){
				if(val){
					this.queryProcessParams.name=val
				}else{
					delete this.queryProcessParams.name
				}
				this.listDefinition()
			},
			startProcess(row){
				//发起流程
				if(this.isSelect){
					setPagesParam('onSelected', row, 1)
				}else{
					uni.redirectTo({
						url:'/pages_flow/process_form?procDefId='+row.Id
					})
				}
				
			},
			listDefinition() {
				definitionList(this.queryProcessParams).then(response => {
					this.iconsGroups = response.data;
				}).catch(err=>{
					this.setMsgTop(err)
				});
			},
			closeAdd() {
				//关闭流程选择页
				
				let pages = getCurrentPages(); // 获取当前页面栈的实例
				let prevPage = pages[pages.length - 2]; //上一个页面
				let curPage = pages[pages.length - 1].route; //当前页面
				if (!prevPage || prevPage == undefined || prevPage == null) {
					uni.switchTab({
						url: '/pages/profile/profile'
					})
				}else{
					// #ifdef H5
					history.back();
					// #endif
					// #ifndef H5
					uni.navigateBack()
					// #endif
				}
			}
		}
	}
</script>

<style lang="less" scoped>
	.process_list {
		color: #333333;
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		background-color: #ffffff;

		.group_li {
			margin-top: 30rpx;
			padding-bottom: 30rpx;

			.group_title {
				font-size: 28rpx;
				color: #999999;
			}

			.group_pro_lis {
				display: flex;
				justify-content: flex-start;
				align-items: flex-start;
				flex-wrap: wrap;

				.pro_li {
					width: 210rpx;
					min-height: 124rpx;
					display: flex;
					flex-direction: column;
					justify-content: space-between;
					align-items: center;
					margin-top: 50rpx;
					margin-right: 40rpx;
					padding-bottom: -4rpx;

					.icon_con {
						width: 80rpx;
						height: 80rpx;
						display: flex;
						align-items: center;
						justify-content: center;
						background: rgba(255, 133, 61, 1);
						border-radius: 10rpx;
					}

					.name {
						margin-top: 16rpx;
						font-size: 24rpx;
						color: #333333;
						word-break: keep-all;
						white-space: pre-wrap;
						line-height: 32rpx;
					}
				}

				.pro_li:nth-child(3n) {
					margin-right: 0;
				}
				.pro_li:hover{
					opacity: 0.8;
				}
			}
		}
	}
</style>