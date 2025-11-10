<template>
	<view>
		<view class="list" style="margin-top: 20rpx;">
			<navigator class="row" url="/pages/user/pro_settings_lunbo">
				<text>轮播图</text>
				<view>
					<uni-icons type="right" color="#666666" size="20"></uni-icons>
				</view>
			</navigator>
			<navigator class="row" url="/pages/user/pro_settings_title">
				<text>标题</text>
				<view>
					<uni-icons type="right" color="#666666" size="20"></uni-icons>
				</view>
			</navigator>
		</view>

		<view class="list">
			<view class="row">
				<text>搜索框</text>
				<view>
					<switch @change="getSearch" :checked="search" />
				</view>
			</view>
			<view class="row">
				<text>产品分类展示</text>
				<view>
					<switch @change="getCategory" :checked="showCategory" />
				</view>
			</view>
			<view class="row">
				<text>产品展示</text>
				<view>
					<switch @change="getPro" :checked="showPro" />
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		addSetting
	} from '@/api/product.js'
	import {
		getOrg
	} from '@/api/org.js'
	export default {
		data() {
			return {
				search: false, //设置搜索是否显示
				showCategory: false, //设置分类是否显示
				showPro: false, //产品中心是否显示
				orgInfo: null,
				userInfo: null,
				ProConfig: {}, //产品设置信息
				config: {},
				typeList: [],
			}
		},
		onLoad() {
			this.reloadPages()

		},
		methods: {
			async reloadPages() {
				this.userInfo = await this.$store.dispatch("userInfo");
				this.orgInfo = await getOrg(this.userInfo.OrgId);
				this.ProConfig = this.orgInfo.data.ProConfig;
				if (this.ProConfig) {
					this.config = JSON.parse(this.ProConfig);
					let config2=JSON.parse(this.ProConfig)
					if(config2[0]){
						this.config={
							oldInfo:config2
						}
					}
					if(this.config.oldInfo){
						this.config.oldInfo.map((item,index)=>{
							// console.log("子级数据",item);
							if(item.type=='search'){
								this.search=item.data
							}
							if(item.type=='showCategory'){
								this.showCategory=item.data
							}
							if(item.type=='showPro'){
								this.showPro=item.data
								// console.log("商品展示情况",this.showPro);
							}
						})
					}
					// console.log("转化后产品设置信息this.config", this.config);
					if (this.config.search) {
						this.search = this.config.search
					}
					if (this.config.showCategory) {
						this.showCategory = this.config.showCategory
					}
					if (this.config.showPro) {
						this.showPro = this.config.showPro
					}
				}

			},
			checkFun() {
				this.config = {
					search:this.search,
					showCategory:this.showCategory,
					showPro:this.showPro
				}
				addSetting({
					OrgId: this.userInfo.OrgId,
					proConfig: JSON.stringify(this.config)
				}).then(re => {
					if (re.code == 0) {
						uni.showToast({
							icon: 'success',
							title: '设置成功'
						});
					}
				})
			},
			getSearch() {
				this.search = !this.search
				this.checkFun();

			},
			getCategory() {
				this.showCategory = !this.showCategory
				this.checkFun();

			},
			getPro() {
				this.showPro = !this.showPro
				this.checkFun();

			},

		}
	}
</script>

<style lang="scss">
	.list {
		width: 100%;
		margin-bottom: 30rpx;

		.row {
			width: 100%;
			height: 120rpx;
			line-height: 120rpx;
			border-bottom: 1rpx solid #F6F6F6;
			background-color: #fff;
			display: flex;
			justify-content: space-between;
			color: #666666;
			font-family: pfzho;
			box-sizing: border-box;
			padding: 0 30rpx;

			switch {
				transform: scale(0.7)
			}
		}
	}
</style>
