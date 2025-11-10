<template>
	<view>
		<view class="topSearch">
			<uni-search-bar :focus="true" cancelButton="none" v-model="searchVal" placeholder="请输入关键词"
				@input="load_data()" @clear="searchVal = '',load_data()" @iconClick="load_data()"></uni-search-bar>
		</view>
		<view class="topheight-zw"></view>
		<view class="ls-wrap" v-if="userlist.length==0">
			<view class="ls-t border-bottom" v-if="historylist.length>0">
				<view class="l">历史记录</view>
				<van-icon name="delete-o" color="#333" size="44rpx" @click="clearHistory" />
			</view>
			<view class="ls-c">
				<view class="c-item" v-for="hitem in historylist" @click="searchVal = hitem,load_data()">
					{{hitem}}
				</view>
			</view>
			<van-empty description="暂无数据" />
		</view>

		<view v-else style="padding:0rpx 20rpx 80rpx 20rpx;">
			<navigator :url="'/pages/depman/member_edit?id=' + mem.Id+'&pageLen=1'" v-for="mem in userlist" :key="mem.Id"
				class="u-item" style="margin-bottom: 20rpx;">
				<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="mem.Avatar"
					@error="hdImgErr(mem)" />
				<view class="c">
					<view class="tp">
						<text class="name">{{mem.RealName}}</text>
					</view>
					<view class="bm" v-if="mem.post_name!=''">
						{{mem.post_name}}
					</view>
				</view>
				<navigator v-if="mem.Mobile!=''" url="" @click.stop="callTel(mem.Mobile)">
					<uni-icons type="phone" size="22" color="#333333"></uni-icons>
				</navigator>
			</navigator>
		</view>
	</view>
</template>

<script>
	import {
		getUserList,
		getId2CardId
	} from '@/api/user'

	export default {
		data() {
			return {
				searchVal: '',
				userlist: [],
				historylist: []
			};
		},
		onLoad: function() {
			
			this.historylist = uni.getStorageSync('tmpsearchList' + this.$store.state.userInfo.Id) || [];
			// console.log("this.$store.state.userInfo.Id",this.$store.state.userInfo.Id,this.historylist);
		},
		onShow() {
			this.load_data()
		},
		methods: {
			load_data() {
				if (this.searchVal == "") {
					this.userlist.length = [];
				} else {
					getUserList({
						"showAll": true,
						"key": this.searchVal
					}).then(rsp => {
						// console.log("成员信息",rsp);
						this.userlist = rsp.data.List;
					})
				}

			},
			hdImgErr: function(item) {
				if (item.Sex == 1) {
					item.Headimg = '/static/user/user_girl.png';
				} else {
					item.Headimg = '/static/user/user_man.png';
				}
			},
			clearHistory() {
				uni.showModal({
					title: '提示',
					content: "确定要清除历史记录吗?",
					success: (res) => {
						if (res.confirm) {
							this.historylist = [];
							uni.setStorageSync('tmpsearchList' + this.$store.state.userInfo.Id, []);
						}
					}
				});

			},
			jmp2Person(mem) {
				let hasold = false;
				for (let i = 0; i < this.historylist.length; i++) {
					if (this.historylist[i] == mem.RealName) {
						this.historylist.splice(i, 1);
						hasold = true;
						break;
					}
				}

				if (this.historylist.length > 50 && !hasold) {
					this.historylist.shift();
				}
				this.historylist.unshift(mem.RealName);
				uni.setStorageSync('tmpsearchList' + this.$store.state.userInfo.Id, this.historylist);
				getId2CardId(mem.Id).then(rsp=>{
					uni.navigateTo({
						url: '/pages/card_center/card?id=' + rsp.data
					});
				});
				
			}
		}
	}
</script>

<style lang="scss">
	.topSearch {
		background-color: #fff;
		position: fixed;
		top: 0;
		width: 100vw;
	}

	.topheight-zw {
		height: 56px;
	}

	.u-item {
		height: 140rpx;
		background-color: #FFFFFF;
		display: flex;
		flex-direction: row;
		align-items: center;
		padding: 10rpx 30rpx;
	
		.ximg {
			display: flex;
			overflow: hidden;
			width: 112rpx;
			height: 112rpx;
			border-radius: 50%;
		}
	
		.c {
			flex: 1;
			padding-left: 20rpx;
	
			.tp {
				display: flex;
				flex-direction: row;
				align-items: center;
	
				.name {
					font-size: 30rpx;
					color: #333333;
					margin-right: 20rpx;
				}
			}
	
			.bm {
				display: flex;
				flex-direction: row;
				align-items: center;
				margin-top: 20rpx;
				color: #999;
				font-size: 24rpx;
			}
	
		}
	}

	.ls-wrap {
		display: flex;
		flex-direction: column;
		padding: 0 30rpx;
		background-color: #fff;

		.ls-t {
			display: flex;
			flex-direction: row;
			justify-content: space-between;
			height: 90rpx;
			align-items: center;

			.l {
				font-size: 30rpx;
				font-weight: bold;
			}
		}

		.ls-c {
			display: flex;
			flex-direction: row;
			flex-wrap: nowrap;
			padding: 20rpx 0;

			.c-item {
				background-color: #f7f8fa;
				border-radius: 10rpx;
				padding: 6rpx 20rpx;
				font-size: 24rpx;
				margin-right: 20rpx;
			}
		}
	}
</style>
