<template>
	<view class="personData">
		<top leftIcon="icon-fanhui" leftText="Back" backgroundColor="#161A26" title="Edit profile" class="CRM-header"></top>
		<view class="personData-data">
			<view class="personData-data-header">
				
			</view>
			<view class="personData-data-parent">
				<view class="nameTitle">
					User name
				</view>
				<view class="inputValue">
					<input  type="text" class="addPool-easyinput" placeholder="Please enter the user name" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
				
			</view>
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Sex
				</view>
				<view class="gender" >
					<view :class="[selected==index?'active':'on']" class="gender-item" v-for="(item,index) in arr" :key="index" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</view>
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Telephone number
				</view>
				<view class="content">
					<view>
						13809876543
					</view>
					<view class="contentRight">
						<view>
							Binding
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					E-mail
				</view>
				<view class="content">
					<view>
						Non
					</view>
					<view class="contentRight">
						<view>
							Binding
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Department
				</view>
				<view class="inputValue">
					<input  type="text" disabled class="addPool-easyinput bgColor" placeholder="Please enter the user name" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
				
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Roles
				</view>
				<view class="inputValue">
					<input  type="text" disabled class="addPool-easyinput bgColor" placeholder="Company administrator, Agent, Initial member" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Creation date
				</view>
				<view class="inputValue">
					<input  type="text" disabled class="addPool-easyinput bgColor" placeholder="2023-09-02 20:00:00" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>
			
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Others
				</view>
				<view class="content">
					<view style="color:#fff;">
						Change password
					</view>
					<view class="contentRight">
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			<view class="save-submit">
				Save
			</view>
			<view style="height:20px;"></view>
		</view>
	</view>
</template>

<script>
	export default {
		data(){
			return{
				selected:0,
				arr:[
					{
					title:'Male'
					},
					{
					title:'Female'
					},
				]
			}
		},
		methods:{
			handClick(inx){
				this.selected=inx
			},
		}
	}
</script>

<style lang="less" scoped>
	.personData{
	.personData-data{
		.save-submit{
			width:92%;
			margin:0rpx 4%;
			background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
			height:100rpx;
			border-radius: 7rpx;
			color:#fff;
			line-height: 100rpx;
			text-align: center;
			font-size:36rpx;
		}
		.personData-data-parent{
			width:92%;
			margin:30rpx 4%;
			color:#fff;
			.content{
				display: flex;
				height: 88rpx;
				align-items: center;
				border:1rpx solid rgba(255, 255, 255, 0.2);
				margin-top:20rpx;
				border-radius: 6rpx;
				padding:0rpx 20rpx;
				color:rgba(255, 255, 255, 0.3);
				.contentRight{
					display: flex;
					margin-left:auto;
					.icon-a-youjiantoubai{
						color:rgba(255, 255, 255, .2);
						font-size:24rpx;
						margin-left:10rpx;
					}
				}
			}
			.gender{
				display: flex;
				justify-content: space-between;
				.gender-item{
					width: 48%;
					height:88rpx;
					line-height: 88rpx;
					text-align: center;
					border-radius: 6rpx;
					margin-top:15rpx;
				}
				.active{
					border:1rpx solid rgba(255, 53, 53, 1);
					color:rgba(255, 53, 53, 1);
				}
				.on{
					border:1rpx solid rgba(255, 255, 255, 0.2);
					color:rgba(255, 255, 255, .6);
				}
			}
			.addPool-easyinput{
				background: #161A26!important;
				border:1rpx solid rgba(255, 255, 255, .2);
				border-radius: 12rpx;
				font-size:30rpx;
				height: 88rpx;
				color:#fff;
				padding:0rpx 20rpx;
			}
			.bgColor{
				background: rgba(28, 34, 50, 1)!important;
			}
			.inputValue{
				margin-top:15rpx;
			}
			.nameTitle{
				color:rgba(255, 255, 255, .4);
			}
		}
		.personData-data-header{
			width: 160rpx;
			height:160rpx;
			background: pink;
			border-radius: 50%;
			margin:50rpx auto;
		}
	}
	
	}
</style>