<template>
	<view>
		<uni-nav-bar :status-bar="statusbar" :fixed="fixed" :border="false" height="88rpx" :zIndex="zIndex"
			:backgroundColor="isNoBg?'':backgroundColor">
			<template v-slot:allslot>
				<view class="search_con" :class="{'noLeftpadding':noLeftpadding}"
					:style="{'background-color':isNoBg?'':backgroundColor}">
					<view class="search" :class="{'only_search':isOnlySearch}">
						<uni-easyinput prefixIcon="icon-sousuo" :placeholder="pal" v-model="key" clearSize="36rpx"
							@iconClick="searchKey" @confirm="searchKey" :prefixIconSize="12"
							placeholderStyle="color:#D8D8D8;font-size:28rpx" @clear="clearNull" :styles="customstyles" inputHeight="64rpx" :isCustom="true" prefixIconFocusColor="#D8D8D8"
							prefixIconColor="#D8D8D8" @input="searchInput" @focus="searchFocus">
						</uni-easyinput>
					</view>
					<view class="text" @click.stop="openSelect" v-if="!isRightText&&!isOnlySearch">
						<custom-icons iconsName="icon-shaixuan" iconsSize="34rpx" iconsColor="#999999"></custom-icons>
					</view>
					<view class="text" @click.stop="closeSearch" v-if="isRightText">
						取消
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
			statusbar: {
				type: Boolean,
				default: false
			},
			backgroundColor: {
				type: String,
				default: '#fff'
			},
			isNoBg: {
				type: Boolean,
				default: false
			},
			isOnlySearch: { //是否只有搜索框
				type: Boolean,
				default: false
			},
			pal: { //提示词
				type: String,
				default: '请输入名称或者是编码'
			},
			zIndex: {
				type: Number,
				default: 995
			},
			inputBg: {
				type: String,
				default: '#fff'
			},
			isRightText:{ //右侧是否是文本
				type: Boolean,
				default: false
			},
			hasKey:{//已经填写的过滤关键字
				type: String,
				default: ''
			}
		},
		watch: {
			inputBg(newVal, oldVal) {
				// 当 count 发生变化时触发
				this.customstyles.backgroundColor = this.inputBg
			}
		},
		data() {
			return {
				key: '',
				customstyles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F7F6F6',
					borderColor: '#F8F8F8'
				}
			};
		},
		beforeMount() {
			this.customstyles.backgroundColor = this.inputBg
		},
		mounted() {
			if(this.hasKey){
				this.key=this.hasKey
			}
		},
		methods: {
			setsearchval(val){
				this.key=val
			},
			searchFocus(){
				this.$emit('searchFocus')
			},
			closeSearch(){
				//关闭打开的搜索弹出
				this.$emit('closeSearch')
			},
			openSelect() {
				//打开弹窗
				this.$emit('openSelect')
			},
			searchKey() {
				this.$emit("searching", this.key)
			},
			clearNull() {
				this.$emit('clearSearch')
				this.$emit("searching", this.key)
			},
			searchInput(){
				this.$emit('searchInput',this.key)
			}
		}
	}
</script>

<style lang="less" scoped>
	.search_con {
		width: 100%;
		padding: 12rpx 20rpx;
		box-sizing: border-box;
		// background-color: #161A26;
		height: 88rpx;
		display: flex;
		justify-content: space-between;
		align-items: center;
		z-index: 9;

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
			// background-color: #161A26;
			background-color: #FFFFFF;
			// padding: 0 6rpx 0 20rpx;
			box-sizing: border-box;
			border-radius: 10rpx;


			.input_text {
				width: 644rpx;
				font-size: 28rpx;
			}

			&.only_search {
				width: 100%;

				.input_text {
					width: 100%;
				}
			}

		}

		.text {
			font-size: 28rpx;
			color: #999999;
			z-index: 9;
		}
	}
</style>