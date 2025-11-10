<template>
	<view class="tree-item">
		<view class="head" @click.stop="changeli" :class="{'input_sel':isInputSel}">
			<view class="left_con">
				<view @click.stop="changeShow" v-if="!isInputSel" :class="{'flex_icon':!isInputSel}">
					<view class="left-icon t-icon-zhankai"
						v-if="item[defaultProps.children] && item[defaultProps.children].length > 0&&!show"></view>
					<view class="left-icon t-icon-shouqi rt45"
						v-if="!item[defaultProps.children]||item[defaultProps.children].length==0 ||show"></view>
					<view class="img_con">
						<image class="image" :src="item[defaultProps.photoUrl]+'?wh=500x500'" mode=""></image>
					</view>
				</view>
				<text class="txt">{{item[defaultProps.label]}}</text>
			</view>
			<view class="right_icon" @click.stop="editItemNode(item)" v-if="!isInputSel">
				<custom-icons iconsName="icon-bianji" iconsSize="28rpx"></custom-icons>
			</view>
			<view class="right_icon"
				v-if="isInputSel&&item[defaultProps.children] && item[defaultProps.children].length > 0&&!show"
				@click.stop="changeShow">
				<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"></custom-icons>
			</view>
			<view class="right_icon xuanzhuan_top" v-if="isInputSel&&show" @click.stop="changeShow">
				<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"></custom-icons>
			</view>
		</view>
		<view class="content" v-if="item[defaultProps.children] && item[defaultProps.children].length > 0" v-show="show"
			:class="{'input_sel_content':isInputSel}">
			<tree-node v-for="sitem in item[defaultProps.children]" :item="sitem" :key="sitem[defaultProps.id]"
				:defaultProps="defaultProps"></tree-node>
		</view>
	</view>
</template>

<script>
	import TreeNode from './tree-node.vue'
	export default {
		name: 'TreeNode',
		componentName: 'TreeNode',
		props: {
			item: {
				type: Object,
				default: () => {
					return {}
				}
			}
		},
		inject: ['defaultProps', 'onClickItem', 'editItem', 'isInputSel'],
		data() {
			return {
				show: false
			}
		},
		beforeMount() {
			if (!this.isInputSel) {
				this.show = true
			}
		},
		methods: {
			editItemNode(item) {
				this.editItem(item)
			},
			changeShow() {

				if (this.item[this.defaultProps.children] && this.item[this.defaultProps.children].length > 0) {
					this.show = !this.show;
				}
			},
			changeli() {
				this.onClickItem(this.item);
			},
		}
	}
</script>

<style scoped lang="scss">
	@mixin animate2 {
		-moz-transition: all .2s linear;
		-webkit-transition: all .2s linear;
		-o-transition: all .2s linear;
		-ms-transition: all .2s linear;
		transition: all .2s linear;
	}

	.tree-item {
		.head {
			display: flex;
			justify-content: space-between;
			align-items: center;
			line-height: 100rpx;

			&.input_sel {
				line-height: 80rpx;

				.txt {
					font-size: 28rpx;
					color: #FFF;
				}
			}

			.left_con {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.flex_icon {
					display: flex;
					justify-content: flex-start;
					align-items: center;
				}
			}

			.txt {
				font-size: 32rpx;
				color: #FFF;
			}

			.right_icon {
				&.xuanzhuan_top {
					transform: rotate(180deg);
					-webkit-transform: rotate(180deg);
					transition: transform ease 0.5s;
				}
			}
		}

		.left-icon {
			width: 30rpx;
			height: 30rpx;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			margin-right: 20rpx;
			@include animate2;
			transform: rotate(-90deg);
			-ms-transform: rotate(-90deg);
			-moz-transform: rotate(-90deg);
			-webkit-transform: rotate(-90deg);
			-o-transform: rotate(-90deg);

			&.rt45 {
				transform: rotate(0deg);
				-ms-transform: rotate(0deg);
				-moz-transform: rotate(0deg);
				-webkit-transform: rotate(0deg);
				-o-transform: rotate(0deg);
			}
		}

		.img_con {
			width: 40rpx;
			height: 40rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			margin-right: 20rpx;

			.image {
				width: 40rpx;
				height: 40rpx;
			}
		}

		.content {
			padding-left: 40rpx;

			&.input_sel_content {
				padding-left: 20rpx;
			}
		}
	}
</style>