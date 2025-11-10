<template>
	<view>
		<hx-navbar :config="navconfig" @clickBtn="onClickBtn" @searchClick="jmp2Search">
		</hx-navbar>
		<view class="nav-mb-wrap" v-if="curdep!=null&&curdep.parentId!=0">
			<view class="nav-mb">
				<template v-for="(navit,idx) in navlist">
					<navigator url="" @click="backDep(idx)" v-if="(idx+1)!=navlist.length" class="xi ac">
						{{navit.label}}
					</navigator>
					<uni-icons v-if="(idx+1)!=navlist.length" type="forward" size="16" color="#999999"
						style="margin: 0 10rpx;" class="xi"></uni-icons>
					<text v-if="(idx+1)==navlist.length" class="xi">{{navit.label}}</text>
				</template>
			</view>
		</view>

		<view style="height: 70rpx;" v-if="curdep!=null&&curdep.parentId!=0"></view>
		<view class="page-contain">
			<view v-if="curdep==null" style="padding-top: 60rpx;">
				<uni-load-more iconType="circle" :showText="false" status="loading" />
			</view>
			<empty v-else-if="curdep.children==null&&curdep.memlist.length==0" imgsrc="/static/empty/data_empty.png"
				txt="暂无数据"></empty>
			<view v-else>
				<view class="item-field" v-for="item in curdep.children">
					<navigator class="item-nav" :url="'/pages/depman/depman?id='+item.id" animation-type="none">
						<view class="l">{{item.label}}</view>
						<view class="r">
							<uni-icons type="forward" size="22" color="#999999"></uni-icons>
						</view>
					</navigator>
				</view>

				<navigator :url="'/pages/user/user-detail?id=' + mem.Id" v-for="mem in curdep.memlist" :key="mem.Id"
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

		<view class="bottom-fix" v-if="max>1">
			<view style="padding: 0 30rpx;">
				<view class="select-area">
					<view class="l" v-if="selected.length==0"><text style="color: #999;font-size: 24rpx;">请选择人员</text>
					</view>
					<view class="l" v-else>
						<fr-image v-for="item in selected" class="ximg-sel" :lazy-load="true" mode="aspectFill"
							:src="item.Headimg" @error="hdImgErr(item)" loading-ing-img="two-balls" />
					</view>
					<button class="mini-btn" type="primary" size="mini" @click="confirmClick()">确定</button>
				</view>
			</view>

		</view>
	</view>
</template>

<script>
	import {
		getDeptTree
	} from '@/api/dept'
	import {
		getUserList
	} from '@/api/user'
	export default {
		data() {
			return {
				navconfig: {
					leftButton: [{
						key: 'btn1',
						txt: '关闭',
						position: null
					}],
					search: {
						value: '',
						placeholder: '请输入关键词',
						disabled: true
					}
				},
				curdep: null,
				dpmap: new Map(),
				navlist: [],
				depid: 0
			};
		},
		onLoad: function(options) {
			this.depid = parseInt(options.id || "0");
		},
		onShow() {
			this.reloadpage();
		},
		methods: {
			jmp2Search() {
				uni.navigateTo({
					url: '/pages/depman/user-search',
					animationType: "none"
				});
			},
			levAdd(dpid) {
				let xdd = this.dpmap.get(dpid);
				if (xdd.parentId != 0) {
					this.levAdd(xdd.parentId);
				}
				this.navlist.push(xdd);
			},
			initDeptMap(node) {
				for (let idx = 0; idx < node.children.length; idx++) {
					let curnode = node.children[idx];
					this.dpmap.set(curnode.id, curnode);
					if (curnode.hasOwnProperty("children")) {
						this.initDeptMap(curnode);
					}
				}
			},
			async reloadpage() {
				let usrInfo = await this.$store.dispatch("userInfo");
				let rsp = await getDeptTree({
					OrgId: usrInfo.OrgId
				});
				this.dpmap = new Map();
				this.dpmap.set(rsp.data[0].id, rsp.data[0]);
				this.initDeptMap(rsp.data[0]);
				if (this.depid == 0) {
					this.curdep = rsp.data[0]
				} else {
					this.curdep = this.dpmap.get(this.depid);
				}
			
				let xrsp = await getUserList({
					"deptId": this.curdep.id,
					"showAll": true
				});
				this.curdep.memlist = xrsp.data.List;
				this.navlist = [];
				this.levAdd(this.curdep.id);
			},
			
			hdImgErr: function(item) {
				if (item.Sex == 1) {
					item.Headimg = '/static/user/user_girl.png';
				} else {
					item.Headimg = '/static/user/user_man.png';
				}
			},
			backDep(idx) {
				uni.navigateBack({
					delta: (idx + 1)
				});
			}
		}
	}
</script>

<style lang="scss">
	.page-contain {
		padding: 20rpx 30rpx 20rpx 30rpx;
	}
	
	.nav-mb-wrap {
		position: fixed;
		width: 100vw;
	
		.nav-mb {
			background-color: #FFFFFF;
			display: flex;
			flex-direction: row;
			flex-wrap: nowrap;
			overflow-x: scroll;
			height: 70rpx;
			padding: 0 30rpx;
			align-items: center;
	
			.xi {
				flex-shrink: 0;
			}
	
			.ac {
				color: #50A6FA;
			}
		}
	}
	
	
	.item-field {
		background: #FFFFFF;
		border-radius: 6rpx;
		margin-bottom: 20rpx;
	
		.item-nav {
			display: flex;
			flex-direction: row;
			justify-content: space-between;
			align-items: center;
			height: 90rpx;
			padding: 0 30rpx;
	
			.l {
				font-size: 28rpx;
				font-weight: bold;
				color: #333;
			}
		}
	
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

	.bottom-fix {
		position: fixed;
		z-index: 10;
		bottom: 0;
		width: 100vw;
		background-color: #FFFFFF;

		.select-area {
			display: flex;
			flex-direction: row;
			align-items: center;
			height: 80rpx;

			.l {
				flex: 1;
				display: flex;
				flex-direction: row;
				flex-wrap: wrap;
			}
		}
	}

	.ximg-sel {
		display: flex;
		overflow: hidden;
		width: 60rpx;
		height: 60rpx;
		border-radius: 50%;
		margin-right: 20rpx;
	}
</style>
