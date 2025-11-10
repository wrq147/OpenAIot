<template>
	<view>
		<uni-nav-bar :status-bar="false" :fixed="fixed" :border="false" height="88rpx" :zIndex="zIndex"
			:backgroundColor="backgroundColor">
			<template v-slot:allslot>
				<view class="search_con" :class="{'noLeftpadding':noLeftpadding}">
					<view class="search" :class="{'only_search':isOnlySearch}">
						<uni-easyinput prefixIcon="icon-sousuo" :placeholder="pal"
							v-model="key" clearSize="18" @iconClick="searchKey" @confirm="searchKey"
							:prefixIconSize="12" placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:28rpx"
							@clear="clearNull" :styles="customstyles" primaryColor="rgba(255, 255, 255, 0.5)"
							inputHeight="64rpx" :isCustom="true" prefixIconFocusColor="rgba(255, 255, 255, 0.5)"
							prefixIconColor="rgba(255, 255, 255, 0.2)">
						</uni-easyinput>
					</view>
					<view class="text" @click.stop="openSelect" v-if="!isOnlySearch">
						<custom-icons iconsName="icon-shaixuan" iconsSize="34rpx"
							iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
					</view>
				</view>
			</template>
		</uni-nav-bar>
	</view>
</template>

<script>
	export default {
		name: "search-compt", //搜索主键
		props: {
			fixed: {
				type: [Boolean, String],
				default: true
			},
			noLeftpadding: { //设置最外层是否需要边框
				type: Boolean,
				default: false
			},
			backgroundColor: {
				type: String,
				default: ''
			},
			isOnlySearch: { //是否只有搜索框
				type: Boolean,
				default: false
			},
			pal:{//提示词
				type: String,
				default: 'Please enter the name or number'
			},
			zIndex:{
				type:Number,
				default:995
			}
		},
		data() {
			return {
				key: '',
				customstyles: {
					color: '#ffffff',
					backgroundColor: '#161A26',
					disableColor: '#F7F6F6',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				}
			};
		},
		methods: {
			openSelect() {
				//打开弹窗
				this.$emit('openSelect')
			},
			searchKey() {
				this.$emit("searching", this.key)
			},
			clearNull() {
				this.$emit("searching", this.key)
			}
		}
	}
</script>

<style lang="less" scoped>
	.search_con {
		width: 100%;
		padding: 12rpx 20rpx;
		box-sizing: border-box;
		background-color: #161A26;
		height: 88rpx;
		display: flex;
		justify-content: space-between;
		align-items: center;

		&.noLeftpadding {
			padding-left: 0;
			padding-right: 0;
		}

		.search {
			width: 644rpx;
			height: 64rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			background-color: #161A26;
			// padding: 0 6rpx 0 20rpx;
			box-sizing: border-box;
			border-radius: 10rpx;
			

			.input_text {
				width: 644rpx;
				font-size: 28rpx;
			}
			&.only_search{
				width: 100%;
				.input_text{
					width: 100%;
				}
			}

		}

		.text {
			font-size: 28rpx;
			color: #999999;
		}
	}
</style>