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
				<label class="dept-item border-bottom" v-if="item.children==null|| item.children.length==0" @click="radioClick(item)">
					<view>
						<radio :checked="ifIn(item)" color="#50A6FA" style="transform:scale(0.7)" />
					</view>
					<view class="ltxt">{{item.label}}</view>
				</label>
				<view @click="jmp2Next(item)" class="dept-item border-bottom" v-else>
					<view @click.stop="radioClick(item)">
						<radio :checked="ifIn(item)" color="#50A6FA" style="transform:scale(0.7)" />
					</view>
					<view class="ltxt">{{item.label}}</view>
					<uni-icons v-if="item.children&&item.children.length>0" type="forward" size="22" color="#999999">
					</uni-icons>
				</view>
			</navigator>

			<empty v-if="curdep.children.length==0&&curdep.memlist.length==0" imgsrc="/static/empty/data_empty.png"
				txt="没有可以选择的部门"></empty>
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
						<!-- <view class="usrtg" v-else @click="radioClick(it)">
							<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="it.Avatar"
								@error="hdImgErr(it)" />
						</view> -->
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
		reloadPrePage,
		setPagesParam
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
			// console.log("options", options);
			this.depid = parseInt(options.id || "0");
			this.parentId = parseInt(options.in || "0");
			if (this.depid != 0) {
				this.selected = this.$store.state.submitMod.formArrary.pop();
			}
			let usrInfo = await this.$store.dispatch("userInfo");
			let rsp = await getDeptTree({
				OrgId: usrInfo.OrgId
			});
			let reqId = this.depid == 0 ? rsp.data[0].id : this.depid;
			// console.log("部门rsp", rsp, this.depid);
			this.dpmap = new Map();
			this.dpmap.set(rsp.data[0].id, rsp.data[0]);
			this.initDeptMap(rsp.data[0]);

			if (this.depid == 0) {
				this.curdep = rsp.data[0]
			} else {
				this.curdep = this.dpmap.get(this.depid);
			}

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
				this.selected = []
				this.selected.push(item)

				this.$forceUpdate();
			},
			ifIn(item) {
				return this.selected && this.selected.length > 0 && this.selected[0].id == item.id;
			},
			jmp2Next(item) {
				if (item.children && item.children.length > 0) {
					this.$store.state.submitMod.formArrary.push(this.selected);
					// console.log("item", item);
					this.parentId = item.parentId
					uni.navigateTo({
						url: '/pages/depman/only_dept_import?in=' + item.parentId + '&id=' + item.id
					});
				}
			},
			async confirmClick() {

				try {
					// console.log("选中的部门",this.selected);
					setPagesParam('choiceDept',this.selected[0],this.navlist.length,1000)
					// setTimeout(() => {
					// 	uni.navigateBack({
					// 		delta: this.navlist.length
					// 	});
					// }, 1500);
				} catch (err) {
					console.info('异常：', err);
					uni.hideLoading();
				}

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
