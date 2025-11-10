<template>
	<page-meta :page-style="`overflow:${pageScrollFlag?'hidden':'visible'}`"></page-meta>
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
			<view v-else-if="curdep.children || curdep.memlist.length>0">
				<drag-list ref="dragRf" :list="curdep.children" :itemHeight="133" @change="sortChange"
					@StartMove="this.pageScrollFlag=true" @EndMove="this.pageScrollFlag=false">
					<template slot-scope="{item}">
						<view class="item-field">
							<navigator class="item-nav" :url="'/pages/depman/depman?id='+item.id" animation-type="none">
								<view class="l">
									<image src="../../static/mulu.png" mode="aspectFill"
										style="width: 68rpx;height: 68rpx;margin-right: 20rpx;"></image>
									<text>{{item.label}}</text>
								</view>
								<view class="r">
									<uni-icons type="forward" size="22" color="#999999"></uni-icons>
								</view>
							</navigator>
						</view>
						<view style="height: 20rpx;"></view>
					</template>
				</drag-list>
				<view :style="{'height':depTop}"></view>
				<navigator :url="'/pages/depman/member_edit?id=' + mem.Id+'&pageLen='+navlist.length" v-for="mem in curdep.memlist" :key="mem.Id"
					class="u-item">
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
			<view v-else style="padding-bottom: 10rpx;">
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
				<J-skeleton :loading="true" :showTitle="true" :row="2"></J-skeleton>
			</view>

		</view>
		<view class="bottom-fix">
			<navigator class="addmem-ss border-right" :url="'/pages/depman/user-yq?dp='+curdep.id">邀请员工</navigator>
			<navigator class="add-ss border-right" url="" @click="addDept">添加子级部门</navigator>
			<navigator class="setdep-ss" url="" @click="onMore" v-if="curdep!=null&&curdep.parentId!=0">更多管理</navigator>
			<navigator class="setdep-ss" url="" @click="onImort" v-else>移入部门</navigator>
		</view>
	</view>
</template>

<script>
	import {
		getDeptTree,
		sortDept,
		delDept
	} from '@/api/dept'
	import {
		getUserList
	} from '@/api/user'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import dragList from '@/components/gzz-drag/drag-list.vue';
	export default {
		components: {
			dragList
		},
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
				depid: 0,
				pageScrollFlag: false
			};
		},
		onLoad: function(options) {
			this.depid = parseInt(options.id || "0");
		},
		computed: {
			depTop() {
				if (this.curdep == null || this.curdep.children == null) {
					return "0rpx";
				}
				return this.curdep.children.length * 133 + 'rpx';
			}
		},
		onShow() {
			this.reloadpage();
		},
		methods: {
			callTel(tel) {
				uni.makePhoneCall({
					phoneNumber: tel
				});
			},
			async sortChange(list) {
				let usrInfo = await this.$store.dispatch("userInfo");
				let idlist = list.map(x => {
					return x.id;
				});

				await sortDept({
					idList: idlist,
					orgId: usrInfo.OrgId
				});
			},
			jmp2Search() {
				uni.navigateTo({
					url: '/pages/depman/user-search',
					animationType: "none"
				});
			},
			levAdd(dpid) {
				let xdd = this.dpmap.get(dpid);
				// console.log("部门树？", xdd);
				if (xdd.parentId != 0) {
					this.levAdd(xdd.parentId);
				}
				this.navlist.push(xdd);
			},
			async initDeptMap(node) {
				for (let idx = 0; idx < node.children.length; idx++) {

					let curnode = node.children[idx];

					// console.log("组合时用户列表curnode",curnode);
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
				let reqId = this.depid == 0 ? rsp.data[0].id : this.depid;
				let xrsp = await getUserList({
					"deptId": reqId,
					"showAll": true,
					"deptIdWithChildren": false, //deptIdWithChildren为false的时候表示需要通过子级筛选
				});
				this.dpmap = new Map();
				this.dpmap.set(rsp.data[0].id, rsp.data[0]);
				this.initDeptMap(rsp.data[0]);
				if (this.depid == 0) {
					this.curdep = rsp.data[0]
				} else {
					this.curdep = this.dpmap.get(this.depid);
				}
				// console.log("bumen  this.curdep",this.depid,this.curdep,this.dpmap.get(this.depid));
				this.curdep.memlist = xrsp.data.List;
				// console.log("成员列表",this.curdep.memlist);
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
					delta: idx
				});
			},
			addDept() {
				this.$store.state.submitMod.formArrary.push(this.curdep);
				uni.navigateTo({
					url: '/pages/depman/dept_edit'
				});
			},
			onClickBtn(data) {

				if (data.key == "btn1") {
					uni.navigateBack({
						delta: this.navlist.length
					});
				}
			},
			onImort() {
				uni.navigateTo({
					url: '/pages/depman/dept_import?in=' + this.curdep.id
				});
			},
			onMore() {
				// console.log("部门信息",this.curdep);
				uni.showActionSheet({
					itemList: ['修改当前部门名称', '从其他部门移入', '删除当前部门'],
					success: res => {
						if (res.tapIndex == 0) {
							this.$store.state.submitMod.formArrary.push(this.curdep);
							uni.navigateTo({
								url: '/pages/depman/dept_edit?id=' + this.curdep.id
							});
						} else if (res.tapIndex == 1) {
							this.onImort();
						} else if (res.tapIndex == 2) {
							uni.showModal({
								title: '提示',
								content: '确定删除'+this.curdep.label+'吗？',
								success: async res => {
									if (res.confirm) {
										uni.showLoading({
											title: '加载中...'
										});
										delDept(this.curdep.id).then(rsp => {
											uni.hideLoading();
											uni.showToast({
												icon: 'success',
												title: '删除成功',
												duration: 1000
											});
											setTimeout(() => {
												uni.navigateBack();
											}, 1000);
										}).catch(err => {
											uni.hideLoading();
										});
									}
								}
							})

						}

					}
				});
			}
		}
	}
</script>

<style lang="scss">
	.page-contain {
		padding: 20rpx 0 110rpx 0;

	}

	page {
		background-color: #FFFFFF;
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
		border-top: 1rpx solid rgba(153, 153, 153, 0.1);
		padding: 0 30rpx;

		.item-nav {
			display: flex;
			flex-direction: row;
			justify-content: space-between;
			align-items: center;
			height: 130rpx;


			.l {
				font-size: 28rpx;
				font-weight: bold;
				color: #333;
				display: flex;
				align-items: center;
			}
		}

	}

	.u-item {
		height: 130rpx;
		background-color: #FFFFFF;
		display: flex;
		flex-direction: row;
		align-items: center;
		padding: 0rpx 30rpx;
		border-top: 1rpx solid rgba(153, 153, 153, 0.1);

		.ximg {
			display: flex;
			overflow: hidden;
			width: 68rpx;
			height: 68rpx;
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

	.u-item:last-child {
		border-bottom: 1rpx solid rgba(153, 153, 153, 0.1);
	}

	.bottom-fix {
		position: fixed;
		z-index: 10;
		bottom: 0;
		width: 100vw;
		display: flex;
		flex-direction: row;
		background-color: #FFFFFF;
		box-shadow: 0px 0px 8px rgba(0, 0, 0, 0.1);


		.addmem-ss,
		.add-ss,
		.setdep-ss {
			display: flex;
			height: 100rpx;
			flex: 1;
			color: #50A6FA;
			font-size: 28rpx;
			background-color: #FFFFFF;
			justify-content: center;
			align-items: center;
		}
	}
</style>
