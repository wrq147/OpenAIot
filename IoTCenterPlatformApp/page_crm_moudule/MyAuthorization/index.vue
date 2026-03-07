<template>
	<view class="MyAuthorization">
		<top   leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#fff" title="我的授权"  class="CRM-header"></top>
		<view class="MyAuthorization-list">
			<view class="MyAuthorization-list-item" v-for="(item,index) in arr" :key="index">
				<view class="left">
					<img :src="item.Logo" alt="" />
				</view>
				<view class="right">
					<view class="title">
						{{item.ParentOrgName}}
					</view>
					<view class="content">
						代理级别: {{item.GradeName}}
					</view>
					<view class="content">
						{{item.createTime}}
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
	</view>
</template>

<script>
	import {
		AuthorizationData,//我的授权
	} from "@/api/crmApi";
	export default {
		data(){
			return{
				arr:[],
				queryData:{
					// Belong:1,
					pageNum:1,
					pageSize:30,
				},
				key:'',
				status: 'loading',
			}
		},
		onLoad() {
			this.list();
		},
		methods:{
			list(){
				AuthorizationData().then((res)=>{
					this.arr=res.data
					if(res.code==0){
						if (this.queryData.pageNum == 1) {
							this.arr = []
						}
						this.arr = [...this.arr, ...res.data];
						if (res.data.length < this.queryData.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
						//console.log(res,'res')
					}
				})
			},
		}
	}
</script>

<style lang="less" scoped>

	.MyAuthorization{
		.MyAuthorization-list{
			padding: 0rpx 30rpx;
			margin-top:20rpx;
			.MyAuthorization-list-item{
				height:180rpx;
				padding:0rpx 20rpx;
				background: #fff;
				border-radius: 12rpx;
				display: flex;
				align-items: center;
				margin-bottom: 20rpx;
				.left{
					width: 120rpx;
					height: 120rpx;
					background: #fff;
					border-radius: 12rpx;
					img{
						width: 100%;
						height: 100%;
					}
				}
				.right{
					color:#fff;
					margin-left:30rpx;
					.content{
						font-size:24rpx;
						color:rgba(153, 153, 153, 1);
						margin-top:10rpx;
					}
					.title{
						font-size:32rpx;
						color:rgba(51, 51, 51, 1);
						font-weight: bold;
						
					}
				}
			}
		}
	}
</style>
