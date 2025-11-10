<template>
	<view class="sel-wrap">
		<navigator url="" class="list-item border-all" v-for="item in orgList" @click="onSelected(item)">
			<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="item.Logo" />
			<view class="xit-cont">
				<text class="t">{{item.OrgName}}</text>
				<text class="xp">行业：{{item.IndustryName}}</text>
				<text class="p">规模：{{item.SizeName}}</text>
			</view>
		</navigator>
		<view class="border-btn">
			<button class="btn" @click="onCreate">
				创建企业
			</button>
		</view>
	</view>
</template>

<script>
	import {
		userOrgList
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				orgList: []
			};
		},
		onLoad() {
			this.reloadpage();
		},
		onUnload: function() {
			this.$store.state.submitMod.formArrary.pop();
		},
		methods: {
			onCreate() {
				uni.navigateTo({
					url: '/pages/company/company_add'
				});
			},
			async reloadpage() {
				try {
					//获取用户的企业列表
					let rsp = await userOrgList();
					this.orgList = rsp.data;
					await this.refreshList();
				} catch (err) {
					console.info('异常：', err);
				}
			},
			onSelected(item) {
				let cardData = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1];
				cardData.OrgName = item.OrgName;
				cardData.OrgId = item.Id;
				cardData.DeptName = item.dept_name;
				cardData.PostName = item.post_name;
				reloadPrePage(1, "company");
				uni.navigateBack();
			},
			async refreshList() {
				for (let xi = 0; xi < this.orgList.length; xi++) {
					let x = this.orgList[xi];
					x.IndustryName = "";
					x.SizeName = "";
					if (x.Logo == "") {
						x.Logo = "/static/user/company.png";
					}
					if(x.Size){
						let rt = await this.$store.dispatch("dictName", {
							name: "org_size",
							value: x.Size
						});
						x.SizeName = rt;
					}
					if(x.Industry){
						let rt2 = await this.$store.dispatch("industryName", x.Industry);
						x.IndustryName = rt2;
					}
				}
				this.$forceUpdate();
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #fff;
	}

	.border-all:after {
		border-radius: 10rpx;
	}

	.sel-wrap {
		padding: 0 30rpx;
		padding-bottom: 140rpx;//底部增加内边距，企业多的时候防止最后一个企业被遮住
	}
	.border-btn{
		background-color: #fff;
	}

	.list-item {
		display: flex;
		

		.ximg {
			width: 174rpx;
			height: 174rpx;
			border-radius: 8rpx;
			overflow: hidden;
			margin: 12rpx;
		}

		.xit-cont {
			margin-left: 36rpx;
			display: flex;
			flex-direction: column;

			.t {
				margin-top: 30rpx;
				font-size: 30rpx;
				font-weight: bold;
				color: #333333;
			}

			.xp {
				margin-top: 20rpx;
				margin-bottom: 10rpx;
			}

			.p,
			.xp {
				font-size: 22rpx;
				color: #999999;
			}
		}
	}
</style>
