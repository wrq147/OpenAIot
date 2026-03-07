<template>
	<view>
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#ffffff" rightWidth="157rpx" :isleftBack="true">
		</top>
		<view class="process_con">
			<view class="process_li" v-for="item in list" @click.stop="toProccessInfo(item)">
				<view class="li_con">
					<view class="li_left" :style="{'background':item.Background}">
						<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
							iconsColor="#ffffff"></custom-icons>
					</view>
					<view class="li_right">
						<view class="name">
							{{item.FlowName}}
						</view>
						<view class="info_li">
							接收时间: {{item.StartTime}}
						</view>
						<view class="info_li">
							任务节点: {{item.StepName}}
						</view>
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>

		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
	</view>
</template>

<script>
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {devPlaneTaskInfo} from '@/api/devplane.js'
	export default {
		data() {
			return {
				topTitle: '待办项',
				list: [],
				status:'noMore',
				isreload:false,
			};
		},
		async onLoad(options) {
			if(options.id){
				try{
					let res=await devPlaneTaskInfo({id:options.id})
					console.log("任务详情",res.data.TodoTasks);
					this.list=JSON.parse(JSON.stringify(res.data.TodoTasks))
				}catch(e){
					//TODO handle the exception
					console.log(e);
					this.setMsgTop(e)
				}
			}
		},
		onShow() {
			if(this.isreload){
				setPagesParam('loadTaskList', 'load', 1)
				this.isreload=false
			}
		},
		methods: {
			toProccessInfo(row) {
				uni.navigateTo({
					url: '/pages_flow/process_detail?id=' + row.ExecutionNodeId + '&toDo=true'
				})
			},
			loadData(query) {
				if(query){
					this.isreload=true
				}
				
			}
		}
	}
</script>

<style lang="less">
	.process_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.process_li {
			margin-top: 20rpx;
			// display: flex;
			// justify-content: flex-start;
			// align-items: flex-start;
			padding: 24rpx 30rpx;
			width: 100%;
			box-sizing: border-box;
			background-color: #FFFFFF;
			color: #333;
			border-radius: 10rpx;

			.li_con {
				display: flex;
				justify-content: flex-start;
				align-items: flex-start;

				.li_left {
					width: 80rpx;
					height: 80rpx;
					background-color: rgba(255, 133, 61, 1);
					border-radius: 10rpx;
					display: flex;
					justify-content: center;
					align-items: center;
					margin-right: 20rpx;
					margin-top: 6rpx;
				}

				.li_right {
					.name {
						font-size: 30rpx;
						line-height: 44rpx;
					}

					.info_li {
						font-size: 24rpx;
						line-height: 40rpx;
						color: #999999;
						margin-top: 10rpx;
					}
				}
			}

			.view_line {
				width: 100%;
				height: 1rpx;
				background-color: #EAEAEA;
				margin-top: 20rpx;
			}

			.hadle_con {
				margin-top: 24rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;

				.tips {
					padding: 6rpx 8rpx;
					font-size: 20rpx;
					height: 20rpx;
					line-height: 20rpx;
					width: inherit;
					border-radius: 4rpx;
					background-color: #EEFAF2;
					color: #50C97A;

					&.in {
						background-color: #E9F1FF;
						color: #2371FF;
					}

					&.Draft {
						background-color: #F8F8F8;
						color: #999999;
					}

					&.Rejected {
						background-color: #FFF5F5;
						color: #FF3535;
					}
				}

				.hadle_right {
					display: flex;
					justify-content: flex-end;
					align-items: center;

					.hadle_li {
						display: flex;
						justify-content: flex-end;
						align-items: center;
						margin-right: 52rpx;

						.text {
							font-size: 28rpx;
							margin-left: 10rpx;
							color: #999999;
						}
					}

					.hadle_li:last-child {
						margin-right: 0;
					}
				}

			}

		}
	}
</style>