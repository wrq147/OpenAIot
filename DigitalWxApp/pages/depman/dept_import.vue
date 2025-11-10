<template>
	<view>
		<template v-if="curdep!=null&&curdep.parentId!=0">
			<view class="nav-mb-wrap">
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
			<view class="border-bottom" style="height: 70rpx;"></view>
		</template>
		<view v-else class="border-top"></view>

		<view class="dept-cont" v-if="curdep!=null">
			<navigator url="" v-for="(item, index) in curdep.children" :key="item.id">
				<label class="dept-item border-bottom" v-if="item.children==null" @click="radioClick(item)">
					<view>
						<radio :disabled="isDis(item)" :checked="ifIn(item)" color="#50A6FA"
							style="transform:scale(0.7)" />
					</view>
					<view class="ltxt">{{item.label}}</view>
				</label>
				<view @click="jmp2Next(item)" class="dept-item border-bottom" v-else>
					<view @click.stop="radioClick(item)">
						<radio :disabled="isDis(item)" :checked="ifIn(item)" color="#50A6FA"
							style="transform:scale(0.7)" />
					</view>
					<view class="ltxt">{{item.label}}</view>
					<uni-icons type="forward" size="22" color="#999999"></uni-icons>
				</view>
			</navigator>
			<navigator url="" class="mems" v-for="mem in curdep.memlist" :key="mem.Id">
				<label class="mem-item border-bottom" @click="radioClick(mem)">
					<view>
						<radio :checked="ifIn(mem)" color="#50A6FA" style="transform:scale(0.7)" />
					</view>
					<view class="row-cc">
						<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="mem.Avatar"
							@error="hdImgErr(mem)" />

						<text class="name">{{mem.RealName}}</text>
					</view>
				</label>
			</navigator>

			<empty v-if="curdep.children.length==0&&curdep.memlist.length==0" imgsrc="/static/empty/data_empty.png"
				txt="没有可以导入的人员与部门"></empty>
		</view>


		<view class="bottom-fix" v-if="selected.length>0">
			<view class="select-area">
				<view class="l">
					<view v-for="it in selected">
						<view class="tag" v-if="it.RealName==null" @click="radioClick(it)">
							{{it.label}}
							<uni-icons type="closeempty" size="12" color="#ffffff" style="margin-left: 10rpx;">
							</uni-icons>
						</view>
						<view class="usrtg" v-else @click="radioClick(it)">
							<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="it.Avatar"
								@error="hdImgErr(it)" />
						</view>
					</view>
				</view>
				<button class="mini-btn" type="primary" size="mini" @click="confirmClick()">确定</button>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getDeptTree,
		moveDept
	} from '@/api/dept'
	import {
		getUserList
	} from '@/api/user'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				curdep: null,
				dpmap: new Map(),
				navlist: [],
				depid: 0,
				parentId: 0,
				selected: []
			};
		},
		async onLoad(options) {

			this.depid = parseInt(options.id || "0");
			this.parentId = parseInt(options.in);
			if (this.depid != 0) {
				this.selected = this.$store.state.submitMod.formArrary.pop();
			}
			let usrInfo = await this.$store.dispatch("userInfo");
			let rsp = await getDeptTree({
				OrgId: usrInfo.OrgId
			});
			let reqId = this.depid == 0 ? rsp.data[0].id : this.depid;
			let xrsp = await getUserList({
				"deptId": reqId,
				"showAll": true,
				"deptIdWithChildren": false,
			});

			// console.log("部门rsp",rsp);
			this.dpmap = new Map();
			this.dpmap.set(rsp.data[0].id, rsp.data[0]);
			this.initDeptMap(rsp.data[0]);
			if (this.depid == 0) {
				this.curdep = rsp.data[0]
			} else {
				this.curdep = this.dpmap.get(this.depid);
			}
			this.curdep.memlist = xrsp.data.List;
			this.navlist = [];
			this.levAdd(this.curdep.id);
		},
		methods: {
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
			radioClick(item) {
				if (item.RealName == null) {
					if (item.parentId == this.parentId || item.id == this.parentId) {
						return;
					}
					let idx = this.selected.findIndex(it => it.id == item.id && it.label != null);
					if (idx > -1) {
						this.selected.splice(idx, 1);
					} else {
						this.selected.push(item);
					}
				} else {
					let idx = this.selected.findIndex(it => it.Id == item.Id && it.RealName != null);
					if (idx > -1) {
						this.selected.splice(idx, 1);
					} else {
						this.selected.push(item);
					}
				}

				this.$forceUpdate();
			},
			ifIn(item) {
				if (item.RealName != null) {
					return this.selected.findIndex(it => it.Id == item.Id && it.RealName != null) > -1;
				} else {
					return this.selected.findIndex(it => it.id == item.id && it.label != null) > -1;
				}
			},
			jmp2Next(item) {
				this.$store.state.submitMod.formArrary.push(this.selected);
				uni.navigateTo({
					url: '/pages/depman/dept_import?in=' + this.parentId + '&id=' + item.id
				});
			},
			isDis(item) {
				return item.parentId == this.parentId || item.id == this.parentId;
			},
			async confirmClick() {
				uni.showModal({
					title: '提示',
					content: '确定移入' + this.dpmap.get(this.parentId).label + '吗？',
					success: async res => {
						if (res.confirm) {
							uni.showLoading({
								title: '加载中...'
							});

							try {
								let depids = [];
								let uids = [];
								this.selected.forEach(it => {
									if (it.RealName == null) {

										depids.push(it.id);
									} else {
										uids.push(it.Id);
									}
								});
								await moveDept(depids, uids, this.parentId);
								uni.hideLoading();
								uni.showToast({
									icon: 'success',
									title: '移动成功',
									duration: 1500
								});
								setTimeout(() => {
									uni.navigateBack({
										delta: this.navlist.length
									});
								}, 1500);
							} catch (err) {
								console.info('异常：', err);
								uni.hideLoading();
							}
						}
					}
				})

			}
		}
	}
</script>

<style lang="scss">
	.nav-mb-wrap {
		position: fixed;
		width: 100vw;
		z-index: 99;

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

	.dept-cont {
		background-color: #FFFFFF;

		.dept-item {
			height: 100rpx;
			display: flex;
			align-items: center;
			padding: 0 30rpx;

			.ltxt {
				margin-left: 20rpx;
				flex: 1;
			}
		}

		.mems {
			display: flex;
			flex-direction: column;

			.mem-item {
				padding: 0 30rpx;
				display: flex;
				flex-direction: row;
				align-items: center;
				height: 100rpx;

				.row-cc {
					display: flex;
					flex-direction: row;
					align-items: center;

				}

				.ximg {
					display: flex;
					overflow: hidden;
					width: 68rpx;
					height: 68rpx;
					border-radius: 50%;
					margin-left: 10rpx;
					margin-right: 20rpx;
				}
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
			padding: 14rpx 30rpx;

			.l {
				flex: 1;
				display: flex;
				flex-direction: row;
				flex-wrap: wrap;
				align-items: center;
				margin-left: -20rpx;

				.tag {
					border-radius: 8rpx;
					background-color: #999999;
					color: #FFFFFF;
					font-size: 24rpx;
					height: 50rpx;
					padding: 0 16rpx;
					cursor: pointer;
					height: 50rpx;
					display: flex;
					align-items: center;
					margin-left: 20rpx;
				}

				.usrtg {
					margin-left: 20rpx;

					.ximg {
						display: flex;
						overflow: hidden;
						width: 68rpx;
						height: 68rpx;
						border-radius: 50%;
					}
				}

			}

			.mini-btn {
				background-color: #50A6FA;
				display: inline-flex;
				align-items: center;
			}
		}
	}
</style>
