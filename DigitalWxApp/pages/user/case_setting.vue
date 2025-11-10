<template>
	<view>
		<view class="list" style="margin-top: 20rpx;">
			<navigator class="row" url="/pages/user/case_setting_lunbo">
				<text>轮播图</text>
				<view>
					<uni-icons type="right" color="#666666" size="20"></uni-icons>
				</view>
			</navigator>
			<navigator class="row" url="/pages/user/case_setting_title">
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
				<text>案例展示</text>
				<view>
					<switch @change="getCase" :checked="showCase"/>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		addSetting
	} from '@/api/example.js'
	import {
		getOrg
	} from '@/api/org.js'
	export default {
		data() {
			return {
				search: false,//设置搜索是否显示
				showCase:false,//案例中心是否显示
				orgInfo: null,
				userInfo: null,
				CaseConfig: {}, //案例设置信息
				config: {},
				typeList:[],
			}
		},
		onLoad() {
			this.reloadPages()

		},
		methods: {
			async reloadPages() {
				this.userInfo = await this.$store.dispatch("userInfo");
				this.orgInfo = await getOrg(this.userInfo.OrgId);
				this.CaseConfig = this.orgInfo.data.CaseConfig;
				if (this.CaseConfig) {
					this.config = JSON.parse(this.CaseConfig);
					let config2=JSON.parse(this.CaseConfig)
					if(config2[0]){//判断config2是不是一个数组
						this.config={
							oldInfo:config2
						}
					}
					if(this.config.oldInfo){
						this.config.oldInfo.map((item,index)=>{
							if(item.type=='search'){
								this.search=item.data
							}
							if(item.type=='showCase'){
								this.showCase=item.data
							}
						})
					}
					// console.log("转化后案例设置信息this.config", this.config);
					if (this.config.search) {
						this.search = this.config.search
					}
					if (this.config.showCase) {
						this.showCase = this.config.showCase
					}
				}
				if(this.CaseConfig=='[]'){
					this.config={}
				}

			},
			checkFun(typeStr,data){//添加修改设置信息
				this.config.search=this.search
				this.config.showCase=this.showCase
				addSetting({
					OrgId: this.userInfo.OrgId,
					caseConfig: JSON.stringify(this.config)
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
				this.search=!this.search
				this.checkFun();

			},
			getCase() {
				this.showCase=!this.showCase
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
