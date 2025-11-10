<template>
	<view class="common_wrap wrap">
		<view class="title">
			您的同事邀请您加入企业
		</view>
		<view class="row">
			<text>所在企业</text>
			<view class="right">
				<text>{{OrgName}}</text>
			</view>
		</view>
		<view class="row">
			<text>所属部门</text>
			<view class="right">
				<text>{{DeptName}}</text>
			</view>
		</view>
		<view class="row">
			<text>姓名</text>
			<view class="right">
				<uni-easyinput class="iptTxt" trim="both" maxlength="20" @focus="realName.err=false"
					v-model="realName.value" :inputBorder="false" placeholder="请输入姓名">
				</uni-easyinput>
			</view>
		</view>
		<view class="row">
			<text>职位</text>
			<view class="right">
				<uni-easyinput class="iptTxt" trim="both" maxlength="20" @focus="posName.err=false"
					v-model="posName.value" :inputBorder="false" placeholder="请输入职位">
				</uni-easyinput>
			</view>
		</view>


		<view class="common_btn" @click="onJoin">
			加入
		</view>
	</view>
</template>

<script>
	import {
		parseYq,
		switchOrg,
		joinOrg
	} from '@/api/org.js'
	export default {
		data() {
			return {
				OrgName: "",
				DeptName: "",
				realName: {
					value: "",
					err: false
				},
				posName: {
					value: "",
					err: false
				},
				yqcode:null
			}
		},
		async onLoad(options) {
			try {
				this.yqcode = options.yq || "";
				let res = await parseYq(this.yqcode);
				this.OrgName = res.data.OrgName;
				this.DeptName = res.data.DeptName;
				let rsp = await this.$store.dispatch("userInfo");
				this.realName.value = rsp.data.name;
			} catch (err) {
				console.info('异常：', err);
			}

		},
		methods: {
			async onJoin(){
				uni.showLoading({
					title: '加载中...'
				});

				try{
					let joinAfter=await joinOrg({"code":this.yqcode,"realName":this.realName.value,"postName":this.posName.value});
					// console.log("加入企业",joinAfter);
					uni.hideLoading();
					this.$store.commit('SET_USER_INFO', null);
					uni.showToast({
						icon: 'success',
						title: '加入成功',
						duration: 1500
					});
					setTimeout(() => {
						uni.switchTab({
							url: 'pages/user/user'
						});
					}, 1500);
				}
				catch{
					uni.hideLoading();
				}
		
			}
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

	.common_btn {
		margin-top: 230rpx;
	}
</style>
