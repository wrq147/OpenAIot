<template>
	<view>
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" :rightText="allSelect"
			rightWidth="157rpx" :isleftBack="true" backgroundColor="#fff" @clickRight="handleCheckAllChange"></top>
		<search-compt :isOnlySearch="true" pal="请输入名称" @searching="searchUser"
			backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<view class="employee_con">
			<view class="employee_ul">
				<view class="employee_li" v-if="nowDeptId&&nowDeptId!=null&&!search" @click.stop="beforeNode">
					<view class="li_left">
						<view class="bumenicons_con" v-if="deptStack.length==0">
							<custom-icons iconsName="icon-gongsi" iconsSize="28rpx" iconsColor="#333333"></custom-icons>
						</view>
						<view class="bumenicons_con" v-if="deptStack.length>0">
							<custom-icons iconsName="icon-bumenguanli-fanhuishangji" iconsSize="28rpx"
								iconsColor="#333333"></custom-icons>
						</view>
						<view class="bumenicons_con" v-if="deptStack.length>0">
							<custom-icons iconsName="icon-bumenguanli-bumentubiao" iconsSize="28rpx"
								iconsColor="#333333"></custom-icons>
						</view>
						<view class="li_text">
							{{deptStackStr}}
						</view>
					</view>
				</view>
				<!-- <view class="employee_li">
					<view class="li_left">
						<view class="bumenicons_con t-icon-bumenguanli-fanhuishangji"></view>
						<view class="li_text">
							Fujian Huade Group
						</view>
					</view>
				</view> -->
				<view class="employee_li" v-for="row in orgs" @click.stop="nextNode(row)">
					<view class="li_left">
						<view class="icons_con t-icon-gouxuan1" v-if="setSelectIcon(row)&&row.type=='user'"></view>
						<view class="icons_con t-icon-gouxuan1" v-if="setSelectIcon(row)&&type=='dept'"
							@click.stop="selectChange(row)"></view>
						<view class="icons_con" v-if="!setSelectIcon(row)&&row.type=='user'">
							<custom-icons iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="#EAEAEA"></custom-icons>
						</view>
						<view class="icons_con" v-if="!setSelectIcon(row)&&type=='dept'" @click.stop="selectChange(row)">
							<custom-icons iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="#EAEAEA"></custom-icons>
						</view>
						<!-- <view class="icons_con t-icon-weixuanzhong" v-if="!setSelectIcon(row)&&row.type=='user'"></view>
						<view class="icons_con t-icon-weixuanzhong" v-if="!setSelectIcon(row)&&type=='dept'"
							@click.stop="selectChange(row)"></view> -->
						<!-- <view class="icons_con t-icon-zuneiyixuan"></view> -->
						<image class="ava_image" :src="row.avatar" mode=""
							v-if="row.type=='user'&&$isNotEmpty(row.avatar)"></image>
						<view class="ava_image" v-if="row.type=='user'&&!$isNotEmpty(row.avatar)">
							{{getShortName(row.name)}}
						</view>
						<!-- <view class="bumenicons_con t-icon-bumenguanli-bumentubiao" v-if="row.type=='dept'"></view> -->
						<view class="bumenicons_con" v-if="row.type=='dept'">
							<custom-icons iconsName="icon-bumenguanli-bumentubiao" iconsSize="28rpx"
								iconsColor="#333333"></custom-icons>
						</view>
						<view class="li_text">
							{{row.name}}
						</view>
					</view>
					<view class="li_right" v-if="row.type=='dept'">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#333333"></custom-icons>
					</view>
					<view class="li_right" v-if="row.type=='user'">
						{{row.remark}}
					</view>
				</view>
			</view>
			<view class="alselect_con_zhanwei" v-if="multiple">

			</view>
			<view class="alselect_con_con" v-if="multiple&&type=='user'">
				<view class="alselect_con">
					<scroll-view class="alselect_ul" scroll-x="true">
						<view class="alselect_ul_con">
							<view class="select_li" v-for="item in select">
								<image class="avatar_image" :src="item.avatar" mode=""
									v-if="item.type=='user'&&$isNotEmpty(item.avatar)"></image>
								<view class="avatar_image" v-if="item.type=='user'&&!$isNotEmpty(item.avatar)">
									{{getShortName(item.name)}}
								</view>
							</view>
							<view class="btn_zhanwei">

							</view>
						</view>
					</scroll-view>
					<view class="com_btn_con">
						<view class="com_btn" @click="finishMulSelect">
							确定({{select.length}})
						</view>
					</view>
				</view>
			</view>
			<view class="alselect_con_con" v-if="multiple&&type=='dept'">
				<button class="submit_button" @click="finishMulSelect">
					确定({{select.length}})
				</button>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getOrgTree,
		getUserByName
	} from "@/api/user.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: '选择人员', //标题
				multiple: false, //是否是多选
				visible: false,
				loading: false,
				checkAll: false,
				nowDeptId: null,
				isIndeterminate: false,
				searchUsers: [],
				nodes: [],
				select: [],
				search: "",
				deptStack: [],
				type: "user", //org选择部门/人员  user-选人  dept-选部门 role-选角色
				deptUserInfo: {

				}
			};
		},
		onLoad(options) {
			if (options.multiple) {
				this.multiple = !!options.multiple
			}
			if (options.title) {
				this.topTitle = options.title
			}
			if (options.type) {
				this.type = options.type
			}
			if (options.selected) {
				let selected = JSON.parse(options.selected)
				this.select = Object.assign([], selected);
				this.selectToLeft();
			}
			// this.nowDeptId=126788920627269
			this.$nextTick(() => {
				this.getOrgList()
			})
		},
		computed: {
			deptStackStr() {
				let str = ''
				let arr = this.deptStack.map((v) => v.name)
				// console.log(arr,'arr');
				str = arr.join('>')
				// return String(this.deptStack.map((v) => v.name)).replaceAll(",", " > ");
				return str
			},
			orgs() {
				return !this.search || this.search.trim() === "" ?
					this.nodes :
					this.searchUsers;
			},
			showUsers() {
				return this.search || this.search.trim() !== "";
			},
			allSelect() {
				if (this.multiple) {
					if (this.checkAll) {
						return '取消全选'
					} else {
						return '全选'
					}
				} else {
					return ''
				}
			}
		},
		methods: {
			setSelectIcon(node) {
				//设置选中，未选中，有子项选中的图标样式
				if (this.type == 'user') {
					if (node.type == 'user') {
						return node.selected
					}
				} else {
					return node.selected
				}

			},
			handleCheckAllChange() {
				//全选事件
				this.checkAll = !this.checkAll
				this.nodes.map((node) => {
					if (this.checkAll) {
						if (!node.selected && !this.disableDept(node)) {
							node.selected = true;
							this.select.push(node);
						}
					} else {
						node.selected = false;
						for (let i = 0; i < this.select.length; i++) {
							if (this.select[i].id === node.id) {
								this.select.splice(i, 1);
								break;
							}
						}
					}
				});
				this.$forceUpdate()
			},
			show(selected, t) {

				this.getOrgList();
			},
			init(selected) {

			},
			disableDept(node) {
				return this.type === "user" && "dept" === node.type;
			},
			nextNode(node) {
				//进入下一级
				if (this.type == 'user' && node.type == 'dept') {
					this.nowDeptId = node.id;
					this.deptStack.push(node);
					this.getOrgList('next');
				} else if (this.type == 'dept' && node.type == 'dept') {
					this.nowDeptId = node.id;
					this.deptStack.push(node);
					this.getOrgList('next');
				} else {
					this.selectChange(node)
				}
			},
			selectChange(node) {
				if (node.selected) {
					this.checkAll = false;
					for (let i = 0; i < this.select.length; i++) {
						if (this.select[i].id === node.id) {
							this.select.splice(i, 1);
							break;
						}
					}
					node.selected = false;
				} else if (!this.disableDept(node)) {
					node.selected = true;
					let nodes = this.search.trim() === "" ? this.nodes : this.searchUsers;
					if (!this.multiple) {
						nodes.map((nd) => {
							if (node.id !== nd.id) {
								nd.selected = false;
							}
						});
					}
					if (node.type === "dept") {
						if (!this.multiple) {
							this.select = [node];
							this.$nextTick(() => {
								let selsee = JSON.parse(JSON.stringify(this.select))
								setPagesParam('selectDept', selsee, 1)

							})
						} else {
							this.select.unshift(node);
						}
					} else {
						if (!this.multiple) {
							this.select = [node];
							this.$nextTick(() => {
								let selsee = JSON.parse(JSON.stringify(this.select))
								setPagesParam('selectEmplee', selsee, 1)
							})
						} else {
							this.select.push(node);
						}
					}
				}
				this.getCheckVal()
			},
			finishMulSelect() {
				//完成多选
				if (this.type == 'user') {
					setPagesParam('selectEmplee', this.select, 1)
				} else if (this.type == 'dept') {
					setPagesParam('selectDept', this.select, 1)
				}
			},
			getOrgList() {
				this.$refs.promptMsg.loadingOpen()
				getOrgTree({
						deptId: this.nowDeptId,
						type: this.type
					})
					.then((rsp) => {
						this.$refs.promptMsg.loadingColse()
						this.nodes = rsp.data;
						this.selectToLeft()
					})
					.catch((err) => {
						this.$refs.promptMsg.loadingColse()
						this.setMsgTop(err)
					});
			},
			selectToLeft() {
				let nodes = this.search.trim() === "" ? this.nodes : this.searchUsers;
				nodes.map((node) => {
					for (let i = 0; i < this.select.length; i++) {
						if (this.select[i].id === node.id) {
							node.selected = true;
							break;
						} else {
							node.selected = false;
						}
					}
				});
				this.getCheckVal()
			},
			getCheckVal() {
				//设置全选框的值
				if (this.type == 'user') { //员工是否全选
					let noselect = this.orgs.find(row => row.selected === false && row.type === 'user')
					let user = this.orgs.find(row => row.type === 'user')
					if (noselect === undefined && user !== undefined) {
						this.checkAll = true
					} else {
						this.checkAll = false
					}
				} else if (this.type == 'dept') { //部门是否全选
					let noselect = this.orgs.find(row => row.selected === false && row.type === 'dept')
					let dept = this.orgs.find(row => row.type === 'dept')
					if (noselect === undefined && dept !== undefined) {
						this.checkAll = true
					} else {
						this.checkAll = false
					}
				}
			},
			getShortName(name) {
				if (name) {
					return name.length > 2 ? name.substring(1, 3) : name;
				}
				return "**";
			},
			searchUser(val) {
				this.search = val
				let userName = this.search.trim();
				this.searchUsers = [];
				this.loading = true;
				getUserByName({
						realName: userName
					})
					.then((rsp) => {
						this.loading = false;
						this.searchUsers = rsp.data;
						this.selectToLeft();
					})
					.catch((err) => {
						this.loading = false;
						this.$message.error("接口异常");
					});
			},
			beforeNode() {
				if (this.deptStack.length === 0) {
					return;
				}
				if (this.deptStack.length < 2) {
					this.nowDeptId = null;
				} else {
					this.nowDeptId = this.deptStack[this.deptStack.length - 2].id;
				}
				this.deptStack.splice(this.deptStack.length - 1, 1);
				this.getOrgList();
			},
			getEmployeList() {
				//获取员工列表

			},
		}
	}
</script>

<style lang="less" scoped>
	.employee_con {
		min-height: calc(100vh - 176rpx);
		background-color: #fff;

		.alselect_con_zhanwei {
			width: 100%;
			height: 98rpx;
		}

		.alselect_con_con {
			position: fixed;
			bottom: 0;
			left: 0;
			width: 100%;
			height: 98rpx;
			background-color: rgba(255, 255, 255, 1);
			box-shadow: 0rpx -6rpx 14rpx 0rpx rgba(0,0,0,0.03);

			.submit_button {
				border-radius: 0;
			}
		}

		.alselect_con {
			position: relative;
			// width: 100%;
			height: 98rpx;
			// background: rgba(28, 34, 50, 1);
			padding: 20rpx;
			box-sizing: border-box;
			display: flex;
			justify-content: space-between;
			align-items: center;

			.alselect_ul {
				white-space: nowrap;
				width: 100%;
				.alselect_ul_con{
					vertical-align: top;
				}
				.select_li {
					display: inline-flex;
					margin-right: 10rpx;
					width: 60rpx;
					height: 60rpx;

					.avatar_image {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						background: rgba(28, 34, 50, 1);
					}
				}

				.btn_zhanwei {
					display: inline-block;
					width: 236rpx;
					height: 68rpx;
				}
			}

			.com_btn_con {
				position: absolute;
				right: 0;
				top: center;
				// width: 100%;
				padding: 15rpx 22rpx 15rpx 22rpx;
				background: rgba(255, 255, 255, 1);
			}

			.com_btn {

				display: flex;
				align-items: center;
				justify-content: center;
				font-size: 28rpx;
				font-weight: 500;
				color: #fff;
				padding: 0 42rpx;
				height: 68rpx;
				background: rgba(35, 113, 255, 1);
				border-radius: 10px;
			}
		}

		.employee_ul {
			.employee_li {
				display: flex;
				justify-content: space-between;
				align-items: center;
				color: #333;
				min-height: 100rpx;
				padding: 20rpx;
				width: 100%;
				box-sizing: border-box;

				.li_left {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.ava_image {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						margin-right: 20rpx;
						background: rgba(28, 34, 50, 1);
					}

					.icons_con {
						min-width: 32rpx;
						height: 32rpx;
						margin-right: 20rpx;
						display: flex;
						justify-content: center;
						align-items: center;
					}

					.bumenicons_con {
						min-width: 28rpx;
						height: 28rpx;
						margin-right: 16rpx;
						display: flex;
						justify-content: center;
						align-items: center;
					}

					.li_text {
						max-width: calc(100% - 32rpx -20rpx -28rpx -16rpx - 20rpx);
					}
				}
				.li_right{
					font-size: 20rpx;
					color: #999999;
					max-width: 200rpx;
					overflow: hidden; /* 隐藏溢出的内容 */
					white-space: nowrap; /* 确保文本不会换行 */
					text-overflow: ellipsis; /* 可选：当文本被裁剪时显示省略号 */
				}
			}
		}
	}
</style>